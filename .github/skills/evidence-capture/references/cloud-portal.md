# Procedure — cloud portal

**Owning investigator**: `ad-environment-investigator`
**Bound by**: `.copilot/context/10.00-application-development/03-evidence-access-policy.md`

## Precondition

The capability matrix in `src/docs/_evidence/_discovery.md` records a **cloud portal** surface as present and reachable with grants the user already holds. If it does not, record the absence and STOP.

## Prefer the definition over the portal

Read the infrastructure definition **first**. The portal establishes what is actually provisioned; the definition establishes what is intended. A difference between them is itself a finding worth recording.

Visit the portal only for facts the definition cannot establish: what exists right now, what tier it runs at, and what is configured outside the definition.

## Announce before a production read

Production is reachable only after an explicit announcement and an explicit approval:

```
Source:      [the portal, named generically]
Environment: production
Reason:      [the fact that no lower environment can establish]
Operation:   read-only — [what will be opened]
```

Proceed only on approval. A refusal is a normal outcome — record a gap and continue.

## Capture

1. Open an **independent, undocked, visible** browser window.
2. Capture the **resource inventory** for the scope under investigation, then one asset per resource the investigation needs.
3. Capture configuration blades as text where the portal offers a text or template view — text is diffable.
4. Capture scaling, networking and identity settings only where a documentation page or an invariant class needs them.

## Read-only boundary

- You MUST NEVER create, modify, delete, restart, scale, redeploy or rotate anything.
- You MUST NEVER open a secret store's secret value, even when the grant allows it. Record that the reference exists; never the value.
- You MUST NEVER download a certificate, key or connection string.

## Classify — this surface is internal by default

| Content | Classification |
|---|---|
| A logical resource role and its tier | public |
| Subscription, tenant, resource group or account identifiers | internal |
| Hostnames, private endpoints, management URLs | internal |
| Connection strings, keys, certificate material | internal — and never captured at all |

A portal screenshot almost always carries an identifier in a header or breadcrumb. Treat every portal asset as **internal** unless you have confirmed otherwise, and expose in a published page only the logical name.

## Stamp and record

Write the sibling `.provenance.yml` with `surface: cloud-portal` and `classification: internal`, then add records to `_evidence/{component-id}/environment.md`. A record whose assertion or asset carries an identifier goes to `environment.internal.md` with a redacted stub left behind.

## On failure

No grant, no reachable environment, or approval refused → record a gap stating what was sought, which environment was attempted and what blocked it.
