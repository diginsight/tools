---
description: Output format for one robustness finding — invariant, evidence, what breaks, proposed change, verification and the purpose-derivation guard
domain: "application-development"
---

# Finding record

**Audience**: agent. One record per finding. Records become steps in the emitted plan file.

```markdown
### [id] — [short title] ([severity])

| Field | Value |
|---|---|
| `id` | `[invariant-class]-[nn]` |
| `class` | [identity-and-keys \| batch-and-quota-limits \| concurrency \| secret-exposure \| injection \| deserialisation \| outbound-calls \| determinism \| configuration-drift] |
| `component` | [component-id] |
| `severity` | [🔴 \| 🟠 \| 🟡] — [reason, required only when it differs from the catalogue default] |

**Invariant**: [the invariant, restated for this component]

**Evidence**: `[area]-nn`, `[area]-nn` — [what the records establish]

**What breaks**: [the concrete consequence — a wrong result, an exposure, a
failure mode. NEVER "this is risky".]

**Proposed change**: [what to change, specific enough to start work]

**Verification**: [how to tell the change worked — an assertion, an observation,
a measurement. NEVER "review the code".]

**Purpose derivation**: [REQUIRED only when the proposal removes, replaces or
redesigns anything]

| Question | Answer |
|---|---|
| Derived purpose | [purpose, with derivation source] |
| Still served? | [yes/no, with evidence] |
| What stops working | [consequence of applying the proposal] |
```

## Rules

- A finding MUST cite at least one dossier record `id`. A finding without one is speculation and MUST be dropped.
- `what-breaks` and `verification` MUST both be concrete. These two fields decide whether the finding is ever closed.
- Without a complete **Purpose derivation** block, a removal, replacement or redesign proposal MUST be downgraded to a question in the plan's discovery section.
- A finding MUST NEVER state a weakness in exploit-actionable detail. Route detail to the internal dossier and cite the stub.
- De-escalating on the grounds that "it has never happened" is FORBIDDEN. Absence of an incident is not evidence of an upheld invariant.
- 🔴 findings go to the current plan, 🟠 to a sibling plan, 🟡 to the park lot. NEVER mix tiers into one list.

## Plan step form

```markdown
### [n]. [short title] (🟡 todo)

[Finding body as above.]
```

Use suffix status marking. Checkbox lists are FORBIDDEN in plan files.

## References

- **📖** `.copilot/context/10.00-application-development/09-hardening-invariant-catalog.md` — the nine classes and defaults
- **📖** `.copilot/context/10.00-application-development/10-hardening-tiering-and-routing.md` — tiering, routing, scope guard
- **📖** `.github/instructions/plan-marking.instructions.md` — suffix status marking

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
  consumers:
    - "ad-robustness-analyst"
    - "ad-robustness-manager"
    - "02.00-ad-harden-scan"
    - "02.01-ad-harden-plan"
  changes:
    - "v1.0.0: Initial creation"
---
-->
