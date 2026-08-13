$ErrorActionPreference = 'Stop'
$root = 'C:\dev\darioa\darioairoldi\Learn'
Set-Location $root
$utf8 = New-Object System.Text.UTF8Encoding $false

$flat   = Join-Path $root '06.00-idea\learning-hub\00-learning-hub.md'
$dir    = Join-Path $root '06.00-idea\learning-hub\00-learning-hub'
$nested = Join-Path $dir  '00-learning-hub.md'

if (-not (Test-Path $flat)) { throw "flat master not found at $flat" }
if (Test-Path $nested)      { throw "nested target already exists at $nested" }

# 1) Move flat -> 00-learning-hub/00-learning-hub.md
New-Item -ItemType Directory -Force -Path $dir | Out-Null
Move-Item $flat $nested -Force
Write-Host "1) moved -> $nested"

# 2) The master moved one level deeper: fix its own relative links.
#    - every  ](../   gains a level  -> ](../../
#    - siblings inside learning-hub gain a  ../  (they were relative to learning-hub/)
$m = [System.IO.File]::ReadAllText($nested)
$m = $m -replace '\]\(\.\./', '](../../'
$m = $m -replace '\]\(04-platform-and-consumers', '](../04-platform-and-consumers'
$m = $m -replace '\]\(01-learning-hub-overview/',  '](../01-learning-hub-overview/'
$m = $m -replace '\]\(02-documentation-taxonomy/',  '](../02-documentation-taxonomy/'
$m = $m -replace '\]\(03-automated-content-lifecycle/', '](../03-automated-content-lifecycle/'
[System.IO.File]::WriteAllText($nested, $m, $utf8)
Write-Host "2) fixed master outbound links"

function Fix($rel, $from, $to) {
  $p = Join-Path $root $rel
  if (-not (Test-Path $p)) { Write-Host "   SKIP (missing) $rel"; return }
  $t = [System.IO.File]::ReadAllText($p)
  if ($t.Contains($from)) {
    $n = ($t.Length - $t.Replace($from,'').Length) / $from.Length
    [System.IO.File]::WriteAllText($p, $t.Replace($from,$to), $utf8)
    Write-Host "   fixed x$n  $rel"
  } else { Write-Host "   no-match  $rel" }
}

# 3a) siblings under 06.00-idea/ and the src/docs plans:
#     learning-hub/00-learning-hub.md -> learning-hub/00-learning-hub/00-learning-hub.md
$g = 'learning-hub/00-learning-hub.md'
$gN = 'learning-hub/00-learning-hub/00-learning-hub.md'
@(
  '06.00-idea\autonomous-streams\autonomous-streams.md',
  '06.00-idea\iqpilot\01-iqpilot-overview.md',
  '06.00-idea\own-your-learning-loop\01-own-your-learning-loop-overview.md',
  '06.00-idea\self-updating-engine\00-one-engine-many-streams.md',
  '06.00-idea\tuneiq\01-tuneiq-design.md',
  'src\docs\90. Issues\202607\20270720.01-learninghub-stratreview\00.01-learning-hub-improvements-mount-plan.md',
  'src\docs\90. Issues\202607\20270720.01-learninghub-stratreview\overview.md'
) | ForEach-Object { Fix $_ $g $gN }

# 3b) within learning-hub/01-learning-hub-overview/: ../00-learning-hub.md -> ../00-learning-hub/00-learning-hub.md
Fix '06.00-idea\learning-hub\01-learning-hub-overview\01-learning-hub-introduction.md' '](../00-learning-hub.md)' '](../00-learning-hub/00-learning-hub.md)'
Fix '06.00-idea\learning-hub\01-learning-hub-overview\02-using-learning-hub-for-learning-technologies.md' '](../00-learning-hub.md)' '](../00-learning-hub/00-learning-hub.md)'

# 3c) platform (sibling flat file inside learning-hub): ](00-learning-hub.md) -> ](00-learning-hub/00-learning-hub.md)
Fix '06.00-idea\learning-hub\04-platform-and-consumers.md' '](00-learning-hub.md)' '](00-learning-hub/00-learning-hub.md)'

Write-Host "DONE"
