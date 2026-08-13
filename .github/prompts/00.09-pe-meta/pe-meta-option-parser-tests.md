# PE-meta option parser test evidence

Deterministic test scenarios documenting expected parser behavior for all PE-meta commands. Each row defines an input invocation and the expected system response — either acceptance with parsed values or rejection with corrective message.

**Normative reference:** `pe-meta-option-applicability-matrix.md`

## Accepted combinations by command family

### Review family

| # | Invocation | Parsed options | Notes |
|---|---|---|---|
| A-R01 | `/pe-meta-review path.md` | dim=full, deps=none, scope=all | All defaults |
| A-R02 | `/pe-meta-review path.md --dim structural` | dim=structural, deps=none, scope=all | Dimension filter |
| A-R03 | `/pe-meta-review path.md --deps full` | dim=full, deps=full (depth 5), scope=all | Full dep traversal |
| A-R04 | `/pe-meta-review path.md --deps direct` | dim=full, deps=direct (depth 1), scope=all | First-level deps |
| A-R05 | `/pe-meta-review path.md --deps 3` | dim=full, deps=3, scope=all | Explicit numeric depth |
| A-R06 | `/pe-meta-review path.md --scope context --deps full` | dim=full, deps=full, scope=context | Scope+deps compose: traverse full depth, focus on context deps |
| A-R07 | `/pe-meta-prompt-review path.md --scope context --deps direct` | dim=full, deps=direct, scope=context | Type-specific: filter to context deps, first-level only |
| A-R08 | `/pe-meta-agent-review path.md --scope instructions --deps full` | dim=full, deps=full, scope=instructions | Focus on instruction deps |
| A-R09 | `/pe-meta-review path.md --skip research` | dim=full, deps=none, scope=all, skip=[research] | Skip Phase 1 (type-specific: research valid) |
| A-R10 | `/pe-meta-review path.md --skip external` | dim=full, deps=none, scope=all, skip=[external] | No internet fetching |
| A-R11 | `/pe-meta-review path.md --dim quality --deps full --scope prompts --skip research` | dim=quality, deps=full, scope=prompts, skip=[research] | All options combined |
| A-R12 | `/pe-meta-review path.md --dim reliability` | dim=reliability (`D28-reproducibility` through `D35-portability-boundary`), deps=none, scope=all | Reliability group resolves to system-reliability dimensions |
| A-R13 | `/pe-meta-review path.md --dim adherence` | dim=adherence (`D5-boundaries`, `D6-consistency`, `D16-adherence`, `D18-coverage`), deps=none, scope=all | Canonical adherence group |
| A-R14 | `/pe-meta-review path.md --dim robustness` | _rejected_ | Retired alias: `robustness` is no longer accepted — see CF-05. Callers must pick `adherence` (consumer-correctness) or `reliability` (system-level) explicitly. |
| A-R15 | `/pe-meta-review path.md --dim invented-name` | _rejected_ | Unknown `--dim` value — see CF-05 |
| A-R16 | `/pe-meta-review path.md --dim &#68;6` | dim=[`D6-consistency`], deps=none, scope=all | Dual-acceptance: bare `D#` ID (without suffix) accepted (backward-compatible) and normalized internally to canonical form |
| A-R17 | `/pe-meta-review path.md --dim D6-consistency` | dim=[`D6-consistency`], deps=none, scope=all | Dual-acceptance: canonical `D#-readable-id` form accepted (preferred) |
| A-R18 | `/pe-meta-review path.md --dim D99` | _rejected_ | Out-of-range dimension ID — see CF-05; valid range is `D1-metadata` to `D35-portability-boundary` |
| A-R19 | `/pe-meta-review path.md --dim &#68;6-wrong-suffix` | _rejected_ | Suffix mismatch — `D6-consistency` canonical suffix is `consistency`, not `wrong-suffix` — see CF-05 |
| A-R20 | `/pe-meta-review path.md` | mode=apply, dim=full, deps=none, scope=all | `--mode` defaults to `apply` (review writes corrections in place — vision v15.2 § Option applicability matrix marks `--mode` ✅ for Review) |
| A-R21 | `/pe-meta-review path.md --mode plan` | mode=plan, dim=full, deps=none, scope=all | `--mode plan` previews corrections and emits a `plan-file=<path>` marker on the Phase 8 first-line log (no in-place writes) |
| A-R22 | `/pe-meta-review path.md --mode apply` | mode=apply, dim=full, deps=none, scope=all | Explicit `apply` matches the default; writes corrections in place |

