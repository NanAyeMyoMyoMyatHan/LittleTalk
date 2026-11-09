using LittleTalk.Application.Features.LessonPlans;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class LessonPlanValidationPolicyTests
{
    private readonly LessonPlanValidationPolicy _policy = new();

    [Fact]
    public void ValidateCreate_ShouldFail_WhenTitleOrTopicMissing()
    {
        var request = new CreateLessonPlanRequestModel
        {
            Title = "",
            Topic = "   ",
            LearningLevelId = 1,
            AgeGroup = "5-7",
            LearningObjectives = "Learn words"
        };

        var result = _policy.ValidateCreate(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
        Assert.Contains(result.Errors, e => e.Contains("Lesson plan title is required"));
        Assert.Contains(result.Errors, e => e.Contains("Topic is required"));
    }

    [Fact]
    public void ValidateCreate_ShouldSucceed_WhenValid()
    {
        var request = new CreateLessonPlanRequestModel
        {
            Title = " Animals at the Zoo ",
            Topic = " Wild Animals ",
            LearningLevelId = 1,
            AgeGroup = "4-6 yrs",
            DurationMinutes = 45,
            LearningObjectives = "Students will be able to name 5 wild animals and describe sounds."
        };

        var result = _policy.ValidateCreate(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Animals at the Zoo", request.Title);
        Assert.Equal("Wild Animals", request.Topic);
    }
}
