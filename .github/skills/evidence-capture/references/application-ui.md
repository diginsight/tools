# Procedure — application UI

**Owning investigator**: `ad-code-investigator`
**Bound by**: `.copilot/context/10.00-application-development/03-evidence-access-policy.md`

## Precondition

The capability matrix in `src/docs/_evidence/_discovery.md` records a **running application** surface as present. If it does not, record the absence and STOP.

## Discover the address and the start method

Resolve from repository evidence only:

| Source | Yields |
|---|---|
| Launch or run profiles | the start command and the local address |
| Entry points | which executables are startable and which are libraries |
| Settings sources | which configuration the local run will bind |
| Deployment descriptors | the hosted address per environment |

Prefer a **local run** over any deployed environment. Start it in a visible foreground console the user can stop.

If the application cannot start locally — a missing dependency, an unavailable backing service, an absent secret — record a gap. NEVER create a credential, seed a database or stub a dependency to force a start.

## Capture

1. Open an **independent, undocked, visible** browser window at the discovered address.
2. Bring the window to the front and let the page settle before capturing — an occluded or still-loading window produces a misleading image.
3. Capture the **entry surface** first, then one asset per distinct state the investigation needs.
4. For a flow, capture each step in order and name the assets in that order.
5. Where a screen is data-driven, prefer a state produced by the repository's own sample or seed data.

## Read-only boundary

- You MAY navigate, expand, filter and sort.
- You MUST NEVER submit a form that writes, deletes or triggers a job.
- You MUST NEVER sign in with credentials the user has not already established in the session.

## Classify

| Content | Classification |
|---|---|
| Layout, navigation, labels, empty states | public |
| Real user names, addresses, identifiers or records | internal — recapture against sample data |
| A visible token, session identifier or internal host | internal — recapture |

## Stamp and record

Write the sibling `.provenance.yml` with `surface: application-ui`, then add records to `_evidence/{component-id}/code.md`, each citing the asset path in `where` with `kind: capture` and the environment the capture came from.

## On failure

Cannot start, cannot reach, or blocked by authentication → record a gap stating the start method attempted, the address discovered and what blocked it.
