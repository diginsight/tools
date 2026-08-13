---
description: Authoring discipline for vision-amendment plans — per-item scope tags, principle-impact tagging, coverage promises — so plans cannot silently absorb scope expansion
applyTo: '*vision*plan*.md'
domain: "prompt-engineering"
goal: "Force vision-amendment plans to declare per-item scope, principle impact, and downstream landing so scope expansion is visible at authoring time"
rationales:
  - "v14→v15 inflated from 1 trigger to 10 goal items because scope expansion happened silently inside a single plan (see 20260525.03-staleness-review/05-vision-usecase-plan-rules/01-overview.md)"
  - "Per-item tagging makes scope expansion auditable — reviewers spot `[scope-expansion]` in the active goal list before it lands"
  - "Principle impact tagging closes the loop with vision-frontmatter.instructions.md — every change carries an attestation against the named principles"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Vision Amendment Plan Rules

## Purpose

Plans that amend a vision document (matched by filename containing both `vision` and `plan`) MUST structure their § Goal section so the actionability gate in `plan-execution.instructions.md` can mechanically verify three properties: scope discipline (no silent expansion), principle impact (no silent change to declared principles), and coverage promise (every in-scope item has a downstream landing).

This file defines the per-item tagging format. The gate that enforces it lives in `plan-execution.instructions.md`. The principles being audited are declared in the target vision per `vision-frontmatter.instructions.md`.

## Required Goal-Section Shape

The § Goal section MUST contain:

1. A verbatim one-sentence restatement of the original trigger
2. A table where every row carries: `# | item | scope tag | principle impact | downstream landing`
3. Zero `[scope-expansion]` rows in the active list — they are quarantined to § Park lot per `plan-execution.instructions.md`

## Scope Tag Taxonomy

| Tag | Meaning | Allowed in active goal list? |
|---|---|---|
| `[in-scope: original]` | Direct realization of the verbatim trigger | YES |
| `[scope-expansion: needs-own-plan]` | Distinct activity surfaced during authoring | NO — move to § Park lot, spawn a sibling plan |
| `[vision-only: no-downstream]` | Clarification or wording change with no downstream landing | YES (exempt from coverage promise) |

## Principle Impact Format

Every `[in-scope]` row MUST include a principle-impact cell using this format:

```
preserves: <id>, <id>; touches: <id> (P0 consent | P1 justification: <reason>)
```

- `preserves:` — comma-separated list of principle ids from the vision's `principles:` block that this item leaves intact. At least one entry required.
- `touches:` — principles this item modifies. For each:
  - **P0 touches** MUST add an explicit consent line below the table: *"This item touches P0 `<id>`. Vision version bump required. Consent confirmed by: <author>."*
  - **P1 touches** MUST add a justification phrase inline: `touches: <id> (P1 justification: <one sentence>)`
  - **P2 touches** require no extra line; the `touches:` mention is sufficient

`[vision-only]` items MUST still list `preserves:` (so reviewers can verify the change does not contradict a principle by accident).

## Coverage Promise Format

Every `[in-scope]` row MUST name a downstream landing:

- An existing artifact path: `landing: <path>` — must resolve at plan-completion time
- A spawned sibling plan id: `landing: <plan-id>.md` — must exist as a file under the same plan folder
- Vision-only items use: `landing: vision-only` — exempt

## Rules

- Vision-amendment plans MUST open § Goal with a verbatim one-sentence restatement of the original trigger
- Every goal item MUST carry exactly one scope tag from the taxonomy
- The active goal list MUST contain zero `[scope-expansion]` items — they are reclassified, spawned, or parked
- Every `[in-scope]` item MUST list at least one principle in `preserves:`
- Every `[in-scope]` item that touches a P0 principle MUST carry an explicit consent line referencing the vision id and the consenting author
- Every `[in-scope]` item that touches a P1 principle MUST carry an inline justification phrase
- Every `[in-scope]` item MUST name a downstream landing (`landing: <path-or-plan-id>`)
- `[vision-only]` items MUST list `landing: vision-only` and MUST still list `preserves:`
- A plan that fails any of these rules MUST stay in `status: draft` per the actionability gate in `plan-execution.instructions.md`
- Edge cases surfaced during execution MUST go to § Park lot — they MUST NOT be appended to the active goal list

## Worked Example

The canonical reference is [01-vision-usecase-plan-rules-plan.md](../../src/docs/90.%20Issues/202605/20260525.03-staleness-review/05-vision-usecase-plan-rules/01-vision-usecase-plan-rules-plan.md). Every row in its § Goal table is shaped to this rule; the § Park lot demonstrates the quarantine pattern.

## Quality Checklist

- [ ] § Goal opens with verbatim trigger restatement
- [ ] Every row has `# | item | scope tag | principle impact | downstream landing`
- [ ] No `[scope-expansion]` in active list
- [ ] Every `[in-scope]` row lists ≥ 1 principle in `preserves:`
- [ ] P0 touches have an explicit consent line
- [ ] P1 touches have an inline justification phrase
- [ ] Every `[in-scope]` row names a landing that resolves
- [ ] § Park lot exists for surfaced edge cases (may be empty)

## References

- **📘** `.github/instructions/vision-frontmatter.instructions.md` — declares the `principles:` block this file audits against
- **📘** `.github/instructions/plan-execution.instructions.md` — defines the actionability gate that enforces these rules
- **📒** `src/docs/90. Issues/202605/20260525.03-staleness-review/05-vision-usecase-plan-rules/01-overview.md` — sub-issue analysis that motivated this rule
- **📒** `src/docs/90. Issues/202605/20260525.03-staleness-review/05-vision-usecase-plan-rules/01-vision-usecase-plan-rules-plan.md` — worked example

<!--
instruction_metadata:
  version: "1.0.0"
  last_updated: "2026-05-31"
-->
