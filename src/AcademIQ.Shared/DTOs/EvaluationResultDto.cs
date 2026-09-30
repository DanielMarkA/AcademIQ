using System;

namespace AcademIQ.Shared.DTOs;

public class EvaluationResultDto
{
    public Guid SubmissionId { get; set; }
    public decimal TotalAwardedScore { get; set; }
    public decimal MaxPossibleScore { get; set; }
    public decimal Percentage { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public double GpaValue { get; set; }
    public bool IsPassing { get; set; }
}

