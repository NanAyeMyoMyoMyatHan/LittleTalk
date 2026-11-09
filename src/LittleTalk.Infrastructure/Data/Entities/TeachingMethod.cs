using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class TeachingMethod
{
    public int TeachingMethodId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string TeachingInstructions { get; set; } = null!;

    public string? SuitableAgeGroup { get; set; }

    public int? SuitableLearningLevelId { get; set; }

    public string? RequiredMaterials { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<LessonStep> LessonSteps { get; set; } = new List<LessonStep>();

    public virtual LearningLevel? SuitableLearningLevel { get; set; }
}
