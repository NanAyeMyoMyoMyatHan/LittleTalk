using FluentValidation;
using LittleTalk.Application.Features.LessonPlans.DTOs;

namespace LittleTalk.Application.Features.LessonPlans.Validators;

public class CreateLessonPlanValidator : AbstractValidator<CreateLessonPlanDto>
{
    public CreateLessonPlanValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

        RuleFor(x => x.Topic)
            .NotEmpty().WithMessage("Topic is required.")
            .MaximumLength(100).WithMessage("Topic cannot exceed 100 characters.");

        RuleFor(x => x.LearningLevelId)
            .GreaterThan(0).WithMessage("Valid Learning Level is required.");

        RuleFor(x => x.AgeGroup)
            .NotEmpty().WithMessage("Target age group is required.")
            .MaximumLength(50).WithMessage("Age group cannot exceed 50 characters.");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(10, 180).WithMessage("Duration must be between 10 and 180 minutes.");

        RuleFor(x => x.LearningObjectives)
            .NotEmpty().WithMessage("Learning objectives are required.");

        RuleFor(x => x.Status)
            .Must(s => s is "Draft" or "Published" or "Completed" or "Archived")
            .WithMessage("Status must be 'Draft', 'Published', 'Completed', or 'Archived'.");
    }
}

public class UpdateLessonPlanValidator : AbstractValidator<UpdateLessonPlanDto>
{
    public UpdateLessonPlanValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

        RuleFor(x => x.Topic)
            .NotEmpty().WithMessage("Topic is required.")
            .MaximumLength(100).WithMessage("Topic cannot exceed 100 characters.");

        RuleFor(x => x.LearningLevelId)
            .GreaterThan(0).WithMessage("Valid Learning Level is required.");

        RuleFor(x => x.AgeGroup)
            .NotEmpty().WithMessage("Target age group is required.");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(10, 180).WithMessage("Duration must be between 10 and 180 minutes.");

        RuleFor(x => x.LearningObjectives)
            .NotEmpty().WithMessage("Learning objectives are required.");

        RuleFor(x => x.Status)
            .Must(s => s is "Draft" or "Published" or "Completed" or "Archived")
            .WithMessage("Status must be 'Draft', 'Published', 'Completed', or 'Archived'.");
    }
}

public class CreateLessonStepValidator : AbstractValidator<CreateLessonStepDto>
{
    public CreateLessonStepValidator()
    {
        RuleFor(x => x.StepName)
            .NotEmpty().WithMessage("Step name is required.")
            .MaximumLength(100).WithMessage("Step name cannot exceed 100 characters.");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(1, 120).WithMessage("Step duration must be between 1 and 120 minutes.");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Step instructions are required.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(1).WithMessage("Sort order must be at least 1.");
    }
}

public class AddLessonVocabularyValidator : AbstractValidator<AddLessonVocabularyDto>
{
    public AddLessonVocabularyValidator()
    {
        RuleFor(x => x.Word)
            .NotEmpty().WithMessage("Vocabulary word is required.")
            .MaximumLength(100).WithMessage("Word cannot exceed 100 characters.");

        RuleFor(x => x.Meaning)
            .NotEmpty().WithMessage("Meaning is required.")
            .MaximumLength(250).WithMessage("Meaning cannot exceed 250 characters.");

        RuleFor(x => x.ExampleSentence)
            .NotEmpty().WithMessage("Example sentence is required.")
            .MaximumLength(500).WithMessage("Example sentence cannot exceed 500 characters.");
    }
}
