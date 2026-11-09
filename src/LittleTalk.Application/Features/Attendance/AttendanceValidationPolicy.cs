using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Attendance;

namespace LittleTalk.Application.Features.Attendance;

public class AttendanceValidationPolicy
{
    private static readonly string[] AllowedSessionStatuses = { "Scheduled", "InProgress", "Completed", "Cancelled" };
    private static readonly string[] AllowedAttendanceStatuses = { "Present", "Absent", "Late", "Excused" };

    public Result ValidateCreateSession(CreateClassSessionRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        if (request.CourseId <= 0)
            errors.Add("Valid Course is required.");

        if (request.TeacherId <= 0)
            errors.Add("Valid Teacher is required.");

        if (request.SessionDate == default)
            errors.Add("Session date is required.");

        if (request.StartTime.HasValue && request.EndTime.HasValue && request.StartTime.Value >= request.EndTime.Value)
            errors.Add("Start time must be before end time.");

        request.RoomOrLink = request.RoomOrLink?.Trim();
        request.Notes = request.Notes?.Trim();

        return errors.Count > 0
            ? Result.ValidationError(errors, "Session validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateUpdateSession(UpdateClassSessionRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        if (request.TeacherId <= 0)
            errors.Add("Valid Teacher is required.");

        if (request.SessionDate == default)
            errors.Add("Session date is required.");

        if (request.StartTime.HasValue && request.EndTime.HasValue && request.StartTime.Value >= request.EndTime.Value)
            errors.Add("Start time must be before end time.");

        if (!string.IsNullOrWhiteSpace(request.Status) && !AllowedSessionStatuses.Contains(request.Status.Trim(), StringComparer.OrdinalIgnoreCase))
            errors.Add("Status must be 'Scheduled', 'InProgress', 'Completed', or 'Cancelled'.");

        request.RoomOrLink = request.RoomOrLink?.Trim();
        request.Notes = request.Notes?.Trim();

        return errors.Count > 0
            ? Result.ValidationError(errors, "Session update validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateBatchAttendance(BatchRecordAttendanceRequestModel request)
    {
        if (request == null || request.Records == null || request.Records.Count == 0)
            return Result.ValidationError("Attendance records list cannot be empty.");

        var errors = new List<string>();

        foreach (var rec in request.Records)
        {
            if (rec.StudentId <= 0)
            {
                errors.Add($"Invalid Student ID ({rec.StudentId}) in attendance record.");
            }

            rec.Status = rec.Status?.Trim() ?? "Present";
            if (!AllowedAttendanceStatuses.Contains(rec.Status, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Invalid attendance status '{rec.Status}' for Student ID {rec.StudentId}. Allowed: Present, Absent, Late, Excused.");
            }

            rec.Remarks = rec.Remarks?.Trim();
        }

        return errors.Count > 0
            ? Result.ValidationError(errors, "Batch attendance validation failed.")
            : Result.Success("Validation succeeded.");
    }
}
