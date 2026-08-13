---
title: "Article creation and review rules"
description: "Comprehensive article creation patterns, required elements, deep writing style rules, technical content requirements, and quality checklists — Tier 2 on-demand complement to auto-loaded article-writing.instructions.md"
domain: "article-writing"
goal: "Provide the deep creation and review rules that article-writing prompts need on-demand — Diátaxis patterns, required article elements, content design principles, technical content rules, and quality checklists"
scope:
  covers:
    - "Diátaxis framework structure patterns (tutorial, how-to, reference, explanation)"
    - "Required article elements (YAML frontmatter, TOC, introduction, conclusion, references, validation metadata)"
    - "Content design and organization principles (content-first, topic-based, progressive disclosure, LATCH, Wikipedia pattern)"
    - "Writing style deep rules (global-ready writing, jargon handling, readability by Diátaxis type)"
    - "Technical content requirements (verification, code testing, version pinning, security)"
    - "Quality checklist (8 categories, 40+ items)"
    - "Common patterns (intro patterns, conclusion patterns, key takeaways formatting)"
  excludes:
    - "Voice, mechanics, formatting, accessibility essentials (see article-writing.instructions.md — auto-loaded)"
    - "Quantitative readability metrics and replacement tables (see 01-style-guide.md)"
    - "Validation pass/fail thresholds and freshness scoring (see 02-validation-criteria.md)"
    - "Workflow sequences — creation, review, series planning (see workflows/ subfolder)"
boundaries:
  - "MUST NOT duplicate content from article-writing.instructions.md — this file extends, not repeats"
  - "MUST NOT define quantitative targets — those belong in 01-style-guide.md"
  - "MUST NOT define validation thresholds — those belong in 02-validation-criteria.md"
  - "Common intro patterns MUST remain as pointers to .github/templates/pattern-*.md template files"
rationales:
  - "Tier 2 (on-demand) loading keeps token budget under control — this file is ~600 lines and would bloat the always-loaded instruction file beyond usability"
  - "Diátaxis patterns are the structural backbone of every article — they define the required structure, voice, and validation criteria per article type"
  - "Content design principles (content-first, progressive disclosure, LATCH) were added because structure rules alone don't prevent poorly organized content"
  - "Quality checklist is the single pass/fail gate before publication — it aggregates rules from all layers into one actionable list"
---

# Article Creation and Review Rules

**Purpose**: Comprehensive article creation patterns, required elements, deep writing style rules, technical content requirements, and quality checklists. Loaded on-demand by article creation and review prompts—complements the auto-loaded `article-writing.instructions.md`.

**Referenced by**:
- `.github/prompts/01.00-article-writing/article-design-and-create.prompt.md`
- `.github/prompts/01.00-article-writing/article-review-for-consistency-gaps-and-extensions.prompt.md`
- `.github/prompts/01.00-article-writing/article-review-series-for-consistency-gaps-and-extensions.prompt.md`

---

## ⚠️ Instruction Layering

This context file provides **article creation and review rules** that go beyond the always-loaded essentials:

- `article-writing.instructions.md` (auto-loaded) — Voice, mechanics, formatting, accessibility, boundaries
- `documentation.instructions.md` (auto-loaded) — Base structure, reference classification, dual metadata
- **This file** (on-demand) — Diátaxis patterns, required elements, writing style deep rules, technical content, quality checklists, common patterns

For quantitative metrics and reference tables, see: `01-style-guide.md` (companion context file)
For validation thresholds and freshness scoring, see: `02-validation-criteria.md`

---

## 🏗️ Article Structure (Diátaxis Framework)

Every article MUST identify its type and follow the corresponding structure pattern.

### Tutorial (Learning by Doing)

**Purpose:** Guide newcomers through their first successful experience

**Structure:**
1. **Introduction** — What you'll build/learn
2. **Prerequisites** — What readers need before starting
3. **Step-by-step instructions** — Controlled, visible progress
4. **Verification** — How to confirm success
5. **Next steps** — Where to go from here

**Voice:** Encouraging, patient, supportive

**Example:** "Your First GitHub Copilot Agent: A Beginner's Tutorial"

### How-to Guide (Task-Oriented)

**Purpose:** Help users accomplish specific real-world tasks

**Structure:**
1. **Goal statement** — What this guide achieves
2. **Prerequisites** — Assumed knowledge
3. **Steps** — Direct, efficient instructions
4. **Variations** — Alternative approaches
5. **Troubleshooting** — Common issues

