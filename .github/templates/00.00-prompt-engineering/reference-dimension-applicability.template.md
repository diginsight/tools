---
title: "Reference — Dimension Applicability Matrix"
description: "The 35-dimension × 8-artifact-type applicability matrix lifted from 05.07-pe-meta-dimension-catalog.md. Reference table — load only when applicability data is needed."
domain: "prompt-engineering"
parent_artifact: ".copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md"
goal: "Provide the full 35×8 dimension-to-artifact-type applicability matrix as a load-on-demand reference, keeping the parent catalog under the context token budget."
scope:
  covers:
    - "Applicability matrix for `D1-metadata` through `D35-portability-boundary` across Context, Instruction, Agent, Prompt, Skill, Template, Hook, Snippet"
    - "Per-artifact-type total counts"
    - "Footnote on `D17-cross-coherence` for context files (peer-review mode)"
  excludes:
    - "Dimension specifications (live in parent catalog Dimension table)"
    - "Dimension groups/shortcuts (live in parent catalog)"
    - "Cost gradient and handler mapping (live in parent catalog or sibling reference)"
boundaries:
  - "MUST stay byte-identical to the matrix in the parent catalog at extraction time — this template is a verbatim mirror, not a fork"
  - "MUST be updated atomically with the parent catalog when dimensions or artifact types change"
rationales:
  - "The 35×9 matrix is reference data — most consumers only need decision points, not the full grid"
  - "Extraction reduces parent catalog by ~500 tokens, bringing it under the 2,500-token hard ceiling"
  - "Load-on-demand via 📖 reference preserves accessibility without paying the inline cost on every context load"
---

# Reference — Dimension Applicability Matrix

**Parent artifact:** [05.07-pe-meta-dimension-catalog.md](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md)

This reference holds the full applicability matrix that maps each of the 35 review dimensions to the 8 PE artifact types. Use it when you need to know which dimensions apply to a given artifact type — for routine review work, the parent catalog's Dimension table, Dimension groups, and Cost gradient are usually sufficient.

## Dimension applicability matrix

| Dimension | Context | Instruction | Agent | Prompt | Skill | Template | Hook | Snippet |
|---|---|---|---|---|---|---|---|---|
| `D1-metadata` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `D2-references` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `D3-token-budget` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ✅ |
| `D4-tool-alignment` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D5-boundaries` | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `D6-consistency` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `D7-non-redundancy` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `D8-prioritization` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `D9-clarity` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `D10-completeness` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `D11-actionability` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `D12-staleness` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `D13-source-verification` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `D14-craftsmanship` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `D15-vision-alignment` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| `D16-adherence` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D17-cross-coherence` | ✅* | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D18-coverage` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D19-artifact-structure` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| `D20-token-chain` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D21-deterministic-first` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D22-context-optimization` | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `D23-reference-efficiency` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D24-handoff-efficiency` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D25-processing-efficiency` | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D26-model-routing` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D27-model-adherence` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D28-reproducibility` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D29-regression-protection` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| `D30-metadata-guard` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `D31-multipass-validation-invariant` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D32-rollback-readiness` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D33-boundary-actionability` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D34-autonomy-calibration` | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| `D35-portability-boundary` | ✅ | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Total** | **18** | **14** | **30** | **31** | **8** | **8** | **5** | **6** |

*`D17-cross-coherence` for context files = peer-review mode (checks against peer context files loaded by same consumers)

**Parent artifact:** [05.07-pe-meta-dimension-catalog.md](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md)

<!--
template_metadata:
  version: "1.0.0"
  last_updated: "2026-05-25"
-->
