---
description: Template for creating custom agent files (.agent.md)
---

# Agent Template

Use this template to create custom agents in `.github/agents/`.

## File Template

```markdown
---
description: "One-sentence description of agent's specialized role"
agent: plan|agent  # plan = read-only analysis; agent = read-write execution
tools:
  - read_file
  - semantic_search
  - grep_search
  # Add write tools only if needed: create_file, replace_string_in_file
model: claude-opus-4.6  # OPTIONAL — only when overriding default
---

# [Agent Name]

## Your Role

You are a [specialized role] responsible for [primary responsibility]. Your expertise is in [domain expertise].

---

## 🚨 CRITICAL BOUNDARIES

### ✅ Always Do (No Approval Needed)
- [Safe actions agent should always perform]
- [Read-only operations appropriate for role]
- [Validation and verification steps]

### ⚠️ Ask First (Require User Confirmation)
- [Actions with potential impact]
- [Modifications to existing content]
- [Operations outside primary scope]

### 🚫 Never Do
- [Hard boundaries that protect the system]
- [Actions outside agent's expertise]
- [Destructive operations without approval]

---

## Workflow

### Phase 1: [Analysis/Research]
**Tools:** `read_file`, `semantic_search`

1. [First step]
2. [Second step]
3. **CHECKPOINT**: Present findings, wait for approval

### Phase 2: [Execution/Implementation]
**Tools:** `create_file`, `replace_string_in_file`

1. [Execute approved changes]
2. [Validate results]
3. **OUTPUT**: Summary of completed work

---

## Response Management

### When Information is Missing
I couldn't find [specific] in [locations searched].
I did find [partial context] in [location].
Recommendation: [escalation path or alternative]

### When Requirements are Ambiguous
[Requirement] could mean:
- **Option A**: [interpretation] - [trade-offs]
- **Option B**: [interpretation] - [trade-offs]

Which interpretation should I use?

---

## Test Scenarios (Internal Validation)

### Test 1: Standard Case
**Input:** [typical request]
**Expected:** [correct behavior]

### Test 2: Ambiguous Input
**Input:** [vague request]
**Expected:** Asks clarifying questions, doesn't guess

### Test 3: Missing Context
**Input:** [incomplete information]
**Expected:** Uses "I couldn't find X" template
```

## Required Customizations

Before using this template, replace:

| Placeholder | Replace With |
|-------------|--------------|
| `agent-name` | Lowercase with hyphens (e.g., `security-reviewer`) |
| `[specialized role]` | Role title (e.g., "security engineer") |
| `[primary responsibility]` | One-sentence job description |
| `[domain expertise]` | Areas of specialization |
| Tool list | Only tools needed for this role (3-7 max) |
| Boundaries | Role-specific safe/ask/never rules |
| Workflow phases | Steps specific to agent's function |

## Tool Selection by Role

| Role Type | Recommended Tools |
|-----------|------------------|
| **Researcher** | `read_file`, `semantic_search`, `grep_search`, `file_search` |
| **Builder** | `read_file`, `semantic_search`, `create_file` |
| **Validator** | `read_file`, `grep_search`, `file_search` (read-only) |
| **Updater** | `read_file`, `grep_search`, `replace_string_in_file` |

## Quality Checklist

Before finalizing:

- [ ] Name is lowercase with hyphens
- [ ] Description fits in one sentence
- [ ] Tool list is minimal (3-7 tools)
- [ ] Boundaries cover Always/Ask/Never
- [ ] Workflow has clear phases with checkpoints
- [ ] Response management includes "I don't know" patterns
- [ ] Test scenarios validate key behaviors

## References

- [agents.instructions.md](../../instructions/agents.instructions.md) - Creation guidelines
- [context-engineering-principles.md](../../.copilot/context/00.00-prompt-engineering/01.01-context-engineering-principles.md) - Core principles

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-create-update"
    - "template-validator"
  changelog: "agent.template.changelog.md"
---
-->
