---
title: "DevOps"
author: "Dario Airoldi"
date: "2026-08-16"
categories: [repository-documentation, devops]
description: "Build and deployment automation present in the repository."
---

# DevOps

## 🎯 Purpose

This chapter records the build and deployment automation implied by the repository: Microsoft-hosted GitHub Actions workflows deliver the solution to Azure App Service and manage tool deployments.

## 🗺️ Pages in this chapter

| Page | Covers |
|---|---|
| [deployment automation](deployment-automation.md) | GitHub Actions and Azure deployment flow |

## 🔑 Key points

- The repository contains deployment automation for tools and App Service rollout.
- The main deployment target is Azure App Service.
- The repository does not establish an end-to-end code pipeline beyond the workflow definitions themselves.

## 🔗 Related

- [Infrastructure](../05.%20Infrastructure/overview.md)
- [Security](../09.%20Security/overview.md)
- [Validation](../08.%20Validation/overview.md)
