---
title: "Article review workflow"
description: "Process for reviewing and updating existing articles to maintain accuracy, currency, and quality"
domain: "article-writing"
goal: "Define when and how to review existing articles so that content stays accurate, current, and aligned with quality standards — including scheduled cycles, triggered reviews, and common review scenarios"
scope:
  covers:
    - "Scheduled review cycles (by content type: technical docs, tutorials, concept articles, reference)"
    - "Triggered review conditions (version updates, reader reports, cross-reference changes, freshness drops)"
    - "Review process (triage, review, update, re-validate)"
    - "Content stability guide (stable vs. volatile content identification)"
    - "Common review scenarios (technology update, reader-reported issue, scheduled review, series update)"
  excludes:
    - "Article creation workflow (see workflows/01-article-creation-workflow.md)"
    - "Series-level review and consistency (see workflows/03-series-planning-workflow.md)"
    - "Freshness scoring formula and SLA tier definitions (see 02-validation-criteria.md)"
boundaries:
  - "MUST reference actual prompt files — no phantom/placeholder names"
  - "MUST NOT redefine freshness scoring — reference 02-validation-criteria.md as authoritative source"
  - "Review scenarios MUST include step-by-step procedures, not just one-line summaries"
rationales:
  - "Scheduled review cycles prevent silent content degradation — the most common documentation failure mode"
  - "Triggered reviews complement scheduled reviews by catching urgent changes between cycles"
  - "Content stability guide helps reviewers prioritize volatile content (API references, version-specific docs) over stable content (concept articles)"
  - "Common scenarios with step-by-step procedures were expanded because one-line summaries didn't provide actionable guidance"
---

# Article Review Workflow

**Purpose**: Process for reviewing and updating existing articles to maintain accuracy, currency, and quality.

**Referenced by**:
- `.github/prompts/01.00-article-writing/article-review-for-consistency-gaps-and-extensions.prompt.md`

---

## When to Review

### Scheduled Reviews

| Content Type | Review Cycle | Fact-Check | Link Check |
|-------------|-------------|------------|------------|
| Technical docs (version-specific) | Every 90 days | Every 30 days | Monthly |
| Tutorials / how-to guides | Every 90 days | Every 30 days | Monthly |
| Concept articles | Annually | Every 90 days | Quarterly |
| Reference material | With technology updates | Every 30 days | Quarterly |

### Triggered Reviews

Review immediately when:
- Technology has a major version update
- Reader reports an issue or inaccuracy
- Related articles are updated (cross-reference check)
- Content freshness score drops below 50 (see `02-validation-criteria.md`)
- More than 180 days since last review

---

## Review Process

```
Check Status → Prioritize → Research → Update → Re-validate → Publish
```

### Step 1: Check Current Status

Examine the bottom validation metadata (HTML comment) for:
- `last_run` timestamps — which validations are stale?
- Article status — published, draft, deprecated?
- Days since last update

**Freshness assessment**: Use the 5-signal freshness scoring from `02-validation-criteria.md`.

### Step 2: Prioritize Validations

| Priority | Dimension | Stale Threshold |
|----------|-----------|----------------|
| **Critical** | Facts, links | >30 days for technical content |
| **Important** | Grammar, readability | >7 days if content changed |
| **Optional** | Gap analysis, understandability | Run if major changes |

### Step 3: Review Content for Currency

- [ ] Version numbers current?
- [ ] APIs or features changed?
- [ ] Links all working?
- [ ] Code examples still valid?
- [ ] Best practices still current?
- [ ] Screenshots/images outdated?

### Step 4: Run Stale Validations

**Automated approach**: Run `article-review-for-consistency-gaps-and-extensions.prompt.md` — handles reference verification, gap discovery, consistency analysis, and classification in a single pass (7 phases).

**Manual approach**: Check each validation dimension from `02-validation-criteria.md` against current content. Use IQPilot MCP tools when available.

### Step 5: Update Content

**Minor updates** (version bump: 1.0 → 1.1):
- Fix broken links, update version numbers, correct factual errors

