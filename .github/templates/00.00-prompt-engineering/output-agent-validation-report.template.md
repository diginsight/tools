---
description: "Structured validation report format for agent file review"
---

# Agent Validation Report

**Agent**: [agent-name] | **Date**: [ISO 8601] | **Validator**: agent-validator

## Quick Summary

| Check | Status |
|-------|--------|
| Structure | ✅/❌ |
| Tool Alignment | ✅/❌ |
| Tool Count | ✅/⚠️/❌ |
| Boundaries | ✅/❌ |
| Process | ✅/❌ |
| Cross-Artifact | ✅/❌ |
| **Overall** | **[PASS/WARN/FAIL]** |

**Quality Score**: [N]/10 | **Issues Found**: [N] (Critical: [N], High: [N], Medium: [N], Low: [N])

## Phase Reports

| Element | Status | Issue |
|---------|--------|-------|
| description | ✅/❌ | [issue if any] |
| agent mode | ✅/❌ | [issue if any] |
| tools array | ✅/❌ | [issue if any] |
| tool count | ✅/⚠️/❌ | [N] tools |
| handoffs | ✅/❌/N/A | [issue if any] |

**Structure Score**: [N]/5

### Tool Alignment

| Tool | Type | Allowed | Status |
|------|------|---------|--------|
| [tool-1] | read | ✅ | ✅ |
| [tool-2] | write | ❌ | ❌ VIOLATION |

**Alignment Status**: [✅ PASS / ❌ FAIL] | **Violations**: [list]

### Boundary Analysis

| Tier | Items | Min Required | Status |
|------|-------|--------------|--------|
| Always Do | [N] | 3 | ✅/❌ |
| Ask First | [N] | 1 | ✅/❌ |
| Never Do | [N] | 2 | ✅/❌ |

**Coverage**: Tools [N]/[total], Responsibilities [N]/[total] | **Gaps**: [list or "None"] | **Score**: [N]/10

### Quality Assessment

| Dimension | Weight | Score | Weighted |
|-----------|--------|-------|----------|
| Structure | 20% | [0-10] | [result] |
| Tool Alignment | 30% | [0-10] | [result] |
| Tool Count | 15% | [0-10] | [result] |
| Boundaries | 20% | [0-10] | [result] |
| Process Clarity | 15% | [0-10] | [result] |
| **Total** | 100% | | **[N]/10** |

**Thresholds**: 9-10 ✅ Excellent | 7-8 🟢 Good | 5-6 ⚠️ Acceptable | 3-4 🟠 Poor | 0-2 ❌ Critical

### Cross-Artifact Alignment

| Check | Status | Details |
|-------|--------|---------|
| Instruction rule coverage | ✅/❌ | [N]/[total] CRITICAL/HIGH rules covered |
| Context references valid | ✅/❌ | [N]/[total] references resolve |
| Handoff targets exist | ✅/❌ | [list of verified/missing targets] |
| Prompt alignment | ✅/❌/N/A | [orchestrator prompt status] |

## Issues List

### Issue [N]: [Title]

**Severity**: [CRITICAL/HIGH/MEDIUM/LOW] | **Category**: [Structure/Alignment/Boundaries/Quality] | **Effort**: [Low/Medium/High]
**Current State**: [what's wrong]
**Required State**: [what it should be]
**Suggested Fix**: [specific fix]

## Recommendations

1. [Priority 1 recommendation]
2. [Priority 2 recommendation]

## Handoff

**Validation Status**: [PASS/FAIL] | **Ready for Use**: [Yes/No] | **Handoff to Builder**: [Yes (with issues) / No (clean)]

---

# Quick Validation: [agent-name]

- Structure: ✅/❌ | Tool Alignment: ✅/❌ | Tool Count: [N] ✅/⚠️/❌ | Boundaries: ✅/❌
- **Result**: [PASS/FAIL]
- **Critical Issues**: [None / List]

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-validator"
  changelog: "output-agent-validation-report.template.changelog.md"
---
-->
