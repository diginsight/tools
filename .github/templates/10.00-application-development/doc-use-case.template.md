---
description: Page format for a repository use case — an actor, a goal, the observable flow and what proves it works
domain: "application-development"
---

# Use case page

**Audience**: agent. One page per actor-goal pair the repository actually supports.

```markdown
---
title: "[goal, stated as an outcome]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, use-cases]
description: "[one sentence: what an actor accomplishes]"
source_sets:
  - entry-points
  - domain-model
  - test-surface
---

# [goal, stated as an outcome]

## 🎯 Goal

- **Actor**: [who or what initiates] ^[[area]-nn]
- **Outcome**: [what is true afterwards] ^[[area]-nn]
- **Component**: [component-id]

## ✅ Preconditions

| Precondition | Established by |
|---|---|
| [what must already be true] | ^[[area]-nn] |

## 🔬 Flow

1. [observable step] ^[[area]-nn]
2. [observable step] ^[[area]-nn]

```mermaid
[sequence — see doc-mermaid-patterns.template.md; include only when the flow branches]
```

## 🔀 Alternate and failure paths

| Condition | Behaviour | Established by |
|---|---|---|
| [condition] | [what happens instead] | ^[[area]-nn] |

## 🧪 What proves it

[Which tests or observations demonstrate this use case works. Link to Validation.] ^[[area]-nn]

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]

## 🔗 Related

- [[page]]([relative-path]) — [why]

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

- The filename MUST NOT match `p[0-9]-[0-9]{2}-*-usecase.md` and the H1 MUST NOT begin with `UC-`. Those forms belong to prompt-engineering use cases governed by `use-case-documents.instructions.md`, which requires a vision anchor and dimension coverage that a repository-derived use case does not have.
- Every step MUST be **observable**. NEVER document an internal call sequence as a user-visible step.
- A use case with no evidenced actor is not a use case — it is an implementation detail belonging to Architecture.
- NEVER invent a failure path. An unevidenced one is a marked gap.

## References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — tie-breaker 2
- **📖** `.github/templates/10.00-application-development/doc-mermaid-patterns.template.md` — diagram shapes

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
