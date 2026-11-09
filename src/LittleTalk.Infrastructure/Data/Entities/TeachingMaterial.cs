using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class TeachingMaterial
{
    public int MaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    public string MaterialType { get; set; } = null!;

    public string? FileUrl { get; set; }

    public string? Description { get; set; }

    public string? SuitableAgeGroup { get; set; }

    public int? LearningLevelId { get; set; }

    public int UploadedBy { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual LearningLevel? LearningLevel { get; set; }

    public virtual ICollection<LessonPlanMaterial> LessonPlanMaterials { get; set; } = new List<LessonPlanMaterial>();

    public virtual User UploadedByNavigation { get; set; } = null!;
}
