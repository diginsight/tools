---
description: Page format for one security requirement — the addressable unit an assessor cites, produced only for requirements the declared catalogue marks applicable
domain: "application-development"
---

# Security requirement page

**Audience**: agent. **Conditional** — produce ONLY for a requirement the declared catalogue marks **applicable**. A requirement marked not applicable stays a row in the requirement index with its reason and gets NO page (📖 `12-security-assessment-model.md`).

**Filename**: `{CONTROL-ID}-{title-slug}.md` — **never the control id alone**. Two dimensions routinely assign the same id to unrelated controls, and an id-only name silently overwrites one page with the other.

```markdown
---
title: "[CONTROL-ID] — [requirement title]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, security]
description: "[one sentence: what this requirement asks of the repository]"
source_sets:
  - authn-authz-surface
  - transport-and-crypto
  - infrastructure-definition
---

# [CONTROL-ID] — [requirement title]

- **Dimension**: [the declared dimension this requirement belongs to] ^[[area]-nn]
- **Control family**: [family name] → [`../control-families/[family].md`]
- **Status**: [implemented / partial / not implemented]

## 📄 Requirement

> [The requirement text, verbatim from the declared catalogue.] ^[[area]-nn]

## 🧩 Evidence argument

[How this repository addresses the requirement, in two to five sentences grounded in dossier records. Name the components, mechanisms and configuration that carry it. Every assertion anchored.] ^[[area]-nn]

[Where the family page carries the cross-cutting narrative, link to it rather than restating it.] → [`../control-families/[family].md`]

## ⚠️ Shortfall

[Only where status is partial or not implemented. State what is absent as an absence. NEVER describe how the gap would be exploited.] ^[[area]-nn]

**[internal]** `security.internal.md#[id]`

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]

## 📚 Reference

- **Source**: [the declared catalogue and the record within it] ^[[area]-nn]
- **Framework**: [the published framework name, where the dimension is a public one]

<!-- Source: [path or record identifier the requirement text was taken from] -->

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

- The requirement text MUST be reproduced **verbatim** from the declared catalogue. NEVER paraphrase it, and NEVER author a requirement the catalogue does not contain.
- The evidence argument MUST be grounded in this repository's dossier records and anchored like any other assertion. An argument that cannot be anchored is `not established`.
- A status MUST be evidenced. NEVER record `implemented` because a framework, platform or library usually provides the control.
- The page MUST stay small. Cross-cutting narrative belongs on the family page and MUST be linked, NEVER duplicated here.
- A shortfall MUST be stated as an absence. NEVER publish a bypass path, a payload or a reproduction step — that detail goes to the internal sibling.
- A claim about the **running system** that this repository's sources cannot confirm MUST be marked as requiring external verification, and MUST NOT be rated.
- NEVER name a customer, organisation, auditor, certification body or programme. A published, vendor-neutral framework MAY be named.

## References

- **📖** `.copilot/context/10.00-application-development/12-security-assessment-model.md` — dimensions, applicability and the naming rule
- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — sensitive classification
- **📖** `.github/templates/10.00-application-development/doc-security-requirement-index.template.md` — the index that lists this page

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
