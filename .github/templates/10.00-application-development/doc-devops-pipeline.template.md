---
description: Page format for one build, gate or release pipeline in the DevOps chapter — triggers, stages, gates, artifacts and environment progression
domain: "application-development"
---

# DevOps pipeline page

**Audience**: agent. One page per pipeline definition established by discovery.

```markdown
---
title: "[pipeline name]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, devops]
description: "[one sentence: what this pipeline produces or promotes]"
source_sets:
  - pipeline-definition
  - release-gates
  - deployment-descriptor
---

# [pipeline name]

## 🎯 What it does

[What the pipeline produces or promotes, and for which components.] ^[[area]-nn]

- **Defined in**: [path] ^[[area]-nn]
- **Components**: [component-id, …]

## ⚡ Triggers

| Trigger | Condition | Established by |
|---|---|---|
| [push / pull request / schedule / manual] | [branch, path filter or cron] | ^[[area]-nn] |

## 🔀 Stages

```mermaid
[pipeline — see doc-mermaid-patterns.template.md]
```

| Stage | Does | Runs on | Established by |
|---|---|---|---|
| [name] | [what it performs] | [agent or runner kind] | ^[[area]-nn] |

## 🚦 Gates

| Gate | Blocks | Criterion | Established by |
|---|---|---|---|
| [name] | [which stage] | [what must pass] | ^[[area]-nn] |

## 📦 Artifacts

| Artifact | Produced by | Consumed by | Established by |
|---|---|---|---|
| [name] | [stage] | [stage or environment] | ^[[area]-nn] |

## 🪜 Environment progression

[Which environments it deploys to, in what order, and what promotes between them.] ^[[area]-nn]

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]
<!--
verification_stamp:
  generated: "[YYYY-MM-DD]"
  verified: "[YYYY-MM-DD]"
  evidence:
    - dossier: "_evidence/[component-id]/devops.md"
      observed: "[YYYY-MM-DD]"
  gates: "[pass | pass-with-gaps | fail]"
  open_gaps: [n]
-->
```

## Rules

- A disabled or archived pipeline MUST be documented as disabled, NEVER omitted. Its absence from the docs reads as "there is no pipeline".
- NEVER publish a variable value, a secret name that reveals a service identity, or a runner hostname.
- A gate MUST be evidenced from the definition. NEVER infer a gate from a stage name.
- Run history is evidence of behaviour, not of definition. Attribute it as observation.

## References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — tie-breaker 4
- **📖** `doc-mermaid-patterns.template.md` — pipeline diagram

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
