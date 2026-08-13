<#
.SYNOPSIS
    PE tool alignment checker — verifies plan-mode agents have no write tools.

.DESCRIPTION
    Reads all agent files (.github/agents/**/*.agent.md) and validates that:
    - Agents with `agent: plan` do NOT include any write/execute tools
    - Agents with `agent: agent` include at least one write tool (warning only)
    - Tool count is within 3-7 range

    Write tools: create_file, replace_string_in_file, multi_replace_string_in_file,
                 run_in_terminal, edit_notebook_file, run_notebook_cell

.PARAMETER WorkspaceRoot
    Root of the workspace. Defaults to 3 levels up from script location.

.OUTPUTS
    Array of PSCustomObject with: file, mode, issue, severity
#>
param(
    [string]$WorkspaceRoot
)

if (-not $WorkspaceRoot) {
    $WorkspaceRoot = Resolve-Path (Join-Path $PSScriptRoot (Join-Path ".." (Join-Path ".." "..")))
}

$writeTools = @(
    "create_file"
    "replace_string_in_file"
    "multi_replace_string_in_file"
    "run_in_terminal"
    "edit_notebook_file"
    "run_notebook_cell"
)

$violations = @()

function Get-AgentModeAndTools {
    param([string]$FilePath)

    $content = Get-Content -Path $FilePath -TotalCount 40 -Encoding UTF8 -ErrorAction SilentlyContinue
    if (-not $content) { return $null }

    $mode = $null
    $tools = @()
    $inFrontmatter = $false
    $inTools = $false

    foreach ($line in $content) {
        if ($line -match '^\s*---\s*$') {
            if ($inFrontmatter) { break }
            $inFrontmatter = $true
            continue
        }
        if (-not $inFrontmatter) { continue }

        # Tools array handling
        if ($inTools) {
            if ($line -match "^\s*-\s+'([^']+)'") {
                $tools += $Matches[1]
                continue
            }
            elseif ($line -match '^\s*-\s+"([^"]+)"') {
                $tools += $Matches[1]
                continue
            }
            elseif ($line -match '^\s*-\s+(\S+)') {
                $tools += $Matches[1]
                continue
            }
            else {
                $inTools = $false
            }
        }

        if ($line -match '^agent:\s*(\S+)') {
            $mode = $Matches[1].Trim("'").Trim('"')
        }
        elseif ($line -match '^tools:\s*$') {
            $inTools = $true
        }
        elseif ($line -match "^tools:\s*\[(.+)\]") {
            $inner = $Matches[1]
            foreach ($t in ($inner -split ',')) {
                $t = $t.Trim().Trim("'").Trim('"')
                if ($t) { $tools += $t }
            }
        }
    }

    return @{ mode = $mode; tools = $tools }
}

$agentDir = Join-Path $WorkspaceRoot ".github/agents"
if (-not (Test-Path $agentDir)) { return $violations }

$agentFiles = Get-ChildItem -Path $agentDir -Filter "*.agent.md" -Recurse

foreach ($file in $agentFiles) {
    $rel = ".github/agents/" + ($file.FullName.Substring($agentDir.Length + 1) -replace '\\', '/')
    $data = Get-AgentModeAndTools -FilePath $file.FullName

    if (-not $data -or -not $data.mode) { continue }

    $mode = $data.mode
    $tools = $data.tools

    # Check: plan-mode agents must NOT have write tools
    if ($mode -eq "plan") {
        $foundWriteTools = $tools | Where-Object { $_ -in $writeTools }
        if ($foundWriteTools) {
            $violations += [PSCustomObject]@{
                file     = $rel
                mode     = $mode
                issue    = "Plan-mode agent has write tool(s): $($foundWriteTools -join ', ')"
                severity = "ERROR"
            }
        }
    }

    # Check: agent-mode should have at least one write tool (warning)
    if ($mode -eq "agent") {
        $foundWriteTools = $tools | Where-Object { $_ -in $writeTools }
        if (-not $foundWriteTools -or $foundWriteTools.Count -eq 0) {
            $violations += [PSCustomObject]@{
                file     = $rel
                mode     = $mode
                issue    = "Agent-mode agent has no write tools — consider using plan mode instead"
                severity = "WARN"
            }
        }
    }

    # Check: tool count within 3-7 range
    if ($tools.Count -gt 0) {
        if ($tools.Count -lt 3) {
            $violations += [PSCustomObject]@{
                file     = $rel
                mode     = $mode
                issue    = "Tool count ($($tools.Count)) below minimum of 3"
                severity = "WARN"
            }
        }
        elseif ($tools.Count -gt 7) {
            $violations += [PSCustomObject]@{
                file     = $rel
                mode     = $mode
                issue    = "Tool count ($($tools.Count)) exceeds maximum of 7"
                severity = "ERROR"
            }
        }
    }
}

return $violations
