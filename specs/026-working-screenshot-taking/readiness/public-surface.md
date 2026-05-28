# Public Surface

Status: intentional public surface change.

Changed packages:

- `FS.Skia.UI.SkiaViewer` 0.1.27-preview.1 adds screenshot request/result
  traceability fields, capture mode, pixel validation, blocked-stage details,
  and explicit workflow effects for artifact validation and cleanup.
- `FS.Skia.UI.Testing` 0.1.27-preview.1 adds screenshot record parsing and PNG
  artifact validation contracts.

Evidence:

- `specs/026-working-screenshot-taking/readiness/fsi/screenshot-contracts.txt`
- `specs/026-working-screenshot-taking/readiness/package-surface-baseline.md`
- `dotnet test tests/Package.Tests/Package.Tests.fsproj --no-restore`: PASS
