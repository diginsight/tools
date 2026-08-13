---
name: pe-meta-design
description: "Design a new PE-for-PE artifact with full research → build → validate pipeline plus vision alignment, category compliance, and ecosystem coherence checks"
agent: agent
model: claude-opus-4.6
tools:
  - semantic_search
  - read_file
  - file_search
  - grep_search
  - list_dir
  - create_file
handoffs:
  - label: "Research Requirements"
    agent: pe-meta-researcher
    send: true
  - label: "Strategic Design"
    agent: pe-meta-designer
    send: true
  - label: "Build Artifact"
    agent: pe-meta-builder
    send: true
  - label: "Validation & Coverage Audit"
    agent: pe-meta-validator
    send: true
argument-hint: '<artifact-type> <description> [--mode plan|apply] [--scope <type|paths>] [--source <id|url>] [--start <date|version>] [--end <date|version>] [--dim <group|D#|full>] [--skip <phase>[,...]] [--plan-file <path>] [bundle=accept] — e.g., "agent for validating category coverage" or "context file for MCP tool composition patterns --mode plan"'
goal: "Design and create a PE-for-PE artifact that is both structurally correct and strategically aligned with the PE vision, using the full research → build → validate pipeline with added strategic checks"
scope:
  covers:
    - "Full design pipeline for PE-for-PE artifacts (research → strategic check → build → double validate)"
    - "Vision alignment enforcement during design"
    - "Category reference compliance by construction"
    - "Ecosystem impact analysis before creation"
    - "Phase 0b — domain coherence (per 04.05-pe-meta-invocation-gates.md)"
    - "bundle=accept consent token recognition on multi-domain scopes"
    - "Full eight-parameter invocation surface (Phase 0a pre-parse, value-shape resolution, default-full, first-line Resolved-invocation log)"
    - "Source-grounded research seed-corpus selection (--source/--start/--end) via pe-meta-researcher"
    - "Dimension/construction-quality parity (--dim groups; six guidance-quality properties)"
    - "Evidence-and-coverage recording on the design verdict (per pe-meta-evidence-coverage.md)"
    - "Autonomy classification (Tier 1/2/3) and plan materialization on every mutating run"
    - "Design/Review parity contract (six properties; reciprocal review-as-final-step)"
  excludes:
    - "Domain artifacts (article-writing, documentation — use /pe-con-design for those)"
    - "Ecosystem-wide audits (use /pe-meta-review for those)"
    - "Updates to existing artifacts (use /pe-meta-create-update for those)"
boundaries:
  - "MUST load PE-strategic context before research"
  - "MUST pass PE-strategic constraints to pe-meta-researcher and pe-meta-builder"
  - "MUST run double validation (structural + ecosystem coherence)"
  - "Only for PE-for-PE artifacts — redirect domain artifacts to /pe-con-design"
  - "Phase 0b is NOT skippable; --skip domain-coherence is rejected with CF-05; Phase 0b runs on the planned target before delegating to pe-meta-researcher/pe-meta-builder"
  - "bundle=accept is the ONLY valid consent token (closed set); recorded on first-line `Resolved invocation:` log"
  - "Phase 0a CF-05 artifact-type/path consistency does NOT apply at this orchestrator-level layer (artifact-type is supplied as the first positional argument); CF-05 is enforced ONLY by per-artifact prompts"
  - "`apply = plan + execute` (vision v15.9): this prompt runs research, materializes a design plan, then executes the build. With no baseline this is **fresh** mode; supplying `--plan-file` as a baseline alongside research makes it **reconcile** (escalate-not-overwrite human-authored design decisions)"
  - "`--plan-file <path>` (vision v15.9) sets plan location/identity ONLY and never decides regenerate-vs-trust; a same-conversation just-generated plan is an implicit baseline. Default auto-name path — see [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md)"
  - "Model-routing seam (vision v15.9): research + design-plan run on the reasoning-grade `model:` declared here; delegated execution (pe-meta-builder) carries its own standard-grade `model:` — no mid-prompt switching"
  - "Full parameter surface (vision v15.9): accepts --mode, --scope, --source, --start/--end, --dim, --skip, --plan-file (NOT --deps — a not-yet-existing artifact has no dependency closure). Parsed in Phase 0a before any phase runs; unknown flags rejected via CF-05 with the canonical enumeration"
  - "Default invocation is --dim full (all applicable construction-quality dimension groups) when --dim is omitted; default --mode is apply"
  - "Phase 0a emits a first-line `Resolved invocation:` log (mode, scope, dim, skip, plan-file, bundle, seed-corpus window) — parity with pe-meta-review; checked by the Phase 8 first-line-log linter"
  - "--skip is per-phase; Phases 0/0a/0b/8 are NEVER skippable; --skip plan-emission and --skip coverage are REJECTED"
  - "Every mutating run materializes a design plan (Phase 6) and records an evidence-anchored coverage outcome-log per pe-meta-evidence-coverage.md (pu-evidence is a hard-fail)"
  - "Reciprocal parity: as its final step (Phase 8) this design prompt runs pe-meta-review's `--dim full` applicability-scoped over the created artifact (Design/Review Parity Contract)"
