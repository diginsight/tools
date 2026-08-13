---
description: "Input analysis guidance for article review workflows"
---

# Article Review - Input Analysis Guidance

Detailed extraction process and workflow examples for Phase 1 of `article-review-for-consistency-gaps-and-extensions.prompt.md`.

---

## Information Sources (Prioritized)

| Priority | Source | Description |
|----------|--------|-------------|
| 1 | **Explicit user input** | User-specified file, sections, concerns override everything |
| 2 | **Attached files** | Files attached with `#file:path/to/article.md` |
| 3 | **Active file/selection** | Content from open editor or selected text |
| 4 | **Workspace context** | Files in active folder, article metadata |
| 5 | **Inferred/derived** | Information calculated from analysis |

---

## Extraction Process

### 1. Identify target article

- Check chat message for explicit file path or article name
- Check for attached files with `#file:path/to/article.md` syntax
- Check active editor for open markdown files (prefer `.md` in content folders)
- List markdown files in workspace if needed
- **If multiple sources:** Use priority order above
- **If none found:** List available articles and ask user to specify

### 2. Identify priority areas

- Extract explicit focus areas from chat message:
  - Section names: "update the Custom Agents section"
  - Topics: "verify VS Code references", "add MCP coverage"
  - Placeholders: `{{update section X}}`, `{{verify references}}`
- Detect selected text in editor (user highlighted specific sections)
- Check conversation history for mentioned concerns or issues
- Scan article bottom YAML for previous review TODOs or notes

### 3. Identify known gaps

- Extract user-mentioned gaps: "missing information on handoffs"
- Detect temporal indicators: "update to latest VS Code version"
- Check article metadata for last update date (calculate staleness)
- Note version numbers mentioned (VS Code 1.x, Visual Studio 17.x)

### 4. Determine review scope

| Scope | Trigger |
|-------|---------|
| **Targeted** | User specified specific sections or concerns |
| **Comprehensive** | No specific priorities mentioned |
| **Validation-only** | Focus on references and accuracy checks |
| **Update-only** | Focus on version currency and new features |
| **Comprehensive + Expansion** | Include adjacent topic discovery (default) |

---

## Workflow Examples

### Scenario A: Explicit file + specific section

```
User: "Review tech/PromptEngineering/01.md and update the custom agents section"

Result:
- Article: tech/PromptEngineering/01.md (explicit input, priority 1)
- Scope: Targeted
- High Priority: "custom agents section"
```

### Scenario B: Attached file + selected text

```
User: "/article-review #file:01.md" (with "Chat Modes" section selected)

Result:
- Article: 01.md (attached file, priority 2)
- High Priority: "Chat Modes" section (user selection, priority 3)
```

### Scenario C: Active file + placeholders

```
User: "/article-review {{verify references}} {{add MCP section}}"

Result:
- Article: [active file in editor] (priority 3)
- Scope: Targeted dual-focus
- High Priority: Verify references, Add MCP section
```

### Scenario D: Minimal context

```
User: "/article-review"

Result:
- Check active editor → use if found
- Otherwise → list markdown files, ask user to select
- Scope: Comprehensive (no priorities specified)
```

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "article-review-for-consistency-gaps-and-extensions"
  changelog: "guidance-input-analysis.template.changelog.md"
---
-->
