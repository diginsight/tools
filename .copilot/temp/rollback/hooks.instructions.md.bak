---
description: Rules for creating and maintaining agent hook configurations that provide deterministic lifecycle automation
applyTo: '.github/hooks/**'
version: "1.3.0"
last_updated: "2026-03-19"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Agent Hook Rules

## Purpose

Hook configurations provide **deterministic lifecycle automation** — shell commands that run at specific agent events. They execute code, not LLM interpretation. Every hook MUST be valid JSON.

## Severity Index

**CRITICAL** — block on failure:
- **[C6]** Valid JSON: invalid syntax silently breaks automation

**HIGH** — fix before use:
- **[H8]** Imperative language: explicit event-to-action mapping

**MEDIUM** — fix when convenient:
- **[M6]** Naming: kebab-case `{purpose}.json`

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)

## Rules

### JSON Structure
- MUST use valid JSON syntax
- MUST use `"type": "command"` for every hook entry
- MUST include explicit `"timeout"` for every hook (default: 30s)
- MUST place hook files in `.github/hooks/` root — no subfolders

### Lifecycle Events
- MUST use only: `SessionStart`, `UserPromptSubmit`, `PreToolUse`, `PostToolUse`, `SubagentStart`, `SubagentStop`, `PreCompact`, `Stop`
- MUST NOT use hooks for tasks requiring LLM reasoning — use prompts/agents

### Security
- `PreToolUse` deny hooks MUST document security rationale
- MUST NOT weaken security-critical hooks without explicit user approval

### Cross-Platform
- SHOULD provide OS-specific overrides (`"windows"`, `"linux"`, `"osx"`) when scripts differ
- Companion scripts MUST exist at referenced path before activation
- MUST use relative paths from workspace root

**📖 Lifecycle events and JSON schema:** `.copilot/context/00.00-prompt-engineering/03.03-agent-hooks-reference.md`

## Quality Checklist

- [ ] Valid JSON syntax (C6)
- [ ] Every entry has `"type": "command"` and `"timeout"`
- [ ] Events match the 8 supported lifecycle events
- [ ] Companion scripts exist at referenced paths
- [ ] Security rationale documented for deny hooks

## References

- **📖** `.copilot/context/00.00-prompt-engineering/03.03-agent-hooks-reference.md` — Hook schema
- **📖** `.copilot/context/00.00-prompt-engineering/03.04-mcp-server-design-patterns.md` — Hooks vs MCP vs tools
