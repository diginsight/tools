# Plan — pe-meta-update apply: pe-meta-validator.agent.md

- **Run-id:** 20260612-pe-meta-validator-apply
- **Invocation:** `/pe-meta-update --mode=apply --scope=.github/agents/00.09-pe-meta/pe-meta-validator.agent.md --deps=full`
- **Breadth:** full · **Bundle:** single-domain · **Status:** done

## Approved changes

| # | Dimension | Severity | File | Action |
|---|-----------|----------|------|--------|
| 1 | D1/D14 | LOW | pe-meta-validator.agent.md | Trim `description` to one sentence < 200 chars |
| 2 | D6 | LOW | pe-meta-validator.agent.md | Add Test Scenario row 4 for `--audit` Coverage Audit mode |

## Version sync

- Bump bottom `agent_metadata.version` 2.5.0 → 2.5.1, `last_updated` → 2026-06-12
- No top-frontmatter `version` (agent files keep version in bottom block) — no desync to reconcile
- Add changelog entry v2.5.1

## Park lot

- Cohort-wide capability count (4–8) vs instruction "3–5" guideline — out of single-file scope; candidate for a future cohort/instruction reconciliation run.

## Regression (Phase 7)

- 4 capabilities baseline preserved; handoff `pe-meta-optimizer` resolves; version sync verified.
