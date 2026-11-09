using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class LessonPlanMaterial
{
    public int LessonPlanMaterialId { get; set; }

    public int LessonPlanId { get; set; }

    public int MaterialId { get; set; }

    public string? UsageInstructions { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual LessonPlan LessonPlan { get; set; } = null!;

    public virtual TeachingMaterial Material { get; set; } = null!;
}