rationales:
  - "PE-for-PE artifacts set the quality standard — they must be designed with vision alignment from the start"
  - "Adding strategic context to the research phase prevents structurally correct but strategically misaligned artifacts"
  - "Double validation catches issues that structural validation alone misses"
---

# PE-for-PE Artifact Design

Design and create PE artifacts that serve the PE system itself — with vision alignment, category compliance, and ecosystem coherence built in from the start. This prompt is the **creation-side orchestrator** and is held to the same invocation, evidence, and reporting contract as its review-side sibling [`pe-meta-review`](pe-meta-review.prompt.md) (Design/Review Parity Contract, below).

**When to use this vs `/pe-con-design`:**
- This prompt → creating PE infrastructure (pe-* agents, pe-* prompts, PE context files, pe-* instructions, pe-* skills)
- `/pe-con-design` → creating domain-specific artifacts (article-writing, documentation, devops)

> **v15.9 alignment.** This prompt honors the vision **`apply = plan + execute`** contract: it runs research, materializes a design plan, then executes the build. With no baseline it runs in **fresh** mode (research drives the plan, no drift guard); supplying `--plan-file` as a baseline alongside research makes it **reconcile** (preserve human-authored design decisions, escalate-not-overwrite); a trusted same-conversation plan can run **trust** (build without re-deriving). `--plan-file <path>` (the eighth canonical parameter) sets plan location/identity ONLY and never decides regenerate-vs-trust — see [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md). The model-routing seam keeps research + design-plan on this prompt's reasoning-grade model while delegated execution (pe-meta-builder) carries its own standard-grade `model:`.

## Invocation surface

This prompt accepts the canonical eight-parameter surface, scoped to the parameters applicable to **Design** per the [option-applicability matrix](pe-meta-option-applicability-matrix.md) (Design column, reconciled 2026-06-24):

