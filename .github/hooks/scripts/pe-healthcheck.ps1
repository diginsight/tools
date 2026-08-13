<#
.SYNOPSIS
    PE composite healthcheck — runs all deterministic PE validation checks.

.DESCRIPTION
    Orchestrates all PE deterministic check scripts and produces a unified report.
    Designed to run standalone (human-readable output) or as a hook companion
    (JSON output when -AsJson is specified).

    Checks run:
    1. Reference integrity (📖 references resolve to existing files)
    2. YAML frontmatter validation (required fields per artifact type)
    3. Tool alignment (plan-mode agents have no write tools)
    4. Boundary count (3/1/2 minimums for agent boundary sections)

    Note: Token budget and staleness checks remain in pe-staleness-check.ps1
    (SessionStart hook) to avoid duplication. This script focuses on structural
    integrity checks not covered by the staleness hook.

.PARAMETER WorkspaceRoot
    Root of the workspace. Defaults to 3 levels up from script location.

.PARAMETER AsJson
    Output JSON (for hook integration) instead of human-readable text.

.PARAMETER AsHookResponse
    Output hook I/O protocol JSON (reads stdin, writes systemMessage).
    Mutually exclusive with -AsJson.

.EXAMPLE
    # Human-readable report
    .\pe-healthcheck.ps1

    # JSON output for programmatic use
    .\pe-healthcheck.ps1 -AsJson

    # Hook companion mode (reads stdin, writes hook response JSON)
    echo '{}' | .\pe-healthcheck.ps1 -AsHookResponse
#>
param(
    [string]$WorkspaceRoot,
    [switch]$AsJson,
    [switch]$AsHookResponse
)

if ($AsHookResponse) {
    $null = [Console]::In.ReadToEnd()
}

if (-not $WorkspaceRoot) {
    $WorkspaceRoot = Resolve-Path (Join-Path $PSScriptRoot (Join-Path ".." (Join-Path ".." "..")))
}

$scriptDir = $PSScriptRoot

# --- Run all checks ---
$results = @{
    references    = @()
    yaml          = @()
    toolAlignment = @()
    boundaries    = @()
}

$checkScripts = @{
    references    = "pe-check-references.ps1"
    yaml          = "pe-check-yaml.ps1"
    toolAlignment = "pe-check-tool-alignment.ps1"
    boundaries    = "pe-check-boundaries.ps1"
}

foreach ($check in $checkScripts.GetEnumerator()) {
    $scriptPath = Join-Path $scriptDir $check.Value
    if (Test-Path $scriptPath) {
        try {
            $output = & $scriptPath -WorkspaceRoot $WorkspaceRoot
            if ($output) {
                $results[$check.Key] = @($output)
            }
        }
        catch {
            $results[$check.Key] = @([PSCustomObject]@{
                    file     = "(script error)"
                    issue    = "Check script failed: $($_.Exception.Message)"
                    severity = "ERROR"
                })
        }
    }
    else {
        $results[$check.Key] = @([PSCustomObject]@{
                file     = "(missing)"
                issue    = "Check script not found: $($check.Value)"
                severity = "WARN"
            })
    }
}

# --- Count totals ---
$errorCount = 0
$warnCount = 0
foreach ($check in $results.Values) {
    foreach ($item in $check) {
        if ($item.severity -eq "ERROR") { $errorCount++ }
        elseif ($item.severity -eq "WARN") { $warnCount++ }
        else { $errorCount++ }  # References don't have severity field — count as errors
    }
}

# Adjust: reference violations don't have a severity property — they're all errors
$refCount = $results.references.Count
$errorCount = $errorCount - $refCount  # Remove double-count from the else branch
$errorCount += $refCount               # Add back as errors

$totalIssues = $errorCount + $warnCount

# --- Output ---
if ($AsJson) {
    $jsonResult = @{
        timestamp  = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
        summary    = @{
            errors   = $errorCount
            warnings = $warnCount
            total    = $totalIssues
            checks   = @{
                references    = $results.references.Count
                yaml          = $results.yaml.Count
                toolAlignment = $results.toolAlignment.Count
                boundaries    = $results.boundaries.Count
            }
        }
        violations = @{
            references    = @($results.references | ForEach-Object {
                    @{ sourceFile = $_.sourceFile; lineNumber = $_.lineNumber; referencedPath = $_.referencedPath }
                })
            yaml          = @($results.yaml | ForEach-Object {
                    @{ file = $_.file; field = $_.field; issue = $_.issue; severity = $_.severity }
                })
            toolAlignment = @($results.toolAlignment | ForEach-Object {
                    @{ file = $_.file; mode = $_.mode; issue = $_.issue; severity = $_.severity }
                })
            boundaries    = @($results.boundaries | ForEach-Object {
                    @{ file = $_.file; section = $_.section; issue = $_.issue; severity = $_.severity }
                })
        }
    }
    $jsonResult | ConvertTo-Json -Depth 5 -Compress
    return
}

