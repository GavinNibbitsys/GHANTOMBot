using System.Linq;
using System.Threading;
using GHANTOM.Core;
using GHANTOM.Core.Journal;
using static GHANTOM.Core.ConsolePrinter;

namespace REPPLIF;

// REPPLIF: GHANTOM's second window. Gentle, friendly, and a little doubtful
// about the whole "roast the user" plan — plays its half of the scripted
// cross-talk in GHANTOM.Core's conversation script and does its own soft
// idle check-ins. No random dialogue; every line is hand-authored.
internal static class Program
{
    private const string AppName = "REPPLIF";
    private const ConsoleColor Ink = ConsoleColor.Blue;

    private static void Main(string[] args)
    {
        // --untroll: clean this machine and exit.
        if (args.Contains("--untroll"))
        {
            StartupManager.Disable(AppName);
            Color("REPPLIF autostart removed. Take care of yourself, okay?", Ink);
            return;
        }

        bool trollMode = args.Contains("--troll");
        var save = SaveFile.Load(AppName);
        int closeCount = save.CloseCount;
        SaveFile.Save(AppName, save);

        if (trollMode)
        {
            StartupManager.Enable(AppName);
            ConsoleCloseGuard.Enable(AppName, "--troll");
        }

        Console.Title = "REPPLIF";
        EnableAnsi();
        ClearSafe();
        Banner();
        Intro(closeCount);
        WatchLoop();
    }

