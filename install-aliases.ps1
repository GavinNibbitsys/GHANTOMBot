<#
Installs `ghantom` / `repplif` PowerShell functions into your $PROFILE so you
can launch the bots by name instead of typing the full exe path. Also installs
`ghantom-update` / `repplif-update`, which pull the latest git changes and
rebuild both exes -- save data lives outside the repo
(%LOCALAPPDATA%\GHANTOM\*.save.json) so updating never touches it. Safe to
re-run; it won't duplicate entries.
#>

$repoRoot = $PSScriptRoot

$markerStart = '# >>> GHANTOM aliases (install-aliases.ps1) >>>'
$markerEnd = '# <<< GHANTOM aliases (install-aliases.ps1) <<<'

$block = @"
$markerStart
function ghantom { Start-GhantomBot -BotName 'GHANTOM' -Args `$args }
function repplif { Start-GhantomBot -BotName 'REPPLIF' -Args `$args }

function Start-GhantomBot {
    param(
        [Parameter(Mandatory)][string]`$BotName,
        [string[]]`$Args
    )

    `$repoRoot = '$repoRoot'
    `$exePath = Join-Path `$repoRoot "publish\`$BotName\`$BotName.exe"

    if (-not (Test-Path `$exePath)) {
        `$found = Get-ChildItem -Path `$repoRoot -Recurse -Filter "`$BotName.exe" -ErrorAction SilentlyContinue |
            Where-Object { `$_.FullName -notmatch '\\obj\\' } |
            Select-Object -First 1
        if (`$found) {
            `$exePath = `$found.FullName
        }
    }

    if (-not (Test-Path `$exePath)) {
        Write-Warning "Couldn't find `$BotName.exe under `$repoRoot. Build it (see README's 'Building from source') or download it from the repo's Releases page."
        return
    }

    & `$exePath @Args
}

function ghantom-update { Update-GhantomBots }
function repplif-update { Update-GhantomBots }

function Update-GhantomBots {
    `$repoRoot = '$repoRoot'

    `$dirty = git -C `$repoRoot status --porcelain
    if (`$dirty) {
        Write-Warning "Uncommitted changes in `$repoRoot -- 'git pull' may refuse to run. Commit or stash first if it does."
    }

    Write-Host "Pulling latest changes in `$repoRoot..."
    git -C `$repoRoot pull --ff-only
    if (`$LASTEXITCODE -ne 0) {
        Write-Warning "git pull failed (probably diverged/conflicting local changes) -- resolve it manually in `$repoRoot, then re-run the update."
        return
    }

    `$dotnet = 'dotnet'
    if (-not (& `$dotnet --list-sdks 2>`$null)) {
        `$fallback = Join-Path `$env:LOCALAPPDATA 'Microsoft\dotnet\dotnet.exe'
        if (Test-Path `$fallback) {
            `$dotnet = `$fallback
        }
    }

    foreach (`$bot in @('GHANTOM', 'REPPLIF')) {
        Write-Host "Publishing `$bot..."
        & `$dotnet publish "`$repoRoot\`$bot\`$bot.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "`$repoRoot\publish\`$bot"
        if (`$LASTEXITCODE -ne 0) {
            Write-Warning "Publish failed for `$bot -- see errors above."
        }
    }

    Write-Host "Update complete. Save data under `$env:LOCALAPPDATA\GHANTOM\ is untouched -- run 'ghantom' or 'repplif' to launch the updated build."
}
$markerEnd
"@

if (-not (Test-Path $PROFILE)) {
    $profileDir = Split-Path $PROFILE -Parent
    if (-not (Test-Path $profileDir)) {
        New-Item -ItemType Directory -Path $profileDir -Force | Out-Null
    }
    New-Item -ItemType File -Path $PROFILE -Force | Out-Null
}

$existing = Get-Content $PROFILE -Raw -ErrorAction SilentlyContinue
if ($existing -and $existing.Contains($markerStart)) {
    $pattern = [regex]::Escape($markerStart) + '.*?' + [regex]::Escape($markerEnd)
    $updated = [regex]::Replace($existing, $pattern, [System.Text.RegularExpressions.MatchEvaluator]{ param($m) $block }, 'Singleline')
    Set-Content -Path $PROFILE -Value $updated -Encoding utf8
    Write-Host "Updated existing GHANTOM aliases in $PROFILE."
} else {
    Add-Content -Path $PROFILE -Value "`n$block" -Encoding utf8
    Write-Host "Added GHANTOM aliases to $PROFILE."
}

Write-Host "Restart your shell or run '. `$PROFILE' to use 'ghantom', 'repplif', 'ghantom-update', and 'repplif-update' now."
