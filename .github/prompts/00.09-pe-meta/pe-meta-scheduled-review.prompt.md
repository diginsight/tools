---
name: pe-meta-scheduled-review
description: "Lightweight periodic PE health check — detects stale artifacts, runs a scoped health review, proposes fixes, and updates the review log. Designed for weekly use with minimal cognitive load."
agent: agent
model: claude-opus-4.6
tools:
  - read_file
  - grep_search
  - file_search
  - list_dir
  - replace_string_in_file
handoffs:
  - label: "Audit Ecosystem Health"
    agent: pe-meta-validator
    send: true
  - label: "Apply Targeted Fixes"
    agent: pe-meta-optimizer
    send: true
agents: ['*']
argument-hint: 'Optional: "--scope context|agents|prompts|instructions|skills|hooks|snippets|templates [bundle=accept]" to limit review. Default: auto-detect stale areas.'
goal: "Detect stale PE artifacts and run scoped health reviews with minimal cognitive load"
scope:
  covers:
    - "Staleness detection via last_updated dates and review log"
    - "Scoped health review on auto-detected stale areas"
    - "Reliability spot-check (`--dim reliability`, `D28-reproducibility` through `D35-portability-boundary`) included in the auto-rotation cycle"
    - "Fix proposal and targeted application via meta-optimizer"
    - "Phase 0b — domain coherence (per 04.05-pe-meta-invocation-gates.md)"
    - "bundle=accept consent token recognition on multi-domain stale-area scopes"
  excludes:
    - "Full 8-phase pipeline (pe-meta-review handles this)"
    - "Release-driven monitoring (pe-meta-review --source <url> handles this)"
boundaries:
  - "MUST auto-detect stale areas before auditing"
  - "MUST summarize context before handoffs (>8K token trigger)"
  - "MUST update review log after completion"
  - "Phase 0b is NOT skippable; --skip domain-coherence is rejected with CF-05; Phase 0b runs on the resolved stale-area set BEFORE delegating to meta-validator/meta-optimizer"
  - "bundle=accept is the ONLY valid consent token (closed set); recorded on first-line `Resolved invocation:` log"
  - "Phase 0a CF-05 artifact-type/path consistency does NOT apply at this orchestrator-level layer; when this prompt delegates to pe-meta-review, Phase 0b is enforced by the delegate as well"
rationales:
  - "Lightweight design optimized for weekly execution reduces review fatigue"
  - "Auto-detecting stale areas avoids re-checking healthy artifacts"
---

# Scheduled PE Review

Lightweight periodic review optimized for weekly execution. Detects what's stale → audits only stale areas → proposes fixes → updates the review log.

> **v15.4 alignment.** This prompt rejects `--mode` and is therefore **exempt** from the vision v15.4 `apply = plan + execute` contract and the eighth canonical parameter `--plan-file` (applicability matrix: `--plan-file` ❌ for the Scheduled family). It is likewise **exempt** from the § Iteration budget checkpoint contract — assessment-only / scheduled families do not materialize an apply-pipeline plan or emit a `spillover=` marker. It proposes fixes (Phase 3) and applies low-risk ones (Phase 4) within its own bounded scan, not via the autonomous per-cycle change cap, so no plan artifact is produced.

**Design principle**: Minimal user input. Run `/pe-meta-scheduled-review` with no arguments for a full auto-detected review, or add `--scope <type>` to focus on one artifact type.

## Handoff Data Contracts

| Transition | Strategy | Include | Exclude | Max tokens |
|---|---|---|---|---|
| **Orchestrator → meta-validator** (Phase 2) | send: true | Stale areas from Phase 1, scope, audit dimensions | Staleness scan raw data | ≤1,000 |
| **meta-validator → Orchestrator** | Structured report | Severity-scored findings with fix recommendations | Full audit analysis | ≤1,500 |
| **Orchestrator → meta-optimizer** (Phase 4) | send: true | Approved fixes only (file + specific change) | Rejected fixes, Phase 2 analysis | ≤500 |

## Summarization Protocol

| After Phase | Summarize to | Max tokens | Discard |
|---|---|---|---|
| Phase 0.5 (Effectiveness) | Pattern summary (recurring issues, failed workflows) | ≤200 | Individual log entries, raw dates |
| Phase 1 (Staleness) | Stale areas table + scope decision | ≤300 | Raw date comparisons, file listings |
| Phase 2 (Health review) | Severity-scored findings | ≤1,000 | Full audit analysis |
| Phase 3 (Fix proposal) | Approved fixes list | ≤500 | Rejected proposals, discussion |
| Phase 4 (Apply) | Applied/failed status per fix | ≤300 | Fix implementation details |

