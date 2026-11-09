using LittleTalk.Application.Features.Attendance;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Attendance;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class AttendanceValidationPolicyTests
{
    private readonly AttendanceValidationPolicy _policy = new();

    [Fact]
    public void ValidateCreateSession_ShouldFail_WhenCourseIdOrTeacherIdInvalid()
    {
        var request = new CreateClassSessionRequestModel
        {
            CourseId = 0,
            TeacherId = -1,
            SessionDate = DateOnly.FromDateTime(DateTime.Today)
        };

        var result = _policy.ValidateCreateSession(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
        Assert.Contains(result.Errors, e => e.Contains("Course"));
        Assert.Contains(result.Errors, e => e.Contains("Teacher"));
    }

    [Fact]
    public void ValidateCreateSession_ShouldFail_WhenStartTimeAfterEndTime()
    {
        var request = new CreateClassSessionRequestModel
        {
            CourseId = 1,
            TeacherId = 2,
            SessionDate = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(11, 0),
            EndTime = new TimeOnly(10, 0)
        };

        var result = _policy.ValidateCreateSession(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("Start time must be before end time"));
    }

    [Fact]
    public void ValidateCreateSession_ShouldSucceed_WhenValid()
    {
        var request = new CreateClassSessionRequestModel
        {
            CourseId = 1,
            TeacherId = 2,
            SessionDate = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 45),
            RoomOrLink = " Room 101 "
        };

        var result = _policy.ValidateCreateSession(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Room 101", request.RoomOrLink);
    }

    [Fact]
    public void ValidateBatchRecordAttendance_ShouldFail_WhenRecordsEmpty()
    {
        var request = new BatchRecordAttendanceRequestModel
        {
            Records = new List<StudentAttendanceRecordItemModel>()
        };

        var result = _policy.ValidateBatchAttendance(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("cannot be empty"));
    }

    [Fact]
    public void ValidateBatchRecordAttendance_ShouldFail_WhenStatusInvalid()
    {
        var request = new BatchRecordAttendanceRequestModel
        {
            Records = new List<StudentAttendanceRecordItemModel>
            {
                new() { StudentId = 1, Status = "UnknownStatus" }
            }
        };

        var result = _policy.ValidateBatchAttendance(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("UnknownStatus"));
    }

    [Fact]
    public void ValidateBatchRecordAttendance_ShouldSucceed_WhenValid()
    {
        var request = new BatchRecordAttendanceRequestModel
        {
            Records = new List<StudentAttendanceRecordItemModel>
            {
                new() { StudentId = 1, Status = "Present", Remarks = " Participated nicely " },
                new() { StudentId = 2, Status = "Late" }
            }
        };

        var result = _policy.ValidateBatchAttendance(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Participated nicely", request.Records[0].Remarks);
    }
}
