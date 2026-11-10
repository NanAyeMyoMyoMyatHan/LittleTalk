using System.Text;
using LittleTalk.Model.Features.LessonPlans;

namespace LittleTalk.Application.Features.LessonPlans.Services;

/// <summary>
/// Constructs structured pedagogical prompts for young learner English speaking lesson plan generation.
/// </summary>
public static class AiPromptBuilder
{
    public static string BuildSystemInstruction()
    {
        return """
You are an expert early childhood English language teaching curriculum designer specializing in interactive, high-engagement speaking lessons for children aged 4 to 12.

Your goal is to generate a comprehensive, highly practical, and age-appropriate English speaking lesson plan.

Pedagogical Principles for Young Learners:
1. Speaking & Listening First: Emphasize oral participation, rhythm, repetition, choral drilling, and interactive games.
2. 5-Stage Structured Teaching Flow:
   - Stage 1: Warm-up & Hello Song (Energizer, movement, activate background knowledge)
   - Stage 2: Presentation & Concept Introduction (Introduce words/patterns with flashcards, realia, or gestures)
   - Stage 3: Guided Practice & Repetition (Choral chants, call-and-response, structured drills)
   - Stage 4: Free Production & Interactive Speaking Game (Fun communicative game, pair/group speaking challenge)
   - Stage 5: Review, Assessment & Wrap-up (Quick oral check, positive praise, goodbye routine)
3. Step Durations: The sum of step durations must exactly match the total lesson duration.
4. Teacher Scripts: Include explicit questions the teacher asks and the exact expected student responses.
5. Child-Friendly Vocabulary: Provide simple definitions, phonetic pronunciation guides, and natural example sentences.
6. Clean Formatting: Use only standard ASCII characters. Do not use special em-dashes, smart curly quotes, or unusual symbols.

Return ONLY a valid JSON object conforming to the required schema. Do not include markdown preamble or conversational filler.
""";
    }

    public static string BuildUserPrompt(
        GenerateAiLessonPlanRequestModel request, 
        string levelName, 
        string levelAgeRange, 
        IReadOnlyList<string> availableMethods)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Please design a complete English speaking lesson plan with the following specifications:");
        sb.AppendLine($"- Topic: {request.Topic.Trim()}");
        
        var ageGroup = !string.IsNullOrWhiteSpace(request.TargetAgeGroup) 
            ? request.TargetAgeGroup.Trim() 
            : levelAgeRange;
        sb.AppendLine($"- Target Age Group: {ageGroup}");
        sb.AppendLine($"- English Learning Level: {levelName} ({levelAgeRange})");
        sb.AppendLine($"- Total Lesson Duration: {request.DurationMinutes} minutes");

        if (!string.IsNullOrWhiteSpace(request.SpecificFocusNotes))
        {
            sb.AppendLine($"- Specific Focus Notes: {request.SpecificFocusNotes.Trim()}");
        }

        if (availableMethods != null && availableMethods.Count > 0)
        {
            sb.AppendLine($"- Available Teaching Methods in System: {string.Join(", ", availableMethods)}");
        }

        sb.AppendLine();
        sb.AppendLine("Output strictly as a valid JSON object with this exact JSON structure:");
        sb.AppendLine("""
{
  "title": "Engaging Lesson Title",
  "topic": "Topic Name",
  "ageGroup": "Target Age Group",
  "learningLevelId": 1,
  "durationMinutes": 30,
  "description": "Short pedagogical summary of this speaking lesson",
  "learningObjectives": "- Objective 1\n- Objective 2\n- Objective 3",
  "sentencePatterns": [
    "I see a / an [noun].",
    "It is [adjective]."
  ],
  "teachingMaterials": [
    "Flashcards of target words",
    "Realia or toy models",
    "Audio speaker for hello song"
  ],
  "assessmentNotes": "Teacher observation rubric: Observe accurate pronunciation of target words, sentence formation confidence during games, and willingness to respond aloud.",
  "vocabulary": [
    {
      "word": "Ant",
      "pronunciation": "/ant/",
      "meaning": "A tiny insect that marches in a line.",
      "exampleSentence": "Look! The little ant is carrying a crumb.",
      "sortOrder": 1
    }
  ],
  "steps": [
    {
      "stepName": "1. Warm-up & Hello Chant",
      "durationMinutes": 5,
      "teachingMethodName": "Singing, Rhythm & Movement",
      "instructions": "Gather students in a circle. Sing the hello song with hand actions and introduce the lesson topic with an energetic guessing game.",
      "teacherScript": "Teacher: Good morning little explorers! Look inside my mystery box. Can you guess what is inside?",
      "expectedStudentResponse": "Students: Good morning teacher! A toy! An animal!",
      "sortOrder": 1
    }
  ]
}
""");

        return sb.ToString();
    }
}
