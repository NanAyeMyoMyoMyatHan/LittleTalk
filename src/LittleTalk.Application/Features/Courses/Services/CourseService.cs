using Microsoft.EntityFrameworkCore;
using LittleTalk.Application.Features.Courses;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Courses;

namespace LittleTalk.Application.Features.Courses.Services;

public sealed class CourseService : ICourseService
{
    private readonly LittleTalkDbContext _context;
    private readonly CourseValidationPolicy _validationPolicy;

    public CourseService(
        LittleTalkDbContext context,
        CourseValidationPolicy validationPolicy)
    {
        _context = context;
        _validationPolicy = validationPolicy;
    }

    public async Task<Result<PagedListResponseModel<CourseSummaryResponseModel>>> GetCoursesAsync(
        SearchCourseRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Courses
            .AsNoTracking()
            .Include(c => c.LearningLevel)
            .Include(c => c.CourseTeachers)
                .ThenInclude(ct => ct.Teacher)
            .Include(c => c.Enrollments)
            .AsQueryable();

        var levelId = request.LearningLevelId ?? request.LevelId;
        if (levelId.HasValue && levelId.Value > 0)
        {
            query = query.Where(c => c.LearningLevelId == levelId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(c => c.Status.ToLower() == request.Status.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            query = query.Where(c => c.CourseName.ToLower().Contains(search) || c.CourseCode.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CourseSummaryResponseModel
            {
                CourseId = c.CourseId,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Description = c.Description,
                LearningLevelId = c.LearningLevelId,
                LearningLevelName = c.LearningLevel.LevelName,
                DurationWeeks = c.DurationWeeks,
                MaxStudents = c.MaxStudents,
                Status = c.Status,
                EnrolledStudentsCount = c.Enrollments.Count(e => e.Status == "Enrolled"),
                PrimaryTeacherName = c.CourseTeachers
                    .Where(ct => ct.IsActive && ct.RoleInCourse == "Primary")
                    .Select(ct => ct.Teacher.FullName)
                    .FirstOrDefault(),
                CreatedDate = c.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedList = new PagedListResponseModel<CourseSummaryResponseModel>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedListResponseModel<CourseSummaryResponseModel>>.Success(pagedList, "Courses retrieved successfully.");
    }

    public async Task<Result<CourseDetailResponseModel>> GetCourseByIdAsync(
        int courseId, 
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0)
        {
            return Result<CourseDetailResponseModel>.ValidationError("Valid Course ID is required.");
        }

        var course = await _context.Courses
            .AsNoTracking()
            .Include(c => c.LearningLevel)
            .Include(c => c.CourseTeachers)
                .ThenInclude(ct => ct.Teacher)
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.CourseId == courseId, cancellationToken);

        if (course == null)
        {
            return Result<CourseDetailResponseModel>.NotFound($"Course with ID ({courseId}) was not found.");
        }

        return Result<CourseDetailResponseModel>.Success(MapToDetailResponse(course), "Course details retrieved successfully.");
    }

    public async Task<Result<CourseDetailResponseModel>> CreateCourseAsync(
        CreateCourseRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        // 1. ValidationPolicy
        var validation = _validationPolicy.ValidateCreate(request);
        if (!validation.IsSuccess)
        {
            return Result<CourseDetailResponseModel>.ValidationError(validation.Errors, validation.Message);
        }

        // 2. Duplicate check
        var codeExists = await _context.Courses
            .AnyAsync(c => c.CourseCode.ToLower() == request.CourseCode.ToLower(), cancellationToken);
        if (codeExists)
        {
            return Result<CourseDetailResponseModel>.Duplicate($"Course code '{request.CourseCode}' is already registered.");
        }

        // 3. Foreign key check
        var levelExists = await _context.LearningLevels
            .AnyAsync(l => l.LearningLevelId == request.LearningLevelId && l.IsActive, cancellationToken);
        if (!levelExists)
        {
            return Result<CourseDetailResponseModel>.ValidationError("The selected learning level is invalid or inactive.");
        }

        var course = new Course
        {
            CourseCode = request.CourseCode.ToUpper(),
            CourseName = request.CourseName,
            Description = request.Description,
            LearningLevelId = request.LearningLevelId,
            DurationWeeks = request.DurationWeeks,
            MaxStudents = request.MaxStudents,
            Status = request.Status,
            CreatedDate = DateTime.UtcNow
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        // Assign initial primary teacher if specified
        if (request.PrimaryTeacherId.HasValue && request.PrimaryTeacherId.Value > 0)
        {
            await AssignTeacherAsync(course.CourseId, new AssignTeacherRequestModel
            {
                TeacherId = request.PrimaryTeacherId.Value,
                RoleInCourse = "Primary"
            }, cancellationToken);
        }

        return await GetCourseByIdAsync(course.CourseId, cancellationToken);
    }

    public async Task<Result<CourseDetailResponseModel>> UpdateCourseAsync(
        int courseId, 
        UpdateCourseRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0)
        {
            return Result<CourseDetailResponseModel>.ValidationError("Valid Course ID is required.");
        }

        var validation = _validationPolicy.ValidateUpdate(request);
        if (!validation.IsSuccess)
        {
            return Result<CourseDetailResponseModel>.ValidationError(validation.Errors, validation.Message);
        }

        var course = await _context.Courses.FindAsync(new object[] { courseId }, cancellationToken);
        if (course == null)
        {
            return Result<CourseDetailResponseModel>.NotFound($"Course with ID ({courseId}) was not found.");
        }

        var levelExists = await _context.LearningLevels
            .AnyAsync(l => l.LearningLevelId == request.LearningLevelId && l.IsActive, cancellationToken);
        if (!levelExists)
        {
            return Result<CourseDetailResponseModel>.ValidationError("The selected learning level is invalid or inactive.");
        }

        course.CourseName = request.CourseName;
        course.Description = request.Description;
        course.LearningLevelId = request.LearningLevelId;
        course.DurationWeeks = request.DurationWeeks;
        course.MaxStudents = request.MaxStudents;
        course.Status = request.Status;
        course.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetCourseByIdAsync(courseId, cancellationToken);
    }

    public async Task<Result> DeleteCourseAsync(
        int courseId, 
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0)
        {
            return Result.ValidationError("Valid Course ID is required.");
        }

        var course = await _context.Courses.FindAsync(new object[] { courseId }, cancellationToken);
        if (course == null)
        {
            return Result.NotFound($"Course with ID ({courseId}) was not found.");
        }

        course.Status = "Archived";
        course.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success($"Course '{course.CourseName}' archived successfully.");
    }

    public async Task<Result<CourseTeacherResponseModel>> AssignTeacherAsync(
        int courseId, 
        AssignTeacherRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        if (courseId <= 0)
            return Result<CourseTeacherResponseModel>.ValidationError("Valid Course ID is required.");

        var validation = _validationPolicy.ValidateAssignTeacher(request);
        if (!validation.IsSuccess)
            return Result<CourseTeacherResponseModel>.ValidationError(validation.Errors, validation.Message);

        var course = await _context.Courses.FindAsync(new object[] { courseId }, cancellationToken);
        if (course == null)
            return Result<CourseTeacherResponseModel>.NotFound($"Course with ID ({courseId}) was not found.");

        var teacher = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == request.TeacherId, cancellationToken);

        if (teacher == null)
            return Result<CourseTeacherResponseModel>.NotFound($"Teacher with User ID ({request.TeacherId}) was not found.");

        if (!teacher.IsActive)
            return Result<CourseTeacherResponseModel>.ValidationError("Cannot assign an inactive teacher.");

        if (teacher.Role.RoleName != "Teacher" && teacher.Role.RoleName != "Admin")
            return Result<CourseTeacherResponseModel>.ValidationError("User must have the Teacher or Admin role.");

        // If assigning as Primary, demote existing active primary to CoTeacher
        if (request.RoleInCourse == "Primary")
        {
            var existingPrimary = await _context.CourseTeachers
                .FirstOrDefaultAsync(ct => ct.CourseId == courseId && ct.RoleInCourse == "Primary" && ct.IsActive, cancellationToken);

            if (existingPrimary != null && existingPrimary.TeacherId != request.TeacherId)
            {
                existingPrimary.RoleInCourse = "CoTeacher";
            }
        }

        var existingAssignment = await _context.CourseTeachers
            .FirstOrDefaultAsync(ct => ct.CourseId == courseId && ct.TeacherId == request.TeacherId, cancellationToken);

        if (existingAssignment != null)
        {
            existingAssignment.RoleInCourse = request.RoleInCourse;
            existingAssignment.IsActive = true;
            existingAssignment.AssignedDate = DateTime.UtcNow;
        }
        else
        {
            existingAssignment = new CourseTeacher
            {
                CourseId = courseId,
                TeacherId = request.TeacherId,
                RoleInCourse = request.RoleInCourse,
                IsActive = true,
                AssignedDate = DateTime.UtcNow
            };
            _context.CourseTeachers.Add(existingAssignment);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var response = new CourseTeacherResponseModel
        {
            CourseTeacherId = existingAssignment.CourseTeacherId,
            TeacherId = teacher.UserId,
            TeacherName = teacher.FullName,
            TeacherEmail = teacher.Email,
            RoleInCourse = existingAssignment.RoleInCourse,
            IsActive = existingAssignment.IsActive,
            AssignedDate = existingAssignment.AssignedDate
        };

        return Result<CourseTeacherResponseModel>.Success(response, "Teacher assigned successfully.");
    }

    public async Task<Result> RemoveTeacherAsync(
        int courseId, 
        int teacherId, 
        CancellationToken cancellationToken = default)
    {
        var assignment = await _context.CourseTeachers
            .FirstOrDefaultAsync(ct => ct.CourseId == courseId && ct.TeacherId == teacherId && ct.IsActive, cancellationToken);

        if (assignment == null)
        {
            return Result.NotFound($"Active assignment for teacher {teacherId} in course {courseId} not found.");
        }

        assignment.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Teacher removed from course successfully.");
    }

    public async Task<Result<List<CourseEnrollmentResponseModel>>> GetCourseEnrollmentsAsync(
        int courseId, 
        CancellationToken cancellationToken = default)
    {
        var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == courseId, cancellationToken);
        if (!courseExists)
        {
            return Result<List<CourseEnrollmentResponseModel>>.NotFound($"Course with ID ({courseId}) was not found.");
        }

        var items = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Where(e => e.CourseId == courseId)
            .OrderByDescending(e => e.EnrollmentDate)
            .Select(e => new CourseEnrollmentResponseModel
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentCode = e.Student.StudentCode,
                StudentFullName = e.Student.FullName,
                StudentNickName = e.Student.NickName,
                Status = e.Status,
                EnrollmentDate = e.EnrollmentDate,
                CompletedDate = e.CompletedDate,
                Notes = e.Notes
            })
            .ToListAsync(cancellationToken);

        return Result<List<CourseEnrollmentResponseModel>>.Success(items, "Enrollments retrieved successfully.");
    }

