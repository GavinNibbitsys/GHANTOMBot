using System.Threading;

namespace GHANTOM.Core;

/// <summary>
/// Shared console output helpers: solid-color lines and the signature
/// character-by-character "typing" effect used by every bot.
/// </summary>
public static class ConsolePrinter
{
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
