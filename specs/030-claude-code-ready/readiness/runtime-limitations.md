# Runtime Limitations

Product runtime behavior, renderer behavior, visual output, gameplay, package publishing, user-local Claude preferences, enterprise managed policy, browser/mobile support, and release distribution are out of scope for this feature.

Runtime constraints inherited from the repository:

- .NET 10 desktop is the target runtime for generated product validation.
- Vulkan and SkiaSharp preview behavior are not changed by this feature.
- There is no software-renderer fallback introduced here.
- Unsupported macOS/mobile/browser targets remain outside this feature.
- Generated project Claude Code readiness is file and workflow readiness, not new renderer or app behavior.
