## Plan output contract (every mutating run)

A plan file is the single pivot artifact every mutating run passes through. Both modes materialize a plan; they differ only in whether it is then executed:

- **`--mode plan`** = materialize plan, stop (`plan = apply minus execute`).
- **`--mode apply`** = materialize plan, **then execute it through the same execution engine** (`apply = plan + execute`).

Plan emission is non-skippable on every mutating run; `--skip plan-emission` is REJECTED with CF-05.

### 1. Path resolution (canonical algorithm)

Default auto-name: `<run-folder>/<NN>-<kebab-name>.plan.md`

- **`<run-folder>`** — the conversation's current working folder when one obviously applies (e.g. a `src/docs/.../<run>` folder the user is already operating in).
- **`<NN>`** — next available two-digit prefix in `<run-folder>` (e.g. `01`, `02`); ensures sortable ordering when multiple plans land in the same folder.
- **`<kebab-name>`** — derived from the resolved invocation, e.g. `pe-meta-review-context-freshness` for `/pe-meta-review --scope context --dim freshness`.
- **Fallback** (no obvious run folder): `.copilot/temp/pe-meta-state/plans/YYYYMMDD-HHMMSS-<kebab-name>.plan.md` (created on demand).

The eighth canonical parameter **`--plan-file <path>`** overrides the location/identity **only** — it never decides whether the plan is regenerated or trusted (see § 4). The same algorithm produces the spillover-plan path with `-spillover` appended: `<run-folder>/<NN>-<kebab-name>-spillover.plan.md` (see [pe-meta-iteration-budget.md](pe-meta-iteration-budget.md)).

### 2. Required plan content

The plan MUST conform to `plan-execution.instructions.md`:

- Frontmatter: `status: draft`, `target_vision_version`, `domain`, `created`, `goal`.
- Clear goal statement (1 sentence).
- One goal-table row per validated finding from Phases 1–4 carrying `scope tag`, `principle impact`, `downstream landing` (per `vision-amendment.instructions.md`).
- Items decomposed to actionability-gate-ready granularity (each item maps to one editable artifact + one verifiable change).
- **Execution-ready precision (load-bearing).** Because a plan may be executed by a cheaper executor model (the reasoning→standard model seam runs execution on a standard model), every actionable row MUST carry either a literal `old → new` edit or an unambiguous anchor (file + section + match target). A cheaper executor applies edits; it does not re-derive intent. A row that cannot be made execution-ready is escalated, not silently applied.
- Park lot section for surfaced edge cases that do not block promotion.
- Exit criteria.

### 3. First-line marker

The Phase 8 `Resolved invocation:` first-line log MUST include:

```text
plan-file=<absolute-or-workspace-relative-path>
```

The linter rejects reports that omit the marker (vision success criterion #12 v15.1 marker extension).

### 4. Execution modes — fresh / reconcile / trust

Whether a mutating run **regenerates** its plan or **trusts** an existing one is governed by two orthogonal booleans — never by file existence:

- **baseline available?** — a plan referenced by `--plan-file`, OR a plan generated **earlier in the same conversation** (a same-conversation plan counts exactly as an on-disk baseline does).
- **research runs?** — Phase 1 research is skipped only via the existing `--skip research`.

| baseline available? | research runs? | Mode | Behavior | Drift guard |
|---|---|---|---|---|
| no | yes | **fresh** | Generate plan from research → write → execute. | skipped (back-to-back) |
| yes | yes | **reconcile** | Load baseline, merge fresh evidence, preserve human decisions, re-verify coverage + actionability, overwrite → execute. | skipped (research re-validates) |
| yes | no | **trust** | Execute the baseline's **human decisions** as-is; per § 5, re-emit a fresh per-dimension `evidence_ref` for every applicable PU (an inherited PASS counts as `never`). | **REQUIRED** (cross-run) |
| no | no | **invalid** | Rejected at `breadth=full`: no baseline to substitute for research. | — |

`--skip research` is legal at `breadth=full` ONLY when a validated baseline plan substitutes for research — a principled exception to `default-full-investigation`, because the plan IS the prior research product.

### 5. Reconcile — preserve human-authored decisions

In **reconcile** mode the baseline is a living input. Machine-derived findings (drift signals, source digests, dimension scores) are refreshed, but **human-authored decisions MUST NOT be silently overwritten**: park-lot rulings, consent lines, scope tags, and human-authored rationales are preserved. When fresh evidence contradicts a human-authored decision, the orchestrator **escalates the conflict** for human resolution rather than overwriting it (the `metadata-guarded-changes` guard/reconcile pair applied to the plan artifact itself).

**A baseline substitutes for research, never for per-dimension evidence.** In both **reconcile** and **trust** modes, a prior run's PASS does NOT carry forward as coverage: every applicable processing unit MUST re-emit a fresh `evidence_ref` (per the [evidence-bound coverage contract](pe-meta-evidence-coverage.md)). A PU whose only support is an inherited PASS is treated as `never` and re-exercised. The baseline inherits human *decisions*; it never inherits the *proof* that a dimension was checked.

**Re-running the same command lowers baseline confidence, never raises it.** When the new invocation has the **same `--scope`+`--dim`** as the baseline run, the re-run is itself evidence the prior pass may be incomplete: it MUST re-derive per-dimension evidence (above) AND inherit the baseline's `shallow-sweep=suspected` state if set, clearing it only when the previously silent body groups (content `D9`–`D11`, efficiency `D20`–`D27`, reliability `D28`–`D35`) produce fresh body-level evidence. A same-command re-run can therefore never auto-upgrade a `suspected` baseline to a clean health score.

### 6. Drift guard (trust mode only)

Target-section hashing: hash each target section when the plan is written; re-hash before applying.

- **fresh / reconcile** (same-run): guard **skipped** — plan and execute are back-to-back, no drift window exists.
- **trust** (cross-run, research skipped): guard **REQUIRED** — a target may have changed since the baseline was written. On a hash mismatch the run degrades gracefully — it escalates the drifted row for re-assessment rather than applying a stale edit blindly.

### 7. Why on-disk, not chat-only

A chat-only findings report disappears with the conversation. A plan file landed in the run folder is reviewable later, version-controlled, and promotable to `actionable` through the same gate that governs human-authored plans — closing the human-handoff loop that `human-governance-autonomous-execution` (P1) requires. Because `apply` now materializes the same artifact, that guarantee holds on **every** mutating run, not only on plan-mode previews.

### 8. Forward references

- Vision § Plan output contract — authoritative rule.
- Vision § Plan execution modes, § Reconcile semantics, § Model-routing seam, § Execution-ready precision and drift-guard scope.
- Vision § Iteration budget — overflow spillover plan (a special case of the always-plan checkpoint; uses the same path algorithm).
- `plan-execution.instructions.md` — plan file lifecycle and actionability gate.
