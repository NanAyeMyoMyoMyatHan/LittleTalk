using Microsoft.EntityFrameworkCore;
using LittleTalk.Application.Features.Activities;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Activities;

namespace LittleTalk.Application.Features.Activities.Services;

public sealed class ActivityService : IActivityService
{
    private readonly LittleTalkDbContext _context;
    private readonly ActivityValidationPolicy _validationPolicy;

    public ActivityService(
        LittleTalkDbContext context,
        ActivityValidationPolicy validationPolicy)
    {
        _context = context;
        _validationPolicy = validationPolicy;
    }

    public async Task<Result<PagedListResponseModel<ActivitySummaryResponseModel>>> GetActivitiesAsync(
        SearchActivityRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Activities
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.CreatedByNavigation)
            .AsQueryable();

        if (request.IsActive.HasValue) 
            query = query.Where(a => a.IsActive == request.IsActive.Value);

        if (request.CategoryId.HasValue && request.CategoryId.Value > 0) 
            query = query.Where(a => a.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.Difficulty)) 
            query = query.Where(a => a.DifficultyLevel.ToLower() == request.Difficulty.Trim().ToLower());

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            query = query.Where(a => a.ActivityName.ToLower().Contains(search) || a.Description.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.CreatedDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ActivitySummaryResponseModel
            {
                ActivityId = a.ActivityId,
                CategoryId = a.CategoryId,
                CategoryName = a.Category.CategoryName,
                ActivityName = a.ActivityName,
                Description = a.Description,
                SuitableAgeGroup = a.SuitableAgeGroup,
                DifficultyLevel = a.DifficultyLevel,
                DurationMinutes = a.DurationMinutes,
                ExpectedLearningOutcome = a.ExpectedLearningOutcome,
                CreatedByName = a.CreatedByNavigation.FullName,
                IsActive = a.IsActive,
                CreatedDate = a.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedList = new PagedListResponseModel<ActivitySummaryResponseModel>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedListResponseModel<ActivitySummaryResponseModel>>.Success(pagedList, "Activities retrieved successfully.");
    }

    public async Task<Result<ActivityDetailResponseModel>> GetActivityByIdAsync(
        int activityId, 
        CancellationToken cancellationToken = default)
    {
        if (activityId <= 0)
            return Result<ActivityDetailResponseModel>.ValidationError("Valid Activity ID is required.");

        var a = await _context.Activities
            .AsNoTracking()
            .Include(act => act.Category)
            .Include(act => act.CreatedByNavigation)
            .FirstOrDefaultAsync(act => act.ActivityId == activityId, cancellationToken);

        if (a == null)
            return Result<ActivityDetailResponseModel>.NotFound($"Activity with ID ({activityId}) was not found.");

        var response = new ActivityDetailResponseModel
        {
            ActivityId = a.ActivityId,
            CategoryId = a.CategoryId,
            CategoryName = a.Category.CategoryName,
            ActivityName = a.ActivityName,
            Description = a.Description,
            Instructions = a.Instructions,
            SuitableAgeGroup = a.SuitableAgeGroup,
            DifficultyLevel = a.DifficultyLevel,
            DurationMinutes = a.DurationMinutes,
            RequiredMaterials = a.RequiredMaterials,
            ExpectedLearningOutcome = a.ExpectedLearningOutcome,
            CreatedByName = a.CreatedByNavigation.FullName,
            IsActive = a.IsActive,
            CreatedDate = a.CreatedDate,
            UpdatedDate = a.UpdatedDate
        };

        return Result<ActivityDetailResponseModel>.Success(response, "Activity details retrieved successfully.");
    }

    public async Task<Result<ActivityDetailResponseModel>> CreateActivityAsync(
        CreateActivityRequestModel request, 
        int currentUserId, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateCreate(request);
        if (!validation.IsSuccess)
            return Result<ActivityDetailResponseModel>.ValidationError(validation.Errors, validation.Message);

        var categoryExists = await _context.ActivityCategories
            .AnyAsync(c => c.CategoryId == request.CategoryId, cancellationToken);
        if (!categoryExists)
            return Result<ActivityDetailResponseModel>.NotFound($"Activity Category with ID ({request.CategoryId}) was not found.");

        var activity = new Activity
        {
            CategoryId = request.CategoryId,
            ActivityName = request.ActivityName,
            Description = request.Description,
            Instructions = request.Instructions,
            SuitableAgeGroup = request.SuitableAgeGroup,
            DifficultyLevel = request.DifficultyLevel,
            DurationMinutes = request.DurationMinutes,
            RequiredMaterials = request.RequiredMaterials,
            ExpectedLearningOutcome = request.ExpectedLearningOutcome,
            CreatedBy = currentUserId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetActivityByIdAsync(activity.ActivityId, cancellationToken);
    }

    public async Task<Result<ActivityDetailResponseModel>> UpdateActivityAsync(
        int activityId, 
        UpdateActivityRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        if (activityId <= 0)
            return Result<ActivityDetailResponseModel>.ValidationError("Valid Activity ID is required.");

        var validation = _validationPolicy.ValidateUpdate(request);
        if (!validation.IsSuccess)
            return Result<ActivityDetailResponseModel>.ValidationError(validation.Errors, validation.Message);

        var activity = await _context.Activities.FindAsync(new object[] { activityId }, cancellationToken);
        if (activity == null)
            return Result<ActivityDetailResponseModel>.NotFound($"Activity with ID ({activityId}) was not found.");

        var categoryExists = await _context.ActivityCategories
            .AnyAsync(c => c.CategoryId == request.CategoryId, cancellationToken);
        if (!categoryExists)
            return Result<ActivityDetailResponseModel>.NotFound($"Activity Category with ID ({request.CategoryId}) was not found.");

        activity.CategoryId = request.CategoryId;
        activity.ActivityName = request.ActivityName;
        activity.Description = request.Description;
        activity.Instructions = request.Instructions;
        activity.SuitableAgeGroup = request.SuitableAgeGroup;
        activity.DifficultyLevel = request.DifficultyLevel;
        activity.DurationMinutes = request.DurationMinutes;
        activity.RequiredMaterials = request.RequiredMaterials;
        activity.ExpectedLearningOutcome = request.ExpectedLearningOutcome;
        activity.IsActive = request.IsActive;
        activity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetActivityByIdAsync(activityId, cancellationToken);
    }

    public async Task<Result> DeleteActivityAsync(
        int activityId, 
        CancellationToken cancellationToken = default)
    {
        if (activityId <= 0)
            return Result.ValidationError("Valid Activity ID is required.");

        var activity = await _context.Activities.FindAsync(new object[] { activityId }, cancellationToken);
        if (activity == null)
            return Result.NotFound($"Activity with ID ({activityId}) was not found.");

        activity.IsActive = false;
        activity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success($"Activity '{activity.ActivityName}' deactivated successfully.");
    }

    public async Task<Result<List<ActivityCategoryResponseModel>>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.ActivityCategories
            .AsNoTracking()
            .Select(c => new ActivityCategoryResponseModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                Icon = c.Icon,
                ActivitiesCount = c.Activities.Count(a => a.IsActive)
            })
            .ToListAsync(cancellationToken);

        return Result<List<ActivityCategoryResponseModel>>.Success(items, "Activity categories retrieved successfully.");
    }
}
