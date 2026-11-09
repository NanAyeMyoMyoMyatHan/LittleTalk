using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? NickName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public int LearningLevelId { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string? ParentName { get; set; }

    public string? ParentContact { get; set; }

    public string? EmergencyNotes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual LearningLevel LearningLevel { get; set; } = null!;
}
