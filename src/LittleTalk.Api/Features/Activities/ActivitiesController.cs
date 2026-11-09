using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Api.Common;
using LittleTalk.Application.Features.Activities.Services;
using LittleTalk.Infrastructure.Services;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Activities;

namespace LittleTalk.Api.Features.Activities;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class ActivitiesController : BaseApiController
{
    private readonly IActivityService _activityService;
    private readonly ICurrentUserService _currentUserService;

    public ActivitiesController(IActivityService activityService, ICurrentUserService currentUserService)
    {
        _activityService = activityService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedListResponseModel<ActivitySummaryResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivities(
        [FromQuery] SearchActivityRequestModel request, 
        CancellationToken cancellationToken)
    {
        var result = await _activityService.GetActivitiesAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(Result<List<ActivityCategoryResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _activityService.GetCategoriesAsync(cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<ActivityDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _activityService.GetActivityByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<ActivityDetailResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateActivityRequestModel request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? 1;
        var result = await _activityService.CreateActivityAsync(request, userId, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetById), new { id = result.Data?.ActivityId });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result<ActivityDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateActivityRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _activityService.UpdateActivityAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _activityService.DeleteActivityAsync(id, cancellationToken);
        return ToActionResult(result);
    }
}
