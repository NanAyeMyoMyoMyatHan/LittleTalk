using LittleTalk.Model.Common;

namespace LittleTalk.Model.Features.Attendance;

public class SearchClassSessionRequestModel : PaginationRequestModel
{
    public int? CourseId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public string? Status { get; set; }
}

public class CreateClassSessionRequestModel
{
    public int CourseId { get; set; }
    public int? LessonPlanId { get; set; }
    public int TeacherId { get; set; }
    public DateOnly SessionDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? RoomOrLink { get; set; }
    public string? Notes { get; set; }
}

public class UpdateClassSessionRequestModel
{
    public int? LessonPlanId { get; set; }
    public int TeacherId { get; set; }
    public DateOnly SessionDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? RoomOrLink { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string? Notes { get; set; }
}

public class BatchRecordAttendanceRequestModel
{
    public List<StudentAttendanceRecordItemModel> Records { get; set; } = new();
}

public class StudentAttendanceRecordItemModel
{
    public int StudentId { get; set; }
    public string Status { get; set; } = "Present";
    public string? Remarks { get; set; }
}

public class ClassSessionResponseModel
{
    public int SessionId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int? LessonPlanId { get; set; }
    public string? LessonTitle { get; set; }
    public string? LessonTopic { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateOnly SessionDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? RoomOrLink { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int TotalEnrolledStudents { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ClassSessionDetailResponseModel : ClassSessionResponseModel
{
    public List<SessionRosterStudentResponseModel> Roster { get; set; } = new();
}

public class SessionRosterStudentResponseModel
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public int? AttendanceId { get; set; }
    public string? AttendanceStatus { get; set; }
    public string? Remarks { get; set; }
    public DateTime? RecordedDate { get; set; }
}

public class StudentAttendanceHistoryResponseModel
{
    public int AttendanceId { get; set; }
    public int SessionId { get; set; }
    public DateOnly SessionDate { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? LessonTitle { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime RecordedDate { get; set; }
}
