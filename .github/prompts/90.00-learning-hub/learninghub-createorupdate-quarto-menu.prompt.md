---
name: learninghub-createorupdate-quarto-menu
description: "DEPRECATED — the Learning Hub navigation is built at runtime by Learn.Web; there is no _quarto.yml to validate or fix"
agent: agent
model: claude-opus-4.6
domain: "learning-hub"
tools:
  - read_file
argument-hint: '(deprecated — navigation updates automatically; no action needed)'
---

# DEPRECATED — Quarto Navigation Validation & Fix

> **This prompt is retired.** The Learning Hub is no longer a Quarto static site. Navigation is built
> **at runtime** by `DynamicNavBuilder` (`src/Learn.Web`) from the live content hierarchy — there is
> **no `_quarto.yml`, `project.render`, or `navigation.json`** to validate or fix.

## Why it is retired

- A menu item exists **because its folder/file exists** in the content source (filesystem or Azure Blob) — nothing to wire.
- Ordering, labels, icons, and visibility come from the deterministic `NavRules` plus optional per-folder
  `metadata.yml` (`label`/`short`/`icon`/`order`/`hidden`/`topbar-hidden`/`topbar-align`).
- Working artifacts stay out of the menu automatically: `_`/`.`-prefixed folders are skipped and
  `publish: false` files are excluded — no allow-list to maintain.

## What to do instead

- **Add or change content** → create/edit the Markdown in the content source; it appears on the next request.
- **Adjust a folder's label / icon / order / visibility** → edit that folder's `metadata.yml`.
- **Understand the rules** →
  - 📖 `.copilot/context/90.00-learning-hub/06-folder-organization-and-navigation.md`
  - 📖 `.copilot/context/90.00-learning-hub/07-sidebar-menu-rules.md`
  - Implementation: `src/Learn.Web/Navigation/DynamicNavBuilder.cs`, `src/Learn.Web.Shared/Navigation/NavRules.cs`

<!--
prompt_metadata:
  version: "11.0.0"
  created: "2026-01-31T00:00:00Z"
  last_updated: "2026-07-20T00:00:00Z"
  changelog: "learninghub-createorupdate-quarto-menu.prompt.changelog.md"
  status: "deprecated"
-->
