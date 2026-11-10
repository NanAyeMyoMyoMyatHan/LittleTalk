using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using LittleTalk.Application.Features.LessonPlans.Services;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class AiLessonPlanGeneratorTests
{
    private LittleTalkDbContext CreateInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<LittleTalkDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new LittleTalkDbContext(options);
    }

    [Fact]
    public void PromptBuilder_BuildSystemInstruction_ShouldContainCorePrinciples()
    {
        var instruction = AiPromptBuilder.BuildSystemInstruction();

        Assert.Contains("5-Stage Structured Teaching Flow", instruction);
        Assert.Contains("Speaking & Listening First", instruction);
        Assert.Contains("Teacher Scripts", instruction);
        Assert.Contains("Child-Friendly Vocabulary", instruction);
    }

    [Fact]
    public void PromptBuilder_BuildUserPrompt_ShouldIncludeTopicAndDuration()
    {
        var request = new GenerateAiLessonPlanRequestModel
        {
            Topic = "Ant",
            DurationMinutes = 30,
            TargetAgeGroup = "5-7 years",
            SpecificFocusNotes = "Focus on /a/ phonics"
        };

        var prompt = AiPromptBuilder.BuildUserPrompt(request, "Starter", "4-5 years", new[] { "TPR", "Singing" });

        Assert.Contains("Topic: Ant", prompt);
        Assert.Contains("30 minutes", prompt);
        Assert.Contains("5-7 years", prompt);
        Assert.Contains("Focus on /a/ phonics", prompt);
        Assert.Contains("TPR, Singing", prompt);
    }

    [Fact]
    public async Task GenerateLessonPlan_ShouldFail_WhenTopicIsEmpty()
    {
        using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var config = new ConfigurationBuilder().Build();
        var clientFactory = new TestHttpClientFactory();
        var service = new AiLessonPlanGeneratorService(context, clientFactory, config, NullLogger<AiLessonPlanGeneratorService>.Instance);

        var request = new GenerateAiLessonPlanRequestModel
        {
            Topic = "",
            LearningLevelId = 1,
            DurationMinutes = 30
        };

        var result = await service.GenerateLessonPlanAsync(request, 1, "Teacher");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
        Assert.Contains("Topic is required", result.Message);
    }

    [Fact]
    public async Task GenerateLessonPlan_ShouldFail_WhenDurationOutOfRange()
    {
        using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var config = new ConfigurationBuilder().Build();
        var clientFactory = new TestHttpClientFactory();
        var service = new AiLessonPlanGeneratorService(context, clientFactory, config, NullLogger<AiLessonPlanGeneratorService>.Instance);

        var request = new GenerateAiLessonPlanRequestModel
        {
            Topic = "Colors",
            LearningLevelId = 1,
            DurationMinutes = 10 // Less than minimum 15
        };

        var result = await service.GenerateLessonPlanAsync(request, 1, "Teacher");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
        Assert.Contains("between 15 and 180 minutes", result.Message);
    }

    [Fact]
    public async Task GenerateLessonPlan_ShouldFail_WhenLearningLevelInactiveOrMissing()
    {
        using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        var config = new ConfigurationBuilder().Build();
        var clientFactory = new TestHttpClientFactory();
        var service = new AiLessonPlanGeneratorService(context, clientFactory, config, NullLogger<AiLessonPlanGeneratorService>.Instance);

        var request = new GenerateAiLessonPlanRequestModel
        {
            Topic = "Animals",
            LearningLevelId = 999,
            DurationMinutes = 30
        };

        var result = await service.GenerateLessonPlanAsync(request, 1, "Teacher");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.ValidationError, result.ResultType);
        Assert.Contains("learning level is invalid", result.Message);
    }

    [Fact]
    public async Task GenerateLessonPlan_ShouldFail_WhenTeacherNotAssignedToCourse()
    {
        using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        context.LearningLevels.Add(new LearningLevel
        {
            LearningLevelId = 1,
            LevelName = "Starter",
            LevelCode = "LVL-START",
            IsActive = true
        });
        context.Courses.Add(new Course
        {
            CourseId = 10,
            CourseCode = "ENG-10",
            CourseName = "Kids English Cohort",
            LearningLevelId = 1,
            Status = "Active"
        });
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder().Build();
        var clientFactory = new TestHttpClientFactory();
        var service = new AiLessonPlanGeneratorService(context, clientFactory, config, NullLogger<AiLessonPlanGeneratorService>.Instance);

        var request = new GenerateAiLessonPlanRequestModel
        {
            Topic = "Ant",
            LearningLevelId = 1,
            CourseId = 10,
            DurationMinutes = 30
        };

        // Current user is Teacher (Id=2) who is NOT in CourseTeachers
        var result = await service.GenerateLessonPlanAsync(request, 2, "Teacher");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Unauthorized, result.ResultType);
        Assert.Contains("not assigned to teach this course", result.Message);
    }

    [Fact]
    public async Task GenerateLessonPlan_ShouldSucceed_ForAdminEvenWithoutCourseAssignment()
    {
        using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        context.LearningLevels.Add(new LearningLevel
        {
            LearningLevelId = 1,
            LevelName = "Starter",
            LevelCode = "LVL-START",
            AgeRangeDescription = "4-5 years",
            IsActive = true
        });
        context.Courses.Add(new Course
        {
            CourseId = 10,
            CourseCode = "ENG-10",
            CourseName = "Kids English Cohort",
            LearningLevelId = 1,
            Status = "Active"
        });
        context.TeachingMethods.Add(new TeachingMethod
        {
            TeachingMethodId = 1,
            Name = "Singing, Rhythm & Movement",
            Description = "Music",
            TeachingInstructions = "Sing together",
            IsActive = true
        });
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder().Build();
        var clientFactory = new TestHttpClientFactory();
        var service = new AiLessonPlanGeneratorService(context, clientFactory, config, NullLogger<AiLessonPlanGeneratorService>.Instance);

        var request = new GenerateAiLessonPlanRequestModel
        {
            Topic = "Ant",
            LearningLevelId = 1,
            CourseId = 10,
            DurationMinutes = 30
        };

        var result = await service.GenerateLessonPlanAsync(request, 1, "Admin");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Ant", result.Data.Topic);
        Assert.Equal(30, result.Data.DurationMinutes);
        Assert.NotEmpty(result.Data.Steps);
        Assert.NotEmpty(result.Data.Vocabulary);
        Assert.NotEmpty(result.Data.SentencePatterns);
        Assert.NotEmpty(result.Data.TeachingMaterials);
        Assert.NotEmpty(result.Data.AssessmentNotes);
    }

    [Fact]
    public async Task GenerateLessonPlan_ShouldSucceed_WithCompletePedagogicalContent()
    {
        using var context = CreateInMemoryDbContext(Guid.NewGuid().ToString());
        context.LearningLevels.Add(new LearningLevel
        {
            LearningLevelId = 2,
            LevelName = "Explorer - Daily Words & Phrasing",
            LevelCode = "LVL-EXPLORER",
            AgeRangeDescription = "6-7 years",
            IsActive = true
        });
        context.TeachingMethods.AddRange(
            new TeachingMethod { TeachingMethodId = 1, Name = "Total Physical Response (TPR)", Description = "TPR", TeachingInstructions = "Move", IsActive = true },
            new TeachingMethod { TeachingMethodId = 2, Name = "Flashcard Method", Description = "Flashcards", TeachingInstructions = "Show", IsActive = true },
            new TeachingMethod { TeachingMethodId = 6, Name = "Repetition & Chanting", Description = "Chant", TeachingInstructions = "Repeat", IsActive = true },
            new TeachingMethod { TeachingMethodId = 8, Name = "Singing, Rhythm & Movement", Description = "Sing", TeachingInstructions = "Sing", IsActive = true }
        );
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder().Build();
        var clientFactory = new TestHttpClientFactory();
        var service = new AiLessonPlanGeneratorService(context, clientFactory, config, NullLogger<AiLessonPlanGeneratorService>.Instance);

        var request = new GenerateAiLessonPlanRequestModel
        {
            Topic = "Ant",
            LearningLevelId = 2,
            TargetAgeGroup = "5-7 years",
            DurationMinutes = 30
        };

        var result = await service.GenerateLessonPlanAsync(request, 2, "Teacher");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Ant", result.Data.Topic);
        Assert.Equal("5-7 years", result.Data.AgeGroup);
        Assert.Equal(30, result.Data.DurationMinutes);

        // Verify 5 pedagogical steps
        Assert.Equal(5, result.Data.Steps.Count);
        Assert.All(result.Data.Steps, s =>
        {
            Assert.False(string.IsNullOrWhiteSpace(s.StepName));
            Assert.True(s.DurationMinutes > 0);
            Assert.False(string.IsNullOrWhiteSpace(s.Instructions));
            Assert.False(string.IsNullOrWhiteSpace(s.TeacherScript));
            Assert.False(string.IsNullOrWhiteSpace(s.ExpectedStudentResponse));
        });

        // Verify total duration equals requested duration
        var totalStepMinutes = result.Data.Steps.Sum(s => s.DurationMinutes);
        Assert.Equal(30, totalStepMinutes);

        // Verify vocabulary
        Assert.NotEmpty(result.Data.Vocabulary);
        Assert.Contains(result.Data.Vocabulary, v => v.Word.Equals("Ant", StringComparison.OrdinalIgnoreCase));
        Assert.All(result.Data.Vocabulary, v =>
        {
            Assert.False(string.IsNullOrWhiteSpace(v.Word));
            Assert.False(string.IsNullOrWhiteSpace(v.Meaning));
            Assert.False(string.IsNullOrWhiteSpace(v.Pronunciation));
            Assert.False(string.IsNullOrWhiteSpace(v.ExampleSentence));
        });

        // Verify sentence patterns & assessment notes
        Assert.NotEmpty(result.Data.SentencePatterns);
        Assert.False(string.IsNullOrWhiteSpace(result.Data.AssessmentNotes));
    }

    private class TestHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new HttpClient();
    }
}
