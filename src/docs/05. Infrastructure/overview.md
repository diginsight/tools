---
title: "Infrastructure"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, infrastructure]
description: "The repository's provisioned runtime environment and Azure-hosted dependencies."
---

# Infrastructure

## 🎯 Purpose

This chapter records the infrastructure footprint implied by the repository: Azure App Service deployment, Azure Monitor and Key Vault integration, and the storage and Cosmos DB dependencies used by the feed-monitor job.

## 🗺️ Pages in this chapter

| Page | Covers |
|---|---|
| [feed-monitor — infrastructure](feed-monitor/infrastructure.md) | Deployment and Azure resource dependencies for the job |

## 🔑 Key points

- Azure App Service is the named hosting target for deployment workflows.
- The runtime depends on Azure Storage, Cosmos DB and Azure Monitor.
- The repository itself does not establish live resource values or endpoints; those remain external and are recorded as unreachable.

## 🔗 Related

- [Architecture](../03.%20Architecture/overview.md)
- [DevOps](../10.%20DevOps/overview.md)
- [Security](../09.%20Security/overview.md)
