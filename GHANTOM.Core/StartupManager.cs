using Microsoft.Win32;

namespace GHANTOM.Core;

/// <summary>
/// Manages the HKCU ...\Run autostart entry. Parameterized by app name so
/// GHANTOM and the second bot each get their own Run value instead of
/// clobbering a single shared key.
/// </summary>
public static class StartupManager
{
    private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    private static string CurrentExePath =>
        "\"" + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\"";

    public static void Enable(string appName)
    {
        using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
        key?.SetValue(appName, CurrentExePath);
    }

    public static void Disable(string appName)
    {
        using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
        if (key?.GetValue(appName) != null)
            key.DeleteValue(appName, false);
    }

    public static bool IsEnabled(string appName)
    {
        using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, false);
        return key != null && key.GetValue(appName) != null;
    }
}
