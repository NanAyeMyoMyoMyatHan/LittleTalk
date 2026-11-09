using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Students;

namespace LittleTalk.Application.Features.Students.Services;

public interface IStudentService
{
    Task<Result<PagedListResponseModel<StudentSummaryResponseModel>>> GetStudentsAsync(
        SearchStudentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<StudentDetailResponseModel>> GetStudentByIdAsync(
        int studentId, 
        CancellationToken cancellationToken = default);

    Task<Result<StudentDetailResponseModel>> CreateStudentAsync(
        CreateStudentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<StudentDetailResponseModel>> UpdateStudentAsync(
        int studentId, 
        UpdateStudentRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteStudentAsync(
        int studentId, 
        CancellationToken cancellationToken = default);

    Task<Result<StudentLearningHistoryResponseModel>> GetStudentLearningHistoryAsync(
        int studentId, 
        CancellationToken cancellationToken = default);
}
