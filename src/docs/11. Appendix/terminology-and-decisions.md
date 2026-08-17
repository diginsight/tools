---
title: "Terminology and decisions"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, appendix]
description: "Key terms and decisions governing how this documentation set treats recipes, scaffolds and repository gaps."
source_sets: [repository-context, documentation-structure]
---

# Terminology and decisions

## 🎯 Summary

This appendix records the repository facts that matter to understanding the documentation set without turning them into a chapter-specific policy document.

## 📚 Terms

| Term | Meaning |
|---|---|
| Core component | A component with direct functional priority and a documented repository purpose |
| Supporting component | A secondary component that contributes to the main workflow |
| Tooling component | A utility or inspection project with clear technical role but no direct business endpoint |
| Peripheral component | A scaffold-like or placeholder component whose purpose is not established in-source |
| Evidential gap | A fact that is absent from repository evidence and therefore must be marked rather than assumed |

## 🧭 Decisions

- The repository contains at least one clearly business-purpose job, `feed-monitor`.
- The six API scaffold projects are preserved as repository artifacts but documented as placeholder components pending external clarification.
- The documentation set is intentionally explicit about what is not established.

## 🔗 Related

- [Appendix overview](overview.md)
- [Home](../00.%20Home/overview.md)
- [Reference](../06.%20Reference/overview.md)

<!--
verification_stamp:
  generated: "2026-08-16"
  verified: ""
  evidence:
    - dossier: "_evidence/_discovery.md"
      observed: "2026-08-16"
  gates: "pass-with-gaps"
  open_gaps: 1
-->
