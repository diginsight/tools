# Publication Ready Checklist

Complete checklist for validating articles before publication.

## Structure Requirements

### Required Sections

- [ ] **Title (H1)** - Single H1 at document start
- [ ] **Introduction** - Explains topic and what readers will learn
- [ ] **Body** - Clear section headings (H2, H3 hierarchy)
- [ ] **Conclusion** - Summarizes key points
- [ ] **References** - All sources cited with classification

### Conditional Sections

- [ ] **Table of Contents** - Required if article > 500 words
- [ ] **Prerequisites** - If article requires prior knowledge
- [ ] **Next Steps** - If part of a series

## Metadata Requirements

### Top YAML Block (frontmatter)

```yaml
---
title: "Required"
author: "Required"
date: "Required (YYYY-MM-DD)"
categories: [Required]
description: "Required"
---
```

- [ ] All required fields present
- [ ] Date in correct format
- [ ] Categories are valid
- [ ] Description is concise (< 200 chars)

### Bottom HTML Comment (Validation)

- [ ] Present at end of file
- [ ] `article_metadata.filename` matches actual filename
- [ ] `created` date is set
- [ ] `last_updated` reflects most recent change

## Reference Requirements

### Classification

- [ ] All references have emoji markers
- [ ] No 📕 (unverified) markers in final version
- [ ] Official sources marked 📘
- [ ] Community sources marked 📗 or 📒

### Format

- [ ] Format: `**[Title](url)** \`[📘 Type]\``
- [ ] Description follows each reference (2-4 sentences)
- [ ] Links tested and working
- [ ] Grouped by category (Official, Community, Internal)

## Content Quality

### Writing Style

- [ ] Active voice preferred
- [ ] Sentences 15-25 words (average)
- [ ] Paragraphs 3-5 sentences
- [ ] No jargon without explanation

### Formatting

- [ ] Proper heading hierarchy (H1 → H2 → H3)
- [ ] `backticks` for inline code, filenames, commands
- [ ] **Bold** for emphasis
- [ ] *Italic* for definitions
- [ ] `<mark>` tags for key terms/concepts

### Code Examples

- [ ] Language identifiers on all code blocks
- [ ] Code tested and functional
- [ ] Comments explain non-obvious parts
- [ ] Consistent style with article guidelines

### Links

- [ ] Descriptive link text (not "click here")
- [ ] Internal links use relative paths
- [ ] External links tested
- [ ] Alt text for all images

## Validation Runs

### Before First Review

- [ ] Grammar review completed
- [ ] Readability review completed
- [ ] Structure validation passed

### Before Publication

- [ ] Fact-checking completed (technical claims)
- [ ] All links verified working
- [ ] Cross-references updated
- [ ] Validation metadata timestamps current

## Final Sign-Off

| Check | Status | Date | Reviewer |
|-------|--------|------|----------|
| Structure | ⬜ | | |
| References | ⬜ | | |
| Grammar | ⬜ | | |
| Technical Accuracy | ⬜ | | |
| Publication Ready | ⬜ | | |

## Quick Reference

### Must Have (Publication Blockers)

1. Title, Introduction, Conclusion, References
2. Top YAML with all required fields
3. Reference classifications (no 📕)
4. Working links

### Should Have (Quality Gates)

1. Table of Contents (if >500 words)
2. Code examples with language IDs
3. Marked key terms
4. Validation metadata current

### Nice to Have (Polish)

1. Diagrams/images where helpful
2. Cross-references to related articles
3. Next steps section
4. Version history in metadata
