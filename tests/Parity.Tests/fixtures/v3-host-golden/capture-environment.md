# V3 host parity oracle — capture environment (FR-005)

Scene-output is the **authoritative** parity oracle; the reference screenshots in
`screenshots/` are **corroboration only**. This file records the capture environment so a
screenshot mismatch is attributable to environment rather than regression.

## Pin

- Baseline SHA: `031e56072779c736adf6dd8b0345e17b58a62e73`

## Environment

| Field | Value |
|-------|-------|
| OS | Linux 7.0.10-arch1-1 x86_64 (Arch Linux) |
| GPU / driver | AMD/ATI Cezanne (Radeon Vega Series), GPU passthrough |
| Vulkan | present (`vulkaninfo` available); SkiaSharp viewer renders through Vulkan |
| Display | `DISPLAY=:1`, `WAYLAND_DISPLAY=wayland-0` |
| .NET / toolchain | .NET SDK 10.0.300, `net10.0` target |
| Capture date | 2026-06-02 |

## Authoritative oracle — deterministic scene-output (SC-003)

`scene-output/<seed>.txt` for the three closed seeds — `basic-viewer`,
`effects-gallery`, `screenshot-gallery` — captured from the current host's `Scene`
values via the deterministic encoder (`ParitySceneOutput.encode`: element kinds from
`Scene.describe`, `Scene.diagnostics`, and `Scene.renderReadbackEvidence` including its
environment-independent `DeterministicHash`). The encoding is fixed and versioned with the
fixture (`format: scene-output/v1`); it contains no timestamps or environment-dependent
fields and re-derives **byte-identically** (0-byte diff). Re-derive + assert:

```bash
dotnet test tests/Parity.Tests/Parity.Tests.fsproj --filter "FullyQualifiedName~scene-output"
```

## Corroboration — reference screenshots

- `screenshots/basic-viewer.png` — **real Vulkan-rendered frame** (640×480 RGBA PNG)
  captured from the current host. Command:

  ```bash
  dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- \
    --screenshot-smoke --output=tests/Parity.Tests/fixtures/v3-host-golden/screenshots/basic-viewer.png
  ```

  The Vulkan path renders and writes the PNG, confirming GPU-passthrough capture works in
  this environment; the windowed program then stays open and is terminated after the frame
  is written (it is a persistent viewer, not a one-shot).

- `effects-gallery` / `screenshot-gallery` reference frames are **deferred at the pin**:
  at this SHA those two galleries expose **no non-interactive screenshot entry point**
  (only `--contract-smoke` and an interactive windowed run that captures on a keypress).
  Capturing them would require either an interactive keypress or a sample-code change,
  which is **out of scope** for this record-and-oracle feature (FR-010/SC-007). Their
  parity is fully covered by the **authoritative** scene-output goldens above, which pass
  byte-identically for all three seeds.

## Failure-class note (corroboration screenshots)

A reference-screenshot mismatch is classified `UnsupportedEnvironment` (environment drift —
GPU/driver, Vulkan, display, or windowing) unless the **authoritative** scene-output also
drifts; only a scene-output drift indicates a `ProductDefect`. The known `SkiaViewer.Tests`
headless libdecor-gtk crash is an environment failure class on hosts without a working
Vulkan surface; it does not affect the authoritative scene-output oracle.
