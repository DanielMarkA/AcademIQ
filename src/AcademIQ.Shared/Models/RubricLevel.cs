using System;

namespace AcademIQ.Shared.Models;

public class RubricLevel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RubricCriterionId { get; set; }
    public decimal Points { get; set; }
    public string Label { get; set; } = string.Empty; // e.g. "Exemplary", "Proficient", "Developing", "Unacceptable"
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}

