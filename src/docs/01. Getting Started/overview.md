---
title: "Getting Started"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, getting-started]
description: "How to open, build and understand the Diginsight Tools workspace."
---

# Getting Started

## 🎯 Purpose

This chapter explains how the repository is built, which projects matter most, and how a newcomer can validate the workspace before moving into the deeper architecture and infrastructure documentation.

## 🧭 Recommended flow

1. Open the repository root and confirm the .NET SDK version from `global.json`.
2. Restore the solution with `dotnet restore` on `src/Diginsight.Tools.sln`.
3. Build the solution with `dotnet build src/Diginsight.Tools.sln`.
4. Review the primary runtime entry points in `src/30.00 Job/Diginsight.Tools.FeedMonitor` and the console utility under `src/01.00 Console/CosmosdbConsole`.
5. Use the chapter links below to read the relevant component documentation.

## 🧱 Project entry points

| Location | Purpose |
|---|---|
| `src/30.00 Job/Diginsight.Tools.FeedMonitor/` | Primary job component |
| `src/01.00 Console/CosmosdbConsole/` | Operational Cosmos DB console |
| `src/20.00 Api/` | Web API hosts and scaffolds |
| `src/MIPDocumentInspector/` | Legacy Windows desktop tooling |

## 🔗 Related

- [Home](../00.%20Home/overview.md)
- [Architecture](../03.%20Architecture/overview.md)
- [DevOps](../10.%20DevOps/overview.md)
