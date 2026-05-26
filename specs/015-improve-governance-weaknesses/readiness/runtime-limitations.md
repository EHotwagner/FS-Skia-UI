# Runtime Limitations

This governance feature documents current runtime boundaries without claiming
new platform support.

- Runtime platform: .NET 10 desktop.
- Renderer boundary: Vulkan remains the current renderer path.
- Dependency maturity: SkiaSharp preview dependencies remain part of the
  current support risk.
- Unsupported targets: unsupported macOS/mobile/browser targets are roadmap
  work, not support added by this feature.
- Fallback boundary: no software-renderer fallback is added.
- Package/API/runtime support expansion: no package/API/runtime support expansion
  is included in this feature.
- Toolchain boundary: validation remains focused on governance scripts,
  generated guidance, and readiness artifacts unless the final risk level
  requires broad product validation.
