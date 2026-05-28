# Runtime Limitations

Status: recorded.

- Target runtime: .NET 10 desktop hosts that can run the existing viewer-backed
  graphical path.
- Graphics stack: existing SkiaSharp preview and Silk.NET/Vulkan dependencies.
- Unsupported scope: unsupported macOS/mobile/browser capture, broad new desktop platform
  support, and software-renderer fallback unless separately planned.
- Evidence rule: an unsupported host can produce real negative evidence, but it
  cannot satisfy screenshot success.

The implemented path uses the existing SkiaSharp preview render-target PNG
writer and preserves the no software-renderer fallback constraint.
