using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LittleTalk.Infrastructure.Data;
using LittleTalk.Infrastructure.Data.Entities;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Application.Features.LessonPlans.Services;

/// <summary>
/// Service responsible for generating pedagogical English speaking lesson plans using AI.
/// Communicates with Google Gemini (or OpenAI-compatible) endpoints using IHttpClientFactory.
/// </summary>
public sealed class AiLessonPlanGeneratorService : IAiLessonPlanGeneratorService
{
    private readonly LittleTalkDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiLessonPlanGeneratorService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AiLessonPlanGeneratorService(
        LittleTalkDbContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AiLessonPlanGeneratorService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<GenerateAiLessonPlanResponseModel>> GenerateLessonPlanAsync(
        GenerateAiLessonPlanRequestModel? request,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            _logger.LogWarning("[LittleTalk.Application] GenerateLessonPlanAsync => Request payload was null.");
            return Result<GenerateAiLessonPlanResponseModel>.ValidationError("Request body cannot be empty.");
        }

        _logger.LogInformation(
            "[LittleTalk.Application] GenerateLessonPlanAsync => Starting AI lesson plan generation. Topic={Topic}, LevelId={LevelId}, UserId={UserId}",
            request.Topic, request.LearningLevelId, currentUserId);

        // 1. Domain Validation
        var validationResult = ValidateRequest(request);
        if (!validationResult.IsSuccess)
        {
            _logger.LogWarning(
                "[LittleTalk.Application] GenerateLessonPlanAsync => Request validation failed. Message={Message}",
                validationResult.Message);
            return validationResult;
        }

        // 2. Validate Learning Level Existence
        var learningLevel = await _context.LearningLevels
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.LearningLevelId == request.LearningLevelId && l.IsActive, cancellationToken);

        if (learningLevel == null)
        {
            return Result<GenerateAiLessonPlanResponseModel>.ValidationError("The selected learning level is invalid or inactive.");
        }

