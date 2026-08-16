---
title: "FeedMonitor robustness fixes — correctness and security"
author: "Dario Airoldi"
date: "2026-08-15"
categories: [feedmonitor, robustness, security, cosmosdb, azure-storage]
description: "Actionable plan to remediate the 14 correctness and security findings in Diginsight.Tools.FeedMonitor, with unit tests and an Azurite end-to-end run."
status: actionable
---

# FeedMonitor robustness fixes — correctness and security

**Date:** August 15, 2026
**Author:** Dario Airoldi
**Status:** actionable — gate passed 2026-08-15, no items executed yet
**Component:** `src/30.00 Job/Diginsight.Tools.FeedMonitor`
**Source analysis:** 21 findings; this plan covers findings **1–14** (🔴 correctness + 🔴 security)
**Deferred:** findings 15–21 → § Park lot → `02-resilience-and-hygiene.md`

---

## 📋 Table of contents

1. [🎯 Goal and scope](#-goal-and-scope)
2. [🧭 Decisions taken](#-decisions-taken)
3. [⚠️ Pre-flight warnings](#️-pre-flight-warnings)
4. [⚙️ WS-A — shared key primitives](#️-ws-a--shared-key-primitives)
5. [⚙️ WS-B — parser item identity](#️-ws-b--parser-item-identity)
6. [⚙️ WS-C — Cosmos DB sink](#️-ws-c--cosmos-db-sink)
7. [⚙️ WS-D — Table Storage sink](#️-ws-d--table-storage-sink)
8. [⚙️ WS-E — Blob Storage sink](#️-ws-e--blob-storage-sink)
9. [⚙️ WS-F — run counters](#️-ws-f--run-counters)
10. [⚙️ WS-G — security hardening](#️-ws-g--security-hardening)
11. [🧪 WS-H — unit test project](#-ws-h--unit-test-project)
12. [🧪 WS-I — Azurite end-to-end validation](#-ws-i--azurite-end-to-end-validation)
13. [🔎 Discovery](#-discovery)
14. [🧭 Open decisions](#-open-decisions)
15. [🅿️ Park lot](#️-park-lot)
16. [✅ Exit criteria](#-exit-criteria)
17. [📚 References](#-references)

---

## 🎯 Goal and scope

Remediate the 14 correctness and security defects in `Diginsight.Tools.FeedMonitor` so that a scheduled WebJob run is **idempotent, injection-safe, and free of secret leakage**, and prove it with unit tests plus a local Azurite end-to-end run.

**In scope** — findings 1–14:

| # | Finding | Workstream |
|---|---|---|
| 1 | Atom items never receive a `Guid` | WS-B |
| 2 | Table `PartitionKey`/`RowKey` contain `/` | WS-D |
| 3 | `TransactionalBatch` exceeds the 100-op / 2 MB limits | WS-C |
| 4 | New Cosmos items get a random `id` | WS-C |
| 5 | Blob fallback path uses non-deterministic `GetHashCode()` | WS-E |
| 6 | `SanitizeForBlobPath` does not strip `/` | WS-E |
| 7 | `processedCount` is a data race and is overwritten | WS-F |
| 8 | Channel metadata never refreshed after first discovery | WS-C |
| 9 | `feedChannel.PartitionKey = "/"` for every channel | WS-C |
| 10 | Storage secrets serialized into logs and traces | WS-G |
| 11 | OData filter injection from feed-controlled content | WS-G |
| 12 | `TypeNameHandling.Auto` in the Cosmos serializer | WS-G |
| 13 | Unbounded, untimed, uncancelled feed download | WS-G |
| 14 | Full blob content written to the activity payload | WS-G |

**Out of scope** — findings 15–21 (resilience, dead config, doc drift, duplicated code). Each carries a disposition in § Park lot.

**Explicit non-goal:** the Table Storage sink is **retained**. It exists as reference sample code and MUST keep working; it is fixed, not removed.

---

## 🧭 Decisions taken

Closed before authoring — no step below depends on an unresolved choice.

| Id | Decision | Basis |
|---|---|---|
| `D1-scope-tiers` | Cover findings 1–14 only; park 15–21 | User answer, 2026-08-15 |
| `D2-green-field` | Stored key/id schemes may change in a breaking way; no migration or dual-read required | User answer, 2026-08-15 — containers hold nothing to preserve |
| `D3-test-surface` | Add an xUnit project **and** run end-to-end against Azurite | User answer, 2026-08-15 |
| `D4-table-retained` | Table sink stays; keys are sanitized rather than the sink dropped | User instruction, 2026-08-15 |
| `D5-redaction-api` | Redact via `Diginsight.Stringify.NonStringifiableMemberAttribute` | Confirmed present in `Diginsight.Stringify.dll` (net10 lib) alongside `NonStringifiableObjectAttribute` |
| `D6-cosmos-id-source` | Cosmos `id` derives from the item `Guid` through `CosmosDbHelper.SanitizeCosmosDbId` | Helper confirmed in `Diginsight.Components.Azure.dll`, already used at `FeedMonitorBackgroundService.cs:190` |
| `D7-channel-concurrency` | Channel upsert stays last-write-wins; no ETag/optimistic concurrency | Two config feeds sharing a `<link>` produce the same channel id, but all writers derive identical content from the same source — divergence is not possible under `D2-green-field` |

---

## ⚠️ Pre-flight warnings

Read before touching any file.

- **Edit the right file.** `src/Job/Diginsight.Tools.FeedMonitor/FeedMonitorBackgroundService.cs` (no `30.00 ` prefix) is a **0-byte git-tracked orphan** belonging to no project. The real file is `src/30.00 Job/Diginsight.Tools.FeedMonitor/FeedMonitorBackgroundService.cs`.
- **Lock files.** `Directory.Build.props` sets `RestorePackagesWithLockFile=true`. Every restore after a `.csproj` change needs `--force-evaluate`, and `--force-evaluate` is restore-only (it fails `dotnet build` with MSB1001).
- **Build baseline.** Current `Diginsight.Tools.sln` builds with **0 errors and ~166 pre-existing nullable warnings (CS8604/CS8618)**. The bar for this plan is 0 errors and **no net-new warnings**.
- **Out-of-solution project.** `src/20.00 Api/SmartDocs/` is on disk but not in the solution; restore it separately if any shared props change.
- **Line numbers** cited below are as of commit on `main`, 2026-08-15. Re-anchor by symbol name if they have drifted.

---

## ⚙️ WS-A — shared key primitives (🟡 todo)

Everything downstream depends on these, so they land first.

### Step A1 — Create `Helper/FeedKeys.cs` (🟡 todo)

Add a new `internal static class FeedKeys` in `src/30.00 Job/Diginsight.Tools.FeedMonitor/Helper/FeedKeys.cs` exposing exactly four members:

1. `static string Sha256Hex(string value, int length)` — UTF-8 SHA-256, lowercase hex, truncated to `length`. Deterministic across processes. Replaces every use of `string.GetHashCode()`.
2. `static string ToCosmosId(string guid)` — returns `CosmosDbHelper.SanitizeCosmosDbId(guid)` when the result is ≤ 200 chars; otherwise the first 191 chars + `-` + `Sha256Hex(guid, 8)`. Never returns empty (empty input → `ArgumentException`).
3. `static string ToStorageKey(string value)` — Azure Table key sanitizer. Replaces each of `/`, `\`, `#`, `?` and every char in `U+0000–U+001F` / `U+007F–U+009F` with `-`; collapses runs of `-`; trims leading/trailing `-` and spaces. If the result exceeds 900 chars, truncates to 891 and appends `-` + `Sha256Hex(value, 8)`. Empty result → `"unknown"`.
4. `static string ToBlobPathSegment(string value)` — as today's `SanitizeForBlobPath` **plus** `/` in the invalid-character set, and with `.` collapsed so that no segment is `.` or `..`. Empty result → `"untitled"`.

Acceptance: the class has no dependency on `FeedMonitorBackgroundService`; all four members are pure and side-effect free.

### Step A2 — Delete the superseded private helpers (🟡 todo)

Remove `SanitizeForBlobPath` (`FeedMonitorBackgroundService.cs:732`) and redirect its two call sites (`:560`, `:585`) to `FeedKeys.ToBlobPathSegment`. Leave `ExtractUniqueIdentifier` in place for now — WS-E rewrites its body.

---

## ⚙️ WS-B — parser item identity (🟡 todo)

Fixes finding **1**.

### Step B1 — Populate `Guid` in `ParseAtomEntry` (🟡 todo)

In `src/30.00 Job/Diginsight.Tools.FeedMonitor/Helper/AtomFeedParser.cs`, inside `ParseAtomEntry` (~`:106`), set `Guid` using this resolution order, first non-empty wins:

1. `entry.Element(ns + "id")?.Value`
2. the `alternate` link `Href` (already computed further down — hoist the assignment)
3. `FeedKeys.Sha256Hex($"{title}|{published:o}", 32)`

### Step B2 — Make the RSS fallback explicit (🟡 todo)

In `Helper/RSSFeedParser.cs:118`, `Guid` currently takes `<guid>` verbatim and may be `null`. Apply the same three-step ladder: `<guid>` → `<link>` → `Sha256Hex(title|pubDate, 32)`.

### Step B3 — Change `FeedItemBase.Guid` to non-nullable and guard (🟡 todo)

`Models/Base/FeedItemBase.cs` declares `public string Guid { get; set; }` under `<Nullable>enable</Nullable>` with no initializer. Initialize to `string.Empty`, and in `ReadAllFeedsAsync` (`FeedMonitorBackgroundService.cs:~197`, right after the null-channel guard) drop items with an empty `Guid`:

```csharp
var skipped = feedChannel.Items.RemoveAll(i => string.IsNullOrWhiteSpace(i.Guid));
if (skipped > 0) { logger.LogWarning("{FeedUri}: dropped {Skipped} item(s) with no resolvable Guid", feedUri, skipped); }
```

Acceptance: for every feed in `appsettings.HierarchicalFeeds.Example.json`, 100 % of parsed items carry a non-empty `Guid`, asserted by WS-H tests.

---

## ⚙️ WS-C — Cosmos DB sink (🟡 todo)

Fixes findings **3, 4, 8, 9**.

### Step C1 — Channel partition key from the feed host (🟡 todo)

`FeedMonitorBackgroundService.cs:185–186` computes `feedDomain` and then discards it, assigning `feedChannel.PartitionKey = "/"`. Replace with `feedChannel.PartitionKey = feedDomain;`.

### Step C2 — Refresh channel metadata on upsert (🟡 todo)

Rewrite `UpsertFeedChannel2CosmosDBAsync` (`:243`). Current behaviour patches only `LastSeen`/`LastUpdated`/`DateModified` on a raw `JObject`, freezing `Title`, `Description`, `CategoryPath` and `ImageUrl` at first discovery.

New behaviour — single path, no read/patch split:

1. Read the existing document (`ReadItemObservableAsync<JObject>`), catching `CosmosException` with `StatusCode == NotFound`.
2. If found: copy `FirstDiscovered` and `DateCreated` from the stored document onto the freshly parsed `feedChannel`.
3. If not found: set `FirstDiscovered = utcNow`, `DateCreated = utcNow`.
4. Always: `LastSeen = utcNow`, `DateModified = utcNow`.
5. Always: `UpsertItemAsync(feedChannel, partitionKey, ...)` — the freshly parsed, strongly typed entity.

Per `D7-channel-concurrency`, no ETag is applied.

### Step C3 — Deterministic Cosmos item id (🟡 todo)

`EntityBase`'s constructor assigns `Id = Guid.NewGuid().ToString()`, so every "new" item is an unconditional insert. In `UpsertFeedItems2CosmosDBAsync` (`:289`), inside the existing per-item loop (`:~300`), set:

```csharp
item.Id = FeedKeys.ToCosmosId(item.Guid);
```

### Step C4 — Stabilise the item partition key (🟡 todo)

Same loop. The partition key is `{link}-{year}` where `year` comes from `PublicationDate`. If a feed later starts emitting a `pubDate` for an item that previously had none, the item re-buckets into a different partition and duplicates despite C3. Fix the date source to a single stable expression and document it:

```csharp
// Partition bucket is fixed by the item's own publication date; LastUpdated is deliberately not used
// as a fallback because it changes over the item's lifetime and would re-bucket the item.
var itemDate = item.PublicationDate;
```

That is already the current expression — the step is to **add the comment and an assertion test**, so a future edit does not introduce a `?? LastUpdated` fallback. No behaviour change.

### Step C5 — Chunk and verify the transactional batch (🟡 todo)

`:411` puts every item of a partition into one `TransactionalBatch` and never inspects the response. Replace with:

1. **Deduplicate** `feedItemsToUpsert` by `Id` (keep last) — Cosmos rejects two operations on the same id inside one batch.
2. **Chunk** at **90 operations** per batch.
3. **Await and check** each `TransactionalBatchResponse`: if `!IsSuccessStatusCode`, log the batch status plus the index and status of the first failing operation, then **fall back to per-item `UpsertItemAsync`** for that chunk so one oversized or conflicting item cannot lose the other 89.
4. Increment the counter only for operations that actually succeeded.

Acceptance: a 250-item partition produces 3 batches (90/90/70); a chunk that returns `RequestEntityTooLarge` is retried item-by-item and the surviving items are persisted.

---

## ⚙️ WS-D — Table Storage sink (🟡 todo)

Fixes finding **2**. The sink is retained per `D4-table-retained`.

### Step D1 — Sanitize both table keys (🟡 todo)

Azure Table Storage forbids `/`, `\`, `#`, `?` and control characters in `PartitionKey` and `RowKey`. Today `UpdateFeedItems2TableStorage` uses `{link}-{year}` (contains `//` from the scheme) as the partition key and the raw item `Guid` (a URL) as the row key — every write is rejected.

In `FeedMonitorBackgroundService.cs:525`, change the entity construction to:

```csharp
var tableEntity = new TableEntity(FeedKeys.ToStorageKey(partitionKeyValue), FeedKeys.ToStorageKey(newItem.Guid))
```

### Step D2 — Sanitize the keys used in the existence queries (🟡 todo)

The lookup at `:483` and `:497` must filter on the **same sanitized** partition key, otherwise dedup silently never matches and every run re-writes every row. Sanitize `partitionKeyValue` once before building either filter, and compare the sanitized row keys against `FeedKeys.ToStorageKey(item.Guid)` rather than the raw `Guid` (`:486`, `:~478`).

### Step D3 — Cap oversized string properties (🟡 todo)

Azure Tables rejects string properties above 64 KiB and entities above 1 MiB; `Description` regularly exceeds this. Before adding `Description` to the entity, truncate to 32 000 characters and append `"…[truncated]"` when truncation occurs. Apply the same cap to `Title` and `Categories`.

Acceptance: ingesting `https://devblogs.microsoft.com/dotnet/feed/` against Azurite writes rows with no 400 response, and a second run writes 0 new rows.

---

## ⚙️ WS-E — Blob Storage sink (🟡 todo)

Fixes findings **5, 6**.

### Step E1 — Deterministic fallback path (🟡 todo)

`FeedMonitorBackgroundService.cs:600` uses `item.Guid.GetHashCode().ToString("X8")`. .NET Core randomizes string hashing per process, so the long-path fallback produces a different folder on every run. Replace with `FeedKeys.Sha256Hex(item.Guid, 8)`.

### Step E2 — Route all path building through `FeedKeys.ToBlobPathSegment` (🟡 todo)

Three call sites currently build path segments from feed-controlled text:

- `:560` — `sanitizedDomain`
- `:585` — `sanitizedTitle`
- `:~713` — inside `ExtractUniqueIdentifier`

Point all three at `FeedKeys.ToBlobPathSegment`, which (unlike today's helper) removes `/` and neutralizes `.`/`..`. The `feeds/{domain}/{year}/{month}/` prefix keeps its literal separators because it is assembled from constants and already-sanitized segments.

Acceptance: an item whose title is `a/../../b` yields exactly one folder under `feeds/{domain}/{yyyy}/{MM}/`, asserted by a WS-H test.

---

## ⚙️ WS-F — run counters (🟡 todo)

Fixes finding **7**.

### Step F1 — Replace the shared mutable counter (🟡 todo)

`processedCount` is declared once at `FeedMonitorBackgroundService.cs:137`, written non-atomically from 8 parallel feed iterations, and reassigned in sequence by the Cosmos, Table and Blob branches (`:~208`, `:~215`, `:~220`) so only the last sink's value survives.

Replace with three `int` locals in `ReadAllFeedsAsync` accumulated via `Interlocked.Add`, one per sink, and log them separately.

### Step F2 — Fix the completion log message (🟡 todo)

`:239` reads `"Completed one-time plant license activation processing. Processed count: {ProcessedCount}"` — a copy-paste leftover from an unrelated job. Replace with a message naming the three counters and the feed count.

---

## ⚙️ WS-G — security hardening (🟡 todo)

Fixes findings **10, 11, 12, 13, 14**.

### Step G1 — Redact secrets from telemetry payloads (🟡 todo)

Six activity payloads stringify a whole client-configuration object into console, log4net **and** Azure Monitor: `FeedMonitorBackgroundService.cs:743`, `:761`, `:873`, `:969`, `:1006`, `:1049`.

Apply `[NonStringifiableMember]` (`Diginsight.Stringify`) to exactly these properties:

| File | Properties |
|---|---|
| `Configuration/CosmosDBClientConfiguration.cs` | `ConnectionString`, `AuthKey` |
| `Configuration/BlobClientConfiguration.cs` | `ConnectionString`, `SasToken`, `AccountKey` |
| `Configuration/TableClientConfiguration.cs` | `ConnectionString`, `SasToken`, `AccountKey` |
| `Configuration/FileStorageClientConfiguration.cs` | `ConnectionString` |
| `Configuration/QueueStorageClientConfiguration.cs` | `ConnectionString` |
| `Configuration/FeedMonitorConfiguration.cs` | `ClientSecret` |

Negative branch is defined in § Discovery `DSC1-nested-redaction`.

### Step G2 — Stop logging blob content (🟡 todo)

`:677` places the entire `content` string in the `UploadBlobAsync` activity payload — every article body into telemetry. Change the payload to `() => new { blobPath, contentType, contentLength = content.Length }`.

### Step G3 — Parameterize the Table Storage filters (🟡 todo)

`:483` and `:497` interpolate `feedChannel.Id` — derived from the feed's own `<link>` element, i.e. third-party content — straight into an OData filter with no quote escaping. A `<link>` containing `x' or FeedId ne '` rewrites the query.

Replace both string interpolations with `TableClient.CreateQueryFilter($"...")`, the `FormattableString` overload, which escapes parameters. Keep the `select: new[] { "RowKey" }` projection on the first query and **add** the same projection to the second (`:497`), which currently pulls whole entities.

### Step G4 — Disable polymorphic type resolution in the serializer (🟡 todo)

`Helper/NewtonsoftJsonCosmosSerializer.cs:27` sets `TypeNameHandling = TypeNameHandling.Auto`. `FeedItemBaseConverter` and `FeedChannelBaseConverter` already resolve subtypes from the `Type` discriminator, so the setting is redundant and is a standing insecure-deserialization risk.

Set `TypeNameHandling = TypeNameHandling.None`.

Acceptance: a round-trip of one `RSSFeedItem` and one `AtomFeedItem` still deserializes to the correct concrete type, and the serialized JSON contains no `$type` property.

### Step G5 — Harden the feed download (🟡 todo)

`:162–165` constructs `new HttpClient()` per feed inside a parallelized loop and calls `GetStringAsync(feedUri)` with no cancellation token, no timeout, and no response-size cap — while `Program.cs` already registers `AddHttpClient()` and never uses it.

1. In `Program.cs`, register a named client `"feeds"`: `Timeout = TimeSpan.FromSeconds(30)`, default `User-Agent` header moved here from the call site, and a primary handler with `AutomaticDecompression = GZip | Deflate | Brotli`.
2. Inject `IHttpClientFactory` into `FeedMonitorBackgroundService`.
3. Reject any `feedConfig.Uri` whose scheme is not `http`/`https` before the request, logging a warning and skipping the feed.
4. Replace the download with `SendAsync(..., HttpCompletionOption.ResponseHeadersRead, cancellationToken)`, `EnsureSuccessStatusCode()`, then read the body through a **16 MiB cap** — abort and skip the feed with a warning if the cap is exceeded.

Acceptance: the existing `catch` block still swallows per-feed failures and continues to the next feed; a feed returning 404 logs a warning and does not abort the run.

---

## 🧪 WS-H — unit test project (🟡 todo)

### Step H1 — Create the project (🟡 todo)

Create `src/60.00 Test/Diginsight.Tools.FeedMonitor.Tests/Diginsight.Tools.FeedMonitor.Tests.csproj`:

- `TargetFramework` `net10.0`, `Nullable` enable, `IsPackable` false
- `PackageReference`: `Microsoft.NET.Test.Sdk` `18.*`, `xunit` `2.*`, `xunit.runner.visualstudio` `3.*`
- `ProjectReference` to `..\..\30.00 Job\Diginsight.Tools.FeedMonitor\Diginsight.Tools.FeedMonitor.csproj`
- Add `InternalsVisibleTo` to the FeedMonitor project so `FeedKeys` (internal) is testable

The folder name follows the existing `NN.NN Name` convention (`01.00 Console`, `20.00 Api`, `30.00 Job`, `40.00 Service`, `50.00 Docs`).

### Step H2 — Register in the solution (🟡 todo)

Add a `60.00 Test` solution folder to `src/Diginsight.Tools.sln` and nest the new project under it, mirroring how `30.00 Job` nests `Diginsight.Tools.FeedMonitor`.

### Step H3 — Add fixture feeds (🟡 todo)

Add three `Content` files under `Fixtures/`, copied to output: `rss-with-guid.xml`, `rss-without-guid.xml`, `atom.xml`. Capture them once from the feeds listed in `appsettings.HierarchicalFeeds.Example.json` and commit them so the tests stay offline and deterministic.

### Step H4 — Write the tests (🟡 todo)

One test per finding, each named for the finding it locks down:

| Test | Locks |
|---|---|
| `AtomParser_AssignsGuidToEveryEntry` | #1 |
| `RssParser_FallsBackToLinkWhenGuidMissing` | #1 |
| `ToStorageKey_RemovesForbiddenCharsAndIsStable` | #2 |
| `ToStorageKey_TruncatesAboveNineHundredCharsWithHashSuffix` | #2 |
| `BatchChunking_SplitsTwoFiftyItemsIntoNinetyNinetySeventy` | #3 |
| `ToCosmosId_IsDeterministicAndWithinLimits` | #4 |
| `Sha256Hex_ReturnsKnownConstantForKnownInput` | #5 |
| `ToBlobPathSegment_NeutralizesSlashAndDotDot` | #6 |
| `CreateQueryFilter_EscapesEmbeddedSingleQuote` | #11 |
| `Serializer_RoundTripsSubtypesWithoutTypeProperty` | #12 |
| `ClientConfigurations_DoNotStringifySecrets` | #10 |

`Sha256Hex_ReturnsKnownConstantForKnownInput` asserts against a hard-coded expected hash — that is what makes a regression to `GetHashCode()` fail the build rather than silently pass.

### Step H5 — Restore, build, run (🟡 todo)

```powershell
cd "c:\dev\darioa\Diginsight\tools.01\src"
dotnet restore "Diginsight.Tools.sln" --force-evaluate --nologo
dotnet build   "Diginsight.Tools.sln" --no-restore -v m --nologo
dotnet test    "60.00 Test\Diginsight.Tools.FeedMonitor.Tests\Diginsight.Tools.FeedMonitor.Tests.csproj" --no-build --nologo
```

Acceptance: 0 build errors, no net-new warnings above the ~166 baseline, all tests green.

---

## 🧪 WS-I — Azurite end-to-end validation (🟡 todo)

### Step I1 — Start Azurite (🟡 todo)

Start blob + table emulation in a **visible foreground console** the operator can stop with Ctrl+C:

```powershell
azurite --silent --location "$env:TEMP\azurite-feedmonitor" --blobPort 10000 --tablePort 10002
```

Negative branch: see § Discovery `DSC2-azurite-availability`.

### Step I2 — Configure the local run (🟡 todo)

In `appsettings.local.json` (developer-local, never committed with real secrets): set `FeedMonitor:BlobStorage:ConnectionString` and `FeedMonitor:TableStorage:ConnectionString` to `UseDevelopmentStorage=true`, set `FeedMonitor:CosmosDB:Enabled` to `false`, and populate `FeedMonitor:Feeds` with the four entries from `appsettings.HierarchicalFeeds.Example.json` (two RSS, one Atom, one hierarchical child).

### Step I3 — Bootstrap the container and table (🟡 todo)

The service never calls `CreateIfNotExists`, so a fresh emulator has no `feeds` container or table. Create both once with Azure Storage Explorer or `az storage`, and record the commands used in the validation note. *(Automatic bootstrap is finding 17 and is parked — this step is the manual stand-in for this plan only.)*

### Step I4 — Run twice and record evidence (🟡 todo)

Run the WebJob in a **visible foreground console**, twice in a row, and capture:

1. **Run 1** — Table rows created (> 0), blob folders created under `feeds/{domain}/{yyyy}/{MM}/`, Atom feed items present with non-empty `RowKey`.
2. **Run 2 (idempotency)** — 0 new Table rows, 0 new blob folders, counters report 0 processed.
3. **Secret scan** — `Select-String` the full console transcript for `AccountKey`, `SasToken`, `AuthKey`, `ConnectionString` values; expect **zero** matches of actual secret values.
4. **Content scan** — confirm no article body appears in the transcript (locks #14).

Record the outcome as a validation note beside this plan, at `_validation/validation-sequence.md` in this issue folder, with `publish: false` in its frontmatter.

---

## 🔎 Discovery

Items undecidable until execution; each carries a defined negative branch.

- **`DSC1-nested-redaction`** — whether `[NonStringifiableMember]` is honoured when the decorated type is reached *indirectly*, as a member of the anonymous object passed to `StartMethodActivity`. Resolved by running `ClientConfigurations_DoNotStringifySecrets` (Step H4). **If the attribute is not honoured for nested members:** replace each of the six payloads at `:743`, `:761`, `:873`, `:969`, `:1006`, `:1049` with an explicit projection of non-secret fields only — for example `() => new { cfg.Enabled, cfg.Database, cfg.Collection, cfg.ConnectionMode }` — and keep the attributes in place as defence in depth. Gates: Step G1.
- **`DSC2-azurite-availability`** — whether Azurite is installed on the machine. **If absent:** install with `npm install -g azurite`. **If that also fails** (no Node.js, restricted machine): run WS-I against the Test-environment storage account using `appsettings.Test.local.json`, and note the substitution in the validation record. Gates: Steps I1–I4.

---

## 🧭 Open decisions

None. All decisions were closed before this body was authored — see § Decisions taken.

---

## 🅿️ Park lot

Findings surfaced by the analysis but excluded by `D1-scope-tiers`. None of these may be executed as part of this plan.

| Id | Item | Disposition |
|---|---|---|
| `PL-1-feed-abort` | #15 — a rethrow in `UpsertFeedChannel2CosmosDBAsync` escapes to the outer `catch` and silently ends processing of all remaining feeds | → `02-resilience-and-hygiene.md` |
| `PL-2-dynamic-enable-nre` | #16 — clients are built in the constructor but `Enabled` is re-read at runtime; toggling a sink on after startup throws `NullReferenceException` | → `02-resilience-and-hygiene.md` |
| `PL-3-storage-bootstrap` | #17 — no `CreateIfNotExistsAsync` for the blob container or table; Step I3 is the manual stand-in | → `02-resilience-and-hygiene.md` |
| `PL-4-conditional-get` | #18 — no `ETag`/`If-Modified-Since`, so every feed is fully re-downloaded every 10 minutes | → `02-resilience-and-hygiene.md` |
| `PL-5-minpubdate-widening` | #19 — one item with a bogus old date expands the partition scan to ~56 cross-partition queries per feed | → `02-resilience-and-hygiene.md` |
| `PL-6-blob-toctou` | #20 — `ExistsAsync` then three separate uploads with `metadata.json` written last; a crash leaves orphaned `content.md` | → `02-resilience-and-hygiene.md` |
| `PL-7-dead-config` | #21 — `LoadExistingFeeds`, `RunsSchedulingInMinutes`, `TimeToLive`, `ParentFeedUri`, `TenantId`/`ClientId`/`ClientSecret`, `allMetricValues`, the unused `credentialProvider` at `:144`, and the unused file/queue options monitors | → `02-resilience-and-hygiene.md` |
| `PL-8-doc-drift` | `README_HierarchicalFeeds.md` documents `FeedSources`, `FeedSourceDetails`, `FeedSourceMetadata` and `IsPrimarySource`, none of which exist; `README.md` is empty | → `02-resilience-and-hygiene.md` |
| `PL-9-sink-duplication` | ~150 duplicated lines of partition/min-date/year logic between `UpsertFeedItems2CosmosDBAsync` and `UpdateFeedItems2TableStorage` | → defer — revisit after WS-A lands, since `FeedKeys` removes part of the duplication |
| `PL-10-hierarchy-unused` | `ParentFeedUri` is configured but never read, so the advertised feed *hierarchy* is not modelled at all | → defer — belongs to the feature plan, not a robustness fix |

---

## ✅ Exit criteria

All must hold before this plan moves to `done`.

- Every step in WS-A through WS-I carries `(✅ done)`. (🟡 todo)
- `dotnet build src/Diginsight.Tools.sln` reports 0 errors and no net-new warnings above the ~166 baseline. (🟡 todo)
- All 11 tests from Step H4 pass. (🟡 todo)
- The Azurite run is idempotent: run 2 writes 0 new Table rows and 0 new blob folders. (🟡 todo)
- Atom items are persisted with non-empty keys in all three sinks. (🟡 todo)
- The console transcript from Step I4 contains no secret values and no article bodies. (🟡 todo)
- `_validation/validation-sequence.md` exists with the run 1 / run 2 evidence. (🟡 todo)
- Findings 15–21 remain untouched and are recorded in § Park lot with dispositions. (✅ done)

---

## 📚 References

**[Azure Cosmos DB transactional batch limits](https://learn.microsoft.com/azure/cosmos-db/nosql/transactional-batch)** 📘 [Official]
Documents the 100-operation and 2 MB ceilings that Step C5 chunks against, and the requirement that all operations in a batch share one logical partition key.

**[Understanding the Table service data model](https://learn.microsoft.com/rest/api/storageservices/understanding-the-table-service-data-model)** 📘 [Official]
Defines the characters forbidden in `PartitionKey`/`RowKey` (`/`, `\`, `#`, `?`, control ranges) and the 64 KiB property / 1 MiB entity size limits that Steps D1–D3 respect.

**[Naming and referencing containers, blobs, and metadata](https://learn.microsoft.com/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata)** 📘 [Official]
Source for the 1 024-character blob-name ceiling and the path-segment rules that `FeedKeys.ToBlobPathSegment` enforces in Step E2.

**[Querying tables and entities — OData filter syntax](https://learn.microsoft.com/rest/api/storageservices/querying-tables-and-entities)** 📘 [Official]
Background for Step G3; `TableClient.CreateQueryFilter` is the escaping-safe way to build these filters from untrusted input.

**[TypeNameHandling caveats](https://www.newtonsoft.com/json/help/html/SerializeTypeNameHandling.htm)** 📗 [Verified Community]
Newtonsoft's own warning that `TypeNameHandling` other than `None` is unsafe with data from an external source — the basis for Step G4.

**[RFC 4287 — The Atom Syndication Format, §4.2.6 `atom:id`](https://www.rfc-editor.org/rfc/rfc4287#section-4.2.6)** 📘 [Official]
Establishes `atom:id` as the permanent, universally unique identifier of an entry, which is why Step B1 uses it as the primary `Guid` source.

**[IHttpClientFactory guidance](https://learn.microsoft.com/dotnet/core/extensions/httpclient-factory)** 📘 [Official]
Rationale for Step G5 replacing per-iteration `new HttpClient()` with a named factory client.

<!--
validations:
  grammar: {status: "not_run", last_run: null}
  readability: {status: "not_run", last_run: null}
  links: {status: "not_run", last_run: null}

article_metadata:
  filename: "01-robustness-fixes.plan.md"
  plan_status: "actionable"
  gate_run: "2026-08-15"
-->
