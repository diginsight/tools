---
description: "Domain-specific examples for prompt engineering artifacts"
---

<!-- Unique to this template: Three concrete domain examples (article-writing, validation, code-review) showing MUST/WILL/NEVER rule patterns. Not duplicated in any context or instruction file. -->

# Domain Guidance Examples

Reference examples for common domain types:

## Article-Writing Domain

```markdown
# Example: article-writing.instructions.md

## Purpose
Guide prompts/agents that create, review, or update article content.

## Core Principles
- Articles MUST include title, date, author, and categories in YAML frontmatter
- Articles MUST follow reference classification (📘 Official, 📗 Verified, 📒 Community, 📕 Unverified)
- Articles WILL use clear heading hierarchy (H1 for title, H2 for sections)
- Articles NEVER include unverified claims without citation
```

## Validation Domain

```markdown
# Example: validation.instructions.md

## Purpose
Guide prompts/agents that validate content quality, structure, or accuracy.

## Core Principles
- Validation MUST check all required elements before approval
- Validation MUST provide specific feedback for each failed check
- Validation WILL use structured output format (pass/fail/warning)
- Validation NEVER approves content with critical errors
```

## Code-Review Domain

```markdown
# Example: code-review.instructions.md

## Purpose
Guide prompts/agents that review code for quality, security, and maintainability.

## Core Principles
- Reviews MUST check for security vulnerabilities before other concerns
- Reviews MUST provide actionable feedback with code examples
- Reviews WILL prioritize critical issues over style preferences
- Reviews NEVER approve code with failing tests
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-createorupdate-prompt-guidance"
  changelog: "guidance-domain-examples.template.changelog.md"
---
-->