**Trigger**: Before EVERY handoff, estimate accumulated context. If >8,000 tokens: MUST summarize all prior phases to their "Summarize to" format before proceeding.

**📖 Full strategies:** `.copilot/context/00.00-prompt-engineering/02.02-context-window-and-token-optimization.md`

## Phase 0.5: Effectiveness log check

Read `.copilot/context/00.00-prompt-engineering/05.05-practical-effectiveness-log.md`.

1. Check for entries added since the last scheduled review (compare dates against the review log)
2. If entries exist, summarize patterns: recurring friction points, failed workflows, successful workflows
3. Surface any pattern that should inform Phase 2 audit priorities (e.g., a workflow that repeatedly fails suggests its agent or prompt needs targeted review)

**Output to user**: Brief effectiveness summary — what's been logged, any actionable patterns.

If no new entries → report "No effectiveness data since last review" and proceed to Phase 1.

## Phase 0b — Domain coherence

This prompt enforces the Phase 0b domain coherence gate defined in [`04.05-pe-meta-invocation-gates.md`](../../../.copilot/context/00.00-prompt-engineering/04.05-pe-meta-invocation-gates.md) (upstream authority: current vision document § Domain-coherent batching). The 3-tier metadata-first domain resolution algorithm, seed-vs-deps decision matrix, dispatch table, `bundle=…` closed set, and `bundle=accept` consent semantics are specified there and MUST NOT be re-implemented here.

**Locally true for this prompt:**

1. **Scope = resolved stale-area set.** The output of Phase 1 (auto-detected stale files or `--scope`-filtered subset) is the in-scope file set. Each stale file's declared `domain:` frontmatter is read first (Tier 1); `pe-domain-map.yaml` (Tier 2) and `unknown` (Tier 3) follow.
2. **Gate runs BEFORE Phase 2 (Health review).** Domain footprint is computed on the stale set, before delegating to meta-validator. Multi-domain stale areas block (in `--mode apply` semantics) until `bundle=accept` consent or a per-domain split is selected.
3. **Consent token.** `bundle=accept` is the only valid token (closed set, case-sensitive). Recorded on the first-line `Resolved invocation:` log as `bundle=accepted-bundle`.
4. **Delegation note.** Phase 4 "Apply fixes" may delegate to `pe-meta-review` for non-trivial reconciliation; when it does, Phase 0b is re-enforced by the delegate on its own resolved scope (no double-prompting if the consent token is already provided).
5. **Phase 0a CF-05 does NOT apply at this layer.** Stale-area scanning is artifact-type-agnostic; CF-05 is enforced ONLY by per-artifact prompts when invoked by Phase 4.
6. **`--skip domain-coherence` rejected.** Phase 0b is not skippable; bypass multi-domain gating only via `bundle=accept`.

## Phase 1: Staleness scan

Read `05.04-meta-review-log.md` and scan `.copilot/context/00.00-prompt-engineering/*.md` YAML frontmatter for `last_updated` dates.

**Collect**:
1. Context files with `last_updated` > 90 days old
2. Instruction files with `last_updated` > 90 days old
3. Instruction file cascade check: for each instruction file with `context_dependencies`, find the newest `last_updated` among context files in that folder — flag if any context file is newer than the instruction file
4. Agent files with `last_updated` > 90 days old (scan `.github/agents/**/*.agent.md` YAML frontmatter)
5. Agent file cascade check: for each agent file with `context_dependencies`, find the newest `last_updated` among context files in that dependency folder — flag if any context file is newer than the agent file
6. Skill SKILL.md files with `last_updated` > 90 days old (if `version`/`last_updated` present in YAML)
7. Template files: scan `.github/templates/` bottom `template_metadata` HTML comments for `last_updated` — flag templates > 90 days old or missing bottom metadata
8. Time since last plan-mode run (from review log)
9. Time since last apply-mode run (from review log)
10. Any unprocessed VS Code/Copilot releases (from "Last processed versions" table)

**If `--scope` is specified**: Limit scan to artifacts of that type only.

**Output to user**: Brief staleness summary table — what's stale, how stale, and what this review will cover.

