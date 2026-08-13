---
title: "Per-type pe-meta prompt template"
description: "Skeleton for pe-meta-{type}-{review|create-update} prompts — routes every per-type entry path through the assess/evidence-coverage technique module so a direct per-type call reaches the same evidence depth as the /pe-meta-review orchestrator"
domain: "prompt-engineering"
---

# Per-type pe-meta prompt template

> **Audience**: `pe-meta-builder` and per-type prompt authors. Scaffold for the per-type meta prompts (`pe-meta-{type}-review`, `pe-meta-{type}-create-update`). The design family reaches this depth indirectly through its review-parity gate — author it to call the review path, not to re-inline the module.
>
> Replace every `[placeholder]`. Keep the Phase 0a/0b gates and the technique-module mechanics as **references** — never inline them.

## Prompt frontmatter (fill per type)

```yaml
name: pe-meta-[type]-[review|create-update]
description: "[one sentence]"
agent: agent
model: claude-opus-4.6
tools: [semantic_search, read_file, file_search, grep_search, list_dir, replace_string_in_file, create_file]
handoffs:
  - {label: "Validate", agent: pe-meta-validator, send: true}
argument-hint: '[<file-path>] [--mode plan|apply] [--dim <group|D#|full>] [--deps none|direct|full] [--scope <type>] [--skip research|external]'
goal: "[shared quality objective + scope intent, applicability-scoped]"
```

## Phase 0a CF-05 + Phase 0b — Invocation gates

Enforce the **Phase 0a CF-05 artifact-type/path consistency check** AND the **Phase 0b domain coherence gate** per [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md). Reference, do NOT inline. CF-05 expected root for this type: `[.github/prompts/ | .github/agents/ | ...]`.

## Type dispatch

1. Load the `[type]` checklist from [`05.08-pe-meta-type-checklists.md`](../../../.copilot/context/00.00-prompt-engineering/05.08-pe-meta-type-checklists.md).
2. Resolve the applicable-dimension subset from [`05.07-pe-meta-dimension-catalog.md`](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md) (DSC-1: when a dimension's applicability is ambiguous, treat it as applicable).

## Assess-phase technique module (MANDATORY — entry-point depth parity)

Include the first **technique module** so a direct per-type call carries the orchestrator's evidence depth:

```text
#file:.github/prompt-snippets/pe-meta-evidence-coverage.md
```

This binds the module's **invocation contract**: emit `dim_evidence[]` (one `{dim, status, evidence_ref}` per applicable dimension — **passes included**, with a non-empty anchored `evidence_ref`), the three markers below, and hand the outcome log to the independent second actor before any clean health score.

## Process

1. Parse `--dim`, `--deps`, `--scope`, and `--skip`.
2. Run the type dispatch (above).
3. `[type-specific checks — e.g. argument-hint example present, phases numbered and ordered]`.
4. Run selected dimensions, recording `dim_evidence[]` per the technique module.
5. **Independent Coverage Audit** — hand the outcome log to `@pe-meta-validator` (Coverage Audit mode) to re-derive `pu-evidence`/`subcheck-coverage`/`shallow-sweep` before any clean health score. Divergence is a hard-fail (reconciled, NOT self-attested).
6. Emit the severity-ranked report.

## Output contract (first-line log + evidence-depth hard-fail)

Open the report with a first-line `Resolved invocation:` log carrying the existing markers PLUS the three evidence markers:

```text
Resolved invocation: --mode=<plan|apply> … | plan-file=<path-or-none> | spillover=<path-or-none> | pu-evidence=<evidenced>/<applicable> | subcheck-coverage=<fully-covered-dims>/<applicable-dims> | shallow-sweep=<clean|suspected>
```

Hard-fail conditions (per the technique module, mode-independent):

- `pu-evidence` `<evidenced> < <applicable>` → hard-fail on BOTH `--mode plan` and `--mode apply`.
- any dimension `subcheck-coverage <evaluated>/<declared> < 1` → graded `partial`, BLOCKS a clean health score.
- `shallow-sweep=suspected` → BLOCKS clean until body-level `evidence_ref` or an explicit acknowledgment is recorded.
- Phase-5 audit verdict diverges from the self-computed markers → hard-fail.

Point the body output at the type's report template (`[output-{type}-review-report].template.md` — e.g. `output-prompt-review-report.template.md` for the prompt type).

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-06-24"
  created: "2026-06-24"
  consumers:
    - "pe-meta-builder"
    - "pe-meta-prompt-review"
    - "pe-meta-prompt-design"
  changes:
    - "v1.0.0: Initial creation — per-type pe-meta prompt scaffold routing through the assess/evidence-coverage technique module (depth-parity pilot WS-B)"
---
-->
