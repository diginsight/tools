<#
.SYNOPSIS
    PE evidence-anchor validation (R4 Layer A) — deterministically verifies that
    every `dim_evidence[]` entry in a pe-meta run outcome log carries a real,
    resolvable, distinct verbatim anchor. No LLM judgment.

.DESCRIPTION
    Implements the Layer-A checks from the shared evidence-bound coverage
    contract (.github/prompt-snippets/pe-meta-evidence-coverage.md):

      1. Resolvability   — the cited file exists and the cited line is in range.
      2. Literal-containment — the verbatim quoted snippet appears at file:line.
      3. Distinctness    — the same evidence_ref is not reused across PUs.

    The guard is intentionally TOLERANT and assumption-free, per the plan-04
    flexibility constraint:
      - It derives the artifact path from each outcome-log entry's `file`
        field — it never enumerates specific pe-meta files.
      - It reads the run from a run-id or an explicit JSONL path — no fixed
        cohort is baked in.
      - Adding a new agent, renaming headers, or running on a non-pe-meta
        domain requires NO edit to this script.

    Anchor convention (from the contract): an evidence_ref carries an `L<line>`
    locator AND a verbatim snippet in backticks, e.g.
        "frontmatter L5: `goal: \"...\"`"
    The path comes from the entry's `file`; the line from `L<n>`; the verbatim
    text from the backticked snippet. An entry with no backticked snippet fails
    the missing-anchor check.

.PARAMETER RunId
    The pe-meta run id. The outcome log is resolved to
    <WorkspaceRoot>/.copilot/temp/pe-meta-state/outcomes/<RunId>.jsonl.

.PARAMETER JsonlPath
    Explicit path to an outcome-log JSONL file. Overrides -RunId.

.PARAMETER WorkspaceRoot
    Root of the workspace. Defaults to 3 levels up from script location.

.PARAMETER AsJson
    Output JSON (for orchestrator/hook integration) instead of human-readable text.

.OUTPUTS
    Array of PSCustomObject with: file, dim, check, severity, detail
    (empty array = all anchors valid).

.EXAMPLE
    .\pe-check-evidence-anchors.ps1 -RunId agents-deps-full-20260607-r2

.EXAMPLE
    .\pe-check-evidence-anchors.ps1 -JsonlPath .copilot/temp/pe-meta-state/outcomes/run.jsonl -AsJson
#>
param(
    [string]$RunId,
    [string]$JsonlPath,
    [string]$WorkspaceRoot,
    [switch]$AsJson
)

if (-not $WorkspaceRoot) {
    $WorkspaceRoot = Resolve-Path (Join-Path $PSScriptRoot (Join-Path ".." (Join-Path ".." "..")))
}

$violations = @()

# Resolve the outcome-log path: explicit JSONL wins, else derive from run-id.
if (-not $JsonlPath) {
    if (-not $RunId) {
        Write-Error "Provide -RunId or -JsonlPath."
        exit 2
    }
    $JsonlPath = Join-Path $WorkspaceRoot ".copilot/temp/pe-meta-state/outcomes/$RunId.jsonl"
}
elseif (-not [System.IO.Path]::IsPathRooted($JsonlPath)) {
    $JsonlPath = Join-Path $WorkspaceRoot $JsonlPath
}

if (-not (Test-Path $JsonlPath)) {
    Write-Error "Outcome log not found: $JsonlPath"
    exit 2
}

# Whitespace-normalize for tolerant verbatim matching.
function ConvertTo-NormalizedText {
    param([string]$Text)
    if ($null -eq $Text) { return "" }
    return ($Text -replace '\s+', ' ').Trim()
}

# Cache file lines so each artifact is read at most once.
$fileLineCache = @{}
function Get-FileLines {
    param([string]$RelPath)
    if ($fileLineCache.ContainsKey($RelPath)) { return $fileLineCache[$RelPath] }
    $full = if ([System.IO.Path]::IsPathRooted($RelPath)) { $RelPath } else { Join-Path $WorkspaceRoot $RelPath }
    if (-not (Test-Path $full)) { $fileLineCache[$RelPath] = $null; return $null }
    $lines = Get-Content -Path $full -Encoding UTF8 -ErrorAction SilentlyContinue
    $fileLineCache[$RelPath] = $lines
    return $lines
}

# Track every evidence_ref string for the distinctness check.
$seenRefs = @{}

