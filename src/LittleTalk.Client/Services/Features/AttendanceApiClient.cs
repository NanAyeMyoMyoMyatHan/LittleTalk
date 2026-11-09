using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Attendance;

namespace LittleTalk.Client.Services.Features;

public class AttendanceApiClient : BaseFeatureApiClient, IAttendanceApiClient
{
    public AttendanceApiClient(HttpClient http, CustomAuthenticationStateProvider authStateProvider) 
        : base(http, authStateProvider)
    {
    }

    public async Task<Result<PagedListResponseModel<ClassSessionResponseModel>>> GetSessionsAsync(
        SearchClassSessionRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var query = $"api/attendance/sessions?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        if (request.CourseId.HasValue)
            query += $"&courseId={request.CourseId.Value}";
        if (request.FromDate.HasValue)
            query += $"&fromDate={request.FromDate.Value:yyyy-MM-dd}";
        if (request.ToDate.HasValue)
            query += $"&toDate={request.ToDate.Value:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(request.Status))
            query += $"&status={Uri.EscapeDataString(request.Status)}";

        return (await SendGetAsync<PagedListResponseModel<ClassSessionResponseModel>>(query)) 
               ?? Result<PagedListResponseModel<ClassSessionResponseModel>>.SystemError("No response received from server.");
    }

    public async Task<Result<ClassSessionDetailResponseModel>> GetSessionByIdAsync(
        int sessionId, 
        CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<ClassSessionDetailResponseModel>($"api/attendance/sessions/{sessionId}"))
               ?? Result<ClassSessionDetailResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result<ClassSessionDetailResponseModel>> CreateSessionAsync(
        CreateClassSessionRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        return (await SendPostAsync<ClassSessionDetailResponseModel, CreateClassSessionRequestModel>("api/attendance/sessions", request))
               ?? Result<ClassSessionDetailResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result<ClassSessionDetailResponseModel>> UpdateSessionAsync(
        int sessionId, 
        UpdateClassSessionRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        return (await SendPutAsync<ClassSessionDetailResponseModel, UpdateClassSessionRequestModel>($"api/attendance/sessions/{sessionId}", request))
               ?? Result<ClassSessionDetailResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result> DeleteSessionAsync(
        int sessionId, 
        CancellationToken cancellationToken = default)
    {
        return (await SendDeleteAsync($"api/attendance/sessions/{sessionId}"))
               ?? Result.SystemError("No response received from server.");
    }

    public async Task<Result<ClassSessionDetailResponseModel>> BatchRecordAttendanceAsync(
        int sessionId, 
        BatchRecordAttendanceRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        return (await SendPostAsync<ClassSessionDetailResponseModel, BatchRecordAttendanceRequestModel>($"api/attendance/sessions/{sessionId}/record", request))
               ?? Result<ClassSessionDetailResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result<List<StudentAttendanceHistoryResponseModel>>> GetStudentAttendanceHistoryAsync(
        int studentId, 
        CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<List<StudentAttendanceHistoryResponseModel>>($"api/attendance/student/{studentId}"))
               ?? Result<List<StudentAttendanceHistoryResponseModel>>.SystemError("No response received from server.");
    }
}
