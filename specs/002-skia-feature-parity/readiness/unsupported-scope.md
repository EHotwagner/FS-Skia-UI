# Unsupported Scope Handling

Feature: `002-skia-feature-parity`

## Fallback Renderer

The pinned upstream baseline documents a Vulkan GPU backend with GL raster fallback. FS-Skia-UI intentionally does not implement that fallback. Any baseline capability that depends on OpenGL, software raster fallback, or runtime renderer switching is classified as `Excluded` or `Adapted` in parity evidence.

Required handling:

- Public configuration exposes no renderer selector.
- Startup diagnostics report Vulkan capability failures explicitly.
- Tests must assert that fallback renderer language is absent from public configuration and not suggested as a recovery path.

## Non-Elmish Integration

The pinned upstream baseline exposes observable scene/input streams. FS-Skia-UI intentionally exposes an Elmish-only public integration boundary.

Required handling:

- User state remains in the consumer `Model`.
- User events are represented by `ViewerEvent` and mapped to consumer `Msg`.
- I/O is represented by `ViewerEffect<'msg>` or `Cmd<'msg>` and interpreted at the application edge.
- Baseline observable-stream APIs are mapped to `Adapted` in parity evidence, not duplicated.
