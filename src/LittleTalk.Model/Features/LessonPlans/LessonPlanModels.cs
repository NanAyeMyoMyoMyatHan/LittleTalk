using LittleTalk.Model.Common;

namespace LittleTalk.Model.Features.LessonPlans;

public class SearchLessonPlanRequestModel : PaginationRequestModel
{
    public int? CourseId { get; set; }
    public int? LearningLevelId { get; set; }
    public int? LevelId { get => LearningLevelId; set => LearningLevelId = value; }
    public string? Status { get; set; }
}

public class CreateLessonPlanRequestModel
{
    public int? CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LearningLevelId { get; set; }
    public string AgeGroup { get; set; } = string.Empty;
    public int DurationMinutes { get; set; } = 45;
    public string LearningObjectives { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public List<CreateLessonStepRequestModel>? InitialSteps { get; set; }
}

public class UpdateLessonPlanRequestModel
{
    public int? CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LearningLevelId { get; set; }
    public string AgeGroup { get; set; } = string.Empty;
    public int DurationMinutes { get; set; } = 45;
    public string LearningObjectives { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
}

public class CreateLessonStepRequestModel
{
    public string StepName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int? TeachingMethodId { get; set; }
    public string? TeachingMethodCustom { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public string? TeacherScript { get; set; }
    public string? ExpectedStudentResponse { get; set; }
    public int SortOrder { get; set; } = 1;
}

public class UpdateLessonStepRequestModel : CreateLessonStepRequestModel { }

public class AddLessonVocabularyRequestModel
{
    public string Word { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string? Pronunciation { get; set; }
    public string ExampleSentence { get; set; } = string.Empty;
    public string? Topic { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public int SortOrder { get; set; } = 1;
}

public class LinkMaterialRequestModel
{
    public int MaterialId { get; set; }
    public string? UsageInstructions { get; set; }
    public int SortOrder { get; set; } = 1;
}

public class LinkActivityRequestModel
{
    public int ActivityId { get; set; }
    public int? LessonStepId { get; set; }
    public int StepOrder { get; set; } = 1;
    public string? Notes { get; set; }
}

public class DuplicateLessonPlanRequestModel
{
    public string NewTitle { get; set; } = string.Empty;
    public int? TargetCourseId { get; set; }
}

public class LessonPlanSummaryResponseModel
{
    public int LessonPlanId { get; set; }
    public int? CourseId { get; set; }
    public string? CourseName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LearningLevelId { get; set; }
    public string LearningLevelName { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StepsCount { get; set; }
    public int VocabularyCount { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class LessonPlanDetailResponseModel : LessonPlanSummaryResponseModel
{
    public string LearningObjectives { get; set; } = string.Empty;
    public DateTime? UpdatedDate { get; set; }
    public List<LessonStepResponseModel> Steps { get; set; } = new();
    public List<LessonVocabularyResponseModel> Vocabulary { get; set; } = new();
    public List<LessonPlanMaterialResponseModel> Materials { get; set; } = new();
    public List<LessonActivityResponseModel> Activities { get; set; } = new();
}

public class LessonStepResponseModel
{
    public int LessonStepId { get; set; }
    public int LessonPlanId { get; set; }
    public string StepName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int? TeachingMethodId { get; set; }
    public string? TeachingMethodName { get; set; }
    public string? TeachingMethodCustom { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public string? TeacherScript { get; set; }
    public string? ExpectedStudentResponse { get; set; }
    public int SortOrder { get; set; }
}

public class LessonVocabularyResponseModel
{
    public int VocabularyId { get; set; }
    public int LessonPlanId { get; set; }
    public string Word { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string? Pronunciation { get; set; }
    public string ExampleSentence { get; set; } = string.Empty;
    public string? Topic { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public int SortOrder { get; set; }
}

public class LessonPlanMaterialResponseModel
{
    public int LessonPlanMaterialId { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? UsageInstructions { get; set; }
    public int SortOrder { get; set; }
}

public class LessonActivityResponseModel
{
    public int LessonActivityId { get; set; }
    public int ActivityId { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int? LessonStepId { get; set; }
    public string? LessonStepName { get; set; }
    public int StepOrder { get; set; }
    public string? Notes { get; set; }
}
