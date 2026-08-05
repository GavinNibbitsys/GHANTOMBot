using System.IO;
using System.Linq;
using System.Text.Json;

namespace GHANTOM.Core;

internal sealed class ConvoState
{
    public string ExchangeId { get; set; }
    public int TurnIndex { get; set; }
    public string SpeakerTurn { get; set; }
    public string LockOwner { get; set; }
    public DateTime LockUtc { get; set; }
    public DateTime LastExchangeEndUtc { get; set; }
    public int NextAmbientIndex { get; set; }
}

/// <summary>
/// Thin file-based cue channel that lets two independent exes take turns
/// printing a shared, pre-authored <see cref="Exchange"/> across their own
/// separate console windows. Coordination only — every line spoken comes
/// straight out of <see cref="ConversationScript"/>, never generated here.
/// </summary>
public static class ConversationChannel
{
    private const double StaleLockSeconds = 20;
    private const double AmbientCooldownSeconds = 150;

    private static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GHANTOM", "convo.json");

    /// <summary>
    /// Call once per poll tick. If it's this bot's turn in an active
    /// exchange, prints that line and advances the shared state.
    /// </summary>
    public static void Tick(string myName, ConsoleColor ink)
    {
        var state = Load();

        // Self-heal: the other bot may have closed mid-exchange.
        if (state.ExchangeId != null && (DateTime.UtcNow - state.LockUtc).TotalSeconds > StaleLockSeconds)
        {
            state.ExchangeId = null;
            state.SpeakerTurn = null;
            state.LastExchangeEndUtc = DateTime.UtcNow;
            Save(state);
            return;
        }

        if (state.ExchangeId == null || state.SpeakerTurn != myName)
            return;

        PlayCurrentTurn(state, myName, ink);
    }

    /// <summary>
    /// Attempts to start a specific reactive exchange. Only succeeds if no
    /// exchange is currently active and this bot speaks its opening line.
    /// </summary>
    public static bool TryStart(string myName, string exchangeId, ConsoleColor ink)
    {
        var exchange = ConversationScript.Get(exchangeId);
        if (exchange == null || exchange.Turns.Count == 0) return false;
        if (exchange.Turns[0].Speaker != myName) return false;

        var state = Load();
        if (state.ExchangeId != null) return false;

        state.ExchangeId = exchangeId;
        state.TurnIndex = 0;
        state.SpeakerTurn = myName;
        state.LockOwner = myName;
        state.LockUtc = DateTime.UtcNow;
        Save(state);

        PlayCurrentTurn(Load(), myName, ink);
        return true;
    }

    /// <summary>True when no exchange is currently active - safe to start a new one.</summary>
    public static bool IsIdle() => Load().ExchangeId == null;

    /// <summary>
    /// Deterministic round-robin through the ambient exchanges: only fires
    /// when nothing is active and the cooldown has elapsed, and only the bot
    /// who speaks that exchange's opening line will claim it.
    /// </summary>
    public static bool TryStartNextAmbient(string myName, ConsoleColor ink)
    {
        var state = Load();
        if (state.ExchangeId != null) return false;
        if ((DateTime.UtcNow - state.LastExchangeEndUtc).TotalSeconds < AmbientCooldownSeconds) return false;

        var ambientIds = ConversationScript.ForTrigger("ambient").Select(e => e.Id).ToList();
        if (ambientIds.Count == 0) return false;

        int idx = ((state.NextAmbientIndex % ambientIds.Count) + ambientIds.Count) % ambientIds.Count;
        var exchange = ConversationScript.Get(ambientIds[idx]);
        if (exchange.Turns[0].Speaker != myName) return false;

        state.NextAmbientIndex = idx + 1;
        state.ExchangeId = exchange.Id;
        state.TurnIndex = 0;
        state.SpeakerTurn = myName;
        state.LockOwner = myName;
        state.LockUtc = DateTime.UtcNow;
        Save(state);

        PlayCurrentTurn(Load(), myName, ink);
        return true;
    }

    private static void PlayCurrentTurn(ConvoState state, string myName, ConsoleColor ink)
    {
        if (state.ExchangeId == null || state.SpeakerTurn != myName) return;

        var exchange = ConversationScript.Get(state.ExchangeId);
        if (exchange == null || state.TurnIndex >= exchange.Turns.Count)
        {
            state.ExchangeId = null;
            state.SpeakerTurn = null;
            state.LastExchangeEndUtc = DateTime.UtcNow;
            Save(state);
            return;
        }

        ConsolePrinter.Slow(exchange.Turns[state.TurnIndex].Line, ink, 20);
        state.TurnIndex++;

        if (state.TurnIndex >= exchange.Turns.Count)
        {
            state.ExchangeId = null;
            state.SpeakerTurn = null;
            state.LastExchangeEndUtc = DateTime.UtcNow;
        }
        else
        {
            state.SpeakerTurn = exchange.Turns[state.TurnIndex].Speaker;
        }

        state.LockOwner = myName;
        state.LockUtc = DateTime.UtcNow;
        Save(state);
    }

    private static ConvoState Load()
    {
        try
        {
            string dir = Path.GetDirectoryName(FilePath);
            Directory.CreateDirectory(dir);
            if (!File.Exists(FilePath)) return new ConvoState();

            var state = JsonSerializer.Deserialize<ConvoState>(File.ReadAllText(FilePath));
            return state ?? new ConvoState();
        }
        catch
        {
            // Locked/corrupt cue file: treat as idle rather than crash the loop.
            return new ConvoState();
        }
    }

    private static void Save(ConvoState state)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch
        {
            // Best-effort; a missed write just means the other bot waits a tick longer.
        }
    }
}
