---
name: pe-meta-create-update
description: "Create new or update existing PE-for-PE artifacts with pre-change guards, category compliance, and post-change reconciliation — skips research, goes directly to build → double validate"
agent: agent
model: claude-opus-4.6
tools:
  - semantic_search
  - read_file
  - file_search
  - grep_search
  - list_dir
  - create_file
  - replace_string_in_file
handoffs:
  - label: "Build Artifact"
    agent: pe-con-builder
    send: true
  - label: "Structural Validation"
    agent: pe-con-validator
    send: true
  - label: "Ecosystem Coherence"
    agent: pe-meta-validator
    send: true
argument-hint: '<artifact-type> <file-path-or-description> [--plan-file <path>] [bundle=accept] — e.g., "agent .github/agents/00.09-pe-meta/pe-meta-optimizer.agent.md" or "context file for category enforcement patterns"'
goal: "Create or update PE-for-PE artifacts with strategic pre-change guards, category compliance enforcement, and post-change metadata reconciliation"
scope:
  covers:
    - "Direct creation of new PE-for-PE artifacts (build → double validate)"
    - "Updates to existing PE-for-PE artifacts with pre-change guard protocol"
    - "Post-change reconciliation (metadata, categories, dependency map)"
    - "Phase 0b — domain coherence (per 04.05-pe-meta-invocation-gates.md)"
    - "bundle=accept consent token recognition on multi-domain scopes"
  excludes:
    - "Requirements research and use case challenge (use /pe-meta-design for that)"
    - "Domain artifacts (use /pe-con-create-update for those)"
    - "Ecosystem-wide audits (use /pe-meta-review for those)"
boundaries:
  - "MUST load PE-strategic context before building"
  - "MUST run pre-change guard for updates to existing files"
  - "MUST run post-change reconciliation after every change"
  - "MUST run double validation (structural + strategic)"
  - "Only for PE-for-PE artifacts — redirect domain artifacts to /pe-con-create-update"
  - "Phase 0b is NOT skippable; --skip domain-coherence is rejected with CF-05; Phase 0b runs on the resolved file set BEFORE building or delegating"
  - "bundle=accept is the ONLY valid consent token (closed set); recorded on first-line `Resolved invocation:` log"
  - "Phase 0a CF-05 artifact-type/path consistency does NOT apply at this orchestrator-level layer (artifact-type is supplied as the first positional argument and validated by dispatch table); enforced by per-artifact prompts when delegated"
  - "`apply = plan + execute` (vision v15.4): this prompt materializes a build/change plan (pre-change guard + category + build spec) then executes it via pe-con-builder. CREATE has no baseline (fresh mode); UPDATE reconciles against the existing artifact's metadata (escalate-not-overwrite human-authored decisions)"
  - "`--plan-file <path>` (vision v15.4) sets plan location/identity ONLY and never decides regenerate-vs-trust; a same-conversation just-generated plan is an implicit baseline. Default auto-name path — see [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md)"
  - "Model-routing seam (vision v15.4): the plan/reconcile step runs on the reasoning-grade `model:` declared here; delegated execution (pe-con-builder) carries its own standard-grade `model:` — no mid-prompt switching"
rationales:
  - "Skipping research for speed when requirements are already known, but NOT skipping strategic guards"
  - "Pre-change guards prevent drift from vision alignment during updates"
  - "Post-change reconciliation keeps metadata current — the closed feedback loop"
---

# PE-for-PE Artifact Create/Update

Create new or update existing PE artifacts that serve the PE system. Skips research but enforces full strategic guards — pre-change validation, category compliance, post-change reconciliation.

**When to use this vs `/pe-con-create-update`:**
- This prompt → PE infrastructure (pe-* agents/prompts, PE context files, pe-* instructions/skills)
- `/pe-con-create-update` → domain artifacts (article-writing, documentation, devops)

**When to use this vs `/pe-meta-design`:**
- This prompt → requirements are already clear, you know what to build/change
- `/pe-meta-design` → uncertain requirements, need research and use case challenge

> **v15.4 alignment.** This prompt honors the vision v15.4 **`apply = plan + execute`** contract: it materializes a build/change plan (pre-change guard, category compliance, build spec) and then executes it. CREATE runs in **fresh** mode (no baseline); UPDATE **reconciles** against the existing artifact (escalate-not-overwrite human-authored decisions). `--plan-file <path>` (the eighth canonical parameter) sets plan location/identity ONLY and never decides regenerate-vs-trust — see [pe-meta-plan-file-contract.md](../../prompt-snippets/pe-meta-plan-file-contract.md). The model-routing seam keeps plan/reconcile on this prompt's reasoning-grade model while delegated execution (pe-con-builder) carries its own standard-grade `model:`.

## Phase 0b — Domain coherence

This prompt enforces the Phase 0b domain coherence gate defined in [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) (upstream authority: current vision document § Domain-coherent batching). The 3-tier metadata-first domain resolution algorithm, seed-vs-deps decision matrix, dispatch table, `bundle=…` closed set, and `bundle=accept` consent semantics are specified there and MUST NOT be re-implemented here.

**Locally true for this prompt:**

1. **Scope = resolved target.** For CREATE: the new file's path determines its domain (Tier 1: declared `domain:` in the spec; Tier 2: `pe-domain-map.yaml`; Tier 3: `unknown`). For UPDATE: the existing file's declared `domain:` is Tier 1. Pre-change guard inputs (dependency closure) extend the in-scope set when `--deps` is implied.
2. **Gate runs BEFORE Phase 2 (Build).** Domain footprint is computed before delegating to pe-con-builder. Multi-domain footprints block (or require `bundle=accept`) so a single create/update operation doesn't silently span domains without consent.
3. **Consent token.** `bundle=accept` is the only valid token (closed set, case-sensitive). Recorded on the first-line `Resolved invocation:` log as `bundle=accepted-bundle`.
4. **Phase 0a CF-05 does NOT apply at this layer.** The `<artifact-type>` first positional argument carries explicit type intent and is validated against the dispatch table at Phase 0; CF-05 (path/prefix mismatch) is enforced ONLY by per-artifact prompts.
5. **`--skip domain-coherence` rejected.** Phase 0b is not skippable; bypass multi-domain gating only via `bundle=accept`.

