using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Api.Common;
using LittleTalk.Application.Features.Assessments.Services;
using LittleTalk.Infrastructure.Services;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Assessments;

namespace LittleTalk.Api.Features.Assessments;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class AssessmentsController : BaseApiController
{
    private readonly IAssessmentService _assessmentService;
    private readonly ICurrentUserService _currentUserService;

    public AssessmentsController(IAssessmentService assessmentService, ICurrentUserService currentUserService)
    {
        _assessmentService = assessmentService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedListResponseModel<AssessmentSummaryResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssessments(
        [FromQuery] SearchAssessmentRequestModel request, 
        CancellationToken cancellationToken)
    {
        var result = await _assessmentService.GetAssessmentsAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<AssessmentDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _assessmentService.GetAssessmentByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<AssessmentDetailResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateAssessmentRequestModel request, CancellationToken cancellationToken)
    {
        var teacherId = _currentUserService.UserId ?? 1;
        var result = await _assessmentService.CreateAssessmentAsync(request, teacherId, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetById), new { id = result.Data?.AssessmentId });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result<AssessmentDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAssessmentRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _assessmentService.UpdateAssessmentAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _assessmentService.DeleteAssessmentAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("student/{studentId:int}")]
    [ProducesResponseType(typeof(Result<List<AssessmentSummaryResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentAssessments(int studentId, CancellationToken cancellationToken)
    {
        var result = await _assessmentService.GetStudentAssessmentsAsync(studentId, cancellationToken);
        return ToActionResult(result);
    }
}
