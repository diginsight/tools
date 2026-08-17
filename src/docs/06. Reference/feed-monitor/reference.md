---
title: "feed-monitor — reference"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, reference]
description: "Configuration keys, feed parser choices and sink fan-out as expressed by the feed-monitor component."
source_sets: [options-model, entry-points, persistence-model]
---

# feed-monitor — reference

## 🎯 Reference surface

The repository establishes a feed-processing job that reads feed documents, chooses a parser by root element, and writes to enabled Azure sinks.

## 📌 Key contracts

| Contract | Value |
|---|---|
| Feed parsing | RSS or Atom document parser selection |
| Sink model | Cosmos DB, Blob Storage, Table Storage |
| Configuration pattern | Named options objects and environment-specific settings |
| Execution model | one-pass hosted service then self-termination |

## 🕳️ Not established

> **Not established**: exact secret values, live endpoint URIs and serialisation details. The code establishes the model and selection logic, but not the deployed target values. ^[gap]

## 🔗 Related

- [Architecture](../../03.%20Architecture/feed-monitor/architecture.md)
- [Use Case](../../04.%20Use%20Cases/feed-monitor/use-case.md)
- [Reference overview](../overview.md)

<!--
verification_stamp:
  generated: "2026-08-16"
  verified: ""
  evidence:
    - dossier: "_evidence/feed-monitor/code.md"
      observed: "2026-08-16"
  gates: "pass-with-gaps"
  open_gaps: 1
-->
