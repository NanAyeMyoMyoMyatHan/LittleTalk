using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Students;

namespace LittleTalk.Client.Services.Features;

public class StudentApiClient : BaseFeatureApiClient, IStudentApiClient
{
    public StudentApiClient(HttpClient http) : base(http)
    {
    }

    public async Task<Result<PagedListResponseModel<StudentSummaryResponseModel>>?> GetStudentsAsync(SearchStudentRequestModel request)
    {
        var query = $"api/students?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";
        if (request.LevelId.HasValue)
            query += $"&levelId={request.LevelId.Value}";
        if (request.IsActive.HasValue)
            query += $"&isActive={request.IsActive.Value}";
        if (!string.IsNullOrWhiteSpace(request.SortBy))
            query += $"&sortBy={Uri.EscapeDataString(request.SortBy)}&sortDescending={request.SortDescending}";

        return await SendGetAsync<PagedListResponseModel<StudentSummaryResponseModel>>(query);
    }

    public async Task<Result<StudentDetailResponseModel>?> GetStudentByIdAsync(int id)
    {
        return await SendGetAsync<StudentDetailResponseModel>($"api/students/{id}");
    }

    public async Task<Result<StudentDetailResponseModel>?> CreateStudentAsync(CreateStudentRequestModel request)
    {
        return await SendPostAsync<StudentDetailResponseModel, CreateStudentRequestModel>("api/students", request);
    }

    public async Task<Result<StudentDetailResponseModel>?> UpdateStudentAsync(int id, UpdateStudentRequestModel request)
    {
        return await SendPutAsync<StudentDetailResponseModel, UpdateStudentRequestModel>($"api/students/{id}", request);
    }

    public async Task<Result?> DeleteStudentAsync(int id)
    {
        return await SendDeleteAsync($"api/students/{id}");
    }

    public async Task<Result<StudentLearningHistoryResponseModel>> GetStudentLearningHistoryAsync(int id)
    {
        return await SendGetAsync<StudentLearningHistoryResponseModel>($"api/students/{id}/history");
    }
}