        // 3. Admin-Assigned Topic / Course Teacher Authorization Check
        if (request.CourseId.HasValue && request.CourseId.Value > 0)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == request.CourseId.Value, cancellationToken);

            if (course == null)
            {
                return Result<GenerateAiLessonPlanResponseModel>.NotFound($"Course with ID ({request.CourseId.Value}) was not found.");
            }

            if (!string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                var isTeacherAssigned = await _context.CourseTeachers
                    .AsNoTracking()
                    .AnyAsync(ct => ct.CourseId == request.CourseId.Value && ct.TeacherId == currentUserId && ct.IsActive, cancellationToken);

                if (!isTeacherAssigned)
                {
                    _logger.LogWarning(
                        "[LittleTalk.Application] GenerateLessonPlanAsync => Unauthorized course access attempt. UserId={UserId}, CourseId={CourseId}",
                        currentUserId, request.CourseId.Value);
                    return Result<GenerateAiLessonPlanResponseModel>.Unauthorized("You are not assigned to teach this course cohort.");
                }
            }
        }

        // 4. Retrieve Active Teaching Methods to Provide Context for AI
        var activeMethods = await _context.TeachingMethods
            .AsNoTracking()
            .Where(m => m.IsActive)
            .OrderBy(m => m.TeachingMethodId)
            .ToListAsync(cancellationToken);

        var methodNames = activeMethods.Select(m => m.Name).ToList();

        // 5. Check AI API Configuration
        var apiKey = _configuration["AiSettings:ApiKey"] 
            ?? Environment.GetEnvironmentVariable("AI_API_KEY") 
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        var provider = _configuration["AiSettings:Provider"] ?? "Gemini";
        var model = _configuration["AiSettings:Model"] ?? "gemini-2.5-flash";

        GenerateAiLessonPlanResponseModel? planResult = null;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning(
                "[LittleTalk.Application] GenerateLessonPlanAsync => AI API key not configured in User Secrets or environment. Generating via high-quality pedagogical template engine.");
            planResult = GenerateTemplateLessonPlan(request, learningLevel, activeMethods);
        }
        else
        {
            try
            {
                planResult = await CallAiProviderAsync(request, learningLevel, methodNames, provider, model, apiKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[LittleTalk.Application] GenerateLessonPlanAsync => AI provider call failed. Falling back to template generator. Error={ErrorMessage}",
                    ex.Message);
                planResult = GenerateTemplateLessonPlan(request, learningLevel, activeMethods);
            }
        }

        if (planResult == null)
        {
            return Result<GenerateAiLessonPlanResponseModel>.SystemError("Unable to generate lesson plan content.");
        }

        // 6. Post-process: Map Teaching Methods & Sanitize Text
        PostProcessLessonPlan(planResult, request, learningLevel, activeMethods);

        _logger.LogInformation(
            "[LittleTalk.Application] GenerateLessonPlanAsync => AI lesson plan generation completed successfully. Title={Title}, StepsCount={StepsCount}",
            planResult.Title, planResult.Steps.Count);

        return Result<GenerateAiLessonPlanResponseModel>.Success(planResult, "AI lesson plan generated successfully.");
    }

    private static Result<GenerateAiLessonPlanResponseModel> ValidateRequest(GenerateAiLessonPlanRequestModel? request)
    {
        if (request == null)
        {
            return Result<GenerateAiLessonPlanResponseModel>.ValidationError("Request body cannot be empty.");
        }

        request.Topic = request.Topic?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(request.Topic))
        {
            return Result<GenerateAiLessonPlanResponseModel>.ValidationError("Topic is required.");
        }

        if (request.Topic.Length > 100)
        {
            return Result<GenerateAiLessonPlanResponseModel>.ValidationError("Topic cannot exceed 100 characters.");
        }

        if (request.LearningLevelId <= 0)
        {
            return Result<GenerateAiLessonPlanResponseModel>.ValidationError("Valid Learning Level is required.");
        }

        if (request.DurationMinutes is < 15 or > 180)
        {
            return Result<GenerateAiLessonPlanResponseModel>.ValidationError("Duration must be between 15 and 180 minutes.");
        }

        return Result<GenerateAiLessonPlanResponseModel>.Success(new GenerateAiLessonPlanResponseModel());
    }

    private async Task<GenerateAiLessonPlanResponseModel?> CallAiProviderAsync(
        GenerateAiLessonPlanRequestModel request,
        LearningLevel level,
        List<string> methodNames,
        string provider,
        string model,
        string apiKey,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("AiClient");

        var systemPrompt = AiPromptBuilder.BuildSystemInstruction();
        var userPrompt = AiPromptBuilder.BuildUserPrompt(request, level.LevelName, level.AgeRangeDescription ?? "4-12 years", methodNames);

        if (provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase))
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var requestBody = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = systemPrompt } }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = userPrompt } }
                    }
                },
                generationConfig = new
                {
                    response_mime_type = "application/json",
                    temperature = 0.7
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, jsonContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "[LittleTalk.Application] CallAiProviderAsync => Gemini API returned error status {StatusCode}: {ErrorBody}",
                    response.StatusCode, errorText);
                throw new HttpRequestException($"Gemini API error ({response.StatusCode}): {errorText}");
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(responseJson);

            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException("Gemini returned empty text.");
            }

            return CleanAndDeserializePlan(text);
        }
        else
        {
            // OpenAI-compatible Chat Completion API
            var url = "https://api.openai.com/v1/chat/completions";
            var requestBody = new
            {
                model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.7,
                response_format = new { type = "json_object" }
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "[LittleTalk.Application] CallAiProviderAsync => OpenAI API returned error status {StatusCode}: {ErrorBody}",
                    response.StatusCode, errorText);
                throw new HttpRequestException($"OpenAI API error ({response.StatusCode}): {errorText}");
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(responseJson);

            var text = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException("OpenAI returned empty text.");
            }

            return CleanAndDeserializePlan(text);
        }
    }

    private static GenerateAiLessonPlanResponseModel? CleanAndDeserializePlan(string rawJson)
    {
        var cleaned = rawJson.Trim();

        // Strip Markdown code block markers if present
        if (cleaned.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[7..];
        }
        else if (cleaned.StartsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[3..];
        }

        if (cleaned.EndsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[..^3];
        }

        cleaned = SanitizeText(cleaned);

        return JsonSerializer.Deserialize<GenerateAiLessonPlanResponseModel>(cleaned, JsonOptions);
    }

    private static void PostProcessLessonPlan(
        GenerateAiLessonPlanResponseModel plan,
        GenerateAiLessonPlanRequestModel request,
        LearningLevel level,
        List<TeachingMethod> availableMethods)
    {
        plan.Topic = request.Topic.Trim();
        plan.LearningLevelId = request.LearningLevelId;
        plan.DurationMinutes = request.DurationMinutes;

        if (string.IsNullOrWhiteSpace(plan.AgeGroup))
        {
            plan.AgeGroup = !string.IsNullOrWhiteSpace(request.TargetAgeGroup)
                ? request.TargetAgeGroup.Trim()
                : (level.AgeRangeDescription ?? "5-7 years");
        }

        plan.Title = SanitizeText(plan.Title);
        plan.Description = SanitizeText(plan.Description);
        plan.LearningObjectives = SanitizeText(plan.LearningObjectives);
        plan.AssessmentNotes = SanitizeText(plan.AssessmentNotes);

        // Map Teaching Methods to Database IDs
        int order = 1;
        foreach (var step in plan.Steps)
        {
            step.StepName = SanitizeText(step.StepName);
            step.Instructions = SanitizeText(step.Instructions);
            step.TeacherScript = SanitizeText(step.TeacherScript);
            step.ExpectedStudentResponse = SanitizeText(step.ExpectedStudentResponse);
            step.SortOrder = order++;

            if (step.TeachingMethodId == null && !string.IsNullOrWhiteSpace(step.TeachingMethodName))
            {
                var matched = availableMethods.FirstOrDefault(m =>
                    m.Name.Contains(step.TeachingMethodName, StringComparison.OrdinalIgnoreCase) ||
                    step.TeachingMethodName.Contains(m.Name, StringComparison.OrdinalIgnoreCase));

                if (matched != null)
                {
                    step.TeachingMethodId = matched.TeachingMethodId;
                    step.TeachingMethodName = matched.Name;
                }
            }
        }

        int vocabOrder = 1;
        foreach (var vocab in plan.Vocabulary)
        {
            vocab.Word = SanitizeText(vocab.Word);
            vocab.Meaning = SanitizeText(vocab.Meaning);
            vocab.Pronunciation = SanitizeText(vocab.Pronunciation);
            vocab.ExampleSentence = SanitizeText(vocab.ExampleSentence);
            vocab.SortOrder = vocabOrder++;
        }
    }

    private static string SanitizeText(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        return input
            .Replace("â€“", "-")
            .Replace("â€”", "-")
            .Replace("â€˜", "'")
            .Replace("â€™", "'")
            .Replace("â€œ", "\"")
            .Replace("â€\u009d", "\"")
            .Replace("–", "-")
            .Replace("—", "-")
            .Replace("“", "\"")
            .Replace("”", "\"")
            .Replace("‘", "'")
            .Replace("’", "'")
            .Trim();
    }

    /// <summary>
    /// Fallback pedagogical generator that produces rich, authentic children's speaking lesson plans
    /// when the AI API key is not configured or offline during development.
    /// </summary>
    private static GenerateAiLessonPlanResponseModel GenerateTemplateLessonPlan(
        GenerateAiLessonPlanRequestModel request,
        LearningLevel level,
        List<TeachingMethod> methods)
    {
        var topic = request.Topic.Trim();
        var ageGroup = !string.IsNullOrWhiteSpace(request.TargetAgeGroup)
            ? request.TargetAgeGroup.Trim()
            : (level.AgeRangeDescription ?? "5-7 years");

        var totalMinutes = request.DurationMinutes;
        var step1Mins = Math.Max(5, (int)(totalMinutes * 0.15));
        var step2Mins = Math.Max(5, (int)(totalMinutes * 0.20));
        var step3Mins = Math.Max(8, (int)(totalMinutes * 0.30));
        var step4Mins = Math.Max(8, (int)(totalMinutes * 0.25));
        var step5Mins = totalMinutes - (step1Mins + step2Mins + step3Mins + step4Mins);
        if (step5Mins < 4)
        {
            step5Mins = 5;
            step3Mins = totalMinutes - (step1Mins + step2Mins + step4Mins + step5Mins);
        }

        var singingMethod = methods.FirstOrDefault(m => m.Name.Contains("Singing", StringComparison.OrdinalIgnoreCase)) ?? methods.FirstOrDefault();
        var flashcardMethod = methods.FirstOrDefault(m => m.Name.Contains("Flashcard", StringComparison.OrdinalIgnoreCase)) ?? methods.FirstOrDefault();
        var repeatMethod = methods.FirstOrDefault(m => m.Name.Contains("Repetition", StringComparison.OrdinalIgnoreCase)) ?? methods.FirstOrDefault();
        var roleplayMethod = methods.FirstOrDefault(m => m.Name.Contains("Role-play", StringComparison.OrdinalIgnoreCase)) ?? methods.FirstOrDefault();
        var qaMethod = methods.FirstOrDefault(m => m.Name.Contains("Question", StringComparison.OrdinalIgnoreCase)) ?? methods.FirstOrDefault();

        var response = new GenerateAiLessonPlanResponseModel
        {
            Title = $"Fun with {topic}: Kids Speaking Adventures",
            Topic = topic,
            AgeGroup = ageGroup,
            LearningLevelId = request.LearningLevelId,
            DurationMinutes = totalMinutes,
            Description = $"An interactive, high-energy English speaking lesson designed for young learners ({ageGroup}). Focuses on oral vocabulary acquisition, choral chanting, and communicative games centered on {topic}.",
            LearningObjectives = $"- Children can clearly identify and name key vocabulary related to {topic}.\n- Children can confidently articulate the sentence pattern 'I see a / an {topic.ToLower()}.'\n- Children engage in interactive pair speaking games and respond aloud to teacher questions.",
            SentencePatterns = new List<string>
            {
                $"I see a / an {topic.ToLower()}.",
                $"Look at the {topic.ToLower()}!",
                $"What is this? It is a {topic.ToLower()}."
            },
            TeachingMaterials = new List<string>
            {
                $"Colorful flashcards of {topic} and related vocabulary",
                "Realia or small toy models",
                "Speaker for Hello Song and movement beats",
                "Magic mystery box or reveal cloth"
            },
            AssessmentNotes = $"Assessment Guidelines:\n- Pronunciation: Note accuracy of target words and phonics sounds.\n- Vocabulary: Check instant recognition and oral recall without hesitation.\n- Sentence Formation: Evaluate confidence using 'I see a...' in speaking games.\n- Active Participation: Observe vocal enthusiasm and body gestures.",
            Vocabulary = new List<AiLessonVocabularyDto>
            {
                new()
                {
                    Word = topic,
                    Meaning = $"The central subject of our lesson ({topic.ToLower()}).",
                    Pronunciation = $"/{topic.ToLower()}/",
                    ExampleSentence = $"Look! There is a little {topic.ToLower()} right here.",
                    SortOrder = 1
                },
                new()
                {
                    Word = "Tiny",
                    Meaning = "Very, very small in size.",
                    Pronunciation = "/tai-nee/",
                    ExampleSentence = "The tiny creature is moving fast.",
                    SortOrder = 2
                },
                new()
                {
                    Word = "Fast",
                    Meaning = "Moving quickly with high speed.",
                    Pronunciation = "/fast/",
                    ExampleSentence = "It can run very fast across the grass.",
                    SortOrder = 3
                },
                new()
                {
                    Word = "Friend",
                    Meaning = "Someone you play with and care about.",
                    Pronunciation = "/frend/",
                    ExampleSentence = "The animals play together like good friends.",
                    SortOrder = 4
                }
            },
            Steps = new List<AiLessonStepDto>
            {
                new()
                {
                    StepName = "1. Warm-up & Hello Greeting Song",
                    DurationMinutes = step1Mins,
                    TeachingMethodId = singingMethod?.TeachingMethodId,
                    TeachingMethodName = singingMethod?.Name ?? "Singing, Rhythm & Movement",
                    Instructions = $"Welcome children to the classroom circle. Sing the high-energy Hello Song accompanied by clapping and stomping. Introduce {topic} using a mystery box peek game.",
                    TeacherScript = $"Teacher: Hello my wonderful friends! Are you ready for fun English today? Look what I have in my mystery box! Knock, knock! What can it be?",
                    ExpectedStudentResponse = "Students: Hello teacher! Yes! Open the box! What is it?",
                    SortOrder = 1
                },
                new()
                {
                    StepName = $"2. Presentation & Flashcard Discovery ({topic})",
                    DurationMinutes = step2Mins,
                    TeachingMethodId = flashcardMethod?.TeachingMethodId,
                    TeachingMethodName = flashcardMethod?.Name ?? "Flashcard Method",
                    Instructions = $"Gradually reveal the flashcard for {topic} from behind a cover. Model the correct pronunciation with big mouth gestures. Introduce the target sentence pattern.",
                    TeacherScript = $"Teacher: Look closely! It has tiny legs and it walks... One, two, three... It is an {topic.ToLower()}! Say it with me: {topic}!",
                    ExpectedStudentResponse = $"Students: {topic}! {topic}!",
                    SortOrder = 2
                },
                new()
                {
                    StepName = "3. Guided Choral Chanting & TPR Drill",
                    DurationMinutes = step3Mins,
                    TeachingMethodId = repeatMethod?.TeachingMethodId,
                    TeachingMethodName = repeatMethod?.Name ?? "Repetition & Chanting",
                    Instructions = $"Lead a rhythmic chant using Total Physical Response. Students stomp or crawl while chanting the core sentence pattern together in groups.",
                    TeacherScript = $"Teacher: Let's pretend to march like an {topic.ToLower()}! March, march! Chant with me: 'I see an {topic.ToLower()}! Tiny, tiny {topic.ToLower()}!'",
                    ExpectedStudentResponse = $"Students: (Marching in place) I see an {topic.ToLower()}! Tiny, tiny {topic.ToLower()}!",
                    SortOrder = 3
                },
                new()
                {
                    StepName = "4. Communicative Speaking Game (Magic Pass)",
                    DurationMinutes = step4Mins,
                    TeachingMethodId = roleplayMethod?.TeachingMethodId,
                    TeachingMethodName = roleplayMethod?.Name ?? "Role-play & Dramatization",
                    Instructions = $"Play an interactive pass-and-say game. Pass a soft toy or picture card while music plays. When the music stops, the child stands and speaks the sentence pattern.",
                    TeacherScript = $"Teacher: Pass the card quickly! (Music stops) Sarah, show your card! What do you see?",
                    ExpectedStudentResponse = $"Students: I see a {topic.ToLower()}! It is small and fast!",
                    SortOrder = 4
                },
                new()
                {
                    StepName = "5. Quick Oral Review & Goodbye Cheer",
                    DurationMinutes = step5Mins,
                    TeachingMethodId = qaMethod?.TeachingMethodId,
                    TeachingMethodName = qaMethod?.Name ?? "Question and Answer (Q&A) Drill",
                    Instructions = $"Perform a fast round of high-five flashcard taps. Each child says one word or sentence before giving a high-five and singing the Goodbye Song.",
                    TeacherScript = $"Teacher: Fantastic speaking today, superstars! Who can tell me our magic word for high-five?",
                    ExpectedStudentResponse = $"Students: {topic}! High five, teacher! Goodbye!",
                    SortOrder = 5
                }
            }
        };

        return response;
    }
}
