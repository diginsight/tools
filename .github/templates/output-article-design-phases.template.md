# Article Design and Creation - Phase Outputs Template

Use these output formats when running the `article-design-and-create.prompt.md` workflow.

---

## Phase 1: Requirements Summary Output

```markdown
## Article Requirements

### Core Requirements
- **Topic**: [Full topic description]
- **Scope**: [What article will cover specifically]
- **Target Audience**: [beginner/intermediate/advanced]
- **Template**: [template-name.md]

### Structure Requirements
**Outline** (if provided):
- [Section 1]
- [Section 2]
- ...

**Or**: Outline to be generated from research (Phase 3)

### Special Requirements
- [User-specified requirements, if any]
- [Must-include examples or sections]
- [Related workspace articles to reference]

### Constraints
- **Length target**: [word count, if specified]
- **Focus areas**: [specific emphasis, if specified]
- **Exclusions**: [topics to avoid, if specified]

Proceed with Phase 2? (yes/no)
```

---

## Phase 2: Workspace Context Discovery Output

```markdown
## Workspace Context Discovery

### Related Articles Found
- **[Article Title]** (`path/to/article.md`) - [Relationship: duplicate/prerequisite/related/advanced]
- ...

### Integration Strategy
- **Link as prerequisites**: [articles]
- **Link as related reading**: [articles]
- **Avoid duplicating**: [topics already covered elsewhere]
- **Reference in series**: [if part of learning path]

### Template and Conventions
- **Using template**: `.github/templates/[template-name.md]`
- **Metadata structure**: [confirmed from 02-dual-yaml-metadata.md]
- **Repository conventions**: [any special formatting rules]

Proceed with Phase 3? (yes/no)
```

---

## Phase 3: Comprehensive Topic Research Output

```markdown
## Comprehensive Topic Research

### Core Topic: [Topic Name]

**Primary Official Sources:**
- **[Title]** - [URL] - [What it covers]
- ...

**Key Concepts Identified:**
- [Concept 1]: [Brief description]
- [Concept 2]: [Brief description]
- ...

**Current State:**
- Latest version: [version number]
- Recent changes: [summary of last 6-12 months]
- Deprecations: [any deprecated features to note]

### Adjacent Topics Discovered

**High Relevance (Should Include):**
- **[Adjacent Topic]**
  - Source: [URL/workspace file]
  - Rationale: [Why this belongs in article]
  - Suggested placement: [Where to integrate]

**Medium Relevance (Consider Mentioning):**
- **[Adjacent Topic]**
  - Source: [URL]
  - Rationale: [Connection to main topic]
  - Suggested treatment: [Brief mention + link / Separate section]

**Low Relevance (Defer):**
- **[Topic]**: [Why deferring]

### Alternatives Discovered

**For [Core Technology/Approach]:**

1. **[Alternative Name]**
   - **Use case fit**: [When to consider this option]
   - **Key differences**: [How it differs from main approach]
   - **Trade-offs**: [Pros and cons]
   - **Source**: [URL]
   - **Suggested treatment**: [Brief mention in body / Comparison appendix]

### Community Patterns & Best Practices
- [Pattern 1]: [Description and source]
- [Pattern 2]: [Description and source]

### Reference URL List
**All URLs discovered (will be classified in Phase 4):**
- [URL 1] - [Title] - [Description]
- [URL 2] - [Title] - [Description]

Proceed with Phase 4? (yes/no)
```

---

## Phase 4: Reference Verification & Classification Output

```markdown
## Reference Verification & Classification

### Verification Results
- **Total URLs checked**: [count]
- **Valid**: [count]
- **Broken/Inaccessible**: [count]
- **Redirected**: [count]

### Classified References (Ready for Article)

#### Official Documentation
**[Title](url)** `[📘 Official]`  
[Description explaining what it covers and why it's valuable]

#### Community Resources
**[Title](url)** `[📗 Verified Community]`  
[Description]

**[Title](url)** `[📒 Community]`  
[Description]

#### Examples and Repositories
**[Title](url)** `[📘 Official]` / `[📒 Community]`  
[Description]

### Broken References (Excluded)
- [URL] - [Error: 404 / timeout / etc]

Proceed with Phase 5? (yes/no)
```

---

## Phase 5: Article Structure Design Output

