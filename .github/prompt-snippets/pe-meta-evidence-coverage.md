# pe-meta evidence-bound coverage and shallow-sweep guard

> Shared coverage-depth contract for pe-meta review work. Included by the orchestrator (`pe-meta-review`) and the per-dimension validator (`pe-meta-validator`) so depth-of-evidence is enforced identically wherever dimensions are assessed. Breadth markers (`phase4-coverage`, `dims-exercised`) verify *invocation*; this contract verifies *evidence*; the independent-audit contract (below) verifies the verification was done by a **second actor**, not self-attested.

## Technique module contract

This snippet is the **first named technique module** of the self-updating engine — the concrete realization of the engine's **Assess** phase. Consumers (the `pe-meta-review` orchestrator, the `pe-meta-validator`, and the per-type meta prompts) include it so a review reaches the **same evidence depth regardless of which command invokes it** — depth is a property of the work, not the entry path.

| Field | Value |
|---|---|
| **Module id** | `assess/evidence-coverage` |
| **Engine phase** | Assess |
| **Inputs** | in-scope artifact set; applicable-dimension set (from [05.07-pe-meta-dimension-catalog.md](../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md)); per-type sub-checks (from [05.08-pe-meta-type-checklists.md](../../.copilot/context/00.00-prompt-engineering/05.08-pe-meta-type-checklists.md)); `--mode plan\|apply` |
| **Outputs** | the `dim_evidence[]` outcome log; first-line markers `pu-evidence=<e>/<a>`, `subcheck-coverage=<ev>/<decl>` per dimension, the graded verdict, and `shallow-sweep=<clean\|suspected>`; the hard-fail conditions defined below |
| **Cost tier** | deterministic Layer A (zero-LLM, every PU, `Evaluation: hook:.github/hooks/scripts/pe-check-evidence-anchors.ps1`) + sampled Layer B (reasoning) |

**Invocation contract.** A consumer MUST:

1. `#file:`-include this snippet (`#file:.github/prompt-snippets/pe-meta-evidence-coverage.md`).
2. Emit `pu-evidence`, `subcheck-coverage`, and `shallow-sweep` on its first-line `Resolved invocation:` log.
3. Hand the outcome log to the **independent second actor** (`@pe-meta-validator` in Coverage Audit mode) before declaring a clean health score — the verdict is reconciled, never self-attested.

> *"Technique module" is an implementation-layer label realized by this snippet. Naming the technique-module layer at the vision level and adding the entry-point depth-parity principle to the engine vision are a sequenced human-only engine-vision amendment (the PL-3/PL-4 park-lot items of the per-type depth-parity pilot plan), not part of this contract.*

## Evidence-bound coverage (the depth invariant)

A **processing unit (PU)** is one `(artifact × applicable-dimension)`. A PU is **covered** only when its outcome-log entry carries a non-empty `evidence_ref` — for **both** findings and passes:

| Outcome | Required `evidence_ref` |
|---|---|
| `fail` / `partial` (finding) | file+line, tool result, or quoted text proving the defect |
| `pass` | a one-line proof the check actually ran — the tool output, the file+line inspected, or the quoted text that establishes the PASS |

An empty `evidence_ref` on **any** applicable PU — `status: pass` included — means the PU is **uncovered**. A self-asserted PASS with no evidence is byte-for-byte indistinguishable from a dimension that was never exercised, so it does not count. (The recorded *state* of a PU — `pass` included — is the graded verdict defined below, not a separate enum; an empty `evidence_ref` records `never`. See **Unified recorded-state enum**.)

### Mandatory machine anchor (the verbatim-anchor rule)

Every `evidence_ref` MUST carry a deterministically checkable anchor: a **`path:line` locator** AND a **verbatim quoted snippet** copied from that exact location (or a captured tool-output line for tool-derived evidence). A bare prose assertion ("goal is single-sentence") with no `path:line` + quote is **not** a valid `evidence_ref` — it gives the independent audit nothing to verify. The locator+quote pair is what makes Layer-A verification (below) possible on 100% of PUs without any LLM call.

