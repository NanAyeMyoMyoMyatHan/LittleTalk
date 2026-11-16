using LittleTalk.Shared;
using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Dashboard;

namespace LittleTalk.Domain.Features.Dashboard.Services;

public interface IDashboardService
{
    Task<Result<TeacherDashboardResponseModel>> GetTeacherDashboardAsync(int teacherUserId, CancellationToken cancellationToken = default);
    Task<Result<AdminDashboardResponseModel>> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
}