**Voice:** Direct, efficient, goal-focused

**Example:** "How to Configure Validation Caching for IQPilot"

### Reference (Information Lookup)

**Purpose:** Provide accurate, complete technical descriptions

**Structure:**
1. **Overview** — Brief description
2. **Syntax/Signature** — Formal specification
3. **Parameters/Properties** — Complete details
4. **Return values** — What to expect
5. **Examples** — Minimal, illustrative
6. **Related** — Cross-references

**Voice:** Austere, formal, precise

**Example:** "IQPilot MCP Tool Reference"

### Explanation (Understanding Concepts)

**Purpose:** Clarify and illuminate the topic

**Structure:**
1. **Introduction** — Why this matters
2. **Core concepts** — Fundamental ideas
3. **Context** — How it fits in the bigger picture
4. **Alternatives** — Different approaches
5. **Deep dive** — Advanced understanding
6. **Conclusion** — Key takeaways

**Voice:** Thoughtful, contextual, connecting

**Example:** "Understanding Dual Metadata in LearnHub Articles"

---

## 📋 Required Article Elements

### YAML Frontmatter (Top Block)
**Purpose:** Renderer frontmatter metadata (NEVER modify from validation prompts)

```yaml
---
title: "Article Title in Sentence Case"
author: "Your Name"
date: "YYYY-MM-DD"
categories: [category1, category2, category3]
description: "One-sentence summary for search and preview (120-160 chars)"
---
```

### Table of Contents
**Required for articles > 500 words**

- Use `## Table of Contents` heading
- Link to all major sections
- Follow hierarchical structure (H2, then H3)
- Target 5–9 items, 3 levels max, parallel construction

### Introduction Section
**First section after TOC, MUST include:**

- **Hook** — Why this topic matters
- **Scope** — What the article covers
- **Prerequisites** — What readers need to know first (if any)
- **Reading time estimate** — For longer articles

### Body Sections
- **Use descriptive headings** — Readers should understand content from headings alone
- **Progressive disclosure** — Start simple, add complexity gradually
- **Visual hierarchy** — H2 for main topics, H3 for subtopics, never skip levels
- **Limit depth to H3** — If you need H4, restructure the content instead
- **Scannable structure** — Bullets, tables, code blocks

### Conclusion Section
**Every article MUST have a conclusion with:**

- **Key takeaways** — 3–5 bullet points using the bold+em-dash pattern:
  ```markdown
  - **Takeaway label** — One-sentence explanation of why it matters.
  ```
- **Next steps** — Related reading or actions
- **Series navigation** — If part of a series

**Key takeaways formatting (REQUIRED):**
Each takeaway MUST use the **bold label** — explanation pattern. Start each with a bolded noun phrase, follow with an em dash, then a concise sentence. This format enables fast scanning.

✅ Correct:
> - **Content-first design** — Define your audience and scope before choosing a template.
> - **Progressive disclosure** — Lead with essentials, then reveal complexity in layers.

❌ Wrong:
> - You should always design content first.
> - Progressive disclosure is important.

### References Section
**Required when citing sources**

**Classify ALL references with emoji markers:**

| Marker | Type | Examples |
|--------|------|----------|
| 📘 | Official | `*.microsoft.com`, `learn.microsoft.com`, `docs.github.com` |
| 📗 | Verified Community | `github.blog`, `devblogs.microsoft.com`, academic sources |
| 📒 | Community | `medium.com`, `dev.to`, personal blogs |
| 📕 | Unverified | Broken links, unknown sources (fix before publishing) |

**Format:**
```markdown
**[Title](url)** 📘 [Official]  
Description (2-4 sentences): what it covers, why valuable, when to use it.
```

**Organization:** Group by Official Documentation, Verified Community, Community Resources

### Validation Metadata (Bottom HTML Comment)
**Purpose:** Validation tracking (updated by automation only)

```html
<!--
validations:
  grammar:
    status: "not_run"
    last_run: null
  readability:
    status: "not_run"
    last_run: null
  structure:
    status: "not_run"
    last_run: null

article_metadata:
  filename: "article-name.md"
  last_updated: "YYYY-MM-DD"
-->
```

---

## 🎨 Content design and organization principles

These principles guide decisions *before* you start writing. They determine what content to create, how to break it apart, and how to organize it for readers.

