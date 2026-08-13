---
description: "Fix report format for validator-to-builder feedback loop"
---

# Validation Fix Report

## Issue Summary

| # | Severity | Line(s) | Issue | Rule ID | Fix instruction | Dependencies |
|---|---|---|---|---|---|---|
| 1 | CRITICAL | L[N] | [description] | [C1/H2/etc.] | [specific fix] | None |
| 2 | HIGH | L[N]-L[N] | [description] | [rule ID] | [specific fix] | Depends on #1 |
| 3 | MEDIUM | L[N] | [description] | [rule ID] | [specific fix] | None |

## Fix Priority Order

Fix in this order: [#1 → #2 (dependency) → remaining by severity].

CRITICAL issues MUST be fixed first. Issues with dependencies MUST be fixed after their prerequisite.

## Context for Fixes

| Item | Value |
|---|---|
| **Rule references** | [context file paths for the rules cited above] |
| **Governing instruction file** | [path] |
| **Builder should re-read** | [specific file sections if rule context is needed] |

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-validator"
    - "context-validator"
    - "documentation-validator"
    - "hook-validator"
    - "instruction-validator"
    - "meta-validator"
    - "prompt-snippet-validator"
    - "prompt-validator"
    - "skill-validator"
    - "template-validator"
  changelog: "output-validator-fixes.template.changelog.md"
---
-->
