using System;
using AcademIQ.Shared.Enums;

namespace AcademIQ.Shared.Models;

public class ActivityNotification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public NotificationType Type { get; set; } = NotificationType.General;
    public bool IsRead { get; set; } = false;
    public string? ActionUrl { get; set; }
    public string? CourseCode { get; set; }
}

