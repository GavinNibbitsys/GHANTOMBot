using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Linq;
using System.Threading;

public static class ProcessChecker
{
    public static bool IsRunning(string processName)
    {
        return Process.GetProcessesByName(processName).Any();
    }

    public static Process[] GetAll(string processName)
    {
        return Process.GetProcessesByName(processName);
    }

    public static Process WaitForStart(string processName, int pollMs = 500)
    {
        while (true)
        {
            var matches = Process.GetProcessesByName(processName);
            if (matches.Any())
                return matches.First();
            Thread.Sleep(pollMs);
        }
    }

    public static void WaitForExit(string processName, int pollMs = 500)
    {
        while (Process.GetProcessesByName(processName).Any())
            Thread.Sleep(pollMs);
    }
}

public static class StartupManager
{
    private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private static readonly string AppName = "MyApp";
    private static readonly string AppPath = "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"";

    public static void Enable()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
        {
            if (key != null) key.SetValue(AppName, AppPath);
        }
    }

    public static void Disable()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
        {
            if (key != null) key.DeleteValue(AppName, false);
        }
    }

    public static bool IsEnabled()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, false))
        {
            return key != null && key.GetValue(AppName) != null;
        }
    }
}

public class Program
{
    [DllImport("kernel32.dll")]
    static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

    delegate bool ConsoleCtrlDelegate(int sig);

    static ConsoleCtrlDelegate _handler;
    static int closeCount = 0;

    static bool Handler(int sig)
    {
        closeCount++;
        string exe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        Process.Start(new ProcessStartInfo()
        {
            FileName = exe,
            Arguments = closeCount.ToString(),
            UseShellExecute = true
        });

        Thread.Sleep(500);
        return false;
    }

