# Runtime Limitations

This cleanup preserves the existing runtime support boundary.

- Supported development validation targets .NET 10 desktop hosts.
- Rendering remains based on Vulkan-capable SkiaSharp preview dependencies
  where the existing viewer stack requires them.
- Unsupported macOS/mobile/browser hosts remain explicit unsupported evidence
  classifications rather than screenshot or launch success.
- There is no software-renderer fallback added by this feature.
- Host limitations are reported as environment or unsupported-host evidence;
  they are not treated as product success for visual proof.
