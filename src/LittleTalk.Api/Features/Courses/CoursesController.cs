using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Api.Common;
using LittleTalk.Application.Features.Courses.Services;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Courses;

namespace LittleTalk.Api.Features.Courses;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class CoursesController : BaseApiController
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedListResponseModel<CourseSummaryResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(
        [FromQuery] SearchCourseRequestModel request, 
        CancellationToken cancellationToken)
    {
        var result = await _courseService.GetCoursesAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<CourseDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById(int id, CancellationToken cancellationToken)
    {
        var result = await _courseService.GetCourseByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<CourseDetailResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _courseService.CreateCourseAsync(request, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetCourseById), new { id = result.Data?.CourseId });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<CourseDetailResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _courseService.UpdateCourseAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(int id, CancellationToken cancellationToken)
    {
        var result = await _courseService.DeleteCourseAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("{id:int}/teachers")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<CourseTeacherResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AssignTeacher(int id, [FromBody] AssignTeacherRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _courseService.AssignTeacherAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}/teachers/{teacherId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTeacher(int id, int teacherId, CancellationToken cancellationToken)
    {
        var result = await _courseService.RemoveTeacherAsync(id, teacherId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}/enrollments")]
    [ProducesResponseType(typeof(Result<List<CourseEnrollmentResponseModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseEnrollments(int id, CancellationToken cancellationToken)
    {
        var result = await _courseService.GetCourseEnrollmentsAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("{id:int}/enroll")]
    [ProducesResponseType(typeof(Result<CourseEnrollmentResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> EnrollStudent(int id, [FromBody] EnrollStudentRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _courseService.EnrollStudentAsync(id, request, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetCourseById), new { id });
    }

    [HttpPut("{id:int}/enrollments/{studentId:int}/status")]
    [ProducesResponseType(typeof(Result<CourseEnrollmentResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentStatus(int id, int studentId, [FromBody] UpdateEnrollmentStatusRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _courseService.UpdateEnrollmentStatusAsync(id, studentId, request, cancellationToken);
        return ToActionResult(result);
    }
}
