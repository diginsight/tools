---
name: pe-meta-instruction-create-update
description: "Create or update a PE-for-PE instruction file with applyTo precision and minimization guards"
agent: agent
model: claude-opus-4.6
tools: [read_file, file_search, grep_search, list_dir, create_file, replace_string_in_file, multi_replace_string_in_file]
handoffs:
  - {label: "Build", agent: pe-meta-builder, send: true}
  - {label: "Validate", agent: pe-meta-validator, send: true}
argument-hint: '<file-path-or-description> [--dim <group>]'
goal: "Create or update PE-for-PE instruction files with applyTo precision and minimization"
scope:
  covers: ["Direct creation", "Updates with applyTo preservation guard", "Minimization enforcement"]
  excludes: ["Design (use /pe-meta-instruction-design)", "Domain instructions"]
boundaries:
  - "MUST verify applyTo pattern matches intended files only"
  - "MUST not exceed minimization threshold (rules-only, no prose)"
  - "MUST verify description field present"
rationales:
  - "Type-specific PE-meta workflow improves reliability and maintainability"
  - "Explicit orchestration metadata supports deterministic validation and safer automation"
---

# Instruction Create/Update

> **v15.4 alignment.** This prompt always writes (creation/update is not assessment-only) and rejects `--mode`, so it is **exempt** from the vision v15.4 `apply = plan + execute` contract and the eighth canonical parameter `--plan-file` (applicability matrix: both ❌ for this family). It honors vision v15.4 § Iteration budget — when a run hits the per-cycle change cap with validated work remaining, it emits a checkpoint/spillover plan (see [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md), now framed as the always-plan checkpoint with a `trust`-mode resume) and records a `spillover=<path-or-none>` marker on the first-line `Resolved invocation:` log.

## Phase 0a CF-05 + Phase 0b — Invocation gates

This prompt enforces the **Phase 0a CF-05 artifact-type/path consistency check** AND the **Phase 0b domain coherence gate** defined in [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) (upstream authority: vision v15 § Domain detection, § Pipeline phases).

**Locally true for this prompt:**

- **CF-05 expected root.** Any resolved target path (positional `<file-path>`, `--scope`, or design output target) MUST resolve under `.github/instructions/`. Paths outside this root are REJECTED before Phase 0b runs; the rejection message MUST suggest the canonical replacement prompt name from the SoT § Per-prompt-class applicability matrix.
- **Phase 0b scope.** Resolved file set = the target path (+ closure under `--deps full` when this prompt's argument-hint exposes `--deps`); degenerate single-file scope is single-domain by construction.
- **Algorithm.** 3-tier metadata-first per the SoT: Tier 1 = each in-scope file's declared `domain:` frontmatter; Tier 2 = optional `pe-domain-map.yaml`; Tier 3 = `unknown`. The seed path does NOT constrain consumer domains when `--deps full` traverses the closure.
- **Gate timing.** Runs BEFORE delegating to handoffs declared in this prompt's frontmatter.
- **Consent tokens.** `bundle=accept` is recognized AND propagated when delegating to the orchestrator so it does not re-prompt; `--skip domain-coherence` is REJECTED with CF-05.
- **When delegated from an orchestrator.** Phase 0a CF-05 is verified by the dispatcher and Phase 0b has already run on the single-domain resolved scope — this section's gate is a no-op in that path.

## Process
1. Determine mode: CREATE or UPDATE
2. **UPDATE**: Pre-change guard — verify applyTo pattern unchanged unless explicitly requested
3. Load checklist from `05.08-pe-meta-type-checklists.md` → instruction section
4. Build via `@pe-meta-builder` with minimization (instruction-minimization)
5. Validate via `@pe-meta-validator` (scoped by `--dim`)
6. Post-change: test applyTo against workspace, version bump

## Phase ordering and option behavior

1. Phase ordering: parse inputs first, execute the type-specific workflow second, then validate and report.
2. `--dim` restricts which quality dimensions the validation sub-step evaluates during pre-change review steps; default (omitted) = `full`.
3. `--scope` filters which artifact types to focus on when composing dependencies.
4. Options `--mode`, `--deps`, and `--skip` are NOT supported for create-update commands — reject per `pe-meta-option-applicability-matrix.md`.

## Output contract (spillover marker)

The report MUST open with a first-line `Resolved invocation:` log echoing the `spillover=` marker:

```text
Resolved invocation: --scope=<…> … | spillover=<path-or-none>
```

If the per-cycle change cap is hit with validated work remaining, emit a spillover plan at `<run-folder>/<NN>-<kebab-name>-spillover.plan.md` per [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md) and record `spillover=<path>`; otherwise record `spillover=none`. `--mode plan` is NOT offered by this family, so no `plan-file=` marker is emitted.

<!--
prompt_metadata:
  version: "2.2.1"
  last_updated: "2026-06-25"
-->
