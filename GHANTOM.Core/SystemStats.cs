using System.Runtime.InteropServices;

namespace GHANTOM.Core;

/// <summary>
/// System-wide CPU and memory load, plus the current hour for time-of-day
/// lines. Uses GetSystemTimes / GlobalMemoryStatusEx so there's no dependency
/// on performance counters (which are often disabled or corrupt on user PCs).
///
/// CPU is measured as the delta between successive <see cref="CpuPercent"/>
/// calls, so keep one instance alive across the poll loop. The first call
/// returns 0 (it only establishes a baseline).
/// </summary>
public sealed class SystemStats
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetSystemTimes(out ulong idle, out ulong kernel, out ulong user);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    [StructLayout(LayoutKind.Sequential)]
    private struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;      // percentage of physical memory in use
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    private ulong _prevIdle, _prevKernel, _prevUser;
    private bool _primed;

    /// <summary>System-wide CPU usage % since the previous call (0 on first call).</summary>
    public double CpuPercent()
    {
        if (!GetSystemTimes(out ulong idle, out ulong kernel, out ulong user))
            return 0;

        if (!_primed)
        {
            _prevIdle = idle; _prevKernel = kernel; _prevUser = user;
            _primed = true;
            return 0;
        }

        ulong idleDelta = idle - _prevIdle;
        ulong kernelDelta = kernel - _prevKernel; // kernel time includes idle
        ulong userDelta = user - _prevUser;
        _prevIdle = idle; _prevKernel = kernel; _prevUser = user;

        ulong total = kernelDelta + userDelta;
        if (total == 0) return 0;

        double busy = (double)(total - idleDelta) / total;
        if (busy < 0) busy = 0;
        if (busy > 1) busy = 1;
        return busy * 100.0;
    }

    /// <summary>Percentage of physical RAM in use (0 if unavailable).</summary>
    public int MemoryUsedPercent()
    {
        var status = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
        return GlobalMemoryStatusEx(ref status) ? (int)status.dwMemoryLoad : 0;
    }

    /// <summary>Current local hour (0-23), for late-night lines.</summary>
    public static int CurrentHour => DateTime.Now.Hour;
}
