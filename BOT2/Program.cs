using System.Linq;
using System.Threading;
using GHANTOM.Core;
using static GHANTOM.Core.ConsolePrinter;

namespace BOT2;

// NOTE: placeholder second bot. Identity (name/color/personality) and the
// scripted cross-talk with GHANTOM are built out in Phase 4. For now it shares
// GHANTOM.Core, honors the same --troll/--untroll flags, and holds a window open.
internal static class Program
{
    private const string AppName = "BOT2";
    private const ConsoleColor Ink = ConsoleColor.Cyan;

    private static void Main(string[] args)
    {
        if (args.Contains("--untroll"))
        {
            StartupManager.Disable(AppName);
            Color("BOT2 autostart removed.", Ink);
            return;
        }

        bool trollMode = args.Contains("--troll");
        int closeCount = args.Select(a => int.TryParse(a, out int n) ? n : -1).FirstOrDefault(n => n >= 0);
        if (closeCount < 0) closeCount = 0;

        if (trollMode)
        {
            StartupManager.Enable(AppName);
            ConsoleCloseGuard.Enable(closeCount, "--troll");
        }

        Console.Title = "BOT2 (placeholder)";
        Console.Clear();
        Color("[BOT2] online. (placeholder — cross-talk with GHANTOM lands in Phase 4)", Ink);

        // Idle heartbeat so the window stays open like a real bot session.
        while (true)
            Thread.Sleep(1000);
    }
}
