---
title: "pe-meta-optimizer.agent — change history"
description: "Per-version change history for pe-meta-optimizer.agent."
last_updated: "2026-06-25"
status: "living"
---

# Change history — pe-meta-optimizer.agent

## v1.2.1 — 2026-06-25

D1/D30-metadata-guard — normalized the bottom `agent_metadata:` block to the canonical `pe-agents.instructions.md` schema: reordered to `version`-first (`version, last_updated, created, changelog`) and removed the off-contract `created_by:` key. Scope-internal contract re-judgment — the `/pe-meta-review --scope .github/agents/00.09-pe-meta --mode apply` run was re-evaluated against the canonical agent contract alone (not the wider agent cohort), so `created_by:` is treated as off-contract here. No rule, capability, or boundary changed; non-breaking.

## v1.2.0 — 2026-06-12

Adopted metadata-precedence runtime grounding (vision v15.6.0). Added a single collective grounding directive to CRITICAL BOUNDARIES and removed body entries that restated YAML boundaries verbatim (one-file-at-a-time, preserve-rules, remove-canonical, files-outside-scope, skip-revalidation, 3-iteration cap). Three-tier entries are now additive only. Retired per-boundary bijection (H14).

## v1.1.4 — 2026-06-07

Restored canonical agent section structure (issue 20260607.01 plan 03 R3, human decision A) — reverted the pe-meta-cluster self-normalization that diverged from agent.template.md and the 6 non-pe-meta agents. Renamed `## Persona` → `## Your Expertise` and re-applied emoji boundary headers (`## 🚨 CRITICAL BOUNDARIES`, `### ✅ Always Do`, `### ⚠️ Ask First`, `### 🚫 Never Do`). Synced bottom-metadata version/last_updated.

## v1.1.3 — 2026-06-07

Restored four `→` arrows corrupted to `?` (CP1252) in the Test Scenarios table (R2, D14-craftsmanship). Reconciled Phase 1.5 from bespoke `.copilot/temp/rollback/` snapshots to git-history-based rollback so the rollback strategy is consistent with sibling pe-meta mutating agents (R3) — the designer-supplied per-change rollback strategy plus git is now the single deliberate path across all steps. Found by the 2026-06-07 follow-up re-audit.

## v1.1.2 — 2026-06-06

Synced bottom-metadata version/last_updated to the authoritative frontmatter values (were stale at 1.0 / 2026-03-20).
