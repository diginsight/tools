---
description: Page format for the security requirement index — the single at-a-glance table of every requirement in the declared catalogue, grouped by family and sorted by control id
domain: "application-development"
---

# Security requirement index page

**Audience**: agent. **Conditional** — produce ONLY when the repository declares an assessment catalogue. This is the one page that accounts for **every** requirement, applicable or not, so a reader can tell coverage from omission (📖 `12-security-assessment-model.md`).

```markdown
---
title: "Security requirements — [component-id]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, security]
description: "Every requirement this component is assessed against, and where each is documented."
source_sets:
  - authn-authz-surface
  - transport-and-crypto
  - infrastructure-definition
---

# Security requirements — [component-id]

## 🎯 Scope

[What this component is assessed against and where that was declared, in two or three sentences.] ^[[area]-nn]

- **Catalogue source**: [path or record] ^[[area]-nn]

## 🧭 Declared dimensions

| Dimension | Framework | Families | Applicable requirements | Coverage |
|---|---|---|---|---|
| [dimension name] | [published framework name, or `organisation-specific`] | [n] | [n] | [covered / partial / **not covered**] |

> A dimension the repository declares and this component does not cover is a **non-conformance**, not an omission. It MUST appear in this table marked `not covered`.

## 📊 At a glance

| | Count |
|---|---|
| Requirements in catalogue | [n] |
| Applicable | [n] |
| Not applicable | [n] |
| Implemented | [n] |
| Partial | [n] |
| Not implemented | [n] |

## 📋 Requirements

### [control family name] — [dimension name]

| Control | Requirement | Status | Page |
|---|---|---|---|
| [CONTROL-ID] | [short title] | [implemented / partial / not implemented] | [`requirements/[CONTROL-ID]-[title-slug].md`] |
| [CONTROL-ID] | [short title] | not applicable — [reason] | — |

[Repeat one section per control family. Families from different dimensions are peers and appear at the same level, in catalogue order.]

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought — an unreadable catalogue source belongs here, naming the source]. ^[gap]

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

- The index MUST list **every** requirement the catalogue contains, applicable or not. A requirement omitted here is indistinguishable from one the catalogue never had.
- A `not applicable` row MUST carry its reason and MUST NOT link to a page. Only applicable requirements get pages.
- Rows MUST be grouped by control family and sorted by control id within each group.
- Families from different dimensions MUST appear as **peers**. NEVER nest a dimension's families beneath a grouping node marking their origin.
- Every declared dimension MUST appear in the dimensions table, including one this component does not cover — recorded as `not covered`.
- An unreadable catalogue source MUST be recorded as a gap naming the source. NEVER present it as an empty catalogue.
- NEVER name a customer, organisation, auditor, certification body or programme. A published, vendor-neutral framework MAY be named.

## References

- **📖** `.copilot/context/10.00-application-development/12-security-assessment-model.md` — dimensions, applicability and the conformance rule
- **📖** `.github/templates/10.00-application-development/doc-security-requirement.template.md` — the per-requirement page this index links to
- **📖** `.github/templates/10.00-application-development/doc-security-control-family.template.md` — the family narrative

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
