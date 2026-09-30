using System;
using System.Collections.Generic;
using AcademIQ.Shared.Enums;

namespace AcademIQ.Shared.Models;

public class Assignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InstructionsMarkdown { get; set; } = string.Empty;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public decimal TotalPoints { get; set; } = 100m;
    public decimal WeightPercentage { get; set; } = 20m;
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Upcoming;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<RubricCriterion> RubricCriteria { get; set; } = new();
    public List<Submission> Submissions { get; set; } = new();
}

