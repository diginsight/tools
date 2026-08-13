---
name: pe-meta-adherence
description: "Generate a guidance-first adherence matrix — for a given guidance file, check which consumers implement which rules"
agent: agent
model: claude-opus-4.6
tools:
  - semantic_search
  - read_file
  - file_search
  - grep_search
  - list_dir
  - replace_string_in_file
  - create_file
argument-hint: '<guidance-file-path> [--mode plan|apply] [bundle=accept] — e.g., ".copilot/context/00.00-prompt-engineering/01.07-critical-rules-priority-matrix.md"'
goal: "Produce an adherence matrix showing which consumers implement which rules from the target guidance file, with gap severity and recommendations"
scope:
  covers:
    - "Guidance-first review mode implementation"
    - "Consumer discovery for target guidance file"
    - "Rule extraction from guidance"
    - "Adherence verification per consumer per rule"
    - "Adherence matrix with gap severity classification"
    - "Auto-sampling N consumers per guidance file when invoked by orchestrator with derived breadth=full (vision v14 R8)"
    - "Phase 0b — domain coherence (per 04.05-pe-meta-invocation-gates.md)"
    - "bundle=accept consent token recognition on multi-domain consumer scopes"
    - "Audited-prompt rubric extension R-Pa/R-Pb — verify Phase 0a CF-05 capability + Phase 0b bundle=… marker in audited prompts' `Resolved invocation:` logs"
  excludes:
    - "Individual artifact review (use /pe-meta-{type}-review)"
    - "Domain artifacts (use /pe-con-review)"
boundaries:
  - "Default mode: apply — implements non-breaking improvements autonomously; proposes breaking changes for human confirmation. Use `--mode plan` to opt into assess-only output (findings report without changes)"
  - "Risk classification determines execution, not command identity — low-risk findings are applied without gating"
  - "Write tools (`replace_string_in_file`, `create_file`) are active by default (`--mode apply`). Suppressed ONLY when `--mode plan` is explicitly specified OR when the finding is classified as breaking"
  - "MUST verify target is PE guidance before proceeding"
  - "MUST discover ALL consumers, not just obvious ones"
  - "MUST classify gaps by severity"
  - "MUST produce machine-readable adherence matrix"
  - "MUST enforce option applicability from pe-meta-option-applicability-matrix.md"
  - "MUST be the canonical command for guidance-first behavior"
  - "MUST honor orchestrator pass-through of derived `breadth=full` by auto-sampling N consumers per guidance file (N from `sampling.adherence_consumers_per_file` in `.copilot/config/pe-self-update.config.json`), stratified by artifact-type and layer; MUST NOT auto-sample on `breadth=incremental` or `breadth=bounded-delta`"
  - "Phase 0b is NOT skippable; --skip domain-coherence is rejected with CF-05; Phase 0b runs on the resolved guidance + sampled-consumer set BEFORE adherence verification"
  - "bundle=accept is the ONLY valid consent token (closed set); recorded on first-line `Resolved invocation:` log"
  - "Phase 0a CF-05 artifact-type/path consistency does NOT apply at this orchestrator-level layer (guidance file is artifact-type-agnostic by design); CF-05 is enforced ONLY by per-artifact prompts"
  - "Rubric R-Pa: audited prompts MUST display Phase 0a CF-05 rejection capability (when applicable) — part of Phase 3 adherence verification when checking consumers of vision-v15 capability"
  - "Rubric R-Pb: audited prompts MUST emit `bundle=…` marker on first-line `Resolved invocation:` log — part of Phase 3 adherence verification when checking consumers of vision-v15 capability"
handoffs:
  - {label: "Apply complex improvements", agent: pe-con-builder, send: true}
rationales:
  - "Guidance-first review catches rules that no consumer implements (dead guidance)"
  - "Adherence matrices enable systematic gap tracking across the PE system"
  - "Severity classification prioritizes remediation effort"
  - "This prompt is the canonical owner of guidance-first adherence capability"
  - "Stratified sampling (R8) on full sweeps gives statistically meaningful coverage without scanning every consumer, keeping full-mode runtime bounded"
---

# Guidance-First Adherence Matrix

For a given guidance file, extract all rules, discover all consumers, and check adherence per consumer per rule. Produces a gap-severity matrix.

> **v15.4 alignment.** This prompt honors the vision v15.4 **`apply = plan + execute`** contract: every `--mode apply` run first materializes/reconciles a plan, then executes it; `--mode plan` materializes the same plan and stops (see § Plan output contract and [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md)). The eighth canonical parameter **`--plan-file`** sets plan location/identity only; the **fresh / reconcile / trust** execution modes follow from (baseline-available? × research-runs?), and the § Iteration budget checkpoint (see [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md)) emits a plan with a `trust`-mode resume when a run hits the per-cycle change cap. The first-line `Resolved invocation:` log echoes `plan-file=<path-or-none>` and `spillover=<path-or-none>` markers.

