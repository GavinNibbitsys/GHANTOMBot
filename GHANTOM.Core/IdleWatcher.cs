using System.Runtime.InteropServices;

namespace GHANTOM.Core;

/// <summary>
/// Time since the last keyboard/mouse input, system-wide. Powers AFK jabs
/// ("been staring at that for 6 minutes, huh").
/// </summary>
public static class IdleWatcher
{
    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    /// <summary>How long since the user last touched keyboard or mouse.</summary>
    public static TimeSpan GetIdleTime()
    {
        var lii = new LASTINPUTINFO { cbSize = (uint)Marshal.SizeOf<LASTINPUTINFO>() };
        if (!GetLastInputInfo(ref lii))
            return TimeSpan.Zero;

        // Both are unsigned tick counts; unchecked subtraction handles wraparound.
        uint idleMs = unchecked((uint)Environment.TickCount - lii.dwTime);
        return TimeSpan.FromMilliseconds(idleMs);
    }
}
