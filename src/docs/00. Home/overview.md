---
title: "Home"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, home]
description: "Repository overview for the Diginsight Tools workspace and its primary components."
---

# Home

## 🎯 What this repository is

This repository holds the Diginsight tooling workspace for a set of .NET samples, jobs, APIs and legacy desktop components. The active work is centered on a feed-monitoring job, a small Cosmos DB console utility, and a cluster of ASP.NET Core scaffold APIs that are present in the current tree but not yet fully specialized.

## 🗂️ Repository shape

| Area | Role |
|---|---|
| `src/01.00 Console/` | Console and operator tooling |
| `src/20.00 Api/` | ASP.NET Core API hosts |
| `src/30.00 Job/` | Background jobs and automation |
| `src/MIPDocumentInspector/` | Legacy Windows desktop inspection tooling |
| `.github/workflows/` | Deployment and release automation |

## 🔑 Key points

- The repository is a multi-component .NET workspace.
- The primary documented business capability is `feed-monitor`.
- Several API projects are template scaffolds and remain purpose-unclear in the current repository evidence.
- The main deployment path is Azure App Service with GitHub Actions.

## 🔗 Related

- [Getting Started](../01.%20Getting%20Started/overview.md)
- [Architecture](../03.%20Architecture/overview.md)
- [Other Components](../07.%20Other%20Components/overview.md)
