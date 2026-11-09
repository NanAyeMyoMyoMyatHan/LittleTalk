namespace LittleTalk.Application.Features.Dashboard.DTOs;

public class TeacherDashboardDto
{
    public int TotalAssignedCourses { get; set; }
    public int TotalAssignedStudents { get; set; }
    public int TotalLessonPlansCreated { get; set; }
    public int TotalAssessmentsCompleted { get; set; }
    public decimal OverallCourseAttendanceRate { get; set; }
    public List<UpcomingSessionDto> UpcomingSessions { get; set; } = new();
    public List<RecentActivityItemDto> RecentAssessments { get; set; } = new();
}

public class AdminDashboardDto
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalActiveCourses { get; set; }
    public int TotalLessonPlans { get; set; }
    public int TotalClassSessions { get; set; }
    public int TotalAssessments { get; set; }
    public decimal SystemAttendanceRate { get; set; }
    public decimal SystemAverageSpeakingScore { get; set; }
    public List<LevelEnrollmentCountDto> LevelDistribution { get; set; } = new();
    public List<UpcomingSessionDto> UpcomingSessions { get; set; } = new();
}

public class UpcomingSessionDto
{
    public int SessionId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? LessonTitle { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateOnly SessionDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? RoomOrLink { get; set; }
    public int EnrolledStudentsCount { get; set; }
}

public class RecentActivityItemDto
{
    public int AssessmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public decimal OverallScore { get; set; }
    public DateTime Date { get; set; }
    public string EvaluatorTeacherName { get; set; } = string.Empty;
}

public class LevelEnrollmentCountDto
{
    public string LevelName { get; set; } = string.Empty;
    public int StudentsCount { get; set; }
}
