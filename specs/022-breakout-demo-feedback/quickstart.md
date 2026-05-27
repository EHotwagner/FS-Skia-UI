# Quickstart: Breakout Demo Feedback

Run these checks after tasks are generated and implemented.

## Failing-First Contract Checks

```bash
./fake.sh build -t Verify
```

Expected coverage:

- Scene semantic tests fail before circle/ellipse public contracts are
  implemented and pass through the packed `.fsi` surface after implementation.
- SkiaViewer/Testing tests fail before screenshot success/unsupported contracts
  and report conventions are implemented.
- Generated guidance tests fail when source, docs, tests, or package surface
  reference different viewer launch names.

## Generated Template Checks

```bash
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedGuidanceCheck
```

Expected coverage:

- Fresh generated app follows the documented viewer launch path.
- Selected persistent launch contract is
  `Viewer.runApp viewerOptions Product.Program.generatedHost`.
- Generated examples render circular or elliptical entities without rectangle
  substitutions.
- Generated guidance keeps app update commands separate from viewer effects.
- Generated evidence commands write consistent key-value reports.

## Evidence Checks

```bash
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Required readiness files:

- `specs/022-breakout-demo-feedback/readiness/generated-viewer-guidance.md`
- `specs/022-breakout-demo-feedback/readiness/scene-shape-evidence.md`
- `specs/022-breakout-demo-feedback/readiness/screenshot-evidence.md`
- `specs/022-breakout-demo-feedback/readiness/effect-boundary-guidance.md`
- `specs/022-breakout-demo-feedback/readiness/evidence-report-conventions.md`

## Screenshot Evidence Behavior

On a supported desktop host, run the generated screenshot evidence command and
verify `status=ok`, `evidence-kind=screenshot`, dimensions, output path, and
screenshot path.

On a host where screenshot capture is unavailable, run the same command and
verify `status=unsupported`, `unsupported-host-reason`, and
`fallback=deterministic-scene-evidence` with no screenshot proof claim.
