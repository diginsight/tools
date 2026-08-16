---
title: "Evidence access policy"
description: "How investigators may reach live sources — grant-bounded, non-destructive, lower-environment-first access with production announcement, sensitive-material classification and provenance stamping"
domain: "application-development"
goal: "Allow investigation to reach any evidence source the user can already reach, while making the access non-destructive, environment-appropriate and auditable, and keeping sensitive material out of published pages"
scope:
  covers:
    - "The six access rules and what compliance looks like for each"
    - "Environment preference order and how environments are established"
    - "Production announcement protocol and refusable reads"
    - "Read-only enforcement per source kind"
    - "Reaching an external configuration root through a declared pointer"
    - "Sensitive-material classification test"
    - "Provenance record form"
  excludes:
    - "Step-by-step capture procedures (see the evidence-capture skill)"
    - "Where sensitive facts are stored (see 02-evidence-dossier-schema.md)"
    - "Which invariants a security finding maps to (see 09-hardening-invariant-catalog.md)"
boundaries:
  - "NEVER provision, escalate or request new permissions — operate strictly within grants the user already holds"
  - "NEVER perform a write, mutation or bulk extraction against any live source"
  - "NEVER touch production without announcing it first and NEVER treat silence as consent"
rationales:
  - "There is no useful restriction on which sources may be consulted — accuracy depends on reaching them; the meaningful controls are on how"
  - "Preferring lower environments with better data gives representative evidence without production risk"
  - "A provenance stamp is what makes a live-sourced fact re-checkable later, turning a screenshot into evidence"
---

# Evidence access policy

**Purpose**: How investigators may reach live sources, and what must never leave them.

**Referenced by**:
- all six investigator agents
- `.github/skills/evidence-capture/SKILL.md` and its four procedures
- `.github/prompts/10.00-application-development/01.01-ad-docs-investigate.prompt.md`

---

## 🔑 The six access rules

There is **no restriction on which sources an investigator may reach**. The limits are on *how*.

| # | Rule | Compliance looks like |
|---|---|---|
| 1 | **Grant-bounded** | access uses credentials and sessions the user already holds; the stream never provisions, escalates or requests new permissions, and never asks the user to paste a secret |
| 2 | **Non-destructive** | read-only wherever the source supports it; no write, no mutation, no state change, no job trigger |
| 3 | **Lower environments preferred** | production is discouraged; where unavoidable it is announced before the read happens |
| 4 | **Better data first** | among lower environments, prefer the one whose data is representative — typically stage over test |
| 5 | **Sensitive material contained** | sensitive facts go to the `*.internal.md` sibling, never to a published page |
| 6 | **Provenance stamped** | every live-sourced fact records source, environment and timestamp |

---

## 🪜 Environment preference order

Environments are **established, never assumed**. Naming conventions differ per repository, so an investigator determines which environments exist from deployment descriptors, pipeline definitions, configuration overrides or the user — never from a guessed name list.

Once established, rank them:

| Rank | Prefer | Because |
|---|---|---|
| 1 | the lower environment with the most representative data | representative evidence, no production risk |
| 2 | any other lower environment | safe, but the evidence may be thin or synthetic |
| 3 | production | last resort, announced, and only for facts unobtainable elsewhere |

When two lower environments are equally reachable, prefer the one with **better data** — a stage environment carrying realistic volumes usually beats a test environment carrying fixtures.

---

## 📢 Production announcement protocol

Before any production-touching read, state:

| Field | Content |
|---|---|
| **Source** | what is about to be read |
| **Environment** | that it is production, named as the repository names it |
| **Reason** | which fact is needed and why no lower environment can supply it |
| **Operation** | the exact read to be performed, and that it is read-only |

Then **wait**. Silence is not consent.

A production read is **refusable** — and MUST be refused by the investigator itself — when:

- the same fact is obtainable from a lower environment;
- the operation is not strictly read-only;
- it would return personal data;
- the user has not answered the announcement.

On refusal, record a gap (📖 `02-evidence-dossier-schema.md`) rather than proceeding.

---

## 🔒 Read-only enforcement by source kind

| Source kind | Permitted | Forbidden |
|---|---|---|
| **Repository** | read files, read history | any write outside `src/docs/` |
| **External configuration root** | read the files the declared pointer resolves to | reaching it by any route other than that pointer; enumerating outside the root it names; copying its files into this repository |
| **Database** | schema, index and shape inspection; bounded sampling only where required to establish a shape | bulk extraction, exports, any DDL or DML, long-running scans |
| **Cloud portal** | inventory and configuration views, captures | create, modify, delete, scale, restart, secret reveal |
| **CI portal** | pipeline definitions, run history, logs | queue a run, cancel a run, modify a definition, read protected variables |
| **API explorer** | read the description; execute only idempotent read operations against a lower environment | execute any operation with side effects, in any environment |
| **Running application** | navigate and observe | submit data, mutate state, authenticate as another principal |

Databases are the sharpest case: the stream needs **shape**, not **contents**. Record the schema, the keys, the indexes and the cardinality class — never the rows.

An **external configuration root** is the second sharpest, for two reasons. It sits outside the repository, so the only legitimate way to it is the pointer the repository itself declares — following a guessed or searched-for path turns an investigation into filesystem trawling. And it exists precisely to hold what the repository does not commit, so treat its contents as `credential`-bearing until established otherwise: record which settings it overrides, never the values it overrides them with.

---

## 🕵️ Sensitive-material classification

A fact is sensitive when it falls into any class below. Sensitive facts go to the `*.internal.md` sibling with a redacted stub on the published side.

| Class | Examples |
|---|---|
| `credential` | keys, connection strings, tokens, certificates, any secret value or its distinguishing prefix |
| `personal-data` | anything identifying a person, including sample rows that happen to carry real values |
| `exploit-enabling` | an unpatched weakness stated precisely enough to be actionable by a reader |
| `internal-surface` | internal hostnames, private endpoints, management URLs, tenant or subscription identifiers |

When in doubt, classify **sensitive**. The cost of an over-classified fact is one redirection; the cost of an under-classified one is disclosure in a published document.

> This repository is public. A published page may state **that** a control exists and **whether** it is adequate. It may not state how to defeat it.

---

## 🧾 Provenance record

Every fact obtained from a live source carries:

```markdown
- **source**: {what was read — resource, endpoint, table, pipeline}
- **environment**: {environment as the repository names it}
- **observed**: {ISO date}
```

Captured assets carry the same three fields as a machine-readable comment beside the asset, so a later run can detect drift without re-opening the source (📖 `.github/skills/evidence-capture/SKILL.md`).

A live-sourced record without provenance is downgraded to `claimed` confidence and is not publishable as fact.

---

## References

- **📖** `00-stream-contract.md` — determinability routing and the escalation format used when access is refused
- **📖** `01-discovery-model.md` — the capability matrix that says which surfaces exist
- **📖** `02-evidence-dossier-schema.md` — the published / internal split and gap records
- **📖** `08-verification-gates.md` — the exposure gate that enforces the split
- **📖** `.github/skills/evidence-capture/SKILL.md` — the capture procedures bound by this policy

## Version history

| Version | Date | Change | Author |
|---|---|---|---|
| 1.0.0 | 2026-08-16 | Initial version | System |
| 1.1.0 | 2026-08-16 | Added the external configuration root as a source kind, reachable only through the declared pointer and credential-bearing by default | System |

<!--
context_metadata:
  version: "1.1.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
-->
