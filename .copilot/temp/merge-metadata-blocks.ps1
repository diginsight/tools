#requires -Version 5.1
[CmdletBinding()]
param([switch]$Apply)

$ErrorActionPreference = 'Stop'
$root = 'c:\dev\darioairoldi\Learn'
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

# NOTE: templates are EXCLUDED — their trailing example blocks are content, not duplicate metadata.
$keyByPrefix = @(
    @{ Prefix = "$root\.github\agents"; Key = 'agent_metadata' }
    @{ Prefix = "$root\.github\prompts"; Key = 'prompt_metadata' }
    @{ Prefix = "$root\.copilot\context"; Key = 'context_metadata' }
    @{ Prefix = "$root\.github\instructions"; Key = 'instruction_metadata' }
)

function Get-CanonicalKey([string]$full) {
    foreach ($k in $keyByPrefix) { if ($full.StartsWith($k.Prefix, [StringComparison]::OrdinalIgnoreCase)) { return $k.Key } }
    return $null
}
function Normalize-Version([string]$v) {
    if (-not $v) { return $null }
    $v = $v.Trim().Trim('"')
    if ($v -notmatch '^\d') { return $v }
    $p = $v -split '\.'; while ($p.Count -lt 3) { $p += '0' }
    return ($p[0..2] -join '.')
}
function Higher-Version([string]$a, [string]$b) {
    if (-not $a) { return $b }; if (-not $b) { return $a }
    try { $va = [version](Normalize-Version $a) } catch { return $b }
    try { $vb = [version](Normalize-Version $b) } catch { return $a }
    if ($va -ge $vb) { return (Normalize-Version $a) } else { return (Normalize-Version $b) }
}
function Norm-Date([string]$d) {
    if (-not $d) { return $null }
    $d = $d.Trim().Trim('"')
    $m = [regex]::Match($d, '^\d{4}-\d{2}-\d{2}'); if ($m.Success) { return $m.Value }
    try { return ([datetime]::Parse($d)).ToString('yyyy-MM-dd') } catch { return $d }
}
function Newer-Date([string]$a, [string]$b) {
    if (-not $a) { return $b }; if (-not $b) { return $a }
    try { $da = [datetime]::Parse($a.Trim('"')) } catch { return $b }
    try { $db = [datetime]::Parse($b.Trim('"')) } catch { return $a }
    if ($da -ge $db) { return $a } else { return $b }
}

$report = @()