if ($AsHookResponse) {
    $response = @{
        continue   = $true
        stopReason = ""
    }

    if ($totalIssues -gt 0) {
        $messageParts = @()

        if ($results.references.Count -gt 0) {
            $fileList = ($results.references | ForEach-Object { "  - $($_.sourceFile):$($_.lineNumber) -> $($_.referencedPath)" }) -join "`n"
            $messageParts += "PE REFERENCE INTEGRITY: $($results.references.Count) broken reference(s):`n$fileList"
        }

        if ($results.yaml.Count -gt 0) {
            $errors = $results.yaml | Where-Object { $_.severity -eq "ERROR" }
            if ($errors) {
                $fileList = ($errors | ForEach-Object { "  - $($_.file): $($_.field) - $($_.issue)" }) -join "`n"
                $messageParts += "PE YAML VALIDATION: $($errors.Count) frontmatter error(s):`n$fileList"
            }
        }

        if ($results.toolAlignment.Count -gt 0) {
            $errors = $results.toolAlignment | Where-Object { $_.severity -eq "ERROR" }
            if ($errors) {
                $fileList = ($errors | ForEach-Object { "  - $($_.file): $($_.issue)" }) -join "`n"
                $messageParts += "PE TOOL ALIGNMENT: $($errors.Count) tool/mode violation(s):`n$fileList"
            }
        }

        if ($results.boundaries.Count -gt 0) {
            $fileList = ($results.boundaries | ForEach-Object { "  - $($_.file): $($_.issue)" }) -join "`n"
            $messageParts += "PE BOUNDARY VALIDATION: $($results.boundaries.Count) boundary violation(s):`n$fileList"
        }

        if ($messageParts.Count -gt 0) {
            $response.systemMessage = $messageParts -join "`n`n"
        }
    }

    $response | ConvertTo-Json -Compress
    return
}

# --- Human-readable output ---
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  PE HEALTHCHECK REPORT — $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# References
Write-Host "─── 📖 Reference Integrity ───" -ForegroundColor Yellow
if ($results.references.Count -eq 0) {
    Write-Host "  ✅ All references resolve to existing files" -ForegroundColor Green
}
else {
    Write-Host "  ❌ $($results.references.Count) broken reference(s):" -ForegroundColor Red
    foreach ($v in $results.references) {
        Write-Host "     $($v.sourceFile):$($v.lineNumber)" -ForegroundColor White -NoNewline
        Write-Host " -> " -ForegroundColor DarkGray -NoNewline
        Write-Host "$($v.referencedPath)" -ForegroundColor Red
    }
}
Write-Host ""

# YAML
Write-Host "─── 📋 YAML Frontmatter ───" -ForegroundColor Yellow
if ($results.yaml.Count -eq 0) {
    Write-Host "  ✅ All frontmatter valid" -ForegroundColor Green
}
else {
    $yamlErrors = $results.yaml | Where-Object { $_.severity -eq "ERROR" }
    $yamlWarns = $results.yaml | Where-Object { $_.severity -eq "WARN" }
    if ($yamlErrors) {
        Write-Host "  ❌ $($yamlErrors.Count) error(s):" -ForegroundColor Red
        foreach ($v in $yamlErrors) {
            Write-Host "     $($v.file)" -ForegroundColor White -NoNewline
            Write-Host " [$($v.field)] " -ForegroundColor DarkGray -NoNewline
            Write-Host "$($v.issue)" -ForegroundColor Red
        }
    }
    if ($yamlWarns) {
        Write-Host "  ⚠️  $($yamlWarns.Count) warning(s):" -ForegroundColor Yellow
        foreach ($v in $yamlWarns) {
            Write-Host "     $($v.file)" -ForegroundColor White -NoNewline
            Write-Host " [$($v.field)] " -ForegroundColor DarkGray -NoNewline
            Write-Host "$($v.issue)" -ForegroundColor Yellow
        }
    }
}
Write-Host ""

# Tool alignment
Write-Host "─── 🔧 Tool Alignment ───" -ForegroundColor Yellow
if ($results.toolAlignment.Count -eq 0) {
    Write-Host "  ✅ All agent modes align with tool sets" -ForegroundColor Green
}
else {
    $toolErrors = $results.toolAlignment | Where-Object { $_.severity -eq "ERROR" }
    $toolWarns = $results.toolAlignment | Where-Object { $_.severity -eq "WARN" }
    if ($toolErrors) {
        Write-Host "  ❌ $($toolErrors.Count) error(s):" -ForegroundColor Red
        foreach ($v in $toolErrors) {
            Write-Host "     $($v.file)" -ForegroundColor White -NoNewline
            Write-Host " [$($v.mode)] " -ForegroundColor DarkGray -NoNewline
            Write-Host "$($v.issue)" -ForegroundColor Red
        }
    }
    if ($toolWarns) {
        Write-Host "  ⚠️  $($toolWarns.Count) warning(s):" -ForegroundColor Yellow
        foreach ($v in $toolWarns) {
            Write-Host "     $($v.file)" -ForegroundColor White -NoNewline
            Write-Host " [$($v.mode)] " -ForegroundColor DarkGray -NoNewline
            Write-Host "$($v.issue)" -ForegroundColor Yellow
        }
    }
}
Write-Host ""

# Boundaries
Write-Host "─── 🛡️ Boundary Validation ───" -ForegroundColor Yellow
if ($results.boundaries.Count -eq 0) {
    Write-Host "  ✅ All agents meet boundary minimums (3/1/2)" -ForegroundColor Green
}
else {
    Write-Host "  ❌ $($results.boundaries.Count) violation(s):" -ForegroundColor Red
    foreach ($v in $results.boundaries) {
        Write-Host "     $($v.file)" -ForegroundColor White -NoNewline
        Write-Host " " -NoNewline
        Write-Host "$($v.issue)" -ForegroundColor Red
    }
}
Write-Host ""

# Summary
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
if ($totalIssues -eq 0) {
    Write-Host "  ✅ ALL CHECKS PASSED — 0 issues found" -ForegroundColor Green
}
else {
    $color = if ($errorCount -gt 0) { "Red" } else { "Yellow" }
    Write-Host "  SUMMARY: $errorCount error(s), $warnCount warning(s)" -ForegroundColor $color
}
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