### `pu-evidence` marker

Emit `pu-evidence=<evidenced>/<applicable>` where `<applicable>` = the count of dimensions applicable to the artifact type **per the [05.07](../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md) applicability matrix** (the full type-applicable set — one PU per `(artifact × 05.07-applicable-dimension)`), and `<evidenced>` = count carrying a non-empty `evidence_ref`. `05.08` supplies the **sub-checks** for those applicable dimensions that declare rows; a dimension applicable per `05.07` with no `05.08` rows still requires one anchored `evidence_ref`. `<evidenced> < <applicable>` is a **hard failure on BOTH `--mode plan` and `--mode apply`** — depth-of-evidence is mode-independent: a plan that asserts clean passes without proof misleads exactly as much as an apply that does.

**Collapsed-denominator hard-fail.** `<applicable>` MUST equal the `05.07` applicability-matrix count for the artifact type. A run whose reported `<applicable>` is **less than** that count has silently narrowed its applicable set — typically by sourcing the set from the `05.08` structural checklist (≈8 rows) instead of the `05.07` matrix (≈28 for prompts/agents). This is a **hard failure on BOTH `--mode plan` and `--mode apply`** regardless of how many PUs are evidenced: the false-clean signature is `pu-evidence=8/8` where the type's matrix count is ≈28. `05.08` is the sub-check source, **never** the applicable-set source.

## Sub-check completeness and the graded dimension verdict

`pu-evidence` proves every *dimension* was touched with one real quote — but a dimension is a **checklist**, not a scalar. A dimension can be evidenced on its easiest sub-check while the harder ones never run (the 2026-06-11 `D5` false-clean: the `8/3/6` count was cited, the H14 boundary↔YAML alignment row never executed). The recorded PU stays `(artifact × applicable-dimension)` — sub-checks are **not** a third persisted axis — but each dimension's `evidence_ref` set MUST discharge **every sub-check declared for its artifact type** in [05.08-pe-meta-type-checklists.md](../../.copilot/context/00.00-prompt-engineering/05.08-pe-meta-type-checklists.md). This is the `dimension-rule-self-containment` principle (vision v15.5): a rule that names a dimension is reachable from that dimension's row set, never only from a different file.

### `subcheck-coverage` marker (per dimension)

Emit `subcheck-coverage=<evaluated>/<declared>` **per dimension**, where `<declared>` = count of 05.08 sub-check rows applicable to the artifact type and `<evaluated>` = count carrying their own anchored `evidence_ref`. **This does NOT redefine `pu-evidence`** — the PU stays per-dimension; `subcheck-coverage` is an *additional* per-dimension completeness ratio that feeds the graded verdict below. `<evaluated> < <declared>` deterministically forces the dimension to `partial`.

### Graded dimension verdict

Each dimension carries a graded verdict (vision v15.5 `evidence-based`), computed from its sub-check states plus the anchor-type of each passed sub-check:

| Grade | Condition | Counts as OK? | Effect on health |
|---|---|---|---|
| `verified` | 100% sub-checks evaluated, all pass, every property sub-check backed by a **property** anchor | ✅ yes | clean |
| `pass-weak` | 100% evaluated, all pass, but ≥1 property sub-check backed only by a **presence** anchor | ⚠️ provisional | flagged, not clean |
| `partial` | `<evaluated> < <declared>` — ≥1 declared sub-check never ran, none failed | ❌ no — **unverified** | **blocks clean** |
| `fail` | ≥1 sub-check failed | ❌ no — finding | blocks clean |

`partial` is computed **deterministically** from the `subcheck-coverage` ratio — no heuristic, no sampling, no second-actor reasoning — and fires regardless of *which* dimension is thin (so it catches `D5`/`D6`/`D19`, none of which sit in the frontmatter cluster `shallow-sweep` watches). `partial` is the grade the 2026-06-11 run *should* have reported on `D5`/`D6`/`D19` instead of green.

### Unified recorded-state enum

