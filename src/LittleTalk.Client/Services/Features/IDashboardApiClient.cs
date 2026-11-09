using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Dashboard;

namespace LittleTalk.Client.Services.Features;

public interface IDashboardApiClient
{
    Task<Result<TeacherDashboardResponseModel>> GetTeacherDashboardAsync(CancellationToken cancellationToken = default);
    Task<Result<AdminDashboardResponseModel>> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
}
