using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Activities;

namespace LittleTalk.Application.Features.Activities;

public class ActivityValidationPolicy
{
    private static readonly string[] AllowedDifficulties = { "Easy", "Medium", "Hard", "All Levels" };

    public Result ValidateCreate(CreateActivityRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        request.ActivityName = request.ActivityName?.Trim() ?? string.Empty;
        request.Description = request.Description?.Trim() ?? string.Empty;
        request.Instructions = request.Instructions?.Trim() ?? string.Empty;
        request.SuitableAgeGroup = request.SuitableAgeGroup?.Trim();
        request.DifficultyLevel = request.DifficultyLevel?.Trim() ?? "Medium";
        request.RequiredMaterials = request.RequiredMaterials?.Trim();
        request.ExpectedLearningOutcome = request.ExpectedLearningOutcome?.Trim();

        if (request.CategoryId <= 0)
            errors.Add("Valid Activity Category is required.");

        if (string.IsNullOrWhiteSpace(request.ActivityName))
            errors.Add("Activity name is required.");
        else if (request.ActivityName.Length > 100)
            errors.Add("Activity name cannot exceed 100 characters.");

        if (string.IsNullOrWhiteSpace(request.Description))
            errors.Add("Description is required.");

        if (string.IsNullOrWhiteSpace(request.Instructions))
            errors.Add("Instructions are required.");

        if (request.DurationMinutes is <= 0 or > 120)
            errors.Add("Duration must be between 1 and 120 minutes.");

        if (!AllowedDifficulties.Contains(request.DifficultyLevel, StringComparer.OrdinalIgnoreCase))
            errors.Add("Difficulty level must be 'Easy', 'Medium', 'Hard', or 'All Levels'.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Activity validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateUpdate(UpdateActivityRequestModel request)
    {
        return ValidateCreate(request);
    }
}