### Creation family (design + create-update)

| # | Invocation | Parsed options | Notes |
|---|---|---|---|
| A-C01 | `/pe-meta-design "description"` | dim=full, scope=all | Defaults |
| A-C02 | `/pe-meta-prompt-design "description" --dim structural` | dim=structural, scope=all | Dimension filter only |
| A-C03 | `/pe-meta-create-update path.md` | dim=full, scope=all | Defaults |
| A-C04 | `/pe-meta-context-create-update path.md --scope instructions` | dim=full, scope=instructions | Scope filter |

### Guidance-first family (adherence)

| # | Invocation | Parsed options | Notes |
|---|---|---|---|
| A-G01 | `/pe-meta-adherence path.md` | deps=none, scope=all | Defaults (no --dim) |
| A-G02 | `/pe-meta-adherence path.md --deps full` | deps=full, scope=all | Full dep-chain adherence |
| A-G03 | `/pe-meta-adherence path.md --scope context --deps direct` | deps=direct, scope=context | Focus on context adherence |
| A-G04 | `/pe-meta-adherence path.md --mode plan` | mode=plan, deps=none, scope=all | `--mode plan` previews remediations and emits a `plan-file=<path>` marker (guidance-first — vision v15.2 § Option applicability matrix marks `--mode` ✅) |
| A-G05 | `/pe-meta-adherence path.md --mode apply` | mode=apply, deps=none, scope=all | `--mode apply` applies remediations in place; `apply` is the default when `--mode` is omitted |

### Scheduled-review family

| # | Invocation | Parsed options | Notes |
|---|---|---|---|
| A-S01 | `/pe-meta-scheduled-review` | dim=auto, deps=auto-rotation, scope=all | All auto (rotation selects) |
| A-S02 | `/pe-meta-scheduled-review --dim freshness` | dim=freshness, deps=auto-rotation, scope=all | Dim override, deps from rotation |
| A-S03 | `/pe-meta-scheduled-review --deps full` | dim=auto, deps=full, scope=all | Override rotation depth |
| A-S04 | `/pe-meta-scheduled-review --scope context --deps direct` | dim=auto, deps=direct, scope=context | Scope + depth override |
| A-S05 | `/pe-meta-scheduled-review --skip research` | dim=auto, deps=auto-rotation, scope=all, skip=[research] | Skip Phase 1 |
| A-S06 | `/pe-meta-scheduled-review --skip external` | dim=auto, deps=auto-rotation, scope=all, skip=[external] | No internet fetching |

### Orchestrator-pipeline parsing (review apply-mode)

> Formerly the **Update family**. Vision v15.9.0 consolidated Update into Review — these full-pipeline orchestrator tests now exercise `/pe-meta-review`. `/pe-meta-update` remains accepted as a deprecated alias (see § Alias routing behavior).

