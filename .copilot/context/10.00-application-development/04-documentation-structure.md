---
title: "Documentation structure — chapters, placement and page shapes"
description: "The canonical eleven-chapter set, the component pivot rule, the page-shape catalogue that decides the template count, major-versus-minor placement and the mapping onto existing folders via metadata.yml"
domain: "application-development"
goal: "Make placement deterministic — given a component and a fact, exactly one chapter and exactly one page shape are correct, so two runs over the same repository place the same fact identically"
scope:
  covers:
    - "The eleven canonical chapters"
    - "Component pivot rule below a chapter"
    - "Page-shape catalogue and its template binding"
    - "Major versus minor placement by component priority"
    - "Mapping chapters onto existing folders through metadata.yml"
    - "Placement tie-breakers"
  excludes:
    - "How a page is written once placed (see 07-documentation-authoring-criteria.md)"
    - "Component priority definitions (see 01-discovery-model.md)"
    - "Template bodies (see .github/templates/10.00-application-development/)"
boundaries:
  - "The chapter set is FIXED — a stream MUST NOT invent, merge or rename a chapter"
  - "NEVER rename an existing folder to realise a chapter — use metadata.yml"
  - "Every page shape MUST bind to exactly one template, and every template MUST be bound by at least one shape"
rationales:
  - "A fixed chapter set is what lets a reader move between two documented repositories without relearning the layout"
  - "Deriving the template count from page shapes rather than chapters prevents fifteen near-identical templates or one template stretched over incompatible content"
  - "Realising chapters through metadata.yml keeps existing folder names and their inbound links intact"
---

# Documentation structure

**Purpose**: The canonical chapter set, placement rules and page-shape catalogue for repository-derived documentation.

**Referenced by**:
- `ad-documentation-manager.agent.md`, `ad-documentation-author.agent.md`, `ad-documentation-verifier.agent.md`
- `.github/prompts/10.00-application-development/01.02-ad-docs-write.prompt.md`
- `.github/templates/10.00-application-development/doc-documentation-structure.template.md`

---

## 📚 The eleven chapters

Fixed and always present. A chapter with nothing to say carries its overview page stating that, rather than being omitted.

| # | Chapter | Answers |
|---|---|---|
| 1 | **Home** | what this repository is, in one page |
| 2 | **Getting Started** | how a newcomer builds and runs it |
| 3 | **Architecture** | how it is structured and why |
| 4 | **Use Cases** | what an actor can accomplish with it |
| 5 | **Infrastructure** | what is provisioned, per environment |
| 6 | **Reference** | the precise surface — types, keys, operations, tables |
| 7 | **Other Components** | 🟡 Tooling and ⚪ Peripheral components |
| 8 | **Validation** | what is tested and what that proves |
| 9 | **Security** | the observable security posture |
| 10 | **DevOps** | how it is built, gated and shipped |
| 11 | **Appendix** | glossary, decision record, superseded material |

---

## 🧭 Component pivot

Below a chapter, subfolders are **component-pivoted** when two or more components are relevant to that chapter, and **flat** otherwise. The decision is per chapter, not per repository — a repository may pivot Reference while keeping Getting Started flat.

---

## 🧩 Page-shape catalogue

The authority that decides the template count. Every shape binds to exactly one template.

| Page shape | Template | Used by chapters |
|---|---|---|
| Chapter overview | `doc-chapter-overview.template.md` | all eleven, including Home and Getting Started |
| System and logical architecture | `doc-architecture-page.template.md` | Architecture |
| Reference entry | `doc-reference-entry.template.md` | Reference |
| API unit | `doc-api-unit.template.md` | Reference |
| Use case | `doc-use-case.template.md` | Use Cases |
| Environment | `doc-infrastructure-environment.template.md` | Infrastructure |
| Security posture | `doc-security-posture.template.md` | Security |
| Security control family | `doc-security-control-family.template.md` | Security |
| Pipeline | `doc-devops-pipeline.template.md` | DevOps |
| Validation unit | `doc-validation-unit.template.md` | Validation |
| Minor component | `doc-component-minor.template.md` | Other Components, Appendix |

Three further templates are **structural** rather than page shapes — `doc-documentation-structure.template.md`, `doc-evidence-dossier.template.md`, `doc-mermaid-patterns.template.md` — and one belongs to the robustness stream, `finding-record.template.md`.

> **Security control family** is conditional: it is produced only when the repository declares a control catalogue to document against. Absent a catalogue, Security carries its overview and posture pages only.

---

## 🔴 Major versus minor placement

| Priority | Documented in |
|---|---|
| 🔴 Core, 🟠 Supporting | the main chapters — Architecture, Use Cases, Reference, Validation, Security, DevOps, Infrastructure |
| 🟡 Tooling, ⚪ Peripheral | *Other Components* and the Appendix, and nowhere else |

A 🔴 Core component whose only page sits under *Other Components* is a **defect**, not a stylistic choice — it means discovery mis-tiered it or placement ignored the tier.

The reverse is equally a defect: a build script promoted into Architecture inflates the apparent system and buries the components that matter.

---

## 🗺️ Mapping onto existing folders

Chapters are realised through per-folder `metadata.yml`, so **existing folders keep their names** and their inbound links.

| Key | Effect |
|---|---|
| `label` | the chapter name shown in navigation |
| `order` | position in the chapter sequence |
| `icon` | the chapter's navigation icon |
| `hidden` | excludes a folder from navigation without deleting it |

### Worked example — this repository

`src/docs/` currently holds two content folders. Neither is renamed.

| Existing folder | `metadata.yml` | Result |
|---|---|---|
| `80. Usecases/` | `label: Use Cases`, `order: 4` | becomes the Use Cases chapter in place |
| `90. Issues/` | `label: Issues`, `order: 90` | **not** one of the eleven — working documents (plans, investigations) that sit outside the chapter set |

Missing chapters are created as new folders alongside them. A folder outside the chapter set is legitimate; it simply carries no page shape and is never a placement target for generated content.

---

## ⚖️ Placement tie-breakers

Given a component and a fact, exactly one chapter is correct. Apply in order and stop at the first that resolves.

| # | Rule |
|---|---|
| 1 | If the fact describes a **precise surface** a caller must match exactly — a type, a key, an operation, a table — it belongs to **Reference**, wherever else it is also interesting |
| 2 | If it describes **what an actor achieves**, it belongs to **Use Cases** |
| 3 | If it describes **what is provisioned or where it runs**, it belongs to **Infrastructure** |
| 4 | If it describes **how the repository is built or shipped**, it belongs to **DevOps** |
| 5 | If it describes **what is proven and how**, it belongs to **Validation** |
| 6 | If it describes **a control or an exposure**, it belongs to **Security** |
| 7 | If it describes **structure or a design rationale**, it belongs to **Architecture** |
| 8 | Otherwise it is orientation material and belongs to **Home** or **Getting Started** |

A fact that resolves to two chapters is written **once** in the chapter that owns it and **linked** from the other. Duplicating it guarantees the two copies diverge on the next update.

---

## References

- **📖** `01-discovery-model.md` — component priority and layout mode
- **📖** `05-source-sets-and-propagation.md` — how a changed source role maps to affected pages
- **📖** `07-documentation-authoring-criteria.md` — how a page is written once placed
- **📖** `08-verification-gates.md` — navigation coverage and placement gates
- **📖** `.github/templates/10.00-application-development/` — the fifteen templates bound above

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
