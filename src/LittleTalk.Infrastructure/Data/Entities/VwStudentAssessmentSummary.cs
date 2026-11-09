using System;
using System.Collections.Generic;

namespace LittleTalk.Infrastructure.Data.Entities;

public partial class VwStudentAssessmentSummary
{
    public int AssessmentId { get; set; }

    public int StudentId { get; set; }

    public string StudentCode { get; set; } = null!;

    public string StudentFullName { get; set; } = null!;

    public string? StudentNickName { get; set; }

    public int? SessionId { get; set; }

    public int? CourseId { get; set; }

    public string? CourseName { get; set; }

    public int? LessonPlanId { get; set; }

    public string? LessonTitle { get; set; }

    public int EvaluatorTeacherId { get; set; }

    public string EvaluatorTeacherName { get; set; } = null!;

    public DateTime AssessmentDate { get; set; }

    public string AssessmentStatus { get; set; } = null!;

    public decimal? StoredOverallScore { get; set; }

    public int? CriteriaCount { get; set; }

    public decimal? CalculatedOverallScore { get; set; }

    public int? LowestCriterionScore { get; set; }

    public int? HighestCriterionScore { get; set; }
}
