---
title: "Repository Configuration (appsettings.json) and External Material Resolution"
description: "Defines the .NET-style layered appsettings.json configuration model for Learning Hub — environment + folder-depth + internal/external + non-versioned user precedence — and how non-public session material (video, slides, transcripts) in an external repository mirror is resolved automatically"
version: "1.1.1"
last_updated: "2026-07-20"
domain: "learning-hub"
goal: "Codify a deterministic, .NET-analogous configuration layering so private/non-shareable material can live in an external repository mirror and be resolved automatically without polluting the public repo"
scope:
  covers:
    - "Root-level appsettings.json and appsettings.{environment}.json"
    - "Environment selection (default Production) and precedence"
    - "Non-versioned user overrides (appsettings.user.json) — git-ignored, highest file layer"
    - "Folder-depth precedence (inner overrides outer; repo root is base)"
    - "Internal/external repository precedence (external mirror overrides internal)"
    - "Repository:ExternalRepositories setting (path + description objects) and defaults"
    - "External material resolution (transcript/video/slides) by mirrored relative path"
  excludes:
    - "Folder naming conventions (see 06-folder-organization-and-navigation.md)"
    - "Runtime rendering/navigation (see Learn.Web / DynamicNavBuilder)"
    - "Dual article metadata (see 02-dual-yaml-metadata.md)"
boundaries:
  - "MUST treat the public repo (Learn) as the base layer and never require private values to live in it"
  - "MUST NOT copy non-public material into the public repo — resolve it in place from the external mirror"
  - "MUST resolve ExternalRepositories from the root appsettings chain only (no recursive bootstrap)"
rationales:
  - "A .NET-style layering is familiar, deterministic, and supports per-folder overrides"
  - "An external mirror keeps non-shareable assets out of the public repo while still automatable"
---

# Repository configuration (appsettings.json) and external material resolution

## Purpose

Define how Learning Hub reads configuration from `appsettings.json` files and how it resolves **session material that cannot be published publicly** (for example original videos, slide decks, or full transcripts). Such material lives in an **external repository mirror** (e.g. `..\Learn.internal` or `…\OneDrive\Learn`) and MUST be picked up automatically when processing content, while the public `Learn` repo stays free of non-shareable assets.

## Referenced by

- `.github/instructions/article-writing.instructions.md` — Source Material Resolution rule (all article writing)
- `.github/prompts/01.00-article-writing/article-generate-techsession-summary.prompt.md` — transcript/slide source resolution
- `.github/prompts/01.00-article-writing/article-generate-techsession-analysis.prompt.md` — transcript/slide source resolution
- `.github/prompts/90.00-learning-hub/learninghub-analyze-build-conference-sessions.prompt.md` — consults external mirrors for transcripts/material
- `src/docs/80. Usecases/202606/20260605.02-conference-download/01-event-ingestion-pe-artifacts-plan.md` — ingestion pipeline
- Any future ingestion agent/skill that needs non-public assets

## Configuration model

Configuration is composed from layered `appsettings.json` files, following **the same precedence rules as a .NET application**. Three orthogonal dimensions stack:

| Dimension | Layers (low → high precedence) | Notes |
|---|---|---|
| **Folder depth** | repo root → … → target content folder | Deeper (more specific) folder overrides shallower. Root `Learn\appsettings.json` is the base. |
| **Environment** | `appsettings.json` → `appsettings.{environment}.json` | Default environment is `Production`. Set via `LEARNHUB_ENVIRONMENT` (fallback `DOTNET_ENVIRONMENT`). |
| **User override (non-versioned)** | `appsettings.json` → `appsettings.user.json` | `appsettings.user.json` is **git-ignored** (never committed) and overrides the committed files at the same folder level. The env-specific variant follows .NET's environment-first naming: `appsettings.{environment}.user.json`. |
| **Repository** | internal (`Learn`) → external mirror | An external mirror file at the same relative path overrides the internal one. |
| **Environment variables** | (highest) | A matching env var overrides all files, as in .NET. |

### Deterministic load order

For a target content folder `F`, compose providers in this order (each later provider overrides earlier on key conflict):

```text
for level in (repo-root → … → F):        # shallow to deep; deeper wins
    load  <internal>/<level>/appsettings.json
    load  <external>/<level>/appsettings.json          # external overrides internal (generic)
    load  <internal>/<level>/appsettings.{ENV}.json
    load  <external>/<level>/appsettings.{ENV}.json     # external overrides internal (env)
    load  <internal>/<level>/appsettings.user.json     # non-versioned user override (generic)
    load  <external>/<level>/appsettings.user.json     # non-versioned user override (external)
    load  <internal>/<level>/appsettings.{ENV}.user.json   # non-versioned user override (env)
    load  <external>/<level>/appsettings.{ENV}.user.json
finally:
    apply matching environment variables                # highest precedence
```

**Resulting precedence within one folder level** (low → high): committed-generic < committed-env < **user-generic < user-env** (`appsettings.user*.json`). The user files always override their committed counterparts at the same level. **Folder depth dominates** all file kinds: a deeper-folder value overrides any shallower-folder value (including a shallower-folder user override). Environment variables win overall.

`<external>` iterates every entry in `Repository:ExternalRepositories` (later entries override earlier). Each external root is expected to **mirror the public repo's relative folder layout**.

