---
title: "Run model — modes, checkpoints and resumability"
description: "How a stream run starts, what each of the three modes scopes, the four checkpoints, the run state that makes a run resumable, processing order, breadth control and termination"
domain: "application-development"
goal: "Make a run something a user can start with one sentence, watch, interrupt and resume — instead of an all-or-nothing generation that must be relaunched from the beginning"
scope:
  covers:
    - "Mode recognition vocabulary and ambiguity tie-break"
    - "What each mode scopes"
    - "The four checkpoints and their semantics"
    - "Run state and resumability"
    - "Processing order"
    - "Breadth control"
    - "Termination and the run report"
  excludes:
    - "Change-set resolution and the dimension sweep (see 06-change-impact-assessment.md)"
    - "Which gates run at a chapter checkpoint (see 08-verification-gates.md)"
    - "Finding tiering (see 10-hardening-tiering-and-routing.md)"
boundaries:
  - "NEVER ask the user to disambiguate a mode twice — state the inference and proceed"
  - "NEVER pass a production-touching checkpoint without an explicit answer"
  - "NEVER restart a resumable run from the beginning when run state exists"
rationales:
  - "A user should not have to learn a command surface — the manager infers the mode from an ordinary sentence"
  - "Checkpoints are where scope errors become cheap to correct; without them a wrong inference costs the whole run"
  - "Long runs get interrupted, so resumability is a requirement rather than a convenience"
---

# Run model

**Purpose**: How a stream run starts, checkpoints, resumes and ends.

**Referenced by**:
- `ad-documentation-manager.agent.md`, `ad-robustness-manager.agent.md`
- every prompt in `.github/prompts/10.00-application-development/`

---

## 🗣️ Mode recognition

The manager infers the mode from the user's sentence. There is no command syntax to learn.

| Mode | Recognised from | Scopes |
|---|---|---|
| **create** | "document this repository", "generate the docs", "write documentation for this repo" | the full registry × the full chapter set |
| **revise** | "refresh", "update the documentation", "bring the docs up to date" | only pages whose evidence moved |
| **change-driven** | "for this PR", "for these commits", "for what I just changed", any reference to a diff | only pages selected by the union in 📖 `06-change-impact-assessment.md` |

For the robustness stream the same three shapes apply: scan everything, re-scan what moved, or scan only what a change touched.

### Ambiguity tie-break

| Situation | Resolution |
|---|---|
| No documentation exists | **create**, whatever the wording suggested |
| Documentation exists and a change reference is present | **change-driven** |
| Documentation exists and no change reference is present | **revise** |

State the inferred mode in one line and proceed. **Never ask twice** — a run that opens with a clarification round has already cost more than a wrong inference corrected at the first checkpoint.

---

## 🚩 Checkpoints

Four, and only four. Every other step proceeds without interruption.

| # | Checkpoint | Type | Carries |
|---|---|---|---|
| 1 | **Registry** — after discovery | notify | stack profile summary, capability matrix, component registry with priorities, and the diff against the previous registry |
| 2 | **Change-set mapping** — change-driven only | notify | the nine-dimension table and every page decision |
| 3 | **Chapter** — after each chapter | notify | pages written, gate outcomes, open gaps |
| 4 | **Production read** — before any production-touching read | **approval** | source, environment, reason, exact read (📖 `03-evidence-access-policy.md`) |

**Notify** checkpoints report and continue. **Approval** checkpoints stop and wait; silence is not consent.

Checkpoint 1 is the highest-value one: a mis-tiered component caught there costs a sentence, and caught at the end costs a chapter.

---

## 💾 Run state and resumability

Run state is written to `src/docs/_evidence/_run-state.md` with `publish: false`, and updated at every checkpoint.

| Field | Content |
|---|---|
| `mode` | the inferred mode |
| `scope` | components × chapters, or the change-driven page list |
| `completed` | units finished, each with its outcome |
| `pending` | units not yet attempted |
| `blocked` | units halted, each with its escalation |
| `updated` | when the state was last written |

On resume: read the state, report what is left, and continue from `pending`. Completed units are not re-run — their idempotency check would return "unchanged" anyway, but skipping them is what makes a resume fast (📖 `00-stream-contract.md` § Idempotency).

Run state older than the discovery stamp is discarded; the repository moved underneath it.

---

## 🔢 Processing order

Deterministic, so two runs produce the same sequence and a resume lands in the same place.

| Level | Order |
|---|---|
| Components | registry priority — 🔴 Core, then 🟠 Supporting, then 🟡 Tooling, then ⚪ Peripheral; ties broken by registry order |
| Chapters | fixed chapter sequence 1–11 (📖 `04-documentation-structure.md`) |
| Within a chapter | overview page last, so it summarises pages that already exist |
| Investigation | all six areas for a component complete before authoring starts on it |

---

## 📏 Breadth control

A run may be narrowed at the start, and the narrowing is recorded in the run state.

| Narrowing | Effect |
|---|---|
| By priority | for example 🔴 Core only |
| By component | a named subset of the registry |
| By chapter | a named subset of the eleven |

A narrowed run reports its own narrowing in the final report, so partial coverage is never mistaken for complete coverage.

---

## 🏁 Termination and report

A run ends when `pending` is empty, or when every remaining unit is `blocked`.

| Section | Content |
|---|---|
| **Mode and scope** | what ran, including any narrowing |
| **Produced** | pages written or findings raised, by chapter or tier |
| **Gate outcomes** | pass / pass-with-gaps / fail counts |
| **Open gaps** | every marked gap, with what was attempted |
| **Blocked** | every escalation, in the escalation format |
| **Next** | what a following run should pick up |

A run that produced nothing because everything was already current is a **success**, reported as such — it is the idempotency guarantee working.

---

## References

- **📖** `00-stream-contract.md` — roles, idempotency and the escalation format
- **📖** `01-discovery-model.md` — discovery, the registry and staleness
- **📖** `03-evidence-access-policy.md` — the production announcement behind checkpoint 4
- **📖** `04-documentation-structure.md` — the fixed chapter sequence
- **📖** `06-change-impact-assessment.md` — change-driven scope
- **📖** `08-verification-gates.md` — the gate outcomes reported at chapter checkpoints

## Version history

| Version | Date | Change | Author |
|---|---|---|---|
| 1.0.0 | 2026-08-16 | Initial version | System |

<!--
context_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
-->
