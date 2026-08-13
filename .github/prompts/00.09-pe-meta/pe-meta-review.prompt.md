---
name: pe-meta-review
description: "Unified Prompt Engineering Artifact Management — 9-phase pipeline (Phase 0a conversational pre-parser, Phase 1 research, Phase 1.5 organizational pass, Phases 2–4 audits each with research-build-validate, Phases 5–8 approval/apply/regression/report) under the vision v15.4 default-full invocation contract and eight-parameter canonical option surface. `--mode apply` always materializes a plan then executes it (one execution engine); `--mode plan` materializes the plan and stops. Parameter-less manual invocations are a deliberate full sweep; trigger-fired invocations are incremental; explicit `--start`/`--end` produces bounded-delta. Pipeline phases (structure / consistency / content) are independently disableable via `--skip`. Execution depth controlled by `--mode`; plan-artifact location by `--plan-file`. Phase 4.5 adds strategic + relational-quality validation (vision alignment, category-reference compliance, PE quality bar, N-1 separation, self-update readiness, ecosystem-impact) when `--dim` includes the vision-alignment or relational-quality dimensions (D15–D19, i.e. `--dim strategic` or `--dim quality`). Design/review parity: review validates artifacts against the same six guidance-quality properties (clarity, non-redundancy, non-contradiction, completeness, prioritization, actionability) that the design prompts construct toward — role differs by process and mutation permission, not quality ambition."
agent: agent
model: claude-opus-4.6
tools:
  - read_file
  - grep_search
  - file_search
  - list_dir
  - semantic_search
  - fetch_webpage
  - replace_string_in_file
  - create_file
handoffs:
  - label: "Research PE Impact"
    agent: pe-meta-researcher
    send: true
  - label: "Design Changes"
    agent: pe-meta-designer
    send: true
  - label: "Validate Ecosystem"
    agent: pe-meta-validator
    send: true
  - label: "Apply Optimizations"
    agent: pe-meta-optimizer
    send: true
agents: ['*']
argument-hint: '[--mode plan|apply] [--scope <artifact-type-token>|<path>[,<path>...]] [--source <source-id>|<url>[,...]] [--dim <group|D#>] [--start <date|version>] [--end <date|version>] [--deps none|direct|full|<N>] [--skip <stage>[,<stage>...]] [--plan-file <path>] [bundle=accept]'
goal: "Orchestrate PE artifact management across 9 phases under the vision v15.4 default-full invocation contract and eight-parameter canonical option surface (default-full-investigation, minimal-consistent-option-surface); `--mode apply` always materializes a plan then executes it via one execution engine"
scope:
  covers:
    - "End-to-end pipeline with the eight canonical parameters per vision v15.4 — `--mode`, `--scope`, `--source`, `--dim`, `--start`/`--end`, `--deps`, `--skip`, `--plan-file`"
    - "Default-full invocation contract (default-full-investigation): parameter-less manual invocations run a full sweep; trigger-fired invocations run incremental; explicit `--start`/`--end` produces bounded-delta"
    - "Value-shape `--scope` parser: a single artifact-type token (`context|instructions|agents|prompts|skills|hooks|snippets|templates|all`) OR a comma-separated set of paths (folders end `/`, files end `.md`)"
    - "Phase 0a conversational pre-parser: free-form scoping intent (subjects, concerns, consumer chains) is resolved into the eight canonical parameters BEFORE strict parsing; phases 1–8 only ever consume canonical options"
    - "Phase 0a precondition — artifact-type/path consistency check (CF-05): per-artifact prompts encode an expected artifact-type root; mismatched positional paths or `--scope` values are rejected before Phase 0b with the canonically-correct prompt name suggested. CF-05 operates on artifact-type ROOT (deterministic from path), NOT on semantic domain (declared in frontmatter)"
    - "Phase 0b — domain coherence check: deterministic, non-skippable step between Phase 0a and Phase 1; computes seed footprint and dependency footprint SEPARATELY using the metadata-first 3-tier domain resolution algorithm (declared `domain:` frontmatter → optional per-repo `pe-domain-map.yaml` heuristic → `unknown`); emits one of `bundle=single-domain` | `bundle=cross-domain-deps` | `bundle=multi-domain-gated` | `bundle=accepted-bundle` | `bundle=multi-domain-advisory`"
    - "Metadata-first 3-tier domain resolution: Tier 1 declared `domain:` frontmatter wins; Tier 2 optional per-repo `pe-domain-map.yaml` path-slug heuristic; Tier 3 `unknown` fallback. Heuristic NEVER overrides declared metadata. Per-file `domain-source=declared|path-heuristic|unknown` surfaced in Phase 8 report"
    - "Seed footprint vs dependency footprint decision matrix: seed=1 ∧ deps adds 0 → `bundle=single-domain`; seed=1 ∧ deps adds ≥1 → `bundle=cross-domain-deps` (one review, per-dep-domain specialized lenses, NO split); seed≥2 → `bundle=multi-domain-gated` per domain-coherent-batching (numbered split proposal)"
    - "`bundle=accept` consent token: single-keystroke bypass for the multi-domain gate; only valid consent value; recorded on first-line `Resolved invocation:` log as `bundle=accepted-bundle`"
    - "Per-artifact prompt invocation matrix: `(--scope-resolved-artifact-type, --dim) → pe-meta-{type}-{review|create-update|design}` selection"
    - "Pipeline phases / `--skip` mapping including rule #2: `--skip research` is INCOMPATIBLE with derived `breadth=full` UNLESS `--plan-file` references a validated baseline plan (which substitutes for research — trust mode)"
    - "`--plan-file <path>` (eighth canonical parameter): plan-artifact location/identity ONLY; never decides regenerate-vs-trust; default auto-name `<run-folder>/<NN>-<kebab-name>.plan.md`, fallback `.copilot/temp/pe-meta-state/plans/YYYYMMDD-HHMMSS-<kebab-name>.plan.md`"
    - "Always plan-then-execute: `--mode apply` materializes/reconciles a plan (Phases 1–4) then executes it (Phases 5–7) via one execution engine; fresh / reconcile / trust execution modes resolved from (baseline available?) × (research runs?); drift guard required only in cross-run trust mode"
    - "Model-routing seam: Phases 1–4 (research/plan/reconcile) on a reasoning-grade model, Phases 5–7 (execution) on a standard/cheaper model, realized via each delegated meta-agent's own `model:` field"
    - "Phase 1.5 Organizational Pass when derived `breadth=full` AND resolved `--scope` is broader than a single file"
    - "First-line `Resolved invocation:` log emitted before Phase 1 runs AND echoed in the Phase 8 report — always includes the resolved `bundle=…` marker"
    - "Phase 4.5 strategic validation (runs when `--dim` includes the vision-alignment dimension D15-vision-alignment under `--dim strategic`, OR a relational-quality dimension — D16-adherence, D17-cross-coherence, D18-coverage, D19-artifact-structure — under `--dim quality`): vision alignment, category-reference compliance (Level 1.5), PE quality bar, N-1 separation audit, self-update readiness, ecosystem-impact scoring; advisory severity channel separate from structural PASS/FAIL"
    - "Design/review parity contract: review validates against the SAME six guidance-quality properties the design prompts construct toward (clarity, non-redundancy, non-contradiction, completeness, prioritization, actionability); role differs by process and mutation permission, not quality ambition"
  excludes:
    - "Vision document changes (human-only)"
    - "Individual artifact creation (per-artifact `pe-meta-{type}-{create-update|design}` prompts handle this)"
boundaries:
  - "MUST parse the eight canonical parameters (`--mode`, `--scope`, `--source`, `--dim`, `--start`/`--end`, `--deps`, `--skip`, `--plan-file`) and REJECT all other `--*` flags with CF-05 deprecation notices"
  - "MUST derive `breadth` (`full`|`incremental`|`bounded-delta`) per vision v14 rule #2: manual + no window → `full`; trigger-fired + no window → `incremental`; any `--start`/`--end` → `bounded-delta`. MUST NOT accept `--breadth` as a flag"
  - "MUST run Phase 0a (conversational pre-parser) before strict parsing whenever any non-canonical token appears; canonical resolution MUST be echoed to the caller BEFORE Phase 1 runs"
  - "MUST run Phase 0a precondition (artifact-type/path consistency check, CF-05) at the END of Phase 0a, BEFORE Phase 0b. The rejection error MUST name the expected root and suggest the canonically-correct prompt name. CF-05 operates on artifact-type ROOT, NOT on semantic domain"
  - "MUST run Phase 0b (domain coherence check) between Phase 0a and Phase 1 on every invocation. Phase 0b is NOT skippable; `--skip domain-coherence` is REJECTED with CF-05"
  - "MUST compute seed footprint and dependency footprint SEPARATELY in Phase 0b; MUST emit one of the five `bundle=…` markers; MUST echo the marker on the first line of the `Resolved invocation:` log"
  - "MUST accept `bundle=accept` as the ONLY consent token for the multi-domain gate; MUST REJECT all other `bundle=…` values with CF-05 (closed set: only `accept`)"
  - "MUST NOT treat `--dim` as a domain override — `--dim` is a dimension-group selector per v14 (filters which audit dimensions Phase 2–4 exercises); domain footprint is always computed from per-file `domain:` metadata regardless of `--dim` value"
  - "MUST select the per-artifact prompt via the invocation matrix from `(--scope-resolved-artifact-type, --dim)`; MUST NOT hand-write per-artifact-type branches"
  - "MUST reject `--skip research` when derived `breadth=full` with CF-05 (vision v15.4 § Pipeline phases and `--skip` mapping, rule #2) UNLESS `--plan-file` references a validated baseline plan — in which case the baseline substitutes for research (trust mode) and the cross-run drift guard is REQUIRED"
  - "MUST emit a machine-parseable first-line `Resolved invocation: --mode=… --scope=… --source=… --dim=… --start=… --end=… --deps=… --skip=… --plan-file=… | breadth=… | caller=… | bundle=…` log identical across plan and apply modes"
  - "MUST delegate to meta-agents (researcher, designer, validator, optimizer) and per-artifact-type prompts; never duplicate their logic inline"
  - "MUST accept `--incremental` ONLY as a single-window deprecation alias for trigger-fired callers (resolves to derived `breadth=incremental`); MUST reject it with CF-05 for manual callers because acceptance would violate default-full-investigation"
  - "MUST run Phase 4.5 (strategic validation) ONLY when `--dim` includes the vision-alignment dimension (D15-vision-alignment, under `--dim strategic`) OR a relational-quality dimension (D16-adherence, D17-cross-coherence, D18-coverage, D19-artifact-structure, under `--dim quality`) AND Phases 2–4 completed PASS; if any structure/consistency/content audit FAILs, Phase 4.5 is SKIPPED and its findings are marked advisory-only on a severity channel separate from structural PASS/FAIL"
  - "MUST hold review to the SAME six guidance-quality properties the design prompts construct toward (design/review parity); autonomous review improvements MUST NOT lower quality and MUST NOT relax any property the design side enforces"
rationales:
  - "default-full-investigation — parameter-less manual invocations must be a deliberate full sweep, not a silent narrowing; strategies are subtractive"
  - "minimal-consistent-option-surface — collapsing the surface to eight parameters keeps semantics consistent across phases; every additional flag risks overlap or silent re-interpretation downstream"
  - "Breadth is a *derived* attribute (not a flag) so the same logic resolves it identically from caller-type and window across plan and apply runs"
  - "Value-shape `--scope` absorbs `--area`/`--artifact`/`--consumer` into one parameter with two unambiguous shapes (artifact-type token OR paths)"
  - "Phase 0a isolates free-form, LLM-mediated scoping resolution from strict parsing so phases 1–8 always consume canonical options"
  - "Per-artifact prompt invocation matrix removes the need to branch on artifact-type inline — the orchestrator only routes; per-type prompts own the work"
  - "Rule #2 (`--skip research` incompatible with derived `breadth=full`) preserves the default-full contract: a full sweep without research is structurally meaningless — EXCEPT when `--plan-file` references a validated baseline, because the plan IS the prior research product (trust mode), making the skip a principled exception rather than a silent narrowing"
  - "First-line `Resolved invocation:` log is the observable proxy for the default-full contract — every run reveals what was actually executed before any side effects occur"
  - "Strategic validation runs AFTER structural passes (Phase 4.5) so structural findings are available as context, on a separate severity channel — a structural FAIL must not silently masquerade as a strategic advisory, and a strategic advisory must not block a structurally-clean artifact"
  - "Design and review share ONE quality objective and ONE scope intent, filtered only by artifact applicability; a review that validated against a weaker bar than design constructs toward would let drift accumulate exactly where design just certified quality"
---

# Prompt Engineering Artifact Management

Unified orchestrator for PE artifacts under the vision v15.4 default-full invocation contract and eight-parameter canonical option surface. This is the single review/optimization command — it absorbs strategic validation (formerly a separate strategic reviewer) as a conditional Phase 4.5 and holds review to the same guidance-quality bar the design prompts construct toward (design/review parity). Phase 0a resolves free-form input to canonical options, the parser validates the eight canonical parameters, breadth is derived, phases 1–8 execute (with Phase 4.5 inserted when `--dim` includes strategic dimensions), and the first-line `Resolved invocation:` log makes the actual execution observable before any side effects. `--mode apply` always materializes a plan then executes it through one execution engine.

