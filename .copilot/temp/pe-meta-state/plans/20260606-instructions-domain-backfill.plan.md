---
status: done
target_vision_version: "15.4.0"
domain: "prompt-engineering"
created: "2026-06-06"
goal: "Backfill the required `domain:` frontmatter field on 13 instruction files under .github/instructions/ that are missing it, per pe-instruction-files.instructions.md [C6] and 00.03-metadata-contracts.md. Each edit also bumps version (minor) and sets last_updated to 2026-06-06."
run_id: "instructions-apply-20260606"
resolved_invocation: "--mode=apply --scope=.github/instructions/ --source= --dim=full --start=none --end=none --deps=none --skip= | breadth=full | caller=manual | plan-file=.copilot/temp/pe-meta-state/plans/20260606-instructions-domain-backfill.plan.md | bundle=accepted-bundle"
---

# Plan — instruction-file `domain:` backfill

## Goal

Add the required `domain:` scalar to the 13 instruction files missing it. Classification: LOW severity, Tier 1 deterministic (additive required metadata field), non-breaking (goal/scope/boundaries preserved). Approved by user 2026-06-06.

## Goal table

| # | File | scope tag | principle impact | downstream landing | edit |
|---|---|---|---|---|---|
| 1 | pe-agents.instructions.md | [metadata-fix] | none (additive) | Phase 0b Tier-1 resolution | add `domain: "prompt-engineering"`; version 1.10.0→1.11.0; last_updated→2026-06-06 |
| 2 | pe-prompts.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.5.0→1.6.0; →2026-06-06 |
| 3 | pe-skills.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.5.1→1.6.0; →2026-06-06 |
| 4 | pe-instruction-files.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.7.0→1.8.0; →2026-06-06 |
| 5 | pe-common.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.7.0→1.8.0; →2026-06-06 |
| 6 | pe-context-files.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.5.1→1.6.0; →2026-06-06 |
| 7 | pe-copilot-instructions-file.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.2.1→1.3.0; →2026-06-06 |
| 8 | pe-hooks.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.4.0→1.5.0; →2026-06-06 |
| 9 | pe-prompt-snippets.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.4.1→1.5.0; →2026-06-06 |
| 10 | pe-templates.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.5.1→1.6.0; →2026-06-06 |
| 11 | plan-marking.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "prompt-engineering"`; 1.2.0→1.3.0; →2026-06-06 |
| 12 | article-writing.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "article-writing"`; 1.3.1→1.4.0; →2026-06-06 |
| 13 | documentation.instructions.md | [metadata-fix] | none | Phase 0b Tier-1 | add `domain: "article-writing"` (no `documentation` domain registered; user approved); 1.9.0→1.10.0; →2026-06-06 |

## Park lot

- **[H10] applyTo overlaps** (advisory, NOT in this plan's scope): `plan-execution`/`plan-marking` share `*plan*`; `documentation`⊃`article-writing`; `vision-amendment`⊂`vision-frontmatter`; `use-case-documents`⊂`documentation`. Architectural / HIGH-risk — deferred for a dedicated decision.

## Exit criteria

- All 13 files carry a valid single-scalar `domain:` field.
- No file declares the reserved `domain: "unknown"`.
- Each edited file has version bumped (minor) and last_updated = 2026-06-06.
- Phase 7 regression: re-grep frontmatter confirms 17/17 instruction files now declare `domain:`.
