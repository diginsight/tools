---
description: "Mermaid diagram patterns for design orchestration workflows"
---

# Design Workflow Diagrams

## Prompt Design (8-phase)

```
┌─────────────────────────────────────────────────────────────────┐
│                    PROMPT DESIGN & CREATE                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: Requirements Gathering (prompt-researcher)            │
│     └─► Use case challenge (3-7 scenarios)                      │
│     └─► Tool discovery from scenarios                           │
│     └─► Scope boundary definition                               │
│           │                                                     │
│           ▼ [GATE: Requirements validated?]                     │
│                                                                 │
│  Phase 2: Pattern Research (prompt-researcher)                  │
│     └─► Search context files (NOT internet)                     │
│     └─► Find 3-5 similar prompts                                │
│     └─► Extract proven patterns                                 │
│           │                                                     │
│           ▼ [GATE: Patterns identified?]                        │
│                                                                 │
│  Phase 3: Structure Definition (Orchestrator)                   │
│     └─► Architecture decision (single vs. orchestrator+agents)  │
│     └─► Existing agent inventory                                │
│     └─► New agent identification                                │
│           │                                                     │
│           ▼ [GATE: Architecture decided?]                       │
│                                                                 │
│  Phase 4: File Creation                                         │
│     ├─► [If Single] prompt-builder creates prompt               │
│     └─► [If Orchestrator] Phase 4a + 4b (see below)             │
│           │                                                     │
│           ▼ [GATE: Files created?]                              │
│                                                                 │
│  Phase 4a: Agent Creation (if orchestrator architecture)        │
│     ├─► agent-researcher: Role challenge & research             │
│     ├─► agent-builder: Create agent file                        │
│     └─► agent-validator: Validate agent                         │
│           │ (repeat for each new agent)                         │
│           ▼                                                     │
│  Phase 4b: Orchestrator Creation                                │
│     └─► prompt-builder: Create orchestrator file                │
│           │                                                     │
│           ▼ [GATE: All files created?]                          │
│                                                                 │
│  Phase 5: Agent Updates (if existing agents need changes)       │
│     └─► agent-builder: Modify existing agents                   │
│     └─► agent-validator: Re-validate updated agents             │
│           │                                                     │
│           ▼ [GATE: Dependencies resolved?]                      │
│                                                                 │
│  Phase 6: Prompt Validation (prompt-validator)                  │
│     └─► Tool alignment check                                    │
│     └─► Structure compliance                                    │
│     └─► Quality scoring                                         │
│           │                                                     │
│           ▼ [GATE: Validation passed?]                          │
│                                                                 │
│  Phase 7: Issue Resolution (prompt-builder, if needed)          │
│     └─► Fix identified prompt issues                            │
│     └─► Re-validate                                             │
│           │                                                     │
│           ▼ [GATE: All issues resolved?]                        │
│                                                                 │
│  Phase 8: Final Review & Completion                             │
│     └─► Summary of all created/updated files                    │
│     └─► Usage instructions                                      │
│           │                                                     │
│           ▼ [COMPLETE]                                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Agent Design (8-phase)

```
┌─────────────────────────────────────────────────────────────────┐
│                    AGENT DESIGN & CREATE                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: Requirements Gathering (agent-researcher)             │
│     └─► Use case challenge (3-7 scenarios)                     │
│     └─► Tool discovery from scenarios                          │
│     └─► Scope boundary definition                              │
│           │                                                     │
│           ▼ [GATE: Requirements validated?]                     │
│                                                                 │
│  Phase 2: Pattern Research (agent-researcher)                   │
│     └─► Search context files (NOT internet)                    │
│     └─► Find 3-5 similar agents                                │
│     └─► Extract proven patterns                                │
│           │                                                     │
│           ▼ [GATE: Patterns identified?]                        │
│                                                                 │
│  Phase 3: Structure Definition (agent-researcher)               │
│     └─► Complete specification                                 │
│     └─► Tool alignment verification                            │
│     └─► Three-tier boundaries                                  │
│           │                                                     │
│           ▼ [GATE: Spec complete? Tool count 3-7?]              │
│                                                                 │
│  Phase 4: Agent Creation (agent-builder)                        │
│     └─► Pre-save validation                                    │
│     └─► File creation                                          │
│     └─► Structure verification                                 │
│           │                                                     │
│           ▼ [GATE: File created successfully?]                  │
│                                                                 │
│  Phase 5: Dependency Analysis (agent-researcher)                │
│     └─► Identify dependent agents                              │
│     └─► Check if updates needed                                │
│     └─► Generate update plans                                  │
│           │                                                     │
│           ▼ [GATE: Dependencies resolved?]                      │
│                                                                 │
│  Phase 6: Recursive Agent Creation (if needed)                  │
│     └─► Create dependent agents                                │
│     └─► Update existing agents                                 │
│           │                                                     │
│           ▼ [GATE: All agents ready?]                           │
│                                                                 │
│  Phase 7: Validation (agent-validator)                          │
│     └─► Tool alignment check                                   │
│     └─► Structure compliance                                   │
│     └─► Quality scoring                                        │
│           │                                                     │
│           ▼ [GATE: Validation passed?]                          │
│                                                                 │
│  Phase 8: Issue Resolution (agent-builder, if needed)           │
│     └─► Fix identified issues                                  │
│     └─► Re-validate                                            │
│           │                                                     │
│           ▼ [COMPLETE]                                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Skill Design (6-phase)

