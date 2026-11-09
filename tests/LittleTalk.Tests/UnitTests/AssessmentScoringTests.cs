using LittleTalk.Application.Features.Assessments.DTOs;
using Xunit;

namespace LittleTalk.Tests.UnitTests;

public class AssessmentScoringTests
{
    [Fact]
    public void CalculateAverageScore_ShouldReturnCorrectMathematicalAverage()
    {
        // Arrange
        var criteria = new List<CriterionInputDto>
        {
            new() { CriterionName = "Pronunciation", Score = 4 },
            new() { CriterionName = "Vocabulary", Score = 5 },
            new() { CriterionName = "Fluency", Score = 3 },
            new() { CriterionName = "Sentence Formation", Score = 4 },
            new() { CriterionName = "Confidence", Score = 5 },
            new() { CriterionName = "Listening and Responding", Score = 4 }
        };

        // (4 + 5 + 3 + 4 + 5 + 4) = 25 / 6 = 4.16666... => 4.17
        var expectedAverage = 4.17m;

        // Act
        var calculatedAverage = Math.Round(criteria.Average(c => (decimal)c.Score), 2);

        // Assert
        Assert.Equal(expectedAverage, calculatedAverage);
    }

    [Fact]
    public void CalculateAverageScore_ShouldReturnFive_WhenAllScoresAreMax()
    {
        // Arrange
        var criteria = new List<CriterionInputDto>
        {
            new() { CriterionName = "Pronunciation", Score = 5 },
            new() { CriterionName = "Vocabulary", Score = 5 },
            new() { CriterionName = "Fluency", Score = 5 },
            new() { CriterionName = "Sentence Formation", Score = 5 },
            new() { CriterionName = "Confidence", Score = 5 },
            new() { CriterionName = "Listening and Responding", Score = 5 }
        };

        // Act
        var calculatedAverage = Math.Round(criteria.Average(c => (decimal)c.Score), 2);

        // Assert
        Assert.Equal(5.00m, calculatedAverage);
    }

    [Fact]
    public void CalculateAverageScore_ShouldReturnOne_WhenAllScoresAreMin()
    {
        // Arrange
        var criteria = new List<CriterionInputDto>
        {
            new() { CriterionName = "Pronunciation", Score = 1 },
            new() { CriterionName = "Vocabulary", Score = 1 },
            new() { CriterionName = "Fluency", Score = 1 },
            new() { CriterionName = "Sentence Formation", Score = 1 },
            new() { CriterionName = "Confidence", Score = 1 },
            new() { CriterionName = "Listening and Responding", Score = 1 }
        };

        // Act
        var calculatedAverage = Math.Round(criteria.Average(c => (decimal)c.Score), 2);

        // Assert
        Assert.Equal(1.00m, calculatedAverage);
    }
}
