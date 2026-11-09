using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Dashboard;

namespace LittleTalk.Application.Features.Dashboard.Services;

public interface IDashboardService
{
    Task<Result<TeacherDashboardResponseModel>> GetTeacherDashboardAsync(int teacherUserId, CancellationToken cancellationToken = default);
    Task<Result<AdminDashboardResponseModel>> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
}
