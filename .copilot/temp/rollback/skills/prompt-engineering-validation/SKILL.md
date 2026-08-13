---
name: prompt-engineering-validation
description: >
  Reusable validation patterns for prompt engineering artifacts: 
  use case challenge, role validation, tool alignment verification, 
  workflow reliability testing, boundary actionability checks,
  YAML frontmatter validation, required sections check, and convention compliance.
  Use when creating prompts, validating agents, reviewing tool alignment,
  testing workflow reliability, checking YAML frontmatter, verifying required sections,
  or auditing naming conventions for GitHub Copilot customization files.
---

# Prompt Engineering Validation Skill

## Purpose

Provide reusable validation workflows for prompt and agent engineering. Eliminates duplication of validation logic across prompt-researcher, agent-researcher, prompt-validator, agent-validator, prompt-builder, and agent-builder agents. Also provides system-level checks for cross-artifact consistency, redundancy, and token budget compliance.

## When to Use

Activate this skill when:
- **Creating prompts/agents**: "Validate requirements for this new prompt"
- **Challenging goals**: "Test this prompt purpose with use cases"
- **Validating roles**: "Check if this role is appropriate for the goal"
- **Checking tool alignment**: "Verify tool/agent mode alignment"
- **Testing workflows**: "Test this workflow for failure modes"
- **Reviewing boundaries**: "Check if boundaries are actionable"
- **Checking redundancy**: "Find duplicated rules across PE artifacts"
- **Cross-artifact consistency**: "Verify rules agree across instructions and context files"
- **Token budget audit**: "Check which PE files exceed their token budget"
- **YAML validation**: "Check if this artifact's YAML frontmatter is valid"
- **Section completeness**: "Verify this file has all required sections"
- **Convention check**: "Verify naming, location, and extension conventions"

Do NOT use this skill for:
- Article content review (use `article-review` skill)
- Full system coherence audit (use `artifact-coherence-check` skill)
- Code review or security auditing
- Context file creation (use context-builder agent)

## Quick Reference

### Validation Sequence

```
1. Complexity Assessment → determines depth
2. Use Case Challenge → discovers gaps (3/5/7 scenarios)
3. Role Validation → authority + expertise + specificity
4. Tool Alignment → mode matches tool capabilities
5. Workflow Reliability → failure mode analysis
6. Boundary Actionability → testable by AI
7. Artifact Redundancy → single-source-of-truth compliance
8. Cross-Artifact Consistency → rules agree across layers
9. Token Budget Audit → files within size limits
10. YAML Frontmatter Validation → required fields per artifact type
11. Required Sections Check → type-specific section completeness
12. Convention Compliance → naming, location, extension rules
```

### Complexity → Depth Mapping

| Complexity | Indicators | Use Cases | Validation |
|------------|------------|-----------|------------|
| **Simple** | 1-2 objectives, standard role, obvious tools | 3 | Quick |
| **Moderate** | 3+ objectives, domain expertise needed | 5 | Standard |
| **Complex** | Multiple interpretations, novel workflow, >7 tools | 7 | Deep |

### Tool Alignment Rules

**📖 Canonical source:** `.copilot/context/00.00-prompt-engineering/01.04-tool-composition-guide.md` — full allowed/forbidden tool lists per mode, tool categories, composition patterns.

**Alignment formula** (quick check):
- `plan` + write tools = **CRITICAL violation** (BLOCK)
- `agent` + no write tools = **WARNING** (should this be `plan`?)
- Tool count outside 3–7 = **WARNING** (>7 = tool clash risk)

**📖 Full verification checklist:** [tool-alignment.template.md](./templates/tool-alignment.template.md)

### Tool Count Budget

| Range | Status | Action |
|-------|--------|--------|
| 1-2 | ⚠️ Sparse | Verify task is truly simple |
| 3-7 | ✅ Optimal | Proceed |
| 8+ | ❌ Tool clash | MUST decompose into agents |

**📖 Tool count range and decomposition rules:** `01.06-system-parameters.md` → Agent Boundaries

## Detailed Workflows

### Workflow 1: Use Case Challenge

Use [use case challenge template](./templates/use-case-challenge.template.md) to test goals.

**Process:**
1. Generate N use cases (3/5/7 based on complexity)
2. For each scenario, test: Does goal clearly indicate what to do?
3. Record gaps: ambiguities, missing tools, scope boundary questions
4. Refine goal to address discovered gaps
5. Present critical gaps to user for clarification

**Gap Severity Classification:**

| Severity | Impact | Action |
|----------|--------|--------|
| CRITICAL | Multiple valid interpretations, different tool needs | BLOCK — ask user |
| HIGH | Affects scope or major workflow phases | ASK — present options |
| MEDIUM | Minor edge case handling | PROPOSE — suggest default |
| LOW | Cosmetic or optional enhancement | DEFER — note for later |

### Workflow 2: Role Validation

Use [role validation template](./templates/role-validation.template.md) to verify roles.

**Three Tests:**
1. **Authority**: Can this role make necessary judgments?
2. **Expertise**: Does role imply required knowledge?
3. **Specificity**: Is role concrete or generic?

