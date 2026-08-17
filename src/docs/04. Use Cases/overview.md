---
title: "Use Cases"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, use-cases]
description: "The operational goals and accessible user actions described by the repository's active runtime components."
---

# Use Cases

## 🎯 Purpose

This chapter captures the use cases the repository currently supports from source evidence: the feed-monitor background job performs scheduled content harvesting, while the Cosmos DB console supports operator-oriented database inspection and maintenance.

## 🗺️ Pages in this chapter

| Page | Covers |
|---|---|
| [feed-monitor — use case](feed-monitor/use-case.md) | Scheduled content intake and sink fan-out |
| [cosmosdb-console — use case](cosmosdb-console/use-case.md) | Query and maintenance actions against Cosmos DB |

## 🔑 Key points

- The repository is primarily oriented around data collection and persistence.
- `feed-monitor` is the automation actor that makes the repository useful at runtime.
- `cosmosdb-console` is an operator tool for inspecting and interacting with a live Cosmos DB instance.

## 🔗 Related

- [Architecture](../03.%20Architecture/overview.md)
- [Infrastructure](../05.%20Infrastructure/overview.md)
- [Validation](../08.%20Validation/overview.md)
