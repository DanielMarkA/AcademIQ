using System;
using System.Collections.Generic;
using System.Linq;
using AcademIQ.Shared.Common;
using AcademIQ.Shared.DTOs;
using AcademIQ.Shared.Enums;
using AcademIQ.Shared.Models;

namespace AcademIQ.Client.Services;

public class AppStateService
{
    public event Action? OnChange;
    public event Action<string, string>? OnToast;

    public bool IsOfflineMode { get; set; } = true;
    public UserProfile CurrentUser { get; set; } = new();
    public List<Course> Courses { get; set; } = new();
    public List<Assignment> Assignments { get; set; } = new();
    public List<SubmissionDraft> Drafts { get; set; } = new();
    public List<Milestone> Milestones { get; set; } = new();
    public List<ActivityNotification> Notifications { get; set; } = new();
    public List<string> PendingUploadFiles { get; set; } = new() { "recovery-chart.png", "results.csv" };

    public Guid CurrentSubmissionId { get; set; } = Guid.NewGuid();
    public Dictionary<Guid, RubricEvaluation> ActiveEvaluations { get; set; } = new();
    public string OverallGradingFeedback { get; set; } = "Strong analysis overall. Make sure to cite performance comparison tables in the final revision.";

    public SubmissionDraft? ActiveDraft { get; set; }
    public Dictionary<string, bool> CourseOfflineStatus { get; set; } = new()
    {
        { "CS-401", true },
        { "CS-315", true },
        { "ENG-210", false }
    };

    public AppStateService()
    {
        InitializeSeedData();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void TriggerToast(string message, string type = "success")
    {
        OnToast?.Invoke(message, type);
    }

    public void ToggleOfflineMode()
    {
        IsOfflineMode = !IsOfflineMode;
        TriggerToast(IsOfflineMode ? "Switched to Simulated Offline Mode" : "Connected to Campus Network & Synced", IsOfflineMode ? "info" : "success");
        NotifyStateChanged();
    }

    public void SwitchUser(UserRole role)
    {
        if (role == UserRole.Instructor)
        {
            CurrentUser = new UserProfile
            {
                StudentIdNumber = "FAC-1002",
                FullName = "Prof. Lee",
                Email = "prof.lee@northbridge.edu",
                Role = UserRole.Instructor,
                Department = "Computer Science",
                Major = "Systems & Architecture",
                OfflineModeEnabled = true,
                AutoSyncEnabled = true
            };
            TriggerToast("Switched user context to Prof. Lee (Instructor)");
        }
        else
        {
            CurrentUser = new UserProfile
            {
                StudentIdNumber = "STU-20418",
                FullName = "Maya Chen",
                Email = "maya.chen@northbridge.edu",
                Role = UserRole.Student,
                Department = "Computer Science",
                Major = "B.S. Software Engineering",
                CurrentSemester = 6,
                CumulativeGpa = 3.88,
                StorageQuotaBytes = 2147483648,
                UsedStorageBytes = 509607936,
                OfflineModeEnabled = true,
                AutoSyncEnabled = true
            };
            TriggerToast("Switched user context to Maya Chen (Student)");
        }
        NotifyStateChanged();
    }

    public void SetCriterionScore(Guid criterionId, decimal points, string feedback = "")
    {
        if (ActiveEvaluations.TryGetValue(criterionId, out var eval))
        {
            eval.AwardedPoints = points;
            if (!string.IsNullOrEmpty(feedback)) eval.FeedbackNotes = feedback;
        }
        else
        {
            ActiveEvaluations[criterionId] = new RubricEvaluation
            {
                SubmissionId = CurrentSubmissionId,
                RubricCriterionId = criterionId,
                AwardedPoints = points,
                FeedbackNotes = feedback
            };
        }
        NotifyStateChanged();
    }

    public EvaluationResultDto GetCurrentEvaluationScore(Assignment assignment)
    {
        return RubricScoreCalculator.CalculateScore(
            CurrentSubmissionId,
            assignment.RubricCriteria,
            ActiveEvaluations.Values
        );
    }

    public void SaveActiveDraft(string content)
    {
        if (ActiveDraft == null) return;

        ActiveDraft.MarkdownContent = content;
        ActiveDraft.LastSavedLocallyAt = DateTime.UtcNow;
        ActiveDraft.SyncStatus = SyncStatus.LocalOnly;
        
        var words = content.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        ActiveDraft.WordCount = words.Length;
        ActiveDraft.CharacterCount = content.Length;

        NotifyStateChanged();
    }

    public void CreateNewDraft(Guid assignmentId, string title)
    {
        var newDraft = new SubmissionDraft
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = CurrentUser.Id,
            AssignmentTitle = title,
            MarkdownContent = $"# {title}\n\n## Introduction\nEnter project background and goals here.\n\n## Methodology\nDescribe test setup and implementation.",
            LastSavedLocallyAt = DateTime.UtcNow,
            SyncStatus = SyncStatus.LocalOnly,
            WordCount = 18,
            CharacterCount = 120,
            Version = 1
        };

        Drafts.Insert(0, newDraft);
        ActiveDraft = newDraft;
        TriggerToast($"Created new draft: {title}");
        NotifyStateChanged();
    }

