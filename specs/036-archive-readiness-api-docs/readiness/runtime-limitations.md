# Runtime Limitations

This feature does not expand runtime support.

- .NET 10 desktop remains the target runtime context for repository tooling and generated products.
- Vulkan host availability remains environment-dependent.
- SkiaSharp preview behavior is unchanged.
- unsupported macOS/mobile/browser targets remain unsupported for this repository workflow.
- no software-renderer fallback is introduced.

Archive readiness and API-reference evaluation are documentation and governance work. Runtime rendering, screenshots, desktop host behavior, persistent windows, and visual evidence are out of scope unless a future feature changes the public runtime contract.

- **OpenGL backend (feature 119).** The rendering backend was migrated from Vulkan to OpenGL in feature 119 (`119-opengl-present-backend`); this runtime envelope is updated so governance no longer asserts a backend that no longer exists (FR-010). The historical Vulkan context above is retained for provenance.
