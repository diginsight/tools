---
description: Status marking rules for plan files — suffix notation, section/item classification, and consistency enforcement
applyTo: '*plan*'
domain: "prompt-engineering"
goal: "Single source of truth for plan marking format — referenced by documentation.instructions.md and plan-execution.instructions.md"
rationales:
  - "Marking rules were duplicated across two instruction files, causing drift and inconsistent enforcement"
  - "Point-of-action principle: models need format rules present where plan operations trigger"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Plan Marking Rules

## Purpose

Defines HOW to mark status on plan items and sections. Complements `plan-execution.instructions.md` (WHEN/WHY to mark, validation discipline). Both fire on `*plan*` files — no conflict: execution rules govern behavior, this file governs format.

## Status Markers

| Marker | Suffix | Use |
|---|---|---|
| ✅ | `(✅ done)` | Implemented/completed, acceptance conditions met |
| 🟡 | `(🟡 todo)` | Pending, in progress, or awaiting conditions |
| 📌 | `(📌 next steps)` | Follow-up after current scope |

## Format Rules (CRITICAL)

- MUST use suffix-only notation: `Task text. (✅ done)`
- MUST NOT use checkbox syntax (`[x]` or `[ ]`) — convert ALL to suffix on first edit
- MUST NOT use prefix notation (`🟡 todo: Task text`)
- MUST NOT mix prefix and suffix on same line
- Completed items MUST include a short completion note when non-obvious
- Pending items SHOULD include `To do:` explanation
- Leading heading emojis are decorative ONLY — MUST NOT use status emojis (✅, 🟡, 📌) as heading prefix; use neutral emojis (📋, 🔎, 🧭, ⚙️, 🧪)

## Identifier Readability

Applies to plan-local cross-reference ids (decisions, workstreams, gaps, parked items, open decisions).

- MUST be self-descriptive: ordinal + kebab slug — `D1-parity-authority`, `WS-A-parameter-surface`, `G9-tier-misdelegation`, `PL-1-vision-matrix`
- MUST NOT use a bare ordinal (`D1`, `WS-A`, `G9`, `PL-1`) as the canonical id or in references
- Externally-defined ids (e.g. vision-canonical `P-1`…`P-4`) are exempt — they mirror their source verbatim

## Section Classification

Classify BEFORE marking. Not all content is actionable.

| Section type | Definition | Suffix rule |
|---|---|---|
| **Action** | Work to perform (deliverables, implementation steps, stabilization) | `(✅ done)` when done; `(🟡 todo)` when pending |
| **Analysis** | Analytical work whose documented content IS the deliverable | `(✅ done)` once analysis is written — documenting findings completes it |
| **Proposal** | Design decision awaiting implementation | `(🟡 todo)` until realized; `(✅ done)` after |

**Sections that do NOT get suffixes:** Goal/objective statements (`## 🎯 Objective`), purely informational tables, non-actionable structural headings.

## Item Classification

| Item type | Definition | Marker rule |
|---|---|---|
| **Actionable** | Task requiring work (create, update, add, remove) | MUST carry suffix |
| **Observation** | Factual statement (X supports Y, X appears in Y) | MUST NOT carry suffix |
| **Design-decision** | Proposal element describing how something should work | MUST carry suffix (pending until implemented) |

**Test:** If removing it means work won't get done → actionable. If it means a fact won't be recorded → observation.

## Consistency Rules

- Section suffix and contained list items MUST be status-consistent
- Section suffix and status-bearing paragraphs MUST be status-consistent
- When status changes, update BOTH section suffix and affected item suffixes in same edit
- Plain unmarked actionable bullets in plan sections are NOT allowed
- Plan TOCs MUST include "things to do" sections when such sections exist

## Exit Criteria Mapping

- `(🟡 todo)` while conditions are pending
- `(✅ done)` only after ALL conditions are met
- `(📌 next steps)` only for deferred post-exit follow-ups, not pending criteria

## Cross-Document Applicability

The suffix notation defined here is the canonical format for status marking across the repository — NOT only for `*plan*` files. The same format MUST be used when marking discrete units of executed or pending work in other document types:

| Document type | What gets marked |
|---|---|
| **Issue reports** (`overview.md`, `*-issue.md`, `analysis.md`) | `### Step N — …` headings under Resolution/Remediation/Fix sections; numbered action items in the same sections; **all bulleted Verification / Acceptance / Follow-up checklists** (no `[x]` / `[ ]`) |
| **Post-mortems / RCA** | `### Step N`, `### Phase N`, `### Fix N` headings under Resolution/Action-items sections; verification checklists |
| **Change narratives** (release-notes-style) | Numbered list items describing executed changes |
| **Analysis documents** with explicit action sections | Action items under § Recommendations, § Next actions, § Follow-ups, § Verification |

**Checkbox ban applies cross-document:** In every document type listed above, `- [x]` and `- [ ]` MUST be rewritten as plain bullets with a `(✅ done)` / `(🟡 todo)` suffix. This applies regardless of whether the list is titled "checklist" — the suffix is the authoritative status carrier, the checkbox glyph is not.

**Discovery:** The base `documentation.instructions.md` file (auto-loaded for all `.md`) carries a § `Procedural step markers (executed-work narratives)` subsection that points back here. Agents editing a non-`*plan*` file with executed-work step headings MUST find the rule via that path.

**Out of scope:** Tutorial/how-to procedures authored for the reader. Those describe work the reader will do, not work the author has done — they MUST NOT carry status suffixes.

<!--
instruction_metadata:
  version: "1.4.0"
  last_updated: "2026-06-23"
-->
