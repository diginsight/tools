---
title: "Reference"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, reference]
description: "The precise runtime contracts and configuration keys described by repository evidence."
---

# Reference

## 🎯 Purpose

The Reference chapter focuses on the exact contracts the repository exposes: configuration keys, storage names, entry-point surfaces, and the command and service responsibilities the code establishes.

## 🗺️ Pages in this chapter

| Page | Covers |
|---|---|
| [feed-monitor — reference](feed-monitor/reference.md) | Feed parser, options model and sink configuration keys |
| [cosmosdb-console — reference](cosmosdb-console/reference.md) | Operator commands and Cosmos DB access routines |

## 🔑 Key points

- The repository exposes configuration by named settings objects and Azure resource keys.
- `feed-monitor` documents the sink and parser model in more detail than the scaffold APIs.
- The repository does not declare a live database instance or secret values in source.

## 🔗 Related

- [Architecture](../03.%20Architecture/overview.md)
- [Home](../00.%20Home/overview.md)
- [Appendix](../11.%20Appendix/overview.md)
