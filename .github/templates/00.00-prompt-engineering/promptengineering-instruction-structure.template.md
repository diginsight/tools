---
description: "Structural template for PE instruction file scaffolding"
---

# Instruction File Structure Template (Prompt Engineering Domain)

Use this structure when creating or updating instruction files for prompt/agent/skill guidance:

```markdown
---
description: "One-sentence purpose"
applyTo: 'glob/pattern/**/*.md'
---

# [Title]

## Purpose
[One paragraph explaining file's role]

## Context Engineering Principles
**📖 Complete guidance:** `.copilot/context/00.00-prompt-engineering/01.01-context-engineering-principles.md`
[Summary of key principles - reference don't duplicate]

## Tool Selection
**📖 Complete guidance:** `.copilot/context/00.00-prompt-engineering/02-tool-composition-guide.md`
[Summary of tool/agent alignment]

## Required YAML Frontmatter
[Template with required fields]

## Templates
[Reference to .github/templates/]

## Repository-Specific Patterns
[Validation caching, dual YAML, etc.]

## Best Practices
[5-7 actionable items]

## References
[Links to sources]
```

## Required Sections Checklist

- [ ] YAML frontmatter with `description` and `applyTo` fields
- [ ] Purpose paragraph
- [ ] Context Engineering Principles (reference, don't embed)
- [ ] Tool Selection guidance (reference, don't embed)
- [ ] Required YAML Frontmatter section
- [ ] Templates section with references
- [ ] Repository-Specific Patterns
- [ ] Best Practices (5-7 items)
- [ ] References section with source links

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-createorupdate-promptengineering-guidance"
  changelog: "promptengineering-instruction-structure.template.changelog.md"
---
-->
