---
description: Rules for creating and maintaining reusable prompt-snippet fragments included via #file references
applyTo: '.github/prompt-snippets/**/*.md'
version: "1.3.0"
last_updated: "2026-03-19"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Prompt-Snippet Rules

## Purpose

Prompt snippets are **reusable Markdown fragments** included on-demand via `#file:.github/prompt-snippets/filename.md`. Not indexed as slash commands, not auto-injected. Each snippet MUST be concise, self-contained, and non-duplicative.

## Severity Index

**CRITICAL** — block on failure:
- **[C3]** Token budget: snippets ≤500 words

**HIGH** — fix before use:
- **[H3]** Single source of truth: no duplication with context or instruction files
- **[H8]** Imperative language: concise, actionable content

**MEDIUM** — fix when convenient:
- **[M6]** Naming: kebab-case `{purpose}.md`

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)

## Rules

- MUST NOT include YAML frontmatter — snippets are raw Markdown fragments
- MUST include a brief header comment explaining purpose and usage
- MUST be self-contained — work when included without surrounding context
- MUST be placed in `.github/prompt-snippets/` root — no subfolders
- MUST NOT exceed **500 words**
- MUST NOT duplicate content from context or instruction files
- Before modifying, discover all consumers via `grep_search` for `prompt-snippets/{filename}`
- Renaming MUST update all consumer `#file:` references

### Scoping Rules
- >500 words or shared reference → **context file**, not snippet
- Enforces rules for a file type → **instruction file**, not snippet
- One-off inline block → embed directly, don't create snippet
- Snippets are for fragments used by 2+ consumers via `#file:`

## Quality Checklist

- [ ] ≤500 words (C3)
- [ ] No duplication with context/instruction files (H3)
- [ ] Self-contained — works without surrounding context
- [ ] All consumers verified before modification

## References

- **📖** Context engineering: see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)
- **📖** File type decisions: see `file-type-guide` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)
