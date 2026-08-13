# PE Meta-Update Changelist — 2026-05-27

**Invocation:** `/pe-meta-update --mode apply --scope context`
**Pipeline:** Source Research → Structure → Consistency → Content → Approval → Apply → Regression → Report
**Scope filter:** `.copilot/context/**/*.md` (50 files)
**Phase 1 (external sources):** No new release-notes URL provided. Last external monitor: VS Code 1.117 (2026-04-27). No new external findings — internal state used for audits.

---

## Summary of findings

| ID | Severity | Phase | Title | Files | Autonomy |
|---|---|---|---|---|---|
| S1 | CRITICAL | Structure | Root context index missing YAML frontmatter | 1 | Approval required |
| C1 | HIGH | Consistency | 7 broken cross-references in 2 files | 2 | Approval required |
| S2 | MEDIUM | Structure | 2 empty placeholder folders never populated | 2 dirs | Approval required |
| C2 | MEDIUM | Consistency | Stale file-inventory tables in root index | 1 | Approval required |
| N1 | MEDIUM | Content | 6 `01.00-article-writing/` files dated 2026-04-26 (31d stale) | 6 | Approval required |
| N2 | LOW | Content | Pre-existing token-budget overruns (documented exceptions) | 5 | No action |
| N3 | LOW | Structure | Two parallel root indexes overlap (folder-index vs structure-index) | 2 | Observation only |

Risk-ordering: non-regressive metadata fixes → reference repairs → cleanup → content sync.

---

## S1 [CRITICAL] — Root index missing frontmatter

**File:** `.copilot/context/00.00-context-folder-index.md`
**Problem:** No YAML frontmatter at all. Violates `00.03-metadata-contracts.md` v1.1.0 required fields (`title`, `description`, `version`, `last_updated`, `domain`, `goal`, `scope`, `boundaries`, `rationales`).
**Why missed:** Yesterday's 2026-05-26 Bundle A targeted `90.00-learning-hub/`; the root index was outside that batch.

### Resolution options

- **Option A (recommended)** — Add complete frontmatter consistent with the contract, then proceed to fix C2 inventory drift in the same edit.
- **Option B** — Add minimal frontmatter only; defer remaining fields. Reduces risk but leaves contract partially unmet.

**Diff preview (Option A frontmatter block):**

```yaml
---
title: "Context folder structure and source mapping"
description: "Root index of .copilot/context/: folder purposes, source-to-context mapping, generation rules, and folder numbering convention"
version: "1.1.0"
last_updated: "2026-05-27"
domain: "prompt-engineering"
goal: "Provide a single navigation entry-point for the entire context tree and document how source artefacts map to context files"
scope:
  covers:
    - "Folder purposes (00.00-prompt-engineering, 01.00-article-writing, 90.00-learning-hub)"
    - "Source-to-context mapping (which articles feed which context files)"
    - "Context-file generation workflow and numbering convention"
  excludes:
    - "Per-folder file inventories (see each folder's local structure-index)"
    - "Detailed content rules (see individual context files)"
boundaries:
  - "MUST point to canonical per-folder indexes rather than duplicating file inventories"
  - "MUST NOT contain rule definitions — only navigation and source mapping"
rationales:
  - "Single root index lets new contributors find the right folder without scanning all 50 files"
  - "Source mapping table prevents orphan context files when source articles are renamed or removed"
---
```

---

## C1 [HIGH] — 7 broken cross-references

### Link 1 — Broken name (file renamed in 2026-04-28 PE cleanup)

**File:** `.copilot/context/00.00-context-folder-index.md` line 9
**Before:** `[context-files.instructions.md](../../.github/instructions/context-files.instructions.md)`
**After:** `[pe-context-files.instructions.md](../../.github/instructions/pe-context-files.instructions.md)`

### Link 2 — Wrong relative path

**File:** `.copilot/context/00.00-context-folder-index.md`
**Before:** `.copilot/context/00.00-prompt-engineering/` (resolves to `.copilot/context/.copilot/context/...`)
**After:** `./00.00-prompt-engineering/`

### Link 3 — Missing subdirectory in path

**File:** `.copilot/context/00.00-context-folder-index.md`
**Before:** `./01.04-tool-composition-guide.md`
**After:** `./00.00-prompt-engineering/01.04-tool-composition-guide.md`

### Links 4–7 — Path-depth bugs (`../..` → `../../..`)

**File:** `.copilot/context/01.00-article-writing/02-validation-criteria.md`
File is two levels deep under `.copilot/`; references need three `../` to reach repo root.

| Line | Before | After |
|---|---|---|
| 128 | `../../.github/prompts/01.00-article-writing/article-review-series-for-consistency-gaps-and-extensions.prompt.md` | `../../../.github/prompts/01.00-article-writing/article-review-series-for-consistency-gaps-and-extensions.prompt.md` |
| 150 | `../../03.00-tech/40.00-technical-writing/02-structure-and-information-architecture.md` | `../../../03.00-tech/40.00-technical-writing/02-structure-and-information-architecture.md` |
| 151 | `../../03.00-tech/40.00-technical-writing/08-consistency-standards-and-enforcement.md` | `../../../03.00-tech/40.00-technical-writing/08-consistency-standards-and-enforcement.md` |
| 160 | `../../03.00-tech/40.00-technical-writing/09-measuring-readability-and-comprehension.md` | `../../../03.00-tech/40.00-technical-writing/09-measuring-readability-and-comprehension.md` |

