---
description: "Structured validation report format for prompt file review"
---

# Validation Report: [File Name]

**Validation Date:** [ISO 8601 timestamp]
**Validator:** prompt-validator agent
**Overall Status:** ✅ PASSED / ⚠️ PASSED WITH WARNINGS / ❌ FAILED

---

## Executive Summary

### Overall Assessment
[1-2 sentence summary of validation outcome]

### Scores Summary
- **Tool Alignment:** ✅ PASS / ❌ FAIL (CRITICAL)
- **Structure:** [X]/[Y] checks passed
- **Conventions:** [X]/[Y] checks passed
- **Patterns:** [X]/[Y] patterns followed
- **Production Readiness:** [X]/5 criteria met
- **Quality:** [X]/[Y] criteria met
- **Overall:** [X]/[Y] total ([percentage]%)

### Critical Issues (must fix before use)
[Count]: [If 0, state "None found"]
1. [Issue 1 with severity]

### Warnings (recommended fixes)
[Count]: [If 0, state "None found"]
1. [Warning 1]

### Minor Issues (optional improvements)
[Count]: [If 0, state "None found"]
1. [Minor issue 1]

---

## Detailed Findings

### 1. Tool Alignment (CRITICAL)

**Mode**: [plan/agent]

| Tool | Type | Allowed | Status |
|------|------|---------|--------|
| [tool-1] | [read/write] | [Yes/No] | ✅/❌ |

**Alignment Status**: [✅ PASS / ❌ FAIL - CRITICAL]
**Violations**: [None / List violations]

### 2. Structure Validation

#### YAML Frontmatter
**Status:** ✅ Valid / ❌ Invalid

**Required fields check:**
- [ ] `description:` - [Status]
- [ ] `agent:` - [Status]
- [ ] `tools:` - [Status]

#### Section Completeness
**Present:** [X]/[Y] required sections
**Missing:** [List with expected locations]

#### Markdown Formatting
- [✅/❌] Heading hierarchy valid
- [✅/❌] Code blocks properly formatted
- [✅/❌] Lists properly structured

### 3. Convention Compliance

#### Naming
- **File name:** [name] - ✅ Valid / ❌ Invalid
- **YAML fields:** - ✅ Complete / ❌ Missing: [list]

#### Tool Configuration
- **Agent type:** `agent: [type]`
- **Tool count:** [count]
- **Alignment:** ✅ Valid / ❌ Issues

#### Metadata
- **Bottom block:** ✅ Present / ❌ Missing

### 4. Pattern Consistency

#### Similar Files Analyzed
1. `[file-path]` - [Similarity description]

#### Consistency Assessment
- **Overall:** ✅ Consistent / ⚠️ Minor deviations / ❌ Significant issues
- **Intentional variations:** [List if documented]

### 5. Production Readiness

| Requirement | Status | Details |
|---|---|---|
| Data Gaps Handling | ✅/⚠️/❌ | [description] |
| "I Don't Know" Responses | ✅/⚠️/❌ | [description] |
| Error Recovery | ✅/⚠️/❌ | [description] |
| Embedded Tests | ✅/❌ | [N] scenarios found |
| Token Count & Context Rot | ✅/⚠️/🔴 | ~[N] tokens |

**Production Readiness Score:** [X]/5 criteria met

### 6. Quality Assessment

| Dimension | Score | Issues |
|---|---|---|
| Clarity | [X]/[Y] | [line N: issue] |
| Completeness | [X]/[Y] | [gaps] |
| Actionability | [X]/[Y] | [ambiguities] |
| Context Engineering | [X]/[6] | [violations] |

---

## Recommendations

### Critical (Must fix before use)
1. **[Issue]** — Line [N]: [Problem] → [Fix]

### Moderate (Should fix)
1. **[Issue]** — Line [N]: [Problem] → [Fix]

### Minor (Consider improving)
1. **[Issue]** — Line [N]: [Problem] → [Fix]

---

## Next Steps

### If ✅ PASSED
File is ready for use. Optional improvements listed above.

### If ⚠️ PASSED WITH WARNINGS
File is usable but has [count] warnings. Consider handoff to `prompt-builder`.

### If ❌ FAILED
File has [count] critical issues. Hand off to `prompt-builder` with this report.

---

## Validation Metadata

```yaml
validation:
  date: "[ISO 8601]"
  validator: "prompt-validator"
  file_validated: "[file-path]"
  overall_status: "passed/passed_with_warnings/failed"
  scores:
    structure: [X]/[Y]
    conventions: [X]/[Y]
    patterns: [X]/[Y]
    production_readiness: [X]/5
    quality: [X]/[Y]
    overall: [X]/[Y]
  issues:
    critical: [count]
    moderate: [count]
    minor: [count]
```

---

## Validation Criteria by File Type

### For Validation Prompts
- Uses `agent: plan` (read-only)
- Implements 7-day caching pattern
- Has bottom metadata block
- Tools are read-only only

### For Implementation Prompts
- Uses `agent: agent` (write access)
- Has research phase before implementation
- Includes validation phase after implementation
- Tools include both read and write capabilities

### For Orchestration Prompts
- Has `handoffs:` section with valid agents
- Each handoff has `label`, `agent`, `send` fields
- Validates each phase output before proceeding

### For Agent Files
- Description is concise (one sentence)
- Tool list matches role (researcher = read-only, builder = write)
- Handoffs defined if agent coordinates others

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-validator"
  changelog: "output-prompt-validator-report.template.changelog.md"
---
-->
