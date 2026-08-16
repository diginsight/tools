---
description: Scaffold for the generated documentation tree under src/docs — chapter folders, metadata.yml shape and the component pivot
domain: "application-development"
---

# Documentation structure scaffold

**Audience**: agent. Produce the folder tree, not prose.

## Tree

```text
src/docs/
├── _evidence/                      # publish: false — never linked
│   ├── _discovery.md
│   ├── _run-state.md
│   └── [component-id]/
│       ├── code.md
│       ├── code.internal.md        # only when sensitive facts exist
│       ├── data.md
│       ├── configuration.md
│       ├── environment.md
│       ├── devops.md
│       └── security.md
├── [home-folder]/                  # metadata.yml → label: Home,             order: 1
├── [getting-started-folder]/       # metadata.yml → label: Getting Started,  order: 2
├── [architecture-folder]/          # metadata.yml → label: Architecture,     order: 3
├── [use-cases-folder]/             # metadata.yml → label: Use Cases,        order: 4
├── [infrastructure-folder]/        # metadata.yml → label: Infrastructure,   order: 5
├── [reference-folder]/             # metadata.yml → label: Reference,        order: 6
├── [other-components-folder]/      # metadata.yml → label: Other Components, order: 7
├── [validation-folder]/            # metadata.yml → label: Validation,       order: 8
├── [security-folder]/              # metadata.yml → label: Security,         order: 9
├── [devops-folder]/                # metadata.yml → label: DevOps,           order: 10
└── [appendix-folder]/              # metadata.yml → label: Appendix,         order: 11
```

## Rules

- An existing folder MUST be reused by adding `metadata.yml`. NEVER rename it.
- A folder outside the eleven keeps its own `metadata.yml` and is NEVER a placement target.
- Every chapter folder MUST contain an overview page, even when the chapter is otherwise empty.

## `metadata.yml`

```yaml
label: "[chapter name]"
short: "[abbreviated label for narrow navigation]"
icon: "[icon token]"
order: [1-11]
```

## Component pivot

Add one subfolder per relevant component **only when two or more components are relevant to that chapter**.

```text
[chapter-folder]/
├── index.md                        # chapter overview — written last
├── [component-id-a]/
│   └── [page].md
└── [component-id-b]/
    └── [page].md
```

Single relevant component → pages sit directly in the chapter folder.

## References

- **📖** `.copilot/context/10.00-application-development/04-documentation-structure.md` — chapters, placement, tie-breakers
- **📖** `.copilot/context/10.00-application-development/01-discovery-model.md` — layout mode and priority

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-08-16"
  created: "2026-08-16"
  consumers:
    - "ad-documentation-manager"
    - "ad-documentation-author"
    - "01.02-ad-docs-write"
  changes:
    - "v1.0.0: Initial creation"
---
-->
