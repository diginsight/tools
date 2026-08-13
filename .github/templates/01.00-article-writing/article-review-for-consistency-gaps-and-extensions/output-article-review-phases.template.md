---
description: "Phase structure for single-article review workflows"
---

# Article Review - Phase Outputs Template

Use these output formats when running the `article-review-for-consistency-gaps-and-extensions.prompt.md` workflow.

**Location:** `.github/templates/01.00-article-writing/article-review-for-consistency-gaps-and-extensions/`
**Prompt:** `.github/prompts/01.00-article-writing/article-review-for-consistency-gaps-and-extensions.prompt.md`

---

## Phase 1: Review Context Analysis Output

```markdown
## Review Context Analysis

### Article Identification
- **Source:** [explicit/attached/active/workspace/user-selected]
- **File path:** `[full path to article]`
- **Article title:** [extracted from H1 or frontmatter]
- **Last updated:** [from top YAML date field]
- **Time since update:** [calculated: X months/days]

### Review Scope
**Type:** [Targeted / Comprehensive / Validation-only / Update-only]

**High Priority (Must Address):**
- [User-specified sections or concerns]
- [Critical gaps mentioned explicitly]

**Medium Priority (Should Address):**
- [Selected sections in editor]
- [Conversation context issues]
- [Metadata TODOs]

**Low Priority (Consider Addressing):**
- [General improvements]
- [Inferred gaps from quick scan]

### Explicit Requirements
- [User-stated requirements verbatim]
- [Placeholders provided: {{requirement}}]

### Initial Observations
- **Obvious version gaps:** [VS Code 1.x → current is 1.106+]
- **Deprecated features mentioned:** [.chatmode.md references]
- **Broken reference indicators:** [404 errors noticed]

**Proceed with Phase 2-5 using this context? (yes/no/modify)**
```

---

## Phase 2: Reference Inventory Output

```markdown
## Reference Inventory

| URL | Status | Title | Notes |
|-----|--------|-------|-------|
| https://example.com/doc | ✅ Valid | [Document Title] | Fetched successfully |
| https://old.site/page | ❌ Broken | [Old Page] | 404 error, needs replacement |
| https://moved.com/new | ⚠️ Redirected | [New Location] | Redirected from /old |

**Note:** Classification (📘 📗 📒 📕) happens in Phase 4 after all references are discovered.
```

---

## Phase 3: Topic Expansion & Gap Discovery Output

```markdown
## Topic Expansion Results

### Core Topics (from article)
- [List of topics currently covered]

### Adjacent Topics (discovered, NOT in article)

**From Workspace Context:**
- [Topic] (found in [X] related articles)

**From Official Docs:**
- [Topic] (from [documentation source])

**From Release Notes:**
- [Version/Date]: [New feature/change]

**From Community:**
- [Pattern/practice] (from [source])

### Alternatives Discovered

**For [Core Technology/Approach in Article]:**
- **[Alternative Name]**
  - **Use case fit**: [When to consider this alternative]
  - **Trade-offs**: [Pros vs cons compared to main approach]
  - **Source**: [Reference URL/documentation]
  - **Suggested treatment**: [Brief mention / Comparison appendix / Separate article]

### Relevance Assessment
- **High relevance** (should be in article): [topics]
- **Medium relevance** (consider adding): [topics]
- **Low relevance** (mention briefly or defer): [topics]

---

## Gap Discovery Report

### Accuracy Issues (Critical Fixes)
- **[Article claim]** vs **[Actual per source]**
  - Source: [URL] - [Brief description]
  - Impact: [Why this matters]

### Coverage Gaps (Missing Content in Existing Sections)
- **[Missing detail/example in covered topic]**
  - Source: [URL] - [Brief description]
  - Relevance: [Why readers need this]

### New Topic Gaps (Adjacent Topics Not Covered)

**High Relevance (Should Add):**
- **[Topic Name]**
  - Source: [URL/workspace file]
  - Rationale: [Why this topic belongs in article]
  - Suggested placement: [Where to add]

**Medium Relevance (Consider Adding):**
- **[Topic Name]**
  - Source: [URL/workspace file]
  - Suggested placement: [Integration approach]

**Low Relevance (Defer or Brief Mention):**
- **[Topic Name]**
  - Suggested action: [Brief mention + link, or defer entirely]

### New Reference Candidates
1. **[Title]** - [URL]
   - Description: [What it covers and why it's valuable]
   - Coverage: [Core topics / Adjacent topics]
```

---

## Phase 3.5: Consistency Analysis Output

```markdown
## Consistency Analysis Report

### Inconsistencies Found: [count]
| Location | Issue | Current Research Shows | Recommended Fix |
|----------|-------|------------------------|----------------|
| Section X, Line Y | [Description] | [What Phase 3 revealed] | [Suggestion] |

### Redundancies Found: [count]
| Locations | Duplicated Content | Consolidation Strategy |
|-----------|-------------------|------------------------|
| Section A + B | [What's repeated] | [How to merge] |

### Contradictions Found: [count]
| Conflict | Article Claims | Phase 3 Research | Resolution |
|----------|----------------|------------------|------------|
| [Topic] | [Statement in article] | [Current best practice] | [Which is correct + source] |
```

---

## Phase 5: Comprehensive Gap Analysis Output

