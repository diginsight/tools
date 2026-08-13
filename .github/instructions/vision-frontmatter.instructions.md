---
description: Schema and discipline for vision documents — prioritized `scope.covers` block, `principles:` block with optional rationale and relies_on, body principle/rationale conventions, and Most recent changes block size cap
applyTo: '06.00-idea/**/*vision*.md'
domain: "prompt-engineering"
goal: "Make vision invariants (scope items AND principles) explicit, prioritized, and auditable, and keep vision documents readable by capping prose that tends to grow unbounded across versions"
rationales:
  - "Implicit principles get silently absorbed by amendment plans (see 20260525.03-staleness-review/05-vision-usecase-plan-rules/01-overview.md)"
  - "A declared list lets the vision-amendment instruction file run a mechanical principle-impact check"
  - "Three-tier priority (P0/P1/P2) lets the gate scale friction to amendment risk"
  - "Scope items can be invariant too — a flat `scope.covers` list at parity with aspirational items hides which capabilities define the system; per-item priority makes invariance explicit on both sides (scope AND principles) and lets the amendment gate enforce parity rules across both"
  - "A separate frontmatter `rationales:` block duplicates body rationale content and creates two unsynchronized lists — inlining rationale on a per-principle basis (`rationale:` field) collapses the duplication while preserving the why-justification where it matters"
  - "Rationale IDs referenced by `relies_on:` must be readable in isolation — opaque prefix codes (e.g. `R-L4`) force every reader to cross-reference; readable kebab-case IDs (e.g. `external-knowledge`) carry the semantic in the identifier itself"
  - "The `Most recent changes` block accretes detail every version and quickly becomes unreadable — a hard size cap forces overflow into the canonical changelog where it belongs"
  - "Block presence is SHOULD not MUST — the P0-count rule MUST therefore be conditional on presence, otherwise validators flag a false contradiction on documents that legitimately omit the block"
  - "Per-version history kept in two places (a changelog file AND a vision-metadata changes: array) drifts out of sync — the array stopped at v15.5 while the vision was at v15.6; a single-source-of-truth rule for the changelog file removes the drift class"
  - "An unstructured changelog (version-only headings, prose-narrated P0 accounting, no retention rule) is neither deep-linkable nor auditable nor bounded — structured entries, full-SemVer headings, stable anchors, and a retention policy make it machine-readable and growth-bounded"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Vision Document Rules

## Purpose

This file governs five aspects of vision documents (and, via the `applyTo` glob, the sibling `*.changelog.md` file):

1. **The `scope.covers:` frontmatter block** — names the capabilities the vision commits to, each tagged with priority (P0/P1/P2/aspirational). Per-item priority is the amendment gate's primary signal for which capabilities define the system vs. which are mechanism or aspirational.
2. **The `principles:` frontmatter block** — names the prescriptive invariants the vision commits to, each tagged with priority and optionally carrying inline `rationale:` and `relies_on:` cross-references. Downstream amendment plans (governed by `vision-amendment.instructions.md`) tag every goal item with its principle impact. The block is currently SHOULD (not MUST); when it is absent the amendment gate is silently skipped — amendments still land, but principle-impact tagging is not enforced. Future promotion of the block to MUST is tracked separately.
3. **Body rationale identifier convention** — rationale IDs referenced from `relies_on:` and from body cross-references MUST be readable kebab-case identifiers (no opaque prefix codes).
4. **Body conventions for sections that tend to grow unbounded** — currently the `Most recent changes` block, which accretes detail every version and quickly becomes unreadable if uncapped.
5. **The sibling `*.changelog.md` file** — the single source of truth for per-version vision history: heading grain, entry shape, SemVer bump rule, `## Unreleased` staging, retention/archival, ordering, and reference integrity (see § Changelog File Rules).

This file does NOT define amendment-plan rules — those live in `vision-amendment.instructions.md`.

## Required Block Shape — `scope.covers`

```yaml
scope:
  covers:
    - id: <kebab-case-id>           # unique within the block
      priority: P0 | P1 | P2 | aspirational   # see semantics below
      item: "<short capability statement>"     # what the vision commits to delivering
  excludes:
    - "<flat string — priority not used on excludes>"
```

## Required Block Shape — `principles`

