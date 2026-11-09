using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class LessonVocabulary
{
    public int VocabularyId { get; set; }

    public int LessonPlanId { get; set; }

    public string Word { get; set; } = null!;

    public string Meaning { get; set; } = null!;

    public string? Pronunciation { get; set; }

    public string ExampleSentence { get; set; } = null!;

    public string? Topic { get; set; }

    public string? ImageUrl { get; set; }

    public string? AudioUrl { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual LessonPlan LessonPlan { get; set; } = null!;
}
