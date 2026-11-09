using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Students;

namespace LittleTalk.Application.Features.Students;

public class StudentValidationPolicy
{
    private static readonly string[] AllowedGenders = { "Boy", "Girl", "Prefer not to say" };

    public Result ValidateCreate(CreateStudentRequestModel request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            errors.Add("Full name is required.");
        }
        else if (request.FullName.Trim().Length > 100)
        {
            errors.Add("Full name cannot exceed 100 characters.");
        }

        if (!string.IsNullOrWhiteSpace(request.NickName) && request.NickName.Trim().Length > 50)
        {
            errors.Add("Nickname cannot exceed 50 characters.");
        }

        if (request.DateOfBirth == default)
        {
            errors.Add("Date of birth is required.");
        }
        else
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (request.DateOfBirth > today.AddYears(-2))
            {
                errors.Add("Student must be at least 2 years old.");
            }
            if (request.DateOfBirth < today.AddYears(-18))
            {
                errors.Add("Student must be under 18 years old.");
            }
        }

        if (request.LearningLevelId <= 0)
        {
            errors.Add("Valid Learning Level is required.");
        }

        if (!string.IsNullOrWhiteSpace(request.Gender) && !AllowedGenders.Contains(request.Gender.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            errors.Add("Gender must be 'Boy', 'Girl', or 'Prefer not to say'.");
        }

        if (!string.IsNullOrWhiteSpace(request.ParentContact) && request.ParentContact.Trim().Length > 50)
        {
            errors.Add("Parent contact cannot exceed 50 characters.");
        }

        // Normalize inputs
        request.FullName = request.FullName?.Trim() ?? string.Empty;
        request.NickName = request.NickName?.Trim();
        request.Gender = request.Gender?.Trim();
        request.ParentName = request.ParentName?.Trim();
        request.ParentContact = request.ParentContact?.Trim();
        request.Notes = request.Notes?.Trim();

        return errors.Count > 0 
            ? Result.ValidationError(errors, "Student validation failed.") 
            : Result.Success();
    }

    public Result ValidateUpdate(int studentId, UpdateStudentRequestModel request)
    {
        if (studentId <= 0)
        {
            return Result.ValidationError("Valid Student ID is required.");
        }

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            errors.Add("Full name is required.");
        }
        else if (request.FullName.Trim().Length > 100)
        {
            errors.Add("Full name cannot exceed 100 characters.");
        }

        if (!string.IsNullOrWhiteSpace(request.NickName) && request.NickName.Trim().Length > 50)
        {
            errors.Add("Nickname cannot exceed 50 characters.");
        }

        if (request.DateOfBirth == default)
        {
            errors.Add("Date of birth is required.");
        }
        else
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (request.DateOfBirth > today.AddYears(-2))
            {
                errors.Add("Student must be at least 2 years old.");
            }
            if (request.DateOfBirth < today.AddYears(-18))
            {
                errors.Add("Student must be under 18 years old.");
            }
        }

        if (request.LearningLevelId <= 0)
        {
            errors.Add("Valid Learning Level is required.");
        }

        if (!string.IsNullOrWhiteSpace(request.Gender) && !AllowedGenders.Contains(request.Gender.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            errors.Add("Gender must be 'Boy', 'Girl', or 'Prefer not to say'.");
        }

        // Normalize inputs
        request.FullName = request.FullName?.Trim() ?? string.Empty;
        request.NickName = request.NickName?.Trim();
        request.Gender = request.Gender?.Trim();
        request.ParentName = request.ParentName?.Trim();
        request.ParentContact = request.ParentContact?.Trim();
        request.Notes = request.Notes?.Trim();

        return errors.Count > 0 
            ? Result.ValidationError(errors, "Student validation failed.") 
            : Result.Success();
    }

    public Result ValidateSearch(SearchStudentRequestModel request)
    {
        if (request.PageNumber < 1) request.PageNumber = 1;
        if (request.PageSize < 1) request.PageSize = 10;
        if (request.PageSize > 100) request.PageSize = 100;

        if (request.LearningLevelId.HasValue && request.LearningLevelId.Value <= 0)
        {
            request.LearningLevelId = null;
        }

        request.SearchTerm = request.SearchTerm?.Trim();
        request.SortBy = request.SortBy?.Trim().ToLower();

        return Result.Success();
    }
}
