namespace LittleTalk.Model.Features.Reports;

public class StudentProgressReportResponseModel
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public int Age { get; set; }
    public string CurrentLearningLevel { get; set; } = string.Empty;
    public DateTime ReportGeneratedDate { get; set; } = DateTime.UtcNow;

    // Attendance Summary
    public int TotalSessionsConducted { get; set; }
    public int SessionsPresent { get; set; }
    public int SessionsLate { get; set; }
    public int SessionsAbsent { get; set; }
    public decimal AttendancePercentage { get; set; }

    // Assessment Overview
    public int TotalAssessments { get; set; }
    public decimal OverallAverageScore { get; set; }

    // Criteria Breakdown (Radar/Bar Chart data)
    public List<CriterionAverageResponseModel> CriteriaBreakdown { get; set; } = new();

    // Progress Over Time (Line Chart data)
    public List<ScoreTimelinePointResponseModel> ScoreTimeline { get; set; } = new();

    // Qualitative Feedback Highlights
    public List<string> HighlightedStrengths { get; set; } = new();
    public List<string> HighlightedImprovementAreas { get; set; } = new();
    public List<RecentFeedbackItemResponseModel> RecentTeacherFeedback { get; set; } = new();
}

public class CriterionAverageResponseModel
{
    public string CriterionName { get; set; } = string.Empty;
    public decimal AverageScore { get; set; } // 1.00 - 5.00
    public int EvaluationsCount { get; set; }
}

public class ScoreTimelinePointResponseModel
{
    public DateTime Date { get; set; }
    public decimal OverallScore { get; set; }
    public string? LessonTitle { get; set; }
    public string EvaluatorTeacherName { get; set; } = string.Empty;
}

public class RecentFeedbackItemResponseModel
{
    public DateTime Date { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string? LessonTitle { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public decimal? OverallScore { get; set; }
}
