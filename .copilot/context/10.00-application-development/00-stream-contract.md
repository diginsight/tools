---
title: "Autonomous stream contract"
description: "The contract every role in the documentation and robustness streams obeys — role boundaries, determinability routing, idempotency, unverifiable-assertion protocol and escalation format"
domain: "application-development"
goal: "Define the single shared contract that keeps discovery, investigation, authoring and verification separable, so a second run over unchanged evidence produces no diff and no role ever invents a fact"
scope:
  covers:
    - "Definition of a stream: input, output, elementary actions, convergence"
    - "The four roles and the may-investigate / may-write boundary matrix"
    - "Determinability routing — write it, ask, or defer to discovery"
    - "Idempotency rule and the verification stamp"
    - "Unverifiable-assertion protocol"
    - "Escalation format"
    - "Scope-derivation precondition on removal recommendations"
  excludes:
    - "How a run is sequenced and checkpointed (see 11-run-model.md)"
    - "Dossier record shape (see 02-evidence-dossier-schema.md)"
    - "Chapter set and page placement (see 04-documentation-structure.md)"
    - "Which gates the verifier runs (see 08-verification-gates.md)"
boundaries:
  - "MUST NOT restate rules owned by another file in this domain — reference them"
  - "The role boundary matrix is authoritative here — agents MUST reference it, never redefine it"
  - "NEVER permit an assertion derived from absence of evidence"
rationales:
  - "Role separation only buys quality if the boundary is written once and referenced, otherwise each agent drifts its own interpretation"
  - "Idempotency is what makes 'update' safe to run at any time — without it, every run is a rewrite and reviewers stop trusting diffs"
  - "Routing every non-established fact to the user is the mechanism that keeps generated documentation honest"
---

# Autonomous stream contract

**Purpose**: The contract every role in the repository-documentation and repository-robustness streams obeys.

**Referenced by**:
- `.github/agents/10.00-application-development/ad-*.agent.md` (all eleven agents)
- `.github/prompts/10.00-application-development/01.*-ad-docs-*.prompt.md`, `02.*-ad-harden-*.prompt.md`
- `.github/instructions/repository-docs.instructions.md`

---

## 🌊 What a stream is

A **stream** turns a repository it has not seen before into a stable, reviewable output.

| Stream | Input | Output |
|---|---|---|
| Repository documentation | repository source + live evidence | chapter pages under `src/docs/` |
| Repository robustness | the same evidence dossiers | findings + an actionable plan file |

Both are loops over three **elementary actions**, never a single monolithic pass:

| Action | Unit of work |
|---|---|
| **investigate** | one area × one component → one dossier |
| **write** | one page ← its dossier set + its template |
| **scan** | one component × one invariant class → findings |

**Convergence property**: re-running a stream **updates** rather than duplicates. Two runs over the same evidence produce the same output.

---

## 👥 The four roles

Each role fails differently, so each is governed differently. The boundary below is a **hard** boundary.

| Role | Responsibility | May investigate? | May write |
|---|---|---|---|
| **Discovery** | stack profile, capability matrix, component registry | ✅ yes | registry only |
| **Investigation** | gather and record evidence for one area of one component | ✅ yes | dossier only |
| **Authoring** | render a dossier plus a template into a page | 🚫 **no** | pages only |
| **Verification** | run gates over pages and across the touched set | re-read only | corrections only |

**The authoring boundary is the load-bearing rule.** An author that may investigate will fill a template section by looking something up, and the resulting page cannot be distinguished from one grounded in recorded evidence. When a required fact is absent from the dossier, authoring **MUST stop** and return a named gap.

---

## 🧭 Determinability routing

Every role resolves an uncertain fact through this table. There is no fourth option.

| Condition | Route |
|---|---|
| Fact established from evidence, and the change has exactly one reasonable form | → write it |
| Fact established, but the change has more than one reasonable form | → **§ Open decisions**, back to the user |
| Fact **not** established from evidence | → **§ Open decisions**, back to the user — NEVER inferred from absence |
| Fact undecidable until execution | → **§ Discovery**, with a defined negative branch |

---

## ♻️ Idempotency

A second run over unchanged evidence MUST produce **no diff**.

Mechanism:

1. Every generated page carries a **verification stamp** recording the evidence state it was built from (📖 `05-source-sets-and-propagation.md`).
2. Before rewriting, the stream compares current evidence state against the stamp.
3. Unchanged → the page is left untouched and the stamp's timestamp is refreshed only if a gate re-ran.
4. Changed → only the sections whose source records moved are rewritten.

Blind regeneration is a defect, not a safe default. A run that rewrites unchanged pages destroys the reviewer's ability to read a diff.

---

## 🚫 Unverifiable-assertion protocol

NEVER assert from absence of evidence. Absence is itself a fact, and it is a **weaker** fact than presence.

| Observed | MUST be recorded as | MUST NEVER be written as |
|---|---|---|
| No test project found | "no test project was found under the searched roots" | "the component is untested by design" |
| No retry policy in source | "no retry configuration was located" | "the component does not retry" |
| No authentication middleware | "no authentication middleware was found in the composition root" | "the endpoint is anonymous" |

The sequence is always: **mark the gap → ask → write**. Writing before asking is how a plausible-sounding wrong statement enters documentation and survives review.

---

## 📤 Escalation format

An item routed back to the user carries exactly three things:

| Field | Content |
|---|---|
| **Unknown** | the specific fact that could not be established, stated as a question |
| **Attempted** | which evidence sources were searched and what each returned |
| **Gated** | which pages, sections or findings cannot complete until it is answered |

An escalation without the **Attempted** field is not actionable — the user cannot tell whether the search was competent.

---

## 🔒 Scope-derivation precondition

Before recommending **removal, replacement or redesign** of any component, the stream MUST have derived and recorded that component's **purpose** in the registry (📖 `01-discovery-model.md`).

A recommendation that contradicts a component's recorded purpose is a **scope error**, not a finding. Sample code, reference implementations and deliberate spikes look like dead code to a scan that never asked why they exist.

---

## References

- **📖** `01-discovery-model.md` — the registry that carries component purpose and priority
- **📖** `02-evidence-dossier-schema.md` — the investigation → authoring handoff contract
- **📖** `05-source-sets-and-propagation.md` — verification stamp and impact propagation
- **📖** `11-run-model.md` — how a manager sequences and checkpoints a run
- **📖** `.github/instructions/plan-execution.instructions.md` — where § Open decisions and § Discovery are defined

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
