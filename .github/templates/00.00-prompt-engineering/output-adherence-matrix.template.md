---
template_metadata:
  name: "output-adherence-matrix"
  description: "Output format for /pe-meta-adherence — guidance-first adherence matrix showing which consumers implement which rules"
  version: "1.0.0"
  last_updated: "2026-05-15"
  used_by:
    - "pe-meta-adherence prompt"
    - "pe-meta-validator (guidance-first mode)"
---

# Adherence Matrix Template

## Usage

This template structures the output of guidance-first reviews (`/pe-meta-adherence`). Fill each section based on the target guidance file and its consumers.

---

## Adherence Matrix: [guidance file path]

**Reviewed**: [date]
**Review mode**: Guidance-first (`D16-adherence`)
**Rules extracted**: [N]
**Consumers found**: [M]

### Rules extracted

| # | Rule ID | Rule summary | Severity | Location in file |
|---|---|---|---|---|
| R1 | [short ID] | [one-line summary] | CRITICAL/HIGH/MEDIUM | § [section name] |
| R2 | | | | |

### Consumer adherence

| Consumer | R1 | R2 | R3 | ... | Score | Gap count |
|---|---|---|---|---|---|---|
| [consumer file path] | ✅ | ⚠️ | ❌ | ... | [X/N] | [count] |
| [consumer file path] | ✅ | ✅ | ✅ | ... | [X/N] | [count] |

**Legend**: ✅ = fully implemented | ⚠️ = partially implemented | ❌ = missing

### Gaps (severity-ranked)

| # | Consumer | Rule | Gap type | Severity | Finding | Recommendation |
|---|---|---|---|---|---|---|
| 1 | [path] | R[n] | Missing / Partial / Inaccurate | CRITICAL/HIGH/MEDIUM | [what's wrong] | [how to fix] |

### Summary

- **Total rules**: [N]
- **Total consumers**: [M]
- **Full adherence**: [count] consumers implement all rules
- **Gaps found**: [count] across [count] consumers
- **CRITICAL gaps**: [count]
- **HIGH gaps**: [count]
