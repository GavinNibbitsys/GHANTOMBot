namespace GHANTOM.Core;

/// <summary>One line in a scripted exchange.</summary>
public sealed class Turn
{
    public string Speaker { get; set; }
    public string Line { get; set; }
}

/// <summary>
/// A fixed, hand-authored back-and-forth between GHANTOM and REPPLIF.
/// Turns are always played in order — nothing here is randomized.
/// </summary>
public sealed class Exchange
{
    public string Id { get; set; }

    /// <summary>"ambient" for idle-timer banter, or a reactive key like
    /// "onProcess:chrome", "onIdle", "lateNight", "onClose".</summary>
    public string Trigger { get; set; }

    public List<Turn> Turns { get; set; } = new();
}