| # | Invocation | Parsed options | Notes |
|---|---|---|---|
| A-U01 | `/pe-meta-review` | mode=apply, dim=full, scope=all, skip=[], caller=manual, breadth=full | All defaults |
| A-U02 | `/pe-meta-review --mode plan --skip research` | mode=plan, dim=full, scope=all, skip=[research] | Canonical assessment-only invocation (former `healthcheck` preset). NOTE: Rejected if caller=manual (CF-05 via R-S01) because `--skip research` is incompatible with derived `breadth=full`. Valid only when paired with explicit `--start`/`--end` or trigger-fired context. |
| A-U03 | `/pe-meta-review --mode apply --dim efficiency --skip research,structure,consistency` | mode=apply, dim=efficiency, scope=all, skip=[research,structure,consistency] | Canonical optimization invocation (former `performancecheck` preset); delegates apply to `@meta-optimizer`. Same rule #2 caveat as A-U02. |
| A-U04 | `/pe-meta-review --mode plan` | mode=plan, dim=full, scope=all, skip=[] | Plan-only with full pipeline |
| A-U05 | `/pe-meta-review --scope context --dim freshness` | mode=apply, dim=freshness, scope=context, skip=[] | Scope + dim filter |
| A-U06 | `/pe-meta-review --mode apply --skip research,structure` | mode=apply, dim=full, scope=all, skip=[research,structure] | Multi-stage skip — caller MUST be trigger-fired or include `--start`/`--end` |
| A-U07 | `/pe-meta-review --mode plan --skip research --scope prompts --dim quality` | mode=plan, dim=quality, scope=prompts, skip=[research] | Plan + scope + dim composition — same rule #2 caveat |
| A-U08 | `/pe-meta-review --incremental --scope agents` _(trigger-fired caller)_ | mode=apply, dim=full, scope=agents, skip=[], caller=trigger-fired, breadth=incremental | `--incremental` is the single preserved v13.x alias and is VALID **only** on trigger-fired callers. Manual `--incremental` → R-A01. |
| A-U09 | `/pe-meta-review --source vscode-release-notes` | mode=apply, dim=full, scope=all, source=[vscode-release-notes], skip=[] | Single source filter passed through to researcher |
| A-U10 | `/pe-meta-review --source vscode-release-notes,anthropic-model-cards` | mode=apply, dim=full, scope=all, source=[vscode-release-notes,anthropic-model-cards], skip=[] | Multi-source filter |
| A-U11 | `/pe-meta-review --start 2026-04-01 --end 2026-05-01` | mode=apply, dim=full, scope=all, start=2026-04-01, end=2026-05-01, breadth=bounded-delta | Both endpoints explicit → bounded-delta |
| A-U12 | `/pe-meta-review --start 2026-04-01` | mode=apply, dim=full, scope=all, start=2026-04-01, end=resolved_from_lookback.default_days, breadth=bounded-delta, defaulted=[end] | Missing `--end` defaults from `lookback.default_days` |
| A-U13 | `/pe-meta-review --end 2026-05-01` | mode=apply, dim=full, scope=all, start=resolved_from_lookback.default_days, end=2026-05-01, breadth=bounded-delta, defaulted=[start] | Missing `--start` defaults from `lookback.default_days` |
| A-U14 | `/pe-meta-review --start 1.099 --source vscode-release-notes` | mode=apply, dim=full, scope=all, source=[vscode-release-notes], start=`<resolved-timestamp-of-1.099>`, breadth=bounded-delta, start_shape=version | Version-shaped bound, singleton source → Phase 0a resolves `1.099` to its publish timestamp via the source's `version_scheme` (semver). Resolves to the SAME window as the equivalent date bound. |
| A-U15 | `/pe-meta-review --start 1.099` | REJECT (CF-05) | Version-shaped bound with no `--source` → `version window requires a single --source` |
| A-U16 | `/pe-meta-review --start 1.099 --source vscode-release-notes,anthropic-model-cards` | REJECT (CF-05) | Version-shaped bound with non-singleton `--source` → `version window requires a single --source` |
| A-U17 | `/pe-meta-review --start 2025-06-18 --source mcp-spec` _(version token against a `version_scheme: none` source — hypothetical)_ | REJECT (CF-05) | A version-shaped bound against a `version_scheme: none` source → `source <id> has no version scheme; use a date window` |

### Derived breadth acceptance