$logLines = Get-Content -Path $JsonlPath -Encoding UTF8 -ErrorAction SilentlyContinue
foreach ($logLine in $logLines) {
    if ([string]::IsNullOrWhiteSpace($logLine)) { continue }

    try { $entry = $logLine | ConvertFrom-Json -ErrorAction Stop }
    catch { continue }   # non-JSON / partial line — skip silently

    if (-not $entry.dim_evidence) { continue }
    $file = [string]$entry.file

    foreach ($de in $entry.dim_evidence) {
        $dim = [string]$de.dim
        $ref = [string]$de.evidence_ref

        # An empty evidence_ref is the pu-evidence linter's job, not ours; skip.
        if ([string]::IsNullOrWhiteSpace($ref)) { continue }

        # --- Distinctness check (across all PUs in the run) ---
        $refKey = ConvertTo-NormalizedText $ref
        if ($seenRefs.ContainsKey($refKey)) {
            $violations += [PSCustomObject]@{
                file     = $file
                dim      = $dim
                check    = "distinctness"
                severity = "HIGH"
                detail   = "evidence_ref reused (first seen on $($seenRefs[$refKey])) — batch-marking signature"
            }
        }
        else {
            $seenRefs[$refKey] = "$file/$dim"
        }

        # --- Parse the anchor: L<line> locator + backticked verbatim snippet ---
        $lineMatch = [regex]::Match($ref, '\bL(\d+)\b')
        $quoteMatch = [regex]::Match($ref, '`([^`]+)`')

        if (-not $quoteMatch.Success) {
            $violations += [PSCustomObject]@{
                file     = $file
                dim      = $dim
                check    = "missing-anchor"
                severity = "HIGH"
                detail   = "evidence_ref has no backticked verbatim snippet (cannot be machine-verified)"
            }
            continue
        }
        $quote = $quoteMatch.Groups[1].Value

        $lines = Get-FileLines -RelPath $file
        if ($null -eq $lines) {
            $violations += [PSCustomObject]@{
                file     = $file
                dim      = $dim
                check    = "resolvability"
                severity = "HIGH"
                detail   = "cited file does not exist or is unreadable"
            }
            continue
        }

        # --- Resolvability + literal-containment ---
        $normQuote = ConvertTo-NormalizedText $quote
        if ($lineMatch.Success) {
            $lineNo = [int]$lineMatch.Groups[1].Value
            if ($lineNo -lt 1 -or $lineNo -gt $lines.Count) {
                $violations += [PSCustomObject]@{
                    file     = $file
                    dim      = $dim
                    check    = "resolvability"
                    severity = "HIGH"
                    detail   = "cited line L$lineNo is out of range (file has $($lines.Count) lines)"
                }
                continue
            }
            # Containment window: cited line ±2 to tolerate minor drift since logging.
            $lo = [Math]::Max(0, $lineNo - 3)
            $hi = [Math]::Min($lines.Count - 1, $lineNo + 1)
            $window = ConvertTo-NormalizedText (($lines[$lo..$hi]) -join ' ')
            if ($window -notmatch [regex]::Escape($normQuote)) {
                $violations += [PSCustomObject]@{
                    file     = $file
                    dim      = $dim
                    check    = "literal-containment"
                    severity = "HIGH"
                    detail   = "quoted snippet not found at $file L$lineNo (+/-2) — mis-pointed or fabricated"
                }
            }
        }
        else {
            # No line locator — fall back to whole-file containment.
            $whole = ConvertTo-NormalizedText ($lines -join ' ')
            if ($whole -notmatch [regex]::Escape($normQuote)) {
                $violations += [PSCustomObject]@{
                    file     = $file
                    dim      = $dim
                    check    = "literal-containment"
                    severity = "HIGH"
                    detail   = "quoted snippet not found anywhere in $file — fabricated or wrong file"
                }
            }
        }
    }
}

if ($AsJson) {
    [PSCustomObject]@{
        run        = [System.IO.Path]::GetFileNameWithoutExtension($JsonlPath)
        violations = $violations
        verdict    = if ($violations.Count -eq 0) { "clean" } else { "suspected" }
    } | ConvertTo-Json -Depth 6
    return
}

if ($violations.Count -eq 0) {
    Write-Host "Evidence anchors: clean ($([System.IO.Path]::GetFileName($JsonlPath)))" -ForegroundColor Green
}
else {
    Write-Host "Evidence anchors: $($violations.Count) violation(s) — shallow-sweep=suspected" -ForegroundColor Yellow
    $violations | Format-Table file, dim, check, severity, detail -AutoSize
}

return $violations