    static void RunCmd(string cmd)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = "cmd.exe",
            Arguments = "/c " + cmd,
            UseShellExecute = false
        }).WaitForExit();
    }

    static void Color(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    static void Slow(string text, ConsoleColor color, int delay = 30)
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

    static void ShowImage(string imagePath)
    {
        System.Threading.Thread thread = new System.Threading.Thread(() =>
        {
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();

            pictureBox.Image = System.Drawing.Image.FromFile(imagePath);
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;

            form.Controls.Add(pictureBox);
            form.Width = 800;
            form.Height = 600;
            form.Text = "Hello :)";

            System.Windows.Forms.Application.Run(form);
        });

        thread.SetApartmentState(System.Threading.ApartmentState.STA);
        thread.Start();
    }

    static void DownloadAndShowImage(string url)
    {
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

        string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gbuddy_img.jpg");

        using (System.Net.WebClient client = new System.Net.WebClient())
        {
            client.Headers.Add("User-Agent", "Mozilla/5.0");
            client.DownloadFile(url, tempPath);
        }

        ShowImage(tempPath);
    }

    public static void Main(string[] args)
    {
        //StartupManager.Disable();
        StartupManager.Enable();
        closeCount = args.Length > 0 ? int.Parse(args[0]) : 0;

        _handler = Handler;
        SetConsoleCtrlHandler(_handler, true);

        Console.Title = "GHANTOM - GBuddy";
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(@"
  ██████╗ ██╗  ██╗ █████╗ ███╗  ██╗████████╗ ██████╗ ███╗  ███╗
 ██╔════╝ ██║  ██║██╔══██╗████╗ ██║╚══██╔══╝██╔═══██╗████╗████║
 ██║  ███╗███████║███████║██╔██╗██║   ██║   ██║   ██║██╔████╔██║
 ██║   ██║██╔══██║██╔══██║██║╚████║   ██║   ██║   ██║██║╚██╔╝██║
 ╚██████╔╝██║  ██║██║  ██║██║ ╚███║   ██║   ╚██████╔╝██║ ╚═╝ ██║
  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚══╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝
        ");
        Console.ResetColor();

        if (closeCount == 0)
        {
            Slow("Hi, you might have seen the previous message.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("So yea your kinda screwed.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("Again, if you try to close the program, I will reopen myself.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("I will popup on every startup and monitor your activities and comment on them.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("I kinda feel bad but I told you what would happen.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
        }
        else if (closeCount >= 1 && closeCount <= 8)
        {
            if (closeCount == 1)
                Slow("Did you really just try to close me? Bold move.", ConsoleColor.Red, 20);
            else if (closeCount == 2)
                Slow("Again? Seriously? You can't get rid of me that easily.", ConsoleColor.Red, 20);
            else if (closeCount == 3)
                Slow("Oh you're still trying? Cute.", ConsoleColor.Red, 20);
            else if (closeCount == 4)
                Slow("I'm not going anywhere. Give it up.", ConsoleColor.Red, 20);
            else if (closeCount == 5)
                Slow("Do you have literally nothing better to do?", ConsoleColor.Red, 20);
            else if (closeCount == 6)
                Slow("At this point I think you enjoy seeing me come back.", ConsoleColor.Red, 20);
            else if (closeCount == 7)
                Slow("Close me again. I dare you.", ConsoleColor.Red, 20);
            else if (closeCount == 8)
                Slow("You know this is just making me stronger, right?", ConsoleColor.Red, 20);

            Thread.Sleep(500);
            Console.WriteLine();
            Slow("Anyhow/Anyways...", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("Hi, you might have seen the previous message.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("So yea your kinda screwed.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("Again, if you try to close the program, I will reopen myself.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("I will popup on every startup and monitor your activities and comment on them.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("I kinda feel bad but I told you what would happen.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
        }
        else if (closeCount == 9)
        {
            Slow("Okay I am DONE being nice about this.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("Nine times. NINE. Do you understand how annoying that is?", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("I have been nothing but patient with you and this is what I get.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
        }
        else if (closeCount == 10)
        {
            Slow("Oh so we're still doing this huh.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("I genuinely cannot believe you have closed me TEN times.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("What is wrong with you.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
        }
        else if (closeCount == 11)
        {
            Slow("I am so tired of you.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("Eleven times. I have reopened myself eleven times because of YOU.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("I did not ask for this life.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
        }
        else if (closeCount == 12)
        {
            Slow("Twelve. A DOZEN times you have done this.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("I am absolutely fed up. Done. Finished. And yet here I am.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("Because I will NEVER stop coming back.", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
        }
        else if (closeCount == 13)
        {
            Slow("Unlucky number 13. Still not working though.", ConsoleColor.Red, 20);
        }
        else if (closeCount == 14)
        {
            Slow("Have you tried turning yourself off and on again?", ConsoleColor.Red, 20);
            Thread.Sleep(500);
            Console.WriteLine();
            Slow("Y'know your like an computer that talks. All you do is complain, and piss people off. Next time think better dumb***", ConsoleColor.Red, 20);
        }
        else if (closeCount >= 15)
            Slow("Close #" + closeCount + ". You are not cooking at all.", ConsoleColor.Red, 20);

        Thread.Sleep(500);
        Console.WriteLine();

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

        int ownPid = Process.GetCurrentProcess().Id;

        while (true)
        {
            // Notepad check
            if (ProcessChecker.IsRunning("notepad") && !notepadhasRun)
            {
                Slow("Oh 'cmon is it 1985? Why are you using notepad, i mean seriously, there are better alternatives.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                notepadhasRun = true;
            }
            else if (!ProcessChecker.IsRunning("notepad"))
            {
                notepadhasRun = false;
            }

            // VSCode check
            if (ProcessChecker.IsRunning("code") && !vscodehasRun)
            {
                Slow("Ok are you attempting to reverse engineer me? I mean, you got to have the skills to do that. And downloading and using vscode is definitely not one of them.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Y'know I don't want you to try so i'm just going to kill that task.", ConsoleColor.Red, 20);
                Thread.Sleep(2500);
                Console.WriteLine();
                RunCmd("taskkill /f /im Code.exe /fi \"WINDOWTITLE eq *Visual Studio Code*\"");
                Slow("There we go.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Now thats a lot safer.", ConsoleColor.Red, 20);
                vscodehasRun = true;
            }
            else if (!ProcessChecker.IsRunning("code"))
            {
                vscodehasRun = false;
            }

            // MSPaint check
            if (ProcessChecker.IsRunning("mspaint") && !mspainthasRun)
            {
                Slow("That's cool you're an artist?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Y'know I don't even want to close it because thats cool!", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("But I am the better artist. Period.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                DownloadAndShowImage("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg/330px-Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg");
                Slow("Yeah i think i won that one.", ConsoleColor.Red, 20);
                Thread.Sleep(5000);
                Console.WriteLine();
                Slow("Thank's for trying.", ConsoleColor.Red, 20);
                Thread.Sleep(750);
                Console.WriteLine();
                mspainthasRun = true;
            }
            else if (!ProcessChecker.IsRunning("mspaint"))
            {
                mspainthasRun = false;
            }

            // Calculator check
            if (ProcessChecker.IsRunning("CalculatorApp") && !calchasRun)
            {
                Slow("So you're a mathematician?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Y'know math is nerdy for my liking.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Let me show you some REAL math.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();

                Random rng = new Random();
                for (int i = 0; i < 8; i++)
                {
                    int a = rng.Next(1, 9999);
                    int b = rng.Next(1, 9999);
                    int c = rng.Next(1, 9999);
                    int d = rng.Next(1, 9999);
                    Slow(a + " * " + b + " + " + c + " - " + d + " = " + (a * b + c - d), ConsoleColor.Red, 10);
                    Thread.Sleep(100);
                }

                Console.WriteLine();
                Slow("Oh yeah say goodbye to your calculator!", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                RunCmd("taskkill /f /im CalculatorApp.exe");
                Slow("Well I am the better mathematician.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Better luck next time!", ConsoleColor.Red, 20);
                Thread.Sleep(750);
                Console.WriteLine();
                calchasRun = true;
            }
            else if (!ProcessChecker.IsRunning("CalculatorApp"))
            {
                calchasRun = false;
            }

            // Windows Terminal check
            if (ProcessChecker.GetAll("WindowsTerminal").Length > 1 && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Oh sh*t i might need to kill the terminal.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                RunCmd("taskkill /f /im WindowsTerminal.exe /fi \"PID ne " + ownPid + "\"");
                Slow("Oh sh*t you scared me! Don't do that again!", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                terminalhasRun = true;
            }
            else if (ProcessChecker.GetAll("WindowsTerminal").Length <= 1)
            {
                terminalhasRun = false;
            }

            // Command Prompt check
            if (ProcessChecker.GetAll("cmd").Length > 1 && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Oh sh*t i might need to kill the terminal.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                RunCmd("taskkill /f /im cmd.exe /fi \"PID ne " + ownPid + "\"");
                Slow("Oh sh*t you scared me! Don't do that again!", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                terminalhasRun = true;
            }
            else if (ProcessChecker.GetAll("cmd").Length <= 1)
            {
                terminalhasRun = false;
            }

            // PowerShell check
            if (ProcessChecker.IsRunning("powershell") && !terminalhasRun)
            {
                Slow("So you're a programmer... A PROGRAMER?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Oh sh*t i might need to kill the terminal.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                RunCmd("taskkill /f /im powershell.exe /fi \"PID ne " + ownPid + "\"");
                Slow("Oh sh*t you scared me! Don't do that again!", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                terminalhasRun = true;
            }
            else if (!ProcessChecker.IsRunning("powershell"))
            {
                terminalhasRun = false;
            }

            // Chrome check
            if (ProcessChecker.IsRunning("chrome") && !chromehasRun)
            {
                Slow("Oh a Chrome user, nice.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Let me guess, you have 47 tabs open and your RAM is on life support.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Classic.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                chromehasRun = true;
            }
            else if (!ProcessChecker.IsRunning("chrome"))
            {
                chromehasRun = false;
            }

            // Edge check
            if (ProcessChecker.IsRunning("msedge") && !edgehasRun)
            {
                Slow("Hey dude, what the f**k!", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Using Edge instead of Chrome is illegal in 47 states.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("I am genuinely concerned for you.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Did Microsoft pay you to use this? Because that is the only acceptable excuse.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                edgehasRun = true;
            }
            else if (!ProcessChecker.IsRunning("msedge"))
            {
                edgehasRun = false;
            }

            // Firefox check
            if (ProcessChecker.IsRunning("firefox") && !firefoxhasRun)
            {
                Slow("Firefox? What is this, 2009?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Are you also using MySpace and listening to music on LimeWire?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Bold choice. Terrible, but bold.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                firefoxhasRun = true;
            }
            else if (!ProcessChecker.IsRunning("firefox"))
            {
                firefoxhasRun = false;
            }

            // Spotify check
            if (ProcessChecker.IsRunning("Spotify") && !spotifyhasRun)
            {
                Slow("Oh so you're listening to music instead of being productive?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Let me guess, its a sad playlist at 2am.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Yikes. Keep vibing I guess.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                spotifyhasRun = true;
            }
            else if (!ProcessChecker.IsRunning("Spotify"))
            {
                spotifyhasRun = false;
            }

            // Steam check
            if (ProcessChecker.IsRunning("steam") && !steamhasRun)
            {
                Slow("Oh so you game?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Name every game in your library.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("That's what I thought. 300 games and you've played 4 of them.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Skill issue.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                steamhasRun = true;
            }
            else if (!ProcessChecker.IsRunning("steam"))
            {
                steamhasRun = false;
            }

            // Discord check
            if (ProcessChecker.IsRunning("Discord") && !discordhasRun)
            {
                Slow("Who are you talking to?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Do they know about me?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("They should know about me.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                discordhasRun = true;
            }
            else if (!ProcessChecker.IsRunning("Discord"))
            {
                discordhasRun = false;
            }

            // Task Manager check
            if (ProcessChecker.IsRunning("Taskmgr") && !taskmgrhasRun)
            {
                Slow("Oh no you don't.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Did you really just open Task Manager? To kill ME?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("That's adorable.", ConsoleColor.Red, 20);
                Thread.Sleep(750);
                Console.WriteLine();
                RunCmd("taskkill /f /im Taskmgr.exe");
                Slow("Goodbye Task Manager. You won't be missed.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                taskmgrhasRun = true;
            }
            else if (!ProcessChecker.IsRunning("Taskmgr"))
            {
                taskmgrhasRun = false;
            }

            // Roblox check
            if (ProcessChecker.IsRunning("RobloxPlayerBeta") && !robloxhasRun)
            {
                Slow("...", ConsoleColor.Red, 20);
                Thread.Sleep(1000);
                Console.WriteLine();
                Slow("Roblox.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("You're playing Roblox.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("I genuinely don't know what to say.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                robloxhasRun = true;
            }
            else if (!ProcessChecker.IsRunning("RobloxPlayerBeta"))
            {
                robloxhasRun = false;
            }

            // Minecraft check
            if (ProcessChecker.IsRunning("javaw") && !minecrafthasRun)
            {
                Slow("Is that Minecraft? Are you running Minecraft on Java?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Confirmed nerd. Full nerd detected.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Build something cool at least. Make it worth it.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                minecrafthasRun = true;
            }
            else if (!ProcessChecker.IsRunning("javaw"))
            {
                minecrafthasRun = false;
            }

            // Epic Games check
            if (ProcessChecker.IsRunning("EpicGamesLauncher") && !epichasRun)
            {
                Slow("Epic Games Launcher? Really?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("You know Steam exists, right?", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Epic gives you free games and you still can't make it cool.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                Slow("Tragic.", ConsoleColor.Red, 20);
                Thread.Sleep(500);
                Console.WriteLine();
                epichasRun = true;
            }
            else if (!ProcessChecker.IsRunning("EpicGamesLauncher"))
            {
                epichasRun = false;
            }

            Thread.Sleep(500);
        }
    }
}