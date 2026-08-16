---
description: Page format for a system and logical architecture page — context, structure, flows, decisions and physical placement
domain: "application-development"
---

# Architecture page

**Audience**: agent. One page per component in multi-component mode; one page total in single-component mode.

```markdown
---
title: "[component-id] — architecture"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, architecture]
description: "[one sentence: what this component is and how it is structured]"
source_sets: [composition-root, entry-points, domain-model, infrastructure-definition]
---

# [component-id] — architecture

## 🎯 Purpose and context

[Layer 1. What the component exists for, derived from evidence — not from its name.] ^[[area]-nn]

```mermaid
[system context — see doc-mermaid-patterns.template.md]
```

## 🧱 Structure

[Layer 2. The parts and their responsibilities. One row per part.]

| Part | Responsibility | Established by |
|---|---|---|
| [name] | [what it owns] | ^[[area]-nn] |

## 🔀 Key flows

```mermaid
[sequence — see doc-mermaid-patterns.template.md; include only when the flow branches]
```

## 🔗 Dependencies

| Depends on | Direction | Protocol | Established by |
|---|---|---|---|
| [component-id or external role] | [inbound/outbound] | [protocol] | ^[[area]-nn] |

## 🧭 Design decisions

[Layer 3. Only decisions the evidence establishes. One inferred from shape alone is a claim — attribute it or omit it.]

| Decision | Evidenced by | Consequence |
|---|---|---|
| [decision] | ^[[area]-nn] | [what it constrains] |

## 🏗️ Physical placement

[Where it runs. Link to Infrastructure rather than restating environment detail.] ^[[area]-nn]

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

- A table of contents is required once the page exceeds 500 words.
- Purpose MUST come from derived purpose in the registry, NEVER from the folder name.
- A diagram MUST NOT show a part, boundary or arrow that no record establishes.
- Exact signatures, keys and settings belong to Reference. Link, do not inline.
- NEVER name an external product, customer or environment from outside this repository.

## References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — placement tie-breakers
- **📖** `doc-mermaid-patterns.template.md` — diagram shapes
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