All target files confirmed to exist.

---

## S2 [MEDIUM] — Empty placeholder folders

**Folders:**
- `.copilot/context/00.00-prompt-engineering/dependency-map/` (contains only `.gitkeep`)
- `.copilot/context/00.00-prompt-engineering/structure/` (contains only `.gitkeep`)

**Background:** Created by the 2026-05-21 splitting plan as targets for appendix content. The appendices were ultimately archived to `99.00-temp/context-archives/` instead, leaving these folders orphaned. Confirmed unreferenced by any context file.

### Resolution options

- **Option A (recommended)** — Delete both folders.
- **Option B** — Keep folders, add `README.md` documenting their deprecated status.
- **Option C** — Restore the archived appendix content from `99.00-temp/context-archives/` (reverses 2026-05-21 decision; out of scope here).

---

## C2 [MEDIUM] — Stale inventory tables in root index

**File:** `.copilot/context/00.00-context-folder-index.md`
**Problems:**
- `00.00-prompt-engineering/` inventory shows 24 files; actual count is 35.
- `01.00-article-writing/` inventory shows 5 entries; missing `03-article-creation-rules.md` (6th file).
- "Article-to-Context Reverse Mapping" still uses single-digit numbering (1–15) that does not match the current dotted scheme (01.01–05.08).

### Resolution options

- **Option A (recommended)** — Replace per-folder inventory tables with pointers to the canonical per-folder structure indexes. Eliminates drift permanently. Keep the source-mapping tables (still useful and stable).
- **Option B** — Regenerate inventory tables verbatim. Information drifts again at the next file add/remove.

---

## N1 [MEDIUM] — `01.00-article-writing/` staleness (6 files)

All six files dated `last_updated: 2026-04-26` (31 days old today). Per the staleness rule in `05.04-meta-review-log.md` (>30 days = overdue category-level review).

**Files:**
- `01-style-guide.md`
- `02-validation-criteria.md`
- `03-article-creation-rules.md`
- `workflows/01-article-creation-workflow.md`
- `workflows/02-review-workflow.md`
- `workflows/03-series-planning-workflow.md`

### Resolution options

- **Option A (recommended for this run)** — Touch-only refresh: bump `last_updated` to 2026-05-27 only on files that receive actual content changes (`02-validation-criteria.md` via C1, plus the root index via S1+C1+C2). Leave the other four files untouched and flag them for a focused content review next pass. Avoids fake "freshness" stamps.
- **Option B** — Refresh `last_updated` on all six files now. Suppresses the staleness signal but does not verify content is still accurate.
- **Option C** — Defer; record in meta-review-log that a content review of `01.00-article-writing/` is pending.

---

## N2 / N3 — Informational only (no action)

- **N2:** `03-article-creation-rules.md` (4229 tokens), `02-dual-yaml-metadata.md` (3482), `04-reference-classification.md` (2961), `02-validation-criteria.md` (2308), `workflows/03-series-planning-workflow.md` (2341). All exceed nominal 2,500-token soft target; `03-article-creation-rules.md` carries its own rationale documenting the deliberate Tier-2 size choice.
- **N3:** Two parallel root indexes (`00.00-context-folder-index.md` and `00.00-prompt-engineering/00.00-context-structure-index.md`) have overlapping navigation scope. Option A on C2 reduces this overlap.

---

## Proposed apply set (if Option A approved on every finding)

| Step | File / dir | Operation |
|---|---|---|
| 1 | `.copilot/context/00.00-context-folder-index.md` | Add frontmatter (S1) + fix 3 links (C1 #1–3) + replace stale inventory tables with pointers (C2) + bump `last_updated` to 2026-05-27 |
| 2 | `.copilot/context/01.00-article-writing/02-validation-criteria.md` | Fix 4 link-depth bugs (C1 #4–7) + bump `last_updated` to 2026-05-27 |
| 3 | `.copilot/context/00.00-prompt-engineering/dependency-map/` | Delete folder + `.gitkeep` (S2) |
| 4 | `.copilot/context/00.00-prompt-engineering/structure/` | Delete folder + `.gitkeep` (S2) |
| 5 | `.copilot/context/00.00-prompt-engineering/05.04-meta-review-log.md` | Append today's entry; update file manifest; bump version to 1.5.0 (Phase 8) |

**Rollback:** Snapshots written to `.copilot/temp/rollback/{filename}.20260527-HHMMSS.bak` for steps 1, 2, 5 before each edit. Step 3–4 deletions trivially reversible (folders contained only `.gitkeep`).

**Regression test (Phase 7):**
- Re-scan all `.copilot/context/**/*.md` for broken markdown links → expected: 0
- Verify all files have full frontmatter → expected: 50/50 valid
- Spot-check entry points in `00.01-governance-and-capability-baseline.md` Categories 1–5 still resolve

---

## Approval choices

Reply with one of:

- **`approve all (Option A)`** — apply every change above as recommended
- **`approve except N1`** — apply S1, C1, S2, C2 but skip touch-only date bumps
- **`custom`** — list which findings to apply and which option per finding
- **`cancel`** — abort without changes
