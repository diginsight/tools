---
description: "Report formats for meta-validator — design validation, implementation validation, ecosystem audit, dimension-based review, and adherence matrix"
---

# Meta-Validator Report Templates

## Mode 1: Design Validation Report

```markdown
## PE Design Validation Report

**Date:** [ISO 8601]
**Mode:** Design Validation (Pre-Implementation)
**Changes assessed:** [N] modifications, [N] new artifacts

### Capability Preservation (CRITICAL)

| # | Change | Target | Breaks Existing? | Details |
|---|---|---|---|---|
| 1 | [title] | `[path]` | ✅ Safe / ❌ BREAKS | [details] |

### Design Consistency

| Check | Status | Details |
|---|---|---|
| Internal consistency | ✅/❌ | [contradictions between proposed changes] |
| External consistency | ✅/❌ | [contradictions with unmodified artifacts] |
| Dependency order | ✅/❌ | [ordering issues] |
| Agent routing | ✅/❌ | [misassigned changes] |

### Design Completeness

| Check | Status | Details |
|---|---|---|
| Research coverage | ✅/❌ | [dropped findings] |
| New artifact specs | ✅/❌ | [incomplete specifications] |
| Cross-references | ✅/❌ | [missing dep map entries] |
| Rollback strategies | ✅/❌ | [missing rollback plans] |

### Goal Alignment

| Check | Status | Details |
|---|---|---|
| Quality dimension mapping | ✅/❌ | [unmapped changes] |
| Overall coherence | ✅/❌ | [misaligned changes] |
| Scope control | ✅/❌ | [scope creep items] |

### Efficiency

| Check | Status | Details |
|---|---|---|
| Token budget compliance | ✅/❌ | [over-budget artifacts] |
| Redundancy avoidance | ✅/❌ | [duplicated content] |
| Consolidation opportunities | ✅/❌ | [mergeable changes] |
| Context rot prevention | ✅/❌ | [bloat risks] |

### Security and Guardrails

| Check | Status | Details |
|---|---|---|
| Boundary tiers | ✅/❌ | [missing boundaries] |
| Iteration limits | ✅/❌ | [unbounded loops] |
| Tool alignment | ✅/❌ | [misaligned agents] |
| Guardrail preservation | ✅/❌ | [weakened guardrails] |

### Issues Found

| # | Severity | Change | Issue | Recommendation |
|---|---|---|---|---|
| 1 | [CRITICAL/HIGH/MEDIUM/LOW] | [change #] | [description] | [fix/accept/defer] |

### Verdict

**Overall:** [✅ SAFE TO PROCEED / ⚠️ PROCEED WITH FIXES / ❌ UNSAFE — REDESIGN REQUIRED]
**Blocking issues:** [N]
**Recommendation:** [Proceed / Fix and re-validate / Return to designer]
```

---

## Mode 2: Implementation Validation Report

```markdown
## PE Implementation Validation Report

**Date:** [ISO 8601]
**Mode:** Implementation Validation (Post-Implementation)
**Changes validated:** [N] modifications, [N] new artifacts

### System-Level Results

| Dimension | Status | Findings |
|---|---|---|
| Completeness | ✅/❌ | [All changes applied / Missing: ...] |
| Effectiveness | ✅/❌ | [Goals achieved / Issues: ...] |
| Reliability | ✅/❌ | [No contradictions / Found: ...] |
| Efficiency | ✅/❌ | [No redundancies / Found: ...] |
| Security & Guardrails | ✅/❌ | [Boundaries intact / Found: ...] |

### Cross-Artifact Consistency

| Check | Status | Details |
|---|---|---|
| Rule consistency | ✅/❌ | [description] |
| Cross-reference integrity | ✅/❌ | [description] |
| Handoff chain validity | ✅/❌ | [description] |
| Dependency map accuracy | ✅/❌ | [description] |

### Per-Artifact Validation

| # | File | Type | Validator | Status | Issues |
|---|---|---|---|---|---|
| 1 | `[path]` | [type] | [validator] | ✅/❌ | [description] |

### Issues Found

| # | Severity | File | Issue | Recommendation |
|---|---|---|---|---|
| 1 | [CRITICAL/HIGH/MEDIUM/LOW] | `[path]` | [description] | [fix suggestion] |

### Verdict

**Overall:** [✅ PASS / ⚠️ PASS WITH WARNINGS / ❌ FAIL]
**Blocking issues:** [N]
**Recommendation:** [Approve / Fix issues / Revert changes]
```

