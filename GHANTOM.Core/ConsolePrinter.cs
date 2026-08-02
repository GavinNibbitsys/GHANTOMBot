using System.Threading;

namespace GHANTOM.Core;

/// <summary>
/// Shared console output helpers: solid-color lines and the signature
/// character-by-character "typing" effect used by every bot.
/// </summary>
public static class ConsolePrinter
{
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
