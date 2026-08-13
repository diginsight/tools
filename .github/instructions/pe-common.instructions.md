---
description: Shared PE artifact rules — context engineering, tool selection, validation caching, production readiness, uncertainty management
applyTo: '.github/prompts/**/*.md,.github/agents/**/*.agent.md'
domain: "prompt-engineering"
goal: "Provide the shared prompt-and-agent baseline rules (tool alignment, token budget, production readiness, references, runtime grounding) that type-specific instruction files extend and override on conflict"
rationales:
  - "A single shared-baseline file removes duplicated cross-artifact rules from the prompt and agent instruction files"
  - "Centralizing the shared severity index keeps the type-specific instruction files focused on their type-specific additions"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# PE Common Instructions

Shared rules for all prompt and agent files. This is the designated shared-baseline instruction file for those two artifact types. Type-specific rules remain in `prompts.instructions.md` and `agents.instructions.md` and override this baseline on conflicts.

## Scope Boundary

- This file is the single allowed shared-baseline overlap under `pe-instruction-files.instructions.md`
- It MUST contain only cross-artifact prompt-and-agent rules
- Prompt-specific and agent-specific rules MUST stay in their type-specific instruction files

## Severity Index

**CRITICAL** — block on failure:
- **[C1]** Tool alignment: `plan` = read-only; `agent` = read+write. NEVER mix.
- **[C3]** Token budget compliance per artifact type
- **[C5]** No circular dependencies (context → instructions → agents → prompts)
- **[C7]** ❌ NEVER modify top YAML from validation prompts. ✅ Update bottom metadata only.

**HIGH** — fix before use:
- **[H2]** Tool count: 3–7 per artifact; >7 = decompose
- **[H4]** Response management: data gap scenarios MUST be defined. 📖 See `04.03-production-readiness-patterns.md` for implementation patterns.
- **[H5]** Error recovery: fallback behavior for tool failures MUST be defined. 📖 See `04.03-production-readiness-patterns.md` for implementation patterns.
- **[H6]** Embedded test scenarios (3–5 per prompt, 3 per agent)
- **[H7]** Narrow scope: one primary goal per artifact
- **[H8]** Imperative language: MUST/NEVER/ALWAYS
- **[H12]** Cross-reference integrity: all `📖` links resolve
- **[H13]** Full-filename references: `📖` refs MUST use the full filename (e.g., `📖 \`02.04-agent-shared-patterns.md\``), NEVER bare numeric prefixes (e.g., `📖 \`02.04\``). Prefixes are fragile — files can be renamed or renumbered.
- **[H14]** Runtime grounding (metadata precedence): agents and prompts MUST enforce YAML `boundaries:` and `scope:` with precedence over body content (metadata wins on conflict). Agents: a single collective directive grounds the whole `boundaries:` list; three-tier entries are additive and MUST NOT restate a YAML boundary verbatim. Prompts: workflow includes a scope enforcement step. 📖 See `00.03-metadata-contracts.md` — Runtime grounding protocol.

**MEDIUM** — fix when convenient:
- **[M1]** Template externalization: inline blocks >10 lines → template
- **[M2]** Early commands: critical instructions in first 30%
- **[M4]** Group `📖` references preferred over individual file refs

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)

## Context Engineering Principles

All PE artifacts MUST follow these context engineering principles: narrow scope, early commands, imperative language, template externalization (>10 lines → template), context minimization, uncertainty management, reference-based architecture. Agents additionally require three-tier boundaries (see `agents.instructions.md`).

**📖 Complete guidance:** [01.01-context-engineering-principles.md](../../.copilot/context/00.00-prompt-engineering/01.01-context-engineering-principles.md)

## Production Readiness

Every PE artifact MUST implement: response management, error recovery, embedded test scenarios (3–5), token budget compliance, context rot prevention, template externalization.

**📖** Full requirements: [04.03-production-readiness-patterns.md](../../.copilot/context/00.00-prompt-engineering/04.03-production-readiness-patterns.md)

## Quality Checklist

- [ ] Tool alignment matches agent mode (C1)
- [ ] Token budget within type-specific limit (C3)
- [ ] No circular dependencies (C5)
- [ ] Response management and "I don't know" scenarios defined (H4)
- [ ] 3–5 embedded test scenarios (H6)
- [ ] All `📖` references resolve (H12)
- [ ] Runtime grounding: body enforces YAML boundaries/scope with precedence; collective directive present, no verbatim restatement (H14)

## References

- **📖** Context engineering principles: see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)
- **📖** [01.04-tool-composition-guide.md](../../.copilot/context/00.00-prompt-engineering/01.04-tool-composition-guide.md) — Tool categories, costs
- **📖** [04.01-validation-caching-pattern.md](../../.copilot/context/00.00-prompt-engineering/04.01-validation-caching-pattern.md) — Validation caching
- **📖** [04.03-production-readiness-patterns.md](../../.copilot/context/00.00-prompt-engineering/04.03-production-readiness-patterns.md) — Production readiness
- **📖** `.github/templates/` — 26+ reusable templates

<!--
instruction_metadata:
  version: "1.9.0"
  last_updated: "2026-06-12"
-->
