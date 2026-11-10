using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Api.Common;
using LittleTalk.Application.Features.LessonPlans.Services;
using LittleTalk.Infrastructure.Services;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Api.Features.LessonPlans;

[ApiController]
[Route("api/[controller]")]
[Route("api/lesson-plans")]
[Authorize(Roles = "Admin,Teacher")]
public class LessonPlansController : BaseApiController
{
    private readonly ILessonPlanService _lessonPlanService;
    private readonly IAiLessonPlanGeneratorService _aiLessonPlanGeneratorService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<LessonPlansController> _logger;

    public LessonPlansController(
        ILessonPlanService lessonPlanService,
        IAiLessonPlanGeneratorService aiLessonPlanGeneratorService,
        ICurrentUserService currentUserService,
        ILogger<LessonPlansController> logger)
    {
        _lessonPlanService = lessonPlanService;
        _aiLessonPlanGeneratorService = aiLessonPlanGeneratorService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpPost("generate-ai")]
    [ProducesResponseType(typeof(Result<GenerateAiLessonPlanResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GenerateAiLessonPlan(
        [FromBody] GenerateAiLessonPlanRequestModel request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[LittleTalk.Api] GenerateAiLessonPlan => Lesson plan AI generation requested. Topic={Topic}, LevelId={LevelId}, UserId={UserId}",
            request?.Topic, request?.LearningLevelId, _currentUserService.UserId);

        var userId = _currentUserService.UserId ?? 1;
        var role = _currentUserService.Role ?? "Teacher";

        var result = await _aiLessonPlanGeneratorService.GenerateLessonPlanAsync(request, userId, role, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedListResponseModel<LessonPlanSummaryResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLessonPlans(
        [FromQuery] SearchLessonPlanRequestModel request, 
        CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.GetLessonPlansAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<LessonPlanDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLessonPlanById(int id, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.GetLessonPlanByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<LessonPlanDetailResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateLessonPlan([FromBody] CreateLessonPlanRequestModel request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? 1;
        var result = await _lessonPlanService.CreateLessonPlanAsync(request, userId, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetLessonPlanById), new { id = result.Data?.LessonPlanId });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result<LessonPlanDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateLessonPlan(int id, [FromBody] UpdateLessonPlanRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.UpdateLessonPlanAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLessonPlan(int id, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.DeleteLessonPlanAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("{id:int}/duplicate")]
    [ProducesResponseType(typeof(Result<LessonPlanDetailResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DuplicateLessonPlan(int id, [FromBody] DuplicateLessonPlanRequestModel request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? 1;
        var result = await _lessonPlanService.DuplicateLessonPlanAsync(id, request, userId, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetLessonPlanById), new { id = result.Data?.LessonPlanId });
    }

    // --- Steps ---
    [HttpPost("{id:int}/steps")]
    [ProducesResponseType(typeof(Result<LessonStepResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddStep(int id, [FromBody] CreateLessonStepRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.AddStepAsync(id, request, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetLessonPlanById), new { id });
    }

    [HttpPut("{id:int}/steps/{stepId:int}")]
    [ProducesResponseType(typeof(Result<LessonStepResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStep(int id, int stepId, [FromBody] UpdateLessonStepRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.UpdateStepAsync(id, stepId, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}/steps/{stepId:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteStep(int id, int stepId, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.DeleteStepAsync(id, stepId, cancellationToken);
        return ToActionResult(result);
    }

    // --- Vocabulary ---
    [HttpPost("{id:int}/vocabulary")]
    [ProducesResponseType(typeof(Result<LessonVocabularyResponseModel>), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddVocabulary(int id, [FromBody] AddLessonVocabularyRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.AddVocabularyAsync(id, request, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetLessonPlanById), new { id });
    }

    [HttpDelete("{id:int}/vocabulary/{vocabId:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteVocabulary(int id, int vocabId, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.DeleteVocabularyAsync(id, vocabId, cancellationToken);
        return ToActionResult(result);
    }

    // --- Materials Link ---
    [HttpPost("{id:int}/materials")]
    [ProducesResponseType(typeof(Result<LessonPlanMaterialResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LinkMaterial(int id, [FromBody] LinkMaterialRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.LinkMaterialAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}/materials/{materialId:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnlinkMaterial(int id, int materialId, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.UnlinkMaterialAsync(id, materialId, cancellationToken);
        return ToActionResult(result);
    }

    // --- Activities Link ---
    [HttpPost("{id:int}/activities")]
    [ProducesResponseType(typeof(Result<LessonActivityResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LinkActivity(int id, [FromBody] LinkActivityRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.LinkActivityAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}/activities/{activityId:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnlinkActivity(int id, int activityId, CancellationToken cancellationToken)
    {
        var result = await _lessonPlanService.UnlinkActivityAsync(id, activityId, cancellationToken);
        return ToActionResult(result);
    }
}
