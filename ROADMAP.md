# GHANTOM Roadmap

## Context

GHANTOM is a for-fun Windows "troll bot" that watches what you're doing and roasts you in a
console window. Two friends are building it together; the code lives at
`https://github.com/GavinNibbitsys/GHANTOMBot` but nothing is checked out locally yet
(`A:\GHANTOM stuffs` is empty, no local git).

**Current state of the repo (4 files, C#):**
- `gbuddy.cs` (~700 lines) — the real app. A `while(true)` loop polls ~15 processes by name
  (`ProcessChecker.IsRunning`) and prints escalating **hand-authored** roasts one character at
  a time (`Slow()`). It hijacks the console close handler to restart itself
  (`SetConsoleCtrlHandler`), writes itself into the `HKCU\...\Run` startup key
  (`StartupManager.Enable`), and downloads the Mona Lisa as a gag when MS Paint is detected.
  No window-title, idle-time, or system-stat sensing yet.
- `GHANTOM.cs` — a bootstrapper that downloads `gbuddy.exe` from the repo and runs it.
- `GHANTOM.exe`, `gbuddy.exe` — committed compiled binaries.

**Goal:** get the project onto both machines with clean git; restructure the loose files into a
proper .NET solution; add a **second bot** (its own name, its own exe/window) that trades
**scripted** lines with GHANTOM; add the three new sensing sources (active window title, idle
time, system stats); keep the console output and the **deterministic, hand-authored** style
(no random dialogue).

**Decisions locked in (from the team):**
1. Convert to a proper .NET solution (`.sln` + projects), not loose `.cs` files.
2. Aggressive persistence (autostart + restart-on-close) gated behind a `--troll` flag; OFF by
   default in dev builds.
3. Drop the old download-and-run bootstrapper (`GHANTOM.cs`). Each bot is its own
   self-contained exe.
4. `.exe` files gitignored; distribute via GitHub Releases.
5. **Two bots = two separate exes / windows.** GHANTOM + a second, differently-named bot.
6. Their cross-talk is **scripted** (you author both sides), triggered **both** on a timer
   (ambient banter) and by what you're doing (reactive). Coordinated via a thin cue channel.
7. **No random dialogue** — keep the deterministic, count/context-driven authored style.

**Open item:** the second bot needs a name (placeholder below: `BOT2`). Pick one before Phase 4.

**Scope note:** joke toy for the team's own machines / consenting friends. The autostart +
restart-on-close pattern is what antivirus heuristics flag as malware, so Defender/SmartScreen
false-positives on the built exes are expected — a distribution nuisance to plan around.

---

## Phase 0 — Wire up GitHub (do first)

`gh` CLI is **not** installed; `git` is. Use plain git (optionally `winget install GitHub.cli`
for easier Releases later).

1. **Clone into the working dir** (it's empty):
   `git clone https://github.com/GavinNibbitsys/GHANTOMBot.git "A:\GHANTOM stuffs"`
2. **Set local git identity** if not global (`git config user.name` / `user.email`).
3. **Add a .NET `.gitignore`** before restructuring: `bin/`, `obj/`, `*.exe`, `*.dll`, `*.pdb`,
   `.vs/`, `*.user`. Use GitHub's standard `VisualStudio`/`Dotnet` template as the base.
4. **Untrack committed binaries:** `git rm --cached GHANTOM.exe gbuddy.exe` (keeps local files).
5. **Branch flow for two people:** work on short-lived feature branches
   (`git switch -c feature/second-bot`), merge/PR into `main`. Agree on it so you don't clobber
   each other.

---

## Phase 1 — Restructure into a solution (shared core + two bot exes)

Two bots share almost everything (sensing, the console printer, persistence, the conversation
engine). Put the shared code in a class library and make each bot a thin exe over it.

```
GHANTOM.sln
  GHANTOM.Core/        class library — all shared logic
  GHANTOM/             console exe — GHANTOM's identity + its authored lines
  BOT2/                console exe — second bot's identity + its authored lines
```

1. `dotnet new sln`; `dotnet new classlib -n GHANTOM.Core`;
   `dotnet new console -n GHANTOM`; `dotnet new console -n BOT2`; add all to the sln; both exes
   reference Core.
2. Target a Windows TFM (code uses WinForms/Drawing/Registry). In each `.csproj`:
   ```xml
   <TargetFramework>net10.0-windows</TargetFramework>   <!-- or net8.0-windows; match `dotnet --list-sdks` -->
   <UseWindowsForms>true</UseWindowsForms>
   ```
3. **Move existing logic from `gbuddy.cs` into `GHANTOM.Core`**, split by concern:
   `ProcessWatcher`, `ConsolePrinter` (the `Slow()`/`Color()` methods), `StartupManager`,
   `ConsoleCloseGuard` (the `SetConsoleCtrlHandler` restart logic). GHANTOM's specific roast
   lines stay in the `GHANTOM` project.
4. **Delete `GHANTOM.cs`** (the downloader) entirely.
5. **Self-contained single-file publish** per bot (friends won't need the .NET runtime):
   `dotnet publish GHANTOM -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`
   (same for `BOT2`). Expect large exes (~60–150MB each); drop `--self-contained` for tiny exes
   that require the runtime installed.
6. Confirm `dotnet run --project GHANTOM` still prints the roasts.

---

## Phase 2 — Gate persistence behind `--troll`

Make dev builds harmless; keep the full prank one flag away. Applies to **both** exes (shared
`StartupManager` / `ConsoleCloseGuard` in Core).

1. In each `Main`, `bool trollMode = args.Contains("--troll");`.
2. Guard on `trollMode`: `StartupManager.Enable()` (Run key), the `SetConsoleCtrlHandler`
   restart-on-close, and any self-relaunch. Without the flag, closing exits normally and
   nothing is written to startup.
3. Add `--untroll` → `StartupManager.Disable()` (already exists) so a machine can be cleaned.
4. README: `<bot>.exe` = quiet dev run; `<bot>.exe --troll` = full prank; `--untroll` = remove
   autostart.

---

## Phase 3 — Add the three new sensing sources

Team wants active window title, idle time, and system stats added to the existing process-name
polling. Each is a small helper in `GHANTOM.Core` (matching the `ProcessChecker` style), usable
by **both** bots, wired into the existing `while(true)` + `Thread.Sleep(500)` loop.

1. **Active window title** — `WindowWatcher`, P/Invoke `GetForegroundWindow` + `GetWindowText`
   (+ `GetWindowThreadProcessId`). Far more specific than process name (actual tab/file:
   "YouTube — Chrome", "main.cs — VS Code"). Fire on title change.
2. **Idle time** — `IdleWatcher`, P/Invoke `GetLastInputInfo` + `Environment.TickCount` →
   seconds since last input. Enables AFK / "staring at that for 6 minutes" jabs.
3. **System stats** — `SystemStats`, `System.Diagnostics.PerformanceCounter`
   (`Processor % Processor Time _Total`, `Memory Available MBytes`) or WMI via
   `System.Management`; plus `DateTime.Now.Hour` for late-night lines. "CPU's on fire",
   "47 Chrome tabs is a choice".
4. Wire in next to the existing checks, each with an already-fired guard (like `notepadhasRun`)
   plus a cooldown. **These watchers also feed Phase 4** — an event here can trigger a two-bot
   exchange.

---

## Phase 4 — The second bot + scripted cross-talk (the headline feature)

Two independent exes/windows that appear to converse, using **authored** exchanges kept in sync
by a thin cue channel. GHANTOM's line prints in GHANTOM's window; BOT2's reply prints in BOT2's
window — the conversation reads as split across the two windows.

**Data model (authored, deterministic — no randomness):**
- A **conversation script**: an ordered set of `Exchange`s. Each `Exchange` is a fixed sequence
  of `Turn`s: `{ speaker: "GHANTOM" | "BOT2", line: "..." }`. You write both sides. Optionally
  tag exchanges with a trigger (`onIdle`, `onProcess:chrome`, `onClose`, `ambient`) so the right
  bit plays for the right situation.
- Ship the script as an **embedded resource in `GHANTOM.Core`** so both exes load the identical
  set and single-file publish keeps working.

**Cue channel (coordination only, not message generation) — `ConversationChannel` in Core:**
- Both bots already poll every 500ms; have them also read/write a small shared state file, e.g.
  `%LOCALAPPDATA%\GHANTOM\convo.json`, holding `{ activeExchangeId, turnIndex, speakerTurn,
  lockOwner }`. (Named pipes are the cleaner upgrade later; a watched file is the simplest fit
  for the existing poll loop and is easy to debug.)
- **Turn-taking:** a bot only speaks when `speakerTurn == its name`. After it prints its turn,
  it advances `turnIndex`/`speakerTurn` in the file; the other bot sees it next tick and plays
  the next line. A `lockOwner` + timestamp prevents both starting an exchange at once and
  recovers if one bot isn't running.

**Triggers (both, per the team):**
- **Ambient/timer:** each bot, when idle and no exchange is active, may (on its own loose timer)
  claim the channel and kick off an `ambient` exchange.
- **Reactive:** a Phase-3 watcher event (you open Chrome, go idle, try to close a bot) claims the
  channel and starts the matching exchange; the other bot answers on its turns.

**BOT2 project:** same Core, its own name/console title/color and its own authored lines. Give it
a distinct personality (e.g. rival or sidekick) so the exchanges land. **Name it before coding.**

---

## Phase 5 — Distribution via GitHub Releases

- Publish both exes (Phase 1 command).
- Create a Release and **attach both** exes (don't commit them). With CLI:
  `gh release create v0.1 .\...\GHANTOM.exe .\...\BOT2.exe`. Otherwise upload via the web UI.
- README note: run both exes for the full two-bot experience; Defender/SmartScreen may warn on
  first run (expected for this kind of app).

---

## Verification

1. `dotnet build` on the solution succeeds; `dotnet run --project GHANTOM` prints roasts and,
   with no flag, exits cleanly on Ctrl+C and adds **no** Run key
   (`reg query HKCU\Software\Microsoft\Windows\CurrentVersion\Run`).
2. `--troll` restores restart-on-close + sets the Run key; `--untroll` removes it. (both bots)
3. New sensing: switch foreground windows → title line fires; sit idle → idle line fires; peg
   CPU / open many tabs → stats line fires.
4. **Two-bot cross-talk:** run `GHANTOM.exe` and `BOT2.exe` together → an ambient exchange plays
   across the two windows in correct turn order with no overlap; doing a triggering action
   (e.g. open Chrome) starts the matching exchange; closing one bot doesn't leave the other
   stuck (lock recovers).
5. `git status` shows no `bin/`, `obj/`, or `*.exe` staged; old committed exes untracked.
6. `dotnet publish` yields runnable single exes on a machine without the SDK; a Release exists
   with both attached and the repo file list is source-only.
