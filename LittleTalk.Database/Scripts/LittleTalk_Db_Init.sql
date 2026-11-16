-- ============================================================================
-- LittleTalk Database Initialization Script
-- Engine: Microsoft SQL Server 2022+ / 2025
-- Architecture: Database-First Normalized Schema (20 Tables)
-- ============================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LittleTalkDb')
BEGIN
    CREATE DATABASE [LittleTalkDb] COLLATE SQL_Latin1_General_CP1_CI_AS;
END
GO

USE [LittleTalkDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- ----------------------------------------------------------------------------
-- Drop existing objects in reverse order if re-initializing
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.vw_StudentAssessmentSummaries', N'V') IS NOT NULL DROP VIEW dbo.vw_StudentAssessmentSummaries;
IF OBJECT_ID(N'dbo.TR_Attendance_ValidateEnrollment', N'TR') IS NOT NULL DROP TRIGGER dbo.TR_Attendance_ValidateEnrollment;
IF OBJECT_ID(N'dbo.TR_AssessmentDetails_MaintainOverallScore', N'TR') IS NOT NULL DROP TRIGGER dbo.TR_AssessmentDetails_MaintainOverallScore;
IF OBJECT_ID(N'dbo.TR_ClassSessions_ValidateConsistency', N'TR') IS NOT NULL DROP TRIGGER dbo.TR_ClassSessions_ValidateConsistency;
IF OBJECT_ID(N'dbo.TR_CourseTeachers_ValidateTeacherRole', N'TR') IS NOT NULL DROP TRIGGER dbo.TR_CourseTeachers_ValidateTeacherRole;

IF OBJECT_ID(N'dbo.AssessmentDetails', N'U') IS NOT NULL DROP TABLE dbo.AssessmentDetails;
IF OBJECT_ID(N'dbo.Assessments', N'U') IS NOT NULL DROP TABLE dbo.Assessments;
IF OBJECT_ID(N'dbo.Attendance', N'U') IS NOT NULL DROP TABLE dbo.Attendance;
IF OBJECT_ID(N'dbo.ClassSessions', N'U') IS NOT NULL DROP TABLE dbo.ClassSessions;
IF OBJECT_ID(N'dbo.LessonActivities', N'U') IS NOT NULL DROP TABLE dbo.LessonActivities;
IF OBJECT_ID(N'dbo.Activities', N'U') IS NOT NULL DROP TABLE dbo.Activities;
IF OBJECT_ID(N'dbo.ActivityCategories', N'U') IS NOT NULL DROP TABLE dbo.ActivityCategories;
IF OBJECT_ID(N'dbo.LessonVocabulary', N'U') IS NOT NULL DROP TABLE dbo.LessonVocabulary;
IF OBJECT_ID(N'dbo.LessonPlanMaterials', N'U') IS NOT NULL DROP TABLE dbo.LessonPlanMaterials;
IF OBJECT_ID(N'dbo.TeachingMaterials', N'U') IS NOT NULL DROP TABLE dbo.TeachingMaterials;
IF OBJECT_ID(N'dbo.LessonSteps', N'U') IS NOT NULL DROP TABLE dbo.LessonSteps;
IF OBJECT_ID(N'dbo.TeachingMethods', N'U') IS NOT NULL DROP TABLE dbo.TeachingMethods;
IF OBJECT_ID(N'dbo.LessonPlans', N'U') IS NOT NULL DROP TABLE dbo.LessonPlans;
IF OBJECT_ID(N'dbo.Enrollments', N'U') IS NOT NULL DROP TABLE dbo.Enrollments;
IF OBJECT_ID(N'dbo.CourseTeachers', N'U') IS NOT NULL DROP TABLE dbo.CourseTeachers;
IF OBJECT_ID(N'dbo.Courses', N'U') IS NOT NULL DROP TABLE dbo.Courses;
IF OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL DROP TABLE dbo.Students;
IF OBJECT_ID(N'dbo.LearningLevels', N'U') IS NOT NULL DROP TABLE dbo.LearningLevels;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- ============================================================================
-- 1. Identity & Authorization Domain
-- ============================================================================

CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) NOT NULL,
    RoleName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(250) NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Roles_CreatedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId),
    CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName),
    CONSTRAINT CK_Roles_RoleName CHECK (RoleName IN ('Admin', 'Teacher', 'Parent'))
);
GO

CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) NOT NULL,
    RoleId INT NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    ProfileImageUrl NVARCHAR(500) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
    FailedLoginAttempts INT NOT NULL CONSTRAINT DF_Users_FailedLoginAttempts DEFAULT 0,
    LockoutEnd DATETIME2 NULL,
    LastLoginDate DATETIME2 NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_FailedLoginAttempts CHECK (FailedLoginAttempts >= 0),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles (RoleId) ON DELETE NO ACTION
);
GO

-- ============================================================================
-- 2. Academic Hierarchy & Student Domain
-- ============================================================================

CREATE TABLE dbo.LearningLevels (
    LearningLevelId INT IDENTITY(1,1) NOT NULL,
    LevelName NVARCHAR(50) NOT NULL,
    LevelCode NVARCHAR(20) NOT NULL,
    AgeRangeDescription NVARCHAR(50) NULL,
    Description NVARCHAR(500) NULL,
    SortOrder INT NOT NULL CONSTRAINT DF_LearningLevels_SortOrder DEFAULT 0,
    IsActive BIT NOT NULL CONSTRAINT DF_LearningLevels_IsActive DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_LearningLevels_CreatedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_LearningLevels PRIMARY KEY CLUSTERED (LearningLevelId),
    CONSTRAINT UQ_LearningLevels_LevelName UNIQUE (LevelName),
    CONSTRAINT UQ_LearningLevels_LevelCode UNIQUE (LevelCode),
    CONSTRAINT CK_LearningLevels_SortOrder CHECK (SortOrder >= 0)
);
GO

CREATE TABLE dbo.Students (
    StudentId INT IDENTITY(1,1) NOT NULL,
    StudentCode NVARCHAR(30) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    NickName NVARCHAR(50) NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(15) NULL,
    LearningLevelId INT NOT NULL,
    ProfileImageUrl NVARCHAR(500) NULL,
    ParentName NVARCHAR(100) NULL,
    ParentContact NVARCHAR(50) NULL,
    EmergencyNotes NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Students_IsActive DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Students_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_Students PRIMARY KEY CLUSTERED (StudentId),
    CONSTRAINT UQ_Students_StudentCode UNIQUE (StudentCode),
    CONSTRAINT CK_Students_Gender CHECK (Gender IS NULL OR Gender IN ('Boy', 'Girl', 'Prefer not to say')),
    CONSTRAINT FK_Students_LearningLevels FOREIGN KEY (LearningLevelId) REFERENCES dbo.LearningLevels (LearningLevelId) ON DELETE NO ACTION
);
GO

