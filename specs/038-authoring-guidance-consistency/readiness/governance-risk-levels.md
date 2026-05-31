# Governance Risk Levels (T003)

This feature's governance risk level is **Medium**.

## Levels

- **Small.** Internal-only change (Tier 2) with no public `.fsi`, no generated
  output, and no command-surface change. Focused validation = the single package
  test plus `Dev`.
- **Medium.** Public `.fsi` contract change (Tier 1) and/or generated-output
  change that the guidance/template gates assert. Focused validation = the
  contract-specific gate (`PackageSurfaceCheck` + refreshed surface baselines)
  for the `.fsi` delta, plus the generated-output gates
  (`GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`) for the
  template delta. **This feature.**
- **Broad.** Cross-cutting change to the evidence gates themselves, the
  capability catalog, or the dependency boundary. Focused validation = the full
  sequential FAKE order plus `EvidenceGraph`/`EvidenceAudit`.

## Required evidence for the selected (Medium) level

The required evidence for the chosen level is the focused validation named below;
broad validation evidence is added only at integration.

## Focused validation required for the selected (Medium) level

- US3/US6 `.fsi` delta → `./fake.sh build -t PackageSurfaceCheck` + refreshed
  `readiness/surface-baselines/*`.
- US1 skill-resolution guard, US2 bundled API reference, US4 domain-agnostic
  generated guidance, US5 effects page → `./fake.sh build -t
  GeneratedGuidanceCheck` and `./fake.sh build -t TemplateCheck`.
- Generated-product file-lists for the new artifacts → `./fake.sh build -t
  GeneratedProductCheck`.

## When broad validation is required

Only at integration (T036), because US2/US4/US5 alter generated output that
`GeneratedGuidanceCheck`/`TemplateCheck`/`GeneratedProductCheck` assert, and the
SC-001 governing integration task must confirm a freshly generated consumer
project builds, tests, and produces evidence using only local references. The
full sequential FAKE order is run there.

## Non-authoritative aggregate policy

Aggregate / full-suite FAKE runs recorded under `readiness/logs/` are
**non-authoritative**: a flake in an unrelated headless GUI test (e.g. the known
`SkiaViewer.Tests` libdecor-gtk crash in aggregate runs) does not invalidate a
green focused gate. The focused gate result is authoritative; the aggregate is
recorded for context only and never substituted for a focused gate.
