---
name: pe-meta-context-design
description: "Design a new PE-for-PE context file with construction invariants, layer correctness, and non-redundancy checks"
agent: agent
model: claude-opus-4.6
tools: [semantic_search, read_file, file_search, grep_search, list_dir, create_file, replace_string_in_file, multi_replace_string_in_file]
handoffs:
  - label: "Research"
    agent: pe-meta-researcher
    send: true
  - label: "Build"
    agent: pe-meta-builder
    send: true
  - label: "Validate"
    agent: pe-meta-validator
    send: true
argument-hint: '<description> — e.g., "context file for model routing patterns"'
goal: "Ensure a PE-for-PE context file meets the shared quality objective and scope intent (reliability, effectiveness, efficiency) with type-applicable requirements"
scope:
  covers: ["Shared quality objective and scope intent enforcement (applicability-scoped)", "Requirements gathering with use-case challenge", "Pattern research from existing context files", "Construction with 6 guidance quality invariants", "Layer correctness and non-redundancy validation"]
  excludes: ["Domain context files (use /pe-con-design)", "Updates to existing files (use /pe-meta-context-create-update)"]
boundaries:
  - "MUST share the same quality objective and scope intent as /pe-meta-context-review (applicability-scoped)"
  - "MUST run the review-parity gate as the final design step: execute the SAME applicable review dimension set that /pe-meta-context-review runs (--dim full, applicability-scoped per the 05.07 dimension applicability matrix) via @pe-meta-validator; the artifact is NOT complete until that validation PASSes — non-breaking findings are fixed in place, breaking findings are escalated for human confirmation"
  - "MUST verify file fits a STRUCTURE-README Functional Category"
  - "MUST use Level 1.5 category references"
  - "MUST meet exemplary quality bar from 05.06"
rationales:
  - "Context files are guidance producers — construction invariants ensure quality propagates to all consumers"
  - "Layer correctness prevents rule duplication across context hierarchy"
---

# Context File Design

> **v15.4 alignment.** This prompt always writes (creation/update is not assessment-only) and rejects `--mode`, so it is **exempt** from the vision v15.4 `apply = plan + execute` contract and the eighth canonical parameter `--plan-file` (applicability matrix: both ❌ for this family). It honors vision v15.4 § Iteration budget — when a run hits the per-cycle change cap with validated work remaining, it emits a checkpoint/spillover plan (see [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md), now framed as the always-plan checkpoint with a `trust`-mode resume) and records a `spillover=<path-or-none>` marker on the first-line `Resolved invocation:` log.

## Phase 0a CF-05 + Phase 0b — Invocation gates

This prompt enforces the **Phase 0a CF-05 artifact-type/path consistency check** AND the **Phase 0b domain coherence gate** defined in [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) (upstream authority: vision v15 § Domain detection, § Pipeline phases).

**Locally true for this prompt:**

- **CF-05 expected root.** Any resolved target path (positional `<file-path>`, `--scope`, or design output target) MUST resolve under `.copilot/context/`. Paths outside this root are REJECTED before Phase 0b runs; the rejection message MUST suggest the canonical replacement prompt name from the SoT § Per-prompt-class applicability matrix.
- **Phase 0b scope.** Resolved file set = the target path (+ closure under `--deps full` when this prompt's argument-hint exposes `--deps`); degenerate single-file scope is single-domain by construction.
- **Algorithm.** 3-tier metadata-first per the SoT: Tier 1 = each in-scope file's declared `domain:` frontmatter; Tier 2 = optional `pe-domain-map.yaml`; Tier 3 = `unknown`. The seed path does NOT constrain consumer domains when `--deps full` traverses the closure.
- **Gate timing.** Runs BEFORE delegating to handoffs declared in this prompt's frontmatter.
- **Consent tokens.** `bundle=accept` is recognized AND propagated when delegating to the orchestrator so it does not re-prompt; `--skip domain-coherence` is REJECTED with CF-05.
- **When delegated from an orchestrator.** Phase 0a CF-05 is verified by the dispatcher and Phase 0b has already run on the single-domain resolved scope — this section's gate is a no-op in that path.

## Process
1. Requirements gathering — use-case challenge (3-7 scenarios for who loads this and why)
2. Research existing context files for overlap (STRUCTURE-README categories)
3. Load type-specific checklist from `05.08-pe-meta-type-checklists.md` → context section
4. Determine Functional Category placement
5. Build via `@pe-meta-builder` with construction invariants enforced
6. **Review-parity gate** — run the full applicable review dimension set (`--dim full`, applicability-scoped per [05.07-pe-meta-dimension-catalog.md](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md)) via `@pe-meta-validator` (structural + strategic) — identical to what `/pe-meta-context-review` runs. That review path includes the [assess/evidence-coverage technique module](../../prompt-snippets/pe-meta-evidence-coverage.md), so the design path inherits the same evidence depth (`pu-evidence`/`subcheck-coverage`/`shallow-sweep`) without re-inlining it. The artifact is NOT done until this PASSes: fix non-breaking findings in place, escalate breaking findings.
7. Update STRUCTURE-README with new file entry

## Construction Invariants (Context-Specific)
- **Non-redundancy**: No rule duplicated from another context file at same or higher layer
- **Non-contradiction**: No rule conflicts with another context file
- **Non-ambiguity**: Every rule has single clear interpretation
- **Testability**: Each rule can be verified by checking consumer artifacts
- **Completeness**: Topic fully covered without gaps that force consumers to guess
- **Layer-correctness**: Rules at appropriate abstraction level for this file's position

## Phase ordering and option behavior

1. Phase ordering: parse inputs first, execute the type-specific workflow second, then validate and report.
2. `--dim` restricts which quality dimensions to evaluate during design validation steps.
3. `--scope` filters which artifact types to focus on when composing dependencies.
4. Options `--mode`, `--deps`, and `--skip` are NOT supported for design commands — reject per `pe-meta-option-applicability-matrix.md`.

## Output contract (spillover marker)

The report MUST open with a first-line `Resolved invocation:` log echoing the `spillover=` marker:

```text
Resolved invocation: --scope=<…> … | spillover=<path-or-none>
```

If the per-cycle change cap is hit with validated work remaining, emit a spillover plan at `<run-folder>/<NN>-<kebab-name>-spillover.plan.md` per [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md) and record `spillover=<path>`; otherwise record `spillover=none`. `--mode plan` is NOT offered by this family, so no `plan-file=` marker is emitted.

<!--
prompt_metadata:
  version: "2.3.1"
  last_updated: "2026-06-24"
-->
