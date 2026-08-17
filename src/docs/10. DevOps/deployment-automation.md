---
title: "Deployment automation"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, devops]
description: "The GitHub Actions deployment workflows and their Azure App Service target."
source_sets: [deployment, workflow-definition]
---

# Deployment automation

## 🎯 Summary

The repository contains workflow definitions that deploy tooling artifacts and Azure App Service content, demonstrating a build-and-deploy pipeline with Azure as the runtime target.

## 🧱 Workflow signals

- `.github/workflows/20.DeployTools.yml` targets deployment of tool artifacts.
- `.github/workflows/21.DeployAppService.yml` targets Azure App Service deployment.
- These are the repository's only explicit deployment automation definitions in the workspace.

## 🕳️ Not established

> **Not established**: the exact runtime sequence, approvals and environment promotion path beyond the workflow definitions themselves. The repository names the target and automation, but not the live release policy. ^[gap]

## 🔗 Related

- [DevOps overview](overview.md)
- [Infrastructure](../05.%20Infrastructure/overview.md)
- [Security](../09.%20Security/overview.md)

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
