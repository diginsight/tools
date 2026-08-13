---
title: "Series planning workflow"
description: "Process for planning, creating, and maintaining multi-article series with cross-article consistency"
domain: "article-writing"
goal: "Define the lifecycle for multi-article series (define → structure → plan metadata → create → validate → publish → maintain) so that cross-article consistency is maintained from planning through maintenance"
scope:
  covers:
    - "When to create a series (breadth, depth, progression, audience criteria)"
    - "Series lifecycle (7 phases from define to maintain)"
    - "Diátaxis category planning (coverage matrix, concentration thresholds)"
    - "Folder structure and terminology planning"
    - "Cross-reference strategy selection (5 strategies)"
    - "Worked example (REST API Documentation Series)"
    - "Common series patterns"
    - "Series consistency checklist"
  excludes:
    - "Single-article creation (see workflows/01-article-creation-workflow.md)"
    - "Single-article review (see workflows/02-review-workflow.md)"
    - "Series-level validation dimensions and thresholds (see 02-validation-criteria.md)"
boundaries:
  - "MUST reference actual prompt files — no phantom/placeholder names"
  - "MUST NOT redefine series-level validation dimensions — reference 02-validation-criteria.md"
  - "Category coverage matrix and terminology maps use YAML template format for machine readability"
rationales:
  - "Series planning prevents inconsistency that emerges when articles are written independently without shared terminology, structure, or progression planning"
  - "Diátaxis category planning with concentration thresholds ensures series don't become monolithic (all tutorials) or fragmented (random mix)"
  - "The worked example (REST API series) was added because abstract rules without concrete application leave planners uncertain about how to apply them"
  - "Cross-reference strategy selection table was added because different series structures need different linking patterns (linear navigation vs. hub-spoke vs. matrix)"
---

# Series Planning Workflow

**Purpose**: Process for planning, creating, and maintaining multi-article series with cross-article consistency.

**Referenced by**:
- `.github/prompts/01.00-article-writing/article-review-series-for-consistency-gaps-and-extensions.prompt.md`

---

## When to Create a Series

A topic warrants a series when:
- **Breadth**: Topic has 3+ distinct subtopics requiring separate articles
- **Depth**: Single article would exceed 3,000 words or 20+ minutes read time
- **Progression**: Natural learning path from foundational to advanced
- **Audience**: Different subtopics serve different skill levels

---

## Series Lifecycle

```
Define → Structure → Plan Metadata → Create Articles → Validate Series → Publish → Maintain
```

### Phase 1: Define

Establish the series boundaries:

| Decision | Question to Answer |
|----------|-------------------|
| **Scope** | What does this series cover? What does it NOT cover? |
| **Audience** | Who is reading this? What do they already know? |
| **Outcome** | What can the reader do after completing the series? |
| **Size** | How many articles? (Recommended: 5–12) |
| **Type** | Foundational / Tutorial / Deep Dive (see patterns below) |

### Phase 2: Structure

Map articles with explicit dependencies:

```yaml
# Series structure template
series:
  title: "[Series Name]"
  type: foundational | tutorial | deep-dive
  articles:
    - number: "00"
      title: "Introduction and Overview"
      prerequisites: none
      concepts_introduced: [list key concepts]
    - number: "01"
      title: "[First Topic]"
      prerequisites: ["00"]
      concepts_introduced: [list]
    # ... continue for each article
```

**Dependency rules:**
- Every concept MUST be introduced before it's used
- Article 00 MUST provide a series overview and reading guide
- Maximum 2 prerequisites per article (keep paths short)
- Allow parallel reading tracks where possible

**Content-first design step:**
Before structuring, answer these three questions:
1. What content already exists in the workspace on this topic? (Use `semantic_search` to check.)
2. Who needs it? Define the primary audience and skill level for each article.
3. What's the minimum viable series that covers all audience needs without duplication?

**Terminology handoff rule:**
For each concept, identify the article that introduces it. All subsequent articles MUST use the same term without redefining it. Maintain a terminology map:

```yaml
# Terminology map — add to series planning notes
terminology:
  - term: "progressive disclosure"
    introduced_in: "02"
    definition: "Layering content from surface to detail to expert depth"
  - term: "Diátaxis"
    introduced_in: "00"
    definition: "Four-type documentation framework: tutorial, how-to, reference, explanation"
```

**Diátaxis category planning:**
For each planned article, assign a Diátaxis type. Build a category coverage matrix alongside the structure template:

```yaml
# Category coverage matrix — add to series planning notes
categories:
  concept:  # Explanation articles
    - "00-overview.md"
    - "01-foundations.md"
  howto:    # How-to guides
    - "02-basic-setup.md"
  tutorial: # Step-by-step tutorials
    - "03-first-project.md"
  reference: # Lookup content
    - "04-api-reference.md"
```

