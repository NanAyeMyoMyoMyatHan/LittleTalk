using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Attendance;

namespace LittleTalk.Application.Features.Attendance.Services;

public interface IAttendanceService
{
    Task<Result<PagedListResponseModel<ClassSessionResponseModel>>> GetSessionsAsync(
        SearchClassSessionRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<ClassSessionDetailResponseModel>> GetSessionByIdAsync(
        int sessionId, 
        CancellationToken cancellationToken = default);

    Task<Result<ClassSessionDetailResponseModel>> CreateSessionAsync(
        CreateClassSessionRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<ClassSessionDetailResponseModel>> UpdateSessionAsync(
        int sessionId, 
        UpdateClassSessionRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteSessionAsync(
        int sessionId, 
        CancellationToken cancellationToken = default);

    Task<Result<ClassSessionDetailResponseModel>> BatchRecordAttendanceAsync(
        int sessionId, 
        BatchRecordAttendanceRequestModel request, 
        int recordedByUserId, 
        CancellationToken cancellationToken = default);

    Task<Result<List<StudentAttendanceHistoryResponseModel>>> GetStudentAttendanceHistoryAsync(
        int studentId, 
        CancellationToken cancellationToken = default);
}
