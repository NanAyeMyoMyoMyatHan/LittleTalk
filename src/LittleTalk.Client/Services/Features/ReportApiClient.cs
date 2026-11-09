using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Reports;

namespace LittleTalk.Client.Services.Features;

public class ReportApiClient : BaseFeatureApiClient, IReportApiClient
{
    public ReportApiClient(HttpClient http, CustomAuthenticationStateProvider authStateProvider) 
        : base(http, authStateProvider)
    {
    }

    public async Task<Result<StudentProgressReportResponseModel>> GetStudentProgressReportAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return (await SendGetAsync<StudentProgressReportResponseModel>($"api/reports/student/{studentId}"))
               ?? Result<StudentProgressReportResponseModel>.SystemError("No response received from server.");
    }
}
