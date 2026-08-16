---
description: Page format for one API unit in the Reference chapter — a callable surface with its operations, contracts, authorisation and failure modes
domain: "application-development"
---

# API unit page

**Audience**: agent. One page per callable surface — an endpoint group, a message contract, a scheduled trigger or a command-line surface.

```markdown
---
title: "[api unit name]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, reference]
description: "[one sentence: what this surface lets a caller do]"
source_sets:
  - entry-points
  - authn-authz-surface
  - domain-model
---

# [api unit name]

## 🎯 What this surface is

- **Kind**: [http endpoint group | message contract | scheduled trigger | cli surface]
- **Component**: [component-id]
- **Declared in**: [path#symbol] ^[[area]-nn]

## 📋 Operations

| Operation | Invocation | Input | Output | Established by |
|---|---|---|---|---|
| [name] | [verb + route, message name, schedule, or command] | [type or shape] | [type or shape] | ^[[area]-nn] |

## 🔐 Authorisation

| Operation | Required identity | Required permission | Established by |
|---|---|---|---|
| [name] | [caller kind] | [permission] | ^[[area]-nn] |

[No authorisation evidence → a marked gap, NEVER "anonymous".]

## ⚠️ Failure modes

| Condition | Response | Established by |
|---|---|---|
| [condition] | [status, fault or behaviour] | ^[[area]-nn] |

## 📏 Limits

| Limit | Value | Established by |
|---|---|---|
| [batch size / payload size / rate / timeout] | [value] | ^[[area]-nn] |

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]

## 🔗 Related

- [[reference entry]]([relative-path]) — [the types this surface exchanges]
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

- An operation MUST be evidenced from the declaration, not from a route naming convention.
- Absence of authorisation evidence MUST be a marked gap. Recording it as "anonymous" asserts a security fact nobody established.
- NEVER publish a live host, a token, or a working example that includes credentials.
- Types belong to the reference entry page. Link, do not restate the member table.

## References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — tie-breaker 1
- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — what must not be published

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
