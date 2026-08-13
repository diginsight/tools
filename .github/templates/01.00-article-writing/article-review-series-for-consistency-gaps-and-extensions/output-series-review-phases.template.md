---
description: "Phase structure for article series review workflows"
---

# Series Review - Phase Output Templates

Use these output formats when running `article-review-series-for-consistency-gaps-and-extensions.prompt.md`.

**Location:** `.github/templates/01.00-article-writing/article-review-series-for-consistency-gaps-and-extensions/`
**Prompt:** `.github/prompts/01.00-article-writing/article-review-series-for-consistency-gaps-and-extensions.prompt.md`

---

## Phase 1: Series Discovery Output

```markdown
### Series Discovery Results

**Series Name:** {{name from metadata or user input}}
**Total Articles:** {{count}}
**Discovery Method:** {{Explicit List / Folder Pattern / Metadata Discovery}}

**Reading Order:**
1. [article-title-1.md](path/to/article-1.md)
2. [article-title-2.md](path/to/article-2.md)
3. [article-title-3.md](path/to/article-3.md)
...
```

---

## Phase 1: Series Goals and Scope Output

```markdown
### Series Goals and Scope

**Primary Goal:** {{what series teaches or accomplishes}}
**Target Audience:** {{beginner/intermediate/advanced, role}}

**Scope Boundaries:**
- **In Scope:** {{topics covered}}
- **Out of Scope:** {{explicitly excluded topics}}

**Learning Progression:**
- **Pattern:** {{linear/branching/modular}}
- **Depth:** {{overview-only / intermediate / comprehensive}}

**Article Inventory:**

| # | Title | File | Word Count | Key Topics | Pub Date |
|---|-------|------|------------|------------|----------|
| 1 | {{title}} | {{filename}} | {{~words}} | {{topics}} | {{date}} |
```

---

## Phase 1: Evaluation Criteria Output

```markdown
### Evaluation Criteria

**Consistency Requirements:**
- {{dimension}}: {{standard}}

**Redundancy Rules:**
- {{rule}}: {{threshold}}

**Coverage Standards:**
- Core topics: {{list of must-have topics based on goals}}
- Supporting topics: {{should-have topics}}
- Adjacent topics: {{nice-to-have topics}}

**Proportionality Matrix:**
- High-relevance: {{criteria}} → {{treatment}}
- Medium-relevance: {{criteria}} → {{treatment}}
- Low-relevance: {{criteria}} → {{treatment}}
```

---

## Phase 2: Cross-Article Content Analysis Output

```markdown
### Cross-Article Content Analysis

**Total Unique Terms:** {{count}}
**Terminology Consistency Score:** {{percentage}}% ({{consistent}} / {{total}})

**Total Concepts Explained:** {{count}}
**Redundant Explanations:** {{count}} ({{percentage}}%)

**Cross-Reference Health:**
- ✅ Valid links: {{count}}
- 🔴 Broken links: {{count}}
- ⚠️ Missing expected links: {{count}}
```

---

## Phase 2.5: Content Architecture Output

