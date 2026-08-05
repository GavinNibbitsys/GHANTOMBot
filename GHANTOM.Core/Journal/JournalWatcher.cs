using System.IO;
using System.Linq;
using GHANTOM.Core;

namespace GHANTOM.Core.Journal;

/// <summary>
/// Matches the active window title against planted journal filenames. Per
/// LORE.md, opening a planted file is how GHANTOM "notices" - no polling of
/// file contents, just the same window-title watching GHANTOM already does.
/// </summary>
public static class JournalWatcher
{
    /// <summary>
    /// Call when the active window title changes. Returns true if a planted
    /// file (or the capstone) was matched, so the caller can skip its generic
    /// "I can read your window titles" line for this tick.
    /// </summary>
    public static bool CheckTitle(string title, ConsoleColor ink)
    {
        if (string.IsNullOrWhiteSpace(title)) return false;

        var manifest = JournalManifest.Load();
        bool changed = false;
        bool handled = false;

        // Capstone: no deflection line - discovering it hands off to the
        // scripted GHANTOM<->REPPLIF exchange instead of a normal reaction.
        if (manifest.CapstonePlanted && !manifest.CapstoneDiscovered &&
            title.Contains(JournalArchive.CapstoneFileName, StringComparison.OrdinalIgnoreCase))
        {
            manifest.CapstoneDiscovered = true;
            CollectCopy(JournalArchive.CapstoneFileName, JournalArchive.Capstone?.Content);
            changed = true;
            handled = true;
        }
        else
        {
            foreach (var state in manifest.Files)
            {
                if (state.Discovered) continue;
                if (!title.Contains(state.FileName, StringComparison.OrdinalIgnoreCase)) continue;

                state.Discovered = true;
                var entry = JournalArchive.RegularEntries.FirstOrDefault(e => e.FileName == state.FileName);
                CollectCopy(state.FileName, entry?.Content);
                ConsolePrinter.Slow(JournalArchive.ReactionFor(state.FileName), ink, 20);
                changed = true;
                handled = true;
                break; // one discovery per tick is plenty
            }

            if (JournalPlanter.PlantCapstoneIfComplete(manifest))
                changed = true;
        }

        if (changed) JournalManifest.Save(manifest);
        return handled;
    }

    private static void CollectCopy(string fileName, string content)
    {
        if (content == null) return;
        try
        {
            Directory.CreateDirectory(JournalManifest.CollectedFolder);
            File.WriteAllText(Path.Combine(JournalManifest.CollectedFolder, fileName), content);
        }
        catch
        {
            // Best-effort convenience copy; the discovery flag is already saved either way.
        }
    }
}
