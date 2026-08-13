---
name: pe-con-design
description: "Orchestrates the complete PE artifact design workflow for ANY artifact type — research → build → validate using consolidated agents with artifact-type dispatch"
agent: agent
model: claude-opus-4.6
tools:
  - semantic_search
  - read_file
  - file_search
  - create_file
handoffs:
  - label: "Research Requirements"
    agent: pe-con-researcher
    send: true
  - label: "Build Artifact"
    agent: pe-con-builder
    send: true
  - label: "Validate Artifact"
    agent: pe-con-validator
    send: true
argument-hint: '<artifact-type> <description> — e.g., "agent for reviewing article frontmatter" or "hook for PreToolUse deny on .env files"'
goal: "Orchestrate multi-phase creation of any PE artifact type with quality gates via consolidated dispatch"
scope:
  covers:
    - "Full design pipeline (research → build → validate) for all 8 PE artifact types"
    - "Use case challenge, pattern discovery, and requirements validation"
  excludes:
    - "System-level PE management (meta-prompts handle this)"
    - "Direct file updates without research (pe-con-create-update handles this)"
boundaries:
  - "MUST route through all 3 phases: researcher → builder → validator"
  - "MUST NOT skip validation phase"
rationales:
  - "Single prompt replaces 7 type-specific design prompts by dispatching to the same 3 consolidated agents"
  - "Quality gates between phases catch issues before they propagate"
---

# PE Consolidated Design

Design and create ANY PE artifact type through a full research → build → validate pipeline.

## Supported artifact types

Agent, Prompt, Context file, Instruction file, Skill, Template, Hook, Prompt snippet

## Workflow

1. **Research** — `@pe-con-researcher` discovers requirements, challenges use cases, finds patterns
2. **Build** — `@pe-con-builder` constructs the artifact using dispatch table for template/rule selection
3. **Validate** — `@pe-con-validator` checks structure, compliance, and cross-artifact coherence

## How to use

Specify the artifact type and description:

```
/pe-con-design agent for reviewing article frontmatter
/pe-con-design hook for PreToolUse deny on .env files
/pe-con-design context file for MCP server patterns
/pe-con-design prompt for generating tech session summaries
```

The prompt determines the artifact type from your description and routes through the appropriate research → build → validate pipeline automatically.

<!--
prompt_metadata:
  version: "1.0.0"
  last_updated: ""
-->
