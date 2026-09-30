using System;
using System.Collections.Generic;

namespace AcademIQ.Shared.Models;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string InstructorEmail { get; set; } = string.Empty;
    public string AcademicTerm { get; set; } = "Fall 2026";
    public string ColorHex { get; set; } = "#4F46E5";
    public string Icon { get; set; } = "book";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Assignment> Assignments { get; set; } = new();
    public List<Milestone> Milestones { get; set; } = new();
}

