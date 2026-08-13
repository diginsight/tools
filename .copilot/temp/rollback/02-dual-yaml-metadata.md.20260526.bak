# Dual Metadata Block Structure for Articles

## Overview

All articles use **two metadata blocks** with clear separation:

1. **Top YAML Block** - Document frontmatter (visible, for rendering and site generation)
2. **Bottom HTML Comment with YAML** - Article management metadata (hidden from rendering)

**Key Benefits:**
- ✅ Static site generators (Quarto, Hugo, Jekyll, etc.) render articles using top YAML
- ✅ Bottom metadata completely hidden from readers
- ✅ All metadata travels with article (no orphaned files)
- ✅ YAML structure preserved for easy parsing
- ✅ Clear separation: document properties vs. internal tracking

---

## Complete Article Structure

```markdown
---
# TOP YAML BLOCK: Document Metadata
# Purpose: Document rendering and site generation
# Modified by: Authors manually (NOT by validation prompts)
# Used by: Static site generator (Quarto, Hugo, Jekyll, etc.), site navigation
# Compatibility: Standard YAML frontmatter recognized by most Markdown processors
title: "Your Article Title"
author: "Author Name"
date: "2025-11-21"
categories: [technology, tutorial]
description: "Brief SEO-friendly description"
draft: false
---

# Article Title

Your article content here...

## Introduction

Content...

## Main Sections

Content with code examples, explanations, etc.

## Conclusion

Summary and key takeaways...

## References

- [Official Documentation](https://example.com)
- [Related Article](https://example.com)

<!-- 
---
# BOTTOM YAML BLOCK: Validation Metadata
# Purpose: Quality tracking, validation results, analytics
# Modified by: AI validation prompts + MetadataWatcher
# Used by: Validation workflows, quality reports, analytics
# NOTE: This entire block is inside HTML comment - invisible when rendered

validations:
  grammar:
    last_run: "2025-11-21"
    model: "gpt-4o"
    tool: "grammar-review.prompt"
    outcome: "passed"
    issues_found: 0
    notes: "No issues found"
  
  readability:
    last_run: "2025-11-21"
    model: "gpt-4o"
    tool: "readability-review.prompt"
    outcome: "passed"
    flesch_score: 65.3
    grade_level: 9.2
    notes: "Good readability for target audience"
  
  understandability:
    last_run: null
    model: null
    tool: "understandability-review.prompt"
    outcome: null
    target_audience: null
    notes: null
  
  structure:
    last_run: "2025-11-21"
    model: "gpt-4o"
    tool: "structure-validation.prompt"
    outcome: "passed"
    has_toc: true
    has_introduction: true
    has_conclusion: true
    has_references: true
    heading_structure: "valid"
    issues_found: 0
    notes: "All required sections present"
  
  facts:
    last_run: "2025-11-20"
    model: "gpt-4o"
    tool: "fact-checking.prompt"
    outcome: "passed"
    claims_checked: 15
    sources_verified: 8
    sources:
      - url: "https://docs.microsoft.com/..."
        verified: true
        accessed: "2025-11-20"
    issues_found: 0
    notes: "All claims verified"
  
  logic:
    last_run: null
    model: null
    tool: "logic-analysis.prompt"
    outcome: null
    flow_score: null
    notes: null

article_metadata:
  filename: "article-name.md"
  created: "2025-11-15"
  last_updated: "2025-11-21"
  version: "1.2"
  status: "reviewed"
  content_type: "concepts"        # overview | getting-started | concepts | how-to | analysis | reference | resources
  subcategory: null                # For how-to: task-guide | patterns | techniques | methodology
                                   # For analysis: technology-radar | comparative | strategy | trend
  word_count: 2847
  reading_time_minutes: 11
  primary_topic: "Technology Topic"

cross_references:
  related_articles:
    - "related-article-1.md"
    - "related-article-2.md"
  series: "Tutorial Series Name"
  prerequisites:
    - "prerequisite-article.md"
---
-->
```

---

## Top YAML Block Rules

### Fields (Document Metadata)

**Required:**
- `title`: Article title (string)
- `author`: Author name (string)
- `date`: Publication/update date (YYYY-MM-DD)

**Optional:**
- `categories`: Array of category tags
- `description`: SEO description (string)
- `draft`: Boolean (true hides from site)
- `toc`: Boolean (table of contents)
- `number-sections`: Boolean
- Additional fields specific to your static site generator (Quarto, Hugo, Jekyll, etc.)

### When Authors Should Update Top YAML:

- Changing article title
- Updating publication date
- Adding/changing categories
- Modifying description
- Changing draft status

---

## Bottom HTML Comment Block Rules

