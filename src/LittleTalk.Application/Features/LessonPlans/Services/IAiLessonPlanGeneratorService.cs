using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Application.Features.LessonPlans.Services;

public interface IAiLessonPlanGeneratorService
{
    /// <summary>
    /// Generates a structured English speaking lesson plan tailored for children using AI.
    /// </summary>
    /// <param name="request">Topic, level, age group, duration, and optional focus notes.</param>
    /// <param name="currentUserId">Authenticated user ID making the request.</param>
    /// <param name="currentUserRole">Role of the authenticated user (Admin or Teacher).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the structured lesson plan draft.</returns>
    Task<Result<GenerateAiLessonPlanResponseModel>> GenerateLessonPlanAsync(
        GenerateAiLessonPlanRequestModel? request,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken = default);
}
