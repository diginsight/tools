# Back-fill a bottom {type}_metadata block (version 1.0.0 + last_updated) into version-less
# in-scope artifacts. Appends a fresh OWN-type block at EOF; never touches example blocks
# (e.g. template article_metadata output blocks). UTF-8 no-BOM, emoji-safe.

$root = 'c:\dev\darioairoldi\Learn'
$today = '2026-06-12'
$enc = New-Object System.Text.UTF8Encoding($false)

$cats = @{
    Prompts   = @{ Dir = "$root\.github\prompts"; Key = 'prompt_metadata' }
    Templates = @{ Dir = "$root\.github\templates"; Key = 'template_metadata' }
}

$report = @()

foreach ($k in $cats.Keys) {
    $dir = $cats[$k].Dir
    $key = $cats[$k].Key
    $files = Get-ChildItem -Recurse -File -Path $dir -Include *.md -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -notlike '*.changelog.md' }
    foreach ($f in $files) {
        $raw = [System.IO.File]::ReadAllText($f.FullName)

        # Skip if file already has ANY version field anywhere
        if ($raw -match '(?m)^\s*version:\s') {
            continue
        }

        # Safety: skip if the LAST html comment already carries our own key
        $lastCommentIdx = $raw.LastIndexOf('<!--')
        if ($lastCommentIdx -ge 0) {
            $lastComment = $raw.Substring($lastCommentIdx)
            if ($lastComment -match "(?m)^\s*$([regex]::Escape($key)):") {
                $report += [pscustomobject]@{ File = $f.FullName.Substring($root.Length + 1); Action = 'skip-already-own-key' }
                continue
            }
        }

        $block = "<!--`n${key}:`n  version: `"1.0.0`"`n  last_updated: `"$today`"`n-->`n"

        # Normalize trailing whitespace to exactly one newline, then blank line + block
        $trimmed = $raw.TrimEnd("`r", "`n", " ", "`t")
        $new = $trimmed + "`n`n" + $block

        [System.IO.File]::WriteAllText($f.FullName, $new, $enc)
        $report += [pscustomobject]@{ File = $f.FullName.Substring($root.Length + 1); Action = "appended-$key" }
    }
}

$report | ConvertTo-Json -Depth 4 | Set-Content "$root\.copilot\temp\backfill-report.json" -Encoding utf8
Write-Host ("Processed: " + $report.Count)
$report | Group-Object Action | ForEach-Object { Write-Host ("  " + $_.Name + ": " + $_.Count) }
$report | Sort-Object File | ForEach-Object { Write-Host ("  " + $_.Action + "  " + $_.File) }
