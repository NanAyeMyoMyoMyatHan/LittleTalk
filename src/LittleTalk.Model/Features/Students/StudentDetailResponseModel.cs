namespace LittleTalk.Model.Features.Students;

public class StudentDetailResponseModel
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int Age { get; set; }
    public string? Gender { get; set; }
    public int LearningLevelId { get; set; }
    public string LearningLevelName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? ParentName { get; set; }
    public string? ParentContact { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<StudentEnrollmentSummaryModel> Enrollments { get; set; } = new();
}

public class StudentEnrollmentSummaryModel
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class StudentLearningHistoryResponseModel
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<StudentEnrollmentHistoryItemModel> Enrollments { get; set; } = new();
    public List<StudentAssessmentHistoryItemModel> Assessments { get; set; } = new();
}

public class StudentEnrollmentHistoryItemModel
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class StudentAssessmentHistoryItemModel
{
    public int AssessmentId { get; set; }
    public DateTime AssessmentDate { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal? OverallScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public string EvaluatorTeacherName { get; set; } = string.Empty;
}