    private static void Banner()
    {
        Console.WriteLine(@"
[0;97;40m▄▄[0;96;40m▄[0;97;40m▄▄[0;96;40m▄▄[0;36;40m▄[0;96;40m▄[0;37;40m   [0;97;40m▄▄▄[0;96;40m▄[0;97;40m▄[0;96;40m▄▄[0;36;40m▄[0;96;40m▄[0;36;40m▄▄[0;37;40m [0;97;40m▄▄[0;96;40m▄[0;97;40m▄▄[0;96;40m▄▄[0;36;40m▄[0;96;40m▄[0;37;40m   [0;97;40m▄▄[0;96;40m▄[0;97;40m▄▄[0;96;40m▄▄[0;36;40m▄[0;96;40m▄[0;37;40m   [0;97;40m▄▄▄[0;96;40m▄[0;97;40m▄[0;37;40m       [0;97;40m▄▄▄[0;96;40m▄[0;97;40m▄[0;37;40m [0;97;40m▄▄▄[0;96;40m▄[0;97;40m▄[0;96;40m▄▄[0;36;40m▄[0;96;40m▄[0;36;40m▄▄[0m
[0;97;40m█[0;90;40m▒▓████▓▒[0;36;40m▀▄[0;37;40m [0;97;40m█[0;90;40m▒▓█████▓▒[0;90;46m░[0;37;40m [0;97;40m█[0;90;40m▒▓████▓▒[0;36;40m▀▄[0;37;40m [0;97;40m█[0;90;40m▒▓████▓▒[0;36;40m▀▄[0;37;40m [0;97;40m█[0;90;40m▒▓█[0;36;40m█[0;37;40m       [0;97;40m█[0;37;40m [0;90;40m▒▓[0;36;40m█[0;37;40m [0;97;40m█[0;90;40m▒▓█████▓▒[0;90;46m░[0m
[0;96;40m█[0;90;40m░▒░[0;36;40m█▀▀▄[0;90;40m▒░[0;90;46m░[0;37;40m [0;97;40m█[0;90;40m░▒░[0;96;46m▄▄[0;36;40m█[0;96;46m▄[0;36;40m▀▀▀[0;37;40m [0;96;40m█[0;90;40m░▒░[0;36;40m█▀▀▄[0;90;40m▒░[0;90;46m░[0;37;40m [0;96;40m█[0;90;40m░▒░[0;36;40m█▀▀▄[0;90;40m▒░[0;90;46m░[0;37;40m [0;97;40m█[0;90;40m░▒░[0;36;40m█[0;37;40m       [0;96;40m█[0;90;40m░▒░[0;36;40m█[0;37;40m [0;97;40m█[0;90;40m░▒░[0;96;46m▄▄[0;36;40m█[0;96;46m▄[0;36;40m▀▀▀[0m
[0;96;40m█[0;90;40m░▒░[0;96;40m▀▀[0;36;40m▀[0;90;40m░░[0;36;40m▄▀[0;37;40m [0;96;40m█[0;90;40m░▒▒▒▒░[0;90;46m░[0;37;40m    [0;96;40m█[0;90;40m░▒░[0;96;40m▀▀[0;36;40m▀[0;90;40m░░[0;36;40m▄▀[0;37;40m [0;96;40m█[0;90;40m░▒░[0;96;40m▀▀[0;36;40m▀[0;90;40m░░[0;36;40m▄▀[0;37;40m [0;96;40m█[0;90;40m░▒▒[0;36;40m█[0;37;40m       [0;96;40m█[0;90;40m░▒░[0;36;46m█[0;37;40m [0;96;40m█[0;90;40m░▒▒▒▒░[0;90;46m░[0;37;40m   [0m
[0;96;46m▄[0;37;40m [0;90;40m░[0;37;40m [0;36;40m█▀▄[0;37;40m [0;90;40m░[0;37;40m [0;90;46m░[0;37;40m [0;96;40m█[0;37;40m [0;90;40m░[0;37;40m [0;36;40m█[0;96;46m▄▄[0;36;40m█[0;96;40m▄[0;36;40m▄▄[0;37;40m [0;96;46m▄[0;37;40m [0;90;40m░[0;37;40m [0;36;40m█▀▀▀▀[0;37;40m   [0;96;46m▄[0;37;40m [0;90;40m░[0;37;40m [0;36;40m█▀▀▀▀[0;37;40m   [0;96;40m█[0;37;40m [0;90;40m░[0;37;40m [0;36;40m█[0;96;40m▄▄[0;36;40m▄[0;96;40m▄[0;36;40m▄▄[0;37;40m [0;96;46m▄[0;37;40m [0;90;40m░[0;37;40m [0;36;40m█[0;37;40m [0;96;40m█[0;37;40m [0;90;40m░[0;37;40m [0;36;40m█▀▀▀[0;37;40m   [0m
[0;36;40m█■[0;96;40m·[0;90;40m-[0;90;46m░[0;37;40m [0;97;40m▄[0;96;40m▀[0;37;40m  [0;90;46m▒[0;37;40m [0;36;40m█■[0;96;40m·[0;90;40m-[0;37;40m      [0;90;46m▒[0;37;40m [0;36;40m█■[0;96;40m·[0;90;40m-[0;90;46m░[0;37;40m       [0;36;40m█■[0;96;40m·[0;90;40m-[0;90;46m░[0;37;40m       [0;36;40m█■[0;96;40m·[0;90;40m-[0;37;40m      [0;90;46m▒[0;37;40m [0;36;40m█■[0;96;40m·[0;90;40m-[0;90;46m░[0;37;40m [0;36;40m█■[0;96;40m·[0;90;40m-[0;36;40m█[0;37;40m      [0m
[0;36;40m▀▀▀▀▀[0;37;40m  [0;96;40m▀▄▄[0;36;40m▀[0;37;40m [0;96;40m▀[0;36;40m▀▀▀▀▀▀▀▀▀▀[0;37;40m [0;36;40m▀▀▀▀▀[0;37;40m       [0;36;40m▀▀▀▀▀[0;37;40m       [0;96;40m▀[0;36;40m▀▀▀▀▀▀▀▀▀▀[0;37;40m [0;36;40m▀▀▀▀▀[0;37;40m [0;96;40m▀[0;36;40m▀▀▀▀[0;37;40m      [0m
        ");
        Console.ResetColor();
    }

    private static void Pause() { Thread.Sleep(500); Console.WriteLine(); }

    private static void TryStartCapstoneExchange()
    {
        var manifest = JournalManifest.Load();
        if (!manifest.CapstoneDiscovered || manifest.CapstoneExchangeStarted) return;
        if (!ConversationChannel.IsIdle()) return;

        if (ConversationChannel.TryStart(AppName, "capstone_journals_found", Ink))
        {
            manifest.CapstoneExchangeStarted = true;
            JournalManifest.Save(manifest);
        }
    }

    private static void Intro(int closeCount)
    {
        Slow("Hi! I'm REPPLIF.", Ink, 20); Pause();
        Slow("GHANTOM said I should introduce myself. Honestly, I'm a little doubtful about all this.", Ink, 20); Pause();
        Slow("But I promise I'm the friendly one. Mostly here to keep an eye on things. Gently.", Ink, 20); Pause();

        if (closeCount >= 1)
        {
            string times = closeCount == 1 ? "once" : closeCount + " times";
            Slow("Oh - you've tried closing GHANTOM " + times + "? I don't think that's going to work, sorry.", Ink, 20); Pause();
        }
    }

    private static void WatchLoop()
    {
        const double idleThresholdSeconds = 300; // longer than GHANTOM's jab; reads as a gentle follow-up
        bool idleCommented = false;

        while (true)
        {
            // Play our half of any active scripted exchange, and offer up
            // the next ambient bit on a deterministic cycle when nothing's
            // going on.
            ConversationChannel.Tick(AppName, Ink);
            ConversationChannel.TryStartNextAmbient(AppName, Ink);

            // Capstone: GHANTOM watches for the last journal file being
            // opened and flags it in the shared manifest; REPPLIF speaks the
            // opening line of that exchange, so REPPLIF is the one who claims it.
            TryStartCapstoneExchange();

            double idleSec = IdleWatcher.GetIdleTime().TotalSeconds;
            if (idleSec >= idleThresholdSeconds && !idleCommented)
            {
                idleCommented = true;
                Slow("Hey, you've been away a while. I hope that's a good thing and not a GHANTOM thing.", Ink, 20); Pause();
            }
            else if (idleSec < 5)
            {
                idleCommented = false;
            }

            Thread.Sleep(500);
        }
    }
}
