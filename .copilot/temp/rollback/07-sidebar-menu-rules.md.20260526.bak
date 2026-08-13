# Sidebar Menu Rules for Quarto Navigation

**Purpose**: Define rules for generating sidebar menu items from folder/file structures, independent of strict naming conventions.

**Referenced by**: 
- `.github/prompts/90.00-learning-hub/learninghub-createorupdate-quarto-menu.prompt.md`
- `_quarto.yml` (sidebar configuration)

---

## Folder-to-Menu-Item Transformation

### Numeric Prefix Removal

When transforming folder names to menu items, **remove numeric prefixes together with their separator** (hyphen OR space).

**Supported prefix patterns:**

| Pattern | Regex | Example Input | Menu Output |
|---------|-------|---------------|-------------|
| `XX.YY-name` | `^\d+\.\d+-` | `01.00-news/` | "News" |
| `XX.YY name` | `^\d+\.\d+\s+` | `01.00 news/` | "News" |
| `XX-name` | `^\d+-` | `03-concepts/` | "Concepts" |
| `XX name` | `^\d+\s+` | `03 concepts/` | "Concepts" |

**Transformation algorithm:**
```
1. Match prefix: /^(\d+\.?\d*)([-\s]+)/
2. Remove matched prefix AND separator
3. Convert remaining name:
   - Replace hyphens with spaces
   - Replace underscores with spaces
   - Capitalize first letter of each word (Title Case)
```

**Examples:**

| Folder Name | Menu Item |
|-------------|-----------|
| `01.00-news/` | "News" |
| `01.00 news/` | "News" |
| `05.02-prompt-engineering/` | "Prompt Engineering" |
| `05.02 prompt engineering/` | "Prompt Engineering" |
| `03.00-tech/` | "Tech" |
| `03.00 tech/` | "Tech" |

### Date Prefix Handling

Date-prefixed folders/files retain their date in menu items (dates provide context).

**⚠️ CRITICAL RULE: Standard Separator Format**

The separator between the date prefix and the topic name MUST be displayed as ` - ` (space-dash-space) regardless of the original separator character in the folder/file name.

**Transformation Rule:**
1. Detect date prefix (YYYYMMDD or YYYYMM)
2. Replace ANY separator character(s) after the date with ` - ` (space-dash-space)
3. Transform the remaining name (hyphens/underscores → spaces, Title Case)

| Original Separator | Example Input | Menu Output |
|--------------------|---------------|-------------|
| Hyphen (`-`) | `20260130-6-advanced-rules/` | "20260130 - 6 Advanced Rules" |
| Space (` `) | `20260130 6-advanced-rules/` | "20260130 - 6 Advanced Rules" |
| Underscore (`_`) | `20260130_6-advanced-rules/` | "20260130 - 6 Advanced Rules" |
| Multiple (`- `) | `20260130- 6-advanced-rules/` | "20260130 - 6 Advanced Rules" |

**More examples:**

| Pattern | Example Input | Menu Output |
|---------|---------------|-------------|
| `YYYYMMDD-name` | `20251224-vscode-release/` | "20251224 - VS Code Release" |
| `YYYYMMDD name` | `20251224 vscode release/` | "20251224 - VS Code Release" |
| `YYYYMM-name` | `202506-build-2025/` | "202506 - Build 2025" |

**Date detection regex:** `/^(20\d{2})(0[1-9]|1[0-2])(\d{2})?[-_\s]+/`

**Transformation algorithm:**
```
1. Match date prefix: /^(20\d{2})(0[1-9]|1[0-2])(\d{2})?/
2. Remove all separator characters immediately after date: /[-_\s]+/
3. Insert standard separator: " - "
4. Transform remaining name:
   - Replace hyphens with spaces
   - Replace underscores with spaces  
   - Capitalize first letter of each word (Title Case)
```

**Rule:** If folder starts with a valid date pattern, **keep the date**, replace separator(s) with ` - `, transform the rest.

---

## Menu Item Naming Rules

### Title Resolution Order

When determining menu item text, use this priority:

| Priority | Source | Example |
|----------|--------|---------|
| 1 | YAML frontmatter `title:` field | `title: "Getting Started with Azure"` → "Getting Started with Azure" |
| 2 | First H1 heading in content | `# Overview` → "Overview" |
| 3 | Filename (transformed) | `getting-started.md` → "Getting Started" |
| 4 | Folder name (transformed) | `azure-functions/` → "Azure Functions" |

### Filename-to-Menu Transformation

Transform filenames to menu text using:

```
1. Remove extension (.md, .qmd)
2. Remove numeric prefix + separator (same rules as folders)
3. Replace hyphens/underscores with spaces
4. Apply Title Case
```

**Examples:**

| Filename | Menu Item |
|----------|-----------|
| `01-summary.md` | "Summary" |
| `01 summary.md` | "Summary" |
| `getting-started.md` | "Getting Started" |
| `azure_functions_overview.md` | "Azure Functions Overview" |

### Shortest Possible Name Principle

Menu items should avoid redundancy with parent context:

