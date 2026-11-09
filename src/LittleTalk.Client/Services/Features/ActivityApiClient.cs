using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Activities;

namespace LittleTalk.Client.Services.Features;

public class ActivityApiClient : BaseFeatureApiClient, IActivityApiClient
{
    public ActivityApiClient(HttpClient http, CustomAuthenticationStateProvider authStateProvider) 
        : base(http, authStateProvider)
    {
    }

    public async Task<Result<PagedListResponseModel<ActivitySummaryResponseModel>>?> GetActivitiesAsync(SearchActivityRequestModel request)
    {
        var query = $"api/activities?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";
        if (request.CategoryId.HasValue)
            query += $"&categoryId={request.CategoryId.Value}";
        if (!string.IsNullOrWhiteSpace(request.Difficulty))
            query += $"&difficulty={Uri.EscapeDataString(request.Difficulty)}";
        if (request.IsActive.HasValue)
            query += $"&isActive={request.IsActive.Value}";

        return await SendGetAsync<PagedListResponseModel<ActivitySummaryResponseModel>>(query);
    }

    public async Task<Result<List<ActivityCategoryResponseModel>>?> GetCategoriesAsync()
    {
        return await SendGetAsync<List<ActivityCategoryResponseModel>>("api/activities/categories");
    }

    public async Task<Result<ActivityDetailResponseModel>?> GetActivityByIdAsync(int id)
    {
        return await SendGetAsync<ActivityDetailResponseModel>($"api/activities/{id}");
    }

    public async Task<Result<ActivityDetailResponseModel>?> CreateActivityAsync(CreateActivityRequestModel request)
    {
        return await SendPostAsync<ActivityDetailResponseModel, CreateActivityRequestModel>("api/activities", request);
    }

    public async Task<Result<ActivityDetailResponseModel>?> UpdateActivityAsync(int id, UpdateActivityRequestModel request)
    {
        return await SendPutAsync<ActivityDetailResponseModel, UpdateActivityRequestModel>($"api/activities/{id}", request);
    }

    public async Task<Result?> DeleteActivityAsync(int id)
    {
        return await SendDeleteAsync($"api/activities/{id}");
    }
}
