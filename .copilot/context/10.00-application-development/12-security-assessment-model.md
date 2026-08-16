---
title: "Security assessment model"
description: "How a repository's security assessment is modelled — declared dimensions, catalogue-defined control families, per-requirement pages, catalogue discovery and the conformance rule that makes an uncovered dimension visible"
domain: "application-development"
goal: "Let the Security chapter document a repository against any number of requirement frameworks — public or organisation-specific — with the same structure, the same conformance guarantee and no framework hard-coded into the domain"
scope:
  covers:
    - "Assessment dimensions and how they are declared"
    - "Control families as catalogue-defined peers"
    - "The requirement as the documented unit"
    - "Catalogue discovery and the unreadable-source gap"
    - "Requirement page naming and cross-dimension id collision"
    - "The coverage conformance rule"
  excludes:
    - "Page shapes and folder layout (see 04-documentation-structure.md)"
    - "What may and may not be published (see 03-evidence-access-policy.md)"
    - "Evidence records, gaps and the internal split (see 02-evidence-dossier-schema.md)"
    - "Requirement text itself — no catalogue content is carried by this domain"
boundaries:
  - "NEVER assume a framework applies to a repository that has not declared it"
  - "NEVER carry any organisation's requirement text, control list or assessment content in this domain"
  - "NEVER treat an unreadable catalogue source as an empty one"
  - "NEVER nest one requirement set beneath another to mark its origin"
rationales:
  - "Fixing the frameworks in the domain would make it work for one organisation and silently mis-document every other"
  - "A requirement is what an assessor cites, so it must be the addressable unit — a table row has no address"
  - "Two frameworks routinely reuse the same control id for different controls, so id-only naming loses a page without any error"
  - "An uncovered dimension looks exactly like a complete chapter unless conformance is stated as a rule"
---

# Security assessment model

**Purpose**: How the Security chapter is organised when a repository is assessed against one or more requirement frameworks.

**Referenced by**: `04-documentation-structure.md`, `ad-security-investigator.agent.md`, `doc-security-control-family.template.md`, `doc-security-requirement.template.md`, `doc-security-requirement-index.template.md`

---

## 🧭 Assessment dimensions

An **assessment dimension** is one requirement framework a repository declares itself assessed against.

Dimensions are **discovered from the repository, never assumed**. A repository may declare none, one, or several, and the model behaves identically at every count.

Two dimension *kinds* recur often enough to name, as recognised examples only:

| Kind | Asks | A public framework of this kind |
|---|---|---|
| **Runtime posture** | what the running system protects, and how | **NIST SP 800-53 Rev.5** |
| **Development lifecycle** | how the software was specified, built and maintained | **IEC 62443-4-1** |

> Naming these is a **recognition aid, not a requirement**. A repository declaring neither is not deficient; a repository declaring an organisation-specific framework instead is treated exactly the same way. This domain MUST NOT require, presume or default to any framework.

---

## 🧩 Control families

A **control family** is a grouping the catalogue itself defines. The domain MUST NOT invent, rename or reorder families.

Families from different dimensions are **peers**: same level, same index, same page shape, same obligation. A family authored by a standards body and a family authored by an organisation sit side by side.

> **NEVER** collapse a dimension's families beneath a single grouping node that marks their origin. A lifecycle framework's practices are first-level families, not children of a "lifecycle" container. Nesting makes one requirement set structurally second-class and, in practice, easy to skip.

The family page is where **narrative depth lives** — how the component satisfies the family as a whole. This is what keeps the per-requirement pages small enough to remain useful at catalogue scale.

---

## 📄 Requirements

The **requirement** is the documented unit, because it is the unit an assessor, an auditor or a later run cites.

- Each **applicable** requirement MUST have its own page.
- A requirement the catalogue marks **not applicable** MUST stay a row in the requirements index carrying its stated reason, and MUST NOT receive a page.
- A requirement page carries the requirement verbatim, an evidence argument grounded in this repository's dossier records, and a reference block naming its source.
- An evidence argument MUST cite dossier records like every other assertion (📖 `05-source-sets-and-propagation.md`). A requirement page is not exempt from traceability.

---

## 🔎 Catalogue discovery

The catalogue is **discovered, never invented**.

1. Locate the source the repository declares — a requirements file, an assessment record, a catalogue export. Its **location and format are discovered**, never presumed.
2. Read the dimensions, families, requirements and applicability it states.
3. If no source is declared, `security` carries its overview and posture pages only, and the absence is stated.
4. If a source is declared but **cannot be read** — unsupported format, unresolvable path, unreadable encoding — record a **gap naming the source**.

> An unreadable catalogue MUST NEVER be reported as an empty one. The two produce identical-looking chapters and mean opposite things.

---

## 🏷️ Requirement page naming

A requirement page MUST be named `{CONTROL-ID}-{title-slug}` — **never the control id alone**.

Two frameworks routinely assign the same id to unrelated controls: a runtime framework's `SI-2` may mean flaw remediation while a lifecycle framework's `SI-2` means secure coding standards. Under id-only naming the second page overwrites the first, and nothing reports it — the chapter simply carries one fewer requirement than the catalogue declares.

---

## ✅ Conformance rule

> Every major component MUST be covered for **every dimension the repository declares**.

A component covered for one declared dimension and not another is **non-conforming**. That shortfall MUST be recorded as a coverage gap naming the component and the uncovered dimension. It MUST NEVER be resolved by omitting the dimension, and MUST NEVER be left implicit.

Applicability follows: a repository that declares a catalogue makes `security` **applicable**, independently of whether any other security source set resolves.

---

## 🚫 What this model never does

- **NEVER** carries any organisation's requirement text, control list, assessment answers or reviewer commentary — those are inputs a run reads, not content this domain holds.
- **NEVER** names a customer, organisation, product or environment. Public frameworks are nameable; identities are not.
- **NEVER** asserts a requirement is satisfied because a framework, platform or library usually provides the control.

---

## 📚 References

- **📖** `04-documentation-structure.md` — the Security page shapes and folder layout this model fills
- **📖** `02-evidence-dossier-schema.md` — records, gaps, coverage declaration and applicability
- **📖** `03-evidence-access-policy.md` — what a security page may and may not publish
- **📖** `05-source-sets-and-propagation.md` — traceability anchors and re-verification on source change

<!--
context_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
-->
