---
title: "Repository security posture"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, security]
description: "The security properties and gaps established by repository config and Azure references."
source_sets: [configuration, deployment, observability]
---

# Repository security posture

## 🎯 Summary

The repository signals a security posture centered on Azure deployment controls, including Key Vault and Azure Monitor references, while intentionally avoiding the storage of live secrets or endpoints.

## 🧱 Evidence

- `appsettings.json` and environment configuration files point to Azure Key Vault and related cloud resources.
- Azure Monitor configuration is referenced for telemetry export.
- The deployment workflows target Azure App Service, which is the host environment for the app.

## 🕳️ Not established

> **Not established**: live secret values, resource IDs, and production access boundaries. The repository records configuration keys and service intent, not credential material or live resource access. ^[gap]

## 🔗 Related

- [Security overview](overview.md)
- [Infrastructure](../05.%20Infrastructure/overview.md)
- [DevOps](../10.%20DevOps/overview.md)

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
