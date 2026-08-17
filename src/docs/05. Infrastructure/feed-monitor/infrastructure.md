---
title: "feed-monitor — infrastructure"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, infrastructure]
description: "Deployment and hosting facts for the feed-monitor job and its Azure dependencies."
source_sets: [deployment, environment, configuration]
---

# feed-monitor — infrastructure

## 🎯 Summary

The repository names Azure App Service as the deployment target and links the job to Azure Storage, Cosmos DB, Key Vault and Azure Monitor configuration.

## 🧱 Infrastructure signals

- Deployment workflows target Azure App Service.
- The job is configured for Azure Storage, Cosmos DB and telemetry export.
- Resource instance values and live endpoint details are not present in source.

## 🕳️ Not established

> **Not established**: the specific Azure subscription, resource group, environment names and exact live resource topology. The repository establishes the service intent and configuration keys, not the actual provisioned environment. ^[gap]

## 🔗 Related

- [DevOps](../../10.%20DevOps/deployment-automation.md)
- [Security](../../09.%20Security/security-posture.md)
- [Infrastructure overview](../overview.md)

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
