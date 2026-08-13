---
name: pe-meta-prompt-review
description: "Review a PE-for-PE prompt across selected dimensions with argument-hint and phase checks"
agent: agent
model: claude-opus-4.6
tools: [semantic_search, read_file, file_search, grep_search, list_dir, replace_string_in_file, create_file]
handoffs:
  - {label: "Validate", agent: pe-meta-validator, send: true}
  - {label: "Apply complex improvements", agent: pe-con-builder, send: true}
argument-hint: '<file-path> [--mode plan|apply] [--dim <group|D#|full> (default: full)] [--deps none|direct|full|<N>] [--scope <type>] [--skip research|external]'
goal: "Ensure a PE-for-PE prompt meets the shared quality objective and scope intent (reliability, effectiveness, efficiency) with type-applicable requirements"
scope:
  covers: ["Shared quality objective and scope intent enforcement (applicability-scoped)", "Dimension-scoped review", "Argument-hint quality check", "Phase structure verification", "Handoff resolution"]
  excludes: ["Domain prompts"]
boundaries:
  - "MUST share the same quality objective and scope intent as /pe-meta-prompt-design (applicability-scoped)"
  - "Default mode: apply — implements non-breaking improvements autonomously; proposes breaking changes for human confirmation. Use `--mode plan` to opt into assess-only output (findings report without changes)"
  - "Risk classification determines execution, not command identity — low-risk findings are applied without gating"
  - "Write tools (`replace_string_in_file`, `create_file`) are active by default (`--mode apply`). Suppressed ONLY when `--mode plan` is explicitly specified OR when the finding is classified as breaking"
  - "MUST always check argument-hint and phase structure"
rationales:
  - "Type-specific PE-meta workflow improves reliability and maintainability"
  - "Explicit orchestration metadata supports deterministic validation and safer automation"
---

# Prompt Review

> **v15.4 alignment.** This prompt honors the vision v15.4 **`apply = plan + execute`** contract: every `--mode apply` run first materializes/reconciles a plan, then executes it; `--mode plan` materializes the same plan and stops (see § Plan output contract and [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md)). The eighth canonical parameter **`--plan-file`** sets plan location/identity only; the **fresh / reconcile / trust** execution modes follow from (baseline-available? × research-runs?), and the § Iteration budget checkpoint (see [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md)) emits a plan with a `trust`-mode resume when a run hits the per-cycle change cap. The first-line `Resolved invocation:` log echoes `plan-file=<path-or-none>` and `spillover=<path-or-none>` markers.

## Phase 0a CF-05 + Phase 0b — Invocation gates

This prompt enforces the **Phase 0a CF-05 artifact-type/path consistency check** AND the **Phase 0b domain coherence gate** defined in [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) (upstream authority: vision v15 § Domain detection, § Pipeline phases).

**Locally true for this prompt:**