    public void SubmitCurrentDraft()
    {
        if (ActiveDraft == null) return;

        ActiveDraft.SyncStatus = SyncStatus.Synced;
        Notifications.Insert(0, new ActivityNotification
        {
            Id = Guid.NewGuid(),
            Title = $"{ActiveDraft.AssignmentTitle} Submitted",
            Message = "Your coursework draft has been finalized and queued for instructor grading.",
            Timestamp = DateTime.UtcNow,
            Type = NotificationType.General,
            IsRead = false,
            CourseCode = "CS-401"
        });

        TriggerToast($"{ActiveDraft.AssignmentTitle} successfully submitted!");
        NotifyStateChanged();
    }

    public void SyncPendingUploads()
    {
        if (PendingUploadFiles.Count == 0)
        {
            TriggerToast("No pending uploads in the local sync queue.", "info");
            return;
        }

        var count = PendingUploadFiles.Count;
        PendingUploadFiles.Clear();

        Notifications.Insert(0, new ActivityNotification
        {
            Id = Guid.NewGuid(),
            Title = "Uploads successfully synchronized",
            Message = $"{count} file attachments were uploaded to university servers.",
            Timestamp = DateTime.UtcNow,
            Type = NotificationType.SyncCompleted,
            IsRead = false,
            CourseCode = "Saved work"
        });

        TriggerToast($"Successfully uploaded {count} attachments!");
        NotifyStateChanged();
    }

    public void RefreshCourseMaterials()
    {
        TriggerToast("Course materials updated and cached for offline use!");
        NotifyStateChanged();
    }

    public void ToggleCourseOffline(string courseCode)
    {
        if (CourseOfflineStatus.TryGetValue(courseCode, out var isOffline))
        {
            CourseOfflineStatus[courseCode] = !isOffline;
            var state = CourseOfflineStatus[courseCode] ? "downloaded for offline use" : "removed from local storage";
            TriggerToast($"{courseCode} {state}");
            NotifyStateChanged();
        }
    }

