using System;
using System.Collections.Generic;
using System.Linq;
using AcademIQ.Shared.DTOs;
using AcademIQ.Shared.Models;

namespace AcademIQ.Shared.Common;

public static class RubricScoreCalculator
{
    public static EvaluationResultDto CalculateScore(
        Guid submissionId,
        IEnumerable<RubricCriterion>? criteria,
        IEnumerable<RubricEvaluation>? evaluations)
    {
        var criteriaList = criteria?.ToList() ?? new List<RubricCriterion>();
        var evalDict = evaluations?
            .GroupBy(e => e.RubricCriterionId)
            .ToDictionary(g => g.Key, g => g.Last()) 
            ?? new Dictionary<Guid, RubricEvaluation>();

        decimal totalEarned = 0m;
        decimal totalPossible = 0m;

        foreach (var crit in criteriaList)
        {
            var maxCritPoints = crit.MaxPoints * crit.Weight;
            totalPossible += maxCritPoints;

            if (evalDict.TryGetValue(crit.Id, out var eval))
            {
                totalEarned += eval.AwardedPoints * crit.Weight;
            }
        }

        if (totalPossible <= 0)
        {
            totalPossible = 100m;
        }

        var percentage = Math.Round((totalEarned / totalPossible) * 100m, 2);
        var letterGrade = GetLetterGrade(percentage);
        var gpa = GetGpaValue(percentage);

        return new EvaluationResultDto
        {
            SubmissionId = submissionId,
            TotalAwardedScore = Math.Round(totalEarned, 2),
            MaxPossibleScore = Math.Round(totalPossible, 2),
            Percentage = percentage,
            LetterGrade = letterGrade,
            GpaValue = gpa,
            IsPassing = percentage >= 60m
        };
    }

    public static string GetLetterGrade(decimal percentage)
    {
        return percentage switch
        {
            >= 93m => "A",
            >= 90m => "A-",
            >= 87m => "B+",
            >= 83m => "B",
            >= 80m => "B-",
            >= 77m => "C+",
            >= 73m => "C",
            >= 70m => "C-",
            >= 67m => "D+",
            >= 60m => "D",
            _ => "F"
        };
    }

    public static double GetGpaValue(decimal percentage)
    {
        return percentage switch
        {
            >= 93m => 4.0,
            >= 90m => 3.7,
            >= 87m => 3.3,
            >= 83m => 3.0,
            >= 80m => 2.7,
            >= 77m => 2.3,
            >= 73m => 2.0,
            >= 70m => 1.7,
            >= 67m => 1.3,
            >= 60m => 1.0,
            _ => 0.0
        };
    }
}

