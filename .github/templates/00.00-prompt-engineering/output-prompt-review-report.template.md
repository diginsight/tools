---
description: "Structured review report format for prompt and agent review"
---

# Prompt Validation Report Template

Output format template for Phase 5 (Final Report) of the `prompt-review` orchestrator.

---

## Final Validation Report

```markdown
# Prompt Validation Report: [prompt-name]

**Date**: [ISO 8601]
**Status**: [✅ PASSED / ⚠️ PASSED WITH WARNINGS / ❌ FAILED]

---

## Quick Summary

| Check | Status |
|-------|--------|
| Tool Alignment | ✅/❌ |
| Goal & Role | ✅/⚠️/❌ |
| Structure | ✅/⚠️/❌ |
| Boundaries | ✅/⚠️/❌ |
| Conventions | ✅/⚠️/❌ |
| Production Readiness | ✅/⚠️/❌ |

**Quality Score**: [N]/10

---

## Prompt Configuration

- **File**: `.github/prompts/[prompt-name].prompt.md`
- **Mode**: [plan/agent]
- **Tools**: [list]
- **Handoffs**: [list or none]
- **Complexity**: [Simple/Moderate/Complex]

---

## Validation Details

### Tool Alignment (CRITICAL)
[Detailed alignment check results]

### Goal & Role Validation
- **Use cases tested**: [N]
- **Goal gaps found**: [None / List with severity]
- **Role assessment**: [authority: ✅/❌, expertise: ✅/❌, specificity: ✅/❌]

### Boundary Analysis
- Always Do: [N] items [✅/❌]
- Ask First: [N] items [✅/❌]
- Never Do: [N] items [✅/❌]

### Production Readiness
| Requirement | Status |
|---|---|
| Response Management | ✅/❌ |
| Error Recovery | ✅/❌ |
| Embedded Tests | ✅/❌ ([N] found, [N] required) |
| Token Budget | ✅/❌ ([N] tokens, limit: [N]) |
| Context Rot Prevention | ✅/❌/N/A |
| Template Externalization | ✅/❌ |

---

## Issues and Resolution

| # | Issue | Severity | Status |
|---|-------|----------|--------|
| 1 | [description] | [level] | ✅ Fixed / ⚠️ Open |
...

---

## Certification

**Validation Status**: [CERTIFIED / NOT CERTIFIED]
**Validated By**: prompt-review orchestrator
**Date**: [ISO 8601]
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-review"
    - "prompt-review"
  changelog: "output-prompt-review-report.template.changelog.md"
---
-->
