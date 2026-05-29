# Runtime Limitations

Status: complete.

Task: T054
Captured: 2026-05-29T12:20:00+02:00

Out-of-scope items remain deferred:

- browser screenshot capture
- mobile screenshot capture
- release publishing/package distribution
- new Bomberman gameplay
- renderer replacement or Vulkan redesign
- platform expansion beyond current Windows/Linux scope

Current host notes:

- Generated screenshot evidence succeeded with a live viewer render-target PNG.
- Generated default launch stayed alive until timeout and emitted GTK module warnings only.
- GTK module warnings are preserved in `readiness/generated-persistent-launch.log` and are not converted into a success artifact by themselves.

Runtime contract terms:

- .NET 10 desktop: target runtime for this feature.
- SkiaSharp preview: existing renderer dependency family remains in use.
- unsupported macOS/mobile/browser: no new macOS, mobile, or browser screenshot support is claimed.
- no software-renderer fallback: unsupported host evidence must stay explicit and must not be converted into a false success via a hidden software-renderer fallback.
