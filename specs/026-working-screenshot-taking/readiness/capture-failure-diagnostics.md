# Capture Failure Diagnostics

Status: pending implementation.

This file will record real unsupported-host or failed-capture attempts with the
earliest blocked stage, classification, category, host facts, attempted command,
message, diagnostics, and an explicit `proves-screenshot=false` outcome.

## Initial Diagnostics

No unsupported-host outcome was observed during the initial FSI smoke run. The
supported viewer render-target path produced
`specs/026-working-screenshot-taking/readiness/artifacts/working-screenshot-record.png`.
Explicit unsupported/failure command evidence remains pending US3.

## Invalid Capture Request Diagnostic

- Command: `dotnet fsi specs/026-working-screenshot-taking/readiness/fsi/screenshot-failure-smoke.fsx`
- Transcript: `specs/026-working-screenshot-taking/readiness/fsi/screenshot-failure-smoke.txt`
- Status: `ScreenshotFailed`
- Blocked stage: `Capture`
- Classification: `ProductDefect`
- Category: `Screenshot`
- Proves screenshot: `false`
- Artifact path: `None`
- Message: `Screenshot evidence request validation failed.`

This is a real failed command attempt through the public SkiaViewer screenshot
entry point. It does not manufacture a successful screenshot artifact.
