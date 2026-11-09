using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Activities;

namespace LittleTalk.Client.Services.Features;

public interface IActivityApiClient
{
    Task<Result<PagedListResponseModel<ActivitySummaryResponseModel>>?> GetActivitiesAsync(SearchActivityRequestModel request);
    Task<Result<List<ActivityCategoryResponseModel>>?> GetCategoriesAsync();
    Task<Result<ActivityDetailResponseModel>?> GetActivityByIdAsync(int id);
    Task<Result<ActivityDetailResponseModel>?> CreateActivityAsync(CreateActivityRequestModel request);
    Task<Result<ActivityDetailResponseModel>?> UpdateActivityAsync(int id, UpdateActivityRequestModel request);
    Task<Result?> DeleteActivityAsync(int id);
}
