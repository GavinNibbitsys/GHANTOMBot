using System.Runtime.InteropServices;
using System.Threading;

namespace GHANTOM.Core;

/// <summary>
/// Shared console output helpers: solid-color lines and the signature
/// character-by-character "typing" effect used by every bot.
/// </summary>
public static class ConsolePrinter
{
    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    /// <summary>Classic conhost doesn't interpret raw ANSI/SGR escape codes
    /// until this mode is turned on; without it the banner's color codes
    /// would print as literal garbage text instead of a colored image.</summary>
    public static void EnableAnsi()
    {
        try
        {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (GetConsoleMode(handle, out uint mode))
                SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }
        catch { }
    }

    /// <summary>Console.Clear() throws if output is redirected (e.g. piped to a
    /// file/log); swallow that instead of crashing the whole prank over it.</summary>
    public static void ClearSafe()
    {
        try { Console.Clear(); } catch { }
    }

    public static void Color(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void Slow(string text, ConsoleColor color, int delay = 30)
    {
        Console.ForegroundColor = color;
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.WriteLine();
        Console.ResetColor();
    }
}
