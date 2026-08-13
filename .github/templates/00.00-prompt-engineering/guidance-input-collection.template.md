---
description: "Input collection guidance for interactive prompt workflows"
---

<!-- Unique to this template: Interactive questionnaire for gathering domain guidance requirements from users. Not duplicated in any context or instruction file. -->

# Domain Guidance Input Collection Template

Use this template when user provides incomplete information for guidance file creation:

```
📥 DOMAIN GUIDANCE SETUP

To generate effective guidance, I need:

1. **Domain name**: What area does this guidance cover?
   Example: "article-writing", "code-review", "authentication"

2. **Target files**: What type of guidance file(s) to create?
   - [ ] Instruction file (.github/instructions/{domain}.instructions.md)
   - [ ] Context file (.copilot/context/{domain}/*.md)
   - [ ] Both

3. **Target paths**: Where should files be created?
   Default: `.github/instructions/{domain}.instructions.md`

4. **Context sources**: What content should inform this guidance?
   - Existing files in repository
   - External URLs (documentation, style guides)
   - Principles you want encoded

5. **Key principles**: What rules MUST the guidance enforce?
   - Required elements
   - Quality criteria
   - Anti-patterns to prevent

Please provide these details so I can generate appropriate guidance.
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-createorupdate-prompt-guidance"
  changelog: "guidance-input-collection.template.changelog.md"
---
-->
