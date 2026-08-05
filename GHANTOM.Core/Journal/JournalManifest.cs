using System.IO;
using System.Text.Json;

namespace GHANTOM.Core.Journal;

public sealed class JournalFileState
{
    public string FileName { get; set; }
    public string PlantedPath { get; set; }
    public bool Discovered { get; set; }
}

/// <summary>
/// Persisted, cross-process state for the planted-journal-files mechanic.
/// Both GHANTOM (which owns planting/discovery) and REPPLIF (which only
/// watches for the capstone flag) load and save this independently, the same
/// best-effort load-modify-save pattern <see cref="SaveFile"/> and
/// <see cref="ConversationChannel"/> already use for their shared files.
/// </summary>
public sealed class JournalManifestData
{
    public List<JournalFileState> Files { get; set; } = new();
    public bool CapstonePlanted { get; set; }
    public string CapstonePlantedPath { get; set; }
    public bool CapstoneDiscovered { get; set; }
    public bool CapstoneExchangeStarted { get; set; }
    public bool CreatorsMessageDelivered { get; set; }
}

public static class JournalManifest
{
    private static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GHANTOM", "journal_manifest.json");

    public static string CollectedFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GHANTOM", "CollectedJournal");

    public static JournalManifestData Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new JournalManifestData();
            var data = JsonSerializer.Deserialize<JournalManifestData>(File.ReadAllText(FilePath));
            return data ?? new JournalManifestData();
        }
        catch
        {
            // Corrupt or locked manifest: start fresh rather than crash the prank.
            return new JournalManifestData();
        }
    }

    public static void Save(JournalManifestData data)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch
        {
            // Best-effort; a missed write just means this gets retried next tick.
        }
    }
}
