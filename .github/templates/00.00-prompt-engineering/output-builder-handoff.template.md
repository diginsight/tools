---
description: "Structured builder output format for consistent validator handoff"
---

# Builder Handoff Report

## Operation

| Field | Value |
|---|---|
| **Action** | [Created / Updated / v2 Created] |
| **File path(s)** | [paths] |
| **Based on** | [researcher report reference or handoff source] |

## Requirements Traceability

| # | Requirement | Status | Implementation notes |
|---|---|---|---|
| R1 | [requirement from research report] | ✅ Implemented | [brief description] |
| R2 | [requirement from research report] | ⚠️ Partial | [what's missing and why] |
| R3 | [requirement from research report] | ❌ Deferred | [reason — scope, budget, dependency] |

## Decisions and Trade-offs

| Decision | Rationale |
|---|---|
| [decision made] | [grounded in context file / rule ID] |

## Receiver Context

| Item | Value |
|---|---|
| **Governing instruction file** | [path to instruction file for this artifact type] |
| **Key context files for validation** | [2-4 most relevant context file paths] |
| **Domain context (if non-PE)** | [domain-specific context paths, or "N/A — PE domain"] |
| **Known limitations** | [list, or "None"] |

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-builder"
    - "context-builder"
    - "documentation-builder"
    - "hook-builder"
    - "instruction-builder"
    - "meta-optimizer"
    - "prompt-builder"
    - "prompt-snippet-builder"
    - "skill-builder"
    - "template-builder"
  changelog: "output-builder-handoff.template.changelog.md"
---
-->
