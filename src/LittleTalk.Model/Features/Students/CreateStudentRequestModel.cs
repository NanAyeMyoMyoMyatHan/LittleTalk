namespace LittleTalk.Model.Features.Students;

public class CreateStudentRequestModel
{
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public int LearningLevelId { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? ParentName { get; set; }
    public string? ParentContact { get; set; }
    public string? Notes { get; set; }
}
