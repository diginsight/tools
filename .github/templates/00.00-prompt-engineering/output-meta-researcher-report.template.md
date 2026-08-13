---
description: "Structured report format for meta-researcher analysis output"
---

<!-- Category: output -->
<!-- Audience: agent (meta-designer receives this report) -->

# Meta-Researcher Report Template

## Report Structure

```markdown
## PE Technology Research Report

**Source:** [URL / description / attached file]
**Category:** [Editor & tooling / AI models / Interaction protocols / Best practices / Platform changes / Multiple]
**Date:** [source date]
**Researcher:** meta-researcher
**Research sources used:** [list: context files read, 05.02 articles read, external URLs fetched (if any), internet searches performed (if any)]

### Key Changes
1. [Change description — what's new or different]
   - **Evidence:** [direct quote or excerpt from source material]
   - **Verified against:** [which internal context file or 05.02 article confirms/contradicts this]
2. ...

### PE Improvement Opportunities

For each opportunity, provide enough rationale and direction that the designer can proceed without re-researching:

#### Opportunity 1: [descriptive title]

| Field | Value |
|---|---|
| **Source change** | [what changed in the external source] |
| **Affected artifact types** | [context/instruction/agent/prompt/skill/template/hook/prompt-snippet] |
| **Quality dimension** | [Robustness/Effectiveness/Token efficiency/Time efficiency] |
| **Current state** | [what the PE artifacts currently do/say about this topic — reference specific context file or artifact] |
| **Gap or improvement** | [what's missing or could be better] |
| **Recommended direction** | [what the designer should aim for — the *what* and *why*, not the exact implementation] |
| **Evidence** | [direct quote, excerpt, or link from source proving this change is warranted] |
| **Confidence** | [High — official docs / Medium — verified community / Low — unverified] |

#### Opportunity N: ...

### PE Structure Assessment

#### Structural Inventory

| Location | Expected | Found | Mismatches |
|---|---|---|---|
| Context files (`.copilot/context/00.00-prompt-engineering/`) | [N per STRUCTURE-README] | [N on disk] | [list any missing or extra] |
| Agents (`.github/agents/00.00-prompt-engineering/`) | [N] | [N] | [list] |
| Prompts (`.github/prompts/00.00-prompt-engineering/`) | [N] | [N] | [list] |
| Instructions (`.github/instructions/`) | [N] | [N] | [list] |
| Skills (`.github/skills/`) | [N] | [N] | [list] |
| Templates (`.github/templates/00.00-prompt-engineering/`) | [N] | [N] | [list] |
| Hooks (`.github/hooks/`) | [N] | [N] | [list] |
| Snippets (`.github/prompt-snippets/`) | [N] | [N] | [list] |

#### Symmetry Check (builder → validator pairs)

| Artifact type | Builder | Validator | Symmetric? |
|---|---|---|---|
| [type] | [exists/missing] | [exists/missing] | ✅/❌ |

#### Orphan Detection

| File | Type | Referenced by | Status |
|---|---|---|---|
| [filename] | [agent/prompt/etc] | [none found] | ⚠️ Orphan |

#### Context Coverage Gaps

| Rule/concept referenced by downstream artifacts | Canonical context file | Status |
|---|---|---|
| [rule] | [file or MISSING] | ✅/❌ |

#### Structural Recommendations

| # | Finding | Impact | Recommendation |
|---|---|---|---|
| 1 | [what's suboptimal] | [reliability/effectiveness/efficiency] | [what to change and why] |

### Affected Artifacts (from dependency map)
| File | Why | Impact |
|---|---|---|
| `[path]` | [covers changed concept — with specific section/rule reference] | High/Med/Low |

### Internet Findings Validation

| # | Finding | Source | Source type | Validates against internal guidance? | Improves reliability/effectiveness/efficiency? | Verdict |
|---|---|---|---|---|---|---|
| 1 | [finding] | [URL] | [Official docs/Spec/Research/Blog/Tutorial] | [Yes — aligns with X / No — contradicts Y / New — not covered] | [Yes — how / No — why not] | ✅ Integrate / ⚠️ Integrate with caution / ❌ Exclude |

**Excluded findings** (with rationale):
- [finding]: excluded because [unverifiable / contradicts established principle X / no measurable improvement / potentially misleading because Y]

### Prioritized Recommendations
1. [Action item with rationale — clear enough that the designer understands the goal without reading your sources]
```

