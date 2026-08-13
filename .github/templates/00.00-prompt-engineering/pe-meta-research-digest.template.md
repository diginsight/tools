---
description: "Output template for pe-meta-researcher when derived breadth=incremental — change digest since per-source last_review_timestamp."
---

<!-- Category: output -->
<!-- Audience: agent (pe-meta-review orchestrator consumes this digest) -->
<!-- v14: emitted when derived breadth=incremental (trigger-fired callers only) -->

# PE Meta-Research Output — Change Digest Template (derived `breadth=incremental`)

> Use this template when the orchestrator passes derived `breadth=incremental` (trigger-fired caller, no `--start`/`--end`). The output is a **change digest** of every observation since each source's `last_review_timestamp` recorded in `<state.path>/triggers/<source-id>.json`.

## Required output frontmatter

```yaml
---
output_shape: "digest"
derived_breadth: "incremental"
resolved_invocation: "<verbatim Resolved invocation line passed by orchestrator>"
researcher_run_id: "<ISO-8601 timestamp>"
sources_consulted: [list of source-id strings]
external_skipped: true|false
per_source_anchors:
  - source_id: "<id>"
    last_review_timestamp: "<ISO-8601 read from triggers/<id>.json>"
    last_digest_hash: "<hash from triggers/<id>.json — empty string if first run>"
---
```

## Required sections

### 1. Resolved invocation echo

First line of the body MUST be the verbatim `Resolved invocation: …` log passed by the orchestrator.

### 2. Per-source change entries (`digest.entries[]`)

For each source consulted, emit ONLY change entries observed since `last_review_timestamp`. If a source has no changes in the window, include an explicit empty entry:

```yaml
digest:
  entries:
    - source_id: "<id>"
      since: "<last_review_timestamp from triggers/<id>.json>"
      observed_at: "<ISO-8601 of researcher run>"
      changes:
        - description: "<one-sentence change>"
          evidence: "<URL or quote>"
          affects_artifact_types: ["..."]
          quality_dimensions: ["..."]
          confidence: "high|medium|low"
        # OR (no changes):
      # changes: []
    - source_id: "<next-id>"
      ...
```

### 3. Missing-state warnings

If any source's `triggers/<id>.json` was missing on this run, list it here. The orchestrator treats those sources as a per-source `full` sweep while keeping global derived breadth at `incremental`.

| source-id | reason | treated-as |
|---|---|---|
| `<id>` | no prior state file | full-sweep-for-this-source |

### 4. New `last_digest_hash` per source

Emit a fresh `last_digest_hash` value (deterministic over the change-entries content) per source. Phase 8 persists this back to `<state.path>/triggers/<source-id>.json`:

```yaml
new_anchors:
  - source_id: "<id>"
    last_digest_hash: "<sha256-or-similar>"
```

### 5. Aggregated improvement opportunities

Same per-opportunity rich table as the snapshot template, but limited to opportunities arising from the digest entries (NOT a full catalog rescan).

## Boundaries

- MUST NOT emit a full PE structural inventory (that belongs in the snapshot shape).
- MUST NOT include `window.start`/`window.end` fields (those belong in the window-digest shape).
- MUST emit `digest.entries[]` for EVERY source listed in `sources_consulted`, even when empty.
- MUST emit `new_anchors[]` so Phase 8 has fresh values to persist after successful applies.
- MUST be reachable ONLY when caller-type is `trigger-fired` (orchestrator enforces this — researcher rejects manual callers requesting this shape).

<!--
template_metadata:
  filename: "pe-meta-research-digest.template.md"
  version: "1.0.0"
  last_updated: "2026-05-29"
  purpose: "v14 research output shape selected when orchestrator passes derived breadth=incremental. One of three shapes in the research output contract."
  consumers:
    - "pe-meta-review.prompt.md (Phase 1 1a, Phase 4 screening input, Phase 8 anchor persistence)"
    - "pe-meta-researcher.agent.md (Phase 5 output emission)"
-->
