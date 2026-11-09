namespace LittleTalk.Application.Features.Assessments.DTOs;

public class AssessmentSummaryDto
{
    public int AssessmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentFullName { get; set; } = string.Empty;
    public string? StudentNickName { get; set; }
    public int? SessionId { get; set; }
    public string? CourseName { get; set; }
    public int? LessonPlanId { get; set; }
    public string? LessonTitle { get; set; }
    public int EvaluatorTeacherId { get; set; }
    public string EvaluatorTeacherName { get; set; } = string.Empty;
    public DateTime AssessmentDate { get; set; }
    public string Status { get; set; } = string.Empty; // Draft, Finalized
    public decimal? OverallScore { get; set; }
    public int CriteriaCount { get; set; }
    public string? GeneralFeedback { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class AssessmentDetailDto : AssessmentSummaryDto
{
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<CriterionScoreDto> Details { get; set; } = new();
}

public class CriterionScoreDto
{
    public int AssessmentDetailId { get; set; }
    public string CriterionName { get; set; } = string.Empty;
    public int Score { get; set; } // 1 - 5
    public string? Notes { get; set; }
}

public class CreateAssessmentDto
{
    public int StudentId { get; set; }
    public int? SessionId { get; set; }
    public int? LessonPlanId { get; set; }
    public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Draft"; // Draft, Finalized
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? GeneralFeedback { get; set; }
    public List<CriterionInputDto> Criteria { get; set; } = new();
}

public class UpdateAssessmentDto
{
    public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Draft";
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? GeneralFeedback { get; set; }
    public List<CriterionInputDto> Criteria { get; set; } = new();
}

public class CriterionInputDto
{
    public string CriterionName { get; set; } = string.Empty;
    public int Score { get; set; } // 1 - 5
    public string? Notes { get; set; }
}
