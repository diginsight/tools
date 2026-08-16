---
description: Page format for one security control family — produced only when the repository declares a control catalogue to document against
domain: "application-development"
---

# Security control family page

**Audience**: agent. **Conditional** — produce ONLY when discovery established that the repository declares a control catalogue. Absent a catalogue, Security carries its overview and posture pages only.

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

- **Catalogue**: [the catalogue this repository declares] ^[[area]-nn]
- **Declared in**: [path] ^[[area]-nn]

## 📋 Controls

| Control | Status | Implemented by | Established by |
|---|---|---|---|
| [control id and name] | [implemented / partial / not implemented / not applicable] | [component-id or resource] | ^[[area]-nn] |

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

- The catalogue MUST be one the repository itself declares. NEVER import an external framework the repository never referenced.
- A control status MUST be evidenced. An unevidenced control is `not established`, NEVER `implemented`.
- `not applicable` MUST carry a reason grounded in evidence, NEVER convenience.
- Absence MUST be stated as absence. NEVER describe how the missing control would be defeated.
- NEVER name an external certification body, auditor, customer or programme.

## References

- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — sensitive classification
- **📖** `.github/templates/10.00-application-development/doc-security-posture.template.md` — the unconditional companion page

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
