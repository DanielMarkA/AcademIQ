using System;
using System.Collections.Generic;

namespace AcademIQ.Shared.Models;

public class RubricCriterion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssignmentId { get; set; }
    public string Title { get; set; } = string.Empty; // e.g. "Technical Depth", "Code Style & Modularity"
    public string Description { get; set; } = string.Empty;
    public decimal MaxPoints { get; set; } = 25m;
    public decimal Weight { get; set; } = 1.0m; // multiplier or weight
    public int OrderIndex { get; set; }
    public List<RubricLevel> Levels { get; set; } = new();
}

