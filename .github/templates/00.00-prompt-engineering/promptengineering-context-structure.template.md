---
description: "Structural template for PE context file scaffolding"
---

# Context File Structure Template (Prompt Engineering Domain)

Use this structure when creating or updating context files for prompt engineering patterns:

```markdown
# [Topic] for GitHub Copilot

**Purpose**: [Single sentence]
**Referenced by**: [List of files that use this]

---

## [Section 1]
[Content with tables, examples, code blocks]

## [Section 2]
[Content]

---

## References
[Source links]
```

## Required Sections Checklist

- [ ] Title formatted as "{Topic} for GitHub Copilot"
- [ ] Purpose statement (single sentence)
- [ ] Referenced by list (which instruction files or prompts use this)
- [ ] Multiple content sections with clear headings
- [ ] References section with source links

## Content Guidelines

- Use tables for structured comparisons
- Include code examples where applicable
- Provide specific examples from this repository
- Keep sections focused and scannable
- Use proper Markdown formatting (backticks for code, bold for emphasis)

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-createorupdate-promptengineering-guidance"
  changelog: "promptengineering-context-structure.template.changelog.md"
---
-->
