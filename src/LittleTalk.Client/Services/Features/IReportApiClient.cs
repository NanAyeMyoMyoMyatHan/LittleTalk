using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Reports;

namespace LittleTalk.Client.Services.Features;

public interface IReportApiClient
{
    Task<Result<StudentProgressReportResponseModel>> GetStudentProgressReportAsync(int studentId, CancellationToken cancellationToken = default);
}
