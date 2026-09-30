using System;
using System.Collections.Generic;

namespace AcademIQ.Shared.DTOs;

public class EvaluationItemDto
{
    public Guid CriterionId { get; set; }
    public Guid? SelectedLevelId { get; set; }
    public decimal AwardedPoints { get; set; }
    public string FeedbackNotes { get; set; } = string.Empty;
}

public class EvaluationRequestDto
{
    public Guid SubmissionId { get; set; }
    public Guid AssignmentId { get; set; }
    public string EvaluatorName { get; set; } = string.Empty;
    public string OverallFeedback { get; set; } = string.Empty;
    public List<EvaluationItemDto> CriterionEvaluations { get; set; } = new();
}

