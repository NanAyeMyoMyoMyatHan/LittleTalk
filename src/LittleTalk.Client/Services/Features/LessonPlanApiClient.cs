using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Client.Services.Features;

public class LessonPlanApiClient : BaseFeatureApiClient, ILessonPlanApiClient
{
    public LessonPlanApiClient(HttpClient http) : base(http)
    {
    }

    public async Task<Result<PagedListResponseModel<LessonPlanSummaryResponseModel>>?> GetLessonPlansAsync(SearchLessonPlanRequestModel request)
    {
        var query = $"api/lesson-plans?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";
        if (request.CourseId.HasValue)
            query += $"&courseId={request.CourseId.Value}";
        var levelId = request.LearningLevelId ?? request.LevelId;
        if (levelId.HasValue)
            query += $"&learningLevelId={levelId.Value}";
        if (!string.IsNullOrWhiteSpace(request.Status))
            query += $"&status={Uri.EscapeDataString(request.Status)}";

        return await SendGetAsync<PagedListResponseModel<LessonPlanSummaryResponseModel>>(query);
    }

    public async Task<Result<LessonPlanDetailResponseModel>?> GetLessonPlanByIdAsync(int id)
    {
        return await SendGetAsync<LessonPlanDetailResponseModel>($"api/lesson-plans/{id}");
    }

    public async Task<Result<LessonPlanDetailResponseModel>?> CreateLessonPlanAsync(CreateLessonPlanRequestModel request)
    {
        return await SendPostAsync<LessonPlanDetailResponseModel, CreateLessonPlanRequestModel>("api/lesson-plans", request);
    }

    public async Task<Result<LessonPlanDetailResponseModel>?> UpdateLessonPlanAsync(int id, UpdateLessonPlanRequestModel request)
    {
        return await SendPutAsync<LessonPlanDetailResponseModel, UpdateLessonPlanRequestModel>($"api/lesson-plans/{id}", request);
    }

    public async Task<Result?> DeleteLessonPlanAsync(int id)
    {
        return await SendDeleteAsync($"api/lesson-plans/{id}");
    }

    public async Task<Result<LessonPlanDetailResponseModel>?> DuplicateLessonPlanAsync(int id, DuplicateLessonPlanRequestModel request)
    {
        return await SendPostAsync<LessonPlanDetailResponseModel, DuplicateLessonPlanRequestModel>($"api/lesson-plans/{id}/duplicate", request);
    }

    public async Task<Result<LessonStepResponseModel>?> AddStepAsync(int id, CreateLessonStepRequestModel request)
    {
        return await SendPostAsync<LessonStepResponseModel, CreateLessonStepRequestModel>($"api/lesson-plans/{id}/steps", request);
    }

    public async Task<Result<LessonStepResponseModel>?> UpdateStepAsync(int id, int stepId, UpdateLessonStepRequestModel request)
    {
        return await SendPutAsync<LessonStepResponseModel, UpdateLessonStepRequestModel>($"api/lesson-plans/{id}/steps/{stepId}", request);
    }

    public async Task<Result?> DeleteStepAsync(int id, int stepId)
    {
        return await SendDeleteAsync($"api/lesson-plans/{id}/steps/{stepId}");
    }

    public async Task<Result<LessonVocabularyResponseModel>?> AddVocabularyAsync(int id, AddLessonVocabularyRequestModel request)
    {
        return await SendPostAsync<LessonVocabularyResponseModel, AddLessonVocabularyRequestModel>($"api/lesson-plans/{id}/vocabulary", request);
    }

    public async Task<Result?> DeleteVocabularyAsync(int id, int vocabId)
    {
        return await SendDeleteAsync($"api/lesson-plans/{id}/vocabulary/{vocabId}");
    }

    public async Task<Result<LessonPlanMaterialResponseModel>?> LinkMaterialAsync(int id, LinkMaterialRequestModel request)
    {
        return await SendPostAsync<LessonPlanMaterialResponseModel, LinkMaterialRequestModel>($"api/lesson-plans/{id}/materials", request);
    }

    public async Task<Result?> UnlinkMaterialAsync(int id, int materialId)
    {
        return await SendDeleteAsync($"api/lesson-plans/{id}/materials/{materialId}");
    }

    public async Task<Result<LessonActivityResponseModel>?> LinkActivityAsync(int id, LinkActivityRequestModel request)
    {
        return await SendPostAsync<LessonActivityResponseModel, LinkActivityRequestModel>($"api/lesson-plans/{id}/activities", request);
    }

    public async Task<Result?> UnlinkActivityAsync(int id, int activityId)
    {
        return await SendDeleteAsync($"api/lesson-plans/{id}/activities/{activityId}");
    }
}
