$root = 'c:\dev\darioairoldi\Learn'
function Get-Hist {
    param([string[]]$paths)
    $changes = New-Object System.Collections.Generic.List[string]
    $vh = New-Object System.Collections.Generic.List[string]
    foreach ($p in $paths) {
        if (-not (Test-Path $p)) { continue }
        Get-ChildItem -Recurse -File -Path $p -Include *.md -ErrorAction SilentlyContinue | ForEach-Object {
            if ($_.Name -like '*.changelog.md') { return }
            if ($_.FullName -match '\\old\\') { return }
            $c = Get-Content -Raw $_.FullName
            $lc = $c.LastIndexOf('<!--')
            $tail = if ($lc -ge 0) { $c.Substring($lc) } else { '' }
            $front = ''
            if ($c.StartsWith('---')) {
                $e = $c.IndexOf("`n---", 3)
                if ($e -ge 0) { $front = $c.Substring(0, $e) }
            }
            if ($tail -match '(?m)^\s*changes:\s*$' -or $front -match '(?m)^\s*changes:\s*$') { $changes.Add($_.FullName) }
            if ($c -match '(?m)^\s*version_history:\s*$') { $vh.Add($_.FullName) }
        }
    }
    return , @($changes, $vh)
}
$peRes = Get-Hist @("$root\.github\agents", "$root\.github\prompts", "$root\.copilot\context", "$root\.github\templates")
$artRes = Get-Hist @("$root\01.00-news", "$root\02.00-events", "$root\03.00-tech", "$root\04.00-howto", "$root\05.00-issues", "$root\06.00-idea")
"PE  changes-array files: {0}   version_history files: {1}" -f $peRes[0].Count, $peRes[1].Count
"ART changes-array files: {0}   version_history files: {1}" -f $artRes[0].Count, $artRes[1].Count
"`n--- ARTICLE version_history (active) ---"
$artRes[1] | ForEach-Object { $_.Replace($root + '\', '') }
"`n--- ARTICLE changes-array (active) ---"
$artRes[0] | ForEach-Object { $_.Replace($root + '\', '') }