```yaml
principles:
  - id: <kebab-case-id>           # unique within the block
    priority: P0 | P1 | P2         # see semantics below
    statement: "<one sentence>"    # what the principle commits to
    rationale: "<one sentence>"    # OPTIONAL — inline justification when not self-evident
    relies_on: [<id>, <id>, ...]   # OPTIONAL — readable kebab-case IDs of body rationale entries
```

The separate top-level frontmatter `rationales:` block (vN ≤ 14 convention) is RETIRED — per-principle inline `rationale:` plus body rationale section together cover the same surface without duplication. Vision documents created after v1.3.0 of this rule MUST NOT carry a top-level `rationales:` block; existing documents SHOULD migrate at the next version bump.

## Priority Semantics — applies to BOTH `scope.covers` and `principles`

| Priority | Meaning | Amendment friction |
|---|---|---|
| **P0** | Core — removing or weakening this changes what the vision IS | MUST bump vision version + explicit consent line on every amendment-plan item that touches it |
| **P1** | Strategic choice — the vision could have been built differently, but this choice is load-bearing for current implementations | MUST add changelog entry + justification line on every amendment-plan item that touches it; no version bump required |
| **P2** | Operational detail — small enough that changing it does not invalidate downstream artifacts | Free to change; no ceremony |
| **aspirational** | (`scope.covers` only) Declared scope item that is NOT yet implemented | Treated as not-shipped: MUST NOT be cited as a load-bearing dependency by other artifacts; promotion to P0/P1/P2 follows the same friction as adding a new item at that priority |

## Rules

### Rules for `principles:` block

Unless otherwise noted, every rule below is conditional on the `principles:` block being present. When the block is absent, only the first rule applies.

- Vision documents SHOULD declare a `principles:` block (initial release; promotion to MUST tracked separately). When the block is absent, the remaining rules in this section do not apply and the amendment gate is silently skipped
- When a `principles:` block is present, each entry MUST carry `id`, `priority`, `statement` — all three populated; `rationale` and `relies_on` are OPTIONAL
- Every `id` MUST be unique within the block and kebab-case
- Every `statement` MUST be a single sentence (one period, one main verb)
- When present, `rationale` MUST be a single sentence (same shape rule as `statement`)
- When present, `relies_on` MUST be a YAML flow sequence of readable kebab-case IDs — every referenced ID MUST resolve to an entry in the body `## ⚙️ The rationale` section (or equivalent rationale registry section)
- `priority` MUST be exactly one of `P0`, `P1`, `P2` — no other values (the `aspirational` tier is `scope.covers`-only)
- When a `principles:` block is present, it MUST contain between 3 and 7 P0 entries — broader interpretation invalidates the gate; narrower interpretation makes everything a P0. If the vision genuinely has fewer than 3 P0 principles, see the floor-handling rule under Bootstrap and Migration
- The `principles:` block is the ONLY authoritative source for vision principles — the body MUST NOT contradict it; body principle headings (e.g. `### Staleness avoidance first`) MUST include a `**Priority: P0**` (or `P1`/`P2`) line that matches the frontmatter
- Adding, removing, or repriotising a P0 entry MUST be accompanied by a vision version bump
- Adding, removing, or repriotising a P1 entry MUST be accompanied by a changelog entry; no version bump required
- P2 entries change freely; no special ceremony
- Initial declaration of the block (naming existing implicit principles) is NOT a version bump — declaration ≠ amendment
- Body text MAY reference principle ids using inline code (`` `portable` ``) so amendment plans can grep for impact

### Rules for `scope.covers:` block

Unless otherwise noted, every rule below is conditional on `scope.covers:` using the per-item priority shape. Flat string lists (vN ≤ 14 convention) are accepted during a migration window but SHOULD be migrated at the next version bump.

- When `scope.covers:` uses the per-item priority shape, each entry MUST carry `id`, `priority`, `item` — all three populated
- Every `id` MUST be unique within the block and kebab-case
- Every `item` MUST be a short capability statement (one sentence or noun phrase)
- `priority` MUST be exactly one of `P0`, `P1`, `P2`, `aspirational`
- `scope.covers.p0` count MUST be between 3 and 7 (same band rationale as principles)
- An entry marked `aspirational` MUST NOT be referenced by any other artifact as a load-bearing dependency (consumer artifacts that grep for it MUST be flagged); promotion to `P0`/`P1`/`P2` is treated as an addition at that priority
- Adding, removing, or repriotising a `P0` cover entry MUST be accompanied by a vision version bump (parity with P0 principles)
- Adding, removing, or repriotising a `P1` cover entry MUST be accompanied by a changelog entry; no version bump required
- `P2` and `aspirational` entries change freely; no special ceremony
- The `relies_on` field is NOT permitted on `scope.covers` entries at this version (deferred to future revision)

