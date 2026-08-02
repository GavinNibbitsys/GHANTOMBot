using System.Diagnostics;

namespace GHANTOM.Core;

/// <summary>Fire-and-wait helper for shell commands (taskkill, etc.).</summary>
public static class Shell
{
    public static void RunCmd(string cmd)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c " + cmd,
            UseShellExecute = false
        }).WaitForExit();
    }
}
