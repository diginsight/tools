---
title: "Robustness tiering and routing"
description: "Severity tiers and their destinations, the finding record shape, the plan handoff contract and the scope guard that prevents removal recommendations without derived purpose"
domain: "application-development"
goal: "Turn a scan into an actionable plan by routing each finding to the destination that matches its severity, so urgent defects are not buried under hygiene items"
scope:
  covers:
    - "The three severity tiers and their destinations"
    - "Escalation and de-escalation rules"
    - "The finding record shape"
    - "The plan handoff contract"
    - "The scope guard on removal, replacement and redesign"
  excludes:
    - "The invariant classes themselves (see 09-hardening-invariant-catalog.md)"
    - "Plan file lifecycle and marking (see plan-execution.instructions.md and plan-marking.instructions.md)"
    - "Run modes and checkpoints (see 11-run-model.md)"
boundaries:
  - "NEVER put a 🟡 hygiene finding into the current plan — it belongs in the park lot"
  - "NEVER emit a finding without a proposed change AND a way to tell the change worked"
  - "NEVER recommend removal, replacement or redesign of a component whose purpose has not been derived and recorded"
rationales:
  - "A single undifferentiated finding list gets triaged once and then ignored; tiering is what keeps the urgent items visible"
  - "A finding without a verification method cannot be closed, so it stays open forever and erodes trust in the whole list"
  - "The scope guard exists because deliberate sample code and template scaffolding are the two things a scanner most reliably misreads as dead weight"
---

# Robustness tiering and routing

**Purpose**: Where a finding goes, and what it must carry to be actionable.

**Referenced by**:
- `ad-robustness-manager.agent.md`, `ad-robustness-analyst.agent.md`
- `.github/prompts/10.00-application-development/02.01-ad-harden-plan.prompt.md`
- `.github/templates/10.00-application-development/finding-record.template.md`

---

## 🎚️ Severity tiers

| Tier | Covers | Destination |
|---|---|---|
| 🔴 **Correctness and security** | the repository produces a wrong result, or is exposed | the **current plan** as an actionable step |
| 🟠 **Resilience** | it works, but fails badly under load, latency or partial failure | a **sibling plan**, referenced from the current one |
| 🟡 **Hygiene** | maintainability, clarity, consistency | the **park lot** |

### Escalation and de-escalation

Defaults from the catalogue are adjusted by evidence, and the adjustment is always stated with its reason.

| Move | When |
|---|---|
| 🟠 → 🔴 | the component is 🔴 Core **and** the failure path reaches a caller or corrupts stored state |
| 🔴 → 🟠 | the component is 🟡 Tooling or ⚪ Peripheral **and** the defect cannot reach production |
| any → 🟡 | evidence shows the invariant is upheld elsewhere, leaving only a clarity concern |

De-escalating to 🟡 on the grounds that "it has never happened" is not permitted. Absence of an incident is not evidence of an upheld invariant.

---

## 🧾 Finding record shape

| Field | Content |
|---|---|
| `id` | `{invariant-class}-{nn}` |
| `class` | one of the nine invariant classes |
| `component` | component id from the registry |
| `severity` | 🔴 / 🟠 / 🟡, with the reason if it differs from the catalogue default |
| `invariant` | the invariant, restated for this component |
| `evidence` | dossier record ids that establish the violation |
| `what-breaks` | the concrete consequence — not "this is risky" |
| `proposed-change` | what to change, specifically enough to start |
| `verification` | how to tell the change worked |
| `purpose-derivation` | required when the proposal removes, replaces or redesigns anything |

`what-breaks` and `verification` are the two fields that most often get written vaguely and are the two that decide whether the finding is ever closed.

---

## 📋 Plan handoff

The scan emits a plan file, not a report. It must satisfy `.github/instructions/plan-execution.instructions.md` and `.github/instructions/plan-marking.instructions.md`.

| Requirement | |
|---|---|
| **Status** | `draft` while any 🔴 finding lacks a `verification`; `actionable` once all do |
| **Marking** | suffix notation `(🟡 todo)` / `(🟢 done)` — checkbox lists are forbidden |
| **Step granularity** | one step per finding, each independently executable and independently verifiable |
| **Ordering** | 🔴 first; within a tier, by component priority from the registry |
| **Sibling plan** | 🟠 findings go to a sibling plan, which the current plan references |
| **Park lot** | 🟡 findings, each with enough context to be picked up later without re-scanning |
| **Discovery** | any finding blocked on missing evidence, recorded with what was attempted |

An emitted plan carrying no 🔴 findings is a legitimate outcome and is stated as such — it is not padded with 🟡 items to look substantial.

---

## 🔒 Scope guard

Restates `D9` because this is where it is enforced.

Before proposing **removal, replacement or redesign** of any component, the finding MUST record:

| # | Recorded |
|---|---|
| 1 | the component's derived purpose, with the derivation source |
| 2 | whether that purpose is still served, and by what evidence |
| 3 | what would stop working if the proposal were applied |

Without all three, the proposal is downgraded to a **question** in the discovery section.

Two failure modes this blocks:

- a deliberate sample or reference implementation removed as dead code;
- scaffolding left by a project template redesigned as though it were intentional architecture.

Both are cheap to prevent here and expensive to reverse after the plan is executed.

---

## References

- **📖** `00-stream-contract.md` — the scope-derivation precondition and escalation format
- **📖** `01-discovery-model.md` — component priority, used in ordering and escalation
- **📖** `09-hardening-invariant-catalog.md` — the classes and their default severities
- **📖** `11-run-model.md` — how a scan run is sequenced and resumed
- **📖** `.github/instructions/plan-execution.instructions.md` — plan file lifecycle

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
