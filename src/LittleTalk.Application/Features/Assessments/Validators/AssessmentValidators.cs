using FluentValidation;
using LittleTalk.Application.Features.Assessments.DTOs;

namespace LittleTalk.Application.Features.Assessments.Validators;

public class CreateAssessmentValidator : AbstractValidator<CreateAssessmentDto>
{
    private static readonly HashSet<string> ValidCriteria = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pronunciation", "Vocabulary", "Fluency", "Sentence Formation", "Confidence", "Listening and Responding"
    };

    public CreateAssessmentValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid Student ID is required.");

        RuleFor(x => x.AssessmentDate)
            .NotEmpty().WithMessage("Assessment date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5)).WithMessage("Assessment date cannot be in the future.");

        RuleFor(x => x.Status)
            .Must(s => s is "Draft" or "Finalized")
            .WithMessage("Status must be 'Draft' or 'Finalized'.");

        RuleForEach(x => x.Criteria).ChildRules(c =>
        {
            c.RuleFor(cr => cr.CriterionName)
                .Must(name => ValidCriteria.Contains(name))
                .WithMessage($"Criterion name must be one of: {string.Join(", ", ValidCriteria)}");

            c.RuleFor(cr => cr.Score)
                .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");
        });

        // If finalized, enforce that all 6 criteria are provided!
        When(x => x.Status == "Finalized", () =>
        {
            RuleFor(x => x.Criteria)
                .Must(c => c.Count == 6)
                .WithMessage("All 6 assessment criteria must be evaluated before finalizing.")
                .Must(c => c.Select(cr => cr.CriterionName.ToLower()).Distinct().Count() == 6)
                .WithMessage("All 6 distinct assessment criteria must be evaluated.");
        });
    }
}

public class UpdateAssessmentValidator : AbstractValidator<UpdateAssessmentDto>
{
    private static readonly HashSet<string> ValidCriteria = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pronunciation", "Vocabulary", "Fluency", "Sentence Formation", "Confidence", "Listening and Responding"
    };

    public UpdateAssessmentValidator()
    {
        RuleFor(x => x.AssessmentDate)
            .NotEmpty().WithMessage("Assessment date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5)).WithMessage("Assessment date cannot be in the future.");

        RuleFor(x => x.Status)
            .Must(s => s is "Draft" or "Finalized")
            .WithMessage("Status must be 'Draft' or 'Finalized'.");

        RuleForEach(x => x.Criteria).ChildRules(c =>
        {
            c.RuleFor(cr => cr.CriterionName)
                .Must(name => ValidCriteria.Contains(name))
                .WithMessage($"Criterion name must be one of: {string.Join(", ", ValidCriteria)}");

            c.RuleFor(cr => cr.Score)
                .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");
        });

        When(x => x.Status == "Finalized", () =>
        {
            RuleFor(x => x.Criteria)
                .Must(c => c.Count == 6)
                .WithMessage("All 6 assessment criteria must be evaluated before finalizing.")
                .Must(c => c.Select(cr => cr.CriterionName.ToLower()).Distinct().Count() == 6)
                .WithMessage("All 6 distinct assessment criteria must be evaluated.");
        });
    }
}
