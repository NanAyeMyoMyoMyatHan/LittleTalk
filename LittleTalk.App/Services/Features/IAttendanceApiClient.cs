using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Attendance;

namespace LittleTalk.App.Services.Features;

public interface IAttendanceApiClient
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
        CancellationToken cancellationToken = default);

    Task<Result<List<StudentAttendanceHistoryResponseModel>>> GetStudentAttendanceHistoryAsync(
        int studentId, 
        CancellationToken cancellationToken = default);
}