```
┌─────────────────────────────────────────────────────────────────┐
│                      SKILL DESIGN & CREATE                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: Requirements Gathering (orchestrator + researcher)    │
│     └─► Scope definition (domain, workflows, platforms)         │
│     └─► Discovery scenario challenge (3-5 prompts)              │
│     └─► Scope boundary definition                               │
│           │                                                     │
│           ▼ [GATE: Requirements validated?]                     │
│                                                                 │
│  Phase 2: Gap & Overlap Research (skill-researcher)             │
│     └─► Scan existing skills for overlaps                       │
│     └─► Identify referenced but missing resources               │
│     └─► Extract proven patterns from existing skills            │
│           │                                                     │
│           ▼ [GATE: No overlaps? Patterns identified?]           │
│                                                                 │
│  Phase 3: Structure Definition (orchestrator)                   │
│     └─► Description formula application                         │
│     └─► Resource architecture (templates, checklists, scripts)  │
│     └─► Progressive disclosure layer mapping                    │
│     └─► Present plan to user for approval                       │
│           │                                                     │
│           ▼ [GATE: User approved?]                              │
│                                                                 │
│  Phase 4: Skill Creation (skill-builder)                        │
│     └─► Create SKILL.md with YAML + required sections           │
│     └─► Create resource files (templates, checklists, etc.)     │
│     └─► Pre-save validation (name, description, paths, budget)  │
│           │                                                     │
│           ▼ [GATE: Files created?]                              │
│                                                                 │
│  Phase 5: Validation (skill-validator)                          │
│     └─► Description quality (formula, ≤1,024 chars)             │
│     └─► Progressive disclosure compliance                       │
│     └─► Resource integrity (all paths resolve)                  │
│     └─► Cross-platform portability                              │
│     └─► Body word count (≤1,500 words)                          │
│     └─► [If issues: hand off to skill-builder → re-validate]   │
│           │                                                     │
│           ▼ [GATE: Validation passed?]                          │
│                                                                 │
│  Phase 6: Final Report                                          │
│     └─► Skill summary with discovery test                       │
│     └─► Resource inventory                                      │
│     └─► Recommendations for consumers                           │
│           │                                                     │
│           ▼ [COMPLETE]                                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Instruction File Design (6-phase)

```
┌─────────────────────────────────────────────────────────────────┐
│                 INSTRUCTION FILE DESIGN & CREATE                │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: Requirements Gathering (orchestrator)                 │
│     └─► Domain and purpose                                      │
│     └─► Target file patterns (applyTo)                          │
│     └─► Key rules to enforce                                    │
│     └─► Context sources (URLs, files, descriptions)             │
│           │                                                     │
│           ▼ [GATE: Requirements complete?]                      │
│                                                                 │
│  Phase 2: Conflict & Coverage Research (instruction-researcher) │
│     └─► Scan all existing instruction files                     │
│     └─► applyTo overlap detection                               │
│     └─► Rule contradiction check                                │
│     └─► Layer boundary audit (rules vs knowledge)               │
│     └─► Coverage gap identification                             │
│           │                                                     │
│           ▼ [GATE: No conflicts? Gaps identified?]              │
│                                                                 │
│  Phase 3: Structure Definition (orchestrator)                   │
│     └─► Final applyTo pattern                                   │
│     └─► Section outline (rules grouping)                        │
│     └─► Context file references to include                      │
│     └─► Present plan to user for approval                       │
│           │                                                     │
│           ▼ [GATE: User approved?]                              │
│                                                                 │
│  Phase 4: File Creation (instruction-builder)                   │
│     └─► Create instruction file with YAML + rules              │
│     └─► Pre-save validation (conflicts, budget, structure)      │
│           │                                                     │
│           ▼ [GATE: File created?]                               │
│                                                                 │
│  Phase 5: Validation (instruction-validator)                    │
│     └─► applyTo pattern integrity                               │
│     └─► No conflicts with existing instructions                 │
│     └─► Rule consistency with context files                     │
│     └─► Token budget (≤1,500)                                   │
│     └─► Reference integrity (all links resolve)                 │
│     └─► [If issues: hand off to builder → re-validate]         │
│           │                                                     │
│           ▼ [GATE: Validation passed?]                          │
│                                                                 │
│  Phase 6: Final Report                                          │
│     └─► File summary with coverage analysis                     │
│     └─► Integration guidance for consumers                      │
│           │                                                     │
│           ▼ [COMPLETE]                                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-design"
    - "context-information-design"
    - "instruction-file-design"
    - "prompt-design"
    - "skill-design"
  changelog: "workflow-design-diagrams.template.changelog.md"
---
-->