**Major updates** (version bump: 1.x → 2.0):
- Rewrite outdated sections, add new information, restructure if needed

**Deprecated content**: Move to appendix sections — never delete historical information.

### Step 6: Re-validate After Changes

If content changed, rerun:
- Grammar (if text changed)
- Readability (if substantial changes)
- Logic (if structure changed)
- Facts (for new claims)

Update bottom validation metadata with new timestamps and outcomes.

### Step 7: Publish Update

1. Verify all critical validations passed
2. Update bottom metadata: `last_updated`, `version`, `next_review_date`
3. Set next review date based on content type (see scheduled reviews table)
4. Commit changes

---

## Content Stability Guide

| Stability | Examples | Review Frequency |
|-----------|----------|-----------------|
| **Highly volatile** | Frameworks, tools, VS Code features | Monthly or with releases |
| **Moderately stable** | Design patterns, git workflows | Quarterly |
| **Stable** | HTTP basics, algorithm theory | Annually |

---

## Common Scenarios

### Technology Version Update

1. **Search** for version references across the article (framework versions, SDK versions, CLI commands)
2. **Review** each mention against the latest official release notes
3. **Update** version numbers, API changes, and deprecated features
4. **Test** all code examples against the new version—don't assume backward compatibility
5. **Update prerequisites** if minimum versions changed
6. **Re-validate** grammar (if text changed) and facts; update bottom metadata

**Trigger:** Major or minor version release of a referenced technology.

### Broken Links

1. **Run** link checker (`scripts/check-links-enhanced.ps1` or `markdown-link-check`)
2. **For each broken link:** search the target site for a replacement URL; if the page was removed, check [archive.org](https://web.archive.org/) for a cached version
3. **Update** the reference entry—if the source domain changed reliability level, re-classify the emoji marker (e.g., 📗 → 📒)
4. **Re-validate** the references section; update bottom metadata

**Trigger:** Monthly link check or reader report. Priority: P1 (24-hour SLA).

### Reader-Reported Issue

1. **Verify** the reported issue by reproducing the problem or checking the claim
2. **Assess severity** using the P0–P4 tiers from `02-validation-criteria.md`—security/accuracy errors are P0 (4-hour SLA), broken code is P1 (24 hours)
3. **Fix** the issue and note what changed in the bottom metadata
4. **Re-validate** affected dimensions (facts if a claim was wrong, grammar if text changed, logic if structure changed)
5. **Update** bottom metadata with new timestamps and version bump

**Trigger:** Reader feedback via any channel.

### Routine Quarterly Review

1. **Check** the freshness score using the 5-signal formula from `02-validation-criteria.md`—if score >80, a quick scan suffices
2. **Scan** for obvious issues: outdated screenshots, stale version numbers, new best practices in the field
3. **Run** fact-checking on technical claims (versions, APIs, pricing, feature availability)
4. **Update** any outdated content; apply the 20% rule (improve docs you touch by 20%)
5. **Re-validate** if content changed; update bottom metadata and set `next_review_date`

**Trigger:** 90 days for technical content, annually for concept articles (see scheduled reviews table).

---

## References

- **Internal:** `.github/prompts/01.00-article-writing/article-review-for-consistency-gaps-and-extensions.prompt.md`
- **Internal:** `.copilot/context/01.00-article-writing/02-validation-criteria.md` (freshness scoring, SLA tiers)
- **Internal:** `.github/instructions/article-writing.instructions.md`

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 2.1.0 | 2026-03-01 | Expanded 4 common review scenarios from one-line summaries to step-by-step procedures with triggers and SLA references. Source: Gap 2 from context file audit. | System |
| 2.0.0 | 2026-02-28 | Complete rewrite: replaced phantom prompt names with actual prompt files; consolidated 405 lines to ~150 lines; added content stability guide; added freshness scoring reference; streamlined common scenarios. Source: 40.00-technical-writing articles 05, 10 | System |
| 1.0.0 | 2025-12-26 | Initial version | System |

<!--
context_metadata:
  version: "2.1.0"
  last_updated: "2026-04-26"
-->