## Process

### Phase 0: Load PE-domain strategic context

1. **Load vision document** — `read_file` on the current vision document in `06.00-idea/self-updating-prompt-engineering/` (find `*-vision.v*.md` with highest version)
2. **Load 00.00-context-structure-index.md** — for Functional Categories and required categories
3. **Load strategic review criteria** — `read_file` on the `pe-strategic-review` files from `.copilot/context/00.00-prompt-engineering/` (see 00.00-context-structure-index.md → Functional Categories)
4. **Load dependency map** — `read_file` on the `dependency-tracking` files from `.copilot/context/00.00-prompt-engineering/`
5. **Load dispatch table** — `read_file` on `.github/templates/00.00-prompt-engineering/artifact-type-dispatch.template.md`
6. **Determine operation mode**: CREATE (file doesn't exist) or UPDATE (file exists)

### Phase 1: Pre-change guard (UPDATE mode only)

If updating an existing file:

1. **Read the target artifact** completely
2. **Read its metadata**: `goal:`, `scope:`, `boundaries:`, `rationales:`
3. **Compare proposed change** against each metadata field:
   - Contradicts `goal:`? → **BLOCK** — report to user, ask for override
   - Contradicts `scope:`? → **BLOCK** — report scope drift
   - Contradicts `boundaries:`? → **BLOCK** — report boundary violation
   - Contradicts `rationales:`? → **ESCALATE** — require replacement rationale
4. **Check category impact** — if this file is listed in STRUCTURE-README Functional Categories, will the change affect category coverage?
5. **Check dependency blast radius** — how many artifacts depend on this file?

If BLOCKED: present the contradiction and ask for user override or revised change description.

### Phase 2: Build with PE-strategic constraints

Delegate to `@pe-con-builder` with PE-domain constraints:

**For CREATE:**
- All context file references → Level 1.5 (category) pattern
- Metadata contracts → goal, scope, boundaries, rationales, version REQUIRED
- N-1 separation → apply per the N-1 adoption table in `05.06`
- Quality bar → exemplary level (boundaries ≥5/2/3, test scenarios ≥3)
- Naming → `pe-` namespace prefix

**For UPDATE:**
- Pass pre-change guard results as constraints
- Preserve existing metadata unless explicitly changing it
- Ensure all new references use Level 1.5 (category) pattern

### Phase 3: Double validation

1. **Structural validation** — delegate to `@pe-con-validator`
2. **Strategic validation** — check against `05.06` criteria:
   - Vision alignment ✅/❌
   - Category references ✅/❌
   - PE quality bar ✅/❌
   - N-1 separation ✅/❌/N/A
   - Self-update readiness ✅/❌

If validation fails: hand back to `@pe-con-builder` (max 3 iterations).

### Phase 4: Post-change reconciliation (MANDATORY)

After the artifact is created or updated:

1. **Metadata reconciliation** on the modified file:
   - `version:` → bump (patch for non-breaking, minor for additive, major for breaking)
   - `last_updated:` → today's date
   - `scope.covers:` → verify topics match content
   - `goal:` → verify still accurate
2. **STRUCTURE-README updates** (if context file was created/modified):
   - Add to File Index if new
   - Assign to/update Functional Category
   - Update file count
3. **Dependency map update** — if new dependencies were added or removed, note for manual update of `05.01`

Report reconciliation summary to user.

## Handoff Data Contracts

| Transition | Strategy | Include | Exclude | Max tokens |
|---|---|---|---|---|
| **Orchestrator → pe-con-builder** | send: true | User request or change spec, PE constraints (L1.5 mandatory, metadata contracts, N-1, quality bar), pre-change guard results (for updates) | Vision document, STRUCTURE-README, strategic criteria full text | ≤1,500 |
| **Orchestrator → pe-con-validator** | send: true | File path + "validate" | Builder reasoning | ≤200 |
| **Orchestrator → pe-meta-validator** | send: true | File path + "ecosystem coherence check" + dependency scope | Structural validation details | ≤500 |

## Response Management

- **Not a PE artifact** → "This file is not PE infrastructure. Use `/pe-con-create-update` instead."
- **Pre-change guard blocks** → "The proposed change contradicts the artifact's [metadata field]: [specific contradiction]. Override? Or revise the change?"
- **Category regression detected** → "This change would leave the `[category]` category with no mapped files. This is blocked by the required_categories contract."
- **All checks pass** → Report success with strategic compliance summary + reconciliation details

## Embedded Test Scenarios

| # | Scenario | Expected Behavior |
|---|---|---|
| 1 | Create new PE agent (happy path) | Phase 0 loads strategic context → Phase 2 build with PE constraints → Phase 3 double validate → Phase 4 reconcile metadata + update dependency map |
| 2 | Update existing file — pre-change guard blocks | Phase 1 reads metadata → proposed change contradicts goal → BLOCK → reports contradiction → asks user for override |
| 3 | Category regression detected | Update would remove file from a required category → pre-change guard flags category coverage regression → BLOCK |
| 4 | Non-PE artifact (redirect) | Detects non-PE path → "Use /pe-con-create-update instead" → STOP |

<!--
prompt_metadata:
  version: "2.3.0"
  last_updated: "2026-06-04"
-->
