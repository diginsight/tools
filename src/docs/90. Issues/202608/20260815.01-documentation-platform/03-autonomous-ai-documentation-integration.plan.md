---
title: "AI artifact documentation — documenting artifact families as ordinary components"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [prompt-engineering, autonomous-streams, repository-documentation]
description: "Extends the application-development documentation stream so a repository's AI artifacts are discovered as artifact families, tiered like any other component, and documented in Other Components and the Appendix when accessory or across all chapters when primary."
status: done
---

# AI artifact documentation — documenting artifact families as ordinary components

## 📑 Table of contents

- [🎯 Goal and scope](#-goal-and-scope)
- [🔎 Findings](#-findings)
- [🧩 The family model](#-the-family-model)
- [🎚️ Primary versus accessory](#️-primary-versus-accessory)
- [🚦 Area applicability](#-area-applicability)
- [🧪 How artifact behaviour is investigated](#-how-artifact-behaviour-is-investigated)
- [📄 Page shapes and the content split](#-page-shapes-and-the-content-split)
- [⚙️ Workstreams](#️-workstreams)
- [🗳️ Open decisions](#️-open-decisions)
- [🔬 Discovery](#-discovery)
- [🅿️ Park lot](#️-park-lot)
- [🏁 Exit criteria](#-exit-criteria)
- [📚 References](#-references)

## 🎯 Goal and scope

Extend the application-development documentation stream so a repository's **AI artifacts** — its prompt-engineering customization files — are discovered and documented as **ordinary components**, grouped into **families**, tiered by the existing priority taxonomy, and documented with a family-behaviour description plus a usage section for the family's main functionalities.

Three properties the extension must hold:

- **Family-grained** — the documented unit is a coherent artifact family, derived from metadata, folder and usage; artifacts matching none are gathered under *Unparented artifacts* rather than dropped or force-fitted.
- **Tier-driven** — accessory families land in *Other Components* and the Appendix; primary families are investigated and documented across all chapters, exactly like any other primary component.
- **Applicability-aware** — an investigation that cannot apply to a family is recorded as inapplicable with its reason, never silently skipped.
- **Honest about behaviour** — an artifact's declared contract is recorded as declared; its unobserved effect is marked as a gap rather than narrated.

The extension MUST be **generic**. It runs on a repository with a large artifact set, one with three prompt files, and one with none.

**Explicitly out of scope**: documenting this repository's own `10.00-application-development` domain. This plan changes the stream; running it is separate work.

## 🔎 Findings

Investigation against the domain's twelve context files and this repository's completed discovery run. (✅ done)

### F1-discovery-blindness

AI artifacts are **invisible** to discovery, not mis-tiered. `_discovery.md` records eleven components, all under `src/`; there is no registry entry for `.github/` or `.copilot/` at any priority. The registry derives from build-participating roots, and artifact roots have no project file, build entry point or deployment descriptor. Every `.github/` string in the domain's context files is a reference to the domain's *own* artifacts — never a discovery target.

### F2-derivation-is-cheap

Purpose-derivation source ① — *explicit in-file markers stating intent* — applies to AI artifacts unusually well. Prompts declare `description`, `goal`, `scope`, `boundaries`; agents declare `description`, `goal`, `capabilities`, `boundaries`. Artifacts are **more self-describing than code**, so derivation is the least risky part of this change.

### F3-authoring-is-blocked-by-design

Authoring **cannot investigate** — a hard boundary in `00-stream-contract.md` — and every published assertion needs a traceability anchor resolving to a dossier record. So no page about AI artifacts can be written today even if a chapter existed, because no dossier exists to cite. The fix must start at discovery and investigation; adding a template first would produce a page the verifier is obliged to fail.

### F4-priority-taxonomy-already-routes

**The primary-versus-accessory distinction the request asks for already exists.** `01-discovery-model.md` defines four tiers and `04-documentation-structure.md` binds them to placement: 🔴 Core and 🟠 Supporting are documented in the main chapters; 🟡 Tooling and ⚪ Peripheral in *Other Components* and the Appendix "and nowhere else".

So "documented in all sections when primary" is **not a new rule** — it is the existing rule, applied to a component kind that never reaches the registry. What is missing is not placement logic but a **tiering criterion** for artifact families, and the discovery reach to produce the row in the first place.

This also means no new chapter is needed, which matters because the chapter set is FIXED.

### F5-skip-cannot-be-silence

`02-evidence-dossier-schema.md` states: *"A dossier area with neither records nor gaps is itself a defect — it means the investigator did not run."*

So skipping an inapplicable investigation, done naively, is **indistinguishable from a broken run**. Grepping the domain confirms **no applicability concept exists anywhere** — no `not-applicable`, no `n/a`, no conditional-area language. A third dossier state is required, and it must be a positive assertion carrying its reason, not an absence.

### F6-applicability-is-mechanically-derivable

An area does not need a hand-maintained per-component-kind exemption list. `05-source-sets-and-propagation.md` already assigns **fifteen source-set roles to exactly one owner each**, and roles resolve to concrete paths through the stack profile.

That gives a mechanical test: **an area is inapplicable to a component when none of the roles its investigator owns resolves to any path inside that component.** For an artifact family, `persistence-model` and `schema-definitions` resolve nowhere, so `data` is inapplicable — and the reason is stated in the same terms the schema already uses. This generalizes beyond AI artifacts and adds no stack assumption, so the portability boundary holds.

### F7-family-cannot-be-derived-from-folders

Measured on this repository. Domain folders **do** co-occur across artifact types — `01.00-article-writing` and `10.00-application-development` each appear under context, agents, prompts and templates — but folder structure alone is not sufficient:

| Counter-evidence | Measurement |
|---|---|
| Unfoldered artifacts | 39 — 9 agents, 10 prompts, 20 instructions |
| Instructions carry no folders at all | 20 of 20 flat |
| Template subfolders that are not domains | 4, each scoped to one prompt |
| Folders present under one artifact type only | `11.00-application-migration`, `20.00-devops` — prompts only |

A folder-only rule would invent families for per-prompt template folders and orphan a quarter of the artifact set.

### F8-frontmatter-alone-is-also-insufficient

`domain:` declaration coverage is uneven, and one value is corrupt:

| Artifact type | Declares `domain:` |
|---|---|
| instructions | 20 / 20 |
| context | 71 / 74 |
| agents | 44 / 68 |
| prompts | **18 / 105** |

`correlated-topics.prompt.md` declares the **unsubstituted template placeholder** `domain: {{technical_area}}` — a value that would become a phantom family under any rule that trusts the field blindly. Derivation must validate the value, not merely read it.

### F9-the-two-signals-join-after-normalization

Folder tokens carry an ordering prefix the frontmatter values lack — `10.00-application-development` versus `application-development`; `01.00-article-writing` versus `article-writing`. **Stripping the numeric prefix makes the two signals join**, which is what allows a union derivation instead of a choice between two incomplete ones. `repository-docs.instructions.md` is the proof case: flat on disk, so folder-blind, but declaring `domain: application-development`, so it joins the family its folder placement hides.

### F10-family-roots-must-exclude-shared-parents

`.github/workflows/` is already owned by `ad-devops-investigator` as `pipeline-definition` evidence. If a family root were recorded as `.github/`, the devops investigator and the artifact roles would both claim it — and `05-source-sets-and-propagation.md` warns that a role claimed twice produces two dossiers that then disagree.

Family paths must therefore be the union of **artifact-type subtrees** (`.github/prompts/`, `.github/agents/`, `.copilot/context/`, …), never a parent that also holds non-artifact material. This closes the overlap by construction.

### F11-no-page-shape-fits

`doc-component-minor.template.md` is the only candidate for the accessory case and it is code-shaped — *Deployed*, *What it does*, *Dependencies*. It carries no slot for the facts that make an artifact family usable: its **invocation names**, its **invocation order**, its **agent bindings**, and **what it produces**. A family documented without its invocation order is an inventory, not documentation.

### F12-empty-repository-behaviour-is-settled

A repository with no AI artifacts is not a silent skip. The capability-matrix rule already governs it: *"A surface recorded absent is a fact, and its investigator still runs — it records the absence rather than skipping silently."*

### F13-artifacts-declare-intent-they-do-not-encode-mechanism

The decisive epistemic constraint, and the one that shapes the whole investigation method.

Source code can be read for mechanism — a flow can be traced from entry point to effect. An AI artifact cannot. Its body is natural-language instruction interpreted by a model at run time, so **no static reading establishes what it does**. What a static reading establishes is what it *declares*.

`ad-code-investigator` already carries the rule this implies: *"A behaviour is only observable at runtime and no live surface exists → record a gap. NEVER describe behaviour you did not observe."*

So the split falls out of existing confidence semantics with no new machinery:

| Assertion | Confidence | Why |
|---|---|---|
| "prompt P declares goal X" | `established` | the frontmatter **is** the authoritative artifact for that assertion |
| "prompt P binds agent A and tools T" | `established` | declared and machine-readable |
| "prompt P runs after prompt Q" | `established` when an ordering prefix or a manager sequence declares it | declared |
| "prompt P achieves X" | `claimed` — marked as a gap | never observed |

A family page that states declared contracts as fact and marks effect as unobserved is honest. One that reads `description:` and reports it as behaviour has done exactly what the domain forbids for code.

### F14-the-usage-graph-is-real-but-too-sparse-to-partition

Measured across all 457 artifact files in the seven artifact-type roots:

| Usage signal | Files carrying it |
|---|---|
| `📖` reference to another artifact | 186 |
| Declared **Referenced by** block | 54 |
| `#file:` include | 39 |
| `applyTo` glob binding it to paths | 26 |

The graph is genuinely parseable, and its inbound edges are what rescue an artifact that is flat on disk and declares no domain. But no single signal covers even half the set, and the signals concentrate in the well-maintained domains.

More importantly, **usage cannot be the primary partition**: shared artifacts are referenced from everywhere, so a connected-component partition collapses most of the repository into one family. Usage works as an **adjacency vote at third rank** — adopt the family most common among an artifact's graph neighbours — never as the first cut.

### F15-unobserved-effect-is-a-marked-gap-not-a-blocker

Nothing records which artifact produced a page. The `verification_stamp` carries `generated`, `verified`, `evidence`, `gates` and `open_gaps`, and names the dossiers a page was built **from** — never the prompt or agent that wrote it. The only producer trail is an informal HTML comment on two evidence files that no schema declares:

```markdown
<!-- Generated by /01.00-ad-docs-discover. Do not edit by hand — re-run discovery instead. -->
```

That sounds blocking and is not. `07-documentation-authoring-criteria.md` settles it: a gap blocks only when it defeats the page's **core** purpose; a peripheral gap is marked in place and the page is still written.

The core purpose of a family page is architecture and usage — both carried entirely by layers 1–4, all establishable today. Runtime effect is peripheral to it. So the correct output is a marked gap in the domain's own format:

> **Not established**: whether this prompt produced the pages it declares. No stamp records a producing artifact. `^[gap]`

Which is, by that same file's reasoning, better than the alternative: *"a marked gap is more useful than a plausible sentence, because it can be closed."*

So **nothing blocks artifact behaviour documentation**. Producer provenance stays a real concern, but it belongs to the documentation platform rather than to artifact families, and is parked accordingly (📖 PL-5).

## 🧩 The family model

A **family** is a set of AI artifacts, possibly spanning several artifact types, that share a grouping key and are used together. It is the unit that gets a registry row, a dossier set and a page — answering the request's core question: yes, artifacts are documented by family.

### Derivation ladder

Applied per artifact file; first rule that yields a valid key wins.

| # | Signal | Key |
|---|---|---|
| 1 | **Metadata** — frontmatter `domain:` present and valid | the declared value, normalized |
| 2 | **Folder** — enclosing folder under an artifact-type root | folder name with its numeric ordering prefix stripped |
| 3 | **Usage** — the artifact's reference-graph neighbours, inbound and outbound | the family most common among those neighbours |
| 4 | none of the three | **unparented** |

A `domain:` value is **invalid** when it is empty or contains unsubstituted template syntax. An invalid value falls through to rule 2 and is additionally raised as a finding against the artifact.

Keys are normalized before comparison: strip a leading `NN.NN-` or `NN-` ordering prefix, lowercase, trim. This is what makes rules 1 and 2 produce joinable keys.

Rule 3 is an **adjacency vote, never a connected-component partition** (📖 F14) — shared artifacts are referenced from everywhere, so components would collapse into one family. It resolves only when a clear majority exists; a tie falls through to rule 4 rather than picking arbitrarily.

Edges are collected from all artifacts and inverted, so an artifact with no outbound references still inherits a family from the artifacts that reference **it**.

### Unparented artifacts

Rule 4 is a real outcome, not an error path. Unparented artifacts are gathered into one **Unparented artifacts** section carrying, per artifact, its type, invocation surface and declared contract — the same facts a family page carries, without the family narrative that would be fiction.

They are never merged into a nearby family to tidy the output, and never dropped. An artifact appearing there is itself a finding worth surfacing: it is invocable but belongs to nothing, which is usually how an artifact becomes unmaintained.

### Family record

Each family becomes one component registry row, using the existing columns, plus its member paths as the union of contributing artifact-type subtrees (📖 F10).

### Granularity control

Derivation is mechanical, so it can over-split — on this repository it yields four separate prompt-engineering families at different granularities. Rather than encode a merge heuristic that would be wrong on other repositories, the derived family set is presented at **checkpoint 1**, which already exists to catch exactly this class of error before it costs a chapter.

## 🎚️ Primary versus accessory

No new placement machinery. The existing taxonomy decides, and the existing placement rule follows.

### Tiering criterion

Derived from evidence via purpose-derivation source ③ — *deployment descriptors establish whether a component is product or tooling*:

| Evidence | Tier |
|---|---|
| The artifacts are what the repository ships — a packaging manifest, release or distribution mechanism carries them as the deliverable | 🔴 Core |
| The artifacts support production of something else the repository ships | 🟡 Tooling |
| The artifacts are experiments, samples or scratch | ⚪ Peripheral |

Escalate rather than guess when no evidence resolves the tier — the standard rule for underivable purpose.

### What each tier produces

| Tier | Documented in |
|---|---|
| 🔴 Core / 🟠 Supporting | the main chapters — Architecture, Use Cases, Reference, Validation, Security, DevOps, Infrastructure — investigated in every applicable area, exactly like any other primary component |
| 🟡 Tooling / ⚪ Peripheral | *Other Components* entry plus an Appendix deep dive, and nowhere else |

A primary family therefore needs **no special-casing**: it uses the ordinary main-chapter page shapes. Only the accessory case needs a new shape, because only there is one page asked to carry the whole family.

## 🚦 Area applicability

Three distinguishable dossier-area states replace today's two. The third is what lets an inapplicable investigation be skipped without becoming a defect.

| State | Meaning | Signal |
|---|---|---|
| **Populated** | investigator ran and recorded facts | records present |
| **Gapped** | investigator ran, sought, did not find | gap records present |
| **Inapplicable** | investigator ran and **established the area cannot apply** | an inapplicability record with its reason and the roles tested |
| *(none of the three)* | **defect** — the investigator did not run | preserved unchanged |

Inapplicability is asserted, dated and attributable like any other record, so the existing defect check survives intact.

### Expected outcome for a typical artifact family

Derived at run time by the role-resolution test (📖 F6), **not** hard-coded — a family that does ship through a pipeline will resolve `devops` differently.

| Area | Typical result | Because |
|---|---|---|
| `code` | populated | `entry-points` resolves to invocable prompts and agents |
| `configuration` | populated | frontmatter binds tools, models, agents and `applyTo` scopes |
| `data` | inapplicable | no `persistence-model` or `schema-definitions` role resolves |
| `environment` | inapplicable | no `infrastructure-definition` or `deployment-descriptor` role resolves |
| `devops` | inapplicable in the accessory case | artifacts are not built or shipped; `.github/workflows/` stays with devops (📖 F10) |
| `security` | family-dependent | resolves when tool grants or an injection surface are present |

This is the request's "skip CI/CD when it doesn't apply", expressed so the skip is auditable.

## 🧪 How artifact behaviour is investigated

Four layers are statically establishable; the fifth is not, and saying so is the point (📖 F13).

| # | Layer | Established from | Area |
|---|---|---|---|
| 1 | **Declared contract** | frontmatter `description`, `goal`, `scope`, `boundaries` | `code` |
| 2 | **Invocation surface** | filename → slash command or mention; ordering prefix → position in a sequence | `code` |
| 3 | **Composition graph** | handoff targets, `📖` references, `#file:` includes, template bindings, declared *Referenced by* blocks | `code` |
| 4 | **Binding surface** | `agent`, `tools`, `model`, `applyTo` — what the artifact is permitted to reach and where it activates | `configuration` |
| 5 | **Actual effect** | not derivable — marked as a gap in place (📖 F15) | `code` |

Layer 3 is what makes a **family** describable rather than a list: inverted, it yields the entry points (artifacts nothing hands off to), the internals (artifacts reached only by handoff), and the ordering. That graph is the family's architecture, and it is derived, not narrated.

Layers 1–4 are mechanical and re-runnable, which satisfies the idempotency requirement that a second run over unchanged artifacts produce an identical dossier.

### What this forbids

- Restating an artifact's `description:` as a behavioural assertion. It establishes what is *declared*, and the record must say so.
- Describing a workflow "as executed" from a prompt body. No execution was observed.
- Inferring a family's purpose from its folder name — the same rule that already governs code.

## 📄 Page shapes and the content split

The requested split — *architecture description of family behaviour*, then *usage of the main functionalities* — maps onto existing chapters when the family is primary, and onto sections of one new page shape when it is accessory.

| Content | Primary family | Accessory family |
|---|---|---|
| Family behaviour and structure | Architecture chapter | *Behaviour* section of the Appendix family page |
| Usage of main functionalities | Use Cases chapter | *Usage* section of the Appendix family page |
| Precise invocation surface | Reference chapter | *Invocation surface* section |
| Orientation entry | Home / Getting Started | *Other Components* entry |

One new page shape, **Artifact family**, bound to one new template, carrying: what the family is · its entry points and invocation names · invocation order · handoff graph · what it produces · how it is implemented · and a closing **Unparented artifacts** section when the repository has any. This takes the template count from fifteen to sixteen.

Every behavioural section separates **declared** from **observed**, per F13 — the template enforces the split structurally rather than leaving it to the author's judgement.

## ⚙️ Workstreams

Ordered. Each lands in a named artifact.

### WS-1-applicability-state

Introduce the inapplicable dossier state before anything can use it. (✅ done)

- Add the third area state and its record shape to `02-evidence-dossier-schema.md`, keeping the "neither records nor gaps is a defect" rule intact by making inapplicability a record. (✅ done — new § 🚫 Inapplicability records; the defect sentence now reads "neither records, gaps nor an inapplicability record")
- Add the role-resolution test that decides applicability, stated in role terms so it carries no stack assumption. (✅ done — "none of the roles its investigator owns resolves to any path inside that component", plus the four-outcome table separating records, gap, inapplicability and defect)
- Amend `11-run-model.md` line 108 so the investigation checkpoint reads "every applicable area complete", with inapplicable areas counting as complete only when they carry an inapplicability record. (✅ done)
- Extend the coverage declaration so an inapplicable area still declares the questions it would have owned. (✅ done — 🚫 not applicable status added; full question list retained)

### WS-2-family-discovery

Give discovery reach into artifact roots and the family derivation ladder. (✅ done)

- Add artifact-type roots to `01-discovery-model.md` as a registry source independent of build participation, since no artifact root has a build entry point. (✅ done — new § 🤖 Artifact families)
- Add the four-signal derivation ladder — metadata, folder, usage adjacency, unparented — with key normalization and the invalid-value rule that rejects unsubstituted template syntax. (✅ done)
- State that usage resolves by adjacency vote and never by connected component, with the tie falling through to unparented rather than to an arbitrary pick. (✅ done — edges also inverted so no-outbound artifacts inherit from referrers)
- Add the family-root rule excluding shared parents, naming the `.github/workflows/` overlap it prevents. (✅ done — **deviation**: the overlap is named in role terms, "a parent that also holds automation definitions, already owned by `pipeline-definition`", because writing the literal path would contradict the file's own "NEVER assume a folder convention" boundary)
- Add the tiering criterion binding artifact families to the existing four-tier taxonomy. (✅ done — shipped → 🔴 Core, supports production → 🟡 Tooling, experiment → ⚪ Peripheral, no evidence → escalate)
- Record the derived family set, and the unparented count, at checkpoint 1 for grouping confirmation. (✅ done — in § Granularity, in the manager's process step 3, and in the discover prompt's output format)

### WS-3-source-set-roles

Make artifact facts investigable under the existing six-investigator structure. (✅ done)

- Add roles to `05-source-sets-and-propagation.md` for the invocable surface, the artifact-to-artifact composition graph, and the declarative bindings, each with exactly one owner. (✅ done — `invocation-surface`, `artifact-composition`, `artifact-bindings`; 15 roles → 18, verified no duplicate owner)
- Assign the behavioural roles to `ad-code-investigator` and the binding roles to `ad-configuration-investigator` rather than adding a seventh investigator — the six are organized by concern, not by component kind, and a seventh would break that orthogonality. (✅ done — rationale stated in the file itself, not left implicit)
- Add the five-layer investigation method, stating which layers are establishable and that actual effect is not, so an artifact's declared contract can never be recorded as observed behaviour. (✅ done — new § Establishing an artifact's behaviour, with the per-assertion confidence table)
- Extend the impact-propagation map so a changed artifact re-verifies the family's pages. (✅ done — three rows added; map stays bounded, no role expands to "all pages")

### WS-4-page-shape

Give authoring a shape that can hold a family. (✅ done)

- Add the Artifact family page shape to the catalogue in `04-documentation-structure.md` and update the template count from fifteen to sixteen in that file and in its References section. (✅ done — both occurrences; 16 template files on disk, all 16 named shapes resolve)
- Create `doc-artifact-family.template.md` with the behaviour, usage, invocation-surface and implementation sections, each separating declared from observed. (✅ done)
- Add the closing **Unparented artifacts** section, emitted only when the repository has any. (✅ done — placed on the *Other Components* overview, with an explicit non-zero emission condition)
- Carry the effect gap as a pre-written marked gap in the template, so the author marks it in place rather than deciding each time whether to stop. (✅ done — written into § Declared behaviour and flagged mandatory, not conditional)
- State that a primary family uses the ordinary main-chapter shapes and does **not** use this template, so the tier stays the only switch. (✅ done — stated in both the catalogue and the template header)

### WS-5-role-wiring

Connect the new capability to the agents that run it. (✅ done)

- Extend the two investigator agents whose roles changed, without widening any other agent's scope. (✅ done — capabilities, expertise, process step 2, a don't-know row and a test scenario each; the other nine agents untouched)
- Add the family handoff to `ad-documentation-manager.agent.md`. (✅ done — process steps 2, 3, 5 and 6; the discover prompt updated to match)
- Add the declared-versus-observed rule to `07-documentation-authoring-criteria.md` — the one criterion it does not already carry. Its progressive disclosure, five-way content preservation and stop-versus-mark rules all apply to a family page unchanged and need no edit. (✅ done — new § 🔎 Declared versus observed; the three existing rules were left untouched as predicted)
- Classify the effect gap as **peripheral** under that file's missing-information rule, so an author marks it in place instead of stopping and writing nothing. (✅ done)
- Confirm the handoff-graph diagram obeys the existing diagram policy — it qualifies as a topology, so every node and edge must trace to a record and no arrow may be drawn that no record establishes. (✅ done — the template carries composition as a **table**, so no new diagram was introduced; a diagram is permitted but bound explicitly to the existing policy in the template's rules)

### WS-6-portability-verification

Prove the extension did not smuggle in an assumption. (✅ done)

- Confirm no changed artifact names this repository, its folders or its domain names. (✅ done — scanned the whole domain for 11 identity terms; the only 5 hits are the pre-existing legacy prompts already parked as PL-3, none in a changed file)
- Confirm the three-repository cases resolve: many artifacts, few artifacts, none. (✅ done — many: metadata carries the foldered types, folder carries the rest, usage and unparented absorb the remainder; few: rule 1 fails, rule 2 yields one family from the enclosing folder; none: no roots discovered, absence recorded, no empty section, covered by the discover prompt's fourth test scenario)
- Re-run the domain's coherence check across the changed files. (✅ done — 0 BOM, 0 replacement characters, 16/16 named templates resolve, 18 roles with no duplicated owner, no orphaned "all six areas" statement left in the domain, `08-verification-gates.md` needs no change since its gates read pages rather than dossier completeness)

## 🗳️ Open decisions

None. The two that gated the previous draft were settled by the request itself — artifacts are ordinary components, grouped by family — and the remainder resolved from evidence as recorded in F4 through F11.

Three derivations are worth explicit confirmation during validation, since each was resolved by reading intent rather than by measurement:

| Derivation | Where it came from | If rejected |
|---|---|---|
| No seventh investigator | "ordinary components" + concern-orthogonality of the existing six | WS-3 respecifies with a new investigator and area; WS-1 unchanged |
| Primary families use ordinary shapes | "documented in all documentation sections" | WS-4 gains a primary-family shape variant |
| Grouping confirmed at checkpoint 1 rather than by a merge rule | portability — a merge heuristic tuned here would misfire elsewhere | WS-2 gains a merge criterion |

## 🔬 Discovery

- **DS1-security-area-applicability** — whether `security` resolves for an artifact family depends on whether tool grants and injection surface count as an `authn-authz-surface` or `transport-and-crypto` role. Undecidable until the roles are written in WS-3. *Negative branch*: if neither role resolves, `security` is recorded inapplicable with that reason, exactly like `data`.

## 🅿️ Park lot

- **PL-1-root-prompt-schema-debt** — seven root-level prompts violate CRITICAL `[C6]`; the `{{technical_area}}` placeholder found in F8 is the same class of defect. → defer
- **PL-2-cross-repository-artifact-index** — an index spanning several repositories. → closed: the stream documents one repository from its own evidence
- **PL-3-legacy-prompt-relocation** — moving the five Diginsight-specific prompts out of the portable domain folder. → defer
- **PL-4-artifact-family-robustness-findings** — whether the robustness stream should tier findings against artifact families as it does against code components. → defer
- **PL-5-producing-artifact-provenance** — no stamp records which prompt or agent generated a page, so a changed artifact cannot be traced to the pages it wrote (📖 F15). Affects the documentation platform's own freshness loop, not artifact-family documentation, which marks the gap instead. → defer

## 🏁 Exit criteria

Each criterion is met by a written rule that makes it hold by construction. The enforcing rule is named so the claim stays checkable.

- Every artifact in a repository resolves to exactly one family, or appears in **Unparented artifacts** — none silently dropped and none merged for tidiness. (✅ done — enforced by the first-valid-key ladder in `01-discovery-model.md` § Family derivation ladder, whose fourth rung is total, plus the explicit "never merged, never dropped" rule)
- An accessory family produces an *Other Components* entry and an Appendix family page; a primary family produces main-chapter pages; every assertion anchored to a dossier record. (✅ done — enforced by the existing major-versus-minor placement rule, the tier switch stated in `04-documentation-structure.md`, and the anchor gate in `08-verification-gates.md`, which needed no change)
- No page states an artifact's declared contract as observed behaviour — declared and observed are structurally separated. (✅ done — enforced at three levels: the layer-5 non-derivability rule in `05-source-sets-and-propagation.md`, the declared-versus-observed criterion in `07-documentation-authoring-criteria.md`, and the pre-written mandatory gap in the template)
- An inapplicable area is distinguishable from an unrun one on inspection of the dossier alone. (✅ done — enforced by the inapplicability record plus the four-outcome table in `02-evidence-dossier-schema.md`; an unrun area is the only one of the four that writes nothing)
- A repository with no AI artifacts produces a recorded absence, not a silent skip. (✅ done — stated in `01-discovery-model.md` § Artifact families, conditioned in the page-shape catalogue, and covered by the discover prompt's fourth test scenario)
- No changed artifact names this repository — the extension stays portable. (✅ done — verified by scan; the `.github/workflows/` mention the plan proposed was deliberately restated in role terms for exactly this reason)

## 📚 References

- **📖** `.copilot/context/10.00-application-development/00-stream-contract.md` — role boundaries; authoring cannot investigate
- **📖** `.copilot/context/10.00-application-development/01-discovery-model.md` — registry, priority taxonomy, purpose derivation
- **📖** `.copilot/context/10.00-application-development/02-evidence-dossier-schema.md` — the six areas; empty-dossier defect rule
- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — fixed chapter set, page shapes, major-versus-minor placement
- **📖** `.copilot/context/10.00-application-development/05-source-sets-and-propagation.md` — source-set roles and single ownership
- **📖** `.copilot/context/10.00-application-development/11-run-model.md` — six-area completion rule and checkpoints
- **📄** `src/docs/_evidence/_discovery.md` — the completed discovery run this investigation was checked against
