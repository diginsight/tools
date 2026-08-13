# PE Artifact Maintenance Checklist

Step-by-step guide for running periodic reviews of the prompt engineering artifact system. Follow this checklist on the suggested cadence to keep artifacts current, coherent, and efficient.

## Maintenance Cadence

| Review | Prompt | Frequency | Trigger | Typical Duration |
|---|---|---|---|---|
| **Full system review** | `/meta-prompt-engineering-update healthcheck` | Biweekly | Scheduled | ~10 min |
| **Post-change review** | `/meta-prompt-engineering-update healthcheck [scope] references+coherence` | After major changes | Event-driven | ~5 min |
| **Optimization pass** | `/meta-prompt-engineering-update performancecheck` | As needed | After review finds issues | ~15 min |
| **Best practices update** | `/meta-prompt-engineering-update` | On VS Code releases | Event-driven | ~20 min |

## Biweekly Full Review

**When:** Every 2 weeks (suggest: 1st and 15th of each month)

### Steps

- [ ] **1. Run full review**: `/meta-prompt-engineering-update healthcheck`
  - No parameters = all scopes, all dimensions
  - Review the health score in the report

- [ ] **2. Check the review score**
  - ✅ Score ≥ 90: System healthy — no action needed
  - ⚠️ Score 70–89: Run `/meta-prompt-engineering-update performancecheck` for the flagged issues
  - ❌ Score < 70: Investigate CRITICAL findings manually

- [ ] **3. If fixes needed**: `/meta-prompt-engineering-update performancecheck`
  - Review the optimization plan before approving
  - Verify no capabilities lost after fixes

- [ ] **4. Update last review date** in this checklist (bottom of file)

## Post-Change Review

**When:** After adding/modifying/deleting context files, instructions, agents, or prompts

### Steps

- [ ] **1. Identify scope**: Which artifact type was changed?
- [ ] **2. Run targeted review**: `/meta-prompt-engineering-update healthcheck [scope] references+coherence`
  - Examples:
    - Changed a context file: `/meta-prompt-engineering-update healthcheck context references+coherence`
    - Changed an agent: `/meta-prompt-engineering-update healthcheck agents rules+structure`
    - Changed a prompt: `/meta-prompt-engineering-update healthcheck prompts references`

- [ ] **3. Verify dependency map** is current:
  - New artifact? → Must be in the `dependency-tracking` file (see 00.00-context-structure-index.md → Functional Categories)
  - Deleted artifact? → Must be removed from map
  - Changed references? → Map must reflect new refs

## VS Code Release Update

**When:** New VS Code or GitHub Copilot release with prompt engineering changes

### Steps

- [ ] **1. Gather release info**: Collect release notes URL or blog post
- [ ] **2. Run update prompt**: `/meta-prompt-engineering-update` with the URL
  - The prompt will:
    - Fetch and analyze the release notes
    - Identify which PE artifacts are affected
    - Propose updates with impact analysis

- [ ] **3. Review proposed changes** before approving
- [ ] **4. After updates applied**: `/meta-prompt-engineering-update healthcheck all coherence+references`
- [ ] **5. Update ROADMAP.md** with next review date

## Quick Reference: Review Dimensions

| Dimension | What it checks | Common issues found |
|---|---|---|
| `coherence` | Rules agree across layers | Different thresholds for same rule |
| `completeness` | No missing files, dep map current | New artifact not in dep map |
| `structure` | YAML valid, boundaries 3/1/2 | Missing boundary tiers |
| `rules` | Tool alignment, template-first compliance | plan mode with write tools |
| `references` | All links and handoffs resolve | Broken `📖` links, missing handoff targets |
| `budgets` | Files within token limits | Context file >375 lines |

## Quick Reference: Review Scopes

| Scope | Location scanned | File count (typical) |
|---|---|---|
| `all` | Everything below | ~50 |
| `context` | `.copilot/context/00.00-prompt-engineering/` | 18 |
| `instructions` | `.github/instructions/` | 4–6 |
| `agents` | `.github/agents/00.00-prompt-engineering/` | 12 |
| `prompts` | `.github/prompts/00.00-prompt-engineering/` | 11 |
| `skills` | `.github/skills/` | 2–3 |

## Review History

| Date | Type | Score | Issues Found | Action Taken |
|---|---|---|---|---|
| 2026-03-08 | Full system setup | N/A | Initial creation | Phases 1–6 completed |
| _next_ | | | | |
