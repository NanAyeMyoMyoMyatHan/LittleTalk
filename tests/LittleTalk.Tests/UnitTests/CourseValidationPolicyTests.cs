using LittleTalk.Application.Features.Courses;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Courses;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class CourseValidationPolicyTests
{
    private readonly CourseValidationPolicy _policy = new();

    [Fact]
    public void ValidateCreate_ShouldFail_WhenCodeOrNameIsEmpty()
    {
        var request = new CreateCourseRequestModel
        {
            CourseCode = "",
            CourseName = "   ",
            LearningLevelId = 1
        };

        var result = _policy.ValidateCreate(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
        Assert.Contains(result.Errors, e => e.Contains("Course code is required"));
        Assert.Contains(result.Errors, e => e.Contains("Course name is required"));
    }

    [Fact]
    public void ValidateCreate_ShouldSucceed_WhenValid()
    {
        var request = new CreateCourseRequestModel
        {
            CourseCode = " ENG-101 ",
            CourseName = " English Explorers 1 ",
            LearningLevelId = 1,
            MaxStudents = 12
        };

        var result = _policy.ValidateCreate(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("ENG-101", request.CourseCode);
        Assert.Equal("English Explorers 1", request.CourseName);
    }

    [Fact]
    public void ValidateAssignTeacher_ShouldFail_WhenRoleInvalid()
    {
        var request = new AssignTeacherRequestModel
        {
            TeacherId = 2,
            RoleInCourse = "Auditor"
        };

        var result = _policy.ValidateAssignTeacher(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("Role in course must be"));
    }
}