## Option applicability contract

Load `.github/prompts/00.09-pe-meta/pe-meta-option-applicability-matrix.md` before parsing options.

1. This command accepts only a guidance target path as primary input.
2. If unsupported options are provided, reject using deterministic corrective guidance from the matrix.
3. Guidance-first requests MUST resolve to this prompt's behavior.

**When to use**: When you want to verify that a guidance file's rules are actually implemented by its consumers. Complements individual artifact review (which checks one consumer against all guidance) with the inverse perspective (one guidance against all consumers).

## Phase 0b — Domain coherence

This prompt enforces the Phase 0b domain coherence gate defined in [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) (upstream authority: current vision document § Domain-coherent batching). The 3-tier metadata-first domain resolution algorithm, seed-vs-deps decision matrix, dispatch table, `bundle=…` closed set, and `bundle=accept` consent semantics are specified there and MUST NOT be re-implemented here.

**Locally true for this prompt:**

1. **Scope = guidance + discovered/sampled consumers.** The `<guidance-file-path>` plus the consumer set produced by Phase 2 (and the sampled subset from Phase 2a, when `breadth=full`) form the in-scope file set. Each in-scope file's declared `domain:` frontmatter is read first (Tier 1); `pe-domain-map.yaml` (Tier 2) and `unknown` (Tier 3) follow.
2. **Gate runs BEFORE Phase 3 (Verify Adherence).** Domain footprint is computed after consumer discovery/sampling, before per-cell verification. Cross-domain consumer sets in `--mode apply` block until `bundle=accept` consent or per-domain split is selected; `--mode plan` is advisory.
3. **Consent token.** `bundle=accept` is the only valid token (closed set, case-sensitive). Recorded on the first-line `Resolved invocation:` log as `bundle=accepted-bundle`.
4. **Phase 0a CF-05 does NOT apply at this layer.** Guidance files are artifact-type-agnostic; CF-05 (artifact-type/path consistency) is enforced ONLY by per-artifact prompts.
5. **`--skip domain-coherence` rejected.** Phase 0b is not skippable; bypass multi-domain gating only via `bundle=accept`.

## Process

### Phase 1: Read and Extract Rules

1. **Read target guidance file** completely
2. **Extract rules** — identify all prescriptive statements:
   - `**Rule**:` blocks (N-1 labeled)
   - `MUST` / `NEVER` / `ALWAYS` statements
   - Table rows with requirement columns
   - Boundary items (Always Do / Never Do)
3. **Number rules** sequentially (R1, R2, R3, ...)
4. **Report**: "[N] rules extracted from [file]"

### Phase 2: Discover Consumers

Find all artifacts that reference or should reference this guidance file:

1. **Direct references** — grep for file name, category name, or `📖` references
2. **Category consumers** — check STRUCTURE-README for which artifacts list this file's category in `context_dependencies`
3. **Implicit consumers** — artifacts whose scope overlaps with the guidance topic (discovered via `semantic_search`)

**Report**: "[N] consumers discovered: [list with discovery method]"

### Phase 2a: Sampling (when invoked by orchestrator with derived breadth=full)

When the caller is `pe-meta-review` AND passes derived `breadth=full`, auto-sample consumers instead of verifying every one. Otherwise (manual invocation, `breadth=incremental`, or `breadth=bounded-delta`), verify ALL discovered consumers.

1. **Load sampling size**: read `sampling.adherence_consumers_per_file` from `.copilot/config/pe-self-update.config.json` (call this `N`).
2. **Stratify consumers** by two axes:
   - **Artifact-type**: `context`, `instructions`, `agents`, `prompts`, `skills`, `templates`, `hooks`, `snippets`
   - **Layer**: `meta` (under `.github/` or `.copilot/context/00.00-prompt-engineering/`) vs `domain` (everything else)
3. **Allocate N across strata** proportionally to stratum population, with a floor of 1 per non-empty stratum.
4. **Sample within each stratum** using a deterministic order (alphabetical by file path) so the same `breadth=full` run on unchanged inputs picks the same consumers.
5. **Report**: "Sampled M of [N-total] consumers (orchestrator breadth=full): [strata breakdown]". List the actual sampled paths so downstream phases are auditable.

The remaining (unsampled) consumers are recorded as `not_inspected_in_this_run` with their strata so a follow-up run can target them.

### Phase 3: Verify Adherence

For each consumer × each rule:

