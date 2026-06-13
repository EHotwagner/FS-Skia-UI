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

- **OpenGL backend (feature 119).** The rendering backend was migrated from Vulkan to OpenGL in feature 119 (`119-opengl-present-backend`); this runtime envelope is updated so governance no longer asserts a backend that no longer exists (FR-010). The historical Vulkan context above is retained for provenance.
