---
description: Rules for creating and maintaining instruction files that provide path-specific AI guidance via applyTo patterns
applyTo: '.github/instructions/*.instructions.md'
version: "1.9.0"
last_updated: "2026-06-06"
domain: "prompt-engineering"
goal: "Enforce that instruction files provide path-specific enforcement rules with unique, non-overlapping applyTo scopes"
rationales:
  - "Overlapping applyTo patterns cause unpredictable rule precedence"
  - "A single designated shared-baseline instruction file may overlap multiple artifact types when its precedence and scope are explicit"
  - "Two instruction files may share an applyTo pattern when they govern orthogonal concerns and each documents that it does not conflict with the other — overlap is harmless; only undocumented competing precedence is the harm"
  - "Instructions must contain enforcement rules, not knowledge (which belongs in context files)"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# Instruction File Rules

## Purpose

Instruction files provide **path-specific AI guidance** auto-injected via `applyTo` glob patterns. They contain enforcement rules — not knowledge or embeddable content. VS Code loads ALL instruction files whose `applyTo` matches the target file and concatenates them — overlap does not select one winner. Each file MUST therefore have an `applyTo` scope that is either unique, or overlaps only under one of the two documented overlap shapes (§ Permitted Overlaps): a single designated shared-baseline file, or an explicitly-coordinated orthogonal pair.

## Severity Index

**CRITICAL** — block on failure:
- **[C3]** Token budget: instruction files ≤1,500 tokens
- **[C6]** YAML frontmatter: all required fields present and valid

**HIGH** — fix before use:
- **[H8]** Imperative language: MUST/NEVER/ALWAYS — no suggestions
- **[H9]** Required sections: H1 title, Purpose, at least one Rules section
- **[H10]** `applyTo` conflict-free: no overlap with other instruction files, except a documented overlap shape — a single shared-baseline file OR an explicitly-coordinated orthogonal pair (§ Permitted Overlaps)

**MEDIUM** — fix when convenient:
- **[M1]** Template externalization: no knowledge blocks >10 lines
- **[M6]** Naming: kebab-case `{domain}.instructions.md`

**📖 Full priority matrix:** see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)

## Required YAML Frontmatter

```yaml
---
description: "One-sentence description of what these instructions enforce"
applyTo: '{glob pattern targeting specific file types}'
version: "1.0.0"
last_updated: "YYYY-MM-DD"
domain: "prompt-engineering"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---
```

| Field | Required | Criteria |
|-------|----------|----------|
| `description` | ✅ MUST | Non-empty, one sentence, describes the rules enforced |
| `applyTo` | ✅ MUST | Valid glob pattern matching ONLY the intended file types |
| `version` | ✅ MUST | Semantic version (`major.minor.patch`). Increment on every change. |
| `last_updated` | ✅ MUST | ISO date (`YYYY-MM-DD`). Update on every change. Enables staleness detection. |
| `domain` | ✅ MUST | Single scalar identifying the semantic domain the instructions target (e.g. `"prompt-engineering"`, `"article-writing"`). 📖 See `00.03-metadata-contracts.md` § `domain:` field semantics. |
| `context_dependencies` | ✅ MUST | Folder paths of referenced context files. Enables cascade staleness detection. Required for ALL instruction files that reference context files via `📖`. |

### Permitted Overlaps

`applyTo` overlap is permitted ONLY under one of these two documented shapes. Any other overlap is an [H10] violation.

#### Shape 1 — Shared baseline

One instruction file MAY intentionally overlap multiple artifact-type instruction files when ALL of the following are true:

- It is explicitly described as the **shared baseline** for those artifact types
- It contains only cross-artifact rules that apply equally across those artifact types
- It documents precedence: type-specific instruction files override the shared baseline on conflicts
- Consuming instruction files reference it as the shared baseline
- No second overlapping shared-baseline file exists

#### Shape 2 — Coordinated orthogonal pair

Two instruction files MAY share the same `applyTo` pattern when ALL of the following are true:

- They govern **orthogonal concerns** (e.g. one governs behavior/WHEN, the other governs format/HOW) — neither restates the other's rules
- **Each** file documents the coexistence and explicitly states there is no conflict, naming the peer (e.g. *"Both fire on `*plan*` — no conflict: execution rules govern behavior, this file governs format"*)
- A narrower override (a third file matching a subset glob, e.g. `*vision*plan*.md` ⊂ `*plan*`) is allowed within the pair when it documents that it overrides on its narrower scope
- The pair stays a pair — a third co-equal file on the same pattern requires re-justifying all members against this shape

### Cascade Validation Rule

When ANY context file in a listed `context_dependencies` folder has a `last_updated` newer than the instruction file's `last_updated`, the instruction file MUST be re-validated within 7 days.

## Rules

- MUST be placed in `.github/instructions/` root — **no subfolders**
- MUST use imperative language: **MUST**, **WILL**, **NEVER**, **ALWAYS**
- MUST stay within **1,500-token budget**
- MUST NOT embed knowledge blocks >10 lines — reference context files via `📖`
- MUST NOT duplicate rules from other instruction/context files
- `applyTo` MUST NOT overlap with other instruction files, except under a documented overlap shape — verify before committing
- The only allowed overlaps are the two documented shapes in § Permitted Overlaps: a single shared-baseline file, or an explicitly-coordinated orthogonal pair
- **Rules** (MUST/MUST NOT) → instruction files. **Knowledge** (why/how) → context files.
- MUST be aware that VS Code also discovers instructions from `.claude/rules` (uses `paths` instead of `applyTo`) and configurable locations via `chat.instructionsFilesLocations` setting — verify no conflicts with these alternate paths

## Quality Checklist

- [ ] YAML: all required fields present and valid (C6)
- [ ] Token budget ≤1,500 (C3)
- [ ] `applyTo` verified conflict-free, or documented under a Permitted Overlap shape — shared-baseline or coordinated orthogonal pair (H10)
- [ ] Imperative language throughout (H8)
- [ ] No knowledge blocks >10 lines (M1)
- [ ] References section with `📖` markers
- [ ] `context_dependencies` covers all `📖` context folder references

## References

- **📖** Context engineering: see `validation-rules` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)
- **📖** File type decisions: see `file-type-guide` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)
- **📖** Token budgets: see `token-optimization` in `.copilot/context/00.00-prompt-engineering/` (00.00-context-structure-index.md → Functional Categories)
