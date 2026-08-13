# Plan — PE meta-review (apply) · prompt-engineering context folder

- **Run ID**: `20260625-pe-context-full-apply`
- **Resolved invocation**: `--mode=apply --scope=.copilot/context/00.00-prompt-engineering/ --source=<all> --dim=full --start=none --end=none --deps=none --skip=<none> | breadth=full | caller=manual | bundle=single-domain`
- **Phase 0b**: single prompt-engineering domain folder → `bundle=single-domain` (no domain-coherence gate)
- **Coverage**: 44/44 context files body-audited (5 parallel read-only Explore subagents) + deterministic metadata/registration sweep
- **Dimension contract**: 35 dims (catalog `05.07`); context-applicable subset exercised (D1, D2, D3, D6, D7, D8, D9, D10, D11, D12, D13, D14, D15, D17, D19, D22, D30, D35)
- **Status**: done

---

## Registration / structural integrity — CLEAN

- `00.04-context-category-catalog.md` (v2.4.0) — all 44 files registered across 23 categories; historical `04.02` gap closed. ✅
- `00.00-context-structure-index.md` (v5.2.0) — `required_categories` (23) matches catalog. ✅
- `05.07-pe-meta-dimension-catalog.md` — 35-dim disjoint partition (18+8+1+8) verified. ✅

---

## Findings

### Confirmed — auto-apply (LOW, deterministic)

| ID | File | Dim | Severity | Evidence anchor | Fix |
|----|------|-----|----------|-----------------|-----|
| F1 | `03.02-model-specific-optimization.md` | D14-craftsmanship | LOW | L36 Purpose: `…which model processes it. — the same prompt produces different results depending on which model processes it.` (duplicated clause) | Remove the duplicated second clause; keep one sentence ending in a period. |
| F2 | `04.01-validation-caching-pattern.md` | D14-craftsmanship | LOW | L111-113: `- Γ£à Good:` / `- Γ¥î Bad:` (4 occurrences) — CP1252-corrupted UTF-8 | `Γ£à`→`✅`, `Γ¥î`→`❌` |
| F3 | `05.08-pe-meta-type-checklists.md` | D2-references / D10-completeness | LOW | Prompt-files table (L113+) omits the `All markdown links resolve` row present in the Context (L57-58) and Agent (L80-81) tables; D2 spec (`05.07` L48) covers markdown links for all types | Add `All markdown links resolve` row to the Prompt files table. |

### Escalate — discretionary (require user decision; NOT auto-applied)

| ID | File | Dim | Severity | Observation | Options |
|----|------|-----|----------|-------------|---------|
| E1 | `00.06-folder-metadata-inheritance.md` | D1-metadata / D30 | MEDIUM | Bottom block carries `status: "proposed"` — not in the canonical 00.03 two-field contract (`version` + `last_updated`); also redundant with the header `> Status: PROPOSED` blockquote. File is an intentional PROPOSED spec. | (a) leave as-is; (b) remove the field (header blockquote already conveys status); (c) formally add `status` to the 00.03 contract. |
| E2 | `04.05-pe-meta-invocation-gates.md` | D1-metadata | LOW | Bottom block uses an extended schema (`filename`, `created`, `purpose`) with `version` ordered last, vs the canonical 2-field block. | (a) leave; (b) normalize to canonical `version`/`last_updated` ordering and drop extras. |

### Rejected — false positives / non-defects (no change)

| File | Claimed | Reason rejected |
|------|---------|-----------------|
| `00.03`, `00.05` | `last_updated` "lags calendar date" | `last_updated` reflects the last **content** change, not the current date — bumping without an edit is wrong. |
| `05.01` / `05.04` | "shared version 5.5.2" | Independent files coincidentally at the same SemVer is not a defect. |
| `00.01` | "ghost version 2.0.3" | Accurate historical note inside the v2.1.1 changelog row describing a past bottom-block desync; version-history table correctly omits it. |
| `05.07` L51 | `≥5/2/3` vs `≥5/≥2/≥3` | Understandable shorthand; cosmetic. |
| `03.01` | missing `## Version History` table | Not contract-required; the bottom `context_metadata` block is the canonical version record. |
| `00.02` | "B0" jargon undefined | Plan-workstream shorthand inside changelog/narrative — acceptable in historical prose. |
| `05.04` | `last_updated` staleness vs entries | Absorbed by this run's mandatory Phase 8 log append. |

---

## Apply order

1. F1 — `03.02` duplicated clause (single replace) → re-read → bump bottom-block version (patch) + `last_updated` 2026-06-25.
2. F2 — `04.01` mojibake (multi-replace, 4 glyphs) → re-read → bump version + `last_updated`.
3. F3 — `05.08` add table row → re-read → bump version + `last_updated`.
4. Phase 7 regression (`get_errors` on edited files).
5. Phase 8 — append run entry to `05.04-meta-review-log.md` (bump its version/last_updated); write outcome JSONL.
