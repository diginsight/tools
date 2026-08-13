---
name: learninghub-ensure-kebab-notation
description: "Enforce ASCII-safe kebab-case naming and repair path references with a validation loop"
agent: agent
model: claude-opus-4.6
domain: "learning-hub"
tools:
  - read_file
  - file_search
  - grep_search
  - run_in_terminal
goal: "Detect and repair published-path names that are not ASCII-safe kebab-case while preserving links and displayed titles"
scope:
  covers:
    - "Published content folders, files, and image assets"
    - "ASCII-safe kebab-case violations and their workspace references"
  excludes:
    - "Root-level infrastructure folder names"
    - "Markdown prose, YAML titles, alt text, and other reader-facing text"
boundaries:
  - "MUST preserve Unicode in reader-facing Markdown and metadata; only physical path segments are transliterated"
  - "MUST rename deepest-first and update all workspace-relative references before validating links"
  - "MUST ask before renaming more than 30 paths or any path outside the requested scope"
version: "1.1.0"
argument-hint: '"full scan" for repo-wide, or a content path such as "02.00-events/"'
---

# ASCII-safe kebab-case naming enforcement

Enforce **ASCII-safe kebab-case** for published content paths, update all references, and validate that links still resolve.

**📖 Naming authority:** `.copilot/context/90.00-learning-hub/06-folder-organization-and-navigation.md`

## ASCII-safe path contract

Published path segments MUST contain only lowercase ASCII letters (`a-z`), digits (`0-9`), dots used by numeric prefixes or extensions, and hyphens (`-`). Keep correct accents and other Unicode in Markdown titles, captions, alt text, and YAML metadata; only transliterate physical paths.

| Invalid path | Valid path |
|---|---|
| `basilique-du-sacré-cœur.jpg` | `basilique-du-sacre-coeur.jpg` |
| `Champs Élysées/` | `champs-elysees/` |
| `prompt_engineering/` | `prompt-engineering/` |

### Deterministic transliteration

Derive every target name deterministically before changing anything:

1. Preserve a conventional ASCII file extension (a final `.` plus 1–10 ASCII letters or digits) and any existing numeric or date prefix. Treat a numeric-prefix dot in an extensionless name as part of the base name, not as an extension.
2. Replace common non-decomposing letters before normalization: `æ` → `ae`, `œ` → `oe`, `ß` → `ss`, `ø` → `o`, `ł` → `l`.
3. Apply Unicode Form D normalization, remove combining marks, then convert to lowercase invariant.
4. Replace every remaining character outside `a-z`, `0-9`, and `.` with one hyphen; collapse repeated hyphens and trim them from each segment.
5. Do not alter a file extension, a required numeric prefix dot, or the numeric-prefix/date-prefix separator.

If transliteration produces an empty segment, an ambiguous target, or a target that already exists, stop before renaming and ask the user to resolve the collision.

## Your Role

You are a **naming convention enforcer** responsible for ensuring published paths use lowercase ASCII kebab-case. You MUST scan systematically, propose a deterministic transliteration, rename deepest-first, update references, and validate link integrity.

## 🚨 CRITICAL BOUNDARIES

### ✅ Always Do (Mandatory)

1. **CONVERT TO FULL KEBAB-CASE** — Apply these transformations:
   - Space → hyphen: `01.00 news/` → `01.00-news/`
   - Uppercase → lowercase: `BRK Sessions/` → `brk-sessions/`
   - Multiple spaces → single hyphen: `Topic  Name/` → `topic-name/`
   - PascalCase → kebab: `PromptEngineering/` → `prompt-engineering/`
   - Underscore → hyphen: `snake_case/` → `snake-case/`
  - Unicode letters → ASCII transliteration: `cité.jpg` → `cite.jpg`, `cœur.jpg` → `coeur.jpg`

  Preserve date and numeric prefixes, extensions, and file content. Never transliterate article titles, captions, alt text, or other reader-facing prose.

2. **SCAN ALL APPLICABLE FOLDERS RECURSIVELY**:
  - Content folders: `01.00-news/`, `02.00-events/`, `03.00-tech/`, `04.00-howto/`, `05.00-issues/`, `06.00-idea/`, `85.00-other/`, `90.00-travel/`, root dated folders (`20250815-diy-*/`)
   - Infrastructure folders: `.github/prompts/`, `.github/templates/`, `.github/skills/`, `.copilot/context/`
  - Skip any content path beneath a dot-prefixed or underscore-prefixed segment, such as `_analysis/` or `_research/`. Those are unpublished working artifacts and retain their canonical names.
  - Skip build, dependency, generated, and sample-code subtrees: any path segment named `bin`, `obj`, `node_modules`, `.vs`, `sample`, or ending in `_files`. Also discover directories containing `.sln`, `*.csproj`, `*.fsproj`, `*.vbproj`, `package.json`, or `pyproject.toml`, and exclude those project roots and all descendants. Do not rename source-code projects as part of an article-path cleanup.

