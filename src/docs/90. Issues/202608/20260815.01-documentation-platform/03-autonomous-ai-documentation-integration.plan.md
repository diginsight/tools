---
title: "AI artifact documentation — integrating PE artifacts into repository documentation"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [prompt-engineering, autonomous-streams, repository-documentation]
description: "Extends the application-development documentation stream so a repository's own AI artifacts — prompts, agents, instructions, context — are discovered, investigated and documented under Other Components and the Appendix, on any repository."
status: draft
---

# AI artifact documentation — integrating PE artifacts into repository documentation

## 📑 Table of contents

- [🎯 Goal and scope](#-goal-and-scope)
- [🔎 Findings](#-findings)
- [🧭 What the shape would be](#-what-the-shape-would-be)
- [🗳️ Open decisions](#️-open-decisions)
- [🔬 Discovery](#-discovery)
- [🅿️ Park lot](#️-park-lot)
- [🏁 Exit criteria](#-exit-criteria)
- [📚 References](#-references)

## 🎯 Goal and scope

Extend the application-development documentation stream so that a repository's **AI artifacts** — its prompt-engineering customization files — are discovered, investigated and documented as part of every repository documentation run.

AI artifacts are not a repository's primary functionality. They are the tooling its authors use. So they belong in **Other Components**, with full appendices dedicated to artifact-category usage and implementation — not in Architecture, Reference or Use Cases.

The extension MUST be **generic**. It runs on a repository with a large PE artifact set, on one with three prompt files, and on one with none at all — and produces a defensible result in each case.

**Explicitly out of scope**: documenting the artifacts of the `10.00-application-development` domain *specifically*. This plan changes the stream so it documents whatever AI artifacts it finds, in any repository.

## 🔎 Findings

Investigation of the current domain against this repository's completed discovery run. (✅ done)

### F1-discovery-blindness

AI artifacts are **invisible** to discovery, not mis-tiered. `src/docs/_evidence/_discovery.md` records eleven components, all under `src/`. There is no registry entry for `.github/` or `.copilot/` at any priority.

The registry is derived from build-participating roots. `.github/` and `.copilot/` have no project file, no build entry point and no deployment descriptor, so nothing in the discovery model reaches them. Every `.github/` string in the domain's twelve context files is a reference to the domain's *own* artifacts — never a discovery target.

### F2-derivation-is-cheap

The five purpose-derivation sources are code-shaped, but source ① — *explicit in-file markers stating intent* — applies to AI artifacts unusually well. Every prompt declares `description`, `goal`, `scope` and `boundaries`; every agent declares `description`, `goal`, `capabilities` and `boundaries`.

AI artifacts are **more self-describing than code**. Once they are in scope, derivation is strong and cheap — this is the least risky part of the change.

### F3-authoring-is-blocked-by-design

Six investigators own fifteen source-set roles. None covers authored AI artifacts.

Because Authoring **cannot investigate** — a hard role boundary in `00-stream-contract.md` — and because every assertion on a generated page must carry a traceability anchor `^[{area}-{nn}]` resolving to a dossier record, no page about AI artifacts can be written today *even if a chapter existed*. There is no dossier to cite.

This is why the fix must start at discovery and investigation. Adding a template first would produce a page the verifier is obliged to fail.

### F4-six-area-rule-collision

Two existing rules collide if AI artifacts become ordinary registry components:

| Source | Rule |
|---|---|
| `11-run-model.md` | "all six areas for a component complete before authoring starts on it" |
| `02-evidence-dossier-schema.md` | "A dossier area with neither records nor gaps is itself a defect — it means the investigator did not run" |

An AI artifact domain has no persistence model, no authn surface, no deployment descriptor. Five of six investigators would produce empty dossiers, and each empty dossier is a **defect by the schema's own definition**.

So either area applicability becomes conditional on component kind, or AI artifacts sit outside the component registry entirely. This is the fork in `D1-artifact-registry-shape`.

### F5-placement-already-legal

No new chapter is needed, which matters because the chapter set is FIXED and "a stream MUST NOT invent, merge or rename a chapter".

The existing rule already routes 🟡 Tooling and ⚪ Peripheral to *Other Components* and the Appendix "and nowhere else". The placement requested is reachable within the current chapter set.

### F6-no-page-shape-fits

Eleven page shapes exist. `doc-component-minor.template.md` is the only candidate and it is code-shaped — *Deployed*, *What it does*, *Dependencies*. It carries no slot for the facts that make an AI artifact usable: its **invocation name**, its **position in an invocation order**, its **agent binding**, and **what it produces**.

A prompt family documented without its invocation order is not documentation; it is an inventory.

### F7-ripple-is-bounded

Adding an artifact area touches a known, closed set:

| Artifact | Change |
|---|---|
| `02-evidence-dossier-schema.md` | the six-areas table |
| `05-source-sets-and-propagation.md` | new roles and their single owner |
| `11-run-model.md` | "all six areas" and the investigation ordering rule |
| `01-discovery-model.md` | how artifact roots enter the registry |
| `04-documentation-structure.md` | new page shape, and the "fifteen templates" count |
| `ad-documentation-manager.agent.md` | one new handoff |
| new agent + new template | one each |

### F8-empty-repository-behaviour-is-settled

A repository with no AI artifacts is **not** a silent skip. The capability matrix rule already governs this: "A surface recorded absent is a fact, and its investigator still runs — it records the absence rather than skipping silently."

Resolved from evidence, not an open decision.

## 🧭 What the shape would be

Recorded so the open decisions are concrete, not abstract. **Not** an actionable body — no step below is committed until `D1`–`D3` close.

| Placement | Content |
|---|---|
| *Other Components* | one compact entry per artifact grouping — what it is, how it is invoked, what it produces |
| *Appendix* | the deep dive — category usage, invocation order, the handoff graph, and how the artifacts are implemented |

The Appendix is where the user's "full appendices dedicated to prompt category usage and implementation" lands.

## 🗳️ Open decisions

### D1-artifact-registry-shape

Do AI artifacts enter the **component registry** as ordinary components, or a **separate registry section**?

| Fork | Consequence |
|---|---|
| **A — ordinary components** | smaller conceptual change; requires making area applicability conditional per component kind, which weakens the "empty dossier is a defect" check for every component |
| **B — separate section** | keeps the six-area rule intact and unweakened; adds a second registry concept and its own page shapes |

- **Resolved by**: user decision — architectural preference with real trade-offs on both sides
- **Gates**: every subsequent step

### D2-grouping-granularity

What is **one** documented unit?

| Option | Result on this repository |
|---|---|
| per PE domain folder | one unit per domain, each spanning its context + agents + prompts + templates |
| per artifact type | one unit for all prompts, one for all agents, one for all context |

Per-file granularity is excluded — it would produce dozens of units and no reader benefit.

- **Resolved by**: user decision
- **Gates**: registry rows, page count, Appendix structure

### D3-artifact-type-scope

Which artifact types are in scope — **prompts and agents only**, or the full set (context, instructions, agents, prompts, skills, templates, prompt-snippets, `copilot-instructions.md`)?

- **Resolved by**: user decision
- **Gates**: investigator scope, template section set

## 🔬 Discovery

- **DS1-workflows-ownership** — `.github/workflows/` is already cited as evidence for deployment targets and the CI capability surface, owned by `ad-devops-investigator`. Whether an artifact investigator would overlap it is undecidable until the artifact area's roles are defined. *Negative branch*: if any overlap is found, `.github/workflows/` stays owned by devops and is explicitly excluded from the artifact area — a role claimed twice produces two dossiers that then disagree.

## 🅿️ Park lot

- **PL-1-root-prompt-schema-debt** — seven root-level prompts violate CRITICAL `[C6]` (no `name:`, deprecated `mode: ask`). → defer
- **PL-2-cross-repository-artifact-index** — an index of AI artifacts spanning several repositories. → closed: out of scope, the stream documents one repository from its own evidence
- **PL-3-legacy-prompt-relocation** — moving the five Diginsight-specific prompts out of the portable domain folder. → defer

## 🏁 Exit criteria

- `D1`–`D3` closed, and the actionability gate re-run against the written body. (🟡 todo)
- A repository with AI artifacts produces an *Other Components* entry and an Appendix page for each grouping, every assertion anchored to a dossier record. (🟡 todo)
- A repository with **no** AI artifacts produces a recorded absence, not a silent skip. (🟡 todo)
- No artifact of the `10.00-application-development` domain names this repository — the extension stays portable. (🟡 todo)

## 📚 References

- **📖** `.copilot/context/10.00-application-development/00-stream-contract.md` — role boundaries; authoring cannot investigate
- **📖** `.copilot/context/10.00-application-development/01-discovery-model.md` — registry, priority taxonomy, purpose derivation
- **📖** `.copilot/context/10.00-application-development/02-evidence-dossier-schema.md` — the six areas; empty-dossier defect rule
- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — fixed chapter set, page shapes, minor placement
- **📖** `.copilot/context/10.00-application-development/05-source-sets-and-propagation.md` — source-set roles and single ownership
- **📖** `.copilot/context/10.00-application-development/11-run-model.md` — six-area completion rule
- **📄** `src/docs/_evidence/_discovery.md` — the completed discovery run this investigation was checked against