### Structure

The bottom block is wrapped in HTML comments to hide from rendering:

```markdown
<!-- 
---
validations:
  grammar: {...}
article_metadata:
  filename: "article.md"
---
-->
```

**Key points:**
- HTML comment markers: `<!--` and `-->`
- Standard YAML delimiters inside: `---` at start and end
- Proper YAML indentation (2 spaces)
- All validation prompts update this block

### Fields (Article Additional Metadata)

#### `validations` Section

Each validation type has its own subsection:

```yaml
validations:
  grammar:
    last_run: "YYYY-MM-DD"        # ISO date string
    model: "gpt-4o"                # AI model used
    tool: "grammar-review.prompt"  # Prompt file name
    outcome: "passed"              # passed | minor_issues | needs_revision
    issues_found: 0                # Integer count
    notes: "Description"           # Human-readable summary
```

**Validation types:**
- `grammar` - Grammar, spelling, punctuation
- `readability` - Reading level, flow, consistency
  - Additional fields: `flesch_score` (float), `grade_level` (float)
- `understandability` - Audience comprehension
  - Additional field: `target_audience` (string)
- `structure` - Template compliance, required sections
  - Additional fields: `has_toc`, `has_introduction`, `has_conclusion`, `has_references` (booleans), `heading_structure` (string)
- `facts` - Factual accuracy, source verification
  - Additional fields: `claims_checked` (int), `sources_verified` (int), `sources` (array)
- `logic` - Logical flow, concept connections
  - Additional field: `flow_score` (float)

#### `article_metadata` Section

Article tracking, classification, and analytics:

```yaml
article_metadata:
  filename: "article-name.md"      # Filename (auto-updated by MetadataWatcher)
  created: "YYYY-MM-DD"            # Creation date
  last_updated: "YYYY-MM-DD"       # Last modification (updated by prompts)
  version: "1.0"                   # Semantic version
  status: "draft"                  # draft | in-review | reviewed | published
  content_type: "concepts"         # Category from 7-category taxonomy:
                                   #   overview | getting-started | concepts | 
                                   #   how-to | analysis | reference | resources
  subcategory: null                # For how-to: task-guide | patterns | techniques | methodology
                                   # For analysis: technology-radar | comparative | strategy | trend
                                   # null for other content types
  word_count: 2500                 # Integer (updated by prompts)
  reading_time_minutes: 10         # Calculated: word_count / 250
  primary_topic: "Topic Name"      # Main subject
```

**Content Type Values:**
- `overview` — First-touch orientation ("What is this?")
- `getting-started` — Tutorials and quickstarts ("How do I begin?")
- `concepts` — Explanatory content ("How does this work?")
- `how-to` — Task accomplishment ("How do I accomplish X?")
- `analysis` — Strategic evaluation ("What approach should we use?")
- `reference` — Authoritative lookup ("What are the exact specifications?")
- `resources` — Supporting materials ("Where can I learn more?")

#### `cross_references` Section

Links to related content:

```yaml
cross_references:
  related_articles:                # Array of related article filenames
    - "related-1.md"
    - "related-2.md"
  series: "Series Name"            # Series this article belongs to (or null)
  prerequisites:                   # Array of prerequisite article filenames
    - "prereq-1.md"
```

### ✅ Rules for Validation Prompts

When updating bottom metadata block:

1. **Read entire article** including both blocks
2. **Parse HTML comment** to extract YAML content
3. **Update ONLY your validation section**
4. **Update `article_metadata.last_updated`** to today
5. **Update `article_metadata.word_count`** if analyzing content
6. **PRESERVE all other validation sections** exactly as they are
7. **Serialize back to YAML** maintaining structure
8. **Wrap in HTML comment** with YAML delimiters
9. **Replace entire bottom block** with updated version

### Reading Pattern (Regex)

```regex
<!--\s*\r?\n---\s*\r?\n(.*?)\r?\n---\s*\r?\n-->
```

This matches:
- `<!--` - Opening HTML comment
- Optional whitespace and newline
- `---` - YAML block start
- Newline
- `(.*?)` - YAML content (captured group)
- Newline
- `---` - YAML block end
- Optional whitespace and newline
- `-->` - Closing HTML comment

### Writing Pattern (Pseudocode)