### Rules for rationale identifiers (body `## ⚙️ The rationale` section)

- Rationale entry IDs MUST be readable kebab-case identifiers — opaque prefix codes (e.g. `R-L4`, `R-P1`, `R-S1`, `R-G1`) are NOT permitted in new or revised vision documents
- Each rationale entry MUST be uniquely identifiable by its kebab-case ID without requiring cross-reference (e.g. `external-knowledge`, `metadata-driven`, `autonomy-gradient`, `domain-coherent-batching`)
- `relies_on:` in the `principles:` block, and any inline body cross-reference, MUST use the same kebab-case IDs
- Existing documents using opaque prefix codes SHOULD migrate at the next version bump; partial migration (mixed prefix-coded and kebab-case IDs) is NOT permitted within a single document

## Bootstrap and Migration

Vision documents created before this rule existed SHOULD acquire the block via a single in-version amendment that names existing principles without changing them. The bootstrap is recorded in the changelog as additive; no version bump.

Where the bootstrap cannot honestly name an existing principle (the principle was implicit but never operationalized), the candidate is DROPPED from the bootstrap rather than declared as a fiction — and proposed as a fresh P0 in a later version bump.

### Floor-handling — fewer than 3 genuine P0 principles

The 3-to-7 P0 band assumes the vision has matured enough to express at least three load-bearing invariants. If, after honest enumeration, the vision genuinely has fewer than 3 P0 candidates:

- DO NOT inflate the block by promoting P1 candidates to P0 just to meet the floor — that defeats the gate's purpose
- DO NOT declare a `principles:` block with fewer than 3 P0 entries — the gate cannot meaningfully run on it
- INSTEAD, omit the block entirely and record in the vision changelog the reason for deferral ("vision has <3 P0 invariants at this version; principles block deferred to vN+1")
- The amendment gate is silently skipped for that version (see Purpose); the vision owner accepts the loss of mechanical principle-impact tagging until the principle set matures

## How Amendment Plans Consume This Block

Amendment plans matched by `vision-amendment.instructions.md` (`*vision*plan*.md`) read this block to perform per-item principle-impact tagging. The exact format of that tagging lives in `vision-amendment.instructions.md`.

## Tracking-Metadata Placement

A vision document is an article. Its **change-prone tracking metadata** — `version`, `last_updated`, and the optional `changelog:` pointer — lives in the **bottom `article_metadata:` block**, exactly like every other article and like every dual-block PE artifact (per `00.03-metadata-contracts.md`). It MUST NOT be carried in top frontmatter.

- The top YAML frontmatter holds only invariant discovery/config fields (`title`, `author`, `date`, `status`, `domain`, `categories`, `goal`, `description`) plus the governance blocks (`scope.covers:`, `principles:`). These are author/design-owned.
- `version`/`last_updated` are NOT a vision-specific exception: the same uniform rule applies, so there is no top/bottom duplication to drift. The version-bump rules above still apply — they govern *when* the version changes, not *where* it is stored.
- The `scope.covers:`/`principles:` blocks stay in top frontmatter because the amendment gate reads them there; tracking fields are not part of that gate.

## Body Conventions

### `Most recent changes` block — size cap

Vision documents typically open with a blockquote summarizing the latest version's changes, placed just before `## 🎯 The problem`. This block is a TLDR for orientation — NOT a release-notes substitute. The canonical changelog lives in the sibling `*.changelog.md` file referenced from the block's tail.

**Rules:**

