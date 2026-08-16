---
title: "Verification gates"
description: "What a page and a page set must pass — per-page gates, six cross-page lenses, the exposure gate and how a failed gate is handled"
domain: "application-development"
goal: "Catch the failures that survive a page-by-page read — contradiction between pages, repeated facts drifting apart, broken links and disclosure — before anything is considered done"
scope:
  covers:
    - "Per-page gates and their pass criteria"
    - "The six cross-page lenses"
    - "The exposure gate"
    - "Gate outcomes and failure handling"
    - "What the verifier may and may not do"
  excludes:
    - "How a page is written (see 07-documentation-authoring-criteria.md)"
    - "Prose quality rules (see article-writing.instructions.md and the article-review skill)"
    - "Robustness findings (see 09-hardening-invariant-catalog.md)"
boundaries:
  - "The verifier MUST NOT rewrite a page — it reports, the author rewrites"
  - "NEVER mark a page verified while any per-page gate fails"
  - "NEVER publish a page that fails the exposure gate, regardless of every other outcome"
rationales:
  - "Page-by-page review cannot see contradiction between pages, which is the defect readers notice first"
  - "Separating verification from authoring stops the same reasoning that produced an error from clearing it"
  - "The exposure gate is absolute because this content is public and disclosure is not correctable after the fact"
---

# Verification gates

**Purpose**: What a page, and a set of pages, must pass before the run reports success.

**Referenced by**:
- `ad-documentation-verifier.agent.md`, `ad-documentation-manager.agent.md`
- `.github/prompts/10.00-application-development/01.03-ad-docs-verify.prompt.md`

---

## ✅ Per-page gates

Applied to every generated page. All must pass.

| # | Gate | Passes when |
|---|---|---|
| 1 | **Anchor coverage** | every assertion carries a traceability anchor, or sits inside a marked gap |
| 2 | **Anchor resolution** | every anchor resolves to a record that exists in the named dossier |
| 3 | **Confidence honesty** | no `claimed`-confidence record is stated as fact; it is attributed |
| 4 | **Stamp completeness** | the verification stamp is present, and its `evidence.observed` dates match the dossiers actually read |
| 5 | **Source-set declaration** | `source_sets` is declared and every role in it is a real role |
| 6 | **Template conformance** | the page follows its bound template's required sections |
| 7 | **Gap marking** | every unestablished fact is marked, not hedged |
| 8 | **Preservation** | no human-authored block was dropped without classification |

Gates 1, 2 and 8 are the load-bearing ones — they are what make `D14` enforceable rather than aspirational.

---

## 🔭 Cross-page lenses

Applied to the page set after the pages are written. Each lens is a separate pass; combining them causes the later ones to be skipped in practice.

| # | Lens | Asks |
|---|---|---|
| 1 | **Readability in context** | reading these pages in navigation order, does the story hold — or does a chapter assume something never introduced? |
| 2 | **Message prioritisation** | does each page lead with what matters most, or has detail floated to the top? |
| 3 | **Repeated-fact consistency** | where a fact appears on more than one page, do the statements agree exactly? |
| 4 | **Internal non-contradiction** | does any page assert something another page denies? |
| 5 | **Bidirectional link validity** | does every link resolve, and does every page that should be linked from somewhere actually have an inbound link? |
| 6 | **Navigation coverage** | is every generated page reachable, and does every navigation entry point at a page that exists? |

Lens 3 has a standing preference: a fact appearing on two pages **should be written once and linked**, because two copies diverge at the next update. When duplication is unavoidable, the lens verifies exact agreement.

Lens 5 is bidirectional on purpose. A page nothing links to is invisible even though it exists, and that is indistinguishable from a page that was never written.

---

## 🚨 The exposure gate

Absolute. A failure here blocks publication regardless of every other outcome.

| Check | Fails when |
|---|---|
| **No internal link** | a published page links to, or cites the path of, a `*.internal.md` file |
| **No secret value** | any credential, key, token, connection string or certificate material appears in a published page |
| **No personal data** | any identifying value appears, including inside sample output |
| **No exploit detail** | a weakness is described precisely enough to act on |
| **No internal surface** | an internal hostname, private endpoint, management URL or tenant identifier appears |
| **Dossiers unpublished** | every file under `_evidence/` carries `publish: false` |

Classification is owned by 📖 `03-evidence-access-policy.md`. This gate only enforces it.

On failure: **remove the exposure first**, then report. Do not report and leave it in place pending a decision.

---

## 🧯 Outcomes and failure handling

| Outcome | Meaning | Next |
|---|---|---|
| `pass` | every gate and lens passed | record in the stamp; the chapter closes |
| `pass-with-gaps` | gates passed; marked gaps remain | record the gap count in the stamp; report the gaps as work |
| `fail` | at least one gate failed | report to the manager; the author rewrites; re-verify |

The verifier **reports, it does not repair**. Letting it edit collapses the separation that gives its judgement independent value — and a verifier that fixes its own findings has no incentive to look hard for more.

A page that has failed the same gate twice is escalated rather than retried a third time. Repeated identical failure usually means the dossier is deficient, not the prose.

---

## References

- **📖** `00-stream-contract.md` — roles, and the escalation format used on repeated failure
- **📖** `02-evidence-dossier-schema.md` — coverage declarations and gap records
- **📖** `03-evidence-access-policy.md` — sensitive-material classification
- **📖** `05-source-sets-and-propagation.md` — anchors, source sets and the stamp
- **📖** `07-documentation-authoring-criteria.md` — what the author was required to do

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
