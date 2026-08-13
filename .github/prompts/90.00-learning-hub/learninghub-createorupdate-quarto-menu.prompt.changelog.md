---
title: "learninghub-createorupdate-quarto-menu.prompt — change history"
description: "Per-version change history for learninghub-createorupdate-quarto-menu.prompt."
last_updated: "2026-07-20T00:00:00Z"
status: "living"
---

# Change history — learninghub-createorupdate-quarto-menu.prompt

## v11.0.0 — 2026-07-20T00:00:00Z

DEPRECATED — the Learning Hub migrated from the Quarto static site to a dynamic Blazor app (`Learn.Web`) that builds navigation at runtime from the live content hierarchy. There is no `_quarto.yml` / `project.render` / `navigation.json` to validate. The prompt body is now a deprecation stub pointing to the runtime builder (`DynamicNavBuilder` / `NavRules`) and per-folder `metadata.yml`.

## v10.3.0 — 2026-07-14T00:00:00Z

Added leaked-working-artifact detection: exclude files marked `publish: false` (engine-neutral non-publish marker; legacy `draft: true` also honored) and `_analysis/` / `_`-prefixed folders from `project.render` and sidebar, and detect + remove any that leaked in (prevents intermediate analysis material from being published)

## v10.1.0 — 2026-01-31T00:00:00Z

Removed --no-browser flag to enable visual menu verification

## v10.1.0 — 2026-01-31T00:00:00Z

Added Step 4.3 user confirmation before final summary

## v10.0.0 — 2026-01-31T00:00:00Z

CRITICAL FIX - Added mandatory path verification step (Phase 2.2)

## v10.0.0 — 2026-01-31T00:00:00Z

Changed role from 'architect' to 'validator' (accuracy over design)

## v10.0.0 — 2026-01-31T00:00:00Z

Added explicit list_dir verification for EACH project.render path

## v10.0.0 — 2026-01-31T00:00:00Z

Added Phase 2 mandatory output format before proceeding

## v10.0.0 — 2026-01-31T00:00:00Z

Updated Test 1 to match actual typo detection scenario

## v10.0.0 — 2026-01-31T00:00:00Z

Added warning: quarto preview doesn't detect dangling refs

## v10.0.0 — 2026-01-31T00:00:00Z

Simplified Phase 3-4 (removed redundant sidebar rules)
