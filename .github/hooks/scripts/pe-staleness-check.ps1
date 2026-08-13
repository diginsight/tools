<#
.SYNOPSIS
    PE staleness check — SessionStart hook companion script.
    Scans .copilot/context/ files for stale last_updated dates, detects
    empty/stub files, checks token budget compliance, and checks
    the meta-review log for overdue reviews.

.DESCRIPTION
    Called by .github/hooks/pe-staleness-check.json on every session start.
    Reads JSON from stdin (hook I/O protocol), writes JSON to stdout.

    Checks:
    - Empty/stub files: context files < 500 bytes
    - Context file staleness: 90 days since last_updated
    - Template file staleness: 90 days since last_updated (from bottom template_metadata)
    - Token budget violations: context files > 375 lines, instruction files > 220 lines,
      agent files > 375 lines, simple prompts > 220 lines, orchestrator prompts > 375 lines,
      meta prompts > 450 lines (three-tier model from 01.06-system-parameters.md)
    - Meta-review overdue: 30 days since last review log update
#>

# Read stdin (hook input JSON) — required by I/O protocol but not used here
$null = [Console]::In.ReadToEnd()

$contextDir = Join-Path (Join-Path (Join-Path (Join-Path $PSScriptRoot "..") "..") "..") (Join-Path ".copilot" "context")
$resolved = Resolve-Path $contextDir -ErrorAction SilentlyContinue
$contextDir = if ($resolved) { $resolved.Path } else { $null }

$stalenessThresholdDays = 90
$metaReviewThresholdDays = 30
$today = Get-Date

$staleFiles = @()
$warnings = @()
$stubFiles = @()

# --- Check for empty or stub files (< 500 bytes) ---
if ($contextDir -and (Test-Path $contextDir)) {
    $allMdFiles = Get-ChildItem -Path $contextDir -Filter "*.md" -Recurse

    foreach ($file in $allMdFiles) {
        if ($file.Length -lt 500) {
            $relativePath = $file.FullName.Substring($contextDir.Length + 1) -replace '\\', '/'
            $stubFiles += @{
                file  = $relativePath
                bytes = $file.Length
            }
        }
    }
}

# --- Check context file last_updated dates ---
if ($contextDir -and (Test-Path $contextDir)) {
    $mdFiles = Get-ChildItem -Path $contextDir -Filter "*.md" -Recurse

    foreach ($file in $mdFiles) {
        $content = Get-Content -Path $file.FullName -TotalCount 20 -ErrorAction SilentlyContinue
        if (-not $content) { continue }

        # Extract last_updated from YAML frontmatter
        $inFrontmatter = $false
        $lastUpdated = $null

        foreach ($line in $content) {
            if ($line -match '^\s*---\s*$') {
                if ($inFrontmatter) { break }
                $inFrontmatter = $true
                continue
            }
            if ($inFrontmatter -and $line -match '^\s*last_updated:\s*"?(\d{4}-\d{2}-\d{2})"?\s*$') {
                $lastUpdated = $Matches[1]
                break
            }
        }

        if ($lastUpdated) {
            try {
                $updateDate = [datetime]::ParseExact($lastUpdated, "yyyy-MM-dd", $null)
                $ageDays = ($today - $updateDate).Days

                if ($ageDays -gt $stalenessThresholdDays) {
                    $relativePath = $file.FullName.Substring($contextDir.Length + 1) -replace '\\', '/'
                    $staleFiles += @{
                        file        = $relativePath
                        lastUpdated = $lastUpdated
                        ageDays     = $ageDays
                    }
                }
            }
            catch {
                # Skip files with unparseable dates
            }
        }
    }
}

# --- Token budget checks (lines → approximate tokens at 6.67 tokens/line) ---
$budgetViolations = @()

# Budget thresholds: artifact type → max lines (from 01.06-system-parameters.md)
$budgets = @{
    "context"             = 375   # 2,500 tokens
    "instruction"         = 220   # 1,500 tokens
    "agent"               = 375   # 2,500 tokens
    "prompt_simple"       = 220   # 1,500 tokens
    "prompt_orchestrator" = 375   # 2,500 tokens
    "prompt_meta"         = 450   # 3,000 tokens
}

