---
name: pe-artifact-coherence-check
description: >
  Cross-artifact consistency validation for prompt engineering systems:
  rule consistency, reference integrity, handoff chain validation,
  redundancy detection, and token budget auditing.
  Use when reviewing PE artifact coherence, detecting contradictions
  between artifacts, or auditing system-wide consistency.
---

# Artifact Coherence Check Skill

## Purpose

Validate that prompt engineering artifacts cooperate correctly without contradictions, broken references, or redundant content. Provides systematic checks that can be invoked by `meta-validator` (Ecosystem Audit mode), validation prompts, or directly by users.

## When to Use

Activate this skill when:
- **System audit**: "Check if PE artifacts are consistent"
- **After changes**: "Verify no contradictions after updating context files"
- **Dependency check**: "Are all cross-references valid?"
- **Redundancy scan**: "Find duplicated content across PE artifacts"
- **Token audit**: "Check which files exceed their budget"
- **Handoff validation**: "Do all handoff targets exist?"

Do NOT use this skill for:
- Single-file prompt/agent validation (use `pe-prompt-engineering-validation` skill)
- Article content review (use `article-review` skill)
- Creating or modifying PE artifacts (use builder/updater agents)

## Quick Reference

### Coherence Check Sequence

```
1. Reference Integrity  → all links resolve
2. Rule Consistency     → no contradictions between files
3. Handoff Chain        → all targets exist
4. Redundancy Scan      → no duplicated canonical content
5. Token Budget         → all files within limits
6. Dependency Map       → map matches actual state
```

### Severity Classification

| Severity | Examples | Action |
|---|---|---|
| **CRITICAL** | Contradictory rules, broken handoff chain, plan+write tools | BLOCK — must fix immediately |
| **HIGH** | Broken reference links, missing required sections | Fix before next use |
| **MEDIUM** | Token budget exceeded, minor redundancy | Fix when convenient |
| **LOW** | Stale version history, cosmetic inconsistencies | Note for future |

## Detailed Workflows

### Workflow 1: Reference Integrity Check

Verify all cross-references between PE artifacts resolve to existing files.

**Process:**
1. Scan target file(s) for markdown links and `📖` references
2. Extract all referenced file paths
3. Verify each path resolves using `file_search` or `list_dir`
4. Report broken references with file, line number, and target path
5. Scan for slash-command references (`/[a-z]+-[a-z-]+` patterns in body text)
6. For each slash-command, verify a `.prompt.md` file exists with matching `name:` YAML field
7. Exclude known built-in commands: `/create-prompt`, `/create-agent`, `/create-instruction`, `/create-skill`, `/create-hook`
8. Report broken slash-command references with file, line number, and referenced name

**Check template:** [reference-integrity.template.md](./templates/reference-integrity.template.md)

**Common reference patterns to check:**
- `📖 Complete guidance:` links to context folders
- `📖 See:` links to specific context files
- Template references: `.github/templates/*.template.md`
- Skill references: `pe-prompt-engineering-validation` skill name
- Handoff targets: `agent:` values in YAML `handoffs:` arrays
- Slash-command references: `/pe-gra-*`, `/pe-con-*`, `/pe-meta-*` must match `name:` in a `.prompt.md` file

### Workflow 2: Rule Consistency Check

Verify rules don't contradict across artifact layers.

**Process:**
1. Identify canonical rules from context files (Layer 1)
2. Check instruction files (Layer 2) — do they agree or contradict?
3. Check agents (Layer 3) — do inline rules match context + instructions?
4. Check prompts (Layer 4) — do workflow rules align?

**Key rules to cross-check:**

| Rule | Canonical Source | Check Against |
|---|---|---|
| Tool alignment (plan=read-only) | `01.04-tool-composition-guide.md` | All agents, PE-validation skill |
| Template-first (>10 lines) | `01.01-context-engineering-principles.md` (P8) | `pe-prompts.instructions.md`, `pe-agents.instructions.md` |
| Three-tier boundaries (min items) | `01.06-system-parameters.md` | All agents |
| Token budgets | `01.06-system-parameters.md` | All instruction files |
| Validation caching (7-day) | `04.01-validation-caching-pattern.md` | Validation prompts |
| Handoff `send:` strategy | `02.01-handoffs-pattern.md` | All agents with handoffs |

**Contradiction detection:**
- Same concept with different thresholds (e.g., "5 lines" vs "10 lines")
- Different tool lists for same agent mode
- Conflicting boundary rules (one file says "Always Do", another says "Ask First")

### Workflow 3: Handoff Chain Validation

Verify all agent handoff chains are complete and valid.

