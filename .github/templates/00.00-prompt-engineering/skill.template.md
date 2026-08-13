---
description: "Template for creating SKILL.md files with progressive disclosure"
---

# Skill Template

Use this template to create new GitHub Copilot Agent Skills.

## Directory Structure

Create the following structure in `.github/skills/`:

```
.github/skills/your-skill-name/
├── SKILL.md                    # Required: Copy from template below
├── templates/                  # Optional: Reusable templates
│   └── example.template.ext
├── examples/                   # Optional: Real-world examples
│   └── example-usage.ext
├── scripts/                    # Optional: Automation scripts
│   └── setup.sh
└── README.md                   # Optional: Additional documentation
```

## SKILL.md Template

Copy the content below to `.github/skills/your-skill-name/SKILL.md`:

---

```markdown
---
# REQUIRED: Lowercase, hyphens only, max 64 characters
name: your-skill-name

# REQUIRED: Max 1024 characters
# Include: [What it does] + [Technologies] + "Use when" + [Scenarios]
description: >
  Brief description of what this skill does and the technologies involved.
  Use when [specific scenario 1], [specific scenario 2], or [specific scenario 3].
---

# Your Skill Name

<!-- 
  TIP: Use a clear, descriptive title that matches the skill's purpose.
  This title appears when Copilot references the skill.
-->

## Purpose

<!-- 
  REQUIRED: 1-2 sentences explaining the skill's goal.
  Keep it concise - detailed information goes in Workflow section.
-->

[One paragraph describing what this skill accomplishes and why it's useful.]

## When to Use

<!-- 
  REQUIRED: Help Copilot and users understand activation scenarios.
  Be specific about what triggers this skill.
-->

Activate this skill when:
- **[Scenario category 1]**: "[Example prompt phrasing]"
- **[Scenario category 2]**: "[Example prompt phrasing]"
- **[Scenario category 3]**: "[Example prompt phrasing]"

Do NOT use this skill for:
- [Related but different task - suggest alternative]
- [Out of scope task - suggest alternative]

## Workflow

<!-- 
  REQUIRED: The core instructions for the skill.
  Use numbered steps for procedures, bullet lists for options.
-->

### Step 1: [First Action]

[Description of what to do first]

```language
// Code example if applicable
```

### Step 2: [Second Action]

[Description of next step]

### Step 3: [Third Action]

[Continue until workflow is complete]

## Templates

<!-- 
  OPTIONAL: Reference templates included in the skill directory.
  Use relative paths from the SKILL.md location.
-->

Use these templates as starting points:

- **[Template Name](./templates/example.template.ext)** - [Brief description of when to use]
- **[Another Template](./templates/another.template.ext)** - [Brief description]

## Examples

<!-- 
  OPTIONAL: Reference real-world examples that demonstrate the skill.
  Examples should show completed work, not just templates.
-->

See these examples for common patterns:

- **[Basic Example](./examples/basic.ext)** - [What it demonstrates]
- **[Advanced Example](./examples/advanced.ext)** - [What it demonstrates]

## Scripts

<!-- 
  OPTIONAL: Reference automation scripts in the skill directory.
  Note: Scripts require user approval before execution.
-->

Run these scripts to automate setup:

- **[Setup Script](./scripts/setup.sh)** - [What it configures]

## Common Issues

<!-- 
  OPTIONAL: Document known problems and solutions.
  Helps users troubleshoot without additional support.
-->

### Issue: [Problem Description]

**Symptom**: [What the user sees]  
**Solution**: [How to fix it]

### Issue: [Another Problem]

**Symptom**: [What the user sees]  
**Solution**: [How to fix it]

## Resources

<!-- 
  OPTIONAL: External references for deeper learning.
  Use for official documentation, not skill-specific resources.
-->

- [Official Documentation](https://example.com/docs)
- [Related Guide](https://example.com/guide)
```

---

## Naming Conventions

### Skill Name (`name` field)

| ✅ Valid | ❌ Invalid | Reason |
|----------|-----------|--------|
| `webapp-testing` | `WebApp Testing` | No uppercase or spaces |
| `security-review` | `test` | Too generic |
| `kubernetes-deploy` | `my-super-long-skill-name-that-exceeds-64-characters` | Too long |

### Directory Name

Match the `name` field exactly:
- `name: webapp-testing` → `.github/skills/webapp-testing/`

## Description Best Practices

### Formula

```
[What it does] + [Technologies/tools] + "Use when" + [specific scenarios]
```

### Examples

**Good:**
```yaml
description: >
  Generate comprehensive test suites for React components using Jest and 
  React Testing Library. Use when writing unit tests, creating integration 
  tests, testing user interactions, or debugging test failures.
```

**Bad:**
```yaml
description: Helps with testing  # Too vague, no technologies, no scenarios
```

## File Path Rules

| ✅ Do | ❌ Don't |
|-------|---------|
| `[Template](./templates/test.js)` | `[Template](/home/user/project/templates/test.js)` |
| `[Example](./examples/basic.ts)` | `[Example](https://github.com/user/repo/examples/basic.ts)` |

## Length Guidelines

| Content | Recommended | Maximum |
|---------|-------------|---------|
| Description | 100-200 chars | 1024 chars |
| SKILL.md body | 500-1000 words | 1500 words |
| Templates | 20-50 lines | 100 lines |
| Examples | 30-80 lines | 150 lines |

## Checklist Before Committing

- [ ] `name` is lowercase with hyphens, ≤64 characters
- [ ] `description` includes technologies AND "Use when" scenarios
- [ ] All resource paths are relative (`./templates/`, `./examples/`)
- [ ] Templates are minimal and ready to use
- [ ] Examples demonstrate real-world patterns
- [ ] Tested activation with sample prompts
- [ ] Verified cross-platform compatibility (if using scripts)

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "skill-create-update"
    - "template-validator"
  changelog: "skill.template.changelog.md"
---
-->
