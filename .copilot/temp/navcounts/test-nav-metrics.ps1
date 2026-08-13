# Navigation metrics pipeline test: baseline convergence, add-article, remove-article.
# Run against a locally running Learn.Web (https://localhost:7280) with Testing:ContentMutationEnabled=true.

$ErrorActionPreference = 'Stop'
$base = 'https://localhost:7280'
$folder = '05.00-issues'
$name = 'zz-nav-count-probe'
$pass = 0
$fail = 0

function Get-Cells {
    $raw = (Invoke-WebRequest -Uri "$base/_nav/metrics" -SkipCertificateCheck).Content
    ($raw | ConvertFrom-Json -AsHashtable).cells
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
        Start-Sleep -Milliseconds 800
    }
    throw "index did not converge within $timeoutSec s"
}

function Check($label, $actual, $expected) {
    # Write-Host, not pipeline output: keeps function return values clean.
    if ($actual -eq $expected) { $script:pass++; Write-Host "  PASS  $label  ($actual)" }
    else { $script:fail++; Write-Host "  FAIL  $label  expected=$expected actual=$actual" }
}

"=== 1. Baseline: index converges, every cell Complete ==="
$c = Wait-Converged
$notComplete = @($c.Keys | Where-Object { $c[$_].coverage -ne 2 })
Check "all cells settled" (@($c.Keys | Where-Object { $c[$_].isDirty }).Count) 0
Check "all cells Complete" $notComplete.Count 0
$rootBefore = $c[''].count
$issuesBefore = $c[$folder].count
"  root total = $rootBefore ; $folder = $issuesBefore"

"`n=== 2. Root level served over the nav API matches the index ==="
$level = Invoke-RestMethod -Uri "$base/_nav/children?prefix=" -SkipCertificateCheck
foreach ($n in $level | Where-Object { $_.prefix }) {
    Check "level[$($n.prefix)] == index" $n.articleCount $c[$n.prefix].count
    Check "level[$($n.prefix)] coverage Complete" $n.countCoverage 2
}

"`n=== 3. Add an article -> counts increment up the whole spine ==="
$null = Invoke-RestMethod -Method Post -Uri "$base/_test/article?folder=$folder&name=$name" -SkipCertificateCheck
$c = Wait-Converged
Check "$folder count +1" $c[$folder].count ($issuesBefore + 1)
Check "root count +1" $c[''].count ($rootBefore + 1)
Check "$folder still Complete" $c[$folder].coverage 2
Check "root still Complete" $c[''].coverage 2
$level = Invoke-RestMethod -Uri "$base/_nav/children?prefix=" -SkipCertificateCheck
$issuesNode = $level | Where-Object { $_.prefix -eq $folder }
Check "nav API reflects new count" $issuesNode.articleCount ($issuesBefore + 1)

"`n=== 4. Remove the article -> counts return to baseline ==="
$null = Invoke-RestMethod -Method Delete -Uri "$base/_test/article?folder=$folder&name=$name" -SkipCertificateCheck
$c = Wait-Converged
Check "$folder back to baseline" $c[$folder].count $issuesBefore
Check "root back to baseline" $c[''].count $rootBefore
Check "$folder still Complete" $c[$folder].coverage 2

"`n=== 5. Repeat add/remove x3 -> idempotent, no drift ==="
for ($i = 1; $i -le 3; $i++) {
    $null = Invoke-RestMethod -Method Post -Uri "$base/_test/article?folder=$folder&name=$name-$i" -SkipCertificateCheck
}
$c = Wait-Converged
Check "root +3" $c[''].count ($rootBefore + 3)
for ($i = 1; $i -le 3; $i++) {
    $null = Invoke-RestMethod -Method Delete -Uri "$base/_test/article?folder=$folder&name=$name-$i" -SkipCertificateCheck
}
$c = Wait-Converged
Check "root back to baseline after burst" $c[''].count $rootBefore
Check "no cell left dirty" (@($c.Keys | Where-Object { $c[$_].isDirty }).Count) 0

"`n================================"
"PASS: $pass   FAIL: $fail"
if ($fail -gt 0) { exit 1 }
