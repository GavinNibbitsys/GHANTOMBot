using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GHANTOM.Core;

/// <summary>
/// Reads the currently focused window's title (and owning process). Far more
/// specific than a process-name check — it sees the actual tab/file, e.g.
/// "YouTube - Google Chrome" or "Program.cs - GHANTOM - Visual Studio Code".
/// </summary>
public static class WindowWatcher
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    /// <summary>Title of the foreground window, or "" if none/unreadable.</summary>
    public static string GetActiveWindowTitle()
    {
        IntPtr handle = GetForegroundWindow();
        if (handle == IntPtr.Zero) return "";

        int len = GetWindowTextLength(handle);
        if (len <= 0) return "";

        var sb = new StringBuilder(len + 1);
        GetWindowText(handle, sb, sb.Capacity);
        return sb.ToString();
    }

    /// <summary>Process name owning the foreground window, or "" if unknown.</summary>
    public static string GetActiveProcessName()
    {
        IntPtr handle = GetForegroundWindow();
        if (handle == IntPtr.Zero) return "";

        GetWindowThreadProcessId(handle, out uint pid);
        if (pid == 0) return "";

        try { return Process.GetProcessById((int)pid).ProcessName; }
        catch { return ""; }
    }
}