**Process:**
1. Extract `handoffs:` from all agent YAML frontmatter
2. For each handoff target, verify the agent file exists
3. Verify the target agent has appropriate mode and tools
4. Check for orphaned agents (exist but never referenced as handoff target)
5. Check for circular handoffs (A→B→A without termination condition)

**Expected chains:**
```
prompt-researcher → prompt-builder → prompt-validator ↔ prompt-builder
agent-researcher → agent-builder → agent-validator ↔ agent-builder
context-builder → prompt-validator
instruction-builder → prompt-validator
meta-validator → meta-optimizer → prompt-validator
```

### Workflow 4: Redundancy Scan

Detect duplicated content that violates single-source-of-truth principle.

**Process:**
1. For each canonical rule (from context files), search for inline copies in agents/instructions
2. Flag any instruction/agent embedding >5 lines of content available in context files
3. Check for identical tables, checklists, or code examples across files
4. Calculate estimated token waste from redundancy

**Search patterns:**

| Content | Canonical Source | Search Agents/Instructions For |
|---|---|---|
| Tool alignment rules | `01.04-tool-composition-guide.md` | "plan.*read-only", "write tools", "tool clash" |
| Boundary pattern | `01.01-context-engineering-principles.md` | "Always Do.*Ask First.*Never Do" |
| Template-first rules | `01.01-context-engineering-principles.md` | "10 lines", "externalize", "template-first" |
| Reliability checksum | `02.01-handoffs-pattern.md` | "Goal Preservation", "Scope Boundaries" |

### Workflow 5: Token Budget Audit

Verify all PE artifacts are within their token budgets.

**Process:**
1. Count lines for each artifact (lines × 6 ≈ tokens estimate)
2. Compare against budgets from `01.06-system-parameters.md`
3. Flag files exceeding WARNING threshold
4. Recommend split or compression for files exceeding CRITICAL threshold

**📖 Budget thresholds (per-type limits, warning/critical levels):** `01.06-system-parameters.md` → Token Budgets

### Workflow 6: Dependency Map Verification

Verify the dependency map matches the actual artifact state.

**Process:**
1. Load the `dependency-tracking` file from `.copilot/context/00.00-prompt-engineering/` (see 00.00-context-structure-index.md → Functional Categories)
2. Compare listed files against actual files on disk
3. Verify reference counts by spot-checking 3–5 high-impact entries
4. Flag any files present on disk but missing from the map

## Templates

- **[Reference Integrity](./templates/reference-integrity.template.md)** — Structured format for reporting broken references
- **[Coherence Report](./templates/coherence-report.template.md)** — Summary template for full coherence audit results

## Common Issues

### Issue: Contradictory Token Budgets

**Symptom**: Different budget numbers for same artifact type across files
**Solution**: Update all references to match canonical source (`01.01-context-engineering-principles.md`)

### Issue: Orphaned Agents

**Symptom**: Agent file exists but no prompt or other agent references it as a handoff target
**Solution**: Either add the agent to an orchestration workflow or deprecate it

### Issue: Stale Dependency Map

**Symptom**: New artifacts exist but aren't listed in the `dependency-tracking` file (see 00.00-context-structure-index.md → Functional Categories)
**Solution**: Update the dependency map with new entries and reference counts

## Dimension Mapping

This skill's workflows map to the 35-dimension quality framework (see [`05.07-pe-meta-dimension-catalog.md`](../../../.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md)):

| Workflow | Dimensions Covered |
|---|---|
| Reference Integrity | `D2-references` |
| Rule Consistency | `D6-consistency`, `D17-cross-coherence` |
| Handoff Chain | `D5-boundaries`, `D24-handoff-efficiency` |
| Redundancy Scan | `D7-non-redundancy` |
| Token Budget | `D3-token-budget`, `D20-token-chain` |
| Dependency Map | `D17-cross-coherence`, `D22-context-optimization` |

Use `--dim <D#-readable-id>` with `/pe-meta-review` to invoke specific dimension checks selectively (bare `D#` is also accepted as input).
Use `--with-deps` to trace cross-artifact consistency chains.

## Resources

- **📖 Dependency map:** see `dependency-tracking` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)
- **📖 Lifecycle management:** `.copilot/context/00.00-prompt-engineering/05.02-artifact-lifecycle-management.md`
- **📖 Entry points:** `.copilot/context/00.00-prompt-engineering/05.03-pe-workflow-entry-points.md`
- **📖 Context engineering principles:** `.copilot/context/00.00-prompt-engineering/01.01-context-engineering-principles.md`
- **📖 Dimension catalog:** `.copilot/context/00.00-prompt-engineering/05.07-pe-meta-dimension-catalog.md`
- **📖 Type-specific checklists:** `.copilot/context/00.00-prompt-engineering/05.08-pe-meta-type-checklists.md`
