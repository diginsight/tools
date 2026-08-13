---
template_metadata:
  name: "output-dimension-report"
  description: "Per-dimension finding report format for pe-meta-validator — one section per assessed dimension with status, evidence, findings, and metrics"
  version: "1.1.0"
  last_updated: "2026-06-07"
  used_by:
    - "pe-meta-validator"
    - "All pe-meta review prompts"
---

# Dimension Report Template

## Usage

This template structures per-dimension findings in validation reports. One section per assessed dimension. Skip dimensions not included in the `--dim` scope.

---

## D[N]: [dimension name]

**Status**: ✅ Pass / ⚠️ Partial / ❌ Fail
**Evidence**: [REQUIRED — non-empty for EVERY result, passes included. A finding's defect proof, or the one-line proof a PASS was actually derived: file+line, tool output, or quoted body text. An empty Evidence field is invalid output per the evidence-bound coverage contract.]
**Model used**: deterministic / standard / reasoning
**Applicable**: Yes / Skipped (not applicable to [type])

### Findings

| # | Severity | Finding | Location | Recommendation |
|---|---|---|---|---|
| 1 | CRITICAL/HIGH/MEDIUM/LOW | [description] | [file:line or section] | [action] |

### Metrics

[Dimension-specific metrics. Examples:]
- `D1-metadata`: `goal: ✅ | scope: ✅ | boundaries: ✅ | rationales: ❌ | version: ✅`
- `D2-references`: `📖 refs: 7/7 ✅ | markdown links: 3/3 ✅ | slash-commands: 2/2 ✅`
- `D3-token-budget`: `Token count: 1,847 / 2,500 budget (74%)`
- `D5-boundaries`: `Always: 6 (≥5 ✅) | Ask: 2 (≥2 ✅) | Never: 4 (≥3 ✅)`
- `D23-reference-efficiency`: `Refs total: 8 | Used: 6 | Unused: 2 | Upfront: 5 | Phase-specific: 3`

---

## Report header (use before dimension sections)

```markdown
## Validation Report: [artifact path]

**Reviewed**: [date]
**Artifact type**: [context/agent/prompt/instruction/skill/template/hook/snippet]
**Dimensions assessed**: [--dim value or "full"]
**Review mode**: Individual / Dependency-aware / Guidance-first
**Applicable dimensions**: [N] of 27

### Summary

| Category | Dims assessed | ✅ Pass | ⚠️ Partial | ❌ Fail |
|---|---|---|---|---|
| Structural | [count] | | | |
| Quality | [count] | | | |
| Strategic | [count] | | | |
| Efficiency | [count] | | | |
| External | [count] | | | |
| **Total** | **[count]** | | | |

### CRITICAL findings (immediate action)
[list or "None"]

### HIGH findings (fix before use)
[list or "None"]
```
