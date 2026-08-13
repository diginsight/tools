---
description: "Phase structure for agent design orchestration workflows"
---

# Agent Design Orchestrator Output Templates

**Purpose:** Reusable output format templates for the agent-design orchestrator workflow.

**Referenced by:** `agent-design.prompt.md` and related orchestrator prompts

---

## Workflow Progress Report

```markdown
# Agent Creation Workflow: [agent-name]

**Status**: [In Progress / Complete / Blocked]
**Current Phase**: [N] - [Phase Name]

## Progress

| Phase | Status | Notes |
|-------|--------|-------|
| 1. Requirements | ✅/🔄/❌ | [notes] |
| 2. Research | ✅/🔄/❌ | [notes] |
| 3. Structure | ✅/🔄/❌ | [notes] |
| 4. Creation | ✅/🔄/❌ | [notes] |
| 5. Dependencies | ✅/🔄/❌ | [notes] |
| 6. Recursive | ✅/🔄/❌/N/A | [notes] |
| 7. Validation | ✅/🔄/❌ | [notes] |
| 8. Resolution | ✅/🔄/❌/N/A | [notes] |

## Agents Created
- `[agent-name].agent.md` - [status]
- [dependent agents if any]

## Blocking Issues
[None / List issues]

## Next Action
[Description of next step]
```

---

## Completion Summary

```markdown
# Agent Creation Complete: [agent-name]

## Summary
- **Agent**: `[agent-name].agent.md`
- **Mode**: [plan/agent]
- **Tools**: [N] - [list]
- **Quality Score**: [N]/10

## Files Created
1. `.github/agents/[agent-name].agent.md`
[Additional files if any]

## Validation Results
- Tool Alignment: ✅
- Structure: ✅
- Quality: [score]

## Next Steps
- Agent is ready for use
- Test with example scenarios
- Consider integration with orchestration prompts
```

---

## Phase 1: Research Request Delegation Template

Use this template when delegating to `agent-researcher` in Phase 1. Fill in bracketed values from the orchestrator's complexity assessment.

```markdown
## Research Request

**Agent Role**: [from user request]
**Complexity**: [Simple/Moderate/Complex]
**Validation Depth**: [Quick/Standard/Deep]
**Use Cases to Generate**: [3/5/7]

Please:
1. Challenge this role with [N] realistic use cases
2. Discover tool requirements from scenarios
3. Define scope boundaries (IN/OUT)
4. Validate tool count is 3-7
5. Apply depth-appropriate validation:
   - **Quick**: Basic role check, pattern-match tool composition
   - **Standard**: Role + workflow check, tool discovery + validation, boundary actionability test
   - **Deep**: Full role/workflow/tool analysis, handoff dependency mapping, boundary cross-reference against failure modes
6. Use these tool inference hints as starting points (discover actual needs through use cases):
   - Researcher role → `semantic_search, grep_search, read_file, file_search`
   - Builder role → `read_file, semantic_search, create_file, file_search`
   - Validator role → `read_file, grep_search, file_search`
   - Updater role → `read_file, grep_search, replace_string_in_file, multi_replace_string_in_file`
   - Test Agent role → `read_file, semantic_search, run_in_terminal`
   - Security Agent role → `semantic_search, grep_search, read_file`
   - **📖 Full patterns:** `.copilot/context/00.00-prompt-engineering/01.04-tool-composition-guide.md` → "Tool Selection by Agent Role"
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-design"
  changelog: "output-agent-design-phases.template.changelog.md"
---
-->