    public Course? GetCourseByCode(string code)
    {
        return Courses.FirstOrDefault(c => string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase)) 
               ?? Courses.FirstOrDefault();
    }

    public void MarkAllNotificationsAsRead()
    {
        foreach (var notif in Notifications)
        {
            notif.IsRead = true;
        }
        TriggerToast("Marked all notifications as read");
        NotifyStateChanged();
    }

    private void InitializeSeedData()
    {
        CurrentUser = new UserProfile
        {
            StudentIdNumber = "STU-20418",
            FullName = "Maya Chen",
            Email = "maya.chen@northbridge.edu",
            Role = UserRole.Student,
            Department = "Computer Science",
            Major = "B.S. Software Engineering",
            CurrentSemester = 6,
            CumulativeGpa = 3.88,
            StorageQuotaBytes = 2147483648,
            UsedStorageBytes = 509607936,
            OfflineModeEnabled = true,
            AutoSyncEnabled = true
        };

        var cs401 = new Course
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Code = "CS-401",
            Title = "Advanced Systems",
            Description = "Study modern operating systems through reliability, concurrency, storage, and hands-on system design.",
            InstructorName = "Prof. Lee",
            InstructorEmail = "prof.lee@northbridge.edu",
            AcademicTerm = "Fall 2026",
            ColorHex = "#1E7E5A"
        };

        var cs315 = new Course
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Code = "CS-315",
            Title = "Data Structures",
            Description = "Rigorous exploration of abstract data types, balanced trees, graph traversal, and complexity analysis.",
            InstructorName = "Dr. Sofia Malik",
            InstructorEmail = "sofia.malik@northbridge.edu",
            AcademicTerm = "Fall 2026",
            ColorHex = "#2563EB"
        };

        var eng210 = new Course
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Code = "ENG-210",
            Title = "Technical Writing",
            Description = "Clear rhetoric, API documentation, research proposals, and engineering reports.",
            InstructorName = "Elena Ruiz",
            InstructorEmail = "elena.ruiz@northbridge.edu",
            AcademicTerm = "Fall 2026",
            ColorHex = "#D97706"
        };

        Courses = new List<Course> { cs401, cs315, eng210 };

        var lab3Crit1 = new RubricCriterion
        {
            Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
            Title = "Technical Depth",
            Description = "Architecture, reasoning, and correctness",
            MaxPoints = 5m,
            Weight = 1.0m,
            OrderIndex = 1
        };
        var lab3Crit2 = new RubricCriterion
        {
            Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
            Title = "Code Quality",
            Description = "Readability, structure, and maintainability",
            MaxPoints = 5m,
            Weight = 1.0m,
            OrderIndex = 2
        };
        var lab3Crit3 = new RubricCriterion
        {
            Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
            Title = "Documentation",
            Description = "Methods, evidence, and references",
            MaxPoints = 5m,
            Weight = 1.0m,
            OrderIndex = 3
        };

        var labReport3 = new Assignment
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            CourseId = cs401.Id,
            CourseCode = "CS-401",
            CourseTitle = "Advanced Systems",
            Title = "Lab Report 3",
            Description = "Analyze system recovery behavior and communicate your findings in a concise technical report.",
            InstructionsMarkdown = "Observe, test, and explain recovery behavior when interrupted by sudden process termination.",
            DueDate = DateTime.UtcNow.AddDays(4),
            TotalPoints = 100m,
            Status = AssignmentStatus.InProgress,
            RubricCriteria = new List<RubricCriterion> { lab3Crit1, lab3Crit2, lab3Crit3 }
        };

        Assignments = new List<Assignment> { labReport3 };

        ActiveEvaluations[lab3Crit1.Id] = new RubricEvaluation
        {
            SubmissionId = CurrentSubmissionId,
            RubricCriterionId = lab3Crit1.Id,
            AwardedPoints = 4m,
            FeedbackNotes = "Strong analysis; add one edge-case test for file recovery."
        };
        ActiveEvaluations[lab3Crit2.Id] = new RubricEvaluation
        {
            SubmissionId = CurrentSubmissionId,
            RubricCriterionId = lab3Crit2.Id,
            AwardedPoints = 5m,
            FeedbackNotes = "Clear modules and consistent naming throughout."
        };
        ActiveEvaluations[lab3Crit3.Id] = new RubricEvaluation
        {
            SubmissionId = CurrentSubmissionId,
            RubricCriterionId = lab3Crit3.Id,
            AwardedPoints = 3m,
            FeedbackNotes = "Cite the performance table and expand setup notes."
        };

        var initialDraft = new SubmissionDraft
        {
            Id = Guid.NewGuid(),
            AssignmentId = labReport3.Id,
            StudentId = CurrentUser.Id,
            AssignmentTitle = "Lab Report 3 Draft",
            MarkdownContent = @"# Lab Report 3

## Method
We implemented a service worker to cache course resources.
- Offline shell
- IndexedDB queue
- Conflict resolution

## 3. Results
We compared checkpoint recovery with log replay across five interrupted write runs.

### 3.1 Recovery time

| Strategy | Median | Data restored |
| :--- | :---: | :---: |
| Checkpoint | 84 ms | 100% |
| Log replay | 127 ms | 100% |

The checkpoint approach recovered **34% faster** in our test environment. However, its periodic snapshots added a small writing cost during normal operation.

> Each trial was repeated three times after a clean restart.

## 4. Discussion
Through consistent state journaling, offline data mutations remain reliable across browser lifecycles.",
            LastSavedLocallyAt = DateTime.UtcNow.AddMinutes(-2),
            SyncStatus = SyncStatus.PendingSync,
            WordCount = 842,
            CharacterCount = 5120,
            Version = 3
        };

        var draft2 = new SubmissionDraft
        {
            Id = Guid.NewGuid(),
            AssignmentId = Guid.NewGuid(),
            StudentId = CurrentUser.Id,
            AssignmentTitle = "Lab 2",
            MarkdownContent = "# Lab 2: Concurrency Patterns\n\nSubmitted final code and analysis.",
            LastSavedLocallyAt = DateTime.UtcNow.AddDays(-7),
            SyncStatus = SyncStatus.Synced,
            WordCount = 1250,
            CharacterCount = 7400,
            Version = 5
        };

        var draft3 = new SubmissionDraft
        {
            Id = Guid.NewGuid(),
            AssignmentId = Guid.NewGuid(),
            StudentId = CurrentUser.Id,
            AssignmentTitle = "Final Project",
            MarkdownContent = "# Distributed Cache Architecture",
            LastSavedLocallyAt = DateTime.UtcNow.AddDays(-14),
            SyncStatus = SyncStatus.LocalOnly,
            WordCount = 120,
            CharacterCount = 800,
            Version = 1
        };

        Drafts = new List<SubmissionDraft> { initialDraft, draft2, draft3 };
        ActiveDraft = initialDraft;

        Milestones = new List<Milestone>
        {
            new Milestone
            {
                Id = Guid.NewGuid(),
                CourseId = cs401.Id,
                Title = "Lab Report 3",
                Description = "Draft + rubric",
                StartDate = DateTime.UtcNow.AddDays(-3),
                DueDate = DateTime.UtcNow.AddDays(4),
                Status = MilestoneStatus.InProgress,
                OrderIndex = 1
            },
            new Milestone
            {
                Id = Guid.NewGuid(),
                CourseId = cs401.Id,
                Title = "Midterm",
                Description = "Oct 19 · 09:00",
                StartDate = DateTime.UtcNow.AddDays(7),
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = MilestoneStatus.InProgress,
                OrderIndex = 2
            },
            new Milestone
            {
                Id = Guid.NewGuid(),
                CourseId = cs401.Id,
                Title = "Final Project",
                Description = "Research build",
                StartDate = DateTime.UtcNow.AddDays(20),
                DueDate = DateTime.UtcNow.AddDays(35),
                Status = MilestoneStatus.NotStarted,
                OrderIndex = 3
            }
        };

        Notifications = new List<ActivityNotification>
        {
            new ActivityNotification
            {
                Id = Guid.NewGuid(),
                Title = "Grade posted for Systems Quiz 2",
                Message = "You earned 18/20. Review the two questions you missed.",
                Timestamp = DateTime.UtcNow.AddHours(-1),
                Type = NotificationType.GradePublished,
                IsRead = false,
                CourseCode = "CS-401"
            },
            new ActivityNotification
            {
                Id = Guid.NewGuid(),
                Title = "Prof. Lee left feedback on your draft",
                Message = "\"Your comparison is clear. Add one sentence about the limits of the sample size.\"",
                Timestamp = DateTime.UtcNow.AddHours(-3),
                Type = NotificationType.General,
                IsRead = false,
                CourseCode = "CS-401"
            },
            new ActivityNotification
            {
                Id = Guid.NewGuid(),
                Title = "Your saved work is up to date",
                Message = "Lab Report 3 and two attachments were sent successfully.",
                Timestamp = DateTime.UtcNow.AddHours(-5),
                Type = NotificationType.SyncCompleted,
                IsRead = true,
                CourseCode = "Saved work"
            },
            new ActivityNotification
            {
                Id = Guid.NewGuid(),
                Title = "Lab Report 3 is due Friday",
                Message = "Your draft has 842 words. Submit by Oct 4 at 11:59 PM.",
                Timestamp = DateTime.UtcNow.AddDays(-1),
                Type = NotificationType.AssignmentDue,
                IsRead = true,
                CourseCode = "CS-401"
            },
            new ActivityNotification
            {
                Id = Guid.NewGuid(),
                Title = "New course announcement",
                Message = "Sample recovery data is now available in Course Materials.",
                Timestamp = DateTime.UtcNow.AddDays(-2),
                Type = NotificationType.CourseAnnouncement,
                IsRead = true,
                CourseCode = "CS-401"
            },
            new ActivityNotification
            {
                Id = Guid.NewGuid(),
                Title = "Proposal revision received",
                Message = "Your Technical Writing assignment was received and is awaiting review.",
                Timestamp = DateTime.UtcNow.AddDays(-3),
                Type = NotificationType.General,
                IsRead = true,
                CourseCode = "ENG-210"
            }
        };
    }
}
