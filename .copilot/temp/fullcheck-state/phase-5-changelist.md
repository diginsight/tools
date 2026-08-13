# Fullcheck Change Plan — 2026-04-28

**Mode:** fullcheck | **Scope:** all | **Flags:** none
**Source:** Current state analysis + internet research

## Consolidated Findings (Phases 1–4)

| # | Finding | Phase | Severity | Dependents | Action |
|---|---|---|---|---|---|
| 1 | 9 duplicate instruction file pairs | 2 | CRITICAL | All PE artifacts | Delete 9 unprefixed files |
| 2 | 2 duplicate skill pairs | 2 | HIGH | Agents referencing skills | Delete 2 unprefixed skill folders |
| 3 | `00.03-metadata-contracts.md` missing from STRUCTURE-README | 2 | HIGH | All metadata consumers | Add to File Index + governance category |
| 4 | STRUCTURE-README file count stale (29→31) | 2+4 | MEDIUM | STRUCTURE-README consumers | Update count |
| 5 | Version history says 18 categories, actual 17 | 4 | LOW | None | Fix typo |
| 6 | pe-meta-researcher/validator stale metadata | 4 | MEDIUM | Meta workflow | Bump version + last_updated |
| 7 | copilot-instructions.md references unprefixed names | 3 | MEDIUM | All PE workflows | Update after duplicate removal |
| 8 | Legacy old/ directory in granular prompts | 2 | MEDIUM | None | Delete after confirming no consumers |
| 9 | Asymmetric granular prompt coverage | 2 | LOW | None | Document as intentional |

---

## Proposed Changes (execution order)

### Change 1: Delete 9 unprefixed instruction files (CRITICAL) — ✅ DONE

**Files to DELETE:**
- `.github/instructions/agents.instructions.md`
- `.github/instructions/context-files.instructions.md`
- `.github/instructions/copilot-instructions-file.instructions.md`
- `.github/instructions/hooks.instructions.md`
- `.github/instructions/instruction-files.instructions.md`
- `.github/instructions/prompt-snippets.instructions.md`
- `.github/instructions/prompts.instructions.md`
- `.github/instructions/skills.instructions.md`
- `.github/instructions/templates.instructions.md`

**Rationale:** pe-prefixed versions are newer, have enhanced metadata (goal, rationales), and align with namespace convention. Both versions auto-inject with identical applyTo patterns — removing unprefixed halves token injection cost (~10,800 tokens saved per context window).

**Risk:** LOW — pe-prefixed versions contain all rules from unprefixed versions plus metadata improvements.

---

### Change 2: Delete 2 unprefixed skill folders (HIGH) — ✅ DONE

**Folders to DELETE:**
- `.github/skills/artifact-coherence-check/` (entire folder)
- `.github/skills/prompt-engineering-validation/` (entire folder)

**Rationale:** Identical content to pe-prefixed versions. Duplicate skill descriptions cause non-deterministic skill discovery.

---

### Change 3: Add `00.03-metadata-contracts.md` to STRUCTURE-README (HIGH) — ✅ DONE

**File:** `.copilot/context/00.00-prompt-engineering/00.00-context-structure-index.md`

**Changes:**
- Add `00.03` to Tier 0 File Index table
- Add `00.03` to `governance` functional category
- Update file count from "29" to "31"
- Fix version history "18 categories" → "17 categories"

---

### Change 4: Update pe-meta agent metadata (MEDIUM) — ✅ DONE

**Files:**
- `.github/agents/00.09-pe-meta/pe-meta-researcher.agent.md` → version 1.1.0, last_updated 2026-04-28
- `.github/agents/00.09-pe-meta/pe-meta-validator.agent.md` → version 1.1.0, last_updated 2026-04-28

---

### Change 5: Update copilot-instructions.md references (MEDIUM) — ✅ DONE (no changes needed — no unprefixed PE names found)

**File:** `.github/copilot-instructions.md`

Update General Rules table to reference pe-prefixed instruction file names (after Change 1 removes unprefixed).

---

### Change 6: Delete legacy old/ directory in granular prompts (MEDIUM) — ✅ DONE

**Folder to DELETE:** `.github/prompts/00.02-pe-granular/old/`

**Contains:** 2 superseded prompt files with non-standard naming.

---

### Change 7: Document asymmetric prompt coverage (LOW) — ✅ DONE

**File:** `.copilot/context/00.00-prompt-engineering/05.03-pe-workflow-entry-points.md`

Add a note explaining that hooks and prompt-snippets intentionally have only create-update prompts (no design/review orchestration needed for simple artifact types).
