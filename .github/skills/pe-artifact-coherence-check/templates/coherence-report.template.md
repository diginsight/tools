# Coherence Audit Report

## Audit Metadata

**Date:** [YYYY-MM-DD]
**Scope:** [Full system / Targeted: specific files]
**Auditor:** meta-validator agent (Ecosystem Audit mode)

## Executive Summary

| Check | Status | Findings |
|---|---|---|
| Reference Integrity | ✅/❌ | [N] broken of [N] total |
| Rule Consistency | ✅/❌ | [N] contradictions |
| Handoff Chains | ✅/❌ | [N] broken chains |
| Redundancy | ✅/⚠️ | [N] duplications, ~[N] tokens wasted |
| Token Budgets | ✅/⚠️ | [N] files over budget |
| Dependency Map | ✅/❌ | [N] discrepancies |

**Overall Status:** ✅ HEALTHY / ⚠️ NEEDS ATTENTION / ❌ CRITICAL ISSUES

## Findings by Severity

### 🔴 CRITICAL ([N] findings)

| # | Category | File | Issue | Recommended Fix |
|---|---|---|---|---|
| 1 | [check type] | `[path]` | [description] | [action] |

### 🟡 HIGH ([N] findings)

| # | Category | File | Issue | Recommended Fix |
|---|---|---|---|---|
| 1 | [check type] | `[path]` | [description] | [action] |

### 🟢 MEDIUM ([N] findings)

| # | Category | File | Issue | Recommended Fix |
|---|---|---|---|---|
| 1 | [check type] | `[path]` | [description] | [action] |

### ℹ️ LOW ([N] findings)

| # | Category | File | Issue | Recommended Fix |
|---|---|---|---|---|
| 1 | [check type] | `[path]` | [description] | [action] |

## Token Budget Summary

| File | Type | Lines | Est. Tokens | Budget | Status |
|---|---|---|---|---|---|
| `[path]` | [type] | [N] | [N] | [N] | ✅/⚠️/❌ |

## Redundancy Summary

| Duplicated Content | Source (canonical) | Found In | Est. Tokens Wasted |
|---|---|---|---|
| [content description] | `[canonical file]` | `[duplicate file]` | ~[N] |

## Recommended Actions

1. **[Priority]**: [Action description] → affects [N] dependents
2. ...

## Next Steps

- [ ] Hand off CRITICAL + HIGH findings to `meta-optimizer` for fixes
- [ ] Re-run coherence check after fixes applied
- [ ] Update dependency map if new artifacts were discovered
