---
description: "Output template for pe-meta-researcher when derived breadth=bounded-delta — change digest within explicit --start/--end window."
---

<!-- Category: output -->
<!-- Audience: agent (pe-meta-review orchestrator consumes this window digest) -->
<!-- v14: emitted when derived breadth=bounded-delta (any --start or --end present) -->

# PE Meta-Research Output — Bounded-Window Digest Template (derived `breadth=bounded-delta`)

> Use this template when the orchestrator passes derived `breadth=bounded-delta` (any caller, with `--start` and/or `--end` present). The output is a **change digest bounded by explicit endpoints** — NOT by per-source state.

## Required output frontmatter

```yaml
---
output_shape: "window-digest"
derived_breadth: "bounded-delta"
resolved_invocation: "<verbatim Resolved invocation line passed by orchestrator>"
researcher_run_id: "<ISO-8601 timestamp>"
sources_consulted: [list of source-id strings]
external_skipped: true|false
window:
  start: "<YYYY-MM-DD or ISO-8601>"   # explicit endpoint from caller (or default lookback if start omitted)
  end:   "<YYYY-MM-DD or ISO-8601>"   # explicit endpoint from caller (or 'now' if end omitted)
  resolved_default_used: "start|end|none"   # which endpoint(s) defaulted to lookback.default_days or 'now'
---
```

## Required sections

### 1. Resolved invocation echo

First line of the body MUST be the verbatim `Resolved invocation: …` log passed by the orchestrator, including the explicit `--start=…` and `--end=…` values.

### 2. Window declaration

Restate the explicit window endpoints AND whether either defaulted from `pe-self-update.config.json` → `lookback.default_days`:

```yaml
window:
  start: "<value>"
  end:   "<value>"
  span_days: <integer>
  defaulted: ["start"] | ["end"] | ["start", "end"] | []
```

### 3. Per-source window entries (`window.entries[]`)

For each source consulted, emit ONLY change entries observed within `[window.start, window.end]`. If a source has no changes in the window, include an explicit empty entry:

```yaml
window:
  entries:
    - source_id: "<id>"
      observed_at: "<ISO-8601 of researcher run>"
      changes:
        - description: "<one-sentence change>"
          evidence: "<URL or quote>"
          observed_date: "<ISO-8601 within window>"
          affects_artifact_types: ["..."]
          quality_dimensions: ["..."]
          confidence: "high|medium|low"
      # OR (no changes in window):
      # changes: []
```

### 4. Aggregated improvement opportunities

Same per-opportunity rich table as the snapshot template, but limited to opportunities arising from changes within the bounded window.

## Boundaries

- MUST emit `window.start` and `window.end` fields (those distinguish this shape from the digest shape).
- MUST NOT emit per-source `last_review_timestamp` anchors (that belongs in the incremental digest shape — Phase 8 does NOT advance per-source anchors for bounded-delta runs).
- MUST NOT emit a full PE structural inventory (that belongs in the snapshot shape).
- MUST emit `window.entries[]` for EVERY source listed in `sources_consulted`, even when empty.
- Manual callers reach this shape when they pass `--start` and/or `--end`; trigger-fired callers reach this shape when they pass an override window.
- This shape REPLACES the v13.x `--breadth catch-up` lookback mode (which is retired).

<!--
template_metadata:
  filename: "pe-meta-research-window-digest.template.md"
  version: "1.0.0"
  last_updated: "2026-05-29"
  purpose: "v14 research output shape selected when orchestrator passes derived breadth=bounded-delta (any --start/--end). One of three shapes in the research output contract. Replaces v13.x --breadth catch-up lookback behavior."
  consumers:
    - "pe-meta-review.prompt.md (Phase 1 1a, Phase 4 screening input)"
    - "pe-meta-researcher.agent.md (Phase 5 output emission)"
-->
