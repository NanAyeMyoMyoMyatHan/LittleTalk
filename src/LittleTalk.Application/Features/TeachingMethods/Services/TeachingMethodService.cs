using Microsoft.EntityFrameworkCore;
using LittleTalk.Application.Features.TeachingMethods;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.TeachingMethods;

namespace LittleTalk.Application.Features.TeachingMethods.Services;

public sealed class TeachingMethodService : ITeachingMethodService
{
    private readonly LittleTalkDbContext _context;
    private readonly TeachingMethodValidationPolicy _validationPolicy;

    public TeachingMethodService(
        LittleTalkDbContext context,
        TeachingMethodValidationPolicy validationPolicy)
    {
        _context = context;
        _validationPolicy = validationPolicy;
    }

    public async Task<Result<List<TeachingMethodResponseModel>>> GetAllMethodsAsync(
        bool? isActive = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.TeachingMethods
            .AsNoTracking()
            .Include(tm => tm.SuitableLearningLevel)
            .AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(tm => tm.IsActive == isActive.Value);
        }

        var items = await query
            .OrderBy(tm => tm.Name)
            .Select(tm => new TeachingMethodResponseModel
            {
                TeachingMethodId = tm.TeachingMethodId,
                Name = tm.Name,
                Description = tm.Description,
                TeachingInstructions = tm.TeachingInstructions,
                SuitableAgeGroup = tm.SuitableAgeGroup,
                SuitableLearningLevelId = tm.SuitableLearningLevelId,
                SuitableLearningLevelName = tm.SuitableLearningLevel != null ? tm.SuitableLearningLevel.LevelName : null,
                RequiredMaterials = tm.RequiredMaterials,
                IsActive = tm.IsActive,
                CreatedDate = tm.CreatedDate,
                UpdatedDate = tm.UpdatedDate
            })
            .ToListAsync(cancellationToken);

        return Result<List<TeachingMethodResponseModel>>.Success(items, "Teaching methods retrieved successfully.");
    }

    public async Task<Result<TeachingMethodResponseModel>> GetMethodByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<TeachingMethodResponseModel>.ValidationError("Valid Teaching Method ID is required.");
        }

        var tm = await _context.TeachingMethods
            .AsNoTracking()
            .Include(t => t.SuitableLearningLevel)
            .FirstOrDefaultAsync(t => t.TeachingMethodId == id, cancellationToken);

        if (tm == null)
        {
            return Result<TeachingMethodResponseModel>.NotFound($"Teaching Method with ID ({id}) was not found.");
        }

        var response = new TeachingMethodResponseModel
        {
            TeachingMethodId = tm.TeachingMethodId,
            Name = tm.Name,
            Description = tm.Description,
            TeachingInstructions = tm.TeachingInstructions,
            SuitableAgeGroup = tm.SuitableAgeGroup,
            SuitableLearningLevelId = tm.SuitableLearningLevelId,
            SuitableLearningLevelName = tm.SuitableLearningLevel != null ? tm.SuitableLearningLevel.LevelName : null,
            RequiredMaterials = tm.RequiredMaterials,
            IsActive = tm.IsActive,
            CreatedDate = tm.CreatedDate,
            UpdatedDate = tm.UpdatedDate
        };

        return Result<TeachingMethodResponseModel>.Success(response, "Teaching method retrieved successfully.");
    }

    public async Task<Result<TeachingMethodResponseModel>> CreateMethodAsync(
        CreateTeachingMethodRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateCreate(request);
        if (!validation.IsSuccess)
        {
            return Result<TeachingMethodResponseModel>.ValidationError(validation.Errors, validation.Message);
        }

        var nameExists = await _context.TeachingMethods
            .AnyAsync(m => m.Name.ToLower() == request.Name.ToLower(), cancellationToken);
        if (nameExists)
        {
            return Result<TeachingMethodResponseModel>.Duplicate($"Teaching method '{request.Name}' already exists.");
        }

        var entity = new TeachingMethod
        {
            Name = request.Name,
            Description = request.Description,
            TeachingInstructions = request.TeachingInstructions,
            SuitableAgeGroup = request.SuitableAgeGroup,
            SuitableLearningLevelId = request.SuitableLearningLevelId,
            RequiredMaterials = request.RequiredMaterials,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _context.TeachingMethods.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetMethodByIdAsync(entity.TeachingMethodId, cancellationToken);
    }

    public async Task<Result<TeachingMethodResponseModel>> UpdateMethodAsync(
        int id, 
        UpdateTeachingMethodRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<TeachingMethodResponseModel>.ValidationError("Valid Teaching Method ID is required.");
        }

        var validation = _validationPolicy.ValidateUpdate(request);
        if (!validation.IsSuccess)
        {
            return Result<TeachingMethodResponseModel>.ValidationError(validation.Errors, validation.Message);
        }

        var entity = await _context.TeachingMethods.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            return Result<TeachingMethodResponseModel>.NotFound($"Teaching Method with ID ({id}) was not found.");
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.TeachingInstructions = request.TeachingInstructions;
        entity.SuitableAgeGroup = request.SuitableAgeGroup;
        entity.SuitableLearningLevelId = request.SuitableLearningLevelId;
        entity.RequiredMaterials = request.RequiredMaterials;
        entity.IsActive = request.IsActive;
        entity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetMethodByIdAsync(id, cancellationToken);
    }

    public async Task<Result> DeleteMethodAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.ValidationError("Valid Teaching Method ID is required.");
        }

        var entity = await _context.TeachingMethods.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            return Result.NotFound($"Teaching Method with ID ({id}) was not found.");
        }

        entity.IsActive = false;
        entity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success($"Teaching Method '{entity.Name}' deactivated successfully.");
    }
}
