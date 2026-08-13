---
description: "Authoring scaffold for a domain's `domain_profile:` section — the per-domain identity-and-contract (grounding + contract facets) declared once in a `.copilot/context/{domain}/_metadata.yml` and referenced by id from any artifact that declares the domain."
domain: "prompt-engineering"
---

# Domain profile authoring template

Fill in this `_metadata.yml` scaffold for a single domain folder under `.copilot/context/{domain}/`. The `domain_profile:` section is the domain's **identity-and-contract**: it is defined **once per domain** and is **NOT inherited** — artifacts pick it up by declaring `domain: {id}` (or listing the id in `additional_domains:`).

**📖 Authoritative schema + merge/composition rules:** [../../../.copilot/context/00.00-prompt-engineering/00.06-folder-metadata-inheritance.md](../../../.copilot/context/00.00-prompt-engineering/00.06-folder-metadata-inheritance.md)

## When to author one

- MUST author a `domain_profile:` for any domain whose artifacts need role/domain-switch grounding (`domain-expertise-injection`) or per-domain Type-B staleness sources (`external-knowledge`).
- MUST place it in that domain's `.copilot/context/{domain}/_metadata.yml` — NEVER in a PE artifact folder or a vision/use-case document.
- One profile per domain; do NOT restate it per artifact.

## Scaffold

```yaml
# .copilot/context/{domain}/_metadata.yml
schema_version: 1
inherit: true
identity:                              # INHERITED by descendant context files in this domain folder
  author: "[author name]"
  domain: [domain-id]                  # the single primary domain id (kebab-case)
  scope: "[what this domain folder's context covers]"
domain_profile:                        # NOT inherited — the domain's identity & contract; referenced by id
  # ── grounding facets (a pass ACQUIRES these to work in the domain) ──
  role: "[expert framing / persona, e.g. 'Senior Azure solutions architect']"
  authoritative_sources:
    - { url: "[https://canonical-docs-root]", version_scheme: [date|semver|build] }
  monitored_invariants:
    - "[domain truth watched for drift, e.g. 'Managed identity preferred over connection strings']"
  # ── contract facets (every artifact in the domain RESPECTS / is graded against these) ──
  scope: "[what the domain covers / excludes]"
  boundaries:
    - { id: [b-id], text: "[domain-wide constraint every artifact must respect]" }
  quality_criteria:
    - { id: [q-id], text: "[domain-specific dimension of 'good' applied when grading]" }
  conventions:
    - "[domain terminology / naming / style rule]"
```

## Authoring checklist

- [ ] `role` states a concrete expert persona (not a generic "assistant").
- [ ] Every `authoritative_sources` entry has a real canonical URL + a `version_scheme` so Type-B staleness can fire.
- [ ] `monitored_invariants` are falsifiable domain truths, not vague aspirations.
- [ ] `boundaries`/`quality_criteria` carry stable `id`s (the keyed-merge uses them for multi-domain composition).
- [ ] No secrets/tenant IDs in any field (redact to `<placeholder>`).
- [ ] Grounding facets are *loaded*; contract facets are *checked against* — keep the two groups distinct.

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-06-21"
  created: "2026-06-21"
  consumers:
    - "00.00-context-folder-index.md"
---
-->
