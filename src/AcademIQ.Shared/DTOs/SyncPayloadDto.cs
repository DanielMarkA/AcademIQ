using System;
using System.Collections.Generic;
using AcademIQ.Shared.Models;

namespace AcademIQ.Shared.DTOs;

public class SyncBatchRequestDto
{
    public Guid StudentId { get; set; }
    public DateTime ClientTimestampUtc { get; set; } = DateTime.UtcNow;
    public List<SubmissionDraft> PendingDrafts { get; set; } = new();
    public List<SyncItem> QueuedOperations { get; set; } = new();
}

public class SyncBatchResponseDto
{
    public bool Success { get; set; }
    public int ProcessedCount { get; set; }
    public DateTime ServerTimestampUtc { get; set; } = DateTime.UtcNow;
    public List<Guid> SuccessfullySyncedDraftIds { get; set; } = new();
    public List<string> SyncErrors { get; set; } = new();
    public List<ActivityNotification> NewNotifications { get; set; } = new();
}

