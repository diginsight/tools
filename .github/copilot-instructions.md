# Learning Hub — Global Project Instructions

> This file is injected **last** into every system prompt and contains repo-specific conventions that complement the general-purpose context and instruction files referenced below.
> It contains only repo-specific rules that cannot live in general-purpose context or instruction files.

---

## Repository Identity

This is the **Learning Hub** — a personal learning and documentation site, delivered as a **fully dynamic Markdown-rendering application** (`src/Learn.Web`).
Content is authored as Markdown and rendered to HTML **on demand at request time** — there is **no build step**, so new or changed content is live immediately.

- **Owner**: Dario Airoldi
- **Content scope**: Technical learning, knowledge development, prompt engineering, events, how-to guides, ideas
- **Rendering**: `Learn.Web` renders Markdown → HTML per request; navigation is built at runtime from the live content hierarchy
- **Content source**: filesystem (dev) or Azure Blob Storage (prod), selected by configuration

---

## General Rules (via Context & Instructions)

The following rules are defined in context files and instruction files. They apply to ALL Markdown documentation repositories (not just this one). Do NOT duplicate them here — reference them.

| Rule | Canonical source | Applies to |
|---|---|---|
| **Kebab-case naming** | `.copilot/context/90.00-learning-hub/06-folder-organization-and-navigation.md` | All folders and files |
| **Dual metadata system** | `.copilot/context/90.00-learning-hub/02-dual-yaml-metadata.md` | All article `.md` files |
| **Article writing** | `.github/instructions/article-writing.instructions.md` | All `.md` files |
| **Documentation structure** | `.github/instructions/documentation.instructions.md` | All `.md` files |
| **PE artifacts** | `.copilot/context/00.00-prompt-engineering/00.00-context-structure-index.md` | PE customization files |
| **Reference classification** | `.copilot/context/90.00-learning-hub/04-reference-classification.md` | All references |

---

## PE Artifact Map (Quick Reference)

| What | Where | Trigger |
|---|---|---|
| **Context files** | `.copilot/context/{domain}/` | Semantic search (automatic) |
| **Instruction files** | `.github/instructions/` | `applyTo` glob (automatic) |
| **Agent files** | `.github/agents/{domain}/` | `@mention` by user |
| **Prompt files** | `.github/prompts/{domain}/` | `/command` by user |
| **Skill files** | `.github/skills/{name}/SKILL.md` | AI-discovered via description |
| **Template files** | `.github/templates/{domain}/` | `📖` reference from consumers |
| **Prompt snippets** | `.github/prompt-snippets/` | `#file:` reference |
| **copilot-instructions** | `.github/copilot-instructions.md` (this file) | Always injected last |

**📖 Full file-type decision guide**: `.copilot/context/00.00-prompt-engineering/01.03-file-type-decision-guide.md`

---

## Repo-Specific Rules

### Content Area Prefixes

| Prefix | Area | Content type |
|---|---|---|
| `01.00-news/` | News | Date-prefixed articles (newest-first in nav) |
| `02.00-events/` | Events | Conference/event notes |
| `03.00-tech/` | Technology | Technical learning articles |
| `04.00-howto/` | How-to | Task-oriented guides |
| `05.00-issues/` | Issues | Problem/solution documentation |
| `06.00-idea/` | Ideas | Explorations and concepts |
| `90.00-travel/` | Travel | Travel documentation |
| `99.00-temp/` | Temporary | Scratch/working files |

### Learn.Web Application

- **Projects**: `src/Learn.Web` (server host) + `src/Learn.Web.Client` (WASM) + `src/Learn.Web.Shared` (RCL)
- **Rendering**: Markdig renders Markdown → HTML on demand — no static output, no `docs/` publish, no build step
- **Navigation**: built at runtime by `DynamicNavBuilder` (`/_nav` API) from the live content hierarchy — NOT a static file
- **Folder metadata**: optional per-folder `metadata.yml` (`label`/`short`/`icon`/`order`/`hidden`/`topbar-hidden`/`topbar-align`) drives the sidebar and top bar
- **Top YAML in articles is renderer frontmatter** (title/author/date) — NEVER modify from validation prompts
- **Observability**: Diginsight (server project only)
- **Local testing**: ALWAYS run the server in a **visible console window** (a normal foreground terminal, NOT a hidden/background process) so the user can see it and stop it (Ctrl+C) to repeat a test. Rebuild `Learn.Web` (do not use `--no-build`) so client WASM changes are served.
- **Validation (MANDATORY for any behavior/UI change)**: validate in a **visible browser** and record the run as a **validation-sequence markdown with screenshots** under the work item's `_validation/` folder. Full rules: `.github/instructions/testing-validation.instructions.md`.

### MetadataWatcher

A .NET tool (`src/MetadataWatcher/`) that watches for file changes and updates bottom validation metadata. Build tasks are defined in `.vscode/tasks.json`.

### Scripts

Utility scripts in `scripts/`:
- `check-links-enhanced.ps1` — Link validation
- `fix-encoding.ps1` — Encoding fixes for Markdown files

> **Legacy removed**: the retired Quarto machinery (`_quarto.yml`, `index.qmd`, `navigation.json`, `scripts/generate-navigation.ps1`, `header-includes.html`, `theme-*.scss`, `styles*.css`, `_filters/`, `_includes/`, and the disabled Quarto workflows) has been **removed**. The live site builds navigation at runtime from the content hierarchy; the local `docs/` folder is gitignored Quarto output and is not used.

---

## Cross-Cutting Conventions

These conventions are **specific to this repository** and not covered by general instruction/context files:

1. **Language**: Write in English (international audience)
2. **Images**: Store in `images/` subfolder within the article's folder
3. **Series articles**: Include prev/next navigation links in conclusions

### Instruction File Loading (CRITICAL)

ALWAYS — make sure that ALL instruction files whose `applyTo` pattern matches any file relevant to the current task are loaded into the system prompt so that rules from all matching files are applied simultaneously.