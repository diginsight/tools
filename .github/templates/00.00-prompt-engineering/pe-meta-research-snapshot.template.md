---
description: "Output template for pe-meta-researcher when derived breadth=full — current-state snapshot of monitored sources catalog and PE structural inventory."
---

<!-- Category: output -->
<!-- Audience: agent (pe-meta-review orchestrator consumes this snapshot) -->
<!-- v14: emitted when derived breadth=full -->

# PE Meta-Research Output — Snapshot Template (derived `breadth=full`)

> Use this template when the orchestrator passes derived `breadth=full` (manual caller, no `--start`/`--end` window). The output is a **current-state snapshot** of the monitored-sources catalog plus a full PE-artifact structural inventory.

## Required output frontmatter

```yaml
---
output_shape: "snapshot"
derived_breadth: "full"
resolved_invocation: "<verbatim Resolved invocation line passed by orchestrator>"
researcher_run_id: "<ISO-8601 timestamp>"
sources_consulted: [list of source-id strings from pe-self-update.config.json]
external_skipped: true|false
---
```

## Required sections

### 1. Resolved invocation echo

First line of the body MUST be the verbatim `Resolved invocation: …` log passed by the orchestrator.

### 2. Catalog snapshot

For EVERY monitored source listed in `pe-self-update.config.json` → `monitored_sources.*`, emit a row in the snapshot table:

| source-id | kind | current-version-or-state | last-observed-change | pe-relevance |
|---|---|---|---|---|
| `<id>` | platform/model/ecosystem | `<version>` | `<ISO-8601>` | yes/no |

### 3. PE-relevant changes (`pe-relevant-changes[]`)

For each source whose snapshot row has `pe-relevance: yes`, enumerate observed surface or capability changes:

```yaml
pe-relevant-changes:
  - source_id: "<id>"
    change: "<one-sentence description>"
    evidence: "<URL or direct quote>"
    affects_artifact_types: ["context", "instructions", "agents", "prompts", "skills", "templates", "hooks", "snippets"]
    quality_dimensions: ["robustness", "effectiveness", "token-efficiency", "time-efficiency"]
    confidence: "high|medium|low"
```

This list is the primary handoff to Phase 4's screening step.

### 4. Structural inventory

Full PE artifact catalog count (one row per artifact type, all 8 types). Compare on-disk count against the canonical inventory in `00.01-governance-and-capability-baseline.md`:

| artifact-type | expected | found | mismatches |
|---|---|---|---|

### 5. Symmetry check (builder ↔ validator pairings)

Every artifact-type that requires a builder/validator pair (per architecture) MUST appear in this table:

| artifact-type | builder | validator | symmetric? |
|---|---|---|---|

### 6. Orphan detection

Every artifact MUST have at least one declared consumer in the dependency map; rows in this table are violations:

| file | type | referenced-by | status |
|---|---|---|---|
| `<path>` | `<type>` | none | orphan |

### 7. Context coverage gaps

| rule-or-concept | canonical-context-file | status |
|---|---|---|

### 8. Improvement opportunities

Same rich per-opportunity table as the legacy meta-researcher report (`output-meta-researcher-report.template.md` § PE Improvement Opportunities). Phase 1.5 Organizational Pass and Phase 4's screening step read this section.

## Boundaries

- MUST NOT include change-digest entries (use `pe-meta-research-digest.template.md` for that shape).
- MUST NOT include `window.start`/`window.end` fields (use `pe-meta-research-window-digest.template.md` for that shape).
- MUST include the `pe-relevant-changes[]` array even when empty (set to `[]`).

<!--
template_metadata:
  filename: "pe-meta-research-snapshot.template.md"
  version: "1.0.0"
  last_updated: "2026-05-29"
  purpose: "v14 research output shape selected when orchestrator passes derived breadth=full. One of three shapes in the research output contract."
  consumers:
    - "pe-meta-review.prompt.md (Phase 1 1a, Phase 1.5 screening input, Phase 4 screening input)"
    - "pe-meta-researcher.agent.md (Phase 5 output emission)"
-->
