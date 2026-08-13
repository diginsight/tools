---
description: Required structure for use-case documents and their folder overview files — canonical header fields, priority taxonomy, vision anchor, default breadth, related-use-cases linkage, dimension-catalog anchor, dimensions-covered matrix
applyTo: '**/*usecases/**/*.md'
version: "1.2.1"
last_updated: "2026-06-06"
domain: "prompt-engineering"
goal: "Ensure every use-case document AND every folder overview carries the metadata needed for prioritization, coverage audits, and vision-traceability against the canonical dimension catalog"
rationales:
  - "Vision-amendment plans need a coverage promise — every in-scope item lands somewhere; use cases are the most common landing"
  - "Without a declared priority and vision anchor, coverage audits cannot mechanically verify that high-priority vision items have realized use cases"
  - "A consistent shape across use cases makes batch operations (audits, rotations, scheduled reviews) tractable"
  - "Folder READMEs are the navigational entry point — they MUST anchor to the dimension catalog and publish a dimensions-covered matrix so 'every dimension covered by ≥1 use case' is mechanically verifiable"
  - "`usecase-index.json` is the machine-readable coverage object — it MUST carry a `dimensions_covered` field per entry so an audit script can aggregate coverage without parsing 30+ markdown bodies"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
  - ".copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md"
---

# Use-Case Document Rules

## Purpose

Use-case documents describe a single, named user-visible behavior or operational pattern. They are the canonical landing target for in-scope vision-amendment items (see `vision-amendment.instructions.md` — coverage promise). This file specifies the required filename, header, and section shape so audits can be mechanical.

## Scope of Application

A file matched by this rule is a use-case document if EITHER:

- Its filename matches `p[0-9]-[0-9]{2}-<slug>-usecase.md` (canonical use-case filename), OR
- Its H1 begins with `UC-` followed by an identifier (`# UC-23: Process reproducibility`)

Folder overview files (`00-overview.md`), index files, and templates within the same folder are NOT use-case documents and are exempt from the use-case body rules. They MAY still be subject to overview/general documentation rules (see § Folder Overview Rules).

## Canonical Filename

`p<priority-digit>-<order-in-group>-<slug>-usecase.md`

- `<priority-digit>` — one of `0`, `1`, `2` matching the Priority field in the header
- `<order-in-group>` — two-digit zero-padded ordinal within the priority group (`01`, `02`, …)
- `<slug>` — kebab-case description of the use case
- `-usecase` — fixed suffix that lets instruction files target use-case bodies with a narrower `applyTo` glob (e.g. `**/*-usecase.md`) without matching folder overviews or indices

Examples: `p0-01-process-reproducibility-usecase.md`, `p1-02-multipass-validation-invariant-usecase.md`.

## Canonical Header

The H1 MUST be `# UC-<numeric-id>: <title-case-statement>` where `<numeric-id>` is unique within the use-case set.

Immediately below the H1, a single blockquote MUST list these fields in this order:

```markdown
> **Group:** <group-folder-name>
> **Priority:** P0 | P1 | P2
> **Order in group:** <integer>
> **Vision anchor:** <rationale-id | section name | rule id>
> **Default breadth:** full | incremental | bounded-delta
```

Optional fields MAY follow the required block in the same blockquote:

- `**Dimensions:** <comma-separated list>`
- `**Command family:** <family-name>`
- `**Primary entry point:** <invocation string>`
- `**Allowed option classes:** <comma-separated>`
- `**Bundle disposition:** <classification>`
- `**Scope mechanism:** <token | path-set | positional | default-all>`

## Required Sections

Every use-case document MUST contain, in this order:

1. `## 🎯 Purpose` — one paragraph stating the user-visible behavior being verified or realized
2. `## ⚙️ Invocation` *(or `## ⚙️ Goal` for non-command use cases)* — the entry-point command(s) or the operational context
3. `## 🔬 Behavior` — numbered list of observable steps
4. `## 📐 Dimensions covered` — explicit list of dimension ids this use case exercises, in canonical `D#-readable-id` form (see [dimension catalog](../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md))
5. `## 🔗 Related use cases` — links to peer use cases that share dimensions, depend on, or contrast with this one

Optional sections (use as relevant): `## 🚦 Reliability analysis`, `## 💰 Cost & efficiency`, `## ⚠️ Risks`, `## 📜 References`.

## Rules

- Filename MUST match the canonical pattern `p<priority>-<order>-<slug>-usecase.md`
- The H1 MUST follow the `# UC-<id>: <title>` shape
- The header blockquote MUST contain the five required fields (Group, Priority, Order in group, Vision anchor, Default breadth) in the specified order
- `Priority` MUST be exactly one of `P0`, `P1`, `P2`
- `Default breadth` MUST be exactly one of `full`, `incremental`, `bounded-delta`
- The priority digit in the filename MUST match the `Priority` field value
- The order-in-group in the filename MUST match the `Order in group` field value
- `Vision anchor` MUST resolve to either a declared principle id, a rationale id, or an existing section name in the target vision
- Required sections MUST be present in the specified order; optional sections MAY appear anywhere after the required set
- `## 📐 Dimensions covered` entries MUST use the canonical `D#-readable-id` form per [05.07-pe-meta-dimension-catalog.md](../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md). Bare `D#` is forbidden in this section.
- `## 🔗 Related use cases` MUST list at least one peer; for a single isolated use case, list `_(none — first of its kind)_`
- A use-case document MUST NOT contain TODO markers or scope-expansion items — surface those in the relevant plan's § Park lot instead
- Renaming a use case (changing priority, order, or slug) MUST update the filename, the H1, and the header blockquote together

## Folder Overview Rules

