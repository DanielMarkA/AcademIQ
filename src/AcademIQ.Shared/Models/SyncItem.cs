using System;
using AcademIQ.Shared.Enums;

namespace AcademIQ.Shared.Models;

public class SyncItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EntityType { get; set; } = string.Empty; // "SubmissionDraft", "RubricEvaluation", etc.
    public Guid EntityId { get; set; }
    public string Action { get; set; } = "UPSERT"; // "CREATE", "UPDATE", "DELETE", "UPSERT"
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public SyncStatus Status { get; set; } = SyncStatus.PendingSync;
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; } = 0;
}

