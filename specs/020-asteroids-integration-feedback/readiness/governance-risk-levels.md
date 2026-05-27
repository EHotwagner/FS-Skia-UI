# Governance Risk Levels

Task: T038

This feature is a broad Tier 1 change because it changes public Scene/Testing
contracts, generated game layout behavior, generated product validation,
readiness evidence, and evidence audit expectations.

## Levels

- `small`: one focused readiness wording change or package-local helper with no
  public surface or generated-product effect. Required evidence is the touched
  package test or file scan.
- `medium`: generated template guidance, validation helper behavior, or public
  guidance. Required evidence is the affected package tests plus the owning
  FAKE target.
- `broad`: public `.fsi` changes, generated executable behavior, package
  surface baselines, generated product behavior, or readiness/audit semantics.
  Required evidence is focused validation plus broad validation.

## Broad Validation

Broad validation for this feature is `./fake.sh build -t Verify`, supported by
`PackageSurfaceCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`,
`TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit`.

If an aggregate target fails or hangs, record it as a non-authoritative
aggregate until focused reruns isolate the stage, elapsed duration, last
observed command, focused rerun, focused rerun result, and product evidence
authority.
