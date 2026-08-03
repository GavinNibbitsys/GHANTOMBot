# GHANTOM

A for-fun Windows troll bot. GHANTOM watches what you're doing (open apps, active
window title, idle time, CPU/RAM) and roasts you for it in a console window.
REPPLIF is its second window — the gentle, doubtful-but-friendly counterpart who
occasionally talks back to GHANTOM in scripted exchanges.

This is a joke for consenting friends on their own machines. It is not malware,
but it *behaves* like the annoying kind of software on purpose (autostart,
restart-on-close), so see the Defender/SmartScreen note below before you run it
on someone.

## Installing

No installer, no admin rights, no .NET install needed — just two exes.

1. Grab `GHANTOM.exe` and/or `REPPLIF.exe` from [Releases](../../releases).
2. Save them wherever's convenient. They don't need to be in the same
   folder, or in any particular folder at all.
3. Double-click to run, or run from a terminal if you want to pass flags
   (see below).

Run both for the two-bot experience — see "Running it" below. First launch
will likely trip a SmartScreen warning; that's expected, see the
Defender/SmartScreen note below before you click past it.

## Running it

Each bot is its own exe: `GHANTOM.exe` and `REPPLIF.exe`. Run either or both.

- **No flags** — quiet dev/preview run. Comments on what you're doing, but
  closing the window closes it for good and nothing touches your startup
  programs.
- **`--troll`** — the full prank. Adds itself to your Windows startup
  (`HKCU\...\Run` — a per-user registry key, no admin rights needed) and
  relaunches itself if you try to close it.
- **`--untroll`** — removes the startup entry so the bot stops launching at
  login. Run this to clean a machine.

Run both `GHANTOM.exe` and `REPPLIF.exe` together for the two-bot experience —
they talk to each other across their two windows via a small shared state file
in `%LOCALAPPDATA%\GHANTOM\`, so it doesn't matter which folder each one runs
from.

## Heads up: Defender / SmartScreen

Autostart + restart-on-close is exactly the pattern antivirus heuristics flag.
Expect a SmartScreen "Windows protected your PC" prompt on first run, and
possibly a Defender warning. That's expected for this kind of app, not a sign
anything's actually wrong — click "More info" -> "Run anyway" if you trust the
source.

## Building from source

Requires the .NET 8 SDK.

```
dotnet build GHANTOM.sln -c Release
```

To produce standalone single-file exes that don't need .NET installed on the
target machine:

```
dotnet publish GHANTOM\GHANTOM.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish\GHANTOM
dotnet publish REPPLIF\REPPLIF.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish\REPPLIF
```

Built exes aren't committed to this repo — grab prebuilt ones from
[Releases](../../releases) instead if you don't need to build from source.

Working from a clone and don't want to type the full exe path every time?
Run `.\install-aliases.ps1` once to add `ghantom` and `repplif` PowerShell
functions to your `$PROFILE` that find the exe under this repo (built or
published) — flags like `--troll`/`--untroll` still pass through (e.g.
`ghantom --troll`). This only works from a local clone, not with exes
downloaded standalone from Releases.