There is **one** enum for the recorded `(artifact × dimension)` state, not two. The recorded state is the **graded verdict** (`verified | pass-weak | partial | fail`) **plus** the sentinel **`never`** — a PU that was not yet exercised (empty `evidence_ref`, or an inherited PASS deliberately demoted on a re-run). This realizes the vision `processing-state-is-multidimensional` line: *"each dimension's recorded state is the graded verdict that consolidates its sub-checks."* The legacy coverage `status` enum folds in directly:

| Legacy coverage `status` | Unified recorded state |
|---|---|
| `pass` | `verified` or `pass-weak` (graded by anchor type) |
| `fail` | `fail` |
| `partial` | `partial` |
| `never` | `never` (un-exercised sentinel — orthogonal to the verdict, *not* a quality grade) |

`never` is the only state that is not a quality verdict: it means "no verdict yet," so an inherited `verified`/`pass-weak` is demoted to `never` on a same-command re-run (per the [plan-file contract](pe-meta-plan-file-contract.md) § 5) and must be re-exercised before it can re-earn a verdict.

## Two-layer evidence verification (deterministic always, reasoning on sample + on doubt)

Verifying that an `evidence_ref` is *real* splits into two layers with different cost and coverage. **Layer A is deterministic and runs on EVERY applicable PU**; **Layer B is reasoning and runs on a sample and on doubt**. Counting a non-empty `evidence_ref` is necessary but not sufficient — a ref can be non-empty yet fabricated or mis-pointed.

### Layer A — deterministic, every PU (no LLM judgment)

Given the mandatory `path:line` + verbatim-quote anchor, three checks run mechanically on every PU:

1. **Resolvability** — the cited `path` exists and the cited `line` is within the file's range.
2. **Literal-containment** — the quoted snippet appears **verbatim** at the cited file+line (whitespace-normalized substring match). This catches mis-pointed and fabricated refs with **zero LLM calls**.
3. **Distinctness** — the same `evidence_ref` string MUST NOT be reused across multiple PUs (the batch-marking signature — one quote pasted under many dimensions).
4. **Anchor-type classification (presence vs property)** — for a sub-check whose 05.08 row tests a *property* (a comparison or computed result, e.g. H14 "every YAML boundary maps to a body entry"), the anchor MUST cite the **result of the test** (the unmapped entry, the mismatched count), not merely a heading or a section's existence. An anchor that cites only a heading/count for a property sub-check is a **presence anchor** and is flagged — feeding the `pass-weak` grade with a cheap pattern check, zero LLM calls. (A sub-check that *is* a presence test — "persona section present" — is correctly satisfied by a presence anchor.)

Any Layer-A failure deterministically downgrades the run to `shallow-sweep=suspected` and surfaces a finding — no sampling, every PU. Layer A is the mechanically-computable guard extracted into code (see the delegation annotation below).

### Layer B — reasoning, sample + on doubt (LLM judgment)

Layer A proves the quote *exists* at the cited line; only reasoning can judge whether it *supports* the asserted status. The independent auditor (below) therefore:

- **Samples** `N` refs per run — `N = max(3, ceil(0.15 × evidenced))`, weighted toward the evidence-bearing groups (`D14-craftsmanship`, `D28`–`D35` reliability) where shallow passes historically hid — re-reads the cited content, and confirms semantic support for the asserted dimension status.
- **Escalates on doubt**: any PU whose Layer-A anchor is thin or generic (quote present but boilerplate, e.g. a section heading reused as proof) is re-read regardless of whether it fell in the sample.
- A sampled or escalated ref that does **not** support its status downgrades the run to `shallow-sweep=suspected` and surfaces a finding.

## Independent audit — two actors, not self-attestation

The coverage markers above are only trustworthy if a **different actor** computes them from the outcome log than the one that wrote the log. The orchestrator (`pe-meta-review`) writes the Phase 4 `dim_evidence[]` outcome log; before the run closes it hands that log to `@pe-meta-validator` (read-only, already a sibling) running its **Coverage Audit** mode. The validator:

