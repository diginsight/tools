---
title: "pe-meta-researcher.agent — change history"
description: "Per-version change history for pe-meta-researcher.agent."
last_updated: "2026-06-25"
status: "living"
---

# Change history — pe-meta-researcher.agent

## v2.3.1 — 2026-06-25

D1/D30-metadata-guard — normalized the bottom `agent_metadata:` block to the canonical `pe-agents.instructions.md` schema: reordered to `version`-first (`version, last_updated, created, changelog`) and removed the off-contract `created_by:` key. Scope-internal contract re-judgment — the `/pe-meta-review --scope .github/agents/00.09-pe-meta --mode apply` run was re-evaluated against the canonical agent contract alone (not the wider agent cohort), so `created_by:` is treated as off-contract here. No rule, capability, or boundary changed; non-breaking.

## v2.3.0 — 2026-06-12

Adopted metadata-precedence runtime grounding (vision v15.6.0). Added a single collective grounding directive to CRITICAL BOUNDARIES and removed body entries that restated YAML boundaries verbatim (config-load, output-shape selection, verbatim invocation echo, --source pass-through, trust classification, finding-to-artifact mapping, incremental new_anchors, never-modify-artifacts, never-skip-mapping, self-contained reports, shape mismatch, digest-shape rejection, non-catalog source). Three-tier entries are now additive only. Retired per-boundary bijection (H14).

## v2.2.2 — 2026-06-07

Restored canonical agent section structure (issue 20260607.01 plan 03 R3, human decision A) — reverted the pe-meta-cluster self-normalization that diverged from agent.template.md and the 6 non-pe-meta agents. Renamed `## Persona` → `## Your Expertise` and re-applied emoji boundary headers (`## 🚨 CRITICAL BOUNDARIES`, `### ✅ Always Do`, `### ⚠️ Ask First`, `### 🚫 Never Do`). Synced bottom-metadata version/last_updated to 2026-06-07.

## v2.2.1 — 2026-06-06

Refreshed the dimension-range reference from `D1-metadata` through `D27-model-adherence` to `D1-metadata` through `D35-portability-boundary` to match dimension catalog v1.5.0 (35 dimensions, D28-D35 reliability group).

## v2.2.0 — 2026-05-29

Rebased onto vision v14. Added Configuration source section (pe-self-update.config.json). Replaced single output template with three-shape output contract (snapshot/digest/window-digest) selected by derived breadth. Added --source pass-through filter. Added per-source state I/O (read for incremental, persist new_anchors[] for Phase 8). Retired v13.x --breadth catch-up lookback in favor of --start/--end resolving to bounded-delta. Replaced --no-external with --skip external. Added explicit rejection of manual digest-shape requests. Updated process phases 1, 2, 4, 5 to align with three-shape contract.

## v2.0.0 — 2026-03-08

Externalized report template, validation checklist, and Step 3 checklists to output-meta-researcher-report.template.md