| Status | Meaning |
|---|---|
| ✅ FULL | Rule fully implemented |
| ⚠️ PARTIAL | Rule partially implemented or paraphrased with lost nuance |
| ❌ MISSING | Rule not implemented |
| ➖ N/A | Rule not applicable to this consumer type |

#### Rubric extensions for vision-v15 capability audits

When the target guidance is a vision-v15 capability source (e.g., `pe-meta-review.prompt.md` Phase 0a/0b sections, `pe-meta-option-applicability-matrix.md`, `pe-meta-option-parser-tests.md`), apply these additional rules per audited prompt consumer:

| Rule | Check | Pass criteria |
|---|---|---|
| **R-Pa** | Phase 0a CF-05 capability | Audited per-artifact prompt MUST display CF-05 artifact-type/path rejection in its embedded test scenarios or option-parser examples; orchestrator-level prompts MUST explicitly state CF-05 does NOT apply at their layer |
| **R-Pb** | Phase 0b `bundle=…` log marker | Audited prompt's documented `Resolved invocation:` log shape MUST include `bundle=` field with a value from the closed set `{single-domain, cross-domain-deps, multi-domain-gated, accepted-bundle, multi-domain-advisory}` |

When the target guidance is NOT vision-v15 capability content, R-Pa/R-Pb are marked `➖ N/A` for all consumers and do not affect coverage scoring.

### Phase 4: Generate Adherence Matrix

**📖 Output format**: `.github/templates/00.00-prompt-engineering/output-adherence-matrix.template.md`

```markdown
## Adherence Matrix: [guidance file]

| Rule | Description | Consumer 1 | Consumer 2 | ... | Coverage |
|---|---|---|---|---|---|
| R1 | [brief] | ✅ | ❌ | ... | X/Y |
| R2 | [brief] | ⚠️ | ✅ | ... | X/Y |
```

### Phase 5: Gap Analysis

Classify gaps by severity:

| Severity | Criteria |
|---|---|
| **CRITICAL** | Core rule (top 5 priority) missing from 50%+ consumers |
| **HIGH** | Any rule missing from a Tier 1 consumer (6+ dependents) |
| **MEDIUM** | Rule partially implemented or missing from 1-2 consumers |
| **LOW** | Minor nuance lost in paraphrase |

**Report**: Severity-ranked gap list with recommended remediation (which consumer needs what change).

### Phase 5a: Output contract (plan-file + spillover markers)

The report MUST open with a first-line `Resolved invocation:` log that echoes the `plan-file=` and `spillover=` markers:

```text
Resolved invocation: --mode=<plan|apply> … | bundle=<accepted-bundle|none> | plan-file=<path-or-none> | spillover=<path-or-none>
```

- **`--mode plan` (assessment-only):** emit an actionable plan file at the canonical plan-mode path per [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md) and record `plan-file=<path>`. When `--mode apply`, record `plan-file=none`.
- **`--mode apply` overflow:** if the per-cycle change cap is hit with validated remediations remaining, emit a spillover plan at `<run-folder>/<NN>-<kebab-name>-spillover.plan.md` per [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md) and record `spillover=<path>`; otherwise record `spillover=none`.

## Response Management

- **Not a guidance file** → "This file doesn't contain prescriptive rules. Use `/pe-meta-review` for individual artifact review."
- **Zero consumers found** → "No consumers reference this guidance. Either the guidance is orphaned or consumers use it implicitly without declaration."
- **100% adherence** → "Full adherence — all [N] rules implemented across [M] consumers."

## Embedded Test Scenarios

| # | Scenario | Expected |
|---|---|---|
| 1 | Critical rules file with many consumers | Extract rules → discover 10+ consumers → matrix with coverage % → severity-ranked gaps |
| 2 | Orphaned guidance (no consumers) | Discover 0 consumers → report orphaned status |
| 3 | Full adherence | All cells ✅ → report full adherence, no gaps |
| 4 | Non-guidance file | Redirect to /pe-meta-review → STOP |
| 5 | Orchestrator passes derived `breadth=full` (R8) | Phase 2a auto-samples N consumers (from `sampling.adherence_consumers_per_file`), stratified by artifact-type × layer; unsampled consumers listed as `not_inspected_in_this_run` |
| 6 | Orchestrator passes derived `breadth=incremental` | No auto-sampling; verify ALL discovered consumers |
| 7 | Manual invocation (no orchestrator) | No auto-sampling; verify ALL discovered consumers |

## Risk Classification

#file:.github/prompt-snippets/pe-meta-risk-classification.md

<!--
prompt_metadata:
  version: "2.2.0"
  last_updated: "2026-05-31"
-->
