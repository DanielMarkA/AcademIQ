using System;
using AcademIQ.Shared.Enums;

namespace AcademIQ.Shared.Models;

public class UserProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StudentIdNumber { get; set; } = "STU-2026-8841";
    public string FullName { get; set; } = "Daniel Aguilar";
    public string Email { get; set; } = "daniel.aguilar@university.edu";
    public string AvatarUrl { get; set; } = "images/default-avatar.png";
    public UserRole Role { get; set; } = UserRole.Student;
    public string Department { get; set; } = "Computer Science & Engineering";
    public string Major { get; set; } = "B.S. Software Engineering";
    public int CurrentSemester { get; set; } = 6;
    public double CumulativeGpa { get; set; } = 3.88;
    public long StorageQuotaBytes { get; set; } = 52428800; // 50MB local storage quota
    public long UsedStorageBytes { get; set; } = 1258291;   // ~1.2MB
    public bool OfflineModeEnabled { get; set; } = true;
    public bool AutoSyncEnabled { get; set; } = true;
}

