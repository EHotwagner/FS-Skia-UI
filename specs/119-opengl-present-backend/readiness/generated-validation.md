# Generated Validation (feature 119)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Route output

`./fake.sh build -t Route` was run against the working-tree diff. Because the diff changes the
public `FS.Skia.UI.SkiaViewer` `.fsi` (the `Host/Vulkan.fsi` → `Host/OpenGl.fsi` replacement, the
reconciled diagnostic DUs, and the re-documented `ViewerPresentMode`), the dependency manifest, and
governance/constitution paths, Route escalates to the **package-surface** tier and prints
`Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedGuidanceCheck,
TemplateDrift, EvidenceGraph, EvidenceAudit`. Only the printed gates are run, sequentially (shared
`.fake` state). See `governance-risk-levels.md`.

## Why the generated project is unaffected at default

Generated products consume only the **source-stable** high-level entry points
(`runInteractiveApp` / `runInteractiveViewer` / `ViewerOptions` / `Viewer.runApp`), which are
unchanged. The only generated-product change is governance-token: the generated
`runtime-limitations.md` seed flips "Vulkan backend required" → "OpenGL backend required"
(`GeneratedProduct.fs`), checked by `GeneratedProductCheck` / `GeneratedGuidanceCheck`. No change
to default/minimal generated contents, selected Controls guidance, or local skills.

## Template pin lag (deferred, expected)

The `dotnet new fs-skia-ui` template pin is a **separate follow-up track** (`/fs-skia-template-update`),
not in this feature's merge scope. `TemplateCheck` / `GeneratedProductCheck` may show the known
pin-lag against the prior package version until the bumped libs are packed and the template re-pin
follow-up runs. package-resolution=resolved for the repo-built packages.
