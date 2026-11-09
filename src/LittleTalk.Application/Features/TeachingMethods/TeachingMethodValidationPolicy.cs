using LittleTalk.Model.Common;
using LittleTalk.Model.Features.TeachingMethods;

namespace LittleTalk.Application.Features.TeachingMethods;

public class TeachingMethodValidationPolicy
{
    public Result ValidateCreate(CreateTeachingMethodRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        request.Name = request.Name?.Trim() ?? string.Empty;
        request.Description = request.Description?.Trim() ?? string.Empty;
        request.TeachingInstructions = request.TeachingInstructions?.Trim() ?? string.Empty;
        request.SuitableAgeGroup = request.SuitableAgeGroup?.Trim();
        request.RequiredMaterials = request.RequiredMaterials?.Trim();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Teaching method name is required.");
        else if (request.Name.Length > 100)
            errors.Add("Name cannot exceed 100 characters.");

        if (string.IsNullOrWhiteSpace(request.Description))
            errors.Add("Description is required.");

        if (string.IsNullOrWhiteSpace(request.TeachingInstructions))
            errors.Add("Teaching instructions are required.");

        if (request.SuitableLearningLevelId.HasValue && request.SuitableLearningLevelId.Value <= 0)
            errors.Add("Suitable learning level ID must be a positive integer.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Teaching method validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateUpdate(UpdateTeachingMethodRequestModel request)
    {
        return ValidateCreate(request);
    }
}
