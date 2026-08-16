---
title: "Discovery model — stack profile, capability matrix and component registry"
description: "The shared discovery artifact every role consumes: how the stack profile, live-surface capability matrix and component registry are derived, persisted and detected as stale"
domain: "application-development"
goal: "Establish one discovered — never assumed — description of a repository, so that every downstream rule resolves against real facts instead of a hard-coded stack"
scope:
  covers:
    - "Stack profile: the facts discovered about languages, build, tests, deployment and configuration"
    - "Capability matrix: which live evidence surfaces the repository actually exposes"
    - "Component registry: id, path, purpose, priority, dependencies"
    - "Priority taxonomy and what it drives"
    - "Layout mode: single-component versus multi-component"
    - "Purpose derivation and its evidence requirement"
    - "Persistence location and staleness detection"
  excludes:
    - "How dossiers record evidence (see 02-evidence-dossier-schema.md)"
    - "Which chapters exist and where a page lands (see 04-documentation-structure.md)"
    - "Run sequencing and checkpoints (see 11-run-model.md)"
boundaries:
  - "NEVER assume a stack, a folder convention or an environment naming scheme — every entry is discovered"
  - "A component whose purpose cannot be derived MUST be escalated, never guessed"
  - "The priority taxonomy is authoritative here — other files MUST reference, not redefine it"
rationales:
  - "Hard-coding a stack is what makes a documentation agent unportable; resolving through a discovered profile is what makes the same artifacts run on any repository"
  - "Purpose derivation is the direct counter-measure to remove-it recommendations aimed at deliberate sample code"
  - "Both streams reading one registry is what prevents them disagreeing about what the repository contains"
---

# Discovery model

**Purpose**: The shared discovery artifact every role consumes — the stack profile, the live-surface capability matrix and the component registry.

**Referenced by**:
- `.github/agents/10.00-application-development/ad-documentation-manager.agent.md`, `ad-robustness-manager.agent.md`
- all six investigator agents
- `.github/prompts/10.00-application-development/01.00-ad-docs-discover.prompt.md`

---

## 🧱 Stack profile

The set of facts **discovered**, never assumed, about the repository. Every later rule resolves against this profile rather than naming a technology.

| Fact | What is established |
|---|---|
| **Languages and runtimes** | which languages are present and at what target versions |
| **Build entry points** | how the repository is built, and from which root files |
| **Dependency strategy** | how dependencies are declared and whether versions are locked |
| **Test surface** | which test projects or suites exist, and what they cover |
| **Deployment targets** | where artifacts are meant to run, per deployable component |
| **Configuration mechanism** | how settings reach the running process, and how environments differ |
| **Observability** | how the repository emits logs, traces and metrics, if at all |

Each entry records **where it was established from**. A profile entry without an evidence location is incomplete.

---

## 📡 Capability matrix

Which live evidence surfaces this repository actually exposes. Discovered, never assumed. This drives which investigators have work to do and which capture procedures are runnable.

| Surface | Present when |
|---|---|
| Running application | the repository builds something with a user-facing entry point that can be started |
| API explorer | a deployable component publishes an interactive API description |
| Cloud portal | infrastructure definitions or deployment descriptors name a hosting platform the user can reach |
| CI portal | pipeline definitions exist and their runs are visible to the user |
| Database | a persistence target is configured and reachable under the access policy |

A surface recorded **absent** is a fact, and its investigator still runs — it records the absence rather than skipping silently (📖 `00-stream-contract.md` § Unverifiable-assertion protocol).

---

## 🗂️ Component registry

One row per component.

| Column | Content |
|---|---|
| `id` | stable kebab-case identifier used by dossier paths and page anchors |
| `path` | repository-relative root of the component |
| `purpose` | why the component exists, with its derivation evidence |
| `priority` | one of the four tiers below |
| `dependencies` | the other component ids it requires |

### Priority taxonomy

| Priority | Meaning | Drives |
|---|---|---|
| 🔴 **Core** | the repository's reason to exist | documented in the main chapters; investigated first |
| 🟠 **Supporting** | required for Core to function | documented in the main chapters |
| 🟡 **Tooling** | build, test, automation | *Other Components* and the Appendix only |
| ⚪ **Peripheral** | samples, scratch, experiments | *Other Components* and the Appendix only |

Priority drives **both placement and processing order**. A 🔴 Core component with its only page under *Other Components* is a defect.

---

## 🧭 Layout mode

| Mode | Condition | Effect |
|---|---|---|
| **single-component** | exactly one 🔴 Core component and no 🟠 Supporting components | chapters are flat |
| **multi-component** | anything else | chapters gain a component pivot below them |

The chapter set is **identical** in both modes — only the pivot changes (📖 `04-documentation-structure.md`).

---

## 🔍 Purpose derivation

Purpose MUST be derived from evidence and the evidence MUST be recorded, not just the conclusion.

Derivation sources, in order of strength:

| # | Source | Establishes |
|---|---|---|
| 1 | Explicit in-code or in-file markers stating intent | purpose directly |
| 2 | Entry-point analysis — what the component starts and what it then does | purpose behaviourally |
| 3 | Deployment descriptors — whether and where it is shipped | whether it is product or tooling |
| 4 | Configuration defaults — what it expects to be given | its role in the system |
| 5 | Sibling README or solution-file claims | purpose as **claimed**, to be corroborated |

A component whose purpose cannot be derived from any of these is **escalated**, never guessed. Two failure modes this prevents:

- a deliberate sample or reference implementation read as dead code;
- a scaffold left by a project template read as an intended component.

Both are recorded as purpose findings, not as removal recommendations (📖 `00-stream-contract.md` § Scope-derivation precondition).

---

## 💾 Persistence and staleness

| Concern | Rule |
|---|---|
| **Location** | the profile, matrix and registry are written to `src/docs/_evidence/_discovery.md` with `publish: false` |
| **Single copy** | every role and every later run reads that one file — a role that rebuilds it privately breaks convergence |
| **Stamp** | the file records when discovery ran and which repository revision it observed |
| **Staleness** | the registry is stale when the repository revision moved, when a component root was added or removed, or when a build entry point changed |
| **On stale** | discovery re-runs and the diff is reported at the registry checkpoint — components are never silently added or dropped |

---

## References

- **📖** `00-stream-contract.md` — role boundaries and determinability routing
- **📖** `02-evidence-dossier-schema.md` — where dossiers live and what they record
- **📖** `03-evidence-access-policy.md` — how live surfaces in the capability matrix may be reached
- **📖** `04-documentation-structure.md` — how priority maps onto chapter placement
- **📖** `11-run-model.md` — the registry checkpoint

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