---

## Mode 3: Ecosystem Audit Report

```markdown
## PE Ecosystem Audit Report

**Date:** [ISO 8601]
**Mode:** Ecosystem Audit
**Scope:** [all / targeted categories]
**Artifacts audited:** [N]

### Summary

| Severity | Count | Status |
|---|---|---|
| CRITICAL | [N] | 🔴 Must fix |
| HIGH | [N] | 🟡 Should fix soon |
| MEDIUM | [N] | 🟢 Fix when convenient |
| LOW | [N] | ℹ️ Note for future |

### Findings

#### CRITICAL
1. [Finding with file path, issue, fix]

#### HIGH
1. [Finding...]

#### MEDIUM / LOW
[Grouped findings]

### Redundancy Summary
| Duplicated Content | Files | Tokens Wasted | Recommended Action |
|---|---|---|---|

### Token Budget Summary
| File | Current | Budget | Status |
|---|---|---|---|

### Recommendations
1. [Prioritized action items]

### Health Score
**Overall:** [N]/100
```

---

## Mode 4: Dimension-Based Review Report

Use when reviewing an artifact with `--dim` parameter. Structure from `output-dimension-report.template.md`.

```markdown
## Dimension Review: [artifact path]

**Date:** [ISO 8601]
**Mode:** Individual / Dependency-aware / Guidance-first
**Dimensions assessed:** [--dim value]
**Artifact type:** [type]
**Applicable dimensions:** [N] of 27 (per applicability matrix)

### Dimension Results

[For each assessed dimension, use the per-dimension format from output-dimension-report.template.md]

### Cross-Dependency Coherence (if --with-deps)

| Dependency | D17-cross-coherence Status | Contradictions found |
|---|---|---|
| [context file path] | ✅/❌ | [description or "None"] |

### Adherence Summary (if D16-adherence assessed)

| Rule source | Rules checked | Implemented | Gaps |
|---|---|---|---|
| [context file] | [N] | [N] | [N] |

### Consumer Impact (if context file target)

**Affected consumers:** [N] artifacts load this file
**Re-assessment recommended:** [Yes/No — Yes if rules were changed]
```

---

## Mode 5: System-Wide Apply-Mode Review Report (Phase A-F)

Use for `/pe-meta-review --mode apply`. Follows artifact-type ordering from vision v12.

```markdown
## System-Wide Apply-Mode Review Report

**Date:** [ISO 8601]
**Trigger:** [platform release / scheduled / manual]

### Phase A: Context Files (Foundation Layer)
| File | Dims assessed | ✅ | ⚠️ | ❌ | Blocking? |
|---|---|---|---|---|---|

### Phase B: Instruction Files
| File | Dims assessed | ✅ | ⚠️ | ❌ | R-S8 compliant? |
|---|---|---|---|---|---|

### Phase C: Agent Bodies (adherence against Phase A/B)
| File | Dims assessed | ✅ | ⚠️ | ❌ | D16-adherence |
|---|---|---|---|---|---|

### Phase D: Prompts (adherence against Phase A/B/C)
| File | Dims assessed | ✅ | ⚠️ | ❌ | D16-adherence |
|---|---|---|---|---|---|

### Phase E: Templates, Snippets, Hooks
| File | Dims assessed | ✅ | ⚠️ | ❌ |
|---|---|---|---|---|

### Phase F: Skills
| File | Dims assessed | ✅ | ⚠️ | ❌ |
|---|---|---|---|---|

### Phase Ordering Notes
[Any Phase A issues that blocked Phase C/D assessment]
```

<!--
---
template_metadata:
  version: "2.0.0"
  last_updated: "2026-05-15"
  created: "2026-03-20"
  consumers:
    - "pe-meta-validator"
    - "pe-meta review prompts"
  changelog: "output-meta-validator-reports.template.changelog.md"
---
-->
