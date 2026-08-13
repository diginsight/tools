---
description: Rules for maintaining the repository-wide copilot-instructions.md file — the highest-authority PE artifact, always injected last into every system prompt
applyTo: '.github/copilot-instructions.md'
version: "1.0.0"
last_updated: "2026-03-19"
context_dependencies:
  - ".copilot/context/00.00-prompt-engineering/"
---

# copilot-instructions.md Governance Rules

## Purpose

`copilot-instructions.md` is the **highest-authority PE artifact** — always injected last, giving it final say on repo-specific conventions. This instruction file enforces its structure, content boundaries, and maintenance triggers.

## CRITICAL Rules

- **[C6]** MUST contain these required sections in order:
  1. **Repository Identity** — repo name, owner, content scope, SSG, output directory
  2. **General Rules** — reference table pointing to canonical sources (context/instruction files) — NEVER duplicate rules inline
  3. **PE Artifact Map** — quick-reference table of all artifact types, locations, and triggers
  4. **Repo-Specific Rules** — content area prefixes, Quarto config, tooling (MetadataWatcher, scripts)
  5. **Cross-Cutting Conventions** — repo-specific conventions not covered elsewhere
- **[C3]** MUST stay concise — this file injects into EVERY prompt. Minimize token footprint.

## HIGH Rules

- **[H8]** MUST use imperative language: MUST/NEVER/ALWAYS — no suggestions
- General Rules table MUST reference canonical sources — NEVER restate rules from context or instruction files
- PE Artifact Map MUST stay current — update when artifact types are added, moved, or removed
- MUST be updated within 7 days after any structural PE change (new artifact type, folder reorganization, new instruction file)

## MEDIUM Rules

- **[M1]** Keep tables compact — one row per rule/artifact type, no inline explanations
- Include `📖` reference to `01.03-file-type-decision-guide.md` for PE Artifact Map details
- Repo-Specific Rules section SHOULD only contain rules that cannot live in context or instruction files
- SHOULD note instruction priority hierarchy (Personal > Repository > Organization) when relevant — organization-level instructions are GA as of April 2026

## Maintenance Triggers

Update `copilot-instructions.md` when:
- A new artifact type is introduced (add to PE Artifact Map)
- Content area prefixes change (update Repo-Specific Rules)
- A new instruction or context file is created that affects repo-wide conventions (add to General Rules table)
- Quarto configuration changes significantly (update Repo-Specific Rules)

## Quality Checklist

- [ ] All five required sections present and in order (C6)
- [ ] No duplicated rules from context/instruction files — references only
- [ ] PE Artifact Map matches actual folder structure
- [ ] Content area prefix table matches existing folders
- [ ] Token footprint minimized — no verbose explanations

## References

- **📖** `.copilot/context/00.00-prompt-engineering/00.01-governance-and-capability-baseline.md` — System governance
- **📖** File type decisions: see `file-type-guide` in `.copilot/context/00.00-prompt-engineering/` (STRUCTURE-README.md → Functional Categories)
- **📖** `.github/instructions/instruction-files.instructions.md` — Instruction file rules (meta-governance)
