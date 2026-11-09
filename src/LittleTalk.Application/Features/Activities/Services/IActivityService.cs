using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Activities;

namespace LittleTalk.Application.Features.Activities.Services;

public interface IActivityService
{
    Task<Result<PagedListResponseModel<ActivitySummaryResponseModel>>> GetActivitiesAsync(
        SearchActivityRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result<ActivityDetailResponseModel>> GetActivityByIdAsync(
        int activityId, 
        CancellationToken cancellationToken = default);

    Task<Result<ActivityDetailResponseModel>> CreateActivityAsync(
        CreateActivityRequestModel request, 
        int currentUserId, 
        CancellationToken cancellationToken = default);

    Task<Result<ActivityDetailResponseModel>> UpdateActivityAsync(
        int activityId, 
        UpdateActivityRequestModel request, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteActivityAsync(
        int activityId, 
        CancellationToken cancellationToken = default);

    Task<Result<List<ActivityCategoryResponseModel>>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);
}
