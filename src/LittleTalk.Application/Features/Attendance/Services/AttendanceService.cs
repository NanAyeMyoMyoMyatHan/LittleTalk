using Microsoft.EntityFrameworkCore;
using LittleTalk.Application.Features.Attendance;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Attendance;

namespace LittleTalk.Application.Features.Attendance.Services;

public sealed class AttendanceService : IAttendanceService
{
    private readonly LittleTalkDbContext _context;
    private readonly AttendanceValidationPolicy _validationPolicy;

    public AttendanceService(
        LittleTalkDbContext context,
        AttendanceValidationPolicy validationPolicy)
    {
        _context = context;
        _validationPolicy = validationPolicy;
    }

    public async Task<Result<PagedListResponseModel<ClassSessionResponseModel>>> GetSessionsAsync(
        SearchClassSessionRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.ClassSessions
            .AsNoTracking()
            .Include(cs => cs.Course)
                .ThenInclude(c => c.Enrollments)
            .Include(cs => cs.LessonPlan)
            .Include(cs => cs.Teacher)
            .Include(cs => cs.Attendances)
            .AsQueryable();

        if (request.CourseId.HasValue && request.CourseId.Value > 0)
        {
            query = query.Where(cs => cs.CourseId == request.CourseId.Value);
        }

        if (request.FromDate.HasValue) query = query.Where(cs => cs.SessionDate >= request.FromDate.Value);
        if (request.ToDate.HasValue) query = query.Where(cs => cs.SessionDate <= request.ToDate.Value);
        if (!string.IsNullOrWhiteSpace(request.Status)) query = query.Where(cs => cs.Status.ToLower() == request.Status.Trim().ToLower());

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            query = query.Where(cs => 
                cs.Course.CourseName.ToLower().Contains(search) || 
                (cs.LessonPlan != null && cs.LessonPlan.Title.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(cs => cs.SessionDate)
            .ThenByDescending(cs => cs.StartTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(cs => new ClassSessionResponseModel
            {
                SessionId = cs.SessionId,
                CourseId = cs.CourseId,
                CourseName = cs.Course.CourseName,
                CourseCode = cs.Course.CourseCode,
                LessonPlanId = cs.LessonPlanId,
                LessonTitle = cs.LessonPlan != null ? cs.LessonPlan.Title : null,
                LessonTopic = cs.LessonPlan != null ? cs.LessonPlan.Topic : null,
                TeacherId = cs.TeacherId,
                TeacherName = cs.Teacher.FullName,
                SessionDate = cs.SessionDate,
                StartTime = cs.StartTime,
                EndTime = cs.EndTime,
                RoomOrLink = cs.RoomOrLink,
                Status = cs.Status,
                Notes = cs.Notes,
                TotalEnrolledStudents = cs.Course.Enrollments.Count(e => e.Status == "Enrolled"),
                PresentCount = cs.Attendances.Count(a => a.Status == "Present"),
                AbsentCount = cs.Attendances.Count(a => a.Status == "Absent"),
                LateCount = cs.Attendances.Count(a => a.Status == "Late"),
                CreatedDate = cs.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedList = new PagedListResponseModel<ClassSessionResponseModel>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedListResponseModel<ClassSessionResponseModel>>.Success(pagedList, "Sessions retrieved successfully.");
    }

    public async Task<Result<ClassSessionDetailResponseModel>> GetSessionByIdAsync(
        int sessionId, 
        CancellationToken cancellationToken = default)
    {
        if (sessionId <= 0)
            return Result<ClassSessionDetailResponseModel>.ValidationError("Valid Session ID is required.");

        var session = await _context.ClassSessions
            .AsNoTracking()
            .Include(cs => cs.Course)
                .ThenInclude(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
            .Include(cs => cs.LessonPlan)
            .Include(cs => cs.Teacher)
            .Include(cs => cs.Attendances)
            .FirstOrDefaultAsync(cs => cs.SessionId == sessionId, cancellationToken);

        if (session == null)
            return Result<ClassSessionDetailResponseModel>.NotFound($"Class Session with ID ({sessionId}) was not found.");

        var activeEnrollments = session.Course.Enrollments
            .Where(e => e.Status == "Enrolled")
            .OrderBy(e => e.Student.FullName)
            .ToList();

        var roster = activeEnrollments.Select(e =>
        {
            var att = session.Attendances.FirstOrDefault(a => a.StudentId == e.StudentId);
            return new SessionRosterStudentResponseModel
            {
                StudentId = e.StudentId,
                StudentCode = e.Student.StudentCode,
                FullName = e.Student.FullName,
                NickName = e.Student.NickName,
                AttendanceId = att?.AttendanceId,
                AttendanceStatus = att?.Status,
                Remarks = att?.Remarks,
                RecordedDate = att?.RecordedDate
            };
        }).ToList();

        var response = new ClassSessionDetailResponseModel
        {
            SessionId = session.SessionId,
            CourseId = session.CourseId,
            CourseName = session.Course.CourseName,
            CourseCode = session.Course.CourseCode,
            LessonPlanId = session.LessonPlanId,
            LessonTitle = session.LessonPlan?.Title,
            LessonTopic = session.LessonPlan?.Topic,
            TeacherId = session.TeacherId,
            TeacherName = session.Teacher.FullName,
            SessionDate = session.SessionDate,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            RoomOrLink = session.RoomOrLink,
            Status = session.Status,
            Notes = session.Notes,
            TotalEnrolledStudents = activeEnrollments.Count,
            PresentCount = session.Attendances.Count(a => a.Status == "Present"),
            AbsentCount = session.Attendances.Count(a => a.Status == "Absent"),
            LateCount = session.Attendances.Count(a => a.Status == "Late"),
            CreatedDate = session.CreatedDate,
            Roster = roster
        };

        return Result<ClassSessionDetailResponseModel>.Success(response, "Session details retrieved successfully.");
    }

    public async Task<Result<ClassSessionDetailResponseModel>> CreateSessionAsync(
        CreateClassSessionRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateCreateSession(request);
        if (!validation.IsSuccess)
            return Result<ClassSessionDetailResponseModel>.ValidationError(validation.Errors, validation.Message);

        var course = await _context.Courses.FindAsync(new object[] { request.CourseId }, cancellationToken);
        if (course == null)
            return Result<ClassSessionDetailResponseModel>.NotFound($"Course with ID ({request.CourseId}) was not found.");

        var teacher = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == request.TeacherId, cancellationToken);

        if (teacher == null)
            return Result<ClassSessionDetailResponseModel>.NotFound($"Teacher User with ID ({request.TeacherId}) was not found.");

        if (!teacher.IsActive)
            return Result<ClassSessionDetailResponseModel>.ValidationError("Cannot assign an inactive teacher.");

        if (teacher.Role.RoleName != "Admin")
        {
            var isStaffed = await _context.CourseTeachers
                .AnyAsync(ct => ct.CourseId == request.CourseId && ct.TeacherId == request.TeacherId && ct.IsActive, cancellationToken);
            if (!isStaffed)
            {
                return Result<ClassSessionDetailResponseModel>.ValidationError($"Teacher '{teacher.FullName}' is not assigned to course '{course.CourseName}'.");
            }
        }

        if (request.LessonPlanId.HasValue && request.LessonPlanId.Value > 0)
        {
            var plan = await _context.LessonPlans.FindAsync(new object[] { request.LessonPlanId.Value }, cancellationToken);
            if (plan == null)
                return Result<ClassSessionDetailResponseModel>.NotFound($"Lesson Plan with ID ({request.LessonPlanId.Value}) was not found.");

            if (plan.CourseId.HasValue && plan.CourseId.Value != request.CourseId)
            {
                return Result<ClassSessionDetailResponseModel>.ValidationError("The selected lesson plan belongs to a different course.");
            }
        }

        var session = new ClassSession
        {
            CourseId = request.CourseId,
            LessonPlanId = request.LessonPlanId,
            TeacherId = request.TeacherId,
            SessionDate = request.SessionDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RoomOrLink = request.RoomOrLink,
            Status = "Scheduled",
            Notes = request.Notes,
            CreatedDate = DateTime.UtcNow
        };

        _context.ClassSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetSessionByIdAsync(session.SessionId, cancellationToken);
    }

    public async Task<Result<ClassSessionDetailResponseModel>> UpdateSessionAsync(
        int sessionId, 
        UpdateClassSessionRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        if (sessionId <= 0)
            return Result<ClassSessionDetailResponseModel>.ValidationError("Valid Session ID is required.");

        var validation = _validationPolicy.ValidateUpdateSession(request);
        if (!validation.IsSuccess)
            return Result<ClassSessionDetailResponseModel>.ValidationError(validation.Errors, validation.Message);

        var session = await _context.ClassSessions.FindAsync(new object[] { sessionId }, cancellationToken);
        if (session == null)
            return Result<ClassSessionDetailResponseModel>.NotFound($"Class Session with ID ({sessionId}) was not found.");

        var teacher = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == request.TeacherId, cancellationToken);

        if (teacher == null)
            return Result<ClassSessionDetailResponseModel>.NotFound($"Teacher User with ID ({request.TeacherId}) was not found.");

        if (!teacher.IsActive)
            return Result<ClassSessionDetailResponseModel>.ValidationError("Cannot assign an inactive teacher.");

        if (request.LessonPlanId.HasValue && request.LessonPlanId.Value > 0)
        {
            var plan = await _context.LessonPlans.FindAsync(new object[] { request.LessonPlanId.Value }, cancellationToken);
            if (plan == null)
                return Result<ClassSessionDetailResponseModel>.NotFound($"Lesson Plan with ID ({request.LessonPlanId.Value}) was not found.");

            if (plan.CourseId.HasValue && plan.CourseId.Value != session.CourseId)
            {
                return Result<ClassSessionDetailResponseModel>.ValidationError("The selected lesson plan belongs to a different course.");
            }
        }

        session.LessonPlanId = request.LessonPlanId;
        session.TeacherId = request.TeacherId;
        session.SessionDate = request.SessionDate;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;
        session.RoomOrLink = request.RoomOrLink;
        session.Status = request.Status;
        session.Notes = request.Notes;
        session.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetSessionByIdAsync(sessionId, cancellationToken);
    }

    public async Task<Result> DeleteSessionAsync(
        int sessionId, 
        CancellationToken cancellationToken = default)
    {
        if (sessionId <= 0)
            return Result.ValidationError("Valid Session ID is required.");

        var session = await _context.ClassSessions.FindAsync(new object[] { sessionId }, cancellationToken);
        if (session == null)
            return Result.NotFound($"Class Session with ID ({sessionId}) was not found.");

        session.Status = "Cancelled";
        session.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Class Session cancelled successfully.");
    }

    public async Task<Result<ClassSessionDetailResponseModel>> BatchRecordAttendanceAsync(
        int sessionId, 
        BatchRecordAttendanceRequestModel request, 
        int recordedByUserId, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateBatchAttendance(request);
        if (!validation.IsSuccess)
            return Result<ClassSessionDetailResponseModel>.ValidationError(validation.Errors, validation.Message);

        var session = await _context.ClassSessions
            .Include(cs => cs.Attendances)
            .FirstOrDefaultAsync(cs => cs.SessionId == sessionId, cancellationToken);

        if (session == null)
            return Result<ClassSessionDetailResponseModel>.NotFound($"Class Session with ID ({sessionId}) was not found.");

        if (session.Status == "Cancelled")
            return Result<ClassSessionDetailResponseModel>.ValidationError("Cannot record attendance for a cancelled session.");

        var studentIds = request.Records.Select(r => r.StudentId).Distinct().ToList();
        var enrolledStudentIds = await _context.Enrollments
            .Where(e => e.CourseId == session.CourseId && e.Status == "Enrolled" && studentIds.Contains(e.StudentId))
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

        var nonEnrolledStudents = studentIds.Except(enrolledStudentIds).ToList();
        if (nonEnrolledStudents.Count > 0)
        {
            return Result<ClassSessionDetailResponseModel>.ValidationError($"One or more students (IDs: {string.Join(", ", nonEnrolledStudents)}) are not actively enrolled in the course for this session.");
        }

        foreach (var record in request.Records)
        {
            var existing = session.Attendances.FirstOrDefault(a => a.StudentId == record.StudentId);
            if (existing != null)
            {
                existing.Status = record.Status;
                existing.Remarks = record.Remarks;
                existing.RecordedBy = recordedByUserId;
                existing.RecordedDate = DateTime.UtcNow;
            }
            else
            {
                var newAtt = new LittleTalk.Infrastructure.Data.Entities.Attendance
                {
                    SessionId = sessionId,
                    StudentId = record.StudentId,
                    Status = record.Status,
                    Remarks = record.Remarks,
                    RecordedBy = recordedByUserId,
                    RecordedDate = DateTime.UtcNow
                };
                _context.Attendances.Add(newAtt);
            }
        }

        if (session.Status == "Scheduled")
        {
            session.Status = "Completed";
            session.UpdatedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await GetSessionByIdAsync(sessionId, cancellationToken);
    }

    public async Task<Result<List<StudentAttendanceHistoryResponseModel>>> GetStudentAttendanceHistoryAsync(
        int studentId, 
        CancellationToken cancellationToken = default)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId, cancellationToken);
        if (!studentExists)
            return Result<List<StudentAttendanceHistoryResponseModel>>.NotFound($"Student with ID ({studentId}) was not found.");

        var items = await _context.Attendances
            .AsNoTracking()
            .Include(a => a.Session)
                .ThenInclude(s => s.Course)
            .Include(a => a.Session)
                .ThenInclude(s => s.LessonPlan)
            .Include(a => a.Session)
                .ThenInclude(s => s.Teacher)
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.Session.SessionDate)
            .Select(a => new StudentAttendanceHistoryResponseModel
            {
                AttendanceId = a.AttendanceId,
                SessionId = a.SessionId,
                SessionDate = a.Session.SessionDate,
                CourseName = a.Session.Course.CourseName,
                LessonTitle = a.Session.LessonPlan != null ? a.Session.LessonPlan.Title : null,
                TeacherName = a.Session.Teacher.FullName,
                Status = a.Status,
                Remarks = a.Remarks,
                RecordedDate = a.RecordedDate
            })
            .ToListAsync(cancellationToken);

        return Result<List<StudentAttendanceHistoryResponseModel>>.Success(items, "Attendance history retrieved successfully.");
    }
}
