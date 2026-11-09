using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Client.Services.Features;

public interface ILessonPlanApiClient
{
    Task<Result<PagedListResponseModel<LessonPlanSummaryResponseModel>>?> GetLessonPlansAsync(SearchLessonPlanRequestModel request);
    Task<Result<LessonPlanDetailResponseModel>?> GetLessonPlanByIdAsync(int id);
    Task<Result<LessonPlanDetailResponseModel>?> CreateLessonPlanAsync(CreateLessonPlanRequestModel request);
    Task<Result<LessonPlanDetailResponseModel>?> UpdateLessonPlanAsync(int id, UpdateLessonPlanRequestModel request);
    Task<Result?> DeleteLessonPlanAsync(int id);
    Task<Result<LessonPlanDetailResponseModel>?> DuplicateLessonPlanAsync(int id, DuplicateLessonPlanRequestModel request);
    Task<Result<LessonStepResponseModel>?> AddStepAsync(int id, CreateLessonStepRequestModel request);
    Task<Result<LessonStepResponseModel>?> UpdateStepAsync(int id, int stepId, UpdateLessonStepRequestModel request);
    Task<Result?> DeleteStepAsync(int id, int stepId);
    Task<Result<LessonVocabularyResponseModel>?> AddVocabularyAsync(int id, AddLessonVocabularyRequestModel request);
    Task<Result?> DeleteVocabularyAsync(int id, int vocabId);
    Task<Result<LessonPlanMaterialResponseModel>?> LinkMaterialAsync(int id, LinkMaterialRequestModel request);
    Task<Result?> UnlinkMaterialAsync(int id, int materialId);
    Task<Result<LessonActivityResponseModel>?> LinkActivityAsync(int id, LinkActivityRequestModel request);
    Task<Result?> UnlinkActivityAsync(int id, int activityId);
}