| # | Caller-type | Window endpoints | Expected derived `breadth` | Researcher output shape |
|---|---|---|---|---|
| A-D01 | manual | none | full | snapshot |
| A-D02 | trigger-fired | none | incremental | digest |
| A-D03 | manual | `--start 2026-04-01 --end 2026-05-01` | bounded-delta | window-digest |
| A-D04 | trigger-fired | `--start 2026-04-01` | bounded-delta | window-digest (`end` defaulted) |
| A-D05 | manual | `--start 1.099 --source vscode-release-notes` (version shape) | bounded-delta | window-digest (`1.099` resolved to timestamp via `version_scheme`) |

### Phase 0a conversational pre-parser

| # | Free-form intent | Resolved canonical invocation |
|---|---|---|
| A-P0a-01 | "Refresh the prompt-engineering context with the latest changes from VS Code release notes since April." | `/pe-meta-review --scope context --source vscode-release-notes --start 2026-04-01` |
| A-P0a-02 | "Run a full sweep against all agents." | `/pe-meta-review --scope agents` (derives `breadth=full` because caller=manual + no window) |

### Per-artifact matrix routing

| # | Invocation | Expected routing |
|---|---|---|
| A-M01 | `/pe-meta-review --scope context --dim freshness` (review dim-family) | Per matrix: routes to `/pe-meta-context-review` |
| A-M02 | `/pe-meta-review --scope agents --dim structural` (create-update dim-family) | Per matrix: routes to `/pe-meta-agents-create-update` |
| A-M03 | `/pe-meta-review --scope prompts --dim strategic` (design dim-family) | Per matrix: routes to `/pe-meta-prompts-design` |

### First-line `Resolved invocation:` log

| # | Invocation | Expected first line of Phase 8 report |
|---|---|---|
| A-L01 | `/pe-meta-review --scope context --source vscode-release-notes --start 2026-04-01` | `Resolved invocation: --mode=apply --scope=context --source=vscode-release-notes --dim=full --start=2026-04-01 --end=<resolved> --deps=none --skip= \| breadth=bounded-delta \| caller=manual` |

### External-platform reconciliation (review with an external `--source <url>`)

> Replaces the retired Release-monitor family (vision v15.2 removed the Release-diff/Release-monitor command family; external-platform reconciliation is now a review invocation scoped to an external `--source <url>`).

| # | Invocation | Parsed options | Notes |
|---|---|---|---|
| A-RM01 | `/pe-meta-review --source <url>` | mode=apply, dim=full, scope=all, source=[<url>] | Ad-hoc URL source; defaults otherwise |
| A-RM02 | `/pe-meta-review --source <url> --mode plan` | mode=plan, dim=full, scope=all, source=[<url>] | Preview only |
| A-RM03 | `/pe-meta-review --source <url> --scope prompts --dim freshness` | mode=apply, dim=freshness, scope=prompts, source=[<url>] | Filter scope/dims |
| A-RM04 | `/pe-meta-review --source <url> --skip external` | mode=apply, dim=full, scope=all, source=[<url>], skip=[external] | No additional URL fetching beyond the named source |

## Rejected combinations with corrective messages

| # | Invalid invocation | Rejection | Corrective action |
|---|---|---|---|
| R-02 | `/pe-meta-design desc --deps full` | `--deps is not supported by pe-meta-design because design does not traverse dependencies.` | Remove --deps or use /pe-meta-review for dependency analysis. |
| R-03 | `/pe-meta-adherence path.md --dim full` | `--dim is not supported by pe-meta-adherence because adherence uses a fixed dimension set.` | Remove --dim. |
| R-04 | `/pe-meta-review path.md --skip structure` | `--skip structure is not supported in type-specific review because phase-level skipping requires the orchestration pipeline.` | Use --skip research or --skip external, or use /pe-meta-review for phase control. |
| R-05 | `/pe-meta-review path.md --skip consistency` | `--skip consistency is not supported in type-specific review because phase-level skipping requires the orchestration pipeline.` | Use --skip research or --skip external, or use /pe-meta-review for phase control. |
| R-06 | `/pe-meta-design desc --skip research` | `--skip is not supported by pe-meta-design because design has no pipeline phases.` | Remove --skip. |
| R-08 | `/pe-meta-adherence path.md --skip external` | `--skip is not supported by pe-meta-adherence because adherence has no pipeline phases.` | Remove --skip. |
| R-09 | `/pe-meta-scheduled-review --mode apply` | `--mode is not supported by pe-meta-scheduled-review because scheduled-review delegates execution to sub-commands.` | Remove --mode. For apply control, configure the scheduled-review delegation target. |
| R-10 | `/pe-meta-scheduled-review --skip structure` | `--skip structure is not supported in scheduled-review because phase-level skipping requires the update pipeline.` | Use --skip research or --skip external, or invoke /pe-meta-review directly for phase control. |
| R-11 | `/pe-meta-create-update path.md --deps direct` | `--deps is not supported by pe-meta-create-update because create-update does not traverse dependencies.` | Remove --deps. |
| R-12 | `/pe-meta-create-update path.md --mode plan` | `--mode is not supported by pe-meta-create-update because create-update always writes.` | Remove --mode. |

