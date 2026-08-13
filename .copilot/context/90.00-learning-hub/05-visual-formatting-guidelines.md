---
title: "Visual Formatting Guidelines for Learning Hub"
description: "Defines criteria-based rules for when and how to use visual formatting — `<mark>` tags, emojis, and table of contents — to enhance readability without overuse"
domain: "learning-hub"
goal: "Provide evidence-based guidance for visual formatting that improves readability while preventing decoration overuse"
scope:
  covers:
    - "Mark tag (`<mark>`) usage: purpose, when to use, criteria, density guidelines"
    - "Emoji section markers for visual navigation"
    - "Table of contents for article structure navigation"
    - "Critical retention vs. general emphasis distinction"
    - "Density limits and overuse anti-patterns"
  excludes:
    - "General Markdown formatting (see documentation.instructions.md)"
    - "Typography and accessibility (see separate accessibility context)"
boundaries:
  - "MUST limit `<mark>` to critical retention content only"
  - "MUST NOT use visual formatting as decoration"
rationales:
  - "Density limits preserve highlighting effectiveness and visual hierarchy"
  - "Criteria-based rules prevent subjective formatting decisions"
---

# Visual Formatting Guidelines for Learning Hub

**Purpose**: Defines when and how to use visual formatting elements (`<mark>` tags, emojis, and table of contents) to improve readability and understandability in Learning Hub articles.

**Referenced by**: `.github/instructions/article-writing.instructions.md`, validation prompts, article templates

---

## Overview

Visual formatting elements enhance article scannability and comprehension when used purposefully. Overuse dilutes their impact. This guide provides **criteria-based rules** for three key elements:

1. **`<mark>` highlighting** — Drawing attention to critical concepts
2. **Emoji section markers** — Creating visual navigation anchors
3. **Table of Contents** — Enabling structured article navigation

---

## 1. Mark Tag (`<mark>`) Usage

### Purpose

The `<mark>` HTML tag creates a <mark>highlighted background</mark> that draws the reader's eye to critical information. Use it to signal "this is important to remember."

### When to Use `<mark>`

| ✅ Use `<mark>` for | ❌ Don't use `<mark>` for |
|---------------------|---------------------------|
| Key concepts readers must retain | General emphasis (use **bold** instead) |
| Critical warnings or constraints | Decorative highlighting |
| Essential definitions on first introduction | Every new term |
| "Remember this" takeaways | Code or technical syntax |
| Counter-intuitive facts that might be overlooked | Headers or titles |

### Criteria for `<mark>` Application

Apply `<mark>` when ALL of the following are true:

1. **Critical Retention**: The information is essential for understanding the rest of the article
2. **Non-Obvious**: The concept might be overlooked or underestimated by readers
3. **Limited Scope**: You're highlighting a phrase or short sentence (not paragraphs)
4. **Low Frequency**: Maximum 3-5 `<mark>` instances per 1000 words

### Examples

**✅ Good usage:**
```markdown
The dual metadata system uses <mark>two separate YAML blocks</mark>—one for 
rendering, one for validation tracking.

<mark>Never modify the top YAML block</mark> from validation prompts—it's 
reserved for renderer frontmatter.

Authentication tokens have a <mark>24-hour expiration</mark> regardless of 
activity.
```

**❌ Bad usage (overuse):**
```markdown
<mark>GitHub Copilot</mark> uses <mark>AI</mark> to provide 
<mark>code suggestions</mark>. It works with <mark>multiple languages</mark>
and supports <mark>various IDEs</mark>.
```

### Density Guidelines

| Article Length | Maximum `<mark>` Tags | Rationale |
|----------------|----------------------|-----------|
| < 500 words | 1-2 | Short content needs minimal highlighting |
| 500-1500 words | 3-5 | Standard article length |
| 1500-3000 words | 5-8 | Longer content can sustain more highlights |
| > 3000 words | 8-12 | Long-form needs navigation aids |

### Placement Strategy

