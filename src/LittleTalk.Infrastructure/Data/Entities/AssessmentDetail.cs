using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class AssessmentDetail
{
    public int AssessmentDetailId { get; set; }

    public int AssessmentId { get; set; }

    public string CriterionName { get; set; } = null!;

    public int Score { get; set; }

    public string? Notes { get; set; }

    public virtual Assessment Assessment { get; set; } = null!;
}
