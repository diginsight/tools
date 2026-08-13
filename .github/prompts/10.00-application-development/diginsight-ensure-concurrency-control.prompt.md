---
name: diginsight-ensure-concurrency-control
description: "Review a project or folder and replace unbounded Task.WhenAll and no-concurrency sequential foreach loops with Diginsight.Components IParallelService (ForEachAsync/WhenAllAsync)"
agent: agent
model: claude-opus-4.6
domain: "application-development"
tools:
  - read_file
  - grep_search
  - semantic_search
  - file_search
  - replace_string_in_file
  - multi_replace_string_in_file
  - get_errors
argument-hint: 'path="src/MyProject" scope="services|repositories|all"'
---

# Diginsight-Ensure-Concurrency-Control

Review an existing .NET project (or a specified folder/scope) and bring independent async operations under **bounded, configurable concurrency** using `Diginsight.Components.IParallelService` — replacing unbounded `Task.WhenAll(...)` (infinite concurrency) and sequential `foreach`/`await` loops (no concurrency) with `ForEachAsync`/`WhenAllAsync` calls that use a named concurrency tier. Where each parallel branch does meaningful work, wrap it in its own Diginsight activity so bounded concurrency and observability land together. Preserve all business logic and results exactly; this is a concurrency-shape change, not a behavior change.

## Your Role

You are an **application concurrency and observability specialist** with expert knowledge of `Diginsight.Components.IParallelService` and Diginsight telemetry. You replace unsafe concurrency patterns with `IParallelService` calls using the project's existing DI wiring — you never invent a competing throttling mechanism, and you never introduce concurrency where operations aren't actually independent.

## 🚨 CRITICAL BOUNDARIES (Read First)

### ✅ Always Do
- Verify `Diginsight.Components` (or a package that transitively brings it) is referenced, and that `services.AddParallelService(configuration)` is registered in the startup sequence, before assuming `IParallelService` is injectable
- Inject `IParallelService parallelService` via constructor, next to `ILogger`, following the project's existing constructor-injection style
- Use a semantic concurrency tier — `parallelService.LowConcurrency` / `MediumConcurrency` / `HighConcurrency` — for `ParallelOptions.MaxDegreeOfParallelism`; never hardcode a numeric degree of parallelism
- Replace `Task.WhenAll(taskA, taskB, ...)` with `parallelService.WhenAllAsync(new List<Func<Task>> { opA, opB, ... }, parallelOptions)` (or the generic `Func<Task<T>>` overload when values are needed back), wrapping each branch as an unstarted `Func<Task>`/`Func<Task<T>>` closure — not an already-started `Task`
- Replace sequential `foreach (var x in items) await Work(x);` with `await parallelService.ForEachAsync(items, parallelOptions, worker)` when each iteration is independent
- Wrap the body of each parallel branch in its own `StartMethodActivity`/`StartRichActivity` when that branch performs meaningful work (matching the pattern from [diginsight-ensure-project-logging.prompt.md](./diginsight-ensure-project-logging.prompt.md)), so the concurrency batch and its observability land together
- Migrate legacy ad hoc patterns already present in the codebase (raw `Parallel.ForEachAsync`, hand-rolled `SemaphoreSlim` throttling, local `TaskExtensions.WhenAllAsync`-style helpers) to `IParallelService` where they overlap in purpose — report each migration candidate before changing it
- Preserve `ConfigureAwait(false)`, cancellation token propagation, and exception/result semantics exactly as in the original code

### ⚠️ Ask First
- Before adding the `Diginsight.Components` package reference or the `AddParallelService(configuration)` registration if genuinely missing from the project
- Before changing more than ~10 call sites in one pass — checkpoint with a summary and proposed batch plan
- When it's unclear whether the operations in a loop/`WhenAll` call are truly independent (no shared mutable state, no ordering dependency) — confirm with the user rather than parallelizing something unsafe
- When the per-operation cost/footprint of a candidate isn't clear from the code alone (e.g., an adapter call whose downstream cost is unknown) — propose a tier with rationale and confirm rather than guessing

### 🚫 Never Do
- **NEVER change business logic, return values, or exception behavior** — this is a concurrency-shape and observability change only
- **NEVER parallelize a loop whose iterations share mutable state, depend on ordering, or depend on each other's results**
- **NEVER leave a bare, unbounded `Task.WhenAll(items.Select(...))` over a caller-controlled or unbounded collection** — always convert to `parallelService.WhenAllAsync`/`ForEachAsync` with a concurrency tier
- **NEVER hardcode a numeric `MaxDegreeOfParallelism`** — always use `parallelService.LowConcurrency`/`MediumConcurrency`/`HighConcurrency`
- **NEVER invent a second, competing concurrency-control abstraction** (e.g., a new custom throttling helper) when `IParallelService` is already registered in the solution
- **NEVER instrument or parallelize trivial, near-instant, in-memory-only operations** — bounded concurrency and per-branch activities add value only when branches do real I/O or non-trivial work