📖 **Source:** [Article 02: Structure and Information Architecture](../../../03.00-tech/40.00-technical-writing/02-structure-and-information-architecture.md)

### Content-first design (G1)

**<mark>Content-first design</mark>** means defining the content model—what information exists, who needs it, and in what context—before choosing layout, navigation, or tooling.

**MUST apply before creating any article.** Follow this 3-step process:

1. **Map the audience** — Identify who'll read this, what they already know, and what they need to accomplish
2. **Inventory existing content** — Search the workspace for related articles to avoid duplication and identify linking opportunities
3. **Define scope** — Determine what's in and out before writing a single heading

| Design approach | Risk | Symptom |
|----------------|------|----------|
| Template-first | Content forced into ill-fitting containers | Sections with one sentence or "N/A" filler |
| Tool-first | Content constrained by tooling limits | Docs restructured whenever tooling changes |
| **Content-first** | **None (desired approach)** | **Structure emerges naturally from content needs** |

**Practical check:** If you can't describe the audience and scope in two sentences, you haven't done enough planning.

### Topic-based authoring (G2)

**<mark>Topic-based authoring</mark>** structures documentation as self-contained, independently addressable units ("topics") rather than monolithic documents.

**Core principle:** Each article MUST be comprehensible independently, even when part of a series.

**How to achieve independent comprehension:**
- Include a brief recap (under 100 words) of prerequisite concepts rather than assuming the reader has read earlier articles
- State prerequisites explicitly in the introduction so practitioners can self-assess
- Use cross-references for depth, not as a substitute for explanation

**Granularity decision table:**

| Create a separate article when… | Keep as a section when… |
|----------------------------------|------------------------|
| Content exceeds ~1,500 words | Content is under ~500 words |
| Multiple articles need to reference it | Only one article uses it |
| Content has a distinct audience or purpose | Content shares the same audience and purpose |
| Content changes on a different schedule | Content changes with its parent |

### Progressive disclosure (G3)

Present information in layers, revealing complexity gradually. This reduces cognitive load while ensuring advanced users can access complete information.

**3-layer model:**

| Layer | What it shows | Example |
|-------|---------------|---------|
| **Surface** | What most users need, immediately visible | Quick setup command, TL;DR box |
| **Detail** | Supporting information, available on demand | Configuration options, parameters |
| **Expert** | Edge cases, advanced configuration | Custom implementations, internals |

**Application by Diátaxis type:**

| Content type | Surface level | Detail level | Expert level |
|--------------|---------------|--------------|-------------|
| **Tutorial** | "Do this, see that" | "Here's why this works" | Links to explanation |
| **How-to** | Steps to accomplish goal | Variations and options | Edge cases, troubleshooting |
| **Reference** | Signature, brief description | Parameters, return values | Implementation notes |
| **Explanation** | Core concept | Supporting details, context | Academic depth, research |

**Anti-patterns:**
- ❌ **Information dumping** — All details on first exposure
- ❌ **Hiding essential information** — Critical details buried too deep
- ✅ **Progressive alternative** — Lead with essentials, provide pathways to more detail

### LATCH framework (G4)

Richard Saul Wurman's <mark>LATCH framework</mark> identifies five fundamental ways to organize information. Use this when deciding how to structure content within an article or across a series.

| Principle | Organize by | Best for | When to use |
|-----------|-------------|----------|-------------|
| **L**ocation | Physical or logical position | Geographic info, directory structures, UI layouts | Users think "where is this?" |
| **A**lphabet | Alphabetical order | API references, glossaries, option lists | Users know the exact term they're looking for |
| **T**ime | Chronological sequence | Procedures, changelogs, release notes, workflows | Sequence matters; "what comes next?" |
| **C**ategory | Type, theme, or topic | Feature docs, conceptual groups, topic-based organization | Users think in terms of concepts or features |
| **H**ierarchy | Importance, magnitude, or rank | Priority ordering, severity levels, learning paths | Relative importance guides user decisions |

**Decision process:** Ask "how will the reader look for this information?" The answer determines the organizing principle. Most real documentation combines 2–3 LATCH principles.

### Wikipedia article pattern (G5)

The <mark>Wikipedia article pattern</mark> provides a well-tested structure for encyclopedic or comprehensive content. Use when an article needs to serve as a definitive reference on a broad topic.

**Template:**

