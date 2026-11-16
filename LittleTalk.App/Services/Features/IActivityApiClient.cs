using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Activities;

namespace LittleTalk.App.Services.Features;

public interface IActivityApiClient
{
    Task<Result<PagedListResponseModel<ActivitySummaryResponseModel>>?> GetActivitiesAsync(SearchActivityRequestModel request);
    Task<Result<List<ActivityCategoryResponseModel>>?> GetCategoriesAsync();
    Task<Result<ActivityDetailResponseModel>?> GetActivityByIdAsync(int id);
    Task<Result<ActivityDetailResponseModel>?> CreateActivityAsync(CreateActivityRequestModel request);
    Task<Result<ActivityDetailResponseModel>?> UpdateActivityAsync(int id, UpdateActivityRequestModel request);
    Task<Result?> DeleteActivityAsync(int id);
}