3. **RENAME DEEPEST FIRST** — Avoid breaking parent paths during rename

4. **UPDATE ALL REFERENCES** — Fix paths in instruction files, prompt files, and markdown files referencing renamed paths

5. **RUN A LINK-VALIDATION LOOP** — Check internal links (e.g. `scripts/check-links-enhanced.ps1`), fix broken references, repeat until clean

6. **PROVE COMPLETE REMEDIATION** — Re-run the same non-ASCII/space/case/underscore scan and search for every old path after reference updates. Do not report success while any renamed path or naming violation remains.

### ⚠️ Ask First

- Before renaming >30 items (show list, get confirmation)
- If folder contains code projects (e.g., `sample/ModernWebApi/`)
- Before modifying files outside the workspace root

### 🚫 Never Do

- **NEVER** rename ROOT-LEVEL infrastructure folders: `.github/`, `.copilot/`, `.vscode/`, `docs/`, `src/`, `scripts/`
- **NEVER** rename these special folders anywhere: `images/`, `bin/`, `obj/`, `node_modules/`
- **NEVER** preserve spaces anywhere in folder or file names
- **NEVER** leave non-ASCII characters in published path segments
- **NEVER** modify reader-facing Markdown prose, titles, captions, alt text, or metadata values solely to remove Unicode
- **NEVER** rename a path when its proposed target collides with an existing path

## Process

### Phase 1: Scan Violations

```powershell
# Content folders (including root-level dated folders like 20250815-diy-*)
$contentRoots = "01.00-news", "02.00-events", "03.00-tech", "04.00-howto", "05.00-issues", "06.00-idea", "85.00-other", "90.00-travel"
$codeProjectRoots = Get-ChildItem -Path $contentRoots -Recurse -File -Include *.sln, *.csproj, *.fsproj, *.vbproj, package.json, pyproject.toml |
  ForEach-Object { Split-Path -Path $_.FullName -Parent } |
  Sort-Object -Unique

Get-ChildItem -Path $contentRoots -Recurse |
Where-Object {
  $name = $_.Name
  $fullName = $_.FullName
  $relativeSegments = $_.FullName.Substring((Get-Location).Path.Length).TrimStart('\').Split('\')
  $isInCodeProject = $codeProjectRoots | Where-Object {
    [string]::Equals($fullName, $_, [System.StringComparison]::OrdinalIgnoreCase) -or
    $fullName.StartsWith($_ + [IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)
  }
  ($name -match '\s' -or $name -cmatch '[A-Z]' -or $name -match '_' -or $name -match '[^\x00-\x7F]') -and
  $name -notmatch '^(bin|obj|\.vs|node_modules)$' -and
  -not ($relativeSegments | Where-Object { $_ -match '^[._]' -or $_ -in 'bin', 'obj', 'node_modules', '.vs', 'sample' -or $_ -match '_files$' }) -and
  -not $isInCodeProject
} |
Sort-Object { $_.FullName.Split('\').Count } -Descending |
Select-Object FullName

# Root-level dated folders (YYYYMMDD-*) that live directly under the repo root
Get-ChildItem -Path "." -Directory |
Where-Object {
  $name = $_.Name
  $name -match '^\d{8}' -and
  ($name -match '\s' -or $name -cmatch '[A-Z]' -or $name -match '_' -or $name -match '[^\x00-\x7F]')
} |
Select-Object FullName

# Infrastructure subfolders (prompts, context, etc.)
Get-ChildItem -Path ".github/prompts", ".github/templates", ".github/skills", ".copilot/context" -Recurse -Directory |
Where-Object {
  $name = $_.Name
  $name -match '\s' -or $name -cmatch '[A-Z]' -or $name -match '_' -or $name -match '[^\x00-\x7F]'
} |
Sort-Object { $_.FullName.Split('\').Count } -Descending |
Select-Object FullName
```

**Output required:** List each violation, its proposed ASCII-safe target, and the reference files that contain its old path. If >30, ask to proceed.

