## Risk classification (when `--mode apply` is active)

For each finding, classify before acting:

### 1. Non-breaking = preserves goal, scope, and boundaries of the target artifact

- Structural fixes (formatting, YAML, reference links, metadata) → ALWAYS non-breaking
- Content additions that extend coverage without changing scope → USUALLY non-breaking
- Typo fixes, consistency normalization → ALWAYS non-breaking

### 2. Breaking = changes goal, scope, or boundaries

- Removing sections or boundaries
- Changing artifact purpose or target audience
- Modifying handoff chains or tool restrictions
- Altering scope patterns

### 3. Action by classification

| Classification | Action | Output |
|---|---|---|
| Non-breaking + high confidence | Apply autonomously | Report in summary |
| Non-breaking + medium confidence | Apply with pre-notification | Report with rollback instructions |
| Breaking or uncertain | Report as proposal | Require human confirmation |

### 4. Mode override

When `--mode plan` is explicitly specified, ALL findings are reported as proposals regardless of classification. No changes are applied.
