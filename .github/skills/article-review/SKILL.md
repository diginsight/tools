---
name: article-review
description: >
  Validation checklists and output templates for technical documentation
  quality review. Used by documentation-validator agent for systematic
  article review. Also usable directly for quick manual self-checks.
  Use when reviewing markdown articles, checking reference classifications,
  or preparing content for publication.
---

# Article Review Skill

## Purpose

Provide reusable validation checklists, output templates, and troubleshooting guidance for technical documentation quality review. Eliminates duplication of validation criteria across documentation agents and manual review workflows.

## Relationship to Documentation Validator

This skill provides the **validation resources** (checklists, templates, format guides). The `documentation-validator` agent is the **execution engine** that uses these resources to perform deep 7-dimension validation with active tools (`fetch_webpage`, readability scoring, series-level analysis).

| Concern | This skill | documentation-validator agent |
|---------|-----------|-------------------------------|
| **Role** | "What to check" — portable checklists and templates | "Who checks and how deep" — execution with tools and triad handoffs |
| **Depth** | Checklist-level (pass/fail per item) | Quantitative (Flesch scores, severity-scored findings with line references) |
| **Tools** | None — pure instructions and patterns | `read_file`, `grep_search`, `fetch_webpage`, etc. |
| **Triad** | Standalone resource | Part of researcher→builder→validator triad |

**When to use which:**
- **Quick manual self-check** → Use this skill's checklists directly
- **Full quality validation** → Use `@documentation-validator` (which loads this skill's resources)
- **Orchestrated review** → Use `/documentation-review` prompt (delegates to validator agent)

## When to Use

Activate this skill when:
- **Quick structure check**: "Does this article have all required sections?"
- **Reference formatting**: "How should I format this reference entry?"
- **Pre-submission checklist**: "Is this article ready for publication?"
- **Review output format**: "What format should a review summary use?"

Do NOT use this skill for:
- Full 7-dimension quality validation (use `@documentation-validator`)
- Grammar or readability scoring (use `@documentation-validator`)
- Active link verification (use `@documentation-validator` with `fetch_webpage`)
- Creating new articles (use `@documentation-builder`)
- Code review or security auditing

## Validation Resources

### Checklists

| Resource | Purpose | Use when |
|----------|---------|----------|
| [Publication-Ready Checklist](./checklists/publication-ready.md) | Complete pre-publication validation — structure, metadata, references | Before publishing or during Phase 1 structural review |

### Templates

| Resource | Purpose | Use when |
|----------|---------|----------|
| [Review Summary](./templates/review-summary.md) | Standard output format for review results — per-dimension scores, reference status, quality ratings | Generating review reports (Phase 5 output) |
| [Reference Entry](./templates/reference-entry.md) | Correct reference formatting with emoji classification and domain guide | Adding or fixing references |

## Common Issues

### Issue: Missing Reference Classification

**Symptom**: References listed without emoji markers  
**Solution**: Add appropriate marker based on source domain. See [Reference Entry template](./templates/reference-entry.md) for domain classification guide.

### Issue: Top YAML Modified by Validation

**Symptom**: Frontmatter metadata changed unexpectedly  
**Solution**: Validation should ONLY modify bottom HTML comment metadata. Restore top YAML from git history.

### Issue: Broken Internal Links

**Symptom**: Links to other articles return 404  
**Solution**: Use relative paths. Verify target file exists. Check for renamed files.

## Resources

- 📖 `.copilot/context/01.00-article-writing/02-validation-criteria.md` — 7 validation dimensions and thresholds
- 📖 `.copilot/context/90.00-learning-hub/04-reference-classification.md` — Reference classification rules
- 📖 `.copilot/context/90.00-learning-hub/02-dual-yaml-metadata.md` — Dual YAML metadata pattern
- 📖 `.github/instructions/documentation.instructions.md` — Documentation base rules
