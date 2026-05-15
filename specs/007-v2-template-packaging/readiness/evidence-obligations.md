# Evidence Obligations

Feature: `007-v2-template-packaging`

## Tier 1 Scope

This feature changes the template and governance contract. It does not add or
remove runtime library public APIs.

## Runtime Surface Impact

Runtime `.fsi` files and stable package surface baselines are expected to be a
no-op for this feature. Verification must keep `PackageSurfaceCheck` and the
root `readiness/surface-baselines/*.txt` checks in place.

## MVU Workflow Boundary

Template validation, dependency reporting, generated guidance checks, and drift
detection are I/O-bearing workflows. They must remain represented in
`build.fsx` through:

- `BuildModel`
- `BuildMsg`
- `BuildEffect`
- `init`
- pure `update`
- interpreter functions that execute filesystem and process work at the edge

Governance tests assert emitted effects and real interpreter evidence.

## Real Evidence Required

- Source-directory template install
- Local template package creation and install
- Default and minimal generated projects from both artifact types
- Generated `./fake.sh build -t Dev` smoke logs
- Placeholder and excluded-history scans
- Dependency governance report
- Generated guidance report
- Template drift report
- `Verify` and `Ci` target logs proving V1 workflows still compose with V2 gates

## Deferred Boundaries

Full visual evidence, release validation, a separate external template
repository, and broader distribution automation remain deferred roadmap work.
The V2 pass/fail boundary is non-visual generated project validation and
governance.
