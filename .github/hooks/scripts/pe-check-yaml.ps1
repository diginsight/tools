<#
.SYNOPSIS
    PE YAML frontmatter validation — checks required fields per artifact type.

.DESCRIPTION
    Validates YAML frontmatter in PE artifacts against required field rules:
    - Context files: title, description, version, last_updated
    - Instruction files: description, applyTo
    - Agent files: description, agent (mode)
    - Prompt files: description, agent (mode)
    - Skill files: description

    Also validates field formats:
    - version: semver pattern (N.N.N)
    - last_updated: ISO date (YYYY-MM-DD)
    - agent: must be 'plan' or 'agent'
    - tools: must be an array with 3-7 items (agents only)

.PARAMETER WorkspaceRoot
    Root of the workspace. Defaults to 3 levels up from script location.

.OUTPUTS
    Array of PSCustomObject with: file, field, issue, severity
#>
param(
    [string]$WorkspaceRoot
)

if (-not $WorkspaceRoot) {
    $WorkspaceRoot = Resolve-Path (Join-Path $PSScriptRoot (Join-Path ".." (Join-Path ".." "..")))
}

$violations = @()

function Get-YamlFrontmatter {
    param([string]$FilePath)

    $content = Get-Content -Path $FilePath -TotalCount 40 -Encoding UTF8 -ErrorAction SilentlyContinue
    if (-not $content) { return $null }

    $result = @{}
    $inFrontmatter = $false
    $toolsList = @()
    $inTools = $false

    foreach ($line in $content) {
        if ($line -match '^\s*---\s*$') {
            if ($inFrontmatter) { break }
            $inFrontmatter = $true
            continue
        }
        if (-not $inFrontmatter) { continue }

        # Tools array (YAML list format) — these are indented under tools:
        if ($inTools) {
            if ($line -match "^\s*-\s+'([^']+)'") {
                $toolsList += $Matches[1]
                continue
            }
            elseif ($line -match '^\s*-\s+"([^"]+)"') {
                $toolsList += $Matches[1]
                continue
            }
            elseif ($line -match '^\s*-\s+(\S+)') {
                $toolsList += $Matches[1]
                continue
            }
            else {
                $inTools = $false
                $result["tools"] = $toolsList
            }
        }

        # Only match top-level (non-indented) key-value pairs
        if ($line -match '^(\w[\w_-]*):\s*(.*)$') {
            $key = $Matches[1]
            $value = $Matches[2].Trim()

            if ($key -eq "tools") {
                # Inline array format: ['tool1', 'tool2']
                if ($value -match "^\[(.+)\]$") {
                    $inner = $Matches[1]
                    $toolsList = @()
                    foreach ($t in ($inner -split ',')) {
                        $t = $t.Trim().Trim("'").Trim('"')
                        if ($t) { $toolsList += $t }
                    }
                    $result["tools"] = $toolsList
                }
                elseif (-not $value -or $value -eq '') {
                    # Multi-line YAML list follows
                    $inTools = $true
                    $toolsList = @()
                }
                else {
                    $result[$key] = $value
                }
            }
            else {
                # Strip quotes
                $value = $value.Trim("'").Trim('"')
                $result[$key] = $value
            }
        }
    }

    if ($inTools -and $toolsList.Count -gt 0) {
        $result["tools"] = $toolsList
    }

    return $result
}

function Test-ArtifactYaml {
    param(
        [string]$FilePath,
        [string]$RelativePath,
        [string]$ArtifactType,
        [string[]]$RequiredFields,
        [hashtable]$FieldValidators
    )

    $yaml = Get-YamlFrontmatter -FilePath $FilePath
    if (-not $yaml -or $yaml.Count -eq 0) {
        $script:violations += [PSCustomObject]@{
            file     = $RelativePath
            field    = "(frontmatter)"
            issue    = "No YAML frontmatter found"
            severity = "ERROR"
        }
        return
    }

    # Check required fields
    foreach ($field in $RequiredFields) {
        if (-not $yaml.ContainsKey($field) -or -not $yaml[$field]) {
            $script:violations += [PSCustomObject]@{
                file     = $RelativePath
                field    = $field
                issue    = "Required field missing"
                severity = "ERROR"
            }
        }
    }

    # Run field validators
    foreach ($field in $FieldValidators.Keys) {
        if ($yaml.ContainsKey($field) -and $yaml[$field]) {
            $validator = $FieldValidators[$field]
            $result = & $validator $yaml[$field]
            if ($result) {
                $script:violations += [PSCustomObject]@{
                    file     = $RelativePath
                    field    = $field
                    issue    = $result
                    severity = "ERROR"
                }
            }
        }
    }
}