```markdown
### Staleness scan results

| Area | Stale items | Oldest | Action |
|---|---|---|---|
| Context files | [N] files > 90 days | [file] ([N] days) | Health review |
| Instruction files | [N] files > 90 days | [file] ([N] days) | Health review |
| Instruction cascade | [N] files with newer context deps | [file] | Re-validate |
| Agent files | [N] files > 90 days | [file] ([N] days) | Health review |
| Agent cascade | [N] files with newer context deps | [file] | Re-validate |
| Skills | [N] files > 90 days | [file] ([N] days) | Health review |
| Templates | [N] files > 90 days or missing metadata | [file] ([N] days) | Health review |
| Last plan-mode run | [N] days ago | — | Re-run |
| Last apply-mode run | [N] days ago | — | Informational |

**Review scope**: [auto-detected areas or user-specified scope]
```

If nothing is stale and last plan-mode run was < 14 days ago → report "System is current" and skip to Phase 5 (log update only).

## Phase 1.3: Override follow-up check

Read `05.04-meta-review-log.md` → "Override History" table.

1. Find entries where "Follow-up result" is blank (overrides pending review)
2. For each pending override:
   a. Read the file referenced in the finding
   b. Assess whether the overridden issue has manifested as a real problem (e.g., broken capabilities, consistency issues, user-reported friction)
   c. Record the outcome in the "Follow-up result" column:
      - **"No issues observed"** — the override was safe
      - **"Issue manifested: [description]"** — the override caused problems; add the issue to Phase 2 findings for remediation
      - **"Inconclusive — extend follow-up"** — update "Follow-up due" to the next scheduled review
3. Report findings to user before proceeding

If no pending overrides → report "No override follow-ups pending" and proceed.

## Phase 1.5: Paradigm challenge (conditional)

**Runs every 4th scheduled review.** Check the "Paradigm Challenge Tracking" section in `05.04-meta-review-log.md` to determine the current scheduled review count. If the count is a multiple of 4 (4, 8, 12, ...), run this phase. Otherwise skip to Phase 2.

**Goal**: Prevent local-optima convergence by periodically questioning whether the current PE architecture is still the most effective structure.

1. **State the question**: "Is the current PE architecture (7 artifact types, triad agents, multi-tier context hierarchy) still the most effective structure?"
2. **Research alternatives**: Use `fetch_webpage` on the curated sources from the "Authoritative Sources" table in `05.04-meta-review-log.md` to search for PE frameworks in the Copilot ecosystem that use different approaches
3. **Compare**: Evaluate any alternative approaches against the current system on three dimensions:
   - **Reliability** — repeatable, consistent results across executions
   - **Effectiveness** — goal achievement quality
   - **Efficiency** — token and time cost
4. **Report findings**: Present a brief comparison table. Do NOT propose changes unless a different approach demonstrably outperforms the current system on at least 2 of 3 dimensions
5. **Update tracking**: Record the run in the "Paradigm Challenge Tracking" table in `05.04-meta-review-log.md`

**Output to user**: Brief paradigm challenge summary — what was evaluated, whether any alternative outperforms current architecture, and the recommendation (keep/investigate further/propose changes).

## Phase 2: Scoped health review

Delegate to `@meta-validator` in Ecosystem Audit mode.

- **Scope**: Only the stale areas identified in Phase 1 (or user-specified `--scope`)
- **Dimensions**: `coherence+structure+references+budgets`. The auto-rotation cycle additionally includes `--dim reliability` (`D28-reproducibility` through `D35-portability-boundary`) every 2nd run to surface process-reliability regressions (reproducibility, regression-protection, metadata-guard, multipass-validation-invariant, rollback-readiness, boundary-actionability, autonomy-calibration, portability-boundary). Override with `--dim` to pin a specific group.
- **Output**: Severity-scored findings with fix recommendations

If zero CRITICAL or HIGH issues → report "No action needed" and skip to Phase 5.

## Phase 3: Fix proposal

For each CRITICAL or HIGH finding from Phase 2:

