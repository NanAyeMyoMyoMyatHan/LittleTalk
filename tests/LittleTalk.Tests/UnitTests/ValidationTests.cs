using FluentValidation.TestHelper;
using LittleTalk.Application.Features.Assessments.DTOs;
using LittleTalk.Application.Features.Assessments.Validators;
using LittleTalk.Application.Features.Students.DTOs;
using LittleTalk.Application.Features.Students.Validators;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class ValidationTests
{
    private readonly CreateStudentValidator _studentValidator = new();
    private readonly CreateAssessmentValidator _assessmentValidator = new();

    [Fact]
    public void StudentValidator_ShouldHaveError_WhenFullNameIsEmpty()
    {
        var model = new CreateStudentDto
        {
            FullName = "",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-6)),
            LearningLevelId = 1
        };

        var result = _studentValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void StudentValidator_ShouldHaveError_WhenAgeIsUnderTwoYears()
    {
        var model = new CreateStudentDto
        {
            FullName = "Baby Kid",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)), // 6 months old
            LearningLevelId = 1
        };

        var result = _studentValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Fact]
    public void StudentValidator_ShouldPass_WhenDataIsValid()
    {
        var model = new CreateStudentDto
        {
            FullName = "Emma Watson",
            NickName = "Emmy",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-7)),
            LearningLevelId = 1,
            Gender = "Girl",
            ParentName = "Chris Watson",
            ParentContact = "+1-555-0199"
        };

        var result = _studentValidator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void AssessmentValidator_ShouldHaveError_WhenInvalidCriterionProvided()
    {
        var model = new CreateAssessmentDto
        {
            StudentId = 1,
            AssessmentDate = DateTime.UtcNow,
            Status = "Draft",
            Criteria = new List<CriterionInputDto>
            {
                new() { CriterionName = "InvalidNonExistentCriterion", Score = 4 }
            }
        };

        var result = _assessmentValidator.TestValidate(model);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Criteria"));
    }

    [Fact]
    public void AssessmentValidator_ShouldHaveError_WhenScoreOutOfRange()
    {
        var model = new CreateAssessmentDto
        {
            StudentId = 1,
            AssessmentDate = DateTime.UtcNow,
            Status = "Draft",
            Criteria = new List<CriterionInputDto>
            {
                new() { CriterionName = "Pronunciation", Score = 6 } // Max is 5
            }
        };

        var result = _assessmentValidator.TestValidate(model);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void AssessmentValidator_ShouldEnforceAllSixCriteria_WhenStatusIsFinalized()
    {
        // Only 3 criteria provided for Finalized assessment
        var model = new CreateAssessmentDto
        {
            StudentId = 1,
            AssessmentDate = DateTime.UtcNow,
            Status = "Finalized",
            Criteria = new List<CriterionInputDto>
            {
                new() { CriterionName = "Pronunciation", Score = 5 },
                new() { CriterionName = "Fluency", Score = 4 },
                new() { CriterionName = "Vocabulary", Score = 4 }
            }
        };

        var result = _assessmentValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Criteria);
    }

    [Fact]
    public void AssessmentValidator_ShouldPass_WhenAllSixCriteriaProvidedForFinalized()
    {
        var model = new CreateAssessmentDto
        {
            StudentId = 1,
            AssessmentDate = DateTime.UtcNow,
            Status = "Finalized",
            Criteria = new List<CriterionInputDto>
            {
                new() { CriterionName = "Pronunciation", Score = 5 },
                new() { CriterionName = "Vocabulary", Score = 4 },
                new() { CriterionName = "Fluency", Score = 4 },
                new() { CriterionName = "Sentence Formation", Score = 4 },
                new() { CriterionName = "Confidence", Score = 5 },
                new() { CriterionName = "Listening and Responding", Score = 5 }
            }
        };

        var result = _assessmentValidator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
