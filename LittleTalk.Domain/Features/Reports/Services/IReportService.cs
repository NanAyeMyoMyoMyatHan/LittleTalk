using LittleTalk.Shared;
using LittleTalk.Models.Common;
using LittleTalk.Models.Features.Reports;

namespace LittleTalk.Domain.Features.Reports.Services;

public interface IReportService
{
    Task<Result<StudentProgressReportResponseModel>> GetStudentProgressReportAsync(int studentId, CancellationToken cancellationToken = default);
}