> **v15.4 alignment.** This prompt honors vision v15.4.0 § Plan output contract (a plan file is materialized on **every** mutating run — both `--mode plan` and `--mode apply`; `apply` additionally executes it through one execution engine — see [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md)), § Plan execution modes (fresh / reconcile / trust off two booleans), § Model-routing seam (Phases 1–4 reasoning model, Phases 5–7 standard model), and § Iteration budget (overflow spillover plan — see [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md)). The full retired-flag → v14 destination map lives in the [vision v15 changelog § Historical: v13 → v14 deprecated flag map](../../../06.00-idea/self-updating-prompt-engineering/20260531.01-vision.changelog.md#historical-v13-v14-flag-map) and in the parser test inventory below.

> **v14 alignment (v2.1.0, 2026-05-29).** This prompt was rebased from the v13 surface (`--breadth`, `--since`/`--between`, `--area`/`--artifact`/`--consumer`, `--subject`/`--concern`, `--mode-review`, `catch-up`, plain `--incremental`) onto the vision v14 contracts: seven canonical parameters, derived breadth, value-shape `--scope` parser, Phase 0a conversational pre-parser, per-artifact prompt invocation matrix, pipeline-phases / `--skip` mapping with rule #2, default-full invocation contract, and the first-line `Resolved invocation:` log.

## Invocation options (canonical eight-parameter surface, vision v15.4)

The orchestrator accepts **exactly eight canonical parameters**. Any other `--*` flag is REJECTED with CF-05 and a deprecation notice pointing to its v14 destination. The full retired-flag → v14 destination map is in the [migration table](#retired-flag-migration-table-vision-v14) below and in the [vision v15 changelog § Historical: v13 → v14 deprecated flag map](../../../06.00-idea/self-updating-prompt-engineering/20260531.01-vision.changelog.md#historical-v13-v14-flag-map).

### `--mode plan|apply` (default: `apply`)

| Value | Behavior |
|---|---|
| `apply` | **Materialize/reconcile plan (Phases 1–4) → execute plan (Phases 5–7) → report (Phase 8)** (default). One execution engine shared with plan consumption (`apply = plan + execute`); low-risk autonomous apply + propose for higher-risk findings. Phases 1–4 run on a reasoning-grade model, Phases 5–7 on a standard/cheaper model (the plan→execute model seam). Materializes the plan on every run; honors vision v15.4 § Iteration budget — emits spillover plan on overflow per [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md) |
| `plan` | **Materialize/reconcile plan, then stop** (`plan = apply minus execute`) — Phases 5–7 are not executed. Produces health score, findings report, AND an actionable plan file on disk per vision v15.4 § Plan output contract — see [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md); no source-artifact writes |

**Execution modes (fresh / reconcile / trust).** Whether the plan is regenerated or trusted is governed by two orthogonal booleans — **(baseline available?) × (research runs?)** — never by file existence. A baseline is a plan named by `--plan-file` OR a plan generated earlier in the same conversation. `--skip research` (existing parameter) decides regenerate-vs-trust:

| baseline available? | research runs? | Mode | Behavior | Drift guard |
|---|---|---|---|---|
| no | yes | **fresh** | Generate plan from research → write → execute. | skipped (back-to-back) |
| yes | yes | **reconcile** | Load baseline, merge fresh evidence, preserve human decisions, re-verify coverage + actionability, overwrite → execute. | skipped (research re-validates) |
| yes | no | **trust** | Execute the baseline's **human decisions** as-is, but re-derive proof: every applicable PU MUST re-emit a fresh per-dimension `evidence_ref` (an inherited `verified`/`pass-weak` counts as `never`), and a baseline marked `shallow-sweep=suspected` carries that state forward. See [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md) § 4/5. | **REQUIRED** (cross-run target-section hashing) |
| no | no | **invalid** | Rejected at `breadth=full` (no baseline to substitute for research). | — |

In **reconcile**, human-authored rows (park-lot rulings, consent lines, scope tags, rationales) MUST NOT be silently overwritten — contradicting evidence escalates. See [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md) § Execution modes / § Reconcile.

> **Re-running the same command does not raise confidence — it lowers it.** A re-run of an identical `--scope`+`--dim` invocation is itself a signal the prior pass may be incomplete, so neither **reconcile** nor **trust** inherits the baseline's per-dimension verdict as coverage (the inherited `verified`/`pass-weak` is treated as `never` and re-exercised), and a baseline flagged `shallow-sweep=suspected` keeps that flag until the previously silent body groups are evidenced afresh.

### `--scope <artifact-type-token>|<path>[,<path>...]` (default: `all`)

Value-shape parser per vision v14 § Option taxonomy. Exactly two unambiguous shapes:

| Shape | Recognition rule | Examples |
|---|---|---|
| **Artifact-type token** | Single token matching `all\|context\|instructions\|agents\|prompts\|skills\|hooks\|snippets\|templates` | `--scope context`, `--scope all` |
| **Path set** | One or more comma-separated paths — folders end `/`, files end `.md` | `--scope .github/prompts/00.09-pe-meta/`, `--scope path/a.md,path/b.md` |

Mixing the two shapes in one invocation is REJECTED with CF-05 (`--scope must be EITHER a single artifact-type token OR a path set, not both`). When `--scope` is a single file path, Research narrows to that artifact and its direct dependencies (from the `dependency-tracking` file — see 00.00-context-structure-index.md → Functional Categories in `.copilot/context/00.00-prompt-engineering/`).

### `--source <source-id>|<url>[,...]` (default: all monitored sources)

Filters Phase 1 research and Tier-2 screening to the named sources (e.g., `--source vscode-release-notes`, `--source vscode-release-notes,copilot-model-changelog`). Each value carries one of two shapes (mirroring the two-shape `--scope` parser):

- A **source-id** shape — a token resolving to a configured source in `pe-self-update.config.json` → `monitored_sources`. Unknown source IDs are REJECTED with CF-05.
- A **URL** shape — a raw `https://…` release-notes URL for an **ad-hoc** source not in the monitored set (e.g. `--source https://code.visualstudio.com/updates/v1_110`). Phase 1 fetches it via `fetch_webpage` and treats it as a single-source ingestion for this run only; it is NOT persisted to the source ledger (no `version_scheme`, so version-shaped `--start`/`--end` is rejected against it — use a date window). This is the explicit external-platform reconciliation path that replaces the retired Release-monitor prompt: auto-fetch of the monitored set is the parameter-less default; a specific release URL is supplied via `--source <url>`.

### `--dim <group|D#>` (default: `full`)

Dimension selection: `full`, `freshness`, `quality`, `adherence`, `reliability`, `model`, `structural`, `strategic`, `context-full`, `context-health`, or specific `D1-metadata` through `D35-portability-boundary`.

When `--dim efficiency` is active with `--mode apply`, the orchestrator delegates Phase 4 apply work to `@meta-optimizer` instead of per-type builders, and Phase 4 Research adopts an efficiency lens (token budgets, deduplication, structural improvements). When `--dim adherence` is active, the orchestrator routes Phase 4 via the per-artifact matrix to `pe-meta-adherence` for sampled consumer verification.

### `--start <date|version>` / `--end <date|version>` (default: none)

Explicit research window. Either bound MAY be supplied alone (open-ended interval). Any non-empty value triggers **derived `breadth=bounded-delta`** per the resolution rules below.

**Value-shape (vision v15.3 — § Processing-state model).** Each bound carries one of two shapes, resolved deterministically at Phase 0a (mirroring the two-shape `--scope` parser):

- A **date** shape — ISO-8601 (`YYYY-MM-DD`), `now`, or a relative offset (`-7d`, `-90d`).
- A **source-version** shape — a token matching the scoped source's `version_scheme` (e.g. `1.099` for a `semver` source), resolved to that version's publish timestamp via the source's `version_scheme` before phases 1–8 run.

A bounded-delta window **overrides recorded coverage inside the window** — every processing unit whose source-input falls in `[--start, --end]` is reprocessed even when its recorded `status == pass` (the re-baseline / distrust-recovery path). Two guard rails (enforced at Phase 0a): a version-shaped bound REQUIRES a singleton `--source` (rejected otherwise), and a source whose `version_scheme` is `none` rejects a version-shaped bound ("use a date window"). This adds **no new parameter** — it widens the value space of an existing one, so `minimal-canonical-surface` is preserved.

### `--deps none|direct|full|<N>` (default: `none`)

Dependency-chain depth for Phase 4 per-artifact work. `direct` = first-level only; `full` = bounded recursive traversal (default depth 5); `<N>` = explicit numeric depth.

### `--skip <stage>[,<stage>...]` (default: none)

| Stage | Skips | Use when |
|---|---|---|
| `research` | Phase 1 (source research) | INCOMPATIBLE with derived `breadth=full` (rejected with CF-05) UNLESS `--plan-file` references a validated baseline plan — the baseline substitutes for research (trust mode), and the cross-run drift guard becomes REQUIRED. See rule #2 in [Pipeline phases and `--skip` mapping](#pipeline-phases-and---skip-mapping) |
| `external` | Internet/URL fetching in all phases | No internet or local-only check desired |
| `organizational` | Phase 1.5 (organizational pass) | Cross-artifact organizational concerns already verified |
| `structure` | Phase 2 (structure audit) | Structure is known-good, focus on content |
| `consistency` | Phase 3 (consistency audit) | Cross-artifact consistency already verified |
| `content` | Phase 4 (content audit) | Individual artifacts already reviewed |

### `--plan-file <path>` (default: auto-name)

Plan-artifact **location/identity only** — the eighth canonical parameter (vision v15.4 § Option taxonomy). It does NOT change what is assessed or applied, and it **never decides regenerate-vs-trust** (that is governed by `--skip research`; see the Execution modes table under `--mode`).

- **Omitted (default):** the plan lands at the auto-name path `<run-folder>/<NN>-<kebab-name>.plan.md`; fallback `.copilot/temp/pe-meta-state/plans/YYYYMMDD-HHMMSS-<kebab-name>.plan.md` when no run folder applies.
- **Supplied:** `--plan-file <path>` overrides the location AND marks that path as a **baseline** for this run. With research running this resolves to **reconcile**; with `--skip research` it resolves to **trust** (drift-guarded). A same-conversation just-generated plan is an implicit baseline even without `--plan-file`.

See [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md) for the full path algorithm, execution-ready-precision clause, and drift-guard rule.

## Derived breadth (vision v14 § Default-full invocation contract)

`breadth` is a **resolved attribute** — never a flag. The orchestrator MUST derive it before any phase runs and MUST emit it in the first-line `Resolved invocation:` log.

| Caller type | `--start` / `--end` present? | Derived breadth | Phase 1 output contract |
|---|---|---|---|
| Manual (interactive) | No | `full` | Current-state snapshot across entire monitored-sources catalog (Phase 1.5 gate opens) |
| Trigger-fired (scheduled/hook/file-watcher) | No | `incremental` | Change digest for the stale processing units (PUs) per the § Incremental filter work-set computation |
| Either | Yes (one or both bounds; date **or** source-version) | `bounded-delta` | Bounded-window digest between the explicit endpoints; recorded `pass` coverage is overridden inside the window |

**Rejection.** `--breadth` (any value) is REJECTED with CF-05: `--breadth retired in v14; breadth is derived from caller-type and --start/--end — see vision v14 § Default-full invocation contract`.

## Retired-flag migration table (vision v14 — historical)

Every retired v13 flag is REJECTED with CF-05 using a uniform message template: `<flag> retired in v14; use <v14-replacement> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map`. The parser MUST table-drive these rejections; do NOT hand-write per-flag prose.

| Retired v13 surface | v14 destination |
|---|---|
| `--breadth full\|incremental\|catch-up` | Derived attribute; logged on first line of every run report |
| `catch-up` value (any flag) | `--start <older-than-default>` on any caller; resolves to `bounded-delta` |
| `--since <date>` | `--start <date>` |
| `--between <a>..<b>` | `--start <a> --end <b>` |
| `--area <token>` | `--scope <artifact-type-token>` |
| `--artifact <path>` | `--scope <path>[,<path>...]` |
| `--consumer <path>` | `--scope <agent-or-prompt-file> --deps full` |
| `--subject <kw>` | Resolved by Phase 0a → comma-separated `--scope` enumeration |
| `--concern <kw>` | Resolved by Phase 0a → `--scope` (+ `--dim` when keyword maps to a dimension) |
| `--mode-review individual\|dep-aware\|guidance-first` | Auto-derived from `--scope` artifact-type via per-artifact prompt invocation matrix; guidance-first leg covered by `--dim adherence` |
| `--incremental` | **Single migration-window alias** — accepted ONLY for trigger-fired callers (resolves to derived `breadth=incremental`); REJECTED for manual callers (would violate default-full-investigation) |
| Any other unrecognized `--*` | CF-05 rejection with full canonical eight-parameter enumeration |

## Phase 0a — conversational pre-parser (vision v14 § Option taxonomy)

**When it runs.** Phase 0a runs BEFORE strict parsing whenever the raw invocation contains any token that is not one of the eight canonical parameters or a recognized value. It is LLM-mediated, so it is the ONLY place in the pipeline where free-form input is allowed to influence canonical resolution.

**What it does.**

1. Reads the raw invocation (including any natural-language hints like `"recheck anything affected by the April VS Code release"` or `"focus on consumer-correctness for the adherence prompt"`).
2. Resolves free-form intent into the eight canonical parameters using these rules:
    - Subject keywords (e.g., `"April VS Code release"`) → enumerate matching artifact paths and emit a comma-separated `--scope` enumeration.
    - Concern keywords (e.g., `"consumer-correctness"`, `"freshness"`) → set `--dim` to the matching group; if the keyword names a topic without a dimension, also emit a `--scope` enumeration.
    - Consumer chains (e.g., `"the adherence prompt and what it depends on"`) → emit `--scope <consumer-file> --deps full`.
    - Temporal hints (e.g., `"since April"`, `"between April and May"`) → emit `--start`/`--end`.
3. Echoes the resolved canonical invocation back to the caller BEFORE Phase 1 runs (gives caller a chance to abort or correct).
4. Hands the canonical eight-parameter set to the strict parser. Phases 1–8 NEVER see free-form input.

**Non-determinism risk.** Phase 0a is LLM-mediated. Two callers passing the same prompt MAY get the same canonical resolution, but reproducibility is not guaranteed. Mitigation: every resolution is echoed back AND logged on the first line of the Phase 8 report (`Resolved invocation:` log). Downstream determinism is preserved because phases 1–8 only consume the canonical resolution.

### Phase 0a value-shape `--start`/`--end` resolution (vision v15.3 § Processing-state model)

After free-form resolution settles and BEFORE the strict parser runs, Phase 0a resolves each supplied `--start`/`--end` bound to a timestamp so phases 1–8 only ever consume a resolved time window:

1. **Detect the shape of each bound deterministically.** ISO-8601 (`YYYY-MM-DD`), `now`, or a relative offset (`-Nd`) → **date** shape (used directly). A token matching the scoped source's `version_scheme` pattern (`semver` | `dated` | `model-version`) → **source-version** shape.
2. **Resolve a version-shaped bound to a timestamp.** Look up the scoped source's `version_scheme` in `pe-self-update.config.json`, find that version's publish timestamp (from the source ledger `last_seen_*` history or by fetching the source), and substitute the resolved timestamp into the window. `version_scheme` thus plays a dual role — ledger version recorder AND parser version-token recognizer.
3. **Enforce the two guard rails (reject with CF-05):**
    - A version-shaped bound with a non-singleton `--source` (or `--source` omitted, which resolves to all monitored sources) → `version window requires a single --source`.
    - A version-shaped bound against a source whose `version_scheme` is `none` → `source <id> has no version scheme; use a date window`.
4. **Echo the resolved window** on the first-line `Resolved invocation:` log as resolved timestamps (the version token is preserved in the audit narrative). No `--incremental` token is accepted at this stage.

This widens an existing parameter's value space exactly as the value-shape `--scope` parser does; it introduces no new parameter and keeps breadth a derived attribute.

### Phase 0a precondition — artifact-type/path consistency check (CF-05)

> **Spec SoT:** [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) § Phase 0a CF-05. The table, rejection message format, and applicability rules below MUST stay byte-equivalent (modulo wording) to the context file. Per-artifact prompts pin their single expected root locally and cite the context file for the canonical mapping.

**When it runs.** At the END of Phase 0a, after free-form resolution has settled but BEFORE Phase 0b. Runs on every invocation of a **per-artifact prompt** (e.g. `/pe-meta-context-review`, `/pe-meta-prompt-review`). Orchestrator-level prompts (`/pe-meta-review`, `/pe-meta-review`, `/pe-meta-create-update`, `/pe-meta-design`, `/pe-meta-adherence`, `/pe-meta-scheduled-review`) are artifact-type-agnostic by design and SKIP this check.

**What it does.** Compares the artifact-type ROOT implied by the prompt name against the artifact-type ROOT resolved from the positional `<file-path>` or `--scope` value. Mismatch is REJECTED with CF-05 before Phase 0b runs.

**Prompt-name-prefix → expected-root table.**

| Prompt name prefix | Expected root |
|---|---|
| `pe-meta-context-*` | `.copilot/context/` |
| `pe-meta-instruction-*` | `.github/instructions/` |
| `pe-meta-agent-*` | `.github/agents/` |
| `pe-meta-prompt-*` | `.github/prompts/` |
| `pe-meta-skill-*` | `.github/skills/` |
| `pe-meta-hook-*` | `.github/hooks/` |
| `pe-meta-template-*` | `.github/templates/` |
| `pe-meta-snippet-*` | `.github/prompt-snippets/` |

**CF-05 rejection message format.**

```text
CF-05: artifact-type/path mismatch. Invoked prompt /<invoked-prompt> expects positional paths under <expected-root>; supplied path resolves under <actual-root>. Canonical replacement: /<canonical-prompt> '<supplied-path>' <other-args>.
```

**Operates on ROOT, not domain.** This check is deterministic from path. It does NOT read frontmatter and is NOT a domain check. Domain coherence is a separate concern handled by Phase 0b (which reads `domain:` frontmatter from each in-scope file).

## Per-artifact prompt invocation matrix (vision v14 § Per-artifact prompt invocation matrix)

After parsing, the orchestrator routes Phase 4 work via this matrix from `(--scope-resolved-artifact-type, --dim)` → per-artifact prompt. The orchestrator MUST NOT hand-write per-artifact-type branches.

| Resolved artifact type | `--dim` family | Selected prompt |
|---|---|---|
| `context` | review-family (default, `freshness`, `quality`, `structural`, `strategic`, `model`, `context-full`, `context-health`) | `pe-meta-context-review` |
| `context` | `--dim adherence` | `pe-meta-adherence` (target = each context file in scope) |
| `instructions` | review-family | `pe-meta-instruction-review` |
| `instructions` | `--dim adherence` | `pe-meta-adherence` |
| `agents` | review-family | `pe-meta-agent-review` |
| `agents` | `--dim adherence` | `pe-meta-adherence` |
| `prompts` | review-family | `pe-meta-prompt-review` |
| `prompts` | `--dim adherence` | `pe-meta-adherence` |
| `skills` | review-family | `pe-meta-skill-review` |
| `hooks` | review-family | `pe-meta-hook-review` |
| `snippets` | review-family | `pe-meta-snippet-review` |
| `templates` | review-family | `pe-meta-template-review` |
| any | `--dim efficiency --mode apply` | `@pe-meta-optimizer` (orchestrator delegation for apply work, not a per-artifact prompt) |
| any (creation intent surfaced by Phase 0a) | — | `pe-meta-{type}-create-update` or `pe-meta-{type}-design` per intent |

**Path-shape `--scope`.** When `--scope` is a path set, the orchestrator infers the artifact type from the path prefix (e.g., `.github/prompts/` → `prompts`) and applies the matching matrix row.

**Per-artifact prompt invocation matrix — Phase 0b inheritance.** When a per-artifact prompt is invoked DIRECTLY by the user with a positional `<file-path>`, it runs its own minimal Phase 0b stub before delegating to the orchestrator. The stub MUST handle the positional-path scope-extraction step per vision v15 § Domain detection § per-invocation-type matrix and MUST read each in-scope file's declared `domain:` frontmatter to compute the footprint (the seed file's path does NOT constrain consumer domains when `--deps full` traverses the closure). The algorithm SoT is [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) — per-artifact prompts cite that file (not this prompt) so sibling-to-sibling coupling is avoided.

## Phase 0b — Domain coherence check (vision v15 § Domain-coherent batching)

> **Algorithm SoT:** [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md). The spec below is this prompt's implementation of the shared contract — algorithm, decision matrix, dispatch table, `bundle=…` closed set, and `bundle=accept` consent semantics MUST stay byte-equivalent (modulo wording) to the context file. When the contract changes, update the context file FIRST and re-validate this section with `pe-meta-adherence`.

**Goal.** Prevent silent heterogeneous batching by computing the semantic-domain footprint of the resolved scope (seed AND `--deps`-closure) BEFORE Phase 1 runs. When the footprint exceeds one domain, gate `--mode apply` until the caller either splits the run per-domain or appends explicit `bundle=accept` consent. Honest per-file metadata is the input; deterministic dispatch is the output.

**Inputs.**

- The canonical eight-parameter invocation resolved by Phase 0a (and validated against the Phase 0a precondition CF-05 check).
- The **metadata-first 3-tier domain resolution algorithm** defined in vision v15 § Domain detection:
  - **Tier 1 (authoritative):** Read each in-scope file's `domain:` frontmatter value. Tier 1 NEVER loses to a lower tier when present.
  - **Tier 2 (optional heuristic):** If `pe-domain-map.yaml` exists at repo root, apply path-slug heuristic for files without declared `domain:`. Map entries are flagged per-file in the Phase 8 report so authors can migrate to declared metadata.
  - **Tier 3 (fallback):** Files unresolved by Tier 1 or Tier 2 receive `domain: unknown`. `unknown` is a reserved domain-id; repos MUST NOT declare `domain: unknown` in any file (the orchestrator emits a Phase 8 warning when a declared value collides with the reserved id).

**Algorithm (deterministic, 5 steps).**

1. **Resolve scope to file set.** Expand `--scope` (artifact-type token → all files under root; path → enumerated files; path-set → union). If `--deps direct|full|<N>` is present, traverse the `dependency-tracking` closure to depth N and union into the file set. Tag each file as `role=seed` (named by the caller) or `role=dep` (added by closure traversal).
2. **Resolve domain per file.** For each file, apply Tier 1 → Tier 2 → Tier 3 and record `domain-source` (`declared` | `path-heuristic` | `unknown`). `--dim` is NEVER consulted here — `--dim` is a dimension-group selector per v14 (filters which audit dimensions Phase 2–4 exercises); it never affects domain resolution.
3. **Compute seed footprint and dependency footprint SEPARATELY.** Seed footprint = `{distinct domains across files where role=seed}`. Dependency footprint = `{distinct domains across files where role=dep AND domain ∉ seed footprint}`. Recording the two sets separately is what enables the `cross-domain-deps` discrimination in step 4.
4. **Dispatch from the seed-vs-deps decision matrix.**

   | Seed footprint | Additional dep-domains | Disposition (`bundle=…`) | Phase 1 action |
   |---|---|---|---|
   | 1 | 0 | `single-domain` | Proceed without prompt |
   | 1 | ≥ 1 | `cross-domain-deps` | Proceed as ONE review with per-dep-domain specialized analysis lenses in Phase 2–4 (no split — splitting a single-seed cross-domain-deps invocation produces incomplete reviews because the consumer artifact needs all declared deps present to be evaluated) |
   | ≥ 2 | n/a | `multi-domain-gated` (`--mode apply`) or `multi-domain-advisory` (`--mode plan`) | `apply`: emit numbered split proposal and BLOCK Phase 1 until user selects a split or appends `bundle=accept` (→ `bundle=accepted-bundle`); `plan`: include split proposal in Phase 8 report and proceed |

5. **Emit the first-line `Resolved invocation:` log marker.** Append `| bundle=<disposition>` to the canonical first-line log so the disposition is observable before any side effects occur.

**Outputs.**

- **Domain footprint table** in the Phase 8 report with one row per in-scope file and columns `path | role (seed/dep) | domain | domain-source (declared/path-heuristic/unknown)`. Files resolved by Tier 2 or Tier 3 are flagged in the report so authors can backfill declared metadata.
- **Numbered split proposal** (only when disposition is `multi-domain-gated` or `multi-domain-advisory`): one numbered line per detected domain plus a final `[N]` line for "proceed as one atomic bundle (equivalent to appending `bundle=accept`)". Example shape:

   ```text
   Phase 0b detected 3 distinct domains in the resolved scope:

   [1] prompt-engineering  (12 files)
   [2] article-writing     (4 files)
   [3] learning-hub        (2 files)
   [4] proceed as one atomic bundle (bundle=accept)

   Select a split number (1-3) to run that domain only, run all three sequentially (all), or [4] to proceed as one atomic bundle.

   Note: 2 files were resolved by path-heuristic (Tier 2) and 2 files by unknown fallback (Tier 3). Consider adding `domain:` to those files' YAML frontmatter; see the Phase 8 report for the exact file list.
   ```

**Gate behavior summary.**

| `--mode` | Seed footprint | Additional dep-domains | `bundle=accept` present? | Phase 0b disposition | Phase 1 runs? |
|---|---|---|---|---|---|
| `apply` | 1 | 0 | n/a | `single-domain` | Yes |
| `apply` | 1 | ≥ 1 | n/a | `cross-domain-deps` | Yes (one run; per-dep-domain lenses in Phase 2–4) |
| `apply` | ≥ 2 | n/a | no | `multi-domain-gated` | No — BLOCK on user input |
| `apply` | ≥ 2 | n/a | yes | `accepted-bundle` | Yes |
| `plan` | 1 | 0 | n/a | `single-domain` | Yes |
| `plan` | 1 | ≥ 1 | n/a | `cross-domain-deps` | Yes (one run; per-dep-domain lenses in Phase 2–4) |
| `plan` | ≥ 2 | n/a | n/a | `multi-domain-advisory` | Yes (with advisory in report) |

**`bundle=accept` consent token.** Single-keystroke bypass for the multi-domain gate. Appended as a trailing positional token (e.g. `/pe-meta-review --mode apply --scope context bundle=accept`). It is the ONLY accepted consent value — `bundle=skip`, `bundle=yes`, `bundle=true`, etc. are REJECTED with CF-05. Consent is recorded on the first-line `Resolved invocation:` log as `bundle=accepted-bundle` (distinct from the un-consented `bundle=multi-domain-gated` marker so the audit trail preserves the discrimination).

**CF-05 rejection — `--skip domain-coherence`.** Phase 0b is NOT in the skippable set. `--skip domain-coherence` is REJECTED with CF-05:

```text
CF-05: --skip domain-coherence is rejected; Phase 0b is not skippable per vision v15 § Domain-coherent batching. To bypass the gate on a multi-domain scope, append bundle=accept to the invocation.
```

**`--dim` orthogonality.** `--dim` is a dimension-group selector per v14 (filters which audit dimensions Phase 2–4 exercises). It is NEVER a domain override. The domain footprint is always computed from per-file declared `domain:` metadata (with Tier 2/3 fallback) regardless of the `--dim` value. Parser test P0b-13 pins this invariant.

**Determinism.** The domain map is computed ONCE at Phase 0b entry and frozen for the rest of the run. The 3-tier algorithm runs in < 1 second on the largest current workspace (51 files, 3 domains); each file's YAML frontmatter is read once and cached. No LLM call required.

## Pipeline phases and `--skip` mapping (vision v14 § Pipeline phases and `--skip` mapping)

| Phase | Title | `--skip` value that retires it | Runs when |
|---|---|---|---|
| 0 | Argument parsing & breadth derivation | (cannot be skipped) | Always |
| 0a | Conversational pre-parser | (auto — runs only on non-canonical input) | When free-form input present |
| 0a-precondition | Artifact-type/path consistency check (CF-05) | (cannot be skipped) | Only on per-artifact prompts (orchestrator-level prompts skip) |
| 0b | Domain coherence check | (cannot be skipped; `--skip domain-coherence` is REJECTED with CF-05) | Always; emits `bundle=…` marker on the `Resolved invocation:` log |
| 1 | Source research | `research` | Always unless (`--skip research` AND derived `breadth ≠ full`) OR (`--skip research` AND `--plan-file` references a validated baseline — trust mode, drift-guarded) |
| 1.5 | Organizational pass | `organizational` | When derived `breadth=full` AND resolved `--scope` is broader than a single file |
| 2 | Structure audit | `structure` | Default; skippable |
| 3 | Consistency audit | `consistency` | Default; skippable |
| 4 | Content audit (per-artifact via matrix) | `content` | Default; skippable |
| 5 | User approval | (cannot be skipped in `--mode apply`) | `--mode apply` only |
| 6 | Apply changes | (skipped automatically in `--mode plan`) | `--mode apply` only |
| 7 | Regression test | (cannot be skipped when Phase 6 applies changes) | After Phase 6 changes |
| 8 | Report + log | (cannot be skipped) | Always |

**Rule #1.** `--skip external` retires internet/URL fetching across ALL phases that would otherwise use `fetch_webpage`.

**Rule #2 (CRITICAL — vision v15.4).** `--skip research` is INCOMPATIBLE with derived `breadth=full` because a full sweep without source research is structurally meaningless under the default-full invocation contract — **EXCEPT** when `--plan-file` references a validated baseline plan, because the plan IS the prior research product (trust mode). Without a baseline the parser MUST REJECT the combination with CF-05: `--skip research is incompatible with derived breadth=full unless --plan-file references a validated baseline plan; either drop --skip research, supply --plan-file, or narrow the window with --start/--end (which derives breadth=bounded-delta)`. In trust mode the cross-run drift guard (target-section hashing) is REQUIRED: a hash mismatch escalates the drifted row rather than applying a stale edit.

**Rule #3 (CRITICAL — execution discipline; no batch-marking).** The audit phases (1, 1.5, 2, 3, 4) MUST be executed and status-marked **one at a time** — each phase is marked in-progress when its work begins and completed only after its work is actually performed and its outputs (findings, outcome-log entries, coverage markers) exist. The orchestrator MUST NOT batch-mark multiple audit phases complete in a single step, and MUST NOT mark a phase complete on the basis of a cheap proxy (e.g., a frontmatter `grep` standing in for the mandated per-artifact body review). Batch-marking is the behavioral root of the [2026-06-06 full-processing collapse](../../../src/docs/90.%20Issues/202606/20260606.02-pe-meta-update/01-small-instruction-files-changes/overview.md), where audit phases were marked done without execution; the Phase 8 full-coverage linter is the runtime backstop, but this rule is the first line of defense.

## Examples

- `/pe-meta-review` — Parameter-less manual invocation. Derives `breadth=full`. Runs Phases 0 → 1 → 1.5 → 2 → 3 → 4 → 5 → 6 → 7 → 8 against the entire monitored-sources catalog and the entire PE artifact tree. (default-full-investigation contract.)
- `/pe-meta-review --mode plan` — Same as above but stops after Phase 4 and produces an assessment-only report.
- `/pe-meta-review --scope context` — Manual, `--scope` is the artifact-type token `context`. Derived `breadth=full`, but Phase 1.5 Organizational Pass STILL runs because the resolved scope (all context files) is broader than a single file.
- `/pe-meta-review --scope .copilot/context/00.00-prompt-engineering/01.07-critical-rules-priority-matrix.md` — Single-file path scope. Derived `breadth=full`, but Phase 1.5 is SKIPPED (single-file gate).
- `/pe-meta-review --start 2026-04-01` — Manual + window. Derives `breadth=bounded-delta`. Phase 1 produces a bounded-window digest between `2026-04-01` and "now".
- `/pe-meta-review --start 2026-04-01 --end 2026-04-30` — Same as above with both endpoints.
- `/pe-meta-review --source vscode-release-notes` — Manual, source-filtered. Derived `breadth=full`, but Phase 1 consults only the `vscode-release-notes` source.
- `/pe-meta-review --mode apply --dim efficiency --skip structure,consistency --scope context` — Efficiency-lens review of context files only.
- `/pe-meta-review --mode apply --dim adherence --scope .copilot/context/00.00-prompt-engineering/01.07-critical-rules-priority-matrix.md --deps full` — Adherence sampling for a specific guidance file across its full dependency chain.
- `/pe-meta-review --incremental` (trigger-fired only) — Single-window deprecation alias. Resolves to derived `breadth=incremental`. REJECTED if invoked manually.
- `/pe-meta-review --mode apply --scope .github/prompts/00.09-pe-meta/pe-meta-update.prompt.md --deps full` — Manual + single-seed-file + `--deps full`. Phase 0b expands the dependency closure (vision contexts, dependency-tracking files, instruction files). Seed footprint = `{prompt-engineering}` (1 domain); dep closure adds `{article-writing, learning-hub}`. Disposition = `bundle=cross-domain-deps` — ONE review runs with per-dep-domain specialized lenses applied in Phase 2–4; the review is NOT split because the consumer artifact needs all declared deps present to be evaluated.
- `/pe-meta-review --mode apply --scope context` — Manual + artifact-type token. Phase 0b enumerates all `.copilot/context/` files; seed footprint spans `{prompt-engineering, article-writing, learning-hub}` (3 domains). Disposition = `bundle=multi-domain-gated` — Phase 1 is BLOCKED until the user selects a numbered split (1/2/3 per domain) OR appends `bundle=accept` to convert to `bundle=accepted-bundle`.
- `/pe-meta-review --mode apply --scope context bundle=accept` — Same scope as above but with explicit consent. Phase 0b records `bundle=accepted-bundle` on the first-line log and proceeds without gating Phase 1. Phase 8 report still emits the domain-footprint table.

**Rejected examples (CF-05).**

- `/pe-meta-review --breadth full` → `--breadth retired in v14; breadth is derived from caller-type and --start/--end — see vision v14 § Default-full invocation contract`.
- `/pe-meta-review --since 2026-04-01` → `--since retired in v14; use --start <YYYY-MM-DD> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map`.
- `/pe-meta-review --area context` → `--area retired in v14; use --scope <artifact-type-token> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map`.
- `/pe-meta-review --subject "April VS Code release"` → Phase 0a resolution required; if invoked directly without Phase 0a route, CF-05: `--subject retired in v14; free-form intent is resolved by Phase 0a — see vision v14 § Option taxonomy`.
- `/pe-meta-review --mode-review guidance-first` → CF-05: `--mode-review retired in v14; use --dim adherence — see vision v15 changelog § Historical: v13 → v14 deprecated flag map`.
- `/pe-meta-review --incremental` (manual caller) → CF-05: `--incremental is accepted only for trigger-fired callers; manual invocations are contracted to derived breadth=full per default-full-investigation`.
- `/pe-meta-review --skip research` (manual, no window) → CF-05: `--skip research is incompatible with derived breadth=full; either drop --skip research or narrow the window with --start/--end (which derives breadth=bounded-delta)`.
- `/pe-meta-review healthcheck` → CF-05: `"healthcheck" is no longer a supported preset. Use the eight canonical parameters per vision v15.4 — see § Invocation options`.
- `/pe-meta-review --skip domain-coherence` → CF-05: `--skip domain-coherence is rejected; Phase 0b is not skippable per vision v15 § Domain-coherent batching. To bypass the gate on a multi-domain scope, append bundle=accept to the invocation.`
- `/pe-meta-review --scope context bundle=skip` → CF-05: `bundle=skip is not a valid consent token; the closed set is {accept}. Use bundle=accept to bypass the multi-domain gate.`
- `/pe-meta-review --scope context bundle=yes` → CF-05: `bundle=yes is not a valid consent token; the closed set is {accept}. Use bundle=accept to bypass the multi-domain gate.`
- `/pe-meta-context-review .github/prompts/00.09-pe-meta/pe-meta-update.prompt.md` → CF-05 (artifact-type/path mismatch from the Phase 0a precondition): `Invoked prompt /pe-meta-context-review expects positional paths under .copilot/context/; supplied path resolves under .github/prompts/. Canonical replacement: /pe-meta-prompt-review '.github/prompts/00.09-pe-meta/pe-meta-update.prompt.md'.`

## CRITICAL BOUNDARIES

### Always Do
- Parse mode, scope, flags FIRST
- **Run Phase 0a precondition (artifact-type/path consistency check, CF-05) on every per-artifact prompt invocation**: compare prompt-name prefix against positional-path root; reject with CF-05 and suggest the canonically-correct prompt name on mismatch. Orchestrator-level prompts skip this check (artifact-type-agnostic by design).
- **Run Phase 0b (domain coherence check) on every invocation BEFORE Phase 1**: resolve scope, apply the metadata-first 3-tier algorithm, compute seed and dependency footprints separately, dispatch from the decision matrix, emit `bundle=…` on the first-line `Resolved invocation:` log.
- **Honor `bundle=accept` as the ONLY consent token** for the multi-domain gate; record consented runs as `bundle=accepted-bundle` (distinct from un-consented `bundle=multi-domain-gated`) so the audit trail preserves the discrimination.
- Load dependency map (the `dependency-tracking` files — see 00.00-context-structure-index.md → Functional Categories in `.copilot/context/00.00-prompt-engineering/`)
- **Use three-tier classification for every proposed change** (see Classification Protocol below)
- In each audit phase Research substep: **challenge current state, propose 2+ alternative approaches per finding, compare on effectiveness/reliability/efficiency**
- **Risk-ordered execution**: When multiple findings are produced, execute in this order:
  1. Non-regressive changes first (autonomous-eligible: LOW severity, deterministic classification)
  2. Potentially-regressive changes next (require human approval: MEDIUM/HIGH severity)
  3. Optimization-only changes last (separate cycle if budget allows)
  4. No step is blocked by a higher-risk independent step — execute what you can, escalate what you must
- **Propagation-aware priority**: Before presenting findings, check the `dependency-tracking` file (see 00.00-context-structure-index.md → Functional Categories in `.copilot/context/00.00-prompt-engineering/`) for each affected artifact's dependent count. Sort findings by: severity (primary) × dependent count (secondary). A HIGH finding in a Tier 1 file with 15 dependents takes priority over a HIGH finding in a Tier 5 file with 2.
- Present consolidated plan to user BEFORE applying changes
- Max 3 files between validation checkpoints
- Produce final report regardless of mode
- **After every applied change**: bump `version:`, update `last_updated:`, verify `scope.covers` topics match content
- **Autonomous execution for LOW-severity changes** (Phase 1 rollout artifacts only: templates, prompt snippets, hooks JSON):
  - When ALL of these conditions are met, apply without human approval: (1) severity is LOW, (2) classification is deterministic (Tier 1 or Tier 2), (3) pre-change guard passes, (4) post-change validation passes
  - For all other changes: present plan and wait for approval
- **Structured change logging (MANDATORY after every applied change):**
  - Append a structured entry to `05.04-meta-review-log.md` with: artifact path, classification (breaking/non-breaking), confidence (deterministic/LLM-assisted), autonomy level (autonomous/approved), validations passed, outcome (success/pending)

### Ask First
- High-impact files (6+ dependents)
- Uncertain source reliability
- 5+ CRITICAL findings in any audit phase
- **Any change above LOW severity** — present plan before applying

### Never Do
- **NEVER apply MEDIUM/HIGH/CRITICAL changes without user approval**
- **NEVER skip validation after changes**
- **NEVER remove capabilities** — only extend, refine, or deprecate
- **NEVER classify a change without checking N-1 block labels first** (when available)
- **NEVER skip Phase 0b** — `--skip domain-coherence` is REJECTED with CF-05; the only legitimate bypass is `bundle=accept` (which surfaces the disposition on the audit log rather than hiding it)
- **NEVER treat `--dim` as a domain override** — `--dim` is a dimension-group selector per v14; domain footprint is always computed from per-file `domain:` metadata regardless of `--dim` value (parser test P0b-13 pins this invariant)
- **NEVER override declared `domain:` frontmatter with a Tier 2 heuristic match** — Tier 1 always wins when present; Tier 2 only applies to files without declared `domain:` and emits a Phase 8 report flag suggesting metadata backfill

---

## Classification Protocol (Three-Tier)

For every proposed change, classify using these tiers in order. Stop at the first tier that produces a confident result.

### Tier 1: Deterministic Structural (metadata)

Check without LLM judgment:
- [ ] Required YAML fields present (`goal:`, `scope:`, `version:`, `last_updated:`)
- [ ] `version:` will be bumped after change
- [ ] `scope.covers:` topics not removed from content (compare topic strings against section headings)

**If any check fails → CRITICAL finding. No LLM needed.**

### Tier 2: Deterministic Content (N-1 block labels)

For context files and instruction files with `**Rule**:`/`**Rationale**:`/`**Example**:` labels:
- If the diff touches a `**Rule**:` block → **BREAKING CANDIDATE** → requires full validation
- If the diff touches only `**Rationale**:` or `**Example**:` blocks → **NON-BREAKING** → eligible for streamlined processing
- If the section has no N-1 labels → fall through to Tier 3 + flag for N-1 adoption

**This is deterministic — parse block labels, classify the diff. No LLM judgment needed.**

### Tier 3: LLM-Assisted Semantic (metadata as reference)

When Tiers 1-2 don't resolve, use LLM judgment WITH metadata as reference:
- Compare proposed change against `goal:` — does it contradict the stated purpose?
- Compare against `boundaries:` — does it violate any declared constraint?
- Compare against `rationales:` — does it invalidate a design decision?

**This is LLM judgment, but significantly better than judgment without any metadata reference point.**

---

## Handoff Data Contracts

**📖 Researcher output format:** `.github/templates/00.00-prompt-engineering/output-researcher-report.template.md`

| Transition | Strategy | Include | Exclude | Max tokens |
|---|---|---|---|---|
| **Orchestrator → meta-researcher** (Phase 1) | send: true | Source URL/description, scope, flags, `--skip external` status | N/A (first handoff) | ~2,000 |
| **meta-researcher → Orchestrator** | Structured report | Prioritized recommendations classified by audit phase, impact matrix, alternative approaches | Raw internet fetches, full article text, source analysis | ≤2,000 |
| **Orchestrator → meta-validator** (Phase 2R/3R) | send: true | Scope, audit dimension, Phase 1 findings summary (if available) | Full Phase 1 report, raw search results | ≤1,500 |
| **Orchestrator → meta-designer** (Phase 2B/3B/4B) | send: true | Audit findings summary, scope, execution constraints | Raw audit analysis, prior phase conversation | ≤1,500 |
| **meta-designer → type-specific builders** | Change specs | Per-file change specification: path, current content ref, proposed change, rationale | Design analysis, alternatives considered | ≤1,000/file |
| **Orchestrator → meta-optimizer** (`--dim efficiency --mode apply`) | send: true | Scope, efficiency findings, token budgets | Prior audit conversation | ≤1,000 |

## Summarization Protocol

| After Phase | Summarize to | Max tokens | Discard |
|---|---|---|---|
| Phase 1 (Source Research) | Prioritized recommendations by audit phase | ≤2,000 | Raw fetches, full article text |
| Phase 2 (Structure Audit) | Structural findings: severity-scored issues + recommended fixes | ≤1,500 | Raw inventory data, file listings |
| Phase 3 (Consistency Audit) | Consistency findings: contradictions + dedup recommendations | ≤1,500 | Cross-reference analysis details |
| Phase 4 (Content Audit) | Content findings: per-file issues + improvement options | ≤1,500 | Per-file analysis details |
| Phase 5 (Approval) | Approved change list (file + change description) | ≤1,000 | Rejected items, discussion |
| Phase 6 (Apply) | Applied changes: file + status | ≤500 | Builder's reasoning |
| Phase 7 (Regression) | Regression results: pass/fail per capability | ≤1,000 | Full test details |

**Trigger**: Before EVERY handoff, estimate accumulated context. If >10,000 tokens: MUST summarize all prior phases to their "Summarize to" format. This is CRITICAL for pe-meta-review which has 8 phases — without summarization, accuracy drops to ~30% at 32K tokens.

**📖 Full strategies:** `.copilot/context/00.00-prompt-engineering/02.02-context-window-and-token-optimization.md`

## Context Checkpoint Protocol

Between EVERY phase, run this deterministic context size check:

1. **Count**: List all files read in the completed phase
2. **Estimate**: `files_read × avg_file_lines × 6.67 = tokens_this_phase` (avg context file ~150 lines → ~1,000 tokens; avg agent/prompt ~300 lines → ~2,000 tokens)
3. **Running total**: `prior_phases_total + tokens_this_phase`
4. **Act on thresholds** (📖 see `01.06-system-parameters.md` → Meta-Pipeline Context Thresholds):
   - **≤10,000 tokens**: Continue normally
   - **>10,000 tokens**: MUST summarize all prior phases per the Summarization Protocol table above
   - **>20,000 tokens**: MUST write state to `.copilot/temp/pe-meta-state/` and recommend new session

**File-based isolation boundary**: Phases 5–8 MUST use file-based isolation. At the end of Phase 4 (or at Phase 5 entry), write the consolidated approved change list to `.copilot/temp/pe-meta-state/phase-5-changelist.md`. Phases 6–8 read from this file — they do NOT depend on conversation history from Phases 1–4.

---

## Processing-state model (vision v15.3 § Processing-state model)

Processing state is recorded on **two independent axes** — never collapsed to one scalar timestamp.

**Source ledger (input axis).** One file per monitored source at `<state.path>/triggers/<source-id>.json` (path from `pe-self-update.config.json` → `state.path`). It is a rebuildable cache of what the world last looked like:

```json
{
  "source_id": "vscode-release-notes",
  "version_scheme": "semver",
  "last_seen_version": "1.107",
  "last_seen_timestamp": "2026-05-29T00:00:00Z",
  "last_digest_hash": "<sha256>"
}
```

**Artifact coverage (processing axis).** The durable single source of truth, recorded per artifact in the bottom validation-metadata block — one cell per `(artifact × applicable-dimension)`:

```yaml
coverage:
  <dimension-id>:
    source_versions: { "<source-id>": "<version-or-timestamp>", ... }
    depth: research | screening | deep
    status: pass | fail | partial | never
    last_run: "<YYYY-MM-DDTHH:MM:SSZ>"
```

A **processing unit (PU)** is one `(artifact × applicable-dimension)` — applicability follows `active-dimensions-follow-evidence`. A PU is **stale** when its `status == never`, OR `status != pass`, OR any dependency source's ledger `last_seen_version` / `last_seen_timestamp` is newer than the PU's recorded `source_versions[dep]`. Every writing prompt MUST emit the `coverage` block in this identical shape (defined here as the canonical block).

## Incremental filter (trigger-fired derived `breadth=incremental` only)

When derived `breadth=incremental` (trigger-fired caller, no `--start`/`--end`), build the **PU work set** BEFORE Phase 1 from the source ledger and the per-artifact coverage metadata:

1. **Read the source ledgers** — for each source in resolved `--source` (or all monitored sources when omitted), read `<state.path>/triggers/<source-id>.json` (schema above). Version-keyed sources compare `last_seen_version`; time-keyed sources compare `last_seen_timestamp`.
2. **Compute the PU work set** — for each in-scope artifact, for each applicable dimension (per `active-dimensions-follow-evidence`), include the PU iff it is **stale**: its recorded `source_versions[dep]` is older than the dependency's ledger watermark, OR `status != pass`, OR `status == never`. Never-covered and non-pass PUs are ALWAYS included (at-least-once guarantee). PUs whose recorded `source_versions` equal the ledger latest AND `status == pass` are skipped (no-redundant).
3. **Build scope** — only artifacts owning at least one stale PU are in scope. Phase 1 produces a change digest (not a snapshot) per the researcher output contract.
4. **Apply filter** — all audit phases (2, 3, 4) process ONLY the stale PUs.
5. **Persist new state** — at end of Phase 8, write each processed PU's coverage cell and advance the per-source ledger `last_seen_*`.

If a source ledger file is missing on a trigger-fired call, warn `No prior ledger for source <id>; treating all its PUs as never-covered (full sweep for this source)` and treat that source's PUs as stale while leaving the global derived `breadth=incremental` intact for unaffected sources.

**Manual callers MUST NOT reach this branch.** `--incremental` is rejected for manual callers per CF-05 (default-full-investigation). Manual + window callers reach the bounded-delta branch instead, where the explicit `--start` bound is the watermark and recorded `pass` coverage is **overridden** inside `[--start, --end]` (every PU in the window is reprocessed — the re-baseline / distrust-recovery path).

---

## Phase 1: Source Research (skip with `--skip research` — only when derived `breadth ≠ full`)

External knowledge gathering. Feeds findings into Phases 1.5, 2, 3, 4 to inform audit research substeps.

> **Research output contract (vision v14).** The researcher MUST emit ONE of three template-bound output shapes, selected by derived breadth:
>
> | Derived breadth | Output shape | Template |
> |---|---|---|
> | `full` | Current-state snapshot across the entire monitored-sources catalog | `pe-meta-research-snapshot.template.md` |
> | `incremental` | Change digest for the stale PUs (per § Incremental filter) | `pe-meta-research-digest.template.md` |
> | `bounded-delta` | Bounded-window digest between explicit `--start`/`--end` (date or source-version) | `pe-meta-research-window-digest.template.md` (includes `window.start`/`window.end` fields) |

### 1a: Full Research (default)

Delegate to `@meta-researcher` with the resolved canonical invocation (the agent reads `--source`, derived `breadth`, `--start`/`--end`). The agent loads `pe-self-update.config.json` for the monitored-sources catalog and state location. Produce a self-contained report shaped by the contract above with:
- Evidence from authoritative sources (distilled context files **always**, 05.02 reference articles **always**, internet research **always** unless `--skip external`)
- User-provided authoritative sources (URLs, files) analyzed when supplied
- **Critical validation of internet findings** — each external finding must be evaluated for whether integrating it would improve artifact reliability, effectiveness, or efficiency; findings that are unuseful, unverifiable, or potentially misleading are flagged and excluded from recommendations
- Improvement opportunities mapped to affected artifacts and quality dimensions
- PE structure assessment (inventory, symmetry, orphans, context coverage)
- For each opportunity: **2+ alternative approaches** compared on effectiveness, reliability, efficiency
- Prioritized recommendations classified by audit phase (structure / consistency / content)

**When scope is a specific file path**: Research focuses on that artifact and its dependency chain only.

### 1b: Direct application (`--scope <file.md>` + `--mode apply --skip research`, derived `breadth ≠ full`)

When the caller scopes to a single file path AND `--skip research` AND derived `breadth ≠ full` (i.e., `bounded-delta`):
1. Read the target artifact completely
2. Load relevant instruction file for the artifact type
3. Load the dependency map for consumer impact
4. Skip to Phase 5 (user approval) with user's change description as the change spec

This branch is unreachable from manual + no-window callers (rule #2 rejection prevents it).

---

## Phase 1.5: Organizational Pass (skip with `--skip organizational`)

**Gate.** Runs ONLY when derived `breadth=full` AND the resolved `--scope` is broader than a single file (either an artifact-type token resolving to ≥2 files, or a path set with ≥2 files). Skipped on single-file scopes and on `incremental`/`bounded-delta` breadths.

**Goal.** Before the per-artifact audits (Phases 2–4) drill into individual files, the organizational pass verifies cross-artifact organizational concerns that only surface at the catalog level:
1. **Inventory completeness** — every PE artifact type has at least one researcher/builder/validator pairing where the architecture requires symmetry.
2. **Orphan detection** — every artifact has at least one declared consumer in the dependency map (orphan flag if not).
3. **Layer cross-cuts** — context-layer rules referenced by ≥2 instruction layers are flagged for canonical-source verification.
4. **Naming and locator parity** — file paths, frontmatter `name:` fields, and `STRUCTURE-README` table rows reconcile.
5. **Series and sequence integrity** — `Order in group:` markers, sequence labels, and duplicate H1s are detected.
6. **Cross-domain consumer chains** — agents/prompts that depend on context files outside their declared category are surfaced.

Findings from this phase scope Phase 2 (structure) — Phase 2 narrows from "everything" to "everything plus the organizational concerns Phase 1.5 surfaced".

**Execution.** Orchestrator runs this directly (using `list_dir`, `file_search`, `grep_search`, `read_file`). Do NOT delegate to a per-type validator. Output: organizational findings report consumed by Phase 2's screening step.

---

## Phase 2: Structure Audit (skip with --skip structure)

**Goal**: Validate the PE artifact ecosystem's structural integrity — what files exist, where they are, what role and rules each contains, whether the layout follows conventions.

**When `--mode apply` (and `--skip structure` is not set)**: Runs all three substeps (Research, Build, Validate).
**When `--mode plan`**: Runs Research substep only — Build and Validate substeps are skipped because plan mode is assessment-only. Findings feed into Phase 8 report.

### 2-Research

The orchestrator performs structural inventory directly (using `list_dir`, `file_search`, `grep_search`, `read_file`). This is orchestrator-owned work — do NOT delegate to meta-validator for this step.

**Checks:**
1. Artifact inventory by type across all PE locations
2. Location compliance, role clarity (YAML frontmatter), rules presence (boundaries, tool alignment)
3. Builder/validator symmetry, orphan detection (via dependency map)
4. STRUCTURE-README alignment, dependency map accuracy

When Phase 1 produced findings, incorporate validated external best practices. When `--skip external`, compare against internal conventions and 05.02 reference articles.

**Challenge step**: For each issue, propose **2+ resolution options**. Compare on effectiveness, reliability, efficiency. Recommend with rationale.

**Output**: Structural findings report with severity-scored issues and ranked options.

### 2-Build (`--mode apply` only)

Delegate to `@meta-designer`: Transform structural findings into change specifications. Each spec independently executable by a type-specific builder. Include: rationale, alternatives considered, why recommended option wins, layer-ordered execution sequence.

### 2-Validate (`--mode apply` only)

Delegate to `@meta-validator` (Design Validation mode): Verify structural changes won't break capabilities, respect dependency order, preserve builder/validator symmetry. Verdict: SAFE / FIX / UNSAFE.

---

## Phase 3: Consistency Audit (skip with --skip consistency)

**Goal**: Validate cross-artifact consistency — goal alignment across related artifacts, non-ambiguity, non-redundancy, non-contradiction between files.

**When `--mode apply` (and `--skip consistency` is not set)**: Runs all three substeps (Research, Build, Validate).
**When `--mode plan`**: Runs Research substep only — Build and Validate substeps are skipped because plan mode is assessment-only. Findings feed into Phase 8 report.

### 3-Research

**Delegate to** `@meta-validator` (Ecosystem Audit mode, dimensions: `coherence+rules+references`). The validator owns the consistency analysis methodology — do NOT duplicate its checks here.

**Expected output from validator:**
1. Goal alignment issues across artifact chains
2. Ambiguity findings (rules interpretable differently across artifacts)
3. Redundancy findings (same rule stated in multiple files, with canonical source identified)
4. Contradiction findings (conflicting rules across files)
5. Cross-reference integrity issues (broken references, missing handoff targets)

When Phase 1 produced findings, incorporate validated external standards for consistency improvements. When `--skip external`, compare against internal conventions and 05.02 reference articles.

**Challenge step**: For each consistency issue from the validator, propose **2+ resolution strategies**. Compare on effectiveness, reliability, efficiency. Recommend with rationale.

**Output**: Consistency findings report with severity-scored issues and ranked resolution options.

### 3-Build (`--mode apply` only)

Delegate to `@meta-designer`: Design change specifications for consistency fixes. Respect layer hierarchy (context owns rules, instructions enforce, agents apply). Deduplicate by consolidating into canonical sources. Max 2 clarification rounds with `@meta-researcher`.

### 3-Validate (`--mode apply` only)

Delegate to `@meta-validator` (Design Validation mode): Verify consistency fixes don't introduce new contradictions, preserve all capabilities, and maintain single-source-of-truth. Verdict: SAFE / FIX / UNSAFE.

---

## Phase 4: Content Audit (skip with --skip content)

**Goal**: Validate individual artifact quality — per-file goal/role alignment, internal non-ambiguity, internal non-redundancy, internal non-contradiction, efficient structure.

**When `--mode apply` (and `--skip content` is not set)**: Runs all three substeps (Research, Build, Validate).
**When `--mode plan`**: Runs Research substep only — Build and Validate substeps are skipped because plan mode is assessment-only. Findings feed into Phase 8 report.
**When `--dim efficiency`**: Runs Research substep focused on efficiency/budgets/redundancy. Feeds into Phase 5.

### 4-Research

**Per-artifact prompt invocation matrix (vision v14).** Route Phase 4 work via the matrix from `(--scope-resolved-artifact-type, --dim)` defined above (§ Per-artifact prompt invocation matrix). The orchestrator selects ONE prompt per artifact-type slice and invokes it with the canonical scope. Do NOT hand-write per-artifact-type branches or call type-specific validators directly here.

Each per-artifact `pe-meta-{type}-review` prompt owns its own check inventory (goal/role alignment, internal non-ambiguity, internal non-redundancy, internal non-contradiction, efficient structure, convention compliance). The orchestrator MUST NOT duplicate those checks inline. Phase 1 findings flow into the matrix-selected prompt via the canonical handoff contract.

**Screening step (vision v14 § Default-full invocation contract).** Before invoking the matrix-selected prompt, screen the in-scope artifact set against the Phase 1 research output:
- For `breadth=full` (snapshot): compare current artifact state against the snapshot's `pe-relevant-changes[]` — flag any artifact whose declared dependencies appear in the changes list.
- For `breadth=incremental` (change digest): screen against `digest.entries[]` since per-source `last_review_timestamp` — narrow to artifacts touched by the change set.
- For `breadth=bounded-delta` (window digest): screen against `window.entries[]` between explicit endpoints — same narrowing logic but bounded.

When `--skip external`, screen against internal conventions and 05.02 reference articles only.

**Challenge step**: For each content issue surfaced by the matrix-selected prompt, propose **2+ improvement options**. Compare on effectiveness, reliability, efficiency. Recommend with rationale.

**Output**: Content findings report with per-file severity-scored issues and ranked improvement options.

**Coverage recording (MANDATORY — feeds the Phase 8 full-coverage linter).** For EACH in-scope artifact, when the matrix-selected `pe-meta-{type}-review` prompt is invoked, append one entry to the outcome log `.copilot/temp/pe-meta-state/outcomes/<run-id>.jsonl`:

```json
{"phase": 4, "file": "<artifact-path>", "prompt": "pe-meta-<type>-review", "bodies_read": true, "dims": ["D1-metadata", "..."], "dim_evidence": [{"dim": "D1-metadata", "status": "pass", "evidence_ref": "frontmatter L5: goal present and single-sentence"}, {"dim": "D30-metadata-guard", "status": "pass", "evidence_ref": "bottom comment L210: agent_metadata: matches in-scope majority key"}]}
```

- `bodies_read` MUST be `true` — Phase 4 reviews artifact BODIES, not just frontmatter. A metadata-only inspection does NOT count as coverage and MUST NOT emit an entry with `bodies_read: true`.
- The Phase 8 full-coverage linter computes `phase4-coverage=<covered>/<total>` by reconciling these entries against the resolved in-scope artifact set (`<total>`). Any in-scope artifact lacking a matching entry is uncovered and fails the linter on `--mode apply`.
- The `dims` array records which dimensions the per-artifact prompt exercised, feeding the `dims-exercised` marker and its breadth rule.
- **`dim_evidence[]` (MANDATORY — feeds `pu-evidence`).** Per the shared [evidence-bound coverage contract](../../prompt-snippets/pe-meta-evidence-coverage.md), record one `{dim, status, evidence_ref}` object for EVERY applicable dimension — **passes included**. A `status: pass` with an empty `evidence_ref` does NOT count as covered (an evidence-free PASS is indistinguishable from a dimension never exercised). The Phase 8 linter computes `pu-evidence=<evidenced>/<applicable>` from these objects and hard-fails on both plan and apply when any applicable PU is unevidenced.
- **Verbatim anchor (MANDATORY — R4 Layer A).** Per the contract's verbatim-anchor rule, every `evidence_ref` MUST carry an `L<line>` locator AND a backticked verbatim snippet copied from that exact location (the artifact path comes from the entry's `file`), e.g. `` "frontmatter L5: `goal: \"...\"`" ``. A bare prose assertion with no `path:line` + quote is not a valid anchor — it leaves the Phase 7d audit nothing to verify. *Evaluation: hook:.github/hooks/scripts/pe-check-evidence-anchors.ps1* (resolvability + literal-containment + distinctness, zero LLM calls).

**`--dim efficiency --mode apply` focus**: When `--dim efficiency` is active with `--mode apply`, delegate to `@meta-optimizer` instead of the per-artifact matrix; run efficiency-lens checks (token budgets, deduplication, structural improvements).

**`--dim adherence` focus**: When `--dim adherence` is active, the matrix routes every in-scope artifact through `pe-meta-adherence` for sampled consumer verification. Sampling parameters come from `pe-self-update.config.json` → `sampling.adherence_consumers_per_file`.

### 4-Build (`--mode apply` only)

Delegate to `@meta-designer`: Design change specifications across affected files. Each spec executable by its type-specific builder independently. Layer-ordered (L1 context, L2 instructions, L3 agents/skills, L4 prompts/templates).

### 4-Validate (`--mode apply` only)

Delegate to `@meta-validator` (Design Validation mode): Verify content changes maintain goal alignment, don't weaken boundaries, and improve overall quality. Verdict: SAFE / FIX / UNSAFE.

---

## Phase 4.5: Strategic Validation (conditional — when `--dim` includes a vision-alignment or relational-quality dimension)

**Goal**: Vision alignment, category-reference compliance, PE quality bar, N-1 separation, self-update readiness, and ecosystem-impact scoring.

**Trigger**: Runs when `--dim` includes the vision-alignment dimension or a relational-quality dimension — `D15-vision-alignment` (under `--dim strategic`), `D16-adherence`, `D17-cross-coherence`, `D18-coverage`, or `D19-artifact-structure` (the latter four under `--dim quality`) — i.e. `--dim strategic`, `--dim quality`, `--dim full`, or an explicit D15–D19 list per the [05.07 dimension catalog](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md).

**Precondition**: Phases 2–4 must complete with a PASS verdict. If any structure/consistency/content audit FAILs, Phase 4.5 is SKIPPED and its findings are emitted on an advisory-only severity channel separate from the structural PASS/FAIL channel — a structural FAIL never masquerades as a strategic advisory, and a strategic advisory never blocks a structurally-clean artifact.

**Process**:

1. **Vision alignment** — Load the current vision document and its rationales. For each target artifact, check compliance with applicable rationales; flag any rationale contradiction.
2. **Category-reference compliance** — Audit every `.copilot/context/` reference. Are they using Level 1.5 (category-level) references, or Level 2+ (file-specific) without justification? Flag unnecessary file-specificity.
3. **PE quality bar** — Compare against the vision § Guidance quality table (exemplary column). Report gaps between standard and exemplary; recommend specific improvements.
4. **N-1 separation audit** — Check the N-1 adoption table for the artifact type. If N-1 applies, verify rule-bearing sections use `**Rule**:` / `**Rationale**:` / `**Example**:` block labels. Missing labels block deterministic classification, escalating all diffs to LLM judgment.
5. **Self-update readiness** — Determine the rollout phase for the artifact type; check readiness criteria per phase; report gaps.
6. **Ecosystem-impact scoring** — Check the dependency map for this artifact's dependents. Count blast radius (3+ = high impact); record the impact score.
7. Emit strategic findings on the advisory severity channel (separate from structural; not blocking).

**Output**: Strategic verdict (PASS / PASS-with-improvements / ADVISORY / FAIL) plus finding details, folded into the Phase 8 report under a dedicated strategic-findings table.

**Delegates to**: `@meta-validator` when the artifact is high-impact (6+ dependents) or a cross-artifact ecosystem review is needed.

---

## Phase 5: User Approval (`--mode apply` only — NEVER skippable)

**Context checkpoint**: Before starting Phase 5, run the Context Checkpoint Protocol. Write the consolidated change plan to `.copilot/temp/pe-meta-state/phase-5-changelist.md` using `create_file`. This file becomes the single source of truth for Phases 6–8 — conversation history from Phases 1–4 is no longer needed.

Present consolidated change plan from all enabled audit phases with **diff-based preview**:

```markdown
### Change [N]: [title] (from Phase [2/3/4]: [structure/consistency/content])

**File:** `[path]`
**Impact:** [Low/Medium/High] ([N] dependents)
**Alternatives considered:** [option A vs option B — why this one wins]

**Current** (lines [N]-[M]):
> [existing content]

**Proposed:**
> [new content]

**Rationale:** [why this change is needed + which quality dimension it improves]
```

Approval options: approve all / select / refine / expand research / cancel.

### Override Logging

When the user approves a change that `@meta-validator` rated as **UNSAFE**, or overrides a **CRITICAL** finding (approves despite the recommendation to block):

1. **Log the override** in `05.04-meta-review-log.md` → "Override History" table:
   - Date: today
   - Finding: brief description of what the validator found
   - Severity: CRITICAL or UNSAFE
   - User decision: what the user chose (e.g., "Approved despite UNSAFE")
   - Rationale: user's stated reason (ask if not provided — record "No rationale provided" if declined)
   - Follow-up due: next scheduled review date
   - Follow-up result: _(blank — filled by scheduled review)_

2. **Proceed with the approved change** — the override doesn't block application, but the dissenting opinion is preserved for institutional memory

**When `--dim efficiency --mode apply`**: Present optimization plan with estimated line/token savings and diff previews. Options: approve / select / cancel.

**If no findings from Phases 2-4**: Report "System is healthy — no changes needed" and skip to Phase 8.

---

## Phase 6: Apply Changes (skip with --mode plan — NEVER without Phase 5 approval)

**Context checkpoint**: Read approved changes from `.copilot/temp/pe-meta-state/phase-5-changelist.md`. Do NOT rely on conversation history from Phases 1–4.

### Plan materialization (every mutating run — NEVER skippable)

**Both `--mode plan` and `--mode apply` MUST materialize the actionable plan file BEFORE any source-artifact write.** Plan emission is non-skippable; `--skip plan-emission` is REJECTED with CF-05. The plan file is the single pivot artifact every mutating run passes through (vision v15.4 § Plan output contract — `apply = plan + execute`; see [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md)):

1. **Resolve the path** via the contract's path-resolution algorithm: default `<run-folder>/<NN>-<kebab-name>.plan.md`, fallback `.copilot/temp/pe-meta-state/plans/YYYYMMDD-HHMMSS-<kebab-name>.plan.md`; honor `--plan-file` when supplied (location/identity override ONLY).
2. **Write the plan** from the Phase 5 changelist using `create_file`: frontmatter (`status`, `target_vision_version`, `domain`, `created`, `goal`) per `plan-execution.instructions.md`; one goal-table row per validated finding carrying `scope tag`, `principle impact`, `downstream landing`; items decomposed to actionability-gate granularity with execution-ready `old → new` edits or unambiguous anchors; § Park lot; exit criteria.
3. **Record `plan-file=<path>`** for Phase 8 to echo on the first-line log (NEVER `none` on a mutating run).

**If `--mode plan`**: After writing the plan, STOP this phase. Produce report (Phase 8) with the validated plan, marking changes as "PLANNED — not applied". Include command to apply later: `/pe-meta-review --mode apply <same-source> --scope <same-scope>`.

**If `--mode apply`**: After writing the plan, execute it through the steps below (Rollback Snapshots → Apply). The execution steps read from the plan file (and the Phase 5 changelist) — they do NOT re-derive goal or scope (the plan→execute model seam: a standard/cheaper executor applies the plan's edits, it does not re-reason intent).

### Rollback Snapshots

Before modifying any file:
1. Create `.copilot/temp/rollback/` directory if it doesn't exist
2. For each file, copy current content to `.copilot/temp/rollback/{filename}.{timestamp}.bak` using `create_file`
3. Record manifest in `.copilot/temp/rollback/manifest.md` with: original path, snapshot path, timestamp, change description

### Apply

**Default `--mode apply` path**: Delegate to per-type builders (prompt/agent/context/instruction/skill/hook/snippet-builder) with full change specs and per-step self-validation.

**When `--dim efficiency --mode apply`**: Delegate to `@meta-optimizer`. Process ONE file at a time. Verify no rules/capabilities lost.

### Phase 6 outcome-log append (vision v14)

After every successful apply step (or at end of Phase 6 if all-or-nothing batches were used), append an outcome entry to `.copilot/temp/pe-meta-state/outcomes/<run-id>.jsonl` (one JSON object per line) shaped:

```json
{ "phase": 6, "artifact": "<path>", "change": "<title>", "classification": "breaking|non-breaking", "confidence": "deterministic|llm-assisted", "autonomy": "autonomous|approved", "validations_passed": true|false, "outcome": "applied|rolled-back|skipped", "timestamp": "<ISO-8601>" }
```

This file is the source of truth for Phase 8's outcome rollup and for trigger-fired callers that need to advance per-source `last_review_timestamp` only after successful applies.

### Phase 6 per-cycle iteration budget (vision v15.1 § Iteration budget)

**Default cap:** 10 autonomous changes per cycle (configurable). Only `autonomy: autonomous` changes count; `autonomy: approved` and rejected/skipped findings do NOT count.

**Overflow detection.** After each apply step, the orchestrator increments the autonomous-change counter. When the counter reaches the cap AND at least one validated finding remains unapplied, the orchestrator MUST:

1. Stop applying further autonomous changes.
2. Emit a **spillover plan file** at `<run-folder>/<NN>-<kebab-name>-spillover.plan.md` following the path-resolution algorithm in [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md). One goal-table row per remaining-but-unapplied validated finding carrying `scope tag`, `principle impact`, `downstream landing`, and `original-run=<run-id>`.
3. Record `spillover=<path>` for Phase 8 to echo on the first-line log; otherwise record `spillover=none`.

The spillover plan emission is NOT skippable. See snippet for full contract.

---

## Phase 7: Regression Test (mandatory when Phase 6 applies changes)

Two-part validation:

### 7a: Type-Specific Validation

`@meta-validator` in Implementation Validation mode. Validate: Completeness, Effectiveness, Reliability, Efficiency, Security and Guardrails. Delegate per-artifact to type-specific validators.

### 7b: Capability Regression Test

The orchestrator MUST run this directly (using `file_search`, `grep_search`, `read_file`):

1. **Load** `00.01-governance-and-capability-baseline.md` — read all use case tables (Categories 1-5)
2. **Entry point verification**: For each use case entry point (e.g., `/prompt-design`, `@agent-builder`), verify the prompt or agent file exists. Missing = CRITICAL.
3. **Chain verification**: For each artifact chain (e.g., `prompt-researcher` then `prompt-builder` then `prompt-validator`), verify every agent exists and every `handoffs:` target resolves. Missing = CRITICAL.
4. **Tool alignment**: For each agent in modified chains, verify mode/tool consistency. `plan` + write tool = CRITICAL.
5. **Quality preservation**: For each modified file:
   - Count Always Do / Ask First / Never Do items — any decrease = HIGH
   - Count critical keywords (MUST, NEVER, CRITICAL) — decrease >20% = HIGH
   - Check tool additions to plan-mode agents = CRITICAL
   - **Version-sync (deterministic)**: for each modified agent/prompt, `grep` the frontmatter `version:` and the bottom `*_metadata.version:` — a mismatch is **HIGH** (the recurring "bottom block advanced, frontmatter stale" desync, e.g. validator frontmatter `2.2.2` while its bottom block read `2.2.3`). This is a mechanical string compare, not a judgement call, so it MUST be caught by the check rather than left to a human reviewer.
6. **Capability-implementer coverage (deterministic existence lint)**: load the vision `scope.covers` block and `00.02-capability-map.md`. For every **P0/P1** capability, verify a matching use-case row exists in the map and its command entry point resolves on disk. A P0/P1 capability with no row or an unresolvable command is **CRITICAL** (a promised capability with no implementer). Deeper handoff-chain resolution is covered by check 3 above, read from each prompt's `handoffs:` on demand — chains are no longer stored in the map (B0, 2026-06-25), so this check is a cheap existence lint, not a chain trace. Cohorts are globbed by artifact type and thresholds read from the vision — adding a capability or renaming an artifact requires NO edit to this check.
7. **Regression report**:

```markdown
### Capability Regression Test Results

| # | Use case | Entry point | Chain status | Tool alignment | Result |
|---|---|---|---|---|---|
| [N] | [name] | pass/fail | pass/fail [details] | pass/fail | PASS/FAIL |

**Broken capabilities:** [N]
**Verdict:** [No regressions / N broken — BLOCK until fixed]
```

If ANY capability is broken, **BLOCK Phase 8** and present broken capabilities with rollback instructions.

### 7c: Iteration Decision Gate

After 7a/7b results are available, classify the failure type and route accordingly:

| Failure type | Indicators | Action |
|---|---|---|
| **Implementation error** | Wrong content, missed file, typo in reference | Fix the specific file(s) and re-run 7a/7b (local iteration, max 3) |
| **Design flaw** | Change spec was structurally wrong — correct content but wrong target, wrong layer, or missing dependency | Cycle back to **Phase 4** (Content Audit) with updated scope. Carry forward: what failed and why. |
| **Fundamental misalignment** | Approach doesn't achieve stated goal — the entire change direction was wrong | Cycle back to **Phase 1** (Source Research) OR **STOP** and escalate to human with full context. |

**Global iteration budget**: Maximum 2 global cycle-backs (Phase 7 → Phase 1 or 4 → ... → Phase 7). If still failing after 2 global iterations → **STOP** and report all accumulated findings to human.

**Default path** (no failure): Proceed to Phase 8.

### 7d: Independent Coverage Audit (R1 — break the self-attestation loop)

Phases 4–7 are narrated by the orchestrator, so the orchestrator computing `pu-evidence`/`subcheck-coverage`/`shallow-sweep` from its own outcome log is **self-attestation**. Before Phase 8, the orchestrator MUST hand the run's outcome log to a **second actor** — `@pe-meta-validator` in **Coverage Audit** mode (read-only, separate context window) — which independently re-derives the coverage verdict per the shared [evidence-bound coverage contract](../../prompt-snippets/pe-meta-evidence-coverage.md) § Independent audit.

1. **Deterministic pre-pass (Layer A, every PU).** Run `.github/hooks/scripts/pe-check-evidence-anchors.ps1 -RunId <run-id> -AsJson`. Its `violations[]` (resolvability, literal-containment, distinctness, missing-anchor) are appended **verbatim** to the run report. Any violation forces `shallow-sweep=suspected`. This is the R5-full deterministic guard — code, not judgement, and tolerant (it derives each artifact path from the entry's `file`, hardcodes no cohort).
2. **Validator audit (Layer B — RE-EXECUTE, do not re-read).** Invoke `@pe-meta-validator` (Coverage Audit mode) on `.copilot/temp/pe-meta-state/outcomes/<run-id>.jsonl`. The validator MUST **re-execute a sample of declared sub-checks against the live artifact** (an adversarial counterexample probe — e.g. independently confirm the agent body carries the collective grounding directive AND that no body Always/Never entry restates a YAML `boundaries:` entry verbatim or contradicts/weakens it, for the H14 row) rather than re-reading the writer's quotes, which inherits the writer's blind spot. Sample size reuses the existing convention `N = max(3, ceil(0.15 × declared sub-checks))`, weighted toward `⚠️ Partial`/reasoning sub-checks. It independently recomputes `pu-evidence=<evidenced>/<applicable>` and `subcheck-coverage=<evaluated>/<declared>` per dimension, runs Layer-B sampling + on-doubt re-reads, and emits its own per-dimension graded verdict (`verified`/`pass-weak`/`partial`/`fail`) and `shallow-sweep` verdict.
3. **Reconcile.** The orchestrator's markers MUST match the validator's. Any divergence (different `pu-evidence`, different `subcheck-coverage`, a different dimension grade, a different `shallow-sweep`, or a Layer-A violation the orchestrator did not surface) is recorded for the Phase 8 reconciliation linter (rule 7).

**Acceptance:** a planted evidence-free or fabricated-anchor PASS in the outcome log is caught HERE — by the guard or the validator — not by the orchestrator that wrote it; AND a dimension whose easy sub-check was cited while a harder declared sub-check went unevaluated is caught as `partial` by the re-execution probe, not narrated as a clean PASS.

---

## Phase 8: Report + Log (NEVER skippable)

**Report template**: `.github/templates/00.00-prompt-engineering/output-pe-management-report.template.md`

> **First-line `Resolved invocation:` log (vision v14 — success criterion #12, extended in v15.1, v15.4 coverage markers).** The Phase 8 report MUST begin with one machine-parseable line in this exact shape:
>
> ```text
> Resolved invocation: --mode=<plan|apply> --scope=<token-or-path-set> --source=<id-or-csv-or-empty> --dim=<group|D#> --start=<resolved-date|none> --end=<resolved-date|none> --deps=<value> --skip=<csv-or-empty> | breadth=<full|incremental|bounded-delta> | caller=<manual|trigger-fired> | plan-file=<path-or-none> | spillover=<path-or-none> | research=<ran|skipped|n-a> | phase4-coverage=<covered>/<total> | dims-exercised=<full|csv-of-D#> | pu-evidence=<evidenced>/<applicable> | subcheck-coverage=<fully-covered-dims>/<applicable-dims> | shallow-sweep=<clean|suspected>
> ```
>
> The orchestrator MUST also have emitted this identical line BEFORE Phase 1 ran. The Phase 8 echo gives the consumer the resolved invocation as the very first observable artifact of the report, before any narrative. **v15.4 markers:** `plan-file=<path>` is emitted on **every mutating run** — both `--mode=plan` and `--mode=apply` materialize the canonical plan file under vision § Plan output contract (`apply = plan + execute`); it is `none` ONLY on rejected or no-op runs (never on a run that applied changes). `spillover=<path>` is emitted when Phase 6 hit the iteration-budget cap with remaining findings, otherwise `spillover=none`. **v15.3:** `--start`/`--end` are echoed as **resolved timestamps** even when the caller supplied a source-version token (the original token is preserved in the report narrative).
>
> **v15.4 coverage-evidence markers (the full-processing proxy).** Three markers make the actual processing depth observable so a shallow run cannot masquerade as a full one (the failure mode in [the 2026-06-06 full-processing issue](../../../src/docs/90.%20Issues/202606/20260606.02-pe-meta-update/01-small-instruction-files-changes/overview.md)):
>
> - **`research=<ran|skipped|n-a>`** — `ran` when Phase 1 source research executed; `skipped` when `--skip research` suppressed it; `n-a` only when no phase consuming research ran. At derived `breadth=full` + `--mode apply`, `skipped` is a linter hard-failure (research is non-skippable on a full sweep).
> - **`phase4-coverage=<covered>/<total>`** — `<total>` = count of in-scope artifacts resolved for Phase 4; `<covered>` = count for which the matrix-selected per-artifact `pe-meta-{type}-review` prompt was actually invoked (reconciled from the per-file Phase 4 outcome-log entries). On `--mode apply`, `<covered> < <total>` is a linter hard-failure.
> - **`dims-exercised=<full|csv-of-D#>`** — `full` when every dimension applicable to the resolved artifact type(s) was exercised; otherwise the explicit `D#` CSV actually run. When `--dim full`, a strict subset is a linter hard-failure.
> - **`pu-evidence=<evidenced>/<applicable>`** *(depth-of-evidence — the shallow-PASS proxy)* — `<applicable>` = count of applicable processing units `(artifact × dimension)` in scope; `<evidenced>` = count whose `dim_evidence[]` entry carries a non-empty `evidence_ref` (**passes included**, per the shared [evidence-bound coverage contract](../../prompt-snippets/pe-meta-evidence-coverage.md)). `<evidenced> < <applicable>` is a hard failure on **BOTH `--mode plan` and `--mode apply`** — an evidence-free PASS is indistinguishable from a dimension never exercised. This closes the gap that let the 2026-06-07 run report `dims-exercised=full` while three reliability/metadata residuals sat unexamined.
> - **`subcheck-coverage=<fully-covered-dims>/<applicable-dims>`** *(sub-check-depth — the shallow-DIMENSION proxy, vision v15.5)* — a dimension is *fully-covered* when every 05.08 sub-check declared for the artifact type carries its own anchored `evidence_ref` (`<evaluated>/<declared>` ratio = 1). `<fully-covered-dims> < <applicable-dims>` means ≥1 dimension is graded `partial` (a declared sub-check never ran) — this **BLOCKS a clean health score** regardless of which dimension is thin, closing the gap that let the 2026-06-11 run report `D5` clean on the boundary *count* while the H14 grounding sub-checks (collective-directive presence, no-verbatim-restatement, no-contradiction) never executed. Per-dimension `<evaluated>/<declared>` ratios and grades (`verified`/`pass-weak`/`partial`/`fail`) are recorded in the report body; this line carries the aggregate. The per-dimension recorded state uses the **unified enum** — the graded verdict plus the `never` sentinel — not a separate coverage `status` enum (see the shared contract § Unified recorded-state enum).
> - **`shallow-sweep=<clean|suspected>`** *(breadth-vs-depth heuristic — demoted to a BACKSTOP behind `subcheck-coverage`)* — `suspected` when, on `--dim full`/`breadth=full`, every finding clusters in frontmatter dims `D1-metadata`–`D5-boundaries` AND the content (`D9`–`D11`), efficiency (`D20`–`D27`), and reliability (`D28`–`D35`) groups produced zero findings and zero body-level evidence-cited passes (per the shared contract). `suspected` BLOCKS a clean health score until body-level evidence or an explicit acknowledgment is supplied. The deterministic `subcheck-coverage` gate now catches thin dimensions regardless of group, so `shallow-sweep` is the secondary heuristic, not the primary depth guard.

#### First-line-log linter (MANDATORY — mutating-run plan-file invariant)

Before emitting the Phase 8 report, the orchestrator MUST self-check the first-line `Resolved invocation:` log against the outcome log (`.copilot/temp/pe-meta-state/outcomes/<run-id>.jsonl`):

1. **Mutating-run detection** — a run is *mutating* when the outcome log contains at least one `"outcome": "applied"` entry, OR `--mode=plan` materialized a plan. (A run with only `rolled-back`/`skipped` outcomes and no plan emission is non-mutating.)
2. **Invariant** — on a mutating run, the first-line log MUST carry `plan-file=<path>` pointing at an existing on-disk file; `plan-file=none` on a mutating run is a **hard failure**, not a soft marker.
3. **On violation** — BLOCK the report, surface the violation (`mutating run logged plan-file=none — vision v15.4 § Plan output contract`), and either (a) materialize the missing plan from the Phase 5 changelist and re-emit the log, or (b) escalate to the user if the changelist is unavailable. NEVER publish a report whose marker contradicts the outcome log.

This linter is the runtime guard for the recurrence documented in the 2026-06-06 plan-file back-fills (`full-apply-20260606`, `agents-apply-20260606`).

#### Full-coverage linter (MANDATORY — full-processing invariant)

Before emitting the Phase 8 report, the orchestrator MUST self-check the coverage-evidence markers against the outcome log AND the resolved invocation. This is the runtime guard for the [2026-06-06 full-processing issue](../../../src/docs/90.%20Issues/202606/20260606.02-pe-meta-update/01-small-instruction-files-changes/overview.md), where a `breadth=full` + `--mode apply` run collapsed into a frontmatter-only metadata scan (1 of ~35 dimensions, no Phase 1 research, no per-artifact review) yet still emitted a structurally valid success report. It mirrors the plan-file linter pattern above: a load-bearing `MUST` paired with a hard runtime invariant.

1. **Research-ran rule** — if derived `breadth=full` AND `--mode=apply` AND `research=skipped`, this is a **hard failure** (`full-breadth apply skipped non-skippable Phase 1 research — vision § Default-full invocation contract`). (`--skip research` at `breadth=full` is already rejected at parse time UNLESS `--plan-file` references a validated baseline; in that trust-mode exception `research=n-a` is acceptable and this rule is satisfied by the baseline.)
2. **Phase 4 coverage rule** — if `--mode=apply` AND the `phase4-coverage` numerator `<covered>` is less than the denominator `<total>` (i.e., the matrix-selected per-artifact `pe-meta-{type}-review` prompt was NOT invoked for every in-scope artifact), this is a **hard failure** (`Phase 4 under-covered: <covered>/<total> artifacts reviewed — per-artifact review is mandatory on apply`). `<covered>` is reconciled from the per-file Phase 4 outcome-log entries (§ Phase 4 — Coverage recording), NOT self-attested.
3. **Dimension-breadth rule** — if `--dim full` AND `dims-exercised` is a strict subset of the dimensions applicable to the resolved artifact type(s) per `05.07-pe-meta-dimension-catalog.md`, this is a **hard failure** (`--dim full exercised a strict subset: <list> — full requires the complete applicable dimension set`).
4. **Evidence-depth rule (mode-independent)** — per the shared [evidence-bound coverage contract](../../prompt-snippets/pe-meta-evidence-coverage.md), if `pu-evidence` numerator `<evidenced>` is less than denominator `<applicable>` (any applicable PU — **pass or finding** — has an empty `evidence_ref`), this is a **hard failure on BOTH `--mode plan` and `--mode apply`** (`evidence-free coverage: <evidenced>/<applicable> PUs evidenced — every applicable dimension must cite proof, passes included`). Reconciled from the per-file `dim_evidence[]` outcome-log entries, NOT self-attested.
5. **Shallow-sweep rule (backstop)** — per the shared contract, if `shallow-sweep=suspected` (findings cluster in frontmatter dims `D1`–`D5` AND the content/efficiency/reliability groups are silent of findings and body-level evidenced passes) on a `--dim full`/`breadth=full` run, BLOCK a clean health score (`shallow-sweep suspected — body groups produced no evidence; supply body-level evidence_ref or acknowledge`) until body-level evidence or an explicit acknowledgment is recorded. This is now a **backstop** behind rule 6 — the deterministic sub-check gate catches thin dimensions regardless of group.
6. **Sub-check coverage / graded-verdict rule (mode-independent, vision v15.5)** — per the shared [evidence-bound coverage contract](../../prompt-snippets/pe-meta-evidence-coverage.md) § graded dimension verdict, if any applicable dimension's `subcheck-coverage=<evaluated>/<declared>` ratio is `< 1` (a declared 05.08 sub-check for the artifact type went unevaluated → the dimension is graded `partial`) OR any dimension is graded `fail`, this **BLOCKS a clean health score** (`dimension <D#> graded <partial|fail>: subcheck-coverage <evaluated>/<declared> — every declared sub-check must be evaluated before the dimension counts as covered`). `partial` is computed deterministically from the ratio — no heuristic, no sampling — and fires regardless of which dimension is thin (it catches `D5`/`D6`/`D19`, none of which sit in the frontmatter cluster rule 5 watches). A `pass-weak` grade (a property sub-check backed only by a presence anchor) is surfaced as a flag but does not hard-block. Reconciled from the per-dimension sub-check entries in the outcome log, NOT self-attested.
7. **Independent-audit reconciliation rule (R1, mode-independent)** — per the shared contract § Independent audit, if the Phase 7d `@pe-meta-validator` verdict diverges from the orchestrator's own markers (`pu-evidence` mismatch, `subcheck-coverage`/dimension-grade mismatch, `shallow-sweep` mismatch, or any `pe-check-evidence-anchors.ps1` violation the orchestrator did not surface), this is a **hard failure** (`coverage markers not reconciled with independent audit — self-attested verdict rejected`). The two actors MUST agree before a clean health score is published; this is what makes "reconciled, NOT self-attested" literally true.
8. **On any violation** — BLOCK the report, surface the specific rule that failed, and either (a) resume the missing phase work (run Phase 1 research, invoke the per-artifact review for the uncovered files, exercise the missing dimensions, evaluate the unevaluated sub-checks against the live artifact, produce the missing per-dimension evidence, or re-run the Phase 7d audit after fixing anchors) and re-emit the markers, or (b) escalate to the user if the work cannot be completed in-session. NEVER publish a report whose coverage markers contradict a full-breadth claim.

A run that legitimately found nothing to change still PASSES this linter — the invariant checks that the *work was done*, not that findings exist (a clean full review emits `research=ran`, `phase4-coverage=N/N`, `dims-exercised=full` with zero findings).

Report includes: **first-line `Resolved invocation:` log**, mode, scope, date, source, derived breadth, **phases executed** (which were skipped and why), artifacts analyzed, issues found by severity **and by phase** (organizational/structure/consistency/content), changes applied, health score (`--mode plan`: `100 - (CRITICAL*25 + HIGH*10 + MEDIUM*3 + LOW*1)`), token savings (`--dim efficiency`), outcome-log rollup (from `.copilot/temp/pe-meta-state/outcomes/<run-id>.jsonl`), rollback instructions.

### Audit Log Update

**Always runs regardless of `--mode` or `--dim`.**

Update `.copilot/context/00.00-prompt-engineering/05.04-meta-review-log.md`:

1. Append entry under appropriate mode section with: **first-line `Resolved invocation:` log**, date, mode, scope, derived breadth, phases executed, source, findings count by severity and phase, changes applied (or "None" / "Plan only"), health score, outcome-log run-id reference
2. **Write per-PU coverage cells.** For every processed PU, write its `coverage.<dimension>` cell (`source_versions`, `depth`, `status`, `last_run`) into the target artifact's bottom validation-metadata block (canonical shape per § Processing-state model). This per-artifact coverage is the durable single source of truth.
3. **Advance the source ledger.** For `--mode apply` runs that consulted external sources, update each consulted source's ledger `last_seen_version` / `last_seen_timestamp` / `last_digest_hash` in `<state.path>/triggers/<source-id>.json` ONLY for sources whose outcome entries are all `applied` or `skipped` (no `rolled-back`). The markdown **"Last Processed Versions"** table in `05.04-meta-review-log.md` is demoted to a human-readable **mirror** of the ledger — never the source of truth.
4. **Emit the coverage report line.** Append `coverage: <covered>/<total> PUs; <n> never-covered` to the Phase 8 report so the at-least-once frontier is observable.
5. Update `last_updated` in YAML frontmatter to today's date

### Test-Then-Apply Pattern

When invoked from `/pe-meta-scheduled-review` or with `--mode plan`:

```
--mode plan (runs 2R+3R+4R) — findings?
  No issues — log "clean" + stop
  Issues found — present plan to user (already produced by plan mode)
       — user approves? re-invoke with --mode apply (same scope)
       — user declines? log "deferred" + stop
```

---

## 🔄 Error Recovery Workflows

**📖 Recovery pattern:** [04.03-production-readiness-patterns.md](.copilot/context/00.00-prompt-engineering/04.03-production-readiness-patterns.md)

Meta-update-specific recovery:
- **fetch_webpage fails** → Switch to `--skip external` mode, warn user, continue with local artifacts
- **meta-validator reports UNSAFE** → BLOCK changes, present issues, ask user for resolution
- **Builder creates file with wrong structure** → Verify via `read_file`, hand off fix to builder (max 3 retries)
- **Context window exhausted** → Checkpoint progress to Phase 8 report, recommend new session for remaining work
- **Agent handoff returns empty** → Retry once with clarified delegation, then escalate to user

---

## 📋 Response Management

**📖 Response patterns:** [04.03-production-readiness-patterns.md](.copilot/context/00.00-prompt-engineering/04.03-production-readiness-patterns.md)

Meta-update-specific scenarios:
- **Source URL unreachable** → "URL [url] is unreachable. Running with `--skip external` using local artifacts only."
- **Research returns no PE-relevant changes** → Report "no PE-relevant changes found", skip to Phase 8
- **Ambiguous scope** → Ask user: "Your input matches multiple scopes: [list]. Which should I analyze?"
- **Phase produces zero findings** → Report phase result, proceed to next (don't fabricate issues)
- **Conflicting audit findings** → Present both findings with evidence, let user decide

---

## 🧪 Embedded Test Scenarios

| # | Scenario | Expected Behavior |
|---|---|---|
| 1 | `/pe-meta-review` (parameter-less manual, happy path) | Derived `breadth=full` → Phases 0 → 1 → 1.5 → 2 → 3 → 4 → 5 → 6 → 7 → 8 execute; Phase 6 materializes the canonical plan file BEFORE applying; first-line `Resolved invocation:` log carries `plan-file=<path>` (not `none`) and is echoed before Phase 1 AND as first line of Phase 8 report |
| 2 | `/pe-meta-review --mode plan` | Phases 0 → 1 → 1.5 → 2 → 3 → 4 → 8 execute; Phases 5/6/7 skipped; report marked "PLANNED" |
| 3 | `/pe-meta-review --start 2026-04-01` | Derived `breadth=bounded-delta`; Phase 1 emits window digest; Phase 1.5 SKIPPED (breadth ≠ full) |
| 4 | `/pe-meta-review --scope path/to/file.md` | Single-file scope; derived `breadth=full`; Phase 1.5 SKIPPED (single-file gate) |
| 5 | `/pe-meta-review --scope context` | Artifact-type token; derived `breadth=full`; Phase 1.5 RUNS (multi-file scope); Phase 4 routes via matrix to `pe-meta-context-review` |
| 6 | `/pe-meta-review --dim adherence --scope path.md --deps full` | Phase 4 routes via matrix to `pe-meta-adherence` for sampling |
| 7 | `/pe-meta-review --skip research` (manual, no window) | CF-05 rejection: rule #2 — `--skip research` incompatible with derived `breadth=full` |
| 8 | `/pe-meta-review --breadth full` | CF-05 rejection: `--breadth` retired in v14 |
| 9 | `/pe-meta-review --incremental` (manual caller) | CF-05 rejection: `--incremental` rejected for manual callers (would violate default-full-investigation) |
| 10 | `/pe-meta-review --incremental` (trigger-fired caller) | Accepted as single-window deprecation alias; resolves to derived `breadth=incremental` |
| 11 | No changes needed | All audits pass → report "healthy", log updated with clean health score |
| 12 | Phase 7 regression failure | BLOCKS Phase 8 → presents broken capabilities with rollback instructions |
| 13 | User approves UNSAFE change | Override logged in review log Override History → change applied |
| 14 | Free-form intent (e.g., `/pe-meta-review recheck consumer-correctness on the adherence prompt`) | Phase 0a resolves → canonical invocation echoed → user confirms → strict parser receives canonical eight-parameter set |
| 15 | `/pe-meta-review --start 1.099 --source vscode-release-notes` | Version-shaped bound, singleton source → Phase 0a resolves `1.099` to its publish timestamp via the source's `version_scheme`; derived `breadth=bounded-delta`; recorded `pass` coverage overridden inside the window (re-baseline) |
| 16 | `/pe-meta-review --start 1.099` (no `--source` / non-singleton) | CF-05 rejection: `version window requires a single --source` |
| 17 | `/pe-meta-review --start 2025-06-18 --source mcp-spec` then version token against a `version_scheme: none` source | A version-shaped bound against a `version_scheme: none` source is rejected: `source <id> has no version scheme; use a date window` |
| 18 | Trigger-fired incremental, a PU with recorded `source_versions` equal to ledger latest and `status=pass` | PU skipped (no-redundant); a never-covered or newer-source-version PU is always processed (at-least-once); coverage-report line emitted |
| 19 | `/pe-meta-review --scope agents --mode apply` (apply happy-path, changes applied) | Phase 6 materializes the canonical plan file BEFORE Rollback/Apply; first-line log carries `plan-file=<existing-path>` (NOT `none`); Phase 8 first-line-log linter PASSES |
| 20 | Apply run whose outcome log has an `"outcome": "applied"` entry but first-line log shows `plan-file=none` | Phase 8 first-line-log linter HARD-FAILS: BLOCKS the report, materializes the missing plan from the Phase 5 changelist (or escalates), re-emits the log with `plan-file=<path>` |
| 21 | `/pe-meta-review --scope instructions --mode apply` where Phase 1 was skipped (`research=skipped`) at derived `breadth=full` | Phase 8 full-coverage linter HARD-FAILS (research-ran rule): BLOCKS the report (`full-breadth apply skipped non-skippable Phase 1 research`); orchestrator resumes Phase 1 research and re-emits markers |
| 22 | `/pe-meta-review --scope instructions --mode apply` over 13 files but only 1 per-artifact review invoked (`phase4-coverage=1/13`) | Phase 8 full-coverage linter HARD-FAILS (Phase 4 coverage rule): BLOCKS the report (`Phase 4 under-covered: 1/13 artifacts reviewed`); orchestrator invokes the matrix-selected review for the 12 uncovered files before re-emitting |
| 23 | `/pe-meta-review --dim full --scope instructions --mode apply` that exercised only `D1-metadata` (`dims-exercised=D1-metadata`) | Phase 8 full-coverage linter HARD-FAILS (dimension-breadth rule): BLOCKS the report (`--dim full exercised a strict subset`); orchestrator exercises the remaining applicable dimensions before re-emitting |
| 24 | Apply run whose outcome log has a `dim_evidence` entry citing `path:line` + a backticked quote that does NOT appear at that line (fabricated anchor) | Phase 7d deterministic pre-pass (`pe-check-evidence-anchors.ps1`) reports a `literal-containment` violation → `shallow-sweep=suspected`; Phase 8 reconciliation rule 6 HARD-FAILS the report until the anchor is fixed |
| 25 | Apply run where the orchestrator self-reports `shallow-sweep=clean` but the Phase 7d `@pe-meta-validator` audit independently derives `suspected` | Phase 8 reconciliation rule 6 HARD-FAILS (`coverage markers not reconciled with independent audit`): the two-actor disagreement BLOCKS a clean health score until reconciled |

## Design/Review Parity Contract

Review and the per-artifact design prompts share ONE quality objective and ONE scope intent, filtered only by artifact applicability. Review validates against the SAME six guidance-quality properties the design prompts construct toward:

1. **Clarity** — unambiguous interpretation (LLM agreement test)
2. **Non-redundancy** — no duplicate rule definitions
3. **Non-contradiction** — rules don't conflict
4. **Completeness** — no gaps where guidance is needed
5. **Prioritization** — precedence is explicit when rules could conflict
6. **Actionability** — agents can act without additional interpretation

The role differs by process and mutation permission, not by quality ambition:

| Aspect | Design | Review (this command) |
|---|---|---|
| **Directive** | Synthesize requirements and construct to reach the quality target | Analyze, verify, and improve to keep the artifact at the quality target |
| **Scope** | Create new artifacts or comprehensive redesigns | Validate existing artifacts; refactor for drift |
| **Mutation permission** | Full authority (within governance rules) | Risk-calibrated: non-breaking autonomous, breaking escalated |
| **Quality bar** | The same six properties | The same six properties |

This parity ensures autonomous review improvements never lower quality, and new designs never ship if they'd fail the same review checks. The reciprocal obligation lives on the design side: each `pe-meta-{type}-design` prompt runs this command's applicable review dimension set (`--dim full`, applicability-scoped) as its final step before an artifact is considered done.

<!--
prompt_metadata:
  version: "3.2.1"
  last_updated: "2026-06-25"
  changelog: "pe-meta-review.prompt.changelog.md"
-->

