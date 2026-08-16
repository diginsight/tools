---
description: Approved Mermaid diagram shapes for generated documentation pages — system context, container topology, sequence, state and deployment
domain: "application-development"
---

# Mermaid diagram patterns

**Audience**: agent. Use ONLY these shapes. Every element MUST trace to a dossier record.

## When a diagram is warranted

| Warranted | Not warranted |
|---|---|
| more than three interacting parts | two parts — use prose |
| a sequence with branches | a linear two-step flow |
| a topology or a state machine | a list that happens to be ordered |

## System context — Architecture

```mermaid
flowchart LR
    Actor([**[actor]**]) --> Sys[**[system]**]
    Sys --> Ext[(**[external dependency]**)]
```

## Container topology — Architecture

```mermaid
flowchart TB
    subgraph Boundary["**[deployment boundary]**"]
        A["**[component-id]**<br/>[role]"]
        B["**[component-id]**<br/>[role]"]
    end
    A -->|"[protocol]"| B
    B --> Store[("**[store]**")]
```

## Sequence — Use Cases

```mermaid
sequenceDiagram
    participant A as [actor]
    participant S as [component-id]
    A->>S: [request]
    S-->>A: [response]
    alt [condition]
        S->>S: [alternate path]
    end
```

## State — Architecture, Reference

```mermaid
stateDiagram-v2
    [*] --> [state]
    [state] --> [state]: [trigger]
    [state] --> [*]
```

## Deployment — Infrastructure


```mermaid
flowchart TB
    subgraph Env["**[environment]**"]
        R1["**[resource]**<br/>[sku or tier]"]
        R2["**[resource]**"]
    end
    R1 --> R2
```

## Pipeline — DevOps

```mermaid
flowchart LR
    T["[trigger]"] --> B["[build stage]"] --> G{"[gate]"} --> D["[deploy stage]"]
    G -->|fail| X([stop])
```

## Rules

- A diagram MUST NEVER be the only carrier of a fact — accompany it with prose.
- A diagram MUST NEVER introduce a component, boundary or flow no dossier record establishes.
- Node labels MUST use the component `id` from the registry, not an invented name.
- NEVER use a real external product, customer or environment name from outside the documented repository.
## References

- **📖** `.copilot/context/10.00-application-development/07-documentation-authoring-criteria.md` — diagram policy
- **📖** `.copilot/context/10.00-application-development/01-discovery-model.md` — component ids

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
