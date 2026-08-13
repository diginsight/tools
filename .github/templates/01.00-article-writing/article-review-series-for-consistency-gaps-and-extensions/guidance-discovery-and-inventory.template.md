---
description: "Discovery and inventory guidance for series review workflows"
---

# Series Review - Discovery & Inventory Guidance

Detailed discovery methods, content extraction, and cross-article mapping process for Phases 1-2 of `article-review-series-for-consistency-gaps-and-extensions.prompt.md`.

---

## Step 1: Identify series articles

Accept input through THREE methods (priority order):

### Method 1: Explicit file list (highest priority)

User provides file paths or attached files:
- File paths: "tech/azure/01-intro.md, tech/azure/02-auth.md"
- Attached files: `#file:path/to/article1.md` `#file:path/to/article2.md`

### Method 2: Folder path pattern

User provides folder or pattern:
- Folder: "tech/azure-series/"
- Pattern: "tech/azure/*.md"
- Numbered sequence: "20251108 HowTo Series/"

Use `list_dir` to discover all `.md` files in folder.

### Method 3: Metadata discovery (automatic)

If user provides only series name:
- Search for articles with `cross_references.series.name` matching
- Use `semantic_search` for `'series: "{{series_name}}"'`
- Parse bottom metadata blocks to find series members

**For all methods:**
- Validate files exist using `read_file` (lines 1-5 to confirm)
- Count total articles discovered
- Identify reading order (filenames, numbering, or `cross_references.series.part`)

---

## Step 2: Extract series goals and metadata

**For EACH article, read and extract:**

### Top YAML block (lines 1-15 typically)
- `title`, `date`, `categories`, `description`

### Bottom metadata block (end of file)
- `cross_references.series`: Series name, part number, previous/next links
- `cross_references.related_articles`, `cross_references.prerequisites`
- `article_metadata.filename`
- `validations.*`: Existing validation history

### Content structure
- H1 title, major sections (H2), subsections (H3+)
- Key concepts introduced (terms in bold or code)

### Determine series goals from
- Series README.md file (if present in folder)
- First article's introduction section
- User-provided series description
- Common topics across article titles
- Progression pattern (beginner → advanced, overview → deep-dive)

---

## Step 3: Evaluation criteria standards

### Consistency dimensions

1. **Terminology** — Same concepts use same terms across articles
2. **Code Style** — Consistent formatting, naming conventions
3. **Structure** — Similar section organization where applicable
4. **Tone** — Consistent formality level and voice
5. **Reference Style** — Consistent citation and linking patterns

### Redundancy tolerance

| Level | Threshold |
|-------|-----------|
| **Zero tolerance** | Identical code examples without justification |
| **Low tolerance** | >100 words of duplicate explanatory text |
| **Acceptable** | Brief concept recaps with cross-references |
| **Necessary** | Core definitions repeated in each article |

### Coverage completeness

| Priority | Expectation |
|----------|-------------|
| **Core topics** | MUST be covered based on series goals |
| **Supporting topics** | SHOULD be covered for comprehensiveness |
| **Adjacent topics** | MAY be covered or linked externally |
| **Alternatives** | SHOULD be mentioned; detailed comparison optional (appendix) |

### Proportionality standards

| Relevance | Treatment |
|-----------|-----------|
| **High-relevance** | Detailed coverage in main body |
| **Medium-relevance** | Brief coverage or dedicated section |
| **Low-relevance** | Brief mention with external links |
| **Edge cases** | Appendix or FAQ section |

---

## Phase 2: Content mapping process

### Per-article extraction

For EACH article, read complete file and extract:

1. **Headings** (H1-H6) with line numbers
2. **Code blocks** with languages and line numbers
3. **Bold/italic terms** (likely key concepts)
4. **Internal links** to other articles
5. **Reference URLs** from bottom metadata

Use parallel reads when possible (3-5 articles simultaneously).

### Build terminology index

- Technical terms (appeared in code, bold, or glossary)
- Concept definitions (phrases like "X is...", "X refers to...")
- Abbreviations and acronyms
- Product/service names

### Cross-article maps to build

**1. Terminology cross-reference matrix**

| Term | Article 1 | Article 2 | Variations Found | Consistency |
|------|-----------|-----------|------------------|-------------|
| {{term}} | L{{line}} "{{text}}" | L{{line}} "{{variant}}" | 2 variations | ⚠️ Medium |

**2. Concept coverage matrix**

| Concept | Article 1 | Article 2 | Redundancy Level |
|---------|-----------|-----------|------------------|
| {{concept}} | L{{line}} (500 words) | L{{line}} (180 words) | ⚠️ High |

**3. Cross-reference validation**

| From Article | To Article | Link Text | Line | Status |
|--------------|------------|-----------|------|--------|
| {{article1}} | {{article2}} | [{{text}}]({{path}}) | L{{line}} | ✅ Valid / 🔴 Broken |

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "article-review-series-for-consistency-gaps-and-extensions"
  changelog: "guidance-discovery-and-inventory.template.changelog.md"
---
-->
