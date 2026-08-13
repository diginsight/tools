---
description: Rules for sibling changelog files (*.changelog.md) — naming, hidden-attribute, exemption from article rules, frontmatter, and entry discipline
applyTo: '**/*.changelog.md'
domain: "article-writing"
context_dependencies:
  - ".copilot/context/90.00-learning-hub/"
---

# Changelog File Rules

## Purpose

A changelog file (`*.changelog.md`) is the **single source of truth for one document's per-version history** — the sibling of an article (or vision) document. It is a machine-oriented history record, NOT a reader-facing article. This file is the authority for what a changelog file is, how it is named and hidden, and how its entries are shaped.

> **Overlap is intentional and coordinated.** `**/*.changelog.md` overlaps the article globs in `documentation.instructions.md` and `article-writing.instructions.md`. Those files each document a carve-out delegating changelog files here, so the pair is orthogonal (no competing precedence). For vision changelogs, `vision-frontmatter.instructions.md` § Changelog File Rules adds the vision-specific entry layer on top of these cross-cutting rules.

## Rules

### Naming and pairing

- A changelog file MUST use the `<document-stem>.changelog.md` suffix and live in the **same folder** as the document it tracks (e.g. `using-mkdocs.md` → `using-mkdocs.changelog.md`).
- The tracked document MAY reference it via `article_metadata.changelog:`. When that field is absent, the convention sibling `<document-stem>.changelog.md` is assumed if it exists.

### Exemption from article rules

- Article structure rules (TOC, introduction, conclusion, References section) MUST NOT be applied to changelog files.
- Microsoft voice/tone rules, reference-classification emoji markers, and reading-time/word-count metadata MUST NOT be required.
- The dual-metadata bottom HTML-comment block MUST NOT be added to changelog files — they use a single top frontmatter block only (see below).

### Not published, hidden best-effort

- A changelog file MUST NEVER be wired into the site navigation as a reader-facing page. This is the durable guarantee that it never appears on the published site.
- A changelog file SHOULD carry the filesystem hidden attribute where the host OS supports it. This is a local Explorer convenience only and is NOT versioned; absence of OS support is acceptable and MUST NOT be treated as an error.

### Frontmatter

- A changelog file MUST carry a top YAML frontmatter block with at least `title`, `description`, `last_updated`, and `status: "living"`.
- It MUST NOT carry the bottom validation/`article_metadata` block.

### Entry discipline

- Entries MUST be ordered most-recent-first.
- Each released entry MUST use a full-SemVer heading with date: `## vX.Y.Z — YYYY-MM-DD`.
- A `## Unreleased` staging section MAY sit at the top; on release it is renamed to the cut version and a fresh empty `## Unreleased` is left in its place.
- The tracked document MUST NOT embed a parallel history (no `version_history:` array, no `change_summary:` history log in its metadata).

### Who writes entries

- The article-review process is the authoritative writer of article changelog entries (see `documentation.instructions.md` § Article change history). Other editors SHOULD append an entry on material changes, best-effort.

## Related Instruction Files

| File | Relationship |
|------|--------------|
| `documentation.instructions.md` | Carve-out delegates `*.changelog.md` here; defines the article ↔ changelog pairing |
| `article-writing.instructions.md` | Carve-out: voice/structure rules do not apply to changelog files |
| `vision-frontmatter.instructions.md` | Adds the vision-specific entry layer for vision changelogs |

<!--
instruction_metadata:
  version: "1.0.0"
  last_updated: "2026-06-12"
-->
