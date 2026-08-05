using System.IO;
using System.Linq;
using System.Text.Json;

namespace GHANTOM.Core.Journal;

/// <summary>
/// Loads GHANTOM's planted journal entries, the capstone file, the per-file
/// reaction lines, and the creators' capstone message - all embedded straight
/// from the local (gitignored) Lore/ folder at build time, per Exchange/
/// ConversationScript's existing embedded-resource pattern. If Lore/ isn't
/// present on this machine, every list here is simply empty and the whole
/// mechanic no-ops rather than crashing.
/// </summary>
public static class JournalArchive
{
    public const string CapstoneFileName = "if_you_found_all_of_these.txt";

    private const string JournalPrefix = "GHANTOM.Core.Lore.Journal.";
    private const string ReactionsResource = "GHANTOM.Core.Lore.reactions.json";
    private const string CreatorsMessageResource = "GHANTOM.Core.Lore.creators_message.txt";

    public sealed class JournalEntry
    {
        public string FileName { get; set; }
        public string Content { get; set; }
    }

    private sealed class ReactionRow
    {
        public string FileName { get; set; }
        public string Line { get; set; }
    }

    private static List<JournalEntry> _allEntries;
    private static Dictionary<string, string> _reactions;
    private static string _creatorsMessage;
    private static bool _creatorsMessageLoaded;

    /// <summary>All planted files, capstone included.</summary>
    private static List<JournalEntry> AllEntries => _allEntries ??= LoadEntries();

    /// <summary>The 17 regular journal entries (everything but the capstone).</summary>
    public static IReadOnlyList<JournalEntry> RegularEntries =>
        AllEntries.Where(e => e.FileName != CapstoneFileName).ToList();

    /// <summary>The capstone file, or null if Lore/ isn't present.</summary>
    public static JournalEntry Capstone =>
        AllEntries.FirstOrDefault(e => e.FileName == CapstoneFileName);

    /// <summary>GHANTOM's deflection line for a given planted filename, or a generic fallback.</summary>
    public static string ReactionFor(string fileName)
    {
        var reactions = _reactions ??= LoadReactions();
        return reactions.TryGetValue(fileName, out var line)
            ? line
            : "You weren't supposed to see that. Close it.";
    }

    /// <summary>The real, out-of-fiction capstone reward message, or null if not written yet.</summary>
    public static string CreatorsMessage
    {
        get
        {
            if (!_creatorsMessageLoaded)
            {
                _creatorsMessage = ReadResourceText(CreatorsMessageResource);
                _creatorsMessageLoaded = true;
            }
            return _creatorsMessage;
        }
    }

    private static List<JournalEntry> LoadEntries()
    {
        var asm = typeof(JournalArchive).Assembly;
        var result = new List<JournalEntry>();

        foreach (var name in asm.GetManifestResourceNames())
        {
            if (!name.StartsWith(JournalPrefix)) continue;
            string fileName = name.Substring(JournalPrefix.Length);
            string content = ReadResourceText(name);
            if (content != null)
                result.Add(new JournalEntry { FileName = fileName, Content = content });
        }

        return result;
    }

    private static Dictionary<string, string> LoadReactions()
    {
        string json = ReadResourceText(ReactionsResource);
        if (json == null) return new Dictionary<string, string>();

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var rows = JsonSerializer.Deserialize<List<ReactionRow>>(json, options) ?? new List<ReactionRow>();
            return rows.ToDictionary(r => r.FileName, r => r.Line);
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    private static string ReadResourceText(string resourceName)
    {
        var asm = typeof(JournalArchive).Assembly;
        using Stream stream = asm.GetManifestResourceStream(resourceName);
        if (stream == null) return null;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
