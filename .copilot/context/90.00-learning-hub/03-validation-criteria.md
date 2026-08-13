---
title: "Learning Hub Validation Criteria"
description: "Defines validation criteria, quality thresholds, and content lifecycle standards specific to the Learning Hub — extending general article validation with repository-specific rules"
domain: "learning-hub"
goal: "Establish validation standards and quality gates that ensure articles meet Learning Hub publication requirements"
scope:
  covers:
    - "Repository-specific standards (draft, in-review, published, archived lifecycle)"
    - "Metadata requirements and validation history tracking"
    - "Series-specific validation (consistency, completeness, progression)"
    - "Quality thresholds by dimension (grammar, readability, structure, accuracy, link validity)"
    - "Content category-specific validation focus"
  excludes:
    - "General article writing standards (see article-writing.instructions.md)"
    - "Dual metadata block structure (see 02-dual-yaml-metadata.md)"
boundaries:
  - "MUST define only Learning Hub-specific thresholds above general standards"
  - "MUST NOT duplicate criteria from base documentation.instructions.md"
rationales:
  - "Repository-specific thresholds prevent quality drift across a growing documentation site"
  - "Category-specific validation focus ensures each content type meets its intended purpose"
---

# Learning Hub Validation Criteria

This document defines validation criteria and quality thresholds specific to the Learning Hub documentation site. These supplement the general validation criteria found in `.copilot/context/01.00-article-writing/02-validation-criteria.md`.

## Repository-Specific Standards

### Content Lifecycle

**Status Values:**
- **draft**: Initial creation, not validated
- **in-review**: Validations in progress
- **published**: Approved and live
- **archived**: Outdated but kept for reference

```
Draft → In-Review → Published → [Updates/Revisions] → Archived
```

### Metadata Requirements

**Complete Metadata:**
```yaml
✅ Title, author, dates filled
✅ Tags relevant and complete
✅ Status appropriate for content state
✅ Series info (if applicable)
✅ All validation sections updated
✅ Cross-references current
✅ Analytics computed
```

**Validation History:**
- Timestamps recent (< 7 days for critical validations)
- Models used noted
- Outcomes recorded
- Issues documented

### Series-Specific Validation

**Series Consistency:**
- Terminology consistent across articles
- Style and format uniform
- Cross-references working
- Non-redundant (or justified redundancy)
- Logical progression

**Series Completeness:**
- All promised topics covered
- Prerequisites clear
- Navigation functional
- Gaps between articles bridged

## Learning Hub Content Categories

Content follows the **7-category taxonomy** from the Learning Hub Documentation Taxonomy. See `01-domain-concepts.md` for full definitions.

| Category | Primary Validation Focus | Secondary Focus |
|----------|-------------------------|----------------|
| **Overview** | Clarity, completeness | Readability (Flesch 60-70) |
| **Getting Started** | Accuracy (steps must work) | Logical flow, gap analysis |
| **Concepts** | Logical coherence, completeness | Understandability |
| **How-to: Task Guides** | Fact accuracy, step completeness | Troubleshooting coverage |
| **How-to: Patterns** | Example quality, applicability | When to use / not use |
| **Analysis** | Source quality (📘📗 preferred) | Balanced perspective |
| **Reference** | Exhaustive accuracy, consistency | Format compliance |
| **Resources** | Currency, link validity | Audience indication |

### Quality Thresholds

| Dimension | Minimum Score | Target Score |
|-----------|---------------|-------------|
| Grammar | 90% | 95%+ |
| Readability (Flesch) | 50 | 60-70 |
| Structure | 85% | 95%+ |
| Fact accuracy | 95% | 100% |
| Link validity | 100% | 100% |

### Overview

**Required Sections:**
- What it is (one-paragraph definition)
- Key benefits (3-5 bullet points)
- Who should use it / When to use it
- When NOT to use it (limitations)
- Prerequisites for learning more

**Validation Focus:**
- No technical deep-dives (belongs in Concepts)
- Honest about limitations
- Reading time 3-5 minutes

### Getting Started (Tutorials)

**Required Sections:**
- Prerequisites (explicit, verifiable)
- What you'll build/learn
- Step-by-step instructions (numbered)
- Verification ("You should see...")
- Troubleshooting (common issues)
- Next steps

