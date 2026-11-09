using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Assessments;

namespace LittleTalk.Application.Features.Assessments.Services;

public interface IAssessmentService
{
    Task<Result<PagedListResponseModel<AssessmentSummaryResponseModel>>> GetAssessmentsAsync(
        SearchAssessmentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<AssessmentDetailResponseModel>> GetAssessmentByIdAsync(
        int assessmentId, 
        CancellationToken cancellationToken = default);

    Task<Result<AssessmentDetailResponseModel>> CreateAssessmentAsync(
        CreateAssessmentRequestModel request, 
        int evaluatorTeacherId, 
        CancellationToken cancellationToken = default);

    Task<Result<AssessmentDetailResponseModel>> UpdateAssessmentAsync(
        int assessmentId, 
        UpdateAssessmentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAssessmentAsync(
        int assessmentId, 
        CancellationToken cancellationToken = default);

    Task<Result<List<AssessmentSummaryResponseModel>>> GetStudentAssessmentsAsync(
        int studentId, 
        CancellationToken cancellationToken = default);
}
