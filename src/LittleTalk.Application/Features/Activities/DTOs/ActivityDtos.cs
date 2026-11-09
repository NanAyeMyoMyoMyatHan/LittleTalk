namespace LittleTalk.Application.Features.Activities.DTOs;

public class ActivitySummaryDto
{
    public int ActivityId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? SuitableAgeGroup { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string? ExpectedLearningOutcome { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ActivityDetailDto : ActivitySummaryDto
{
    public string Instructions { get; set; } = string.Empty;
    public string? RequiredMaterials { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class ActivityCategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int ActivitiesCount { get; set; }
}

public class CreateActivityDto
{
    public int CategoryId { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string? SuitableAgeGroup { get; set; }
    public string DifficultyLevel { get; set; } = "Medium";
    public int DurationMinutes { get; set; } = 15;
    public string? RequiredMaterials { get; set; }
    public string? ExpectedLearningOutcome { get; set; }
}

public class UpdateActivityDto : CreateActivityDto
{
    public bool IsActive { get; set; } = true;
}