```markdown
## Article Structure Design

### Target Audience Considerations
- **Level**: [beginner/intermediate/advanced]
- **Assumed knowledge**: [What readers should already know]
- **Learning objectives**: [What readers will learn]

### Article Outline

**1. Introduction**
- What this article covers
- Why it matters
- Prerequisites (with links)
- What readers will learn

**2. [Main Section 1: Core Concept]**
- [Subsection 1.1]
- [Subsection 1.2]
- Code example: [description]

**3. [Main Section 2: Core Feature/Technique]**
- [Subsection 2.1]
- [Brief mention of Alternative X with link to Appendix]

**4. [Main Section 3: Advanced/Integration Topic]**
- [Content from high-relevance adjacent topics]

**5. Conclusion**
- Key takeaways recap
- Related articles links
- Next steps suggestions

**6. References**
- [Classified references from Phase 4]

**7. Appendices** (if applicable)
- **Appendix A: [Alternative X] Comparison**

### Content Integration Plan
- **Core concepts**: [Where each Phase 3 concept appears]
- **Adjacent topics**: [Integration points]
- **Alternatives**: [Brief mention in Section X, detailed in Appendix A]
- **Code examples**: [Planned examples and their sections]

Proceed with Phase 6? (yes/no)
```

---

## Phase 6: Article Creation Summary Output

```markdown
## Article Creation Complete

### Metadata
- **Filename**: [suggested-filename.md]
- **Word count**: [count]
- **Reading time**: [minutes]
- **Categories**: [list]

### Structure Summary
- Sections: [count]
- Code examples: [count]
- References: [count] (📘 Official: X, 📗📒 Community: Y)
- Appendices: [count]
- Internal links: [count]

### Quality Checklist
- ✅ Both YAML metadata blocks included
- ✅ All references verified and classified
- ✅ Table of Contents with working links
- ✅ Code examples with explanations
- ✅ Alternatives discovered and documented
- ✅ Related workspace articles linked
- ✅ Proper heading hierarchy
- ✅ Conclusion with next steps

### Next Steps for User
1. **Review content** for accuracy and voice
2. **Save article** to appropriate workspace location
3. **Run validation prompts** (grammar, readability, structure, facts, logic)
4. **Review validation results** and refine as needed
5. **Publish** when all validations pass

---

[FULL ARTICLE CONTENT BELOW]
```

---

## Article Dual YAML Metadata Structure

**Top YAML Block** (frontmatter metadata - lines 1-10):
```yaml
---
title: "[Article Title]"
author: "[Author Name]"
date: "[YYYY-MM-DD]"
categories: [category1, category2]
description: "[SEO-friendly description, 150-160 chars]"
---
```

**Bottom YAML Block** (Validation metadata - HTML comment at end):
```markdown
<!-- 
---
validations:
  grammar: {last_run: null, model: null, outcome: null, issues_found: 0}
  readability: {last_run: null, model: null, outcome: null, flesch_score: null, grade_level: null}
  understandability: {last_run: null, model: null, outcome: null, target_audience: null}
  structure: {last_run: null, model: null, outcome: null, has_toc: true, has_introduction: true, has_conclusion: true, has_references: true}
  facts: {last_run: null, model: null, outcome: null, claims_checked: 0, sources_verified: 0}
  logic: {last_run: null, model: null, outcome: null, flow_score: null}

article_metadata:
  filename: "[suggested-filename.md]"
  created: "[YYYY-MM-DD]"
  last_updated: "[YYYY-MM-DD]"
  version: "1.0"
  status: "draft"
  word_count: [count]
  reading_time_minutes: [estimate]
  primary_topic: "[topic]"

cross_references:
  related_articles: []
  series: null
  prerequisites: []
---
-->
```

---

## Quality Standards Checklist

### Completeness Checklist
- [ ] Topic researched comprehensively (official docs + community + workspace)
- [ ] Adjacent topics discovered and integrated appropriately
- [ ] Alternatives identified and documented (body + appendix)
- [ ] All references verified and classified
- [ ] Both YAML metadata blocks included and properly formatted
- [ ] Table of Contents with working anchor links
- [ ] Code examples with syntax highlighting and explanations
- [ ] Practical examples/use cases included
- [ ] Related workspace articles linked (from Phase 2)
- [ ] Introduction includes prerequisites and learning objectives
- [ ] Conclusion includes key takeaways and next steps
- [ ] References organized by category with descriptions

### Content Quality Checklist
- [ ] Clear, concise writing (Grade 9-10 level)
- [ ] Active voice, short paragraphs
- [ ] Proper heading hierarchy (H1 > H2 > H3)
- [ ] All claims cited with authoritative sources
- [ ] Current information (version numbers, features checked)
- [ ] No duplication of existing workspace content
- [ ] Audience-appropriate depth and terminology
