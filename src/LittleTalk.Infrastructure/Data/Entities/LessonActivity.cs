using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class LessonActivity
{
    public int LessonActivityId { get; set; }

    public int LessonPlanId { get; set; }

    public int ActivityId { get; set; }

    public int? LessonStepId { get; set; }

    public int StepOrder { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual LessonPlan LessonPlan { get; set; } = null!;

    public virtual LessonStep? LessonStep { get; set; }
}
