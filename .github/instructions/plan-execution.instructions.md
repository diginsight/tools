---
description: Rules for creating and processing plan files — naming, readiness pre-write gate, lifecycle (draft/actionable/in-progress/done), actionability gate, park lot plus open-decisions/discovery, and on-create/on-save/on-ask triggering
applyTo: '**/*.plan.md'
domain: "prompt-engineering"
goal: "Ensure plans are goal-driven, actionable, scope-disciplined, and validated before execution"
rationales:
  - "Plans without a clear, request-aligned goal and resolved unknowns produce misaligned or ambiguous work"
  - "An explicit lifecycle prevents draft plans from being executed by accident"
  - "A park lot and open-decisions list prevent surfaced edge cases and unknowns from silently inflating or stalling the active list"
  - "The filename is the trigger: a plan not named `*.plan.md` never loads these rules, so it is unvalidated by construction"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
  - ".copilot/context/00.00-prompt-engineering/05.11-plan-authoring-discipline.md"
---

# Plan Execution Rules

## Purpose

Rules for **creating** plan files (structure/content) and **processing** them (pre-execution validation and execution discipline).

## Scope

These rules apply ONLY to plans **explicitly triggered by the interactive user** ("execute this plan", "process these steps", or invoking a plan file directly).

They MUST NOT apply to agent-internal todo lists, plans generated/executed autonomously inside prompt workflows, or lightweight task tracking — the agent already controls alignment there, so the gate would only add overhead.

## Plan File Naming (CRITICAL — self-enforcing)

The filename **is** the enforcement trigger. This file loads only for `**/*.plan.md`, so a plan saved under any other name silently escapes every rule below — it is unvalidated by construction, and nothing reports the omission.

- **[C1]** MUST name every plan `<NN>-<kebab-name>.plan.md` — e.g. `01-robustness-fixes.plan.md`
- **[C1]** MUST decide the name at creation time, BEFORE writing the file — renaming afterwards means the body was authored without these rules loaded, so the Actionability Gate MUST then be re-run against the renamed file
- **[C1]** MUST place the plan in the work-item folder it belongs to (`src/docs/90. Issues/<YYYYMM>/<work-item>/`), not at a content root
- NEVER name a plan `overview.md`, `notes.md`, or any name lacking the `.plan.md` suffix — if a document declares `status: draft | actionable | in-progress | done`, it is a plan and MUST carry the suffix

## Plan Creation Rules

- MUST define a clear **goal/scope** and **motivation** before listing steps
- MUST declare `status:` in frontmatter, taking one of `draft | actionable | in-progress | done`
- MUST ensure the plan's `goal:` restates and matches the user's actual request — any narrowing explicit, not silent
- MUST ensure every step is specific, actionable, and unambiguous, and directly contributes to that goal
- NEVER include steps that are vague, aspirational, or lack clear completion criteria
- MUST reach readiness (§ Plan Readiness) BEFORE writing the actionable body — writing is the last step, not the first

## Plan Readiness (pre-write gate)

Writing the actionable body is the LAST step. First, MUST resolve every **blocking unknown** (any ambiguity, missing fact, or undecided option that would make a step a guess) via the **resolution ladder**, in order: (1) **resolve from evidence** (read-only investigation); (2) **ask the user** — any decision/preference/intent evidence cannot settle; batch; NEVER guess; (3) **bound as § Discovery** with a defined negative branch (`if absent → do X`), ONLY for facts undecidable until execution. An unknown unresolved by (1)–(3) is a STOP: the body MUST NOT be written; the plan stays `draft` (goal + § Open decisions + § Discovery only). 📖 Detail + rationale: `05.11-plan-authoring-discipline.md`.

## Plan Lifecycle

Four states; every transition is gated. 📖 Full semantics: `05.11-plan-authoring-discipline.md`.

- `draft` — readiness loop; holds goal + § Open decisions + § Discovery only, **no actionable body** → `actionable` (after the gate passes)
- `actionable` — gate passed, body written, ready but not started → `in-progress` (first executed step) or back to `draft` (a gate condition regresses)
- `in-progress` — ≥1 item executed and marked done → `done`
- `done` — all active items complete + exit criteria met; terminal (more work needs a new plan)

