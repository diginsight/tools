---
description: Instructions for creating and updating effective prompt files
applyTo: '.github/prompts/**/*.md'
version: "1.3.0"
last_updated: "2026-03-19"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Prompt File Creation Instructions

## Purpose

Prompt files are **reusable, plan-level workflows** for common development tasks. They inject into the USER prompt (not system prompt) and define WHAT should be done and HOW.

**📖 Shared PE rules (auto-applied):** [pe-common.instructions.md](pe-common.instructions.md)

## Severity Index (prompt-specific; shared rules in pe-common)

**CRITICAL** — block on failure:
- **[C4]** Handoff targets: every `agent:` in `handoffs:` resolves to existing file
- **[C6]** YAML frontmatter: name, description, agent mode, tools required
- **[C7]** Top YAML never modified: validation prompts MUST NOT touch article YAML

**HIGH** — fix before use:
- **[H9]** Required sections: purpose, workflow steps, output format

**MEDIUM** — fix when convenient:
- **[M3]** Context rot prevention: multi-phase prompts use summarization
- **[M5]** Validation caching: check `last_run` < 7 days before re-validating
- **[M6]** Naming: `[task-name].prompt.md` kebab-case
- **[M10]** Gate checks: phase transitions have completion + goal alignment check

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)

## Required YAML Frontmatter

```yaml
---
name: prompt-file-name
description: "One-sentence description"
agent: plan  # or: agent
model: claude-opus-4.6
tools:
  - read_file
  - grep_search
---
```

**📖 Prompt assembly architecture:** [01.02-prompt-assembly-architecture.md](.copilot/context/00.00-prompt-engineering/01.02-prompt-assembly-architecture.md)
**📖 Model-specific optimization:** [03.02-model-specific-optimization.md](.copilot/context/00.00-prompt-engineering/03.02-model-specific-optimization.md)

## Rules

- **Use specialized templates** from `.github/templates/`: validation, implementation, orchestration, analysis
- Reference context files via `📖` — don't embed shared principles
- Narrow tool scope to only essential tools
- Start with template, test execution on real content, iterate boundaries

**To create new prompts:** Use `@prompt-create-orchestrator` (researcher → builder → validator)

## Quality Checklist

- [ ] YAML: name, description, agent mode, tools present (C6)
- [ ] Handoff targets resolve to existing agents (C4)
- [ ] Purpose and workflow steps defined (H9)
- [ ] Multi-phase prompts use summarization (M3)
- [ ] Tool alignment matches agent mode (C1, via pe-common)
- [ ] Tested on real repository content

## References

- [VS Code: Copilot Customization](https://code.visualstudio.com/docs/copilot/copilot-customization)
- **📖** [.copilot/context/00.00-prompt-engineering/](.copilot/context/00.00-prompt-engineering/) — All PE context
- **📖** `.github/templates/00.00-prompt-engineering/` — Prompt templates
