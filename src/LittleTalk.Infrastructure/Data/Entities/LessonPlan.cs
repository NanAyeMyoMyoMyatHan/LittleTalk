using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class LessonPlan
{
    public int LessonPlanId { get; set; }

    public int? CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string Topic { get; set; } = null!;

    public string? Description { get; set; }

    public int LearningLevelId { get; set; }

    public string AgeGroup { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public string LearningObjectives { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();

    public virtual Course? Course { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual LearningLevel LearningLevel { get; set; } = null!;

    public virtual ICollection<LessonActivity> LessonActivities { get; set; } = new List<LessonActivity>();

    public virtual ICollection<LessonPlanMaterial> LessonPlanMaterials { get; set; } = new List<LessonPlanMaterial>();

    public virtual ICollection<LessonStep> LessonSteps { get; set; } = new List<LessonStep>();

    public virtual ICollection<LessonVocabulary> LessonVocabularies { get; set; } = new List<LessonVocabulary>();
}
