---
description: Page format for a Tooling or Peripheral component — the compact single page that is the entire documentation of a minor component
domain: "application-development"
---

# Minor component page

**Audience**: agent. One page per 🟡 Tooling or ⚪ Peripheral component, in *Other Components* or the Appendix. This page is the component's **entire** documentation.

```markdown
---
title: "[component-id]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, other-components]
description: "[one sentence: what this component is for]"
source_sets:
  - entry-points
  - composition-root
---

# [component-id]

- **Priority**: [🟡 Tooling | ⚪ Peripheral]
- **Path**: [repository-relative root]
- **Deployed**: [yes, where | no]  ^[[area]-nn]

## 🎯 Derived purpose

[Why this component exists, with the derivation source named. This is the
section that stops a deliberate sample being read as dead code.] ^[[area]-nn]

- **Derived from**: [explicit marker | entry-point analysis | deployment descriptor | configuration defaults | sibling claim]

## ⚙️ What it does

[Three to six sentences. Layer 1 and 2 only — a minor component does not get
layer 3.] ^[[area]-nn]

## ▶️ How it is used

[The invocation or trigger, and by whom.] ^[[area]-nn]

## 🔗 Dependencies

| Depends on | Established by |
|---|---|
| [component-id or external role] | ^[[area]-nn] |

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]

<!--
verification_stamp:
  generated: "[YYYY-MM-DD]"
  verified: "[YYYY-MM-DD]"
  evidence:
    - dossier: "_evidence/[component-id]/code.md"
      observed: "[YYYY-MM-DD]"
  gates: "[pass | pass-with-gaps | fail]"
  open_gaps: [n]
-->
```

## Rules

- A minor component MUST NOT gain a page in Architecture, Use Cases, Reference, Validation, Security, DevOps or Infrastructure. Promoting a build script into Architecture inflates the apparent system.
- **Derived purpose is mandatory.** A component whose purpose could not be derived MUST be escalated, NEVER documented as "unclear" and NEVER proposed for removal.
- Keep to one page. If a minor component genuinely needs more, discovery mis-tiered it — re-raise the tier rather than expanding here.
- NEVER carry layer-3 detail. Exact signatures and settings are not documented for minor components.

## References

- **📖** `.copilot/context/10.00-application-development/01-discovery-model.md` — priority taxonomy and purpose derivation
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
