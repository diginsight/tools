---
description: Output format for an evidence dossier — coverage declaration, evidence records, gap records and the sensitive-material split
domain: "application-development"
---

# Evidence dossier

**Audience**: agent (investigator writes, author and analyst read). Path: `src/docs/_evidence/[component-id]/[area].md`.

```markdown
---
component: "[component-id]"
area: "[code | data | configuration | environment | devops | security]"
investigator: "[agent name]"
generated: "[YYYY-MM-DD]"
repository_revision: "[revision observed]"
publish: false
---

# Evidence — [component-id] / [area]

## Coverage

| Question | Status |
|---|---|
| [question this area owns] | ✅ `[area]-01` / 🕳️ gap |

## Records

### `[area]-01`

- **assertion**: [one present-tense sentence]
- **where**: [path#symbol | resource id | capture asset path]
- **kind**: [source | configuration | schema | live-observation | capture]
- **environment**: [environment name — omit for source and configuration]
- **observed**: [YYYY-MM-DD]
- **confidence**: [established | corroborated | claimed]

### `[area]-02` — **[internal]**

- **assertion**: [redacted stub — states that the fact exists and its class]
- **class**: [credential | personal-data | exploit-enabling | internal-surface]
- **detail**: `[area].internal.md#[area]-02`
- **where**: [path]
- **observed**: [YYYY-MM-DD]

## Gaps

### Gap: [question that could not be answered]

- **Sought**: [the fact needed, and which page or finding depends on it]
- **Attempted**: [each source searched and what it returned]
- **Blocked by**: [access | absence | ambiguity]
```

## Rules

- Every record MUST carry all six fields. A record missing one is incomplete.
- An area with neither records nor gaps is a defect — the investigator did not run.
- A sensitive fact MUST appear here ONLY as a redacted stub; detail goes to `[area].internal.md` (`publish: false`).
- This file is regenerated, NEVER hand-edited.
- `claimed` confidence MUST NEVER be published as fact — only as an attributed claim.

## References

- **📖** `.copilot/context/10.00-application-development/02-evidence-dossier-schema.md` — full schema and semantics
- **📖** `.copilot/context/10.00-application-development/03-evidence-access-policy.md` — sensitive classification and provenance

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
  consumers:
    - "ad-code-investigator"
    - "ad-data-investigator"
    - "ad-configuration-investigator"
    - "ad-environment-investigator"
    - "ad-devops-investigator"
    - "ad-security-investigator"
    - "01.01-ad-docs-investigate"
  changes:
    - "v1.0.0: Initial creation"
---
-->
