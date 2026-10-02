using System;
using System.Collections.Generic;
using AcademIQ.Shared.Common;
using AcademIQ.Shared.Models;
using Xunit;

namespace AcademIQ.Tests;

public class RubricScoreCalculatorTests
{
    [Fact]
    public void CalculateScore_WithValidCriteriaAndEvaluations_CalculatesCorrectWeightedScore()
    {
        var submissionId = Guid.NewGuid();
        var crit1 = new RubricCriterion { Id = Guid.NewGuid(), Title = "Technical Depth", MaxPoints = 5m, Weight = 1.0m };
        var crit2 = new RubricCriterion { Id = Guid.NewGuid(), Title = "Code Quality", MaxPoints = 5m, Weight = 1.0m };
        var criteria = new List<RubricCriterion> { crit1, crit2 };

        var evals = new List<RubricEvaluation>
        {
            new() { SubmissionId = submissionId, RubricCriterionId = crit1.Id, AwardedPoints = 4m },
            new() { SubmissionId = submissionId, RubricCriterionId = crit2.Id, AwardedPoints = 5m }
        };

        var result = RubricScoreCalculator.CalculateScore(submissionId, criteria, evals);

        Assert.Equal(submissionId, result.SubmissionId);
        Assert.Equal(9.00m, result.TotalAwardedScore);
        Assert.Equal(10.00m, result.MaxPossibleScore);
        Assert.Equal(90.00m, result.Percentage);
        Assert.Equal("A-", result.LetterGrade);
        Assert.Equal(3.7, result.GpaValue);
        Assert.True(result.IsPassing);
    }

    [Fact]
    public void CalculateScore_WithEmptyCriteria_ReturnsSafeDefaults()
    {
        var submissionId = Guid.NewGuid();
        var result = RubricScoreCalculator.CalculateScore(submissionId, null, null);

        Assert.Equal(submissionId, result.SubmissionId);
        Assert.Equal(0m, result.TotalAwardedScore);
        Assert.Equal(100m, result.MaxPossibleScore);
        Assert.Equal(0m, result.Percentage);
        Assert.Equal("F", result.LetterGrade);
        Assert.False(result.IsPassing);
    }

    [Theory]
    [InlineData(95, "A")]
    [InlineData(91, "A-")]
    [InlineData(88, "B+")]
    [InlineData(84, "B")]
    [InlineData(80, "B-")]
    [InlineData(78, "C+")]
    [InlineData(74, "C")]
    [InlineData(70, "C-")]
    [InlineData(65, "D")]
    [InlineData(50, "F")]
    public void GetLetterGrade_WithDifferentScoreRanges_ReturnsExpectedGrades(decimal score, string expectedGrade)
    {
        var grade = RubricScoreCalculator.GetLetterGrade(score);
        Assert.Equal(expectedGrade, grade);
    }

    [Theory]
    [InlineData(95, 4.0)]
    [InlineData(90, 3.7)]
    [InlineData(87, 3.3)]
    [InlineData(83, 3.0)]
    [InlineData(80, 2.7)]
    [InlineData(77, 2.3)]
    [InlineData(73, 2.0)]
    [InlineData(70, 1.7)]
    [InlineData(60, 1.0)]
    [InlineData(55, 0.0)]
    public void GetGpaValue_WithDifferentScoreRanges_ReturnsExpectedGpa(decimal score, double expectedGpa)
    {
        var gpa = RubricScoreCalculator.GetGpaValue(score);
        Assert.Equal(expectedGpa, gpa);
    }
}
