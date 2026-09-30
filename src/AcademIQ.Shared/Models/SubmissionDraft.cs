using System;
using AcademIQ.Shared.Enums;

namespace AcademIQ.Shared.Models;

public class SubmissionDraft
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string MarkdownContent { get; set; } = string.Empty;
    public DateTime LastSavedLocallyAt { get; set; } = DateTime.UtcNow;
    public SyncStatus SyncStatus { get; set; } = SyncStatus.LocalOnly;
    public int WordCount { get; set; } = 0;
    public int CharacterCount { get; set; } = 0;
    public int Version { get; set; } = 1;
}

