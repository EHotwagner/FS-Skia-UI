# Window Observation Diagnostics

Feature: `021-persistent-launch-evidence`
Updated UTC: `2026-05-27T15:03:33Z`

## Real Launch Attempt

- diagnostic-source: `real-launch`
- command: `dotnet run --project artifacts/generated-products/021-persistent-launch-evidence/app-source/src/Product/Product.fsproj -- --launch-evidence specs/021-persistent-launch-evidence/readiness/persistent-launch-evidence.md`
- command-log: `specs/021-persistent-launch-evidence/readiness/logs/t018-launch-evidence.txt`
- artifact: `specs/021-persistent-launch-evidence/readiness/persistent-launch-evidence.md`
- status: `ok`
- mode: `persistent-evidence`
- window-opened: `true`
- first-frame-presented: `true`
- input-dispatch: `not-required`
- exit-path: `true`
- renderer-mode: `skia`
- controlled-close: `self-closed-for-evidence=true`

The real generated-product evidence command opened the viewer evidence path,
presented a first frame, and exited through the controlled evidence close path.
This is not layout, screenshot, or deterministic hash proof.

## Generic Host Probe

- diagnostic-source: `generic-host-probe`
- `WAYLAND_DISPLAY=wayland-0`
- `DISPLAY=:1`
- `XDG_RUNTIME_DIR=/run/user/1000`
- `DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/1000/bus`
- desktop-prerequisite-stage: `present`
- blocked-stage: `none`
- classification: `none`
- message: desktop session variables required for a graphical launch were present for this run.

## Observation And Capture Classification

- diagnostic-source: `real-launch`
- verifier: `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "FullyQualifiedName~observation|FullyQualifiedName~capture"`
- verifier-log: `specs/021-persistent-launch-evidence/readiness/logs/t023-skiaviewer-tests.txt`
- external-observation-failure-stage: `Observation`
- capture-failure-stage: `Capture`
- forbidden-classification: `headless-only`

The public SkiaViewer classifier preserves viewer-owned `window-opened` and
`first-frame-presented` facts when external title/window matching fails. With
those viewer facts present, an external match failure is classified as
observation-blocked and a capture failure is classified as capture-blocked,
not as desktop-prerequisite or headless-only.

## Synthetic Fixture Distinctions

- diagnostic-source: `synthetic-fixture`
- verifier: `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --filter "FullyQualifiedName~PersistentLaunchArtifactValidation"`
- verifier-log: `specs/021-persistent-launch-evidence/readiness/logs/t024-testing-tests.txt`
- covered synthetic classes: missing required fields, invalid enum/string values, contradictory supported-host pass claims
- readiness status: parser/error handling only; does not satisfy supported-host persistent-launch evidence

The synthetic fixtures are disclosed in `tasks.md` and in the test fixture use
sites. They validate rejection diagnostics only. The supported-host evidence
path for this feature remains the real generated launch artifact above.
