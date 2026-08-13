<#
.SYNOPSIS
    PE file modification tracker — PostToolUse hook companion script.
    Checks whether PE artifacts have their last_updated field bumped
    to today's date after modification.

.DESCRIPTION
    Called by .github/hooks/pe-file-modification-tracker.json after
    create_file or replace_string_in_file completes. Reads hook I/O
    JSON from stdin, checks if the modified file is a PE artifact,
    and warns if last_updated was not updated to today.
#>

$inputJson = [Console]::In.ReadToEnd()

$response = @{
    continue   = $true
    stopReason = ""
}

try {
    $hookInput = $inputJson | ConvertFrom-Json

    # Only process file-writing tools
    $toolName = $hookInput.toolName
    if ($toolName -notin @("create_file", "replace_string_in_file", "multi_replace_string_in_file")) {
        $response | ConvertTo-Json -Compress
        return
    }

    # Extract the file path from tool input
    $filePath = $null
    if ($hookInput.toolInput) {
        if ($hookInput.toolInput.filePath) {
            $filePath = $hookInput.toolInput.filePath
        }
        elseif ($hookInput.toolInput.replacements -and $hookInput.toolInput.replacements.Count -gt 0) {
            # multi_replace: check first replacement's file path
            $filePath = $hookInput.toolInput.replacements[0].filePath
        }
    }

    if (-not $filePath) {
        $response | ConvertTo-Json -Compress
        return
    }

    # Normalize path separators
    $normalizedPath = $filePath -replace '\\', '/'

    # Check if the file is a PE artifact
    $isPeArtifact = $false
    $artifactType = $null

    if ($normalizedPath -match '\.copilot/context/') {
        $isPeArtifact = $true
        $artifactType = "context file"
    }
    elseif ($normalizedPath -match '\.github/instructions/.*\.instructions\.md$') {
        $isPeArtifact = $true
        $artifactType = "instruction file"
    }
    elseif ($normalizedPath -match '\.github/agents/.*\.agent\.md$') {
        $isPeArtifact = $true
        $artifactType = "agent file"
    }
    elseif ($normalizedPath -match '\.github/prompts/.*\.prompt\.md$') {
        $isPeArtifact = $true
        $artifactType = "prompt file"
    }
    elseif ($normalizedPath -match '\.github/skills/.*/SKILL\.md$') {
        $isPeArtifact = $true
        $artifactType = "skill file"
    }
    elseif ($normalizedPath -match '\.github/templates/.*\.template\.md$') {
        $isPeArtifact = $true
        $artifactType = "template file"
    }
    elseif ($normalizedPath -match '\.github/hooks/.*\.json$') {
        $isPeArtifact = $true
        $artifactType = "hook file"
    }
    elseif ($normalizedPath -match '\.github/prompt-snippets/.*\.md$') {
        $isPeArtifact = $true
        $artifactType = "prompt-snippet file"
    }

    if (-not $isPeArtifact) {
        $response | ConvertTo-Json -Compress
        return
    }

    # Check if the file exists and has last_updated set to today
    if (Test-Path $filePath) {
        $content = Get-Content -Path $filePath -TotalCount 20 -ErrorAction SilentlyContinue
        if ($content) {
            $inFrontmatter = $false
            $lastUpdated = $null
            $hasVersion = $false

            foreach ($line in $content) {
                if ($line -match '^\s*---\s*$') {
                    if ($inFrontmatter) { break }
                    $inFrontmatter = $true
                    continue
                }
                if ($inFrontmatter -and $line -match '^\s*last_updated:\s*"?(\d{4}-\d{2}-\d{2})"?\s*$') {
                    $lastUpdated = $Matches[1]
                }
                if ($inFrontmatter -and $line -match '^\s*version:') {
                    $hasVersion = $true
                }
            }

            $todayStr = (Get-Date).ToString("yyyy-MM-dd")

            $messages = @()

            if ($lastUpdated -and $lastUpdated -ne $todayStr) {
                $fileName = Split-Path $filePath -Leaf
                $messages += "PE VERSION REMINDER: You modified $artifactType ``$fileName`` but its ``last_updated`` field is ``$lastUpdated`` (not today: ``$todayStr``). Update the YAML frontmatter ``last_updated`` to ``$todayStr``."
            }

            # Dependency map reminder for new PE artifact creation
            if ($toolName -eq "create_file") {
                $fileName = Split-Path $filePath -Leaf
                $messages += "DEPENDENCY MAP REMINDER: New $artifactType ``$fileName`` created. Update ``.copilot/context/00.00-prompt-engineering/05.01-artifact-dependency-map.md`` to include this artifact with its references and consumers."
            }

            if ($messages.Count -gt 0) {
                $response.systemMessage = $messages -join " | "
            }
        }
    }
}
catch {
    # Non-blocking — don't prevent tool execution on script errors
}

$response | ConvertTo-Json -Compress
