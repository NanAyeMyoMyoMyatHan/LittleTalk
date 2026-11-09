using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Assessments;

namespace LittleTalk.Client.Services.Features;

public interface IAssessmentApiClient
{
    Task<Result<PagedListResponseModel<AssessmentSummaryResponseModel>>> GetAssessmentsAsync(
        SearchAssessmentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<AssessmentDetailResponseModel>> GetAssessmentByIdAsync(
        int id, 
        CancellationToken cancellationToken = default);

    Task<Result<AssessmentDetailResponseModel>> CreateAssessmentAsync(
        CreateAssessmentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<AssessmentDetailResponseModel>> UpdateAssessmentAsync(
        int id, 
        UpdateAssessmentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAssessmentAsync(
        int id, 
        CancellationToken cancellationToken = default);

    Task<Result<List<AssessmentSummaryResponseModel>>> GetStudentAssessmentsAsync(
        int studentId, 
        CancellationToken cancellationToken = default);
}
