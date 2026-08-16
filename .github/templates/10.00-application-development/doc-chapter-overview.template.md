---
description: Page format for a chapter overview — the landing page of any of the eleven chapters, including Home and Getting Started
domain: "application-development"
---

# Chapter overview page

**Audience**: agent. Written **last** within its chapter, so it summarises pages that already exist.

```markdown
---
title: "[chapter name]"
author: "[repository owner]"
date: "[YYYY-MM-DD]"
categories: [repository-documentation, [chapter-slug]]
description: "[one sentence: what this chapter answers]"
source_sets:
  - [role]
---

# [chapter name]

## 📑 Table of contents

[required when the page exceeds 500 words]

## 🎯 What this chapter answers

[Layer 1 — orientation. Two to four sentences. A reader who stops here knows
whether to keep reading.] ^[[area]-nn]

## 🗺️ Pages in this chapter

| Page | Covers |
|---|---|
| [[page title]]([relative-path]) | [one line] ^[[area]-nn] |

## 🔑 Key points

- [the two to five things a reader must take away] ^[[area]-nn]

## 🕳️ Not established

> **Not established**: [what is missing and where it was sought]. ^[gap]

[Omit this section entirely when there are no gaps — an omitted section and an
empty section mean different things.]

## 🔗 Related

- [[chapter or page]]([relative-path]) — [why a reader would go there]

<!--
verification_stamp:
  generated: "[YYYY-MM-DD]"
  verified: "[YYYY-MM-DD]"
  evidence:
    - dossier: "_evidence/[component-id]/[area].md"
      observed: "[YYYY-MM-DD]"
  gates: "[pass | pass-with-gaps | fail]"
  open_gaps: [n]
-->
```

## Rules

- The overview MUST link every page in its chapter. A page with no inbound link fails the navigation lens.
- The overview MUST NOT restate a page's detail — it orients and links.
- A chapter with nothing to say still carries this page, stating that. NEVER omit a chapter.
- Every assertion MUST carry an anchor or sit inside a marked gap.

## References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — the eleven chapters
- **📖** `.copilot/context/10.00-application-development/07-documentation-authoring-criteria.md` — progressive disclosure
- **📖** `.copilot/context/10.00-application-development/05-source-sets-and-propagation.md` — roles, anchors, stamp

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
