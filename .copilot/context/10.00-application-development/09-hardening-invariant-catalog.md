---
title: "Robustness invariant catalog"
description: "Nine stack-agnostic invariant classes with their invariant statement, the dossier evidence to check against, the violation shape and a default severity"
domain: "application-development"
goal: "Give the robustness stream a fixed, repeatable set of things to look for, so two scans of the same repository find the same defects and neither invents a finding the evidence does not support"
scope:
  covers:
    - "The nine invariant classes"
    - "Per class: the invariant, the dossier evidence it is checked against, the violation shape, the default severity"
    - "How a candidate becomes a finding"
    - "What is out of catalogue"
  excludes:
    - "Severity tiering and plan routing (see 10-hardening-tiering-and-routing.md)"
    - "How evidence was gathered (see 02-evidence-dossier-schema.md)"
    - "Documentation authoring (see 07-documentation-authoring-criteria.md)"
boundaries:
  - "NEVER raise a finding that is not traceable to a dossier record — a suspicion is not a finding"
  - "NEVER recommend removal, replacement or redesign before the component's purpose is derived"
  - "NEVER state a weakness in exploit-actionable detail in a published artifact"
rationales:
  - "A fixed catalogue is what makes scans repeatable; an open-ended 'look for problems' produces a different list every run"
  - "Stating invariants stack-agnostically is what lets the same catalogue run on repositories with different technologies"
  - "Requiring dossier traceability is what stops a scan degenerating into plausible-sounding speculation"
---

# Robustness invariant catalog

**Purpose**: What the robustness stream looks for, and what evidence each check consumes.

**Referenced by**:
- `ad-robustness-analyst.agent.md`, `ad-robustness-manager.agent.md`
- `.github/prompts/10.00-application-development/02.00-ad-harden-scan.prompt.md`
- `.github/templates/10.00-application-development/finding-record.template.md`

---

## 🔑 Correctness of identity and limits

| Class | Invariant | Check against | Violation shape | Default |
|---|---|---|---|---|
| **identity-and-keys** | a key uniquely and stably identifies what it names, and remains valid under the store's own constraints | `persistence-model`, `schema-definitions`, `domain-model` records | a key derived from mutable or non-unique data; a composite key missing a discriminator; an identifier that collides across sources | 🔴 |
| **batch-and-quota-limits** | every batch, page or request stays inside the documented limit of the service it targets, under worst case not typical case | `entry-points`, `persistence-model`, `settings-sources` records | an unbounded batch; a limit applied to item count where the service limits payload size; a limit checked before the payload is fully built | 🔴 |
| **concurrency** | concurrent execution cannot corrupt shared state or duplicate an effect | `composition-root`, `entry-points`, `persistence-model` records | shared mutable state without a guard; a read-modify-write without optimistic control; an operation assumed single-instance without enforcement | 🔴 |

---

## 🛡️ Exposure and untrusted input

| Class | Invariant | Check against | Violation shape | Default |
|---|---|---|---|---|
| **secret-exposure** | a secret is referenced, never stored, logged, echoed or committed | `secret-references`, `settings-sources`, `pipeline-definition` records | a literal secret in a settings file or pipeline; a secret written to a log or error message; a secret in a URL or a captured asset | 🔴 |
| **injection** | untrusted input never becomes executable structure in a query, command, path or markup | `entry-points`, `persistence-model` records | string-concatenated queries or commands; caller-supplied path fragments used unnormalised; unescaped input rendered into markup | 🔴 |
| **deserialisation** | untrusted input is deserialised only into types the repository controls, with no type resolution from the payload | `entry-points`, `domain-model` records | polymorphic deserialisation driven by a payload-supplied type name; unbounded object graphs; a permissive binder over an externally reachable surface | 🔴 |

---

## 🌐 Behaviour under the real world

| Class | Invariant | Check against | Violation shape | Default |
|---|---|---|---|---|
| **outbound-calls** | every outbound call has a bounded wait and a defined behaviour on failure | `composition-root`, `entry-points`, `settings-sources` records | no timeout, or a default far above the caller's own budget; retries without a cap or without backoff; a failure path that swallows the error | 🟠 |
| **determinism** | given the same inputs the repository behaves the same way, and where it cannot, the non-determinism is deliberate and bounded | `domain-model`, `test-surface`, `entry-points` records | ambient time, locale, culture or randomness used where a supplied value belongs; iteration-order dependence; a test that passes conditionally | 🟠 |
| **configuration-drift** | every setting the process requires is declared everywhere it runs, with a safe value | `options-model`, `settings-sources`, `deployment-descriptor` records | a bound setting with no declaration in some environment; a permissive default that is safe only in development; a setting declared but never read | 🟠 |

---

## 🧪 From candidate to finding

A candidate is promoted to a finding only when all four hold:

| # | Requirement |
|---|---|
| 1 | It is traceable to at least one dossier record, cited by `id` |
| 2 | The violation shape matches — not merely the class |
| 3 | The affected component's **purpose has been derived** (📖 `01-discovery-model.md`) |
| 4 | The impact is stated in terms of what breaks, not what is untidy |

A candidate failing 1 is speculation. Failing 3 it is the classic false positive: a deliberate sample of an anti-pattern, reported as a defect.

Where the evidence is insufficient to decide, the correct output is a **gap in the dossier**, routed back to investigation — not a hedged finding (📖 `00-stream-contract.md` § Determinability routing).

---

## 🚧 Out of catalogue

Two things are deliberately excluded, because both produce large volumes of low-value findings that crowd out the nine classes above.

| Excluded | Why |
|---|---|
| **Style and preference** | naming, layout, idiom choice — no invariant is at stake |
| **Speculative performance** | a slowness claim with no measurement is not evidence; measure it into a dossier first, then raise it |

Defaults are starting points. Tiering, escalation and de-escalation are owned by 📖 `10-hardening-tiering-and-routing.md`.

---

## References

- **📖** `00-stream-contract.md` — determinability routing and the scope-derivation precondition
- **📖** `01-discovery-model.md` — component registry and purpose derivation
- **📖** `02-evidence-dossier-schema.md` — the records findings cite
- **📖** `03-evidence-access-policy.md` — why exploit detail stays out of published artifacts
- **📖** `10-hardening-tiering-and-routing.md` — severity tiers and plan handoff

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