## Alias routing behavior

| # | Input (legacy syntax) | Routed to (canonical) | Verification |
|---|---|---|---|
| AL-01 | `/pe-meta-review path.md --with-deps` | `--deps full` | Full bounded traversal (depth 5) |
| AL-02 | `/pe-meta-review path.md --with-deps-shallow` | `--deps direct` | First-level only (depth 1) |
| AL-03 | `/pe-meta-review --plan` | `--mode plan` | Preview without apply |
| AL-04 | `/pe-meta-review --no-external` | `--skip external` | No URL fetching |
| AL-05 | `/pe-meta-review --no-research` | `--skip research` | Skip Phase 1 |
| AL-06 | `/pe-meta-review --skip-source` | `--skip research` | Skip Phase 1 |
| AL-07 | `/pe-meta-review --skip-structure` | `--skip structure` | Skip Phase 2 |
| AL-08 | `/pe-meta-review --skip-consistency` | `--skip consistency` | Skip Phase 3 |
| AL-09 | `/pe-meta-review --skip-content` | `--skip content` | Skip Phase 4 |
| AL-10 | `/pe-meta-update <args>` | `/pe-meta-review <args>` | Deprecated command alias: `pe-meta-update.prompt.md` is a redirect stub; the orchestrator engine lives in `pe-meta-review.prompt.md` (vision v15.9.0 Update→Review consolidation). Existing `/pe-meta-update` invocations remain valid and resolve to `/pe-meta-review`. |

## Scope + deps composition behavior

### Orchestration context (review, scheduled-review)

