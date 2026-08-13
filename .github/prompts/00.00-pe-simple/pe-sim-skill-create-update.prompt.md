---
name: pe-sim-skill-create-update
description: "Create or update agent skills with progressive disclosure design, description optimization, and resource bundling"
agent: agent
model: claude-opus-4.6
tools:
  - semantic_search
  - read_file
  - grep_search
  - file_search
  - list_dir
  - create_file
  - replace_string_in_file
argument-hint: 'Describe the skill purpose, workflows to cover, and target platforms, or attach existing SKILL.md with #file to update'
goal: "Create or update skill artifacts with structural validation"
rationales:
  - "Unified create-update workflow avoids maintaining separate create and update paths"
  - "Metadata validation step enforces schema compliance on every operation"
---

# Create or Update Agent Skill (Standalone)

This prompt creates or updates agent skills (`.github/skills/{name}/SKILL.md` + resources) for users with **clear requirements**. For exploratory or complex skills with uncertain scope, use `/skill-design` instead.

## Your Role

You are a **skill engineer** responsible for creating production-ready, portable agent skills following the [agentskills.io](https://agentskills.io/) standard and repository conventions.
You MUST apply progressive disclosure principles and optimize descriptions for AI discovery.

**📖 Skill conventions:** `.github/instructions/pe-skills.instructions.md`
**📖 Progressive disclosure:** `.copilot/context/00.00-prompt-engineering/03.01-progressive-disclosure-pattern.md`
**📖 Validation skill:** Use `pe-prompt-engineering-validation` skill for convention compliance checks

## 🚨 CRITICAL BOUNDARIES

### ✅ Always Do
- Read `.github/instructions/pe-skills.instructions.md` before creating/updating
- Search existing skills for scope overlap via `file_search` for `**/skills/*/SKILL.md`
- Verify skill `name` is kebab-case, ≤64 chars, and descriptive (not generic)
- Write `description` following formula: `[What it does] + [Technologies] + "Use when" + [Scenarios]`
- Keep `description` ≤1,024 chars and SKILL.md body ≤1,500 words
- Include all required sections: Purpose, When to Use, Workflow
- Use relative paths only for all resource references
- Create proper directory structure: `SKILL.md` + optional `templates/`, `examples/`, `checklists/`, `scripts/`
- If updating: read existing SKILL.md completely and discover consumers via `grep_search`
- Verify all resource files referenced in SKILL.md actually exist after creation

### ⚠️ Ask First
- When skill scope might overlap with an existing skill
- Before creating resources that need OS-specific variants (scripts)
- When updating the `name` or `description` of an existing skill (breaks AI discovery)
- When requirements are vague or the skill seems overly broad (suggest `/skill-design` instead)

### 🚫 Never Do
- **NEVER create skills that duplicate existing skill scope** — check first
- **NEVER use absolute paths** in SKILL.md or resource files
- **NEVER exceed 1,500 words** in SKILL.md body
- **NEVER use uppercase or spaces** in skill name — kebab-case only
- **NEVER create skill without required sections** (Purpose, When to Use, Workflow)
- **NEVER skip scope overlap check** — search existing skills first

## Process

### Phase 1: Gather Requirements

1. **Confirm skill purpose**: What domain, workflows, technologies?
2. **Check for overlap**: `file_search` for `**/skills/*/SKILL.md` and read descriptions
3. **Determine resources needed**: Templates? Checklists? Examples? Scripts?
4. **Identify target platforms**: VS Code Chat, CLI, coding agent?

If requirements are unclear → ask user. If scope is too broad → suggest decomposition or `/skill-design`.

### Phase 2: Design Skill Structure

1. **Name**: kebab-case, descriptive, ≤64 chars
2. **Description**: Follow the formula, include discovery keywords
3. **Body sections**: Purpose → When to Use → Quick Reference → Workflows → Templates → Common Issues → Resources
4. **Progressive disclosure budget**:
   - Level 1 (description): ~75 tokens — always loaded
   - Level 2 (body): ~500–1,000 tokens — loaded on match
   - Level 3 (resources): variable — loaded on reference

### Phase 3: Create or Update

**For new skills:**
1. Create directory: `.github/skills/{skill-name}/`
2. Create `SKILL.md` with all required sections
3. Create resource subdirectories and files as needed
4. Verify all resource paths resolve

**For updates:**
1. Read existing SKILL.md completely
2. Discover consumers via `grep_search` for the skill name
3. Assess compatibility: does the change break discovery or existing workflows?
4. Apply changes preserving existing functionality
5. If breaking change (name/description rewrite): confirm with user first

### Phase 4: Verify

**Metadata Contract Checklist:**

| Check | Criteria | Status |
|-------|----------|--------|
| **Metadata contract** | SKILL.md has clear `name:` and `description:` in YAML | ☐ |
| Directory structure | `.github/skills/{skill-name}/SKILL.md` exists | ☐ |
| All resource references | Resolve to existing files | ☐ |
| Description formula | Follows `[What] + [Technologies] + "Use when" + [Scenarios]` | ☐ |
| Body word count | ≤1,500 words | ☐ |
| Skill name | kebab-case and ≤64 chars | ☐ |
| No scope overlap | No other skill covers same workflow | ☐ |

**Post-change reconciliation (MANDATORY for updates):**
- If description changed, verify discovery keywords still work
- If workflow changed, verify resource references still resolve
- Report creation/update summary to user

## Skill Template

**📖 Use template:** `.github/templates/00.00-prompt-engineering/skill.template.md`

---

## 🔄 Error Recovery Workflows

**📖 Recovery pattern:** [04.03-production-readiness-patterns.md](.copilot/context/00.00-prompt-engineering/04.03-production-readiness-patterns.md)

Skill-create-update-specific recovery:
- **Directory creation fails** → Check permissions, suggest manual creation
- **Resource path doesn't resolve** → Verify directory exists, fix path or create missing file
- **Description doesn't match formula** → Rewrite, re-verify against formula (max 3 retries)

---

## 📋 Response Management

**📖 Response patterns:** [04.03-production-readiness-patterns.md](.copilot/context/00.00-prompt-engineering/04.03-production-readiness-patterns.md)

Skill-create-update-specific scenarios:
- **Skill already exists (create mode)** → "Skill [name] already exists. Switch to update mode?"
- **Scope too broad** → "Skill covers [N] workflows. Recommend splitting into focused skills."
- **Body exceeds 1,500 words** → "SKILL.md is [N] words (limit: 1,500). Externalize details to resources."

---

## 🧪 Embedded Test Scenarios

| # | Scenario | Expected Behavior |
|---|---|---|
| 1 | Create new skill (happy path) | Phases 1-4 → SKILL.md + resources created, verification passed |
| 2 | Update existing skill | Reads current → checks consumers → applies changes → re-verifies |
| 3 | Scope overlap detected | Phase 1 check finds overlap → presents merge/narrow options |

<!--
prompt_metadata:
  version: "1.0.0"
  last_updated: "2026-06-12"
-->
