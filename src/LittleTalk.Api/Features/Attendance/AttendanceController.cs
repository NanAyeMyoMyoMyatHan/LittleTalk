using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Api.Common;
using LittleTalk.Application.Features.Attendance.Services;
using LittleTalk.Infrastructure.Services;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Attendance;

namespace LittleTalk.Api.Features.Attendance;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class AttendanceController : BaseApiController
{
    private readonly IAttendanceService _attendanceService;
    private readonly ICurrentUserService _currentUserService;

    public AttendanceController(IAttendanceService attendanceService, ICurrentUserService currentUserService)
    {
        _attendanceService = attendanceService;
        _currentUserService = currentUserService;
    }

    [HttpGet("sessions")]
    [ProducesResponseType(typeof(Result<PagedListResponseModel<ClassSessionResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessions(
        [FromQuery] SearchClassSessionRequestModel request, 
        CancellationToken cancellationToken)
    {
        var result = await _attendanceService.GetSessionsAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("sessions/{sessionId:int}")]
    [ProducesResponseType(typeof(Result<ClassSessionDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionById(int sessionId, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.GetSessionByIdAsync(sessionId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("sessions")]
    [ProducesResponseType(typeof(Result<ClassSessionDetailResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateSession([FromBody] CreateClassSessionRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.CreateSessionAsync(request, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetSessionById), new { sessionId = result.Data?.SessionId });
    }

    [HttpPut("sessions/{sessionId:int}")]
    [ProducesResponseType(typeof(Result<ClassSessionDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSession(int sessionId, [FromBody] UpdateClassSessionRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.UpdateSessionAsync(sessionId, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("sessions/{sessionId:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSession(int sessionId, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.DeleteSessionAsync(sessionId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("sessions/{sessionId:int}/record")]
    [ProducesResponseType(typeof(Result<ClassSessionDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordAttendance(int sessionId, [FromBody] BatchRecordAttendanceRequestModel request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? 1;
        var result = await _attendanceService.BatchRecordAttendanceAsync(sessionId, request, userId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("student/{studentId:int}")]
    [ProducesResponseType(typeof(Result<List<StudentAttendanceHistoryResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentAttendanceHistory(int studentId, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.GetStudentAttendanceHistoryAsync(studentId, cancellationToken);
        return ToActionResult(result);
    }
}
