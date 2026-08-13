---
description: Instructions for creating Agent Skills (SKILL.md files)
applyTo: '.github/skills/**/SKILL.md,.github/templates/skill-*.md'
version: "1.4.0"
last_updated: "2026-03-19"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Agent Skill Creation Instructions

> ⚠️ **Preview Feature**: Agent Skills (VS Code 1.107+) are in Preview. APIs may change.

## Purpose

Agent Skills are **portable, resource-rich workflows** across VS Code, CLI, and GitHub coding agent. They bundle templates, scripts, and examples with instructions following the [agentskills.io](https://agentskills.io/) standard.

## Severity Index

**CRITICAL** — block on failure:
- **[C3]** Token budget: description + body < 1,500 tokens
- **[C6]** YAML frontmatter: name and description required

**HIGH** — fix before use:
- **[H7]** Narrow scope: one workflow domain per skill
- **[H8]** Imperative language: MUST/NEVER in workflow steps
- **[H9]** Required sections: Purpose, When to Use, Workflow

**MEDIUM** — fix when convenient:
- **[M6]** Naming: lowercase with hyphens, ≤64 characters
- **[M7]** Description optimized: [what] + [tech] + "Use when" + [scenarios]

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)

## Required YAML Frontmatter

```yaml
---
name: skill-name           # lowercase, hyphens, max 64 chars
description: >             # max 1024 chars
  What this skill does and technologies involved.
  Use when [scenario 1], [scenario 2], or [scenario 3].
---
```

**Description formula**: `[What it does] + [Technologies] + "Use when" + [Scenarios]`

## Required Directory Structure

```
.github/skills/skill-name/
├── SKILL.md          # Required: Instructions + metadata
├── templates/        # Optional: Reusable templates
├── examples/         # Optional: Real-world examples
└── scripts/          # Optional: Automation scripts
```

## Progressive Disclosure

| Level | What Loads | When | Tokens |
|-------|------------|------|--------|
| **1. Discovery** | name + description | Always | ~50-100 |
| **2. Instructions** | SKILL.md body | Prompt matches description | ~500-1500 |
| **3. Resources** | Templates, examples | Copilot references them | On-demand |

Optimize `description` for discovery accuracy — it determines whether Level 2/3 ever load.

## Rules

- **ALWAYS use relative paths** from SKILL.md — never absolute paths or external URLs
- Required body sections: **Purpose** (1-2 sentences), **When to Use** (bullet list), **Workflow** (steps)
- Description + Body < 1,500 tokens; Level 1 discovery < 100 tokens
- Skills work cross-platform (VS Code, CLI, Coding Agent) — make scripts cross-platform
- Skills have **no tool control, no handoffs, no file pattern activation** — use agents for complex orchestration
- Test: verify discovery ("What skills are available?"), activation with varied phrasings, non-activation for unrelated prompts

**📖 Skill template:** `.github/templates/00.00-prompt-engineering/skill.template.md`

## Quality Checklist

- [ ] Name: lowercase with hyphens, ≤64 chars (M6)
- [ ] Description: capabilities + "Use when" scenarios, ≤1024 chars (M7)
- [ ] All resource paths are relative (C6)
- [ ] Token budget: description + body < 1,500 tokens (C3)
- [ ] Activates for intended prompts, doesn't activate for unrelated ones

## References

- [VS Code: Agent Skills](https://code.visualstudio.com/docs/copilot/customization/agent-skills)
- [agentskills.io](https://agentskills.io/) — Open standard
- **📖** [01.03-file-type-decision-guide.md](.copilot/context/00.00-prompt-engineering/01.03-file-type-decision-guide.md) — Skills vs prompts vs agents
