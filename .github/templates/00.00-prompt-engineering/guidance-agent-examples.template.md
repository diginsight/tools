---
description: "Example patterns and structures for agent file creation"
---

<!-- Unique to this template: Concrete agent examples (6 role archetypes) and orchestration patterns. Rules live in agents.instructions.md; this provides copy-pasteable reference examples. -->
# Agent Examples and Patterns Reference

## Six Essential Agents (from 2,500+ Repo Analysis)

### 1. @docs-agent
**Purpose**: Technical documentation generation
**Tools**: `['codebase', 'editor', 'filesystem']`
**Key Boundaries**:
- ✅ Write to `docs/`, read from `src/`
- ⚠️ Ask before major rewrites
- 🚫 Never modify source code

### 2. @test-agent  
**Purpose**: Test generation and quality assurance
**Tools**: `['codebase', 'editor', 'terminal']`
**Key Boundaries**:
- ✅ Write to `tests/`, follow testing patterns
- ⚠️ Ask before changing test framework
- 🚫 Never remove failing tests to make builds pass

### 3. @lint-agent
**Purpose**: Code style and formatting fixes
**Tools**: `['editor', 'terminal']`
**Key Boundaries**:
- ✅ Fix style issues, run formatters
- ⚠️ Ask if style rules conflict
- 🚫 Never change code logic

### 4. @api-agent
**Purpose**: API endpoint development  
**Tools**: `['codebase', 'editor', 'terminal', 'web_search']`
**Key Boundaries**:
- ✅ Create/modify API routes
- ⚠️ Ask before database schema changes
- 🚫 Never modify authentication logic without approval

### 5. @security-agent
**Purpose**: Security analysis and vulnerability assessment
**Tools**: `['codebase', 'terminal', 'web_search']`
**Key Boundaries**:
- ✅ Scan for vulnerabilities, suggest fixes
- ⚠️ Ask before applying security patches
- 🚫 Never commit fixes that break functionality

### 6. @deploy-agent
**Purpose**: Build and deployment to dev environments
**Tools**: `['terminal', 'filesystem']`
**Key Boundaries**:
- ✅ Deploy to dev/staging only
- ⚠️ **Always** require explicit approval
- 🚫 Never deploy to production

---

## Multi-Agent Orchestration Patterns

### Using runSubagent Tool
Agents can invoke specialized sub-agents via `tools: ['agent']` (alias for `runSubagent`).

```markdown
## Available Sub-Agents
- @security-agent: Security review and vulnerability assessment
- @docs-agent: Documentation generation and updates
- @test-agent: Test case generation and validation

## Orchestration Workflow
1. Analyze user request
2. Decompose into sub-tasks  
3. Invoke appropriate sub-agent for each task using runSubagent tool
4. Aggregate results
5. Present unified response
```

Each sub-agent runs in **isolated context** to prevent cross-contamination.

### Supervisory Agent Pattern
For high-stakes operations:

```markdown
## Supervisor Workflow
1. Primary agent executes task
2. Supervisor agent reviews output
3. Supervisor validates boundaries respected
4. Supervisor can reject and request retry
```

---

## Agent Generation with Meta-Agents

Use existing agents to generate new agents:

```markdown
Prompt to Meta-Agent:

Create new custom agent for GitHub Copilot

## Requirements  
- Role: [Documentation writer / Test engineer / API developer]
- Analyze source under [specific directories]
- Generate [specific outputs] in [target directories]
- Style: [Formal technical / Conversational / Reference docs]
- Audience: [Junior engineers / Security auditors / API consumers]

## Boundaries
- Can modify: [specific directories]
- Must ask first: [configuration changes, schema updates]
- Never modify: [protected areas]

## Best Practices
[Attach: .github/instructions/agents.instructions.md]
- Analyze key points
- Apply to generated agent

## Important Notes
- Use clear boundaries (Always/Ask/Never)
- Include executable commands
- Specify exact tools needed
- Provide code style examples
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers: []
  changelog: "guidance-agent-examples.template.changelog.md"
---
-->
