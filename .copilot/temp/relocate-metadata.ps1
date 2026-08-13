#requires -Version 5.1
[CmdletBinding()]
param(
    [switch]$Apply  # default: dry-run (report only)
)

$ErrorActionPreference = 'Stop'
$root = 'c:\dev\darioairoldi\Learn'

# (folder, include-glob, bottom-key)
$targets = @(
    @{ Path = "$root\.github\agents"; Include = '*.agent.md'; Key = 'agent_metadata' }
    @{ Path = "$root\.github\prompts"; Include = '*.md'; Key = 'prompt_metadata' }
    @{ Path = "$root\.copilot\context"; Include = '*.md'; Key = 'context_metadata' }
    @{ Path = "$root\.github\instructions"; Include = '*.md'; Key = 'instruction_metadata' }
    @{ Path = "$root\.github\templates"; Include = '*.md'; Key = 'template_metadata' }
)

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Normalize-Version([string]$v) {
    if (-not $v) { return $null }
    $v = $v.Trim().Trim('"')
    $parts = $v -split '\.'
    while ($parts.Count -lt 3) { $parts += '0' }
    return ($parts[0..2] -join '.')
}

function Compare-Version([string]$a, [string]$b) {
    # returns the higher of two normalized version strings
    if (-not $a) { return $b }
    if (-not $b) { return $a }
    try { $va = [version](Normalize-Version $a) } catch { return $b }
    try { $vb = [version](Normalize-Version $b) } catch { return $a }
    if ($va -ge $vb) { return (Normalize-Version $a) } else { return (Normalize-Version $b) }
}

function Compare-Date([string]$a, [string]$b) {
    if (-not $a) { return $b }
    if (-not $b) { return $a }
    try { $da = [datetime]::Parse($a.Trim('"')) } catch { return $b }
    try { $db = [datetime]::Parse($b.Trim('"')) } catch { return $a }
    if ($da -ge $db) { return $a.Trim('"') } else { return $b.Trim('"') }
}

function Normalize-Date([string]$d) {
    if (-not $d) { return $null }
    $d = $d.Trim().Trim('"')
    $m = [regex]::Match($d, '^\d{4}-\d{2}-\d{2}')
    if ($m.Success) { return $m.Value }
    try { return ([datetime]::Parse($d)).ToString('yyyy-MM-dd') } catch { return $d }
}

$report = @()
$changed = 0
$skippedCompliant = 0
$needsVersion = @()

