using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Application.Features.LessonPlans.Services;

public interface ILessonPlanService
{
    Task<Result<PagedListResponseModel<LessonPlanSummaryResponseModel>>> GetLessonPlansAsync(
        SearchLessonPlanRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<LessonPlanDetailResponseModel>> GetLessonPlanByIdAsync(
        int lessonPlanId, 
        CancellationToken cancellationToken = default);

    Task<Result<LessonPlanDetailResponseModel>> CreateLessonPlanAsync(
        CreateLessonPlanRequestModel request, 
        int currentUserId, 
        CancellationToken cancellationToken = default);

    Task<Result<LessonPlanDetailResponseModel>> UpdateLessonPlanAsync(
        int lessonPlanId, 
        UpdateLessonPlanRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteLessonPlanAsync(
        int lessonPlanId, 
        CancellationToken cancellationToken = default);

    Task<Result<LessonPlanDetailResponseModel>> DuplicateLessonPlanAsync(
        int lessonPlanId, 
        DuplicateLessonPlanRequestModel request, 
        int currentUserId, 
        CancellationToken cancellationToken = default);

    // Steps
    Task<Result<LessonStepResponseModel>> AddStepAsync(
        int lessonPlanId, 
        CreateLessonStepRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<LessonStepResponseModel>> UpdateStepAsync(
        int lessonPlanId, 
        int stepId, 
        UpdateLessonStepRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteStepAsync(
        int lessonPlanId, 
        int stepId, 
        CancellationToken cancellationToken = default);

    // Vocabulary
    Task<Result<LessonVocabularyResponseModel>> AddVocabularyAsync(
        int lessonPlanId, 
        AddLessonVocabularyRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteVocabularyAsync(
        int lessonPlanId, 
        int vocabularyId, 
        CancellationToken cancellationToken = default);

    // Materials Link
    Task<Result<LessonPlanMaterialResponseModel>> LinkMaterialAsync(
        int lessonPlanId, 
        LinkMaterialRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> UnlinkMaterialAsync(
        int lessonPlanId, 
        int materialId, 
        CancellationToken cancellationToken = default);

    // Activities Link
    Task<Result<LessonActivityResponseModel>> LinkActivityAsync(
        int lessonPlanId, 
        LinkActivityRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> UnlinkActivityAsync(
        int lessonPlanId, 
        int activityId, 
        CancellationToken cancellationToken = default);
}