1. **Introduction**: Mark the central thesis or key takeaway
2. **Body sections**: Mark one critical concept per major section
3. **Warnings/Constraints**: Always mark "never do X" or "always do Y" rules
4. **Conclusion**: Mark the most important summary point

---

## 2. Emoji Section Markers

### Purpose

Emojis create **visual anchors** that help readers scan and navigate documents quickly. They work like road signs—recognizable at a glance.

### When to Use Emojis

| ✅ Use emojis for | ❌ Don't use emojis for |
|-------------------|------------------------|
| Top-level section headers (H2) | Every heading level |
| Table of Contents entries | Body text or paragraphs |
| Quick navigation tables | Inline emphasis |
| Reference documents and READMEs | Formal technical specifications |
| Status indicators | Decorative purposes |

### Criteria for Emoji Application

Apply emojis when ALL of the following are true:

1. **Document Type**: Reference documentation, getting started guides, README files, or navigation-heavy content
2. **Header Level**: H2 or H1 only (never H3 or below)
3. **Consistency**: All H2 sections in the document should have emojis, or none should
4. **Semantic Meaning**: The emoji adds meaning, not just decoration

### Standard Emoji Vocabulary

Use consistent emoji associations across all Learning Hub content:

| Emoji | Meaning | Use For |
|-------|---------|---------|
| 🎯 | Goals/Focus | Objectives, purpose statements, key focuses |
| 📋 | Overview/List | Quick navigation, summaries, checklists |
| 📝 | Instructions | How-to sections, step-by-step guides |
| 🚀 | Quick Start | Getting started, setup instructions |
| ⚠️ | Warning | Important cautions, breaking changes |
| 🚨 | Critical | Critical issues, common problems |
| ✅ | Success/Do | Best practices, recommended actions |
| ❌ | Avoid/Don't | Anti-patterns, things to avoid |
| 🔧 | Configuration | Settings, configuration sections |
| 📦 | Components | Parts, elements, building blocks |
| 🔗 | References | External links, related resources |
| 📚 | Learning | Tutorials, educational content |
| 💡 | Tips | Pro tips, insights, recommendations |
| 🏗️ | Architecture | System design, structure explanations |
| 📊 | Metrics | Performance, analytics, measurements |
| 🛡️ | Security | Security considerations, authentication |
| ⏱️ | Performance | Speed, optimization, timing |
| 🔍 | Research | Analysis, investigation, deep dives |

### Examples

**✅ Good usage (README or reference doc):**
```markdown
## 🎯 Purpose

This tool validates article metadata...

## 📋 Prerequisites

Before you begin, ensure you have...

## 🚀 Quick Start

1. Install dependencies...

## 📝 Configuration

Configure the following settings...

## 🚨 Common Issues

### Issue 1: Missing metadata
```

**❌ Bad usage (overuse at all levels):**
```markdown
## 🎯 Introduction

### 📝 What is GitHub Copilot?

#### 💡 Key Features

##### 🔧 Configuration Options
```

### Document Type Guidelines

| Document Type | Emoji Usage | Rationale |
|---------------|-------------|-----------|
| README files | Required | Navigation aid for quick scanning |
| Getting Started guides | Recommended | Helps readers find sections quickly |
| Reference documentation | Recommended | Creates visual structure |
| Technical articles | Optional | May feel less formal—use judgment |
| Tutorials | Optional | Progress indicators can help |
| Deep-dive analyses | Avoid | Content should feel scholarly |

---

## 3. Table of Contents (TOC)

### Purpose

A TOC provides **structured navigation** for longer documents, helping readers:
- Preview article structure
- Jump to relevant sections
- Understand scope and depth

### When to Include TOC

| Criteria | TOC Required? |
|----------|---------------|
| Article > 500 words | ✅ Yes |
| Article has 4+ H2 sections | ✅ Yes |
| Tutorial with sequential steps | ✅ Yes |
| Short HowTo (< 500 words) | ❌ No |
| Single-topic focused article | ❌ Optional |

