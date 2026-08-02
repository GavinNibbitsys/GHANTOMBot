# GHANTOM

A for-fun Windows troll bot. GHANTOM watches what you're doing (open apps, active
window title, idle time, CPU/RAM) and roasts you for it in a console window.
REPPLIF is its second window — the gentle, doubtful-but-friendly counterpart who
occasionally talks back to GHANTOM in scripted exchanges.

This is a joke for consenting friends on their own machines. It is not malware,
but it *behaves* like the annoying kind of software on purpose (autostart,
restart-on-close), so see the Defender/SmartScreen note below before you run it
on someone.

## Running it

Each bot is its own exe: `GHANTOM.exe` and `REPPLIF.exe`. Run either or both.

- **No flags** — quiet dev/preview run. Comments on what you're doing, but
  closing the window closes it for good and nothing touches your startup
  programs.
- **`--troll`** — the full prank. Adds itself to your Windows startup
  (`HKCU\...\Run`) and relaunches itself if you try to close it.
- **`--untroll`** — removes the startup entry so the bot stops launching at
  login. Run this to clean a machine.

Run both `GHANTOM.exe` and `REPPLIF.exe` together for the two-bot experience —
they talk to each other across their two windows via a small shared state file
in `%LOCALAPPDATA%\GHANTOM\`.

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

Built exes aren't committed to this repo — grab them from
[Releases](../../releases) instead.
