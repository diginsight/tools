---
title: "Reference — PE Workflow Entry Phases A–E"
description: "Phase A (Orchestrator), B (Standalone), C (Validation), D (Direct Agent), and E (Meta-Agents and Meta-Prompts) tables lifted from 05.03-pe-workflow-entry-points.md. Reference tables — load only when the Quick Decision tree directs you to a specific phase."
domain: "prompt-engineering"
parent_artifact: ".copilot/context/00.00-prompt-engineering/05.03-pe-workflow-entry-points.md"
goal: "Provide the five per-phase workflow tables as a load-on-demand reference, keeping the parent entry-point guide under the context token budget."
scope:
  covers:
    - "Phase A: Orchestrator Prompts (5 prompts) + supporting narrative"
    - "Phase B: Standalone Prompts (5 prompts) + Standalone-vs-Orchestrator decision matrix"
    - "Phase C: Validation Prompts (5 prompts) + validation workflow"
    - "Phase D: Direct Agent Usage (agent list)"
    - "Phase E: Meta-Agents and Meta-Prompts (system self-improvement)"
  excludes:
    - "Quick Decision tree (lives in parent — navigational core)"
    - "Consolidated Prompts table (lives in parent — recommended default)"
    - "Common Scenarios mapping (lives in parent)"
    - "Built-in VS Code `/create-*` commands (live in parent)"
boundaries:
  - "MUST stay byte-identical to the Phase A–E content in the parent entry-point guide at extraction time"
  - "MUST be updated atomically with the parent when phase tables change"
rationales:
  - "Phase tables are reference data — most consumers hit the Quick Decision tree first and then drop into ONE phase, not all five"
  - "Extraction reduces the parent guide by ~500 tokens, bringing it under the 2,500-token hard ceiling"
  - "Load-on-demand via 📖 reference preserves accessibility without paying the inline cost on every context load"
---

# Reference — PE Workflow Entry Phases A–E

**Parent artifact:** [05.03-pe-workflow-entry-points.md](../../../.copilot/context/00.00-prompt-engineering/05.03-pe-workflow-entry-points.md)

This reference holds the five per-phase tables (A–E). Start at the parent's Quick Decision tree to pick a phase, then return here for the prompt/agent listings, decision matrices, and workflow diagrams of that phase.

---

## Phase A: Orchestrator Prompts (Complex Creation)

**Use when**: Requirements are unclear, scope is uncertain, or you want guided multi-phase creation with quality gates.

| Prompt | Slash Command | Creates | Agent Team | When to Use |
|---|---|---|---|---|
| `prompt-design` | `/pe-gra-prompt-design` | Prompt files + dependent agents | 8 agents (prompt + agent specialists) | New prompt with uncertain requirements, novel workflow, or when dependent agents may be needed |
| `agent-design` | `/pe-gra-agent-design` | Agent files | 4 agents (agent specialists) | New agent with role that needs challenge-testing, unclear tools, or possible decomposition |
| `context-information-design` | `/pe-gra-context-information-design` | Context files | 3 agents (context specialists) | New domain context with uncertain scope, possible overlap, or multi-file splitting needed |
| `instruction-file-design` | `/pe-gra-instruction-file-design` | Instruction files | 3 agents (instruction specialists) | New instruction file with unclear applyTo, possible conflicts |
| `skill-design` | `/pe-gra-skill-design` | Skills | 3 agents (skill specialists) | New skill with uncertain scope, multiple workflows |

**Coverage note:** Orchestrator prompts (design + review) exist for agents, prompts, context files, instruction files, skills, and templates. Hooks and prompt-snippets intentionally have only create-update prompts — they're simple artifact types where full design/review orchestration adds overhead without value. This asymmetry is by design, not a gap.

**What orchestrators provide that standalone prompts do not:**
- Use case challenge validation (3–7 scenarios)
- Pattern research across existing codebase
- Quality gates between phases
- Automatic validation handoff
- Recursive creation of dependent agents

**Orchestrator workflow:**
```
User request → Researcher (requirements + patterns) → [User approval gate]
    → Builder (file creation) → Validator (quality check)
    → Updater (if issues) → Validator (re-check) → Done
```

---

## Phase B: Standalone Prompts (Direct Creation/Update)

**Use when**: Requirements are clear, you know the pattern, or you want a faster single-agent workflow.

| Prompt | Slash Command | Creates/Updates | When to Use |
|---|---|---|---|
| `prompt-create-update` | `/pe-gra-prompt-create-update` | Prompt files | Clear requirements, known template, quick creation or update |
| `agent-create-update` | `/pe-gra-agent-create-update` | Agent files | Clear role, known tools, quick creation or update |
| `context-information-create-update` | `/pe-gra-context-information-create-update` | Context files | New knowledge to document, source material available |
| `instruction-file-create-update` | `/pe-gra-instruction-file-create-update` | Instruction files | New file-type rules, known applyTo patterns |
| `skill-create-update` | `/pe-gra-skill-create-update` | Skills | Clear scope, known workflows, quick skill creation or update |

**Standalone vs Orchestrator — Decision Matrix:**

| Factor | → Standalone | → Orchestrator |
|---|---|---|
| Requirements clarity | Clear, specific | Vague, exploratory |
| Pattern familiarity | Known pattern, existing template | Novel workflow, no precedent |
| Scope confidence | Well-scoped, single artifact | May need decomposition |
| Tool requirements | Known tools, ≤7 | Unclear tools, possibly >7 |
| Time priority | Fast turnaround needed | Quality over speed |
| Dependent artifacts | None needed | May need supporting agents |