    public async Task<Result<CourseEnrollmentResponseModel>> EnrollStudentAsync(
        int courseId, 
        EnrollStudentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateEnrollStudent(request);
        if (!validation.IsSuccess)
        {
            return Result<CourseEnrollmentResponseModel>.ValidationError(validation.Message);
        }

        var course = await _context.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.CourseId == courseId, cancellationToken);

        if (course == null)
            return Result<CourseEnrollmentResponseModel>.NotFound($"Course with ID ({courseId}) was not found.");

        if (course.Status is "Archived" or "Completed")
            return Result<CourseEnrollmentResponseModel>.ValidationError($"Cannot enroll students into a course with status '{course.Status}'.");

        var student = await _context.Students.FindAsync(new object[] { request.StudentId }, cancellationToken);
        if (student == null)
            return Result<CourseEnrollmentResponseModel>.NotFound($"Student with ID ({request.StudentId}) was not found.");

        if (!student.IsActive)
            return Result<CourseEnrollmentResponseModel>.ValidationError("Cannot enroll an inactive student.");

        var isAlreadyEnrolled = await _context.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.StudentId == request.StudentId && e.Status == "Enrolled", cancellationToken);

        if (isAlreadyEnrolled)
            return Result<CourseEnrollmentResponseModel>.Duplicate($"Student '{student.FullName}' is already actively enrolled in this course.");

        var activeCount = course.Enrollments.Count(e => e.Status == "Enrolled");
        if (activeCount >= course.MaxStudents)
            return Result<CourseEnrollmentResponseModel>.ValidationError($"Course is at full capacity ({course.MaxStudents} students max).");

        var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = request.StudentId,
            Status = "Enrolled",
            EnrollmentDate = DateTime.UtcNow,
            Notes = request.Notes,
            CreatedDate = DateTime.UtcNow
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new CourseEnrollmentResponseModel
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = student.StudentId,
            StudentCode = student.StudentCode,
            StudentFullName = student.FullName,
            StudentNickName = student.NickName,
            Status = enrollment.Status,
            EnrollmentDate = enrollment.EnrollmentDate,
            Notes = enrollment.Notes
        };

        return Result<CourseEnrollmentResponseModel>.Success(response, "Student enrolled successfully.");
    }

    public async Task<Result<CourseEnrollmentResponseModel>> UpdateEnrollmentStatusAsync(
        int courseId, 
        int studentId, 
        UpdateEnrollmentStatusRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var validStatuses = new[] { "Enrolled", "Completed", "Dropped", "Transferred" };
        if (!validStatuses.Contains(request.Status))
        {
            return Result<CourseEnrollmentResponseModel>.ValidationError($"Invalid enrollment status. Allowed values: {string.Join(", ", validStatuses)}");
        }

        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId, cancellationToken);

        if (enrollment == null)
        {
            return Result<CourseEnrollmentResponseModel>.NotFound($"Enrollment for student {studentId} in course {courseId} not found.");
        }

        enrollment.Status = request.Status;
        enrollment.Notes = request.Notes?.Trim() ?? enrollment.Notes;
        enrollment.UpdatedDate = DateTime.UtcNow;

        if (request.Status is "Completed" or "Dropped" or "Transferred")
        {
            enrollment.CompletedDate = DateTime.UtcNow;
        }
        else if (request.Status == "Enrolled")
        {
            enrollment.CompletedDate = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var response = new CourseEnrollmentResponseModel
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            StudentCode = enrollment.Student.StudentCode,
            StudentFullName = enrollment.Student.FullName,
            StudentNickName = enrollment.Student.NickName,
            Status = enrollment.Status,
            EnrollmentDate = enrollment.EnrollmentDate,
            CompletedDate = enrollment.CompletedDate,
            Notes = enrollment.Notes
        };

        return Result<CourseEnrollmentResponseModel>.Success(response, "Enrollment status updated successfully.");
    }

    private static CourseDetailResponseModel MapToDetailResponse(Course course)
    {
        return new CourseDetailResponseModel
        {
            CourseId = course.CourseId,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Description = course.Description,
            LearningLevelId = course.LearningLevelId,
            LearningLevelName = course.LearningLevel?.LevelName ?? string.Empty,
            DurationWeeks = course.DurationWeeks,
            MaxStudents = course.MaxStudents,
            Status = course.Status,
            CreatedDate = course.CreatedDate,
            UpdatedDate = course.UpdatedDate,
            EnrolledStudentsCount = course.Enrollments.Count(e => e.Status == "Enrolled"),
            PrimaryTeacherName = course.CourseTeachers
                .Where(ct => ct.IsActive && ct.RoleInCourse == "Primary")
                .Select(ct => ct.Teacher.FullName)
                .FirstOrDefault(),
            Teachers = course.CourseTeachers.Select(ct => new CourseTeacherResponseModel
            {
                CourseTeacherId = ct.CourseTeacherId,
                TeacherId = ct.TeacherId,
                TeacherName = ct.Teacher.FullName,
                TeacherEmail = ct.Teacher.Email,
                RoleInCourse = ct.RoleInCourse,
                IsActive = ct.IsActive,
                AssignedDate = ct.AssignedDate
            }).ToList(),
            EnrolledStudents = course.Enrollments.Select(e => new CourseEnrollmentResponseModel
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentCode = e.Student?.StudentCode ?? string.Empty,
                StudentFullName = e.Student?.FullName ?? string.Empty,
                StudentNickName = e.Student?.NickName,
                Status = e.Status,
                EnrollmentDate = e.EnrollmentDate,
                CompletedDate = e.CompletedDate,
                Notes = e.Notes
            }).ToList()
        };
    }
}
