# Runtime Limitations

- Supported Linux desktop hosts with common Latin fonts are expected to produce readable default text screenshot evidence.
- Unsupported hosts must report `unsupported-host-reason`, blocked stage, classification, category, and next action without claiming readable default text or accepted interactive-window evidence.
- Browser, mobile, macOS support expansion, release publishing, and new gameplay are deferred.
- Runtime target: .NET 10 desktop generated apps.
- Renderer stack: Vulkan startup and SkiaSharp preview packages are the current graphics path.
- Fallback limitation: there is no software-renderer fallback for persistent viewer readiness.
- Unsupported macOS/mobile/browser scope remains out of this feature.
