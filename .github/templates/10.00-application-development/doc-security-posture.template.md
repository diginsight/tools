---
description: Page format for the security posture page — trust boundaries, identity, data protection, secret handling and known gaps, stated without exploit detail
domain: "application-development"
---

# Security posture page

**Audience**: agent. One page per component, or one total in single-component mode.

```markdown
---
title: "[component-id] — security posture"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, security]
description: "[one sentence: how this component is defended]"
source_sets:
  - authn-authz-surface
  - transport-and-crypto
  - secret-references
---

# [component-id] — security posture

## 🎯 What is protected

[What would matter if it were lost, altered or disclosed.] ^[[area]-nn]

## 🧱 Trust boundaries

| Boundary | Untrusted side | Crossing mechanism | Established by |
|---|---|---|---|
| [boundary] | [who or what] | [how input crosses] | ^[[area]-nn] |

## 🪪 Identity and authorisation

| Concern | Observed | Established by |
|---|---|---|
| Caller identity | [how callers are identified] | ^[[area]-nn] |
| Authorisation model | [how permission is decided] | ^[[area]-nn] |
| Service identity | [how the component authenticates outbound] | ^[[area]-nn] |

## 🔐 Data protection

| Concern | Observed | Established by |
|---|---|---|
| In transit | [mechanism] | ^[[area]-nn] |
| At rest | [mechanism] | ^[[area]-nn] |
| Personal data | [whether any is handled, and how] | ^[[area]-nn] |

## 🗝️ Secret handling

[How secrets are referenced rather than stored.] ^[[area]-nn]

## ⚠️ Observed gaps

| Gap | Class | Severity | Detail |
|---|---|---|---|
| [what is missing, stated as absence] | [invariant class] | [🔴/🟠/🟡] | **[internal]** `security.internal.md#[id]` |

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

- A gap MUST be stated as an **absence** ("no authorisation check was found on this surface"), NEVER as a method of attack.
- Exploit-actionable detail MUST go to `security.internal.md`. The published page cites the stub only.
- NEVER publish a secret, a key prefix, an internal endpoint or a version string that pins a known vulnerability.
- Absence of evidence MUST NEVER be written as presence of a control. "No encryption setting was found" is not "unencrypted".
- Findings raised here MUST also appear in the robustness stream's plan. This page reports posture; it does not schedule work.

## References

- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — sensitive classification
- **📖** `.copilot/context/10.00-application-development/09-hardening-invariant-catalog.md` — invariant classes

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
  consumers: ["ad-documentation-author", "01.02-ad-docs-write"]
  changes: ["v1.0.0: Initial creation"]
---
-->
