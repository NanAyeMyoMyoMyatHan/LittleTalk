namespace LittleTalk.Model.Features.Students;

public class StudentSummaryResponseModel
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int Age { get; set; }
    public string? Gender { get; set; }
    public int LearningLevelId { get; set; }
    public string LearningLevelName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int ActiveEnrollmentsCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