Every folder containing use-case documents (e.g. `01-freshness/`, `05-reliability/`) MUST contain an overview file named `00-overview.md` with:

1. **Dimension catalog anchor.** A `## 📚 Dimension catalog` section (or equivalent inline reference) linking to [05.07-pe-meta-dimension-catalog.md](../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md) and stating that it is the authoritative definition of every `D#-readable-id` dimension and every `--dim` group referenced in the overview.
2. **Dimensions covered matrix.** A `## 📐 Dimensions covered` section listing every dimension exercised by at least one use case in the folder, in canonical `D#-readable-id` form, with the realizing use case(s) per row.
3. **Primary `--dim` group.** A statement (in prose or table) of the `--dim` group(s) the folder's use cases primarily invoke, with a link to the catalog's *§ Dimension groups (shortcuts)*.

The top-level use-case-set overview (sibling of folder overviews) MUST anchor to the dimension catalog and MUST NOT duplicate the catalog's group definitions inline — it delegates to the catalog.

**Filename rationale:** `00-overview.md` is chosen over `README.md` for two reasons: (1) the `00-` numeric prefix sorts before all `pN-NN-…` use-case files in both case-sensitive and case-insensitive folder listings, and (2) the explicit `-overview` suffix lets instruction files target overviews with a narrower `applyTo` glob (e.g. `**/*usecases/**/00-overview.md`) without matching use-case bodies. The previous `README.md` convention does not satisfy either property.

## Index Rules

`usecase-index.json` (sibling of folder overviews) MUST contain, per entry:

- `dimensions_covered`: array of canonical `D#-readable-id` strings, listing every dimension the use case body declares in its `## 📐 Dimensions covered` section
- The array MAY be `[]` only if the use case body explicitly states no dimensions apply (with rationale); this is exceptional
- The array MUST mirror the use-case body — a divergence is a validation failure

## Naming Decisions

- **The canonical use-case filename is `p<priority>-<order>-<slug>-usecase.md`.** Rationale: the `-usecase` suffix lets a future instruction file target use-case bodies specifically (via `applyTo: '**/*-usecase.md'`) while a separate rule set targets folder overviews (`applyTo: '**/*usecases/**/00-overview.md'`). Without the suffix the only available glob — `**/*usecases/**/*.md` — bundles both file classes under one rule set, which has already caused friction. (Supersedes the v1.1.0 decision in this file that rejected `-usecase.md`; the prior rationale weighed only intra-folder uniqueness and missed the cross-folder `applyTo` targeting use case.)
- **The canonical folder-overview filename is `00-overview.md`** (not `README.md`). The `00-` numeric prefix sorts before all `pN-NN-…` files in both case-sensitive and case-insensitive listings, and the explicit `-overview` part is a stable `applyTo` target. GitHub's auto-rendered README behavior is sacrificed deliberately for these two properties.
- **Dimension references in use-case bodies and folder overviews use the canonical `D#-readable-id` form**, not bare `D#`, per [01-dimids-rename-plan.md](../../src/docs/90.%20Issues/202606/20260601.02-dim-readable-ids/01-dimids-rename-plan.md).

## Quality Checklist

### Use-case document
- [ ] Filename matches `p<priority>-<order>-<slug>-usecase.md`
- [ ] H1 is `# UC-<id>: <title>`
- [ ] Header blockquote has all five required fields in order
- [ ] Priority and breadth are exactly within their allowed value sets
- [ ] Priority and order match the filename
- [ ] Vision anchor resolves to a real principle, rationale, or section
- [ ] All five required sections present in order
- [ ] `## 📐 Dimensions covered` uses canonical `D#-readable-id` form (no bare `D#`)
- [ ] `## 🔗 Related use cases` lists at least one peer (or explicit `_(none)_`)
- [ ] No TODO markers or scope-expansion items

### Folder overview (`00-overview.md`)
- [ ] Filename is exactly `00-overview.md` (not `README.md`)
- [ ] Anchors to [05.07-pe-meta-dimension-catalog.md](../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md)
- [ ] Contains a `## 📐 Dimensions covered` matrix mapping `D#-readable-id` rows to realizing use case(s)
- [ ] States the primary `--dim` group(s) invoked by the folder
- [ ] Does NOT duplicate catalog group definitions inline

### Index (`usecase-index.json`)
- [ ] Every entry has a `dimensions_covered` array
- [ ] Every value uses canonical `D#-readable-id` form
- [ ] Index entries mirror the use-case bodies (no drift)

## References

- **📒** `.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md` — authoritative dimension catalog (35 dimensions, 12 `--dim` groups) anchored by every folder README
- **📒** `06.00-idea/self-updating-prompt-engineering/20260503.02-vision-pe-meta-usecases/05-reliability/p0-01-process-reproducibility-usecase.md` — canonical example
- **📒** `06.00-idea/self-updating-prompt-engineering/20260503.02-vision-pe-meta-usecases/05-reliability/p0-04-domain-coherent-batching-usecase.md` — example with full optional-field set
- **📘** `.github/instructions/vision-amendment.instructions.md` — defines the coverage promise that points to use cases as a landing target
- **📘** `.github/instructions/vision-frontmatter.instructions.md` — declares the principles use cases may anchor against
- **🔗** `src/docs/90. Issues/202606/20260601.02-dim-readable-ids/02-align-dimids-usecases-pemeta-plan.md` — issue plan introducing the v1.1.0 README and index rules
- **🔗** `src/docs/90. Issues/202606/20260601.02-dim-readable-ids/04-usecase-suffix-and-overview-sort-plan.md` — v1.2.0 reversal: adopts `-usecase.md` and renames READMEs to `00-overview.md`
