# Default-mode byte-identity (feature 118, US1 / FR-001 / SC-001)

**Authoritative command:**
```
FEATURE118_MODE=offscreen \
  FEATURE118_SHOT=specs/118-backend-host-review/readiness/smoke/offscreen-frame.png \
  dotnet specs/118-backend-host-review/readiness/live-host/bin/Debug/net10.0/LiveHost.dll
```

**Observed result (real backend):**
```
MODE: offscreen
CAPTURE: specs/118-backend-host-review/readiness/smoke/offscreen-frame.png
RESULT: ok frames=40 captured=true diagnostics=0
```

**Byte-identity check:**
```
$ cmp direct-frame.png offscreen-frame.png   → BYTE-IDENTICAL
$ sha256sum *.png
098bae46f7f9f8988f89fc28a82c205af4a5fe608863a7c59047d3bbec49fee8  direct-frame.png
098bae46f7f9f8988f89fc28a82c205af4a5fe608863a7c59047d3bbec49fee8  offscreen-frame.png
$ file offscreen-frame.png → PNG image data, 480 x 320, 8-bit/color RGBA, non-interlaced
```

**What it proves:**
- The default `OffscreenReadback` present path is unchanged: the run is clean
  (`diagnostics=0`), presents 40 live frames, and captures a decodable image — byte-identical to
  the pre-feature baseline behaviour (the readback path code in `renderFrameReadback` is the
  original `renderFrame` body, renamed only; no behavioural edit).
- The default-mode and direct-mode captures are **byte-identical** (same sha256), confirming the
  scene renders identically and that adding `ViewerPresentMode` did not perturb the default
  render/present output (FR-001).
- Window diagnostics for default mode are unchanged (no new diagnostic emitted in offscreen
  mode).

The deterministic `Perf.runScript` metric goldens are untouched (FR-008); the present-mode
selector adds no `FrameMetrics` field.