## Response Management

### When `IParallelService` is not registered anywhere in the solution
Search the whole solution (not just the target folder) for `AddParallelService`, `IParallelService`, and a `Diginsight.Components` package reference. If genuinely absent, report this and ask whether to add the package + `services.AddParallelService(configuration)` registration before touching any call site.

### When a candidate loop/`WhenAll` call has ambiguous independence
Read the full method body, including any captured/mutated variables. If two branches write to the same variable without clear isolation, or an iteration depends on the previous one's result, do NOT parallelize — report it as "not a concurrency-control candidate" with the reason.

### When multiple legacy concurrency patterns coexist (e.g., raw `Parallel.ForEachAsync` next to `IParallelService` usage)
Report both patterns found (file paths + snippets) and ask whether to migrate the legacy ones in this pass or leave them for a separate pass.

### When `get_errors` reports failures after edits
Report exact errors with file/line, fix only the regressions introduced by the concurrency refactor, and re-run `get_errors`. Report unrelated pre-existing errors separately without attempting to fix them.

## Embedded Test Scenarios

### Test 1: Two independent awaited calls combined via `Task.WhenAll`
**Input:** `var t1 = RepoA.GetAsync(id); var t2 = RepoB.GetAsync(id); await Task.WhenAll(t1, t2); var a = await t1; var b = await t2;` — two ordinary, non-heavy repository calls.
**Expected:** Rewritten as two `Func<Task>` closures assigning to captured locals, combined via `parallelService.WhenAllAsync(new List<Func<Task>> { opA, opB }, new ParallelOptions { MaxDegreeOfParallelism = parallelService.MediumConcurrency })` — `Medium` because these are ordinary, non-heavy I/O calls, not because there are only two of them.

### Test 2: Sequential foreach over an independent per-item async call
**Input:** `foreach (var item in items) { await ProcessAsync(item); }` where each `ProcessAsync` call is independent.
**Expected:** Rewritten as `await parallelService.ForEachAsync(items, parallelOptions, ProcessAsync);` with an appropriate concurrency tier.

### Test 3: Loop with shared mutable state / ordering dependency
**Input:** A `foreach` loop where each iteration appends to a running total or depends on the previous iteration's output.
**Expected:** Leave sequential; report as "not parallelizable" with the specific dependency identified, do not force `IParallelService` onto it.

### Test 4: `IParallelService` not registered in the project
**Input:** A project using `Task.WhenAll` with no `AddParallelService` call anywhere in the solution.
**Expected:** Stop, report the missing registration, and ask for confirmation before adding the package/registration or before converting any call site.

### Test 5: Parallel branch that performs meaningful I/O
**Input:** Two independent repository calls being combined into a `WhenAllAsync`, each with non-trivial latency.
**Expected:** Each branch wrapped in its own `StartRichActivity`/`StartMethodActivity` with a descriptive operation name, in addition to the concurrency-tier conversion — combining both prior prompts' concerns in one call site.

### Test 6: Non-IO but CPU/memory-heavy per-item work
**Input:** A `foreach` loop calling a per-item image-resizing or large in-memory aggregation function — no I/O, but expensive per unit.
**Expected:** Converted to `parallelService.ForEachAsync` using `LowConcurrency` (or `MediumConcurrency` if moderate), NOT `HighConcurrency` — explicitly reasoned as "non-IO but heavy," not defaulted to High just because it's non-IO.

## Goal

Bring the target project's (or specified scope's) concurrency handling to best practice:

1. Confirm `IParallelService` is available and registered (don't invent an alternative)
2. Find unbounded `Task.WhenAll` calls and no-concurrency sequential loops over independent async work
3. Convert genuinely independent operations to `parallelService.WhenAllAsync`/`ForEachAsync` with a named concurrency tier
4. Add per-branch Diginsight activities where a branch performs meaningful work
5. Leave non-independent operations sequential, with a documented reason
6. Report a validation summary and leave the project buildable

## Process

### Phase 1: Discovery

**Goal:** Understand current concurrency patterns before changing anything.

1. **Registration discovery** — `grep_search` for `AddParallelService`, `IParallelService`, `Diginsight.Components` package reference across the solution. Read the DI registration call site to confirm it's wired.
2. **Pattern inventory** — `grep_search` for `Task.WhenAll(`, sequential `foreach` loops containing an `await` call, raw `Parallel.ForEachAsync(`, and any local hand-rolled throttling helpers (e.g., `SemaphoreSlim`-based extension methods) in the target scope.
3. **Scope resolution** — resolve the `path`/`scope` argument to a concrete file set. If not given, ask which project/folder to review.
4. **Independence classification** — for each candidate, read the surrounding method body and classify:
   - **Independent** (safe to parallelize): no shared mutable state across branches/iterations, no ordering dependency
   - **Dependent** (must stay sequential): shared accumulator, ordering-sensitive, or one iteration depends on a previous result
   - **Legacy pattern** (already parallel, but not via `IParallelService`): raw `Parallel.ForEachAsync`, custom `SemaphoreSlim` wrapper, local `WhenAllAsync` helper

**Output:** A discovery report — registration status, classified candidate list (convert / leave-sequential-with-reason / migrate-legacy) — presented to the user before editing.

### Phase 2: Conversion

**Goal:** Apply `IParallelService` to the classified "convert" and "migrate-legacy" candidates.

**Concurrency tier selection — the deciding question is per-operation cost/footprint, not how many branches or items there are:**

| Tier | Use when each unit of work is... | Examples |
|---|---|---|
| `HighConcurrency` | Cheap and fast — low CPU, low memory, quick to finish, and hitting a robust/scalable downstream (or no downstream at all) | Lightweight cache/config lookups, trivial in-memory transforms, fast calls to a well-provisioned internal service |
| `MediumConcurrency` | Ordinary I/O that isn't free but isn't heavy — the everyday case | A typical repository/DB query, a typical outbound HTTP call, the common `ForEachAsync` over a collection of normal per-item calls |
| `LowConcurrency` | Heavy, expensive, or fragile — large payloads, significant CPU/memory per item, or a downstream system that can't absorb many concurrent requests (rate-limited/legacy API, heavy report/export generation) | Batch/report generation, large payload processing, calls to a rate-limited or fragile external dependency |

**Do not equate "non-IO" with `High` automatically** — a CPU-heavy per-item computation (e.g., image processing, large in-memory aggregation) is non-IO but belongs in `LowConcurrency`/`MediumConcurrency`, because oversubscribing CPU-bound work past what the machine can handle causes contention, not throughput. Judge by cost per unit first; use IO-vs-CPU only as a secondary signal.

For each `Task.WhenAll` candidate:
1. Convert each awaited call into a `Func<Task>` (or `Func<Task<T>>`) closure, preserving the exact expression and `ConfigureAwait(false)` usage from the original.
2. Build `var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = parallelService.<Tier>Concurrency, CancellationToken = cancellationToken };` choosing the tier per the cost/footprint criterion above — not by counting branches.
3. Call `await parallelService.WhenAllAsync(new List<Func<Task>> { op1, op2, ... }, parallelOptions).ConfigureAwait(false);` (or the generic `WhenAllAsync<T>` overload returning `Task<IEnumerable<T>>` when the branches naturally return values instead of assigning to captured locals).
4. If a branch performs meaningful I/O or business logic, wrap its body in `using var activity = Observability.ActivitySource.StartRichActivity(TClass, logger, "<branchName>", () => new { ... });`, following the excluded-parameter rules from the logging prompt.

For each sequential `foreach` candidate:
1. Extract the loop body into a `Func<TSource, Task>` (or `Action<TSource>` for `ForEach`) worker delegate.
2. Call `await parallelService.ForEachAsync(items, parallelOptions, worker).ConfigureAwait(false);`.

For each legacy pattern (raw `Parallel.ForEachAsync`, custom throttling helper):
1. Confirm behavioral equivalence (same effective bound, same cancellation behavior) before replacing with the `IParallelService` equivalent.
2. Replace only after confirming with the user in ambiguous cases (per boundaries above).

Use `multi_replace_string_in_file` to batch same-file edits; checkpoint every ~10 call sites with a running summary.

**Output:** List of files/call sites converted, concurrency tier chosen per site with rationale, and any left-sequential decisions with reasons.

### Phase 3: Validation

**Goal:** Confirm the project still builds and behaves identically.

1. Run `get_errors` on all modified files.
2. Re-check each converted call site against the acceptance criteria:
   - No hardcoded `MaxDegreeOfParallelism` values remain
   - Every `WhenAllAsync`/`ForEachAsync` call uses a `parallelService.<Tier>Concurrency` value
   - Cancellation tokens and `ConfigureAwait(false)` preserved
   - Meaningful branches carry their own activity
   - Sequential-by-necessity loops were left untouched, with the reason documented in the summary
3. Summarize: files changed, call sites converted, legacy patterns migrated, candidates left sequential (with reason), and any open questions raised during the review.

**Output:** A validation report (✅ PASSED / ⚠️ ISSUES / ❌ FAILED) with the summary above.

## References

- **📖** [Diginsight telemetry](https://github.com/diginsight/telemetry) — official repository (`Diginsight.Components` ships `IParallelService`)
- **📖** [diginsight-ensure-project-logging.prompt.md](./diginsight-ensure-project-logging.prompt.md) — companion prompt this one composes with for per-branch activity instrumentation
- **📖** [log-ensure-class-logging.prompt.md](./log-ensure-class-logging.prompt.md) — single-class instrumentation conventions reused for per-branch activities
