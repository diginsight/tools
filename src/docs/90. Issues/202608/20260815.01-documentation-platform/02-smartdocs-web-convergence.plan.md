---
title: "Diginsight.SmartDocs.Web — convergence, multi-space rendering and Testmc deployment"
author: "Dario Airoldi"
date: "2026-08-15"
categories: [smartdocs, rendering, blazor, deployment, github-actions, azure]
description: "Moves the Markdown rendering application into this repository as Diginsight.SmartDocs.Web, introduces the multi-space model so one host renders several repositories, adds configurable app-level branding, and deploys the same artifact twice — as the single-space learning hub on its existing App Service, and as a multi-space Diginsight documentation site on its own."
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
- [🎨 WS-K-branding — configurable app-level look and feel](#-ws-k-branding--configurable-app-level-look-and-feel)
- [🔨 WS-E-solution — registration and build](#-ws-e-solution--registration-and-build)
- [🔐 WS-F-internal-config — move Testmc configuration to `tools.internal`](#-ws-f-internal-config--move-testmc-configuration-to-toolsinternal)
- [🚀 WS-G-deployment — build and deploy from this repository](#-ws-g-deployment--build-and-deploy-from-this-repository)
- [🏢 WS-L-docs-instance — deploy the documentation site](#-ws-l-docs-instance--deploy-the-documentation-site)
- [📤 WS-H-content-publishing — the `diginsight.tools` space](#-ws-h-content-publishing--the-diginsighttools-space)
- [🧪 WS-I-validation — visible browser evidence](#-ws-i-validation--visible-browser-evidence)
- [🧹 WS-J-retirement — remove from the source repositories](#-ws-j-retirement--remove-from-the-source-repositories)
- [🔎 Discovery](#-discovery)
- [🗳️ Open decisions](#️-open-decisions)
- [🅿️ Park lot](#️-park-lot)
- [🏁 Exit criteria](#-exit-criteria)
- [📚 References](#-references)

## 🎯 Goal and scope

Port the three-project Markdown rendering application into this repository as **`Diginsight.SmartDocs.Web`**, preserving its rendering logic exactly, generalise it from one content root to **many spaces**, add **configurable app-level branding**, and make it deploy from here to the existing Testmc App Service.

The result is **one codebase deployed twice**, as two independent sites differing only by configuration — see `D17-two-deployments-one-codebase`:

| Deployment | Spaces | Routes | Branding | Host |
|---|---|---|---|---|
| **Learning Hub** | one | `/**` at the root, exactly as today | none — current appearance preserved | `learn-testmc-app-itn-01`, replacing `Learn.Web` |
| **Diginsight Documentation** | many — `diginsight.tools`, `diginsight.components`, `diginsight.telemetry`, … | `/{space-id}/**`, generated index at `/` | Diginsight logo and palette | `docs-testmc-app-itn-01`, on the same B1 plan — `D18-docs-instance-host` |

The learning hub is **not** one of the documentation spaces. It is a separate publication that happens to run the same renderer. The single-space shape is not a compatibility shim — it is the same code path with a registry of one, so the hub keeps working with **no URL change, no behaviour change and no content change**.

| Concern | Today | After |
|---|---|---|
| Application location | `Learn.01/src/Learn.Web{,.Client,.Shared}` | `tools.01/src/50.00 Docs/Diginsight.SmartDocs.Web{,.Client,.Shared}` |
| Content roots | one, fixed | many, one per space, config-driven |
| Route | `/**` | single space → `/**` **unchanged**; many spaces → `/{space-id}/**` + generated index at `/` |
| Render model | SSR prerender + interactive WebAssembly | **identical — inherited unchanged** |
| Look and feel | compiled-in stylesheet and title | configurable logo, palette and site title per deployment |
| Testmc configuration | `Learn.internal` | `tools.internal` |
| Build and deploy | `darioairoldi/Learn` → `deploy-learnweb.yml` | `diginsight/tools` → dedicated `22.DeploySmartDocsWeb.yml` |
| Target App Service | `learn-testmc-app-itn-01`, 32-bit worker | `learn-testmc-app-itn-01`, **64-bit worker** |

**In scope** — relocation, the space model, space-addressed endpoints, the generated index and switcher, configurable app-level branding, solution registration, the configuration move, deployment parity with the current action, content publishing for this repository's `src/docs`, browser validation, and retirement at source once verified.

**Explicit non-goals** — no new Azure infrastructure: both App Services, the shared plan and the storage account already exist and are reused (`D18-docs-instance-host`). No AI content services; no change to the rendering pipeline, the navigation algorithm or the component set beyond what the space dimension requires; no per-**space** theme override (branding is per deployment — `PL-4`); no new SEO artifacts beyond preserving what prerendering already produces (`PL-9`); no change to the learning-hub content-publishing workflow beyond what backward compatibility already provides.

## 🧭 Decisions taken

These are closed. Re-opening any of them drops this plan back to `status: draft`.

**`D1-name-smartdocs`** — the projects are `Diginsight.SmartDocs.Web`, `Diginsight.SmartDocs.Web.Client`, `Diginsight.SmartDocs.Web.Shared`. `Smart` is this repository's established marker for AI-assisted components (`SmartAnalyzerApi`, `SmartMonitorApi`, `SmartOptimizerApi`, `SmartTestApi`), so the rendering host joins an existing family rather than arriving as an orphan.

**`D2-folder-50-00-docs`** — the projects live under `src/50.00 Docs/`, following the `NN.NN Name` convention (`01.00 Console`, `10.00 Client`, `20.00 Api`, `30.00 Job`, `40.00 Service`). The FeedMonitor test project was renumbered to `60.00 Test` in the sibling plan to avoid the collision this decision creates.

**`D3-site-section`** — the configuration section is `Site`, holding `Spaces[]`. The current flat `Content` section is replaced, not extended, because a section named for a single content root cannot honestly hold a list of them. The obsolete `Content__*` application settings are removed in the same step that writes the new ones.

**`D4-container-per-space`** — each space maps to its **own blob container** on the shared storage account, not to a prefix inside one container. Rationale: the `learn` container already exists with content at its root, so per-container mapping leaves the learning-hub publishing workflow **completely unchanged**, and a per-repository SAS can be scoped to a container boundary.

**`D5-id-vs-container-naming`** — a space `Id` is a URL path segment and may contain dots (`diginsight.tools`); an Azure blob container name may **not** (lowercase letters, digits and hyphens only). The two are therefore separate fields: `Id: diginsight.tools` maps to `ContainerName: diginsight-tools`. Never derive one from the other by convention.

**`D6-invalidate-backward-compatible`** — the cache-invalidation endpoint keeps its current path `/_nav/invalidate`, its `POST` method and its existing `?path=` parameter, and gains an **optional** `?space={id}` query parameter. Absent parameter means "invalidate every space". The existing learning-hub content workflow therefore keeps working with no edit.

**Correction, verified 2026-08-16.** An earlier form of this decision stated the endpoint "keeps its `X-Invalidate-Key` header". It has no such header today. `ContentOptions.InvalidateApiKey` is declared and `deploy-learninghub.yml` sends `X-Invalidate-Key`, but `NavEndpoints.InvalidateNavCache` never reads it — the live endpoint is **unauthenticated**. Header validation is therefore **new work**, specified in `Step C3`, not a port. The caller is already sending the header, so adding enforcement is backward compatible in the direction that matters.

**`D7-single-space-is-degenerate`** — a configuration with exactly one space must behave exactly as the application does today. This is the regression guard for the whole workstream: if the single-space case changes behaviour, the space model is wrong. `D14-root-mount-when-single-space` states what "exactly as today" means for routing.

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

**`D14-root-mount-when-single-space`** — route prefixing is **conditional on the space count**, not unconditional.

| Spaces configured | `/` serves | Content route | Nav and content APIs |
|---|---|---|---|
| exactly one | that space's content root | `/{**path}` — **byte-identical to today** | space segment resolves to the single space; the segment is **optional** |
| more than one | the generated space index | `/{space-id}/{**path}` | space segment required |

Rationale, and why this was not obvious. The learning hub serves articles at `https://learn-testmc-app-itn-01.azurewebsites.net/00.00-getting-started` — **at the root, with no space prefix**. An unconditional `/{space-id}/**` moves every one of those URLs to `/learn/00.00-getting-started`, breaking every bookmark, every inbound link and — since prerendering exists precisely to be crawled — every indexed URL. The constraint *"LearnHub is currently working fine … SmartDocs should behave in exactly the same way so that LearnHub will be able to continue working"* makes that unacceptable.

This is not a special case bolted onto the space model; it is `D7-single-space-is-degenerate` applied to routing. One space means one unambiguous resolution, so the prefix carries no information and is omitted. The index page and the switcher are likewise suppressed — a switcher offering one destination is noise.

Redirects are **not** the answer and are explicitly rejected: they would leave the learning hub permanently serving 301s for its entire URL space, and `D7` already demands that the single-space case be indistinguishable from today.

**`D15-branding-is-per-deployment`** — logo, colour palette and site title are **app-level** configuration under `Site:Branding`, applied to every space the deployment serves. They are supplied as configuration and content assets, never compiled in.

A space keeps the `Title` and `Icon` it already has in `Step B1`; those label the space inside a branded shell. A space does **not** carry its own logo or palette.

Rationale: the request is *"a custom layout can be configured **for the app**"*, and the observed reference deployment renders every space — `/apidevice/`, `/em-adapter/` and the root index — inside one identical shell with one logo and one palette. Branding identifies the **publisher**, and one deployment has one publisher. Per-space override is a different feature with a different justification and is parked as `PL-4`.

**`D16-prerender-parity-by-construction`** — server-side rendering is **inherited**, and it survives the port only if five structural properties are preserved. Each is a construction rule that governs how steps are written, not a test run afterwards.

| # | Rule | What breaks if it is violated |
|---|---|---|
| 1 | **Symmetric registration** — every service the shared components resolve is registered in *both* `Program.cs` files with the same lifetime | server-only ⇒ prerender succeeds then hydration throws; client-only ⇒ prerender throws |
| 2 | **The space rides in the path, never in DI** | the prerendered article is replaced by an empty or wrong-space page at hydration |
| 3 | **Server content-source lifetimes stay singleton** | captive dependency: host fails at startup in Development, silently serves one space's content in Production |
| 4 | **Assembly identity tracks the rename** | the server discovers no route, prerenders *Not found*, and the client repairs it — the browser looks perfect and prerendering is gone site-wide |
| 5 | **`<base href="/" />` and the `MapStaticAssets()` ordering stay as they are** | relative content fetches break under a space prefix, or the client bootstrap is not served |

Rule 1 is a consequence of how the render model works. `ContentView` resolves `PageLoader`, `INavProvider`, `TocState`, `ArticleState` and `IJSRuntime`; the *same component* runs twice — once on the server during prerender against the server container, once in the browser against the WebAssembly container. Every one of those services is therefore registered twice today, `HttpContentSource` / `HttpNavProvider` on the client standing in for `CachedContentSource` / `ServerNavProvider` on the server. The two `Program.cs` files must always be edited in the same step.

Rule 2 is the most dangerous one and the reason this decision exists. `ContentPage.razor` declares `@page "/"` and `@page "/{*path}"` and passes `Path` down; `ContentView` never reads the URL itself, and `IContentSource.GetAsync` takes the content key **as a string**. That is the seam multi-space must use: the space is carried in the path and resolved to a key identically on both sides. It must **not** be selected by a DI factory reading `IHttpContextAccessor` — there is no `HttpContext` in WebAssembly, so such a design resolves the right space during prerender and nothing after hydration. Every build passes, every unit test passes, and the site loses its rendered first response.

Rule 3 follows from the current lifetimes. `IContentSource` and `IContentLister` are singletons, and `FolderMetricsIndex`, `DynamicNavBuilder`, `CachedDynamicNavBuilder` and `INavBuilder` are singletons that capture them. Making the content source scoped so it can observe the request is a captive dependency. Multi-space must therefore be a **singleton registry of per-space singletons**, selected by an explicit space argument — see `Step B3`.

Rule 4 is a rename hazard, not a design one. `Routes.razor` declares `AppAssembly="typeof(Marker).Assembly"` and the host calls `.AddAdditionalAssemblies(typeof(Learn.Web.Client.Marker).Assembly)`; that call is what lets the **server** find the routable `ContentPage` while prerendering. `WS-A` must treat it as a named artifact.

Consequence: with these five rules held, the port is a rename plus a path-resolution change, and the replacement is behaviourally indistinguishable. `Step I2` then **proves** the rules held — it does not create the property.

**`D17-two-deployments-one-codebase`** — the learning hub and the Diginsight documentation site are **two deployments of the same artifact**, never two spaces of one deployment.

| Deployment | Content | Spaces | Shape | Branding |
|---|---|---|---|---|
| **Learning Hub** | authored personal learning material | one | single-space, root-mounted | none — keeps its current appearance |
| **Diginsight Documentation** | generated per-repository documentation | many | multi-space, index at `/` | Diginsight logo and palette |

There are two independent reasons, and either alone is sufficient.

**They are different publications.** A learning hub is authored, personal, and addressed to its own audience; repository documentation is generated from source, exists once per repository, and belongs under a publisher's shell. Merging them puts a *Learning Hub* tile on the Diginsight documentation index and a *Diginsight Tools* tile inside a personal learning site — each is noise to the other's reader. `D15-branding-is-per-deployment` makes this concrete: one deployment has one publisher, and these have two.

**The merge is mechanically impossible anyway.** Registering `diginsight.tools` as a second space on the learning-hub deployment raises its space count to two, and by `D14-root-mount-when-single-space` that immediately moves every article from `/00.00-getting-started` to `/learn/00.00-getting-started` and replaces `/` with a space index. A single deployment can serve the learning hub unchanged **or** serve several spaces. It cannot do both, and `D7-single-space-is-degenerate` is not negotiable.

This correction supersedes the earlier shape of `Step F1`, `WS-H-content-publishing` and two exit criteria, all of which assumed one App Service carrying both.

**`D18-docs-instance-host`** — the Diginsight documentation site runs on **`docs-testmc-app-itn-01`** in resource group `learn-testmc-rg-itn-01`, sharing the existing `samples-testmc-asp-01` Basic B1 plan in `samples-testmc-rg-itn-01`. Provisioned 2026-08-16.

It is a **separate App Service sharing a plan**, not a separate plan. `D17-two-deployments-one-codebase` requires two hosts because a host serves one space list — not because it needs its own compute. `learn-testmc-app-itn-01` already sits on that plan from a different resource group, so the second site joins an established arrangement at no additional cost.

Created with what `D8-publish-profile-x64` requires, so `Step G3` has nothing to correct on this instance: **64-bit worker**, .NET 10, HTTPS-only, TLS 1.2, FTP disabled, and a system-assigned managed identity (`0442bb0f-9825-4d03-8dd3-4acf18a70e23`) for the container reads granted in `Step L4`.

Accepted consequence: B1 Basic is a single instance with no deployment slots, so the two sites share one worker and each deployment is a short outage for that site alone. `PL-10-docs-plan-capacity` holds the upgrade if that ever matters.

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
- **The render model is inherited, not rebuilt.** `App.razor` marks `HeadOutlet` and `Routes` as `@rendermode="InteractiveWebAssembly"` with prerendering left on, and `ContentView` loads content in `OnParametersSetAsync` — which runs *during* prerender. That pairing is what makes the first response contain real article HTML for crawlers while the client router takes over for SPA navigation afterwards. **Do not change render modes, do not move content loading to `OnAfterRenderAsync`, do not add `PrerenderMode=false`.** Any of the three silently removes server-side rendering while the site still looks correct in a browser.
- **`/_nav` is a route group with five children, not one endpoint.** `NavEndpoints.MapNavEndpoints` maps `/_nav/children`, `/_nav/version`, `/_nav/total`, `/_nav/index` and `/_nav/invalidate`. A naive `/_nav/{space}` route is **ambiguous with all five**. See `Step C2`.
- **`IContentSource` and `IContentLister` are the same object.** `Program.cs` registers the lister by **downcasting** the content source: `services.AddSingleton<IContentLister>(sp => (IContentLister)sp.GetRequiredService<IContentSource>())`. `BlobContentSource`, `FileSystemContentSource` and `CachedContentSource` all implement both; `HttpContentSource` (client-side) implements only `IContentSource`. A per-space factory that returns only `IContentSource` **compiles cleanly and breaks navigation at runtime**. See `Step B3`.
- **`/_nav/invalidate` is unauthenticated today.** The key is configured and sent but never checked. Treat enforcement as new code, not a port — `Step C3`.
- **There are two dependency-injection containers, not one.** `Diginsight.SmartDocs.Web/Program.cs` builds the container used for **prerendering**; `Diginsight.SmartDocs.Web.Client/Program.cs` builds the one used **after hydration**. They register the same service names with the same lifetimes and different implementations. Any step that adds, removes or re-lifetimes a service consumed by a shared component must edit **both**. See `D16-prerender-parity-by-construction`.
- **Never derive the current space from `IHttpContextAccessor` inside a DI factory.** WebAssembly has no `HttpContext`. The space must travel in the route path and be resolved to a content key by code that runs identically on both sides.
- **`.AddAdditionalAssemblies(typeof(Learn.Web.Client.Marker).Assembly)` is load-bearing for prerendering.** It is the only reason the server can route to `ContentPage`. Renaming the client assembly without updating this call removes server-side rendering from every page while leaving the browser experience apparently intact.
- **Line numbers and file inventories in this plan are anchored to 2026-08-15; code observations to 2026-08-16.** Re-locate by symbol name, not by line, if execution is delayed.

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

Apply longest-prefix-first so `Learn.Web.Shared` is not first mangled by the `Learn.Web` rule.

Four artifacts carry the client assembly's identity and must be updated by name rather than left to a bulk replace, per `D16-prerender-parity-by-construction` rule 4:

| Artifact | Location | Role |
|---|---|---|
| `Marker` class | client project root | the public type the host references to name the assembly |
| `AppAssembly="typeof(Marker).Assembly"` | `Routes.razor` (client) | how the **client** router discovers pages |
| `.AddAdditionalAssemblies(typeof(Learn.Web.Client.Marker).Assembly)` | host `Program.cs` | how the **server** discovers pages while prerendering |
| fingerprinted `_framework` payload | build output | keyed by assembly name; a stale `wwwroot` or publish folder serves the old boot manifest |

The third row is the one that fails silently. If it still names the old assembly the server routes nothing, prerenders *Not found*, and the client router repairs the page after load — so a browser shows a correct site with no server-side rendering at all.

### Step A4 — Rewrite non-code identifiers (🟡 todo)

Search for the remaining literal occurrences and update them: `launchSettings.json` profile names, the log4net configuration file, `appsettings*.json` logging category filters (`"Learn.Web": "Information"` → `"Diginsight.SmartDocs.Web": "Information"`), and the static-web-assets file names implied by the assembly rename.

### Step A5 — Prove the rename alone changed nothing (🟡 todo)

Run the renamed application **before any space work begins**, against the same filesystem content root the learning hub uses, and confirm with a plain HTTP client that an article URL returns rendered prose in the first response body.

This is a checkpoint, not a ceremony. At this point exactly one class of defect can exist — a broken identity from `Step A3` — so a failure here has a single cause and is cheap to find. The same failure discovered after `WS-B` and `WS-C` have landed is indistinguishable from a space-resolution bug and expensive to isolate. `D16` rule 4 is verified here and nowhere else.

## 🧱 WS-B-space-model — configuration and resolution (🟡 todo)

### Step B1 — Replace `ContentOptions` with `SiteOptions` (🟡 todo)

In `Diginsight.SmartDocs.Web`, replace `ContentOptions.cs` with `SiteOptions.cs` binding the `Site` section. Per `D17-two-deployments-one-codebase` there are **two** such files, one per deployment, and neither ever contains the other's spaces.

The learning-hub deployment — one space, root-mounted, unbranded:

```jsonc
{
  "Site": {
    "Title": "Learning Hub",
    "NotFoundPath": "404.html",
    "InvalidateApiKey": "",
    "Spaces": [
      {
        "Id": "learn",
        "Title": "Learning Hub",
        "Icon": "🎓",
        "Source": "Blob",
        "Blob": {
          "AccountUri": "https://<account>.blob.core.windows.net",
          "ContainerName": "learn"
        }
      }
    ]
  }
}
```

`RouteBase` is **omitted** here, and would be ignored if present: with one space the content mounts at `/`. The `Id` is still required — it keys the cache (`Step B4`) and names the metrics snapshot (`Step B6`).

The documentation deployment — many spaces, prefixed, branded:

```jsonc
{
  "Site": {
    "Title": "Diginsight Documentation",
    "NotFoundPath": "404.html",
    "InvalidateApiKey": "",
    "Branding": { "ProductName": "Diginsight", "LogoPath": "_brand/logo.svg" },
    "Spaces": [
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
      },
      {
        "Id": "diginsight.components",
        "RouteBase": "/diginsight.components",
        "Title": "Diginsight Components",
        "RepositoryUrl": "https://github.com/diginsight/components",
        "Source": "Blob",
        "Blob": {
          "AccountUri": "https://<account>.blob.core.windows.net",
          "ContainerName": "diginsight-components"
        }
      }
    ]
  }
}
```

Every space in this list is repository documentation, and the list grows by adding entries — `diginsight.telemetry` and the rest — with no code change and no redeployment of the renderer.

A space carries `Id`, `RouteBase`, `Title`, `Icon`, `RepositoryUrl`, `Source` (`Blob` or `FileSystem`), and exactly one of `Blob { AccountUri, ContainerName }` or `FileSystem { RootPath, WatchForChanges }`. Sources are **per space and may differ** — this is what allows one space to be served from the working tree during development while the others stream from storage.

### Step B2 — Add `SpaceRegistry` (🟡 todo)

Create `Spaces/SpaceRegistry.cs`. Responsibilities: validate the configured spaces at startup, expose lookup by id and by route base, and expose the ordered list for the index and switcher.

Validation is fail-fast at startup: ids unique and non-empty; ids restricted to characters valid in a URL path segment; route bases unique and rooted; exactly one source block populated per space. A space list that fails validation must stop the host, not degrade silently — a mis-typed id would otherwise surface as a 404 on a page that used to work.

Bind through `IOptionsMonitor<SiteOptions>` and rebuild the registry on change, so a configuration reload adds a space without a restart.

### Step B3 — Add `SpaceContentSourceRegistry` (🟡 todo)

Create `Spaces/SpaceContentSourceRegistry.cs`: a **singleton** holding one entry per configured space. `BlobContentSource`, `FileSystemContentSource` and `CachedContentSource` are constructed per space and **not otherwise modified**. Entries are built once and rebuilt when the registry rebuilds.

Three constraints, all of them consequences of `D16-prerender-parity-by-construction`:

**The lifetime does not change.** The registry is a singleton and each entry holds singletons. `FolderMetricsIndex`, `DynamicNavBuilder`, `CachedDynamicNavBuilder` and `INavBuilder` are singletons that capture the content source today; a scoped or request-derived content source is a captive dependency in all four. `ValidateScopes` fails the host at startup under Development, and Production binds the first scope's instance forever — every space silently serving the first space's content.

**Selection is by explicit argument, never by ambient request state.** Consumers receive the space id as a **method parameter** — `registry.For(spaceId)` — resolved from the route path by the caller. A DI factory that reads `IHttpContextAccessor` to pick the space works during prerender and returns nothing in WebAssembly, which removes the rendered first response while leaving the browser experience intact.

**Each entry exposes both contracts.** An entry carries `IContentSource` for reads and `IContentLister` for enumeration — a `SpaceContentAccess(IContentSource Source, IContentLister Lister)` record. The navigation builder needs the lister, and today obtains it by downcasting the singleton content source. An entry exposing only `IContentSource` compiles and then fails at the first navigation call.

Remove the `(IContentLister)` downcast registration from the host `Program.cs` in the same step — with per-space sources there is no singleton left to downcast, and leaving it in place resolves the wrong space's lister.

The client container gets the mirror of this: a registry whose entries wrap `HttpContentSource` against the space-addressed endpoint. Same type, same lifetime, same selection call — different implementation. Both `Program.cs` files change together.

### Step B4 — Add the space dimension to the cache key (🟡 todo)

`Caching/ContentPathCacheKey.cs` gains a space segment. Without it, two spaces holding a file at the same relative path collide in cache — a silent cross-space content leak, and the single most likely defect in this workstream.

### Step B5 — Make navigation space-aware (🟡 todo)

`Navigation/DynamicNavBuilder.cs` and `CachedDynamicNavBuilder.cs` take a space and build against that space's content source. `Navigation/FolderMetricsIndex.cs` is keyed per space. `Navigation/NavRules.cs` in the shared project is pure convention logic and stays unchanged — its numeric-prefix, date-prefix and title-casing rules already match this repository's folder naming.

### Step B6 — Make metrics opt-in and per-space (🟡 todo)

`NavStats`, `RepoStats` and `Coverage` are learning-hub-specific measures. Add a per-space boolean that defaults to off, and skip their computation when it is off. A repository documentation space should not be reporting article-coverage percentages.

`FolderMetricsIndex` is a singleton keyed by folder prefix only, and its snapshot is a single file at `Content:MetricsSnapshotPath`. Both must gain the space dimension: one index instance per space, and a snapshot path derived per space (`{configured-path}` for a single space, `{configured-stem}.{space-id}.json` for many). Without this, two spaces with metrics enabled overwrite each other's snapshot on every save, and the warm-up seeds each space from the other's counts.

The startup warm-up loop in `Program.cs` — seed, discover per root branch, drain, prune unreachable, save — runs **once per metrics-enabled space**, sequentially, so a cold multi-space start does not fan out into parallel whole-tree walks.

## 🔌 WS-C-endpoints — space-addressed surface (🟡 todo)

### Step C1 — Add `/_spaces` (🟡 todo)

New `Endpoints/SpaceEndpoints.cs` returning the registry projection consumed by the index page and the switcher: `id`, `routeBase`, `title`, `icon`, `repositoryUrl`, and the live values `articleCount`, `lastPublishedUtc` and `reachable`. The live values are what a statically generated landing page cannot carry, and are the reason this endpoint exists rather than a build-time file.

### Step C2 — Space-address the content and nav endpoints (🟡 todo)

`Endpoints/ContentEndpoints.cs` → `/_content-raw/{space}/{**key}`.

`Endpoints/NavEndpoints.cs` is a `MapGroup("/_nav")` carrying **five** routes. The space segment goes **between the group and each route**, never in place of them:

| Today | After |
|---|---|
| `GET /_nav/children?prefix=` | `GET /_nav/{space}/children?prefix=` |
| `GET /_nav/version` | `GET /_nav/{space}/version` |
| `GET /_nav/total` | `GET /_nav/{space}/total` |
| `GET /_nav/index` | `GET /_nav/{space}/index` |
| `POST /_nav/invalidate?path=` | unchanged — see `Step C3` |

A bare `/_nav/{space}` route MUST NOT be introduced: it is ambiguous with all five children above.

Per `D14-root-mount-when-single-space` the space segment is **optional when exactly one space is configured** — map both the prefixed and unprefixed forms and resolve the unprefixed form to the single space, so the client and any external caller keep working unchanged. With more than one space configured the unprefixed form is not mapped.

Both endpoint families resolve the space through `SpaceRegistry` and return `404` for an unknown id — never fall back to a default space, because a silent fallback turns a typo into wrong content. The existing `/_nav` group endpoint filter that translates a client abort into `499` stays in place and applies to the space-addressed routes unchanged.

### Step C3 — Extend invalidation and enforce its key (🟡 todo)

`/_nav/invalidate` keeps its path, its `POST` method and its existing `?path=` parameter, and accepts an optional `?space={id}`. Present → invalidate that space only. Absent → invalidate all spaces. Per `D6-invalidate-backward-compatible`, this keeps the existing learning-hub content workflow working with no edit to it.

Implement the `X-Invalidate-Key` check, which does not exist today — see the correction under `D6`. Bind the key from `Site:InvalidateApiKey`. When the configured key is empty the endpoint stays open, preserving current behaviour for local runs; when it is non-empty, compare with `CryptographicOperations.FixedTimeEquals` and return `401` on mismatch or absence. The caller already sends the header, so enabling enforcement does not require a workflow edit.

### Step C4 — Group hub subscriptions by space (🟡 todo)

`Navigation/NavHub.cs` and `NavChangePublisher.cs` group SignalR subscriptions by space id, so publishing a change to one space notifies only the clients viewing it. Add a `spaces` group that broadcasts registry changes, so a space added at runtime appears in every connected client's index and switcher without a reload.

### Step C5 — Route client requests through the space (🟡 todo)

In the client project, `HttpContentSource.cs` and `HttpNavProvider.cs` take the current space id from the route and call the space-addressed endpoints. `NavHubClient.cs` subscribes to the current space's group plus the `spaces` group.

The path-to-space-and-key resolution written in `WS-B` must be the **same code** on both sides, living in the shared project. If the server resolves `/learn/guide` to space `learn` + key `guide` and the client resolves it any other way, the prerendered article is replaced at hydration by a different page or a 404 — the exact failure `D16` rule 2 exists to prevent. `HttpContentSource` today fetches the **relative** URL `_content-raw/{contentKey}`, which resolves against `<base href="/" />` from any path depth; keeping the base at `/` is what lets that continue to work under a space prefix.

`Routes.razor` and `ContentPage.razor` live in the client project and today declare `@page "/"` and `@page "/{*path}"`. Per `D14-root-mount-when-single-space` both forms must remain routable: keep the existing catch-all and resolve the leading segment against `SpaceRegistry` at run time rather than declaring a second `@page "/{space}/{*path}"` template — two catch-all templates differing only by a leading segment do not disambiguate reliably. `<base href="/" />` in `App.razor` stays as it is; the WebAssembly `HttpClient` base address derives from it and all API calls are origin-absolute.

### Step C6 — Port the test content endpoints unchanged (🟡 todo)

`Endpoints/TestContentEndpoints.cs` maps `/_test/article` (POST, DELETE) and `/_nav/metrics` only when `Testing:ContentMutationEnabled` is true, which is never the case outside local runs, and it writes through the filesystem source.

Port it as-is with the names updated, bound to the **first `FileSystem` space** in the registry. Do not make it multi-space: it exists to exercise the metrics pipeline on a developer machine, and a configuration-gated dev-only write path does not justify a space dimension. If no `FileSystem` space is configured, do not map the endpoints.

## 🖼️ WS-D-space-index — generated index and switcher (🟡 todo)

Both surfaces render from `/_spaces`. Neither enumerates spaces in markup, so adding a space is a configuration change only. Per `D14-root-mount-when-single-space`, **both surfaces are suppressed entirely when exactly one space is configured**.

### Step D1 — Add the `SpaceIndex` page (🟡 todo)

A page in the shared project, served at `/` when more than one space is configured, rendering one card per space: icon, title, a documentation link to the space's route base, a repository link when `RepositoryUrl` is set, and the live counts from `/_spaces`. It iterates the registry — it must not contain a literal space name. Handle the loading, unreachable-space and empty-registry states explicitly.

### Step D2 — Make the index copy editable without a rebuild (🟡 todo)

Heading, introduction and footer come from an optional Markdown fragment resolved through the ordinary content pipeline; the card grid is generated. Absent fragment → fall back to `Site:Title` and no introduction. This keeps wording a content edit while keeping the space list generated.

### Step D3 — Add the switcher to `TopMenu` (🟡 todo)

`Layout/TopMenu.razor` gains a space switcher bound to the same `/_spaces` projection, marking the current space. `Layout/MainLayout.razor` reads the current space's title and icon for branding. Both update live from the `spaces` hub group. The switcher renders nothing when the registry holds one space — a control offering a single destination is noise, and `D7-single-space-is-degenerate` requires the learning hub's top bar to look exactly as it does today.

### Step D4 — Handle the root route (🟡 todo)

Root behaviour follows the space count, per `D14-root-mount-when-single-space`:

| Spaces | `/` | `/{first-segment}/…` |
|---|---|---|
| one | that space's root content, exactly as today | treated as a content path in that space |
| many | the generated index | resolved against `SpaceRegistry`; unknown id → the configured not-found page, never a default space |

The unknown-first-segment case matters only in the multi-space shape. In the single-space shape there is no first-segment reservation at all, so a content folder may be named anything without colliding with a space id.

## 🎨 WS-K-branding — configurable app-level look and feel (🟡 todo)

Per `D15-branding-is-per-deployment`, one deployment has one brand. Every step here is configuration- or content-driven: onboarding a publisher's identity MUST NOT require a code change or a rebuild.

### Step K1 — Add the `Site:Branding` section (🟡 todo)

Extend `SiteOptions` from `Step B1` with a `Branding` block:

```jsonc
{
  "Site": {
    "Title": "Diginsight Documentation",
    "Branding": {
      "ProductName": "Diginsight Documentation",
      "LogoPath": "_branding/logo.svg",
      "FaviconPath": "_branding/favicon.ico",
      "StylesheetPath": "_branding/theme.css",
      "Palette": {
        "Primary": "#0d6efd",
        "OnPrimary": "#ffffff",
        "Accent": "#0a58ca"
      }
    }
  }
}
```

Every field is optional. All absent → the application renders exactly as the learning hub does today, which is what `D7-single-space-is-degenerate` requires.

`LogoPath`, `FaviconPath` and `StylesheetPath` resolve through the **ordinary content pipeline** against the first configured space, so a publisher supplies its own assets by committing them alongside its documentation. No asset is compiled in and no deployment carries another publisher's marks.

### Step K2 — Emit the palette as CSS custom properties (🟡 todo)

`Palette` entries are written into the rendered `<head>` as CSS custom properties on `:root` (`--sd-primary`, `--sd-on-primary`, `--sd-accent`). Rewrite the fixed colours in `wwwroot/app.css` to reference those properties with the current learning-hub values as fallbacks, so an unconfigured palette is a visual no-op.

Emit them during **prerender**, not from client-side JavaScript — a palette applied after hydration produces a visible flash of the default theme on every cold navigation.

### Step K3 — Bind the shell to the branding (🟡 todo)

`Layout/MainLayout.razor` and `Layout/TopMenu.razor` render `ProductName` and the logo in the top bar in place of the hardcoded learning-hub mark. `App.razor` links `FaviconPath` and, when set, `StylesheetPath` after `app.css` so a publisher stylesheet overrides rather than replaces the base sheet.

In the multi-space shape the top bar shows the publisher brand plus the current space's `Title` and `Icon`; in the single-space shape it shows the brand alone, matching today's layout.

### Step K4 — Confirm the unbranded default is unchanged (🟡 todo)

Run the application with no `Branding` block and compare the top bar, palette and favicon against the current learning-hub site. Any visible difference means a hardcoded value was replaced with a non-equivalent default → fix the fallback rather than the configuration.

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

Port the existing Testmc content — logging levels, `Observability`, `OpenTelemetry`, `AzureKeyVault` — updating the logging category to the new assembly name, and replace the flat `Content` block with the **learning-hub** `Site` block from `Step B1`: exactly one space, `learn`, on the existing `learn` container, with no `RouteBase` and no `Branding`.

Per `D17-two-deployments-one-codebase` this file must **never** gain a second space. Adding one raises the space count to two, and by `D14-root-mount-when-single-space` that prefixes every learning-hub URL and replaces `/` with a space index — the exact regression `D7` forbids. The documentation deployment gets its own configuration file (`Step L2`) alongside its own host (`D18-docs-instance-host`).

Keep the informational `Deployment` block, naming the `learn` container.

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

The App Service and storage identifiers are variables rather than hardcoded literals so that the second deployment target in `Step L1` is a variable set rather than a forked workflow.

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
Site__MetricsSnapshotPath=D:\home\data\nav-metrics-snapshot.json
```

Then delete the settings left behind by the previous flat configuration: `Content__Source`, `Content__Blob__AccountUri`, `Content__Blob__ContainerName`, `Content__InvalidateApiKey` and `Content__MetricsSnapshotPath`. Per `D3-site-section` they no longer bind, and leaving them in place would make a future reader believe the site is single-space. The snapshot path moves to `Site__MetricsSnapshotPath` because `Step B6` derives the per-space file names from it.

Applying settings from the workflow rather than the portal is what keeps environment selection reproducible — the current action does the same, and this is the parity requirement.

### Step G5 — Deploy and confirm the target (🟡 todo)

Download the `smartdocs-web` artifact and deploy it with `azure/webapps-deploy@v3` to the Production slot.

Confirm from the run log that the deployment targeted `learn-testmc-app-itn-01` in resource group `learn-testmc-rg-itn-01`, that the worker platform reads 64-bit, and that the site answers on its hostname.

## 🏢 WS-L-docs-instance — deploy the documentation site (🟡 todo)

This workstream deploys the **second** instance of the same artifact. It adds no application code — everything it needs exists after `WS-B` through `WS-K`. What it adds is a second configuration, a second set of deployment variables, and a data-plane grant.

### Step L1 — Register the documentation deployment variables (🟡 todo)

A second set alongside the `SMARTDOCS_*` set in `Step G1`, following `D10-vars-for-configuration-secrets-for-secrets`:

| Name | Kind | Value |
|---|---|---|
| `DOCS_WEBAPP_NAME` | variable | `docs-testmc-app-itn-01` |
| `DOCS_RESOURCE_GROUP` | variable | `learn-testmc-rg-itn-01` |
| `DOCS_INVALIDATE_KEY` | secret | the documentation instance's invalidation key — **distinct** from the learning hub's |

The keys are separate because the deployments have separate publishers: a workflow that can flush the documentation cache has no business flushing the learning hub's.

### Step L2 — Add the documentation configuration to `tools.internal` (🟡 todo)

A second configuration file carrying the multi-space `Site` block from `Step B1` — `Title: "Diginsight Documentation"`, the `Branding` block from `WS-K-branding`, and one entry per repository space. It must never contain the `learn` space, per `D17-two-deployments-one-codebase`.

The publish-time fail-closed parse in `Step G2` applies to this file too: a documentation deployment carrying no space list would serve an empty index rather than an error.

### Step L3 — Extend `22.DeploySmartDocsWeb.yml` to deploy both instances (🟡 todo)

The build job produces **one** artifact and is not modified — that is the whole point of one codebase. The deploy job becomes a matrix over two targets, each carrying its own web-app name, resource group, configuration file and invalidation key. Steps `G3` and `G4` run per target.

One artifact deployed twice is what makes the two sites provably the same build. Two workflows producing two artifacts would let them drift, and the drift would show up as a behaviour difference nobody could attribute.

### Step L4 — Grant the documentation identity read access to the space containers (🟡 todo)

The instance carries the system-assigned managed identity created with it (`D18-docs-instance-host`). Assign **Storage Blob Data Reader**, scoped **per container** rather than to the account, so onboarding a space is an explicit grant rather than a blanket one already in force.

Reader, not Contributor: the renderer only ever reads. Content is written by `WS-H-content-publishing` under the workflow's own federated identity.

### Step L5 — Validate the documentation instance (🟡 todo)

Against the running site: the generated index at `/` lists every configured space; each space's articles resolve under its `RouteBase`; the branding from `WS-K-branding` is present on every space; and `Step I2`'s three prerender checks pass in this multi-space shape as well as in the single-space one.

The last item is not redundant with `Step I2`. `D16` rule 2 — the space rides in the path, never in DI — can only fail where there is more than one space to get wrong.

## 📤 WS-H-content-publishing — the `diginsight.tools` space (🟡 todo)

### Step H1 — Add `23.PublishDocsContent.yml` (🟡 todo)

New workflow publishing this repository's documentation into the `diginsight-tools` container. Triggers: push to `main` touching `src/docs/**`, plus `workflow_dispatch`. Concurrency group `publish-docs-content`.

Filling a container is independent of which deployment renders it. This workstream produces the content; the **documentation** deployment registers `diginsight.tools` as a space, and the learning-hub deployment never does — `D17-two-deployments-one-codebase`.

Stage `src/docs/**` — Markdown plus images — preserving repository-relative paths, excluding `bin`, `obj` and `node_modules`. Fail if nothing was staged, so an empty stage can never reach the container-reset path.

### Step H2 — Create the container and mirror the content (🟡 todo)

Log in with OIDC using `vars.AZURE_*` and `vars.SMARTDOCS_STORAGE_ACCOUNT` per `D10-vars-for-configuration-secrets-for-secrets`. Create the `diginsight-tools` container if absent, upload the staged content, and only **after a successful upload** prune blobs no longer present in the stage. Upload-then-prune makes a cleanup failure non-destructive: new content is live and stale blobs are removed on the next run.

### Step H3 — Invalidate the space (🟡 todo)

`POST /_nav/invalidate?space=diginsight.tools` with the `X-Invalidate-Key` header carrying `secrets.SMARTDOCS_INVALIDATE_KEY`. Best-effort with `continue-on-error: true` — a cache that refreshes on its own schedule is not a deployment failure.

The target is the **documentation** deployment — `docs-testmc-app-itn-01` per `D18-docs-instance-host`, never `learn-testmc-app-itn-01`. It uses `secrets.DOCS_INVALIDATE_KEY` from `Step L1`, not the learning hub's key.

### Step H4 — Confirm the learning-hub workflow still works (🟡 todo)

The learning-hub content workflow calls `/_nav/invalidate` with no space parameter. Confirm from the running site that this still invalidates successfully after `WS-C-endpoints`. This is the direct test of `D6-invalidate-backward-compatible`.

## 🧪 WS-I-validation — visible browser evidence (🟡 todo)

Mandatory for this change. Record the run as a validation-sequence Markdown with screenshots under this work item's `_validation/` folder, images in `_validation/images/`, front matter `publish: false`, following `testing-validation.instructions.md`.

### Step I0 — Capture the learning-hub baseline before anything changes (🟡 todo)

Against the **currently deployed** site, record the reference every later step is compared against: the top bar, the sidebar tree, a rendered article with its table of contents, the footer counts, and the exact URL of at least three articles at different depths.

Without this, "behaves exactly as today" is an opinion. This step is the reason it can be a measurement.

### Step I1 — Prove single-space compatibility (🟡 todo)

Run `Diginsight.SmartDocs.Web` configured with the **learning-hub space alone** and confirm, against the `Step I0` baseline:

- the three recorded article URLs resolve at the **same paths**, with no space prefix and no redirect (`D14-root-mount-when-single-space`)
- `/` serves the learning-hub root content, **not** a space index
- no space switcher is present in the top bar
- `GET /_nav/children?prefix=`, `/_nav/version`, `/_nav/total`, `/_nav/index` answer at their **unprefixed** paths
- the sidebar tree, table of contents, breadcrumb, prev/next, search overlay and footer counts match the baseline

Any difference here is a `D7` violation and blocks `WS-J-retirement`.

### Step I2 — Prove server-side rendering survived (🟡 todo)

This step **proves** `D16-prerender-parity-by-construction` held. It does not create the property — `WS-A` and `WS-B` do. A failure here means one of the five rules was violated upstream, and the table below identifies which.

Run all three checks in **both** deployment shapes — single-space and multi-space:

| Check | How | A failure points at |
|---|---|---|
| the first response is already rendered | plain HTTP client, no browser; body must contain the article heading and prose | rule 4 (assembly identity) or rule 1 (a service missing server-side) |
| the page is complete without JavaScript | visible browser with JavaScript disabled | same as above |
| hydration does not change what is shown | visible browser, JavaScript on, watch the article through load | rule 2 (space derived from ambient request state) or rule 3 (wrong space's content) |

The third check is the one that catches an asymmetric space resolution: the server renders the right article, the client then resolves the space differently and replaces it. Capture the no-JavaScript view, the raw response excerpt, and the hydration transition.

An accidental loss of prerendering is invisible in a normal browser, because the WebAssembly client fills the page in afterwards. Nothing else in `WS-I` detects it.

### Step I3 — Capture the space index (🟡 todo)

With both spaces configured, navigate to `/`. Capture: both space cards, their icons and titles, the documentation and repository links, and the live counts.

### Step I4 — Capture each space (🟡 todo)

Navigate to `/learn` and to `/diginsight.tools`. Capture for each: the navigation tree, a rendered article with a Mermaid diagram and a table of contents, and the branding showing the correct space title.

### Step I5 — Capture the switcher (🟡 todo)

Switch from one space to the other using the top-bar switcher. Capture before, during and after, confirming both the route base and the branding changed.

### Step I6 — Capture cross-space isolation (🟡 todo)

Request a path that exists in one space and not the other. Capture the not-found page. This is the direct test of the cache-key change in Step B4 — a leak here means Step B4 is incomplete.

### Step I7 — Capture branding applied and absent (🟡 todo)

With a `Site:Branding` block configured — logo, favicon, palette and product name — capture the top bar and an article page. Then remove the block, restart, and capture the same two views. The second pair MUST match the `Step I0` baseline (`Step K4`).

Confirm from the prerendered response that the palette custom properties are present in the first response, per `Step K2` — a palette that only appears after hydration flashes the default theme.

### Step I8 — Capture live invalidation (🟡 todo)

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

None. `OD1-docs-instance-host` closed on 2026-08-16 when `docs-testmc-app-itn-01` was provisioned; it is recorded as `D18-docs-instance-host`, and the workstream it was gating is now written as `WS-L-docs-instance`.

## 🅿️ Park lot

Out of scope for this plan. Not to be executed here.

- **`PL-1-quarto-retirement`** — `_quarto.yml`, `index.qmd`, `.quartoignore` and `quarto-publish.yml` become redundant once the `diginsight.tools` space serves `src/docs`. Retire only after the space is live and verified. → defer
- **`PL-2-ai-content-services`** — semantic search, summarisation and question answering over the spaces, as a separate service rather than inside the renderer. `src/20.00 Api/SmartDocsApi` is the natural home. → defer
- **`PL-3-scaffold-cleanup`** — `src/20.00 Api/SmartDocs` is a git-tracked, solution-absent `dotnet new webapi` scaffold. → defer
- **`PL-4-per-space-theme-override`** — letting an individual **space** override the deployment's logo or palette. App-level branding is now in scope as `WS-K-branding` per `D15-branding-is-per-deployment`; a per-space override is a separate feature needing its own justification. → defer
- **`PL-5-additional-spaces`** — onboarding further repositories. The model supports it with configuration only; each needs a container and a publishing workflow. → defer
- **`PL-6-dedicated-app-service`** — renaming or migrating the learning hub's App Service. → closed: `D17-two-deployments-one-codebase` makes the existing name correct — a host called `learn-testmc-app-itn-01` serving the learning hub is no longer a mismatch, and the documentation site got its own name in `D18-docs-instance-host`.
- **`PL-7-learning-hub-context-generalisation`** — `.copilot/context/90.00-learning-hub/` holds site-specific rules that partly apply to any space. → `01-autonomous-streams-artifacts.plan.md`
- **`PL-8-space-level-authorisation`** — per-space access control. Every space is currently public. → defer
- **`PL-9-seo-artifacts`** — `sitemap.xml`, `robots.txt`, canonical links, meta descriptions and Open Graph tags. The application's entire SEO surface today is one `<PageTitle>` in `ContentView.razor`. Prerendering is preserved and proven by `Step I2`, so nothing regresses; adding these is **new capability** and belongs to its own plan with its own goal. → defer
- **`PL-10-docs-plan-capacity`** — `samples-testmc-asp-01` is Basic B1: one instance, no deployment slots, now shared by two sites. Upgrading to Standard would buy slots and therefore zero-downtime swaps for both. Not needed while these are test deployments. → defer

## 🏁 Exit criteria

- The three projects exist under `src/50.00 Docs/` with the new names, and `Diginsight.Tools.sln` builds with **0 errors**. (🟡 todo)
- `packages.lock.json` is committed for all three projects. (🟡 todo)
- A single-space configuration serves the `Step I0` baseline article URLs at **identical paths**, with no space prefix, no redirect, no index page and no switcher, per `D7-single-space-is-degenerate` and `D14-root-mount-when-single-space`. (🟡 todo)
- An article's **first HTTP response** contains its rendered heading and prose with JavaScript disabled, per `Step I2`. (🟡 todo)
- The renamed application produced a rendered first response at `Step A5`, **before** any space work began. (🟡 todo)
- Hydration changes nothing visible: the article shown once the WebAssembly client takes over is identical to the prerendered one, in **both** deployment shapes. (🟡 todo)
- `IContentSource` and `IContentLister` are still registered as **singletons** on the server, and the host starts under Development with scope validation active, per `D16-prerender-parity-by-construction`. (🟡 todo)
- No space is resolved from `IHttpContextAccessor`: path-to-space resolution lives in the shared project and is called identically by both containers. (🟡 todo)
- The unprefixed `/_nav/*` and `/_content-raw/*` routes still answer in the single-space shape. (🟡 todo)
- The local filesystem-source run renders `src/docs` in a visible browser. (🟡 todo)
- `appsettings.Testmc.json` and the dispatch workflow exist in `tools.internal`. (🟡 todo)
- Every variable and secret in the `Step G1` table exists, and the assertion step passes. (🟡 todo)
- `22.DeploySmartDocsWeb.yml` deployed `Diginsight.SmartDocs.Web` to `learn-testmc-app-itn-01`, and the log confirms the target. (🟡 todo)
- `21.DeployAppService.yml` and `20.DeployTools.yml` are byte-identical to their state before this plan, and a FeedMonitor deployment still succeeds. (🟡 todo)
- The App Service worker platform reads 64-bit and the site starts — no `HTTP 500.32`. (🟡 todo)
- Obsolete `Content__*` application settings are removed. (🟡 todo)
- The learning-hub deployment serves exactly one space from storage, with no index page and no switcher. (🟡 todo)
- The documentation deployment renders every configured repository space from storage, and its generated index at `/` lists them all without any space name appearing in markup. (🟡 todo)
- No configuration file anywhere lists the learning-hub space alongside a repository documentation space, per `D17-two-deployments-one-codebase`. (🟡 todo)
- A configured `Site:Branding` block changes the logo, favicon, product name and palette with no rebuild, and removing it restores the `Step I0` baseline appearance exactly. (🟡 todo)
- The palette custom properties are present in the **prerendered** response, not applied after hydration. (🟡 todo)
- `/_nav/invalidate` returns `401` for a missing or wrong `X-Invalidate-Key` when a key is configured, and the learning-hub content workflow still invalidates successfully with no space parameter. (🟡 todo)
- A validation-sequence Markdown with screenshots exists under `_validation/`, covering the baseline, single-space compatibility, prerendering, the index, both spaces, the switcher, cross-space isolation, branding and live invalidation. (🟡 todo)
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
  revised: "2026-08-16"
  status: "actionable"
  gate_passed: true
  blocking_unknowns_resolved: 21
  open_decisions: 0
  discovery_items: 3
  sibling_plan: "01-autonomous-streams-artifacts.plan.md"
  revision_note: "Code-verified against Learn.01 on 2026-08-16. Added D14 route compatibility, D15 per-deployment branding, D16 prerender parity by construction, D17 two deployments one codebase and D18 docs instance host. D17 split the deployment sections: Step B1 shows two configuration files, Step F1 carries one space, WS-H is instance-scoped. D18 records docs-testmc-app-itn-01, provisioned 2026-08-16 on the shared samples-testmc-asp-01 B1 plan, which closed OD1 and let WS-L-docs-instance be written."
-->
