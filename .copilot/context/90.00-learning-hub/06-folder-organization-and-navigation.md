---
title: "Folder Organization and Navigation Rules"
description: "Establishes folder naming conventions (kebab-case with numeric prefixes), date-prefix patterns, and deterministic runtime navigation ordering built from the live content hierarchy"
domain: "learning-hub"
goal: "Codify kebab-case naming, numeric prefix patterns, and deterministic ordering rules that enable consistent folder discovery and ordering"
scope:
  covers:
    - "Full kebab-case rule (spaces → hyphens)"
    - "Numeric prefix patterns (XX.YY- format)"
    - "Date prefix patterns (YYYYMMDD, YYYYMM)"
    - "Runtime navigation ordering (numeric ascending; news presented newest-first)"
    - "metadata.yml order override for folders"
    - "Working/intermediate artifact marking (publish: false) and _analysis/ working folder"
  excludes:
    - "Sidebar menu item transformation (see 07-sidebar-menu-rules.md)"
    - "Runtime nav implementation (see DynamicNavBuilder / NavRules in src/Learn.Web)"
boundaries:
  - "MUST use hyphens (not spaces) as separator after numeric prefixes"
  - "MUST NOT use spaces in folder/file names"
  - "MUST mark working/intermediate artifacts with `publish: false` and group them in a `_analysis/` working folder, and MUST NOT wire them into navigation"
rationales:
  - "Kebab-case enables clean request-time routing and consistent tooling"
  - "The runtime navigation builder presents news newest-first without manual list maintenance"
---

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

### Ordering (runtime, dynamic navigation)

Navigation is built at **runtime** by `DynamicNavBuilder` / `NavRules` (`src/Learn.Web`) from the live
content hierarchy — there is no glob list or `_quarto.yml` to maintain, and no build step. Ordering is
deterministic:

- **Numeric-prefix folders** (`NN.NN-name`) sort **ascending by prefix**, keeping the numbered areas grouped.
- **Date-prefixed content** (`YYYYMMDD-…`) under `01.00-news/` is presented **newest-first** (the builder
  inverts date order for news); date folders elsewhere sort ascending.
- **`metadata.yml` `order:`** overrides the derived weight for any folder (ascending; joins the numeric group).
- Ties break by name (ordinal).

Because discovery is automatic, **new content appears in the menu as soon as it exists in the content
source** — no list to edit, no rebuild.

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

**Rationale:** the renderer maps file paths to URLs at request time. Full kebab-case produces clean, consistent, SEO-friendly addresses with no encoding issues.

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

---

## Working / Intermediate Artifacts (Never Published)

Investigation and analysis sessions produce **intermediate material** — triage notes, coverage maps, plans, analysis reports, ranked candidate lists, scope notes — that is **not** the published article. This material MUST be marked and grouped so it can never reach the site and can be removed as a unit.

### Rule 1: mark intent with `publish: false` (engine-neutral)

Every intermediate/working file MUST declare its intent in top YAML:

```yaml
---
title: "..."
publish: false   # never publish — working/internal material
---
```

`publish: false` is the **portable source of truth**: it states the decision directly and does not depend on any rendering engine's behavior. The publish pipeline (navigation builder, menu tooling, and any static-site generator) MUST honor it and never render, list, or link the file. Absence of the key means the file is publishable (the common case).

### Rule 2: group under a `_analysis/` working folder

Gather all intermediate artifacts under a working folder, colocated with the article it supports:

```
01.00-news/20260710.01-loop-engineering/
├── overview.md        # the published article (wired into navigation)
├── images/
└── _analysis/         # working folder — every file carries publish: false
    ├── 01-recommended-plan.md
    └── 05-an1-vision-analysis-and-amendment-list.md
```

The folder keeps working material together and makes bulk removal trivial. **Canonical name:** `_analysis/` (any `_`-prefixed name works — `_work/`, `_thinking/` — but use `_analysis/` for consistency).

### Defense in depth (three layers, no single engine)

| Layer | Mechanism | Nature |
|---|---|---|
| 1. Intent (portable) | `publish: false` in top YAML | Engine-neutral source of truth; every pipeline honors it |
| 2. Organization | `_analysis/` working folder | Groups working files; trivial bulk removal |
| 3. Enforcement (runtime) | Not surfaced by the navigation builder | The nav builder skips it |

**Builder bonus (not the guarantee):** the runtime navigation builder skips any path segment beginning with `_` or `.` (`NavRules.IsExcludedName`), so `_analysis/` is auto-excluded from the menu. Treat this as an extra safety net; the guarantee rests on `publish: false` plus not wiring the file in, which hold regardless of engine.

### Boundaries

- MUST mark every intermediate/working artifact with `publish: false` in its top YAML
- MUST place every intermediate/working artifact under a `_analysis/` working folder
- MUST NOT add any `publish: false` file (or `_analysis/` path) to navigation
- MUST keep information flow one-way: working analysis MAY inform or be synthesized into published content, but published content MUST NOT link to, name, or direct readers to `_analysis/`, any other intermediate working folder, a `publish: false` artifact, or an internal research trail
- MUST validate publishable files before completion and remove every reference or link to intermediate working folders and `publish: false` artifacts
- Only the reader-facing published article is wired into navigation

## Sidebar Menu Rules

**📖 Complete guidance:** [07-sidebar-menu-rules.md](./07-sidebar-menu-rules.md)

Menu generation rules are defined separately to allow flexibility when actual folder/file names don't strictly follow kebab-case conventions. The menu rules handle both kebab-case (`01.00-news/`) and space-separated (`01.00 news/`) naming.

**Key principles:**
- Numeric prefixes removed together with separator (hyphen OR space)
- Date prefixes preserved in menu items
- YAML `title:` field takes precedence over filename transformation
- Runtime ordering is deterministic — news is presented newest-first automatically

---

## Summary: Folder Naming Rules

### Content Naming
1. **Full kebab-case** — ALL folders/files, NO spaces anywhere
2. **Numeric prefixes** (`XX.YY-`) — Hyphen separates from name
3. **Date prefixes** (`YYYYMMDD-`) — Hyphen separates from name
4. **Shortest name** — Avoid redundancy with folder context
5. **Title from metadata** — Prefer YAML title over filename

<!--
context_metadata:
  version: "1.2.0"
  last_updated: "2026-07-20"
-->
