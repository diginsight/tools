# Series Review - Architecture Validation Guidance

Detailed architectural validation methods for Phase 2.5 of `article-review-series-for-consistency-gaps-and-extensions.prompt.md`.

**Source rules:**
- [Art. 02: Structure and Information Architecture](../../../03.00-tech/40.00-technical-writing/02-structure-and-information-architecture.md) — Series architecture and planning, splitting/merging criteria, length benchmarks, reading paths
- [Art. 08: Consistency Standards and Enforcement](../../../03.00-tech/40.00-technical-writing/08-consistency-standards-and-enforcement.md) — Series-level audit, structural echo, concept coverage matrix

---

## Check a) Diátaxis compliance per article

Every article in a series MUST have a single, clear Diátaxis type. Mixed-type articles cause confusion — readers expect different things from tutorials vs. explanations.

### Detection method

For EACH article:

1. **Read the stated type** from top YAML (`categories`) or introduction section
2. **Analyze actual content** by scanning for type-specific signals:

| Diátaxis type | Content signals |
|---------------|-----------------|
| **Tutorial** | "You'll build...", sequential steps with verification, encouraging voice, "let's try..." |
| **How-to** | "To accomplish X, do Y", task-oriented steps, assumes competence, variations section |
| **Reference** | Formal syntax, parameter tables, return values, austere tone, no narrative |
| **Explanation** | "Why this matters...", conceptual discussion, alternatives compared, connecting voice |

3. **Flag mismatches** when content signals don't match stated type
4. **Flag mixed types** when one article contains signals from 2+ types

### Splitting criteria for mixed-type articles

When an article mixes Diátaxis types, split it using these criteria from Art. 02:

| Signal | Example | Action |
|--------|---------|--------|
| **Two distinct purposes** | Explaining a concept AND providing a step-by-step procedure | Split into explanation + how-to |
| **Two distinct audiences** | Developers AND managers need different parts | Split into audience-specific articles |
| **Subsection has independent value** | A subsection is frequently cross-referenced | Extract to its own article |

### Common mixed-type patterns

| Pattern | What you'll see | Recommended split |
|---------|-----------------|-------------------|
| Explanation + procedures | Concept sections followed by "how to implement" | Explanation article + how-to article |
| Tutorial + reference | Step-by-step guide with embedded API reference tables | Tutorial article + reference article |
| How-to + explanation | "Do this" steps interspersed with "why it works" sections | How-to article + explanation article (cross-reference for "why") |

---

## Check b) Folder structure analysis

Articles should be organized into folders that reflect their content type and audience. Misplaced articles make discovery harder and signal structural problems.

### Validation method

1. **Map current folder structure** using `list_dir`
2. **For each folder**, determine its implicit Diátaxis type from folder name and existing content
3. **For each article**, check whether its Diátaxis type matches its containing folder
4. **Flag misplaced articles** — articles whose type doesn't match the folder's purpose

### Alignment rules

| Folder convention | Expected content |
|-------------------|------------------|
| `concepts/`, `explain/`, `overview/` | Explanation articles |
| `howto/`, `guides/`, `procedures/` | How-to articles |
| `tutorials/`, `getting-started/` | Tutorial articles |
| `reference/`, `api/`, `specs/` | Reference articles |
| Numbered root level (e.g., `00-`, `01-`) | Any type — check the article's own metadata |

### Common folder issues

| Issue | Detection | Recommendation |
|-------|-----------|----------------|
| **Flat folder** | All articles at root level, no categorical subfolders | Organize into category folders (especially for 5+ articles) |
| **Empty folder** | Folder exists but contains no articles | Remove or populate (check if planned content is missing) |
| **Overcrowded folder** | More than 10 articles without subcategories | Introduce subcategories or splitting |
| **Misplaced article** | Article's Diátaxis type differs from folder's purpose | Move to correct folder or reclassify |

---

## Check c) Article scope and size analysis

Articles should be comparable in depth and reading time. Extreme variance signals scope problems.

### Length benchmarks (from Art. 02)

| Article length | Assessment | Action |
|----------------|-----------|--------|
| **Under 400 lines** | Likely too thin — lacks sufficient depth or examples | Consider expanding, or merging with a related article |
| **400–800 lines** | Sweet spot for most articles | Normal range; no action needed |
| **800–1,000 lines** | Acceptable for complex topics with many code examples | Review for sections that could be extracted |
| **Over 1,000 lines** | Likely too long — reader fatigue and maintenance burden | Identify splitting opportunities |

### Measurement method

For EACH article:

1. **Count total lines** using `read_file` (check the last line number)
2. **Estimate word count** — approximately 10 words per line for prose-heavy articles, fewer for code-heavy
3. **Estimate reading time** — roughly 250 words per minute for technical content
4. **Compare with benchmarks** above
5. **Flag outliers** — articles significantly above or below the series average

### Splitting criteria (from Art. 02)

| Signal | Example | Action |
|--------|---------|--------|
| **Length exceeds reader endurance** | Over 1,000 lines or 25+ minutes reading time | Look for a natural split point |
| **Two distinct audiences** | Developers AND managers both need the content | Split into audience-specific articles |
| **Two distinct purposes** | Explaining a concept AND providing step-by-step procedure | Split into explanation + how-to |
| **Subsection has independent value** | A subsection is frequently cross-referenced from other articles | Extract to its own article |
| **Different update cadences** | One section changes monthly, another is permanent | Split to enable independent maintenance |