```markdown
## Comprehensive Gap Analysis

### Critical Gaps (Must Fix)
**Correctness issues that affect article accuracy**

- **[Gap name]** `[Type: Correctness]`
  - **Current state**: What article says now
  - **Actual state**: What Phase 3 research revealed
  - **Impact**: Why this is critical
  - **Source**: [Tier 1-2 source]
  - **User priority**: ✅ User mentioned / ⬜ Editorial judgment

### High Priority Gaps (Should Fix)
**Major coverage/currency gaps OR user-specified areas**

- **[Gap name]** `[Type: Completeness/Currency]` `[User Priority: Yes/No]`
  - **Missing/outdated**: What's not covered or wrong
  - **Should include**: What research shows is current
  - **Source**: [Tier 1-3 source]

### Medium Priority Gaps (Consider Fixing)
**Adjacent topics, currency issues, or enhancements**

- **[Gap name]** `[Type: Adjacent Topic/Currency/Enhancement]`
  - **Opportunity**: What could be added/improved
  - **Benefit**: How this helps readers
  - **Suggested placement**: [new section, subsection, brief mention]

### Coverage Depth Gaps (Expansion Opportunities)
**Topics already covered but need more depth**

- **[Section Name]** - [Topic needing expansion]
  - **Current coverage**: [What's there now]
  - **Expansion opportunity**: [What deeper coverage would add]
  - **Suggested approach**: [Add subsection / Expand examples / Create appendix]

### Low Priority Gaps (Future Improvements)
- **[Gap name]** `[Type: Enhancement/Adjacent Topic]`
  - **Suggestion**: Possible improvement
  - **Defer reason**: Out of scope / Low impact

### Out of Scope
- **[Topic]**: Better suited for separate article on [topic]
```

---

## Phase 6: Deprecated Content Appendix Template

```markdown
# 📎 Appendix X: [Deprecated Feature/Behavior Name]

**Deprecated as of:** [version/date]
**Replaced by:** [new feature/approach]

[Historical information about the deprecated feature]

### Migration Path
[How to transition from old to new]
```

---

## Phase 7: Bottom Metadata Update Template

The bottom metadata carries only **current state** plus a pointer to the sibling changelog. Per-version history is written to the sibling `*.changelog.md` (see step below) — NEVER embedded here.

```yaml
<!-- 
---
validations:
  consistency_review:
    status: "completed"
    last_run: "{{ISO-8601 timestamp}}"
    model: "claude-sonnet-4.5"
    references_checked: {{count}}
    references_valid: {{count}}
    references_broken: {{count}}
    references_added: {{count}}
    gaps_identified: {{count}}
    gaps_addressed: {{count}}
article_metadata:
  filename: "{{filename}}"
  last_updated: "{{ISO-8601 timestamp}}"
  version: "{{bumped semantic version}}"
  changelog: "{{article-stem}}.changelog.md"
---
-->
```

### Phase 7b: Sibling changelog update (authoritative history)

When this review makes a material change, create or update the sibling `{{article-stem}}.changelog.md` (same folder). It is governed by `changelog-files.instructions.md` — NOT by article structure/voice rules, and it is never wired into the site navigation.

```markdown
## v{{bumped semantic version}} — {{date}}

{{brief description of changes made — references fixed, gaps addressed, sections changed}}
```

Prepend the new entry above earlier entries (most-recent-first). If the file does not exist, create it with a top frontmatter block (`title`, `description`, `last_updated`, `status: "living"`) and SHOULD set the OS hidden attribute where supported.

---

## Final Review Report Output

```markdown
# Article Review Report: [Article Title]

## Summary
- References checked: X (Y valid, Z broken)
- New references discovered: X (`📘 Official`: Y, `📗``📒 Community`: Z)
- Gaps identified: X (Critical: Y, Important: Z)
- **Adjacent topics discovered**: X (High relevance: Y, Medium relevance: Z)
- Sections to update: [list]
- New sections to add: [list for adjacent topics]
- Appendices to add: [list for deprecated content]

## Reference Audit
[Table of all references with status and classification]

## Knowledge Updates Required
[List of information to add/update with sources]

## Proposed Changes
[Summary of changes to be made]

Proceed with updates? (yes/no)
```

---

## Examples

### Reference Classification Example

```markdown
## References

### Official Documentation

**[GitHub Copilot - Repository Instructions](https://docs.github.com/...)** `[📘 Official]`  
The authoritative source for Copilot customization. Updated regularly.

**[VS Code Copilot Customization](https://code.visualstudio.com/docs/...)** `[📘 Official]`  
Microsoft's comprehensive guide to VS Code-specific features.

### Community Resources

**[How to write great AGENTS.md](https://github.blog/...)** `[📗 Verified Community]`  
GitHub blog post analyzing patterns from 2,500+ repositories.

**[Expert Tutorial on Custom Agents](https://example.com/...)** `[📒 Community]`  
Well-researched guide by recognized community expert.
```

### Deprecated Content Appendix Example

```markdown
# 📎 Appendix A: Legacy .chatmode.md Migration

**Deprecated as of:** VS Code 1.106  
**Replaced by:** `.agent.md` files in `.github/agents/`

Prior to VS Code 1.106, custom agents were called "chat modes" and used different conventions:

| Legacy | Current |
|--------|---------|
| `.chatmode.md` extension | `.agent.md` extension |
| `.github/chatmodes/` folder | `.github/agents/` folder |

### Migration Steps
1. VS Code automatically recognizes legacy files
2. Use Quick Fix action to migrate
3. Rename files from `*.chatmode.md` to `*.agent.md`
```

---

## Quality Checklist

- [ ] All URLs verified (fetched successfully or marked broken)
- [ ] All references classified with appropriate markers
- [ ] Critical gaps addressed with proper citations
- [ ] Deprecated content moved to appendices
- [ ] Bottom metadata updated with review results
- [ ] Changelog entry written to sibling `*.changelog.md` (not embedded in metadata)
- [ ] Top YAML block unchanged

<!--
---
template_metadata:
  version: "1.1.0"
  last_updated: "2026-06-12"
  created: "2026-03-20"
  consumers:
    - "article-review-for-consistency-gaps-and-extensions"
  changelog: "output-article-review-phases.template.changelog.md"
---
-->
