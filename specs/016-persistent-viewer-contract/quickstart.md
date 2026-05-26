# Quickstart: Persistent Viewer Contract

## Validate API Design First

1. Update `src/SkiaViewer/SkiaViewer.fsi` with the persistent viewer, generated app host, runtime capability, and launch outcome contract.
2. Add failing semantic tests in `tests/SkiaViewer.Tests/Tests.fs` that exercise the `.fsi` surface.
3. Refresh `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` only after the surface is intentional.

## Implement and Test Package Behavior

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
./fake.sh build -t CapabilityCheck
./fake.sh build -t PackLocal
```

## Validate Generated App Behavior

The generated app default command must attempt persistent launch:

```bash
dotnet run --project src/Product/Product.fsproj
```

Bounded evidence remains explicit:

```bash
dotnet run --project src/Product/Product.fsproj -- --bounded-smoke specs/016-persistent-viewer-contract/readiness/bounded-smoke.txt
dotnet run --project src/Product/Product.fsproj -- --scene-evidence specs/016-persistent-viewer-contract/readiness/headless-scene-evidence.txt
```

## Required Supported-Host Evidence

On a supported desktop host, capture:

```text
specs/016-persistent-viewer-contract/readiness/supported-host-persistent-launch.txt
```

The artifact must include `status=ok`, `mode=persistent-window`, `window-opened=true`, and `exit-path=true`. If the generated profile declares keyboard behavior, it must also include `input-dispatch=true`.

Unsupported-host diagnostics may be captured separately but do not complete the feature.

## Governance Verification

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
```

Expected readiness outputs include:

- `specs/016-persistent-viewer-contract/readiness/persistent-viewer-contract.md`
- `specs/016-persistent-viewer-contract/readiness/generated-default-launch.md`
- `specs/016-persistent-viewer-contract/readiness/bounded-evidence-separation.md`
- `specs/016-persistent-viewer-contract/readiness/runtime-capability-diagnostics.md`
- `specs/016-persistent-viewer-contract/readiness/generated-guidance-check.md`
- `specs/016-persistent-viewer-contract/readiness/evidence-graph.md`
- `specs/016-persistent-viewer-contract/readiness/evidence-audit.md`
