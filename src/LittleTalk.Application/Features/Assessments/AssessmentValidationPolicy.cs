using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Assessments;

namespace LittleTalk.Application.Features.Assessments;

public class AssessmentValidationPolicy
{
    private static readonly string[] AllowedStatuses = { "Draft", "Completed", "Finalized" };

    public Result ValidateCreate(CreateAssessmentRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        if (request.StudentId <= 0)
            errors.Add("Valid Student is required.");

        if (request.SessionId.HasValue && request.SessionId.Value <= 0)
            errors.Add("Session ID must be greater than zero when provided.");

        if (request.LessonPlanId.HasValue && request.LessonPlanId.Value <= 0)
            errors.Add("Lesson Plan ID must be greater than zero when provided.");

        if (request.AssessmentDate == default)
            errors.Add("Assessment date is required.");

        if (string.IsNullOrWhiteSpace(request.Status) || !AllowedStatuses.Contains(request.Status.Trim(), StringComparer.OrdinalIgnoreCase))
            errors.Add("Status must be 'Draft', 'Completed', or 'Finalized'.");

        request.Strengths = request.Strengths?.Trim();
        request.AreasForImprovement = request.AreasForImprovement?.Trim();
        request.GeneralFeedback = request.GeneralFeedback?.Trim();

        if (request.Criteria == null || !request.Criteria.Any())
        {
            errors.Add("At least one assessment criterion score is required.");
        }
        else
        {
            for (int i = 0; i < request.Criteria.Count; i++)
            {
                var c = request.Criteria[i];
                if (string.IsNullOrWhiteSpace(c.CriterionName))
                    errors.Add($"Criterion at index {i} requires a valid name.");
                else
                    c.CriterionName = c.CriterionName.Trim();

                if (c.Score < 1 || c.Score > 5)
                    errors.Add($"Criterion '{c.CriterionName}' score must be between 1 and 5.");

                c.Notes = c.Notes?.Trim();
            }
        }

        return errors.Count > 0
            ? Result.ValidationError(errors, "Assessment validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateUpdate(UpdateAssessmentRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        if (request.AssessmentDate == default)
            errors.Add("Assessment date is required.");

        if (string.IsNullOrWhiteSpace(request.Status) || !AllowedStatuses.Contains(request.Status.Trim(), StringComparer.OrdinalIgnoreCase))
            errors.Add("Status must be 'Draft', 'Completed', or 'Finalized'.");

        request.Strengths = request.Strengths?.Trim();
        request.AreasForImprovement = request.AreasForImprovement?.Trim();
        request.GeneralFeedback = request.GeneralFeedback?.Trim();

        if (request.Criteria != null && request.Criteria.Any())
        {
            for (int i = 0; i < request.Criteria.Count; i++)
            {
                var c = request.Criteria[i];
                if (string.IsNullOrWhiteSpace(c.CriterionName))
                    errors.Add($"Criterion at index {i} requires a valid name.");
                else
                    c.CriterionName = c.CriterionName.Trim();

                if (c.Score < 1 || c.Score > 5)
                    errors.Add($"Criterion '{c.CriterionName}' score must be between 1 and 5.");

                c.Notes = c.Notes?.Trim();
            }
        }

        return errors.Count > 0
            ? Result.ValidationError(errors, "Assessment validation failed.")
            : Result.Success("Validation succeeded.");
    }
}
