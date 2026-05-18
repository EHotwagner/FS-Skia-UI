# Quickstart: Tetris Demo Integration Improvements

## 1. Review The Plan

```bash
sed -n '1,240p' specs/013-tetris-demo-integration/plan.md
```

Confirm the feature remains scoped to viewer input, bounded smoke,
diagnostics, scene evidence, generated templates, and local consumer package
guidance.

## 2. Verify Public Contracts

The public contracts for this feature live under:

```text
src/SkiaViewer/
src/KeyboardInput/
src/Scene/
src/Testing/
```

Exercise the packed/public surface with:

```bash
./fake.sh build -t FsiTranscripts
./fake.sh build -t PackageSurfaceCheck
```

## 3. Run Focused Tests

Target the smallest focused test projects first:

```bash
dotnet test tests/KeyboardInput.Tests/KeyboardInput.Tests.fsproj
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
dotnet test tests/Scene.Tests/Scene.Tests.fsproj
dotnet test tests/Testing.Tests/Testing.Tests.fsproj
dotnet test tests/Governance.Tests/Governance.Tests.fsproj
dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj
dotnet test tests/Package.Tests/Package.Tests.fsproj
```

These tests cover normalized viewer input, bounded smoke evidence, diagnostic
capture/filtering, headless scene evidence, generated template input-flow
coverage, and local package validation.

## 4. Validate Generated Consumer Workflow

Use the repository build workflow once implementation tasks exist:

```bash
./fake.sh build -t PackLocal
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
./fake.sh build -t GeneratedProductCheck
```

The generated guidance must print package identities, versions, local feed
path, consumer configuration, and restore command. Generated product
validation must separate package drift from app/input/rendering defects.
Generated apps should expose startup-focused bounded smoke by default and an
explicit frame-diagnostics smoke mode when frame-loop messages are needed.
The generated app flow evidence covers start, options, primary interaction,
pause/back where generated, and restart/exit through normalized viewer-key
events.

## 5. Collect Readiness Evidence

Populate the required readiness files:

```text
specs/013-tetris-demo-integration/readiness/normalized-viewer-input.md
specs/013-tetris-demo-integration/readiness/bounded-viewer-smoke.md
specs/013-tetris-demo-integration/readiness/diagnostics.md
specs/013-tetris-demo-integration/readiness/headless-scene-evidence.md
specs/013-tetris-demo-integration/readiness/generated-template-input-flows.md
specs/013-tetris-demo-integration/readiness/local-consumer-packages.md
specs/013-tetris-demo-integration/readiness/generated-consumer-validation.md
specs/013-tetris-demo-integration/readiness/evidence-graph.md
specs/013-tetris-demo-integration/readiness/evidence-audit.md
```

Real bounded viewer startup and deterministic scene-level evidence must both
be represented. Unsupported host conditions must be explicit diagnostics, not
silent skips.

## 6. Final Gates

Run the broad gates after focused evidence is current:

```bash
./fake.sh build -t Verify
./fake.sh build -t Ci
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

`Verify` and `Ci` write verdicts under
`specs/013-tetris-demo-integration/readiness/verification-verdicts.md`.
`EvidenceGraph` refreshes the task DAG, and `EvidenceAudit` checks synthetic
propagation and diff-scan blockers.
