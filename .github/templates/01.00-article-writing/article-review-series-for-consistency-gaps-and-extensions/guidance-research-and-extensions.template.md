---
description: "Research and extensions guidance for series review workflows"
---

# Series Review - Research & Extension Guidance

Detailed research methodology for Phases 4-5 of `article-review-series-for-consistency-gaps-and-extensions.prompt.md`.

---

## Gap identification methods

### Method 1: Goal-based gap analysis

1. Review series goals from Phase 1
2. Create topic checklist based on goals
3. Search articles for each topic using `semantic_search`
4. Mark covered vs missing topics

### Method 2: Workspace mining

1. Use `semantic_search` to find related articles outside series
2. Identify topics covered elsewhere but missing in series
3. Suggest incorporating or cross-referencing

### Method 3: Web research (default enabled)

1. **Research current best practices:**
   - Use `fetch_webpage` for official docs (Microsoft Learn, Azure docs, etc.)
   - Query format: "{{series topic}} best practices 2025"
   - Extract recommended approaches, common patterns

2. **Compare with series content:**
   - Are current best practices covered?
   - Missing critical security considerations?
   - Performance optimization topics absent?

3. **Identify outdated content:**
   - Deprecated APIs still featured?
   - Newer alternatives available but not mentioned?

---

## Extension opportunity research

### Adjacent topics (naturally related)

Use `semantic_search` and `fetch_webpage` to research:
- Topics frequently mentioned alongside series topics
- Common next steps after series completion
- Prerequisite knowledge users may lack

### Emerging topics (industry trends)

- Fetch release notes/changelogs from official sources
- Query: "{{topic}} new features 2025"
- Query: "{{topic}} future roadmap"
- Identify paradigm shifts in the domain

---

## Alternatives comparison framework

### Identification process

For major topics/tools in series:
- Query: "alternatives to {{tool/service}}"
- Query: "{{tool}} vs {{competitor}}"
- Fetch comparison articles from official sources

### Objective comparison criteria

| Criterion | Description |
|-----------|-------------|
| **Use Case** | Primary scenarios each option serves |
| **Complexity** | Learning curve and operational overhead |
| **Cost** | Pricing model and typical ranges |
| **Performance** | Available benchmarks or comparisons |
| **Ecosystem** | Maturity, community support, integrations |

### Recommendation pattern

- **Main body mention:** Brief acknowledgment of alternatives (100-200 words)
- **Appendix detail:** Comprehensive comparison (if relevant)
- **When to use alternative:** Specific scenarios favoring each option

---

## Series redefinition assessment

When findings warrant structural changes, evaluate:

### Rename articles (clarity/consistency)
- Does current title accurately reflect content?
- Do titles follow consistent naming pattern?

### Delete articles (redundancy/obsolescence)
- Is all content duplicated in other articles?
- Has content become entirely obsolete?
- **Always Ask First** before recommending deletion

### Add new articles (gap coverage)
- Is gap too large for an existing article section?
- Does topic merit standalone article treatment?
- Position in reading order?

### Reorder articles (logical flow)
- Does current order follow learning progression?
- Are prerequisites properly sequenced?

### Split articles (scope too broad)
- Is article >2x average length in series?
- Does article cover distinctly separate topics?

### Merge articles (scope too narrow)
- Are articles <50% average length?
- Are topics tightly coupled?

---

## Per-article action item generation

For EACH article, generate prioritized action list:

### Priority classification

| Priority | Symbol | Criteria |
|----------|--------|----------|
| **Critical** | 🔴 | Correctness issues, broken links, contradictions |
| **Medium** | 🟡 | Terminology standardization, structure alignment, redundancy |
| **Low** | 🟢 | Code formatting, optional enhancements, minor improvements |

### Required action item fields

Each action MUST include:
- **Issue type** and description
- **Specific file path** and **line numbers**
- **Current text** → **proposed text** (where applicable)
- **Estimated time** to implement
- **Priority** classification

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "article-review-series-for-consistency-gaps-and-extensions"
  changelog: "guidance-research-and-extensions.template.changelog.md"
---
-->
