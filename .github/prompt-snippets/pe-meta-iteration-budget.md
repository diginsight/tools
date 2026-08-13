## Iteration budget and spillover (when `--mode apply` is active)

The orchestrator enforces a per-cycle change cap to bound cost and blast radius. When the cap is hit mid-run, a spillover plan file MUST be emitted so the next cycle can resume without re-investigation.

> **Spillover is a special case of the always-plan checkpoint, not a bolt-on.** Every mutating run already materializes a plan (see [pe-meta-plan-file-contract.md](pe-meta-plan-file-contract.md) § Plan output contract). Overflow simply emits the *remaining-but-unapplied* slice as its own plan and resumes it next cycle. The resume is a **trust-mode** consumption of that spillover plan (baseline available via `original-run=`, research skipped), drift-guarded per the cross-run rule.

### 1. Budget rule

- **Default cap:** 10 autonomous changes per cycle (configurable).
- **What counts:** changes applied by the orchestrator in `--mode apply` under `autonomy_level: autonomous`.
- **What does NOT count:** human-approved changes; assessment-only operations; rejected findings; in-flight investigations (Phases 1–3).
- Applies uniformly to every `/pe-meta-*` command family that writes (per `command-family-agnostic`).

### 2. Overflow trigger

Overflow occurs when Phase 4 (Execution) has applied the maximum allowed autonomous changes AND at least one validated finding remains unapplied.

### 3. Spillover plan emission (non-skippable)

On overflow, the orchestrator MUST write a spillover plan file at:

`<run-folder>/<NN>-<kebab-name>-spillover.plan.md`

Same path-resolution algorithm as [pe-meta-plan-file-contract.md](pe-meta-plan-file-contract.md) (run folder, two-digit prefix, kebab name). Status: `draft`. Conforms to `plan-execution.instructions.md`.

### 4. Spillover plan content

One goal-table row per remaining-but-unapplied validated finding carrying:

- `scope tag`
- `principle impact`
- `downstream landing`
- `original-run=<plan-file-or-run-id>` — linkage so the next cycle can resume context without re-investigating. The next cycle consumes the spillover plan in **trust** mode (baseline available, `--skip research`), subject to the cross-run drift guard.

### 5. First-line marker

The Phase 8 `Resolved invocation:` first-line log MUST emit:

```text
spillover=<absolute-or-workspace-relative-path>   # on overflow
spillover=none                                    # otherwise
```

Linter rejects reports that omit the marker on overflow runs (vision success criterion #12 v15.1 marker extension).

### 6. Forward references

- Vision § Iteration budget — authoritative rule.
- Vision § Plan output contract — shares the same path algorithm; spillover is a special case of the always-plan checkpoint.
- Vision § Plan execution modes — the spillover resume is a trust-mode consumption.