**Category validation criteria:**
- A series with 5+ articles SHOULD have at least 2 Diátaxis types
- A series with 10+ articles SHOULD have at least 3 Diátaxis types
- If a series has only how-to articles, ask: "Are there concepts worth explaining? Are there patterns worth documenting as references?"
- No single type should exceed 60% of the series without justification

📖 **Pass/fail thresholds:** `.copilot/context/01.00-article-writing/02-validation-criteria.md` → "Series-Level Validation Dimensions"

**Folder structure planning:**
Map category names to folders BEFORE creating articles. Use the same naming convention as the series structure template. This prevents the drift that occurs when articles are created and placed ad-hoc.

```yaml
# Folder structure — add to series planning notes
folders:
  "{{series-root}}/01-concepts/":  # Explanation articles
    - "00-overview.md"
    - "01-foundations.md"
  "{{series-root}}/02-howto/":      # How-to guides
    - "02-basic-setup.md"
  "{{series-root}}/03-tutorials/":   # Step-by-step tutorials
    - "03-first-project.md"
  "{{series-root}}/04-reference/":   # Lookup content
    - "04-api-reference.md"
```

**Folder rules:**
- Each folder should align with one Diátaxis type
- Create folders BEFORE writing articles (don't add folders retroactively)
- If an article doesn't fit any existing folder, reconsider its Diátaxis type assignment

**Cross-reference strategy selection:**
Choose your cross-referencing approach for each article and apply it consistently:

| Strategy | When to use | Example |
|----------|-------------|---------|
| Inline references | Concept first mentioned outside its home article | "...as explained in [Article 02](link)..." |
| See Also blocks | Related but not prerequisite content | `> **See also:** [Article on JWT](link)` |
| Prerequisites sections | Required prior reading | `**Prerequisites:** Complete [Article 01](link)` |
| Next Steps links | Natural continuation | `**Next:** [Article 03: Advanced patterns](link)` |
| Hub-and-spoke navigation | Deep Dive series with independent articles | Overview links to all spokes; spokes link back to hub |

### Phase 3: Plan Metadata Consistency

Ensure uniform metadata across all articles in the series:

| Metadata Field | Consistency Rule |
|---------------|-----------------|
| `categories` | Include shared series category + article-specific |
| `date` | Sequential publication dates |
| `author` | Same author (or explicitly noted) |
| Reference format | Same emoji classification style |
| Heading style | Same H2 emoji pattern throughout series |

### Phase 4: Create Articles

For each article:
1. Draft using `article-design-and-create.prompt.md`
2. Include series navigation (previous/next links)
3. Cross-reference related articles in the series
4. Validate individually before series-level validation

**Series navigation template** (add to each article's introduction):
```markdown
> **Series**: [Series Name] — Article N of M
> **Previous**: [Previous Title](link) | **Next**: [Next Title](link)
```

### Phase 5: Validate Series Consistency

Run `article-review-series-for-consistency-gaps-and-extensions.prompt.md` which performs:
- Terminology consistency check across all articles
- Prerequisite coverage verification
- Gap analysis for missing topics
- Cross-reference validation
- Progressive complexity verification

**Key checks:**
- [ ] No concept used before introduced in the series
- [ ] Terminology consistent (same term for same concept throughout)
- [ ] No contradictory guidance between articles
- [ ] Cross-references valid and bidirectional
- [ ] Reading level progression is smooth (not jarring jumps)
- [ ] Each article stands alone for readers who arrive via search

### Phase 6: Publish

1. Publish articles in order (00 first)
2. Navigation updates automatically — the runtime builder surfaces each article (no config to edit)
3. Validate all cross-links work
4. Update series overview (article 00) with final article list

### Phase 7: Maintain

- Review the full series on the same schedule (don't review articles independently)
- When updating one article, check cross-references in adjacent articles
- When adding a new article, re-run series consistency validation
- Track series-level freshness: if >50% of articles are stale, schedule full series review

---

## Worked Example: REST API Documentation Series

This compact example shows the planning workflow applied to a concrete series.

### Phase 1 output: Define

| Decision | Answer |
|----------|--------|
| **Scope** | Building and documenting REST APIs with .NET—covers design through deployment. Excludes GraphQL and gRPC. |
| **Audience** | .NET developers (intermediate); familiar with C# but new to API design best practices |
| **Outcome** | Reader can design, build, document, test, and deploy a production-ready REST API |
| **Size** | 7 articles |
| **Type** | Tutorial series with reference support |

### Phase 2 output: Structure + categories

```yaml
series:
  title: "Building REST APIs with .NET"
  type: tutorial
  articles:
    - { number: "00", title: "Series overview and API design principles", type: explanation, prerequisites: none }
    - { number: "01", title: "Project setup and first endpoint",          type: tutorial,    prerequisites: ["00"] }
    - { number: "02", title: "Data validation and error handling",        type: tutorial,    prerequisites: ["01"] }
    - { number: "03", title: "Authentication with JWT",                   type: how-to,      prerequisites: ["01"] }
    - { number: "04", title: "Testing strategies for APIs",               type: how-to,      prerequisites: ["02"] }
    - { number: "05", title: "API versioning and deprecation",            type: explanation, prerequisites: ["00"] }
    - { number: "06", title: "Endpoint reference",                        type: reference,   prerequisites: none }

categories:
  explanation: ["00", "05"]   # 2 articles (29%)
  tutorial:    ["01", "02"]   # 2 articles (29%)
  how-to:      ["03", "04"]   # 2 articles (29%)
  reference:   ["06"]         # 1 article  (14%)
  # ✅ 4 Diátaxis types, no type >29% — exceeds minimum for 7-article series

folders:
  "rest-api-dotnet/concepts/":   ["00", "05"]
  "rest-api-dotnet/tutorials/":  ["01", "02"]
  "rest-api-dotnet/howto/":      ["03", "04"]
  "rest-api-dotnet/reference/":  ["06"]

terminology:
  - { term: "resource",           introduced_in: "00", definition: "Any object the API exposes (users, orders, products)" }
  - { term: "content negotiation", introduced_in: "00", definition: "Client-server agreement on response format via Accept headers" }
  - { term: "middleware pipeline", introduced_in: "01", definition: ".NET request processing chain (auth → validation → handler)" }
```

### Key planning decisions illustrated

- **Articles 03 and 04 are how-to** (not tutorial) because they solve specific tasks readers may arrive at via search, independent of the series order
- **Article 05 parallels Article 00** — both explanation type but 05 requires no prerequisites, enabling a practitioner who already built their API to jump straight to versioning guidance
- **Article 06 (reference)** has no prerequisites — it's a lookup resource usable independently
- **Cross-reference strategy:** inline references for prerequisite concepts; "Next Steps" links for series progression; Article 06 uses hub-and-spoke (all articles link to it for endpoint details)

---

## Common Series Patterns

### Foundational Series
**Purpose**: Build knowledge from ground up.
**Structure**: Linear progression, each article builds on previous.
**Example**: Technical writing (00-Introduction → 01-Readability → 02-Structure → ...)

### Tutorial Series
**Purpose**: Hands-on skill building through progressive projects.
**Structure**: Each article is a standalone task that builds on accumulated skills.
**Example**: Build an API (01-Setup → 02-Endpoints → 03-Auth → 04-Testing → 05-Deploy)

### Deep Dive Series
**Purpose**: Explore one topic from multiple angles.
**Structure**: Hub-and-spoke — overview article links to independent deep dives.
**Example**: Authentication (00-Overview → 01-OAuth → 02-SAML → 03-JWT → 04-MSAL)

---

## Series Consistency Checklist

Run before publishing any series or after major updates:

### Structural
- [ ] Article 00 exists with series overview
- [ ] All articles numbered sequentially (00, 01, 02...)
- [ ] Dependency graph has no cycles
- [ ] Series navigation present in all articles

### Content
- [ ] Shared glossary terms defined consistently
- [ ] No contradictory recommendations
- [ ] Progressive complexity (no sudden jumps)
- [ ] Each article is self-contained for search traffic

### Technical
- [ ] Code examples use consistent libraries/versions
- [ ] Sample project evolves consistently (tutorial series)
- [ ] All cross-links valid
- [ ] Metadata consistent per Phase 3 table

---

## References

- **Internal:** `.github/prompts/01.00-article-writing/article-review-series-for-consistency-gaps-and-extensions.prompt.md`
- **Internal:** `.copilot/context/01.00-article-writing/02-validation-criteria.md` (quality thresholds)
- **Internal:** `.copilot/context/01.00-article-writing/workflows/01-article-creation-workflow.md` (per-article creation)
- **Internal:** `.github/instructions/article-writing.instructions.md`

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 2.3.0 | 2026-03-01 | Added "Worked Example: REST API Documentation Series" section showing Phases 1-2 applied to a concrete 7-article series with category matrix, folder mapping, terminology map, and key planning decisions. Source: Gap 3 from context file audit. | System |
| 2.2.0 | 2026-03-01 | Added to Phase 2: Diátaxis category planning (category coverage matrix YAML template, validation criteria by series size, 60% concentration threshold), folder structure planning (folder-to-type mapping YAML template, folder rules). Source: analysis-article-writing-system-architectural-gaps.md Change 4. | System |
| 2.1.0 | 2026-03-01 | Added to Phase 2: content-first design step, terminology handoff rule with YAML map template, and cross-reference strategy selection table (5 strategies). Source: Recommendation F from coverage analysis. | System |
| 2.0.0 | 2026-02-28 | Complete rewrite: replaced phantom prompt names with actual prompt files; consolidated 559 lines to ~180 lines; removed verbose REST API examples and detailed templates; added series patterns, dependency rules, metadata consistency table, maintainability guidance. Source: 40.00-technical-writing articles 08, 10 | System |
| 1.0.0 | 2025-12-26 | Initial version | System |

<!--
context_metadata:
  version: "2.3.0"
  last_updated: "2026-04-26"
-->
