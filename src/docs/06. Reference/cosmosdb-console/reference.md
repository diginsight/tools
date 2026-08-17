---
title: "cosmosdb-console — reference"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, reference]
description: "The console's command surface and its Cosmos DB operational role."
source_sets: [entry-points, configuration]
---

# cosmosdb-console — reference

## 🎯 Reference surface

`cosmosdb-console` is a Cocona-based command-line tool that exposes an operational shell for Cosmos DB tasks.

## 📌 Observed features

- Command-line host registration via Cocona.
- Repository-level dependency on Azure Cosmos DB client packages.
- Configuration hooks for database interaction.

## 🕳️ Not established

> **Not established**: the full command inventory and the live target database details. The repository supports the role, but not the final command surface or production environment. ^[gap]

## 🔗 Related

- [Use Case](../../04.%20Use%20Cases/cosmosdb-console/use-case.md)
- [Reference overview](../overview.md)
- [Home](../../00.%20Home/overview.md)

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
