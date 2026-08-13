---
description: "Phase structure for prompt create-update validation workflows"
---

# Prompt Validation Phase Output Templates

**Purpose:** Reusable output format templates for prompt creation/update workflows.

**Referenced by:** `prompt-create-update.prompt.md` and related prompts

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

### From Existing Prompt (if update)
- [What structure exists and will be preserved]

### From Inference
- [What was derived from task type and patterns]

### From Defaults
- [What used template defaults]

### Initial Values
- **Name:** `[prompt-name]`
- **Description:** "[one-sentence]"
- **Goal (initial):** 
  1. [Objective 1]
  2. [Objective 2]
- **Role (initial):** [inferred role]
- **Tools (initial):** [inferred tools]
- **Agent Mode:** [agent/plan/edit/ask]
```

---

## Phase 1: Validation Depth Assessment Output

```markdown
### Validation Depth Assessment

**Complexity Level:** [Simple / Moderate / Complex]
**Use cases to generate:** [3 / 5 / 7]
**Validation approach:** [Reference adaptive-validation-patterns.md for methodology]
```

---

## Phase 1: Goal Challenge Results Output

```markdown
### 4.1 Goal Challenge Results

**Use Cases Generated:** [3/5/7]

[For each use case: scenario, test question, current guidance, gap, tool/scope discoveries, refinement]

**Status:** ✅ Clear and testable → 4.2 | ⚠️ Ambiguities → propose refinements | ❌ Critical gaps → BLOCK
**Refined Goal:** [Updated goal] | **Tools Discovered:** [tool: justification]
**Scope:** IN: [included] | OUT: [excluded]
```

---

## Phase 1: Role Validation Results Output

```markdown
### 4.2 Role Validation Results

**Initial Role:** [from Step 2]
**Authority Test:** [✅/❌ + gap] | **Expertise Test:** [✅/❌ + gap] | **Specificity:** [✅/⚠️]
**Pattern Search:** Found [N] similar roles. Best match: [file path]

**Status:** ✅ Appropriate → 4.3 | ⚠️ Needs refinement | ❌ Mismatch → BLOCK
**Refined Role:** [Updated role with justification]
```

---

## Phase 1: Workflow Validation Results Output

```markdown
### 4.3 Workflow Validation Results

**Initial Workflow:** [list phases]

