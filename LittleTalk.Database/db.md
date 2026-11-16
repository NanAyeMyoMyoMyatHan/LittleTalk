# LittleTalk Database Architecture

## Overview
LittleTalk uses Microsoft SQL Server 2022+ with Entity Framework Core (strict Database-First lifecycle; no Code-First migrations).

## Schema Assets
- `Scripts/LittleTalk_Db_Init.sql`: Master database creation, tables, constraints, foreign keys, triggers, and views.
- `LittleTalkDbContext.cs`: EF Core database context mapped to the database schema.
- `Entities/`: 20 EF Core normalized entity models.

## Core Entities
1. **Roles & Users**: `Role`, `User`
2. **Courses & Levels**: `Course`, `CourseTeacher`, `LearningLevel`
3. **Students & Enrollment**: `Student`, `Enrollment`, `Attendance`, `ClassSession`
4. **Lesson Plans**: `LessonPlan`, `LessonStep`, `LessonActivity`, `LessonVocabulary`, `LessonPlanMaterial`
5. **Pedagogy & Materials**: `TeachingMethod`, `TeachingMaterial`, `Activity`, `ActivityCategory`
6. **Assessments & Analytics**: `Assessment`, `AssessmentDetail`, `VwStudentAssessmentSummary`
