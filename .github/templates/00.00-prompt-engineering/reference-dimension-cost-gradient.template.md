---
title: "Reference — Dimension Cost Gradient"
description: "Estimated cost per --dim invocation lifted from 05.07-pe-meta-dimension-catalog.md. Reference table — load only when cost estimation is needed."
domain: "prompt-engineering"
parent_artifact: ".copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md"
goal: "Provide the per-invocation cost gradient as a load-on-demand reference, keeping the parent catalog under the context token budget."
scope:
  covers:
    - "Cost gradient table: 11 --dim invocations from metadata-only to full --with-deps"
  excludes:
    - "Dimension specifications (live in parent catalog)"
    - "Applicability matrix (lives in sibling reference)"
    - "Dimension-to-handler mapping (lives in parent catalog)"
boundaries:
  - "MUST stay byte-identical to the cost gradient table in the parent catalog at extraction time"
  - "MUST be updated atomically with the parent catalog when dimension groups or costs change"
rationales:
  - "Cost estimates are reference data, queried infrequently during planning, not during execution"
  - "Extraction contributes to bringing the parent catalog under the 2,500-token hard ceiling"
  - "Load-on-demand via 📖 reference preserves accessibility without paying the inline cost on every context load"
---

# Reference — Dimension Cost Gradient

**Parent artifact:** [05.07-pe-meta-dimension-catalog.md](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md)

This reference holds the estimated cost per `--dim` invocation. Use it when planning a review to budget LLM cost — the parent catalog's Dimension groups section already gives the dimension membership of each group.

## Cost gradient

| Invocation | Dimensions | Estimated cost |
|---|---|---|
| `--dim metadata` | `D1-metadata` only | Near-zero (deterministic) |
| `--dim references` | `D2-references` only | Near-zero (deterministic) |
| `--dim structural` | `D1-metadata` through `D5-boundaries`, `D14-craftsmanship` | Low (mostly deterministic) |
| `--dim quality` | `D6-consistency` through `D11-actionability`, `D27-model-adherence` | Medium (LLM reasoning) |
| `--dim efficiency` | 11 dimensions | Medium-high (mixed) |
| `--dim optimize` | 10 dimensions | Medium-high (mixed; delegates to @meta-optimizer during apply) |
| `--dim context-health` | `D6-consistency` through `D12-staleness`, `D22-context-optimization` | Medium-high (quality + organization) |
| `--dim context-full` | 16 dimensions | High (full context assessment) |
| `--dim reliability` | `D28-reproducibility` through `D35-portability-boundary` (8 dimensions) | Medium-high (mostly reasoning; some deterministic for metadata-guard and reproducibility checks) |
| `--dim full` | All 35 | High (full assessment) |
| `--dim full --with-deps` | All 35 + dependency chain | Very high (multi-artifact) |

**Parent artifact:** [05.07-pe-meta-dimension-catalog.md](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md)

<!--
template_metadata:
  version: "1.0.0"
  last_updated: "2026-05-25"
-->
