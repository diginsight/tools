---
title: "pe-meta-validator.agent — change history"
description: "Per-version change history for pe-meta-validator.agent."
last_updated: "2026-06-25"
status: "living"
---

# Change history — pe-meta-validator.agent

## v2.5.3 — 2026-06-25

D5-boundaries / D6-consistency — pinned the evidence-coverage `<applicable>` denominator to the `05.07` applicability matrix. Phase 0 step 3 now **resolves the applicable-dimension SET from `05.07`** (the full type-applicable set = the `<applicable>` denominator) and loads `05.08` only for the sub-checks of those dimensions; `--dim full`/omitted = the full set and `--dim` is subtractive. Added a boundary forbidding the `05.08` enumerated subset (≈8 structural rows) from being used as the applicable-set source — a reported `<applicable>` below the `05.07` count is a **collapsed-denominator hard-fail**. Closes the wiring drift where review runs sourced the denominator from `05.08` and reported a false-clean `pu-evidence=8/8`, defeating the shallow-sweep guard. Part of plan `20260625.07-pe-meta-update/01-improve-flow/01-applicable-dimension-set-no-collapse`. Non-breaking; patch bump.

## v2.5.2 — 2026-06-25

D1/D30-metadata-guard — removed the off-contract bottom-block fields `filename:` and `type:` from `agent_metadata:`. Cohort grep across all 21 `.github/agents/**/*.agent.md` files showed these two keys appear in only this file and pe-meta-builder (2-of-21 outlier), and are absent from both the `pe-agents.instructions.md` canonical schema and the repo-wide cohort norm. `filename` is redundant with the file path and `type` with the `.agent.md` extension; removal is deterministic and non-breaking. The block was also reordered to the canonical `version`-first field order (`version, last_updated, created, changelog`) per a scope-internal contract re-judgment. The `## Handoff Contract` heading was re-verified intact after the edit. Found by a `/pe-meta-review --scope .github/agents/00.09-pe-meta --mode apply` cohort-grounded metadata pass.

## v2.5.1 — 2026-06-12

Body-level refinements from a `/pe-meta-update --mode apply --deps full` single-file run. **D1/D14:** trimmed `description` to a single sentence under 200 chars (was ~256 chars, two sentences, and the cohort outlier) — the "fully self-contained" qualifier is preserved in `goal`. **D6-consistency:** added Test Scenario row 4 covering the `--audit` Coverage Audit mode (the v2.3.0 second-actor mode was documented in modes/capabilities/body but had no test scenario). No rule, capability, or boundary changed; non-breaking. Bottom-block `version`/`last_updated` synced (no top-frontmatter version on agent files). Cohort capability count (4–8 across pe-meta agents vs the instruction's "3–5") parked as out-of-scope for a single-file run.

## v2.5.0 — 2026-06-12

Adopted metadata-precedence runtime grounding (vision v15.6.0). Added a single collective grounding directive to CRITICAL BOUNDARIES and removed the body entries that restated YAML boundaries verbatim — this **reverses the v2.4.0 F2 change**, which had added four Always/Never body twins to satisfy the now-retired H14 bijection. Removed restatements: route-CRITICAL-escalation, exemplary-PE-for-PE bar, --dim/--with-deps parsing, coverage-marker recompute, finding-to-dimension mapping, Phase A-F ordering, lifecycle stage outputs, never-modify-files, never-approve-capability-break, never-delegate-structural, never-skip-applicable-dimension, never-inherit-orchestrator-markers. Three-tier entries are now additive only (deterministic-first, evidenced rows, D30 citation, never-approve-rationale-violations, D16 ordering). Retired per-boundary bijection (H14).

## v2.4.0 — 2026-06-11

Body-level structural + boundary refinements from the dimension-coverage-grading analysis (issue 20260607.02, doc 02). **F1 (D6-consistency):** `goal`/`description` now name the Coverage Audit mode alongside the three review modes — the v2.3.0 second-actor capability was present in capabilities/scope/body but absent from goal/description. **F2 (D5-boundaries, H14 alignment):** added the four missing Always/Never body-enforcement entries for YAML boundaries that had no body counterpart (route CRITICAL findings to human escalation; apply the exemplary PE-for-PE quality bar; emit structured stage outputs in lifecycle mode; never approve changes that break existing capabilities). **F3 (D19-artifact-structure, [M2] early-commands):** moved `## 🚨 CRITICAL BOUNDARIES` above `## Review Modes`/`## Knowledge Loading Contract` into the first ~30% of the body, and severity-ordered items within each tier (canonical Always/Ask/Never tier order preserved per v2.2.4). **F4 (D23-reference-efficiency):** dereferenced `## Coverage Audit Contract` — the Layer A/B mechanics are canonical in pe-meta-evidence-coverage.md, so the section now points to the snippet and keeps only agent-specific invocation detail (script path, compute-before-read ordering, acceptance). No capability removed; non-breaking. Frontmatter and bottom-block `version` synced.

## v2.3.1 — 2026-06-11

D14-craftsmanship fix — split the merged `## Knowledge Loading Contract` heading (the H2 title had run directly into its `Load from …` body sentence on one line, rendering a broken heading). Body-level structural finding from a `/pe-meta-update --mode apply --deps full` single-file run; no rule, capability, or boundary changed. Frontmatter and bottom-block `version`/`last_updated` synced together (the recurring desync discipline).

## v2.3.0 — 2026-06-07

Added Coverage Audit mode (issue 20260607.01 complex-plan R1) — the validator now serves as the independent SECOND actor for a `pe-meta-update` run, re-deriving `pu-evidence`/`shallow-sweep` from the outcome log without trusting the orchestrator's own markers (breaks the self-attestation loop; makes 'reconciled, NOT self-attested' literally true). Added the Coverage Audit Contract (Layer A deterministic via `pe-check-evidence-anchors.ps1`: resolvability + literal-containment + distinctness on every PU; Layer B reasoning: sample N weighted to D14/D28–D35 + on-doubt re-read). Added the matching review mode, capability, scope, Always-Do, and Never-Do (never inherit the orchestrator's verdict) entries.

