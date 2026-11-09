using Microsoft.EntityFrameworkCore;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Reports;

namespace LittleTalk.Application.Features.Reports.Services;

public class ReportService : IReportService
{
    private readonly LittleTalkDbContext _context;

    public ReportService(LittleTalkDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StudentProgressReportResponseModel>> GetStudentProgressReportAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.LearningLevel)
            .Include(s => s.Attendances)
            .Include(s => s.Assessments)
                .ThenInclude(a => a.EvaluatorTeacher)
            .Include(s => s.Assessments)
                .ThenInclude(a => a.LessonPlan)
            .Include(s => s.Assessments)
                .ThenInclude(a => a.AssessmentDetails)
            .FirstOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);

        if (student == null)
            return Result<StudentProgressReportResponseModel>.NotFound($"Student with ID {studentId} was not found.");

        // Attendance stats
        var totalAtt = student.Attendances.Count;
        var presentCount = student.Attendances.Count(a => a.Status == "Present");
        var lateCount = student.Attendances.Count(a => a.Status == "Late");
        var absentCount = student.Attendances.Count(a => a.Status == "Absent");
        var attendancePct = totalAtt > 0
            ? Math.Round(((decimal)(presentCount + lateCount) / totalAtt) * 100, 1)
            : 0;

        // Assessment stats
        var validAssessments = student.Assessments
            .Where(a => a.OverallScore.HasValue)
            .OrderBy(a => a.AssessmentDate)
            .ToList();

        var totalAssessments = validAssessments.Count;
        var overallAvg = totalAssessments > 0
            ? Math.Round(validAssessments.Average(a => a.OverallScore!.Value), 2)
            : 0;

        // Criteria Breakdown
        var allDetails = validAssessments
            .SelectMany(a => a.AssessmentDetails)
            .GroupBy(ad => ad.CriterionName)
            .Select(g => new CriterionAverageResponseModel
            {
                CriterionName = g.Key,
                AverageScore = Math.Round((decimal)g.Average(ad => ad.Score), 2),
                EvaluationsCount = g.Count()
            })
            .OrderByDescending(c => c.AverageScore)
            .ToList();

        // Standardize all 6 criteria if not all present yet
        var standardCriteria = new[] { "Pronunciation", "Vocabulary", "Fluency", "Sentence Formation", "Confidence", "Listening and Responding" };
        foreach (var sc in standardCriteria)
        {
            if (!allDetails.Any(c => c.CriterionName.Equals(sc, StringComparison.OrdinalIgnoreCase)))
            {
                allDetails.Add(new CriterionAverageResponseModel
                {
                    CriterionName = sc,
                    AverageScore = 0,
                    EvaluationsCount = 0
                });
            }
        }

        // Timeline (Date & OverallScore)
        var timeline = validAssessments.Select(a => new ScoreTimelinePointResponseModel
        {
            Date = a.AssessmentDate,
            OverallScore = a.OverallScore!.Value,
            LessonTitle = a.LessonPlan?.Title,
            EvaluatorTeacherName = a.EvaluatorTeacher.FullName
        }).ToList();

        // Qualitative highlights
        var strengths = validAssessments
            .Where(a => !string.IsNullOrWhiteSpace(a.Strengths))
            .OrderByDescending(a => a.AssessmentDate)
            .Select(a => a.Strengths!)
            .Take(5)
            .ToList();

        var improvements = validAssessments
            .Where(a => !string.IsNullOrWhiteSpace(a.AreasForImprovement))
            .OrderByDescending(a => a.AssessmentDate)
            .Select(a => a.AreasForImprovement!)
            .Take(5)
            .ToList();

        var feedback = validAssessments
            .Where(a => !string.IsNullOrWhiteSpace(a.GeneralFeedback))
            .OrderByDescending(a => a.AssessmentDate)
            .Take(5)
            .Select(a => new RecentFeedbackItemResponseModel
            {
                Date = a.AssessmentDate,
                TeacherName = a.EvaluatorTeacher.FullName,
                LessonTitle = a.LessonPlan?.Title,
                Feedback = a.GeneralFeedback!,
                OverallScore = a.OverallScore
            })
            .ToList();

        var report = new StudentProgressReportResponseModel
        {
            StudentId = student.StudentId,
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            NickName = student.NickName,
            Age = CalculateAge(student.DateOfBirth),
            CurrentLearningLevel = student.LearningLevel.LevelName,
            ReportGeneratedDate = DateTime.UtcNow,
            TotalSessionsConducted = totalAtt,
            SessionsPresent = presentCount,
            SessionsLate = lateCount,
            SessionsAbsent = absentCount,
            AttendancePercentage = attendancePct,
            TotalAssessments = totalAssessments,
            OverallAverageScore = overallAvg,
            CriteriaBreakdown = allDetails,
            ScoreTimeline = timeline,
            HighlightedStrengths = strengths,
            HighlightedImprovementAreas = improvements,
            RecentTeacherFeedback = feedback
        };

        return Result<StudentProgressReportResponseModel>.Success(report, "Student progress report generated successfully.");
    }

    private static int CalculateAge(DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age)) age--;
        return age;
    }
}
