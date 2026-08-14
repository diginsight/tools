---
title: "Upgrading Diginsight.Tools from Diginsight 3.5 to 3.7"
author: "Dario Airoldi"
date: "2026-08-13"
categories: [diginsight, observability, migration, dotnet]
description: "Analysis and implementation of the Diginsight 3.5 to 3.7 upgrade across the tools repository, using samples.01 as the reference baseline."
---

# Upgrading Diginsight.Tools from Diginsight 3.5 to 3.7

**Date:** August 13, 2026
**Author:** Dario Airoldi
**Status:** Implemented — build green, runtime smoke-tested
**Component:** `Diginsight.Tools` (all projects)
**Target framework:** .NET 8 → .NET 10
**Reference repository:** `C:\dev\darioa\Diginsight\samples.01`

---

## 📋 Table of contents

1. [🎯 Scope and objective](#-scope-and-objective)
2. [🔍 Baseline: what the repository looked like](#-baseline-what-the-repository-looked-like)
3. [🔬 What changed between 3.5 and 3.7](#-what-changed-between-35-and-37)
4. [🧩 Comparison with the samples repository](#-comparison-with-the-samples-repository)
5. [🛠️ Implementation](#️-implementation)
6. [✅ Verification](#-verification)
7. [📌 Optional follow-ups](#-optional-follow-ups)
8. [🎓 Lessons learned](#-lessons-learned)
9. [📚 References](#-references)

---

## 🎯 Scope and objective

The `tools` repository was pinned to **Diginsight 3.5** while `samples.01` had already moved to **Diginsight 3.7**. The objective was to:

1. Understand how Diginsight telemetry, SmartCache and Components are consumed in this repository.
2. Determine the concrete API and dependency differences between 3.5 and 3.7.
3. Upgrade the repository in place, using `samples.01` as the canonical reference implementation.

The upgrade is **API-surface driven**, not a framework migration: the package bump alone works on `net8.0`. Moving to `net10.0` was a separate, optional step taken afterwards to close the gap with `samples.01`.

---

## 🔍 Baseline: what the repository looked like

### Diginsight consumption per project

| Project | Diginsight usage | Impacted |
|---|---|---|
| `01.00 Console/CosmosdbConsole` | `Diginsight.Core`, `Diginsight.Diagnostics`, `Diginsight.Diagnostics.OpenTelemetry` | ✅ Yes |
| `30.00 Job/Diginsight.Tools.FeedMonitor` | Core + Diagnostics + `Diginsight.Components`, `.Components.Azure`, `.Components.Configuration` | ✅ Yes |
| `20.00 Api/Smart*Api` (6 projects) | None — vanilla `WeatherForecast` templates | ❌ No |
| `MIPDocumentInspector/*` | Legacy `Common.Diagnostics` (`GetLogString()`, `GetCodeSection()`) — Diginsight v1/v2 era | ❌ Out of scope |

Two findings worth recording:

- **SmartCache is not used anywhere in this repository.** `DiginsightSmartcacheVersion` was declared in `Directory.Build.props`, but no project referenced any SmartCache package. The property was still bumped for consistency, but it is currently inert.
- **`src/Job/Diginsight.Tools.FeedMonitor/FeedMonitorBackgroundService.cs`** is an orphan copy that belongs to no project and is not compiled.

### Version pins before the upgrade

| Package family | Requested | Resolved |
|---|---|---|
| `Diginsight.Core` / `.Diagnostics` | `3.5.*` | `3.5.0.1` |
| `Diginsight.Components*` | `1.*` | `1.0.0.87` |

Baseline build result: `Build succeeded — 0 Error(s)`.

---

## 🔬 What changed between 3.5 and 3.7

The differences were established by scanning the exported type surface of both package versions, then confirmed by compilation and by reflecting over the shipped assemblies.

### Breaking API changes

| Diginsight 3.5 | Diginsight 3.7 | Nature |
|---|---|---|
| `IActivityLoggingSampler` | `IActivityLoggingFilter` | Interface renamed |
| `NameBasedActivityLoggingSampler` | `OptionsBasedActivityLoggingFilter` | Implementation renamed and re-based on options |
| `ObservabilityRegistry.RegisterLoggerFactory(factory)` | `LoggerFactoryStaticAccessor.LoggerFactory = factory` | Registry removed |
| `ObservabilityRegistry.RegisterComponent(callback)` | *(no replacement — read the accessor directly)* | Registry removed |
| `new DefaultCredentialProvider(environment)` | `new DefaultCredentialProvider(environment, logger)` | Constructor signature changed |

The `DefaultCredentialProvider` change was the least discoverable one — it lives in `Diginsight.Components.Configuration` and is not exercised anywhere in `samples.01`. Its current shape is:

```csharp
// Diginsight.Components.Configuration
public sealed class DefaultCredentialProvider : ICredentialProvider
{
    public DefaultCredentialProvider(IHostEnvironment environment, ILogger<DefaultCredentialProvider> logger);
    public DefaultCredentialProvider(IHostEnvironment environment, ILogger logger);

    public TokenCredential Get(IConfiguration configuration);
    public X509Certificate2 GetStoredCertificate(string thumbprint);
}
```

### The `ObservabilityRegistry` → `LoggerFactoryStaticAccessor` shift

This is the most consequential change conceptually. In 3.5, static classes registered a *callback* that the registry invoked once the logger factory became available — a push model:

```csharp
// 3.5 — push
static Observability()
{
    ObservabilityRegistry.RegisterComponent(factory => LoggerFactory = factory);
}
```

In 3.7, the logger factory is exposed through a static accessor that consumers read on demand — a pull model:

```csharp
// 3.7 — pull
public static ILoggerFactory? LoggerFactory => LoggerFactoryStaticAccessor.LoggerFactory;
```

The practical consequence is that <mark>`Observability.LoggerFactory` is now nullable</mark>. Every static call site must handle the "not yet initialised" case, which surfaced as `CS8604` warnings in `FeedMonitorBackgroundService`.

### Transitive dependency changes

Diginsight 3.7 is built against the .NET 10 BCL extension packages. Restoring 3.7 against the previous explicit pins produced `NU1605` downgrade errors:

```text
error NU1605: Detected package downgrade: Microsoft.Extensions.Configuration from 10.0.8 to 9.0.19
  Diginsight.Tools.FeedMonitor -> Diginsight.Core 3.7.1.13 -> Microsoft.Extensions.Configuration (>= 10.0.8)
error NU1605: Detected package downgrade: Scrutor from 7.0.0 to 6.1.0
  Diginsight.Tools.FeedMonitor -> Diginsight.Core 3.7.1.13 -> Scrutor (>= 7.0.0)
```

| Dependency | Before | After |
|---|---|---|
| `Microsoft.Extensions.*` | `9.*` | `10.*` (resolved `10.0.8`) |
| `Scrutor` | `6.*` | `7.*` (resolved `7.0.0`) |

**A .NET 10 dependency does not force a .NET 10 target framework.** `Microsoft.Extensions.*` 10.x and `Scrutor` 7.0.0 both still ship a `lib/net8.0` asset, and Diginsight 3.7 itself still ships `lib/net8.0`. The package upgrade was therefore validated on `net8.0` first, and the move to `net10.0` was applied afterwards as a deliberate, independent decision.

---

## 🧩 Comparison with the samples repository

`samples.01/src/03.01 CosmosDB/CosmosdbConsole` is a direct 1:1 counterpart of the tools console, which made it an exact migration oracle.

| Aspect | `tools.01` (after upgrade) | `samples.01` |
|---|---|---|
| `ObservabilityManager.cs` | byte-identical | byte-identical |
| `ObservabilityExtensions.cs` (FeedMonitor) | byte-identical | byte-identical |
| Diginsight versions | `3.7.1.13` / Components `1.0.0.104` | `3.7.1.13` / Components `1.0.0.103` |
| Target framework | `net10.0` | `net10.0` (console), `net8.0;net9.0;net10.0` (API) |
| `Diginsight.Stringify` reference | added | present |
| `System.Linq.Async` | `7.*` | `7.*` |
| `Directory.Build.props` extras | `Nullable enable`, OpenTelemetry version properties, SourceLink | `LangVersion 13`, `Nullable enable`, `SignAssembly`, SourceLink, OpenTelemetry version properties |

After the upgrade, the observability plumbing in both repositories is functionally equivalent. Two differences are deliberate and remain: `LangVersion 13` is not copied (on a uniform `net10.0` target the default is already C# 14, so pinning 13 would be a downgrade), and the packaging/signing properties do not apply because `tools` is an application repository rather than a package repository.

---

## 🛠️ Implementation

### Step 1 — Bump the central version pins (✅ done)

`src/Directory.Build.props`:

```xml
<DiginsightCoreVersion>3.7.*</DiginsightCoreVersion>
<DiginsightSmartcacheVersion>3.7.*</DiginsightSmartcacheVersion>
<DiginsightComponentsVersion>1.*</DiginsightComponentsVersion>
```

### Step 2 — Migrate `CosmosdbConsole` (✅ done)

- `Observability/Observability.cs` — replaced the registry callback with a `LoggerFactoryStaticAccessor` pass-through property.
- `Program.cs` — `Observability.LoggerFactory = …` → `LoggerFactoryStaticAccessor.LoggerFactory = observabilityManager.LoggerFactory;`, and `IActivityLoggingSampler`/`NameBasedActivityLoggingSampler` → `IActivityLoggingFilter`/`OptionsBasedActivityLoggingFilter`.
- `Observability/ObservabilityExtensions.cs` — same sampler-to-filter swap in `AddObservability`.
- `CosmosdbConsole.csproj` — added the conditional `Diginsight.Stringify` project/package reference pair, matching the samples layout.

### Step 3 — Migrate `Diginsight.Tools.FeedMonitor` (✅ done)

- `Observability/Observability.cs` — same accessor conversion.
- `Program.cs` — `ObservabilityRegistry.RegisterLoggerFactory(…)` → `LoggerFactoryStaticAccessor.LoggerFactory = …`.
- `FeedMonitorBackgroundService.cs` — passed the logger to both `DefaultCredentialProvider` constructions, and made the three static `Observability.LoggerFactory` call sites null-safe:

```csharp
var logger = Observability.LoggerFactory?.CreateLogger<FeedMonitorBackgroundService>()
    ?? NullLogger<FeedMonitorBackgroundService>.Instance;
```

- `Diginsight.Tools.FeedMonitor.csproj` — added `Diginsight.Stringify`, removed a duplicate `Microsoft.Extensions.DependencyInjection` entry (`NU1504`), bumped `Microsoft.Extensions.*` to `10.*` and `Scrutor` to `7.*`.

### Step 4 — Regenerate lock files (✅ done)

The repository uses `RestorePackagesWithLockFile=true`, so every restore during this migration required `--force-evaluate`:

```powershell
cd "c:\dev\darioa\Diginsight\tools.01\src"
dotnet restore "Diginsight.Tools.sln" --force-evaluate --nologo
dotnet build   "Diginsight.Tools.sln" --no-restore -v m --nologo
```

Note that `--force-evaluate` is a `restore`-only switch; passing it to `dotnet build` fails with `MSB1001`.

### Step 5 — Move to .NET 10 (✅ done)

Applied after the 3.7 upgrade was verified green on `net8.0`, so the two changes stay independently attributable.

- All 8 projects: `<TargetFramework>net8.0</TargetFramework>` → `net10.0`.
- `global.json`: moved from `src/` to the repository root, and the SDK pin changed from the wildcard `8.0.*` to the exact `10.0.100` with `rollForward: latestMajor`.
- `.github/workflows/20.DeployTools.yml`: `DOTNET_VERSION` `8.0.x` → `10.0.x`, and the publish step `--framework net8.0` → `net10.0`.

**The previous SDK pin never applied — for two independent reasons.** First, `sdk.version` does not support wildcards: a `global.json` containing `99.0.*` is silently ignored and `dotnet --version` still reports the newest installed SDK with exit code 0, whereas the exact `99.0.100` correctly fails. Second, the CLI resolves `global.json` from the **current working directory** upwards, not from the project directory — and CI runs `dotnet restore src/Diginsight.Tools.sln` from the repository root, so `src/global.json` was never even read. Both defects are fixed by the move plus the exact version.

The Azure App Service runtime stack for `diginsighttools-Testmc-job-itn-01` must be switched to .NET 10 before the next deployment — the workflow does not configure it.

### Step 6 — Align the build props and targets (✅ done)

`src/Directory.Build.props` gains `<Nullable>enable</Nullable>` and centralised OpenTelemetry version properties; `src/Directory.Build.targets` gains SourceLink and `ContinuousIntegrationBuild` under GitHub Actions. Both are guarded so the legacy `MIPDocumentInspector` projects — which target `net472`/`net5.0-windows` and live in a separate solution — keep their current behaviour:

```xml
<PropertyGroup Condition="!$(MSBuildProjectDirectory.Contains('MIPDocumentInspector'))">
  <Nullable>enable</Nullable>
</PropertyGroup>
```

`LangVersion` is deliberately **not** set. The samples repository pins it to 13 because it multi-targets `net8.0;net9.0;net10.0`; on a uniform `net10.0` target the default is already C# 14. `WarningAsErrors=nullable` is likewise not copied, because the repository carries pre-existing nullable-annotation debt that would turn into build errors.

### Step 7 — Package follow-ups (✅ done)

- `CosmosdbConsole`: `System.Linq.Async` `6.*` → `7.*` (resolved `7.0.1`).
- `Diginsight.Tools.FeedMonitor`: the four `OpenTelemetry.*` references now use `$(OpenTelemetryVersion)` instead of a literal `1.*`.

### Step 8 — Activate the FeedMonitor observability extension (✅ done)

`Program.cs` was calling the `Diginsight.Components.Configuration` overload, so the repository's own `ObservabilityExtensions.AddObservability` was dead code — and with it HTTP client instrumentation, dynamic log levels and the stringify contracts. It also called `AddAspNetCoreObservability` twice, overwriting `openTelemetryOptions` with a value nothing consumed. Replaced by the wiring used in `samples.01/IdentityAPI`:

```csharp
services.TryAddSingleton<IHttpContextFactory, DefaultHttpContextFactory>();
services.AddObservability(observabilityManager, configuration, hostEnvironment);
```

The `IHttpContextFactory` registration is retained because `AddAspNetCoreObservability` depends on it and this is a console host, not a web host.

### Step 9 — Repair `launchSettings.json` (✅ done)

The `CosmosdbConsole` profile contained a `//` comment, which `dotnet run` rejects. The commented example was promoted into a second, real profile, so the JSON is valid and the example survives.

---

## ✅ Verification

- Solution restores cleanly for all 7 projects; lock files regenerated to `Diginsight.Core 3.7.1.13` and `Diginsight.Components 1.0.0.104`, with `net10.0` recorded as the lock target. (✅ done)
- `dotnet build Diginsight.Tools.sln` → **`Build succeeded. 0 Error(s)`**, both at `net8.0` and after the `net10.0` move. (✅ done)
- No new warnings attributable to the upgrade; the remaining `CS8604`/`CS8618` warnings are pre-existing nullable-annotation debt in the feed parsers and `Executor`. (✅ done)
- The `NU1902` advisories cleared on their own: the floating `1.*` range now resolves `OpenTelemetry.Exporter.OpenTelemetryProtocol 1.17.0` instead of `1.13.1`. (✅ done)
- Runtime smoke test of `CosmosdbConsole` confirms the Diginsight console formatter, activity source and depth/duration columns still work, and that `launchSettings.json` now parses: (✅ done)

```text
Using launch settings from …\CosmosdbConsole\Properties\launchSettings.json...
2026-08-14T07:48:05.026 CosmosdbConsole.Program   DBUG 39c807ef699e3bd6…  1  Program.Main(args:string[…][…]) START
2026-08-14T07:48:07.906 CosmosdbConsole.Executor  DBUG 39c807ef699e3bd6…  1  Executor..ctor() START
2026-08-14T07:48:07.934 CosmosdbConsole.Executor  DBUG 39c807ef699e3bd6…  1  Executor..ctor END
2026-08-14T07:48:07.941 CosmosdbConsole.Program   DBUG 39c807ef699e3bd6…  1  Program.Main END
```

- Startup smoke test of `Diginsight.Tools.FeedMonitor` confirms Step 8 took effect — `OpenTelemetry.Instrumentation.Http.HttpClient` and `OpenTelemetry.Instrumentation.AspNetCore` now appear in the activity source detector output, which they did not before: (✅ done)

```text
New activity source detected: Diginsight.Tools.FeedMonitor
New activity source detected: Diginsight.Components.Configuration
New activity source detected: OpenTelemetry.Instrumentation.Http.HttpClient
New activity source detected: OpenTelemetry.Instrumentation.AspNetCore
```

- End-to-end FeedMonitor execution against live Azure resources. (🟡 todo)
- Azure App Service runtime stack switched to .NET 10 before the next deployment. (🟡 todo)

---

## 📌 Optional follow-ups

These were not required for the upgrade to be correct, but they close the remaining gap with `samples.01`. All were applied after the 3.7 upgrade had been verified green, so the two sets of changes remain independently attributable.

- Bump target frameworks `net8.0` → `net10.0` and align `src/global.json` and the deploy workflow. (✅ done — Step 5)
- Align `src/Directory.Build.props` and `Directory.Build.targets` with the samples version: `Nullable enable`, OpenTelemetry version properties, SourceLink. (✅ done — Step 6)
- Bump `System.Linq.Async` from `6.*` to `7.*` in `CosmosdbConsole`. (✅ done — Step 7)
- Resolve the `NU1902` moderate-severity advisories on `OpenTelemetry.Exporter.OpenTelemetryProtocol 1.13.1` in FeedMonitor. (✅ done — Step 7, resolved to `1.17.0`)
- Reconcile the dead local `ObservabilityExtensions.AddObservability(IServiceCollection, EarlyLoggingManager, IConfiguration, IHostEnvironment)` overload in FeedMonitor. Resolved by **calling** it rather than deleting it, matching `samples.01/IdentityAPI`. (✅ done — Step 8)
- Fix the invalid JSON comment in `CosmosdbConsole/Properties/launchSettings.json`. (✅ done — Step 9)
- Decide the fate of the orphan `src/Job/Diginsight.Tools.FeedMonitor/FeedMonitorBackgroundService.cs`. This is **not** the file under `src/30.00 Job/…`; it sits in a separate `src/Job/` tree with no `.csproj`, is **0 bytes**, and is git-tracked. Left untouched pending a decision. (🟡 todo)

---

## 🎓 Lessons learned

- **A shared reference implementation is the cheapest migration oracle.** Because `samples.01` contained a byte-identical counterpart of both observability bootstraps, most of the migration reduced to a diff rather than an investigation.
- **`NU1605` downgrade errors are a signal, not an obstacle.** They revealed that Diginsight 3.7 had moved to the .NET 10 extension packages — but checking the `lib/` folders of those packages showed `net8.0` assets were still present, which avoided an unnecessary framework migration.
- **Reflection beats guessing for undocumented signature changes.** String-scanning the assembly metadata for `DefaultCredentialProvider` was inconclusive; a throwaway console project that loaded the package and enumerated constructors gave the exact answer immediately.
- **Push-to-pull refactors leak nullability.** Converting `ObservabilityRegistry` callbacks into a static accessor turns a formerly non-null field into a nullable property, so every static logging call site needs a `NullLogger` fallback.
- **An SDK pin that never fires is worse than no pin.** `src/global.json` was inert twice over — wildcard versions are silently discarded, and the file was in the wrong directory relative to the CI working directory. Both failure modes are silent, so the repository appeared pinned for as long as the default SDK happened to be new enough.
- **Dead code hides behind overload resolution.** FeedMonitor's local `AddObservability` looked wired up, but an extension method with the same name from `Diginsight.Components.Configuration` was winning the overload match — so HTTP instrumentation and dynamic log levels had silently never been active. Nothing failed; the telemetry was simply thinner than intended.
- **Separate the compatibility fix from the modernisation.** Verifying the 3.7 upgrade on `net8.0` before touching target frameworks meant that when something broke afterwards, there was only one candidate cause.

---

## 📚 References

**[Diginsight documentation](https://diginsight.github.io/telemetry/)** 📘 [Official]
The official Diginsight site covering the telemetry model, activity-based logging, the console formatter, and the OpenTelemetry integration. Start here for the conceptual model behind `StartMethodActivity` and the depth/duration columns.

**[Diginsight telemetry repository](https://github.com/diginsight/telemetry)** 📘 [Official]
Source repository for `Diginsight.Core`, `Diginsight.Diagnostics` and related packages. Useful for confirming API surface changes between releases when the changelog is silent.

**[Diginsight samples repository](https://github.com/diginsight/samples)** 📘 [Official]
The reference implementations used as the migration oracle in this exercise; the local clone at `C:\dev\darioa\Diginsight\samples.01` was already on 3.7.

**[NuGet error NU1605](https://learn.microsoft.com/en-us/nuget/reference/errors-and-warnings/nu1605)** 📘 [Official]
Explains the package-downgrade detection that surfaced the .NET 10 extension-package requirement. Describes why an explicit lower pin loses to a transitive higher requirement.

**[Package lock files in NuGet](https://learn.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files#locking-dependencies)** 📘 [Official]
Documents `RestorePackagesWithLockFile` and the `--force-evaluate` switch required whenever intended versions change in a lock-file-enabled repository.

---

<!--
validations:
  grammar: {status: "not_run", last_run: null}
  readability: {status: "not_run", last_run: null}
  structure: {status: "not_run", last_run: null}
  references: {status: "not_run", last_run: null}

article_metadata:
  filename: "overview.md"
-->
