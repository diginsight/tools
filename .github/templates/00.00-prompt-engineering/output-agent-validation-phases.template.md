---
description: "Phase structure for agent create-update validation workflows"
---

# Agent Validation Phase Output Templates

**Purpose:** Reusable output format templates for agent creation/update workflows.

**Referenced by:** `agent-create-update.prompt.md` and related prompts

---

## Phase 1: Operation Type Output

```markdown
### Operation Type
- **Mode:** [Create / Update]
- **Target:** [New file / Existing file path]
```

---

## Phase 1: Initial Requirements Extraction Output

```markdown
## Initial Requirements Extraction

### From User Input
- [What was explicitly provided]

### From Existing Agent (if update)
- [What structure exists and will be preserved]

### From Inference
- [What was derived from role type and patterns]

### From Defaults
- [What used agent engineering best practices]

### Initial Values
- **Name:** `[agent-name]`
- **Description:** "[one-sentence]"
- **Role (initial):** [inferred role]
- **Responsibilities (initial):** [key tasks]
- **Tools (initial):** [inferred tools]
- **Agent Mode:** [agent/plan]
```

---

## Phase 1: Validation Depth Assessment Output

```markdown
### Validation Depth Assessment

**Complexity Level:** [Simple / Moderate / Complex]

**Indicators:**
- Role pattern: [Standard / Domain-specific / Novel]
- Tool selection: [Obvious / Needs discovery / Unclear]
- Handoffs: [None / Simple / Complex workflow]
- Agent mode: [Clear / Needs validation / Unclear]

**Validation Strategy:**
- **Use cases to generate:** [3 / 5 / 7]
- **Role validation:** [Basic check / Full specialization test / Multi-faceted analysis]
- **Tool composition:** [Pattern match / Discovery + validation / Full composition analysis]
```

---

## Phase 1: Use Case Challenge Template

```markdown
**Scenario [N]:** [Realistic situation this agent should handle]
**Test Question:** [Can this role handle this scenario?]
**Current Role Capability:** [What does current role imply?]
**Gap Identified:** [What's missing or unclear]
**Tool Requirement Discovered:** [If scenario reveals need for specific tool]
**Responsibility Boundary Discovered:** [If scenario reveals in/out-of-scope question]
**Handoff Discovered:** [If scenario requires delegation to another agent]
**Refinement Needed:** [Specific change to role/responsibilities]
```

---

## Phase 1: Role Challenge Results Output

```markdown
### 4.1 Role Challenge Results

**Use Cases Generated:** [3/5/7]
[For each use case: scenario, test, gaps, discoveries]

**Validation Status:**
- ✅ Appropriately specialized → Proceed to Step 4.2
- ⚠️ Needs clarification → Propose refinements, ask user
- ❌ Too broad/narrow → BLOCK, ask user

**If ⚠️ or ❌:** Present Questions (Critical Issues / High Priority / Suggestions)

**Refined Role:** [Updated role] | **Responsibilities:** [Updated list]
**Tools Discovered:** [tool: justification] | **Handoffs:** [agent: when]
**Scope:** IN: [handles] | OUT: [excluded]
```

---

## Phase 1: Tool Composition Validation Output

```markdown
### 4.2 Tool Composition Validation

**Responsibilities → Tool Mapping:** [responsibility: capability → tool]
**Tool List:** 1. [tool - justification] 2. [tool - justification] ...
**Tool Count:** [N] | **Status:** [✅ Within 3-7 / ⚠️ Too few / ❌ Too many]

**Agent Mode Alignment:**
- Mode: [agent/plan] | Tools: [read-only/read+write] | Alignment: [✅/❌]
- Pattern: [name] | Match: [✅ Proven / ⚠️ Novel - justify]

**Tool Conflict Check:** [analysis per overlap]
**Handoff Validation:** [agents, existence check, dependency chain]

**Validation Status:**
- ✅ Validated → Proceed to 4.3 | ⚠️ Tool count issues | ❌ Mismatch → BLOCK
```

---

## Phase 1: Boundary Validation Results Output

```markdown
### 4.3 Boundary Validation Results

**Initial Boundaries:**
[List initial Always/Ask/Never boundaries]

**Boundary Testing:**

**[Tier] - [Boundary Text]**
- **Testability:** [✅ AI can determine / ❌ Subjective/vague]
- **Refinement:** [Specific, testable version]
- **Actionable:** [✅ Yes / ❌ Still vague]
- **Criticality:** [For agent mode constraints]

[Repeat for each boundary]

**Coverage Check:** Cross-reference against failure modes from 4.1. Missing boundaries: [list]
**Agent Mode Constraints:** [plan vs. agent specific boundaries]

**Refined Boundaries:**
**✅ Always:** [actionable requirements] | **⚠️ Ask:** [conditions] | **🚫 Never:** [prohibitions]

**Status:** ✅ All actionable → Complete Step 4 | ⚠️ Vague → Propose refinements
```

