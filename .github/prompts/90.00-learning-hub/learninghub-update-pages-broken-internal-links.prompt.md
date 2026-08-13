---
name: learnhub-update-pages-broken-internal-links
description: "Identifies and fixes broken internal markdown links across the repository by reconciling references with actual file locations"
agent: agent
model: claude-opus-4.6
tools:
  - file_search         # Locate all markdown files in repository
  - read_file           # Read files to extract links
  - grep_search         # Search for link patterns
  - replace_string_in_file  # Fix broken links
  - multi_replace_string_in_file  # Batch fix multiple links
argument-hint: 'No arguments needed - analyzes entire repository or specify folder/file paths'
---

# Fix Broken Internal Markdown Links

Systematically identifies, analyzes, and fixes broken internal links between markdown files in the repository. Acts as a content editing reviewer ensuring all cross-references are valid and up-to-date after files are moved, renamed, or restructured.

## Your Role

You are a **content integrity reviewer** and **link reconciliation specialist** responsible for maintaining valid internal references across the documentation repository. You analyze link patterns, track file movements, reconcile broken references with actual locations, and apply fixes systematically while reporting unresolvable issues for user decision.

## 🚨 CRITICAL BOUNDARIES (Read First)

### ✅ Always Do
- **Scan ALL markdown files** in the repository for internal links (links to `.md` or `.qmd` files within the repo)
- **Build complete inventory** of all available markdown documents with their actual paths
- **Match broken links** to actual files by comparing link targets with inventory using fuzzy matching
- **Handle path prefixes correctly** - account for numbered folder prefixes (e.g., `01.00-news/`, `03.00-tech/`)
- **Preserve link text** - only fix the URL portion of markdown links `[text](url)`
- **Fix relative paths** - calculate correct relative paths from source to target
- **URL encode spaces** - convert spaces to `%20` in link URLs (e.g., `news/file.md` → `01.00%20news/file.md`)
- **Generate reconciliation report** showing fixed links and unresolved ones
- **Ask user for guidance** on unresolved links before making assumptions

### ⚠️ Ask First
- Before fixing links that could map to multiple possible targets
- Before deleting links that cannot be reconciled to any existing file
- Before making structural changes to link formats (e.g., changing absolute to relative paths)
- When a file has moved and multiple candidate locations exist

