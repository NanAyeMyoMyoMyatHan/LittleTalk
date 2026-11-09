namespace LittleTalk.Application.Features.Students.DTOs;

public class StudentSummaryDto
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year - (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    public string? Gender { get; set; }
    public int LearningLevelId { get; set; }
    public string LearningLevelName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int ActiveEnrollmentsCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class StudentDetailDto : StudentSummaryDto
{
    public string? ParentName { get; set; }
    public string? ParentContact { get; set; }
    public string? EmergencyNotes { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<StudentCourseEnrollmentDto> Enrollments { get; set; } = new();
}

public class StudentCourseEnrollmentDto
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class CreateStudentDto
{
    public string? StudentCode { get; set; } // If null, auto-generated e.g. LT-STU-XXXX
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public int LearningLevelId { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? ParentName { get; set; }
    public string? ParentContact { get; set; }
    public string? EmergencyNotes { get; set; }
}

public class UpdateStudentDto
{
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public int LearningLevelId { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? ParentName { get; set; }
    public string? ParentContact { get; set; }
    public string? EmergencyNotes { get; set; }
    public bool IsActive { get; set; } = true;
}

public class StudentLearningHistoryDto
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public string CurrentLevelName { get; set; } = string.Empty;
    public int TotalSessionsAttended { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int TotalAssessmentsCount { get; set; }
    public decimal AverageOverallScore { get; set; }
    public List<StudentAssessmentHistoryItemDto> RecentAssessments { get; set; } = new();
    public List<StudentCourseEnrollmentDto> EnrolledCourses { get; set; } = new();
}

public class StudentAssessmentHistoryItemDto
{
    public int AssessmentId { get; set; }
    public DateTime AssessmentDate { get; set; }
    public string? CourseName { get; set; }
    public string? LessonTitle { get; set; }
    public string EvaluatorTeacherName { get; set; } = string.Empty;
    public decimal? OverallScore { get; set; }
    public string? GeneralFeedback { get; set; }
    public string Status { get; set; } = string.Empty;
}
