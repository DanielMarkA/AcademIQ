using System;
using System.Collections.Generic;
using AcademIQ.Shared.Enums;

namespace AcademIQ.Shared.Models;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string MarkdownContent { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public decimal? FinalScore { get; set; }
    public decimal MaxScore { get; set; } = 100m;
    public string OverallFeedback { get; set; } = string.Empty;
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Submitted;
    public SyncStatus SyncStatus { get; set; } = SyncStatus.Synced;
    public DateTime? GradedAt { get; set; }
    public string GradedBy { get; set; } = string.Empty;

    public List<RubricEvaluation> Evaluations { get; set; } = new();
}

