using FluentValidation;
using LittleTalk.Application.Features.Attendance.DTOs;

namespace LittleTalk.Application.Features.Attendance.Validators;

public class CreateClassSessionValidator : AbstractValidator<CreateClassSessionDto>
{
    public CreateClassSessionValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Valid Course ID is required.");

        RuleFor(x => x.TeacherId)
            .GreaterThan(0).WithMessage("Valid Teacher ID is required.");

        RuleFor(x => x.SessionDate)
            .NotEmpty().WithMessage("Session Date is required.");

        RuleFor(x => x)
            .Must(x => (!x.StartTime.HasValue && !x.EndTime.HasValue) || 
                       (x.StartTime.HasValue && x.EndTime.HasValue && x.EndTime.Value > x.StartTime.Value))
            .WithMessage("End time must be later than start time if times are specified.");
    }
}

public class BatchRecordAttendanceValidator : AbstractValidator<BatchRecordAttendanceDto>
{
    public BatchRecordAttendanceValidator()
    {
        RuleFor(x => x.Records)
            .NotEmpty().WithMessage("At least one student attendance record must be provided.");

        RuleForEach(x => x.Records).ChildRules(record =>
        {
            record.RuleFor(r => r.StudentId).GreaterThan(0).WithMessage("Valid Student ID is required.");
            record.RuleFor(r => r.Status)
                .Must(s => s is "Present" or "Absent" or "Late" or "Excused")
                .WithMessage("Status must be 'Present', 'Absent', 'Late', or 'Excused'.");
        });
    }
}
