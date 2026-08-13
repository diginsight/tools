<#
.SYNOPSIS
    PE boundary count validation — verifies agent files meet 3/1/2 boundary minimums.

.DESCRIPTION
    Scans all agent files (.github/agents/**/*.agent.md) for the three-tier
    boundary section (CRITICAL BOUNDARIES) and validates:
    - "Always Do" section: minimum 3 items
    - "Ask First" section: minimum 1 item
    - "Never Do" section: minimum 2 items

    Items are counted as lines starting with `- ` or `* ` under each heading.

.PARAMETER WorkspaceRoot
    Root of the workspace. Defaults to 3 levels up from script location.

.OUTPUTS
    Array of PSCustomObject with: file, section, count, minimum, severity
#>
param(
    [string]$WorkspaceRoot
)

if (-not $WorkspaceRoot) {
    $WorkspaceRoot = Resolve-Path (Join-Path $PSScriptRoot (Join-Path ".." (Join-Path ".." "..")))
}

$violations = @()

$agentDir = Join-Path $WorkspaceRoot ".github/agents"
if (-not (Test-Path $agentDir)) { return $violations }

$agentFiles = Get-ChildItem -Path $agentDir -Filter "*.agent.md" -Recurse

foreach ($file in $agentFiles) {
    $rel = ".github/agents/" + ($file.FullName.Substring($agentDir.Length + 1) -replace '\\', '/')
    $lines = Get-Content -Path $file.FullName -Encoding UTF8 -ErrorAction SilentlyContinue
    if (-not $lines) { continue }

    $hasBoundarySection = $false
    $currentSection = $null
    $counts = @{
        "Always Do" = 0
        "Ask First" = 0
        "Never Do"  = 0
    }
    $minimums = @{
        "Always Do" = 3
        "Ask First" = 1
        "Never Do"  = 2
    }

    foreach ($line in $lines) {
        # Detect the boundary heading (with or without emoji)
        if ($line -match '##.*CRITICAL\s+BOUNDARIES' -or $line -match '##.*Boundaries') {
            $hasBoundarySection = $true
            continue
        }

        # Detect subsection headings (H3 with optional emoji)
        if ($line -match '###\s*(?:✅\s*)?Always\s+Do') {
            $currentSection = "Always Do"
            continue
        }
        if ($line -match '###\s*(?:⚠️\s*)?Ask\s+First') {
            $currentSection = "Ask First"
            continue
        }
        if ($line -match '###\s*(?:🚫\s*)?Never\s+Do') {
            $currentSection = "Never Do"
            continue
        }

        # End the current section at the next H2 or H3 that isn't a boundary tier
        if ($currentSection -and $line -match '^##[^#]' -and $line -notmatch 'CRITICAL\s+BOUNDARIES') {
            $currentSection = $null
            continue
        }
        if ($currentSection -and $line -match '^###' -and $line -notmatch 'Always\s+Do|Ask\s+First|Never\s+Do') {
            $currentSection = $null
            continue
        }

        # Count bullet items
        if ($currentSection -and $line -match '^\s*[-*]\s+\S') {
            $counts[$currentSection]++
        }
    }

    if (-not $hasBoundarySection) {
        $violations += [PSCustomObject]@{
            file     = $rel
            section  = "(missing)"
            count    = 0
            minimum  = "N/A"
            severity = "ERROR"
            issue    = "No CRITICAL BOUNDARIES section found"
        }
        continue
    }

    # Validate each boundary tier
    foreach ($section in @("Always Do", "Ask First", "Never Do")) {
        if ($counts[$section] -lt $minimums[$section]) {
            $violations += [PSCustomObject]@{
                file     = $rel
                section  = $section
                count    = $counts[$section]
                minimum  = $minimums[$section]
                severity = "ERROR"
                issue    = "$section has $($counts[$section]) items (minimum: $($minimums[$section]))"
            }
        }
    }
}

return $violations