| Parent Folder | File | Menu Item | Rationale |
|---------------|------|-----------|-----------|
| `20251224-vscode-release/` | `session-summary.md` | "Session Summary" | Folder provides topic |
| `02.01-azure/` | `functions-overview.md` | "Functions Overview" | Parent provides "Azure" |
| `03.00-tech/` | `authentication.md` | "Authentication" | Standalone topic |

---

## Glob vs Explicit List Strategy

### ⚠️ Quarto Glob Sorting Limitation

**Fundamental fact:** Quarto globs (`**/*.md`) sort **ALPHABETICALLY**, producing **oldest-first** for date prefixes:

```
Alphabetical order (what globs produce):
20251111...  ← First (oldest)
20251224...
20260130...  ← Last (newest)
```

**Globs CANNOT produce newest-first ordering.** This is a Quarto limitation.

### Decision Matrix

| Requirement | Use Glob | Use Explicit List |
|-------------|----------|-------------------|
| Auto-discover new content | ✅ YES | ❌ NO (manual maintenance) |
| Newest-first ordering | ❌ IMPOSSIBLE | ✅ YES |
| Oldest-first acceptable | ✅ YES | ✅ YES |
| Non-date prefixed content | ✅ YES | Optional |

### Recommended Strategy by Section

| Section | Strategy | Rationale |
|---------|----------|-----------|
| News (`01.00-*`) | **Explicit list** | Requires newest-first |
| Events (`02.00-*`) | **Glob** | Sorted by session ID is fine |
| Tech (`03.00-*`) | **Glob** | Alphabetical is appropriate |
| How-To (`04.00-*`) | **Glob** | Alphabetical is appropriate |
| Issues (`05.00-*`) | **Glob** | Alphabetical is appropriate |

---

## Special Cases

### Single-Article Folders

When a folder contains only one meaningful article, **collapse to single menu entry**:

```yaml
# Instead of nested:
#   20251224-vscode-release/
#     └── summary.md
#
# Show as single item:
- href: "01.00-news/20251224-vscode-release/summary.md"
  text: "20251224 VS Code Release"
```

**Detection:** Folder contains exactly one `.md` or `.qmd` file (excluding `readme.md`, `index.md` used for other purposes).

### Index Files

| File | Behavior |
|------|----------|
| `index.md` / `index.qmd` | Represents parent folder (folder title used) |
| `readme.md` / `README.md` | Represents parent folder (folder title used) |
| `_index.md` | Hugo convention — treat as index |

### Icon Selection

Choose Bootstrap Icons semantically:

| Category | Recommended Icons |
|----------|-------------------|
| News/Updates | `newspaper`, `megaphone` |
| Events | `calendar-event`, `calendar3` |
| Technologies | `cpu`, `code-slash`, `terminal` |
| How-to/Guides | `tools`, `wrench-adjustable` |
| Ideas/Projects | `lightbulb`, `briefcase` |
| Analysis | `graph-up`, `bar-chart` |
| Reference | `book`, `journal-code` |

---

## Menu Generation Workflow

### For Glob-Based Sections

1. Define glob pattern in `_quarto.yml`
2. Quarto auto-discovers files
3. Menu items derived using Title Resolution Order
4. Sorting is alphabetical (Quarto limitation)

```yaml
- section: "Technologies"
  contents: "03.00-tech/**/*.md"
```

### For Explicit-List Sections

1. List files explicitly in `_quarto.yml`
2. Order determines display order (newest-first for dates)
3. Menu items derived using Title Resolution Order
4. Maintain manually when adding new content

```yaml
- section: "News & Updates"
  contents:
    - "01.00-news/20260130-topic/article.md"  # newest
    - "01.00-news/20260124-topic/article.md"
    - "01.00-news/20260111-topic/article.md"  # oldest
```

### Adding New Content to Explicit Lists

1. Add new entry at **TOP** of list (for newest-first)
2. Run `quarto preview` to verify
3. Commit navigation changes with content

---

## Summary Checklist

### Menu Item Generation
- [ ] Numeric prefixes removed (with separator)
- [ ] Date prefixes preserved with standard ` - ` separator
- [ ] All separator chars after date → ` - ` (space-dash-space)
- [ ] Hyphens/underscores → spaces
- [ ] Title Case applied
- [ ] YAML title takes precedence when available

### Navigation Strategy
- [ ] News sections use explicit lists (newest-first)
- [ ] Non-date sections use globs (alphabetical)
- [ ] Single-article folders collapsed
- [ ] Icons selected semantically

### Tolerance
- [ ] Handles kebab-case names (`01.00-news/`)
- [ ] Handles space-separated names (`01.00 news/`)
- [ ] Handles mixed conventions in same tree

---

## References

- **Internal**: [06-folder-organization-and-navigation.md](./06-folder-organization-and-navigation.md) — Folder naming conventions (prescriptive)
- **External**: [Quarto Sidebar Documentation](https://quarto.org/docs/websites/website-navigation.html#side-navigation)
- **External**: [Bootstrap Icons](https://icons.getbootstrap.com/)

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.1.0 | 2026-01-31 | Added standard separator format rule for date prefixes (` - `) | System |
| 1.0.0 | 2026-01-31 | Initial version — separated from 06-folder-organization-and-navigation.md | System |
