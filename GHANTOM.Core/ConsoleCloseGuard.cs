using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GHANTOM.Core;

/// <summary>
/// Restart-on-close: hooks the console control handler so that when the
/// window is closed the process relaunches itself. The close count is read
/// from and written to the shared <see cref="SaveFile"/>, so escalation
/// survives reboots and manual restarts, not just the parent/child relaunch
/// chain.
/// </summary>
public static class ConsoleCloseGuard
{
    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

    private delegate bool ConsoleCtrlDelegate(int sig);

    // Held in a static field so the delegate isn't garbage-collected while
    // the native side still holds the callback pointer.
    private static ConsoleCtrlDelegate _handler;
    private static string _appName;
    private static string _persistentFlags = "";

    /// <summary>
    /// Enable restart-on-close for <paramref name="appName"/>.
    /// <paramref name="persistentFlags"/> (e.g. "--troll") are forwarded to
    /// the relaunched instance so it stays in the same mode.
    /// </summary>
    public static void Enable(string appName, string persistentFlags = "")
    {
        _appName = appName;
        _persistentFlags = persistentFlags ?? "";
        _handler = Handler;
        SetConsoleCtrlHandler(_handler, true);
    }

    private static bool Handler(int sig)
    {
        var data = SaveFile.Load(_appName);
        data.CloseCount++;
        SaveFile.Save(_appName, data);

        string exe = Process.GetCurrentProcess().MainModule.FileName;
        Process.Start(new ProcessStartInfo
        {
            FileName = exe,
            Arguments = _persistentFlags,
            UseShellExecute = true
        });

        Thread.Sleep(500);
        return false; // allow this instance to close; the relaunch takes over
    }
}
