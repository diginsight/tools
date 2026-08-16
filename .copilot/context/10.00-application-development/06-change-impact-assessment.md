---
title: "Change impact assessment"
description: "How a change set is resolved from a working tree, commit, range or PR, swept across nine documentation dimensions, and mapped to Skip / Extend / Update / Create decisions per page"
domain: "application-development"
goal: "Make change-driven runs complete rather than convenient — every dimension is considered and no-impact is recorded with a reason instead of being skipped"
scope:
  covers:
    - "Change-set resolution from working tree, commit, range, explicit list or PR"
    - "The nine-dimension sweep and the behaviour anchor"
    - "The no-impact recording rule"
    - "Skip / Extend / Update / Create decisions"
    - "Union with the impact propagation map"
    - "The announced mapping checkpoint"
  excludes:
    - "The role-to-page propagation table (see 05-source-sets-and-propagation.md)"
    - "Run modes and checkpoint mechanics (see 11-run-model.md)"
    - "How the resulting pages are written (see 07-documentation-authoring-criteria.md)"
boundaries:
  - "NEVER skip a dimension — a dimension with no impact MUST be recorded as 'no impact — {reason}'"
  - "NEVER let the diff alone define scope; the sweep and the propagation map both contribute"
  - "NEVER write a page before the change-set-to-page mapping has been announced"
rationales:
  - "A diff shows what changed in code, not what became wrong in prose — only a forced sweep catches the second"
  - "Recording no-impact with a reason turns an invisible omission into a reviewable claim"
  - "Announcing the mapping before writing lets the user correct scope while it is still cheap"
---

# Change impact assessment

**Purpose**: Turning a set of changes into the exact set of pages that must be written.

**Referenced by**:
- `ad-documentation-manager.agent.md`, `ad-documentation-verifier.agent.md`
- `.github/prompts/10.00-application-development/01.04-ad-docs-update-from-changes.prompt.md`

---

## 🎯 Change-set resolution

The change set is resolved from whichever input the user supplied. All five forms produce the same downstream artifact: a list of changed paths plus the intent stated alongside them.

| Input | Resolves to |
|---|---|
| **Working tree** | uncommitted modifications, additions and deletions |
| **Single commit** | that commit's diff, plus its message as stated intent |
| **Commit range** | the cumulative diff across the range |
| **Explicit list** | the named paths, treated as changed in full |
| **Pull request** | the PR's diff, plus its title, description and review discussion as stated intent |

Stated intent is **corroborating**, never authoritative. A commit message claiming a behaviour change that the diff does not show is a discrepancy to report, not a fact to document.

When the input is ambiguous, infer the most likely form, **state the inference**, and proceed (📖 `11-run-model.md` § Mode recognition).

---

## 🧭 The nine-dimension sweep

For every change set, all nine dimensions are considered. **Behaviour is the anchor**: it is assessed first, and its answer feeds the other eight.

| # | Dimension | Question |
|---|---|---|
| 1 | **Behaviour** | what does the repository now do that it did not, or no longer do? |
| 2 | **Logical architecture** | did a responsibility, boundary or dependency move? |
| 3 | **Physical architecture** | did what runs where, or how it is hosted, change? |
| 4 | **Use cases** | can an actor now achieve something new, differently, or no longer? |
| 5 | **Reference** | did a type, key, operation, setting or table change shape? |
| 6 | **Validation** | did what is proven, or how it is proven, change? |
| 7 | **Security** | did a control, an exposure or a trust boundary move? |
| 8 | **DevOps** | did build, gating or release change? |
| 9 | **Other** | did anything change that the eight above do not cover? |

### The no-impact rule

A dimension is **never skipped**. When it is unaffected, record it:

```markdown
| Security | no impact — the change is confined to test fixtures and touches no control or trust boundary |
```

"No impact" with a reason is a reviewable claim. A dimension omitted from the table is indistinguishable from a dimension forgotten.

Dimension 9 exists so that a change with no home is surfaced rather than dropped. If it fires repeatedly for the same kind of change, that is evidence the dimension list needs an addition — raise it, do not absorb it.

---

## 📋 Mapping to page decisions

Each affected dimension yields rows. Each row names one page and one decision.

| Decision | When | Effect |
|---|---|---|
| **Skip** | the page exists, is stamped, and no evidence behind it moved | untouched; recorded, not silently omitted |
| **Extend** | the page is correct but incomplete for the change | additive edit; existing content preserved |
| **Update** | the page asserts something the change made untrue | targeted rewrite of the affected assertions only |
| **Create** | the change introduces something with no page | new page from the bound template |

Deletion is **not** a decision here. Content made obsolete is reclassified — *superseded* — and handled by 📖 `07-documentation-authoring-criteria.md`, which decides whether it moves to the Appendix or is removed.

---

## ➕ Union with propagation

Final scope is the **union** of two independently derived sets:

```text
scope = dimension_sweep_pages  ∪  propagation_map_pages
```

They are derived differently on purpose:

- the **sweep** starts from intent and asks what became wrong;
- the **propagation map** (📖 `05-source-sets-and-propagation.md`) starts from changed source roles and asks what was built from them.

Each catches what the other misses. A page selected by the map but not the sweep is usually a stale Reference entry; a page selected by the sweep but not the map is usually a narrative page nobody declared a source set for — which is itself a defect worth reporting.

---

## 📢 The mapping checkpoint

Before any page is written, the mapping is announced:

| Field | Content |
|---|---|
| **Change set** | how it was resolved, and how many paths it contains |
| **Dimension table** | all nine rows, impacts and no-impacts alike |
| **Page decisions** | every row, with its decision and which set selected it |
| **Discrepancies** | anywhere stated intent and the diff disagree |

Then proceed. This is a **notify checkpoint**, not an approval gate — it exists so the user can correct scope while correction is still cheap (📖 `11-run-model.md` § Checkpoints).

---

## References

- **📖** `00-stream-contract.md` — determinability routing when a change's effect is undecidable
- **📖** `05-source-sets-and-propagation.md` — the propagation half of the union
- **📖** `07-documentation-authoring-criteria.md` — how superseded content is handled
- **📖** `08-verification-gates.md` — cross-page review after a change-driven write
- **📖** `11-run-model.md` — modes and checkpoint semantics

## Version history

| Version | Date | Change | Author |
|---|---|---|---|
| 1.0.0 | 2026-08-16 | Initial version | System |

<!--
context_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
-->