foreach ($k in $keyByPrefix) {
    $files = Get-ChildItem -Recurse -File -Path $k.Prefix -Include *.md -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -notlike '*.changelog.md' }
    foreach ($f in $files) {
        $raw = [System.IO.File]::ReadAllText($f.FullName)
        $nl = if ($raw -match "`r`n") { "`r`n" } else { "`n" }
        $canon = Get-CanonicalKey $f.FullName

        # --- collect adjacent trailing comment blocks ---
        $rest = $raw.TrimEnd()
        $blocks = New-Object System.Collections.ArrayList
        while ($true) {
            if ($rest -notmatch '-->\s*$') { break }
            $start = $rest.LastIndexOf('<!--')
            if ($start -lt 0) { break }
            $block = $rest.Substring($start)
            if ($block -notmatch '(?s)^<!--.*-->\s*$') { break }
            [void]$blocks.Insert(0, $block.TrimEnd())
            $rest = $rest.Substring(0, $start).TrimEnd()
        }

        # classify metadata blocks among trailing
        $metaIdx = @()
        for ($i = 0; $i -lt $blocks.Count; $i++) {
            if ($blocks[$i] -match '(?m)^\s*[a-z_]+_metadata:') { $metaIdx += $i }
        }
        if ($metaIdx.Count -le 1) { continue }  # no script-induced duplication

        # parse each metadata block
        $parsed = foreach ($i in $metaIdx) {
            $b = $blocks[$i]
            $key = ([regex]::Match($b, '(?m)^\s*([a-z_]+_metadata):')).Groups[1].Value
            $ver = ([regex]::Match($b, '(?m)^\s*version:\s*(.+?)\s*$')).Groups[1].Value
            $dat = ([regex]::Match($b, '(?m)^\s*last_updated:\s*(.+?)\s*$')).Groups[1].Value
            $rich = ($b -match '(?m)^\s*(changes:|created:|filename:|consumers:)')
            [pscustomobject]@{ Idx = $i; Key = $key; Ver = $ver; Date = $dat; Rich = $rich; Text = $b }
        }

        $finalVer = $null; foreach ($p in $parsed) { $finalVer = Higher-Version $finalVer $p.Ver }
        $finalVer = Normalize-Version $finalVer
        $finalDate = $null; foreach ($p in $parsed) { $finalDate = Newer-Date $finalDate $p.Date }
        $finalDate = Norm-Date $finalDate

        # pick base = richest (prefer), else first
        $base = ($parsed | Where-Object Rich | Select-Object -First 1)
        if (-not $base) { $base = $parsed[0] }

        # rewrite base block: canonical key, reconciled version + last_updated
        $bt = $base.Text
        $bt = [regex]::Replace($bt, '(?m)^(\s*)[a-z_]+_metadata:', "`${1}$canon`:", 1)
        if ([regex]::IsMatch($bt, '(?m)^\s*version:')) {
            $bt = [regex]::Replace($bt, '(?m)^(\s*)version:\s*.+?\s*$', "`${1}version: `"$finalVer`"")
        }
        if ([regex]::IsMatch($bt, '(?m)^\s*last_updated:')) {
            $bt = [regex]::Replace($bt, '(?m)^(\s*)last_updated:\s*.+?\s*$', "`${1}last_updated: `"$finalDate`"")
        }
        else {
            # insert last_updated right after version line (or after key line)
            if ([regex]::IsMatch($bt, '(?m)^\s*version:')) {
                $bt = [regex]::Replace($bt, '(?m)^(\s*)version:(.+)$', "`${1}version:`$2$nl`${1}last_updated: `"$finalDate`"")
            }
            else {
                $bt = [regex]::Replace($bt, "(?m)^(\s*)$canon`:\s*$", "`${1}$canon`:$nl`${1}version: `"$finalVer`"$nl`${1}last_updated: `"$finalDate`"")
            }
        }

        # rebuild trailing region: keep non-metadata trailing comments, plus single merged base block
        $keptComments = @()
        for ($i = 0; $i -lt $blocks.Count; $i++) {
            if ($metaIdx -contains $i) { continue }
            $keptComments += $blocks[$i]
        }
        $tail = ''
        foreach ($c in $keptComments) { $tail += $nl + $nl + $c }
        $tail += $nl + $nl + $bt + $nl

        $newRaw = $rest + $tail

        $report += [pscustomobject]@{
            File      = $f.FullName.Substring($root.Length + 1)
            Blocks    = ($parsed | ForEach-Object { "$($_.Key)=$($_.Ver)" }) -join ' | '
            BaseKey   = $base.Key
            FinalKey  = $canon
            FinalVer  = $finalVer
            FinalDate = $finalDate
        }

        if ($Apply) { [System.IO.File]::WriteAllText($f.FullName, $newRaw, $utf8NoBom) }
    }
}

Write-Host ("MODE: " + $(if ($Apply) { 'APPLY' } else { 'DRY-RUN' })) -ForegroundColor Cyan
Write-Host ("Files merged: " + $report.Count) -ForegroundColor Yellow
$report | ConvertTo-Json -Depth 3 | Set-Content "$root\.copilot\temp\merge-report.json" -Encoding utf8
$report | Sort-Object File | ForEach-Object { "{0}`n    [{1}] -> {2} v{3} ({4})" -f $_.File, $_.Blocks, $_.FinalKey, $_.FinalVer, $_.FinalDate } |
Set-Content "$root\.copilot\temp\merge-report.txt" -Encoding utf8
Write-Host "report written to .copilot/temp/merge-report.txt"