# Check context files
if ($contextDir -and (Test-Path $contextDir)) {
    $budgetFiles = Get-ChildItem -Path $contextDir -Filter "*.md" -Recurse
    foreach ($file in $budgetFiles) {
        $lineCount = (Get-Content -Path $file.FullName -ErrorAction SilentlyContinue | Measure-Object).Count
        if ($lineCount -gt $budgets["context"]) {
            $relativePath = $file.FullName.Substring($contextDir.Length + 1) -replace '\\', '/'
            $budgetViolations += @{
                file   = $relativePath
                type   = "context"
                lines  = $lineCount
                budget = $budgets["context"]
            }
        }
    }
}

# Check instruction files
$workspaceRoot = Resolve-Path (Join-Path $PSScriptRoot (Join-Path ".." (Join-Path ".." "..")))
$instructionDir = Join-Path (Join-Path $workspaceRoot ".github") "instructions"
if (Test-Path $instructionDir) {
    $instrFiles = Get-ChildItem -Path $instructionDir -Filter "*.instructions.md"
    foreach ($file in $instrFiles) {
        $lineCount = (Get-Content -Path $file.FullName -ErrorAction SilentlyContinue | Measure-Object).Count
        if ($lineCount -gt $budgets["instruction"]) {
            $relativePath = ".github/instructions/$($file.Name)"
            $budgetViolations += @{
                file   = $relativePath
                type   = "instruction"
                lines  = $lineCount
                budget = $budgets["instruction"]
            }
        }
    }
}

# Check agent files
$agentDir = Join-Path (Join-Path $workspaceRoot ".github") "agents"
if (Test-Path $agentDir) {
    $agentFiles = Get-ChildItem -Path $agentDir -Filter "*.agent.md" -Recurse
    foreach ($file in $agentFiles) {
        $lineCount = (Get-Content -Path $file.FullName -ErrorAction SilentlyContinue | Measure-Object).Count
        if ($lineCount -gt $budgets["agent"]) {
            $relativePath = $file.FullName.Substring($agentDir.Length + 1) -replace '\\', '/'
            $budgetViolations += @{
                file   = ".github/agents/$relativePath"
                type   = "agent"
                lines  = $lineCount
                budget = $budgets["agent"]
            }
        }
    }
}

# Check prompt files (three-tier: meta > orchestrator > simple)
$promptDir = Join-Path (Join-Path $workspaceRoot ".github") "prompts"
if (Test-Path $promptDir) {
    $promptFiles = Get-ChildItem -Path $promptDir -Filter "*.prompt.md" -Recurse
    foreach ($file in $promptFiles) {
        # Skip deprecated prompts in old/ subfolders
        if ($file.FullName -match '[/\\]old[/\\]') { continue }
        $lineCount = (Get-Content -Path $file.FullName -ErrorAction SilentlyContinue | Measure-Object).Count
        # Classify prompt tier: meta > orchestrator > simple
        if ($file.Name -match '^meta-prompt-engineering-') {
            $tier = "prompt_meta"
            $tierLabel = "meta prompt"
        }
        elseif ($file.Name -match '-design\.prompt\.md$') {
            $tier = "prompt_orchestrator"
            $tierLabel = "orchestrator prompt"
        }
        else {
            $tier = "prompt_simple"
            $tierLabel = "simple prompt"
        }
        if ($lineCount -gt $budgets[$tier]) {
            $relativePath = $file.FullName.Substring($promptDir.Length + 1) -replace '\\', '/'
            $budgetViolations += @{
                file   = ".github/prompts/$relativePath"
                type   = $tierLabel
                lines  = $lineCount
                budget = $budgets[$tier]
            }
        }
    }
}

# --- Check meta-review log ---
$metaReviewLog = Join-Path (Join-Path (Join-Path (Join-Path (Join-Path $PSScriptRoot "..") "..") "..") (Join-Path ".copilot" "context")) (Join-Path "00.00-prompt-engineering" "05.04-meta-review-log.md")
$metaReviewLog = if (Test-Path $metaReviewLog) { (Resolve-Path $metaReviewLog).Path } else { $null }

