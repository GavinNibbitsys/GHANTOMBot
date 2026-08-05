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
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var exchanges = LoadFromResource("GHANTOM.Core.Conversation.script.json", options);

        // The capstone exchange lives in the gitignored Lore/ folder (not the
        // public script.json) so its content never lands in git - it's only
        // present in the compiled exe if Lore/ existed locally at build time.
        exchanges.AddRange(LoadFromResource("GHANTOM.Core.Lore.capstone_exchange.json", options));

        return exchanges;
    }

    private static List<Exchange> LoadFromResource(string resourceName, JsonSerializerOptions options)
    {
        var asm = typeof(ConversationScript).Assembly;
        using Stream stream = asm.GetManifestResourceStream(resourceName);
        if (stream == null) return new List<Exchange>();

        using var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<List<Exchange>>(json, options) ?? new List<Exchange>();
    }
}
