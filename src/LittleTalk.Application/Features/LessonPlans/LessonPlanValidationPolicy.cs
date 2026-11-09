using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Application.Features.LessonPlans;

public class LessonPlanValidationPolicy
{
    private static readonly string[] AllowedStatuses = { "Draft", "Review", "Approved", "Archived" };

    public Result ValidateCreate(CreateLessonPlanRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        request.Title = request.Title?.Trim() ?? string.Empty;
        request.Topic = request.Topic?.Trim() ?? string.Empty;
        request.Description = request.Description?.Trim();
        request.AgeGroup = request.AgeGroup?.Trim() ?? string.Empty;
        request.LearningObjectives = request.LearningObjectives?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Lesson plan title is required.");
        else if (request.Title.Length > 200)
            errors.Add("Lesson plan title cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(request.Topic))
            errors.Add("Topic is required.");
        else if (request.Topic.Length > 100)
            errors.Add("Topic cannot exceed 100 characters.");

        if (request.LearningLevelId <= 0)
            errors.Add("Valid Learning Level is required.");

        if (string.IsNullOrWhiteSpace(request.AgeGroup))
            errors.Add("Age group is required.");

        if (request.DurationMinutes is <= 0 or > 180)
            errors.Add("Duration must be between 1 and 180 minutes.");

        if (string.IsNullOrWhiteSpace(request.LearningObjectives))
            errors.Add("Learning objectives are required.");

        if (!string.IsNullOrWhiteSpace(request.Status) && !AllowedStatuses.Contains(request.Status.Trim(), StringComparer.OrdinalIgnoreCase))
            errors.Add("Status must be 'Draft', 'Review', 'Approved', or 'Archived'.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Lesson plan validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateUpdate(UpdateLessonPlanRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        request.Title = request.Title?.Trim() ?? string.Empty;
        request.Topic = request.Topic?.Trim() ?? string.Empty;
        request.Description = request.Description?.Trim();
        request.AgeGroup = request.AgeGroup?.Trim() ?? string.Empty;
        request.LearningObjectives = request.LearningObjectives?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Lesson plan title is required.");
        else if (request.Title.Length > 200)
            errors.Add("Lesson plan title cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(request.Topic))
            errors.Add("Topic is required.");
        else if (request.Topic.Length > 100)
            errors.Add("Topic cannot exceed 100 characters.");

        if (request.LearningLevelId <= 0)
            errors.Add("Valid Learning Level is required.");

        if (string.IsNullOrWhiteSpace(request.AgeGroup))
            errors.Add("Age group is required.");

        if (request.DurationMinutes is <= 0 or > 180)
            errors.Add("Duration must be between 1 and 180 minutes.");

        if (string.IsNullOrWhiteSpace(request.LearningObjectives))
            errors.Add("Learning objectives are required.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Lesson plan update validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateStep(CreateLessonStepRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Step payload cannot be empty.");

        var errors = new List<string>();

        request.StepName = request.StepName?.Trim() ?? string.Empty;
        request.Instructions = request.Instructions?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.StepName))
            errors.Add("Step name is required.");

        if (request.DurationMinutes is <= 0 or > 120)
            errors.Add("Step duration must be between 1 and 120 minutes.");

        if (string.IsNullOrWhiteSpace(request.Instructions))
            errors.Add("Step instructions are required.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Step validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateVocabulary(AddLessonVocabularyRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Vocabulary payload cannot be empty.");

        var errors = new List<string>();

        request.Word = request.Word?.Trim() ?? string.Empty;
        request.Meaning = request.Meaning?.Trim() ?? string.Empty;
        request.ExampleSentence = request.ExampleSentence?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.Word))
            errors.Add("Vocabulary word is required.");

        if (string.IsNullOrWhiteSpace(request.Meaning))
            errors.Add("Word meaning is required.");

        if (string.IsNullOrWhiteSpace(request.ExampleSentence))
            errors.Add("Example sentence is required.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Vocabulary validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateDuplicate(DuplicateLessonPlanRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Duplicate payload cannot be empty.");

        request.NewTitle = request.NewTitle?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.NewTitle))
            return Result.ValidationError("New lesson plan title is required.");

        return Result.Success("Validation succeeded.");
    }
}
