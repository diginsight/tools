# Tool Alignment Verification Template

## Agent Under Test

> **Agent name:** [agent-name]  
> **Agent mode:** `plan` / `agent`  
> **Declared tools:** [list tools from YAML]

## Step 1: Write Tool Detection

**Write tools found in declared tools:**

| Tool | Write? | Status |
|------|--------|--------|
| `create_file` | ✍️ Write | Found / Not found |
| `replace_string_in_file` | ✍️ Write | Found / Not found |
| `multi_replace_string_in_file` | ✍️ Write | Found / Not found |
| `run_in_terminal` | ✍️ Write | Found / Not found |
| `edit_notebook_file` | ✍️ Write | Found / Not found |
| `run_notebook_cell` | ✍️ Write | Found / Not found |

**Write tools count:** [N]

## Step 2: Mode Alignment Check

| Check | Result | Severity |
|-------|--------|----------|
| Mode is `plan` AND has write tools? | ✅ No / ❌ Yes | CRITICAL — BLOCK |
| Mode is `agent` AND has NO write tools? | ✅ No / ⚠️ Yes | WARNING — should this be `plan`? |
| Tool count in range 3-7? | ✅ Yes / ⚠️ No ([N] tools) | WARNING if outside range |

## Step 3: Tool-to-Phase Mapping

| Workflow Phase | Required Capability | Tool Used | Justified? |
|---------------|--------------------:|-----------|:----------:|
| [e.g., "Discover existing files"] | Search/read | `file_search` | ✅ / ❌ |
| [e.g., "Read file contents"] | Read | `read_file` | ✅ / ❌ |
| [e.g., "Create new file"] | Write | `create_file` | ✅ / ❌ |
| [e.g., "Validate output"] | Read/analyze | `get_errors` | ✅ / ❌ |

**Unused tools:** [List any declared tools not mapped to a phase]  
**Missing tools:** [List any phases without a tool]

## Step 4: Tool Count Assessment

| Range | Current | Status |
|-------|---------|--------|
| 1-2 (sparse) | | ⚠️ Verify task simplicity |
| 3-7 (optimal) | | ✅ Proceed |
| 8+ (clash risk) | | ❌ Decompose into agents |

## Step 5: Handoff Alignment (if applicable)

| Handoff Target | Send Mode | Compatible? | Notes |
|---------------|-----------|:-----------:|-------|
| [agent-name] | `true` / `false` | ✅ / ❌ | |

**Handoff rules:**
- `send: true` — actively passes work to next specialist
- `send: false` — receives work, does not initiate handoff

## Overall Assessment

| Check | Status |
|-------|--------|
| Mode/tool alignment | ✅ / ❌ |
| Tool count budget | ✅ / ⚠️ / ❌ |
| Phase coverage | ✅ / ❌ |
| Handoff compatibility | ✅ / ❌ / N/A |

**Verdict:** ✅ Aligned / ❌ Violations found

## Required Fixes (if any)

1. [Fix 1]
2. [Fix 2]
