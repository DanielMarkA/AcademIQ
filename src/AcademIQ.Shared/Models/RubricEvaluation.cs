using System;

namespace AcademIQ.Shared.Models;

public class RubricEvaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SubmissionId { get; set; }
    public Guid RubricCriterionId { get; set; }
    public Guid? SelectedLevelId { get; set; }
    public decimal AwardedPoints { get; set; }
    public string FeedbackNotes { get; set; } = string.Empty;
}

