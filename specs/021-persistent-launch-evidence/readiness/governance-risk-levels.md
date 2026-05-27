# Governance Risk Levels

Feature: `021-persistent-launch-evidence`

- tier: Tier 1 contracted framework, generated-template, and governance change
- risk level: broad
- supported risk vocabulary: small, medium, broad
- affected packages: `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Testing`, generated product template
- required evidence: public surface baselines, semantic tests, generated
  product validation, generated guidance validation, template validation, and
  readiness artifacts
- required checks: package tests, surface baselines, generated product checks,
  generated guidance checks, template checks, evidence graph, evidence audit,
  and broad `Verify`
- broad validation: `./fake.sh build -t Verify` remains the final aggregate
  validation target after focused evidence is green
- synthetic status: malformed artifact parser fixtures are disclosed separately
  and cannot satisfy supported-host persistent-launch evidence
