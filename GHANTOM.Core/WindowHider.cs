using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace GHANTOM.Core;

/// <summary>
/// Hides (never kills) the top-level windows belonging to processes with a
/// given name. The process keeps running untouched — nothing is force-closed
/// and no unsaved work is lost — the window just vanishes, which is
/// unsettling enough on its own for the prank.
/// </summary>
public static class WindowHider
{
    private const int SW_HIDE = 0;

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    /// <summary>Hides every visible top-level window for processes named
    /// <paramref name="processName"/>. Returns how many windows were hidden.</summary>
    public static int HideAll(string processName)
    {
        int hidden = 0;
        foreach (var p in Process.GetProcessesByName(processName))
        {
            try
            {
                IntPtr hwnd = p.MainWindowHandle;
                if (hwnd != IntPtr.Zero && ShowWindow(hwnd, SW_HIDE))
                    hidden++;
            }
            catch
            {
                // Process may have exited between the snapshot and here; skip it.
            }
        }
        return hidden;
    }
}
