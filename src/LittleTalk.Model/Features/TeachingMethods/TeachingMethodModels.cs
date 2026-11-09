namespace LittleTalk.Model.Features.TeachingMethods;

public class CreateTeachingMethodRequestModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TeachingInstructions { get; set; } = string.Empty;
    public string? SuitableAgeGroup { get; set; }
    public int? SuitableLearningLevelId { get; set; }
    public string? RequiredMaterials { get; set; }
}

public class UpdateTeachingMethodRequestModel : CreateTeachingMethodRequestModel
{
    public bool IsActive { get; set; } = true;
}

public class TeachingMethodResponseModel
{
    public int TeachingMethodId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TeachingInstructions { get; set; } = string.Empty;
    public string? SuitableAgeGroup { get; set; }
    public int? SuitableLearningLevelId { get; set; }
    public string? SuitableLearningLevelName { get; set; }
    public string? RequiredMaterials { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