1. Identify the affected file and its dependents (via the `dependency-tracking` file — see 00.00-context-structure-index.md → Functional Categories in `.copilot/context/00.00-prompt-engineering/`)
2. Propose a specific fix (what to change, not just what's wrong)
3. Classify the fix: `auto-fix` (safe to apply) or `manual-review` (needs user judgment)

Present fix table to user:

```markdown
### Proposed fixes

| # | File | Issue | Severity | Fix | Type |
|---|---|---|---|---|---|
| 1 | [file] | [issue] | CRITICAL | [fix description] | auto-fix |
| 2 | [file] | [issue] | HIGH | [fix description] | manual-review |

Apply all auto-fixes? (yes/no/select)
```

**Wait for user approval before proceeding.**

## Phase 4: Apply fixes

For approved fixes only:
- Apply changes using appropriate builder agents or direct edits (for simple fixes like updating cross-references or dates)
- Validate each changed file — max 3 files between validation checkpoints
- If any fix fails validation → revert that fix and report

## Phase 5: Update review log

**Always runs**, even if no fixes were needed.

Update `.copilot/context/00.00-prompt-engineering/05.04-meta-review-log.md`:
1. Append a new entry under "Plan-mode runs" with date, scope, dimensions, findings, changes, health score
2. Update the `last_updated` date in the YAML frontmatter to today
3. Increment the scheduled review count in the "Paradigm Challenge Tracking" table — add a new row with today's date, the incremented review number, whether Phase 1.5 ran, key findings (if any), and action taken

### Log rotation check

After updating the review log, check whether rotation is needed (📖 thresholds in `01.06-system-parameters.md` → Review Log Management):

1. Count total lines in `05.04-meta-review-log.md`
2. If total exceeds 200 lines:
   a. Move entries older than 6 months to `.copilot/temp/pe-review-archive-{year}.md` (create if it doesn't exist, append if it does)
   b. Keep only the last 6 entries per mode (plan-mode, apply-mode, optimize-mode)
   c. Preserve the "Last Processed Versions" table and "Last Apply-Mode Run File Manifest" section — these are NEVER rotated
3. If total is ≤200 lines: skip rotation

## CRITICAL BOUNDARIES

### Always Do
- Run Phase 1 staleness scan before any health review
- Present findings and fix proposals before applying changes
- Update the review log after every run (even if no issues found)
- Report a final summary with health score
- Run Phase 0b on the resolved stale-area set BEFORE delegating to meta-validator
- Echo `bundle=…` marker on the first line of the `Resolved invocation:` log

### Ask First
- Applying fixes classified as `manual-review`
- Changes to files with 6+ dependents
- Applying more than 5 fixes in a single run

### Never Do
- **NEVER apply fixes without user approval**
- **NEVER modify top YAML frontmatter** in article files
- **NEVER skip the review log update**
- **NEVER remove capabilities** — only extend, refine, or deprecate
- **NEVER bypass Phase 0b** — `--skip domain-coherence` is rejected with CF-05; bypass multi-domain gating ONLY via `bundle=accept`
- **NEVER accept consent tokens other than `bundle=accept`** (case-sensitive, closed set)

---

## 🔄 Error Recovery Workflows

**📖 Recovery pattern:** [04.03-production-readiness-patterns.md](.copilot/context/00.00-prompt-engineering/04.03-production-readiness-patterns.md)

Scheduled-review-specific recovery:
- **meta-validator unavailable** → Retry once, then report partial scan with staleness data only
- **Fix fails validation** → Revert fix, mark as "deferred" in review log
- **Review log missing or corrupted** → Create initial log entry, proceed with full staleness scan
- **Can't determine staleness** → Default to full health-review scope, warn user

---

## 📋 Response Management

**📖 Response patterns:** [04.03-production-readiness-patterns.md](.copilot/context/00.00-prompt-engineering/04.03-production-readiness-patterns.md)

Scheduled-review-specific scenarios:
- **Nothing stale** → "System is current — last plan-mode run [N] days ago. No action needed."
- **Review log not found** → "Review log missing. Creating initial entry and running full scan."
- **Fix classified as manual-review** → Present to user with context, don't auto-apply

---

## 🧪 Embedded Test Scenarios

| # | Scenario | Expected Behavior |
|---|---|---|
| 1 | Everything current | Staleness scan finds nothing → log updated with "clean", no health review |
| 2 | Stale context files found | Health review runs on stale scope → fixes proposed → user approves → applied |
| 3 | Manual-review fix | User prompted for decision → approved: applied, rejected: deferred in log |
| 4 | Pending override follow-ups | Phase 1.3 finds unresolved overrides → evaluates each → updates Follow-up result in review log |
| 5 | Override issue manifested | Phase 1.3 detects override caused problem → adds to Phase 2 findings for remediation |
| 6 | Stale or missing template metadata | Phase 1 flags templates with old `last_updated` or missing bottom metadata → included in Phase 2 scope → fix proposals generated |

<!--
prompt_metadata:
  version: "2.3.0"
  last_updated: "2026-06-04"
-->
