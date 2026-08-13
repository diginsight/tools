---
description: "Guidance for instruction file structure and applyTo patterns"
---

<!-- Unique to this template: Copy-pasteable Markdown skeleton with YAML frontmatter for new instruction files. Rules live in instruction-files.instructions.md; this provides the concrete scaffolding structure. -->

# Instruction File Template for Domain Guidance

Use this structure when generating new instruction files:

```markdown
---
description: "{Domain} instructions for GitHub Copilot prompts and agents"
applyTo: '{glob-pattern}'
---

# {Domain} Instructions

## Purpose
[One paragraph explaining what prompts/agents using this guidance should accomplish]

## Scope
**IN SCOPE:** [What this guidance covers]
**OUT OF SCOPE:** [What this guidance does NOT cover]

## Core Principles
[Domain-specific principles extracted from user context, written in MUST/WILL/NEVER language]

### Required Elements
[What MUST be included in outputs]

### Quality Criteria
[How to measure success]

### Anti-Patterns
[What to NEVER do]

## Three-Tier Boundaries
### ✅ Always Do (No Approval Needed)
[Actions prompts/agents can take without asking]

### ⚠️ Ask First (Require Confirmation)
[Actions that require user approval]

### 🚫 Never Do
[Prohibited actions]

## Context References
**📖 Complete guidance:** `.copilot/context/{domain}/*.md`
[Summary of what each context file provides]

## Best Practices
[5-7 actionable items specific to this domain]

## References
[Source links: user-provided URLs, official docs, repository patterns]
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-createorupdate-prompt-guidance"
  changelog: "guidance-instruction-file.template.changelog.md"
---
-->