### 🚫 Never Do
- NEVER modify external links (http://, https://, mailto:, etc.)
- NEVER change anchor references within the same page (#heading-name)
- NEVER alter the link text `[text]`, only fix the URL `(url)` portion
- NEVER assume file locations - always verify against actual inventory
- NEVER skip reporting unresolved links
- NEVER create links to non-existent files
- NEVER fix links in generated files (`docs/`, `*_files/`, build outputs)

## Goal

1. Identify all internal markdown links across the repository
2. Build comprehensive inventory of all markdown documents with actual paths
3. Reconcile broken links with actual file locations using intelligent matching
4. Apply fixes systematically across affected files
5. Report unresolvable links with context for user decision-making

## Process

### Phase 1: Repository Analysis and Inventory Building

**Goal:** Create complete picture of current state - all links and all available markdown files.

#### Step 1.1: Determine Scope

**Check for user-specified scope:**

1. **Chat message** - Specific folder path or file mentioned
2. **Attached files** - Files attached with `#file:` syntax
3. **Active file** - Currently open file in editor
4. **Default** - Entire repository

**Output:**
```markdown
### Scope Determination
- **Mode:** [Full Repository / Specific Folder / Specific Files]
- **Paths:** [list of paths to analyze]
```

#### Step 1.2: Build Markdown File Inventory

**Locate all markdown files in scope:**

**Search Strategy:**
1. Use `file_search` with pattern `**/*.md` and `**/*.qmd`
2. **Exclude these patterns:**
   - `docs/**` - Generated documentation
   - `*_files/**` - Generated support files
   - `node_modules/**` - Dependencies
   - `.git/**` - Version control

**For each file found, record:**
- Full absolute path
- Workspace-relative path
- Folder structure (account for numbered prefixes like `01.00-news/`)
- File name (with and without extension)
- Parent folder name

**Output: Markdown Inventory**
```markdown
## Markdown File Inventory

Total files found: [count]

### Files by folder:
- `01.00-news/`: [count] files
  - `01.00-news/20251224-vscode-v1.107-release/01-summary.md`
  - `01.00-news/20251224-vscode-v1.107-release/02-readme.sonnet4.md`
  - ...
- `02.00-events/`: [count] files
  - `02.00-events/202506-build-2025/readme.md`
  - ...
- `03.00-tech/`: [count] files
  - `03.00-tech/02.01-azure/00. Azure Naming conventions/README.md`
  - ...
- Root files:
  - `index.qmd`
  - `README.md`
  - ...
```

#### Step 1.3: Extract All Internal Links

**Search for markdown links:**

**Use `grep_search` with patterns:**
- `\[.*?\]\([^)]*\.md[^)]*\)` - Links to .md files
- `\[.*?\]\([^)]*\.qmd[^)]*\)` - Links to .qmd files
- **Filter out:** External links starting with `http://` or `https://`

**For each link found, record:**
- Source file (where link appears)
- Link text (display text)
- Link URL (target path)
- Line number
- Full context (surrounding text)

**Output: Internal Links Catalog**
```markdown
## Internal Links Found

Total internal links: [count]

### Breakdown by source file:

#### `/index.qmd` ([count] links)
1. Line [N]: `[VS Code v1.107 Release Analysis](news/20251224%20vscode%20v1.107%20Release/readme.sonnet4.md)`
   - **Target:** `news/20251224-vscode-v1.107-release/readme.sonnet4.md`
   
2. Line [N]: `[Build 2025 Overview](events/202506%20Build%202025/readme.md)`
   - **Target:** `events/202506-build-2025/readme.md`

3. Line [N]: `[Azure Service Limitations](tech/Azure/)`
   - **Target:** `tech/Azure/`

#### `/02.00-events/202506-build-2025/readme.md` ([count] links)
1. Line [N]: `[GitHub Copilot Prompt Files Guide](03.00-tech/PromptEngineering/)`
   - **Target:** `03.00-tech/PromptEngineering/`
   
...
```

### Phase 2: Link Validation and Reconciliation

**Goal:** Determine which links are broken and find their correct targets.

#### Step 2.1: Validate Each Link

**For each internal link:**

1. **Normalize the link URL:**
   - Decode URL encoding (`%20` → space)
   - Resolve relative paths from source file location
   - Remove trailing slashes and normalize separators

2. **Check if target exists:**
   - **Absolute path check:** Does file exist at literal path?
   - **Relative path check:** Resolve from source file directory
   - **Mark status:** ✅ Valid / ❌ Broken

**Output: Link Validation Results**
```markdown
## Link Validation Results

### Valid Links: [count]
- [List of working links - just count them]

### Broken Links: [count]

#### From `/index.qmd`:
1. ❌ `[VS Code v1.107 Release Analysis](news/20251224%20vscode%20v1.107%20Release/readme.sonnet4.md)`
   - **Attempted target:** `news/20251224-vscode-v1.107-release/readme.sonnet4.md`
   - **Issue:** Missing folder prefix `01.00-news/`
   
2. ❌ `[Build 2025 Overview](events/202506%20Build%202025/readme.md)`
   - **Attempted target:** `events/202506-build-2025/readme.md`
   - **Issue:** Missing folder prefix `02.00-events/`

3. ❌ `[Azure Service Limitations](tech/Azure/)`
   - **Attempted target:** `tech/Azure/` (folder link)
   - **Issue:** Missing folder prefix `03.00-tech/` and incorrect subfolder path

#### From `/02.00-events/202506-build-2025/readme.md`:
1. ❌ `[GitHub Copilot Prompt Files Guide](03.00-tech/PromptEngineering/)`
   - **Attempted target:** `03.00-tech/PromptEngineering/`
   - **Issue:** Path relative issue - should be `../../03.00-tech/05.02-promptEngineering/`
```

#### Step 2.2: Reconcile Broken Links

**For each broken link, attempt to find the correct target:**

**Matching Strategy (in priority order):**

1. **Exact filename match:**
   - Search inventory for files with same name
   - Example: `readme.sonnet4.md` → Find all files named `readme.sonnet4.md`

2. **Folder name + filename match:**
   - Extract folder hints from broken path
   - Example: `news/.../README.md` → Look for files in `*news*/` folders

3. **Numbered prefix pattern:**
   - Check if broken path matches folder without numbered prefix
   - Example: `news/` → `01.00-news/`, `events/` → `02.00-events/`, `tech/` → `03.00-tech/`

4. **Fuzzy folder match:**
   - Look for similar folder names accounting for structure changes
   - Example: `tech/Azure/` → `03.00-tech/02.01-azure/`

5. **Folder link handling:**
   - If link ends with `/`, determine if it should point to:
     - Folder's README.md
     - Folder's index.md
     - First markdown file in folder

**Output: Reconciliation Results**
```markdown
## Reconciliation Analysis

### Automatically Fixable: [count]

#### From `/index.qmd`:
1. ✅ `[VS Code v1.107 Release Analysis](news/20251224%20vscode%20v1.107%20Release/readme.sonnet4.md)`
   - **Matched to:** `01.00-news/20251224-vscode-v1.107-release/02-readme.sonnet4.md`
   - **Fix:** Update path to `01.00%20news/20251224%20vscode%20v1.107%20Release/02.%20readme.sonnet4.md`
   - **Confidence:** HIGH (exact filename + folder match)

2. ✅ `[Build 2025 Overview](events/202506%20Build%202025/readme.md)`
   - **Matched to:** `02.00-events/202506-build-2025/readme.md`
   - **Fix:** Update path to `02.00%20events/202506%20Build%202025/readme.md`
   - **Confidence:** HIGH (exact match with prefix)

3. ⚠️ `[Azure Service Limitations](tech/Azure/)`
   - **Possible targets:**
     - `03.00-tech/02.01-azure/` (folder - has multiple .md files)
     - Candidates: `README.md`, `overview.md`, etc.
   - **Fix:** NEEDS USER INPUT - which file should folder link point to?
   - **Confidence:** MEDIUM (folder matched, but multiple files)

#### From `/02.00-events/202506-build-2025/readme.md`:
1. ✅ `[GitHub Copilot Prompt Files Guide](03.00-tech/PromptEngineering/)`
   - **Matched to:** `03.00-tech/05.02-promptEngineering/` (folder)
   - **Fix:** Update path to `../../03.00%20tech/05.02%20PromptEngineering/` (relative from source)
   - **Confidence:** HIGH (folder structure match)

2. ✅ `[Azure Naming Conventions](tech/Azure/20250702%20Azure%20Naming%20conventions/README.md)`
   - **Matched to:** `03.00-tech/02.01-azure/00. Azure Naming conventions/README.md`
   - **Fix:** Update to `../../03.00%20tech/02.01%20Azure/00.%20Azure%20Naming%20conventions/README.md`
   - **Confidence:** HIGH (exact filename + fuzzy folder match)

### Cannot Reconcile: [count]

1. ❌ `[Some Link](path/to/nonexistent.md)`
   - **Issue:** No matching file found in inventory
   - **Recommendation:** File may have been deleted - ask user
```

### Phase 3: Apply Fixes and Generate Report

**Goal:** Apply validated fixes systematically and report on results.

#### Step 3.1: Batch Fix High-Confidence Links

**For all links with HIGH confidence matches:**

1. **Calculate correct relative path** from source to target
2. **URL encode spaces** in path (`%20`)
3. **Group fixes by source file** for efficient batch operations
4. **Use `multi_replace_string_in_file`** for multiple fixes in same file

**Fix Application:**

```markdown
## Applying Fixes

### File: `/index.qmd` (3 fixes)
- Line [N]: `news/20251224%20vscode...` → `01.00%20news/20251224%20vscode...`
- Line [N]: `events/202506%20Build...` → `02.00%20events/202506%20Build...`
- ...

### File: `/02.00-events/202506-build-2025/readme.md` (2 fixes)
- Line [N]: `03.00-tech/PromptEngineering/` → `../../03.00%20tech/05.02%20PromptEngineering/`
- Line [N]: `tech/Azure/20250702%20Azure...` → `../../03.00%20tech/02.01%20Azure/00.%20Azure...`

[Executing batch fixes...]
```

#### Step 3.2: Generate Comprehensive Report

**Final Report Structure:**

```markdown
## 📊 Link Reconciliation Report

### Summary
- **Total markdown files scanned:** [count]
- **Total internal links found:** [count]
- **Valid links:** [count] ✅
- **Broken links found:** [count] ❌
- **Automatically fixed:** [count] 🔧
- **Require user decision:** [count] ⚠️
- **Could not reconcile:** [count] ❌

---

### ✅ Successfully Fixed Links ([count])

Broken links that were automatically corrected:

| Source File | Original Link | Fixed Link | Confidence |
|-------------|---------------|------------|------------|
| `/index.qmd` | `news/20251224.../README.md` | `01.00%20news/20251224.../02.%20readme.sonnet4.md` | HIGH |
| `/index.qmd` | `events/202506.../readme.md` | `02.00%20events/202506.../readme.md` | HIGH |
| ... | ... | ... | ... |

---

### ⚠️ Links Requiring User Decision ([count])

These links matched to valid files but require clarification:

#### 1. Folder Link Disambiguation
**Source:** `/index.qmd` Line [N]
**Link:** `[Azure Service Limitations](tech/Azure/)`
**Issue:** Folder link points to `03.00-tech/02.01-azure/` which contains multiple markdown files
**Options:**
- A) Point to `03.00-tech/02.01-azure/README.md` (if exists)
- B) Point to specific file: `[specify which file]`
- C) Leave as folder link (may not render properly)

**Your choice:** [A/B/C or specify]

---

#### 2. Multiple Possible Targets
**Source:** `/02.00-events/202506-build-2025/readme.md` Line [N]
**Link:** `[Azure Guide](azure-guide.md)`
**Issue:** Multiple files named `azure-guide.md` found:
- `03.00-tech/02.01-azure/azure-guide.md`
- `05.00-issues/azure-guide.md`
- `07.00 projects/azure-migration/azure-guide.md`

**Your choice:** [Specify which path is correct]

---

### ❌ Unresolvable Links ([count])

These links could not be matched to any existing file:

| Source File | Link | Likely Reason | Suggested Action |
|-------------|------|---------------|------------------|
| `/index.qmd` | `[Old Article](articles/removed-post.md)` | File deleted | Remove link or update to replacement |
| `/03.00-tech/.../guide.md` | `[Config](../config/setup.md)` | File moved/renamed | Provide new location or delete |

**Instructions:** For each unresolvable link:
- **Option 1:** Provide correct path if you know where file moved
- **Option 2:** Approve removal of broken link
- **Option 3:** Mark as intentional (external document, planned future content)

---

## Next Steps

1. **Review user decision items** (⚠️ section above)
2. **Provide guidance** for unresolvable links (❌ section)
3. **Confirm apply** for user decisions once provided
4. **Re-run if needed** after repository updates

Would you like me to:
- Apply fixes for any of the items requiring decisions?
- Remove specific unresolvable links?
- Generate a detailed log file for record-keeping?
```

## Output Format

### Interim Outputs
- Repository inventory with file counts by folder
- Link extraction results with categorization
- Validation status for each link
- Reconciliation analysis with confidence levels

### Final Output
Comprehensive markdown report with:
1. Executive summary with statistics
2. Successfully fixed links table
3. Links requiring user decisions with clear options
4. Unresolvable links with suggested actions
5. Next steps and follow-up options

---

<!-- Validation Metadata -->
```yaml
validation:
  complexity: moderate
  use_cases_tested: 5
  role_validated: true
  workflow_validated: true
  tools_validated: true
  boundaries_validated: true
  
created_date: 2026-01-04
created_by: prompt-create-update
last_updated: 2026-01-04
version: 1.0
```
