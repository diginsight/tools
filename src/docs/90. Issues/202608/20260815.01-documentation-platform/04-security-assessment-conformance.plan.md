---
title: "Security documentation conformance — assessment dimensions, control families and per-requirement pages"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [prompt-engineering, autonomous-streams, repository-documentation, security]
description: "Brings the application-development Security chapter up to the four-part, multi-dimension structure used by the reference documentation agents, and makes the control catalogue discovered and extensible so organisation-specific requirement sets are first-class rather than special-cased."
status: done
---

# Security documentation conformance — assessment dimensions, control families and per-requirement pages

## 📑 Table of contents

- [🎯 Goal and scope](#-goal-and-scope)
- [🔎 Findings](#-findings)
- [🧱 Derived conformance model](#-derived-conformance-model)
- [⚙️ Workstreams](#️-workstreams)
- [🗳️ Open decisions](#️-open-decisions)
- [🔬 Discovery](#-discovery)
- [🅿️ Park lot](#️-park-lot)
- [🏁 Exit criteria](#-exit-criteria)
- [� Discovery outcomes](#-discovery-outcomes)
- [�📚 References](#-references)

## 🎯 Goal and scope

Bring this domain's **Security** chapter to the same documentation structure the two reference documentation agents produce, and give it the **same flexibility** in covering organisation-specific security requirement sets alongside public standards.

Four properties the result must hold:

- **Multi-dimensional** — a repository may be assessed along more than one dimension at once (a runtime-posture standard and a development-lifecycle standard are the reference pair), and every major component must be covered for **every dimension the repository declares**.
- **Requirement-grained** — the documented unit is one requirement, on its own citable page, not a row in a family table.
- **Extensible** — an organisation-specific requirement set is a **first-class family sibling**, never a nested special case and never hard-coded.
- **Discovered** — the catalogue, its dimensions and its families come from sources the repository declares. Nothing is invented and no framework is assumed present.

**Explicitly out of scope**: any organisation's, customer's or product's **own requirement content**. Public standards MAY be named as *recognised* frameworks — they are published, vendor-neutral and not an identity. An organisation's requirement set is supported as an **input the stream reads**, never as content the domain carries. The reference agents name their customer 20 and 19 times respectively; this domain must express the identical capability with **zero** such names.

## 🔎 Findings

Investigation against the two reference agents and this domain's twelve context files, sixteen templates and eleven agents. (✅ done)

### F1-the-reference-set-is-four-part-per-component

Both reference agents organise Security **per major component**, and each component receives the same four-part set:

| # | Part | Carries |
|---|---|---|
| 1 | **Overview** | authentication schemes, authorisation model, access control |
| 2 | **Posture** | observable configuration — secrets, CORS, headers, rate limiting, transport, certificates |
| 3 | **Security architecture** | one page per control family, plus an index — the cross-cutting narrative of how the component satisfies each family as a whole |
| 4 | **Product security considerations** | an overview table of *every* applicable requirement, plus **one small page per requirement** |

Part 3 is explicitly described as *"where the narrative depth lives, so the per-requirement pages can stay small"*. The split is a load-bearing design choice, not decoration: it is what keeps a catalogue of a hundred requirements navigable.

### F2-we-carry-two-of-the-four-parts

`04-documentation-structure.md` binds Security to exactly two page shapes — `doc-security-posture.template.md` and `doc-security-control-family.template.md` — and states the second is conditional on *"the repository declares a control catalogue to document against"*.

So parts 2 and 3 exist in some form. Part 1 is absorbed into the chapter overview, and **part 4 does not exist at all**: our control-family template collapses each control into a table row —

```markdown
| Control | Status | Implemented by | Established by |
```

— with no slot for the requirement's own text, no per-requirement evidence argument, and no citable anchor.

### F3-the-per-requirement-page-is-the-real-difference

A table row cannot carry what the reference requirement page carries: the **verbatim requirement**, a repository-grounded `### Evidence Argument`, and a `## Reference` block reproducing the original requirement text, its discussion and its source anchor.

Three consequences make this structural rather than cosmetic:

- **Citability** — an assessor, an auditor or a later run refers to *one requirement*. A row inside a family page has no address.
- **Bounded growth** — narrative lives once in the family page; each requirement page stays small regardless of catalogue size.
- **Dialogue durability** — when an assessment produces questions and answers, they attach to a requirement. Rows cannot absorb that without the family page degenerating into a transcript.

### F4-control-ids-collide-across-dimensions

The reference agents carry an explicit warning with a worked example: two assessment frameworks can assign **the same control ID to different controls** — a runtime `SI-2` meaning flaw remediation versus a lifecycle `SI-2` meaning secure coding standards.

Their answer is a filename rule: `{CONTROL-ID}-{title-slug}.md`, *"never the control ID alone"*, because the collision silently overwrites one page with the other. Any per-requirement shape we add inherits this hazard the moment a second dimension exists, so the rule must come with it.

### F5-the-catalogue-is-discovered-not-declared-in-the-abstract

Our conditional reads *"the repository declares a control catalogue"* but never says how that declaration is found. The reference agents are concrete about the mechanism while staying stack-agnostic: the catalogue is *"discovered, not invented"*, sourced from assessment workbooks under a **discovered** security-artifacts folder, with the paths marked illustrative.

This matters for the same reason out-of-tree configuration mattered: a catalogue the run cannot locate produces a Security chapter that looks complete and is silently missing its entire requirement dimension.

### F6-organisation-specific-families-are-siblings-not-a-nested-node

This is precisely the flexibility the request asks for, and the reference agents state it twice, emphatically:

> each a first-level control-family sibling of the [public-standard] families, **never collapsed under a single [organisation] grouping node**

The rule generalises cleanly: **a family is a family regardless of who authored it.** An organisation-specific requirement set participates in the same catalogue, the same family index, the same requirement pages and the same conformance rule as a public standard. Nesting it would make organisation-specific requirements structurally second-class and, in practice, easy to skip.

The reference wording also leaves the set open — *"plus any other organization-specific families"* — which is the extension point to preserve.

### F7-a-genuine-conflict-with-our-absence-rule

`ad-security-investigator.agent.md` instructs: *"State an absent control as an **absence** — 'no authorisation attribute is applied to this operation', never 'this operation is anonymous'."*

The reference agents carry an apparently opposite rule:

> **never write a negative security assertion based on absence of evidence in repository files alone** — if a control is not observable in source, configuration, IaC or pipeline definitions, classify it as an **Unverifiable assertion**

These are **not** actually contradictory, and the resolution is a scope distinction our agent half-expresses already:

| Claim | Scope | Verdict |
|---|---|---|
| "no authorisation attribute is applied to this operation" | an observation **about the repository** | publishable — this is what we observed |
| "this operation is unauthenticated" | a claim about the **running system**, which may be governed by an identity provider policy, a network control or a gateway | **not** publishable from repository evidence alone |

Our existing wording forbids the second by example but never states the principle, and offers no protocol for the case. The reference supplies one: qualify with *"requires external verification"*, mark a follow-up, ask the user, and never rate severity until confirmed. That protocol is a real addition, and it is the one place where adopting the reference structure **changes an existing rule** rather than adding beside it.

### F8-the-reference-agents-are-not-portable-and-we-must-not-copy-that

Measured: the customer name appears **19** times in one reference agent and **20** in the other, woven into rule text — control-family lists, framework references, folder descriptions and the conformance rule itself.

Adopting the structure therefore cannot be a transcription. Every organisation-specific reference must be re-expressed as a **role** the repository fills:

| Reference names | We express as |
|---|---|
| a specific runtime cloud-security standard | an assessment dimension the repository declares |
| a specific secure-development-lifecycle standard | a second declared dimension, whose families are lifecycle practices |
| a named organisation's requirement families | organisation-specific families, discovered from the catalogue |
| a named assessment workbook | the declared catalogue source |

### F9-what-we-already-have-is-ahead-of-the-reference

Three things need no work, and one is stronger here than in the reference:

- **Exposure split** — our published/`.internal.md` split, the four-class sensitive test in `03-evidence-access-policy.md`, and the never-in-nav rule match the reference's model.
- **Evidence traceability** — our `^[{area}-nn]` anchors and `verification_stamp` are *stricter*: the reference has no equivalent of an assertion that cannot be published without a resolvable evidence record.
- **Probe prohibition** — *"NEVER exercise, probe, fuzz or bypass a control, in any environment"* has no reference counterpart and must survive unchanged.

## 🧱 Derived conformance model

Derived from F1–F8; recorded here as analysis, not yet as an actionable body. (✅ done)

### Dimensions

An **assessment dimension** is one requirement framework the repository declares itself assessed against. Dimensions are discovered, never assumed, and the domain names none of them.

The conformance rule generalises the reference's "both dimensions or it is a gap" without fixing the count at two:

> Every major component MUST be covered for **every dimension the repository declares**. A component covered for one dimension and not another is **non-conforming** — a coverage-gap record, never a silent omission.

This preserves the reference's guarantee on a repository that declares two dimensions, and behaves correctly on one that declares one, three, or none.

### Families

A **control family** is a grouping the catalogue itself defines. Families from a public standard and families from an organisation-specific requirement set are **peers**: same level, same index, same page shape, same conformance obligation. No family is nested beneath a container that marks its origin.

### Requirements

The documented unit. One page per applicable requirement, named `{CONTROL-ID}-{title-slug}` to survive cross-dimension ID collision (📖 F4), carrying the verbatim requirement, a repository-grounded evidence argument, and a reference block with its source anchor.

### Folder shape

```
security/
  overview.md
  {component}/
    overview.md
    posture.md
    posture.internal.md              # never in nav
    control-families/
      overview.md                    # index across every dimension
      {family}.md
    requirements/
      overview.md                    # every applicable requirement, grouped by family, sorted by id
      {CONTROL-ID}-{title-slug}.md
```

The `{component}` segment follows the existing component-pivot rule in `04-documentation-structure.md` — it is omitted when only one component is relevant to the chapter, so this adds no new layout mode.

### Where each part comes from

| Part | Established by | Source set |
|---|---|---|
| Overview, Posture | `ad-security-investigator` | `authn-authz-surface`, `transport-and-crypto`, `secret-references` |
| Family narrative | `ad-security-investigator` | the same, read against the family's concern |
| Requirement text and reference block | the declared catalogue | a new role — see DS1 |
| Evidence argument | existing dossier records | `^[security-nn]` anchors, unchanged |

## 🗳️ Open decisions

All four resolved; recorded with their resolutions. (✅ done)

### OD1-scope-of-organisation-specific-material — resolved

**Resolution**: an organisation's or customer's **own requirement content is out of scope** for this domain. Public standards — **NIST SP 800-53 Rev.5** and **IEC 62443-4-1** are the two the reference pair uses — **are** in scope and MAY be named as recognised frameworks, because they are published and vendor-neutral. What the agents must gain is the **capability to investigate against an organisation-specific requirement file** supplied to a run, structurally identical to how they handle a public standard.

Consequence for the model: a dimension is a *role*, and a catalogue is an *input*. The domain recognises public frameworks by name as examples; it never requires one, and it never embeds anyone's requirement text.

### OD2-catalogue-source-format — resolved

**Resolution**: **discovered format, gap when unreadable.** The catalogue source is whatever the repository declares. When the stream cannot read it, that is a recorded gap naming the source — never a silently empty requirement set.

### OD3-requirement-page-volume — resolved

**Resolution**: **pages only for applicable requirements.** A requirement the catalogue marks not applicable stays a row in the requirements index carrying its reason, and gets no page.

### OD4-absence-rule-amendment — resolved

**Resolution**: **apply the scope split** of F7. Repository-scoped observations remain publishable unchanged; running-system claims gain the external-verification protocol.

## ⚙️ Workstreams

### WS-1-security-assessment-model-context (✅ done)

Create `.copilot/context/10.00-application-development/12-security-assessment-model.md` — the single authority for the vocabulary this plan introduces.

Must define: **assessment dimension** (declared, discovered, never assumed) · the **conformance rule** of F1 generalised to N dimensions · **control family** as a catalogue-defined peer grouping regardless of author (F6) · **requirement** as the documented unit · **catalogue discovery** and the unreadable-source gap (OD2) · the `{CONTROL-ID}-{title-slug}` collision rule with its worked rationale (F4) · the applicable-only page rule (OD3) · named public frameworks as **recognised examples, never requirements** (OD1).

Created at v1.0.0.

### WS-2-documentation-structure-page-shapes (✅ done)

Update `04-documentation-structure.md`: add the two new page shapes to the catalogue, state the per-component Security folder shape, restate the control-family conditional in dimension terms, and update **both** template-count statements from sixteen to eighteen.

v1.1.0 → v1.2.0. Also corrected a stale rhetorical count in a rationale line, where a previous fifteen-to-sixteen bump had overwritten the chapter-derived figure.

### WS-3-two-new-templates (✅ done)

Add `doc-security-requirement.template.md` (verbatim requirement, repository-grounded evidence argument, reference block with source anchor) and `doc-security-requirement-index.template.md` (scope, at-a-glance counts, every applicable requirement grouped by family and sorted by id, non-applicable rows with reasons).

Both created at v1.0.0. The index also carries the declared-dimensions table that makes an uncovered dimension visible.

### WS-4-control-family-template-extension (✅ done)

Extend `doc-security-control-family.template.md`: attribute each family to its dimension, state the peer rule, and link each control row to its requirement page where one exists.

v1.0.0 → v1.1.0. Gained a *How this component satisfies the family* narrative section — the depth that lets requirement pages stay small — and its "never name an external certification body" rule was relaxed to permit published, vendor-neutral framework names per OD1.

### WS-5-security-investigator (✅ done)

Update `ad-security-investigator.agent.md`: resolve the declared catalogue, establish dimensions and coverage, produce per-requirement evidence arguments, apply the OD4 scope split with its external-verification protocol, and emit open items. Bump to 1.1.0.

Process went from 10 to 13 steps; two don't-know rows and two test scenarios added.

### WS-6-propagation-and-applicability (✅ done)

Wire the catalogue into `05-source-sets-and-propagation.md` (a change to it re-verifies the security pages) and `02-evidence-dossier-schema.md` (a declared catalogue is a third trigger making `security` applicable, alongside `authn-authz-surface` and `transport-and-crypto`). Resolves DS1 and DS2 in passing.

DS1 resolved **towards a role**: `security-catalogue`, owned by `ad-security-investigator`. That choice made the applicability question answer itself — the existing mechanical role-resolution test already makes an area applicable when any role its investigator owns resolves, so no special case was needed. One clarifying sentence was added for repository-level roles, since a catalogue covers a component rather than sitting inside it. DS2 resolved **towards no new gate**: the conformance rule lives in the model and is carried by the existing coverage declaration.

### WS-7-verification (✅ done)

Re-run the identity-leakage scan extended with the reference agents' customer and product tokens, the U+FFFD/BOM scan over every changed file, and the template-count and named-shape-resolves checks.

Results: `bom=0 fffd=0` · `templates_on_disk=18` · `named_in_structure=18 missing=0` · count statement reads `eighteen`. Identity leakage is confined to the same five pre-existing legacy prompt files parked previously; no changed or created file carries a customer, organisation or product name.

## 🏁 Exit criteria

- Every declared dimension is covered for every major component, or the shortfall is a recorded non-conformance. (✅ done)
- An organisation-specific requirement file can be documented against with no domain change. (✅ done)
- No customer, organisation, product or environment name appears anywhere in the domain. (✅ done)
- Both template-count statements agree with the file count on disk. (✅ done)

## 🔬 Discovery outcomes

- **DS1-catalogue-role-ownership** — resolved towards a distinct role. `security-catalogue` is owned by `ad-security-investigator` and carries its own propagation row. (✅ done)
- **DS2-conditional-gate-behaviour** — resolved towards no new gate. The conformance rule lives in `12-security-assessment-model.md` and rides the existing coverage declaration. (✅ done)

## 🔬 Discovery

- **DS1-catalogue-role-ownership** — whether the declared catalogue needs its own source-set role (a `control-catalogue` alongside `authn-authz-surface`) or resolves through the existing `settings-sources` mechanism. Undecidable until OD2 fixes the source format. *Negative branch*: if no distinct role is warranted, the catalogue resolves as ordinary repository evidence and no role is added.
- **DS2-conditional-gate-behaviour** — whether `08-verification-gates.md` needs a conformance gate that fails when a declared dimension is uncovered, or whether the existing coverage-declaration mechanism already carries it. *Negative branch*: if the coverage declaration suffices, no gate is added and the rule lives in the dossier schema.

## 🅿️ Park lot

- **PL-1-security-finding-routing** — whether a coverage gap on a declared dimension should also raise a robustness-stream finding, not only a documentation gap. → defer
- **PL-2-reference-agent-portability-audit** — the two reference agents are themselves non-portable; whether to offer a generalised version back to them is outside this repository's stream. → closed: this domain owns its own artifacts only
- **PL-3-assessment-reconciliation-mode** — the reference agents also reconcile a *reviewed* assessment workbook (assessor questions plus product-team answers) against current code, with a "the code wins" rule and an open-items output. Orthogonal to page structure and not requested. → defer

## 📚 References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — chapter set, page-shape catalogue, component pivot
- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — sensitive classification and the published/internal split
- **📖** `.copilot/context/10.00-application-development/02-evidence-dossier-schema.md` — coverage declaration and the inapplicability record
- **📖** `.github/agents/10.00-application-development/ad-security-investigator.agent.md` — the absence rule amended by F7
- **📖** `.github/templates/10.00-application-development/doc-security-control-family.template.md` — the shape extended by F2
