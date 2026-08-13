<#
.SYNOPSIS
  Externalize embedded `changes:` history from artifact bottom-metadata blocks into
  sibling <stem>.changelog.md files, replacing the array with a `changelog:` pointer.

.PARAMETER Apply
  When omitted, runs in dry-run mode (discovery + preview, no writes).
  When present, performs the migration (snapshots to rollback/, writes changelogs, edits sources).
#>
param([switch]$Apply)

$ErrorActionPreference = 'Stop'
$root = 'c:\dev\darioairoldi\Learn'
$temp = Join-Path $root '.copilot\temp'
$rollback = Join-Path $temp 'rollback-changelog'
$utf8 = New-Object System.Text.UTF8Encoding($false)

$scanPaths = @(
    "$root\.github\agents",
    "$root\.github\prompts",
    "$root\.copilot\context",
    "$root\.github\templates",
    "$root\01.00-news",
    "$root\02.00-events",
    "$root\03.00-tech",
    "$root\04.00-howto",
    "$root\05.00-issues",
    "$root\06.00-idea"
)

function Get-LastCommentBlock {
    param([string]$text)
    $start = $text.LastIndexOf('<!--')
    if ($start -lt 0) { return $null }
    $end = $text.IndexOf('-->', $start)
    if ($end -lt 0) { return $null }
    return [pscustomobject]@{ Start = $start; End = $end + 3; Body = $text.Substring($start, ($end + 3) - $start) }
}