## v2.2.4 — 2026-06-07

Restored canonical agent section structure (issue 20260607.01 plan 03 R3, human decision A) — reverted the pe-meta-cluster self-normalization that diverged from agent.template.md and the 6 non-pe-meta agents. Renamed `## Persona` → `## Your Expertise` and re-applied emoji boundary headers (`## 🚨 CRITICAL BOUNDARIES`, `### ✅ Always Do`, `### ⚠️ Ask First`, `### 🚫 Never Do`).

## v2.2.3 — 2026-06-07

Adopted the shared evidence-bound coverage contract (pe-meta-evidence-coverage.md) — Phase 8 now emits one EVIDENCED row per applicable dimension (passes included, not just findings); a silent PASS is no longer valid output. Added Always-Do rules for per-dimension evidence and D30 canonical-source citation (cite canonical metadata value + authority before recommending a key/field change). Added the matching quality-checklist item. Closes the shallow-sweep gap (2026-06-07 follow-up, I2/I4) where evidence-free PASSes were indistinguishable from unexercised dimensions. (Frontmatter `version:` field synced from a stale `2.2.2` to `2.2.3` by the agents-deps-full-20260607-r2 reconcile run — this entry's body changes had already shipped but the frontmatter field was missed.)

## v2.2.2 — 2026-06-07

Renamed the bottom metadata key from `article_metadata:` to `agent_metadata:` — `article_metadata:` is the article convention; agents use `agent_metadata:`, the majority key among the in-scope pe-meta siblings (3 of 5: designer, optimizer, researcher). Corrects the v2.2.1 premise below: that entry claimed the `article_metadata` block matched the four sibling agents, but the siblings actually use `agent_metadata:`, so v2.2.1 left this file on the wrong key. Found by the 2026-06-07 follow-up re-audit (R1, D30-metadata-guard).

## v2.2.1 — 2026-06-07

Corrected residual `scope.covers` entry from `27-dimension assessment` to `35-dimension assessment` (the 2026-06-06 v2.2.0 refresh updated description/goal/capabilities/boundaries but missed this one line — internal D6 contradiction). Added `version`/`last_updated` to the bottom metadata block (D1 metadata sync the v2.2.0 F3 pass left out). NOTE: the original v2.2.1 text claimed this matched the four sibling agents on the `article_metadata` key — that premise was wrong (siblings use `agent_metadata:`); see v2.2.2. Found during a `--deps full` re-review run agents-deps-full-20260607.

## v2.2.0 — 2026-06-06

Refreshed dimension coverage from 27 to 35 dimensions (catalog v1.5.0). Updated description, goal, capabilities, and the finding-mapping boundary from `D1-metadata` through `D27-model-adherence` to `D1-metadata` through `D35-portability-boundary`. Added a Reliability Pass (Phase 4b) covering `D28-reproducibility` through `D35-portability-boundary` and a corresponding scope.covers entry. Replaced the stale fixed dim count in test scenario 2 with the applicability-matrix reference.

## v2.1.1 — 2026-05-21

Aligned domain with PE agent contract, reduced capabilities to 5, added explicit handoff contract table, tightened process wording for token reduction, and fixed non-resolving test scenario filename.

## v2.0.0 — 2026-05-15

REWRITE — removed all 8 pe-gra validator handoffs, internalized structural checks, added 27-dimension dispatch with --dim parameter, added --with-deps dependency-aware mode, added D17 peer mode for context files, added Phase A-F system-wide ordering, added dimension applicability matrix, added exemplary quality bar enforcement. Aligned with vision v12.

## v1.1.0 — 2026-04-28

Added slash-command reference check to validation, added handoff data contracts

## v1.0.0 — 2026-03-20

Initial version with 3 modes (design/implementation/ecosystem) delegating to pe-gra validators
