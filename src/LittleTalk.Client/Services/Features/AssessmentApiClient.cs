using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Assessments;

namespace LittleTalk.Client.Services.Features;

public class AssessmentApiClient : BaseFeatureApiClient, IAssessmentApiClient
{
    public AssessmentApiClient(HttpClient http, CustomAuthenticationStateProvider authStateProvider) 
        : base(http, authStateProvider)
    {
    }

    public async Task<Result<PagedListResponseModel<AssessmentSummaryResponseModel>>> GetAssessmentsAsync(
        SearchAssessmentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var query = $"api/assessments?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        if (request.StudentId.HasValue)
            query += $"&studentId={request.StudentId.Value}";
        if (request.SessionId.HasValue)
            query += $"&sessionId={request.SessionId.Value}";
        if (!string.IsNullOrWhiteSpace(request.Status))
            query += $"&status={Uri.EscapeDataString(request.Status)}";
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";

        return (await SendGetAsync<PagedListResponseModel<AssessmentSummaryResponseModel>>(query))
               ?? Result<PagedListResponseModel<AssessmentSummaryResponseModel>>.SystemError("No response received from server.");
    }

    public async Task<Result<AssessmentDetailResponseModel>> GetAssessmentByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<AssessmentDetailResponseModel>($"api/assessments/{id}"))
               ?? Result<AssessmentDetailResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result<AssessmentDetailResponseModel>> CreateAssessmentAsync(
        CreateAssessmentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        return (await SendPostAsync<AssessmentDetailResponseModel, CreateAssessmentRequestModel>("api/assessments", request))
               ?? Result<AssessmentDetailResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result<AssessmentDetailResponseModel>> UpdateAssessmentAsync(
        int id, 
        UpdateAssessmentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        return (await SendPutAsync<AssessmentDetailResponseModel, UpdateAssessmentRequestModel>($"api/assessments/{id}", request))
               ?? Result<AssessmentDetailResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result> DeleteAssessmentAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        return (await SendDeleteAsync($"api/assessments/{id}"))
               ?? Result.SystemError("No response received from server.");
    }

    public async Task<Result<List<AssessmentSummaryResponseModel>>> GetStudentAssessmentsAsync(
        int studentId, 
        CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<List<AssessmentSummaryResponseModel>>($"api/assessments/student/{studentId}"))
               ?? Result<List<AssessmentSummaryResponseModel>>.SystemError("No response received from server.");
    }
}
