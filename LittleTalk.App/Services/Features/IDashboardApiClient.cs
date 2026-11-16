using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Dashboard;

namespace LittleTalk.App.Services.Features;

public interface IDashboardApiClient
{
    Task<Result<TeacherDashboardResponseModel>> GetTeacherDashboardAsync(CancellationToken cancellationToken = default);
    Task<Result<AdminDashboardResponseModel>> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
}

