---
description: Instructions for creating and updating custom agent files
applyTo: '.github/agents/**/*.agent.md'
version: "1.8.0"
last_updated: "2026-03-20"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Custom Agent File Creation & Update Instructions

## Purpose

Custom agents are **specialized assistants for specific roles or implementation tasks** with detailed technical instructions, tool access, and autonomous execution.

**📖 Shared PE rules (auto-applied):** [pe-common.instructions.md](pe-common.instructions.md)

## Severity Index (agent-specific; shared rules in pe-common)

**CRITICAL** — block on failure:
- **[C4]** Handoff targets: every `agent:` in `handoffs:` resolves to existing file
- **[C6]** YAML frontmatter: description, tools, agent (mode) required

**HIGH** — fix before use:
- **[H1]** Boundary completeness: ≥3 Always Do, ≥1 Ask First, ≥2 Never Do
- **[H9]** Required sections: persona, boundaries, quality checklist
- **[H11]** Single responsibility: one role per agent (researcher OR builder OR validator)

**MEDIUM** — fix when convenient:
- **[M6]** Naming: kebab-case, action-verb-first, ≤5 words

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)

## Required YAML Frontmatter

**All agents:**

```yaml
---
description: "One-sentence description of agent's role"  # REQUIRED — <200 chars
agent: plan|agent  # REQUIRED — execution mode
tools: ['specific', 'tools', 'only']  # REQUIRED
model: claude-opus-4.6  # OPTIONAL — only when overriding default
handoffs: [...]  # OPTIONAL — for orchestration
---
```

**PE agents** (add these fields):

```yaml
version: "1.0.0"  # REQUIRED
last_updated: "YYYY-MM-DD"  # REQUIRED
context_dependencies:  # REQUIRED — folder-level only
  - "00.00-prompt-engineering/"
domain: "prompt-engineering"  # REQUIRED (from A-23)
capabilities:  # REQUIRED — 3-5 actionable verb phrases (from A-23)
  - "capability description"
goal: "One-sentence purpose statement"  # REQUIRED (from A-23)
```

**Field rules:**
- `name:` — **DO NOT include.** VS Code derives the agent name from the filename.
- `version` — semver string, increment on meaningful changes
- `last_updated` — ISO date of last modification
- `context_dependencies` — list of context **folders** only (e.g., `"00.00-prompt-engineering/"`), not individual files. Used by the scheduled review to detect staleness via cascade validation: if any context file in a dependency folder has a newer `last_updated` than the agent, the agent is flagged as potentially stale.

**📖 Advanced YAML fields + visibility matrix**: [01.02-prompt-assembly-architecture.md](.copilot/context/00.00-prompt-engineering/01.02-prompt-assembly-architecture.md)
**📖 AGENTS.md vs .agent.md comparison**: [01.03-file-type-decision-guide.md](.copilot/context/00.00-prompt-engineering/01.03-file-type-decision-guide.md)

## Rules

- Researchers with `fetch_webpage` MUST validate all internet findings. `Scope: local-only` = MUST NOT use `fetch_webpage`.
- Keep agent core instructions < 1,000 tokens — extract to instruction/context files, use handoffs.
- Start minimal (persona + boundaries + 2-3 commands), test in real scenarios, iterate.
- Three-tier boundaries (Always Do / Ask First / Never Do) are an agent-specific context engineering principle — prompts do not define boundaries.
- Agent conversations accumulate tool results across steps. For multi-step workflows, implement progressive summarization to prevent context bloat.

**📖 Shared patterns (structural):** [02.04-agent-shared-patterns.md](.copilot/context/00.00-prompt-engineering/02.04-agent-shared-patterns.md)
**📖 Shared patterns (workflow):** [02.05-agent-workflow-patterns.md](.copilot/context/00.00-prompt-engineering/02.05-agent-workflow-patterns.md)
**📖 Orchestration:** [02.03-orchestrator-design-patterns.md](.copilot/context/00.00-prompt-engineering/02.03-orchestrator-design-patterns.md)
**📖 Handoffs:** [02.01-handoffs-pattern.md](.copilot/context/00.00-prompt-engineering/02.01-handoffs-pattern.md)

## Quality Checklist

- [ ] YAML: description, agent (mode), tools required; model/handoffs optional; no `name:` field (C6)
- [ ] Tools: 3–7 items, mode-aligned (H2, via pe-common)
- [ ] Three-tier boundaries defined (H1)
- [ ] Single responsibility: one role per agent (H11)
- [ ] "I don't know" scenarios defined (H4, via pe-common)
- [ ] Tested with real repository tasks

## References

- [VS Code: Custom Agents](https://code.visualstudio.com/docs/copilot/copilot-customization)
- **📖** [.copilot/context/00.00-prompt-engineering/](.copilot/context/00.00-prompt-engineering/) — All PE context
- **📖** `.github/templates/00.00-prompt-engineering/agent.template.md` — Agent template
