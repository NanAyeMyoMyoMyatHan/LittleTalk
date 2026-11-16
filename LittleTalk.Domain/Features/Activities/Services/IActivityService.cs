using LittleTalk.Shared;
using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Activities;

namespace LittleTalk.Domain.Features.Activities.Services;

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