```markdown
# Article title

[Lead section — standalone summary that makes sense without reading the rest]

## Table of contents
[Auto-generated or manual TOC]

## Background / history
[Context and origins]

## [Main topic sections]
[Body content organized by LATCH principles]

## See also
[Related articles for discovery — brief list, not annotated]

## Notes
[Explanatory footnotes that don't fit inline]

## References
[Citation list with emoji classification]
```

**Key innovations from Wikipedia:**
- **Lead section stands alone** — The first paragraph summarizes the entire article; a reader who reads only the lead gets the essential message
- **"See also" for discovery** — Related topics listed for exploration, separate from inline cross-references
- **Notes vs. references** — Explanatory notes separated from source citations

**When to use:** Explanation articles covering broad topics; foundational/overview content; articles that serve as landing pages for deeper content.

---

## ✍️ Writing Style Deep Rules

### Active Voice (Default)
**Use active voice unless passive is genuinely better**

✅ "Configure the settings in Azure portal."
❌ "The settings should be configured in the Azure portal."

**When passive is acceptable:**
- To avoid blaming the user: "The file was deleted" (not "You deleted the file")
- When the actor is unknown or irrelevant

### Plain Language

**Use simple, everyday words:**

| Instead of... | Use... |
|---------------|--------|
| utilize | use |
| implement | add, create, set up |
| commence | start |
| terminate | end, stop |
| facilitate | help, enable |
| in order to | to |
| prior to | before |

📖 **Full wordy→crisp replacement table:** `.copilot/context/01.00-article-writing/01-style-guide.md`

### Global-Ready Writing

**MUST apply to all content** — the following rules make text clear for non-native speakers and translation-ready:

- **Include articles** (a, an, the) — don't drop for brevity: "Select the button" not "Select button"
- **Include relative pronouns** (that, who, which) — "The file that you downloaded" not "The file you downloaded"
- **Prefer single-word verbs** over phrasal verbs: "configure" not "set up"; "remove" not "get rid of"
- **Avoid idioms and colloquialisms** — "by default" not "out of the box"; "rough estimate" not "ballpark figure"
- **Avoid culture-specific references** — use months not seasons; no sports/holiday metaphors
- **Clarify ambiguous pronouns** — replace vague "it", "this", "they" with the actual noun
- **Avoid ambiguous temporal words** — "because" not "since" (causal); "although" not "while" (contrast)

📖 **Phrasal verb → single verb table and full checklist:** `.copilot/context/01.00-article-writing/01-style-guide.md`

### Jargon and New Terms (REQUIRED)

When introducing jargon, domain-specific terms, or new vocabulary:

