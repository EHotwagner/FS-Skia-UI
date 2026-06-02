# Runtime limitations

- authoritative command: `./fake.sh build -t Dev` (build + all semantic/parity suites)
- artifact path: `specs/050-v3-host-extraction/readiness/logs/dev.log`
- failure class: UnsupportedEnvironment vs ProductDefect (the host classifies startup failures via its
  9-stage `DiagnosticStage`).
- next action: for a headless GUI flake, re-run the focused suite; scene-output parity is authoritative.

Statements:

- Target platform is .NET 10 desktop (`net10.0`); the viewer host targets **Windows and Linux** only.
- Rendering is **Vulkan** via **SkiaSharp preview** native assets with **no software-renderer fallback**;
  startup fails fast with a structured `RenderDiagnostic` when Vulkan is unavailable.
- **unsupported macOS/mobile/browser**: macOS, mobile, and browser hosts are not supported for the
  persistent viewer host.
- Headless CI cannot guarantee a GPU-passthrough desktop session; persistent-window and reference-frame
  capture are recorded as infeasible (Principle V) where passthrough is absent, with deterministic
  scene-output as the authoritative oracle.