| # | Invocation | Scope effect | Deps effect | Combined behavior |
|---|---|---|---|---|
| SC-01 | `/pe-meta-review --scope context` | Iterate context files only | N/A (orchestrator pipeline doesn't use deps) | Batch processes only context artifacts |
| SC-02 | `/pe-meta-scheduled-review --scope agents --deps full` | Iterate agent files only | Full chain per agent | Reviews each agent with full dependency traversal |
| SC-03 | `/pe-meta-review --scope prompts,skills` | Iterate prompts and skills | N/A | Batch processes prompts + skills only |

### Type-specific context (review, adherence)

| # | Invocation | Scope effect | Deps effect | Combined behavior |
|---|---|---|---|---|
| SC-04 | `/pe-meta-prompt-review p.md --scope context --deps full` | Filter deps to context type | Full depth (5) | Follow full chain, examine only context file dependencies |
| SC-05 | `/pe-meta-agent-review a.md --scope instructions --deps direct` | Filter deps to instructions | First-level only | Check only direct instruction dependencies |
| SC-06 | `/pe-meta-context-review c.md --scope prompts --deps direct` | Filter deps to prompts | First-level | Find prompts that directly consume this context |
| SC-07 | `/pe-meta-review path.md --deps full` (no scope) | All dep types | Full depth | Traverse all dependency types (default behavior) |

## Skip stage validation

### Valid stages per command context

| # | Context | Valid skip stages | Invalid skip stages |
|---|---|---|---|
| SV-01 | Orchestrators (review) | research, external, structure, consistency, content | Any other value |
| SV-02 | Type-specific review | research, external | structure, consistency, content |
| SV-03 | Scheduled-review | research, external | structure, consistency, content |
| SV-04 | Creation (design, create-update) | _(none — --skip not supported)_ | All values |
| SV-05 | Adherence | _(none — --skip not supported)_ | All values |

### Invalid stage name rejection

| # | Invalid invocation | Rejection |
|---|---|---|
| SV-07 | `/pe-meta-review --skip validation` | `Unknown skip stage 'validation'. Valid stages: research, external, structure, consistency, content.` |
| SV-08 | `/pe-meta-review --skip apply` | `Unknown skip stage 'apply'. Valid stages: research, external, structure, consistency, content.` |
| SV-09 | `/pe-meta-review --skip ` (empty) | `--skip requires at least one stage name. Valid stages: research, external, structure, consistency, content.` |

## Conflicting option detection

| # | Invalid invocation | Rejection |
|---|---|---|
| CF-01 | `/pe-meta-review path.md --deps full --deps direct` | `Conflicting --deps values. Specify one: --deps none, --deps direct, --deps full, or --deps <N>.` |
| CF-02 | `/pe-meta-review --mode plan --mode apply` | `Conflicting --mode values. Specify one: --mode plan or --mode apply.` |
| CF-03 | `/pe-meta-review healthcheck` | `"healthcheck" is no longer a supported preset. Use the canonical options per the v13 taxonomy. See pe-meta-review.prompt.md § Invocation options for the mapping.` |
| CF-04 | `/pe-meta-review performancecheck --dim structural` | `"performancecheck" is no longer a supported preset. Use the canonical options per the v13 taxonomy. See pe-meta-review.prompt.md § Invocation options for the mapping.` |
| CF-05 | `/pe-meta-review path.md --dim invented-name` | `Unknown --dim value 'invented-name'. Valid groups: full, structural, quality, strategic, freshness, efficiency, adherence, context-full, context-health, model, optimize, reliability. Or use a specific dimension ID (`D1-metadata` through `D35-portability-boundary`).` |
| CF-07 | `/pe-meta-review fullcheck` | `"fullcheck" is no longer a supported preset. Use the canonical options per the v13 taxonomy. See pe-meta-review.prompt.md § Invocation options for the mapping.` |

## Vision v14 composition + alias gates

| # | Invalid invocation | Rejection |
|---|---|---|
| R-S01 | `/pe-meta-review --skip research` _(manual caller, no `--start`/`--end`)_ | `--skip research is incompatible with derived breadth=full because a full sweep that skips research has no inputs. Either remove --skip research, OR add explicit --start/--end to derive bounded-delta, OR invoke from a trigger-fired context to derive incremental.` |
| R-A01 | `/pe-meta-review --incremental` _(manual caller)_ | `--incremental retired for manual callers in v14; manual callers always derive breadth=full. Use --start/--end for bounded-delta, or trigger-fired context for incremental — see vision v15 changelog § Historical: v13 → v14 deprecated flag map.` |

## Retired v14 flags — uniform CF-05 rejection template

Every retired flag rejection MUST follow the template:

> `<flag> retired in v14; use <v14-replacement> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map`

| # | Retired flag | v14 replacement | Example invocation rejected |
|---|---|---|---|
| RV-01 | `--breadth full\|incremental\|catch-up` | derived from caller-type + `--start`/`--end` | `/pe-meta-review --breadth catch-up` → `--breadth retired in v14; use derived breadth (caller-type + --start/--end) — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-02 | `--since <date>` | `--start <YYYY-MM-DD>` | `/pe-meta-review --since 2026-04-01` → `--since retired in v14; use --start <YYYY-MM-DD> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-03 | `--between <a..b>` | `--start <a> --end <b>` | `/pe-meta-review --between 2026-04-01..2026-05-01` → `--between retired in v14; use --start <a> --end <b> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-04 | `--area <area>` | `--scope <type-token>` or `--scope <comma-paths>` | `/pe-meta-review --area context` → `--area retired in v14; use --scope <type-token> or --scope <comma-paths> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-05 | `--artifact <type>` | `--scope <type-token>` | `/pe-meta-review --artifact agents` → `--artifact retired in v14; use --scope <type-token> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-06 | `--consumer <path>` | `--scope <comma-paths>` | `/pe-meta-review --consumer .github/agents/foo.agent.md` → `--consumer retired in v14; use --scope <comma-paths> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-07 | `--subject <topic>` | `--scope <comma-paths>` (specific file path) | `/pe-meta-review --subject foo` → `--subject retired in v14; use --scope <comma-paths> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-08 | `--concern <category>` | `--dim <group>` | `/pe-meta-review --concern quality` → `--concern retired in v14; use --dim <group> — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-09 | `--mode-review` | `--mode plan` or `--mode apply` (default for `*-review` prompts) | `/pe-meta-review --mode-review` → `--mode-review retired in v14; use --mode plan or --mode apply — see vision v15 changelog § Historical: v13 → v14 deprecated flag map` |
| RV-10 | `--incremental` (manual caller) | — _(manual callers always derive `full`)_ | See R-A01 above |

## Phase 0b — Domain coherence check (vision v15 § Domain-coherent batching)

Tests P0b-01..P0b-14 pin the deterministic behavior of Phase 0b: scope-resolution → metadata-first 3-tier domain resolution → seed-vs-deps decision matrix → `bundle=…` dispatch.

**Setup assumptions for the tests below:**

- `.copilot/context/00.00-prompt-engineering/01.01-foo.md` declares `domain: prompt-engineering`
- `.copilot/context/01.00-article-writing/02-style-guide.md` declares `domain: article-writing`
- `.copilot/context/90.00-learning-hub/03-folder-conventions.md` declares `domain: learning-hub`
- `.github/prompts/00.09-pe-meta/pe-meta-review.prompt.md` declares `domain: prompt-engineering` and depends (via `dependency-tracking`) on files in all three domains above
- `.copilot/context/00.00-prompt-engineering/orphan-no-domain.md` has NO declared `domain:`
- `pe-domain-map.yaml` exists at repo root and contains an entry mapping `.copilot/context/00.00-prompt-engineering/**` → `prompt-engineering`
- `.copilot/context/04.00-misc/uncategorized.md` has NO declared `domain:` AND does not match any `pe-domain-map.yaml` entry

| # | Invocation | Seed footprint | Additional dep-domains | Disposition (`bundle=…`) | Phase 1 runs? | Notes |
|---|---|---|---|---|---|---|
| P0b-01 | `/pe-meta-context-review .copilot/context/00.00-prompt-engineering/01.01-foo.md` | `{prompt-engineering}` (1) | 0 | `single-domain` | Yes | Single seed file, no `--deps`, no cross-domain risk. |
| P0b-02 | `/pe-meta-review --mode apply --scope .github/prompts/00.09-pe-meta/pe-meta-review.prompt.md --deps full` | `{prompt-engineering}` (1) | 2 (`article-writing`, `learning-hub`) | `cross-domain-deps` | Yes (one run; per-dep-domain lenses applied in Phase 2–4) | Single seed + cross-domain deps: NOT split — consumer artifact needs all declared deps present to be evaluated. |
| P0b-03 | `/pe-meta-review --mode apply --scope context` | `{prompt-engineering, article-writing, learning-hub}` (3) | n/a | `multi-domain-gated` | No (BLOCK on numbered split or `bundle=accept`) | Multi-seed `--mode apply` without consent → blocked. |
| P0b-04 | `/pe-meta-review --mode apply --scope context bundle=accept` | `{prompt-engineering, article-writing, learning-hub}` (3) | n/a | `accepted-bundle` | Yes | Multi-seed `--mode apply` WITH explicit consent → proceeds; consent recorded on first-line log. |
| P0b-05 | `/pe-meta-review --mode plan --scope context` | `{prompt-engineering, article-writing, learning-hub}` (3) | n/a | `multi-domain-advisory` | Yes | `--mode plan` produces split-proposal in Phase 8 report; never blocks. |
| P0b-06 | `/pe-meta-review --mode apply --scope context bundle=skip` | n/a | n/a | _rejected_ | n/a | CF-05: `bundle=skip is not a valid consent token; the closed set is {accept}. Use bundle=accept to bypass the multi-domain gate.` |
| P0b-07 | `/pe-meta-review --mode apply --scope context bundle=yes` | n/a | n/a | _rejected_ | n/a | CF-05: same closed-set rejection — only `accept` is valid. |
| P0b-08 | `/pe-meta-review --skip domain-coherence` | n/a | n/a | _rejected_ | n/a | CF-05: `--skip domain-coherence is rejected; Phase 0b is not skippable per vision v15 § Domain-coherent batching. To bypass the gate on a multi-domain scope, append bundle=accept to the invocation.` |
| P0b-09 | `/pe-meta-context-review .copilot/context/00.00-prompt-engineering/orphan-no-domain.md` | `{prompt-engineering}` (1, via Tier 2 path-heuristic match in `pe-domain-map.yaml`) | 0 | `single-domain` | Yes | Tier 2 fires for files without declared `domain:`; per-file `domain-source=path-heuristic` flagged in Phase 8 report with metadata-backfill suggestion. |
| P0b-10 | `/pe-meta-context-review .copilot/context/04.00-misc/uncategorized.md` | `{unknown}` (1, Tier 3 fallback) | 0 | `single-domain` | Yes | No declared `domain:`, no `pe-domain-map.yaml` match → reserved `unknown` domain; per-file `domain-source=unknown` flagged in Phase 8 report. |
| P0b-11 | `/pe-meta-context-review path-with-declared-domain.md` (where `pe-domain-map.yaml` heuristic would suggest a DIFFERENT domain) | `{declared-domain}` (Tier 1 wins) | 0 | `single-domain` | Yes | Tier 1 declared metadata ALWAYS overrides Tier 2 heuristic; Phase 8 report does NOT flag this (declared metadata is authoritative). |
| P0b-12 | `/pe-meta-review --mode apply --scope .copilot/context/00.00-prompt-engineering/01.01-foo.md,.copilot/context/01.00-article-writing/02-style-guide.md` | `{prompt-engineering, article-writing}` (2) | n/a | `multi-domain-gated` | No (BLOCK) | Explicit 2-path scope with 2 distinct seed domains → gated under `--mode apply`. |
| P0b-13 | `/pe-meta-review --mode apply --scope context --dim adherence bundle=accept` | `{prompt-engineering, article-writing, learning-hub}` (3) | n/a | `accepted-bundle` | Yes | **`--dim` is NEVER a domain override.** Domain footprint computed from per-file `domain:` metadata regardless of `--dim adherence`. `bundle=accept` records consent. |
| P0b-14 | `/pe-meta-review --mode apply --scope context bundle=ACCEPT` | n/a | n/a | _rejected_ | n/a | CF-05: `bundle=ACCEPT is not a valid consent token; the closed set is {accept} (case-sensitive). Use bundle=accept to bypass the multi-domain gate.` Confirms consent-token comparison is case-sensitive. |

**First-line `Resolved invocation:` log shape for accepted Phase 0b dispositions.**

```text
Resolved invocation: --mode=apply --scope=context --source=- --dim=full --start=- --end=- --deps=none --skip=- | breadth=full | caller=manual | bundle=accepted-bundle
```

The `bundle=…` field MUST be present on every accepted invocation. Allowed values: `single-domain | cross-domain-deps | multi-domain-gated | accepted-bundle | multi-domain-advisory`. Tests P0b-06, P0b-07, P0b-08, P0b-14 confirm rejections never reach the `Resolved invocation:` log line.

<!--
prompt_metadata:
  version: "1.0.0"
  last_updated: "2026-06-12"
-->
