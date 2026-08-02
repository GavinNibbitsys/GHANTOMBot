using System.IO;
using System.Text.Json;

namespace GHANTOM.Core;

/// <summary>
/// Per-bot persisted state. Unlike the close-count that used to travel only
/// as a relaunch argument, this survives reboots, manual restarts, and
/// anything else that breaks the parent/child process chain.
/// </summary>
public sealed class SaveData
{
    public int CloseCount { get; set; }
    public DateTime FirstRunUtc { get; set; }
    public DateTime LastRunUtc { get; set; }
}

public static class SaveFile
{
    private static string DirPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GHANTOM");

    private static string PathFor(string appName) => Path.Combine(DirPath, appName + ".save.json");

    /// <summary>Loads this bot's save data, or a fresh one if none exists yet.</summary>
    public static SaveData Load(string appName)
    {
        try
        {
            string path = PathFor(appName);
            if (!File.Exists(path))
                return new SaveData { FirstRunUtc = DateTime.UtcNow };

            var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(path));
            return data ?? new SaveData { FirstRunUtc = DateTime.UtcNow };
        }
        catch
        {
            // Corrupt or locked save file: start fresh rather than crash the prank.
            return new SaveData { FirstRunUtc = DateTime.UtcNow };
        }
    }

    /// <summary>Persists this bot's save data, stamping LastRunUtc as now.</summary>
    public static void Save(string appName, SaveData data)
    {
        try
        {
            Directory.CreateDirectory(DirPath);
            data.LastRunUtc = DateTime.UtcNow;
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PathFor(appName), json);
        }
        catch
        {
            // Best-effort; a save failure just means less continuity next run.
        }
    }
}