```python
# 1. Read article content
content = read_file(article_path)

# 2. Extract YAML from HTML comment
match = regex_match(r'<!--\s*\r?\n---\s*\r?\n(.*?)\r?\n---\s*\r?\n-->', content)
yaml_content = match.group(1)

# 3. Parse YAML
metadata = yaml_parse(yaml_content)

# 4. Update YOUR validation section only
metadata['validations']['grammar'] = {
    'last_run': today(),
    'model': 'gpt-4o',
    'outcome': 'passed',
    'issues_found': 0,
    'notes': 'No issues'
}

# 5. Update article_metadata
metadata['article_metadata']['last_updated'] = today()
metadata['article_metadata']['word_count'] = count_words(content)

# 6. PRESERVE all other sections (don't delete anything)

# 7. Serialize back to YAML
updated_yaml = yaml_serialize(metadata)

# 8. Reconstruct HTML comment block
updated_block = f"<!-- \n---\n{updated_yaml.strip()}\n---\n-->"

# 9. Replace in content
updated_content = regex_replace(content, pattern, updated_block)

# 10. Write back
write_file(article_path, updated_content)
```

---

## Do's and Don'ts

### ✅ DO:

- Read both metadata blocks to understand article state
- Check if validation already run recently (avoid redundant work)
- Update only your validation section in bottom block
- Update `article_metadata.last_updated` when you modify metadata
- Update `article_metadata.word_count` when analyzing content
- Preserve all other validation sections exactly
- Use proper YAML indentation (2 spaces)
- Wrap bottom block in HTML comment with YAML delimiters
- Log what you're updating and why

### ❌ DON'T:

- **NEVER modify top YAML block** (Quarto metadata)
- Don't delete other validation sections
- Don't change field names or structure
- Don't remove sections you don't understand
- Don't add top-level keys without discussion
- Don't use tabs for indentation (YAML uses spaces)
- Don't forget to update `last_updated` timestamp
- Don't skip the HTML comment wrapper

---

## Example Validation Updates

### Grammar Validation

```markdown
<!-- 
---
validations:
  grammar:
    last_run: "2025-11-21"
    model: "gpt-4o"
    tool: "grammar-review.prompt"
    outcome: "passed"
    issues_found: 2
    notes: "Fixed 2 spelling errors in technical terms"
  
  # ALL OTHER VALIDATION SECTIONS PRESERVED UNCHANGED
  readability: {...}
  structure: {...}
  facts: {...}
  logic: {...}

article_metadata:
  filename: "article-name.md"
  last_updated: "2025-11-21"  # ← Updated
  word_count: 2847            # ← Updated
  # All other fields preserved

cross_references:
  # Preserved unchanged
---
-->
```

### Multiple Validations (Publish-Ready Check)

```markdown
<!-- 
---
validations:
  grammar:
    last_run: "2025-11-21"
    outcome: "passed"
  readability:
    last_run: "2025-11-21"
    outcome: "passed"
  structure:
    last_run: "2025-11-21"
    outcome: "passed"
  facts:
    last_run: "2025-11-20"
    outcome: "passed"

article_metadata:
  last_updated: "2025-11-21"
  status: "published"  # ← Changed from "reviewed"
  version: "2.0"       # ← Incremented
---
-->
```

---

## MetadataWatcher Behavior

The MetadataWatcher service automatically updates the `filename` field when articles are renamed:

**What it does:**
1. Detects file rename events (`.md` files only)
2. Reads article content
3. Extracts YAML from HTML comment block
4. Updates `article_metadata.filename` to new name
5. Updates `article_metadata.last_updated` to today
6. Writes back to file

**What it doesn't do:**
- Modify top YAML block
- Modify any validation sections
- Create new metadata blocks
- Change any other fields

**Regex used:**
```regex
<!--\s*\r?\n---\s*\r?\n(.*?)\r?\n---\s*\r?\n-->
```

---

## Troubleshooting

### Metadata Not Hidden in Rendered Article

**Cause:** HTML comment not properly formatted
**Fix:** Ensure block is wrapped: `<!-- \n---\n...YAML...\n---\n-->`

### YAML Parse Error

**Cause:** Invalid YAML syntax (indentation, quotes, special characters)
**Fix:** Validate YAML using online parser or YAMLlint

### Validation Section Overwritten

**Cause:** Prompt deleted other sections instead of preserving
**Fix:** Use YAML merge or dict update to preserve existing keys

### Filename Not Updated After Rename

**Cause:** MetadataWatcher not running or file outside watch path
**Fix:** Check service logs, verify file is in workspace

---

## Reference Links

- [Quarto YAML Options](https://quarto.org/docs/reference/formats/html.html)
- [YAML Specification](https://yaml.org/spec/1.2/spec.html)
- [HTML Comments](https://developer.mozilla.org/en-US/docs/Learn/HTML/Introduction_to_HTML/Getting_started#html_comments)
- Article templates: `.github/templates/*.md`
- Validation prompts: `.github/prompts/*.prompt.md`
