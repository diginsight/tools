---
title: "Documentation authoring criteria"
description: "How a page is written from a dossier — progressive disclosure, the five-way content preservation classification, missing-information representation and diagram policy"
domain: "application-development"
goal: "Produce pages that stay readable as they grow, never lose a human contribution to a regeneration, and show absence rather than smoothing over it"
scope:
  covers:
    - "Progressive disclosure applied to a page"
    - "Content preservation: move, user-added valid detail, superseded, evidence-contradicted, ambiguous"
    - "How missing information is represented"
    - "The declared-versus-observed rule and where the boundary is drawn"
    - "Diagram policy and when a diagram is warranted"
    - "The stop-and-return rule for authors"
  excludes:
    - "Prose voice, formatting and accessibility (see article-writing.instructions.md)"
    - "Where a page goes (see 04-documentation-structure.md)"
    - "Which gates a finished page must pass (see 08-verification-gates.md)"
boundaries:
  - "NEVER infer, interpolate or complete a fact the dossier does not carry — stop and return the gap"
  - "NEVER delete human-authored content during a rewrite; classify it first"
  - "NEVER restate rules owned by article-writing.instructions.md or documentation.instructions.md"
rationales:
  - "The author is deliberately blind to source, so the only safe response to a missing fact is to stop — inventing one is undetectable downstream"
  - "Regeneration that silently drops hand-written detail is the failure that makes teams abandon generated documentation"
  - "A marked gap is more useful than a plausible sentence, because it can be closed"
---

# Documentation authoring criteria

**Purpose**: How a page is written from a dossier, and what must survive a rewrite.

**Referenced by**:
- `ad-documentation-author.agent.md`, `ad-documentation-verifier.agent.md`
- `.github/prompts/10.00-application-development/01.02-ad-docs-write.prompt.md`
- every page template in `.github/templates/10.00-application-development/`

> Prose voice, heading mechanics, formatting and accessibility are owned by `.github/instructions/article-writing.instructions.md` and `.github/instructions/documentation.instructions.md`. This file adds only what is specific to evidence-derived pages.

---

## 🪜 Progressive disclosure

A page answers the reader's **first** question completely before opening a second.

| Layer | Carries | Reader who stops here |
|---|---|---|
| 1 — orientation | what this is and why it exists, in a few sentences | knows whether to keep reading |
| 2 — working knowledge | the shape, the flow, the decisions | can reason about it |
| 3 — precise detail | exact names, values, signatures, constraints | can use it correctly |
| 4 — on-demand | edge cases, history, rationale, exceptions | linked out, not inlined |

Layer 4 lives in **separate linked pages**, never in the body. This is the same principle applied to documentation that governs context files: depth is available, not resident.

A page that opens with an exhaustive parameter table has inverted the layers and will be abandoned at the first screen.

---

## 🗃️ Content preservation

When a page already exists, every block of existing content is classified **before** anything is written. Deletion is never the default.

| Classification | Test | Action |
|---|---|---|
| **Move** | correct, but now belongs to a different page or a different layer | relocate; leave a link if it was linked to |
| **User-added valid detail** | not derivable from any dossier, but not contradicted either | **preserve verbatim** and mark it as human-authored |
| **Superseded** | evidence has moved on and a newer statement replaces it | replace in place; move the old statement to the Appendix if it carries historical value |
| **Evidence-contradicted** | a dossier record directly falsifies it | replace, and report the contradiction — it may mean the code changed without anyone noticing |
| **Ambiguous** | cannot be classified with confidence | **preserve**, mark for review, and escalate |

The default for anything unclassifiable is **preserve**. An unnecessary paragraph costs a reader a few seconds; a deleted operational note costs someone an outage.

Human-authored content is marked so later runs recognise it:

```html
<!-- human-authored: preserved across regeneration -->
```

---

## 🕳️ Missing information

The author cannot investigate. When a needed fact is absent from the dossier, there are exactly two legitimate responses.

| Situation | Response |
|---|---|
| The gap blocks the page's core purpose | **stop**; return the gap through the escalation format; write nothing |
| The gap is peripheral | write the page and mark the gap in place |

A marked gap is explicit and machine-detectable:

```markdown
> **Not established**: how the retry interval is chosen. Investigated in `_evidence/{component}/configuration.md`; no setting or default was found. ^[gap]
```

Never acceptable: a hedge that reads as fact ("typically configured via…"), a plausible default, or silence where the template expected content.

---

## 🔎 Declared versus observed

A dossier record's `confidence` says whether a fact was established. It does not say **what kind** of fact it is — and for some evidence the distinction between *the repository says this* and *this is what happens* is the whole meaning.

| The record established | The page MAY state | The page MUST NOT state |
|---|---|---|
| A declaration — what a file says about itself | "it declares X", "it is configured to X" | "it does X", "it achieves X" |
| An observation — a captured run or a traced path | "it does X" | — |
| A declared order | "they are declared to run A then B" | "A runs, then B runs" |

The rule bites hardest on **AI artifacts**, whose bodies are instruction interpreted at run time: a declared goal is authoritative evidence of the declaration and no evidence at all of the outcome (📖 `05-source-sets-and-propagation.md` § Establishing an artifact's behaviour). It applies equally to a code comment stating intent, a README claim and a pipeline step's display name.

The unobserved effect of an artifact family is classified **peripheral**: the page's core purpose is what the family is and how it is used, both fully established by the declared contract, invocation surface, composition graph and bindings. So the author **marks the gap in place and continues** — stopping would suppress a page whose core purpose was met.

Never acceptable: promoting a declaration into an observation by dropping the qualifier, or narrating a declared sequence as an execution.

---

## 📊 Diagram policy

A diagram is warranted when a relationship is **hard to hold in prose** — more than three interacting parts, a sequence with branches, or a topology. Otherwise prose is faster to read and far cheaper to keep true.

| Rule | |
|---|---|
| Every element in a diagram MUST trace to a dossier record, exactly like prose |
| A diagram MUST NOT introduce a component, boundary or flow that no record establishes |
| A diagram MUST be accompanied by prose stating what it shows — it is never the only carrier of a fact |
| Diagram shapes and conventions come from `doc-mermaid-patterns.template.md` |

Speculative arrows are the most common way an architecture page acquires a system that does not exist.

---

## References

- **📖** `00-stream-contract.md` — the escalation format used when an author stops
- **📖** `02-evidence-dossier-schema.md` — the only input an author may read
- **📖** `04-documentation-structure.md` — placement and page shapes
- **📖** `05-source-sets-and-propagation.md` — anchors and the verification stamp
- **📖** `08-verification-gates.md` — what a finished page must pass

## Version history

| Version | Date | Change | Author |
|---|---|---|---|
| 1.0.0 | 2026-08-16 | Initial version | System |
| 1.1.0 | 2026-08-16 | Added the declared-versus-observed rule and classified the artifact effect gap as peripheral | System |

<!--
context_metadata:
  version: "1.1.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
-->
