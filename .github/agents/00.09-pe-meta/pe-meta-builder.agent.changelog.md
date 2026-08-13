---
title: "pe-meta-builder.agent — change history"
description: "Per-version change history for pe-meta-builder.agent."
last_updated: "2026-06-25"
status: "living"
---

# Change history — pe-meta-builder.agent

## v1.1.1 — 2026-06-25

D1/D30-metadata-guard — removed the off-contract bottom-block fields `filename:` and `type:` from `agent_metadata:`. Cohort grep across all 21 `.github/agents/**/*.agent.md` files showed these two keys appear in only this file and pe-meta-validator (2-of-21 outlier), and are absent from both the `pe-agents.instructions.md` canonical schema and the repo-wide cohort norm (pe-gra siblings carry `created/created_by/version/last_updated/changelog`, never `filename`/`type`). `filename` is redundant with the file path and `type` with the `.agent.md` extension; removal is deterministic and non-breaking. The block was also reordered to the canonical `version`-first field order (`version, last_updated, created, changelog`) per a scope-internal contract re-judgment. The repo-wide `created_by:` key (which this file does not carry) was treated as off-contract for the pe-meta scope. Found by a `/pe-meta-review --scope .github/agents/00.09-pe-meta --mode apply` cohort-grounded metadata pass.

## v1.1.0 — 2026-06-12

Adopted metadata-precedence runtime grounding (vision v15.6.0). Added a single collective grounding directive to CRITICAL BOUNDARIES and removed body entries that restated YAML boundaries verbatim (pre-change guard, post-change reconciliation, complete-file read, 3-5 line context, validator handoff, dimension mapping, 3-iteration cap, construction invariants). Three-tier entries are now additive only. Retired per-boundary bijection (H14).

## v1.0.4 — 2026-06-07

Reverted the v1.0.3 'match the four siblings' normalization (issue 20260607.01 plan 03 R3, human decision A). Repo-wide evidence showed the 5 pe-meta agents — not the templates — were the outlier: agent.template.md and all 6 non-pe-meta agents use `## Your Expertise` + emoji boundary headers, so v1.0.3 aligned to a self-normalized minority (the same match-neighbours-vs-match-canonical trap as the article_metadata case). Restored `## Persona` → `## Your Expertise` and emoji boundary headers (`## 🚨 CRITICAL BOUNDARIES`, `### ✅ Always Do`, `### ⚠️ Ask First`, `### 🚫 Never Do`).

## v1.0.3 — 2026-06-07

Normalized section structure to match the four pe-meta sibling agents (designer, optimizer, researcher, validator): renamed `## Your Expertise` to `## Persona` with terse working-stance bullets, and stripped emoji from the four boundary headers (`## 🚨 CRITICAL BOUNDARIES` → `## Critical Boundaries`, `### ✅ Always Do` → `### Always Do`, `### ⚠️ Ask First` → `### Ask First`, `### 🚫 Never Do` → `### Never Do`). Resolves D6-consistency / D19-artifact-structure 4-vs-1 divergence (builder was sole outlier carrying GRA-style emoji headers from its 2026-05-15 creation). Found by the 2026-06-07 body-level structural pass that prior frontmatter-clustered passes missed (B1, B2).

## v1.0.2 — 2026-06-07

Renamed bottom metadata key `article_metadata:` to `agent_metadata:` to match the in-scope pe-meta sibling majority (R1, D30-metadata-guard). Added an explicit rollback boundary declaring git history plus any designer-supplied per-change rollback strategy as the deliberate, consistent rollback path — closes the D32-rollback-readiness gap where this mutating agent declared no rollback path while sibling optimizer did (R3). Found by the 2026-06-07 follow-up re-audit.

## v1.0.1 — 2026-06-06

Corrected domain metadata from meta-operations to prompt-engineering to match the four sibling pe-meta agents (designer, optimizer, researcher, validator) in the same folder; resolves the Phase 0b multi-domain gate caused by an anomalous single-file domain value. Also refreshed the dimension-range reference D1-D27 to D1-D35 (dimension catalog v1.5.0).
