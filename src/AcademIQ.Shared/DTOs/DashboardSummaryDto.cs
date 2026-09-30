using System;
using System.Collections.Generic;
using AcademIQ.Shared.Models;

namespace AcademIQ.Shared.DTOs;

public class DashboardSummaryDto
{
    public UserProfile User { get; set; } = new();
    public int EnrolledCoursesCount { get; set; }
    public int ActiveAssignmentsCount { get; set; }
    public int PendingSubmissionsCount { get; set; }
    public int GradedSubmissionsCount { get; set; }
    public decimal OverallAverageScore { get; set; }
    public int LocalOfflineDraftsCount { get; set; }

    public List<Assignment> UpcomingAssignments { get; set; } = new();
    public List<Milestone> ActiveMilestones { get; set; } = new();
    public List<ActivityNotification> RecentActivity { get; set; } = new();
    public List<Course> EnrolledCourses { get; set; } = new();
}

