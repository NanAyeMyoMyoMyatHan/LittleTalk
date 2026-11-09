using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class ActivityCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
