---
description: How to test and validate Learn.Web changes — always in a visible browser, and always recorded as a validation-sequence markdown with screenshots. Covers when to validate, how to run, what to capture, and where/how to store the artifact.
applyTo: 'src/Learn.Web/**,src/Learn.Web.Client/**,src/Learn.Web.Shared/**'
version: "1.0.0"
last_updated: "2026-07-26"
domain: "learn-web"
---

# Testing & validation rules (Learn.Web)

## Purpose

Make every runtime/UI change to the Learn.Web application **verifiable and reviewable**. Two things are mandatory and non-negotiable after such a change:

1. The behavior is validated in a **visible browser** the user can watch.
2. The validation run is recorded as a **validation-sequence markdown** with screenshots (or a recording), stored next to the work item.

## When this applies

Apply these rules whenever you change **runtime behavior or UI** of Learn.Web (Razor components, layout, navigation, rendering, client interactivity, status bar, styling that affects behavior). It does **not** apply to pure doc edits or non-runtime tooling.

Do not declare the task complete until the validation-sequence artifact exists and every scenario is marked PASS.

## How to run (visible browser — mandatory)

1. **Rebuild** the app — build `Learn.Web` normally (do **not** use `--no-build`, so Client WASM changes are served). If a previous instance is locking the output, stop it first.
2. **Run the server in a visible foreground console** (a normal terminal window the user can see and stop with Ctrl+C) — never a hidden/background process.
3. **Open a visible browser window** at the app URL (default `http://localhost:5280/`). Use a real, visible browser window — **never** the hidden in-editor/embedded browser surface. For automated evidence capture, a headed (visible) browser window is acceptable; a hidden/background page is not.
4. Reproduce each scenario end-to-end and read the actual on-screen result (prefer reading the live DOM value of the element under test so the observed value is exact).

## What to capture

For **each** scenario capture:

- the **precondition** (starting state),
- the **action** performed,
- the **expected** result,
- the **observed** result (the exact on-screen value), and
- a **screenshot** (or a short recording) of the validated state.

Screenshots MUST show the element under test in its validated state.

**Evidence format — choose by fit:**

- A **short screen recording** (mp4/gif) is a good fit when the behavior is motion-based **and the clip is short**. Store it in `_validation/images/` and embed/link it.
- Otherwise prefer **per-step PNGs** — one image per processed step. Step-by-step images give a clearer outlook of what was actually done.
- **Preferred layout for per-step images: a two-column table — the step description on the left, the step's image on the right** (one row per step). See the required shape below.

## Where and how to store the artifact

- **Location**: a `_validation/` subfolder **inside the work item's own folder**, kept **together with the issue/use-case definition** it validates. Images go in `_validation/images/`.
  - If the change has no work-item folder, create one under `src/docs/90. Issues/<yyyymm>/<yyyymmdd>.NN-<slug>/` and put `_validation/` inside it.
  - Do **not** put these issue-side validation docs in `src/docs/95. Validations/` — that folder is owned by the validator agent, which manages validations against the use-cases catalog. It is a separate concern from the per-issue `_validation/` artifact.
- **Markdown filename**: `<yyyymmdd>.<NN>-validation-sequence.md` — always prefixed with the reverse date and a sequence number (e.g. `20260726.01-validation-sequence.md`). This ensures chronological ordering and allows multiple validation rounds for the same work item (`.01`, `.02`, …).
- **Image filenames**: `NN-<short-slug>.png` (e.g. `01-hover-folder-shows-own-count.png`), referenced with relative paths (`images/NN-….png`).
- Mark the file **`publish: false`** in frontmatter — it is a working artifact, never wired into site navigation.

## Required shape of `validation-sequence.md`

Frontmatter:

```yaml
---
title: "Validation sequence — <short subject>"
type: validation-sequence
date: "<yyyy-mm-dd>"
publish: false
target:
  area: "<what area/behavior>"
  change: "<one-line description of the change>"
  files: [ "<path>", ... ]
environment:
  url: "http://localhost:5280/"
  build: "<build command + result>"
  browser: "visible browser window (headed)"
result: PASS | FAIL
---
```

Body MUST contain:

1. A one-paragraph statement of the behavior being validated.
2. An **Environment** table (URL, build, browser, date).
3. A **Sequence and results** table with columns: `# | Precondition | Action | Expected | Observed | Result` (the pass/fail matrix).
4. An **Evidence** presentation:
   - **Per-step images (preferred):** a **two-column table — left column = the step description, right column = the step's screenshot** (`![alt](images/NN-….png)`), one row per step. Descriptive alt text is required (accessibility rule from `article-writing.instructions.md`).
   - **OR a short recording:** embed/link the clip when it is short and motion matters.
5. A short **Notes** section for caveats (e.g. responsive mode, timing).

## Never do

- Never validate only by compiling or by `Invoke-WebRequest`/`curl` alone for a UI/behavior change — those confirm the app serves, not that the behavior is correct.
- Never use a hidden/embedded/background browser as the validation surface.
- Never mark a task complete for a Learn.Web behavior/UI change without a `validation-sequence.md` whose scenarios are all PASS.
- Never wire a `_validation/` artifact into the site's render/navigation config.
