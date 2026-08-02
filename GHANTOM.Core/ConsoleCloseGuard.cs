using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GHANTOM.Core;

/// <summary>
/// Restart-on-close: hooks the console control handler so that when the
/// window is closed the process relaunches itself, passing an incremented
/// "close count" so the bot can escalate its reaction each time.
/// </summary>
public static class ConsoleCloseGuard
{
    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

    private delegate bool ConsoleCtrlDelegate(int sig);

    // Held in a static field so the delegate isn't garbage-collected while
    // the native side still holds the callback pointer.
    private static ConsoleCtrlDelegate _handler;
    private static int _closeCount;
    private static string _persistentFlags = "";

    /// <summary>
    /// Enable restart-on-close. <paramref name="closeCount"/> is the count
    /// this instance was launched with; on close it relaunches with count+1.
    /// <paramref name="persistentFlags"/> (e.g. "--troll") are forwarded to the
    /// relaunched instance so it stays in the same mode.
    /// </summary>
    public static void Enable(int closeCount, string persistentFlags = "")
    {
        _closeCount = closeCount;
        _persistentFlags = persistentFlags ?? "";
        _handler = Handler;
        SetConsoleCtrlHandler(_handler, true);
    }

    private static bool Handler(int sig)
    {
        string exe = Process.GetCurrentProcess().MainModule.FileName;
        Process.Start(new ProcessStartInfo
        {
            FileName = exe,
            Arguments = (_persistentFlags + " " + (_closeCount + 1)).Trim(),
            UseShellExecute = true
        });

        Thread.Sleep(500);
        return false; // allow this instance to close; the relaunch takes over
    }
}
