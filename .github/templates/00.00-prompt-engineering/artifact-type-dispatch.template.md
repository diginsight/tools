---
title: "Artifact-type dispatch reference"
description: "Lookup table for PE agents — maps each artifact type to its instruction file, templates, validation rules, quality bar, construction invariants, and applicable dimensions"
audience: "pe-con-researcher, pe-con-builder, pe-con-validator, pe-meta-builder, pe-meta-validator"
---

# Artifact-type dispatch reference

> **Audience**: Consolidated PE agents (`pe-con-*`) and pe-meta agents (`pe-meta-builder`, `pe-meta-validator`). Granular agents (`pe-gra-*`) have artifact type hardcoded — they do NOT load this file.
>
> **pe-meta agents**: Use the "Exemplary bar" and "Construction invariants" columns for PE-for-PE artifacts. Use "Standard bar" for domain artifacts.
>
> **Context file references**: Per R-S5-chain-alignment, the default reference is the folder `.copilot/context/00.00-prompt-engineering/`. The "Key context files" column lists **priority files to load first** for each artifact type — not the only files available. If a listed file is missing or renamed, fall back to scanning the folder.

## Dispatch table

| Artifact type | Instruction file | Primary template | Key context files (load first) | Type-specific validation | Exemplary bar | Construction invariants? | Applicable dims |
|---|---|---|---|---|---|---|---|
| **Agent** | `pe-agents.instructions.md` | `agent.template.md` | `02.04-agent-shared-patterns.md`, `01.04-tool-composition-guide.md` | Tool count 3–7, mode alignment, handoff resolution, boundary completeness | ≥5/2/3 boundaries, full metadata, category refs | NO (consumer) | 21 |
| **Prompt** | `pe-prompts.instructions.md` | `prompt.template.md` | `02.03-orchestrator-design-patterns.md`, `01.04-tool-composition-guide.md` | Gate checks, tool scope, agent references resolve | Full metadata, `--dim` support, model routing | NO (consumer) | 22 |
| **Context file** | `pe-context-files.instructions.md` | — | `01.01-context-engineering-principles.md` | ≤2,500 tokens, single source of truth, no circular deps | Full metadata incl. rationales, N-1 on all rule sections | YES (6 properties) | 16 |
| **Instruction file** | `pe-instruction-files.instructions.md` | `instructions.template.md` | `01.07-critical-rules-priority-matrix.md` | applyTo conflict-free, ≤1,500 tokens, only testable rules | Full metadata, R-S8 testability | YES (rules must be testable) | 13 |
| **Skill** | `pe-skills.instructions.md` | `skill.template.md` | `03.01-progressive-disclosure-pattern.md` | Description ≤1024 chars, Level 1 <100 tokens, required sections | Progressive disclosure required | NO | 6 |
| **Template** | `pe-templates.instructions.md` | — | `03.07-template-authoring-patterns.md` | ≤100 lines, .template.md extension, category prefix | Placeholders documented | NO | 6 |
| **Hook** | `pe-hooks.instructions.md` | — | `03.03-agent-hooks-reference.md` | Valid JSON, supported lifecycle event, companion script exists | N/A | NO | 4 |
| **Prompt snippet** | `pe-prompt-snippets.instructions.md` | — | — | ≤500 words, no YAML frontmatter, self-contained | Reusability required | NO | 5 |

## How consolidated agents use this table

1. **Determine artifact type** from user request or handoff context
2. **`read_file`** this dispatch table (MANDATORY Phase 1 step)
3. **Load the instruction file** for that type (`read_file`)
4. **Load the primary template** if creating/updating (`read_file`)
5. **Load key context files** if the artifact interacts with that domain
6. **Apply type-specific validation** rules during pre-save checks

<!--
template_metadata:
  version: "2.0.0"
  last_updated: "2026-05-15"
-->
