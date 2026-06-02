# Persistent viewer launch / first frame (FR-009 / SC-005, Principle V)

The repointed `samples/BasicViewer` now builds and links against `FS.Skia.UI.Scene` +
`FS.Skia.UI.SkiaViewer` (host via `FS.Skia.UI.SkiaViewer.Host`) and restores/builds green
(`SampleContractSmoke` passed within `Dev`). Its default executable drives the moved host through
`Host.Viewer.create/run`.

**Persistent visible-window first-frame capture is infeasible in this headless environment
(disclosed, not faked).** A persistent visible window rendering a first frame requires a
GPU-passthrough desktop session; the known `SkiaViewer.Tests` libdecor-gtk headless crash and the
lack of guaranteed GPU passthrough block a reliable persistent-window observation here. Per Principle
V this is recorded with the unsupported-host diagnostic rather than substituted with a metadata-only
stand-in. The viewer host's run edge (`ViewerProgram -> Result<unit, RenderDiagnostic>`) is exercised
for shape in `readiness/fsi/skiaviewer-host.txt`; deterministic scene-output is the authoritative
behaviour oracle.

Required host: Windows/Linux desktop session with Vulkan GPU passthrough.
