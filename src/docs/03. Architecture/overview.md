---
title: "Architecture"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, architecture]
description: "Architecture overview for the repository and its primary job component."
---

# Architecture

## 🎯 Purpose

The architecture chapter describes the repository's component boundaries, the main job orchestration pattern, and the relationship between runtime code and its Azure-backed storage dependencies.

## 🗺️ Pages in this chapter

| Page | Covers |
|---|---|
| [feed-monitor — architecture](feed-monitor/architecture.md) | Background job, parser selection, sink fan-out and Azure storage integration |

## 🔑 Key points

- `feed-monitor` is the only clearly business-oriented core component in the current workspace.
- The job is a host-driven, single-pass background worker with independent enabled sinks.
- The repository mixes modern .NET core code with legacy Windows desktop projects.

## 🔗 Related

- [Home](../00.%20Home/overview.md)
- [Infrastructure](../05.%20Infrastructure/overview.md)
- [Reference](../06.%20Reference/overview.md)
