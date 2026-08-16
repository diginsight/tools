---
description: Page format for one security control family — the cross-cutting narrative of how a component satisfies a family as a whole, produced per declared assessment dimension
domain: "application-development"
---

# Security control family page

**Audience**: agent. **Conditional** — produce ONLY when discovery established that the repository declares an assessment catalogue. Absent a catalogue, Security carries its overview and posture pages only.

This page is where the **narrative depth** lives. Keeping it here is what lets the per-requirement pages stay small at catalogue scale (📖 `12-security-assessment-model.md`).

```markdown
---
title: "[control family name]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, security]
description: "[one sentence: what this family of controls covers]"
source_sets:
  - authn-authz-surface
  - transport-and-crypto
  - infrastructure-definition
---

# [control family name]

## 🎯 What this family covers

[The concern the family addresses, in the catalogue's own terms.] ^[[area]-nn]

- **Dimension**: [the declared dimension this family belongs to] ^[[area]-nn]
- **Framework**: [the published framework name, or `organisation-specific`] ^[[area]-nn]
- **Declared in**: [path] ^[[area]-nn]

## 🧩 How this component satisfies the family

[The cross-cutting narrative — the mechanisms, boundaries and decisions that address this family as a whole, rather than requirement by requirement. This is the depth that the per-requirement pages link back to instead of repeating.] ^[[area]-nn]

## 📋 Controls

| Control | Status | Implemented by | Page | Established by |
|---|---|---|---|---|
| [control id and name] | [implemented / partial / not implemented / not applicable] | [component-id or resource] | [`../requirements/[CONTROL-ID]-[title-slug].md`] | ^[[area]-nn] |

## 🧩 Implementation notes

[Only where the mapping is non-obvious. One short paragraph per control.] ^[[area]-nn]

## ⚠️ Partial and missing controls

| Control | What is absent | Severity | Detail |
|---|---|---|---|
| [control id] | [stated as absence] | [🔴/🟠/🟡] | **[internal]** `security.internal.md#[id]` |

## 📌 Not applicable

| Control | Why it does not apply | Established by |
|---|---|---|
| [control id] | [reason grounded in evidence] | ^[[area]-nn] |

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]

<!--
verification_stamp:
  generated: "[YYYY-MM-DD]"
  verified: "[YYYY-MM-DD]"
  evidence:
    - dossier: "_evidence/[component-id]/security.md"
      observed: "[YYYY-MM-DD]"
  gates: "[pass | pass-with-gaps | fail]"
  open_gaps: [n]
-->
```

## Rules

- The catalogue MUST be one the repository itself declares. NEVER import a framework the repository never referenced.
- One page per family **per declared dimension**. Families from different dimensions are **peers** — NEVER nest a dimension's families beneath a grouping node marking their origin.
- A control status MUST be evidenced. An unevidenced control is `not established`, NEVER `implemented`.
- `not applicable` MUST carry a reason grounded in evidence, NEVER convenience.
- The narrative belongs here; a requirement page MUST link to it rather than repeat it.
- A control row MUST link to its requirement page where the requirement is applicable, and MUST leave the link empty where it is not.
- Absence MUST be stated as absence. NEVER describe how the missing control would be defeated.
- NEVER name a customer, organisation, auditor, certification body or programme. A published, vendor-neutral framework MAY be named.

## References

- **📖** `.copilot/context/10.00-application-development/12-security-assessment-model.md` — dimensions, families and the peer rule
- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — sensitive classification
- **📖** `.github/templates/10.00-application-development/doc-security-requirement.template.md` — the per-requirement pages this family links to
- **📖** `.github/templates/10.00-application-development/doc-security-posture.template.md` — the unconditional companion page

<!--
---
template_metadata:
  version: "1.1.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
  consumers:
    - "ad-documentation-author"
    - "01.02-ad-docs-write"
  changes:
    - "v1.0.0: Initial creation"
    - "v1.1.0: Dimension attribution, family narrative section, requirement page links, peer rule; public frameworks nameable"
---
-->