**Self-check before submitting:** For each opportunity, verify the designer can answer "what should change and why?" from your report alone — without reading any source material, 05.02 articles, or external URLs.

## Report Validation Checklist

Before delivering the report, every check must pass:

| # | Check | Criteria |
|---|---|---|
| 1 | **Step alignment** | Report addresses the original research goal. No scope drift. |
| 2 | **Completeness** | Every key change has evidence, affected artifacts, and a quality dimension. |
| 3 | **Self-containment** | Designer can proceed without reading your sources. All quotes/excerpts embedded. |
| 4 | **Actionability** | Every opportunity has a clear recommended direction (not just "investigate further"). |
| 5 | **Consistency** | Recommendations don't contradict each other or existing PE principles. |
| 6 | **Overall goal alignment** | Recommendations collectively improve robustness, effectiveness, and efficiency. |
| 7 | **No capability regression** | No recommendation risks breaking existing working capabilities. Flagged if so. |
| 8 | **Efficiency** | Recommendations prioritized by impact. No duplication of existing guidance. |
| 9 | **Security and guardrails** | Appropriate boundaries included. Existing guardrails not weakened. |
| 10 | **Context rot prevention** | No recommendation causes context window bloat in downstream agents. |
| 11 | **Internet source validation** | Every internet finding critically evaluated. Excluded findings documented. |

## Research Guidance — Step 3 Checklists

### External Validation of Rules

- For each CRITICAL and HIGH rule in `01.07-critical-rules-priority-matrix.md`, search external authoritative sources for evidence that supports, contradicts, or refines the rule
- Flag rules with no external supporting evidence as "internally-derived — needs external validation"
- Flag rules contradicted by external evidence as "externally-challenged — needs review"
- Include supporting/contradicting evidence as direct quotes in the report

### PE Structure Optimization

- Does every artifact type have symmetric builder/validator coverage?
- Are there artifact types without dedicated validators?
- Do orchestration prompts route to all artifact types?
- Is the dependency map current and covers all 9 artifact locations?
- Are handoff chains complete — can every builder delegate to a validator?
- Are there skills, hooks, or prompt-snippets that should exist but don't?
- Is token budget allocation proportional to artifact importance?

### Context Coverage Assessment

- Do context files cover ALL rules that instruction files, agents, and prompts reference?
- Are there context files that no artifact references (orphaned knowledge)?
- Do instruction files add enforcement value beyond context files?
- Are there rules embedded inline in agents/prompts that should be centralized?

### Structural Inventory Procedure

1. List all files in each of the 9 artifact locations via `list_dir`
2. For each agent: verify a matching validator exists (builder → validator symmetry)
3. For each agent: verify at least one prompt or another agent references it — flag orphans
4. For each context file listed in STRUCTURE-README: verify the file exists on disk — flag mismatches
5. Check naming conventions: kebab-case, correct extensions, correct locations

### Structural Challenge Questions

Critically evaluate whether a different approach would yield better reliability, effectiveness, or efficiency:

- **Information organization**: Would merging, splitting, or regrouping context files reduce handoff losses?
- **Task decomposition**: Are agent responsibilities at the right granularity?
- **Role boundaries**: Are there overlapping or missing agent responsibilities?
- **Handoff patterns**: Would fewer, richer handoffs outperform many thin ones?
- **Knowledge distribution**: Is knowledge duplicated across agents when it should be centralized?

For each structural alternative, evaluate against: **Reliability** (reduced variance?), **Effectiveness** (improved accuracy?), **Efficiency** (reduced tokens/time?). Only recommend when the alternative outperforms on at least one criterion without degrading the others.

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "meta-researcher"
  changelog: "output-meta-researcher-report.template.changelog.md"
---
-->
