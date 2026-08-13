---
title: "Sidebar Menu Rules (Runtime Navigation)"
description: "Defines transformation rules for converting folder/file names to sidebar menu items — numeric prefix removal, date-separator normalization, and Title Case application — as implemented by the runtime navigation builder"
domain: "learning-hub"
goal: "Establish deterministic rules for transforming folder/file names into readable sidebar menu items"
scope:
  covers:
    - "Numeric prefix removal and transformation algorithm"
    - "Supported prefix patterns (XX.YY-name, XX.YY name, XX-name, XX name)"
    - "Date prefix handling with space-dash-space separator"
    - "Separator character normalization (hyphen/space/underscore → space-dash-space)"
    - "Transformation examples and regex patterns"
  excludes:
    - "Folder naming conventions (see 06-folder-organization-and-navigation.md)"
    - "Runtime nav implementation (see DynamicNavBuilder / NavRules in src/Learn.Web)"
boundaries:
  - "MUST normalize date separators to space-dash-space format"
  - "MUST NOT modify the content after the separator"
rationales:
  - "Deterministic transformation ensures menu consistency without per-item tweaking"
  - "Date separator normalization prevents visual inconsistency from varied input formats"
---

# Sidebar Menu Rules (Runtime Navigation)

**Purpose**: Define rules for generating sidebar menu items from folder/file structures, independent of strict naming conventions. These rules are implemented by the **runtime navigation builder** (`DynamicNavBuilder` / `NavRules` in `src/Learn.Web`), which builds the live menu from the content hierarchy on each request — there is no menu file to maintain.

**Referenced by**: 
- `src/Learn.Web/Navigation/DynamicNavBuilder.cs` and `src/Learn.Web.Shared/Navigation/NavRules.cs` (implementation)
- Per-folder `metadata.yml` (label/short/icon/order/hidden/topbar-hidden/topbar-align overrides)

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
1. Remove extension (.md)
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

## Ordering (runtime)

The runtime navigation builder discovers content automatically and orders it deterministically — there
are no globs or explicit lists to maintain:

- **Numeric-prefix folders** (`NN.NN-name`) sort **ascending by prefix**.
- **Date-prefixed content** under `01.00-news/` is presented **newest-first**; date folders elsewhere sort ascending.
- **`metadata.yml` `order:`** overrides the derived weight for a folder (ascending; joins the numeric group).
- Ties break by name (ordinal).

New content appears in the menu as soon as it exists in the content source — no list edit, no rebuild.

**📖 Ordering details:** [06-folder-organization-and-navigation.md](./06-folder-organization-and-navigation.md) § Ordering (runtime, dynamic navigation)

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

**Detection:** Folder contains exactly one `.md` file (excluding `readme.md`, `index.md` used for other purposes).

### Index Files

| File | Behavior |
|------|----------|
| `index.md` | Represents parent folder (folder title used) |
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

## How the runtime builder applies these rules

1. `DynamicNavBuilder` lists one level of the content hierarchy from the content source (filesystem or Blob).
2. Working/asset folders (`_`/`.`-prefixed, `images/`) are skipped; `publish: false` files are excluded.
3. Each remaining folder/file becomes a menu item, its label derived via the **Title Resolution Order** above.
4. Items are ordered deterministically (numeric ascending; news newest-first); `metadata.yml` `order:` overrides.
5. `metadata.yml` supplies per-folder `label`/`short`/`icon`, and `hidden`/`topbar-hidden`/`topbar-align` control visibility and placement.

Adding content = create the folder/file in the content source. It appears in the menu on the next request — no list to edit, no preview build, no commit to a navigation file.

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
- [ ] News is presented newest-first (automatic)
- [ ] Single-article folders collapsed
- [ ] Icons selected semantically (via metadata.yml or heuristic)

### Tolerance
- [ ] Handles kebab-case names (`01.00-news/`)
- [ ] Handles space-separated names (`01.00 news/`)
- [ ] Handles mixed conventions in same tree

---

## References

- **Internal**: [06-folder-organization-and-navigation.md](./06-folder-organization-and-navigation.md) — Folder naming conventions (prescriptive)
- **Implementation**: `src/Learn.Web/Navigation/DynamicNavBuilder.cs`, `src/Learn.Web.Shared/Navigation/NavRules.cs`
- **External**: [Bootstrap Icons](https://icons.getbootstrap.com/)

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 2.0.0 | 2026-07-20 | Reframed for the runtime dynamic navigation builder (removed Quarto/glob/_quarto.yml); rules now map to DynamicNavBuilder/NavRules + metadata.yml | System |
| 1.1.0 | 2026-01-31 | Added standard separator format rule for date prefixes (` - `) | System |
| 1.0.0 | 2026-01-31 | Initial version — separated from 06-folder-organization-and-navigation.md | System |

<!--
context_metadata:
  version: "2.0.0"
  last_updated: "2026-07-20"
-->
