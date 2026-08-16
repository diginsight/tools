---
title: "Diginsight.SmartDocs.Web — convergence, multi-space rendering and Testmc deployment"
author: "Dario Airoldi"
date: "2026-08-15"
categories: [smartdocs, rendering, blazor, deployment, github-actions, azure]
description: "Moves the Markdown rendering application into this repository as Diginsight.SmartDocs.Web, introduces the multi-space model so one host renders several repositories, relocates the Testmc configuration to tools.internal, and deploys from this repository to the existing App Service."
status: actionable
---

# Diginsight.SmartDocs.Web — convergence, multi-space rendering and Testmc deployment

## 📑 Table of contents

- [🎯 Goal and scope](#-goal-and-scope)
- [🧭 Decisions taken](#-decisions-taken)
- [⚠️ Pre-flight warnings](#️-pre-flight-warnings)
- [📦 WS-A-relocation — projects into `src/50.00 Docs`](#-ws-a-relocation--projects-into-src5000-docs)
- [🧱 WS-B-space-model — configuration and resolution](#-ws-b-space-model--configuration-and-resolution)
- [🔌 WS-C-endpoints — space-addressed surface](#-ws-c-endpoints--space-addressed-surface)
- [🖼️ WS-D-space-index — generated index and switcher](#️-ws-d-space-index--generated-index-and-switcher)
- [🔨 WS-E-solution — registration and build](#-ws-e-solution--registration-and-build)
- [🔐 WS-F-internal-config — move Testmc configuration to `tools.internal`](#-ws-f-internal-config--move-testmc-configuration-to-toolsinternal)
- [🚀 WS-G-deployment — build and deploy from this repository](#-ws-g-deployment--build-and-deploy-from-this-repository)
- [📤 WS-H-content-publishing — the `diginsight.tools` space](#-ws-h-content-publishing--the-diginsighttools-space)
- [🧪 WS-I-validation — visible browser evidence](#-ws-i-validation--visible-browser-evidence)
- [🧹 WS-J-retirement — remove from the source repositories](#-ws-j-retirement--remove-from-the-source-repositories)
- [🔎 Discovery](#-discovery)
- [🗳️ Open decisions](#️-open-decisions)
- [🅿️ Park lot](#️-park-lot)
- [🏁 Exit criteria](#-exit-criteria)
- [📚 References](#-references)

## 🎯 Goal and scope

Move the three-project Markdown rendering application into this repository as **`Diginsight.SmartDocs.Web`**, generalise it from one content root to **many spaces**, and make it deploy from here to the existing Testmc App Service — so the same host renders both the learning-hub content and this repository's own generated documentation.

| Concern | Today | After |
|---|---|---|
| Application location | `Learn.01/src/Learn.Web{,.Client,.Shared}` | `tools.01/src/50.00 Docs/Diginsight.SmartDocs.Web{,.Client,.Shared}` |
| Content roots | one, fixed | many, one per space, config-driven |
| Route | `/**` | `/{space-id}/**`, with a generated index at `/` |
| Testmc configuration | `Learn.internal` | `tools.internal` |
| Build and deploy | `darioairoldi/Learn` → `deploy-learnweb.yml` | `diginsight/tools` → dedicated `22.DeploySmartDocsWeb.yml` |
| Target App Service | `learn-testmc-app-itn-01`, 32-bit worker | `learn-testmc-app-itn-01`, **64-bit worker** |

**In scope** — relocation, the space model, space-addressed endpoints, the generated index and switcher, solution registration, the configuration move, deployment parity with the current action, content publishing for this repository's `src/docs`, browser validation, and retirement at source once verified.

**Explicit non-goals** — no new Azure infrastructure (the App Service and storage account already exist and are reused); no AI content services; no per-space theming beyond title and icon; no change to the learning-hub content-publishing workflow beyond what backward compatibility already provides.

## 🧭 Decisions taken

These are closed. Re-opening any of them drops this plan back to `status: draft`.

**`D1-name-smartdocs`** — the projects are `Diginsight.SmartDocs.Web`, `Diginsight.SmartDocs.Web.Client`, `Diginsight.SmartDocs.Web.Shared`. `Smart` is this repository's established marker for AI-assisted components (`SmartAnalyzerApi`, `SmartMonitorApi`, `SmartOptimizerApi`, `SmartTestApi`), so the rendering host joins an existing family rather than arriving as an orphan.

**`D2-folder-50-00-docs`** — the projects live under `src/50.00 Docs/`, following the `NN.NN Name` convention (`01.00 Console`, `10.00 Client`, `20.00 Api`, `30.00 Job`, `40.00 Service`). The FeedMonitor test project was renumbered to `60.00 Test` in the sibling plan to avoid the collision this decision creates.

**`D3-site-section`** — the configuration section is `Site`, holding `Spaces[]`. The current flat `Content` section is replaced, not extended, because a section named for a single content root cannot honestly hold a list of them. The obsolete `Content__*` application settings are removed in the same step that writes the new ones.

**`D4-container-per-space`** — each space maps to its **own blob container** on the shared storage account, not to a prefix inside one container. Rationale: the `learn` container already exists with content at its root, so per-container mapping leaves the learning-hub publishing workflow **completely unchanged**, and a per-repository SAS can be scoped to a container boundary.

**`D5-id-vs-container-naming`** — a space `Id` is a URL path segment and may contain dots (`diginsight.tools`); an Azure blob container name may **not** (lowercase letters, digits and hyphens only). The two are therefore separate fields: `Id: diginsight.tools` maps to `ContainerName: diginsight-tools`. Never derive one from the other by convention.

**`D6-invalidate-backward-compatible`** — the cache-invalidation endpoint keeps its current path `/_nav/invalidate` and its `X-Invalidate-Key` header, and gains an **optional** `?space={id}` query parameter. Absent parameter means "invalidate every space". The existing learning-hub content workflow therefore keeps working with no edit.

**`D7-single-space-is-degenerate`** — a configuration with exactly one space must behave exactly as the application does today. This is the regression guard for the whole workstream: if the single-space case changes behaviour, the space model is wrong.

**`D8-publish-profile-x64`** — the publish step produces a **self-contained `win-x64`** payload and keeps the **zero-byte Brotli asset scrub**.

The architecture change is not free: the target App Service currently runs a **32-bit worker process**, and a `win-x64` payload on a 32-bit worker fails to load with `HTTP 500.32 - ANCM Failed to Load dll`. Switching the worker to 64-bit is therefore a **prerequisite of the first deployment**, not a remediation applied after a failure — it is `Step G3`, and it runs before the deploy action every time so a manually reverted portal setting cannot silently break a later run.

The Brotli scrub is unrelated to architecture and stays unconditional: it removes broken Brotli siblings from both `wwwroot/_framework` and the static-web-assets endpoint manifest, so browsers negotiate gzip or identity instead of receiving an empty asset.

**`D9-config-authority`** — `appsettings.Testmc.json` in `tools.internal` is authoritative for the space list. App Service application settings carry only `AppsettingsEnvironmentName` and the invalidation key. Rationale: a space list is a structured array; expressing it as flat double-underscore settings is unreadable and drifts. This differs deliberately from the current action, which pins storage settings as explicit overrides — the override is replaced by fail-closed validation of the configuration file at publish time, which the current action already performs.

**`D10-vars-for-configuration-secrets-for-secrets`** — the rule is mechanical and admits no exceptions: **`vars.`** for everything non-sensitive, **`secrets.`** for everything sensitive.

Non-sensitive means the OIDC identifiers (`AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`), the App Service name, the resource group, the storage account and the container names — none of these grant access on their own. Sensitive means the internal-repository token, the cross-repository dispatch token and the cache-invalidation key.

The source repository placed the OIDC identifiers in `secrets.*`; do not carry that over. Variables and secrets that do not exist yet are **provisioned as part of `Step G1`**, not assumed to be present — an absent value resolves to an empty string rather than an error, so a missing variable surfaces as a confusing login failure much later.

**`D11-cutover-then-retire`** — projects are added here, deployed, and verified in a browser **before** anything is removed from the source repositories. Retirement is `WS-J-retirement`, gated on the validation exit criterion.

**`D12-scaffolds-untouched`** — `src/20.00 Api/SmartDocs` and `src/20.00 Api/SmartDocsApi` are left alone. There is no collision: different folder, different assembly names (`SmartDocs` versus `Diginsight.SmartDocs.Web`). Cleanup is not part of the request and is parked.

**`D13-dedicated-deployment-workflow`** — `Diginsight.SmartDocs.Web` gets its **own end-to-end workflow**, `22.DeploySmartDocsWeb.yml`, which both builds and deploys. It does **not** call the reusable `21.DeployAppService.yml`, and neither that workflow nor its existing caller is modified.

Rationale: `21.DeployAppService.yml` is shaped around WebJob packaging — it hardcodes an artifact name, inspects `Settings.job` to choose triggered versus continuous, and has no notion of application settings or worker architecture. Bending it to also carry a self-contained web-application payload, a settings block and a platform switch would make one file serve two unrelated deployment shapes, and every future change to either would risk the other. A dedicated workflow keeps the FeedMonitor deployment path provably unaffected: it is not touched, so it cannot regress.

## ⚠️ Pre-flight warnings

Read before executing any step.

- **The two repositories share the same `Directory.Build.props` shape** — the same `DiginsightCoreSolutionDirectory` / `DiginsightCoreDirectImport` conditional-import switches and the same `DiginsightCoreVersion` / `DiginsightSmartcacheVersion` / `DiginsightComponentsVersion` properties. The project files therefore port with **name changes only**. Do not restructure them.
- **`RestorePackagesWithLockFile` is `true`** in `src/Directory.Build.props`. The first restore generates `packages.lock.json` for all three new projects. These files must be committed — the deployment workflow's NuGet cache keys on `hashFiles('**/packages.lock.json')`.
- **Baseline is 0 errors and roughly 166 pre-existing nullable warnings** (CS8604 / CS8618). The bar is 0 errors; do not treat the warning count as a regression signal unless it rises sharply.
- **The App Service runs a 32-bit worker process today.** Per `D8-publish-profile-x64` the payload is `win-x64`, so the platform must be switched **before** the first deployment or the site answers `HTTP 500.32 - ANCM Failed to Load dll`. This is `Step G3`.
- **Do not modify `21.DeployAppService.yml` or `20.DeployTools.yml`.** Per `D13-dedicated-deployment-workflow` the FeedMonitor deployment path stays exactly as it is. If a step in `WS-G-deployment` appears to require editing either file, the step is wrong, not the decision.
- **A missing repository variable is not an error at expansion time** — it resolves to an empty string. `Step G1` provisions every variable and secret up front precisely so that an omission fails loudly at a validation step rather than quietly at `azure/login`.
- **`tools.internal` has no `.github/` folder.** The configuration-dispatch workflow directory must be created there.
- **`src/20.00 Api/SmartDocs/` is absent from `Diginsight.Tools.sln`.** If a broad restore is ever run over the folder rather than the solution, it will be picked up. Restore and build against the solution file.
- **Line numbers and file inventories in this plan are anchored to 2026-08-15.** Re-locate by symbol name, not by line, if execution is delayed.

## 📦 WS-A-relocation — projects into `src/50.00 Docs` (🟡 todo)

### Step A1 — Copy the three projects (🟡 todo)

Copy — do not move, per `D11-cutover-then-retire` — from `Learn.01/src/` into `tools.01/src/50.00 Docs/`, excluding `bin/`, `obj/` and `packages.lock.json`:

| Source | Destination |
|---|---|
| `Learn.Web/` | `Diginsight.SmartDocs.Web/` |
| `Learn.Web.Client/` | `Diginsight.SmartDocs.Web.Client/` |
| `Learn.Web.Shared/` | `Diginsight.SmartDocs.Web.Shared/` |

Use `Copy-Item`, not `git mv` — the destination files are untracked at this point.

### Step A2 — Rename the project files and their identity properties (🟡 todo)

Rename each `.csproj` to match its folder, and update `RootNamespace` and `AssemblyName` to the new names. Update `UserSecretsId` in the web project to `diginsight-smartdocs-web`. Update the three `ProjectReference` paths so the web project references the client and shared projects by their new file names, and the client references the shared project by its new file name.

Leave the `PackageReference` and conditional Diginsight `ProjectReference` blocks **untouched** — they resolve against this repository's `Directory.Build.props` unchanged.

### Step A3 — Rewrite namespaces and using directives (🟡 todo)

Replace the namespace roots across all `.cs`, `.razor` and `_Imports.razor` files:

| Old | New |
|---|---|
| `Learn.Web.Shared` | `Diginsight.SmartDocs.Web.Shared` |
| `Learn.Web.Client` | `Diginsight.SmartDocs.Web.Client` |
| `Learn.Web` | `Diginsight.SmartDocs.Web` |

Apply longest-prefix-first so `Learn.Web.Shared` is not first mangled by the `Learn.Web` rule. Also update the assembly name referenced in `App.razor` / `Program.cs` for the WebAssembly root component assembly, and any `typeof(...).Assembly` lookups that name the client assembly.

### Step A4 — Rewrite non-code identifiers (🟡 todo)

Search for the remaining literal occurrences and update them: `launchSettings.json` profile names, the log4net configuration file, `appsettings*.json` logging category filters (`"Learn.Web": "Information"` → `"Diginsight.SmartDocs.Web": "Information"`), and the static-web-assets file names implied by the assembly rename.

## 🧱 WS-B-space-model — configuration and resolution (🟡 todo)

### Step B1 — Replace `ContentOptions` with `SiteOptions` (🟡 todo)

In `Diginsight.SmartDocs.Web`, replace `ContentOptions.cs` with `SiteOptions.cs` binding the `Site` section:

```jsonc
{
  "Site": {
    "Title": "Diginsight Documentation",
    "NotFoundPath": "404.html",
    "InvalidateApiKey": "",
    "Spaces": [
      {
        "Id": "learn",
        "RouteBase": "/learn",
        "Title": "Learning Hub",
        "Icon": "🎓",
        "Source": "Blob",
        "Blob": {
          "AccountUri": "https://<account>.blob.core.windows.net",
          "ContainerName": "learn"
        }
      },
      {
        "Id": "diginsight.tools",
        "RouteBase": "/diginsight.tools",
        "Title": "Diginsight Tools",
        "Icon": "🔧",
        "RepositoryUrl": "https://github.com/diginsight/tools",
        "Source": "Blob",
        "Blob": {
          "AccountUri": "https://<account>.blob.core.windows.net",
          "ContainerName": "diginsight-tools"
        }
      }
    ]
  }
}
```

A space carries `Id`, `RouteBase`, `Title`, `Icon`, `RepositoryUrl`, `Source` (`Blob` or `FileSystem`), and exactly one of `Blob { AccountUri, ContainerName }` or `FileSystem { RootPath, WatchForChanges }`. Sources are **per space and may differ** — this is what allows one space to be served from the working tree during development while the others stream from storage.

### Step B2 — Add `SpaceRegistry` (🟡 todo)

Create `Spaces/SpaceRegistry.cs`. Responsibilities: validate the configured spaces at startup, expose lookup by id and by route base, and expose the ordered list for the index and switcher.

Validation is fail-fast at startup: ids unique and non-empty; ids restricted to characters valid in a URL path segment; route bases unique and rooted; exactly one source block populated per space. A space list that fails validation must stop the host, not degrade silently — a mis-typed id would otherwise surface as a 404 on a page that used to work.

Bind through `IOptionsMonitor<SiteOptions>` and rebuild the registry on change, so a configuration reload adds a space without a restart.

### Step B3 — Add `SpaceContentSourceFactory` (🟡 todo)

Create `Spaces/SpaceContentSourceFactory.cs` resolving a space id to an `IContentSource`. `IContentSource` is already an interface with a single `GetAsync(contentKey, ct)` member, so `BlobContentSource`, `FileSystemContentSource` and `CachedContentSource` are constructed per space and **not otherwise modified**. Cache each resolved source per space id; rebuild the set when the registry rebuilds.

### Step B4 — Add the space dimension to the cache key (🟡 todo)

`Caching/ContentPathCacheKey.cs` gains a space segment. Without it, two spaces holding a file at the same relative path collide in cache — a silent cross-space content leak, and the single most likely defect in this workstream.

### Step B5 — Make navigation space-aware (🟡 todo)

`Navigation/DynamicNavBuilder.cs` and `CachedDynamicNavBuilder.cs` take a space and build against that space's content source. `Navigation/FolderMetricsIndex.cs` is keyed per space. `Navigation/NavRules.cs` in the shared project is pure convention logic and stays unchanged — its numeric-prefix, date-prefix and title-casing rules already match this repository's folder naming.

### Step B6 — Make metrics opt-in per space (🟡 todo)

`NavStats`, `RepoStats` and `Coverage` are learning-hub-specific measures. Add a per-space boolean that defaults to off, and skip their computation when it is off. A repository documentation space should not be reporting article-coverage percentages.

## 🔌 WS-C-endpoints — space-addressed surface (🟡 todo)

### Step C1 — Add `/_spaces` (🟡 todo)

New `Endpoints/SpaceEndpoints.cs` returning the registry projection consumed by the index page and the switcher: `id`, `routeBase`, `title`, `icon`, `repositoryUrl`, and the live values `articleCount`, `lastPublishedUtc` and `reachable`. The live values are what a statically generated landing page cannot carry, and are the reason this endpoint exists rather than a build-time file.

### Step C2 — Space-address the content and nav endpoints (🟡 todo)

`Endpoints/ContentEndpoints.cs` → `/_content-raw/{space}/{**key}`. `Endpoints/NavEndpoints.cs` → `/_nav/{space}`. Both resolve the space through `SpaceRegistry` and return `404` for an unknown id — never fall back to a default space, because a silent fallback turns a typo into wrong content.

### Step C3 — Extend invalidation without breaking the caller (🟡 todo)

`/_nav/invalidate` keeps its path, its `POST` method and its `X-Invalidate-Key` header, and accepts an optional `?space={id}`. Present → invalidate that space only. Absent → invalidate all spaces. Per `D6-invalidate-backward-compatible`, this keeps the existing learning-hub content workflow working with no edit to it.

### Step C4 — Group hub subscriptions by space (🟡 todo)

`Navigation/NavHub.cs` and `NavChangePublisher.cs` group SignalR subscriptions by space id, so publishing a change to one space notifies only the clients viewing it. Add a `spaces` group that broadcasts registry changes, so a space added at runtime appears in every connected client's index and switcher without a reload.

### Step C5 — Route client requests through the space (🟡 todo)

In the client project, `HttpContentSource.cs` and `HttpNavProvider.cs` take the current space id from the route and call the space-addressed endpoints. `NavHubClient.cs` subscribes to the current space's group plus the `spaces` group.

## 🖼️ WS-D-space-index — generated index and switcher (🟡 todo)

Both surfaces render from `/_spaces`. Neither enumerates spaces in markup, so adding a space is a configuration change only.

### Step D1 — Add the `SpaceIndex` page (🟡 todo)

A page in the shared project, served at `/`, rendering one card per space: icon, title, a documentation link to the space's route base, a repository link when `RepositoryUrl` is set, and the live counts from `/_spaces`. It iterates the registry — it must not contain a literal space name. Handle the loading, unreachable-space and empty-registry states explicitly.

### Step D2 — Make the index copy editable without a rebuild (🟡 todo)

Heading, introduction and footer come from an optional Markdown fragment resolved through the ordinary content pipeline; the card grid is generated. Absent fragment → fall back to `Site:Title` and no introduction. This keeps wording a content edit while keeping the space list generated.

### Step D3 — Add the switcher to `TopMenu` (🟡 todo)

`Layout/TopMenu.razor` gains a space switcher bound to the same `/_spaces` projection, marking the current space. `Layout/MainLayout.razor` reads the current space's title and icon for branding. Both update live from the `spaces` hub group.

### Step D4 — Handle the root route (🟡 todo)

`/` renders the index. `/{space-id}/**` renders content. An unknown first segment returns the configured not-found page rather than being treated as a content path in a default space.

## 🔨 WS-E-solution — registration and build (🟡 todo)

### Step E1 — Add a `50.00 Docs` solution folder (🟡 todo)

Add the folder to `src/Diginsight.Tools.sln` and nest the three projects under it, mirroring how `30.00 Job` nests `Diginsight.Tools.FeedMonitor`.

### Step E2 — Restore, generate lock files, build (🟡 todo)

```powershell
cd "c:\dev\darioa\Diginsight\tools.01\src"
dotnet restore "Diginsight.Tools.sln" --force-evaluate --nologo
dotnet build   "Diginsight.Tools.sln" --no-restore -v m --nologo
```

`--force-evaluate` is restore-only; passing it to `build` fails with `MSB1001`. Acceptance: **0 errors**. Commit the three generated `packages.lock.json` files.

### Step E3 — Run locally against the filesystem source (🟡 todo)

Add `appsettings.Development.json` with a single space `diginsight.tools` on `Source: FileSystem`, `RootPath` pointing at `src/docs`, `WatchForChanges: true`. Run the web project in a **visible foreground console** — not a hidden or background process — rebuilding rather than using `--no-build`, so client WebAssembly changes are served. This renders this repository's existing documentation with no storage account and no deployment, and is the cheapest possible proof that the port is sound.

## 🔐 WS-F-internal-config — move Testmc configuration to `tools.internal` (🟡 todo)

Operates in `c:\dev\darioa\Diginsight\tools.internal`.

### Step F1 — Create the configuration file (🟡 todo)

Create `src/50.00 Docs/Diginsight.SmartDocs.Web/appsettings.Testmc.json`, mirroring the path convention already used there for `src/30.00 Job/Diginsight.Tools.FeedMonitor/appsettings.Testmc.json`.

Port the existing Testmc content — logging levels, `Observability`, `OpenTelemetry`, `AzureKeyVault` — updating the logging category to the new assembly name, and replace the flat `Content` block with the `Site` block from Step B1 carrying both spaces:

| Space | Container |
|---|---|
| `learn` | `learn` — the existing container, unchanged |
| `diginsight.tools` | `diginsight-tools` — created in `WS-H-content-publishing` |

Keep the informational `Deployment` block, extending it to name both containers.

### Step F2 — Create the dispatch workflow (🟡 todo)

Create `.github/workflows/` in `tools.internal` — it does not exist yet — and add `deploy-testmc-config.yml`. It triggers on push to `main` touching the configuration file or itself, plus `workflow_dispatch`, and dispatches the SmartDocs deployment workflow in `diginsight/tools`. This mirrors the dispatch that exists in the source internal repository; use this repository's cross-repository token variable rather than carrying over the source repository's token name.

### Step F3 — Confirm the read path (🟡 todo)

The deployment workflow in `WS-G-deployment` checks out `diginsight/tools.internal` using `INTERNAL_REPOSITORY_TOKEN`, which `20.DeployTools.yml` already uses successfully. No new secret is required.

## 🚀 WS-G-deployment — build and deploy from this repository (🟡 todo)

Per `D13-dedicated-deployment-workflow` this workstream adds **one new file** and edits **no existing workflow**.

### Step G1 — Provision the repository variables and secrets (🟡 todo)

Create everything the new workflow reads, before writing it. Per `D10-vars-for-configuration-secrets-for-secrets`, non-sensitive values are repository **variables** and sensitive values are repository **secrets**.

| Name | Kind | Value | Already exists |
|---|---|---|---|
| `AZURE_CLIENT_ID` | variable | OIDC application identifier | yes — used by `21.DeployAppService.yml` |
| `AZURE_TENANT_ID` | variable | tenant identifier | yes |
| `AZURE_SUBSCRIPTION_ID` | variable | subscription identifier | yes |
| `SMARTDOCS_WEBAPP_NAME` | variable | `learn-testmc-app-itn-01` | no — create |
| `SMARTDOCS_RESOURCE_GROUP` | variable | `learn-testmc-rg-itn-01` | no — create |
| `SMARTDOCS_STORAGE_ACCOUNT` | variable | `digitoolstestmcstitn01` | no — create |
| `INTERNAL_REPOSITORY_TOKEN` | secret | read access to `diginsight/tools.internal` | yes — used by `20.DeployTools.yml` |
| `SMARTDOCS_INVALIDATE_KEY` | secret | cache-invalidation key | no — create |

The App Service and storage identifiers are variables rather than hardcoded literals so that `PL-6-dedicated-app-service` becomes a variable change rather than a workflow edit.

Add a first job step that asserts every value above is non-empty and fails the run listing the missing names. Without it an omission surfaces much later as an unauthenticated Azure call.

### Step G2 — Add the build job of `22.DeploySmartDocsWeb.yml` (🟡 todo)

New standalone workflow. Triggers: push to `main` touching `src/50.00 Docs/**`, `src/Directory.Build.props`, `src/Directory.Build.targets` or itself; plus `workflow_dispatch`. `permissions: id-token: write, contents: read`. Concurrency group `deploy-smartdocs`, no cancel-in-progress.

Build job on `self-hosted`:

1. Run the `Step G1` value assertion.
2. Check out this repository.
3. Check out `diginsight/tools.internal` with `secrets.INTERNAL_REPOSITORY_TOKEN`, sparse to the SmartDocs configuration file.
4. Set up the .NET 10 SDK — see `DSC1-runner-sdk`.
5. Restore `src/Diginsight.Tools.sln`, cached on `hashFiles('**/packages.lock.json')`.
6. Publish `-c Release -r win-x64 --self-contained true` per `D8-publish-profile-x64`, into `./publish/Diginsight.SmartDocs.Web`. Fail if the produced executable is missing.
7. Apply the zero-byte Brotli scrub: remove empty `.br` files under `wwwroot/_framework` **and** their entries in the static-web-assets endpoint manifest, then fail the build if any zero-byte framework asset remains.
8. Copy the Testmc configuration from the internal checkout into the publish root. Parse it first and fail closed if it is missing or unparseable — a deployment carrying no space list would serve an empty site.
9. Upload the artifact as `smartdocs-web`.

### Step G3 — Switch the App Service to a 64-bit worker (🟡 todo)

First step of the deploy job, before the deploy action. Log in with `azure/login@v2` using the OIDC variables, then set the worker process to 64-bit on `SMARTDOCS_WEBAPP_NAME` in `SMARTDOCS_RESOURCE_GROUP`.

Run this **on every deployment**, not once by hand: the setting is idempotent, and running it unconditionally means a manually reverted portal value cannot silently break a later run. This is the prerequisite established by `D8-publish-profile-x64`.

### Step G4 — Apply and prune the application settings (🟡 todo)

Still in the deploy job, before the deploy action. Set:

```text
AppsettingsEnvironmentName=Testmc
Site__InvalidateApiKey=<secrets.SMARTDOCS_INVALIDATE_KEY>
Content__MetricsSnapshotPath=D:\home\data\nav-metrics-snapshot.json
```

Then delete the settings left behind by the previous flat configuration: `Content__Source`, `Content__Blob__AccountUri`, `Content__Blob__ContainerName`, `Content__InvalidateApiKey`. Per `D3-site-section` they no longer bind, and leaving them in place would make a future reader believe the site is single-space.

Applying settings from the workflow rather than the portal is what keeps environment selection reproducible — the current action does the same, and this is the parity requirement.

### Step G5 — Deploy and confirm the target (🟡 todo)

Download the `smartdocs-web` artifact and deploy it with `azure/webapps-deploy@v3` to the Production slot.

Confirm from the run log that the deployment targeted `learn-testmc-app-itn-01` in resource group `learn-testmc-rg-itn-01`, that the worker platform reads 64-bit, and that the site answers on its hostname.

## 📤 WS-H-content-publishing — the `diginsight.tools` space (🟡 todo)

### Step H1 — Add `23.PublishDocsContent.yml` (🟡 todo)

New workflow publishing this repository's documentation into the `diginsight-tools` container. Triggers: push to `main` touching `src/docs/**`, plus `workflow_dispatch`. Concurrency group `publish-docs-content`.

Stage `src/docs/**` — Markdown plus images — preserving repository-relative paths, excluding `bin`, `obj` and `node_modules`. Fail if nothing was staged, so an empty stage can never reach the container-reset path.

### Step H2 — Create the container and mirror the content (🟡 todo)

Log in with OIDC using `vars.AZURE_*` and `vars.SMARTDOCS_STORAGE_ACCOUNT` per `D10-vars-for-configuration-secrets-for-secrets`. Create the `diginsight-tools` container if absent, upload the staged content, and only **after a successful upload** prune blobs no longer present in the stage. Upload-then-prune makes a cleanup failure non-destructive: new content is live and stale blobs are removed on the next run.

### Step H3 — Invalidate the space (🟡 todo)

`POST /_nav/invalidate?space=diginsight.tools` with the `X-Invalidate-Key` header carrying `secrets.SMARTDOCS_INVALIDATE_KEY`. Best-effort with `continue-on-error: true` — a cache that refreshes on its own schedule is not a deployment failure.

### Step H4 — Confirm the learning-hub workflow still works (🟡 todo)

The learning-hub content workflow calls `/_nav/invalidate` with no space parameter. Confirm from the running site that this still invalidates successfully after `WS-C-endpoints`. This is the direct test of `D6-invalidate-backward-compatible`.

## 🧪 WS-I-validation — visible browser evidence (🟡 todo)

Mandatory for this change. Record the run as a validation-sequence Markdown with screenshots under this work item's `_validation/` folder, images in `_validation/images/`, front matter `publish: false`, following `testing-validation.instructions.md`.

### Step I1 — Capture the space index (🟡 todo)

Navigate to `/`. Capture: both space cards, their icons and titles, the documentation and repository links, and the live counts.

### Step I2 — Capture each space (🟡 todo)

Navigate to `/learn` and to `/diginsight.tools`. Capture for each: the navigation tree, a rendered article with a Mermaid diagram and a table of contents, and the branding showing the correct space title.

### Step I3 — Capture the switcher (🟡 todo)

Switch from one space to the other using the top-bar switcher. Capture before, during and after, confirming both the route base and the branding changed.

### Step I4 — Capture cross-space isolation (🟡 todo)

Request a path that exists in one space and not the other. Capture the not-found page. This is the direct test of the cache-key change in Step B4 — a leak here means Step B4 is incomplete.

### Step I5 — Capture live invalidation (🟡 todo)

With a browser open on a space, publish a content change and confirm the navigation updates without a reload. Capture before and after.

Note: the automated browser can render in responsive rail mode at roughly 592 pixels and throttles painting when occluded. Bring the window to the front, wait for counts to settle, and read the live DOM value for exact assertions.

## 🧹 WS-J-retirement — remove from the source repositories (🟡 todo)

**Gated.** Do not begin until the `WS-I-validation` exit criterion is met. Every step here is destructive and outside this repository.

### Step J1 — Retire the source deployment workflow (🟡 todo)

In `Learn.01`, delete `.github/workflows/deploy-learnweb.yml`. The application is no longer built there. Leave `deploy-learninghub.yml` in place — it still publishes learning-hub content to the `learn` container and is unaffected.

### Step J2 — Remove the source projects (🟡 todo)

In `Learn.01`, remove `src/Learn.Web/`, `src/Learn.Web.Client/`, `src/Learn.Web.Shared/` and their entries from the solution files. Verify the remaining solution still restores and builds before committing.

### Step J3 — Remove the source internal configuration (🟡 todo)

In `Learn.internal`, remove `src/Learn.Web/appsettings.Testmc.json` and `.github/workflows/deploy-testmc-config.yml`. Both now live in `tools.internal`. Confirm the file exists at the destination before removing the source — the move is complete only when the destination is committed.

### Step J4 — Redeploy and re-verify (🟡 todo)

Trigger the SmartDocs deployment workflow once more after retirement and confirm the site still serves both spaces. This proves no removed file was still participating in the build.

## 🔎 Discovery

Items undecidable until execution. Each carries a defined negative branch.

**`DSC1-runner-sdk`** — the source repository's workflow documents that `actions/setup-dotnet` fails on its self-hosted runner because it cannot write to the system SDK location, and verifies the pre-installed SDK instead. This repository's `20.DeployTools.yml` uses `actions/setup-dotnet@v5` successfully. Start with `actions/setup-dotnet@v5` at `Step G2`. If it fails on the runner → replace it with the pre-installed-SDK verification block, asserting a .NET 10 SDK is present and failing the job otherwise.

**`DSC2-oidc-storage-role`** — whether the OIDC identity used by this repository holds `Storage Blob Data Contributor` on the storage account. Attempt container creation at Step H2. If it fails with an authorisation error → stop and record the required role assignment as an open decision; do not fall back to an account key, which would introduce a secret where managed identity is already the pattern.

**`DSC3-app-service-runtime-stack`** — whether the App Service is configured for a runtime stack that conflicts with a self-contained deployment. Inspect the configuration at `Step G3`, alongside the worker-platform switch. If a framework-dependent stack is pinned → set the stack to the one matching a self-contained deployment in the same step, and record it in the deployment block of the internal configuration file.

## 🗳️ Open decisions

None. All decisions are recorded in § Decisions taken.

## 🅿️ Park lot

Out of scope for this plan. Not to be executed here.

- **`PL-1-quarto-retirement`** — `_quarto.yml`, `index.qmd`, `.quartoignore` and `quarto-publish.yml` become redundant once the `diginsight.tools` space serves `src/docs`. Retire only after the space is live and verified. → defer
- **`PL-2-ai-content-services`** — semantic search, summarisation and question answering over the spaces, as a separate service rather than inside the renderer. `src/20.00 Api/SmartDocsApi` is the natural home. → defer
- **`PL-3-scaffold-cleanup`** — `src/20.00 Api/SmartDocs` is a git-tracked, solution-absent `dotnet new webapi` scaffold. → defer
- **`PL-4-per-space-theming`** — colour and stylesheet per space, beyond title and icon. → defer
- **`PL-5-additional-spaces`** — onboarding further repositories. The model supports it with configuration only; each needs a container and a publishing workflow. → defer
- **`PL-6-dedicated-app-service`** — the host currently runs on an App Service named for the learning hub. Renaming or migrating is cosmetic and carries DNS and role-assignment cost. → defer
- **`PL-7-learning-hub-context-generalisation`** — `.copilot/context/90.00-learning-hub/` holds site-specific rules that partly apply to any space. → `01-autonomous-streams-artifacts.plan.md`
- **`PL-8-space-level-authorisation`** — per-space access control. Every space is currently public. → defer

## 🏁 Exit criteria

- The three projects exist under `src/50.00 Docs/` with the new names, and `Diginsight.Tools.sln` builds with **0 errors**. (🟡 todo)
- `packages.lock.json` is committed for all three projects. (🟡 todo)
- A single-space configuration behaves exactly as the application does today, per `D7-single-space-is-degenerate`. (🟡 todo)
- The local filesystem-source run renders `src/docs` in a visible browser. (🟡 todo)
- `appsettings.Testmc.json` and the dispatch workflow exist in `tools.internal`. (🟡 todo)
- Every variable and secret in the `Step G1` table exists, and the assertion step passes. (🟡 todo)
- `22.DeploySmartDocsWeb.yml` deployed `Diginsight.SmartDocs.Web` to `learn-testmc-app-itn-01`, and the log confirms the target. (🟡 todo)
- `21.DeployAppService.yml` and `20.DeployTools.yml` are byte-identical to their state before this plan, and a FeedMonitor deployment still succeeds. (🟡 todo)
- The App Service worker platform reads 64-bit and the site starts — no `HTTP 500.32`. (🟡 todo)
- Obsolete `Content__*` application settings are removed. (🟡 todo)
- Both spaces render from storage, and the generated index at `/` lists both without any space name appearing in markup. (🟡 todo)
- The learning-hub content workflow still invalidates successfully with no space parameter. (🟡 todo)
- A validation-sequence Markdown with screenshots exists under `_validation/`, covering the index, both spaces, the switcher, cross-space isolation and live invalidation. (🟡 todo)
- Source projects, the source deployment workflow and the source internal configuration are removed, and a subsequent deployment still serves both spaces. (🟡 todo)

## 📚 References

- **📘** `.github/workflows/20.DeployTools.yml` — the existing build workflow establishing the internal-configuration checkout and `vars.AZURE_*` pattern; **not modified** by this plan
- **📘** `.github/workflows/21.DeployAppService.yml` — the reusable WebJob deployment workflow; **not modified** by this plan, per `D13-dedicated-deployment-workflow`
- **📘** `.github/instructions/testing-validation.instructions.md` — mandatory validation-sequence rules for `WS-I-validation`
- **📘** `.github/instructions/plan-execution.instructions.md` — readiness gate and lifecycle this plan was authored against
- **📘** `.github/instructions/plan-marking.instructions.md` — suffix notation and identifier readability used throughout
- **📗** `src/docs/90. Issues/202608/20260815.01-documentation-platform/01-autonomous-streams-artifacts.plan.md` — sibling plan producing the documentation this rendering host serves
- **📗** `src/docs/90. Issues/202608/20260813.02-feedmonitor-feeds-support/01-robustness-fixes.plan.md` — sibling plan holding the `60.00 Test` numbering that `D2-folder-50-00-docs` depends on

<!--
validation_metadata:
  plan_id: "20260815.01-smartdocs-web-convergence"
  created: "2026-08-15"
  status: "actionable"
  gate_passed: true
  blocking_unknowns_resolved: 12
  discovery_items: 3
  sibling_plan: "01-autonomous-streams-artifacts.plan.md"
-->
