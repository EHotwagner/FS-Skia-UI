# Governance Risk Levels (T003)

Defines the small / medium / broad governance risk levels, the focused
validation required for each, when broad validation is required, and the
required evidence.

## Levels

### Small
Internal-only change with no `.fsi` / public-contract impact and no change to
the audit/graph gates themselves (e.g. a readiness note, a comment, a guidance
doc edit). No new package, no generated-product behavior change.

- **Focused validation**: the single FAKE target touching the changed area
  (commonly `Dev`), plus the relevant readiness note.
- **Required evidence**: the focused target log under `readiness/logs/`.

### Medium (this feature's level)
A focused public-contract change OR a change to governance/template behavior
that does not redesign the gates wholesale. Here: the single contract change is
`[<RequireQualifiedAccess>]` on `ControlEventOrigin` (US3); US1/US2 alter the
audit/graph gates' resolution and parsing but not their overall shape; US4 adds
generated content.

- **Focused validation**: `PackageSurfaceCheck` + refreshed surface baselines
  for the contract change; targeted audit-fixture runs for the parsing change;
  generated-product checks for the template change.
- **Broad validation required at integration** (see below) because US1/US2
  alter the audit/graph gates themselves.
- **Required evidence**: surface-baseline delta, audit-fixture PASS/BLOCK
  transcripts, feature-resolution note, FSI load transcript.

### Broad
A wide redesign of the audit rules, package boundaries, or template surface; new
packages; or changes whose blast radius is not locally containable.

- **Focused validation**: insufficient on its own.
- **Required evidence**: the full sequential FAKE order (below) plus all
  per-story evidence.

## When broad validation is required

Broad validation (the full sequential FAKE order) is required at **integration**
for this feature because US1/US2 modify the audit and evidence-graph gates that
all other validation relies on. A green focused gate is never treated as a
substitute for the integration run; non-authoritative aggregate results are
recorded under `readiness/logs/` and labeled non-authoritative.

## Sequential FAKE order (never concurrent — shared `.fake` state)

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Plus `./fake.sh build -t PackageSurfaceCheck` for the US3 baseline refresh.
