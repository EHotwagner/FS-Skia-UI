# Runtime Limitations

Status: pass.

This feature is package-discovery and generated-guidance work. Runtime
rendering behavior is explicitly out of scope.

Known runtime boundaries:

- .NET 10 desktop validation is limited to compile, package, template,
  generated-product, and FSI authoring checks for this feature.
- Vulkan runtime behavior is not changed or validated by this feature.
- SkiaSharp preview runtime behavior is not changed or validated by this
  feature.
- unsupported macOS/mobile/browser targets remain unsupported for this
  package-consumer validation path.
- no software-renderer fallback is introduced by this feature.

The clean package-consumer validation proves package authoring and build
surface, not live desktop rendering, screenshot capture, input dispatch, or
viewer lifecycle behavior.
