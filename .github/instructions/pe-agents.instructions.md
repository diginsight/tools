---
description: Instructions for creating and updating custom agent files
applyTo: '.github/agents/**/*.agent.md'
domain: "prompt-engineering"
goal: "Ensure agent files have proper structure, tool access, and autonomous execution capabilities for their specialized roles"
rationales:
  - "Agents need detailed technical instructions and tool restrictions to avoid scope overlap"
  - "Consistency across agent files prevents misconfiguration"
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
- **[H1]** Boundary completeness: ≥3 Always Do, ≥1 Ask First, ≥2 Never Do (YAML `boundaries:` entries count toward these minimums; the collective grounding directive enforces them — additive three-tier entries need not duplicate them)
- **[H9]** Required sections: persona, boundaries, quality checklist
- **[H11]** Single responsibility: one role per agent (researcher OR builder OR validator)

**MEDIUM** — fix when convenient:
- **[M6]** Naming: kebab-case, action-verb-first, ≤5 words

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)

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

**PE agents** (add these top-frontmatter fields):

```yaml
context_dependencies:  # REQUIRED — folder-level only
  - "00.00-prompt-engineering/"
domain: "prompt-engineering"  # REQUIRED — single scalar; see 00.03-metadata-contracts.md § `domain:` field semantics (multi-domain artifacts NOT supported; cross-domain dependencies handled via Phase 0b `bundle=cross-domain-deps`)
capabilities:  # REQUIRED — 3-5 actionable verb phrases (from A-23)
  - "capability description"
goal: "One-sentence purpose statement"  # REQUIRED (from A-23)
```

> `version`/`last_updated` are **NOT** top-frontmatter fields — they live in the bottom `agent_metadata` block (see Bottom Metadata below).

**Field rules:**
- `name:` — **DO NOT include.** VS Code derives the agent name from the filename.
- `context_dependencies` — list of context **folders** only (e.g., `"00.00-prompt-engineering/"`), not individual files. Used by the scheduled review to detect staleness via cascade validation: if any context file in a dependency folder has a newer bottom-block `last_updated` than the agent's bottom-block `last_updated`, the agent is flagged as potentially stale.

**📖 Advanced YAML fields + visibility matrix**: [01.02-prompt-assembly-architecture.md](../../.copilot/context/00.00-prompt-engineering/01.02-prompt-assembly-architecture.md)
**📖 AGENTS.md vs .agent.md comparison**: [01.03-file-type-decision-guide.md](../../.copilot/context/00.00-prompt-engineering/01.03-file-type-decision-guide.md)

## Bottom Metadata (REQUIRED)

Every PE agent MUST carry change-prone tracking metadata in a bottom `agent_metadata` HTML comment — NOT in top frontmatter. This follows the dual metadata pattern (📖 `00.03-metadata-contracts.md` § Field placement):

```html
<!--
agent_metadata:
  version: "1.0.0"
  last_updated: "YYYY-MM-DD"
  created: "YYYY-MM-DD"        # OPTIONAL
  changelog: "<agent-stem>.changelog.md"   # OPTIONAL — only when a sibling changelog file exists
-->
```

- `version` — SemVer string; increment on meaningful changes (patch/minor/major per `00.03`).
- `last_updated` — ISO `YYYY-MM-DD` of the most recent change.
- Top frontmatter MUST NOT carry `version` or `last_updated` — a single bottom-block source prevents the top/bottom drift the contract eliminates.

- Researchers with `fetch_webpage` MUST validate all internet findings. `Scope: local-only` = MUST NOT use `fetch_webpage`.
- Keep agent core instructions preferably <1,000 tokens for maintainability; treat the per-type checklist ceiling as the enforcement boundary (agent hard ceiling: 2,500 tokens unless superseded).
- Three-tier boundaries (Always Do / Ask First / Never Do) are an agent-specific context engineering principle — prompts do not define boundaries.
- **Runtime grounding (metadata precedence):** the body MUST carry a single collective directive that enforces every YAML `boundaries:` entry throughout execution, with precedence over body content (metadata wins on conflict). Three-tier entries are **additive** — they add thresholds, escalation triggers, or sequencing, and MUST NOT restate a YAML boundary verbatim. 📖 See `00.03-metadata-contracts.md` — Runtime grounding protocol.
- Agent conversations accumulate tool results across steps. For multi-step workflows, implement progressive summarization to prevent context bloat.

**📖 Shared patterns (structural):** [02.04-agent-shared-patterns.md](../../.copilot/context/00.00-prompt-engineering/02.04-agent-shared-patterns.md)
**📖 Shared patterns (workflow):** [02.05-agent-workflow-patterns.md](../../.copilot/context/00.00-prompt-engineering/02.05-agent-workflow-patterns.md)
**📖 Orchestration:** [02.03-orchestrator-design-patterns.md](../../.copilot/context/00.00-prompt-engineering/02.03-orchestrator-design-patterns.md)
**📖 Handoffs:** [02.01-handoffs-pattern.md](../../.copilot/context/00.00-prompt-engineering/02.01-handoffs-pattern.md)

## Quality Checklist

- [ ] YAML: description, agent (mode), tools required; model/handoffs optional; no `name:` field (C6)
- [ ] No `version`/`last_updated` in top frontmatter; both present in bottom `agent_metadata` block
- [ ] Tools: 3–7 items, mode-aligned (H2, via pe-common)
- [ ] Three-tier boundaries defined (H1)
- [ ] Body carries the collective grounding directive; three-tier entries are additive, none restates a YAML boundary verbatim (H14, via pe-common)
- [ ] Single responsibility: one role per agent (H11)
- [ ] "I don't know" scenarios defined (H4, via pe-common)
- [ ] Tested with real repository tasks

## References

- [VS Code: Custom Agents](https://code.visualstudio.com/docs/copilot/copilot-customization)
- **📖** [.copilot/context/00.00-prompt-engineering/](../../.copilot/context/00.00-prompt-engineering/) — All PE context
- **📖** `.github/templates/00.00-prompt-engineering/agent.template.md` — Agent template

<!--
instruction_metadata:
  version: "1.13.0"
  last_updated: "2026-06-12"
-->
