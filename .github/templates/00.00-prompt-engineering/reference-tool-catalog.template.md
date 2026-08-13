---
description: "Tool catalog reference for agent tool alignment verification"
---

# Tool Catalog Reference

**Purpose**: Detailed tool selection examples, composition patterns, anti-patterns, and recipes for prompt/agent authoring.
**Source**: Reference companion to `.copilot/context/00.00-prompt-engineering/01.04-tool-composition-guide.md`.
**Use when**: Creating or updating agent/prompt files and needing detailed tool selection guidance beyond the summary rules.

---

## Tool Categories — Composition Patterns

### Read-Only Composition Patterns (Safe for `agent: plan`)

```yaml
# Pattern 1: Research-first workflow
tools: ['semantic_search', 'read_file', 'grep_search']
# Workflow: semantic_search (find candidates) → read_file (deep dive) → grep_search (verify patterns)

# Pattern 2: Targeted analysis  
tools: ['file_search', 'read_file', 'get_errors']
# Workflow: file_search (find file) → read_file (inspect) → get_errors (validate)

# Pattern 3: Pattern discovery
tools: ['grep_search', 'read_file', 'list_dir']
# Workflow: grep_search (find all occurrences) → read_file (analyze context) → list_dir (check related files)
```

### Write Composition Patterns (Requires `agent: agent`)

```yaml
# Pattern 1: Builder agent (research → create)
tools: ['semantic_search', 'read_file', 'create_file']
# Workflow: semantic_search (find similar) → read_file (analyze patterns) → create_file (generate new)

# Pattern 2: Updater agent (find → modify)
tools: ['grep_search', 'read_file', 'replace_string_in_file']
# Workflow: grep_search (find targets) → read_file (verify context) → replace_string_in_file (update)

# Pattern 3: Batch processor
tools: ['file_search', 'read_file', 'multi_replace_string_in_file']
# Workflow: file_search (find all targets) → read_file (validate) → multi_replace (apply changes)
```

### External Composition Patterns (Use with caution)

```yaml
# Pattern 1: Research with external sources
tools: ['fetch_webpage', 'semantic_search', 'read_file']
# Workflow: fetch_webpage (get official docs) → semantic_search (find local context) → read_file (verify consistency)

# Pattern 2: Best practice discovery
tools: ['github_repo', 'semantic_search', 'grep_search']
# Workflow: github_repo (find patterns) → semantic_search (find similar local code) → grep_search (verify adoption)

# Pattern 3: Build and validate
tools: ['read_file', 'create_file', 'run_in_terminal']  
# Workflow: read_file (check config) → create_file (generate) → run_in_terminal (test build)
```

---

## Tool Selection by Agent Role — Full Descriptions

### Researcher Agent

**Role**: Analyze requirements, discover patterns, gather context

**Recommended Tools**:
```yaml
---
description: "Research specialist for requirements and pattern discovery"
agent: plan  # Read-only enforced
tools:
  - semantic_search  # Find relevant existing code
  - grep_search      # Locate specific patterns
  - read_file        # Deep dive into files
  - file_search      # Find files by name
  - fetch_webpage    # Research official docs (optional)
  - github_repo      # Find external best practices (optional)
---
```

**Typical Workflow**:
1. `semantic_search` to find 3-5 relevant similar files
2. `read_file` to analyze each candidate thoroughly
3. `grep_search` to identify common patterns (e.g., all files using specific YAML field)
4. `fetch_webpage` to verify against official documentation
5. Output: Research report with findings + recommendations

