# Parity reference frame — corroboration (FR-008, Principle V)

Scene-output (`parity-scene-output-diff.md`) is the **authoritative** parity oracle and is clean
(0-byte, all three seeds). The reference rendered frame
(`tests/Parity.Tests/fixtures/v3-host-golden/screenshots/basic-viewer.png`) is corroboration only.

**Headless capture infeasible in this environment (disclosed, not faked).** Capturing a new
`basic-viewer` frame from the moved host requires a GPU-passthrough desktop session driving the
Vulkan/Skia swapchain; the known `SkiaViewer.Tests` libdecor-gtk headless flake and the absence of
guaranteed GPU passthrough make a fresh byte-comparable frame capture unreliable here. Per Principle
V this is recorded rather than substituted with a fake frame. The deterministic scene-output oracle
(which does not touch the GPU) is byte-identical and gates deletion; the moved host's `drawScene`
preserves the legacy Skia draw calls and absorbs the former `SceneConversion` mapping for
`Circle`/`FilledEllipse` (oval-over-bounds), so the rendered frame is behaviour-preserving by
construction.

Required host for a literal frame re-capture: Windows or Linux desktop session with Vulkan GPU
passthrough (`DISPLAY` + working swapchain), matching `capture-environment.md`.
