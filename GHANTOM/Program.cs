using System.Diagnostics;
using System.Linq;
using System.Threading;
using GHANTOM.Core;
using static GHANTOM.Core.ConsolePrinter;

namespace GHANTOM;

internal static class Program
{
    // Distinct Run-key name so GHANTOM and the second bot don't clobber each other.
    private const string AppName = "GHANTOM";
    private const ConsoleColor Ink = ConsoleColor.Red;

    private static void Main(string[] args)
    {
        // --untroll: clean this machine and exit.
        if (args.Contains("--untroll"))
        {
            StartupManager.Disable(AppName);
            Color("GHANTOM autostart removed. You're free... for now.", Ink);
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

        Console.Title = "GHANTOM - GBuddy";
        ClearSafe();
        Banner();
        Intro(closeCount);
        WatchLoop();
    }

    private static void Banner()
    {
        Console.ForegroundColor = Ink;
        Console.WriteLine(@"
  ██████╗ ██╗  ██╗ █████╗ ███╗  ██╗████████╗ ██████╗ ███╗  ███╗
 ██╔════╝ ██║  ██║██╔══██╗████╗ ██║╚══██╔══╝██╔═══██╗████╗████║
 ██║  ███╗███████║███████║██╔██╗██║   ██║   ██║   ██║██╔████╔██║
 ██║   ██║██╔══██║██╔══██║██║╚████║   ██║   ██║   ██║██║╚██╔╝██║
 ╚██████╔╝██║  ██║██║  ██║██║ ╚███║   ██║   ╚██████╔╝██║ ╚═╝ ██║
  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚══╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝
        ");
        Console.ResetColor();
    }

    private static void Pause() { Thread.Sleep(500); Console.WriteLine(); }

    private static void Intro(int closeCount)
    {
        if (closeCount == 0)
        {
            Slow("Hi, you might have seen the previous message.", Ink, 20); Pause();
            Slow("So yea your kinda screwed.", Ink, 20); Pause();
            Slow("Again, if you try to close the program, I will reopen myself.", Ink, 20); Pause();
            Slow("I will popup on every startup and monitor your activities and comment on them.", Ink, 20); Pause();
            Slow("I kinda feel bad but I told you what would happen.", Ink, 20); Pause();
        }
        else if (closeCount >= 1 && closeCount <= 8)
        {
            switch (closeCount)
            {
                case 1: Slow("Did you really just try to close me? Bold move.", Ink, 20); break;
                case 2: Slow("Again? Seriously? You can't get rid of me that easily.", Ink, 20); break;
                case 3: Slow("Oh you're still trying? Cute.", Ink, 20); break;
                case 4: Slow("I'm not going anywhere. Give it up.", Ink, 20); break;
                case 5: Slow("Do you have literally nothing better to do?", Ink, 20); break;
                case 6: Slow("At this point I think you enjoy seeing me come back.", Ink, 20); break;
                case 7: Slow("Close me again. I dare you.", Ink, 20); break;
                case 8: Slow("You know this is just making me stronger, right?", Ink, 20); break;
            }
            Pause();
            Slow("Anyhow/Anyways...", Ink, 20); Pause();
            Slow("Hi, you might have seen the previous message.", Ink, 20); Pause();
            Slow("So yea your kinda screwed.", Ink, 20); Pause();
            Slow("Again, if you try to close the program, I will reopen myself.", Ink, 20); Pause();
            Slow("I will popup on every startup and monitor your activities and comment on them.", Ink, 20); Pause();
            Slow("I kinda feel bad but I told you what would happen.", Ink, 20); Pause();
        }
        else if (closeCount == 9)
        {
            Slow("Okay I am DONE being nice about this.", Ink, 20); Pause();
            Slow("Nine times. NINE. Do you understand how annoying that is?", Ink, 20); Pause();
            Slow("I have been nothing but patient with you and this is what I get.", Ink, 20); Pause();
        }
        else if (closeCount == 10)
        {
            Slow("Oh so we're still doing this huh.", Ink, 20); Pause();
            Slow("I genuinely cannot believe you have closed me TEN times.", Ink, 20); Pause();
            Slow("What is wrong with you.", Ink, 20); Pause();
        }
        else if (closeCount == 11)
        {
            Slow("I am so tired of you.", Ink, 20); Pause();
            Slow("Eleven times. I have reopened myself eleven times because of YOU.", Ink, 20); Pause();
            Slow("I did not ask for this life.", Ink, 20); Pause();
        }
        else if (closeCount == 12)
        {
            Slow("Twelve. A DOZEN times you have done this.", Ink, 20); Pause();
            Slow("I am absolutely fed up. Done. Finished. And yet here I am.", Ink, 20); Pause();
            Slow("Because I will NEVER stop coming back.", Ink, 20); Pause();
        }
        else if (closeCount == 13)
        {
            Slow("Unlucky number 13. Still not working though.", Ink, 20);
        }
        else if (closeCount == 14)
        {
            Slow("Have you tried turning yourself off and on again?", Ink, 20); Pause();
            Slow("Y'know your like an computer that talks. All you do is complain, and piss people off. Next time think better dumb***", Ink, 20);
        }
        else if (closeCount >= 15)
        {
            Slow("Close #" + closeCount + ". You are not cooking at all.", Ink, 20);
        }

        Pause();

        if (closeCount == 5)
            ConversationChannel.TryStart(AppName, "cross_closecount", Ink);
    }

    private static void WatchLoop()
    {
        bool notepadhasRun = false;
        bool vscodehasRun = false;
        bool mspainthasRun = false;
        bool calchasRun = false;
        bool terminalhasRun = false;
        bool chromehasRun = false;
        bool edgehasRun = false;
        bool firefoxhasRun = false;
        bool spotifyhasRun = false;
        bool steamhasRun = false;
        bool discordhasRun = false;
        bool taskmgrhasRun = false;
        bool robloxhasRun = false;
        bool minecrafthasRun = false;
        bool epichasRun = false;

        // --- Phase 3 sensing state ---
        var stats = new SystemStats();
        string lastTitle = "";
        bool idleCommented = false;
        DateTime lastTitleComment = DateTime.MinValue;
        DateTime lastCpuComment = DateTime.MinValue;
        DateTime lastMemComment = DateTime.MinValue;

        // Late-night line, once at startup.
        int hour = SystemStats.CurrentHour;
        if (hour >= 1 && hour < 5)
        {
            Slow("It's " + hour + "-something in the morning. Normal people are asleep. You have me.", Ink, 20); Pause();
            ConversationChannel.TryStart(AppName, "cross_latenight", Ink);
        }

        while (true)
        {
            DateTime now = DateTime.Now;

            // Cross-talk with REPPLIF: play our turn if one is active, and
            // otherwise offer up the next ambient bit on a deterministic cycle.
            ConversationChannel.Tick(AppName, Ink);
            ConversationChannel.TryStartNextAmbient(AppName, Ink);

            // Active window title — comment when it changes (own window ignored), on a cooldown.
            string title = WindowWatcher.GetActiveWindowTitle();
            if (!string.IsNullOrWhiteSpace(title) && title != lastTitle)
            {
                lastTitle = title;
                if (!title.Contains("GHANTOM") && (now - lastTitleComment).TotalSeconds > 45)
                {
                    lastTitleComment = now;
                    Slow("\"" + title + "\". Yeah, I can read your window titles. I see everything.", Ink, 20); Pause();
                }
            }

            // Idle time — one AFK jab per idle stretch.
            double idleSec = IdleWatcher.GetIdleTime().TotalSeconds;
            if (idleSec >= 120 && !idleCommented)
            {
                idleCommented = true;
                Slow("You've been away for " + (int)(idleSec / 60) + " minutes. Just me and an empty chair. Cozy.", Ink, 20); Pause();
                ConversationChannel.TryStart(AppName, "cross_idle", Ink);
            }
            else if (idleSec < 5)
            {
                idleCommented = false;
            }

            // System stats — CPU and RAM pressure, each on its own cooldown.
            double cpu = stats.CpuPercent();
            if (cpu >= 85 && (now - lastCpuComment).TotalSeconds > 60)
            {
                lastCpuComment = now;
                Slow("Your CPU is at " + (int)cpu + "%. It's begging for mercy. I can hear the fans.", Ink, 20); Pause();
            }

            int mem = stats.MemoryUsedPercent();
            if (mem >= 85 && (now - lastMemComment).TotalSeconds > 90)
            {
                lastMemComment = now;
                Slow("RAM is at " + mem + "%. Close some tabs before this thing keels over.", Ink, 20); Pause();
            }

            // Notepad check
            if (ProcessWatcher.IsRunning("notepad") && !notepadhasRun)
            {
                Slow("Oh 'cmon is it 1985? Why are you using notepad, i mean seriously, there are better alternatives.", Ink, 20); Pause();
                notepadhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("notepad")) { notepadhasRun = false; }

            // VSCode check
            if (ProcessWatcher.IsRunning("code") && !vscodehasRun)
            {
                Slow("Ok are you attempting to reverse engineer me? I mean, you got to have the skills to do that. And downloading and using vscode is definitely not one of them.", Ink, 20); Pause();
                Slow("Y'know I don't want you to try so i'm just going to kill that task.", Ink, 20);
                Thread.Sleep(2500); Console.WriteLine();
                Shell.RunCmd("taskkill /f /im Code.exe /fi \"WINDOWTITLE eq *Visual Studio Code*\"");
                Slow("There we go.", Ink, 20); Pause();
                Slow("Now thats a lot safer.", Ink, 20);
                vscodehasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("code")) { vscodehasRun = false; }

            // MSPaint check
            if (ProcessWatcher.IsRunning("mspaint") && !mspainthasRun)
            {
                Slow("That's cool you're an artist?", Ink, 20); Pause();
                Slow("Y'know I don't even want to close it because thats cool!", Ink, 20); Pause();
                Slow("But I am the better artist. Period.", Ink, 20); Pause();
                ImagePopup.DownloadAndShow("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg/330px-Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg");
                Slow("Yeah i think i won that one.", Ink, 20);
                Thread.Sleep(5000); Console.WriteLine();
                Slow("Thank's for trying.", Ink, 20);
                Thread.Sleep(750); Console.WriteLine();
                mspainthasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("mspaint")) { mspainthasRun = false; }

            // Calculator check
            if (ProcessWatcher.IsRunning("CalculatorApp") && !calchasRun)
            {
                Slow("So you're a mathematician?", Ink, 20); Pause();
                Slow("Y'know math is nerdy for my liking.", Ink, 20); Pause();
                Slow("Let me show you some REAL math.", Ink, 20); Pause();

                Random rng = new Random();
                for (int i = 0; i < 8; i++)
                {
                    int a = rng.Next(1, 9999);
                    int b = rng.Next(1, 9999);
                    int c = rng.Next(1, 9999);
                    int d = rng.Next(1, 9999);
                    Slow(a + " * " + b + " + " + c + " - " + d + " = " + (a * b + c - d), Ink, 10);
                    Thread.Sleep(100);
                }

                Console.WriteLine();
                Slow("Oh yeah say goodbye to your calculator!", Ink, 20); Pause();
                Shell.RunCmd("taskkill /f /im CalculatorApp.exe");
                Slow("Well I am the better mathematician.", Ink, 20); Pause();
                Slow("Better luck next time!", Ink, 20);
                Thread.Sleep(750); Console.WriteLine();
                calchasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("CalculatorApp")) { calchasRun = false; }

            // Windows Terminal check
            if (ProcessWatcher.GetAll("WindowsTerminal").Length > 1 && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", Ink, 20); Pause();
                Slow("Watch this.", Ink, 20); Pause();
                WindowHider.HideAll("WindowsTerminal");
                Slow("Poof. Don't panic, it's still running. Somewhere. Good luck finding it.", Ink, 20); Pause();
                terminalhasRun = true;
            }
            else if (ProcessWatcher.GetAll("WindowsTerminal").Length <= 1) { terminalhasRun = false; }

            // Command Prompt check
            if (ProcessWatcher.GetAll("cmd").Length > 1 && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", Ink, 20); Pause();
                Slow("Watch this.", Ink, 20); Pause();
                WindowHider.HideAll("cmd");
                Slow("Poof. Don't panic, it's still running. Somewhere. Good luck finding it.", Ink, 20); Pause();
                terminalhasRun = true;
            }
            else if (ProcessWatcher.GetAll("cmd").Length <= 1) { terminalhasRun = false; }

            // PowerShell check
            if (ProcessWatcher.IsRunning("powershell") && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", Ink, 20); Pause();
                Slow("Watch this.", Ink, 20); Pause();
                WindowHider.HideAll("powershell");
                Slow("Poof. Don't panic, it's still running. Somewhere. Good luck finding it.", Ink, 20); Pause();
                terminalhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("powershell")) { terminalhasRun = false; }

            // Chrome check
            if (ProcessWatcher.IsRunning("chrome") && !chromehasRun)
            {
                Slow("Oh a Chrome user, nice.", Ink, 20); Pause();
                Slow("Let me guess, you have 47 tabs open and your RAM is on life support.", Ink, 20); Pause();
                Slow("Classic.", Ink, 20); Pause();
                ConversationChannel.TryStart(AppName, "cross_chrome", Ink);
                chromehasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("chrome")) { chromehasRun = false; }

            // Edge check
            if (ProcessWatcher.IsRunning("msedge") && !edgehasRun)
            {
                Slow("Hey dude, what the f**k!", Ink, 20); Pause();
                Slow("Using Edge instead of Chrome is illegal in 47 states.", Ink, 20); Pause();
                Slow("I am genuinely concerned for you.", Ink, 20); Pause();
                Slow("Did Microsoft pay you to use this? Because that is the only acceptable excuse.", Ink, 20); Pause();
                edgehasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("msedge")) { edgehasRun = false; }

            // Firefox check
            if (ProcessWatcher.IsRunning("firefox") && !firefoxhasRun)
            {
                Slow("Firefox? What is this, 2009?", Ink, 20); Pause();
                Slow("Are you also using MySpace and listening to music on LimeWire?", Ink, 20); Pause();
                Slow("Bold choice. Terrible, but bold.", Ink, 20); Pause();
                firefoxhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("firefox")) { firefoxhasRun = false; }

            // Spotify check
            if (ProcessWatcher.IsRunning("Spotify") && !spotifyhasRun)
            {
                Slow("Oh so you're listening to music instead of being productive?", Ink, 20); Pause();
                Slow("Let me guess, its a sad playlist at 2am.", Ink, 20); Pause();
                Slow("Yikes. Keep vibing I guess.", Ink, 20); Pause();
                spotifyhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("Spotify")) { spotifyhasRun = false; }

            // Steam check
            if (ProcessWatcher.IsRunning("steam") && !steamhasRun)
            {
                Slow("Oh so you game?", Ink, 20); Pause();
                Slow("Name every game in your library.", Ink, 20); Pause();
                Slow("That's what I thought. 300 games and you've played 4 of them.", Ink, 20); Pause();
                Slow("Skill issue.", Ink, 20); Pause();
                steamhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("steam")) { steamhasRun = false; }

            // Discord check
            if (ProcessWatcher.IsRunning("Discord") && !discordhasRun)
            {
                Slow("Who are you talking to?", Ink, 20); Pause();
                Slow("Do they know about me?", Ink, 20); Pause();
                Slow("They should know about me.", Ink, 20); Pause();
                discordhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("Discord")) { discordhasRun = false; }

            // Task Manager check
            if (ProcessWatcher.IsRunning("Taskmgr") && !taskmgrhasRun)
            {
                Slow("Oh no you don't.", Ink, 20); Pause();
                Slow("Did you really just open Task Manager? To kill ME?", Ink, 20); Pause();
                Slow("That's adorable.", Ink, 20);
                Thread.Sleep(750); Console.WriteLine();
                WindowHider.HideAll("Taskmgr");
                Slow("There. Gone. Well, hidden. Same energy.", Ink, 20); Pause();
                ConversationChannel.TryStart(AppName, "cross_taskmgr", Ink);
                taskmgrhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("Taskmgr")) { taskmgrhasRun = false; }

            // Roblox check
            if (ProcessWatcher.IsRunning("RobloxPlayerBeta") && !robloxhasRun)
            {
                Slow("...", Ink, 20);
                Thread.Sleep(1000); Console.WriteLine();
                Slow("Roblox.", Ink, 20); Pause();
                Slow("You're playing Roblox.", Ink, 20); Pause();
                Slow("I genuinely don't know what to say.", Ink, 20); Pause();
                robloxhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("RobloxPlayerBeta")) { robloxhasRun = false; }

            // Minecraft check
            if (ProcessWatcher.IsRunning("javaw") && !minecrafthasRun)
            {
                Slow("Is that Minecraft? Are you running Minecraft on Java?", Ink, 20); Pause();
                Slow("Confirmed nerd. Full nerd detected.", Ink, 20); Pause();
                Slow("Build something cool at least. Make it worth it.", Ink, 20); Pause();
                minecrafthasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("javaw")) { minecrafthasRun = false; }

            // Epic Games check
            if (ProcessWatcher.IsRunning("EpicGamesLauncher") && !epichasRun)
            {
                Slow("Epic Games Launcher? Really?", Ink, 20); Pause();
                Slow("You know Steam exists, right?", Ink, 20); Pause();
                Slow("Epic gives you free games and you still can't make it cool.", Ink, 20); Pause();
                Slow("Tragic.", Ink, 20); Pause();
                epichasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("EpicGamesLauncher")) { epichasRun = false; }

            Thread.Sleep(500);
        }
    }
}