### Merging criteria (from Art. 02)

| Signal | Example | Action |
|--------|---------|--------|
| **Combined length under 500 lines** | Two thin articles that always link to each other | Merge and redirect |
| **Identical audience and purpose** | Two how-to guides readers always use together | Merge into a single guide |
| **One article is purely setup for the other** | "Prerequisites" article + "Main content" article | Merge with prerequisites as a section |

---

## Check d) Category coverage matrix

A well-designed series covers its topic domain through a balanced mix of Diátaxis types. Imbalances indicate missing content.

### Building the matrix

1. **Count articles by Diátaxis type** across the entire series
2. **Calculate percentages** of each type
3. **Compare against expected distribution** based on series goals

### Coverage thresholds

| Series size | Minimum types expected | Rationale |
|-------------|----------------------|-----------|
| **1–4 articles** | 1 type acceptable | Small series may focus on one purpose |
| **5–9 articles** | At least 2 types | Series should serve both learning and doing |
| **10+ articles** | At least 3 types | Comprehensive series needs explanation, how-to, AND reference or tutorial |

### Balance assessment

| Distribution | Assessment | Action |
|--------------|-----------|--------|
| Single type > 80% | 🔴 Severe imbalance | Add articles of the missing types |
| Single type > 60% | ⚠️ Moderate imbalance | Consider whether the series goals justify this distribution |
| All present types 20–40% each | ✅ Healthy balance | No action needed |
| A type at 0% | ⚠️ Missing type | Evaluate whether series goals require this type |

### Content density by Diátaxis type (from Art. 02)

Different types have different depth expectations:

| Content type | Typical depth | Code density | Example density |
|--------------|--------------|-------------|-----------------|
| **Tutorial** | Moderate (guided pace) | Medium (copy-paste snippets) | High (every step verified) |
| **How-to** | Focused (task-specific) | Medium-high (complete snippets) | Low-medium (target scenario) |
| **Reference** | High (comprehensive) | High (signatures, parameters) | Low (minimal illustrative) |
| **Explanation** | Deep (conceptual) | Low (illustrative only) | Medium (analogies, comparisons) |

---

## Check e) Learning path analysis

A series should support multiple reader journeys. Without explicit learning paths, every reader must start from article 00 and read sequentially, which frustrates experienced practitioners.

### Four reader journeys (from Art. 02)

| Journey | Reader profile | Expected path |
|---------|---------------|---------------|
| **Explorer** | Browsing to learn | Skim TOCs, read articles matching interests |
| **Beginner** | Building skills | Sequential from first to last |
| **Practitioner** | Solving problems | Jump to relevant article via search/TOC |
| **Reviewer** | Validating quality | Validation → Consistency → Style topics |

### Validation method

1. **Map the prerequisite chain** — For each article, identify what it assumes the reader knows
2. **Check for cycles** — No article should require knowledge from a later article in its own prerequisite chain
3. **Identify orphans** — Articles with no inbound cross-references from other series articles
4. **Verify self-containment** — Each article should be comprehensible independently (per topic-based authoring from Art. 02)
5. **Check cross-reference types** — Cross-references should indicate the Diátaxis type of the target

### Prerequisite chain validation

```
Article A ──→ Article B ──→ Article C
                 │
                 ▼
           Article D ──→ Article E
```

**Rules:**
- **No forward dependencies:** Article N should never require Article N+1
- **Brief recaps allowed:** Under 100 words of context from prerequisite articles (per series redundancy policy)
- **Cross-references for depth:** Link to prerequisites, don't embed their content

### Orphan detection

An orphaned article has:
- No inbound links from other series articles
- No outbound links to series articles
- No mention in the series overview or reading paths

**Resolution:** Add cross-references from 2+ related articles, or evaluate whether the article belongs in this series.

### Self-containment check

For EACH article, verify:

| Criterion | Pass | Fail |
|-----------|------|------|
| Introduction states prerequisites explicitly | ✅ | ❌ "Assumes you've read..." without specifying what |
| Key concepts have brief recaps (not just links) | ✅ | ❌ "See Art. 03 for background" with no inline summary |
| Article makes sense if read in isolation | ✅ | ❌ References "the approach discussed earlier" without context |
| Next Steps suggest both sequential and topical paths | ✅ | ❌ Only "See also: next article" |

---

## Architecture health scoring

After completing all five checks, assign an overall architecture health score:

| Score | Meaning | Criteria |
|-------|---------|----------|
| **9–10** | Excellent | All checks pass with minor notes |
| **7–8** | Good | Minor issues in 1–2 checks, no structural problems |
| **5–6** | Needs work | Significant issues in 2–3 checks (e.g., mixed types, missing categories) |
| **3–4** | Poor | Structural problems across multiple checks; splitting/merging needed |
| **1–2** | Critical | Series architecture fundamentally broken; major restructuring required |

**Scoring weight by check:**

| Check | Weight | Rationale |
|-------|--------|-----------|
| a) Diátaxis compliance | 25% | Mixed types are the highest-impact structural problem |
| b) Folder structure | 15% | Affects discoverability but is easy to fix |
| c) Scope/size | 20% | Scope mismatches cause reader fatigue and maintenance burden |
| d) Category coverage | 20% | Missing types leave gaps in the learning experience |
| e) Learning paths | 20% | Path problems affect series usability across all reader profiles |

<!--
template_metadata:
  version: "1.0.0"
  last_updated: "2026-06-12"
-->
