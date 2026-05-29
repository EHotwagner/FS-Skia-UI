# Screenshot Evidence Probe

Status: complete.

Tasks: T031, T032
Captured: 2026-05-29T12:11:00+02:00

## Generated Checkout

Path: `artifacts/template-check/029-bomberman-demo-feedback/source-app`

Template regeneration:

```text
./fake.sh build -t TemplateCheck
```

Result: pass. Log: `readiness/templatecheck-us2-rerun.log`

## Screenshot Command

Command:

```text
dotnet run --project src/V3DotnetAppSource/V3DotnetAppSource.fsproj -- --screenshot-evidence readiness/game-screenshot-evidence.txt
```

Result:

- Exit code: 0
- Command log: `readiness/generated-screenshot-command.log`
- Generated report: `artifacts/template-check/029-bomberman-demo-feedback/source-app/readiness/game-screenshot-evidence.txt`
- Generated artifact: `artifacts/template-check/029-bomberman-demo-feedback/source-app/readiness/game-screenshot-evidence.png`
- Feature-local copied report: `readiness/screenshot-artifacts/game-screenshot-evidence.txt`
- Feature-local copied artifact: `readiness/screenshot-artifacts/game-screenshot-evidence.png`

## Report Fields

- `status=ok`
- `viewer-open-status=ViewerOpenConfirmed`
- `first-frame-status=FirstFramePresentedStatus`
- `capture-availability=CaptureAvailable`
- `capture-source=LiveViewerWindow`
- `fallback=none`
- `blocked-stage=none`
- `classification=none`
- `category=none`
- `pixel-content-validation=PixelContentNonBlank`
- `proves-screenshot=True`
- `diagnostics=...capture-source=live-viewer-window...proves-screenshot=true...`

## Artifact Validation

- PNG signature: `89504e470d0a1a0a`
- File type from host: PNG image data, 640 x 480, 8-bit/color RGBA, non-interlaced
- Artifact bytes: 3968
- Report NUL byte count: 0

The current host produced supported-host nonblank screenshot proof through the generated screenshot command, so SC-004 is satisfied without deferral.