```markdown
### Content Architecture Assessment

#### a) Diátaxis Compliance Per Article

| Article | File | Stated type | Detected type(s) | Compliant? | Issue |
|---------|------|-------------|-------------------|------------|-------|
| {{title}} | {{path}} | {{type}} | {{detected}} | ✅/❌ | {{description or "—"}} |

**Splitting Candidates:**
- **[{{article.md}}](path)** (L1–L{{end}}, {{N}} lines) — Mixes {{type1}} + {{type2}}
  - Lines L{{start}}–L{{end}}: {{type1}} content ({{description}})
  - Lines L{{start}}–L{{end}}: {{type2}} content ({{description}})
  - **Recommendation:** Split into {{N}} articles per Art. 02 splitting criteria: "{{criteria matched}}"

**Type Mismatches:**
- **[{{article.md}}](path)** — In {{folder}}/  ({{folder_type}}) but content is {{actual_type}}
  - **Recommendation:** Move to {{correct_folder}}/

#### b) Folder Structure Analysis

**Current folder structure:**
```
{{series-root}}/
├── {{folder1}}/  ({{N}} articles, {{diátaxis_type}})
│   ├── {{article1.md}}
│   └── {{article2.md}}
├── {{folder2}}/  ({{N}} articles, {{diátaxis_type}})
...
└── {{root-level articles}}  ({{N}} articles, no category)
```

**Issues:**
- {{Empty folder / Overcrowded folder / Misplaced article / No category folder}} — {{details}}

**Folder-Diátaxis Alignment:**
| Folder | Expected type | Actual content types | Aligned? |
|--------|--------------|---------------------|----------|
| {{folder}} | {{type}} | {{types found}} | ✅/❌ |

#### c) Article Scope/Size Analysis

| Article | File | Lines | Assessment | Action |
|---------|------|-------|-----------|--------|
| {{title}} | {{path}} | {{N}} | ✅ Sweet spot / ⚠️ Too thin / 🔴 Too long | {{None / Expand / Split}} |

**Splitting Recommendations:**
- **[{{article.md}}](path)** ({{N}} lines) — Splitting criteria met:
  - {{Criterion}}: {{evidence}}
  - **Proposed split:** {{article-a.md}} ({{type1}}) + {{article-b.md}} ({{type2}})

**Merging Candidates:**
- **[{{article-a.md}}](path)** ({{N}} lines) + **[{{article-b.md}}](path)** ({{N}} lines)
  - Combined: {{N}} lines; same audience and purpose
  - **Proposed merge:** {{merged-article.md}}

#### d) Category Coverage Matrix

| Diátaxis type | Count | % of series | Articles | Assessment |
|--------------|-------|-------------|----------|-----------|
| Explanation (concept) | {{N}} | {{%}} | {{list}} | {{OK / Missing / Over-represented}} |
| How-to | {{N}} | {{%}} | {{list}} | {{OK / Missing / Over-represented}} |
| Tutorial | {{N}} | {{%}} | {{list}} | {{OK / Missing / Over-represented}} |
| Reference | {{N}} | {{%}} | {{list}} | {{OK / Missing / Over-represented}} |
| **Total** | **{{N}}** | **100%** | | |

**Coverage Issues:**
- {{Missing type}}: Series goals require {{type}} content for {{reason}}. Currently 0 articles.
- {{Over-concentration}}: {{type}} represents {{%}}% of series (threshold: 60%)

**Recommendation:** {{Add N explanation articles covering: [topics]; Create reference article for: [topics]}}

#### e) Learning Path Analysis

**Prerequisite Chain:**
```
{{Art. 00}} ──→ {{Art. 01}} ──→ {{Art. 02}}
                    │
                    ▼
              {{Art. 03}} ──→ {{Art. 04}}
```

**Cycle Detection:** {{None found ✅ / Cycle detected: Art. X → Art. Y → Art. X 🔴}}

**Reader Journey Viability:**

| Journey | Viable? | Issues |
|---------|---------|--------|
| Explorer (browse TOCs) | ✅/❌ | {{issue or "Clear TOCs with descriptive headings"}} |
| Beginner (sequential) | ✅/❌ | {{issue or "Linear path available from Art. 00"}} |
| Practitioner (jump to topic) | ✅/❌ | {{issue or "Prerequisites stated; self-contained intros"}} |
| Reviewer (quality-focused) | ✅/❌ | {{issue or "Validation/consistency articles accessible"}} |

**Orphaned Articles:**
- **[{{article.md}}](path)** — No inbound cross-references from other articles; no outbound links to series
  - **Recommendation:** Add cross-references from {{related articles}}

#### Architecture Health Summary

| Check | Status | Findings |
|-------|--------|----------|
| Diátaxis compliance | ✅/⚠️/🔴 | {{N}} compliant, {{N}} mixed-type, {{N}} mismatched |
| Folder structure | ✅/⚠️/🔴 | {{N}} aligned, {{N}} misplaced, {{N}} missing folders |
| Scope/size | ✅/⚠️/🔴 | {{N}} sweet spot, {{N}} too thin, {{N}} too long |
| Category coverage | ✅/⚠️/🔴 | {{N}} types present of 4; {{issues}} |
| Learning paths | ✅/⚠️/🔴 | {{N}} journeys viable; {{N}} orphans |

**Overall Architecture Score:** {{N}}/10
```

---

## Phase 3: Terminology Inconsistencies Output

