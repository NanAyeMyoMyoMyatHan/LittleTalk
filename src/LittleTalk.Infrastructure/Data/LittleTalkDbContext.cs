using System;
using System.Collections.Generic;
using LittleTalk.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LittleTalk.Infrastructure.Data;

public partial class LittleTalkDbContext : DbContext
{
    public LittleTalkDbContext()
    {
    }

    public LittleTalkDbContext(DbContextOptions<LittleTalkDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ActivityCategory> ActivityCategories { get; set; }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<AssessmentDetail> AssessmentDetails { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<ClassSession> ClassSessions { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseTeacher> CourseTeachers { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<LearningLevel> LearningLevels { get; set; }

    public virtual DbSet<LessonActivity> LessonActivities { get; set; }

    public virtual DbSet<LessonPlan> LessonPlans { get; set; }

    public virtual DbSet<LessonPlanMaterial> LessonPlanMaterials { get; set; }

    public virtual DbSet<LessonStep> LessonSteps { get; set; }

    public virtual DbSet<LessonVocabulary> LessonVocabularies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<TeachingMaterial> TeachingMaterials { get; set; }

    public virtual DbSet<TeachingMethod> TeachingMethods { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwStudentAssessmentSummary> VwStudentAssessmentSummaries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=LittleTalkDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Activities_CategoryId");

            entity.Property(e => e.ActivityName).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DifficultyLevel)
                .HasMaxLength(30)
                .HasDefaultValue("Medium");
            entity.Property(e => e.DurationMinutes).HasDefaultValue(15);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SuitableAgeGroup).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Activities)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activities_Categories");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Activities)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activities_Users");
        });

        modelBuilder.Entity<ActivityCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.HasIndex(e => e.CategoryName, "UQ_ActivityCategories_Name").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(50);
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasIndex(e => e.SessionId, "IX_Assessments_SessionId");

            entity.HasIndex(e => e.StudentId, "IX_Assessments_StudentId");

            entity.Property(e => e.AssessmentDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.OverallScore).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");

            entity.HasOne(d => d.EvaluatorTeacher).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.EvaluatorTeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assessments_Users");

            entity.HasOne(d => d.LessonPlan).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.LessonPlanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Assessments_LessonPlans");

            entity.HasOne(d => d.Session).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("FK_Assessments_ClassSessions");

            entity.HasOne(d => d.Student).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assessments_Students");
        });

        modelBuilder.Entity<AssessmentDetail>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("TR_AssessmentDetails_MaintainOverallScore"));

            entity.HasIndex(e => new { e.AssessmentId, e.CriterionName }, "UQ_AssessmentDetails_Assessment_Criterion").IsUnique();

            entity.Property(e => e.CriterionName).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(250);

            entity.HasOne(d => d.Assessment).WithMany(p => p.AssessmentDetails)
                .HasForeignKey(d => d.AssessmentId)
                .HasConstraintName("FK_AssessmentDetails_Assessments");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendance", tb => tb.HasTrigger("TR_Attendance_ValidateEnrollment"));

            entity.HasIndex(e => e.StudentId, "IX_Attendance_StudentId");

            entity.HasIndex(e => new { e.SessionId, e.StudentId }, "UQ_Attendance_Session_Student").IsUnique();

            entity.Property(e => e.RecordedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Remarks).HasMaxLength(250);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.RecordedByNavigation).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.RecordedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Users");

            entity.HasOne(d => d.Session).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_ClassSessions");

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Students");
        });

        modelBuilder.Entity<ClassSession>(entity =>
        {
            entity.HasKey(e => e.SessionId);

            entity.ToTable(tb => tb.HasTrigger("TR_ClassSessions_ValidateConsistency"));

            entity.HasIndex(e => new { e.CourseId, e.SessionDate }, "IX_ClassSessions_CourseId_Date");

            entity.HasIndex(e => e.TeacherId, "IX_ClassSessions_TeacherId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RoomOrLink).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Scheduled");

            entity.HasOne(d => d.Course).WithMany(p => p.ClassSessions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSessions_Courses");

            entity.HasOne(d => d.LessonPlan).WithMany(p => p.ClassSessions)
                .HasForeignKey(d => d.LessonPlanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ClassSessions_LessonPlans");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ClassSessions)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSessions_Users");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(e => e.LearningLevelId, "IX_Courses_LearningLevelId");

            entity.HasIndex(e => e.CourseCode, "UQ_Courses_CourseCode").IsUnique();

            entity.Property(e => e.CourseCode).HasMaxLength(30);
            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.MaxStudents).HasDefaultValue(15);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.LearningLevel).WithMany(p => p.Courses)
                .HasForeignKey(d => d.LearningLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Courses_LearningLevels");
        });

        modelBuilder.Entity<CourseTeacher>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("TR_CourseTeachers_ValidateTeacherRole"));

            entity.HasIndex(e => e.TeacherId, "IX_CourseTeachers_TeacherId");

            entity.HasIndex(e => new { e.CourseId, e.TeacherId }, "UQ_CourseTeachers_ActiveAssignment")
                .IsUnique()
                .HasFilter("([IsActive]=(1))");

            entity.HasIndex(e => e.CourseId, "UQ_CourseTeachers_SingleActivePrimary")
                .IsUnique()
                .HasFilter("([RoleInCourse]='Primary' AND [IsActive]=(1))");

            entity.Property(e => e.AssignedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleInCourse)
                .HasMaxLength(30)
                .HasDefaultValue("Primary");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseTeachers)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_CourseTeachers_Courses");

            entity.HasOne(d => d.Teacher).WithMany(p => p.CourseTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseTeachers_Users");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasIndex(e => e.StudentId, "IX_Enrollments_StudentId");

            entity.HasIndex(e => new { e.CourseId, e.StudentId }, "UQ_Enrollments_ActiveStudentCourse")
                .IsUnique()
                .HasFilter("([Status]='Enrolled')");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Enrolled");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Courses");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Students");
        });

        modelBuilder.Entity<LearningLevel>(entity =>
        {
            entity.HasIndex(e => e.LevelCode, "UQ_LearningLevels_LevelCode").IsUnique();

            entity.HasIndex(e => e.LevelName, "UQ_LearningLevels_LevelName").IsUnique();

            entity.Property(e => e.AgeRangeDescription).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LevelCode).HasMaxLength(20);
            entity.Property(e => e.LevelName).HasMaxLength(50);
        });

        modelBuilder.Entity<LessonActivity>(entity =>
        {
            entity.HasIndex(e => e.ActivityId, "IX_LessonActivities_ActivityId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.StepOrder).HasDefaultValue(1);

            entity.HasOne(d => d.Activity).WithMany(p => p.LessonActivities)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LessonActivities_Activities");

            entity.HasOne(d => d.LessonPlan).WithMany(p => p.LessonActivities)
                .HasForeignKey(d => d.LessonPlanId)
                .HasConstraintName("FK_LessonActivities_LessonPlans");

            entity.HasOne(d => d.LessonStep).WithMany(p => p.LessonActivities)
                .HasPrincipalKey(p => new { p.LessonPlanId, p.LessonStepId })
                .HasForeignKey(d => new { d.LessonPlanId, d.LessonStepId })
                .HasConstraintName("FK_LessonActivities_LessonSteps");
        });

        modelBuilder.Entity<LessonPlan>(entity =>
        {
            entity.HasIndex(e => e.CourseId, "IX_LessonPlans_CourseId");

            entity.HasIndex(e => e.CreatedBy, "IX_LessonPlans_CreatedBy");

            entity.HasIndex(e => e.LearningLevelId, "IX_LessonPlans_LearningLevelId");

            entity.Property(e => e.AgeGroup).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DurationMinutes).HasDefaultValue(45);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.Topic).HasMaxLength(100);

            entity.HasOne(d => d.Course).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_LessonPlans_Courses");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LessonPlans_Users");

            entity.HasOne(d => d.LearningLevel).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.LearningLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LessonPlans_LearningLevels");
        });

        modelBuilder.Entity<LessonPlanMaterial>(entity =>
        {
            entity.HasIndex(e => e.MaterialId, "IX_LessonPlanMaterials_MaterialId");

            entity.HasIndex(e => new { e.LessonPlanId, e.MaterialId }, "UQ_LessonPlanMaterials_Plan_Material").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.SortOrder).HasDefaultValue(1);
            entity.Property(e => e.UsageInstructions).HasMaxLength(500);

            entity.HasOne(d => d.LessonPlan).WithMany(p => p.LessonPlanMaterials)
                .HasForeignKey(d => d.LessonPlanId)
                .HasConstraintName("FK_LessonPlanMaterials_LessonPlans");

            entity.HasOne(d => d.Material).WithMany(p => p.LessonPlanMaterials)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LessonPlanMaterials_TeachingMaterials");
        });

        modelBuilder.Entity<LessonStep>(entity =>
        {
            entity.HasIndex(e => new { e.LessonPlanId, e.SortOrder }, "IX_LessonSteps_LessonPlanId_Sort");

            entity.HasIndex(e => new { e.LessonPlanId, e.LessonStepId }, "UQ_LessonSteps_Plan_Step").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.SortOrder).HasDefaultValue(1);
            entity.Property(e => e.StepName).HasMaxLength(100);
            entity.Property(e => e.TeachingMethodCustom).HasMaxLength(100);

            entity.HasOne(d => d.LessonPlan).WithMany(p => p.LessonSteps)
                .HasForeignKey(d => d.LessonPlanId)
                .HasConstraintName("FK_LessonSteps_LessonPlans");

            entity.HasOne(d => d.TeachingMethod).WithMany(p => p.LessonSteps)
                .HasForeignKey(d => d.TeachingMethodId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_LessonSteps_TeachingMethods");
        });

        modelBuilder.Entity<LessonVocabulary>(entity =>
        {
            entity.HasKey(e => e.VocabularyId);

            entity.ToTable("LessonVocabulary");

            entity.HasIndex(e => e.LessonPlanId, "IX_LessonVocabulary_LessonPlanId");

            entity.Property(e => e.AudioUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ExampleSentence).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Meaning).HasMaxLength(250);
            entity.Property(e => e.Pronunciation).HasMaxLength(100);
            entity.Property(e => e.SortOrder).HasDefaultValue(1);
            entity.Property(e => e.Topic).HasMaxLength(100);
            entity.Property(e => e.Word).HasMaxLength(100);

            entity.HasOne(d => d.LessonPlan).WithMany(p => p.LessonVocabularies)
                .HasForeignKey(d => d.LessonPlanId)
                .HasConstraintName("FK_LessonVocabulary_LessonPlans");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.RoleName, "UQ_Roles_RoleName").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.FullName, "IX_Students_FullName");

            entity.HasIndex(e => e.LearningLevelId, "IX_Students_LearningLevelId");

            entity.HasIndex(e => e.StudentCode, "UQ_Students_StudentCode").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(15);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NickName).HasMaxLength(50);
            entity.Property(e => e.ParentContact).HasMaxLength(50);
            entity.Property(e => e.ParentName).HasMaxLength(100);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            entity.Property(e => e.StudentCode).HasMaxLength(30);

            entity.HasOne(d => d.LearningLevel).WithMany(p => p.Students)
                .HasForeignKey(d => d.LearningLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_LearningLevels");
        });

        modelBuilder.Entity<TeachingMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId);

            entity.HasIndex(e => e.LearningLevelId, "IX_TeachingMaterials_LearningLevelId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaterialName).HasMaxLength(150);
            entity.Property(e => e.MaterialType).HasMaxLength(50);
            entity.Property(e => e.SuitableAgeGroup).HasMaxLength(50);

            entity.HasOne(d => d.LearningLevel).WithMany(p => p.TeachingMaterials)
                .HasForeignKey(d => d.LearningLevelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TeachingMaterials_LearningLevels");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.TeachingMaterials)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeachingMaterials_Users");
        });

        modelBuilder.Entity<TeachingMethod>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_TeachingMethods_Name").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.SuitableAgeGroup).HasMaxLength(50);

            entity.HasOne(d => d.SuitableLearningLevel).WithMany(p => p.TeachingMethods)
                .HasForeignKey(d => d.SuitableLearningLevelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TeachingMethods_LearningLevels");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Users_Username").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<VwStudentAssessmentSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StudentAssessmentSummaries");

            entity.Property(e => e.AssessmentStatus).HasMaxLength(20);
            entity.Property(e => e.CalculatedOverallScore).HasColumnType("decimal(38, 6)");
            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.EvaluatorTeacherName).HasMaxLength(100);
            entity.Property(e => e.LessonTitle).HasMaxLength(150);
            entity.Property(e => e.StoredOverallScore).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.StudentCode).HasMaxLength(30);
            entity.Property(e => e.StudentFullName).HasMaxLength(100);
            entity.Property(e => e.StudentNickName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