---

## Phase C: Validation Prompts (Review/Validate)

**Use when**: You have an existing artifact that needs quality verification.

| Prompt | Slash Command | Validates | Agent Team | When to Use |
|---|---|---|---|---|
| `prompt-review` | `/pe-gra-prompt-review` | Prompt files | `prompt-validator` ↔ `prompt-builder` | After creation, after updates, periodic review |
| `agent-review` | `/pe-gra-agent-review` | Agent files | `agent-validator` ↔ `agent-builder` | After creation, after updates, periodic review |
| `skill-review` | `/pe-gra-skill-review` | Skills | `skill-validator` ↔ `skill-builder` | After creation, after updates, periodic review |
| `instruction-file-review` | `/pe-gra-instruction-file-review` | Instruction files | `instruction-validator` ↔ `instruction-builder` | After creation, after updates, periodic review |
| `context-information-review` | `/pe-gra-context-information-review` | Context files | `context-validator` ↔ `context-builder` | After creation, after updates, periodic review |

**Validation workflow:**
```
Artifact → Tool alignment check (CRITICAL) → Structure check
    → Boundary check → Quality score → [If issues: builder → re-validate]
```

**Validation is mandatory after:**
- Any CRITICAL or HIGH impact change
- New artifact creation (usually auto-triggered via handoff)
- Changes to artifacts with 3+ dependents

---

## Phase D: Direct Agent Usage (Expert Mode)

**Use when**: You know exactly which agent to invoke and want to skip the orchestrator/prompt overhead.

| Agent | Best For |
|---|---|
| `@pe-con-researcher` | Research requirements for any artifact type |
| `@pe-con-builder` | Build/update any artifact type from spec |
| `@pe-con-validator` | Quick validation of any single file |
| `@pe-gra-prompt-researcher` | Quick requirements check, pattern search (prompt-specific) |
| `@pe-gra-prompt-builder` | Build from spec, apply fixes (prompt-specific) |
| `@pe-gra-prompt-validator` | Quick validation of a single prompt file |
| `@pe-gra-agent-researcher` / `@pe-gra-agent-builder` / `@pe-gra-agent-validator` | Same pattern for agents |
| `@pe-gra-context-builder` | Create/update context files |
| `@pe-gra-instruction-builder` | Create/update instruction files |
| `@pe-meta-validator` | Audit PE system (Ecosystem Audit mode) |
| `@pe-meta-optimizer` | Apply optimizations from audit report |

**When to use agents directly instead of prompts:**
- You have a clear, single-step task
- You already have the research report / specification
- You want to skip orchestration overhead
- You are an experienced user who knows the agent capabilities

---

## Phase E: Meta-Agents and Meta-Prompts (System Self-Improvement)

**Use when**: You want to review, optimize, or update the PE artifact system itself.

### Available Meta-Agents

| Agent | Purpose | Mode | Use Directly |
|---|---|---|---|
| `@meta-validator` | Audit all PE artifacts for coherence, redundancy, completeness (Ecosystem Audit mode) | Read-only (`plan`) | `@meta-validator` — produces audit report |
| `@meta-optimizer` | Apply deduplication, token savings, structural improvements | Read-write (`agent`) | `@meta-optimizer <audit-report>` — applies fixes |

**Meta-agent workflow:**
```
@meta-validator (Ecosystem Audit) → audit report → @meta-optimizer → changes → @prompt-validator → re-validation
```

### Meta-Prompts (Slash Commands)

| Prompt | Purpose | Mode |
|---|---|---|
| `/pe-meta-review` | Incorporate new guidance (apply-mode default) | Read-write |
| `/pe-meta-review --mode plan --skip research` | System review by scope+dimension | Read-only |
| `/pe-meta-review --mode apply --dim optimize --skip research,structure,consistency` | Apply optimizations | Read-write |
| `/pe-meta-scheduled-review` | Weekly auto-detect stale areas | Read-write |
| `/pe-meta-review --source <url>` | Reconcile against a VS Code/Copilot release | Read-write |
| `/pe-meta-design` | Design PE-for-PE artifact with vision alignment | Read-write |
| `/pe-meta-create-update` | Create/update PE-for-PE artifact with strategic guards | Read-write |
| `/pe-meta-review` | Review PE-for-PE artifact against vision + ecosystem | Read-only |

**Flags**: `--no-external` (local only), `--no-research` (skip researcher, requires file scope), `--incremental` (only changed files)

**Plan-mode parameters**: `--scope` (`all`/`context`/`agents`/...) + `--dim` (`coherence`/`structure`/`rules`/`references`/`budgets`)

### Scheduled Review Patterns

Three complementary mechanisms cover different triggers:

| Mechanism | Trigger | Scope | LLM needed |
|---|---|---|---|
| `pe-staleness-check` hook | Every session start | All context files | No (shell script) |
| `/pe-meta-scheduled-review` | Weekly (user-initiated) | Auto-detected stale areas | Yes |
| `/pe-meta-review --source <url>` | New VS Code/Copilot release | Affected artifact types | Yes |

**Audit trail**: All reviews are recorded in [05.04-meta-review-log.md](../../../.copilot/context/00.00-prompt-engineering/05.04-meta-review-log.md).

---

**Parent artifact:** [05.03-pe-workflow-entry-points.md](../../../.copilot/context/00.00-prompt-engineering/05.03-pe-workflow-entry-points.md)

<!--
template_metadata:
  version: "1.0.0"
  last_updated: "2026-05-25"
-->