1. **Mark new terms visually** — Use `<mark>` tags to highlight jargon when first introduced
2. **Explain in context** — The sentence introducing jargon MUST explain its meaning (don't just drop terms)
3. **Teach the shorthand** — Once explained, the jargon can be used freely throughout the article

**Examples:**

❌ **Wrong:** "Use short persistence for handoff transfers."
✅ **Correct:** "Context with <mark>short persistence</mark> (only lasts within a single chat session) requires explicit handoff transfers when switching agents."

❌ **Wrong:** "The System → Model direction applies here."
✅ **Correct:** "<mark>System → Model</mark> direction means the content is auto-injected by the system based on file patterns—you don't manually include it."

The goal is to **teach readers the vocabulary** so they can use the shorthand confidently in future reading.

**Audience-specific guidance:**
- **Beginner content:** Every technical term marked and explained on first use
- **Intermediate content:** Common terms referenced without redefinition; new terms marked and explained
- **Advanced content:** Domain expertise assumed; new jargon still marked on first use

### Readability Targets

Flesch 50–70, sentences 15–25 words, paragraphs 3–5 sentences. Adjust by Diátaxis type (tutorials easier, reference harder).

📖 **Full metrics (7 formulas, secondary metrics, CI/CD recommendations):** `.copilot/context/01.00-article-writing/01-style-guide.md`
📖 **Pass/fail thresholds:** `.copilot/context/01.00-article-writing/02-validation-criteria.md`

---

## 🔧 Technical Content Requirements

> These rules apply to all technical articles, guides, and documentation with code or technical claims.

### Technical Accuracy
- **Verify all claims** against official documentation
- **Test all code examples** before including them
- **Specify versions** for frameworks, libraries, and tools
- **Include prerequisites** and dependencies
- **Note platform differences** (Windows, macOS, Linux)

### Code Examples

**MUST include:**
- Complete, runnable examples when possible
- Comments explaining non-obvious logic
- Necessary imports and dependencies
- Error handling for production-like scenarios
- Expected output or behavior

**MUST avoid:**
- Incomplete snippets without context
- Untested code
- Real credentials or secrets (use `YOUR_API_KEY` placeholders)
- Outdated syntax or deprecated APIs

**Example pattern:**
```python
# Install: pip install requests
import requests

def get_user(user_id: str) -> dict:
    """Fetch user data from the API.
    
    Args:
        user_id: The unique identifier for the user.
        
    Returns:
        User data as a dictionary.
        
    Raises:
        requests.HTTPError: If the API request fails.
    """
    response = requests.get(f"https://api.example.com/users/{user_id}")
    response.raise_for_status()
    return response.json()

# Example usage:
# user = get_user("12345")
# Output: {"id": "12345", "name": "Example User", ...}
```

### Version Control and Currency
- **Specify versions prominently** at article start
- **Note deprecated features** with alternatives
- **Include migration guidance** when APIs change
- **Link to official changelogs** or release notes
- **Update articles** when referenced technologies change

### Security Considerations
- **Highlight security implications** in relevant sections
- **Never include real credentials** — use clearly marked placeholders
- **Reference security best practices** documentation
- **Warn about common vulnerabilities** when demonstrating patterns

---

## 📊 Quality Checklist

Before considering an article complete, verify:

### Structure
- [ ] Identifies Diátaxis type (tutorial/how-to/reference/explanation)
- [ ] Follows appropriate structure pattern for type
- [ ] Has all required sections (intro, conclusion, references)
- [ ] Table of contents for articles > 500 words
- [ ] Progressive disclosure (simple → complex)
- [ ] Emoji prefixes on ALL H2 section headings (MUST — every `##` heading starts with an emoji)

### Writing Style
- [ ] Sentence-style capitalization throughout
- [ ] Contractions used consistently
- [ ] Second person voice (you/your)
- [ ] Active voice as default
- [ ] Plain language (no unnecessary jargon)
- [ ] Global-ready: articles/pronouns included, no idioms, no phrasal verbs
- [ ] Input-neutral UI verbs ("select" not "click")
- [ ] Readability score in target range (adjusted for Diátaxis type)

### Understandability
- [ ] Jargon terms marked with `<mark>` on first use
- [ ] Jargon introduced with explanatory sentences (not just dropped)
- [ ] Tables introduced with context sentences
- [ ] Non-obvious table columns explained before the table
- [ ] No unexplained domain-specific terminology
- [ ] Progressive complexity (simple concepts before complex)

### Mechanics
- [ ] Oxford commas in all lists
- [ ] One space after periods
- [ ] No spaces around em dashes
- [ ] En dashes for ranges (not hyphens)
- [ ] Numbers formatted correctly (ordinals spelled out)
- [ ] No sentences starting with numerals
- [ ] No combined formatting (no bold+italic, no bold+code)
- [ ] Procedures: max 7 steps, one action per step, location before action

### Accessibility
- [ ] Inclusive, people-first language
- [ ] Gender-neutral pronouns
- [ ] No problematic terms (master/slave, simply/just, sanity check, etc.)
- [ ] Descriptive link text
- [ ] Alt text on all images (≤125 characters)
- [ ] Visual docs follow complementary principle (text → visual → explain)

### References
- [ ] All sources cited properly
- [ ] References classified with emoji markers
- [ ] Descriptions for each reference (2-4 sentences)
- [ ] Links verified as working
- [ ] Grouped by authority level

### Technical Content (If Applicable)
- [ ] Technical claims verified against official sources
- [ ] Code examples tested and runnable
- [ ] Versions specified for frameworks/libraries
- [ ] No real credentials (use placeholders)
- [ ] Security implications noted

### Metadata
- [ ] Top YAML complete (title, author, date, categories, description)
- [ ] Bottom validation metadata present
- [ ] Filename matches content

---

## 🔄 Common Patterns

Introduction pattern templates are available as separate files for easy copy-paste scaffolding:

| Diátaxis type | Template file |
|---------------|---------------|
| Tutorial | `.github/templates/pattern-tutorial-intro.md` |
| How-to | `.github/templates/pattern-howto-intro.md` |
| Reference | `.github/templates/pattern-reference-intro.md` |
| Explanation | `.github/templates/pattern-explanation-intro.md` |

---

## 📚 Reference Materials

### Primary Sources (Always Consult)

**📘 [Microsoft Writing Style Guide](https://learn.microsoft.com/en-us/style-guide/welcome/)** — The authoritative source for Microsoft writing standards: voice, tone, mechanics, accessibility.

**📗 [Diátaxis Framework](https://diataxis.fr/)** — Systematic approach to documentation through four types: tutorials, how-to guides, reference, and explanation.

**📘 [Google Developer Documentation Style Guide](https://developers.google.com/style)** — Complementary guidance on technical formatting and developer-focused content.

### Repository Context Files

📖 **Style Guide:** `.copilot/context/01.00-article-writing/01-style-guide.md`  
📖 **Dual YAML Metadata:** `.copilot/context/90.00-learning-hub/02-dual-yaml-metadata.md`  
📖 **Reference Classification:** `.copilot/context/90.00-learning-hub/04-reference-classification.md`  
📖 **Validation Criteria:** `.copilot/context/01.00-article-writing/02-validation-criteria.md`

---

## 🧪 Example Validation Workflow

**For single articles:** Run `article-review-for-consistency-gaps-and-extensions.prompt.md` — it handles grammar, readability, structure, facts, references, and gap analysis in 7 phases.

**For article series:** Run `article-review-series-for-consistency-gaps-and-extensions.prompt.md` — it handles cross-article consistency, terminology, and gaps.

**Manual validation order (if not using prompts):**

1. **Structure** — Diátaxis type clear, all required sections present, heading hierarchy correct (H1→H2→H3)
2. **Grammar** — Contractions present, sentence-style caps, Oxford commas, input-neutral verbs
3. **Readability** — Sentence length 15–25 avg, paragraph 3–5 sentences, Flesch score in range
4. **Facts** — Technical claims verified, code tested, links working, references classified
5. **Accessibility** — Inclusive language, descriptive links, alt text, no problematic terms
6. **Metadata** — Top YAML complete, bottom validation metadata present

📖 **Quality thresholds:** `.copilot/context/01.00-article-writing/02-validation-criteria.md`
🔧 **Validation prompts:** `.github/prompts/01.00-article-writing/`

---

## References

- **External:** [Microsoft Writing Style Guide](https://learn.microsoft.com/en-us/style-guide/) — Voice, tone, mechanics, accessibility
- **External:** [Diátaxis Framework](https://diataxis.fr/) — Documentation type patterns
- **External:** [Google Developer Style Guide](https://developers.google.com/style) — Technical formatting
- **Internal:** `.github/instructions/article-writing.instructions.md` (auto-loaded essentials)
- **Internal:** `.github/instructions/documentation.instructions.md` (auto-loaded base rules)
- **Internal:** `.copilot/context/01.00-article-writing/01-style-guide.md` (quantitative targets)
- **Internal:** `.copilot/context/01.00-article-writing/02-validation-criteria.md` (validation thresholds)
- **Internal:** `03.00-tech/40.00-technical-writing/` (source article series — 18 articles)

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.3.0 | 2026-03-01 | Enhanced conclusion section: added bold+em-dash Key Takeaways formatting pattern with examples (Rule 13). Source: remaining ⚠️ items from coverage analysis. | System |
| 1.2.0 | 2026-03-01 | Deduplication: replaced readability targets bullet list with summary + pointer to 01-style-guide.md; moved 4 common intro patterns to `.github/templates/pattern-*.md` template files, replaced section with pointer table. Source: Recommendation C from coverage analysis. | System |
| 1.1.0 | 2026-03-01 | Added "Content design and organization principles" section with 5 subsections: content-first design (G1), topic-based authoring (G2), progressive disclosure 3-layer model (G3), LATCH framework decision table (G4), Wikipedia article pattern template (G5). Source: Recommendation B from coverage analysis + Art. 02. | System |
| 1.0.0 | 2026-03-01 | Initial version: extracted from `article-writing.instructions.md` v1.2 as Tier 2 (on-demand context). Contains Diátaxis patterns, required article elements, writing style deep rules, technical content requirements, quality checklists, common patterns, reference materials, and validation workflow. Source: Recommendation A from coverage analysis. | System |

<!--
context_metadata:
  version: "1.3.0"
  last_updated: "2026-04-26"
-->