**Common Role Anti-Patterns:**/
- "Helpful assistant" → Too generic, lacks authority signal
- "Expert in everything" → Overscoped, unfocused
- "Code reviewer" for documentation tasks → Wrong domain

### Workflow 3: Tool Alignment Verification

Use [tool alignment template](./templates/tool-alignment.template.md) for systematic checks.

**📖 Write tools list and full alignment rules:** `.copilot/context/00.00-prompt-engineering/01.04-tool-composition-guide.md`

### Workflow 4: Workflow Reliability Testing

**For each proposed phase, ask:** "What could go wrong?"

**Common failure modes:**
- Input validation missing (malformed files, wrong format)
- Scale handling absent (large files, many results)
- Error recovery missing (tool failures, network issues)
- Missing dependency discovery (external refs, imports)
- Cache/skip logic absent (repeat validations)

### Workflow 5: Boundary Actionability

**Test each boundary:** Can AI unambiguously determine compliance?

**Vague → Actionable transformation:**

| Vague | Actionable |
|-------|------------|
| "Be thorough" | "Check all 5 criteria: X, Y, Z, A, B" |
| "Be careful" | "NEVER modify files without reading first" |
| "Handle errors" | "When tool fails, log error and skip to next item" |

**Minimum boundary counts:** See `01.06-system-parameters.md` for canonical thresholds (Always Do, Ask First, Never Do minimums).

### Workflow 6: Artifact Redundancy Check

Detect content duplicated across artifact layers that violates single-source-of-truth.

**Process:**
1. Identify the canonical source for the rule being checked (always a context file)
2. Search instruction files, agents, and prompts for inline copies of that content
3. Flag any embedding >5 lines that exists in a context file
4. Calculate estimated token waste (duplicated lines × 6 tokens/line)

**Key rules and their canonical sources:**

| Rule | Canonical Source | Search For |
|---|---|---|
| Tool alignment | `01.04-tool-composition-guide.md` | "plan.*read-only", "write tools" |
| Template-first | `01.01-context-engineering-principles.md` | ">10 lines", "externalize" |
| Three-tier boundaries | `01.06-system-parameters.md` | "Always Do.*Ask First.*Never Do" |
| Reliability checksum | `02.01-handoffs-pattern.md` | "Goal Preservation", "Scope Boundaries" |
| Token budgets | `01.06-system-parameters.md` | Budget tables, line count thresholds |

**Output format:**
```markdown
| Duplicated Content | Canonical Source | Found In | Lines Duplicated | Tokens Wasted |
|---|---|---|---|---|
| [description] | `[file]` | `[file]` | [N] | ~[N] |
```

### Workflow 7: Cross-Artifact Consistency Check

Verify rules agree across artifact layers (context → instructions → agents → prompts).

**Process:**
1. Pick a rule from context files (e.g., "tool count must be 3–7")
2. Check if instruction files state the same threshold
3. Check if agents enforce the same threshold in their boundaries
4. Check if prompts reference the correct threshold
5. Flag any disagreements with both file paths and line numbers

**Common consistency checks:**

| Check | Context Rule | Verify In |
|---|---|---|
| Tool count range | "3–7" in `01.04-tool-composition-guide` | All agent boundaries, PE-validation skill |
| Inline threshold | ">10 lines" in `01.01-context-engineering-principles` | `prompts.instructions.md`, `agents.instructions.md` |
| Boundary minimums | ≥3/≥1/≥2 in `01.06-system-parameters` | All agents (Always, Ask, Never minimums) |
| Validation caching | "7 days" in `04.01-validation-caching-pattern` | Validation prompts (grammar, readability, etc.) |

**Contradiction severity:**

| Type | Severity | Example |
|---|---|---|
| Different thresholds | HIGH | Context says "3–7 tools", agent says "3–5 tools" |
| Opposite rules | CRITICAL | Context says "MUST", agent says "MAY" |
| Missing rule | MEDIUM | Context defines rule, agent doesn't mention it |
| Stronger than source | LOW | Agent says "NEVER" where context says "SHOULD NOT" |

### Workflow 8: Token Budget Audit

Verify PE artifacts stay within their token budget guidelines.

**Process:**
1. Count lines in each target file
2. Estimate tokens: lines × 6 (average)
3. Compare against budgets from `01.06-system-parameters.md`
4. Report files that exceed WARNING or CRITICAL thresholds

**📖 Budget thresholds (per-type limits, warning/critical levels):** `01.06-system-parameters.md` → Token Budgets

**Output format:**
```markdown
| File | Type | Lines | Est. Tokens | Budget | Status |
|---|---|---|---|---|---|
| `[path]` | [type] | [N] | ~[N] | [N] | ✅/⚠️/❌ |
```

## Templates

- **[Use Case Challenge](./templates/use-case-challenge.template.md)** — Structured format for testing goals with scenarios
- **[Role Validation](./templates/role-validation.template.md)** — Authority/expertise/specificity test checklist
- **[Tool Alignment](./templates/tool-alignment.template.md)** — Tool/agent mode verification with write-tool detection

---

