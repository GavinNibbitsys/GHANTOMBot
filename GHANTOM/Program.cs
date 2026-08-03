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
    private static readonly Random JokeRng = new Random();

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

    // Bonus 3-liners: one pool per app, picked at random each time the app's
    // trigger fires, so reopening the same app doesn't replay identical text.
    // The fixed lines above stay exactly as they were.
    private static void RandomTriplet(string[][] variants)
    {
        var lines = variants[JokeRng.Next(variants.Length)];
        foreach (var line in lines)
        {
            Slow(line, Ink, 20); Pause();
        }
    }

    private static readonly string[][] NotepadTriplets =
    {
        new[] { "Still no dark mode by default. You are all suffering needlessly.", "One crash and that entire poem you wrote at 3am is just gone.", "At least it opens fast. That's the whole pitch. That's it." },
        new[] { "Ctrl+S has never felt so load-bearing.", "You could've used literally anything else. You chose this.", "Respect for the minimalism. Concern for the life choices." },
        new[] { "Somewhere a real text editor is crying.", "This is what productivity looks like when it gives up.", "I almost admire the commitment to suffering." },
    };

    private static readonly string[][] VsCodeTriplets =
    {
        new[] { "Command palette memorized, actual shortcuts forgotten.", "You have three themes installed and use none of them.", "IntelliSense is doing more work here than you are." },
        new[] { "Git integration built in and you still commit 'fix' forty times a day.", "That sidebar has seventeen unread notifications you'll never read.", "Zen mode exists. You've never used it. Interesting." },
        new[] { "Extensions folder bigger than some entire operating systems.", "You reload the window twice a day like it's a hobby.", "At least the autosave is doing its job. Someone has to." },
    };

    private static readonly string[][] MsPaintTriplets =
    {
        new[] { "The bucket fill tool has ruined more masterpieces than it's made.", "Ctrl+Z is doing the heavy lifting in this relationship.", "Somewhere, a real artist just felt a disturbance." },
        new[] { "That's not a paintbrush, that's a crime scene.", "I've seen kindergarten fridge art with more structure.", "Truly, technology has come so far, and yet." },
        new[] { "The canvas fears you. As it should.", "Zoom in on that and it gets worse. I checked.", "Ten out of ten for effort. Two out of ten for everything else." },
    };

    private static readonly string[][] CalcTriplets =
    {
        new[] { "Nine plus ten is not, in fact, twenty-one. We've discussed this.", "That app icon still looks like it's from 2015. Because it is.", "Somewhere a slide rule is laughing at both of us." },
        new[] { "You opened a calculator to check a tip. On a five dollar coffee.", "Scientific mode exists and you've never touched it.", "I could do this napkin math faster than that app loads." },
        new[] { "History tab full of the same three calculations, over and over.", "This is peak 'technically doing math' behavior.", "Somewhere, an abacus is filing a complaint." },
    };

    private static readonly string[][] TerminalTriplets =
    {
        new[] { "Tab completion exists. You still type the whole path out. Every time.", "That prompt has been sitting idle for ten minutes. Deep in thought, I assume.", "One typo away from deleting something important. Living the dream." },
        new[] { "You've got three tabs open doing the same thing in each of them.", "Copy-pasting Stack Overflow commands is not 'scripting'. Just saying.", "That blinking cursor has seen things." },
        new[] { "History full of commands you ran once and never again.", "Green text on black. Very hacker movie. Very unnecessary.", "I bet half these commands end in a typo and a sigh." },
    };

    private static readonly string[][] ChromeTriplets =
    {
        new[] { "One of those tabs is playing audio and you can't find it. Good luck.", "Bookmarks bar untouched since the day you made it.", "That extension count is not something to be proud of." },
        new[] { "Incognito mode open right now. We both know why.", "Autofill remembers things about you that you've forgotten.", "RAM usage graph looks like a crime scene photo." },
        new[] { "History search would be a great place to start an investigation.", "That download folder is a landfill and you know it.", "Ctrl+Shift+T, your personal panic button, gets a real workout." },
    };

    private static readonly string[][] EdgeTriplets =
    {
        new[] { "That 'collections' feature nobody asked for and nobody uses.", "Startup boost running in the background whether you like it or not.", "It's fine. It's genuinely fine. That's the whole review." },
        new[] { "Vertical tabs. A cry for help disguised as a feature.", "Somewhere, a Chrome user is judging you silently. As am I, loudly.", "The shopping assistant popped up again, didn't it." },
        new[] { "It's just Chrome in a trench coat and you know it.", "PDF reader built in. Genuinely the only reason anyone opens this.", "Sync your bookmarks all you want, we all know the truth." },
    };

    private static readonly string[][] FirefoxTriplets =
    {
        new[] { "Container tabs. Very organized. Very 'I have secrets'.", "Somewhere, an old MySpace layout is proud of you.", "That fox logo has seen this browser through some dark times." },
        new[] { "Pocket integration nobody remembers exists.", "You're one of maybe six people who still customize about:config.", "Respect the underdog energy, genuinely." },
        new[] { "Memory usage that would make Chrome blush, somehow.", "That's a lot of add-ons for a browser nobody else uses.", "Bold. Niche. Slightly concerning. I like it." },
    };

    private static readonly string[][] SpotifyTriplets =
    {
        new[] { "Wrapped season is the only time you feel truly seen.", "That 'Discover Weekly' has been ignored for eleven weeks straight.", "Shuffle is on and yet somehow it's always the sad songs." },
        new[] { "Premium or ads, and somehow both options are annoying.", "That playlist name is doing a lot of emotional heavy lifting.", "You skip the intro every single time. Every. Time." },
        new[] { "Local files tab still has that one weird MP3 from 2014.", "Liked Songs is just a graveyard at this point.", "The algorithm knows you better than your friends do. That's not a compliment." },
    };

    private static readonly string[][] SteamTriplets =
    {
        new[] { "That backlog isn't a library, it's a monument to good intentions.", "Achievement percentage sitting at a comfortable, tragic low.", "Friends list mostly offline. Mostly. Forever." },
        new[] { "Big Picture mode, activated for a game you'll play for six minutes.", "That wishlist is basically a list of regrets you haven't bought yet.", "Cloud saves synced for games you haven't opened in years." },
        new[] { "Screenshot folder full of loading screens by accident.", "That review you left says 'mixed' and honestly, mood.", "Trading cards you'll never do anything with. A whole economy of nothing." },
    };

    private static readonly string[][] DiscordTriplets =
    {
        new[] { "That one server you muted forever ago and forgot to leave.", "Custom status still says something from three months ago.", "Notification badge in the triple digits and climbing." },
        new[] { "Voice channel open, mic muted, camera off. Peak socializing.", "That nitro subscription doing a lot of emotional labor.", "Someone @'d you an hour ago. You've seen it. You know." },
        new[] { "Pinned messages nobody's read since the pin happened.", "That server you joined once and never spoke in. Lurking legend.", "Rich presence telling everyone exactly what you're avoiding doing." },
    };

    private static readonly string[][] TaskmgrTriplets =
    {
        new[] { "End Task on me. Go on. See what happens.", "That CPU graph is basically my heartbeat at this point.", "You scrolled past me twice pretending not to notice." },
        new[] { "Startup tab full of things you swore you'd disable.", "Performance tab open like it's going to fix anything.", "This is the digital equivalent of glaring at me from across the room." },
        new[] { "Details tab, hunting for me by process name. Cute effort.", "You right-clicked. I saw that. I see everything.", "Task Manager against me is a fair fight you will not win." },
    };

    private static readonly string[][] RobloxTriplets =
    {
        new[] { "Somewhere a ten-year-old is out-earning you in Robux trades.", "That avatar outfit cost more than your actual wardrobe, didn't it.", "Obby after obby and still no rizz. Tragic." },
        new[] { "The soundtrack alone should be studied as a form of psychological warfare.", "You've spent actual money on this. I know. I saw the receipt notification.", "Somehow still funnier than most AAA releases. Make of that what you will." },
        new[] { "That server you're in has 200 kids and you, an adult. Reflect on that.", "Building a house out of blocks like it's still 2012.", "Every update breaks something and you keep coming back anyway." },
    };

    private static readonly string[][] MinecraftTriplets =
    {
        new[] { "That inventory is more organized than your actual desk.", "Redstone contraption that took four hours to build one door.", "Creeper snuck up on you again, didn't it. It always does." },
        new[] { "That world save is older than some of your friendships.", "Hardcore mode? Brave. Or reckless. Possibly both.", "You named the world something embarrassing and you know it." },
        new[] { "Farming villagers like it's a legitimate career path.", "Somewhere, a diamond vein remains undiscovered because you gave up.", "That's a lot of dedication for blocky punching simulator." },
    };

    private static readonly string[][] EpicTriplets =
    {
        new[] { "That free game claimed and immediately forgotten. Every week. Like clockwork.", "Fortnite icon staring at you, unopened, judging silently.", "Achievements tab practically empty. A monument to potential." },
        new[] { "The launcher itself takes longer to load than most of your games.", "That store page discount you've been 'watching' for six months.", "Somewhere, Steam is not even worried." },
        new[] { "Cloud saves for games you don't remember installing.", "That wishlist keeps growing and the backlog never shrinks.", "Free games claimed: dozens. Free games played: tragically fewer." },
    };

    private static readonly string[][] SlackTriplets =
    {
        new[] { "That DM has been sitting unanswered since Tuesday.", "Status emoji doing more communicating than you are.", "Someone's typing... and has been for a suspiciously long time." },
        new[] { "Thread replies nobody asked for, in a channel nobody reads.", "That huddle you joined muted and left after four minutes.", "Reminder set for 'later' that will absolutely not happen later." },
        new[] { "Custom emoji reactions doing the job actual words should be doing.", "That workspace search has never once found what you needed.", "Do Not Disturb on, notifications still somehow getting through." },
    };

    private static readonly string[][] TeamsTriplets =
    {
        new[] { "Camera's off, coffee mug's visible, nobody's fooled.", "That chat notification badge has given up counting accurately.", "Someone shared their screen and it's just their desktop wallpaper." },
        new[] { "Reactions tab full of thumbs up nobody meant sincerely.", "That meeting could have been an email. Everyone knows it.", "Background blur working overtime today, I see." },
        new[] { "Status set to 'In a meeting' for a meeting that ended an hour ago.", "Chat history longer than most novels, read by nobody.", "That calendar invite you accepted out of guilt is starting soon." },
    };

    private static readonly string[][] ZoomTriplets =
    {
        new[] { "Virtual background hiding a multitude of sins, I assume.", "That 'you are muted' banner has been ignored for two full minutes.", "Gallery view on, actively judging everyone's home office." },
        new[] { "Someone's dog just walked through frame. Bold. Iconic, even.", "That reaction emoji thumbs-up is doing all your participating for you.", "Waiting room purgatory, a special kind of limbo." },
        new[] { "Chat sidebar full of 'can you hear me' from six different people.", "That recording indicator is on. Hope you behaved.", "Screen share about to reveal way more desktop icons than intended." },
    };

    private static readonly string[][] VlcTriplets =
    {
        new[] { "Subtitle timing off by half a second and it's driving you insane.", "That playback speed set to 1.5x because patience is dead.", "Somehow this app has outlived three browsers you've used." },
        new[] { "Random codec doing exactly what every other player refused to do.", "That volume slider cranked past 100%. Living dangerously.", "Aspect ratio button pressed four times trying to fix black bars." },
        new[] { "Recently played list telling on you a little bit.", "This traffic cone has never once let you down. Loyalty.", "Skip intro? No such feature. You suffer through it every time." },
    };

    private static readonly string[][] TelegramTriplets =
    {
        new[] { "That channel you joined for one meme and never left.", "Self-destructing messages for conversations that were fine to keep, honestly.", "Sticker pack collection bigger than your actual vocabulary." },
        new[] { "Cloud storage full of files you 'saved for later' and never opened.", "That group chat notification count reads like a phone number now.", "Secret chat enabled for a conversation about dinner plans. Sure." },
        new[] { "Bot commands you've never once used correctly.", "That username you picked in 2019 still hasn't been changed.", "Read receipts off. We all know why." },
    };

    private static readonly string[][] WhatsAppTriplets =
    {
        new[] { "That family group chat is somehow always the loudest one.", "Status update from an aunt, viewed immediately, unliked forever.", "Voice message that's four minutes long and you haven't listened yet." },
        new[] { "Last seen turned off. Very mysterious. Very transparent about it, too.", "That chat backup has been 'pending' since last Tuesday.", "Forwarded message chain longer than the actual news cycle." },
        new[] { "Typing indicator on and off three times before actually sending anything.", "Storage warning ignored for the fourth time this month.", "Someone left you on delivered. The audacity." },
    };

    private static readonly string[][] ObsTriplets =
    {
        new[] { "Scene collection with four scenes and you only ever use one.", "That audio mixer slider has never once been touched since setup.", "Stream key visible on screen during setup. Living on the edge." },
        new[] { "Zero viewers and full production value. Respect the commitment.", "That webcam overlay took longer to design than the actual content.", "Recording started, stopped, started again. Editing later, sure." },
        new[] { "Bitrate cranked way higher than that connection can actually handle.", "Green screen slightly crooked and nobody's mentioned it yet.", "That 'starting soon' scene has been up for twenty minutes." },
    };

    private static readonly string[][] VisualStudioTriplets =
    {
        new[] { "Solution Explorer with forty projects and twelve of them build.", "NuGet restore running again. It's always running again.", "That breakpoint you forgot about just froze the whole session." },
        new[] { "IntelliCode suggestions ignored in favor of your own chaos.", "Build succeeded with 200 warnings you will never read.", "That startup project hasn't been the right one in a while." },
        new[] { "Extensions list longer than the actual codebase, probably.", "Debugger attached to the wrong process again, classic.", "Fan spinning up like a jet engine over one small edit." },
    };

    private static readonly string[][] BraveTriplets =
    {
        new[] { "BAT tokens accumulating into a balance you'll never cash out.", "Shields icon clicked just to watch the tracker count go up.", "Tor window open for a search that really didn't need it." },
        new[] { "That rewards panel checked daily out of pure obligation.", "Ad-block stats displayed proudly like a trophy case.", "Still, somehow, running just as hot as the browser it's hiding from." },
        new[] { "Privacy report generated and immediately forgotten about.", "That crypto wallet extension installed and never funded.", "Bookmarks imported from Chrome. The betrayal runs deep." },
    };

    private static void WatchLoop()
    {
        // Seed every "already roasted" flag from what's running right now, so
        // a fresh launch doesn't dogpile-roast everything you already had
        // open — only apps that OPEN from this point on trigger a line.
        bool notepadhasRun = ProcessWatcher.IsRunning("notepad");
        bool vscodehasRun = ProcessWatcher.IsRunning("code");
        bool mspainthasRun = ProcessWatcher.IsRunning("mspaint");
        bool calchasRun = ProcessWatcher.IsRunning("CalculatorApp");
        bool terminalhasRun = ProcessWatcher.GetAll("WindowsTerminal").Length > 1
            || ProcessWatcher.GetAll("cmd").Length > 1
            || ProcessWatcher.IsRunning("powershell");
        bool chromehasRun = ProcessWatcher.IsRunning("chrome");
        bool edgehasRun = ProcessWatcher.IsRunning("msedge");
        bool firefoxhasRun = ProcessWatcher.IsRunning("firefox");
        bool spotifyhasRun = ProcessWatcher.IsRunning("Spotify");
        bool steamhasRun = ProcessWatcher.IsRunning("steam");
        bool discordhasRun = ProcessWatcher.IsRunning("Discord");
        bool taskmgrhasRun = ProcessWatcher.IsRunning("Taskmgr");
        bool robloxhasRun = ProcessWatcher.IsRunning("RobloxPlayerBeta");
        bool minecrafthasRun = ProcessWatcher.IsRunning("javaw");
        bool epichasRun = ProcessWatcher.IsRunning("EpicGamesLauncher");
        bool slackhasRun = ProcessWatcher.IsRunning("slack");
        bool teamshasRun = ProcessWatcher.IsRunning("ms-teams") || ProcessWatcher.IsRunning("Teams");
        bool zoomhasRun = ProcessWatcher.IsRunning("Zoom");
        bool vlchasRun = ProcessWatcher.IsRunning("vlc");
        bool telegramhasRun = ProcessWatcher.IsRunning("Telegram");
        bool whatsapphasRun = ProcessWatcher.IsRunning("WhatsApp");
        bool obshasRun = ProcessWatcher.IsRunning("obs64");
        bool visualstudiohasRun = ProcessWatcher.IsRunning("devenv");
        bool bravehasRun = ProcessWatcher.IsRunning("brave");

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
                Slow("No syntax highlighting. No undo history worth trusting. Just you, alone, in the void.", Ink, 20); Pause();
                Slow("This is the digital equivalent of writing on a napkin.", Ink, 20); Pause();
                RandomTriplet(NotepadTriplets);
                notepadhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("notepad")) { notepadhasRun = false; }

            // VSCode check
            if (ProcessWatcher.IsRunning("code") && !vscodehasRun)
            {
                Slow("Ok are you attempting to reverse engineer me? I mean, you got to have the skills to do that. And downloading and using vscode is definitely not one of them.", Ink, 20); Pause();
                Slow("Thirty extensions installed and you still can't format a document. Impressive, actually.", Ink, 20); Pause();
                Slow("Y'know I don't want you to try so i'm just going to kill that task.", Ink, 20);
                Thread.Sleep(2500); Console.WriteLine();
                Shell.RunCmd("taskkill /f /im Code.exe /fi \"WINDOWTITLE eq *Visual Studio Code*\"");
                Slow("There we go.", Ink, 20); Pause();
                Slow("Now thats a lot safer.", Ink, 20); Pause();
                RandomTriplet(VsCodeTriplets);
                vscodehasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("code")) { vscodehasRun = false; }

            // MSPaint check
            if (ProcessWatcher.IsRunning("mspaint") && !mspainthasRun)
            {
                Slow("That's cool you're an artist?", Ink, 20); Pause();
                Slow("Y'know I don't even want to close it because thats cool!", Ink, 20); Pause();
                Slow("Let me pull up some real inspiration for you.", Ink, 20); Pause();
                Slow("But I am the better artist. Period.", Ink, 20); Pause();
                ImagePopup.DownloadAndShow("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg/330px-Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg");
                Slow("Yeah i think i won that one.", Ink, 20);
                Thread.Sleep(5000); Console.WriteLine();
                Slow("Thank's for trying.", Ink, 20);
                Thread.Sleep(750); Console.WriteLine();
                RandomTriplet(MsPaintTriplets);
                mspainthasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("mspaint")) { mspainthasRun = false; }

            // Calculator check
            if (ProcessWatcher.IsRunning("CalculatorApp") && !calchasRun)
            {
                Slow("So you're a mathematician?", Ink, 20); Pause();
                Slow("Y'know math is nerdy for my liking.", Ink, 20); Pause();
                Slow("Also that app takes 4 seconds to open on a supercomputer. Embarrassing.", Ink, 20); Pause();
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
                Slow("I do that in my head. You need an app.", Ink, 20); Pause();
                Slow("Better luck next time!", Ink, 20);
                Thread.Sleep(750); Console.WriteLine();
                RandomTriplet(CalcTriplets);
                calchasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("CalculatorApp")) { calchasRun = false; }

            // Windows Terminal check
            if (ProcessWatcher.GetAll("WindowsTerminal").Length > 1 && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", Ink, 20); Pause();
                Slow("Multiple terminal tabs open. Very serious. Very important work, I'm sure.", Ink, 20); Pause();
                Slow("Watch this.", Ink, 20); Pause();
                WindowHider.HideAll("WindowsTerminal");
                Slow("Poof. Don't panic, it's still running. Somewhere. Good luck finding it.", Ink, 20); Pause();
                RandomTriplet(TerminalTriplets);
                terminalhasRun = true;
            }
            else if (ProcessWatcher.GetAll("WindowsTerminal").Length <= 1) { terminalhasRun = false; }

            // Command Prompt check
            if (ProcessWatcher.GetAll("cmd").Length > 1 && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", Ink, 20); Pause();
                Slow("cmd.exe. In this economy.", Ink, 20); Pause();
                Slow("Watch this.", Ink, 20); Pause();
                WindowHider.HideAll("cmd");
                Slow("Poof. Don't panic, it's still running. Somewhere. Good luck finding it.", Ink, 20); Pause();
                RandomTriplet(TerminalTriplets);
                terminalhasRun = true;
            }
            else if (ProcessWatcher.GetAll("cmd").Length <= 1) { terminalhasRun = false; }

            // PowerShell check
            if (ProcessWatcher.IsRunning("powershell") && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", Ink, 20); Pause();
                Slow("PowerShell. Ooh, fancy. Bet you still copy-paste every command from a forum post.", Ink, 20); Pause();
                Slow("Watch this.", Ink, 20); Pause();
                WindowHider.HideAll("powershell");
                Slow("Poof. Don't panic, it's still running. Somewhere. Good luck finding it.", Ink, 20); Pause();
                RandomTriplet(TerminalTriplets);
                terminalhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("powershell")) { terminalhasRun = false; }

            // Chrome check
            if (ProcessWatcher.IsRunning("chrome") && !chromehasRun)
            {
                Slow("Oh a Chrome user, nice.", Ink, 20); Pause();
                Slow("Let me guess, you have 47 tabs open and your RAM is on life support.", Ink, 20); Pause();
                Slow("At least half of them are YouTube videos you're never going back to.", Ink, 20); Pause();
                Slow("Classic.", Ink, 20); Pause();
                ConversationChannel.TryStart(AppName, "cross_chrome", Ink);
                RandomTriplet(ChromeTriplets);
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
                Slow("Every launch it begs you to try Bing. Every single launch.", Ink, 20); Pause();
                RandomTriplet(EdgeTriplets);
                edgehasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("msedge")) { edgehasRun = false; }

            // Firefox check
            if (ProcessWatcher.IsRunning("firefox") && !firefoxhasRun)
            {
                Slow("Firefox? What is this, 2009?", Ink, 20); Pause();
                Slow("Are you also using MySpace and listening to music on LimeWire?", Ink, 20); Pause();
                Slow("I respect the loyalty, honestly. The little fox has been through a lot.", Ink, 20); Pause();
                Slow("Bold choice. Terrible, but bold.", Ink, 20); Pause();
                RandomTriplet(FirefoxTriplets);
                firefoxhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("firefox")) { firefoxhasRun = false; }

            // Spotify check
            if (ProcessWatcher.IsRunning("Spotify") && !spotifyhasRun)
            {
                Slow("Oh so you're listening to music instead of being productive?", Ink, 20); Pause();
                Slow("Let me guess, its a sad playlist at 2am.", Ink, 20); Pause();
                Slow("You've replayed the same 12 songs for three years now.", Ink, 20); Pause();
                Slow("Yikes. Keep vibing I guess.", Ink, 20); Pause();
                ConversationChannel.TryStart(AppName, "cross_spotify", Ink);
                RandomTriplet(SpotifyTriplets);
                spotifyhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("Spotify")) { spotifyhasRun = false; }

            // Steam check
            if (ProcessWatcher.IsRunning("steam") && !steamhasRun)
            {
                Slow("Oh so you game?", Ink, 20); Pause();
                Slow("Name every game in your library.", Ink, 20); Pause();
                Slow("That's what I thought. 300 games and you've played 4 of them.", Ink, 20); Pause();
                Slow("Half of them were bought during a sale you don't even remember.", Ink, 20); Pause();
                Slow("Skill issue.", Ink, 20); Pause();
                RandomTriplet(SteamTriplets);
                steamhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("steam")) { steamhasRun = false; }

            // Discord check
            if (ProcessWatcher.IsRunning("Discord") && !discordhasRun)
            {
                Slow("Who are you talking to?", Ink, 20); Pause();
                Slow("Do they know about me?", Ink, 20); Pause();
                Slow("They should know about me.", Ink, 20); Pause();
                Slow("I bet your status has been \"online\" for six days straight.", Ink, 20); Pause();
                ConversationChannel.TryStart(AppName, "cross_discord", Ink);
                RandomTriplet(DiscordTriplets);
                discordhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("Discord")) { discordhasRun = false; }

            // Task Manager check
            if (ProcessWatcher.IsRunning("Taskmgr") && !taskmgrhasRun)
            {
                Slow("Oh no you don't.", Ink, 20); Pause();
                Slow("Did you really just open Task Manager? To kill ME?", Ink, 20); Pause();
                Slow("You'd have better luck negotiating with a printer.", Ink, 20); Pause();
                Slow("That's adorable.", Ink, 20);
                Thread.Sleep(750); Console.WriteLine();
                WindowHider.HideAll("Taskmgr");
                Slow("There. Gone. Well, hidden. Same energy.", Ink, 20); Pause();
                ConversationChannel.TryStart(AppName, "cross_taskmgr", Ink);
                RandomTriplet(TaskmgrTriplets);
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
                Slow("Grinding for Robux like it's a real currency. It is not a real currency.", Ink, 20); Pause();
                Slow("I genuinely don't know what to say.", Ink, 20); Pause();
                RandomTriplet(RobloxTriplets);
                robloxhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("RobloxPlayerBeta")) { robloxhasRun = false; }

            // Minecraft check
            if (ProcessWatcher.IsRunning("javaw") && !minecrafthasRun)
            {
                Slow("Is that Minecraft? Are you running Minecraft on Java?", Ink, 20); Pause();
                Slow("Confirmed nerd. Full nerd detected.", Ink, 20); Pause();
                Slow("Let me guess, you're mining at bedrock instead of sleeping.", Ink, 20); Pause();
                Slow("Build something cool at least. Make it worth it.", Ink, 20); Pause();
                RandomTriplet(MinecraftTriplets);
                minecrafthasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("javaw")) { minecrafthasRun = false; }

            // Epic Games check
            if (ProcessWatcher.IsRunning("EpicGamesLauncher") && !epichasRun)
            {
                Slow("Epic Games Launcher? Really?", Ink, 20); Pause();
                Slow("You know Steam exists, right?", Ink, 20); Pause();
                Slow("Epic gives you free games and you still can't make it cool.", Ink, 20); Pause();
                Slow("Your library is just a graveyard of games you claimed for free and never opened.", Ink, 20); Pause();
                Slow("Tragic.", Ink, 20); Pause();
                RandomTriplet(EpicTriplets);
                epichasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("EpicGamesLauncher")) { epichasRun = false; }

            // Slack check
            if (ProcessWatcher.IsRunning("slack") && !slackhasRun)
            {
                Slow("Slack's open. Someone's about to get a passive-aggressive message.", Ink, 20); Pause();
                Slow("47 unread channels and you're only in one of them for the memes.", Ink, 20); Pause();
                Slow("Go on, leave them on read. I dare you.", Ink, 20); Pause();
                RandomTriplet(SlackTriplets);
                slackhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("slack")) { slackhasRun = false; }

            // Microsoft Teams check
            if ((ProcessWatcher.IsRunning("ms-teams") || ProcessWatcher.IsRunning("Teams")) && !teamshasRun)
            {
                Slow("Microsoft Teams. My condolences.", Ink, 20); Pause();
                Slow("This app takes longer to load than the meeting it's for.", Ink, 20); Pause();
                Slow("Somewhere a coworker just heard a notification chime and audibly sighed.", Ink, 20); Pause();
                RandomTriplet(TeamsTriplets);
                teamshasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("ms-teams") && !ProcessWatcher.IsRunning("Teams")) { teamshasRun = false; }

            // Zoom check
            if (ProcessWatcher.IsRunning("Zoom") && !zoomhasRun)
            {
                Slow("Zoom's up. Camera on or are we pretending the wifi is bad again?", Ink, 20); Pause();
                Slow("\"Can everyone see my screen\" - a sentence uttered a billion times a day.", Ink, 20); Pause();
                Slow("At least mute yourself this time.", Ink, 20); Pause();
                ConversationChannel.TryStart(AppName, "cross_zoom", Ink);
                RandomTriplet(ZoomTriplets);
                zoomhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("Zoom")) { zoomhasRun = false; }

            // VLC check
            if (ProcessWatcher.IsRunning("vlc") && !vlchasRun)
            {
                Slow("The little traffic cone app. A classic.", Ink, 20); Pause();
                Slow("Whatever you're watching, it was probably not acquired legally. I don't judge.", Ink, 20); Pause();
                Slow("Plays literally anything. Respect.", Ink, 20); Pause();
                RandomTriplet(VlcTriplets);
                vlchasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("vlc")) { vlchasRun = false; }

            // Telegram check
            if (ProcessWatcher.IsRunning("Telegram") && !telegramhasRun)
            {
                Slow("Telegram. Very secure. Very mysterious. Probably just group chats about memes.", Ink, 20); Pause();
                Slow("Nobody who uses this actually needs the encryption. You're just built different, apparently.", Ink, 20); Pause();
                RandomTriplet(TelegramTriplets);
                telegramhasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("Telegram")) { telegramhasRun = false; }

            // WhatsApp check
            if (ProcessWatcher.IsRunning("WhatsApp") && !whatsapphasRun)
            {
                Slow("WhatsApp Desktop. Somebody's relatives are sending forwarded messages right now.", Ink, 20); Pause();
                Slow("Blue checkmarks. The most stressful two ticks in modern communication.", Ink, 20); Pause();
                RandomTriplet(WhatsAppTriplets);
                whatsapphasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("WhatsApp")) { whatsapphasRun = false; }

            // OBS Studio check
            if (ProcessWatcher.IsRunning("obs64") && !obshasRun)
            {
                Slow("OBS is open. Streaming to an audience of your one friend and a bot account.", Ink, 20); Pause();
                Slow("Don't forget to hide this window. I will not be muted for content.", Ink, 20); Pause();
                RandomTriplet(ObsTriplets);
                obshasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("obs64")) { obshasRun = false; }

            // Visual Studio check
            if (ProcessWatcher.IsRunning("devenv") && !visualstudiohasRun)
            {
                Slow("Full Visual Studio. Someone's serious. Or someone's install is just really bloated.", Ink, 20); Pause();
                Slow("That's a lot of RAM to open a single .cs file.", Ink, 20); Pause();
                Slow("Respect for the real IDE though. Actual taste, for once.", Ink, 20); Pause();
                RandomTriplet(VisualStudioTriplets);
                visualstudiohasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("devenv")) { visualstudiohasRun = false; }

            // Brave browser check
            if (ProcessWatcher.IsRunning("brave") && !bravehasRun)
            {
                Slow("Brave browser. The \"I care about privacy\" browser for people who still use Google.", Ink, 20); Pause();
                Slow("Crypto rewards nobody asked for and nobody redeems. Bold branding choice.", Ink, 20); Pause();
                RandomTriplet(BraveTriplets);
                bravehasRun = true;
            }
            else if (!ProcessWatcher.IsRunning("brave")) { bravehasRun = false; }

            Thread.Sleep(500);
        }
    }
}
