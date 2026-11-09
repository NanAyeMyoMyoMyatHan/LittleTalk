using LittleTalk.Application.Features.Assessments;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Assessments;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class AssessmentValidationPolicyTests
{
    private readonly AssessmentValidationPolicy _policy = new();

    [Fact]
    public void ValidateCreate_ShouldFail_WhenStudentIdInvalid()
    {
        var request = new CreateAssessmentRequestModel
        {
            StudentId = 0,
            Criteria = new List<CriterionScoreInputModel>
            {
                new() { CriterionName = "Vocabulary", Score = 4 }
            }
        };

        var result = _policy.ValidateCreate(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
        Assert.Contains(result.Errors, e => e.Contains("Student"));
    }

    [Fact]
    public void ValidateCreate_ShouldFail_WhenCriteriaEmpty()
    {
        var request = new CreateAssessmentRequestModel
        {
            StudentId = 1,
            Criteria = new List<CriterionScoreInputModel>()
        };

        var result = _policy.ValidateCreate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("At least one"));
    }

    [Fact]
    public void ValidateCreate_ShouldFail_WhenScoreOutOfRange()
    {
        var request = new CreateAssessmentRequestModel
        {
            StudentId = 1,
            Criteria = new List<CriterionScoreInputModel>
            {
                new() { CriterionName = "Fluency", Score = 6 }
            }
        };

        var result = _policy.ValidateCreate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("between 1 and 5"));
    }

    [Fact]
    public void ValidateCreate_ShouldSucceed_WhenValid()
    {
        var request = new CreateAssessmentRequestModel
        {
            StudentId = 1,
            Status = "Completed",
            Strengths = "  Great vocabulary!  ",
            Criteria = new List<CriterionScoreInputModel>
            {
                new() { CriterionName = "  Vocabulary  ", Score = 5, Notes = " Excellent recall " }
            }
        };

        var result = _policy.ValidateCreate(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Great vocabulary!", request.Strengths);
        Assert.Equal("Vocabulary", request.Criteria[0].CriterionName);
        Assert.Equal("Excellent recall", request.Criteria[0].Notes);
    }
}