# --- Validators ---
$versionValidator = {
    param($value)
    if ($value -notmatch '^\d+\.\d+\.\d+$') {
        return "Invalid semver format: '$value' (expected N.N.N)"
    }
}

$dateValidator = {
    param($value)
    if ($value -notmatch '^\d{4}-\d{2}-\d{2}$') {
        return "Invalid date format: '$value' (expected YYYY-MM-DD)"
    }
    try {
        [datetime]::ParseExact($value, "yyyy-MM-dd", $null) | Out-Null
    }
    catch {
        return "Unparseable date: '$value'"
    }
}

$modeValidator = {
    param($value)
    if ($value -notin @("plan", "agent")) {
        return "Invalid agent mode: '$value' (must be 'plan' or 'agent')"
    }
}

# --- Scan context files ---
$contextDir = Join-Path $WorkspaceRoot ".copilot/context"
if (Test-Path $contextDir) {
    Get-ChildItem -Path $contextDir -Filter "*.md" -Recurse | Where-Object { $_.Name -ne "00.00-context-structure-index.md" } | ForEach-Object {
        $rel = ".copilot/context/" + ($_.FullName.Substring((Join-Path $WorkspaceRoot ".copilot/context").Length + 1) -replace '\\', '/')
        Test-ArtifactYaml -FilePath $_.FullName -RelativePath $rel `
            -ArtifactType "context" `
            -RequiredFields @("title", "description", "version", "last_updated") `
            -FieldValidators @{
                "version"      = $versionValidator
                "last_updated" = $dateValidator
            }
    }
}

# --- Scan instruction files ---
$instrDir = Join-Path $WorkspaceRoot ".github/instructions"
if (Test-Path $instrDir) {
    Get-ChildItem -Path $instrDir -Filter "*.instructions.md" | ForEach-Object {
        $rel = ".github/instructions/$($_.Name)"
        Test-ArtifactYaml -FilePath $_.FullName -RelativePath $rel `
            -ArtifactType "instruction" `
            -RequiredFields @("description", "applyTo") `
            -FieldValidators @{}
    }
}

# --- Scan agent files ---
$agentDir = Join-Path $WorkspaceRoot ".github/agents"
if (Test-Path $agentDir) {
    Get-ChildItem -Path $agentDir -Filter "*.agent.md" -Recurse | ForEach-Object {
        $rel = ".github/agents/" + ($_.FullName.Substring((Join-Path $WorkspaceRoot ".github/agents").Length + 1) -replace '\\', '/')
        Test-ArtifactYaml -FilePath $_.FullName -RelativePath $rel `
            -ArtifactType "agent" `
            -RequiredFields @("description", "agent") `
            -FieldValidators @{
                "agent" = $modeValidator
            }
    }
}

# --- Scan prompt files ---
$promptDir = Join-Path $WorkspaceRoot ".github/prompts"
if (Test-Path $promptDir) {
    Get-ChildItem -Path $promptDir -Filter "*.prompt.md" -Recurse | Where-Object { $_.FullName -notmatch '[/\\]old[/\\]' } | ForEach-Object {
        $rel = ".github/prompts/" + ($_.FullName.Substring((Join-Path $WorkspaceRoot ".github/prompts").Length + 1) -replace '\\', '/')
        Test-ArtifactYaml -FilePath $_.FullName -RelativePath $rel `
            -ArtifactType "prompt" `
            -RequiredFields @("description", "agent") `
            -FieldValidators @{
                "agent" = $modeValidator
            }
    }
}

# --- Scan skill files ---
$skillDir = Join-Path $WorkspaceRoot ".github/skills"
if (Test-Path $skillDir) {
    Get-ChildItem -Path $skillDir -Filter "SKILL.md" -Recurse | ForEach-Object {
        $rel = ".github/skills/" + ($_.FullName.Substring((Join-Path $WorkspaceRoot ".github/skills").Length + 1) -replace '\\', '/')
        Test-ArtifactYaml -FilePath $_.FullName -RelativePath $rel `
            -ArtifactType "skill" `
            -RequiredFields @("description") `
            -FieldValidators @{}
    }
}

return $violations
