---
name: pe-con-review
description: "Validate and review ANY PE artifact type — runs consolidated validator with artifact-type dispatch for type-specific rules"
agent: plan
model: claude-opus-4.6
tools:
  - semantic_search
  - read_file
  - file_search
  - grep_search
  - list_dir
handoffs:
  - label: "Validate Artifact"
    agent: pe-con-validator
    send: true
  - label: "Fix Issues"
    agent: pe-con-builder
    send: true
argument-hint: '<artifact-type> <file-path> — e.g., "agent .github/agents/00.01-pe-consolidated/pe-con-builder.agent.md"'
goal: "Produce an actionable validation report for any PE artifact type via consolidated dispatch"
scope:
  covers:
    - "Validation and review of all 8 PE artifact types"
    - "Severity-ranked findings with fix recommendations"
  excludes:
    - "File creation (pe-con-create-update handles this)"
    - "System-level PE auditing (meta-prompts handle this)"
boundaries:
  - "MUST stay read-only — plan mode"
  - "MUST hand off to pe-con-builder only when fixes are needed"
rationales:
  - "Single prompt replaces 6 type-specific review prompts"
  - "Plan mode ensures review cannot accidentally modify the file being reviewed"
---

# PE Consolidated Review

Validate and review ANY PE artifact type. Produces a severity-ranked validation report with specific fix recommendations.

## Supported artifact types

Agent, Prompt, Context file, Instruction file, Skill, Template, Hook, Prompt snippet

## Workflow

1. **Validate** — `@pe-con-validator` loads dispatch table, determines artifact type, runs type-specific validation rules
2. **Fix** (if needed) — `@pe-con-builder` applies recommended fixes

## How to use

Specify the artifact type and file path:
```
/pe-con-review agent .github/agents/00.01-pe-consolidated/pe-con-researcher.agent.md
/pe-con-review context .copilot/context/00.00-prompt-engineering/02.04-agent-shared-patterns.md
/pe-con-review hook .github/hooks/pe-healthcheck.json
/pe-con-review instruction .github/instructions/pe-common.instructions.md
```

The validator loads the dispatch table, selects type-specific validation rules, and produces a structured report with CRITICAL/HIGH/MEDIUM/LOW findings.

<!--
prompt_metadata:
  version: "1.0.0"
  last_updated: ""
-->
