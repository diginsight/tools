$root = "c:\dev\darioairoldi\Learn"
Write-Host "=== PHASE 4R: CONTENT AUDIT (01.00-article-writing scope) ==="
Write-Host ""

$files = @()
$files += Get-ChildItem "$root\.copilot\context\01.00-article-writing\*.md"
$files += Get-ChildItem "$root\.copilot\context\01.00-article-writing\workflows\*.md"
$files += Get-ChildItem "$root\.github\agents\01.00-article-writing\*.agent.md"
$files += Get-ChildItem "$root\.github\prompts\01.00-article-writing\*.prompt.md"

Write-Host "--- Tier 1: Deterministic Structural Checks ---"
Write-Host ""

foreach ($f in $files) {
  $content = Get-Content $f.FullName -Raw
  $name = $f.Name
  $lineCount = (Get-Content $f.FullName).Count
  $tokenEst = $lineCount * 6
  
  # Check 1: All 5 required metadata fields present
  $metaFields = @("goal:", "scope:", "boundaries:", "rationales:", "version:")
  $metaMissing = $metaFields | Where-Object { $content -notmatch "(?m)^$_" }
  $metaOk = $metaMissing.Count -eq 0
  
  # Check 2: scope.covers has at least 2 items
  $scopeCovers = ([regex]::Matches($content, '(?m)^\s+-\s+"[^"]*"')).Count
  $scopeOk = $scopeCovers -ge 2
  
  # Check 3: version is valid SemVer
  $versionMatch = [regex]::Match($content, '(?m)^version:\s*"(\d+\.\d+\.\d+)"')
  $versionOk = $versionMatch.Success
  $versionVal = if ($versionMatch.Success) { $versionMatch.Groups[1].Value } else { "MISSING" }
  
  # Check 4: Token budget (context files: <=2500 tokens, agents/prompts: <=2500 tokens as practical limit)
  $isContext = $f.FullName -match "\.copilot\\context"
  $budgetLimit = if ($isContext) { 2500 } else { 2500 }
  $budgetOk = $tokenEst -le $budgetLimit
  
  # Overall
  $status = if ($metaOk -and $scopeOk -and $versionOk -and $budgetOk) { "PASS" } else { "FINDING" }
  $issues = @()
  if (-not $metaOk) { $issues += "missing: $($metaMissing -join ',')" }
  if (-not $scopeOk) { $issues += "scope.covers < 2 items" }
  if (-not $versionOk) { $issues += "invalid version" }
  if (-not $budgetOk) { $issues += "over budget ($tokenEst tokens)" }
  
  $issueStr = if ($issues.Count -gt 0) { " [$($issues -join '; ')]" } else { "" }
  Write-Host "$status | $name | v$versionVal | $lineCount lines (~$tokenEst tok)$issueStr"
}

Write-Host ""
Write-Host "--- Summary ---"
$passCount = ($files | Where-Object {
  $c = Get-Content $_.FullName -Raw
  $lineCount = (Get-Content $_.FullName).Count
  $metaOk = @("goal:", "scope:", "boundaries:", "rationales:", "version:") | Where-Object { $c -notmatch "(?m)^$_" }
  $metaOk.Count -eq 0 -and ($lineCount * 6) -le 2500
}).Count
Write-Host "PASS: $passCount / $($files.Count)"
