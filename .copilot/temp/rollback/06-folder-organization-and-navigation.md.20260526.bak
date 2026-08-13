# Folder Organization and Navigation Rules

This document defines folder naming conventions, organization patterns, and sidebar menu rules for the Learning Hub documentation site.

## Folder Naming Conventions

### Full Kebab-Case Rule (CRITICAL)

**ALL folders and files MUST use full kebab-case with NO spaces.**

| ❌ Invalid | ✅ Valid | Rule |
|------------|----------|------|
| `01.00 news/` | `01.00-news/` | Space after prefix → hyphen |
| `20251224 vscode Release/` | `20251224-vscode-release/` | All spaces → hyphens |
| `202506 build 2025/` | `202506-build-2025/` | Space + words → hyphens |
| `Topic Name/` | `topic-name/` | Spaces → hyphens |
| `PascalCase/` | `pascal-case/` | Split on capitals |
| `File Name.md` | `file-name.md` | Same rules for files |

### Numeric Prefix Patterns

Folders use numeric prefixes for ordering. The format is `XX.YY-` (hyphen, not space):

| Pattern | Example | Purpose |
|---------|---------|---------|
| `XX.00 category/` | `01.00-news/` | Top-level category |
| `XX.YY subcategory/` | `05.02-prompt-engineering/` | Nested subcategory |

**Rules:**
- Integer part (`XX`) determines primary sort order
- Fractional part (`.YY`) determines secondary sort order
- **HYPHEN** separates prefix from name (not space)
- Use kebab-case for multi-word names

### Date Prefix Patterns

Time-sensitive content uses date prefixes with hyphen separator:

| Pattern | Example | Use Case |
|---------|---------|----------|
| `YYYYMMDD-topic/` | `20251224-vscode-v1.107-release/` | Daily content |
| `YYYYMM-event/` | `202506-build-2025/` | Monthly content |

**Rules:**
- **HYPHEN** separates date from topic name (not space)
- Full kebab-case for entire name

---

### ⚠️ CRITICAL: Glob Sorting Behavior (Quarto Limitation)

**Fundamental fact:** Quarto glob patterns (`**/*.md`) sort **ALPHABETICALLY**, which produces **OLDEST-FIRST** for YYYYMMDD prefixes:

```
Glob order (alphabetical):
20251111...  ← First (oldest)
20251224...
20260130...  ← Last (newest)
```

**Globs CANNOT produce newest-first ordering.** This is a Quarto limitation, not a configuration option.

#### Decision Table: Glob vs Explicit List

| Requirement | Use Glob | Use Explicit List |
|-------------|----------|-------------------|
| Auto-discover new content | ✅ YES | ❌ NO (manual maintenance) |
| Newest-first ordering | ❌ NO (impossible) | ✅ YES |
| Oldest-first acceptable | ✅ YES | ✅ YES |
| Non-date prefixed folders | ✅ YES (sorts correctly) | Optional |

#### Recommended Approach by Section

| Section | Recommended | Rationale |
|---------|-------------|----------|
| `01.00-news/` | **Explicit list** | News requires newest-first; manual maintenance is acceptable |
| `02.00-events/` | **Glob** | Events sorted by session ID (alphabetical is fine) |
| `03.00-tech/` | **Glob** | Alphabetical by topic is appropriate |
| `04.00-howto/` | **Glob** | Alphabetical is appropriate |
| `05.00-issues/` | **Glob** | Alphabetical is appropriate |

### Kebab-Case Standard

**ALL names MUST be full kebab-case:**

```
✅ Good: 05.02-prompt-engineering/
✅ Good: 20251224-vscode-release/
✅ Good: azure-functions-tutorial.md

❌ Bad: 05.02-prompt Engineering/  (spaces)
❌ Bad: 05.02-prompt-engineering/  (space after prefix)
❌ Bad: PromptEngineering/         (PascalCase)
```

**Rationale:** Quarto compiles paths to URLs. Full kebab-case produces clean, consistent, SEO-friendly addresses with no encoding issues.

## Folder Organization Patterns

### Learning Hub Subject Folder Template

For technology topics, follow this structure:

```
XX.YY Subject/
├── 00-overview.md           # Overview (first-touch orientation)
├── 01-getting-started.md    # Getting Started (quickstart + tutorial)
├── 02-concepts.md           # Concepts (mental model)
├── 03-how-to-*.md           # How-to guides (task-oriented)
├── 04-analysis-*.md         # Analysis (evaluations, comparisons)
├── 05-reference.md          # Reference (specifications)
├── 06-resources.md          # Resources (curated links, FAQs)
└── images/                  # Supporting images
```

### News and Events Folders

For time-sensitive content:

```
01.00-news/
├── 20260131 Topic A/        # Newest first
│   ├── session-summary.md   # Short article title (folder has context)
│   └── session-analysis.md
├── 20260124 Topic B/
└── 20260111 Topic C/
```

### Single-Article Folders

When a folder contains only one meaningful article, the folder provides context and the article title should be minimal:

| Folder Name | Article Title | ✅/❌ |
|-------------|---------------|-------|
| `20251224-vscode-v1.107-release/` | `Session Summary` | ✅ |
| `20251224-vscode-v1.107-release/` | `Recording Summary: VS Code v1.107 Release Live Stream` | ❌ Redundant |

## Sidebar Menu Rules

**📖 Complete guidance:** [07-sidebar-menu-rules.md](./07-sidebar-menu-rules.md)

Menu generation rules are defined separately to allow flexibility when actual folder/file names don't strictly follow kebab-case conventions. The menu rules handle both kebab-case (`01.00-news/`) and space-separated (`01.00 news/`) naming.

**Key principles:**
- Numeric prefixes removed together with separator (hyphen OR space)
- Date prefixes preserved in menu items
- YAML `title:` field takes precedence over filename transformation
- Globs sort alphabetically (newest-first requires explicit lists)

---

## Summary: Folder Naming Rules

### Content Naming
1. **Full kebab-case** — ALL folders/files, NO spaces anywhere
2. **Numeric prefixes** (`XX.YY-`) — Hyphen separates from name
3. **Date prefixes** (`YYYYMMDD-`) — Hyphen separates from name
4. **Shortest name** — Avoid redundancy with folder context
5. **Title from metadata** — Prefer YAML title over filename
