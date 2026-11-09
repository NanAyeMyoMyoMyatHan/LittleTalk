using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Api.Common;
using LittleTalk.Application.Features.Dashboard.Services;
using LittleTalk.Infrastructure.Services;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Dashboard;

namespace LittleTalk.Api.Features.Dashboard;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class DashboardController : BaseApiController
{
    private readonly IDashboardService _dashboardService;
    private readonly ICurrentUserService _currentUserService;

    public DashboardController(IDashboardService dashboardService, ICurrentUserService currentUserService)
    {
        _dashboardService = dashboardService;
        _currentUserService = currentUserService;
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(Result<AdminDashboardResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken)
    {
        if (_currentUserService.IsInRole("Admin"))
        {
            var adminStats = await _dashboardService.GetAdminDashboardAsync(cancellationToken);
            return ToActionResult(adminStats);
        }

        var teacherId = _currentUserService.UserId ?? 1;
        var teacherStats = await _dashboardService.GetTeacherDashboardAsync(teacherId, cancellationToken);
        return ToActionResult(teacherStats);
    }

    [HttpGet("teacher")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(Result<TeacherDashboardResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeacherDashboard(CancellationToken cancellationToken)
    {
        var teacherId = _currentUserService.UserId ?? 1;
        var result = await _dashboardService.GetTeacherDashboardAsync(teacherId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<AdminDashboardResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminDashboard(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetAdminDashboardAsync(cancellationToken);
        return ToActionResult(result);
    }
}
