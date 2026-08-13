---
description: Rules for creating and maintaining template files used by prompts, agents, and skills
applyTo: '.github/**/*template*'
version: "1.4.0"
last_updated: "2026-03-20"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Template File Instructions

## Purpose

Template files define reusable output formats, input schemas, and document structures. Loaded on-demand via `📖` references — keeping them token-efficient and consistent across consumers.

## Severity Index

**CRITICAL** — block on failure:
- **[C3]** Token budget: templates ≤100 lines

**HIGH** — fix before use:
- **[H8]** Imperative language in agent-consumed templates
- **[H12]** Cross-reference integrity: all `📖` links resolve

**MEDIUM** — fix when convenient:
- **[M1]** Template externalization: split templates >100 lines
- **[M6]** Naming: `{category}-{artifact}-{purpose}.template.md`

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)

## Template Categories

| Prefix | Purpose | Design for |
|--------|---------|------------|
| `output-*` | Report/output formats | **Agent** — parsable structure |
| `input-*` | Input collection schemas | **User** — readable descriptions |
| `guidance-*` | Process guidance | **Agent** — imperative steps |
| `*-structure` | Artifact scaffolds | **Agent** — exact structure |
| `pattern-*` | Content patterns | **User** — readable prose |

## Rules

### Audience-aware design (MUST)
- **Agent-consumed**: parsable tables, `[placeholder]` markers, minimal prose
- **User-consumed**: natural language descriptions, examples, clear instructions
- **Both**: parsable structure with inline descriptions

### Content completeness (MUST)
- Input templates MUST include ALL fields the consumer needs
- Output templates MUST include ALL sections downstream consumers expect
- Never assume the consumer has context beyond the template

### Structural rules
- Use `[placeholder]` markers for fields to fill
- Keep templates under 100 lines — split if larger
- Use `.template.md` extension

### Location
- **Single consumer**: `.github/templates/{prompt-or-agent-name}/`
- **Area-shared**: `.github/templates/{area-name}/`
- **Skill-bundled**: `.github/skills/{skill-name}/templates/`
- **Cross-area**: `.github/templates/` (root)

## Bottom Metadata (REQUIRED)

Every template MUST include a bottom `template_metadata` HTML comment for version tracking and consumer traceability. This follows the **dual metadata pattern** (📖 `02-dual-yaml-metadata.md`):

- **Top YAML** — invariant discovery properties (`description:` only)
- **Bottom HTML comment** — variable tracking properties (`version`, `last_updated`, `consumers`, `changes`)

```html
<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "YYYY-MM-DD"
  created: "YYYY-MM-DD"
  consumers:
    - "consumer-name"
  changes:
    - "v1.0.0: Initial creation"
---
-->
```

Update `last_updated` and `version` on every modification. Keep `consumers` in sync with actual `📖` references.

## Quality Checklist

- [ ] Under 100 lines (C3)
- [ ] Audience-appropriate design (agent vs user)
- [ ] All placeholder fields present for consumer
- [ ] Naming follows category convention (M6)
- [ ] `📖` references resolve (H12)
- [ ] Bottom `template_metadata` block present with `version`, `last_updated`, `consumers`
- [ ] Top YAML includes `description:` field

## References

- **📖** [.copilot/context/00.00-prompt-engineering/](.copilot/context/00.00-prompt-engineering/) — PE context
