using System;
using AcademIQ.Shared.Enums;

namespace AcademIQ.Shared.Models;

public class Milestone
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public Guid? AssignmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public MilestoneStatus Status { get; set; } = MilestoneStatus.NotStarted;
    public int ProgressPercentage { get; set; } = 0;
    public int OrderIndex { get; set; } = 0;
}

