---
description: "Guidance for context file structure and content organization"
---

<!-- Unique to this template: Copy-pasteable Markdown skeleton for new context files. Rules live in context-files.instructions.md; this provides the concrete scaffolding structure. -->

# Context File Template for Domain Guidance

Use this structure when generating new context files:

```markdown
# {Topic} for GitHub Copilot

**Purpose**: [Single sentence describing what this context provides]
**Referenced by**: [List of instruction files or prompts that use this]

---

## Overview
[Brief introduction to the topic]

## Key Concepts
[Core concepts with definitions]

## Detailed Guidelines
[Expanded guidance with examples]

## Examples
[Concrete examples from this repository or user context]

## Anti-Patterns
[What NOT to do, with explanations]

## Checklist
[Quick reference checklist for validation]

---

## References
[Source links]
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-createorupdate-prompt-guidance"
  changelog: "guidance-context-file.template.changelog.md"
---
-->
