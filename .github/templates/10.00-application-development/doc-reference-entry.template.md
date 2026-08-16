---
description: Page format for a reference entry — the precise surface of a type, settings group, persisted entity or key that a caller must match exactly
domain: "application-development"
---

# Reference entry page

**Audience**: agent. Precision over narrative. Layer 3 content — assume the reader already knows why this exists.

```markdown
---
title: "[entry name]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, reference]
description: "[one sentence: what this entry defines]"
source_sets:
  - domain-model
  - options-model
  - persistence-model
  - schema-definitions
---

# [entry name]

## 🎯 What this is

[One to three sentences. Enough to confirm the reader is on the right page.] ^[[area]-nn]

- **Kind**: [type | settings group | persisted entity | key | enumeration]
- **Component**: [component-id]
- **Declared in**: [path#symbol] ^[[area]-nn]

## 📋 Members

| Name | Type | Required | Default | Meaning | Established by |
|---|---|---|---|---|---|
| [name] | [type] | [yes/no] | [value or —] | [what it controls] | ^[[area]-nn] |

## 🔑 Keys and constraints

[Only for persisted entities and settings groups. Omit otherwise.]

| Constraint | Value | Established by |
|---|---|---|
| [partition key / primary key / unique index / range] | [value] | ^[[area]-nn] |

## ⚠️ Usage constraints

[Limits a caller must respect: bounds, cardinality, ordering, immutability.] ^[[area]-nn]

## 🔗 Used by

| Consumer | How | Established by |
|---|---|---|
| [component-id or page] | [read / written / bound at startup] | ^[[area]-nn] |

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]

<!--
verification_stamp:
  generated: "[YYYY-MM-DD]"
  verified: "[YYYY-MM-DD]"
  evidence:
    - dossier: "_evidence/[component-id]/[area].md"
      observed: "[YYYY-MM-DD]"
  gates: "[pass | pass-with-gaps | fail]"
  open_gaps: [n]
-->
```

## Rules

- Every member row MUST carry an anchor. A row without one is an invented member.
- A default value MUST be read from evidence. NEVER infer a default from a type's zero value.
- An unestablished default MUST be written as `—` with a marked gap. NEVER guess.
- NEVER reproduce a secret value, connection string or credential shape here.
- Narrative belongs to Architecture. Link, do not retell.

## References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — tie-breaker 1: precise surfaces belong here
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