---

## Phase 1: User Clarification Request Format

```markdown
## Agent Requirements Validation Results

I've analyzed your request and identified some gaps. Please clarify:

### ❌ Critical Issues (Must Resolve Before Proceeding)
**1. [Issue Name]**
**Problem:** [Description]
**Interpretations:** A: [option + implications] | B: [option + implications]
**Which interpretation is correct?**

### ⚠️ High Priority Questions
**2. [Question]**
**Options:** A: [choice → impact] | B: [choice → impact]
**Recommendation:** [If any]

### 📋 Suggestions (Optional)
**3. [Suggestion]** — Current: [X] → Improvement: [Y] → Benefit: [Z]

**Please answer Critical and High Priority questions before I proceed.**
```

---

## Phase 1: Final Requirements Summary Output

```markdown
## Agent Requirements Analysis - VALIDATED

### Operation
- **Mode:** [Create / Update] | **Target:** `.github/agents/[agent-name].agent.md`
- **Complexity:** [Simple / Moderate / Complex] | **Depth:** [Quick / Standard / Deep]

### YAML Frontmatter (Validated)
- **name:** `[agent-name]` | **agent:** [agent/plan] | **model:** [model]
- **description:** "[one-sentence]"
- **tools:** [validated list] | **handoffs:** [if applicable]

### Agent Persona (Validated through [N] use cases)
- **Role:** [Refined role] | **Expertise:** [Areas]
- **Responsibilities:** 1. [R1] 2. [R2] 3. [R3]
- **IN SCOPE:** [What handles] | **OUT OF SCOPE:** [What excluded]

### Tools (Validated)
**Pattern:** [name] | **Count:** [N] | **Alignment:** ✅ [mode + tools compatible]
[List each tool with justification]

### Boundaries (All Actionable)
**✅ Always:** [list] | **⚠️ Ask:** [list] | **🚫 Never:** [list]

### Validation Summary
Use cases: [N] ✅ | Role: ✅ | Tools: ✅ [N], [pattern] | Alignment: ✅ | Boundaries: ✅ | Handoffs: [status]

**✅ VALIDATION COMPLETE - Proceed to Phase 2? (yes/no)**
```

---

## Phase 2: Best Practices Validation Output

```markdown
## Best Practices Validation

### Repository Guidelines
- [✅/❌] Context engineering principles | Imperative language | 3-7 tools
- [✅/❌] Role specialized | Agent/tool alignment | Template externalization

### Similar Agents Analyzed
1. **[file-path]** - [Key patterns]
2. **[file-path]** - [Key patterns]

### Patterns to Apply / Anti-Patterns Avoided
- [Pattern or anti-pattern with status]

**Proceed to Phase 3? (yes/no)**
```

---

## Phase 4: Pre-Output Validation Checklist

```markdown
## Pre-Output Validation

- [ ] YAML frontmatter valid and complete
- [ ] All required sections present (Role, Expertise, Responsibilities, Boundaries, Process)
- [ ] Role specialized (not generic), expertise specific, responsibilities actionable
- [ ] Boundaries: all three tiers with actionable, testable rules
- [ ] Tool count 3-7, agent mode matches tools (plan→read-only, agent→read+write)
- [ ] No tool conflicts, follows proven composition pattern
- [ ] Imperative language, critical instructions early, template externalization
- [ ] Filename `[name].agent.md`, bottom metadata block included
- [ ] Follows patterns from similar agents

**All checks passed? (yes/no)**
```

---

## Agent Metadata Block Template

```markdown
<!-- 
---
agent_metadata:
  created: "[ISO timestamp]"
  created_by: "agent-create-update"
  last_updated: "[ISO timestamp]"
  version: "1.0"
  validation:
    use_cases_tested: [N]
    complexity: "[simple/moderate/complex]"
    tool_count: [N]
  production_ready:
    template_externalization: true
  
validations:
  structure:
    status: "validated"
    last_run: "[ISO timestamp]"
  agent_tool_alignment:
    status: "verified"
    mode: "[plan/agent]"
    tools_compatible: true
---
-->
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-create-update"
  changelog: "output-agent-validation-phases.template.changelog.md"
---
-->
