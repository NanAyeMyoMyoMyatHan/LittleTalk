using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class LearningLevel
{
    public int LearningLevelId { get; set; }

    public string LevelName { get; set; } = null!;

    public string LevelCode { get; set; } = null!;

    public string? AgeRangeDescription { get; set; }

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<LessonPlan> LessonPlans { get; set; } = new List<LessonPlan>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<TeachingMaterial> TeachingMaterials { get; set; } = new List<TeachingMaterial>();

    public virtual ICollection<TeachingMethod> TeachingMethods { get; set; } = new List<TeachingMethod>();
}