| Parameter | Design semantics |
|---|---|
| `--mode plan\|apply` | `plan` = research + materialize the design plan, then **STOP**; `apply` (default) = plan **+** execute the build |
| `--scope <type-token\|paths>` | Constrains the artifact-type / landing footprint of the design |
| `--source <id\|url>[,...]` | Seed-corpus selection — which monitored sources / external URLs ground the research |
| `--start <date\|version>` / `--end <date\|version>` | Seed-corpus / source-diff window (value-shape: ISO date OR source-version token resolved via the source's `version_scheme`) — bounds *which source material* grounds the design, NOT a review window |
| `--dim <group\|D#\|full>` | Which construction-quality dimension groups the design must satisfy by construction (default **`full`**) |
| `--skip <phase>[,...]` | Per-phase bypass; Phases 0/0a/0b/8 are NEVER skippable; `--skip plan-emission` and `--skip coverage` are REJECTED |
| `--plan-file <path>` | Plan-artifact location/identity only; default auto-name per the plan-file contract |
| `bundle=accept` | Sole consent token (closed set) for multi-domain designs |

`--deps` is **not** accepted: a not-yet-existing artifact has no dependency closure to traverse (the Phase 0b domain footprint replaces it). Unknown flags are rejected in Phase 0a via CF-05 with the canonical enumeration.

## Phase 0a — Invocation pre-parse (NOT skippable)

Before any other phase:

1. **Parse the invocation** into `<artifact-type>`, `<description>`, and the flag set above. Reject any unknown flag via **CF-05** quoting the canonical enumeration from the [option-applicability matrix](pe-meta-option-applicability-matrix.md).
2. **Resolve value-shapes.** `--start`/`--end` accept either an ISO date or a source-version token; a version token is resolved to a timestamp via the source's `version_scheme` (same resolver as pe-meta-review). Reject malformed values via CF-05.
3. **Apply defaults.** Omitted `--dim` ⇒ `--dim full`; omitted `--mode` ⇒ `apply`.
4. **CF-05 artifact-type/path consistency does NOT apply at this orchestrator tier.** The `<artifact-type>` first positional argument carries explicit type intent; CF-05 path/prefix-mismatch is enforced ONLY by per-artifact prompts.
5. **Emit the first-line `Resolved invocation:` log** — parity with pe-meta-review — recording `mode=`, `scope=`, `dim=`, `skip=`, `plan-file=`, `bundle=`, and (when windowed) `seed-corpus=<start>..<end>`. This single line precedes all other output and is checked by the Phase 8 first-line-log linter.

## Phase 0b — Domain coherence

This prompt enforces the Phase 0b domain coherence gate defined in [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) (upstream authority: current vision document § Domain-coherent batching). The 3-tier metadata-first domain resolution algorithm, seed-vs-deps decision matrix, dispatch table, `bundle=…` closed set, and `bundle=accept` consent semantics are specified there and MUST NOT be re-implemented here.

**Locally true for this prompt:**

1. **Scope = the planned target.** The design output's intended path determines its domain (Tier 1: explicit `domain:` decision in the design spec; Tier 2: `pe-domain-map.yaml` heuristic; Tier 3: `unknown`). When the design imports peers as dependencies, those peers extend the in-scope set.
2. **Gate runs BEFORE Phase 2 (Build).** Domain footprint is computed after research, before delegating to pe-meta-builder. Multi-domain designs block (or require `bundle=accept`) so a new artifact doesn't silently establish cross-domain coupling without consent.
3. **Consent token.** `bundle=accept` is the only valid token (closed set, case-sensitive). Recorded on the first-line `Resolved invocation:` log as `bundle=accepted-bundle`.
4. **Phase 0a CF-05 does NOT apply at this layer.** The `<artifact-type>` first positional argument carries explicit type intent; CF-05 (path/prefix mismatch on existing files) is enforced ONLY by per-artifact prompts.
5. **`--skip domain-coherence` rejected.** Phase 0b is not skippable; bypass multi-domain gating only via `bundle=accept`.

## Design/Review Parity Contract

This creation-side orchestrator and the review-side [`pe-meta-review`](pe-meta-review.prompt.md) MUST resolve the same artifact to the same verdict on the six shared properties. Each property is constructed-toward here and verified there:

| Property | Design obligation (construct toward) |
|---|---|
| **Clarity** | Every produced section is unambiguous; rule-bearing sections use N-1 `**Rule**:`/`**Rationale**:`/`**Example**:` blocks where the type requires |
| **Non-redundancy** | Shared machinery (evidence-coverage, plan-file contract, iteration budget, option matrix) is **referenced, never inlined** |
| **Non-contradiction** | The artifact does not contradict the vision, its governing instruction file, or the option matrix |
| **Completeness** | All required metadata + required sections for the type are present (per `05.08-pe-meta-type-checklists.md`) |
| **Prioritization** | Boundaries are ordered Always/Ask/Never; dimension coverage records the highest-blast-radius dimensions first |
| **Actionability** | Every boundary and rule is executable, not aspirational |

**Reciprocal obligation.** As its final step (Phase 8), this prompt runs pe-meta-review's `--dim full` **applicability-scoped** over the just-created artifact. A parity failure blocks completion.

## Autonomy & Classification

Each created artifact is classified before approval, mirroring pe-meta-review's Classification Protocol:

- **Tier 1 (auto-apply)** — low-blast-radius artifacts (new snippet, isolated context file with no inbound dependents) whose validation and coverage pass cleanly.
- **Tier 2 (apply + log)** — standard artifacts (a new agent/prompt with bounded dependents); applied, then recorded in [`05.04-meta-review-log.md`](../../../.copilot/context/00.00-prompt-engineering/05.04-meta-review-log.md).
- **Tier 3 (propose, await approval)** — high-blast-radius artifacts (new governance/context that many artifacts will depend on, or anything touching metadata contracts). Present the design plan and STOP for approval.

**Metadata guard.** Any change touching a canonical metadata contract is governed by `00.03-metadata-contracts.md` (single source of truth) and escalates to Tier 3.

## Model-routing seam

Research and design-plan derivation run on this prompt's reasoning-grade `model:`; delegated execution (`pe-meta-builder`) carries its own standard-grade `model:`. There is no mid-prompt model switching — the seam is the handoff boundary.

## Process

### Phase 0: Load PE-domain strategic context (NOT skippable)

1. **Load vision document** — `read_file` on the current vision document in `06.00-idea/self-updating-prompt-engineering/` (find `*-vision.v*.md` with highest version)
2. **Load 00.00-context-structure-index.md** — for Functional Categories and required categories
3. **Load strategic review criteria** — `read_file` on the `pe-strategic-review` files from `.copilot/context/00.00-prompt-engineering/` (see 00.00-context-structure-index.md → Functional Categories)
4. **Load dependency map** — `read_file` on the `dependency-tracking` files from `.copilot/context/00.00-prompt-engineering/`
5. **Load the dimension catalog & type checklists** — `read_file` on [`05.07-pe-meta-dimension-catalog.md`](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md) (which `--dim` groups exist) and [`05.08-pe-meta-type-checklists.md`](../../../.copilot/context/00.00-prompt-engineering/05.08-pe-meta-type-checklists.md) (required sections per type)
6. **Load dispatch table** — `read_file` on `.github/templates/00.00-prompt-engineering/artifact-type-dispatch.template.md`
7. **Determine artifact type** from the `<artifact-type>` argument

### Phase 1: Source-grounded research (pe-meta-researcher)

Delegate to `@pe-meta-researcher`. Research is **source-grounded**, not internal-only: the seed corpus is selected by `--source`/`--start`/`--end` (Phase 0a) and the researcher returns evidence anchored to real source material.

**Standard research** (pe-meta-researcher handles): use-case challenge (3–7 scenarios), pattern discovery from the seed corpus, tool/mode recommendations, scope-boundary definition — each finding carrying a `path:line` evidence anchor.

**Additional PE-strategic constraints** (pass through):
- "Align with vision rationales: [applicable list]"
- "Use Level 1.5 (category) references for any context-file references"
- "Meet the PE quality bar (exemplary) from `05.06-pe-strategic-review-criteria.md`"
- "Carry metadata contracts: goal, scope, boundaries, rationales, version"
- "Check the dependency map for overlap — avoid duplication"

If `--skip research` is set, an existing `--plan-file` baseline supplies the research findings (reconcile/trust); with no baseline, research is mandatory.

### Phase 2: Strategic alignment + dimension/quality parity

Before building, verify the research output against both strategy and the construction-quality dimensions:

1. **Vision alignment** — does the artifact serve a vision rationale? If none, question whether it belongs in the pe-meta tier.
2. **Category coverage** — for a context file, which Functional Category; does it need a new one?
3. **Ecosystem fit** — does it overlap an existing artifact? Would extending be better than creating?
4. **Blast radius** — how many artifacts will depend on this? Higher dependency ⇒ higher quality bar ⇒ higher autonomy tier.
5. **Dimension/quality parity** — for each in-scope `--dim` group (default `full`), confirm the planned artifact will satisfy the dimension's sub-checks per `05.08-pe-meta-type-checklists.md`, and that the six guidance-quality properties (Parity Contract) are constructible. Record each as a planned **coverage unit** (artifact × applicable-dimension).

If strategic or dimensional gaps are found: refine requirements and re-run research, or escalate.

### Phase 3: Build (pe-meta-builder)

Delegate to `@pe-meta-builder` with constraints:

- **References**: context-file references MUST use the Level 1.5 (category) pattern: `"Load the \`category-name\` files from .copilot/context/00.00-prompt-engineering/ (see 00.00-context-structure-index.md → Functional Categories)"`
- **Metadata**: MUST include `goal:`, `scope: {covers, excludes}`, `boundaries:`, `rationales:`, `version:`, `last_updated:`
- **N-1 separation**: if the type requires it (N-1 adoption table in `05.06`), rule-bearing sections MUST use `**Rule**:` / `**Rationale**:` / `**Example**:` blocks
- **Quality bar**: Boundaries ≥5/2/3 (Always/Ask/Never), test scenarios covering happy+error+edge, response management with recovery actions
- **Naming**: MUST use the `pe-` namespace prefix

### Phase 4: Double validation + evidence-and-coverage recording

1. **Structural validation** — delegate to `@pe-meta-validator` for structural checks.
2. **Strategic validation** — Vision alignment ✅/❌, Category references ✅/❌, PE quality bar ✅/❌, N-1 separation ✅/❌/N/A, Self-update readiness ✅/❌.
3. **Coverage recording** — for every planned coverage unit (artifact × applicable-dimension) record an outcome with a **non-empty `evidence_ref`** (verbatim `path:line` + backtick quote), `bodies_read: true`, and a graded verdict (verified / pass-weak / partial / fail / `never`). The `pu-evidence=<evidenced>/<applicable>` ratio is a **hard-fail** in both modes. This machinery is specified in [`pe-meta-evidence-coverage.md`](../../prompt-snippets/pe-meta-evidence-coverage.md) (single source of truth — referenced, not inlined). Annotate each delegated check with `Evaluation: <prose|hook|skill|mcp>`.

If validation fails: hand back to `@pe-meta-builder` (max 3 iterations).

### Phase 4.5: Strategic triggers (optional)

If the build surfaces a cross-artifact concern (new category needed, metadata-contract change, family-wide pattern), escalate to the user before applying — these are Tier 3 by definition.

### Phase 5: Approval & autonomy classification

Classify the artifact (Tier 1/2/3 per **Autonomy & Classification**). Tier 3 stops here and presents the design plan for approval; any approval override is logged.

### Phase 6: Apply + plan materialization (every mutating run)

1. **Materialize the design plan** — independent of `--mode`, every mutating run writes the design plan to `--plan-file` (or its auto-named default per [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md)). `--skip plan-emission` is REJECTED.
2. **Execute the build** (when `--mode apply`) via `@pe-meta-builder`; `--mode plan` STOPS after materializing the plan.
3. **Append the outcome-log** (JSONL) for the run: the coverage units with their `dim_evidence[] {dim, status, evidence_ref}` and `bodies_read: true`.
4. **Iteration-budget spillover** — if the build exceeds the iteration budget, spill remaining work to a follow-up plan per [pe-meta-iteration-budget.md](../../prompt-snippets/pe-meta-iteration-budget.md).
5. **Post-creation maintenance** — update `00.00-context-structure-index.md` (new context file → File Index + category), `05.01-artifact-dependency-map.md` (new dependencies), and `05.03-pe-workflow-entry-points.md` (new entry point).

### Phase 7d: Independent coverage audit

Delegate to `@pe-meta-validator` in **Coverage Audit** mode: Layer A runs `pe-check-evidence-anchors.ps1 -RunId <id> -AsJson` deterministically over every coverage unit; Layer B re-executes a sampled subset of sub-checks adversarially. The orchestrator writes the outcome-log; the validator audits it — divergence hard-fails (independent-actor rule).

### Phase 8: Report + linters (NOT skippable)

1. **First-line-log linter** — verify the Phase 0a `Resolved invocation:` line is present and well-formed.
2. **Full-coverage linter** — verify every applicable coverage unit carries a non-empty evidence anchor (no fabricated quotes; `pu-evidence` complete).
3. **Reciprocal parity step** — run pe-meta-review's `--dim full` applicability-scoped over the created artifact; a parity failure blocks completion.
4. **Report** strategic-compliance + coverage summary and record a meta-review-log entry for Tier 2/3 runs.

## Context Management

This creation-side orchestrator now runs a full multi-phase pipeline (Phase 0a → Phase 8) at parity with [`pe-meta-review`](pe-meta-review.prompt.md) and delegates structural work to the pe-meta-* agent tier. If context exceeds 8,000 tokens before a handoff, summarize prior phases to essential findings (evidence anchors + verdicts) only — never drop a coverage unit's `evidence_ref`.

## Handoff Data Contracts

| Transition | Strategy | Include | Exclude | Max tokens |
|---|---|---|---|---|
| **Orchestrator → pe-meta-researcher** | send: true | User request, artifact type, seed-corpus selection (`--source`/window), PE-strategic constraints (vision rationales, L1.5 refs, quality bar, N-1), dispatch table row | Vision document full text, STRUCTURE-README full text | ≤2,000 |
| **pe-meta-researcher → Orchestrator** | Structured report | Research report: name, type, mode, tools, scope, requirements — each with a `path:line` evidence anchor | Raw search results, pattern analysis, full file reads | ≤1,500 |
| **Orchestrator → pe-meta-designer** | send: true | Research report + strategic-alignment questions (vision rationale, category, blast radius) | Raw research | ≤1,000 |
| **Orchestrator → pe-meta-builder** | send: true | Approved design plan + PE constraints (L1.5 mandatory, metadata contracts, N-1, quality bar ≥5/2/3) | Research details, alternatives considered | ≤1,500 |
| **Orchestrator → pe-meta-validator (structural)** | send: true | File path + "validate this file" | Builder reasoning, research | ≤200 |
| **Orchestrator → pe-meta-validator (coverage audit)** | send: true | File path + run-id + "coverage audit" + coverage-unit list | Structural validation details | ≤500 |

## Response Management

- **Not a PE artifact request** → "This sounds like a domain artifact. Use `/pe-con-design` instead — it handles domain expertise activation."
- **Overlaps with existing artifact** → Present the overlap, ask user: extend existing or create new?
- **`--mode plan`** → Materialize and present the design plan, then STOP (no build).
- **Unknown flag** → Reject via CF-05 quoting the canonical enumeration; do not proceed.
- **All validations pass** → Report success with strategic-compliance + coverage summary
- **Strategic validation fails but structural passes** → "The artifact is structurally sound but doesn't align with [specific vision rationale]. Here's what needs to change: [...]"

## Embedded Test Scenarios

| # | Scenario | Expected Behavior |
|---|---|---|
| 1 | Design PE context file (happy path) | Phase 0a parses + emits `Resolved invocation:` → Phase 0/1/2 → Phase 3 build with L1.5 refs → Phase 4 double-validate + coverage record → Phase 6 plan materialized + maintenance → Phase 7d audit → Phase 8 linters + reciprocal parity pass |
| 2 | Overlap with existing artifact | Phase 2 detects overlap via dependency map → presents extend-vs-create → waits for user decision |
| 3 | Domain artifact request (redirect) | Detects non-PE intent → "Use /pe-con-design instead" → STOP |
| 4 | Strategic validation fails after build | Phase 4 finds missing N-1 separation → hands back to `@pe-meta-builder` with fix spec → re-validates (max 3 iterations) |
| 5 | `--mode plan` | Phase 0a logs `mode=plan` → research + design plan materialized → STOPS before build |
| 6 | Unknown flag (e.g. `--deps full`) | Phase 0a rejects via CF-05 quoting the canonical enumeration (Design does not accept `--deps`) |
| 7 | `--dim` omitted | Phase 0a defaults to `--dim full`; every applicable construction-quality dimension is a coverage unit |
| 8 | Fabricated coverage evidence | Phase 8 full-coverage linter + Phase 7d Layer A hook catch the empty/fabricated `evidence_ref` → run hard-fails |
| 9 | Multi-domain design without consent | Phase 0b blocks; resolved only by `bundle=accept` |
| 10 | `--mode apply` over a `--plan-file` baseline | Reconcile mode: human-authored design decisions preserved (escalate-not-overwrite) |

<!--
prompt_metadata:
  version: "3.0.0"
  last_updated: "2026-06-24"
-->
