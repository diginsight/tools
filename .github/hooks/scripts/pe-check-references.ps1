<#
.SYNOPSIS
    PE reference integrity checker — validates all 📖 references resolve to existing files.

.DESCRIPTION
    Scans PE artifacts (.copilot/context/, .github/instructions/, .github/agents/,
    .github/prompts/, .github/skills/, .github/templates/, .github/prompt-snippets/)
    for 📖 references and verifies each referenced path exists.

    Supports three reference formats:
    - Backtick-quoted workspace paths: `.copilot/context/path.md`
    - Markdown links: [text](path.md)
    - Bare workspace paths: .copilot/context/path.md or .github/templates/path.md

    Returns an array of violations, each with source file, line number,
    referenced path, and reference text.

.PARAMETER WorkspaceRoot
    Root of the workspace. Defaults to 3 levels up from script location.

.OUTPUTS
    Array of PSCustomObject with: sourceFile, lineNumber, referencedPath, referenceText
#>
param(
    [string]$WorkspaceRoot
)

if (-not $WorkspaceRoot) {
    $WorkspaceRoot = Resolve-Path (Join-Path $PSScriptRoot (Join-Path ".." (Join-Path ".." "..")))
}

$scanDirs = @(
    ".copilot/context"
    ".github/instructions"
    ".github/agents"
    ".github/prompts"
    ".github/skills"
    ".github/templates"
    ".github/prompt-snippets"
    ".github/copilot-instructions.md"
    ".github/00.00-context-structure-index.md"
)

$violations = @()

foreach ($scanDir in $scanDirs) {
    $fullPath = Join-Path $WorkspaceRoot $scanDir

    if (-not (Test-Path $fullPath)) { continue }

    $item = Get-Item $fullPath
    if ($item.PSIsContainer) {
        $mdFiles = Get-ChildItem -Path $fullPath -Filter "*.md" -Recurse
    }
    else {
        $mdFiles = @($item)
    }

    foreach ($file in $mdFiles) {
        $lines = Get-Content -Path $file.FullName -Encoding UTF8 -ErrorAction SilentlyContinue
        if (-not $lines) { continue }

        $sourceRelative = $file.FullName.Substring($WorkspaceRoot.Length + 1) -replace '\\', '/'
        $sourceDir = Split-Path $file.FullName -Parent

        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]

            # Skip lines without the 📖 emoji
            if ($line -notmatch '📖') { continue }

            # Skip lines that are just explaining the pattern (documentation examples)
            if ($line -match '^\s*[-\d].*Use `\*\*📖') { continue }
            if ($line -match '^\s*[-\d].*`📖') { continue }

            $refsFound = @()

            # Pattern 1: Backtick-quoted workspace-relative paths
            # e.g., `.copilot/context/00.00-prompt-engineering/01.06-system-parameters.md`
            # e.g., `.github/templates/00.00-prompt-engineering/workflow-design-diagrams.template.md`
            $backtickMatches = [regex]::Matches($line, '`(\.(?:copilot|github)/[^`]+)`')
            foreach ($m in $backtickMatches) {
                $refPath = $m.Groups[1].Value
                # Strip trailing text after the path (e.g., ` → "Agent Design"`)
                $refsFound += @{ path = $refPath; text = $m.Value }
            }

            # Pattern 2: Markdown links with workspace-relative paths
            # e.g., [text](.copilot/context/.../file.md) or [text](../../path)
            $mdLinkMatches = [regex]::Matches($line, '\[([^\]]*)\]\(([^)]+)\)')
            foreach ($m in $mdLinkMatches) {
                $linkTarget = $m.Groups[2].Value
                # Skip external URLs
                if ($linkTarget -match '^https?://') { continue }
                # Skip anchors-only
                if ($linkTarget -match '^#') { continue }

                # Resolve path
                if ($linkTarget -match '^\.(?:copilot|github)/') {
                    # Already workspace-relative
                    $refsFound += @{ path = $linkTarget; text = $m.Value }
                }
                elseif ($linkTarget -match '^\.\./' -or $linkTarget -match '^[^/]') {
                    # Relative to source file directory
                    $resolved = Join-Path $sourceDir $linkTarget
                    if ($resolved) {
                        try {
                            $resolvedFull = [System.IO.Path]::GetFullPath($resolved)
                            if ($resolvedFull.StartsWith($WorkspaceRoot)) {
                                $resolvedRelative = $resolvedFull.Substring($WorkspaceRoot.Length + 1) -replace '\\', '/'
                                $refsFound += @{ path = $resolvedRelative; text = $m.Value }
                            }
                        }
                        catch { }
                    }
                }
            }

            # Pattern 3: Bare workspace paths (not in backticks or markdown links)
            # Only check for paths not already captured above
            if ($refsFound.Count -eq 0) {
                $bareMatches = [regex]::Matches($line, '(?:^|[\s:])((\.(?:copilot|github))/[^\s`\)]+\.md)')
                foreach ($m in $bareMatches) {
                    $refPath = $m.Groups[1].Value
                    $refsFound += @{ path = $refPath; text = $refPath }
                }
            }

            # Validate each reference
            foreach ($ref in $refsFound) {
                $targetPath = $ref.path -replace '/', '\'
                # Skip placeholder/template paths (contain {variable} patterns)
                if ($targetPath -match '\{[^}]+\}') { continue }
                # Strip trailing directory separator for folder references
                $targetPath = $targetPath.TrimEnd('\')
                $fullTargetPath = Join-Path $WorkspaceRoot $targetPath

                if (-not (Test-Path $fullTargetPath)) {
                    $violations += [PSCustomObject]@{
                        sourceFile     = $sourceRelative
                        lineNumber     = $i + 1
                        referencedPath = $ref.path
                        referenceText  = $ref.text.Trim()
                    }
                }
            }
        }
    }
}

return $violations
