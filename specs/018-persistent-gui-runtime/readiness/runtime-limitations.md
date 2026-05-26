# Runtime Limitations

Runtime scope:

- .NET 10 desktop
- Vulkan-backed Silk.NET windowing
- SkiaSharp preview packages

Unsupported scope:

- unsupported macOS/mobile/browser environments
- no software-renderer fallback
- no new platform support in this feature

Unsupported-host diagnostics can supplement readiness when a desktop session is
not available, but they do not replace supported-host persistent interactive
launch evidence for final merge readiness.
