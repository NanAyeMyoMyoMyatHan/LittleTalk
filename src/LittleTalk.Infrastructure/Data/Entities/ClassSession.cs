using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class ClassSession
{
    public int SessionId { get; set; }

    public int CourseId { get; set; }

    public int? LessonPlanId { get; set; }

    public int TeacherId { get; set; }

    public DateOnly SessionDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? RoomOrLink { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Course Course { get; set; } = null!;

    public virtual LessonPlan? LessonPlan { get; set; }

    public virtual User Teacher { get; set; } = null!;
}
