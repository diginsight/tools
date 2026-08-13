# Reference Integrity Check Template

## File Under Review

**File:** `[file path]`
**Type:** [context / instruction / agent / prompt / skill]

## References Found

| # | Reference Type | Target Path | Line | Status |
|---|---|---|---|---|
| 1 | 📖 link | `[target]` | L[N] | ✅ Exists / ❌ Broken |
| 2 | Template ref | `[target]` | L[N] | ✅ Exists / ❌ Broken |
| 3 | Handoff target | `[agent-name]` | YAML | ✅ Exists / ❌ Broken |
| 4 | Skill ref | `[skill-name]` | L[N] | ✅ Exists / ❌ Broken |

## Summary

- **Total references:** [N]
- **Valid:** [N]
- **Broken:** [N]
- **Status:** ✅ PASS / ❌ FAIL ([N] broken)

## Broken Reference Details

### [Reference #]
- **Source:** `[file path]` line [N]
- **Target:** `[expected path]`
- **Issue:** [File not found / Path changed / Agent doesn't exist]
- **Suggested fix:** [Correct path or removal recommendation]
