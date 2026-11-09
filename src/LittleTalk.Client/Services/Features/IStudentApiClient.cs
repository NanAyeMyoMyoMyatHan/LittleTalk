using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Students;

namespace LittleTalk.Client.Services.Features;

public interface IStudentApiClient
{
    Task<Result<PagedListResponseModel<StudentSummaryResponseModel>>?> GetStudentsAsync(SearchStudentRequestModel request);
    Task<Result<StudentDetailResponseModel>?> GetStudentByIdAsync(int id);
    Task<Result<StudentDetailResponseModel>?> CreateStudentAsync(CreateStudentRequestModel request);
    Task<Result<StudentDetailResponseModel>?> UpdateStudentAsync(int id, UpdateStudentRequestModel request);
    Task<Result?> DeleteStudentAsync(int id);
    Task<Result<StudentLearningHistoryResponseModel>> GetStudentLearningHistoryAsync(int id);
}
