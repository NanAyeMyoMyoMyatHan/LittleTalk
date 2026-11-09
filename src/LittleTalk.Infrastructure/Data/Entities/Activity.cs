using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class Activity
{
    public int ActivityId { get; set; }

    public int CategoryId { get; set; }

    public string ActivityName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Instructions { get; set; } = null!;

    public string? SuitableAgeGroup { get; set; }

    public string DifficultyLevel { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public string? RequiredMaterials { get; set; }

    public string? ExpectedLearningOutcome { get; set; }

    public int CreatedBy { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ActivityCategory Category { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<LessonActivity> LessonActivities { get; set; } = new List<LessonActivity>();
}
