---
description: Page format for a Tooling or Peripheral artifact family — the single page documenting a set of AI artifacts that are invoked together, separating what they declare from what was observed
domain: "application-development"
---

# Artifact family page

**Audience**: agent. One page per 🟡 Tooling or ⚪ Peripheral **artifact family**, in *Other Components* or the Appendix. This page is the family's **entire** documentation.

Use this shape instead of `doc-component-minor.template.md` when the component is an artifact family (📖 `01-discovery-model.md` § Artifact families). The minor-component shape is code-shaped and has no slot for invocation names, ordering, bindings or emitted outputs.

A 🔴 Core or 🟠 Supporting family does **not** use this shape — it is documented in the main chapters with the ordinary page shapes.

```markdown
---
title: "[family-id]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, other-components]
description: "[one sentence: what this family of artifacts is for]"
source_sets:
  - invocation-surface
  - artifact-composition
  - artifact-bindings
---

# [family-id]

- **Priority**: [🟡 Tooling | ⚪ Peripheral]
- **Grouped by**: [metadata | folder | usage]  ^[[area]-nn]
- **Roots**: [the artifact-type subtrees this family spans]
- **Members**: [n artifacts across n types]  ^[[area]-nn]

## 🎯 Derived purpose

[Why this family exists, with the derivation source named.] ^[[area]-nn]

- **Derived from**: [explicit marker | entry-point analysis | deployment descriptor | configuration defaults | sibling claim]

## 🧭 Declared behaviour

[What the family declares it does, stated as declaration. Three to six
sentences drawn from layer 1 — the artifacts' own declared descriptions,
goals, scopes and boundaries.] ^[[area]-nn]

> **Not established**: whether invoking these artifacts produces the declared
> outcome. No execution was observed, and an artifact's body is instruction
> interpreted at run time rather than mechanism that can be read statically.
> Everything above is a declaration, not a measurement. ^[gap]

## ▶️ Invocation surface

| Artifact | Type | Invoked as | Position |
|---|---|---|---|
| [file] | [artifact type] | [the name a caller uses] | [declared order, or —] |

[One sentence on how a caller enters the family, and whether any ordering is
declared or merely conventional.] ^[[area]-nn]

## 🔗 Composition

[What references, hands off to or includes what — derived from the reference
graph, not narrated as an execution.] ^[[area]-nn]

| From | To | Relation |
|---|---|---|
| [artifact] | [artifact or template] | [handoff \| reference \| include \| binding] |

## ⚙️ Bindings

| Artifact | Binds | Activation scope |
|---|---|---|
| [file] | [declared agent, tools, model] | [where it activates, or —] |

^[[area]-nn]

## 📤 Outputs

[What the family declares it produces, and where.] ^[[area]-nn]

## 🕳️ Not established

> **Not established**: [anything else sought and not found, and where it was
> sought]. ^[gap]

<!--
verification_stamp:
  generated: "[YYYY-MM-DD]"
  verified: "[YYYY-MM-DD]"
  evidence:
    - dossier: "_evidence/[family-id]/code.md"
      observed: "[YYYY-MM-DD]"
    - dossier: "_evidence/[family-id]/configuration.md"
      observed: "[YYYY-MM-DD]"
  gates: "[pass | pass-with-gaps | fail]"
  open_gaps: [n]
-->
```

## Unparented artifacts

Where discovery derived **unparented** artifacts, they get one closing section on the *Other Components* overview — never their own page, never merged into a family.

```markdown
## 🧩 Unparented artifacts

[n] artifacts resolved to no family. Each is listed with what it declares; none
is assigned to a neighbouring family.

| Artifact | Type | Invoked as | Declares |
|---|---|---|---|
| [file] | [artifact type] | [name] | [its declared purpose, as declared] |
```

Emit this section **only** when the count is non-zero. A repository with no unparented artifacts carries no empty heading.

## Rules

- **Declared is not observed.** Every behavioural statement MUST be attributable to a declaration in the artifact. NEVER restate a declared description as though the behaviour were measured, and NEVER describe a sequence "as executed" when only a declared order was read.
- The **effect gap is pre-written and mandatory**, not conditional. It is carried in place under § Declared behaviour because the gap is peripheral to the page's core purpose — architecture and usage are fully established by layers 1–4 — so the author marks and continues rather than stopping (📖 `07-documentation-authoring-criteria.md`).
- NEVER infer purpose from a folder name. Folder position is a **grouping** signal, not a purpose signal.
- A composition **diagram** is optional and governed by the existing diagram policy — warranted only where the family has more than three interacting parts, and every node and edge MUST trace to a record. NEVER draw an arrow the composition graph does not establish (📖 `07-documentation-authoring-criteria.md` § Diagram policy).
- One page per family. If a family genuinely needs more, discovery mis-tiered it — re-raise the tier rather than expanding here.
- A section with no established facts is omitted, NEVER left as an empty heading with a placeholder.

## References

- **📖** `.copilot/context/10.00-application-development/01-discovery-model.md` — artifact families, the derivation ladder and family tiering
- **📖** `.copilot/context/10.00-application-development/05-source-sets-and-propagation.md` — the artifact roles and the five-layer establishment method
- **📖** `.copilot/context/10.00-application-development/07-documentation-authoring-criteria.md` — the declared-versus-observed rule and gap marking
- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — major versus minor placement

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
  consumers:
    - "ad-documentation-author"
    - "01.02-ad-docs-write"
  changes:
    - "v1.0.0: Initial creation"
---
-->
