---
description: Template for creating instruction files (.instructions.md)
---

> ⚠️ **ORPHANED TEMPLATE** (2026-03-08)
> This template is not referenced by any prompt, agent, or instruction file.
> **Alternatives:**
> - Use `promptengineering-instruction-structure.template.md` (referenced by `instruction-builder` agent)
> - Use `/instruction-file-create-update` prompt for guided instruction creation
> **Action:** Will be removed after 2026-04-08 unless a consumer is identified.

# Instructions Template

Use this template to create instruction files in `.github/instructions/`.

## File Template

```markdown
---
description: "One-sentence description of what these instructions govern"
applyTo: '**/*.ext'  # Glob pattern for file matching
---

# [Topic] Instructions

## Purpose

[One paragraph explaining when and why these instructions apply.]

---

## Core Rules

### Naming Conventions

- [Convention 1]: [pattern/example]
- [Convention 2]: [pattern/example]
- [Convention 3]: [pattern/example]

### Code Patterns

```[language]
// ✅ Good - [explanation]
[good example code]

// ❌ Bad - [explanation]  
[bad example code]
```

### Structure Requirements

- [Requirement 1]
- [Requirement 2]
- [Requirement 3]

---

## File-Specific Guidance

### For [File Type 1]

[Specific rules for this file type]

### For [File Type 2]

[Specific rules for this file type]

---

## Best Practices

1. **[Practice Name]** - [Imperative statement]
2. **[Practice Name]** - [Imperative statement]
3. **[Practice Name]** - [Imperative statement]

---

## Common Mistakes

| Mistake | Correction |
|---------|------------|
| [Common error] | [How to fix] |
| [Common error] | [How to fix] |

---

## References

- [Link to related documentation]
- [Link to external standards]
```

## ApplyTo Pattern Examples

| Pattern | Matches |
|---------|---------|
| `**/*.py` | All Python files recursively |
| `**/*.ts,**/*.tsx` | All TypeScript files |
| `src/**/*.js` | JavaScript files under src/ |
| `**/test_*.py` | Python test files anywhere |
| `docs/**/*.md` | Markdown files under docs/ |
| `**` | All files (use sparingly) |

## Required Customizations

| Placeholder | Replace With |
|-------------|--------------|
| `[Topic]` | Subject area (e.g., "Python Coding", "React Components") |
| `applyTo` pattern | Glob matching target files |
| Core rules | Domain-specific conventions |
| Code examples | Real patterns from codebase |
| File-specific guidance | Variations by file type |

## Language Guidelines

Use **imperative language**:

| ❌ Weak | ✅ Strong |
|---------|----------|
| You should use... | Use... |
| Consider adding... | Add... |
| It's recommended to... | MUST include... |
| Try to avoid... | NEVER use... |

## Length Guidelines

- **Total file**: < 600 words
- **Each section**: 5-15 lines
- **Examples**: 5-10 lines each

## Quality Checklist

Before finalizing:

- [ ] `applyTo` pattern accurately matches target files
- [ ] Description is one clear sentence
- [ ] Rules are specific and actionable
- [ ] Examples show good AND bad patterns
- [ ] Language is imperative (MUST, NEVER, ALWAYS)
- [ ] No duplication with other instruction files

## References

- [prompts.instructions.md](../../instructions/prompts.instructions.md) - For prompt-specific instructions
- [agents.instructions.md](../../instructions/agents.instructions.md) - For agent-specific instructions
- [skills.instructions.md](../../instructions/skills.instructions.md) - For skill-specific instructions

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers: []
  changelog: "instructions.template.changelog.md"
---
-->