```markdown
### Terminology Inconsistencies

#### Critical Issues (Fix Required)

**1. "{{concept}}" has {{N}} variations:**
- **"{{variant1}}"** - Used in:
  - [article-1.md](path) - L{{line}}, L{{line}}, L{{line}}
- **"{{variant2}}"** - Used in:
  - [article-2.md](path) - L{{line}}, L{{line}}

**Recommendation:** Standardize to **"{{variant1}}"** (most precise, industry-standard)
**Impact:** HIGH - Core concept appears {{count}} times across series

#### Medium Issues (Standardization Recommended)
{{similar structure}}

#### Low Priority (Acceptable Variation)
{{similar structure}}
```

---

## Phase 3: Structural Inconsistencies Output

```markdown
### Structural Inconsistencies

**Missing Sections:**
- [article-2.md](path) - No "Prerequisites" section (others have it)

**Heading Hierarchy Issues:**
- [article-3.md](path) - Jumps from H2 to H4 (L{{line}}) - missing H3 level

**Metadata Gaps:**
- [article-2.md](path) - Missing `cross_references.series.next` link

**Code Formatting Variations:**
- [article-1.md](path) - Uses ```python without explanations
- [article-2.md](path) - Uses code examples with before/after explanations (preferred)

**Recommendation:** Standardize on preferred pattern across series
```

---

## Phase 3: Contradictions Output

```markdown
### Contradictions and Conflicts

**1. Conflicting Best Practices - {{Topic}}**

- **[article-2.md](path)** (L{{line}}-L{{line}}):
  > "{{quote}}"
  
- **[article-4.md](path)** (L{{line}}-L{{line}}):
  > "{{contradicting quote}}"

**Context:** Article 2 published {{date}}, Article 4 published {{date}}
**Resolution Needed:** {{specific action}}
```

---

## Phase 4: Redundancy Analysis Output

```markdown
### Redundant Content Analysis

**Total Redundancies Found:** {{count}}
**Estimated Word Reduction:** {{count}} words ({{percentage}}% of series)

#### High-Priority Consolidation (Identical Content)

**1. "{{concept}}" Explanation - {{N}} words duplicated**

**Primary Location (Keep):**
- **[article-2.md](path)** - L{{line}}-L{{line}} ({{N}} words)
  - **Rationale:** Most comprehensive explanation
  - **Action:** Mark as definitive source

**Duplicate Locations (Replace):**
- **[article-4.md](path)** - L{{line}}-L{{line}} ({{N}} words - identical)
  - **Action:** Replace with cross-reference link
  - **Estimated reduction:** {{N}} words

#### Medium-Priority Consolidation (Overlapping Content)
{{similar structure}}

#### Acceptable Repetition (Context Recaps)
- [article-3.md](path) - L{{line}}: Brief recap ({{N}} words) - within threshold
```

---

## Phase 4: Coverage Gaps Output

```markdown
### Coverage Gaps

#### Critical Gaps (Essential to Series Goals)

**1. {{Topic}} - MISSING**

**Why Critical:** {{rationale tied to series goals}}

**Expected Topics:**
- {{subtopic 1}}
- {{subtopic 2}}

**Current Coverage:** ❌ None found across {{count}} articles

**Evidence from Web Research:**
- [{{Source Title}}]({{URL}}) - {{what it says}}

**Recommendation:** 
- **Add new article:** "{{filename}}" (estimated {{N}} words)
- **OR expand {{article}}:** Add section ({{N}} words)

#### Supporting Gaps (Recommended for Completeness)
{{similar structure}}

#### Adjacent Topics (Optional Coverage)
{{similar structure}}
```

---

## Phase 5: Extension Opportunities Output

```markdown
### Extension Opportunities

#### Adjacent Topics (Natural Next Steps)

**1. {{Topic Name}}**

**Relevance:** {{why this relates to series}}
**Current Mentions:** {{existing coverage or none}}

**Research Findings:**
- [{{Source}}]({{URL}}) - {{evidence}}

**Recommendation:**
- **Action:** {{Add new article / Add section / Add appendix}}
- **Priority:** {{HIGH/MEDIUM/LOW}}
- **Placement:** {{where in series}}

#### Emerging Topics (Recent Developments)
{{similar structure}}
```

---

## Phase 5: Alternatives Analysis Output

```markdown
### Alternatives Analysis

#### Alternative {{Category}}

**Context:** Series focuses on {{primary approach}}

| Option | Best For | Complexity | Cost (est.) | When to Use |
|--------|----------|------------|-------------|-------------|
| **{{Primary}}** | {{use case}} | {{level}} | {{cost}} | {{scenario}} |
| **{{Alt 1}}** | {{use case}} | {{level}} | {{cost}} | {{scenario}} |

