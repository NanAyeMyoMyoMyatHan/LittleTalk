using Microsoft.EntityFrameworkCore;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Dashboard;

namespace LittleTalk.Application.Features.Dashboard.Services;

public class DashboardService : IDashboardService
{
    private readonly LittleTalkDbContext _context;

    public DashboardService(LittleTalkDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TeacherDashboardResponseModel>> GetTeacherDashboardAsync(int teacherUserId, CancellationToken cancellationToken = default)
    {
        var assignedCourseIds = await _context.CourseTeachers
            .Where(ct => ct.TeacherId == teacherUserId && ct.IsActive)
            .Select(ct => ct.CourseId)
            .ToListAsync(cancellationToken);

        var totalStudents = await _context.Enrollments
            .Where(e => assignedCourseIds.Contains(e.CourseId) && e.Status == "Enrolled")
            .Select(e => e.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalPlans = await _context.LessonPlans
            .CountAsync(lp => lp.CreatedBy == teacherUserId && lp.Status != "Archived", cancellationToken);

        var totalAssessments = await _context.Assessments
            .CountAsync(a => a.EvaluatorTeacherId == teacherUserId, cancellationToken);

        var attendances = await _context.Attendances
            .Include(a => a.Session)
            .Where(a => assignedCourseIds.Contains(a.Session.CourseId))
            .ToListAsync(cancellationToken);

        var totalAtt = attendances.Count;
        var presentCount = attendances.Count(a => a.Status is "Present" or "Late");
        var attendancePct = totalAtt > 0 ? Math.Round(((decimal)presentCount / totalAtt) * 100, 1) : 0;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var upcoming = await _context.ClassSessions
            .AsNoTracking()
            .Include(cs => cs.Course)
                .ThenInclude(c => c.Enrollments)
            .Include(cs => cs.LessonPlan)
            .Include(cs => cs.Teacher)
            .Where(cs => assignedCourseIds.Contains(cs.CourseId) && cs.SessionDate >= today && cs.Status != "Cancelled")
            .OrderBy(cs => cs.SessionDate)
            .ThenBy(cs => cs.StartTime)
            .Take(5)
            .Select(cs => new UpcomingSessionResponseModel
            {
                SessionId = cs.SessionId,
                CourseName = cs.Course.CourseName,
                LessonTitle = cs.LessonPlan != null ? cs.LessonPlan.Title : null,
                TeacherName = cs.Teacher.FullName,
                SessionDate = cs.SessionDate,
                StartTime = cs.StartTime,
                EndTime = cs.EndTime,
                RoomOrLink = cs.RoomOrLink,
                EnrolledStudentsCount = cs.Course.Enrollments.Count(e => e.Status == "Enrolled")
            })
            .ToListAsync(cancellationToken);

        var recent = await _context.Assessments
            .AsNoTracking()
            .Include(a => a.Student)
            .Include(a => a.Session)
                .ThenInclude(s => s!.Course)
            .Include(a => a.EvaluatorTeacher)
            .Where(a => a.EvaluatorTeacherId == teacherUserId && a.OverallScore.HasValue)
            .OrderByDescending(a => a.AssessmentDate)
            .Take(5)
            .Select(a => new RecentActivityItemResponseModel
            {
                AssessmentId = a.AssessmentId,
                StudentName = a.Student.FullName,
                CourseName = a.Session != null ? a.Session.Course.CourseName : "Practice Lesson",
                OverallScore = a.OverallScore!.Value,
                Date = a.AssessmentDate,
                EvaluatorTeacherName = a.EvaluatorTeacher.FullName
            })
            .ToListAsync(cancellationToken);

        var data = new TeacherDashboardResponseModel
        {
            TotalAssignedCourses = assignedCourseIds.Count,
            TotalAssignedStudents = totalStudents,
            TotalLessonPlansCreated = totalPlans,
            TotalAssessmentsCompleted = totalAssessments,
            OverallCourseAttendanceRate = attendancePct,
            UpcomingSessions = upcoming,
            RecentAssessments = recent
        };

        return Result<TeacherDashboardResponseModel>.Success(data, "Teacher dashboard stats retrieved successfully.");
    }

    public async Task<Result<AdminDashboardResponseModel>> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var totalStudents = await _context.Students.CountAsync(s => s.IsActive, cancellationToken);
        var totalTeachers = await _context.Users.CountAsync(u => u.Role.RoleName == "Teacher" && u.IsActive, cancellationToken);
        var totalActiveCourses = await _context.Courses.CountAsync(c => c.Status == "Active", cancellationToken);
        var totalLessonPlans = await _context.LessonPlans.CountAsync(lp => lp.Status != "Archived", cancellationToken);
        var totalSessions = await _context.ClassSessions.CountAsync(cancellationToken);
        var totalAssessments = await _context.Assessments.CountAsync(cancellationToken);

        var allAtt = await _context.Attendances.ToListAsync(cancellationToken);
        var presentAtt = allAtt.Count(a => a.Status is "Present" or "Late");
        var attRate = allAtt.Count > 0 ? Math.Round(((decimal)presentAtt / allAtt.Count) * 100, 1) : 0;

        var scoredAssessments = await _context.Assessments.Where(a => a.OverallScore.HasValue).ToListAsync(cancellationToken);
        var avgScore = scoredAssessments.Count > 0 ? Math.Round(scoredAssessments.Average(a => a.OverallScore!.Value), 2) : 0;

        var levelDistribution = await _context.LearningLevels
            .Where(l => l.IsActive)
            .OrderBy(l => l.SortOrder)
            .Select(l => new LevelEnrollmentCountResponseModel
            {
                LevelName = l.LevelName,
                StudentsCount = l.Students.Count(s => s.IsActive)
            })
            .ToListAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var upcoming = await _context.ClassSessions
            .AsNoTracking()
            .Include(cs => cs.Course)
                .ThenInclude(c => c.Enrollments)
            .Include(cs => cs.LessonPlan)
            .Include(cs => cs.Teacher)
            .Where(cs => cs.SessionDate >= today && cs.Status != "Cancelled")
            .OrderBy(cs => cs.SessionDate)
            .ThenBy(cs => cs.StartTime)
            .Take(5)
            .Select(cs => new UpcomingSessionResponseModel
            {
                SessionId = cs.SessionId,
                CourseName = cs.Course.CourseName,
                LessonTitle = cs.LessonPlan != null ? cs.LessonPlan.Title : null,
                TeacherName = cs.Teacher.FullName,
                SessionDate = cs.SessionDate,
                StartTime = cs.StartTime,
                EndTime = cs.EndTime,
                RoomOrLink = cs.RoomOrLink,
                EnrolledStudentsCount = cs.Course.Enrollments.Count(e => e.Status == "Enrolled")
            })
            .ToListAsync(cancellationToken);

        var data = new AdminDashboardResponseModel
        {
            TotalStudents = totalStudents,
            TotalTeachers = totalTeachers,
            TotalActiveCourses = totalActiveCourses,
            TotalLessonPlans = totalLessonPlans,
            TotalClassSessions = totalSessions,
            TotalAssessments = totalAssessments,
            SystemAttendanceRate = attRate,
            SystemAverageSpeakingScore = avgScore,
            LevelDistribution = levelDistribution,
            UpcomingSessions = upcoming
        };

        return Result<AdminDashboardResponseModel>.Success(data, "Admin dashboard stats retrieved successfully.");
    }
}
