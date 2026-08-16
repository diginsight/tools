---
name: evidence-capture
description: >
  Captures visual and structural evidence from live surfaces — API explorers, running application UIs,
  cloud portals and CI portals — into provenance-stamped assets an evidence dossier can cite.
  Uses an independent undocked browser window, discovers URLs from the repository instead of hard-coding them,
  and checks freshness before capturing. Use when investigating a repository's live behaviour, capturing screenshots
  for repository-derived documentation, recording an API surface from an explorer, documenting provisioned cloud
  resources, or recording pipeline definitions and run history.
domain: "application-development"
---

# Evidence capture

## Purpose

Turn a live surface into a **citable artifact** — a captured asset plus a provenance record an evidence dossier can reference. Capture is investigation, never authoring: this skill produces assets and records, never documentation pages.

## When to use

- An investigator needs a fact that exists only at runtime, in a portal, or in an explorer
- A documentation page needs a screenshot of a real surface rather than a described one
- A previously captured asset must be re-checked for drift before a page is republished
- The capability matrix says a surface is present and its owning investigator has work to do

Do **not** use it to browse for orientation, to verify a page after writing, or to reach a surface the capability matrix recorded as absent.

## Access boundary

Every procedure in this skill is bound by the evidence access policy. Before any capture you MUST:

- use only grants the user already holds — NEVER provision, escalate or request permissions
- perform read-only operations — NEVER create, modify, delete, restart or queue anything
- prefer a lower environment; announce and wait before any production-touching read
- classify the result — a capture containing a secret, personal data, an internal hostname or exploit-actionable detail goes to the `*.internal.md` sibling

📖 `.copilot/context/10.00-application-development/03-evidence-access-policy.md`

## Workflow

1. **Confirm the surface exists** — read the capability matrix in `src/docs/_evidence/_discovery.md`. If the surface is absent, record the absence and STOP.
2. **Discover the address** — resolve the URL from repository evidence: launch profiles, deployment descriptors, infrastructure definitions, pipeline definitions or settings. NEVER hard-code or guess a URL, and NEVER carry one over from another repository.
3. **Check freshness** — if an asset for this surface already exists, compare its provenance stamp against the current surface. Unchanged → reuse it and STOP.
4. **Open an independent window** — use an undocked, visible browser window the user can watch. NEVER capture from a hidden or embedded surface.
5. **Capture** — follow the matching procedure below.
6. **Stamp** — write the provenance comment beside the asset.
7. **Record** — add an evidence record citing the asset path to the owning dossier.

## Procedures

| Surface | Procedure | Owning investigator |
|---|---|---|
| API explorer | [references/api-explorer.md](references/api-explorer.md) | `ad-code-investigator` |
| Application UI | [references/application-ui.md](references/application-ui.md) | `ad-code-investigator` |
| Cloud portal | [references/cloud-portal.md](references/cloud-portal.md) | `ad-environment-investigator` |
| CI portal | [references/ci-portal.md](references/ci-portal.md) | `ad-devops-investigator` |

## Asset conventions

- Assets go to `src/docs/_evidence/{component-id}/assets/` — never into a published images folder
- Filenames are kebab-case and describe the subject, not the act: `feed-list-endpoint.png`, not `screenshot-1.png`
- Each asset has a sibling `.provenance.yml`:

```yaml
source: "[what was read]"
environment: "[environment name as this repository names it]"
observed: "[YYYY-MM-DD]"
surface: "[api-explorer | application-ui | cloud-portal | ci-portal]"
classification: "[public | internal]"
```

## When capture fails

Blocked, unreachable or refused → record a **gap** in the owning dossier stating what was attempted and what blocked it. Then either continue without the fact or, where a page depends on it, place a marked placeholder the author can render as a marked gap.

NEVER substitute a described screenshot for a captured one, and NEVER reuse an asset from a different environment or a different repository.

## References

- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — access rules, sensitive classification, provenance
- **📖** `.copilot/context/10.00-application-development/02-evidence-dossier-schema.md` — record and gap shape
- **📖** `.copilot/context/10.00-application-development/01-discovery-model.md` — the capability matrix
