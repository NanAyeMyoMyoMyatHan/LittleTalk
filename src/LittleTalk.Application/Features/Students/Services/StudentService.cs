using Microsoft.EntityFrameworkCore;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Students;

namespace LittleTalk.Application.Features.Students.Services;

public sealed class StudentService : IStudentService
{
    private readonly LittleTalkDbContext _context;
    private readonly StudentValidationPolicy _validationPolicy;

    public StudentService(
        LittleTalkDbContext context,
        StudentValidationPolicy validationPolicy)
    {
        _context = context;
        _validationPolicy = validationPolicy;
    }

    public async Task<Result<PagedListResponseModel<StudentSummaryResponseModel>>> GetStudentsAsync(
        SearchStudentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        _validationPolicy.ValidateSearch(request);

        var query = _context.Students
            .AsNoTracking()
            .Include(s => s.LearningLevel)
            .Include(s => s.Enrollments)
            .AsQueryable();

        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        if (request.LearningLevelId.HasValue && request.LearningLevelId.Value > 0)
        {
            query = query.Where(s => s.LearningLevelId == request.LearningLevelId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.FullName.ToLower().Contains(search) ||
                (s.NickName != null && s.NickName.ToLower().Contains(search)) ||
                s.StudentCode.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy switch
        {
            "fullname" => request.SortDescending ? query.OrderByDescending(s => s.FullName) : query.OrderBy(s => s.FullName),
            "studentcode" => request.SortDescending ? query.OrderByDescending(s => s.StudentCode) : query.OrderBy(s => s.StudentCode),
            "dateofbirth" => request.SortDescending ? query.OrderByDescending(s => s.DateOfBirth) : query.OrderBy(s => s.DateOfBirth),
            "level" => request.SortDescending ? query.OrderByDescending(s => s.LearningLevel.SortOrder) : query.OrderBy(s => s.LearningLevel.SortOrder),
            _ => query.OrderByDescending(s => s.CreatedDate)
        };

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StudentSummaryResponseModel
            {
                StudentId = s.StudentId,
                StudentCode = s.StudentCode,
                FullName = s.FullName,
                NickName = s.NickName,
                DateOfBirth = s.DateOfBirth,
                Age = today.Year - s.DateOfBirth.Year - (today < s.DateOfBirth.AddYears(today.Year - s.DateOfBirth.Year) ? 1 : 0),
                Gender = s.Gender,
                LearningLevelId = s.LearningLevelId,
                LearningLevelName = s.LearningLevel.LevelName,
                ProfileImageUrl = s.ProfileImageUrl,
                IsActive = s.IsActive,
                ActiveEnrollmentsCount = s.Enrollments.Count(e => e.Status == "Enrolled"),
                CreatedDate = s.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedList = new PagedListResponseModel<StudentSummaryResponseModel>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedListResponseModel<StudentSummaryResponseModel>>.Success(pagedList, "Students retrieved successfully.");
    }

    public async Task<Result<StudentDetailResponseModel>> GetStudentByIdAsync(
        int studentId, 
        CancellationToken cancellationToken = default)
    {
        if (studentId <= 0)
        {
            return Result<StudentDetailResponseModel>.ValidationError("Valid Student ID is required.");
        }

        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.LearningLevel)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);

        if (student == null)
        {
            return Result<StudentDetailResponseModel>.NotFound($"Student with ID ({studentId}) was not found.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var response = new StudentDetailResponseModel
        {
            StudentId = student.StudentId,
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            NickName = student.NickName,
            DateOfBirth = student.DateOfBirth,
            Age = today.Year - student.DateOfBirth.Year - (today < student.DateOfBirth.AddYears(today.Year - student.DateOfBirth.Year) ? 1 : 0),
            Gender = student.Gender,
            LearningLevelId = student.LearningLevelId,
            LearningLevelName = student.LearningLevel.LevelName,
            ProfileImageUrl = student.ProfileImageUrl,
            ParentName = student.ParentName,
            ParentContact = student.ParentContact,
            Notes = student.EmergencyNotes,
            IsActive = student.IsActive,
            CreatedDate = student.CreatedDate,
            UpdatedDate = student.UpdatedDate,
            Enrollments = student.Enrollments.Select(e => new StudentEnrollmentSummaryModel
            {
                EnrollmentId = e.EnrollmentId,
                CourseId = e.CourseId,
                CourseName = e.Course.CourseName,
                Status = e.Status
            }).ToList()
        };

        return Result<StudentDetailResponseModel>.Success(response, "Student details retrieved successfully.");
    }

    public async Task<Result<StudentDetailResponseModel>> CreateStudentAsync(
        CreateStudentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        // 1. Validate request using StudentValidationPolicy
        var validationResult = _validationPolicy.ValidateCreate(request);
        if (!validationResult.IsSuccess)
        {
            return Result<StudentDetailResponseModel>.ValidationError(validationResult.Errors, validationResult.Message);
        }

        // 2. Validate foreign key
        var levelExists = await _context.LearningLevels
            .AnyAsync(l => l.LearningLevelId == request.LearningLevelId, cancellationToken);
        if (!levelExists)
        {
            return Result<StudentDetailResponseModel>.NotFound($"Learning level with ID ({request.LearningLevelId}) does not exist.");
        }

        // 3. Duplicate check (FullName + DateOfBirth)
        var isDuplicate = await _context.Students
            .AnyAsync(s => s.FullName.ToLower() == request.FullName.ToLower() && 
                           s.DateOfBirth == request.DateOfBirth && 
                           s.IsActive, cancellationToken);
        if (isDuplicate)
        {
            return Result<StudentDetailResponseModel>.Duplicate($"An active student named '{request.FullName}' with date of birth '{request.DateOfBirth:yyyy-MM-dd}' already exists.");
        }

        // 4. Generate unique student code (STU-000001 format)
        var maxId = await _context.Students.MaxAsync(s => (int?)s.StudentId, cancellationToken) ?? 0;
        var nextCode = $"STU-{(maxId + 1):D6}";

        while (await _context.Students.AnyAsync(s => s.StudentCode == nextCode, cancellationToken))
        {
            maxId++;
            nextCode = $"STU-{(maxId + 1):D6}";
        }

        // 5. Save using EF Core
        var student = new Student
        {
            StudentCode = nextCode,
            FullName = request.FullName,
            NickName = request.NickName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            LearningLevelId = request.LearningLevelId,
            ProfileImageUrl = request.ProfileImageUrl,
            ParentName = request.ParentName,
            ParentContact = request.ParentContact,
            EmergencyNotes = request.Notes,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Return Result<T>
        return await GetStudentByIdAsync(student.StudentId, cancellationToken);
    }

    public async Task<Result<StudentDetailResponseModel>> UpdateStudentAsync(
        int studentId, 
        UpdateStudentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        // 1. Validate request using StudentValidationPolicy
        var validationResult = _validationPolicy.ValidateUpdate(studentId, request);
        if (!validationResult.IsSuccess)
        {
            return Result<StudentDetailResponseModel>.ValidationError(validationResult.Errors, validationResult.Message);
        }

        // 2. Lookup existing student
        var student = await _context.Students.FindAsync(new object[] { studentId }, cancellationToken);
        if (student == null)
        {
            return Result<StudentDetailResponseModel>.NotFound($"Student with ID ({studentId}) was not found.");
        }

        // 3. Validate foreign key
        var levelExists = await _context.LearningLevels
            .AnyAsync(l => l.LearningLevelId == request.LearningLevelId, cancellationToken);
        if (!levelExists)
        {
            return Result<StudentDetailResponseModel>.NotFound($"Learning level with ID ({request.LearningLevelId}) does not exist.");
        }

        // 4. Check duplicate against other students
        var isDuplicate = await _context.Students
            .AnyAsync(s => s.StudentId != studentId && 
                           s.FullName.ToLower() == request.FullName.ToLower() && 
                           s.DateOfBirth == request.DateOfBirth && 
                           s.IsActive, cancellationToken);
        if (isDuplicate)
        {
            return Result<StudentDetailResponseModel>.Duplicate($"Another active student named '{request.FullName}' with date of birth '{request.DateOfBirth:yyyy-MM-dd}' already exists.");
        }

        // 5. Update properties
        student.FullName = request.FullName;
        student.NickName = request.NickName;
        student.DateOfBirth = request.DateOfBirth;
        student.Gender = request.Gender;
        student.LearningLevelId = request.LearningLevelId;
        student.ProfileImageUrl = request.ProfileImageUrl;
        student.ParentName = request.ParentName;
        student.ParentContact = request.ParentContact;
        student.EmergencyNotes = request.Notes;
        student.IsActive = request.IsActive;
        student.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetStudentByIdAsync(student.StudentId, cancellationToken);
    }

    public async Task<Result> DeleteStudentAsync(
        int studentId, 
        CancellationToken cancellationToken = default)
    {
        if (studentId <= 0)
        {
            return Result.ValidationError("Valid Student ID is required.");
        }

        var student = await _context.Students.FindAsync(new object[] { studentId }, cancellationToken);
        if (student == null)
        {
            return Result.NotFound($"Student with ID ({studentId}) was not found.");
        }

        // Soft delete to protect academic and assessment history
        student.IsActive = false;
        student.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success($"Student '{student.FullName}' deactivated successfully.");
    }

    public async Task<Result<StudentLearningHistoryResponseModel>> GetStudentLearningHistoryAsync(
        int studentId, 
        CancellationToken cancellationToken = default)
    {
        if (studentId <= 0)
        {
            return Result<StudentLearningHistoryResponseModel>.ValidationError("Valid Student ID is required.");
        }

        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);

        if (student == null)
        {
            return Result<StudentLearningHistoryResponseModel>.NotFound($"Student with ID ({studentId}) was not found.");
        }

        var enrollments = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrollmentDate)
            .Select(e => new StudentEnrollmentHistoryItemModel
            {
                EnrollmentId = e.EnrollmentId,
                CourseId = e.CourseId,
                CourseName = e.Course.CourseName,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status
            })
            .ToListAsync(cancellationToken);

        var assessments = await _context.Assessments
            .AsNoTracking()
            .Include(a => a.Session)
                .ThenInclude(s => s!.Course)
            .Include(a => a.EvaluatorTeacher)
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.AssessmentDate)
            .Select(a => new StudentAssessmentHistoryItemModel
            {
                AssessmentId = a.AssessmentId,
                AssessmentDate = a.AssessmentDate,
                CourseName = a.Session != null ? a.Session.Course.CourseName : "Direct Evaluation",
                OverallScore = a.OverallScore,
                Status = a.Status,
                EvaluatorTeacherName = a.EvaluatorTeacher.FullName
            })
            .ToListAsync(cancellationToken);

        var response = new StudentLearningHistoryResponseModel
        {
            StudentId = student.StudentId,
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            Enrollments = enrollments,
            Assessments = assessments
        };

        return Result<StudentLearningHistoryResponseModel>.Success(response, "Learning history retrieved successfully.");
    }
}
