---
description: "Mermaid diagram patterns for review validation workflows"
---

# Review Workflow Diagrams

## Agent Review (5-phase)

```
┌─────────────────────────────────────────────────────────────────┐
│                    AGENT REVIEW & VALIDATE                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: Scope Determination                                   │
│     └─► Single agent or batch?                                 │
│     └─► Full validation or quick check?                        │
│           │                                                     │
│           ▼                                                     │
│                                                                 │
│  Phase 2: Tool Alignment Check (CRITICAL)                       │
│     └─► Verify plan mode = read-only tools                     │
│     └─► Verify agent mode = appropriate tools                  │
│     └─► Check tool count (3-7)                                 │
│           │                                                     │
│           ▼ [GATE: Alignment valid?]                            │
│                                                                 │
│  Phase 3: Full Validation (agent-validator)                     │
│     └─► Structure compliance                                   │
│     └─► Boundary completeness                                  │
│     └─► Convention compliance                                  │
│     └─► Quality assessment                                     │
│           │                                                     │
│           ▼ [GATE: Validation passed?]                          │
│                                                                 │
│  Phase 4: Issue Resolution (agent-builder, if needed)           │
│     └─► Categorize issues by severity                          │
│     └─► Apply fixes                                            │
│     └─► Return to Phase 2/3 for re-validation                  │
│           │                                                     │
│           ▼ [Loop until passed or blocked]                      │
│                                                                 │
│  Phase 5: Final Report                                          │
│     └─► Comprehensive validation summary                       │
│     └─► Quality scores                                         │
│     └─► Recommendations                                        │
│           │                                                     │
│           ▼ [COMPLETE]                                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Prompt Review (5-phase)

```
┌─────────────────────────────────────────────────────────────────┐
│                    PROMPT REVIEW & VALIDATE                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: Scope Determination                                   │
│     └─► Single prompt or batch?                                │
│     └─► Full validation or quick check?                        │
│           │                                                     │
│           ▼                                                     │
│                                                                 │
│  Phase 2: Tool Alignment Check (CRITICAL)                       │
│     └─► Verify plan mode = read-only tools                     │
│     └─► Verify agent mode = appropriate tools                  │
│           │                                                     │
│           ▼ [GATE: Alignment valid?]                            │
│                                                                 │
│  Phase 3: Full Validation (prompt-validator)                    │
│     └─► Structure compliance                                   │
│     └─► Boundary completeness                                  │
│     └─► Convention compliance                                  │
│     └─► Quality assessment                                     │
│           │                                                     │
│           ▼ [GATE: Validation passed?]                          │
│                                                                 │
│  Phase 4: Issue Resolution (prompt-builder, if needed)          │
│     └─► Categorize issues by severity                          │
│     └─► Apply fixes                                            │
│     └─► Return to Phase 2/3 for re-validation                  │
│           │                                                     │
│           ▼ [Loop until passed or blocked]                      │
│                                                                 │
│  Phase 5: Final Report                                          │
│     └─► Comprehensive validation summary                       │
│     └─► Quality scores                                         │
│     └─► Recommendations                                        │
│           │                                                     │
│           ▼ [COMPLETE]                                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Skill Review (5-phase)

```
┌─────────────────────────────────────────────────────────────────┐
│                    SKILL REVIEW & VALIDATE                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: Scope Determination                                   │
│     └─► Single skill or layer audit?                            │
│     └─► Full validation or quick check?                         │
│           │                                                     │
│           ▼                                                     │
│                                                                 │
│  Phase 2: Description & Discovery Check (CRITICAL)              │
│     └─► Formula compliance (what + tech + "Use when")           │
│     └─► Character limit (≤1,024)                                │
│     └─► Discovery scenario test                                 │
│           │                                                     │
│           ▼ [GATE: Description valid?]                          │
│                                                                 │
│  Phase 3: Full Validation (skill-validator)                     │
│     └─► Progressive disclosure compliance                       │
│     └─► Required sections (Purpose, When to Use, Workflow)      │
│     └─► Resource integrity (all paths resolve)                  │
│     └─► Cross-platform portability                              │
│     └─► Body word count (≤1,500)                                │
│     └─► Scope overlap check (layer audit only)                  │
│           │                                                     │
│           ▼ [GATE: Validation passed?]                          │
│                                                                 │
│  Phase 4: Issue Resolution (skill-builder, if needed)           │
│     └─► Categorize issues by severity                           │
│     └─► Apply fixes                                             │
│     └─► Return to Phase 2/3 for re-validation                  │
│           │                                                     │
│           ▼ [Loop until passed or blocked]                      │
│                                                                 │
│  Phase 5: Final Report                                          │
│     └─► Validation summary with scores                          │
│     └─► Resource inventory                                      │
│     └─► Recommendations                                         │
│           │                                                     │
│           ▼ [COMPLETE]                                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Instruction File Review (5-phase)

```
┌─────────────────────────────────────────────────────────────────┐
│               INSTRUCTION FILE REVIEW & VALIDATE                │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: Scope Determination                                   │
│     └─► Single file or layer audit?                             │
│     └─► Full validation or targeted check?                      │
│           │                                                     │
│           ▼                                                     │
│                                                                 │
│  Phase 2: applyTo Conflict Check (CRITICAL)                    │
│     └─► Extract patterns from all instruction files             │
│     └─► Cross-check for overlaps                                │
│     └─► Verify scope specificity                                │
│           │                                                     │
│           ▼ [GATE: No conflicting overlaps?]                    │
│                                                                 │
│  Phase 3: Full Validation (instruction-validator)               │
│     └─► YAML frontmatter compliance                             │
│     └─► Token budget (≤1,500)                                   │
│     └─► Layer boundaries (no embedded knowledge >10 lines)      │
│     └─► Rule consistency with context files                     │
│     └─► Reference integrity                                     │
│     └─► Imperative language                                     │
│           │                                                     │
│           ▼ [GATE: Validation passed?]                          │
│                                                                 │
│  Phase 4: Issue Resolution (instruction-builder, if needed)     │
│     └─► Categorize issues by severity                           │
│     └─► Apply fixes                                             │
│     └─► Return to Phase 2/3 for re-validation                  │
│           │                                                     │
│           ▼ [Loop until passed or blocked]                      │
│                                                                 │
│  Phase 5: Final Report                                          │
│     └─► Validation summary with scores                          │
│     └─► applyTo conflict matrix (layer audit)                  │
│     └─► Recommendations                                         │
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
    - "agent-review"
    - "instruction-file-review"
    - "prompt-review"
    - "skill-review"
  changelog: "workflow-review-diagrams.template.changelog.md"
---
-->
