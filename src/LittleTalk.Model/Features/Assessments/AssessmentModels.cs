using LittleTalk.Model.Common;

namespace LittleTalk.Model.Features.Assessments;

public class SearchAssessmentRequestModel : PaginationRequestModel
{
    public int? StudentId { get; set; }
    public int? SessionId { get; set; }
    public string? Status { get; set; }
}

public class CreateAssessmentRequestModel
{
    public int StudentId { get; set; }
    public int? SessionId { get; set; }
    public int? LessonPlanId { get; set; }
    public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Completed";
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? GeneralFeedback { get; set; }
    public List<CriterionScoreInputModel> Criteria { get; set; } = new();
}

public class UpdateAssessmentRequestModel
{
    public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Completed";
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? GeneralFeedback { get; set; }
    public List<CriterionScoreInputModel> Criteria { get; set; } = new();
}

public class CriterionScoreInputModel
{
    public string CriterionName { get; set; } = string.Empty;
    public int Score { get; set; } // 1 - 5
    public string? Notes { get; set; }
}

public class AssessmentSummaryResponseModel
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
    public string Status { get; set; } = string.Empty;
    public decimal? OverallScore { get; set; }
    public int CriteriaCount { get; set; }
    public string? GeneralFeedback { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class AssessmentDetailResponseModel : AssessmentSummaryResponseModel
{
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<CriterionScoreDetailResponseModel> Details { get; set; } = new();
}

public class CriterionScoreDetailResponseModel
{
    public int AssessmentDetailId { get; set; }
    public string CriterionName { get; set; } = string.Empty;
    public int Score { get; set; } // 1 - 5
    public string? Notes { get; set; }
}
