# Runtime Limitations

Runtime scope:

- .NET 10 desktop
- Vulkan-backed desktop presentation
- SkiaSharp preview packages

Unsupported scope:

- unsupported macOS/mobile/browser environments
- no software-renderer fallback
- no new platform support in this feature

Unsupported-host diagnostics can explain why persistent desktop evidence cannot
be collected on a specific host. They do not replace supported-host persistent
interactive launch evidence when a readability claim depends on visible desktop
window behavior.