function Get-MetaField {
    param([string]$block, [string]$name)
    if ($block -match "(?m)^\s*$name`:\s*`"?([^`"\r\n]+)`"?\s*$") { return $Matches[1].Trim().Trim('"') }
    return $null
}

function Normalize-Version {
    param([string]$v)
    $v = $v.TrimStart('v', 'V')
    $parts = $v.Split('.')
    while ($parts.Count -lt 3) { $parts += '0' }
    if ($parts.Count -gt 3) { $parts = $parts[0..2] }
    return 'v' + ($parts -join '.')
}

$inventory = New-Object System.Collections.Generic.List[object]

foreach ($sp in $scanPaths) {
    if (-not (Test-Path $sp)) { continue }
    Get-ChildItem -Recurse -File -Path $sp -Include *.md -ErrorAction SilentlyContinue | ForEach-Object {
        $f = $_
        if ($f.Name -like '*.changelog.md') { return }
        if ($f.FullName -match '\\old\\') { return }
        $raw = [System.IO.File]::ReadAllText($f.FullName)
        if ([string]::IsNullOrWhiteSpace($raw)) { return }
        $blk = Get-LastCommentBlock $raw
        if ($null -eq $blk) { return }
        $block = $blk.Body
        if ($block -notmatch '_metadata:') { return }
        if ($block -notmatch '(?m)^\s\schanges:\s*$') { return }

        # Extract the changes: array lines from the block (key at 2-space indent, items at 4-space)
        $lines = $block -split "`r?`n"
        $ci = -1
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match '^\s\schanges:\s*$') { $ci = $i; break }
        }
        if ($ci -lt 0) { return }
        $entryLines = New-Object System.Collections.Generic.List[string]
        $lastItem = $ci
        for ($j = $ci + 1; $j -lt $lines.Count; $j++) {
            if ($lines[$j] -match '^\s{4}- ') { $entryLines.Add($lines[$j]); $lastItem = $j; continue }
            if ($lines[$j] -match '^\s{6,}\S') { $entryLines.Add($lines[$j]); $lastItem = $j; continue } # wrapped continuation
            break
        }
        if ($entryLines.Count -eq 0) { return }

        $stem = $f.BaseName
        $changelogName = "$stem.changelog.md"
        $lastUpdated = Get-MetaField $block 'last_updated'
        $created = Get-MetaField $block 'created'

        # Parse entries
        $entries = New-Object System.Collections.Generic.List[object]
        foreach ($el in $entryLines) {
            $s = $el.Trim()
            if ($s.StartsWith('- ')) { $s = $s.Substring(2) }
            $s = $s.Trim().Trim('"')
            if ([string]::IsNullOrWhiteSpace($s)) { continue }
            $m = [regex]::Match($s, '^v?(\d+(?:\.\d+){0,2})\s*(?:\(([0-9]{4}-[0-9]{2}-[0-9]{2})\))?\s*:\s*(.*)$')
            if ($m.Success) {
                $entries.Add([pscustomobject]@{ Ver = Normalize-Version $m.Groups[1].Value; Date = $m.Groups[2].Value; Text = $m.Groups[3].Value.Trim() })
            }
            else {
                $entries.Add([pscustomobject]@{ Ver = $null; Date = ''; Text = $s })
            }
        }

        $inventory.Add([pscustomobject]@{
                File          = $f.FullName
                Rel           = $f.FullName.Replace($root + '\', '')
                Stem          = $stem
                ChangelogName = $changelogName
                LastUpdated   = $lastUpdated
                Created       = $created
                EntryCount    = $entries.Count
                Entries       = $entries
                BlockStart    = $blk.Start
                BlockEnd      = $blk.End
                ChangesKeyIdx = $ci
                LastItemIdx   = $lastItem
                BlockLines    = $lines
            })
    }
}

Write-Host ("Discovered {0} files with embedded changes: history" -f $inventory.Count)
$inventory | Group-Object { ($_.Rel -split '\\')[0..1] -join '\' } | Sort-Object Name | ForEach-Object {
    Write-Host ("  {0,-30} {1}" -f $_.Name, $_.Count)
}

# Emit inventory JSON (without heavy nested arrays)
$invOut = $inventory | ForEach-Object {
    [pscustomobject]@{ rel = $_.Rel; changelog = $_.ChangelogName; entries = $_.EntryCount; last_updated = $_.LastUpdated; created = $_.Created }
}
[System.IO.File]::WriteAllText((Join-Path $temp 'externalization-inventory.json'), ($invOut | ConvertTo-Json -Depth 5), $utf8)

if (-not $Apply) {
    Write-Host "`n--- DRY RUN preview (first 2 files) ---"
    foreach ($it in ($inventory | Select-Object -First 2)) {
        Write-Host ("`n### {0}  ->  {1}" -f $it.Rel, $it.ChangelogName)
        foreach ($e in $it.Entries) {
            $d = if ($e.Date) { $e.Date } elseif ($it.LastUpdated) { $it.LastUpdated + ' (from last_updated)' } else { '(date not recorded)' }
            Write-Host ("  ## {0} -- {1}" -f $e.Ver, $d)
            Write-Host ("     {0}" -f $e.Text.Substring(0, [Math]::Min(90, $e.Text.Length)))
        }
    }
    Write-Host "`nDRY RUN complete. Re-run with -Apply to migrate."
    return
}

# APPLY
if (-not (Test-Path $rollback)) { New-Item -ItemType Directory -Path $rollback | Out-Null }
$applied = New-Object System.Collections.Generic.List[object]

foreach ($it in $inventory) {
    $raw = [System.IO.File]::ReadAllText($it.File)

    # snapshot
    $snap = Join-Path $rollback ($it.Rel -replace '[\\/]', '__')
    [System.IO.File]::WriteAllText($snap, $raw, $utf8)

    # build changelog entries (most-recent-first; source arrays already newest-first)
    $sb = New-Object System.Text.StringBuilder
    [void]$sb.AppendLine('---')
    [void]$sb.AppendLine("title: `"$($it.Stem) — change history`"")
    [void]$sb.AppendLine("description: `"Per-version change history for $($it.Stem).`"")
    if ($it.LastUpdated) { [void]$sb.AppendLine("last_updated: `"$($it.LastUpdated)`"") }
    [void]$sb.AppendLine('status: "living"')
    [void]$sb.AppendLine('---')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine("# Change history — $($it.Stem)")
    [void]$sb.AppendLine('')
    $idx = 0
    foreach ($e in $it.Entries) {
        if ($e.Ver) {
            $d = if ($e.Date) { $e.Date }
            elseif ($idx -eq 0 -and $it.LastUpdated) { $it.LastUpdated }
            elseif ($it.Created) { $it.Created }
            else { $null }
            if ($d) { [void]$sb.AppendLine("## $($e.Ver) — $d") }
            else { [void]$sb.AppendLine("## $($e.Ver) — (date not recorded)") }
            [void]$sb.AppendLine('')
            [void]$sb.AppendLine($e.Text)
            [void]$sb.AppendLine('')
        }
        else {
            [void]$sb.AppendLine("## (unversioned)")
            [void]$sb.AppendLine('')
            [void]$sb.AppendLine($e.Text)
            [void]$sb.AppendLine('')
        }
        $idx++
    }
    $clPath = Join-Path $it.File.Substring(0, $it.File.LastIndexOf('\')) $it.ChangelogName
    [System.IO.File]::WriteAllText($clPath, $sb.ToString().TrimEnd() + "`n", $utf8)

    # edit source: replace the changes: block (key line + item lines) with a changelog pointer line
    $lines = $it.BlockLines
    $newBlockLines = New-Object System.Collections.Generic.List[string]
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($i -eq $it.ChangesKeyIdx) {
            $newBlockLines.Add("  changelog: `"$($it.ChangelogName)`"")
            $i = $it.LastItemIdx  # skip the item lines
            continue
        }
        $newBlockLines.Add($lines[$i])
    }
    $newBlock = ($newBlockLines -join "`n")
    $newRaw = $raw.Substring(0, $it.BlockStart) + $newBlock + $raw.Substring($it.BlockEnd)
    [System.IO.File]::WriteAllText($it.File, $newRaw, $utf8)

    # hidden attribute best-effort
    try { (Get-Item $clPath).Attributes += 'Hidden' } catch {}

    $applied.Add([pscustomobject]@{ source = $it.Rel; changelog = $it.ChangelogName; entries = $it.EntryCount })
}

[System.IO.File]::WriteAllText((Join-Path $temp 'externalization-report.json'), ($applied | ConvertTo-Json -Depth 5), $utf8)
Write-Host ("`nAPPLIED: {0} sources migrated, {0} changelog files created." -f $applied.Count)
