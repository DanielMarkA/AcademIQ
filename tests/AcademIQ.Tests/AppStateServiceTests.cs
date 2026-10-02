using System;
using System.Linq;
using AcademIQ.Client.Services;
using AcademIQ.Shared.Enums;
using Xunit;

namespace AcademIQ.Tests;

public class AppStateServiceTests
{
    [Fact]
    public void ToggleOfflineMode_SwitchesOfflineStateAndTriggersToast()
    {
        var service = new AppStateService();
        var initial = service.IsOfflineMode;
        string? toastMessage = null;

        service.OnToast += (msg, _) => toastMessage = msg;
        service.ToggleOfflineMode();

        Assert.Equal(!initial, service.IsOfflineMode);
        Assert.NotNull(toastMessage);
    }

    [Fact]
    public void SwitchUser_ToInstructor_UpdatesCurrentUserProfile()
    {
        var service = new AppStateService();
        service.SwitchUser(UserRole.Instructor);

        Assert.Equal(UserRole.Instructor, service.CurrentUser.Role);
        Assert.Equal("Prof. Lee", service.CurrentUser.FullName);
    }

    [Fact]
    public void SaveActiveDraft_UpdatesWordCountAndTimestamp()
    {
        var service = new AppStateService();
        var text = "# New Title\n\nThis is a test paragraph with seven words.";

        service.SaveActiveDraft(text);

        Assert.NotNull(service.ActiveDraft);
        Assert.Equal(text, service.ActiveDraft.MarkdownContent);
        Assert.Equal(11, service.ActiveDraft.WordCount);
        Assert.Equal(SyncStatus.LocalOnly, service.ActiveDraft.SyncStatus);
    }

    [Fact]
    public void SubmitCurrentDraft_MarksStatusAsSyncedAndAddsNotification()
    {
        var service = new AppStateService();
        var initialNotifCount = service.Notifications.Count;

        service.SubmitCurrentDraft();

        Assert.NotNull(service.ActiveDraft);
        Assert.Equal(SyncStatus.Synced, service.ActiveDraft.SyncStatus);
        Assert.Equal(initialNotifCount + 1, service.Notifications.Count);
    }

    [Fact]
    public void SetCriterionScore_UpdatesEvaluationAndRecalculatesScore()
    {
        var service = new AppStateService();
        var assignment = service.Assignments.First();
        var criterion = assignment.RubricCriteria.First();

        service.SetCriterionScore(criterion.Id, 5m, "Outstanding work!");

        var score = service.GetCurrentEvaluationScore(assignment);
        Assert.NotNull(score);
        Assert.True(score.TotalAwardedScore > 0);
        Assert.Equal("Outstanding work!", service.ActiveEvaluations[criterion.Id].FeedbackNotes);
    }
}
