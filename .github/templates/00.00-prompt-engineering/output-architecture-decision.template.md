---
description: "Architecture decision record format for PE design workflows"
---

# Architecture Decision Output Formats

Output format templates for Phase 3 (Prompt and Agent Structure Definition) of the `prompt-design` orchestrator.

---

## Agent Inventory Analysis Output

```markdown
### Existing Agents Applicable to Task

**Agents found:** [total count]

**Directly applicable:**
- `[agent-name]` - [matches phase X: reason]
- `[agent-name]` - [matches phase Y: reason]

**Potentially extensible:**
- `[agent-name]` - [current: ..., needs: ...]

**Coverage assessment:**
- Phases covered by existing agents: [X%]
- Phases requiring new agents: [Y%]
- Phases requiring orchestrator only: [Z%]

**Reusability score:** [Low/Medium/High]
- Low: Task too unique, agents won't be reused
- Medium: Some agents applicable to similar tasks
- High: Agents solve common patterns, highly reusable
```

---

## New Agent Recommendations Output

```markdown
### Recommended New Agents

**Agent 1: [name]**
- **Purpose:** [reusable capability]
- **Persona:** [specialist role]
- **Tools:** [tool list]
- **Reusability:** [which other tasks could use this]
- **Justification:** [why new agent vs. extending existing]

**Agent 2: [name]**
- [same structure]
```

---

## Architecture Recommendation Output

```markdown
## Phase 3 Complete: Architecture Analysis

### Task Complexity
**Level:** [Low/Medium/High]
**Phases identified:** [count]
- Phase A: [description]
- Phase B: [description]
- Phase C: [description]

**Domains:** [list]
**Tool variation:** [Yes/No - different tools per phase?]

### Agent Inventory
**Existing agents applicable:** [count]
- `[agent-name]` → Phase [X]
- `[agent-name]` → Phase [Y]

**New agents recommended:** [count]
- `[agent-name]` → Phase [Z] (reusable for: ...)

**Coverage:** [X]% existing, [Y]% new, [Z]% orchestrator-only

### Architecture Recommendation

**Recommended approach:** [Single Prompt / Orchestrator + Agents]

**Justification:**
[Explain why this architecture fits based on analysis]

[If Single Prompt:]
**Reason:** Task is focused, no phase separation needed, no applicable agents
**Implementation:** Create single prompt file with all logic
**Template:** `[recommended-template]`

[If Orchestrator + Agents:]
**Reason:** Task has [X] phases, [Y] existing agents applicable, [Z] new reusable agents identified
**Implementation strategy:**
1. Create new agents first: [list]
2. Create orchestrator to coordinate: [existing agents] + [new agents]
**Agent handoffs:** [phase flow diagram]
**Template:** `prompt-orchestrator-template.md`

**Proceed to build phase? (yes/no/modify architecture)**
```

---

## Architecture Decision Framework

Use this table to guide the architecture recommendation:

| Criteria | Single Prompt | Orchestrator + Agents |
|----------|---------------|----------------------|
| **Phases** | 1-2 linear steps | 3+ distinct phases |
| **Domains** | Single domain | Cross-domain |
| **Tools** | Consistent tools | Different tools per phase |
| **Existing agents** | None applicable | 1+ agents reusable |
| **New agents** | None justified | Reusable specialists identified |
| **Complexity** | Low-Medium | Medium-High |
| **Reusability** | Task-specific | Agents solve patterns |

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-design"
  changelog: "output-architecture-decision.template.changelog.md"
---
-->
