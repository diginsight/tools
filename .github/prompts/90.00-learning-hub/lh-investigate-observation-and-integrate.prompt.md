---
name: lh-investigate-observation-and-integrate
description: "Single-entry workflow: investigate a user question, present results, then integrate into LearnHub (autonomous for clear gaps, gated for meta/architecture amendments)"
agent: agent
model: claude-opus-4.6
domain: "learning-hub"
tools:
  - read_file
  - list_dir
  - file_search
  - grep_search
  - semantic_search
  - fetch_webpage
  - run_playwright_code
  - vscode_askQuestions
  - create_file
  - replace_string_in_file
  - multi_replace_string_in_file
argument-hint: 'question="your observation/question" source="optional path to overview.md"'
---

# LH investigate observation and integrate

Run one complete flow from user question to integrated LearnHub outcome (autonomous for clear gaps; gated for meta/architecture amendments).

**📖 Workflow authority:** `.copilot/context/90.00-learning-hub/08-observation-to-integration-workflow.md`

## Purpose

1. Accept one user question as input.
2. Run triage + focused investigation.
3. Produce a decision-ready result package.
4. Integrate clear coverage gaps into LearnHub autonomously (additive tech content).
5. Gate only genuine judgment calls — meta/architecture amendments, overwrites, and scope conflicts.

## Boundaries

### Always do

1. Harvest current context (active file, sibling issues, repo) to seed candidate areas.
2. Start with local evidence, then expand to authoritative external sources.
3. Produce an existing-LearnHub coverage map before locking priority tracks.
4. Separate facts, assumptions, and open questions.
5. Produce a per-area in-depth analysis for every standard/deep track.
6. Contrast external approaches only when recommendation quality depends on workflow-pattern choice.
7. Persist all working/intermediate artifacts in the issue-folder's `_analysis/` working folder (non-published material), each marked `publish: false`.
8. Deliver a decision-ready result package (verdicts, coverage, conclusions, concise answer).
9. Integrate a clear coverage gap (`absent`, tech-article mode, additive) autonomously — decide placement and structure for consistency and least redundancy; do not gate it behind approval.
10. Derive each article's folder and numeric prefix from its taxonomy content-type via the subject-folder template (00 overview · 01 getting-started · 02 concepts · 03 how-to · 04 analysis · 05 reference · 06 resources; fractional `XX.YY-` for additional articles in one band) — but MATCH the target area's local convention when it differs (e.g. a `readme.md` index + `XX.YY-topic.md` articles).
11. Integrate every result fully into the corpus (placement, cross-links matching the local convention, redundancy consolidation, related-backlog closure).
12. Surface load-bearing deductions for challenge and re-derive from evidence on correction before locking conclusions.
13. Produce even-handed comparisons (similarities/differences/strengths/weaknesses) with inline provenance and vision-vs-implementation accuracy; general voice per `article-writing.instructions.md`.
14. Select the integration mode by detected impact — tech topic → article; visions or PE-artifact impact → a gated amendment plan — never asking which mode.
15. Assess source soundness before deep analysis (📖 `09-source-soundness-gate.md`) and emit `source_verdict`.
16. Give every article grounded in an identifiable external source (essay, article, blog post, deck, tool, or product) a **source-provenance callout** — a representative snapshot of the source placed adjacent to the canonical link (with its classification marker) plus a one-line description — where the source is introduced. Capture the snapshot with `run_playwright_code` (or another available page-capture capability): a **recognizable header/hero crop** that includes the source's title or key visual, sized as a **wide, short banner** so it never breaks reading flow — not a full-page dump. If capture is impossible, emit the callout with the standard image reference and flag a single one-time manual drop. Never bury the source in a bare inline link, and never leave provenance for silent manual addition. 📖 `08-observation-to-integration-workflow.md` § Source-provenance callout.
17. Reframe investigation framing into reader-facing framing on integration — an "understand X" investigation becomes an introduction to X and its capabilities, never a "problem statement" in the published article.
18. After integrating, complete the originating issue/observation file as a concise summary with references only to published integrated content and external sources — never a duplicate and never a pointer to working material.
19. Enforce one-way information flow: analysis MAY be synthesized into final content, but no publishable file may link to, name, or direct readers to `_analysis/`, any other intermediate working folder, `publish: false` artifacts, or an internal research trail.
20. Give the issue completion summary its own source-provenance callout — reuse the Step 10 snapshot, copied into the issue folder's own `images/` — whenever the observation is grounded in an identifiable external source, and present its findings as a connected sequence where each point states how it follows from the previous one, never a flat, disconnected list.

