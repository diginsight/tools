---
description: Instructions for creating and maintaining context files
applyTo: '.copilot/context/**/*.md'
version: "1.4.0"
last_updated: "2026-03-19"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Context File Creation & Maintenance Instructions

## Purpose

Context files are **shared reference documents** that provide the single source of truth for principles, patterns, and conventions. Automatically indexed by semantic search — no explicit `#file:` references needed.

## Severity Index

**CRITICAL** — block on failure:
- **[C3]** Token budget: context files ≤2,500 tokens
- **[C5]** No circular dependencies: deps flow upward only
- **[C6]** YAML frontmatter: title, description, version, last_updated

**HIGH** — fix before use:
- **[H3]** Single source of truth: no duplicated content across context files
- **[H7]** Narrow scope: one concept domain per file
- **[H8]** Imperative language: MUST/NEVER/SHOULD
- **[H9]** Required sections: Purpose, Referenced by, core content, References
- **[H12]** Cross-reference integrity: all `📖` links resolve
- **[H13]** Full-filename references: `📖` refs MUST use the full filename (e.g., `📖 \`02.04-agent-shared-patterns.md\``), NEVER bare numeric prefixes (e.g., `📖 \`02.04\``). Prefixes are fragile — files can be renamed or renumbered.

**MEDIUM** — fix when convenient:
- **[M6]** Naming: `[concept-name].md` lowercase hyphenated
- **[M8]** Version history: updated after every meaningful change

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)

## Required YAML Frontmatter

```yaml
---
title: "Context File Title"
description: "One-sentence summary"
version: "1.0.0"
last_updated: "YYYY-MM-DD"
---
```

| Field | Required | Purpose |
|-------|----------|---------|
| `title` | ✅ MUST | Human-readable title (matches H1) |
| `description` | ✅ MUST | One-sentence summary for discovery |
| `version` | ✅ MUST | PATCH: typos, MINOR: new sections, MAJOR: structural |
| `last_updated` | ✅ MUST | ISO date. Meta-prompts flag >90 days stale. |

## Rules

### Core Principles
- Each concept MUST be documented in exactly one file — use cross-references, never duplicate
- Prompts and agents MUST reference via `📖` links, not embed content
- Context files MUST be organized by domain in subdirectories under `.copilot/context/`

### Required Document Structure
- H1 title → **Purpose** statement → **Referenced by** list → core content → **References** → **Version History**
- Domain folders use `{NN.NN}-{domain-name}/` prefix, MUST contain ≥2 files
- Each file MUST include `domain:` in YAML; add `authoritative_sources:` for traceability

### Content Rules
- Use MUST/NEVER (hard), SHOULD/PREFER (best practice), MAY/CAN (optional)
- Code examples must be complete, from THIS repository, and annotated
- Cross-references: `**📖 Related:** [file.md](file.md)` for context-to-context; folder refs from agents/prompts

### Maintenance
- **Create** when same content appears in 3+ prompts/agents
- **Update** when best practices evolve, external docs change, or anti-patterns emerge
- **When too large**: Split by topic, extract examples to templates

## Quality Checklist

- [ ] Purpose statement clear and specific
- [ ] `Referenced by` lists actual dependents (H9)
- [ ] No duplicated content from other context files (H3)
- [ ] Token budget ≤2,500 (C3)
- [ ] Cross-references resolve (H12)
- [ ] References section includes external sources

## References

- [VS Code Copilot Docs](https://code.visualstudio.com/docs/copilot/copilot-customization)
- **📖** File type decisions: see `file-type-guide` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)
- **📖** Token budgets: see `token-optimization` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)