foreach ($t in $targets) {
    $files = Get-ChildItem -Recurse -File -Path $t.Path -Include $t.Include -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -notlike '*.changelog.md' }
    foreach ($f in $files) {
        $raw = [System.IO.File]::ReadAllText($f.FullName)
        $nl = if ($raw -match "`r`n") { "`r`n" } else { "`n" }

        # --- extract top frontmatter ---
        $fmMatch = [regex]::Match($raw, '\A---\r?\n(.*?)\r?\n---\r?\n', 'Singleline')
        $topVer = $null; $topDate = $null
        $fmBody = $null
        if ($fmMatch.Success) {
            $fmBody = $fmMatch.Groups[1].Value
            $vm = [regex]::Match($fmBody, '(?m)^version:\s*(.+?)\s*$')
            if ($vm.Success) { $topVer = $vm.Groups[1].Value.Trim() }
            $dm = [regex]::Match($fmBody, '(?m)^last_updated:\s*(.+?)\s*$')
            if ($dm.Success) { $topDate = $dm.Groups[1].Value.Trim() }
        }

        if (-not $topVer -and -not $topDate) {
            # already compliant (no top tracking fields) — but flag if NO version anywhere
            $hasBot = $raw -match ("(?ms)" + [regex]::Escape($t.Key) + ':.*?version:')
            if (-not $hasBot) { $needsVersion += $f.FullName.Substring($root.Length + 1) }
            $skippedCompliant++
            continue
        }

        # --- locate bottom block ---
        $botKey = $t.Key
        $botRegex = [regex]"(?ms)(<!--\s*\r?\n(?:---\r?\n)?\s*$([regex]::Escape($botKey)):\s*\r?\n)(.*?)(\r?\n\s*(?:---\r?\n)?\s*-->)"
        $botMatch = $botRegex.Match($raw)

        $botVer = $null; $botDate = $null
        if ($botMatch.Success) {
            $inner = $botMatch.Groups[2].Value
            $bvm = [regex]::Match($inner, '(?m)^\s*version:\s*(.+?)\s*$')
            if ($bvm.Success) { $botVer = $bvm.Groups[1].Value.Trim() }
            $bdm = [regex]::Match($inner, '(?m)^\s*last_updated:\s*(.+?)\s*$')
            if ($bdm.Success) { $botDate = $bdm.Groups[1].Value.Trim() }
        }

        $finalVer = Normalize-Version (Compare-Version $topVer $botVer)
        $finalDate = Normalize-Date (Compare-Date $topDate $botDate)

        # --- remove top version/last_updated lines (only within frontmatter) ---
        $newFmBody = $fmBody
        $newFmBody = [regex]::Replace($newFmBody, '(?m)^version:\s*.+?\s*\r?\n', '')
        $newFmBody = [regex]::Replace($newFmBody, '(?m)^last_updated:\s*.+?\s*\r?\n', '')
        # handle case where the removed line is the last line of frontmatter (no trailing newline)
        $newFmBody = [regex]::Replace($newFmBody, '(?m)\r?\n?^version:\s*.+?\s*$', '')
        $newFmBody = [regex]::Replace($newFmBody, '(?m)\r?\n?^last_updated:\s*.+?\s*$', '')

        $newRaw = $raw.Remove($fmMatch.Index, $fmMatch.Length)
        $newFm = "---$nl$newFmBody$nl---$nl"
        $newRaw = $newRaw.Insert($fmMatch.Index, $newFm)

        # --- update or insert bottom block ---
        # recompute bottom match against newRaw (frontmatter edit shifts indices)
        $botMatch2 = $botRegex.Match($newRaw)
        $verLine = "  version: `"$finalVer`""
        $dateLine = "  last_updated: `"$finalDate`""
        if ($botMatch2.Success) {
            $inner = $botMatch2.Groups[2].Value
            $hadVer = [regex]::IsMatch($inner, '(?m)^\s*version:')
            $hadDate = [regex]::IsMatch($inner, '(?m)^\s*last_updated:')
            $newInner = $inner
            if ($hadVer) {
                $newInner = [regex]::Replace($newInner, '(?m)^(\s*)version:\s*.+?\s*$', "`${1}version: `"$finalVer`"")
            }
            if ($hadDate) {
                $newInner = [regex]::Replace($newInner, '(?m)^(\s*)last_updated:\s*.+?\s*$', "`${1}last_updated: `"$finalDate`"")
            }
            # if missing, prepend after the key line
            $prefix = ''
            if (-not $hadVer) { $prefix += "$verLine$nl" }
            if (-not $hadDate) { $prefix += "$dateLine$nl" }
            $replacement = $botMatch2.Groups[1].Value + $prefix + $newInner + $botMatch2.Groups[3].Value
            $newRaw = $newRaw.Remove($botMatch2.Index, $botMatch2.Length).Insert($botMatch2.Index, $replacement)
        }
        else {
            # append a new bottom block at EOF
            $trimmed = $newRaw.TrimEnd()
            $block = "$nl$nl<!--$nl$botKey`:$nl$verLine$nl$dateLine$nl-->$nl"
            $newRaw = $trimmed + $block
        }

        $rel = $f.FullName.Substring($root.Length + 1)
        $report += [pscustomobject]@{
            File       = $rel
            TopVer     = $topVer
            BotVer     = $botVer
            FinalVer   = $finalVer
            TopDate    = $topDate
            BotDate    = $botDate
            FinalDate  = $finalDate
            BotExisted = $botMatch.Success
            Drift      = ($botVer -and (Normalize-Version $topVer) -ne (Normalize-Version $botVer))
        }
        $changed++

        if ($Apply) {
            [System.IO.File]::WriteAllText($f.FullName, $newRaw, $utf8NoBom)
        }
    }
}

Write-Host ("MODE: " + $(if ($Apply) { 'APPLY' } else { 'DRY-RUN' })) -ForegroundColor Cyan
Write-Host "Files to change: $changed   Already-compliant: $skippedCompliant" -ForegroundColor Yellow
Write-Host "Drift cases (top != bottom):" -ForegroundColor Magenta
$report | Where-Object Drift | Sort-Object File | Format-Table File, TopVer, BotVer, FinalVer, TopDate, BotDate, FinalDate -AutoSize
Write-Host "`nFiles with NO version anywhere (NOT touched, manual follow-up):" -ForegroundColor Red
$needsVersion | Sort-Object | ForEach-Object { "  $_" }
Write-Host "`nFull change list count by type:" -ForegroundColor Green
$report | Group-Object { ($_.File -split '[\\/]')[0..1] -join '/' } | ForEach-Object { "  {0}: {1}" -f $_.Name, $_.Count }

$report | ConvertTo-Json -Depth 3 | Set-Content -Path 'c:\dev\darioairoldi\Learn\.copilot\temp\relocate-report.json' -Encoding utf8
Write-Host "`n=== DRIFT DETAIL ===" -ForegroundColor Magenta
$report | Where-Object Drift | Sort-Object File | ForEach-Object {
    "{0}`n    ver: top={1} bot={2} -> {3}   date: top={4} bot={5} -> {6}" -f `
        $_.File, $_.TopVer, $_.BotVer, $_.FinalVer, $_.TopDate, $_.BotDate, $_.FinalDate
}
