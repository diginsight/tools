# Procedure — API explorer

**Owning investigator**: `ad-code-investigator`
**Bound by**: `.copilot/context/10.00-application-development/03-evidence-access-policy.md`

## Precondition

The capability matrix in `src/docs/_evidence/_discovery.md` records an **API explorer** surface as present. If it does not, record the absence and STOP.

## Discover the address

Resolve the explorer URL from repository evidence only:

| Source | Yields |
|---|---|
| Launch or run profiles | the local base address and launch path |
| Deployment descriptors | the hosted base address per environment |
| Composition root | whether the explorer is registered, and under which route |
| Settings and overrides | whether it is enabled in this environment |

NEVER guess a conventional explorer path. NEVER reuse an address from another repository.

If the explorer is registered but disabled in every reachable environment, record a gap — do NOT enable it.

## Capture

1. Open an **independent, undocked, visible** browser window at the discovered address.
2. Wait for the operation list to finish rendering before capturing.
3. Capture the **operation index** — the full list of operation groups and operations.
4. For each operation the investigation needs, expand it and capture its request and response shapes.
5. Where the explorer exposes the raw description document, capture it as text rather than as an image — text is diffable and a screenshot is not.

## Read-only boundary

- You MAY execute an operation ONLY when it is idempotent AND the target is a lower environment.
- You MUST NEVER execute an operation with side effects, in any environment.
- You MUST NEVER submit real credentials, tokens or personal data into a try-it form.

## Classify

| Content | Classification |
|---|---|
| Operation names, shapes, status codes | public |
| A live host, tenant identifier or internal route | internal |
| An authorisation header, key or token visible in the UI | internal — and re-capture without it |

Recapture rather than redact in an image editor. An edited screenshot is no longer evidence.

## Stamp and record

Write the sibling `.provenance.yml` with `surface: api-explorer`, then add records to `_evidence/{component-id}/code.md` — one per operation established, each citing the asset path in `where` and `kind: capture`.

## On failure

Unreachable, unauthenticated or disabled → record a gap in `code.md` stating the address discovered, what was attempted and what blocked it.