**Validation Focus:**
- Steps complete and reproducible
- No assumed knowledge gaps
- Environment requirements specified
- Quickstart (5-min) + Full Tutorial (30-60 min) tiers

### Concepts (Explanatory Content)

**Required Sections:**
- Key terms (defined clearly)
- Core principles (3-5 main ideas)
- How it works (conceptual explanation)
- Common misconceptions
- Related concepts (cross-references)

**Validation Focus:**
- No procedural content (belongs in How-to)
- Layered depth (Core → Architecture → Advanced)
- Connects to prior knowledge

### How-to Guides

**Required Sections:**
- Goal statement (one sentence)
- Prerequisites
- Core content (steps, patterns, or framework)
- Verification or success criteria
- When to use / When NOT to use

**Validation Focus by Subcategory:**
- **Task Guides:** Step completeness, troubleshooting coverage
- **Patterns & Practices:** Multiple examples, context variations
- **Techniques:** Before/after comparisons, measurable improvements
- **Methodology:** Decision points, coordination guidance

### Analysis

**Required Sections:**
- Context and scope (what decision is being addressed)
- Evidence base (sources supporting analysis)
- Key findings or classification
- Recommendations with rationale
- Review date and validity period

**Validation Focus by Subcategory:**
- **Technology Radar:** ADOPT/TRIAL/ASSESS/HOLD justified
- **Comparative:** Criteria explicit, recommendation clear
- **Strategy:** Options evaluated, decision factors documented
- **Trend:** Sources current, predictions qualified

### Reference

**Required Elements per Item:**
- Name/identifier
- Type/syntax
- Description (one sentence)
- Default value (if applicable)
- Constraints/valid values
- Example (minimal, illustrative)

**Validation Focus:**
- Exhaustive coverage
- Consistent format across items
- No explanatory content (link to Concepts)

### Resources

**Required Elements:**
- Clear categorization
- Brief description (1-2 sentences per resource)
- Audience indication (beginner/intermediate/advanced)
- Currency note (when last verified)

**Validation Focus:**
- No dumping links without context
- Resources verified as current
- Session Analyses include speaker, date, event metadata

### Project Documentation

**Required Sections:**
- Project overview
- Architecture decisions
- Implementation details
- Future considerations

**Validation Focus:**
- Current with actual codebase
- Links to relevant source files
- Updated when code changes

## Validation Caching Rules

### 7-Day Caching Policy

Skip validation if ALL conditions met:
- Previous validation outcome was "passed"
- Content unchanged since last validation
- Validation timestamp < 7 days old
- Same validation type requested

Re-validate if ANY condition true:
- Content has been modified
- Previous outcome was not "passed"
- Validation older than 7 days
- Technology/version has major update

### Cache Bypass Triggers

Always re-validate:
- `--force` flag specified
- Article status changing to "published"
- External links need verification
- Technology has security update

## Reference Classification (Learning Hub)

All references must use the classification system:

| Marker | Category | Trust Level |
|--------|----------|-------------|
| 📘 | Official | Highest |
| 📗 | Verified Community | High |
| 📒 | Community | Medium |
| 📕 | Unverified | Low/None |

See `.copilot/context/90.00-learning-hub/04-reference-classification.md` for complete rules.

## Quality Maintenance Schedule

### Quarterly Review

For all published content:
- Check fact currency
- Update version information
- Verify external links
- Assess if content needs refresh

### Annual Review

Comprehensive evaluation:
- Full validation run
- Gap analysis
- Audience relevance check
- Consider if content should be archived

### Event-Triggered Review

When external changes occur:
- Technology major version release
- Breaking API changes
- Security vulnerabilities discovered
- Official documentation restructured

## Cross-Reference Standards

### Related Articles

Each article should identify:
- **Prerequisites**: What readers should know first
- **Related**: Topics at similar complexity
- **Advanced**: Next-level concepts building on this

### Series Navigation

Articles in a series must have:
- Series name in metadata
- Previous/next article links
- Series index reference
- Consistent terminology with series

## IQPilot Integration

When IQPilot MCP server is enabled:
- Validation results cached in article metadata
- Automatic 7-day cache expiry
- 16 specialized tools available
- Cross-reference validation automated

See `getting-started.md` for IQPilot configuration.

<!--
context_metadata:
  version: "1.0.0"
  last_updated: "2026-05-26"
-->