### TOC Placement Rules

1. **Position**: Immediately after the introduction paragraph (before first H2)
2. **Heading**: Use `## Table of Contents` or `## Contents`
3. **Format**: Bulleted list with markdown links
4. **Depth**: Include H2 and H3 levels only (not H4+)

### TOC Structure

```markdown
## Table of Contents

- [Section One](#section-one)
  - [Subsection 1.1](#subsection-11)
  - [Subsection 1.2](#subsection-12)
- [Section Two](#section-two)
- [Section Three](#section-three)
- [References](#references)
```

### Criteria for TOC Entries

Include in TOC:
- All H2 sections (mandatory)
- H3 sections if there are 2+ per H2 (optional but recommended)
- References section (always last)

Exclude from TOC:
- H4 and deeper headings
- Short sections (< 50 words)
- Appendix content

### Special TOC Patterns

**For tutorials with many steps:**
```markdown
## Table of Contents

- [Prerequisites](#prerequisites)
- [Steps](#steps)
  - [Step 1: Setup](#step-1-setup)
  - [Step 2: Configure](#step-2-configure)
  - [Step 3: Test](#step-3-test)
- [Troubleshooting](#troubleshooting)
- [References](#references)
```

**For reference documentation:**
```markdown
## Table of Contents

- [🎯 Overview](#-overview)
- [📋 Quick Reference](#-quick-reference)
- [📝 Detailed Usage](#-detailed-usage)
- [🚨 Common Issues](#-common-issues)
- [🔗 References](#-references)
```

### Auto-Generation Guidance

When prompts generate TOC automatically:
1. Extract all H2 and H3 headings
2. Generate anchor links (lowercase, hyphens for spaces)
3. Preserve emoji prefixes in links if present
4. Validate all anchor links are valid markdown anchors

---

## Integration with Validation

### Structure Validation Checks

The `structure-validation.prompt` should verify:

| Element | Validation Criteria |
|---------|---------------------|
| `<mark>` density | ≤ 5 per 1000 words |
| `<mark>` placement | Not in headings or code blocks |
| Emoji consistency | All H2 sections have emojis, or none do |
| Emoji vocabulary | Uses standard emoji associations |
| TOC presence | Required if article > 500 words |
| TOC completeness | All H2 sections included |
| TOC links | All anchor links valid |

### Readability Validation Checks

The `readability-review.prompt` should consider:

- `<mark>` tags used for genuinely critical content (not decoration)
- Emoji usage appropriate for document type
- TOC helps navigation (not just checkbox compliance)

---

## Quick Reference Card

### `<mark>` Checklist
- [ ] Is this truly critical information?
- [ ] Would a reader overlook this without highlighting?
- [ ] Am I under 5 per 1000 words?
- [ ] Is the highlighted text a short phrase (not a paragraph)?

### Emoji Checklist
- [ ] Is this a README, reference doc, or getting started guide?
- [ ] Am I only using emojis at H2 level?
- [ ] Are all H2s in this document consistent (all have emojis or none do)?
- [ ] Am I using the standard emoji vocabulary?

### TOC Checklist
- [ ] Is the article > 500 words?
- [ ] Does the article have 4+ H2 sections?
- [ ] Is the TOC placed after introduction, before first H2?
- [ ] Are all H2 sections included in TOC?
- [ ] Do all anchor links work correctly?

---

## References

- [Microsoft Writing Style Guide](https://learn.microsoft.com/en-us/style-guide/welcome/) — Formatting and emphasis guidelines
- [MDN: mark element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/mark) — HTML mark tag documentation
- [Unicode Emoji Chart](https://unicode.org/emoji/charts/full-emoji-list.html) — Complete emoji reference
- [Style Guide](../01.00-article-writing/01-style-guide.md) — General formatting standards

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0.0 | 2026-01-24 | Initial version | System |

<!--
context_metadata:
  version: "1.0.0"
  last_updated: "2026-05-26"
-->
