---
description: "Phase structure for prompt guidance validation workflows"
---

# Guidance Prompt Validation Phase Output Templates

**Purpose:** Reusable output format templates for domain guidance creation/update workflows.

**Referenced by:** `prompt-createorupdate-prompt-guidance.prompt.md`

---

## Phase 1: Domain Context Collection Output

```markdown
## Domain Context Summary

**Domain:** {domain-name}
**Target File Type:** [instruction / context / both]
**Target Paths:**
- `.github/instructions/{domain}.instructions.md`
- `.copilot/context/{domain}/*.md`

### Context Sources Analyzed
| Source | Type | Status | Key Findings |
|--------|------|--------|--------------|
| [path/url] | [file/url] | [✅ loaded / ⚠️ partial / ❌ missing] | [summary] |

### Existing Patterns Found
- [Pattern 1]: [where found]
- [Pattern 2]: [where found]

### Key Principles Identified
1. **[Principle name]:** [description from context]
2. **[Principle name]:** [description from context]
```

---

## Phase 2: Domain Requirements Analysis Output

```markdown
## Domain Requirements Analysis

### Required Elements (MUST include)
- [Element 1]
- [Element 2]

### Quality Criteria (Success measures)
- [Criterion 1]
- [Criterion 2]

### Anti-Patterns (NEVER do)
- [Anti-pattern 1]
- [Anti-pattern 2]

### Boundary Rules (Scope limitations)
- IN SCOPE: [what's covered]
- OUT OF SCOPE: [what's excluded]

### Three-Tier Boundary Mapping
| Category | Rules from Context |
|----------|-------------------|
| ✅ Always Do | [extracted rules] |
| ⚠️ Ask First | [extracted rules] |
| 🚫 Never Do | [extracted rules] |
```

---

## Phase 3: Generation Confirmation Output

```markdown
## Files to Generate

### Instruction File
- **Path:** `.github/instructions/{domain}.instructions.md`
- **Scope:** [what it covers]
- **applyTo pattern:** [glob pattern]

### Context Files
| File | Purpose |
|------|---------|
| `.copilot/context/{domain}/01-[topic].md` | [description] |
| `.copilot/context/{domain}/02-[topic].md` | [description] |

**Confirm generation?** [Waiting for user confirmation]
```

---

## Phase 4: Validation Checklist Output

```markdown
## Pre-Output Validation

| Check | Status | Notes |
|-------|--------|-------|
| YAML syntax valid | [✅/❌] | |
| Cross-references exist | [✅/❌] | [list any missing] |
| No circular dependencies | [✅/❌] | |
| Imperative language used | [✅/❌] | [any "should/try" found?] |
| Domain alignment verified | [✅/❌] | |
| Three-tier boundaries complete | [✅/❌] | |

**Validation Result:** [PASS / FAIL - with specific issues]
```

---

## Response Management Templates

### Missing Context Response

```markdown
⚠️ MISSING CONTEXT: {file path or URL} not found.

**Fallback Actions:**
1. Searching for alternatives with `semantic_search`
2. Checking for renamed files with `file_search`

**Options:**
- [ ] Provide alternative source path/URL
- [ ] Confirm proceed with partial context
- [ ] Cancel and provide complete context

**Current coverage:** {X}% of required context available
```

### Ambiguous Domain Response

```markdown
❓ CLARIFICATION NEEDED: Domain "{domain}" requires clarification.

**Question 1: Scope**
What aspects should guidance cover?
- [ ] All aspects of {domain}
- [ ] Only: {specific aspect}

**Question 2: Audience**
Who will use prompts guided by this?
- [ ] Content creators
- [ ] Validators/reviewers
- [ ] Developers

**Question 3: Output Format**
What should guided prompts produce?
- [ ] Markdown files
- [ ] Code files
- [ ] Validation reports

Please respond with your selections.
```

### Tool Failure Response

```markdown
⚠️ TOOL ISSUE: `{tool_name}` returned {empty/error}.

**Error:** {error details if available}
**Fallback:** {specific alternative approach}
**Proceeding with:** {what will be done instead}
```

### Out of Scope Response

```markdown
🚫 OUT OF SCOPE: {requested action} is not within this prompt's boundaries.

**This prompt creates:** Domain-specific instruction and context files
**Your request requires:** {what they actually need}

**Recommended alternative:**
- For prompt files: `prompt-create-update.prompt.md`
- For agent files: Use agent creation prompts
- For skill files: Use skill creation prompts
```

---

## Error Recovery Workflow Templates

### fetch_webpage Failure Recovery

```markdown
**Trigger:** External URL fetch failed
**URL:** {url}
**Error:** {error type}

**Recovery Steps:**
1. ❓ Do you have a local copy of this content?
2. 🔍 Searching repository for similar content...
3. 📂 Checking `.copilot/context/` for related guidance...

**Outcome:** {proceeding with available context / waiting for user input}
```

### create_file Failure Recovery

```markdown
**Trigger:** File creation failed
**Target:** {file path}
**Error:** {error type}

**Recovery Steps:**
1. 📂 Verifying directory exists...
2. 🔍 Checking for conflicting files...
3. 📝 Suggesting corrected path: {suggested path}

**If path cannot be resolved:**
[File content provided below for manual creation]
```

### semantic_search Empty Recovery

```markdown
**Trigger:** No relevant content found
**Query:** "{search query}"

**Recovery Steps:**
1. 🔍 Trying `grep_search` with: "{keywords}"
2. 📂 Trying `file_search` with: "*{pattern}*"
3. 📖 Reading standard locations directly...

**Outcome:** No existing patterns found for {domain}; creating from user context and best practices.
```

### read_file Missing Recovery

```markdown
**Trigger:** User-specified file not found
**Path:** {file path}

**Recovery Steps:**
1. 🔍 Searching for renamed file: `*{filename}*`
2. ❓ Please verify correct path

**Found alternatives:** {list or "none"}
**Proceeding with:** partial context, gap noted in output
```

---

## Guidance Metadata Block Template

```yaml
<!-- 
---
validations:
  structure:
    status: "[pending/validated]"
    last_run: "YYYY-MM-DDTHH:MM:SSZ"
    model: "claude-sonnet-4.5"
  production_ready:
    status: "[pending/validated]"
    last_run: "YYYY-MM-DDTHH:MM:SSZ"
    checks:
      response_management: [true/false]
      error_recovery: [true/false]
      test_scenarios: [count]
      tool_count: [count]
      boundaries: "[complete/partial/missing]"
      template_externalization: [true/false]
article_metadata:
  filename: "{filename}"
  last_updated: "YYYY-MM-DDTHH:MM:SSZ"
  version: "{version}"
---
-->
```

---

## Embedded Test Scenario Templates

### Test Scenario Format

```markdown
### Test {N}: {Test Name} ({Category})

**Input:** "{user request}"
**Preconditions:** {any setup or state required}

**Expected Behavior:**
1. {Step 1}
2. {Step 2}
3. {Step 3}

**Success Criteria:**
- [ ] {Criterion 1}
- [ ] {Criterion 2}

**Category:** [Happy Path / Edge Case / Error Recovery / Boundary Test]
```

### Standard Test Categories

| Category | Purpose | Minimum Count |
|----------|---------|---------------|
| Happy Path | Normal successful flow | 2 |
| Incomplete Input | Missing required information | 1 |
| Missing Resources | Files/URLs not found | 1 |
| Out of Scope | Requests outside boundaries | 1 |
| Existing Patterns | Domain with prior work | 1 |

**Total Minimum:** 6 test scenarios

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "prompt-createorupdate-prompt-guidance"
  changelog: "output-guidance-validation-phases.template.changelog.md"
---
-->
