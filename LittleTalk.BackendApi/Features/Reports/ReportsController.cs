using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Shared;
using LittleTalk.Domain.Features.Reports.Services;
using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Reports;

namespace LittleTalk.BackendApi.Features.Reports;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher,Parent")]
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("student/{studentId:int}")]
    [ProducesResponseType(typeof(Result<StudentProgressReportResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentProgressReport(int studentId, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetStudentProgressReportAsync(studentId, cancellationToken);
        return ToActionResult(result);
    }
}

