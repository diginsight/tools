---
title: "Source-soundness gate"
description: "Rubric and gating verdict for validating source material before investigation and barring integration from ambiguous, contradictory, or unsound sources"
domain: "learning-hub"
---

# Source-soundness gate

## Purpose

Provide the rubric the observation-to-integration workflow uses to decide whether a source deserves deep analysis and, ultimately, integration into the Learning Hub. It is the "garbage in" defence: it validates the **source's content** — not just its publisher's reputation — before effort is spent and before conclusions can be integrated.

## Referenced by

- `.copilot/context/90.00-learning-hub/08-observation-to-integration-workflow.md` (Step 3.5 + integration precondition)
- `.github/prompts/90.00-learning-hub/lh-investigate-observation-and-integrate.prompt.md`
- `.github/agents/lh-observation-investigator.agent.md`

## The six dimensions

Assess the source against each dimension:

| Dimension | The question | Fails when |
|---|---|---|
| **Clarity** | Is the core concept stated unambiguously? | The central claim is vague or reinterpretable multiple ways |
| **Internal consistency** | Is the source free of self-contradiction? | It asserts and denies the same point, or its parts conflict |
| **Sufficiency** | Is there enough substance and specificity to build on? | It is a thin assertion with no supporting detail |
| **Novelty & value** | Does it propose something interesting or useful? | It is trivial, derivative, or already fully covered (see the coverage map) |
| **Verifiability** | Are the key claims checkable against authoritative sources? | Claims are unfalsifiable or contradict established facts |
| **Corroboration** | Is the concept supported by at least one independent source? | It is a lone, unvetted assertion |

Reference-classification reliability (📘📗📒📕) informs but MUST NOT replace this assessment — a reputable publisher can still publish an unsound piece, and a community source can be excellent.

## The gating verdict

Emit exactly one `source_verdict`:

- **`sound`** — clears all dimensions (or only cosmetic gaps). Proceed to investigation and, after approval, integration.
- **`promising-but-unverified`** — valuable and clear, but verifiability or corroboration is incomplete. MUST proceed only with mandatory external corroboration and explicit uncertainty caveats; unresolved contradictions block integration. MAY be upgraded to `sound` once corroborated.
- **`insufficient`** — fails clarity, internal consistency, sufficiency, or value in a way corroboration cannot repair. MUST stop: do not run deep analysis or integrate; return "source insufficient" with the specific evidence or corroboration that would raise the verdict. MAY be recorded as a watch-item.

## Integration precondition

The workflow MUST NOT integrate results into the Learning Hub unless `source_verdict` is `sound`, or a `promising-but-unverified` source has since been corroborated — regardless of how polished the downstream proposal looks. This precondition holds even when every downstream check (evidence, deduction-validation, report-quality) passes.

## Rationale

- A sound deduction from an unsound source is still unsound — downstream validation cannot repair a bad foundation.
- The approval gate fires late, when a polished proposal is hardest to reject; an upfront gate prevents sunk cost and late laundering of bad material.
- This operationalizes existing vision principles at the workflow level: `trust-calibrated-adoption` (unknown or unvetted sources never trigger autonomous adoption), `accuracy-over-everything`, and hallucination reduction/detection/containment.

## References

- `.copilot/context/90.00-learning-hub/08-observation-to-integration-workflow.md` — the workflow this gate protects.
- `06.00-idea/self-updating-engine/20260622.01-self-updating-engine-vision.md` — `trust-calibrated-adoption`.
- `06.00-idea/self-updating-article-writing/20260428.01-vision.v1.md` — `accuracy-over-everything`.
- `06.00-idea/self-updating-research/01.000-vision.v1.md` — hallucination reduction/detection/containment.

## Version history

- **v1.0.0** (2026-07-11): Initial source-soundness rubric — six dimensions, three-verdict gate, and integration precondition.

<!--
context_metadata:
  version: "1.0.0"
  created: "2026-07-11"
  last_updated: "2026-07-11"
-->
