# Consistency stress: repeated add/remove churn, asserting after every step that
#   root cell == sum(root section cells) + root-level standalone articles
# and that every folder cell agrees with the level the nav API serves for its parent.

$ErrorActionPreference = 'Stop'
$base = 'https://localhost:7280'
$pass = 0
$fail = 0

function Get-Cells {
    ((Invoke-WebRequest -Uri "$base/_nav/metrics" -SkipCertificateCheck).Content | ConvertFrom-Json -AsHashtable).cells
}

function Wait-Converged([int]$timeoutSec = 240) {
    # Converged means: the site-root cell is Complete, nothing is dirty, AND the cell count has
    # stopped growing. Without the stability check a cold, nearly-empty index trivially qualifies.
    $deadline = (Get-Date).AddSeconds($timeoutSec)
    $prev = -1
    $stable = 0
    while ((Get-Date) -lt $deadline) {
        $c = Get-Cells
        $ok = $c.ContainsKey('') -and $c[''].coverage -eq 2 -and
        @($c.Keys | Where-Object { $c[$_].isDirty }).Count -eq 0
        if ($ok -and $c.Count -eq $prev) { $stable++ } else { $stable = 0 }
        $prev = $c.Count
        if ($ok -and $stable -ge 2) { return $c }
        Start-Sleep -Milliseconds 700
    }
    throw "index did not converge within $timeoutSec s"
}

function Check($label, $actual, $expected) {
    # Write-Host, not pipeline output: these functions also return the cell table.
    if ($actual -eq $expected) { $script:pass++; Write-Host "  PASS  $label  ($actual)" }
    else { $script:fail++; Write-Host "  FAIL  $label  expected=$expected actual=$actual" }
}

# The site root cell must equal the sum of its direct children: section cells + standalone articles.
function Assert-RootConsistent($label) {
    $c = Wait-Converged
    $lvl = Invoke-RestMethod -Uri "$base/_nav/children?prefix=" -SkipCertificateCheck
    $sections = @($lvl | Where-Object { $_.prefix })
    $standalone = @($lvl | Where-Object { -not $_.prefix -and $_.route }).Count
    $sum = ($sections | ForEach-Object { $c[$_.prefix].count } | Measure-Object -Sum).Sum + $standalone
    Check "$label - root == sum(children)" $c[''].count $sum

    # Every section node served in the root level must carry the same number as its own cell.
    $mismatch = @($sections | Where-Object { $_.articleCount -ne $c[$_.prefix].count }).Count
    Check "$label - level payload == cells" $mismatch 0
    return $c
}

"=== Baseline ==="
$c = Assert-RootConsistent 'baseline'
$root0 = $c[''].count
$issues0 = $c['05.00-issues'].count
"  root=$root0  05.00-issues=$issues0"

"`n=== 10 add/remove cycles, checking consistency after every add and every remove ==="
for ($i = 1; $i -le 10; $i++) {
    $null = Invoke-RestMethod -Method Post -Uri "$base/_test/article?folder=05.00-issues&name=zz-churn-$i" -SkipCertificateCheck
    $c = Assert-RootConsistent "cycle $i add"
    Check "cycle $i add - issues +1" $c['05.00-issues'].count ($issues0 + 1)
    Check "cycle $i add - root +1" $c[''].count ($root0 + 1)

    $null = Invoke-RestMethod -Method Delete -Uri "$base/_test/article?folder=05.00-issues&name=zz-churn-$i" -SkipCertificateCheck
    $c = Assert-RootConsistent "cycle $i del"
    Check "cycle $i del - issues back" $c['05.00-issues'].count $issues0
    Check "cycle $i del - root back" $c[''].count $root0
}

"`n=== Deep nesting: add under a 3-level path ==="
$deep = '02.00-events/202606-build-2026'
$c = Wait-Converged
$deep0 = $c[$deep].count
$null = Invoke-RestMethod -Method Post -Uri "$base/_test/article?folder=$deep&name=zz-deep" -SkipCertificateCheck
$c = Assert-RootConsistent 'deep add'
Check "deep add - node +1" $c[$deep].count ($deep0 + 1)
Check "deep add - root +1" $c[''].count ($root0 + 1)
$null = Invoke-RestMethod -Method Delete -Uri "$base/_test/article?folder=$deep&name=zz-deep" -SkipCertificateCheck
$c = Assert-RootConsistent 'deep del'
Check "deep del - node back" $c[$deep].count $deep0
Check "deep del - root back" $c[''].count $root0

"`n=== Burst: 5 adds across 2 folders in one window, then 5 removes ==="
1..3 | ForEach-Object { $null = Invoke-RestMethod -Method Post -Uri "$base/_test/article?folder=05.00-issues&name=zz-burst-$_" -SkipCertificateCheck }
1..2 | ForEach-Object { $null = Invoke-RestMethod -Method Post -Uri "$base/_test/article?folder=04.00-howto&name=zz-burst-$_" -SkipCertificateCheck }
$c = Assert-RootConsistent 'burst add'
Check "burst - root +5" $c[''].count ($root0 + 5)
1..3 | ForEach-Object { $null = Invoke-RestMethod -Method Delete -Uri "$base/_test/article?folder=05.00-issues&name=zz-burst-$_" -SkipCertificateCheck }
1..2 | ForEach-Object { $null = Invoke-RestMethod -Method Delete -Uri "$base/_test/article?folder=04.00-howto&name=zz-burst-$_" -SkipCertificateCheck }
$c = Assert-RootConsistent 'burst del'
Check "burst - root back" $c[''].count $root0

"`n================================"
"PASS: $pass   FAIL: $fail"
if ($fail -gt 0) { exit 1 }
