# Native startup / cleanup (FR-012)

**Verdict:** PASS — lifetime/behaviour unchanged after the move.

The `VulkanResources` (resource-ownership ledger) and `VulkanStartup` (stage ordering + reverse
cleanup) modules travelled with the host into `src/SkiaViewer/Host/Vulkan.fs(i)` (namespace
`FS.Skia.UI.SkiaViewer.Host`). The startup/cleanup test moved with them into the SkiaViewer test
surface: `tests/SkiaViewer.Tests/NativeStartupCleanupTests.fs` (repointed `open
FS.Skia.UI.SkiaViewer.Host`).

- Command: `dotnet test tests/SkiaViewer.Tests`
- Result (within `Dev`): `Passed! - Failed: 0, Passed: 48` (includes the native startup/cleanup
  acquisition-order + reverse-release + idempotent-shutdown assertions).
- The host's structured diagnostics (`RenderDiagnostic`, 9-stage `DiagnosticStage`, `Diagnostics`
  module) travel unchanged and still fail fast with actionable context (Principle VII).
