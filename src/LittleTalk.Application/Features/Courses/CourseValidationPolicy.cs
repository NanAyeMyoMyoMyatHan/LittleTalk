using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Courses;

namespace LittleTalk.Application.Features.Courses;

public class CourseValidationPolicy
{
    private static readonly string[] AllowedRoles = { "Primary", "CoTeacher", "Assistant", "Substitute" };
    private static readonly string[] AllowedStatuses = { "Active", "Archived", "Upcoming" };

    public Result ValidateCreate(CreateCourseRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        request.CourseCode = request.CourseCode?.Trim() ?? string.Empty;
        request.CourseName = request.CourseName?.Trim() ?? string.Empty;
        request.Description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(request.CourseCode))
            errors.Add("Course code is required.");
        else if (request.CourseCode.Length > 50)
            errors.Add("Course code cannot exceed 50 characters.");

        if (string.IsNullOrWhiteSpace(request.CourseName))
            errors.Add("Course name is required.");
        else if (request.CourseName.Length > 200)
            errors.Add("Course name cannot exceed 200 characters.");

        if (request.LearningLevelId <= 0)
            errors.Add("Valid Learning Level is required.");

        if (request.MaxStudents is < 1 or > 100)
            errors.Add("Maximum students must be between 1 and 100.");

        if (request.DurationWeeks.HasValue && request.DurationWeeks.Value <= 0)
            errors.Add("Duration weeks must be greater than zero.");

        if (!string.IsNullOrWhiteSpace(request.Status) && !AllowedStatuses.Contains(request.Status.Trim(), StringComparer.OrdinalIgnoreCase))
            errors.Add("Status must be 'Active', 'Archived', or 'Upcoming'.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Course validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateUpdate(UpdateCourseRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        request.CourseName = request.CourseName?.Trim() ?? string.Empty;
        request.Description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(request.CourseName))
            errors.Add("Course name is required.");
        else if (request.CourseName.Length > 200)
            errors.Add("Course name cannot exceed 200 characters.");

        if (request.LearningLevelId <= 0)
            errors.Add("Valid Learning Level is required.");

        if (request.MaxStudents is < 1 or > 100)
            errors.Add("Maximum students must be between 1 and 100.");

        if (request.DurationWeeks.HasValue && request.DurationWeeks.Value <= 0)
            errors.Add("Duration weeks must be greater than zero.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Course update validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateAssignTeacher(AssignTeacherRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        var errors = new List<string>();

        if (request.TeacherId <= 0)
            errors.Add("Valid Teacher ID is required.");

        if (string.IsNullOrWhiteSpace(request.RoleInCourse) || !AllowedRoles.Contains(request.RoleInCourse.Trim(), StringComparer.OrdinalIgnoreCase))
            errors.Add("Role in course must be 'Primary', 'CoTeacher', 'Assistant', or 'Substitute'.");

        return errors.Count > 0
            ? Result.ValidationError(errors, "Assign teacher validation failed.")
            : Result.Success("Validation succeeded.");
    }

    public Result ValidateEnrollStudent(EnrollStudentRequestModel request)
    {
        if (request == null)
            return Result.ValidationError("Request payload cannot be empty.");

        if (request.StudentId <= 0)
            return Result.ValidationError("Valid Student ID is required.");

        request.Notes = request.Notes?.Trim();
        return Result.Success("Validation succeeded.");
    }
}