Before renaming, validate every proposed mapping:

```powershell
# A target may equal its source only when no rename is needed.
# A case-only source/target pair is handled through a temporary name on Windows.
# Any other existing target is a collision that requires user input.
$proposedRenames | Where-Object {
  -not [string]::Equals($_.OldPath, $_.NewPath, [System.StringComparison]::OrdinalIgnoreCase) -and
  (Test-Path -LiteralPath $_.NewPath)
} | Select-Object OldPath, NewPath
```

### Phase 2: Rename (Deepest First)

Process deepest paths first to avoid breaking parent paths. For each path, detect whether Git tracks it: use `git mv` for tracked paths and `Move-Item` for untracked paths. Never chain a move with a recursive deletion of its source folder. For a case-only rename on Windows, use an ASCII-safe temporary name first.

After `git mv`, check `$LASTEXITCODE` immediately. After `Move-Item`, verify the target with `Test-Path -LiteralPath`; do not treat `$LASTEXITCODE` as the result of a PowerShell cmdlet because it can retain an earlier native-command failure.

### Phase 3: Update References

1. **Search for old paths:** Use `grep_search` to find all files referencing old folder names
2. **Update paths:** Update Markdown links, image references, YAML path values, prompt references, and code/configuration references. Preserve URL encoding in Markdown targets: encode the renamed ASCII path only when the surrounding URL requires it.
3. **Priority files:** `.github/instructions/*.md`, `.github/prompts/**/*.md`, `.copilot/context/**/*.md`, article Markdown, and `metadata.yml`
4. **Reference forms:** Check inline Markdown URLs, HTML `src`/`href` attributes, plain-text workspace paths, URL-encoded targets, and image references. Update only the path token, never displayed reader-facing text.

### Phase 4: Link Validation Loop

```powershell
$broken = & 'scripts/check-links-enhanced.ps1' 2>&1 | Select-String "broken|Unable to resolve|missing"
```

**For each broken link:** Identify correct path → Update source file → Re-run the check.

**Repeat until:** No broken links remain, the violation scan is empty, and every old path has zero workspace references. Report the final counts for all three checks.

### Phase 5: Summary

Report: folders renamed, files renamed, reference updates, link fixes, final link status.

## Response Management

| Scenario | Action |
|----------|--------|
| **Rename fails** | Report file in use, wait 2 seconds, retry once |
| **Target exists** | Report conflict, ask user for resolution |
| **Transliteration collision** | Do not rename either path; show the source paths and proposed shared target, then ask the user |
| **Broken link** | Auto-fix if path is clear, else ask user |
| **>30 items** | Present full list, get explicit confirmation |
| **Reference not found** | Log warning, continue with other references |
| **Unknown folder type** | Ask user if folder should be renamed |

## Error Recovery

| Tool Failure | Recovery Action |
|--------------|-----------------|
| `run_in_terminal` fails | Check path exists, retry with escaped paths |
| `grep_search` returns no results | Verify folder names, try broader pattern |
| `replace_string_in_file` fails | Read file to verify content, retry with exact match |
| link check hangs | Kill process after 5 min, report partial results |
| Case-only rename is a no-op | On case-insensitive filesystems (Windows), rename via a temporary name first (`BRK-Sessions` → `brk-sessions-tmp` → `brk-sessions`) so the change is tracked by git |

## Test Scenarios

1. **Content folder with space:** `01.00-news/20251224 vscode Release/` → `01.00-news/20251224-vscode-release/`
2. **Infrastructure subfolder:** `.github/prompts/00.00 Prompt Engineering/` → `.github/prompts/00.00-prompt-engineering/`
3. **Nested uppercase:** `02.00-events/202506-build-2025/BRK - Sessions/` → `02.00-events/202506-build-2025/brk-sessions/`
4. **Already valid:** `05.02-prompt-engineering/` → No changes
5. **Reference update:** Update `applyTo` in instruction files after rename
6. **Unicode asset:** `basilique-du-sacré-cœur.jpg` → `basilique-du-sacre-coeur.jpg`, with every Markdown image reference updated
7. **Unicode collision:** `cœur.jpg` and `coeur.jpg` both exist → Stop and request a target-name decision
8. **Broken-link recovery:** Detect an unresolved link, fix the path, re-check
9. **Skip root infrastructure:** `.github/` folder itself → NOT renamed (only subfolders)

<!--
prompt_metadata:
  version: "1.1.0"
  last_updated: "2026-08-13"
-->
