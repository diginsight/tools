---
description: Page format for one environment in the Infrastructure chapter — provisioned resources, topology, configuration surface and provenance
domain: "application-development"
---

# Infrastructure environment page

**Audience**: agent. One page per environment established by discovery.

```markdown
---
title: "[environment name] environment"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, infrastructure]
description: "[one sentence: what runs here and what it is for]"
source_sets:
  - infrastructure-definition
  - deployment-descriptor
  - settings-sources
---

# [environment name] environment

## 🎯 Purpose

[What this environment is for and who uses it.] ^[[area]-nn]

## 🧱 Provisioned resources

| Resource | Kind | Tier or size | Hosts | Established by |
|---|---|---|---|---|
| [logical name] | [resource kind] | [tier] | [component-id] | ^[[area]-nn] |

## 🗺️ Topology

```mermaid
[deployment — see doc-mermaid-patterns.template.md]
```

## ⚙️ Configuration surface

[Which settings differ here. Values ONLY where non-sensitive; otherwise name the setting and link the Reference entry.] ^[[area]-nn]

| Setting | Value here | Established by |
|---|---|---|
| [setting name] | [value or **[internal]**] | ^[[area]-nn] |

## 🔗 Connections

| From | To | Protocol | Established by |
|---|---|---|---|
| [component-id] | [resource or external role] | [protocol] | ^[[area]-nn] |

## 🧾 Provenance

| Field | Value |
|---|---|
| Observed from | [declaration | live observation] |
| Environment | [environment name] |
| Observed | [YYYY-MM-DD] |

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]
<!--
verification_stamp:
  generated: "[YYYY-MM-DD]"
  verified: "[YYYY-MM-DD]"
  evidence:
    - dossier: "_evidence/[component-id]/environment.md"
      observed: "[YYYY-MM-DD]"
  gates: "[pass | pass-with-gaps | fail]"
  open_gaps: [n]
-->
```

## Rules

- NEVER publish an internal hostname, private endpoint, management URL, tenant or subscription identifier. Use a logical name and route the identifier to the internal dossier.
- NEVER publish a connection string, key or certificate material.
- Environment names MUST be the names this repository uses. NEVER import a naming scheme from elsewhere.
- A resource observed live MUST carry provenance. Without it, it is `claimed` and not publishable as fact.

## References

- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — sensitive classification, provenance
- **📖** `doc-mermaid-patterns.template.md` — deployment diagram

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
