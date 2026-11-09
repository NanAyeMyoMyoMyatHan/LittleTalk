using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Api.Common;
using LittleTalk.Application.Features.Students.Services;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Students;

namespace LittleTalk.Api.Features.Students;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class StudentsController : BaseApiController
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Retrieve paginated list of students with optional level and active filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedListResponseModel<StudentSummaryResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents(
        [FromQuery] SearchStudentRequestModel request, 
        CancellationToken cancellationToken)
    {
        var result = await _studentService.GetStudentsAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Retrieve detailed student profile by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<StudentDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentById(int id, CancellationToken cancellationToken)
    {
        var result = await _studentService.GetStudentByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Create a new student with system-generated student code.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<StudentDetailResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateStudent(
        [FromBody] CreateStudentRequestModel request, 
        CancellationToken cancellationToken)
    {
        var result = await _studentService.CreateStudentAsync(request, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetStudentById), new { id = result.Data?.StudentId });
    }

    /// <summary>
    /// Update existing student profile information.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result<StudentDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateStudent(
        int id, 
        [FromBody] UpdateStudentRequestModel request, 
        CancellationToken cancellationToken)
    {
        var result = await _studentService.UpdateStudentAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Soft delete (deactivate) a student record.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id, CancellationToken cancellationToken)
    {
        var result = await _studentService.DeleteStudentAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Retrieve student academic enrollment and speaking assessment history.
    /// </summary>
    [HttpGet("{id:int}/learning-history")]
    [ProducesResponseType(typeof(Result<StudentLearningHistoryResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentLearningHistory(int id, CancellationToken cancellationToken)
    {
        var result = await _studentService.GetStudentLearningHistoryAsync(id, cancellationToken);
        return ToActionResult(result);
    }
}
