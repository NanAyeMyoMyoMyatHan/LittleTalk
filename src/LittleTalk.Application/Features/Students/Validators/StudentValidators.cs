using FluentValidation;
using LittleTalk.Application.Features.Students.DTOs;

namespace LittleTalk.Application.Features.Students.Validators;

public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

        RuleFor(x => x.NickName)
            .MaximumLength(50).WithMessage("Nickname cannot exceed 50 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required.")
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)))
            .WithMessage("Student must be at least 2 years old.")
            .Must(dob => dob >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
            .WithMessage("Student must be under 18 years old.");

        RuleFor(x => x.LearningLevelId)
            .GreaterThan(0).WithMessage("Valid Learning Level is required.");

        RuleFor(x => x.Gender)
            .Must(g => string.IsNullOrEmpty(g) || g is "Boy" or "Girl" or "Prefer not to say")
            .WithMessage("Gender must be 'Boy', 'Girl', or 'Prefer not to say'.");

        RuleFor(x => x.ParentContact)
            .MaximumLength(50).WithMessage("Parent contact cannot exceed 50 characters.");
    }
}

public class UpdateStudentValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required.")
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)))
            .WithMessage("Student must be at least 2 years old.")
            .Must(dob => dob >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
            .WithMessage("Student must be under 18 years old.");

        RuleFor(x => x.LearningLevelId)
            .GreaterThan(0).WithMessage("Valid Learning Level is required.");

        RuleFor(x => x.Gender)
            .Must(g => string.IsNullOrEmpty(g) || g is "Boy" or "Girl" or "Prefer not to say")
            .WithMessage("Gender must be 'Boy', 'Girl', or 'Prefer not to say'.");
    }
}
