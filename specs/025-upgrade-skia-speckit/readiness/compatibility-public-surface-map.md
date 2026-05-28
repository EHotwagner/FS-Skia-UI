# Compatibility Public Surface Map

Evidence:

- Baseline: `readiness/surface-baselines/FS.Skia.UI.txt`
- Focused baselines: `readiness/surface-baselines/FS.Skia.UI.*.txt`
- Pre-upgrade command: `./fake.sh build -t PackageSurfaceCheck`
- Pre-upgrade log: `specs/025-upgrade-skia-speckit/readiness/logs/pre-package-surface-check.log`

| Public area | Classification | Focused equivalent or gap | Decision | Evidence |
|-------------|----------------|---------------------------|----------|----------|
| Scene primitives (`Color`, `Point`, `Rect`, `Scene`, `Paint`, path/image/text shapes) | duplicate | `FS.Skia.UI.Scene` | Keep compatibility surface stable; prefer focused package for new docs. | `readiness/surface-baselines/FS.Skia.UI.txt`, `readiness/surface-baselines/FS.Skia.UI.Scene.txt` |
| Viewer program, Vulkan startup, swapchain/resource contracts | primary-only | Partial focused host in `FS.Skia.UI.SkiaViewer`, but Vulkan presenter remains broad-package implementation | Permanent compatibility surface for this upgrade. | `src/Lib/Library.fsi`, `src/SkiaViewer/SkiaViewer.fsproj` |
| Keyboard compatibility types | duplicate/deprecated candidate | `FS.Skia.UI.KeyboardInput` | Keep broad surface; steer new authoring to focused package. | `src/Lib/KeyboardInput.fsi`, `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt` |
| Diagnostic and parity helper types | primary-only | no complete focused equivalent | Keep until a future compatibility feature creates replacements. | `readiness/surface-baselines/FS.Skia.UI.txt` |
| Broad package identity `FS.Skia.UI` | permanent compatibility surface | focused package set for new products | Preserve package identity and no `.fsi` change in this upgrade. | `PackageSurfaceCheck` pre-upgrade pass |
| Potential delegation from broad scene concepts to focused concepts | facade candidate | `FS.Skia.UI.Scene` | Defer; type identity and package cycles require separate design. | compatibility analysis doc |

No public-surface removal, facade conversion, or deprecation attribute is
approved by this upgrade.

