namespace LittleTalk.Model.Features.Dashboard;

public class TeacherDashboardResponseModel
{
    public int TotalAssignedCourses { get; set; }
    public int TotalAssignedStudents { get; set; }
    public int TotalLessonPlansCreated { get; set; }
    public int TotalAssessmentsCompleted { get; set; }
    public decimal OverallCourseAttendanceRate { get; set; }
    public List<UpcomingSessionResponseModel> UpcomingSessions { get; set; } = new();
    public List<RecentActivityItemResponseModel> RecentAssessments { get; set; } = new();
}

public class AdminDashboardResponseModel
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalActiveCourses { get; set; }
    public int TotalLessonPlans { get; set; }
    public int TotalClassSessions { get; set; }
    public int TotalAssessments { get; set; }
    public decimal SystemAttendanceRate { get; set; }
    public decimal SystemAverageSpeakingScore { get; set; }
    public List<LevelEnrollmentCountResponseModel> LevelDistribution { get; set; } = new();
    public List<UpcomingSessionResponseModel> UpcomingSessions { get; set; } = new();
}

public class UpcomingSessionResponseModel
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

public class RecentActivityItemResponseModel
{
    public int AssessmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public decimal OverallScore { get; set; }
    public DateTime Date { get; set; }
    public string EvaluatorTeacherName { get; set; } = string.Empty;
}

public class LevelEnrollmentCountResponseModel
{
    public string LevelName { get; set; } = string.Empty;
    public int StudentsCount { get; set; }
}
