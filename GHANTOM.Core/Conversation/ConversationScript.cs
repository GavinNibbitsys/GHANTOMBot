using System.IO;
using System.Linq;
using System.Text.Json;

namespace GHANTOM.Core;

/// <summary>
/// Loads the hand-authored conversation script, embedded as a resource so
/// GHANTOM and REPPLIF always see the identical set (and single-file publish
/// keeps working).
/// </summary>
public static class ConversationScript
{
    private static List<Exchange> _all;

    public static IReadOnlyList<Exchange> All => _all ??= Load();

    public static Exchange Get(string id) => All.FirstOrDefault(e => e.Id == id);

    public static IEnumerable<Exchange> ForTrigger(string trigger) =>
        All.Where(e => e.Trigger == trigger);

    private static List<Exchange> Load()
    {
        var asm = typeof(ConversationScript).Assembly;
        using Stream stream = asm.GetManifestResourceStream("GHANTOM.Core.Conversation.script.json");
        if (stream == null) return new List<Exchange>();

        using var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<Exchange>>(json, options) ?? new List<Exchange>();
    }
}
