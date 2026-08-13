---
name: pe-con-create-update
description: "Create new or update existing PE artifacts of ANY type — skips research phase, goes directly to build → validate using consolidated agents with artifact-type dispatch"
agent: agent
model: claude-opus-4.6
tools:
  - semantic_search
  - read_file
  - file_search
  - create_file
  - replace_string_in_file
handoffs:
  - label: "Build Artifact"
    agent: pe-con-builder
    send: true
  - label: "Validate Artifact"
    agent: pe-con-validator
    send: true
argument-hint: '<artifact-type> <file-path-or-description> — e.g., "agent .github/agents/my-agent.agent.md" or "hook for session logging"'
goal: "Create or update any PE artifact type with structural validation via consolidated dispatch"
scope:
  covers:
    - "Direct creation and update of all 8 PE artifact types"
    - "Build → validate pipeline (skips research)"
  excludes:
    - "Requirements research and use case challenge (pe-con-design handles this)"
    - "System-level PE management (meta-prompts handle this)"
boundaries:
  - "MUST route through builder → validator"
  - "MUST NOT skip validation phase"
rationales:
  - "Single prompt replaces 8 type-specific create-update prompts"
  - "Skips research phase for faster iteration when requirements are already known"
---

# PE Consolidated Create/Update

Create new or update existing PE artifacts of ANY type. Skips the research phase — goes directly to build → validate.

## Supported artifact types

Agent, Prompt, Context file, Instruction file, Skill, Template, Hook, Prompt snippet

## Workflow

1. **Build** — `@pe-con-builder` constructs or updates the artifact using dispatch table for template/rule selection
2. **Validate** — `@pe-con-validator` checks structure, compliance, and cross-artifact coherence

## How to use

For **new artifacts** — specify type and description:
```
/pe-con-create-update agent for reviewing article series consistency
/pe-con-create-update hook for PreToolUse deny on credentials
```

For **existing artifacts** — specify type and file path:
```
/pe-con-create-update agent .github/agents/00.01-pe-consolidated/pe-con-builder.agent.md
/pe-con-create-update context .copilot/context/00.00-prompt-engineering/02.04-agent-shared-patterns.md
```

The builder loads the dispatch table, selects the correct instruction file and template, and applies type-specific rules automatically.

<!--
prompt_metadata:
  version: "1.0.0"
  last_updated: ""
-->
