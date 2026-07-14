using System.IO;
using System.Text.Json;
using DrMusa.Common.Enums;

namespace DrMusa.Desktop.Helpers;

/// <summary>
/// Manages the currently authenticated user session and application lock state.
/// Also persists "Remember Me" username between application restarts.
/// </summary>
public static class SessionManager
{
    // ── File path for persisting Remember Me ──────────────────────────────────
    private static readonly string _settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DrMusa", "session.json");

    // ── Session State ─────────────────────────────────────────────────────────
    public static int?      CurrentUserId    { get; private set; }
    public static string?   CurrentUsername  { get; private set; }
    public static string?   CurrentFullName  { get; private set; }
    public static UserRole? CurrentRole      { get; private set; }
    public static bool      IsLoggedIn       => CurrentUserId.HasValue;
    public static bool      IsLocked         { get; private set; }
    public static DateTime  LastActivity     { get; private set; } = DateTime.Now;

    // ── Remember Me ───────────────────────────────────────────────────────────
    public static string?   RememberedUsername  { get; private set; }
    public static bool      RememberMeEnabled   { get; private set; }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>Called after successful login to populate session.</summary>
    public static void SetUser(Business.DTOs.UserDto user, bool rememberMe = false)
    {
        CurrentUserId   = user.Id;
        CurrentUsername = user.Username;
        CurrentFullName = user.FullName;
        CurrentRole     = user.Role;
        IsLocked        = false;
        LastActivity    = DateTime.Now;

        RememberMeEnabled   = rememberMe;
        RememberedUsername  = rememberMe ? user.Username : null;

        PersistSettings(rememberMe ? user.Username : null);
    }

    /// <summary>Clears session on logout.</summary>
    public static void Clear()
    {
        CurrentUserId   = null;
        CurrentUsername = null;
        CurrentFullName = null;
        CurrentRole     = null;
        IsLocked        = false;
    }

    /// <summary>Locks the application without clearing the session.</summary>
    public static void Lock()
    {
        if (IsLoggedIn)
            IsLocked = true;
    }

    /// <summary>Unlocks the application after password re-entry succeeds.</summary>
    public static void Unlock()
    {
        IsLocked     = false;
        LastActivity = DateTime.Now;
    }

    /// <summary>Updates the last activity timestamp.</summary>
    public static void RecordActivity()
        => LastActivity = DateTime.Now;

    /// <summary>Checks if the current user has the specified role.</summary>
    public static bool HasRole(params UserRole[] roles)
    {
        if (!IsLoggedIn || CurrentRole == null) return false;
        
        // Owner has access to everything
        if (CurrentRole == UserRole.Owner) return true;

        return roles.Contains(CurrentRole.Value);
    }

    /// <summary>Checks if current user is Owner or SubAdmin.</summary>
    public static bool IsOwnerOrSubAdmin
        => CurrentRole is UserRole.Owner or UserRole.SubAdmin;

    // ── Persistence ───────────────────────────────────────────────────────────

    /// <summary>Loads remembered username from disk on app startup.</summary>
    public static void LoadSavedSettings()
    {
        try
        {
            if (!File.Exists(_settingsPath)) return;
            var json = File.ReadAllText(_settingsPath);
            var data = JsonSerializer.Deserialize<SessionSettings>(json);
            if (data != null && !string.IsNullOrEmpty(data.RememberedUsername))
            {
                RememberedUsername = data.RememberedUsername;
                RememberMeEnabled  = true;
            }
        }
        catch { /* Ignore persistence failures — non-critical */ }
    }

    private static void PersistSettings(string? rememberedUsername)
    {
        try
        {
            var dir = Path.GetDirectoryName(_settingsPath)!;
            Directory.CreateDirectory(dir);
            var data = new SessionSettings { RememberedUsername = rememberedUsername };
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(data));
        }
        catch { /* Ignore persistence failures — non-critical */ }
    }

    private record SessionSettings
    {
        public string? RememberedUsername { get; init; }
    }
}
