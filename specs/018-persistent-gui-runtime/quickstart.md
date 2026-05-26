# Quickstart: Persistent GUI Runtime

## Design the Public Surface First

1. Update `src/SkiaViewer/SkiaViewer.fsi` with explicit interactive/evidence launch contracts and unambiguous launch outcome fields.
2. Add failing semantic tests through the `.fsi` surface before editing `SkiaViewer.fs`.
3. Refresh surface baselines only after the contract is intentional.

## Validate Interactive Lifecycle

Expected regression coverage:

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "Interactive"
```

The failing-first test must prove that first-frame presentation does not close `Viewer.runApp` when no close action occurred.

On a supported desktop host:

```bash
dotnet run --project src/Product/Product.fsproj
```

Record `readiness/interactive-lifecycle.md` with `mode=interactive-window`, `first-frame-presented=true`, and `self-closed-for-evidence=false`.

## Validate Explicit Evidence Mode

```bash
dotnet run --project src/Product/Product.fsproj -- --bounded-smoke specs/018-persistent-gui-runtime/readiness/evidence-launch-mode.md
```

The evidence output must disclose whether it self-closed and must not claim successful interactive play.

## Validate Desktop Session Readiness

Run the container/session diagnostic before app debugging:

```bash
test -n "$XDG_RUNTIME_DIR"
test -d "$XDG_RUNTIME_DIR"
test "$(stat -c %a "$XDG_RUNTIME_DIR")" = "700"
test -n "$DISPLAY$WAYLAND_DISPLAY"
```

For Wayland:

```bash
test -S "$XDG_RUNTIME_DIR/$WAYLAND_DISPLAY"
```

For X11:

```bash
test -S "/tmp/.X11-unix/X${DISPLAY#:}"
```

Capture results in `readiness/container-session-diagnostics.md`.

## Validate Package Resolution

```bash
dotnet restore src/Product/Product.fsproj --configfile NuGet.config --no-cache
```

Verification must fail on `NU1603` or any requested/resolved `FS.Skia.UI.*` package mismatch. Record requested versions, resolved versions, and package sources in `readiness/package-resolution.md`.

## Validate Generated Tests and Visual Evidence

```bash
dotnet test tests/Product.Tests/Product.Tests.fsproj
./fake.sh build -t GeneratedProductCheck
```

Generated `Verify` must run generated tests when present. Placeholder targets must be labeled non-authoritative.

On a supported graphical host, capture screenshot evidence for the generated game board. If screenshots are unavailable but pixels can be inspected, capture pixel-readback evidence instead. If neither is possible, record an explicit unsupported-host diagnostic in `readiness/game-visual-evidence.md`.

## Validate Governance

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
```

Expected readiness outputs:

- `specs/018-persistent-gui-runtime/readiness/interactive-lifecycle.md`
- `specs/018-persistent-gui-runtime/readiness/evidence-launch-mode.md`
- `specs/018-persistent-gui-runtime/readiness/container-session-diagnostics.md`
- `specs/018-persistent-gui-runtime/readiness/package-resolution.md`
- `specs/018-persistent-gui-runtime/readiness/generated-verify.md`
- `specs/018-persistent-gui-runtime/readiness/game-visual-evidence.md`
- `specs/018-persistent-gui-runtime/readiness/task-workflow-guidance.md`
- `specs/018-persistent-gui-runtime/readiness/evidence-audit.md`
