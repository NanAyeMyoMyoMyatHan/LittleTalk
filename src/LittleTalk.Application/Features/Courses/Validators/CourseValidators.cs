using FluentValidation;
using LittleTalk.Application.Features.Courses.DTOs;

namespace LittleTalk.Application.Features.Courses.Validators;

public class CreateCourseValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Course Code is required.")
            .MaximumLength(30).WithMessage("Course Code cannot exceed 30 characters.");

        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course Name is required.")
            .MaximumLength(100).WithMessage("Course Name cannot exceed 100 characters.");

        RuleFor(x => x.LearningLevelId)
            .GreaterThan(0).WithMessage("Valid Learning Level is required.");

        RuleFor(x => x.MaxStudents)
            .InclusiveBetween(1, 50).WithMessage("Maximum students must be between 1 and 50.");

        RuleFor(x => x.DurationWeeks)
            .GreaterThan(0).When(x => x.DurationWeeks.HasValue).WithMessage("Duration must be a positive number of weeks.");

        RuleFor(x => x.Status)
            .Must(s => s is "Active" or "Draft" or "Completed" or "Archived")
            .WithMessage("Status must be 'Active', 'Draft', 'Completed', or 'Archived'.");
    }
}

public class UpdateCourseValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseValidator()
    {
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course Name is required.")
            .MaximumLength(100).WithMessage("Course Name cannot exceed 100 characters.");

        RuleFor(x => x.LearningLevelId)
            .GreaterThan(0).WithMessage("Valid Learning Level is required.");

        RuleFor(x => x.MaxStudents)
            .InclusiveBetween(1, 50).WithMessage("Maximum students must be between 1 and 50.");

        RuleFor(x => x.DurationWeeks)
            .GreaterThan(0).When(x => x.DurationWeeks.HasValue).WithMessage("Duration must be a positive number of weeks.");

        RuleFor(x => x.Status)
            .Must(s => s is "Active" or "Draft" or "Completed" or "Archived")
            .WithMessage("Status must be 'Active', 'Draft', 'Completed', or 'Archived'.");
    }
}

public class AssignTeacherValidator : AbstractValidator<AssignTeacherToCourseDto>
{
    public AssignTeacherValidator()
    {
        RuleFor(x => x.TeacherId)
            .GreaterThan(0).WithMessage("Valid Teacher ID is required.");

        RuleFor(x => x.RoleInCourse)
            .Must(r => r is "Primary" or "CoTeacher" or "Assistant" or "Substitute")
            .WithMessage("Role must be 'Primary', 'CoTeacher', 'Assistant', or 'Substitute'.");
    }
}
