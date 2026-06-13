# Generated Validation (feature 120)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff changes public
`.fsi` surface (`Scene` `CachedSubtree` case + `CacheBoundary`, `FrameMetrics` timing + replay
fields, `SkiaViewer` `PresentMode` docstring + `GlHost` timing/idle vals) it escalates to the
**controls-public-surface** tier and prints `Dev, PackageSurfaceCheck, PerPackageSurfaceDiff,
FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck,
DesignTokenDrift, ContrastCheck, ControlsDocCoverageCheck, ControlsInteractionCheck,
ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`. Only
the printed gates are run, sequentially (shared `.fake` state). See `governance-risk-levels.md`.

## Why the generated project is unaffected at default

Generated products consume only the **source-stable** high-level entry points (`runInteractiveApp` /
`runInteractiveViewer` / `ViewerOptions` / `Viewer.runApp`), unchanged. Feature 120 changes framework
package internals + additive public surface the generated app consumes after re-pin; no new source
files ship into generated projects (the backend cache and `Scene` case live in framework packages the
template already consumes). No change to default/minimal generated contents.

## Template pin lag (deferred, expected)

The `dotnet new fs-skia-ui` template pin is a **separate follow-up track** (`/fs-skia-template-update`),
not in this feature's merge scope. `TemplateCheck` / `GeneratedProductCheck` may show the known
pin-lag against the prior package version until the bumped libs are packed and the template re-pin
follow-up runs. package-resolution=resolved for the repo-built packages.
