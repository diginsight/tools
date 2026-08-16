---
title: "Source sets and propagation"
description: "The convergence engine — stack-agnostic source-set roles, investigator ownership, traceability anchors, the verification stamp and the changed-role to affected-page propagation map"
domain: "application-development"
goal: "Make an update run both cheap and complete by tying every page to the source roles it was built from, so changed evidence selects exactly the pages that must be re-verified"
scope:
  covers:
    - "Source-set roles stated stack-agnostically"
    - "Investigator ownership of each role"
    - "Traceability anchor format"
    - "Verification stamp contents and placement"
    - "Impact propagation map from changed role to affected pages"
  excludes:
    - "How a change set is resolved from a PR or commit range (see 06-change-impact-assessment.md)"
    - "Dossier record shape (see 02-evidence-dossier-schema.md)"
    - "What the verifier checks (see 08-verification-gates.md)"
boundaries:
  - "NEVER declare a source set as a hard-coded path list — declare roles and resolve them through the stack profile"
  - "Every published assertion MUST carry a traceability anchor to a dossier record"
  - "A page whose stamp is absent MUST be treated as unverified, never as current"
rationales:
  - "Path lists are what make a documentation system unportable and silently wrong after a refactor; roles survive both"
  - "Without a propagation map an update is either a full regeneration (expensive, destroys diffs) or a guess (incomplete)"
  - "Anchors turn review from 'does this sound right' into 'does this match the record', which is checkable"
---

# Source sets and propagation

**Purpose**: The convergence engine — what a page was built from, and which pages a change touches.

**Referenced by**:
- `ad-documentation-author.agent.md`, `ad-documentation-verifier.agent.md`, `ad-documentation-manager.agent.md`
- `.github/prompts/10.00-application-development/01.02-ad-docs-write.prompt.md`, `01.03-ad-docs-verify.prompt.md`, `01.04-ad-docs-update-from-changes.prompt.md`
- `.github/instructions/repository-docs.instructions.md`

---

## 🧩 Source-set roles

Every generated page declares its authoritative inputs as **roles**, never as paths. A role is resolved to concrete paths through the stack profile (📖 `01-discovery-model.md`) at the moment it is used.

| Role | What it is, stack-agnostically | Owner |
|---|---|---|
| `composition-root` | where the process assembles its services and starts | `ad-code-investigator` |
| `entry-points` | the surfaces callers or schedulers invoke | `ad-code-investigator` |
| `domain-model` | the types that carry the repository's own concepts | `ad-code-investigator` |
| `test-surface` | the suites that assert behaviour | `ad-code-investigator` |
| `persistence-model` | the entities, keys and indexes that reach a store | `ad-data-investigator` |
| `schema-definitions` | declared or migrated store structure | `ad-data-investigator` |
| `options-model` | the typed settings the process binds at startup | `ad-configuration-investigator` |
| `settings-sources` | declared settings and per-environment overrides | `ad-configuration-investigator` |
| `secret-references` | how secrets are referenced without being stored | `ad-configuration-investigator` |
| `infrastructure-definition` | declared resources and their topology | `ad-environment-investigator` |
| `deployment-descriptor` | what is deployed where | `ad-environment-investigator` |
| `pipeline-definition` | build, gate and release automation | `ad-devops-investigator` |
| `release-gates` | what must pass before a stage proceeds | `ad-devops-investigator` |
| `authn-authz-surface` | how callers are identified and authorised | `ad-security-investigator` |
| `transport-and-crypto` | how data is protected in transit and at rest | `ad-security-investigator` |

Each role has **exactly one** owner. A role claimed twice produces two dossiers asserting the same fact, which then disagree; an unowned role produces a silent hole.

Declared on the page:

```yaml
source_sets:
  - composition-root
  - options-model
```

---

## ⚓ Traceability anchors

Every assertion on a published page carries an inline anchor back to the dossier record that established it.

```markdown
The worker resumes from the last recorded position rather than replaying the feed. ^[code-04]
```

| Rule | |
|---|---|
| Format | `^[{area}-{nn}]` matching the dossier record `id` |
| Granularity | one anchor per assertion, not one per paragraph |
| Multiple sources | list them: `^[code-04,configuration-02]` |
| Marked gap | `^[gap]` — permitted **only** inside an explicitly marked gap block |

An assertion without an anchor fails the per-page gate. This is the mechanism that makes `D14` — authors do not investigate — checkable after the fact rather than merely asserted.

---

## 🔖 Verification stamp

Every generated page carries a stamp recording what it was built from and when it was last verified.

```html
<!--
verification_stamp:
  generated: "YYYY-MM-DD"
  verified: "YYYY-MM-DD"
  evidence:
    - dossier: "_evidence/{component}/{area}.md"
      observed: "YYYY-MM-DD"
  gates: "pass | pass-with-gaps | fail"
  open_gaps: 0
-->
```

| State | Meaning |
|---|---|
| stamp absent | the page is **unverified** — treat as stale, never as current |
| `evidence.observed` older than the dossier | evidence moved; the page is a rewrite candidate |
| `evidence.observed` equal to the dossier | unchanged; leave the page alone (📖 `00-stream-contract.md` § Idempotency) |
| `gates: fail` | the page is published but known-failing; the verifier owes an escalation |

---

## 🔀 Impact propagation map

Changed source role → the pages that must be re-verified. This is what makes an update cheap *and* complete.

| Changed role | Re-verify |
|---|---|
| `composition-root` | Architecture, Getting Started |
| `entry-points` | Reference (API units), Use Cases, Architecture |
| `domain-model` | Reference (entries), Architecture |
| `test-surface` | Validation |
| `persistence-model`, `schema-definitions` | Reference (entries), Architecture, Infrastructure |
| `options-model`, `settings-sources` | Reference (entries), Getting Started, Infrastructure |
| `secret-references` | Security, Infrastructure |
| `infrastructure-definition`, `deployment-descriptor` | Infrastructure, Architecture (physical), DevOps |
| `pipeline-definition`, `release-gates` | DevOps, Validation |
| `authn-authz-surface`, `transport-and-crypto` | Security, Reference (API units) |

Two properties this map must keep:

- **Reflexive** — every page reachable from at least one role, or it can never be updated automatically.
- **Bounded** — no role expands to "all pages", or the map has stopped selecting and an update becomes a regeneration.

The final scope of a change-driven run is the **union** of this map and the dimension sweep (📖 `06-change-impact-assessment.md`).

---

## References

- **📖** `00-stream-contract.md` — the idempotency rule this stamp implements
- **📖** `01-discovery-model.md` — the stack profile that resolves roles to paths
- **📖** `02-evidence-dossier-schema.md` — the records anchors point at
- **📖** `06-change-impact-assessment.md` — the other half of change-driven scope
- **📖** `08-verification-gates.md` — the gates that read stamps and anchors

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
