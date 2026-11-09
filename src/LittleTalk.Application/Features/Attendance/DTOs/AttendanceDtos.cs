namespace LittleTalk.Application.Features.Attendance.DTOs;

public class ClassSessionDto
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
    public string Status { get; set; } = string.Empty; // Scheduled, InProgress, Completed, Cancelled
    public string? Notes { get; set; }
    public int TotalEnrolledStudents { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ClassSessionDetailDto : ClassSessionDto
{
    public List<SessionRosterStudentDto> Roster { get; set; } = new();
}

public class SessionRosterStudentDto
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public int? AttendanceId { get; set; }
    public string? AttendanceStatus { get; set; } // Present, Absent, Late, Excused, or null if not yet marked
    public string? Remarks { get; set; }
    public DateTime? RecordedDate { get; set; }
}

public class CreateClassSessionDto
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

public class UpdateClassSessionDto
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

public class BatchRecordAttendanceDto
{
    public List<StudentAttendanceRecordItemDto> Records { get; set; } = new();
}

public class StudentAttendanceRecordItemDto
{
    public int StudentId { get; set; }
    public string Status { get; set; } = "Present"; // Present, Absent, Late, Excused
    public string? Remarks { get; set; }
}

public class StudentAttendanceHistoryItemDto
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
