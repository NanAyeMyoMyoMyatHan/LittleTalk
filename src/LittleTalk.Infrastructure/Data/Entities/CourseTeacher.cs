using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class CourseTeacher
{
    public int CourseTeacherId { get; set; }

    public int CourseId { get; set; }

    public int TeacherId { get; set; }

    public string RoleInCourse { get; set; } = null!;

    public DateTime AssignedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User Teacher { get; set; } = null!;
}