1. Independently recomputes `pu-evidence=<evidenced>/<applicable>` from the outcome log (Layer A on every PU).
2. Runs Layer B sampling + on-doubt re-reads.
3. Independently derives `shallow-sweep=<clean|suspected>`.
4. Emits its own verdict, which the orchestrator MUST **reconcile** against its own.

**Divergence between the two verdicts is a hard-fail** — the run BLOCKS until reconciled. This is what makes the phrase "reconciled, NOT self-attested" literally true: two actors, one writing and one auditing, so a shallow pass the orchestrator narrated as clean is caught by an actor that did not share that narration.

## Delegation annotation — rules stay in guidance, evaluation is delegated

A rule that is mechanically computable MAY have its **evaluation** delegated to code (a hook, script, skill, or MCP tool) — but the rule itself NEVER leaves guidance. To keep the single-source-of-truth intact while allowing the evaluator to move (hook today, skill or MCP tomorrow), each delegated rule carries a one-line tag:

```text
Evaluation: <prose self-check | hook:<script-path> | skill:<name> | mcp:<tool>>
```

- The rule's full statement + rationale stays in its canonical guidance file. A rule is **NEVER** reduced to "see the script."
- Only the `Evaluation:` tag changes when the evaluator moves — no rule is rewritten, relocated, or lost.
- An agent reading a rule tagged with a non-`prose self-check` evaluator knows the check is **already enforced deterministically** and MUST NOT re-evaluate it under reasoning — this controls both duplicate evaluation and cost without deleting the rule.

The Layer-A evidence checks above are tagged `Evaluation: hook:.github/hooks/scripts/pe-check-evidence-anchors.ps1` (resolvability, literal-containment, distinctness). The anchor-type classification (check 4) is `Evaluation: prose self-check` until the hook's pattern check is extended — its rule lives here regardless of where evaluation runs.

## Shallow-sweep guard (the breadth-vs-depth heuristic)

On a `--dim full` or derived `breadth=full` run, raise `shallow-sweep-suspected` when BOTH hold:

1. **Findings cluster in frontmatter dims** — every finding maps to `D1-metadata`–`D5-boundaries`.
2. **Body groups are silent** — the content (`D9`–`D11`), efficiency (`D20`–`D27`), and reliability (`D28`–`D35`) groups produced **zero findings AND zero non-trivial evidence-cited passes** (a non-trivial pass cites artifact-**body** content in its `evidence_ref`, not just frontmatter presence).

`shallow-sweep-suspected` BLOCKS a clean health score until the run either (a) supplies body-level `evidence_ref` for the silent groups, or (b) records an explicit acknowledgment that the silent groups were exercised and are genuinely clean. Emit the result as `shallow-sweep=<clean|suspected>` on the first-line `Resolved invocation:` log.

**Reconcile and trust runs are NOT exempt.** A baseline marked `shallow-sweep=suspected` carries that state forward; a same `--scope`+`--dim` re-run inherits `suspected` and clears it only when the previously silent body groups produce fresh body-level `evidence_ref` — it can never auto-upgrade a suspected baseline to clean by inheriting the prior PASS.

## Canonical-source discipline for metadata findings (`D30-metadata-guard`)

A metadata-contract finding (key name, required field, format) MUST cite the **canonical value and its authority** before recommending a change. Artifact-type convention is the authority: agents use `agent_metadata:`, articles use `article_metadata:` (per the documentation base instruction). Resolve the canonical value against the **in-scope majority**, never a same-folder eyeball. A "sync to siblings" recommendation that cannot name which value is canonical and why is rejected.

> **Why this contract exists.** The [2026-06-07 shallow-sweep run](../../src/docs/90.%20Issues/202606/20260607.01-pe-meta-update/01-update-run-analysis/overview.md) reported `dims-exercised=full` with health 100 while three residuals sat unexamined in exactly these silent groups — and a prior metadata "fix" recommended syncing to siblings without establishing the canonical key (and was wrong). This contract makes both failure modes detectable.
