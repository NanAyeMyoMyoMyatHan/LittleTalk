using LittleTalk.Shared; using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Dashboard;

namespace LittleTalk.App.Services.Features;

public class DashboardApiClient : BaseFeatureApiClient, IDashboardApiClient
{
    public DashboardApiClient(HttpClient http, CustomAuthenticationStateProvider authStateProvider) 
        : base(http, authStateProvider)
    {
    }

    public async Task<Result<TeacherDashboardResponseModel>> GetTeacherDashboardAsync(CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<TeacherDashboardResponseModel>("api/dashboard/teacher"))
               ?? Result<TeacherDashboardResponseModel>.SystemError("No response received from server.");
    }

    public async Task<Result<AdminDashboardResponseModel>> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<AdminDashboardResponseModel>("api/dashboard/admin"))
               ?? Result<AdminDashboardResponseModel>.SystemError("No response received from server.");
    }
}