- **CF-05 expected root.** Any resolved target path (positional `<file-path>`, `--scope`, or design output target) MUST resolve under `.github/prompts/`. Paths outside this root are REJECTED before Phase 0b runs; the rejection message MUST suggest the canonical replacement prompt name from the SoT § Per-prompt-class applicability matrix.
- **Phase 0b scope.** Resolved file set = the target path (+ closure under `--deps full` when this prompt's argument-hint exposes `--deps`); degenerate single-file scope is single-domain by construction.
- **Algorithm.** 3-tier metadata-first per the SoT: Tier 1 = each in-scope file's declared `domain:` frontmatter; Tier 2 = optional `pe-domain-map.yaml`; Tier 3 = `unknown`. The seed path does NOT constrain consumer domains when `--deps full` traverses the closure.
- **Gate timing.** Runs BEFORE delegating to handoffs declared in this prompt's frontmatter.
- **Consent tokens.** `bundle=accept` is recognized AND propagated when delegating to the orchestrator so it does not re-prompt; `--skip domain-coherence` is REJECTED with CF-05.
- **When delegated from an orchestrator.** Phase 0a CF-05 is verified by the dispatcher and Phase 0b has already run on the single-domain resolved scope — this section's gate is a no-op in that path.

## Process
1. Parse `--dim`, `--deps`, `--scope`, and `--skip` flags
2. Resolve the applicable-dimension SET from the `05.07-pe-meta-dimension-catalog.md` applicability matrix for the `prompt` type — this is the `<applicable>` denominator (the full type-applicable set, NOT the `05.08` enumerated subset); then load `05.08-pe-meta-type-checklists.md` → prompt section for the SUB-CHECKS of those applicable dimensions that declare rows
3. Check argument-hint includes concrete example
4. Check phases are numbered and logically ordered
5. Run selected dimensions via `@pe-meta-validator`, recording `dim_evidence[]` (one anchored `{dim, status, evidence_ref}` per applicable dimension — passes included) per the § Assess-phase evidence coverage contract
6. If `--deps full`: run full dependency-aware review (bounded recursion, default depth 2, scope-filtered) and verify handoff data contracts
7. If `--deps direct`: run first-level-only dependency checks (scope-filtered) and verify direct handoff data contracts
8. Run the independent Coverage Audit (`@pe-meta-validator`, Coverage Audit mode) and reconcile `pu-evidence`/`subcheck-coverage`/`shallow-sweep`; apply the evidence-depth hard-fails before any clean health score
9. Generate severity-ranked report


## Phase ordering and option behavior

1. Phase ordering: parse inputs first, execute the type-specific workflow second, then validate and report.
2. Default mode is `--mode apply` — assess and implement non-breaking improvements autonomously. Use `--mode plan` to opt into assessment-only output.
3. `--dim` selects which dimension groups run; default (omitted) = `full` — the full `05.07` type-applicable set. `--dim` is **subtractive**: it may NARROW the evaluated set but the default is never a silent subset. `--deps` controls dependency traversal: `none` (per-artifact only), `direct` (first-level deps), `full` (bounded recursive). Default: `none`.
4. `--scope` filters which dependency types to focus on during `--deps` traversal (e.g., `--scope context` focuses on context file dependencies only). When omitted, traverse all dependency types.
5. `--skip research|external` suppresses external source fetching during review.
6. Guidance-first behavior is handled through `/pe-meta-adherence`.
7. When `--mode apply` is active and findings require multi-file changes, delegate to `@pe-con-builder`.

## Risk Classification

#file:.github/prompt-snippets/pe-meta-risk-classification.md

## Assess-phase evidence coverage (entry-point depth parity)

A direct `/pe-meta-prompt-review` call MUST reach the **same evidence depth** as the `/pe-meta-review` orchestrator — depth is a property of the work, not the entry path. This prompt therefore includes the first **technique module** (engine **Assess** phase):

```text
#file:.github/prompt-snippets/pe-meta-evidence-coverage.md
```

**`dim_evidence[]` (MANDATORY).** For EVERY applicable dimension — **passes included** — record one `{dim, status, evidence_ref}` object with a non-empty, anchored `evidence_ref` (`path:line` + verbatim quote). A `status: pass` with an empty `evidence_ref` does NOT count as covered. The SET of applicable dimensions is the `05.07` applicability-matrix set for the `prompt` type (NOT the `05.08` subset); an applicable dimension with no `05.08` rows still requires one anchored `evidence_ref`. Each dimension's `evidence_ref` set MUST discharge every sub-check declared for the `prompt` type in [05.08-pe-meta-type-checklists.md](../../../.copilot/context/00.00-prompt-engineering/05.08-pe-meta-type-checklists.md).

**Independent Coverage Audit (before any clean health score).** Hand the run's outcome log to the existing `Validate` handoff — `@pe-meta-validator` in **Coverage Audit** mode (read-only, separate context) — which independently re-derives `pu-evidence`/`subcheck-coverage`/`shallow-sweep` per the shared [evidence-bound coverage contract](../../prompt-snippets/pe-meta-evidence-coverage.md) § Independent audit. **Divergence is a hard-fail** — reconciled, NOT self-attested.

**Evidence-depth hard-fail (single-file, mode-independent).** Before emitting the report:

- `pu-evidence` `<evidenced> < <applicable>` → **hard failure on BOTH `--mode plan` and `--mode apply`** (an evidence-free PASS is indistinguishable from a dimension never exercised).
- any dimension `subcheck-coverage <evaluated>/<declared> < 1` → graded `partial`, **BLOCKS a clean health score** (a declared sub-check never ran).
- `shallow-sweep=suspected` → **BLOCKS clean** until body-level `evidence_ref` or an explicit acknowledgment is recorded.

The Layer-A anchor checks (resolvability, literal-containment, distinctness) are `Evaluation: hook:.github/hooks/scripts/pe-check-evidence-anchors.ps1` — deterministic, zero-LLM, every PU.

## Output contract (coverage + plan-file + spillover markers)

The report MUST open with a first-line `Resolved invocation:` log echoing the `plan-file=`, `spillover=`, and the three evidence-coverage markers:

```text
Resolved invocation: --mode=<plan|apply> … | plan-file=<path-or-none> | spillover=<path-or-none> | pu-evidence=<evidenced>/<applicable> | subcheck-coverage=<fully-covered-dims>/<applicable-dims> | shallow-sweep=<clean|suspected>
```

- **`pu-evidence` / `subcheck-coverage` / `shallow-sweep`:** computed per the § Assess-phase evidence coverage contract above and reconciled with the independent Coverage Audit. Each carries the evidence-depth hard-fail described there.

- **`--mode plan` (assessment-only):** emit an actionable plan file at the canonical plan-mode path per [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md) and record `plan-file=<path>`. When `--mode apply`, record `plan-file=none`.
- **`--mode apply` overflow:** if the per-cycle change cap is hit with validated findings remaining, emit a spillover plan at `<run-folder>/<NN>-<kebab-name>-spillover.plan.md` per [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md) and record `spillover=<path>`; otherwise record `spillover=none`.

<!--
prompt_metadata:
  version: "2.4.0"
  last_updated: "2026-06-25"
-->
