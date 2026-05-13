# Final Readiness Review

Hard parity gate:

- `readiness/parity-evidence.json` contains one item per pinned baseline capability.
- Every non-conflicting baseline capability is `Supported` or `Adapted`.
- Accepted caveat: synthetic propagation remains from T014 diagnostic fixtures and T077 missing Windows smoke evidence.

Package boundaries:

- `FS.Skia.UI` packs independently.
- `FS.Skia.UI.Charts` packs independently and references core.
- `FS.Skia.UI.Layout` packs independently and references core.

Renderer and state constraints:

- Vulkan-only: no fallback renderer selector is exposed.
- Elmish-only: viewer samples use `Model`, `Msg`, `update`, `view`, and `ViewerEffect`.

Runnable samples:

- BasicViewer
- InteractiveViewer
- ParityGallery
- EffectsGallery
- ChartsGallery
- DataGridGallery
- LayoutGraphGallery
- ScreenshotGallery
- DemoReel
