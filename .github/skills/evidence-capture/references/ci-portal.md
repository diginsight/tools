# Procedure — CI portal

**Owning investigator**: `ad-devops-investigator`
**Bound by**: `.copilot/context/10.00-application-development/03-evidence-access-policy.md`

## Precondition

The capability matrix in `src/docs/_evidence/_discovery.md` records a **CI portal** surface as present. If it does not, record the absence and STOP.

## Prefer the definition over the portal

Read the pipeline definitions in the repository **first** — they establish triggers, stages, gates and environment progression as intended.

The portal establishes something the definition cannot: what actually ran, how often, how long it took and whether it succeeded. Run history is evidence of **behaviour**, never of definition — attribute it as observation.

## Capture

1. Open an **independent, undocked, visible** browser window.
2. Capture the **pipeline inventory** — every workflow or pipeline defined for this repository, with its enabled state.
3. For each pipeline the investigation needs, capture the **recent run history**: outcome, trigger and duration.
4. Capture a **stage view** of one representative successful run to establish real stage ordering.
5. Capture the environment or deployment view where the portal exposes promotion between environments.

## Read-only boundary

- You MUST NEVER queue, re-run, cancel or approve a run.
- You MUST NEVER approve or reject a deployment gate.
- You MUST NEVER open a variable group, secret or protected value. Record that the reference exists; never the value.
- You MUST NEVER modify a pipeline definition, schedule or trigger from the portal.

## Classify

| Content | Classification |
|---|---|
| Stage names, trigger kinds, outcomes, durations | public |
| Environment names as this repository names them | public |
| Runner or agent pool names revealing internal infrastructure | internal |
| Deployment targets carrying a subscription, tenant or host identifier | internal |
| Any variable value, token or credential | internal — and never captured at all |

## Stamp and record

Write the sibling `.provenance.yml` with `surface: ci-portal`, then add records to `_evidence/{component-id}/devops.md`. Each observation record MUST state the date observed — run history goes stale faster than any other evidence kind, and a stale success rate published as current is a false assertion.

## On failure

No access, no runs, or history retained too briefly to be meaningful → record a gap. A pipeline that is defined but has never run is a legitimate and reportable finding, not a failure.