- A `draft` plan MUST NOT have items executed; the actionable body MUST NOT be written while `draft` — authored only at the `draft`→`actionable` transition once the gate passes
- A `done` plan MUST NOT gain or lose items — surface follow-ups via a sibling plan
- `status:` MUST be updated before the corresponding work begins

## Actionability Gate (MANDATORY)

Run BEFORE writing the body and again at the `draft`→`actionable` transition. ALWAYS run all eight (📖 per-check rationale: `05.11-plan-authoring-discipline.md`):

1. **Goal alignment** — goal restates the user's request; narrowing explicit
2. **Goal reachability** — ordered steps reach the goal; no gap or undocumented work
3. **Execution determinism** — each step has exactly one reasonable execution
4. **Clarity & actionability** — verify/confirm/check/assess are NOT actionable unless they carry a defined negative branch or live in § Discovery
5. **Unknown resolution** — every blocking unknown resolved (evidence/ask/discovery); none guessed
6. **Scope discipline** — no item exceeds the verbatim trigger; expansions → § Park lot
7. **Coverage promise** — every in-scope goal item names a downstream landing (path, sibling-plan id, or `vision-only`)
8. **Principle impact** *(vision-amendment plans only)* — every item tagged per `vision-amendment.instructions.md`

On any failure: **resolve FIRST** (evidence → ask user), keep `status: draft`, re-run. NEVER write a body or execute a plan that has not passed.

## Park Lot

Plans MUST contain a § Park lot section (may be empty) for edge cases surfaced during authoring or execution that are out of scope for the current plan.

- Items in § Park lot MUST NOT be executed as part of the current plan
- Each parked item MUST carry a one-line disposition: `→ <sibling-plan-id>.md` (will spawn a new plan), `→ defer` (revisit later, no commitment), or `→ closed: <reason>` (intentionally not pursued)
- Migrating an item from § Park lot into the active goal list MUST drop the plan back to `status: draft` and re-run the Actionability Gate

## Open Decisions and Discovery

- Plans MUST contain a § Open decisions section (may be empty) for IN-SCOPE unresolved decisions awaiting evidence or a user answer (distinct from § Park lot = out-of-scope). Each MUST name what resolves it and which steps it gates
- Plans MAY contain a § Discovery section for items undecidable until execution; each MUST carry a defined negative branch
- Any open decision keeps the plan `draft`; closing all is a Gate precondition

## Trigger

Auto-loaded for `*plan*`. The gate runs: **on create** (blocking precondition before the body is authored — primary enforcement); **on save** (evaluates current state, reports failures inline); **on ask** (runs first when a user asks to execute a plan, regardless of declared status). It does NOT auto-promote `draft`→`actionable` — promotion is an explicit author action.

## Execution Discipline

- MUST validate alignment and actionability BEFORE starting execution — NEVER skip it to "save time"
- MUST mark each step done immediately after finishing it
- MUST NOT execute steps that cannot be verified against the goal; flag and resolve blockers rather than proceeding with uncertainty
- MUST route edge cases surfaced mid-execution to § Park lot, not the active list

## Completion Marking

Format rules (suffix notation, classification, consistency) live in `plan-marking.instructions.md` (co-loaded for `*plan*`). 📖 Authority: `.github/instructions/plan-marking.instructions.md`.

Point-of-action essentials: suffix `Task text. (✅ done)` — NEVER `[x]` or `[ ]`; mark section headings (classify Action/Analysis/Proposal first); goal statements, informational tables, and structural headings get NO suffix.

## References

- **�** `.copilot/context/00.00-prompt-engineering/05.11-plan-authoring-discipline.md` — readiness/lifecycle/gate elaboration + rationale (this file holds the terse MUSTs)
- **�📘** `.github/instructions/plan-marking.instructions.md` — marking format + identifier-readability authority
- **📘** `.github/instructions/vision-amendment.instructions.md` — per-item tagging for Gate check #8 (matches `*vision*plan*.md`)
- **📘** `.github/instructions/vision-frontmatter.instructions.md` — declares the `principles:` block for Gate check #8

<!--
instruction_metadata:
  version: "1.5.0"
  last_updated: "2026-08-16"
-->