**Research Sources:**
- [{{Source}}]({{URL}})

**Recommendations:**
1. **Update [{{article}}](path):** Add brief comparison (150 words)
2. **Add appendix:** Detailed comparison (800 words)
```

---

## Phase 6: Series Redefinition Output

```markdown
## Series Redefinition Recommendations

### Current Series Structure
{{list current articles with word counts}}

### Proposed Series Structure
{{list proposed structure with changes highlighted}}

### Specific Changes

#### Add New Articles
**{{filename}}**
- **Position:** After article-{{N}}
- **Topics:** {{list}}
- **Estimated Length:** {{N}} words
- **Priority:** 🔴 HIGH / 🟡 MEDIUM / 🟢 LOW

#### Rename Articles
| Current Title | Proposed Title | Rationale | Priority |
|---------------|----------------|-----------|----------|

#### Split/Merge Recommendations
{{details with rationale}}
```

---

## Phase 6: Per-Article Action Items Output

```markdown
## Per-Article Action Items

### [{{article-name.md}}](path)

**Priority Summary:** 🔴/🟡/🟢 ({{N}} critical, {{N}} medium, {{N}} low)

#### Critical Actions (Fix Immediately)

**1. {{Issue Description}} (L{{line}})**
- **Issue:** {{what's wrong}}
- **Action:** {{specific fix}}
- **Locations:** L{{line}}: "{{current}}" → "{{fixed}}"
- **Estimated Time:** {{minutes}}

#### Medium Priority Actions
{{similar structure}}

#### Low Priority Actions
{{similar structure}}

### Series-Wide Actions
{{actions affecting multiple articles}}
```

---

## Executive Summary Output

```markdown
## Series Review Executive Summary

**Series:** {{name}}
**Review Date:** {{date}}
**Articles Analyzed:** {{count}}
**Total Word Count:** {{count}} words

### Overall Health Score: {{N}}/10

| Dimension | Score | Status |
|-----------|-------|--------|
| **Consistency** | {{N}}/10 | {{emoji}} {{status}} |
| **Completeness** | {{N}}/10 | {{emoji}} {{status}} |
| **Redundancy** | {{N}}/10 | {{emoji}} {{status}} |
| **Logical Flow** | {{N}}/10 | {{emoji}} {{status}} |
| **Currency** | {{N}}/10 | {{emoji}} {{status}} |
| **Cross-Referencing** | {{N}}/10 | {{emoji}} {{status}} |

### Key Findings

#### Strengths ✅
- {{finding}}

#### Critical Issues 🔴
1. {{issue with evidence}}

#### Improvement Opportunities 🟡
1. {{opportunity}}

### Recommendations Timeline

#### Immediate (1 Week)
{{list with estimated effort}}

#### Short-Term (1 Month)
{{list with estimated effort}}

#### Long-Term (Next Quarter)
{{list with estimated effort}}
```

---

## Metadata Update Template

```yaml
validations:
  series_validation:
    last_run: "{{ISO-8601 timestamp}}"
    model: "{{model used}}"
    series_name: "{{series name}}"
    article_position: {{N}}
    total_articles: {{N}}
    consistency_score: {{1-10}}
    completeness_score: {{1-10}}
    redundancy_score: {{1-10}}
    issues_found: {{count}}
    issues_critical: {{count}}
    issues_medium: {{count}}
    issues_low: {{count}}
```

---

## Quality Checklist

- [ ] All articles in series read completely before analysis
- [ ] Content architecture validated (Diátaxis compliance, folder alignment, scope/size, category coverage, learning paths)
- [ ] Terminology cross-reference matrix built
- [ ] All cross-references between articles validated
- [ ] Inconsistencies identified with specific line numbers
- [ ] Redundancies classified (identical / overlapping / acceptable)
- [ ] Coverage gaps assessed against series goals
- [ ] Web research performed for current best practices
- [ ] Alternatives identified and objectively compared
- [ ] Series redefinition recommendations provided (if warranted)
- [ ] Per-article action items generated with priorities
- [ ] Executive summary with health scores produced
- [ ] Bottom metadata updated in EACH article (no series-level files)
- [ ] Top YAML blocks left unchanged in all articles

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "article-review-series-for-consistency-gaps-and-extensions"
  changelog: "output-series-review-phases.template.changelog.md"
---
-->