-- ============================================================================
-- 3. Courses, Course Staffing & Enrollments
-- ============================================================================

CREATE TABLE dbo.Courses (
    CourseId INT IDENTITY(1,1) NOT NULL,
    CourseCode NVARCHAR(30) NOT NULL,
    CourseName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    LearningLevelId INT NOT NULL,
    DurationWeeks INT NULL,
    MaxStudents INT NOT NULL CONSTRAINT DF_Courses_MaxStudents DEFAULT 15,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Courses_Status DEFAULT 'Active',
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Courses_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_Courses PRIMARY KEY CLUSTERED (CourseId),
    CONSTRAINT UQ_Courses_CourseCode UNIQUE (CourseCode),
    CONSTRAINT CK_Courses_MaxStudents CHECK (MaxStudents > 0),
    CONSTRAINT CK_Courses_DurationWeeks CHECK (DurationWeeks IS NULL OR DurationWeeks > 0),
    CONSTRAINT CK_Courses_Status CHECK (Status IN ('Active', 'Draft', 'Completed', 'Archived')),
    CONSTRAINT FK_Courses_LearningLevels FOREIGN KEY (LearningLevelId) REFERENCES dbo.LearningLevels (LearningLevelId) ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.CourseTeachers (
    CourseTeacherId INT IDENTITY(1,1) NOT NULL,
    CourseId INT NOT NULL,
    TeacherId INT NOT NULL,
    RoleInCourse NVARCHAR(30) NOT NULL CONSTRAINT DF_CourseTeachers_Role DEFAULT 'Primary',
    AssignedDate DATETIME2 NOT NULL CONSTRAINT DF_CourseTeachers_AssignedDate DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL CONSTRAINT DF_CourseTeachers_IsActive DEFAULT 1,
    CONSTRAINT PK_CourseTeachers PRIMARY KEY CLUSTERED (CourseTeacherId),
    CONSTRAINT CK_CourseTeachers_Role CHECK (RoleInCourse IN ('Primary', 'CoTeacher', 'Assistant', 'Substitute')),
    CONSTRAINT FK_CourseTeachers_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses (CourseId) ON DELETE CASCADE,
    CONSTRAINT FK_CourseTeachers_Users FOREIGN KEY (TeacherId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_CourseTeachers_SingleActivePrimary
ON dbo.CourseTeachers (CourseId)
WHERE RoleInCourse = 'Primary' AND IsActive = 1;
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_CourseTeachers_ActiveAssignment
ON dbo.CourseTeachers (CourseId, TeacherId)
WHERE IsActive = 1;
GO

CREATE TABLE dbo.Enrollments (
    EnrollmentId INT IDENTITY(1,1) NOT NULL,
    CourseId INT NOT NULL,
    StudentId INT NOT NULL,
    EnrollmentDate DATETIME2 NOT NULL CONSTRAINT DF_Enrollments_EnrollmentDate DEFAULT GETUTCDATE(),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Enrollments_Status DEFAULT 'Enrolled',
    CompletedDate DATETIME2 NULL,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Enrollments_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_Enrollments PRIMARY KEY CLUSTERED (EnrollmentId),
    CONSTRAINT CK_Enrollments_Status CHECK (Status IN ('Enrolled', 'Completed', 'Dropped', 'Transferred')),
    CONSTRAINT CK_Enrollments_CompletedDate CHECK (CompletedDate IS NULL OR CompletedDate >= EnrollmentDate),
    CONSTRAINT FK_Enrollments_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses (CourseId) ON DELETE NO ACTION,
    CONSTRAINT FK_Enrollments_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students (StudentId) ON DELETE NO ACTION
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_Enrollments_ActiveStudentCourse
ON dbo.Enrollments (CourseId, StudentId)
WHERE Status = 'Enrolled';
GO

-- ============================================================================
-- 4. Lesson Plans, Steps, Pedagogy & Materials Library
-- ============================================================================

CREATE TABLE dbo.LessonPlans (
    LessonPlanId INT IDENTITY(1,1) NOT NULL,
    CourseId INT NULL,
    Title NVARCHAR(150) NOT NULL,
    Topic NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    LearningLevelId INT NOT NULL,
    AgeGroup NVARCHAR(50) NOT NULL,
    DurationMinutes INT NOT NULL CONSTRAINT DF_LessonPlans_Duration DEFAULT 45,
    LearningObjectives NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_LessonPlans_Status DEFAULT 'Draft',
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_LessonPlans_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_LessonPlans PRIMARY KEY CLUSTERED (LessonPlanId),
    CONSTRAINT CK_LessonPlans_Duration CHECK (DurationMinutes > 0),
    CONSTRAINT CK_LessonPlans_Status CHECK (Status IN ('Draft', 'Published', 'Completed', 'Archived')),
    CONSTRAINT FK_LessonPlans_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses (CourseId) ON DELETE SET NULL,
    CONSTRAINT FK_LessonPlans_LearningLevels FOREIGN KEY (LearningLevelId) REFERENCES dbo.LearningLevels (LearningLevelId) ON DELETE NO ACTION,
    CONSTRAINT FK_LessonPlans_Users FOREIGN KEY (CreatedBy) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.TeachingMethods (
    TeachingMethodId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    TeachingInstructions NVARCHAR(MAX) NOT NULL,
    SuitableAgeGroup NVARCHAR(50) NULL,
    SuitableLearningLevelId INT NULL,
    RequiredMaterials NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_TeachingMethods_IsActive DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_TeachingMethods_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_TeachingMethods PRIMARY KEY CLUSTERED (TeachingMethodId),
    CONSTRAINT UQ_TeachingMethods_Name UNIQUE (Name),
    CONSTRAINT FK_TeachingMethods_LearningLevels FOREIGN KEY (SuitableLearningLevelId) REFERENCES dbo.LearningLevels (LearningLevelId) ON DELETE SET NULL
);
GO

CREATE TABLE dbo.LessonSteps (
    LessonStepId INT IDENTITY(1,1) NOT NULL,
    LessonPlanId INT NOT NULL,
    StepName NVARCHAR(100) NOT NULL,
    DurationMinutes INT NOT NULL,
    TeachingMethodId INT NULL,
    TeachingMethodCustom NVARCHAR(100) NULL,
    Instructions NVARCHAR(MAX) NOT NULL,
    TeacherScript NVARCHAR(MAX) NULL,
    ExpectedStudentResponse NVARCHAR(MAX) NULL,
    SortOrder INT NOT NULL CONSTRAINT DF_LessonSteps_SortOrder DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_LessonSteps_CreatedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_LessonSteps PRIMARY KEY CLUSTERED (LessonStepId),
    CONSTRAINT CK_LessonSteps_Duration CHECK (DurationMinutes > 0),
    CONSTRAINT CK_LessonSteps_SortOrder CHECK (SortOrder >= 1),
    CONSTRAINT UQ_LessonSteps_Plan_Step UNIQUE (LessonPlanId, LessonStepId),
    CONSTRAINT FK_LessonSteps_LessonPlans FOREIGN KEY (LessonPlanId) REFERENCES dbo.LessonPlans (LessonPlanId) ON DELETE CASCADE,
    CONSTRAINT FK_LessonSteps_TeachingMethods FOREIGN KEY (TeachingMethodId) REFERENCES dbo.TeachingMethods (TeachingMethodId) ON DELETE SET NULL
);
GO

CREATE TABLE dbo.TeachingMaterials (
    MaterialId INT IDENTITY(1,1) NOT NULL,
    MaterialName NVARCHAR(150) NOT NULL,
    MaterialType NVARCHAR(50) NOT NULL,
    FileUrl NVARCHAR(500) NULL,
    Description NVARCHAR(500) NULL,
    SuitableAgeGroup NVARCHAR(50) NULL,
    LearningLevelId INT NULL,
    UploadedBy INT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_TeachingMaterials_IsActive DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_TeachingMaterials_CreatedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_TeachingMaterials PRIMARY KEY CLUSTERED (MaterialId),
    CONSTRAINT CK_TeachingMaterials_Type CHECK (MaterialType IN ('Flashcard', 'Audio', 'Slide', 'Worksheet', 'Prop', 'Video')),
    CONSTRAINT FK_TeachingMaterials_LearningLevels FOREIGN KEY (LearningLevelId) REFERENCES dbo.LearningLevels (LearningLevelId) ON DELETE SET NULL,
    CONSTRAINT FK_TeachingMaterials_Users FOREIGN KEY (UploadedBy) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.LessonPlanMaterials (
    LessonPlanMaterialId INT IDENTITY(1,1) NOT NULL,
    LessonPlanId INT NOT NULL,
    MaterialId INT NOT NULL,
    UsageInstructions NVARCHAR(500) NULL,
    SortOrder INT NOT NULL CONSTRAINT DF_LessonPlanMaterials_SortOrder DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_LessonPlanMaterials_CreatedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_LessonPlanMaterials PRIMARY KEY CLUSTERED (LessonPlanMaterialId),
    CONSTRAINT UQ_LessonPlanMaterials_Plan_Material UNIQUE (LessonPlanId, MaterialId),
    CONSTRAINT CK_LessonPlanMaterials_SortOrder CHECK (SortOrder >= 1),
    CONSTRAINT FK_LessonPlanMaterials_LessonPlans FOREIGN KEY (LessonPlanId) REFERENCES dbo.LessonPlans (LessonPlanId) ON DELETE CASCADE,
    CONSTRAINT FK_LessonPlanMaterials_TeachingMaterials FOREIGN KEY (MaterialId) REFERENCES dbo.TeachingMaterials (MaterialId) ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.LessonVocabulary (
    VocabularyId INT IDENTITY(1,1) NOT NULL,
    LessonPlanId INT NOT NULL,
    Word NVARCHAR(100) NOT NULL,
    Meaning NVARCHAR(250) NOT NULL,
    Pronunciation NVARCHAR(100) NULL,
    ExampleSentence NVARCHAR(500) NOT NULL,
    Topic NVARCHAR(100) NULL,
    ImageUrl NVARCHAR(500) NULL,
    AudioUrl NVARCHAR(500) NULL,
    SortOrder INT NOT NULL CONSTRAINT DF_LessonVocabulary_SortOrder DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_LessonVocabulary_CreatedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_LessonVocabulary PRIMARY KEY CLUSTERED (VocabularyId),
    CONSTRAINT CK_LessonVocabulary_SortOrder CHECK (SortOrder >= 1),
    CONSTRAINT FK_LessonVocabulary_LessonPlans FOREIGN KEY (LessonPlanId) REFERENCES dbo.LessonPlans (LessonPlanId) ON DELETE CASCADE
);
GO

-- ============================================================================
-- 5. Speaking Activities Bank & Step Linking
-- ============================================================================

CREATE TABLE dbo.ActivityCategories (
    CategoryId INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Icon NVARCHAR(50) NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_ActivityCategories_CreatedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_ActivityCategories PRIMARY KEY CLUSTERED (CategoryId),
    CONSTRAINT UQ_ActivityCategories_Name UNIQUE (CategoryName),
    CONSTRAINT CK_ActivityCategories_Name CHECK (CategoryName IN ('Vocabulary Games', 'Role-play', 'Pronunciation Practice', 'Question and Answer', 'Storytelling', 'Pair Conversation', 'Group Conversation'))
);
GO

CREATE TABLE dbo.Activities (
    ActivityId INT IDENTITY(1,1) NOT NULL,
    CategoryId INT NOT NULL,
    ActivityName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Instructions NVARCHAR(MAX) NOT NULL,
    SuitableAgeGroup NVARCHAR(50) NULL,
    DifficultyLevel NVARCHAR(30) NOT NULL CONSTRAINT DF_Activities_Difficulty DEFAULT 'Medium',
    DurationMinutes INT NOT NULL CONSTRAINT DF_Activities_Duration DEFAULT 15,
    RequiredMaterials NVARCHAR(MAX) NULL,
    ExpectedLearningOutcome NVARCHAR(MAX) NULL,
    CreatedBy INT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Activities_IsActive DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Activities_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_Activities PRIMARY KEY CLUSTERED (ActivityId),
    CONSTRAINT CK_Activities_Difficulty CHECK (DifficultyLevel IN ('Easy', 'Medium', 'Challenging')),
    CONSTRAINT CK_Activities_Duration CHECK (DurationMinutes > 0),
    CONSTRAINT FK_Activities_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.ActivityCategories (CategoryId) ON DELETE NO ACTION,
    CONSTRAINT FK_Activities_Users FOREIGN KEY (CreatedBy) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.LessonActivities (
    LessonActivityId INT IDENTITY(1,1) NOT NULL,
    LessonPlanId INT NOT NULL,
    ActivityId INT NOT NULL,
    LessonStepId INT NULL,
    StepOrder INT NOT NULL CONSTRAINT DF_LessonActivities_StepOrder DEFAULT 1,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_LessonActivities_CreatedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_LessonActivities PRIMARY KEY CLUSTERED (LessonActivityId),
    CONSTRAINT CK_LessonActivities_StepOrder CHECK (StepOrder >= 1),
    CONSTRAINT FK_LessonActivities_LessonPlans FOREIGN KEY (LessonPlanId) REFERENCES dbo.LessonPlans (LessonPlanId) ON DELETE CASCADE,
    CONSTRAINT FK_LessonActivities_Activities FOREIGN KEY (ActivityId) REFERENCES dbo.Activities (ActivityId) ON DELETE NO ACTION,
    CONSTRAINT FK_LessonActivities_LessonSteps FOREIGN KEY (LessonPlanId, LessonStepId) REFERENCES dbo.LessonSteps (LessonPlanId, LessonStepId) ON DELETE NO ACTION
);
GO

-- ============================================================================
-- 6. Classroom Delivery, Attendance & Speaking Assessments
-- ============================================================================

CREATE TABLE dbo.ClassSessions (
    SessionId INT IDENTITY(1,1) NOT NULL,
    CourseId INT NOT NULL,
    LessonPlanId INT NULL,
    TeacherId INT NOT NULL,
    SessionDate DATE NOT NULL,
    StartTime TIME NULL,
    EndTime TIME NULL,
    RoomOrLink NVARCHAR(100) NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_ClassSessions_Status DEFAULT 'Scheduled',
    Notes NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_ClassSessions_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_ClassSessions PRIMARY KEY CLUSTERED (SessionId),
    CONSTRAINT CK_ClassSessions_Status CHECK (Status IN ('Scheduled', 'InProgress', 'Completed', 'Cancelled')),
    CONSTRAINT CK_ClassSessions_Times CHECK ((StartTime IS NULL AND EndTime IS NULL) OR (StartTime IS NOT NULL AND EndTime IS NOT NULL AND EndTime > StartTime)),
    CONSTRAINT FK_ClassSessions_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses (CourseId) ON DELETE NO ACTION,
    CONSTRAINT FK_ClassSessions_LessonPlans FOREIGN KEY (LessonPlanId) REFERENCES dbo.LessonPlans (LessonPlanId) ON DELETE SET NULL,
    CONSTRAINT FK_ClassSessions_Users FOREIGN KEY (TeacherId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.Attendance (
    AttendanceId INT IDENTITY(1,1) NOT NULL,
    SessionId INT NOT NULL,
    StudentId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Remarks NVARCHAR(250) NULL,
    RecordedBy INT NOT NULL,
    RecordedDate DATETIME2 NOT NULL CONSTRAINT DF_Attendance_RecordedDate DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Attendance PRIMARY KEY CLUSTERED (AttendanceId),
    CONSTRAINT UQ_Attendance_Session_Student UNIQUE (SessionId, StudentId),
    CONSTRAINT CK_Attendance_Status CHECK (Status IN ('Present', 'Absent', 'Late', 'Excused')),
    CONSTRAINT FK_Attendance_ClassSessions FOREIGN KEY (SessionId) REFERENCES dbo.ClassSessions (SessionId) ON DELETE NO ACTION,
    CONSTRAINT FK_Attendance_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students (StudentId) ON DELETE NO ACTION,
    CONSTRAINT FK_Attendance_Users FOREIGN KEY (RecordedBy) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.Assessments (
    AssessmentId INT IDENTITY(1,1) NOT NULL,
    StudentId INT NOT NULL,
    SessionId INT NULL,
    LessonPlanId INT NULL,
    EvaluatorTeacherId INT NOT NULL,
    AssessmentDate DATETIME2 NOT NULL CONSTRAINT DF_Assessments_AssessmentDate DEFAULT GETUTCDATE(),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Assessments_Status DEFAULT 'Draft',
    OverallScore DECIMAL(3,2) NULL,
    Strengths NVARCHAR(MAX) NULL,
    AreasForImprovement NVARCHAR(MAX) NULL,
    GeneralFeedback NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Assessments_CreatedDate DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT PK_Assessments PRIMARY KEY CLUSTERED (AssessmentId),
    CONSTRAINT CK_Assessments_Status CHECK (Status IN ('Draft', 'Finalized')),
    CONSTRAINT CK_Assessments_OverallScore CHECK (OverallScore IS NULL OR (OverallScore >= 1.00 AND OverallScore <= 5.00)),
    CONSTRAINT CK_Assessments_AssessmentDate CHECK (AssessmentDate <= GETUTCDATE()),
    CONSTRAINT FK_Assessments_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students (StudentId) ON DELETE NO ACTION,
    CONSTRAINT FK_Assessments_ClassSessions FOREIGN KEY (SessionId) REFERENCES dbo.ClassSessions (SessionId) ON DELETE NO ACTION,
    CONSTRAINT FK_Assessments_LessonPlans FOREIGN KEY (LessonPlanId) REFERENCES dbo.LessonPlans (LessonPlanId) ON DELETE SET NULL,
    CONSTRAINT FK_Assessments_Users FOREIGN KEY (EvaluatorTeacherId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.AssessmentDetails (
    AssessmentDetailId INT IDENTITY(1,1) NOT NULL,
    AssessmentId INT NOT NULL,
    CriterionName NVARCHAR(50) NOT NULL,
    Score INT NOT NULL,
    Notes NVARCHAR(250) NULL,
    CONSTRAINT PK_AssessmentDetails PRIMARY KEY CLUSTERED (AssessmentDetailId),
    CONSTRAINT UQ_AssessmentDetails_Assessment_Criterion UNIQUE (AssessmentId, CriterionName),
    CONSTRAINT CK_AssessmentDetails_CriterionName CHECK (CriterionName IN ('Pronunciation', 'Vocabulary', 'Fluency', 'Sentence Formation', 'Confidence', 'Listening and Responding')),
    CONSTRAINT CK_AssessmentDetails_Score CHECK (Score BETWEEN 1 AND 5),
    CONSTRAINT FK_AssessmentDetails_Assessments FOREIGN KEY (AssessmentId) REFERENCES dbo.Assessments (AssessmentId) ON DELETE CASCADE
);
GO

-- ============================================================================
-- 7. Performance & Foreign Key Indexes
-- ============================================================================

CREATE NONCLUSTERED INDEX IX_Users_RoleId ON dbo.Users (RoleId);
CREATE NONCLUSTERED INDEX IX_Students_LearningLevelId ON dbo.Students (LearningLevelId);
CREATE NONCLUSTERED INDEX IX_Students_FullName ON dbo.Students (FullName);
CREATE NONCLUSTERED INDEX IX_Courses_LearningLevelId ON dbo.Courses (LearningLevelId);
CREATE NONCLUSTERED INDEX IX_CourseTeachers_TeacherId ON dbo.CourseTeachers (TeacherId);
CREATE NONCLUSTERED INDEX IX_Enrollments_StudentId ON dbo.Enrollments (StudentId);
CREATE NONCLUSTERED INDEX IX_LessonPlans_CourseId ON dbo.LessonPlans (CourseId);
CREATE NONCLUSTERED INDEX IX_LessonPlans_LearningLevelId ON dbo.LessonPlans (LearningLevelId);
CREATE NONCLUSTERED INDEX IX_LessonPlans_CreatedBy ON dbo.LessonPlans (CreatedBy);
CREATE NONCLUSTERED INDEX IX_LessonSteps_LessonPlanId_Sort ON dbo.LessonSteps (LessonPlanId, SortOrder);
CREATE NONCLUSTERED INDEX IX_TeachingMaterials_LearningLevelId ON dbo.TeachingMaterials (LearningLevelId);
CREATE NONCLUSTERED INDEX IX_LessonPlanMaterials_MaterialId ON dbo.LessonPlanMaterials (MaterialId);
CREATE NONCLUSTERED INDEX IX_LessonVocabulary_LessonPlanId ON dbo.LessonVocabulary (LessonPlanId);
CREATE NONCLUSTERED INDEX IX_Activities_CategoryId ON dbo.Activities (CategoryId);
CREATE NONCLUSTERED INDEX IX_LessonActivities_ActivityId ON dbo.LessonActivities (ActivityId);
CREATE NONCLUSTERED INDEX IX_ClassSessions_CourseId_Date ON dbo.ClassSessions (CourseId, SessionDate);
CREATE NONCLUSTERED INDEX IX_ClassSessions_TeacherId ON dbo.ClassSessions (TeacherId);
CREATE NONCLUSTERED INDEX IX_Attendance_StudentId ON dbo.Attendance (StudentId);
CREATE NONCLUSTERED INDEX IX_Assessments_StudentId ON dbo.Assessments (StudentId);
CREATE NONCLUSTERED INDEX IX_Assessments_SessionId ON dbo.Assessments (SessionId);
GO

-- ============================================================================
-- 8. Business Rule Triggers (Multi-Row Safe)
-- ============================================================================

CREATE TRIGGER dbo.TR_Attendance_ValidateEnrollment
ON dbo.Attendance
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.ClassSessions cs ON i.SessionId = cs.SessionId
        LEFT JOIN dbo.Enrollments e 
            ON e.CourseId = cs.CourseId 
           AND e.StudentId = i.StudentId 
           AND e.Status = 'Enrolled'
        WHERE e.EnrollmentId IS NULL
    )
    BEGIN
        RAISERROR ('Attendance operation rejected: One or more students are not actively enrolled in the course for the specified class session.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

CREATE TRIGGER dbo.TR_AssessmentDetails_MaintainOverallScore
ON dbo.AssessmentDetails
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AffectedAssessments AS (
        SELECT DISTINCT AssessmentId FROM inserted
        UNION
        SELECT DISTINCT AssessmentId FROM deleted
    )
    UPDATE a
    SET a.OverallScore = sub.CalculatedAverage,
        a.UpdatedDate = GETUTCDATE()
    FROM dbo.Assessments a
    JOIN AffectedAssessments aa ON a.AssessmentId = aa.AssessmentId
    OUTER APPLY (
        SELECT ROUND(AVG(CAST(ad.Score AS DECIMAL(5,2))), 2) AS CalculatedAverage
        FROM dbo.AssessmentDetails ad
        WHERE ad.AssessmentId = a.AssessmentId
    ) sub;
END;
GO

CREATE TRIGGER dbo.TR_ClassSessions_ValidateConsistency
ON dbo.ClassSessions
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Verify LessonPlan course compatibility
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.LessonPlans lp ON i.LessonPlanId = lp.LessonPlanId
        WHERE lp.CourseId IS NOT NULL 
          AND lp.CourseId <> i.CourseId
    )
    BEGIN
        RAISERROR ('ClassSession invalid: The selected lesson plan is assigned to a different course.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Verify Teacher authorization and active status
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.Users u ON i.TeacherId = u.UserId
        JOIN dbo.Roles r ON u.RoleId = r.RoleId
        WHERE u.IsActive = 0
           OR (
               r.RoleName <> 'Admin'
               AND NOT EXISTS (
                   SELECT 1 
                   FROM dbo.CourseTeachers ct
                   WHERE ct.CourseId = i.CourseId
                     AND ct.TeacherId = i.TeacherId
                     AND ct.IsActive = 1
               )
           )
    )
    BEGIN
        RAISERROR ('ClassSession invalid: The assigned teacher is inactive or not assigned to this course.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

CREATE TRIGGER dbo.TR_CourseTeachers_ValidateTeacherRole
ON dbo.CourseTeachers
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.Users u ON i.TeacherId = u.UserId
        JOIN dbo.Roles r ON u.RoleId = r.RoleId
        WHERE r.RoleName NOT IN ('Teacher', 'Admin')
    )
    BEGIN
        RAISERROR ('CourseTeacher assignment invalid: Assigned user must have the Teacher or Admin role.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- ============================================================================
-- 9. Analytical & Reporting Views
-- ============================================================================

CREATE VIEW dbo.vw_StudentAssessmentSummaries AS
SELECT 
    a.AssessmentId,
    a.StudentId,
    s.StudentCode,
    s.FullName AS StudentFullName,
    s.NickName AS StudentNickName,
    a.SessionId,
    cs.CourseId,
    c.CourseName,
    a.LessonPlanId,
    lp.Title AS LessonTitle,
    a.EvaluatorTeacherId,
    u.FullName AS EvaluatorTeacherName,
    a.AssessmentDate,
    a.Status AS AssessmentStatus,
    a.OverallScore AS StoredOverallScore,
    COUNT(ad.AssessmentDetailId) AS CriteriaCount,
    ROUND(AVG(CAST(ad.Score AS DECIMAL(5,2))), 2) AS CalculatedOverallScore,
    MIN(ad.Score) AS LowestCriterionScore,
    MAX(ad.Score) AS HighestCriterionScore
FROM dbo.Assessments a
JOIN dbo.Students s ON a.StudentId = s.StudentId
LEFT JOIN dbo.ClassSessions cs ON a.SessionId = cs.SessionId
LEFT JOIN dbo.Courses c ON cs.CourseId = c.CourseId
LEFT JOIN dbo.LessonPlans lp ON a.LessonPlanId = lp.LessonPlanId
JOIN dbo.Users u ON a.EvaluatorTeacherId = u.UserId
LEFT JOIN dbo.AssessmentDetails ad ON a.AssessmentId = ad.AssessmentId
GROUP BY 
    a.AssessmentId,
    a.StudentId,
    s.StudentCode,
    s.FullName,
    s.NickName,
    a.SessionId,
    cs.CourseId,
    c.CourseName,
    a.LessonPlanId,
    lp.Title,
    a.EvaluatorTeacherId,
    u.FullName,
    a.AssessmentDate,
    a.Status,
    a.OverallScore;
GO

-- ============================================================================
-- 10. Foundational Seed Data
-- ============================================================================

-- Roles
INSERT INTO dbo.Roles (RoleName, Description) VALUES
('Admin', 'System administrator with complete access to manage teachers, courses, students, and settings'),
('Teacher', 'Instructor with access to lesson plans, speaking activities, student assessments, and attendance'),
('Parent', 'Guardian with restricted access to view their child''s speaking progress and feedback');
GO

-- Users (Seed Passwords: Admin@123 for admin, Teacher@123 for teachers)
-- Using standard ASP.NET Core IPasswordHasher<TUser> PBKDF2 HMAC-SHA256 hashes
INSERT INTO dbo.Users (RoleId, Username, Email, PasswordHash, FullName, PhoneNumber, IsActive) VALUES
(1, 'admin', 'admin@littletalk.com', 'AQAAAAIAAYagAAAAECaPnrt7dsuvnD7v5p2hHwUJvUaSMHaVwMRZ9T+huS8WFkxM3x3Xu9yW4dltq92sbg==', 'System Administrator', '+1-555-0100', 1),
(2, 'teacher.sarah', 'teacher.sarah@littletalk.com', 'AQAAAAIAAYagAAAAEL8W6mjL3ukyrCZQCAa4NdHb7ciMGCBy0uaNTZNfUrGGLtS+MCkhOq064RHtf8nz7A==', 'Sarah Jenkins', '+1-555-0101', 1),
(2, 'teacher.david', 'teacher.david@littletalk.com', 'AQAAAAIAAYagAAAAEL8W6mjL3ukyrCZQCAa4NdHb7ciMGCBy0uaNTZNfUrGGLtS+MCkhOq064RHtf8nz7A==', 'David Miller', '+1-555-0102', 1);
GO

-- Learning Levels
INSERT INTO dbo.LearningLevels (LevelName, LevelCode, AgeRangeDescription, Description, SortOrder, IsActive) VALUES
('Starter - Phonics & First Words', 'LVL-STARTER', '4–5 years', 'Focus on alphabet phonics, basic greetings, object naming, and simple repeat-after-me gestures.', 1, 1),
('Explorer - Daily Words & Phrasing', 'LVL-EXPLORER', '6–7 years', 'Focus on everyday classroom vocabulary, short 2-3 word sentences, simple question-and-answer interactions.', 2, 1),
('Speaker - Conversational Fluency', 'LVL-SPEAKER', '8–9 years', 'Focus on describing family, friends, hobbies, asking questions in pairs, and sentence building.', 3, 1),
('Storyteller - Creative Expression', 'LVL-STORY', '10–12 years', 'Focus on short presentations, storytelling, spontaneous dialogue, role-play drama, and fluency confidence.', 4, 1);
GO

-- Teaching Methods Library
INSERT INTO dbo.TeachingMethods (Name, Description, TeachingInstructions, SuitableAgeGroup, SuitableLearningLevelId, RequiredMaterials, IsActive) VALUES
('Total Physical Response (TPR)', 'A method where language is taught through coordinated physical movement and action commands.', '1. Model the action with the spoken word 3 times. 2. Have students perform the action together. 3. Say word without modeling and prompt student movement.', '4–7 years', 1, 'Body movement, action cards', 1),
('Flashcard Method', 'Rapid visual stimulus drill to build instant word-association and pronunciation recall.', '1. Show picture side and say word clearly twice. 2. Show word side and prompt choral repetition. 3. Flip quickly for speed recognition challenge.', '4–9 years', 1, 'Laminated picture/word flashcards', 1),
('Role-play & Dramatization', 'Immersive role assumption where kids act out everyday scenarios in English.', '1. Set up the setting (e.g., family dinner, supermarket). 2. Assign simple dialogue scripts. 3. Let pairs perform with simple props.', '7–12 years', 3, 'Costumes, toy props, dialogue cue cards', 1),
('Storytelling & Puppetry', 'Narrative-based learning where puppet characters engage children in responsive dialogue.', '1. Introduce puppet character with a warm greeting. 2. Tell simple 3-part story using repetitive refrain. 3. Invite children to speak directly to the puppet.', '4–8 years', 2, 'Hand puppets, picture storybook', 1),
('Question and Answer (Q&A) Drill', 'Structured conversational exchanges transitioning from closed to open-ended inquiries.', '1. Teacher asks model student first. 2. Student replies. 3. Student asks next student around the circle (chain drill).', '6–12 years', 2, 'Question ball or talking stick', 1),
('Repetition & Chanting', 'Rhythmic vocal repetition that embeds intonation, phonics, and cadence naturally.', '1. Establish steady beat using hand clapping. 2. Chant rhythmically: "I have a dog! Woof! Woof!". 3. Group and solo repeats.', '4–8 years', 1, 'Metronome or rhythm clappers', 1),
('Pair Work Conversation', 'Guided peer-to-peer dialogues fostering cooperative speaking and confidence.', '1. Teach the target exchange on the board. 2. Model with a co-teacher or student. 3. Pair students up with task cards and monitor for 5 minutes.', '7–12 years', 3, 'Pair dialogue prompt worksheets', 1),
('Singing, Rhythm & Movement', 'Action songs that combine musical ear training with English articulation.', '1. Play melody and sing line by line. 2. Add gestures. 3. Sing at different tempos (slow, fast, whisper).', '4–7 years', 1, 'Audio player, musical shaker toys', 1);
GO

-- Activity Categories
INSERT INTO dbo.ActivityCategories (CategoryName, Description, Icon) VALUES
('Vocabulary Games', 'Engaging interactive games that solidify word retention through play', 'videogame_asset'),
('Role-play', 'Imaginative situational speaking scenarios', 'theater_comedy'),
('Pronunciation Practice', 'Phonics, tongue twisters, and articulation drills', 'record_voice_over'),
('Question and Answer', 'Interactive inquiry games and rapid-fire asking/answering', 'question_answer'),
('Storytelling', 'Creative narrative building and picture description activities', 'auto_stories'),
('Pair Conversation', 'Structured dialogues between student partners', 'people'),
('Group Conversation', 'Whole-class discussions, circle games, and collaborative speaking challenges', 'groups');
GO

-- Reusable Activities
INSERT INTO dbo.Activities (CategoryId, ActivityName, Description, Instructions, SuitableAgeGroup, DifficultyLevel, DurationMinutes, RequiredMaterials, ExpectedLearningOutcome, CreatedBy, IsActive) VALUES
(1, 'Magic Flashcard Peek-a-boo', 'Teacher covers a flashcard with paper and slowly reveals it while students guess the word.', '1. Hide card behind an envelope. 2. Reveal 1 cm at a time. 3. First student to yell out the correct English word wins a star point.', '4–7 years', 'Easy', 10, 'Target flashcards, covering paper', 'Instant vocabulary recall and correct pronunciation of target nouns', 2, 1),
(2, 'Family Photo Show & Tell', 'Students pretend to show their family album to a new friend.', '1. Give student a picture frame prop. 2. Student holds up family card and says: "Look! This is my mother. Her name is Mary." 3. Partner asks: "Do you have a brother?"', '7–9 years', 'Medium', 15, 'Toy picture frame, family drawings', 'Confident sentence production using possessive pronouns and family nouns', 2, 1),
(3, 'Phonics Echo Chamber', 'Sound isolation game where teacher makes a sound and students echo with animal voices.', '1. Teacher produces vowel/consonant target sound (e.g. /d/ /ɒ/ /ɡ/). 2. Call on groups to echo like lions, mice, or robots.', '4–6 years', 'Easy', 10, 'Phonics letter cards', 'Accurate articulation of isolated English phonemes', 3, 1),
(4, 'Hot Potato Question Ball', 'Music plays while ball is passed; student holding ball when music stops answers a question.', '1. Play upbeat music while passing ball. 2. Stop music abruptly. 3. Teacher asks: "Who is in your family?" Student responds in full sentence.', '6–10 years', 'Medium', 12, 'Soft foam ball, music player', 'Quick spontaneous response to Wh- questions without hesitation', 2, 1),
(5, 'Story Chain Relay', 'Each student adds one sentence to continue an ongoing class adventure.', '1. Teacher starts: "Yesterday, the little puppy went to the park." 2. Next student adds: "He saw a big brown cat." 3. Continue around circle.', '8–12 years', 'Challenging', 15, 'Visual prompt posters', 'Narrative flow, past tense usage, and creative English thinking', 3, 1);
GO

-- Reusable Teaching Materials
INSERT INTO dbo.TeachingMaterials (MaterialName, MaterialType, FileUrl, Description, SuitableAgeGroup, LearningLevelId, UploadedBy, IsActive) VALUES
('Family Members Flashcard Pack (Father, Mother, Brother, Sister, Baby)', 'Flashcard', '/assets/materials/flashcards/family_pack_01.pdf', 'High-contrast cartoon flashcards with large print phonetic guides on the back.', '5–9 years', 2, 2, 1),
('Finger Family Action Song (Audio & Backing Track)', 'Audio', '/assets/materials/audio/finger_family_action.mp3', 'Upbeat rhythmic song introducing finger gestures for Father, Mother, Brother, Sister, and Baby.', '4–7 years', 1, 2, 1),
('Daily Classroom Greeting Slide Deck', 'Slide', '/assets/materials/slides/classroom_greetings.pdf', 'Color-coded visual cues for "Good morning", "How are you today?", "I am happy/sleepy/excited".', '4–10 years', 2, 1, 1),
('My Family Tree Coloring & Speaking Worksheet', 'Worksheet', '/assets/materials/worksheets/family_tree_speaking.pdf', 'Draw-and-speak worksheet where kids label family members and present them to the class.', '6–9 years', 2, 2, 1);
GO

-- Courses
INSERT INTO dbo.Courses (CourseCode, CourseName, Description, LearningLevelId, DurationWeeks, MaxStudents, Status) VALUES
('CRS-BEG-KIDS', 'English for Kids (Foundations)', 'Introductory communicative English course designed for children aged 6 to 8. Emphasizes listening comprehension, phonics, and everyday vocabulary.', 2, 12, 12, 'Active'),
('CRS-DAILY-SPEAK', 'Daily English Speaking & Conversation', 'Interactive spoken English focusing on family, friends, hobbies, feelings, and routine situations for elementary learners.', 3, 12, 10, 'Active'),
('CRS-STORY-PRON', 'Storytelling & Pronunciation Lab', 'Expressive speaking workshop developing narrative creativity, clear pronunciation, and presentation confidence.', 4, 10, 8, 'Active');
GO

-- Assign Course Teachers
INSERT INTO dbo.CourseTeachers (CourseId, TeacherId, RoleInCourse, IsActive) VALUES
(1, 2, 'Primary', 1),    -- Sarah Smith is Primary for English for Kids
(1, 3, 'CoTeacher', 1),  -- David Lee is CoTeacher
(2, 2, 'Primary', 1),    -- Sarah Smith is Primary for Daily English
(3, 3, 'Primary', 1);    -- David Lee is Primary for Storytelling
GO

-- Students
INSERT INTO dbo.Students (StudentCode, FullName, NickName, DateOfBirth, Gender, LearningLevelId, ParentName, ParentContact, EmergencyNotes, IsActive) VALUES
('LT-STU-1001', 'Alexander William Chen', 'Alex', '2019-03-15', 'Boy', 2, 'Emily Chen', '+1-555-0201', 'Mild peanut allergy. Very energetic; loves drawing.', 1),
('LT-STU-1002', 'Sophia Grace Miller', 'Sophie', '2019-07-22', 'Girl', 2, 'Robert Miller', '+1-555-0202', 'A bit shy at first; benefits from pair work with supportive partner.', 1),
('LT-STU-1003', 'Liam James Tanaka', 'Liam', '2018-11-05', 'Boy', 3, 'Kenji Tanaka', '+1-555-0203', 'Advanced vocabulary; encourage complete complex sentences.', 1),
('LT-STU-1004', 'Mia Isabella Rossi', 'Mia', '2019-01-30', 'Girl', 2, 'Elena Rossi', '+1-555-0204', 'Loves action songs and TPR activities.', 1),
('LT-STU-1005', 'Ethan Noah Hernandez', 'Ethan', '2018-06-18', 'Boy', 3, 'Carlos Hernandez', '+1-555-0205', 'Strong listening comprehension; working on speech volume and confidence.', 1);
GO

-- Enrollments
INSERT INTO dbo.Enrollments (CourseId, StudentId, Status, Notes) VALUES
(1, 1, 'Enrolled', 'Active student in beginner cohort'),
(1, 2, 'Enrolled', 'Active student in beginner cohort'),
(1, 4, 'Enrolled', 'Active student in beginner cohort'),
(2, 3, 'Enrolled', 'Active student in daily conversation cohort'),
(2, 5, 'Enrolled', 'Active student in daily conversation cohort');
GO

-- Comprehensive Core Lesson Plan: "My Family" (as specified in Project Prompt)
INSERT INTO dbo.LessonPlans (CourseId, Title, Topic, Description, LearningLevelId, AgeGroup, DurationMinutes, LearningObjectives, Status, CreatedBy) VALUES
(1, 'My Family & Loved Ones', 'My Family', 'A high-engagement introductory speaking lesson where young learners identify family members, articulate sentences using "This is my...", and describe their own families with confidence.', 2, '7–9 years', 45, 
'- Children can accurately identify and pronounce key family members (Father, Mother, Brother, Sister).
- Children can introduce their family members using full sentences: "This is my mother / This is my father."
- Children can answer simple spoken questions about their family (e.g., "Who is this?" / "Do you have a brother?").', 
'Published', 2);
GO

-- Lesson Steps for "My Family"
INSERT INTO dbo.LessonSteps (LessonPlanId, StepName, DurationMinutes, TeachingMethodId, Instructions, TeacherScript, ExpectedStudentResponse, SortOrder) VALUES
(1, '1. Warm-up & Greeting', 5, 8, 
'Teacher enthusiastically greets students, leads a quick 2-minute greeting song with hand gestures, and has each child introduce themselves by name.',
'Hello everyone! Good morning! Welcome to English Speaking time! My name is Teacher Sarah. How are you today? Let us sing: Hello, hello, how are you?',
'Good morning Teacher Sarah! I am happy! My name is Alex!', 1),

(1, '2. Vocabulary Presentation', 10, 2, 
'Introduce Father, Mother, Brother, Sister, and Baby using colorful flashcards. Model clear phonics articulation, mouth shape, and rhythmic repetition.',
'Look at my magic card! Who is this? Listen: F-A-T-H-E-R. Father! Say it like a big papa bear: FATHER! Now look here: M-O-T-H-E-R. Mother!',
'Father! Mother! Brother! Sister! Baby!', 2),

(1, '3. Speaking Practice & Sentence Drill', 10, 5, 
'Guide students to build the target sentence structure "This is my [Family Member]." Use echo chamber technique and individual spot checks.',
'When we introduce our family, we say: "This is my mother." Repeat with me: This... is... my... mother! Alex, who is this in your hand?',
'This is my mother. This is my father. This is my brother.', 3),

(1, '4. Interactive Activity & Pair Work', 15, 3, 
'Run the Family Photo Show & Tell activity. Pair up students and give each pair prompt cards. Teacher circulates providing real-time praise and pronunciation guidance.',
'Now it is time for our Family Photo Show & Tell! Turn to your partner. Hold up your card and say: "Look! This is my..." Partner asks: "Who is this?" Let''s go!',
'Look! This is my father. He is tall! Who is this? This is my sister!', 4),

(1, '5. Review & Star Wrap-up', 5, 1, 
'Quick-fire flashcard challenge and question check. Award speaking star stickers and lead goodbye chant.',
'Fantastic job speaking English today! Quick test: Who is this? Yes! Today everyone spoke with great confidence. High five everyone! Goodbye chant!',
'Father! Sister! Goodbye Teacher Sarah! See you next time!', 5);
GO

-- Lesson Vocabulary for "My Family"
INSERT INTO dbo.LessonVocabulary (LessonPlanId, Word, Meaning, Pronunciation, ExampleSentence, Topic, SortOrder) VALUES
(1, 'Father', 'A male parent; dad.', '/ˈfɑːðər/', 'This is my father. He is very kind.', 'Family', 1),
(1, 'Mother', 'A female parent; mom.', '/ˈmʌðər/', 'This is my mother. I love my mother.', 'Family', 2),
(1, 'Brother', 'A boy or man who has the same parents as you.', '/ˈbrʌðər/', 'This is my big brother. We play together.', 'Family', 3),
(1, 'Sister', 'A girl or woman who has the same parents as you.', '/ˈsɪstər/', 'This is my little sister. She is cute.', 'Family', 4),
(1, 'Baby', 'A very young child.', '/ˈbeɪbi/', 'Look at the happy baby!', 'Family', 5);
GO

-- Link Materials to Lesson Plan
INSERT INTO dbo.LessonPlanMaterials (LessonPlanId, MaterialId, UsageInstructions, SortOrder) VALUES
(1, 1, 'Use during Step 2 Vocabulary Presentation and Step 5 Review.', 1),
(1, 2, 'Play during Step 1 Warm-up and Step 3 movement transition.', 2),
(1, 4, 'Distribute to students for Step 4 Pair Work presentation.', 3);
GO

-- Link Activities to Lesson Plan & Steps
INSERT INTO dbo.LessonActivities (LessonPlanId, ActivityId, LessonStepId, StepOrder, Notes) VALUES
(1, 1, 2, 1, 'Warm-up peek-a-boo drill to reveal the family member cards in Step 2.'),
(1, 2, 4, 2, 'Main interactive pair conversation activity in Step 4.');
GO

-- Class Session
INSERT INTO dbo.ClassSessions (CourseId, LessonPlanId, TeacherId, SessionDate, StartTime, EndTime, RoomOrLink, Status, Notes) VALUES
(1, 1, 2, CAST(GETUTCDATE() AS DATE), '09:00:00', '09:45:00', 'Sunshine Classroom A', 'Completed', 'Session delivered smoothly. High engagement during the Family Photo Show & Tell activity.');
GO

-- Attendance for Session
INSERT INTO dbo.Attendance (SessionId, StudentId, Status, Remarks, RecordedBy) VALUES
(1, 1, 'Present', 'Arrived enthusiastically on time.', 2),
(1, 2, 'Present', 'Participated well in pair activities.', 2),
(1, 4, 'Late', 'Arrived 5 minutes late due to traffic; caught up quickly.', 2);
GO

-- Assessment for Alexander (Student 1) with 6 Criteria
INSERT INTO dbo.Assessments (StudentId, SessionId, LessonPlanId, EvaluatorTeacherId, AssessmentDate, Status, Strengths, AreasForImprovement, GeneralFeedback) VALUES
(1, 1, 1, 2, GETUTCDATE(), 'Finalized', 
'Exceptional enthusiasm and voice projection. Pronounced "Father" and "Brother" with clear /ð/ phonics articulation.',
'Practice smoothing out multi-word transitions in "This is my..." without pausing between words.',
'Great job today, Alex! You were eager to participate and introduced your family with big smiles and clear English!');
GO

-- Assessment Details (Score 1-5 across all 6 criteria)
-- Note: Trigger TR_AssessmentDetails_MaintainOverallScore will automatically compute OverallScore = 4.33
INSERT INTO dbo.AssessmentDetails (AssessmentId, CriterionName, Score, Notes) VALUES
(1, 'Pronunciation', 4, 'Clear articulation of the /ð/ sound; slight rush on "brother".'),
(1, 'Vocabulary', 5, 'Mastered all 5 target family words instantly with zero prompting.'),
(1, 'Fluency', 4, 'Smooth sentence flow; minor pause before introducing sister.'),
(1, 'Sentence Formation', 4, 'Consistently produced full sentences: "This is my mother."'),
(1, 'Confidence', 5, 'Energetic body language, made great eye contact, spoke loudly.'),
(1, 'Listening and Responding', 4, 'Responded promptly to "Who is this?" with accurate answers.');
GO

PRINT 'LittleTalkDb initialized successfully with 20 tables, indexes, triggers, and seed data.';
GO
