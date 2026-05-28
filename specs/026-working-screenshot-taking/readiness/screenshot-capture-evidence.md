# Screenshot Capture Evidence

Status: pending implementation.

This file will hold supported-host screenshot capture runs for the working
viewer-backed screenshot path. Accepted rows must cite a PNG produced by working
code, decoded dimensions, non-blank pixel validation, command, host facts,
capture mode, timestamp, and any retry statistics needed for SC-001.

## Initial Working-Code Capture

- Command: `LD_LIBRARY_PATH=/home/developer/.nuget/packages/skiasharp.nativeassets.linux/4.147.0-preview.3.1/runtimes/linux-x64/native:$LD_LIBRARY_PATH dotnet fsi specs/026-working-screenshot-taking/readiness/fsi/screenshot-smoke.fsx`
- Transcript: `specs/026-working-screenshot-taking/readiness/fsi/screenshot-smoke.txt`
- Artifact: `specs/026-working-screenshot-taking/readiness/artifacts/working-screenshot-record.png`
- Record: `specs/026-working-screenshot-taking/readiness/artifacts/working-screenshot-record.txt`
- Status: `ScreenshotOk`
- Capture mode: `ViewerRenderTargetPng`
- Capture source: `LiveViewerWindow`
- Dimensions: `320x200`
- Pixel content validation: `PixelContentNonBlank`
- Proves screenshot: `True`

This is a single-run vertical slice for US1.

## Repeated Capture Stability

- Command: 20 repeated runs of
  `LD_LIBRARY_PATH=/home/developer/.nuget/packages/skiasharp.nativeassets.linux/4.147.0-preview.3.1/runtimes/linux-x64/native:$LD_LIBRARY_PATH dotnet fsi specs/026-working-screenshot-taking/readiness/fsi/screenshot-smoke.fsx`
- Transcript summary: `specs/026-working-screenshot-taking/readiness/fsi/screenshot-repeat.txt`
- Runs: 20
- Accepted artifacts: 20
- Pass rate: 100%

Result: meets SC-001's 95% threshold for the stable graphical smoke sample.
