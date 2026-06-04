# Runtime limitations

## Sandbox aggregate-target bootstrap

The aggregate `Verify`/`Ci` umbrella targets cannot bootstrap the `dotnet-fake` global
tool in this sandbox, so the aggregate handoff verdict reads `degraded`. Every constituent
gate that `./fake.sh build -t Route` prints is run **individually and sequentially**; the
authoritative merge gate is `EvidenceAudit`. This is a non-authoritative aggregate
limitation, not a gate failure (see [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md)).

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux, renders
through **Vulkan**, and depends on a **SkiaSharp preview** native build. Platforms remain
**unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**. The
renderer fix (FR-001/002) changes none of that — it shares one painter (`SceneRenderer`)
across the interactive Vulkan path and the **raster `SKBitmap`** image-evidence path, which
needs no GPU or window system.

## Unsupported scope handling (this feature)

- The feedback extension is **not** installed in this repo, so the `LunarLander1`
  consumer-friction records (LL-1…LL-9) were produced in the consumer; this feature
  verifies the framework fixes locally and in a generated project.
- The renderer fix draws onto a raster `SKBitmap` canvas, so no GPU/window-system
  dependency is required for the image-evidence path. The interactive Vulkan path is
  unchanged in coverage (only refactored to delegate to the shared painter).
- `wrapDeltaX` is a pure value-type utility — no I/O, no host runtime, no platform
  dependency.
