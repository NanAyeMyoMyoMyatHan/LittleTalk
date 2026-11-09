using LittleTalk.Application.Features.Auth;
using LittleTalk.Application.Features.Students;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Auth;
using LittleTalk.Model.Features.Students;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class ValidationPolicyTests
{
    private readonly StudentValidationPolicy _studentPolicy = new();
    private readonly AuthValidationPolicy _authPolicy = new();

    [Fact]
    public void StudentValidationPolicy_ShouldFail_WhenFullNameIsEmpty()
    {
        var request = new CreateStudentRequestModel
        {
            FullName = "   ",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-6)),
            LearningLevelId = 1
        };

        var result = _studentPolicy.ValidateCreate(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
    }

    [Fact]
    public void StudentValidationPolicy_ShouldFail_WhenAgeUnderTwo()
    {
        var request = new CreateStudentRequestModel
        {
            FullName = "Baby John",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)),
            LearningLevelId = 1
        };

        var result = _studentPolicy.ValidateCreate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("at least 2 years old"));
    }

    [Fact]
    public void StudentValidationPolicy_ShouldSucceed_WhenValid()
    {
        var request = new CreateStudentRequestModel
        {
            FullName = "  Emma Watson  ",
            NickName = " Emmy ",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-6)),
            Gender = "Girl",
            LearningLevelId = 1
        };

        var result = _studentPolicy.ValidateCreate(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Emma Watson", request.FullName);
        Assert.Equal("Emmy", request.NickName);
    }

    [Fact]
    public void AuthValidationPolicy_ShouldFail_WhenPasswordTooShort()
    {
        var request = new LoginRequestModel
        {
            UsernameOrEmail = "admin",
            Password = "123"
        };

        var result = _authPolicy.ValidateLogin(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
        Assert.Contains("at least 6 characters", result.Message);
    }

    [Fact]
    public void AuthValidationPolicy_ShouldSucceed_WhenValid()
    {
        var request = new LoginRequestModel
        {
            UsernameOrEmail = " admin@littletalk.com ",
            Password = "SecurePassword123!"
        };

        var result = _authPolicy.ValidateLogin(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("admin@littletalk.com", request.UsernameOrEmail);
    }
}
