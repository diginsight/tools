---
description: Page format for one validation unit — a test suite or verification surface, what it proves, what it does not, and how to run it
domain: "application-development"
---

# Validation unit page

**Audience**: agent. One page per test suite or verification surface established by discovery.

```markdown
---
title: "[validation unit name]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, validation]
description: "[one sentence: what this suite proves]"
source_sets:
  - test-surface
  - entry-points
  - release-gates
---

# [validation unit name]

## 🎯 What it proves

[The behaviour this suite establishes, stated as a claim a reader can rely on.] ^[[area]-nn]

- **Kind**: [unit | integration | contract | end-to-end | manual procedure]
- **Component under test**: [component-id]
- **Defined in**: [path] ^[[area]-nn]

## 📋 Covered behaviours

| Behaviour | Asserted by | Established by |
|---|---|---|
| [behaviour] | [test name or case] | ^[[area]-nn] |

## 🚫 What it does not prove

[Explicit. This is the most useful section on the page — it stops a reader
assuming coverage that does not exist.] ^[[area]-nn]

## ▶️ How to run it

[The invocation, and any prerequisite the suite needs.] ^[[area]-nn]

## 🔗 Dependencies

| Depends on | Kind | Established by |
|---|---|---|
| [fixture, environment or external role] | [required / optional] | ^[[area]-nn] |

## 🚦 Where it gates

[Which pipeline gate consumes this suite, if any. Link to DevOps.] ^[[area]-nn]

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]

<!--
verification_stamp:
  generated: "[YYYY-MM-DD]"
  verified: "[YYYY-MM-DD]"
  evidence:
    - dossier: "_evidence/[component-id]/code.md"
      observed: "[YYYY-MM-DD]"
  gates: "[pass | pass-with-gaps | fail]"
  open_gaps: [n]
-->
```

## Rules

- "What it does not prove" MUST be filled. A blank section here is a defect — it is where over-claimed coverage is caught.
- A test's **name** is not evidence of what it asserts. Establish the assertion from the test body.
- A skipped, ignored or conditionally-run test MUST be recorded as such. Counting it as coverage is a false claim.
- NEVER publish fixture data that carries personal data or a real credential.
- Test counts are not coverage. NEVER present a count as proof of adequacy.

## References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — tie-breaker 5
- **📖** `.copilot/context/10.00-application-development/07-documentation-authoring-criteria.md` — marked gaps

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
  consumers:
    - "ad-documentation-author"
    - "01.02-ad-docs-write"
  changes:
    - "v1.0.0: Initial creation"
---
-->
