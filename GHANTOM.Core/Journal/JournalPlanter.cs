using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GHANTOM.Core.Journal;

/// <summary>
/// Scatters GHANTOM's planted journal files into real parent directories of
/// wherever the exe happens to be running, per LORE.md: "a path or a few
/// back." Never overwrites a file that's already there, and never plants
/// anything if Lore/ wasn't present at build time (RegularEntries is empty).
/// </summary>
public static class JournalPlanter
{
    private const int MaxParentLevels = 3;

    /// <summary>Plants any regular entries that aren't already planted (or whose
    /// planted copy has since disappeared). Call once at troll-mode startup.</summary>
    public static void EnsurePlanted()
    {
        var entries = JournalArchive.RegularEntries;
        if (entries.Count == 0) return;

        var dirs = CandidateDirs();
        if (dirs.Count == 0) return;

        var manifest = JournalManifest.Load();
        bool changed = false;
        int dirIndex = 0;

        foreach (var entry in entries)
        {
            var state = manifest.Files.FirstOrDefault(f => f.FileName == entry.FileName);
            if (state == null)
            {
                state = new JournalFileState { FileName = entry.FileName };
                manifest.Files.Add(state);
            }

            if (state.PlantedPath != null && File.Exists(state.PlantedPath))
                continue;

            if (PlantInto(dirs, ref dirIndex, entry.FileName, entry.Content, out string plantedPath))
            {
                state.PlantedPath = plantedPath;
                state.Discovered = false;
                changed = true;
            }
        }

        if (changed) JournalManifest.Save(manifest);
    }

    /// <summary>Once every regular entry has been discovered, plants the one
    /// capstone file that ties the collection together. Caller owns loading
    /// and saving <paramref name="manifest"/>.</summary>
    public static bool PlantCapstoneIfComplete(JournalManifestData manifest)
    {
        if (manifest.CapstonePlanted) return false;
        if (manifest.Files.Count == 0 || !manifest.Files.All(f => f.Discovered)) return false;

        var capstone = JournalArchive.Capstone;
        if (capstone == null) return false;

        var dirs = CandidateDirs();
        if (dirs.Count == 0) return false;

        int dirIndex = 0;
        if (!PlantInto(dirs, ref dirIndex, capstone.FileName, capstone.Content, out string plantedPath))
            return false;

        manifest.CapstonePlanted = true;
        manifest.CapstonePlantedPath = plantedPath;
        return true;
    }

    private static bool PlantInto(List<string> dirs, ref int startIndex, string fileName, string content, out string plantedPath)
    {
        for (int i = 0; i < dirs.Count; i++)
        {
            string dir = dirs[(startIndex + i) % dirs.Count];
            string path = Path.Combine(dir, fileName);
            if (File.Exists(path)) continue; // never overwrite a real file that's already there

            try
            {
                File.WriteAllText(path, content);
                plantedPath = path;
                startIndex = (startIndex + i + 1) % dirs.Count;
                return true;
            }
            catch
            {
                // Directory not writable/locked - try the next candidate.
            }
        }

        plantedPath = null;
        return false;
    }

    private static List<string> CandidateDirs()
    {
        var dirs = new List<string>();
        try
        {
            string exeDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var dir = new DirectoryInfo(exeDir).Parent;
            for (int i = 0; i < MaxParentLevels && dir != null; i++)
            {
                dirs.Add(dir.FullName);
                dir = dir.Parent;
            }
        }
        catch
        {
            // Fall through with whatever we managed to collect.
        }
        return dirs;
    }
}