**Failure Mode Analysis per Phase:**
- Phase [N]: Test: [failure scenario] | Handling: [addressed/not] | Gap: [what's missing] | Fix: [change]

**Pattern Validation:** [N] similar prompts analyzed, gaps vs. proven patterns: [list]
**Refined Workflow:** [Updated phase structure]
**Status:** ✅ Reliable → 4.4 | ⚠️ Minor gaps | ❌ Fundamental issues → BLOCK
```

---

## Phase 1: Tool Requirements Analysis Output

```markdown
### 4.4 Tool Requirements Analysis

**Phase → Tool Mapping:** [phase: capability → tool]
**Tool List:** 1. [tool - justification] 2. [tool - justification] ...
**Tool Count:** [N] | **Status:** [✅ Within 3-7 / ⚠️ Consider decomposition]

**Agent Mode Alignment:**
- Mode: [agent/plan/edit/ask] | Tools: [read-only/read+write] | Alignment: [✅/❌]
- Pattern: [name] | Match: [✅ Proven / ⚠️ Novel]

**Status:** ✅ Validated → 4.5 | ⚠️ High count | ❌ Mismatch → BLOCK
```

---

## Phase 1: Boundary Validation Results Output

```markdown
### 4.5 Boundary Validation Results

**Initial Boundaries:** [list]

**Boundary Testing per boundary:**
- [Tier] - [Text] | Testability: [✅/❌] | Refinement: [specific version] | Actionable: [✅/❌]

**Coverage Check:** Cross-reference against 4.3 failure modes. Missing: [list]

**Refined Boundaries:**
**✅ Always:** [actionable] | **⚠️ Ask:** [conditions] | **🚫 Never:** [prohibitions]

**Status:** ✅ All actionable → Complete Step 4 | ⚠️ Some vague → Propose refinements
```

---

## Phase 1: User Clarification Request Format

```markdown
## Requirements Validation Results

I've analyzed your request and identified some gaps. Please clarify:

### ❌ Critical Issues (Must Resolve Before Proceeding)

**1. [Issue Name]**

**Problem:** [Description of ambiguity or gap]

**Your goal "[original goal]" could mean:**
- **Scenario A:** [Interpretation 1]
  - **Implications:** Tools: [list], Boundaries: [list], Complexity: [level]
- **Scenario B:** [Interpretation 2]
  - **Implications:** Tools: [list], Boundaries: [list], Complexity: [level]
- **Scenario C:** [Interpretation 3]
  - **Implications:** Tools: [list], Boundaries: [list], Complexity: [level]

**Which interpretation is correct?** Or describe your intent differently.

---

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
## Prompt Requirements Analysis - VALIDATED

### Operation
- **Mode:** [Create / Update] | **Target:** `.github/prompts/[prompt-name].prompt.md`
- **Complexity:** [Simple / Moderate / Complex] | **Depth:** [Quick / Standard / Deep]

### YAML Frontmatter (Validated)
- **name:** `[prompt-name]` | **agent:** [agent/plan/edit/ask] | **model:** [model]
- **description:** "[one-sentence]"
- **tools:** [validated list] | **argument-hint:** "[guidance]"

### Content Structure (Validated)

**Role (Validated):**
[Refined role with authority and expertise for goal]

**Goal (Validated through [N] use cases):**
1. [Refined objective 1]
2. [Refined objective 2]
3. [Refined objective 3]

**Scope Boundaries:**
- **IN SCOPE:** [included] | **OUT OF SCOPE:** [excluded]

### Workflow / Boundaries / Tools (Validated)
- Workflow: [refined phases with failure mode handling]
- Boundaries: **✅ Always:** [list] | **⚠️ Ask:** [list] | **🚫 Never:** [list]
- Tools: [list with phase mapping and justification]

### Validation Summary
Use cases: [N] ✅ | Goal: ✅ | Role: ✅ | Workflow: ✅ | Tools: ✅ [N], [pattern] | Boundaries: ✅

**✅ VALIDATION COMPLETE - Proceed to Phase 2? (yes/no)**
```

---

## Phase 2: Best Practices Validation Output

```markdown
## Best Practices Validation

### Repository Guidelines
- [✅/❌] Context engineering principles | Imperative language | 3-7 tools
- [✅/❌] Narrow scope | Template externalization

### Similar Prompts Analyzed
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
- [ ] All required sections present (Role, Goal, Boundaries, Process, Response Management, Test Scenarios)
- [ ] Role specific with authority, goal has 2-3 validated objectives
- [ ] Boundaries: all three tiers, actionable and testable
- [ ] Process phases handle identified failure modes
- [ ] Tool count 3-7, justified by phases, agent mode compatible
- [ ] Imperative language, critical instructions in first 30%, template externalization
- [ ] Response Management + Error Recovery + Embedded Tests (min 5) included
- [ ] Token budget compliant (≤1500 simple / ≤2500 orchestrator)
- [ ] Filename `[name].prompt.md`, bottom metadata, follows similar prompt patterns

**All checks passed? (yes/no)**
```

---

## Prompt Metadata Block Template

```markdown
<!-- 
---
prompt_metadata:
  created: "[ISO timestamp]"
  created_by: "prompt-create-update"
  last_updated: "[ISO timestamp]"
  version: "1.0"
  validation:
    use_cases_tested: [N]
    complexity: "[simple/moderate/complex]"
    token_count: [approximate]
  production_ready:
    response_management: true
    error_recovery: true
    embedded_tests: [N]
    token_budget_compliant: true
    template_externalization: true
  
validations:
  structure:
    status: "validated"
    last_run: "[ISO timestamp]"
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
    - "prompt-create-update"
  changelog: "output-prompt-validation-phases.template.changelog.md"
---
-->