## Non-versioned user overrides (`appsettings.user.json`)

Like Markdown content, `appsettings*.json` files are **versioned** in Git — except the **user** variant, which is personal and MUST NOT be committed.

- `appsettings.user.json` (and `appsettings.{environment}.user.json`) sit at the **same folder level** as their committed counterpart and **override it**.
- They are the highest-precedence *file* layer within a folder level (only folder depth and environment variables outrank them).
- They are **git-ignored** repo-wide (`appsettings.user.json` and `appsettings.*.user.json`). Never commit them; never put secrets or personal paths in the committed `appsettings.json`.
- Use them for personal, machine-specific values — e.g. a private mirror location only on your machine:

```jsonc
// Learn/appsettings.user.json  (git-ignored — personal override, NOT committed)
{
  "Repository": {
    "ExternalRepositories": [
      { "path": "D:\\private\\Learn.internal", "description": "my local internal mirror" }
    ]
  }
}
```

**Rule:** the committed `appsettings.json` holds shared defaults; `appsettings.user.json` holds personal overrides and stays out of the GitHub repo.

Lists the external repository mirrors that hold non-public material and override files. Each entry is an object with an explicit `path` and a human-readable `description`. Keys use the .NET colon (`:`) notation.

```jsonc
// Learn/appsettings.json (repository root — committed/base layer)
{
  "Repository": {
    "ExternalRepositories": [
      {
        "path": "..\\Learn.internal",        // sibling internal repo (relative to repo root)
        "description": "internal repository"
      }
    ]
  }
}
```

```jsonc
// Learn/appsettings.user.json (git-ignored — personal, machine-specific)
{
  "Repository": {
    "ExternalRepositories": [
      {
        "path": "%USERPROFILE%\\OneDrive\\Learn", // path differs per machine; %USERPROFILE% is expanded
        "description": "Personal repository"
      }
    ]
  }
}
```

**List composition (union, NOT index-merge):** `Repository:ExternalRepositories` entries from all layers are **concatenated** into one list (committed first, then user/external), de-duplicated by resolved `path`. The committed `appsettings.json` holds shared mirrors (the internal repo); personal, machine-specific mirrors (the OneDrive “Personal repository”) go in the git-ignored `appsettings.user.json`. So the effective list above is `[..\Learn.internal, %USERPROFILE%\OneDrive\Learn]`. (This intentionally differs from .NET's default array-by-index merge, which would otherwise overwrite the internal entry.)

**Defaults** (when the setting is absent everywhere): a single entry `..\Learn.internal` (“internal repository”).

**Rules:**
- Each entry MUST provide `path`; `description` is recommended (human context only — not used for resolution).
- `path` MAY be relative to the repo root (`..\Learn.internal`) or absolute.
- `%USERPROFILE%` (and other environment variables) MUST be expanded before use; `<user>` is never a literal.
- The external-repositories list itself is **bootstrap config**: resolve it from the root `appsettings.json` chain (root committed → root env → root user → env vars). Do not recurse into per-folder files to discover mirrors.
- Resolution order across the composed list is committed entries first, then user entries; later entries override earlier on key conflicts during material lookup.
- Missing external roots are skipped silently (a mirror is optional).

## External material resolution

When a phase needs an asset that may be non-public (e.g. `transcript.txt`, original `transcript.docx`, slide PDF, or source video), resolve it in this order and **use the first hit**:

1. The asset in the **public** folder (`Learn/<relative>/asset`).
2. The asset in each **external mirror** at the **same relative path** (`<external>/<relative>/asset`).
3. The asset in the external mirror walking **up the parent hierarchy** of that relative path (nearest parent wins) — supports shared/event-level assets.

External material MUST be **read in place**; never copy it into the public repo. The public `summary.md` MAY reference that the full transcript/recording is private (link or note), preserving attribution without exposing the asset.

### Example

Processing `02.00-events/202606-build-2026/01-general-and-keynotes/key01-microsoft-build-opening-keynote/`:

- Public transcript missing in `Learn/…/key01-…/transcript.txt`.
- Resolver finds `..\Learn.internal\02.00-events\202606-build-2026\01-general-and-keynotes\key01-microsoft-build-opening-keynote\transcript.txt` → used automatically.
- The public `summary.md` is still generated; the full transcript stays private in `Learn.internal`.

## References

- **📖 Related:** [06-folder-organization-and-navigation.md](06-folder-organization-and-navigation.md) — relative-path/folder conventions the mirror reuses
- **📖 Related:** [02-dual-yaml-metadata.md](02-dual-yaml-metadata.md) — article metadata layers
- [.NET configuration providers and precedence](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/) 📘 [Official]
- [Environment-based appsettings in .NET](https://learn.microsoft.com/aspnet/core/fundamentals/environments) 📘 [Official]

## Version history

- **v1.1.0** (2026-06-14): Added non-versioned `appsettings.user.json` (and `appsettings.{environment}.user.json`) override layer — git-ignored, highest-precedence file layer within a folder level; updated model table and load order.
- **v1.0.0** (2026-06-14): Initial version — layered appsettings model (folder depth + environment + internal/external + env vars), `Repository:ExternalRepositories` setting (path + description objects) with defaults, and external material resolution by mirrored relative path.
