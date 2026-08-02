using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace GHANTOM.Core;

/// <summary>
/// Thin wrapper over <see cref="Process"/> lookups by name. Used by the
/// poll loop to detect which apps the victim currently has open.
/// </summary>
public static class ProcessWatcher
{
    public static bool IsRunning(string processName)
    {
        return Process.GetProcessesByName(processName).Any();
    }

    public static Process[] GetAll(string processName)
    {
        return Process.GetProcessesByName(processName);
    }

    public static Process WaitForStart(string processName, int pollMs = 500)
    {
        while (true)
        {
            var matches = Process.GetProcessesByName(processName);
            if (matches.Any())
                return matches.First();
            Thread.Sleep(pollMs);
        }
    }

    public static void WaitForExit(string processName, int pollMs = 500)
    {
        while (Process.GetProcessesByName(processName).Any())
            Thread.Sleep(pollMs);
    }
}
