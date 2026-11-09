using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class Assessment
{
    public int AssessmentId { get; set; }

    public int StudentId { get; set; }

    public int? SessionId { get; set; }

    public int? LessonPlanId { get; set; }

    public int EvaluatorTeacherId { get; set; }

    public DateTime AssessmentDate { get; set; }

    public string Status { get; set; } = null!;

    public decimal? OverallScore { get; set; }

    public string? Strengths { get; set; }

    public string? AreasForImprovement { get; set; }

    public string? GeneralFeedback { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<AssessmentDetail> AssessmentDetails { get; set; } = new List<AssessmentDetail>();

    public virtual User EvaluatorTeacher { get; set; } = null!;

    public virtual LessonPlan? LessonPlan { get; set; }

    public virtual ClassSession? Session { get; set; }

    public virtual Student Student { get; set; } = null!;
}
