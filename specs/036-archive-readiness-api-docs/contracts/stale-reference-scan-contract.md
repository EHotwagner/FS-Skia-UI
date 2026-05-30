# Contract: Stale Reference Scan

The stale-reference scan writes:

`specs/036-archive-readiness-api-docs/readiness/stale-reference-scan.md`

Optional machine-readable findings may be written beside it as
`stale-reference-scan.json`.

## Blocking Scan Areas

- `docs/**`
- `template/**`
- generated guidance files
- build reports and active readiness reports
- `specs/036-archive-readiness-api-docs/**`

## Informational Scan Areas

- historical `specs/*/readiness/**`
- historical `specs/*/{plan.md,tasks.md,quickstart.md,research.md}`
- mailbox or analysis notes unless cited by an active surface

## Required Finding Fields

- `source-path`
- `referenced-path`
- `scan-area`
- `severity`
- `reason`
- `replacement-path`
- `line`
- `next-action`

## Rules

- Active-surface findings are blocking when they cite archived readiness as
  current pass/fail evidence.
- Historical findings are informational unless an active surface uses them as
  current evidence.
- Scanner output must distinguish archived, roadmap/deferred, retained, and
  current evidence.
- Scanner failures must name the exact active file and replacement guidance.

## Failure Conditions

- Any blocking finding remains.
- The scan cannot identify whether a finding came from an active or historical
  surface.
- The scan silently ignores a referenced archived path that appears in active
  generated guidance.