# --- Check template file staleness (bottom HTML comment metadata) ---
$staleTemplates = @()
$templateDir = Join-Path (Join-Path $workspaceRoot ".github") "templates"
if (Test-Path $templateDir) {
    $templateFiles = Get-ChildItem -Path $templateDir -Filter "*.template.md" -Recurse
    foreach ($file in $templateFiles) {
        $content = Get-Content -Path $file.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $content) { continue }

        # Parse last_updated from bottom template_metadata HTML comment
        $templateLastUpdated = $null
        if ($content -match '<!--[\s\S]*?template_metadata:[\s\S]*?last_updated:\s*"?(\d{4}-\d{2}-\d{2})"?[\s\S]*?-->') {
            $templateLastUpdated = $Matches[1]
        }

        if ($templateLastUpdated) {
            try {
                $updateDate = [datetime]::ParseExact($templateLastUpdated, "yyyy-MM-dd", $null)
                $ageDays = ($today - $updateDate).Days
                if ($ageDays -gt $stalenessThresholdDays) {
                    $relativePath = $file.FullName.Substring($templateDir.Length + 1) -replace '\\', '/'
                    $staleTemplates += @{
                        file        = ".github/templates/$relativePath"
                        lastUpdated = $templateLastUpdated
                        ageDays     = $ageDays
                    }
                }
            }
            catch { }
        }
    }
}

$metaReviewOverdue = $false
if ($metaReviewLog) {
    $logContent = Get-Content -Path $metaReviewLog -TotalCount 20 -ErrorAction SilentlyContinue
    $inFrontmatter = $false
    $logLastUpdated = $null

    foreach ($line in $logContent) {
        if ($line -match '^\s*---\s*$') {
            if ($inFrontmatter) { break }
            $inFrontmatter = $true
            continue
        }
        if ($inFrontmatter -and $line -match '^\s*last_updated:\s*"?(\d{4}-\d{2}-\d{2})"?\s*$') {
            $logLastUpdated = $Matches[1]
            break
        }
    }

    if ($logLastUpdated) {
        try {
            $logDate = [datetime]::ParseExact($logLastUpdated, "yyyy-MM-dd", $null)
            $logAgeDays = ($today - $logDate).Days
            if ($logAgeDays -gt $metaReviewThresholdDays) {
                $metaReviewOverdue = $true
                $warnings += "Meta-review log last updated $logAgeDays days ago ($logLastUpdated). Threshold: $metaReviewThresholdDays days. Run ``/pe-meta-review --mode plan --skip research`` to refresh."
            }
        }
        catch { }
    }
}
else {
    $warnings += "Meta-review log not found at .copilot/context/00.00-prompt-engineering/05.04-meta-review-log.md. Consider creating it to track review history."
}

# --- Build system message ---
$messageParts = @()

if ($stubFiles.Count -gt 0) {
    $fileList = ($stubFiles | ForEach-Object { "  - $($_.file) ($($_.bytes) bytes)" }) -join "`n"
    $messageParts += "PE STUB FILE ALERT: $($stubFiles.Count) context file(s) are empty or stubs (< 500 bytes):`n$fileList`nPopulate these files or remove them if no longer needed."
}

if ($staleFiles.Count -gt 0) {
    $fileList = ($staleFiles | ForEach-Object { "  - $($_.file) ($($_.ageDays) days, last updated $($_.lastUpdated))" }) -join "`n"
    $messageParts += "PE STALENESS ALERT: $($staleFiles.Count) context file(s) exceed the $stalenessThresholdDays-day freshness threshold:`n$fileList`nRun ``/pe-meta-review --mode plan --skip research`` on stale areas."
}

if ($staleTemplates.Count -gt 0) {
    $fileList = ($staleTemplates | ForEach-Object { "  - $($_.file) ($($_.ageDays) days, last updated $($_.lastUpdated))" }) -join "`n"
    $messageParts += "PE TEMPLATE STALENESS ALERT: $($staleTemplates.Count) template file(s) exceed the $stalenessThresholdDays-day freshness threshold:`n$fileList`nRun ``/meta-prompt-engineering-scheduled-review --scope templates`` to review."
}

if ($budgetViolations.Count -gt 0) {
    $fileList = ($budgetViolations | ForEach-Object { "  - $($_.file) ($($_.lines) lines, budget: $($_.budget) lines for $($_.type) files)" }) -join "`n"
    $messageParts += "PE TOKEN BUDGET ALERT: $($budgetViolations.Count) file(s) exceed their line budget:`n$fileList`nReduce file size or split into smaller files."
}

if ($warnings.Count -gt 0) {
    $messageParts += ($warnings -join "`n")
}

# --- Output hook response ---
$response = @{
    continue   = $true
    stopReason = ""
}

if ($messageParts.Count -gt 0) {
    $response.systemMessage = $messageParts -join "`n`n"
}

$response | ConvertTo-Json -Compress
