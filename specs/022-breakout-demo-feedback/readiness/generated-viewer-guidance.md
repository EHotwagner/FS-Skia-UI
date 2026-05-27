# Generated Viewer Guidance

Status: in progress.

package-version=FS.Skia.UI.SkiaViewer 0.1.21-preview.1; FS.Skia.UI.Scene 0.1.22-preview.1; FS.Skia.UI.Testing 0.1.22-preview.1
selected-entry-point=Viewer.runApp
selected-contract=Viewer.runApp viewerOptions Product.Program.generatedHost
files-scanned=template/base/src/Product/Program.fs;template/base/tests/Product.Tests/Tests.fs;template/base/docs/product.md;template/fragments/skiaviewer/README.md;docs/generated-apps.md;docs/testing.md;specs/022-breakout-demo-feedback/quickstart.md
deterministic-render-evidence=distinct command family: --scene-evidence / deterministic scene evidence
persistent-launch-evidence=distinct default launch contract: Viewer.runApp viewerOptions Product.Program.generatedHost
screenshot-evidence=distinct command family: --screenshot-evidence with explicit unsupported fallback when capture is unavailable

Required final evidence:

- package version used by generated projects
- selected persistent viewer entry point
- generated source, tests, docs, quickstart, and readiness files scanned
- deterministic render, persistent launch, and screenshot evidence reported as distinct evidence kinds
