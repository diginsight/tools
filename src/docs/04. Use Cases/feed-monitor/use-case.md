---
title: "feed-monitor — use case"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, use-case]
description: "Scheduled feed ingestion and Azure sink fan-out performed by the feed-monitor job."
source_sets: [entry-points, composition-root, options-model]
---

# feed-monitor — use case

## 🎯 Purpose

`feed-monitor` exists to read configured feeds, parse their content, and write the resulting channel and item records to whichever Azure sinks are enabled.

## ⚙️ Actor flow

1. An external scheduler or operator triggers the job.
2. The job reads every configured feed.
3. It chooses the right parser based on the document's root element.
4. It writes channel and item data to any enabled sinks.
5. It stops itself after the pass completes.

## 🕳️ Not established

> **Not established**: the exact trigger, schedule and orchestration owner. The repository establishes the job pattern but not the workflow that starts it. ^[gap]

## 🔗 Related

- [Architecture](../../03.%20Architecture/feed-monitor/architecture.md)
- [Infrastructure](../../05.%20Infrastructure/feed-monitor/infrastructure.md)
- [Use Cases overview](../overview.md)

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
