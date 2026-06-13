# Direct-mode smoke (feature 118, US1)

**Authoritative command:** build `specs/118-backend-host-review/readiness/live-host/LiveHost.fsproj`
(references the local `FS.Skia.UI.SkiaViewer`), then:

```
FEATURE118_MODE=direct \
  FEATURE118_SHOT=specs/118-backend-host-review/readiness/smoke/direct-frame.png \
  dotnet specs/118-backend-host-review/readiness/live-host/bin/Debug/net10.0/LiveHost.dll
```

**Environment:** real AMD Radeon (RADV RENOIR) Vulkan 1.4 GPU, X11 display `:1`. A real
persistent window opens and presents live frames through the production path
`Host.Viewer.run → VulkanHost.run → renderFrame`.

**Observed result (real backend):**
```
MODE: direct
DIAG: Warning VulkanSwapchain SkiaSharp cannot wrap a Vulkan swapchain image as an SKSurface
      (managed-binding limitation, mono/SkiaSharp #1502); DirectToSwapchain is unavailable and
      the viewer uses the OffscreenReadback present path.
CAPTURE: specs/118-backend-host-review/readiness/smoke/direct-frame.png
RESULT: ok frames=40 captured=true diagnostics=1
```

**What it proves:**
- The windowed viewer launches in `DirectToSwapchain` mode against a real Vulkan backend and
  presents 40 live frames (`RESULT: ok frames=40`).
- `DirectToSwapchain` detects the SkiaSharp wrap limitation **once at init** (`probeDirectWrap`)
  and degrades to the proven `OffscreenReadback` present path — exactly one `Warning`
  diagnostic, no crash, no aborted run, no corrupt frame (FR-005).
- On-demand screenshot capture works in direct mode (`captured=true`); the artifact decodes
  (480×320 RGBA PNG).

**What it does NOT prove (honest disclosure):** the readback-free direct present (FR-002/SC-002)
is **not** exercised — it is blocked upstream (SkiaSharp #1502/#2191, see
`../audit/present-path-audit.md`). The on-screen content equals the captured pixels because both
modes present via the readback path on this SkiaSharp build.

- **Failure class:** a missing/unreadable backend or a wrap that unexpectedly succeeds-then-fails
  mid-frame both degrade to `OffscreenReadback` with a Warning (FR-005); a hard backend failure
  classifies as `UnsupportedEnvironment` (not a product defect).
- **Next action:** the readback-free path lands with the OpenGL present backend
  (`../audit/opengl-backend-resolution.md`).
