using Microsoft.EntityFrameworkCore;
using LittleTalk.Application.Features.LessonPlans;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Application.Features.LessonPlans.Services;

public sealed class LessonPlanService : ILessonPlanService
{
    private readonly LittleTalkDbContext _context;
    private readonly LessonPlanValidationPolicy _validationPolicy;

    public LessonPlanService(
        LittleTalkDbContext context,
        LessonPlanValidationPolicy validationPolicy)
    {
        _context = context;
        _validationPolicy = validationPolicy;
    }

    public async Task<Result<PagedListResponseModel<LessonPlanSummaryResponseModel>>> GetLessonPlansAsync(
        SearchLessonPlanRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.LessonPlans
            .AsNoTracking()
            .Include(lp => lp.Course)
            .Include(lp => lp.LearningLevel)
            .Include(lp => lp.CreatedByNavigation)
            .Include(lp => lp.LessonSteps)
            .Include(lp => lp.LessonVocabularies)
            .AsQueryable();

        if (request.CourseId.HasValue && request.CourseId.Value > 0)
        {
            query = query.Where(lp => lp.CourseId == request.CourseId.Value);
        }

        var levelId = request.LearningLevelId ?? request.LevelId;
        if (levelId.HasValue && levelId.Value > 0)
        {
            query = query.Where(lp => lp.LearningLevelId == levelId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(lp => lp.Status.ToLower() == request.Status.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            query = query.Where(lp => lp.Title.ToLower().Contains(search) || lp.Topic.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(lp => lp.CreatedDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(lp => new LessonPlanSummaryResponseModel
            {
                LessonPlanId = lp.LessonPlanId,
                CourseId = lp.CourseId,
                CourseName = lp.Course != null ? lp.Course.CourseName : "Standalone / Shared Template",
                Title = lp.Title,
                Topic = lp.Topic,
                Description = lp.Description,
                LearningLevelId = lp.LearningLevelId,
                LearningLevelName = lp.LearningLevel.LevelName,
                AgeGroup = lp.AgeGroup,
                DurationMinutes = lp.DurationMinutes,
                Status = lp.Status,
                StepsCount = lp.LessonSteps.Count,
                VocabularyCount = lp.LessonVocabularies.Count,
                CreatedByName = lp.CreatedByNavigation.FullName,
                CreatedDate = lp.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedList = new PagedListResponseModel<LessonPlanSummaryResponseModel>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedListResponseModel<LessonPlanSummaryResponseModel>>.Success(pagedList, "Lesson plans retrieved successfully.");
    }

    public async Task<Result<LessonPlanDetailResponseModel>> GetLessonPlanByIdAsync(
        int lessonPlanId, 
        CancellationToken cancellationToken = default)
    {
        if (lessonPlanId <= 0)
        {
            return Result<LessonPlanDetailResponseModel>.ValidationError("Valid Lesson Plan ID is required.");
        }

        var plan = await _context.LessonPlans
            .AsNoTracking()
            .Include(lp => lp.Course)
            .Include(lp => lp.LearningLevel)
            .Include(lp => lp.CreatedByNavigation)
            .Include(lp => lp.LessonSteps)
                .ThenInclude(ls => ls.TeachingMethod)
            .Include(lp => lp.LessonVocabularies)
            .Include(lp => lp.LessonPlanMaterials)
                .ThenInclude(lpm => lpm.Material)
            .Include(lp => lp.LessonActivities)
                .ThenInclude(la => la.Activity)
                    .ThenInclude(a => a.Category)
            .Include(lp => lp.LessonActivities)
                .ThenInclude(la => la.LessonStep)
            .FirstOrDefaultAsync(lp => lp.LessonPlanId == lessonPlanId, cancellationToken);

        if (plan == null)
        {
            return Result<LessonPlanDetailResponseModel>.NotFound($"Lesson Plan with ID ({lessonPlanId}) was not found.");
        }

        return Result<LessonPlanDetailResponseModel>.Success(MapToDetailResponse(plan), "Lesson plan details retrieved successfully.");
    }

    public async Task<Result<LessonPlanDetailResponseModel>> CreateLessonPlanAsync(
        CreateLessonPlanRequestModel request, 
        int currentUserId, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateCreate(request);
        if (!validation.IsSuccess)
        {
            return Result<LessonPlanDetailResponseModel>.ValidationError(validation.Errors, validation.Message);
        }

        var levelExists = await _context.LearningLevels
            .AnyAsync(l => l.LearningLevelId == request.LearningLevelId && l.IsActive, cancellationToken);
        if (!levelExists)
        {
            return Result<LessonPlanDetailResponseModel>.ValidationError("The selected learning level is invalid or inactive.");
        }

        if (request.CourseId.HasValue && request.CourseId.Value > 0)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == request.CourseId.Value, cancellationToken);
            if (!courseExists)
            {
                return Result<LessonPlanDetailResponseModel>.NotFound($"Course with ID ({request.CourseId.Value}) was not found.");
            }
        }

        var plan = new LessonPlan
        {
            CourseId = request.CourseId,
            Title = request.Title,
            Topic = request.Topic,
            Description = request.Description,
            LearningLevelId = request.LearningLevelId,
            AgeGroup = request.AgeGroup,
            DurationMinutes = request.DurationMinutes,
            LearningObjectives = request.LearningObjectives,
            Status = request.Status,
            CreatedBy = currentUserId,
            CreatedDate = DateTime.UtcNow
        };

        if (request.InitialSteps != null && request.InitialSteps.Count > 0)
        {
            int order = 1;
            foreach (var stepDto in request.InitialSteps)
            {
                plan.LessonSteps.Add(new LessonStep
                {
                    StepName = stepDto.StepName.Trim(),
                    DurationMinutes = stepDto.DurationMinutes,
                    TeachingMethodId = stepDto.TeachingMethodId,
                    TeachingMethodCustom = stepDto.TeachingMethodCustom?.Trim(),
                    Instructions = stepDto.Instructions.Trim(),
                    TeacherScript = stepDto.TeacherScript?.Trim(),
                    ExpectedStudentResponse = stepDto.ExpectedStudentResponse?.Trim(),
                    SortOrder = stepDto.SortOrder > 0 ? stepDto.SortOrder : order++,
                    CreatedDate = DateTime.UtcNow
                });
            }
        }

        _context.LessonPlans.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetLessonPlanByIdAsync(plan.LessonPlanId, cancellationToken);
    }

    public async Task<Result<LessonPlanDetailResponseModel>> UpdateLessonPlanAsync(
        int lessonPlanId, 
        UpdateLessonPlanRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        if (lessonPlanId <= 0)
            return Result<LessonPlanDetailResponseModel>.ValidationError("Valid Lesson Plan ID is required.");

        var validation = _validationPolicy.ValidateUpdate(request);
        if (!validation.IsSuccess)
            return Result<LessonPlanDetailResponseModel>.ValidationError(validation.Errors, validation.Message);

        var plan = await _context.LessonPlans.FindAsync(new object[] { lessonPlanId }, cancellationToken);
        if (plan == null)
            return Result<LessonPlanDetailResponseModel>.NotFound($"Lesson Plan with ID ({lessonPlanId}) was not found.");

        plan.CourseId = request.CourseId;
        plan.Title = request.Title;
        plan.Topic = request.Topic;
        plan.Description = request.Description;
        plan.LearningLevelId = request.LearningLevelId;
        plan.AgeGroup = request.AgeGroup;
        plan.DurationMinutes = request.DurationMinutes;
        plan.LearningObjectives = request.LearningObjectives;
        plan.Status = request.Status;
        plan.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetLessonPlanByIdAsync(lessonPlanId, cancellationToken);
    }

    public async Task<Result> DeleteLessonPlanAsync(
        int lessonPlanId, 
        CancellationToken cancellationToken = default)
    {
        if (lessonPlanId <= 0)
            return Result.ValidationError("Valid Lesson Plan ID is required.");

        var plan = await _context.LessonPlans.FindAsync(new object[] { lessonPlanId }, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Lesson Plan with ID ({lessonPlanId}) was not found.");

        plan.Status = "Archived";
        plan.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success($"Lesson Plan '{plan.Title}' archived successfully.");
    }

    public async Task<Result<LessonPlanDetailResponseModel>> DuplicateLessonPlanAsync(
        int lessonPlanId, 
        DuplicateLessonPlanRequestModel request, 
        int currentUserId, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateDuplicate(request);
        if (!validation.IsSuccess)
            return Result<LessonPlanDetailResponseModel>.ValidationError(validation.Message);

        var sourcePlan = await _context.LessonPlans
            .Include(lp => lp.LessonSteps)
            .Include(lp => lp.LessonVocabularies)
            .Include(lp => lp.LessonPlanMaterials)
            .Include(lp => lp.LessonActivities)
            .FirstOrDefaultAsync(lp => lp.LessonPlanId == lessonPlanId, cancellationToken);

        if (sourcePlan == null)
            return Result<LessonPlanDetailResponseModel>.NotFound($"Lesson Plan with ID ({lessonPlanId}) was not found.");

        var newPlan = new LessonPlan
        {
            CourseId = request.TargetCourseId ?? sourcePlan.CourseId,
            Title = string.IsNullOrWhiteSpace(request.NewTitle) ? $"{sourcePlan.Title} (Copy)" : request.NewTitle,
            Topic = sourcePlan.Topic,
            Description = sourcePlan.Description,
            LearningLevelId = sourcePlan.LearningLevelId,
            AgeGroup = sourcePlan.AgeGroup,
            DurationMinutes = sourcePlan.DurationMinutes,
            LearningObjectives = sourcePlan.LearningObjectives,
            Status = "Draft",
            CreatedBy = currentUserId,
            CreatedDate = DateTime.UtcNow
        };

        var stepMapping = new Dictionary<int, LessonStep>();
        foreach (var s in sourcePlan.LessonSteps.OrderBy(x => x.SortOrder))
        {
            var newStep = new LessonStep
            {
                StepName = s.StepName,
                DurationMinutes = s.DurationMinutes,
                TeachingMethodId = s.TeachingMethodId,
                TeachingMethodCustom = s.TeachingMethodCustom,
                Instructions = s.Instructions,
                TeacherScript = s.TeacherScript,
                ExpectedStudentResponse = s.ExpectedStudentResponse,
                SortOrder = s.SortOrder,
                CreatedDate = DateTime.UtcNow
            };
            newPlan.LessonSteps.Add(newStep);
            stepMapping[s.LessonStepId] = newStep;
        }

        foreach (var v in sourcePlan.LessonVocabularies.OrderBy(x => x.SortOrder))
        {
            newPlan.LessonVocabularies.Add(new LessonVocabulary
            {
                Word = v.Word,
                Meaning = v.Meaning,
                Pronunciation = v.Pronunciation,
                ExampleSentence = v.ExampleSentence,
                Topic = v.Topic,
                ImageUrl = v.ImageUrl,
                AudioUrl = v.AudioUrl,
                SortOrder = v.SortOrder,
                CreatedDate = DateTime.UtcNow
            });
        }

        foreach (var m in sourcePlan.LessonPlanMaterials)
        {
            newPlan.LessonPlanMaterials.Add(new LessonPlanMaterial
            {
                MaterialId = m.MaterialId,
                UsageInstructions = m.UsageInstructions,
                SortOrder = m.SortOrder,
                CreatedDate = DateTime.UtcNow
            });
        }

        _context.LessonPlans.Add(newPlan);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var a in sourcePlan.LessonActivities)
        {
            int? mappedStepId = null;
            if (a.LessonStepId.HasValue && stepMapping.TryGetValue(a.LessonStepId.Value, out var targetStep))
            {
                mappedStepId = targetStep.LessonStepId;
            }

            _context.LessonActivities.Add(new LessonActivity
            {
                LessonPlanId = newPlan.LessonPlanId,
                ActivityId = a.ActivityId,
                LessonStepId = mappedStepId,
                StepOrder = a.StepOrder,
                Notes = a.Notes,
                CreatedDate = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync(cancellationToken);

        return await GetLessonPlanByIdAsync(newPlan.LessonPlanId, cancellationToken);
    }

    public async Task<Result<LessonStepResponseModel>> AddStepAsync(
        int lessonPlanId, 
        CreateLessonStepRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateStep(request);
        if (!validation.IsSuccess)
            return Result<LessonStepResponseModel>.ValidationError(validation.Errors, validation.Message);

        var planExists = await _context.LessonPlans.AnyAsync(lp => lp.LessonPlanId == lessonPlanId, cancellationToken);
        if (!planExists)
            return Result<LessonStepResponseModel>.NotFound($"Lesson Plan with ID ({lessonPlanId}) was not found.");

        var step = new LessonStep
        {
            LessonPlanId = lessonPlanId,
            StepName = request.StepName,
            DurationMinutes = request.DurationMinutes,
            TeachingMethodId = request.TeachingMethodId,
            TeachingMethodCustom = request.TeachingMethodCustom?.Trim(),
            Instructions = request.Instructions,
            TeacherScript = request.TeacherScript?.Trim(),
            ExpectedStudentResponse = request.ExpectedStudentResponse?.Trim(),
            SortOrder = request.SortOrder,
            CreatedDate = DateTime.UtcNow
        };

        _context.LessonSteps.Add(step);
        await _context.SaveChangesAsync(cancellationToken);

        var method = step.TeachingMethodId.HasValue 
            ? await _context.TeachingMethods.FindAsync(new object[] { step.TeachingMethodId.Value }, cancellationToken) 
            : null;

        var response = new LessonStepResponseModel
        {
            LessonStepId = step.LessonStepId,
            LessonPlanId = step.LessonPlanId,
            StepName = step.StepName,
            DurationMinutes = step.DurationMinutes,
            TeachingMethodId = step.TeachingMethodId,
            TeachingMethodName = method?.Name,
            TeachingMethodCustom = step.TeachingMethodCustom,
            Instructions = step.Instructions,
            TeacherScript = step.TeacherScript,
            ExpectedStudentResponse = step.ExpectedStudentResponse,
            SortOrder = step.SortOrder
        };

        return Result<LessonStepResponseModel>.Success(response, "Step added successfully.");
    }

    public async Task<Result<LessonStepResponseModel>> UpdateStepAsync(
        int lessonPlanId, 
        int stepId, 
        UpdateLessonStepRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateStep(request);
        if (!validation.IsSuccess)
            return Result<LessonStepResponseModel>.ValidationError(validation.Errors, validation.Message);

        var step = await _context.LessonSteps
            .Include(s => s.TeachingMethod)
            .FirstOrDefaultAsync(s => s.LessonPlanId == lessonPlanId && s.LessonStepId == stepId, cancellationToken);

        if (step == null)
            return Result<LessonStepResponseModel>.NotFound($"Step {stepId} in LessonPlan {lessonPlanId} not found.");

        step.StepName = request.StepName;
        step.DurationMinutes = request.DurationMinutes;
        step.TeachingMethodId = request.TeachingMethodId;
        step.TeachingMethodCustom = request.TeachingMethodCustom?.Trim();
        step.Instructions = request.Instructions;
        step.TeacherScript = request.TeacherScript?.Trim();
        step.ExpectedStudentResponse = request.ExpectedStudentResponse?.Trim();
        step.SortOrder = request.SortOrder;

        await _context.SaveChangesAsync(cancellationToken);

        var response = new LessonStepResponseModel
        {
            LessonStepId = step.LessonStepId,
            LessonPlanId = step.LessonPlanId,
            StepName = step.StepName,
            DurationMinutes = step.DurationMinutes,
            TeachingMethodId = step.TeachingMethodId,
            TeachingMethodName = step.TeachingMethod?.Name,
            TeachingMethodCustom = step.TeachingMethodCustom,
            Instructions = step.Instructions,
            TeacherScript = step.TeacherScript,
            ExpectedStudentResponse = step.ExpectedStudentResponse,
            SortOrder = step.SortOrder
        };

        return Result<LessonStepResponseModel>.Success(response, "Step updated successfully.");
    }

    public async Task<Result> DeleteStepAsync(
        int lessonPlanId, 
        int stepId, 
        CancellationToken cancellationToken = default)
    {
        var step = await _context.LessonSteps
            .FirstOrDefaultAsync(s => s.LessonPlanId == lessonPlanId && s.LessonStepId == stepId, cancellationToken);

        if (step == null)
            return Result.NotFound($"Step {stepId} in LessonPlan {lessonPlanId} not found.");

        _context.LessonSteps.Remove(step);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Step deleted successfully.");
    }

    public async Task<Result<LessonVocabularyResponseModel>> AddVocabularyAsync(
        int lessonPlanId, 
        AddLessonVocabularyRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateVocabulary(request);
        if (!validation.IsSuccess)
            return Result<LessonVocabularyResponseModel>.ValidationError(validation.Errors, validation.Message);

        var planExists = await _context.LessonPlans.AnyAsync(lp => lp.LessonPlanId == lessonPlanId, cancellationToken);
        if (!planExists)
            return Result<LessonVocabularyResponseModel>.NotFound($"Lesson Plan with ID ({lessonPlanId}) was not found.");

        var vocab = new LessonVocabulary
        {
            LessonPlanId = lessonPlanId,
            Word = request.Word,
            Meaning = request.Meaning,
            Pronunciation = request.Pronunciation?.Trim(),
            ExampleSentence = request.ExampleSentence,
            Topic = request.Topic?.Trim(),
            ImageUrl = request.ImageUrl?.Trim(),
            AudioUrl = request.AudioUrl?.Trim(),
            SortOrder = request.SortOrder,
            CreatedDate = DateTime.UtcNow
        };

        _context.LessonVocabularies.Add(vocab);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new LessonVocabularyResponseModel
        {
            VocabularyId = vocab.VocabularyId,
            LessonPlanId = vocab.LessonPlanId,
            Word = vocab.Word,
            Meaning = vocab.Meaning,
            Pronunciation = vocab.Pronunciation,
            ExampleSentence = vocab.ExampleSentence,
            Topic = vocab.Topic,
            ImageUrl = vocab.ImageUrl,
            AudioUrl = vocab.AudioUrl,
            SortOrder = vocab.SortOrder
        };

        return Result<LessonVocabularyResponseModel>.Success(response, "Vocabulary added successfully.");
    }

    public async Task<Result> DeleteVocabularyAsync(
        int lessonPlanId, 
        int vocabularyId, 
        CancellationToken cancellationToken = default)
    {
        var vocab = await _context.LessonVocabularies
            .FirstOrDefaultAsync(v => v.LessonPlanId == lessonPlanId && v.VocabularyId == vocabularyId, cancellationToken);

        if (vocab == null)
            return Result.NotFound($"Vocabulary {vocabularyId} in LessonPlan {lessonPlanId} not found.");

        _context.LessonVocabularies.Remove(vocab);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Vocabulary deleted successfully.");
    }

    public async Task<Result<LessonPlanMaterialResponseModel>> LinkMaterialAsync(
        int lessonPlanId, 
        LinkMaterialRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var planExists = await _context.LessonPlans.AnyAsync(lp => lp.LessonPlanId == lessonPlanId, cancellationToken);
        if (!planExists)
            return Result<LessonPlanMaterialResponseModel>.NotFound($"Lesson Plan with ID ({lessonPlanId}) was not found.");

        var material = await _context.TeachingMaterials.FindAsync(new object[] { request.MaterialId }, cancellationToken);
        if (material == null)
            return Result<LessonPlanMaterialResponseModel>.NotFound($"Teaching Material with ID ({request.MaterialId}) was not found.");

        var existingLink = await _context.LessonPlanMaterials
            .FirstOrDefaultAsync(lpm => lpm.LessonPlanId == lessonPlanId && lpm.MaterialId == request.MaterialId, cancellationToken);

        if (existingLink != null)
        {
            existingLink.UsageInstructions = request.UsageInstructions?.Trim();
            existingLink.SortOrder = request.SortOrder;
        }
        else
        {
            existingLink = new LessonPlanMaterial
            {
                LessonPlanId = lessonPlanId,
                MaterialId = request.MaterialId,
                UsageInstructions = request.UsageInstructions?.Trim(),
                SortOrder = request.SortOrder,
                CreatedDate = DateTime.UtcNow
            };
            _context.LessonPlanMaterials.Add(existingLink);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var response = new LessonPlanMaterialResponseModel
        {
            LessonPlanMaterialId = existingLink.LessonPlanMaterialId,
            MaterialId = material.MaterialId,
            MaterialName = material.MaterialName,
            MaterialType = material.MaterialType,
            FileUrl = material.FileUrl,
            UsageInstructions = existingLink.UsageInstructions,
            SortOrder = existingLink.SortOrder
        };

        return Result<LessonPlanMaterialResponseModel>.Success(response, "Material linked successfully.");
    }

    public async Task<Result> UnlinkMaterialAsync(
        int lessonPlanId, 
        int materialId, 
        CancellationToken cancellationToken = default)
    {
        var link = await _context.LessonPlanMaterials
            .FirstOrDefaultAsync(lpm => lpm.LessonPlanId == lessonPlanId && lpm.MaterialId == materialId, cancellationToken);

        if (link == null)
            return Result.NotFound("Material link not found.");

        _context.LessonPlanMaterials.Remove(link);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Material unlinked successfully.");
    }

    public async Task<Result<LessonActivityResponseModel>> LinkActivityAsync(
        int lessonPlanId, 
        LinkActivityRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var planExists = await _context.LessonPlans.AnyAsync(lp => lp.LessonPlanId == lessonPlanId, cancellationToken);
        if (!planExists)
            return Result<LessonActivityResponseModel>.NotFound($"Lesson Plan with ID ({lessonPlanId}) was not found.");

        var activity = await _context.Activities
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.ActivityId == request.ActivityId, cancellationToken);

        if (activity == null)
            return Result<LessonActivityResponseModel>.NotFound($"Activity with ID ({request.ActivityId}) was not found.");

        if (request.LessonStepId.HasValue)
        {
            var stepValid = await _context.LessonSteps
                .AnyAsync(s => s.LessonPlanId == lessonPlanId && s.LessonStepId == request.LessonStepId.Value, cancellationToken);

            if (!stepValid)
            {
                return Result<LessonActivityResponseModel>.ValidationError($"LessonStep {request.LessonStepId.Value} does not belong to LessonPlan {lessonPlanId}.");
            }
        }

        var lessonActivity = new LessonActivity
        {
            LessonPlanId = lessonPlanId,
            ActivityId = request.ActivityId,
            LessonStepId = request.LessonStepId,
            StepOrder = request.StepOrder,
            Notes = request.Notes?.Trim(),
            CreatedDate = DateTime.UtcNow
        };

        _context.LessonActivities.Add(lessonActivity);
        await _context.SaveChangesAsync(cancellationToken);

        var stepName = request.LessonStepId.HasValue
            ? (await _context.LessonSteps.FindAsync(new object[] { request.LessonStepId.Value }, cancellationToken))?.StepName
            : null;

        var response = new LessonActivityResponseModel
        {
            LessonActivityId = lessonActivity.LessonActivityId,
            ActivityId = activity.ActivityId,
            ActivityName = activity.ActivityName,
            CategoryName = activity.Category.CategoryName,
            LessonStepId = lessonActivity.LessonStepId,
            LessonStepName = stepName,
            StepOrder = lessonActivity.StepOrder,
            Notes = lessonActivity.Notes
        };

        return Result<LessonActivityResponseModel>.Success(response, "Activity linked successfully.");
    }

    public async Task<Result> UnlinkActivityAsync(
        int lessonPlanId, 
        int activityId, 
        CancellationToken cancellationToken = default)
    {
        var link = await _context.LessonActivities
            .FirstOrDefaultAsync(la => la.LessonPlanId == lessonPlanId && la.ActivityId == activityId, cancellationToken);

        if (link == null)
            return Result.NotFound("Activity link not found.");

        _context.LessonActivities.Remove(link);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Activity unlinked successfully.");
    }

    private static LessonPlanDetailResponseModel MapToDetailResponse(LessonPlan plan)
    {
        return new LessonPlanDetailResponseModel
        {
            LessonPlanId = plan.LessonPlanId,
            CourseId = plan.CourseId,
            CourseName = plan.Course?.CourseName ?? "Standalone / Shared Template",
            Title = plan.Title,
            Topic = plan.Topic,
            Description = plan.Description,
            LearningLevelId = plan.LearningLevelId,
            LearningLevelName = plan.LearningLevel?.LevelName ?? string.Empty,
            AgeGroup = plan.AgeGroup,
            DurationMinutes = plan.DurationMinutes,
            LearningObjectives = plan.LearningObjectives,
            Status = plan.Status,
            CreatedByName = plan.CreatedByNavigation?.FullName ?? string.Empty,
            CreatedDate = plan.CreatedDate,
            UpdatedDate = plan.UpdatedDate,
            StepsCount = plan.LessonSteps.Count,
            VocabularyCount = plan.LessonVocabularies.Count,
            Steps = plan.LessonSteps.OrderBy(s => s.SortOrder).Select(s => new LessonStepResponseModel
            {
                LessonStepId = s.LessonStepId,
                LessonPlanId = s.LessonPlanId,
                StepName = s.StepName,
                DurationMinutes = s.DurationMinutes,
                TeachingMethodId = s.TeachingMethodId,
                TeachingMethodName = s.TeachingMethod?.Name ?? s.TeachingMethodCustom,
                TeachingMethodCustom = s.TeachingMethodCustom,
                Instructions = s.Instructions,
                TeacherScript = s.TeacherScript,
                ExpectedStudentResponse = s.ExpectedStudentResponse,
                SortOrder = s.SortOrder
            }).ToList(),
            Vocabulary = plan.LessonVocabularies.OrderBy(v => v.SortOrder).Select(v => new LessonVocabularyResponseModel
            {
                VocabularyId = v.VocabularyId,
                LessonPlanId = v.LessonPlanId,
                Word = v.Word,
                Meaning = v.Meaning,
                Pronunciation = v.Pronunciation,
                ExampleSentence = v.ExampleSentence,
                Topic = v.Topic,
                ImageUrl = v.ImageUrl,
                AudioUrl = v.AudioUrl,
                SortOrder = v.SortOrder
            }).ToList(),
            Materials = plan.LessonPlanMaterials.OrderBy(m => m.SortOrder).Select(m => new LessonPlanMaterialResponseModel
            {
                LessonPlanMaterialId = m.LessonPlanMaterialId,
                MaterialId = m.MaterialId,
                MaterialName = m.Material?.MaterialName ?? string.Empty,
                MaterialType = m.Material?.MaterialType ?? string.Empty,
                FileUrl = m.Material?.FileUrl,
                UsageInstructions = m.UsageInstructions,
                SortOrder = m.SortOrder
            }).ToList(),
            Activities = plan.LessonActivities.OrderBy(a => a.StepOrder).Select(a => new LessonActivityResponseModel
            {
                LessonActivityId = a.LessonActivityId,
                ActivityId = a.ActivityId,
                ActivityName = a.Activity?.ActivityName ?? string.Empty,
                CategoryName = a.Activity?.Category?.CategoryName ?? string.Empty,
                LessonStepId = a.LessonStepId,
                LessonStepName = a.LessonStep?.StepName,
                StepOrder = a.StepOrder,
                Notes = a.Notes
            }).ToList()
        };
    }
}