**Anti-patterns**:
- ❌ Including `create_file` or `replace_string_in_file` (violates read-only role)
- ❌ Including `run_in_terminal` (researchers shouldn't execute code)
- ❌ Too few tools (need at least `semantic_search` + `read_file` for effective research)

### Builder Agent

**Role**: Generate new files based on research and templates

**Recommended Tools**:
```yaml
---
description: "Prompt file generator following validated patterns"
agent: agent  # Default, allows file creation
tools:
  - read_file           # Access templates and context
  - semantic_search     # Find similar patterns for consistency
  - create_file         # Generate new prompts/agents
  - file_search         # Locate templates
---
```

**Typical Workflow**:
1. `read_file` to load template (e.g., `prompt-simple-validation.template.md`)
2. `read_file` to access research report from researcher agent
3. `semantic_search` to find 2-3 similar existing files for consistency
4. `create_file` to generate new prompt/agent with proper structure
5. Handoff to validator agent

**Anti-patterns**:
- ❌ Including `replace_string_in_file` (builders create, updaters modify)
- ❌ Including `run_in_terminal` (builders don't test/execute)
- ❌ Omitting `semantic_search` (builders need context for consistency)

### Validator Agent

**Role**: Quality assurance, syntax checking, best practice verification

**Recommended Tools**:
```yaml
---
description: "Quality assurance and optimization specialist"
agent: plan  # Read-only enforced
tools:
  - read_file     # Inspect prompt/agent files
  - grep_search   # Find potential issues across files
  - get_errors    # Check for syntax errors (if applicable)
---
```

**Typical Workflow**:
1. `read_file` to load file for validation
2. Parse YAML frontmatter (verify required fields)
3. Check three-tier boundaries structure
4. Verify tool list matches agent type
5. `grep_search` to check for anti-patterns
6. Output: Validation report with pass/fail + recommendations

**Anti-patterns**:
- ❌ Including any write tools (validators report, they don't fix)
- ❌ Using `agent: agent` (must be read-only)
- ❌ Including `semantic_search` (expensive, usually not needed for validation)

### Updater Agent

**Role**: Apply targeted modifications to existing files

**Recommended Tools**:
```yaml
---
description: "Specialized updater for existing prompt and agent files"
agent: agent  # Needs write access
tools:
  - read_file                    # Load current file
  - grep_search                  # Find modification targets
  - replace_string_in_file       # Single targeted update
  - multi_replace_string_in_file # Batch updates
---
```

**Typical Workflow**:
1. `read_file` to load file + validation report from validator
2. `grep_search` to locate specific sections needing updates
3. `replace_string_in_file` for single edit, or `multi_replace_string_in_file` for batch
4. Handoff to validator for re-validation

**Anti-patterns**:
- ❌ Including `create_file` (updaters modify, builders create)
- ❌ Using `agent: plan` (updaters need write access)
- ❌ Omitting `grep_search` (updaters need precise targeting)

### Orchestrator Prompt

**Role**: Coordinate handoffs, minimal direct operations

**Recommended Tools**:
```yaml
---
description: "Orchestrates prompt creation workflow via agent handoffs"
tools:
  - read_file        # For Phase 1 requirements analysis only
  - semantic_search  # For determining which agents to invoke
handoffs:
  - label: "Research Requirements"
    agent: prompt-researcher
    send: true
  - label: "Build Prompt"
    agent: prompt-builder
    send: false
  - label: "Validate Prompt"
    agent: prompt-validator
    send: true
---
```

**Typical Workflow**:
1. Phase 1: Gather requirements (minimal `read_file` if needed)
2. Handoff to `prompt-researcher` with requirements
3. Wait for research report
4. Handoff to `prompt-builder` with research
5. Wait for draft prompt
6. Handoff to `prompt-validator` with draft
7. Present validation report to user for approval

**Anti-patterns**:
- ❌ Including write tools (orchestrator delegates, doesn't implement)
- ❌ No handoffs defined (defeats purpose of orchestration)
- ❌ Too many tools (orchestrator should be minimal)

---

## Tool Composition Anti-Patterns — Detailed Examples

### ❌ Anti-Pattern 1: Over-Scoping (Tool Bloat)

**Problem**: Including every possible tool "just in case"

```yaml
# Bad: Researcher agent with unnecessary tools
---
agent: plan
tools:
  - semantic_search
  - grep_search
  - read_file
  - create_file         # ❌ Researchers don't create
  - replace_string_in_file  # ❌ Researchers don't modify
  - run_in_terminal     # ❌ Researchers don't execute
---
```

**Fix**: Only include tools essential for role
```yaml
# Good: Focused tool list
---
agent: plan
tools:
  - semantic_search
  - grep_search
  - read_file
---
```

### ❌ Anti-Pattern 2: Under-Scoping (Missing Critical Tools)

**Problem**: Omitting tools needed for role, forcing workarounds

```yaml
# Bad: Builder agent without read access
---
agent: agent
tools:
  - create_file  # ❌ How to access templates without read_file?
---
```

**Fix**: Include tools for complete workflow
```yaml
# Good: Builder can access templates and research
---
agent: agent
tools:
  - read_file       # Access templates and research reports
  - semantic_search # Find similar patterns
  - create_file     # Generate new files
---
```

### ❌ Anti-Pattern 3: Conflicting Agent Type + Tools

**Problem**: Declaring `agent: plan` but including write tools

```yaml
# Bad: Read-only agent type with write tool
---
agent: plan  # ❌ Says "read-only"
tools:
  - read_file
  - replace_string_in_file  # ❌ But has write tool
---
```

**Fix**: Align agent type with tool capabilities
```yaml
# Good: Read-only agent with read-only tools
---
agent: plan
tools:
  - read_file
  - grep_search
  - semantic_search
---
```

### ❌ Anti-Pattern 4: Redundant Tool Definitions

**Problem**: Defining same tools at prompt and agent level unnecessarily

```yaml
# Agent file
---
agent: agent
tools: ['read_file', 'semantic_search', 'create_file']
---

# Prompt file (same agent selected)
---
tools: ['read_file', 'semantic_search', 'create_file']  # ❌ Redundant
---
```

**Fix**: Only define prompt-level tools if restricting agent's normal scope
```yaml
# Prompt file (ONLY if restricting)
---
tools: ['read_file']  # ✅ Intentionally restricting to read-only for this task
agent: plan           # ✅ Enforce read-only behavior
---
```

### ❌ Anti-Pattern 5: External Tools Without Boundaries

**Problem**: Allowing `run_in_terminal` without constraints

```yaml
# Bad: Unrestricted terminal access
---
tools:
  - read_file
  - create_file
  - run_in_terminal  # ❌ No boundaries defined
---

## Process
1. Create file
2. Run terminal command  # Could execute anything!
```

**Fix**: Define explicit boundaries for dangerous tools
```yaml
# Good: Terminal access with boundaries
---
tools:
  - read_file
  - create_file
  - run_in_terminal
---

## Process
1. Create file
2. Run terminal command to validate syntax

## Boundaries

### ✅ Always Do
- Validate terminal commands before execution
- Use read-only operations when possible (e.g., `cat`, `ls`)

### ⚠️ Ask First  
- Run build commands (`dotnet build`, `npm install`)
- Execute test suites

### 🚫 Never Do
- NEVER run destructive commands (`rm -rf`, `format`, etc.)
- NEVER execute commands that modify system configuration
- NEVER run commands outside the workspace directory
```

---

## Tool Combination Recipes

### Recipe 1: Pattern Discovery (Read-Only)

**Goal**: Find and analyze common patterns across existing files

```yaml
tools: ['grep_search', 'semantic_search', 'read_file', 'list_dir']
```

**Workflow**:
```markdown
1. `grep_search` with regex to find all files matching pattern
   Example: Find all prompts using handoffs: `grep_search("handoffs:", ".github/prompts/**/*.md")`

2. `semantic_search` to understand context and variations
   Example: "What handoff patterns are used in validation prompts?"

3. `read_file` to deep dive into top 3-5 results
   Example: Read each file to extract handoff configurations

4. `list_dir` to check for related files
   Example: List templates directory to see what's available

5. Output structured report with findings
```

### Recipe 2: Template-Based Generation (Write)

**Goal**: Create new file based on template and research

```yaml
tools: ['read_file', 'file_search', 'semantic_search', 'create_file']
```

**Workflow**:
```markdown
1. `file_search` to locate appropriate template
   Example: Find "prompt-simple-validation.template.md"

2. `read_file` to load template content
   Example: Read template to understand structure

3. `semantic_search` to find 2-3 similar existing files
   Example: "Find validation prompts similar to grammar check"

4. `read_file` to analyze similar files for patterns
   Example: Extract common YAML fields and boundary structures

5. `create_file` to generate new file with customizations
   Example: Create new grammar validation prompt with proper structure
```

### Recipe 3: Targeted Update (Write)

**Goal**: Modify specific sections of existing file

```yaml
tools: ['read_file', 'grep_search', 'replace_string_in_file']
```

**Workflow**:
```markdown
1. `read_file` to load current file state
   Example: Read existing prompt to understand structure

2. `grep_search` to locate exact section needing update
   Example: Find "## Boundaries" section across file

3. `read_file` with narrow line range to get precise context
   Example: Read lines 45-60 to get 3 lines before/after target

4. `replace_string_in_file` to apply targeted change
   Example: Update "ask_first" boundary with new rule
```

### Recipe 4: Batch Consistency Update (Write)

**Goal**: Apply same change across multiple files

```yaml
tools: ['file_search', 'read_file', 'multi_replace_string_in_file']
```

**Workflow**:
```markdown
1. `file_search` to find all target files
   Example: "*.prompt.md" in .github/prompts/

2. `read_file` to load each file and validate change needed
   Example: Check if each has old YAML format

3. `multi_replace_string_in_file` to apply changes atomically
   Example: Update YAML frontmatter structure across all files in one operation
```

### Recipe 5: Research + Validation (Read-Only + External)

**Goal**: Verify local implementation against official sources

```yaml
tools: ['semantic_search', 'read_file', 'fetch_webpage', 'grep_search']
```

**Workflow**:
```markdown
1. `semantic_search` to find local implementation
   Example: "Find how we implement agent handoffs"

2. `read_file` to understand current approach
   Example: Read 2-3 agent files with handoffs

3. `fetch_webpage` to get official documentation
   Example: Fetch VS Code Copilot customization docs

4. `grep_search` to find discrepancies
   Example: Search for deprecated handoff patterns

5. Output comparison report with recommendations
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers: []
  changelog: "reference-tool-catalog.template.changelog.md"
---
-->
