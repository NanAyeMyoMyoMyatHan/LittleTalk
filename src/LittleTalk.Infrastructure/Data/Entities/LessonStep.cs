using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class LessonStep
{
    public int LessonStepId { get; set; }

    public int LessonPlanId { get; set; }

    public string StepName { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public int? TeachingMethodId { get; set; }

    public string? TeachingMethodCustom { get; set; }

    public string Instructions { get; set; } = null!;

    public string? TeacherScript { get; set; }

    public string? ExpectedStudentResponse { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<LessonActivity> LessonActivities { get; set; } = new List<LessonActivity>();

    public virtual LessonPlan LessonPlan { get; set; } = null!;

    public virtual TeachingMethod? TeachingMethod { get; set; }
}
