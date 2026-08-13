---
description: "Structured research report format for researcher agent output"
---

# Research Report: [Artifact Name]

## Decision

| Field | Value |
|---|---|
| **Recommendation** | [Create / Update / Split / Merge / No action needed] |
| **Artifact type** | [prompt / agent / context / instruction / skill / hook / snippet] |
| **Target path** | [file path] |

## Specification

| Field | Value |
|---|---|
| **Name** | [kebab-case name] |
| **Type/Mode** | [plan/agent for prompts/agents; tier for context; N/A for others] |
| **Tools** | [specific list, or N/A] |
| **Template** | [recommended template path, or N/A] |

## Requirements

Builder MUST implement all listed requirements.

| # | Requirement | Evidence source |
|---|---|---|
| 1 | [Requirement] | [file or analysis that supports it] |
| 2 | [Requirement] | [file or analysis that supports it] |
| 3 | [Requirement] | [file or analysis that supports it] |

## Boundaries

### Always Do (minimum 3)

- [Boundary from use case analysis]
- [Boundary from use case analysis]
- [Boundary from use case analysis]

### Ask First (minimum 1)

- [Boundary]

### Never Do (minimum 2)

- [Boundary from scope definition]
- [Boundary from scope definition]

## Scope

| Direction | Items |
|---|---|
| **IN** | [responsibilities — what this artifact covers] |
| **OUT** | [exclusions with redirect targets] |

## Consumer Impact

| Consumer file | Relationship |
|---|---|
| [file path] | [how it references or depends on this artifact] |

## Evidence

| Source | Version/Date | Key finding |
|---|---|---|
| [file or URL] | [version or date] | [distilled finding] |
| [file or URL] | [version or date] | [distilled finding] |

## Receiver Context

| Item | Value |
|---|---|
| **Instruction file to load** | [path governing the target artifact type] |
| **Key context files** | [2-4 most relevant context file paths for this task] |
| **Domain context (if non-PE)** | [domain-specific context file paths, or "N/A — PE domain"] |

---

## Type-Specific Sections

Researchers MAY add type-specific sections BELOW this line. The standard fields above MUST appear first and be complete before any extensions.

<!-- Type-specific extension examples:
  - prompt-researcher: Complexity assessment, use case challenge results
  - agent-researcher: Role challenge results, tool alignment analysis
  - context-researcher: Gap analysis, single-source-of-truth check, token estimate
  - instruction-researcher: applyTo conflict matrix, layer boundary analysis
  - skill-researcher: Discovery scenario results, progressive disclosure mapping
  - meta-researcher: Impact matrix, alternative approaches comparison
-->

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "agent-design"
    - "agent-researcher"
    - "context-information-design"
    - "instruction-file-design"
    - "meta-prompt-engineering-update"
    - "prompt-design"
    - "prompt-researcher"
    - "skill-design"
    - "template-design"
  changelog: "output-researcher-report.template.changelog.md"
---
-->
