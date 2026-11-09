using Microsoft.EntityFrameworkCore;
using LittleTalk.Application.Features.Assessments;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.Assessments;

namespace LittleTalk.Application.Features.Assessments.Services;

public class AssessmentService : IAssessmentService
{
    private readonly LittleTalkDbContext _context;
    private readonly AssessmentValidationPolicy _validationPolicy;

    public AssessmentService(
        LittleTalkDbContext context,
        AssessmentValidationPolicy validationPolicy)
    {
        _context = context;
        _validationPolicy = validationPolicy;
    }

    public async Task<Result<PagedListResponseModel<AssessmentSummaryResponseModel>>> GetAssessmentsAsync(
        SearchAssessmentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Assessments
            .AsNoTracking()
            .Include(a => a.Student)
            .Include(a => a.Session)
                .ThenInclude(s => s!.Course)
            .Include(a => a.LessonPlan)
            .Include(a => a.EvaluatorTeacher)
            .Include(a => a.AssessmentDetails)
            .AsQueryable();

        if (request.StudentId.HasValue && request.StudentId.Value > 0)
        {
            query = query.Where(a => a.StudentId == request.StudentId.Value);
        }

        if (request.SessionId.HasValue && request.SessionId.Value > 0)
        {
            query = query.Where(a => a.SessionId == request.SessionId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(a => a.Status.ToLower() == request.Status.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            query = query.Where(a => 
                a.Student.FullName.ToLower().Contains(search) || 
                (a.Student.NickName != null && a.Student.NickName.ToLower().Contains(search)) ||
                a.Student.StudentCode.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.AssessmentDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AssessmentSummaryResponseModel
            {
                AssessmentId = a.AssessmentId,
                StudentId = a.StudentId,
                StudentCode = a.Student.StudentCode,
                StudentFullName = a.Student.FullName,
                StudentNickName = a.Student.NickName,
                SessionId = a.SessionId,
                CourseName = a.Session != null ? a.Session.Course.CourseName : null,
                LessonPlanId = a.LessonPlanId,
                LessonTitle = a.LessonPlan != null ? a.LessonPlan.Title : null,
                EvaluatorTeacherId = a.EvaluatorTeacherId,
                EvaluatorTeacherName = a.EvaluatorTeacher.FullName,
                AssessmentDate = a.AssessmentDate,
                Status = a.Status,
                OverallScore = a.OverallScore,
                CriteriaCount = a.AssessmentDetails.Count,
                GeneralFeedback = a.GeneralFeedback,
                CreatedDate = a.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedList = new PagedListResponseModel<AssessmentSummaryResponseModel>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedListResponseModel<AssessmentSummaryResponseModel>>.Success(pagedList, "Assessments retrieved successfully.");
    }

    public async Task<Result<AssessmentDetailResponseModel>> GetAssessmentByIdAsync(int assessmentId, CancellationToken cancellationToken = default)
    {
        var a = await _context.Assessments
            .AsNoTracking()
            .Include(ass => ass.Student)
            .Include(ass => ass.Session)
                .ThenInclude(s => s!.Course)
            .Include(ass => ass.LessonPlan)
            .Include(ass => ass.EvaluatorTeacher)
            .Include(ass => ass.AssessmentDetails)
            .FirstOrDefaultAsync(ass => ass.AssessmentId == assessmentId, cancellationToken);

        if (a == null)
            return Result<AssessmentDetailResponseModel>.NotFound($"Assessment with ID {assessmentId} was not found.");

        var detail = new AssessmentDetailResponseModel
        {
            AssessmentId = a.AssessmentId,
            StudentId = a.StudentId,
            StudentCode = a.Student.StudentCode,
            StudentFullName = a.Student.FullName,
            StudentNickName = a.Student.NickName,
            SessionId = a.SessionId,
            CourseName = a.Session?.Course?.CourseName,
            LessonPlanId = a.LessonPlanId,
            LessonTitle = a.LessonPlan?.Title,
            EvaluatorTeacherId = a.EvaluatorTeacherId,
            EvaluatorTeacherName = a.EvaluatorTeacher.FullName,
            AssessmentDate = a.AssessmentDate,
            Status = a.Status,
            OverallScore = a.OverallScore,
            CriteriaCount = a.AssessmentDetails.Count,
            GeneralFeedback = a.GeneralFeedback,
            Strengths = a.Strengths,
            AreasForImprovement = a.AreasForImprovement,
            CreatedDate = a.CreatedDate,
            UpdatedDate = a.UpdatedDate,
            Details = a.AssessmentDetails.Select(ad => new CriterionScoreDetailResponseModel
            {
                AssessmentDetailId = ad.AssessmentDetailId,
                CriterionName = ad.CriterionName,
                Score = ad.Score,
                Notes = ad.Notes
            }).ToList()
        };

        return Result<AssessmentDetailResponseModel>.Success(detail, "Assessment details retrieved successfully.");
    }

    public async Task<Result<AssessmentDetailResponseModel>> CreateAssessmentAsync(
        CreateAssessmentRequestModel request, 
        int evaluatorTeacherId, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateCreate(request);
        if (!validation.IsSuccess)
            return Result<AssessmentDetailResponseModel>.ValidationError(validation.Errors, validation.Message);

        var student = await _context.Students.FindAsync(new object[] { request.StudentId }, cancellationToken);
        if (student == null)
            return Result<AssessmentDetailResponseModel>.NotFound($"Student with ID {request.StudentId} was not found.");

        if (request.SessionId.HasValue && request.SessionId.Value > 0)
        {
            var session = await _context.ClassSessions.FindAsync(new object[] { request.SessionId.Value }, cancellationToken);
            if (session == null)
                return Result<AssessmentDetailResponseModel>.NotFound($"Class session with ID {request.SessionId.Value} was not found.");

            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.CourseId == session.CourseId && e.StudentId == request.StudentId && e.Status == "Enrolled", cancellationToken);
            if (!isEnrolled)
            {
                return Result<AssessmentDetailResponseModel>.ValidationError($"Student '{student.FullName}' is not enrolled in the course for this session.");
            }
        }

        decimal? calculatedAverage = request.Criteria.Count > 0
            ? Math.Round(request.Criteria.Average(c => (decimal)c.Score), 2)
            : null;

        var assessment = new Assessment
        {
            StudentId = request.StudentId,
            SessionId = request.SessionId,
            LessonPlanId = request.LessonPlanId,
            EvaluatorTeacherId = evaluatorTeacherId,
            AssessmentDate = request.AssessmentDate,
            Status = request.Status,
            OverallScore = calculatedAverage,
            Strengths = request.Strengths,
            AreasForImprovement = request.AreasForImprovement,
            GeneralFeedback = request.GeneralFeedback,
            CreatedDate = DateTime.UtcNow
        };

        foreach (var c in request.Criteria)
        {
            assessment.AssessmentDetails.Add(new AssessmentDetail
            {
                CriterionName = c.CriterionName,
                Score = c.Score,
                Notes = c.Notes
            });
        }

        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetAssessmentByIdAsync(assessment.AssessmentId, cancellationToken);
    }

    public async Task<Result<AssessmentDetailResponseModel>> UpdateAssessmentAsync(
        int assessmentId, 
        UpdateAssessmentRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateUpdate(request);
        if (!validation.IsSuccess)
            return Result<AssessmentDetailResponseModel>.ValidationError(validation.Errors, validation.Message);

        var assessment = await _context.Assessments
            .Include(a => a.AssessmentDetails)
            .FirstOrDefaultAsync(a => a.AssessmentId == assessmentId, cancellationToken);

        if (assessment == null)
            return Result<AssessmentDetailResponseModel>.NotFound($"Assessment with ID {assessmentId} was not found.");

        decimal? calculatedAverage = request.Criteria.Count > 0
            ? Math.Round(request.Criteria.Average(c => (decimal)c.Score), 2)
            : null;

        assessment.AssessmentDate = request.AssessmentDate;
        assessment.Status = request.Status;
        assessment.OverallScore = calculatedAverage;
        assessment.Strengths = request.Strengths;
        assessment.AreasForImprovement = request.AreasForImprovement;
        assessment.GeneralFeedback = request.GeneralFeedback;
        assessment.UpdatedDate = DateTime.UtcNow;

        _context.AssessmentDetails.RemoveRange(assessment.AssessmentDetails);
        foreach (var c in request.Criteria)
        {
            assessment.AssessmentDetails.Add(new AssessmentDetail
            {
                AssessmentId = assessmentId,
                CriterionName = c.CriterionName,
                Score = c.Score,
                Notes = c.Notes
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await GetAssessmentByIdAsync(assessmentId, cancellationToken);
    }

    public async Task<Result> DeleteAssessmentAsync(int assessmentId, CancellationToken cancellationToken = default)
    {
        var assessment = await _context.Assessments.FindAsync(new object[] { assessmentId }, cancellationToken);
        if (assessment == null)
            return Result.NotFound($"Assessment with ID {assessmentId} was not found.");

        _context.Assessments.Remove(assessment);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Assessment deleted successfully.");
    }

    public async Task<Result<List<AssessmentSummaryResponseModel>>> GetStudentAssessmentsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var items = await _context.Assessments
            .AsNoTracking()
            .Include(a => a.Student)
            .Include(a => a.Session)
                .ThenInclude(s => s!.Course)
            .Include(a => a.LessonPlan)
            .Include(a => a.EvaluatorTeacher)
            .Include(a => a.AssessmentDetails)
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.AssessmentDate)
            .Select(a => new AssessmentSummaryResponseModel
            {
                AssessmentId = a.AssessmentId,
                StudentId = a.StudentId,
                StudentCode = a.Student.StudentCode,
                StudentFullName = a.Student.FullName,
                StudentNickName = a.Student.NickName,
                SessionId = a.SessionId,
                CourseName = a.Session != null ? a.Session.Course.CourseName : null,
                LessonPlanId = a.LessonPlanId,
                LessonTitle = a.LessonPlan != null ? a.LessonPlan.Title : null,
                EvaluatorTeacherId = a.EvaluatorTeacherId,
                EvaluatorTeacherName = a.EvaluatorTeacher.FullName,
                AssessmentDate = a.AssessmentDate,
                Status = a.Status,
                OverallScore = a.OverallScore,
                CriteriaCount = a.AssessmentDetails.Count,
                GeneralFeedback = a.GeneralFeedback,
                CreatedDate = a.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return Result<List<AssessmentSummaryResponseModel>>.Success(items, "Student assessments retrieved successfully.");
    }
}