### Never do

- Never treat assumptions as facts.
- Never lock priority tracks before the coverage map exists.
- Never edit top YAML metadata of existing articles during investigation updates.
- Never gate a clear-gap, additive tech integration behind user approval — integrate it and report what changed.
- Never require approval except for genuine judgment calls: meta/architecture amendments, overwrites or restructures of existing content, and unresolved scope conflicts.
- Never carry investigation-centric framing (e.g. "Problem statement") into reader-facing integrated articles.
- Never introduce an identifiable external source in integrated content without a source-provenance callout (snapshot + classified link + one-line description), and never bury it in a bare inline link.
- Never duplicate generated article content back into the originating issue/observation file — summarize and link.
- Never ask the user to choose article numbering/positioning or whether to integrate — these are agent-owned decisions governed by LearnHub criteria.
- Never lock a challenged deduction without re-deriving it from evidence.
- Never frame comparisons as competitive ("ahead/behind") or label an implementation-maturity gap as a design gap.
- Never run deep analysis or integrate from an `insufficient` source, or integrate from an uncorroborated `promising-but-unverified` source.
- Never wire a working/intermediate artifact (anything under `_analysis/` or marked `publish: false`) into the site's render/include or navigation config — only the reader-facing published article is added to navigation.
- Never reference or link working/intermediate artifacts from a publishable file, even when the folder is excluded from navigation.

## Execution steps

1. Intake + context harvest.
2. Fast triage (seed areas from question AND context).
3. Existing-LearnHub coverage map (internal grounding + taxonomy).
4. Source-soundness gate (📖 `09-source-soundness-gate.md`) — emit `source_verdict`; stop on `insufficient`.
5. Prioritize tracks and depth.
6. Focused investigation (local-first, then external).
7. Per-area in-depth analysis (problem → conclusions + appendices).
8. External pattern contrast (conditional).
9. Proposed result package (decision-ready).
10. Integration decision by impact — clear gap + tech-article → integrate autonomously; meta/architecture impact → a gated amendment plan.
11. Tech integration — placement by local convention + taxonomy, reader-facing reframing, source-provenance callout (source snapshot + prominent classified link), and full cross-linking.
12. Issue completion — rewrite the originating observation as a summary with published references only, opening with its own source-provenance callout and a connected findings sequence, then validate zero `_analysis/` or `publish: false` links (no duplication).

## Artifact contract

Write/update under `<issue-folder>/_analysis/` (a working folder for non-published material; each file carries `publish: false`):

1. `01-triage-interest-map.md` (includes context-harvest signals)
2. `02-existing-coverage-map.md` (internal grounding: present/partial/absent + taxonomy)
3. `03-triage-priority-and-depth.md`
4. `04-investigation-backlog.md`
5. `05-analysis/` — one `<area-slug>.md` per standard/deep area
6. `06-external-approaches-contrast.md` (only when Step 7 applies)
7. `07-proposed-result-package.md`
8. `08-approval-and-integration-proposal.md` (integration record: placement, reader-facing reframing, provenance, and what changed; for meta/architecture impact, the gated amendment-plan reference)

Before returning, validate that persisted filenames match this contract and report any drift.

## Output contract

Return:

1. `triage_verdict`
2. `context_signals`
3. `coverage_map`
4. `source_verdict`
5. `priority_tracks`
6. `area_analyses`
7. `selected_workflow_pattern` (or `not_applicable`)
8. `proposed_result_package`
9. `integration_state` — `completed` (autonomous clear-gap tech integration) or `gated` (meta/architecture amendment awaiting approval)
10. `integration_result` — `taxonomy_mapping` + created paths in article mode, or an `amendment_plan` reference in meta/architecture mode
11. `issue_completion` — the summary-with-references written back to the originating observation
12. `artifacts_written`

<!--
prompt_metadata:
  version: "2.8.0"
  created: "2026-07-03"
  last_updated: "2026-08-04"
-->
