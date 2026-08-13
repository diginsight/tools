# PE-meta command option applicability matrix

This matrix defines the canonical option taxonomy for all PE-meta commands. Options are organized into 8 classes with deterministic applicability rules (vision v15.9 surface). The eighth canonical parameter is `--plan-file` (plan-artifact location/identity).

## Option classes

| Class | Option | Intent | Applicability |
|---|---|---|---|
| Dimension | `--dim <group\|D#\|full>` | What quality dimensions to evaluate | Universal — all review/update/design/create commands |
| Dependency | `--deps none\|direct\|full\|<N>` | How deep to follow dependency chains | Review + guidance-first + scheduled (pass-through); **not** Design (a not-yet-existing artifact has no dependency closure) |
| Scope | `--scope <type-token\|comma-paths>` | Artifact-type token OR comma-separated paths | Universal — all commands |
| Source | `--source <id>\|<url>[,...]` | Filter monitored sources (or an ad-hoc external `--source <url>`) passed through to researcher | Review + Update + scheduled-review; Design (seed-corpus selection) |
| Window | `--start <date\|version>` / `--end <date\|version>` | Bounded-delta endpoints (value-shape: ISO date OR a source-version token resolved to a timestamp via the source's `version_scheme`); derives `breadth=bounded-delta` | Review + Update; Design (seed-corpus / source-diff window) |
| Mode | `--mode plan\|apply` | Whether to preview or execute changes | Review + guidance-first + Update + Design (`plan` = produce design plan and stop; `apply` = plan + build) |
| Skip | `--skip <stage>[,<stage>...]` | Which pipeline phases or resources to bypass | Review + Update (all stages); Design (per-phase; Phases 0/0a/0b/8 never skippable); type-specific review (research, external only) |
| Plan-file | `--plan-file <path>` | Plan-artifact location/identity (eighth canonical parameter); never decides regenerate-vs-trust | Review + Update + Design (each materializes a plan on every mutating run) |

## Canonical applicability matrix

> **v15.9 surface.** Eight canonical parameters. The standalone **Update** command is consolidated into **Review** per vision v15.9 (`apply = plan + execute`); the Update column is retained for historical flag-mapping only — Review now carries the Update applicability. The **Design** column was reconciled to vision parity on 2026-06-24 (design-review parity plan, OD-1): design carries every parameter except `--deps` (a not-yet-existing artifact has no dependency closure to traverse). Where this diverges from a vision Creation-column `❌`, the divergence is an intentional parity resolution; the vision-text reconciliation is human-only (parked, PL-1).

| Option | Review | Design | Create-update | Scheduled-review | Update | Adherence |
|---|---|---|---|---|---|---|
| `--dim` | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| `--deps` | ✅ | ❌ | ❌ | ✅ (pass-through) | ❌ | ✅ |
| `--scope` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `--source` | ✅ | ✅ (seed-corpus) | ❌ | ✅ | ✅ | ❌ |
| `--start`/`--end` | ✅ | ✅ (seed-corpus window) | ❌ | ✅ | ✅ | ❌ |
| `--mode` | ✅ | ✅ | ❌ | ❌ | ✅ | ✅ |
| `--skip` | ✅ (research, external) | ✅ (per-phase) | ❌ | ✅ (research, external) | ✅ (all stages) | ❌ |
| `--plan-file` | ✅ | ✅ | ❌ | ❌ | ✅ | ❌ |

## Derived breadth (vision v15.9)

Breadth is NOT a user-supplied flag — it is **derived** by the orchestrator from caller-type and window endpoints:

| Caller-type | Window endpoints | Derived `breadth` | Researcher output shape |
|---|---|---|---|
| `manual` | none | `full` | snapshot |
| `trigger-fired` | none | `incremental` | digest (reads per-source state) |
| any | `--start` and/or `--end` present | `bounded-delta` | window-digest (defaults endpoints from `lookback.default_days`) |

**Rule #2 (composition constraint):** `--skip research` combined with derived `breadth=full` is REJECTED via CF-05 — a full sweep that skips research has no inputs to act on. Allowed: `--skip research` with `incremental` or `bounded-delta` (state-only re-validation).

**Value-shape `--start`/`--end` (vision v15.3).** Each window bound takes one of two shapes, resolved deterministically at Phase 0a: a **date** (ISO-8601, `now`, or a relative offset `-Nd`) OR a **source-version token** (e.g. `1.099`) resolved to its publish timestamp via the source's `version_scheme` (`semver`\|`dated`\|`model-version`\|`none`). This mirrors the two-shape `--scope` parser and adds **no new parameter** (`minimal-canonical-surface` preserved). Two guard rails: (1) a version-shaped bound requires a **singleton** `--source` — otherwise rejected via CF-05 (`version window requires a single --source`); (2) a version bound against a `version_scheme: none` source is rejected (`source <id> has no version scheme; use a date window`). A `--start`/`--end` window resolves `breadth=bounded-delta` and **overrides** recorded `pass` coverage inside `[--start, --end]` (the re-baseline / distrust-recovery path).

## Per-artifact prompt invocation matrix

The orchestrator routes each (artifact-type, dim-family) pair to the corresponding type-specific prompt — **never** hand-write per-type branches:

| Artifact type | dim-family=review | dim-family=create-update | dim-family=design |
|---|---|---|---|
| `context` | `/pe-meta-context-review` | `/pe-meta-context-create-update` | `/pe-meta-context-design` |
| `instructions` | `/pe-meta-instructions-review` | `/pe-meta-instructions-create-update` | `/pe-meta-instructions-design` |
| `agents` | `/pe-meta-agents-review` | `/pe-meta-agents-create-update` | `/pe-meta-agents-design` |
| `prompts` | `/pe-meta-prompts-review` | `/pe-meta-prompts-create-update` | `/pe-meta-prompts-design` |
| `skills` | `/pe-meta-skills-review` | `/pe-meta-skills-create-update` | `/pe-meta-skills-design` |
| `hooks` | `/pe-meta-hooks-review` | `/pe-meta-hooks-create-update` | `/pe-meta-hooks-design` |
| `snippets` | `/pe-meta-snippets-review` | `/pe-meta-snippets-create-update` | `/pe-meta-snippets-design` |
| `templates` | `/pe-meta-templates-review` | `/pe-meta-templates-create-update` | `/pe-meta-templates-design` |

## Pipeline phases and `--skip` mapping

| Pipeline phase | Stage name (mapped by `--skip`) | Notes |
|---|---|---|
| Phase 0a | _(conversational pre-parser; never skippable)_ | Resolves free-form intent → canonical 7 params |
| Phase 0a-precondition | _(artifact-type/path consistency check; never skippable)_ | CF-05 on prompt-name-prefix vs positional-path-root mismatch; per-artifact prompts only (orchestrator-level prompts skip) |
| Phase 0b | _(domain coherence check; never skippable; `--skip domain-coherence` REJECTED)_ | Computes seed and dependency footprints separately via metadata-first 3-tier algorithm; emits `bundle=…` on first-line `Resolved invocation:` log |
| Phase 1 (research) | `research` | Cannot be skipped on derived `breadth=full` (rule #2) |
| Phase 1.5 (organizational pass) | `structure` | Gated on `breadth=full` AND multi-file scope |
| Phase 2 (structure audit) | `structure` | Orchestrators only |
| Phase 3 (consistency audit) | `consistency` | Orchestrators only |
| Phase 4 (content audit + per-artifact routing) | `content` | Orchestrators only |
| Phase 6 (apply + outcome-log) | _(not skippable)_ | Writes `.copilot/temp/pe-meta-state/outcomes/<run-id>.jsonl` |
| Phase 8 (report + first-line `Resolved invocation:` log) | _(not skippable)_ | Persists `new_anchors[]` for incremental runs after successful applies |

## `--scope` value shape (formal)

`--scope` accepts EXACTLY ONE of:
1. An **artifact-type token**: `context | instructions | agents | prompts | skills | hooks | snippets | templates | all`
2. A **comma-separated path list**: folders end with `/`, files end with `.md`

**Mixing rejected**: `--scope context,.github/prompts/foo.md` → CF-05 (type token cannot coexist with path entries).

**Phase 0b applies universally to every scope-bearing mechanism.** Once `--scope` has been resolved to a concrete file set — whether by an artifact-type token, a comma-separated path list, a positional `<file-path>` (per-artifact prompts), or the default-all expansion — Phase 0b reads each in-scope file's declared `domain:` frontmatter and computes the seed and dependency footprints. The domain-coherence gate is therefore orthogonal to the scope-bearing mechanism: every command family (orchestrator-level + per-artifact + scheduled) honors Phase 0b regardless of `--mode` value.

## Option detail: `--deps`

| Value | Behavior |
|---|---|
| `none` | No dependency traversal (explicit opt-out) |
| `direct` | First-level dependencies only |
| `full` | Bounded recursive traversal (default depth 5) |
| `<N>` | Explicit numeric depth for fine-grained control |
| _(omitted)_ | Equivalent to `none` in review/adherence; auto-rotation in scheduled-review |

## Option detail: `--scope`

**Universal semantics** — accepted in ALL commands with context-dependent behavior:

| Context | Behavior |
|---|---|
| Orchestrators (`update`, `scheduled-review`) | Selects which artifact types to iterate in the batch |
| Type-specific commands (`{type}-review`, `{type}-design`) | Filters which dependency types to focus on during `--deps` traversal |
| Multi-file review | Selects which file types from the input set to process |

**Valid values:** `all` (default), `context`, `instructions`, `agents`, `prompts`, `skills`, `hooks`, `snippets`, `templates`, or a specific file path.

**Composition with `--deps`:** `--scope` controls WHAT to traverse; `--deps` controls HOW DEEP. They compose orthogonally.

## Option detail: `--mode`

| Value | Behavior |
|---|---|
| `plan` | Full R-B-V analysis but stop before apply; presents designed changes with diffs for review |
| `apply` | Full R-B-V + apply changes (default) |

**Note:** `--mode plan --skip research` is the canonical assessment-only invocation (diagnose without designing or applying changes). It supersedes the former `healthcheck` preset.

**Applicability (vision v15.9 § Option applicability matrix; design row reconciled 2026-06-24 per design-review parity plan OD-1).** `--mode` is accepted by **Review ✅** (`apply` default, `--mode plan` previews), **Guidance-first / Adherence ✅** (`apply` default, `--mode plan` previews), **Update ✅** (consolidated into Review per v15.9), and **Design ✅** (`apply` = produce the design plan then build; `--mode plan` = materialize the design plan and STOP). It is rejected by **Create-update ❌** (always writes) and **Scheduled-review ❌** (delegates execution to sub-commands). This reverses the former "Creation always writes" claim for the Design leg: design now honors the plan-then-execute seam like Review. The vision Creation column still shows `--mode ❌`; that contradiction is parked for a human-only vision-text fix (PL-1). The canonical applicability matrix `--mode` row above reflects the reconciled state.

## Option detail: `--skip`

Named stages map to pipeline phases and cross-cutting resources:

| Stage | Maps to | Available in |
|---|---|---|
| `research` | Phase 1 (source research) | All commands with skip support |
| `external` | Internet/URL fetching in all phases | All commands with skip support |
| `structure` | Phase 2 (structure audit) | Orchestrators only |
| `consistency` | Phase 3 (consistency audit) | Orchestrators only |
| `content` | Phase 4 (content audit) | Orchestrators only |

Multiple stages: `--skip research,structure` or repeated: `--skip research --skip structure`.

## Preset aliases (removed)

Positional preset tokens (`healthcheck`, `performancecheck`, `fullcheck`) are no longer supported. The parser MUST refuse any invocation that uses them with the deterministic error documented in `pe-meta-review.prompt.md` § Argument parsing → Rejected preset tokens.

The canonical replacements are:

| Removed preset | Canonical invocation |
|---|---|
| `healthcheck` | `--mode plan --skip research` |
| `performancecheck` | `--mode apply --dim efficiency --skip research,structure,consistency` |
| `fullcheck` | `--mode apply` (or omit `--mode` — `apply` is the default) |

**Composition rule:** explicit user flags always win; multiple `--skip` lists merge with deduplication.

## Backward-compatible aliases

| Legacy option | Canonical equivalent | Notes |
|---|---|---|
| `--with-deps` | `--deps full` | Retained for compatibility |
| `--with-deps-shallow` | `--deps direct` | Retained for compatibility |
| `--plan` | `--mode plan` | Retained as shorthand |
| `--no-external` | `--skip external` | Retained for compatibility |
| `--no-research` | `--skip research` | Retained for compatibility |
| `--skip-source` | `--skip research` | Retained for compatibility |
| `--skip-structure` | `--skip structure` | Retained for compatibility |
| `--skip-consistency` | `--skip consistency` | Retained for compatibility |
| `--skip-content` | `--skip content` | Retained for compatibility |
| `--incremental` | _(no canonical replacement)_ | **PRESERVED EXCLUSIVELY for trigger-fired callers**. Manual use → CF-05 (see retired-flag table below). |

## Retired v13.x flags (vision v15.9)

The parser MUST be table-driven and reject the following flags with the uniform CF-05 template:

> `<flag> retired in v14; use <v14-replacement> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map`

| Retired flag | v14 replacement |
|---|---|
| `--breadth full\|incremental\|catch-up` | Derived from caller-type + `--start`/`--end` (no flag) |
| `--since <date>` | `--start <YYYY-MM-DD>` |
| `--between <a..b>` | `--start <a> --end <b>` |
| `--area <area>` | `--scope <type-token>` or `--scope <comma-paths>` |
| `--artifact <type>` | `--scope <type-token>` |
| `--consumer <path>` | `--scope <comma-paths>` (path entry) |
| `--subject <topic>` | `--scope <comma-paths>` (specific file path) |
| `--concern <category>` | `--dim <group>` |
| `--mode-review` | `--mode plan` (review intent) or `--mode apply` (default for `*-review` prompts) |
| `--incremental` (on manual callers) | _(no replacement)_ — manual callers always derive `full`; if a bounded sweep is needed, use `--start`/`--end` for `bounded-delta` |

## Canonical command ownership for overlap-prone capabilities

| Capability | Canonical command | Compatibility route | Routing rule |
|---|---|---|---|
| Guidance-first adherence matrix | `/pe-meta-adherence` | Scheduled-review guidance-first rotation | Always route to canonical; never accept `--mode guidance-first` in other commands |
| External-platform reconciliation (release-diff) | `/pe-meta-review --source <url>` | _(none — Release-monitor family retired in vision v15.2)_ | Reconcile against a platform/model release via an Update invocation scoped to an external `--source <url>`, optionally windowed with `--start`/`--end` |

Orchestration narratives are intentionally preserved in orchestration prompts and are not overlap defects:

1. Scheduling and cadence guidance → `/pe-meta-scheduled-review`
2. Lifecycle rotation guidance → `/pe-meta-scheduled-review`
3. External-platform reconciliation guidance → `/pe-meta-review --source <url>`
4. Multi-phase orchestration guidance → `/pe-meta-review`

## Deterministic rejection format

When an option is unsupported, respond with:

```
<option> is not supported by <command> because <missing-capability>. <corrective-action>.
```

Corrective action MUST include one of:
1. Remove the option.
2. Use the canonical command that supports the capability.
3. Split the invocation into supported combinations.

### Rejection examples

| Invalid invocation | Rejection message |
|---|---|
| `pe-meta-review target.md --mode apply` | Valid — this is the default behavior. Review assesses and implements non-breaking improvements autonomously. Use `--mode plan` to opt into assessment-only output. |
| `pe-meta-design desc --deps full` | `--deps is not supported by pe-meta-design because design does not traverse dependencies. Remove --deps or use /pe-meta-review for dependency analysis.` |
| `pe-meta-adherence target.md --dim full` | `--dim is not supported by pe-meta-adherence because adherence uses a fixed dimension set. Remove --dim.` |
| `pe-meta-review target.md --skip structure` | `--skip structure is not supported in type-specific review because phase-level skipping requires the orchestration pipeline. Use --skip research or --skip external, or use /pe-meta-review for phase control.` |
| `pe-meta-review target.md --deps full --deps direct` | `Conflicting --deps values. Specify one: --deps none, --deps direct, --deps full, or --deps <N>.` |

## Command examples by option class

### Dimension scoping

```
/pe-meta-review .github/agents/pe-meta-validator.agent.md --dim structural
/pe-meta-review --mode plan --skip research --dim quality --scope context
/pe-meta-context-review .copilot/context/00.00-prompt-engineering/ --dim freshness
/pe-meta-review .github/agents/ --dim adherence
/pe-meta-review .github/prompts/ --dim reliability --mode plan
```

> **No `--dim` aliases.** Every `--dim` value resolves to exactly one canonical group. Unknown values (including the retired `robustness` name) are rejected via CF-05 with the full canonical enumeration. `--dim adherence` covers consumer-correctness dimensions (`D5-boundaries`, `D6-consistency`, `D16-adherence`, `D18-coverage`); `--dim reliability` covers system-reliability dimensions (`D28-reproducibility` through `D35-portability-boundary`). See `.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md` for the full dimension group inventory.

### Dependency traversal

```
/pe-meta-review .github/agents/pe-meta-validator.agent.md --deps full
/pe-meta-prompt-review pe-meta-review.prompt.md --deps direct --scope context
/pe-meta-scheduled-review --deps full --scope agents
```

### Scope filtering

```
/pe-meta-review --mode apply --scope context,instructions
/pe-meta-prompt-review pe-meta-review.prompt.md --scope context --deps full
/pe-meta-agent-review pe-meta-validator.agent.md --scope instructions --deps direct
```

### Mode control

```
/pe-meta-review --mode plan --scope agents
/pe-meta-review --source <url> --mode plan
```

### Pipeline stage skipping

```
/pe-meta-review --mode apply --skip research,structure --scope prompts
/pe-meta-review --mode apply --skip external --dim quality
/pe-meta-context-review .copilot/context/ --skip research
```

### Canonical invocations replacing removed presets

```
/pe-meta-review --mode plan --skip research --scope context --dim freshness
/pe-meta-review --mode apply --dim efficiency --skip research,structure,consistency --scope agents,prompts
/pe-meta-review --mode plan
```

<!--
prompt_metadata:
  version: "1.1.0"
  last_updated: "2026-06-24"
-->