### Workflow 10: YAML Frontmatter Validation

Validate that an artifact's YAML frontmatter contains all required fields for its type.

**Process:**
1. Read the target file's YAML frontmatter block
2. Determine artifact type from file path and extension
3. Check required fields against the type-specific table below
4. Flag missing or malformed fields

**Required YAML fields by artifact type:**

| Field | `.prompt.md` | `.agent.md` | `.instructions.md` | `SKILL.md` | Context `.md` |
|---|:---:|:---:|:---:|:---:|:---:|
| `name` | ✅ | — | — | ✅ | — |
| `description` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `agent` | ✅ | ✅ | — | — | — |
| `tools` | ✅ | ✅ | — | — | — |
| `model` | optional | optional | — | — | — |
| `applyTo` | — | — | ✅ | — | — |
| `title` | — | — | — | — | ✅ |
| `version` | — | — | — | — | ✅ |
| `last_updated` | — | — | — | — | ✅ |
| `handoffs` | if multi-agent | if has workflow | — | — | — |

**Severity:** Missing required field = HIGH. Malformed YAML = CRITICAL.

### Workflow 11: Required Sections Check

Verify an artifact has all required body sections for its type.

**Process:**
1. Read the target file
2. Determine artifact type from file path
3. Scan for required section headings per the type table below
4. Flag missing sections

**Required sections by artifact type:**

| Section | Prompts | Agents | Instructions | Skills | Context files |
|---|:---:|:---:|:---:|:---:|:---:|
| Role/Purpose | ✅ | ✅ | ✅ | ✅ | ✅ |
| Boundaries (Always/Ask/Never) | ✅ | ✅ | — | — | — |
| Process/Workflow | ✅ | ✅ | — | ✅ | — |
| Referenced by | — | — | — | — | ✅ |
| References | — | — | ✅ | — | ✅ |
| Version History | — | optional | — | — | ✅ |
| When to Use | — | — | — | ✅ | — |

**Boundary completeness:** When boundaries are required, verify minimums: ≥ 3 Always Do, ≥ 1 Ask First, ≥ 2 Never Do (see `01.06-system-parameters.md`).

**Severity:** Missing required section = HIGH. Missing boundaries = HIGH.

### Workflow 12: Convention Compliance

Verify naming, location, and extension conventions.

**Process:**
1. Check filename against naming rules for its type
2. Verify file is in the correct directory
3. Verify correct file extension
4. Check for kebab-case compliance

**Convention rules by artifact type:**

| Artifact | Extension | Location | Naming pattern |
|---|---|---|---|
| Prompt | `.prompt.md` | `.github/prompts/{area}/` | `{verb}-{noun}.prompt.md` |
| Agent | `.agent.md` | `.github/agents/{area}/` | `{role-name}.agent.md` |
| Instruction | `.instructions.md` | `.github/instructions/` (flat) | `{domain}.instructions.md` |
| Skill | `SKILL.md` | `.github/skills/{name}/` | `SKILL.md` (fixed name) |
| Context | `.md` | `.copilot/context/{domain}/` | `{NN.NN}-{topic}.md` |
| Template | `.template.md` | `.github/templates/{area}/` | `{category}-{purpose}.template.md` |
| Snippet | `.md` | `.github/prompt-snippets/` | `{topic}.md` |
| Hook | `.json` | `.github/hooks/` | `{purpose}.json` |

**Additional checks:**
- All names MUST be kebab-case (lowercase, hyphens, no spaces)
- Skill names MUST be ≤ 64 characters
- No files in wrong directories (e.g., agent in prompts folder)

**Severity:** Wrong extension = CRITICAL. Wrong location = HIGH. Naming violation = MEDIUM.

---

## Common Issues

### Issue: Use Case Challenge Reveals Too Many Gaps

**Symptom**: 4+ CRITICAL gaps after use case testing  
**Solution**: Goal is too broad. Decompose into multiple prompts/agents. Use orchestrator pattern.

### Issue: Role Fails Authority Test

**Symptom**: Role can't make required judgments  
**Solution**: Add domain qualifier. "Grammar reviewer" → "English grammar and style reviewer with technical writing expertise"

### Issue: Tool Count Exceeds 7

**Symptom**: Phase mapping identifies 8+ tools  
**Solution**: Decompose into orchestrator + specialist agents. Each agent gets 3-5 tools.

### Issue: Plan Mode Needs Write Tools

**Symptom**: Validation finds `plan` agent with `create_file`  
**Solution**: Either change to `agent` mode or remove write tools and restructure workflow.

## Resources

- **📖 Complete validation examples:** `.copilot/context/00.00-prompt-engineering/04.02-adaptive-validation-patterns.md`
- **📖 Tool composition patterns:** `.copilot/context/00.00-prompt-engineering/01.04-tool-composition-guide.md`
- **📖 Context engineering principles:** `.copilot/context/00.00-prompt-engineering/01.01-context-engineering-principles.md`
- **📖 Artifact dependency map:** see `dependency-tracking` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)
- **📖 Full system coherence audit:** `.github/skills/artifact-coherence-check/SKILL.md`