- When a `§ Scope note` blockquote exists immediately after `## 🎯 The problem`, the `Most recent changes` block MUST NOT exceed **twice its word count**
- When no `§ Scope note` blockquote exists, the fallback cap is **80 words** — the same target the rule was calibrated against (a typical scope note is 40–80 words; 2× yields the same ceiling)
- Word count excludes the bold lead-in `**Most recent changes (vN, YYYY-MM-DD).**` and the trailing `📜 Full changelog history:` link line
- When a release introduces more material than fits the cap, the overflow MUST be moved into the changelog file — only the headline-level summary stays in the vision
- The block SHOULD be a single paragraph; multi-paragraph form is allowed only if the total word count still respects the cap
- When the scope note is rewritten and shrinks, any existing changes block that now exceeds the cap MUST be trimmed in the same edit
- Authors SHOULD prefer noun-phrase bullets over dense subordinate-clause prose when nearing the cap (the cap is a hard ceiling, not a writing-style preference)

## Changelog File Rules

The sibling `*.changelog.md` file (matched by this instruction's `applyTo` glob) is the **single source of truth** for per-version vision history. These rules govern its structure so it stays deep-linkable, auditable, and growth-bounded.

### History lives only in the changelog

- Per-version history lives ONLY in the sibling `*.changelog.md`. The vision document's frontmatter, body, and bottom `article_metadata:` block MUST NOT carry a parallel per-version `changes:` list, release-notes array, or equivalent. (Such a list duplicates the changelog and drifts: the retired `article_metadata.changes:` array stopped at v15.5 while the vision was already at v15.6.)
- The vision's `Most recent changes` block (capped per § Body Conventions) is the ONLY in-vision change summary — a TLDR pointer to the changelog, never a history.

### Entry heading grain

- Every released entry MUST use a full SemVer + date heading: `## vMAJOR.MINOR.PATCH — YYYY-MM-DD` (e.g. `## v15.6.0 — 2026-06-12`).
- Two-part (`## v15`) or version-only (`## v14`) headings are NOT permitted — they hide which patch/minor a change belongs to and cannot be deep-linked unambiguously.
- One heading = one released version. A single heading MUST NOT bucket multiple dated sub-releases; split them into one heading per version.

### Entry shape (structured + machine-readable)

Each released entry MUST open with, in order:

1. A machine-readable HTML comment on one line:
   `<!-- entry: { version: "X.Y.Z", date: "YYYY-MM-DD", priority_touch: "P0|P1|P2|none", version_bump: "major|minor|patch|none", p0_touched: true|false, companion_plan: "<filename-or-null>" } -->`
2. A visible status line: `**Priority touch:** <P0|P1|P2|none> · **Version bump:** <major|minor|patch> (X.Y.(Z−1) → X.Y.Z) · **P0 touched:** <yes|no>`
3. A bold one-sentence headline summarizing the release.
4. Per-change bullets, each naming the touched principle / `scope.covers` id and its priority touch.

The `priority_touch` / `p0_touched` fields make the version-bump and autonomy rules auditable rather than narrated in closing prose.

### SemVer bump rule

The vision uses semantic versioning. The bump is determined by the highest-impact change in the release:

| Bump | Trigger |
|---|---|
| **MAJOR** (`X+1.0.0`) | A breaking change to a P0 invariant — removing/weakening a P0 principle or P0 `scope.covers` item, or a goal/scope/boundary contract change that invalidates downstream artifacts |
| **MINOR** (`X.Y+1.0`) | An additive or strategic change — a new P1 principle/scope item, a new capability, or a P1 touch that does not break existing consumers |
| **PATCH** (`X.Y.Z+1`) | Alignment, wording, body-only clarification, or a P2 change that does not alter the contract |

This mirrors the P0/P1/P2 amendment-friction rules above: a P0 touch MUST bump (≥ MINOR, MAJOR if breaking) and carry a consent line; a P1 touch MUST add a changelog entry; P2 changes need no ceremony.

### `## Unreleased` staging section

- The changelog SHOULD keep a `## Unreleased` section immediately below the intro, above the newest released version.
- In-progress amendment plans accumulate notes there before a version is cut.
- On release, the `## Unreleased` heading is renamed to the cut `## vX.Y.Z — YYYY-MM-DD` heading and a fresh empty `## Unreleased` (containing `_Nothing staged._`) is left in its place.

### Retention and archival

- Keep full per-entry detail for the **current major** version only.
- When a new major is cut, prior-major detail MUST be moved to a sibling `*.changelog.archive.md` (one per archived major, or a single growing archive), leaving a one-line pointer in the live changelog.
- Pre-changelog versions (superseded before the changelog was extracted) are summarized in a single `## Earlier versions` section; they are not expanded.
- This bounds the changelog's growth the same way the `Most recent changes` word-cap bounds the vision body.

### Ordering and reference integrity

- Entries MUST be ordered newest-first (`## Unreleased`, then descending version).
- Every provenance link (companion plan, motivating overview) MUST resolve; the changelog MUST be included in the repository link-check pass (`scripts/check-links-enhanced.ps1`).
- Any cross-artifact reference INTO a changelog section MUST target a stable HTML anchor (`<a id="…"></a>`) placed above the heading, never the heading's auto-generated slug — so a heading reword cannot break downstream deep-links.

## Quality Checklist

### `principles:` block

- [ ] `principles:` block present in frontmatter (or, if absent, deferral reason recorded in changelog)
- [ ] If block is present: every entry has `id`, `priority`, `statement`
- [ ] If block is present: every `id` unique and kebab-case
- [ ] If block is present: every `statement` is a single sentence
- [ ] If block is present: every entry that carries `rationale` keeps it to a single sentence
- [ ] If block is present: every `relies_on:` ID resolves to a body rationale entry and uses readable kebab-case (no `R-X#` prefix codes)
- [ ] If block is present: P0 count is between 3 and 7
- [ ] If block is present: body principle headings include a matching `**Priority: …**` line
- [ ] If block is present: body text does not contradict any declared statement
- [ ] If body uses `` `principle-id` `` references, every referenced id exists in the block

### `scope.covers:` block

- [ ] `scope.covers:` uses the per-item priority shape (or migration deferral noted in changelog)
- [ ] If new shape is used: every entry has `id`, `priority`, `item`
- [ ] If new shape is used: every `id` unique and kebab-case
- [ ] If new shape is used: `scope.covers.p0` count is between 3 and 7
- [ ] If new shape is used: no entry carries `relies_on` (deferred at this version)
- [ ] `aspirational` entries are not referenced by any consumer artifact as a load-bearing dependency

### Top-level `rationales:` block

- [ ] No top-level frontmatter `rationales:` block (retired — content lives in body `## ⚙️ The rationale` and in per-principle `rationale:` lines)

### Body rationale section

- [ ] Every rationale entry ID is readable kebab-case (no `R-X#` prefix codes)
- [ ] Document does not mix prefix-coded and kebab-case IDs (full migration required when migrating)

### `Most recent changes` block

- [ ] Word count ≤ cap (2 × scope-note word count, or 80 words if no scope note exists; excluding lead-in and changelog-link tail)

### Tracking-metadata placement

- [ ] `version`/`last_updated` live in the bottom `article_metadata:` block, NOT in top frontmatter (no duplication across blocks)

### Changelog file (`*.changelog.md`)

- [ ] Vision document carries NO parallel per-version `changes:` list (history lives only in the changelog)
- [ ] Every released entry heading is full SemVer + date (`## vX.Y.Z — YYYY-MM-DD`); no version-only or two-part headings
- [ ] No heading buckets multiple dated sub-releases
- [ ] Newest entries carry the machine-readable `<!-- entry: { … } -->` comment and visible `**Priority touch:** … · **Version bump:** … · **P0 touched:** …` status line
- [ ] Version bump matches the SemVer bump rule (MAJOR=breaking/P0 · MINOR=additive/P1 · PATCH=alignment)
- [ ] A `## Unreleased` staging section exists below the intro
- [ ] Entries are ordered newest-first
- [ ] Cross-artifact references into changelog sections use a stable HTML anchor, not an auto-generated heading slug
- [ ] Current-major detail is full; prior-major detail is archived per the retention rule

## References

- **📒** `06.00-idea/self-updating-prompt-engineering/20260531.01-vision.md` — first vision to carry the block
- **📒** `src/docs/90. Issues/202605/20260525.03-staleness-review/05-vision-usecase-plan-rules/01-overview.md` — sub-issue analysis that motivated this rule
- **📘** `.github/instructions/vision-amendment.instructions.md` — consumer of this block
- **📘** `.github/instructions/pe-instruction-files.instructions.md` — instruction-file schema authority

<!--
instruction_metadata:
  version: "1.5.0"
  last_updated: "2026-06-12"
-->
