# US4 Evidence — Generated FSI Load Script

Covers FR-009, SC-005. One documented step loads a generated app and its
transitive `FS.Skia.UI.*` references into FSI with **zero manual
transitive-reference edits**.

## Emission (in-sync, derived — not hand-maintained)

`generateV3Product` (build.fsx) emits `load-product.fsx` into each generated
product after the build, deriving the `#r` set by scanning the built Product
output assembly directory (the transitive FS.Skia.UI.* closure pinned by
`Directory.Packages.props`) plus `Product.dll`. `GeneratedProductCheck` (Status:
Ok) emitted it into all five generated products:

- artifacts/generated-products/037-authoring-audit-robustness/app-source/load-product.fsx
- artifacts/generated-products/037-authoring-audit-robustness/governed-source/load-product.fsx
- artifacts/generated-products/037-authoring-audit-robustness/sample-pack-source/load-product.fsx
- artifacts/generated-products/037-authoring-audit-robustness/app-package/load-product.fsx
- artifacts/generated-products/037-authoring-audit-robustness/headless-scene-source/load-product.fsx

It is registered as generated content (carried from `template/base/` by
`.template.config/template.json`) and required by the generated-product
file-list scan (`scanV3GeneratedRow`); it appears in every regenerated
`readiness/generated-file-lists/*.txt`.

## Generated app-source load-product.fsx (derived set)

```fsharp
// GENERATED — do not edit. Regenerated from Directory.Packages.props and the
// built Product output assembly. Loads the Product app and its transitive
// FS.Skia.UI.* references for FSI in one step:  dotnet fsi load-product.fsx
//
// This script only references and opens the app; it launches nothing, so it
// neither emits nor suppresses host warnings. A missing assembly is a real
// load failure that surfaces normally; benign host-warning classification
// (spec 021) is unaffected.
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.Controls.Elmish.dll"
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.Elmish.dll"
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "src/Product/bin/Debug/net10.0/Product.dll"
open Product
```

The reference set is the actual built transitive closure (Scene, Layout,
KeyboardInput, Elmish, SkiaViewer, Controls, Controls.Elmish, FS.Skia.UI) plus
`Product.dll` — derived, sorted, in sync with the assembly set.

## FSI load transcript (SC-005)

Run from the generated product root, no manual edits:

```
$ dotnet fsi load-product.fsx
exit=0
```

Exit 0: the app and its full transitive `FS.Skia.UI.*` set load with no
unresolved-transitive-reference error, and `open Product` resolves.

## Benign host-warning preservation (FR-009 / spec 021, T026)

The load script only `#r`s assemblies and `open`s `Product` — it launches
nothing, so it neither emits nor suppresses host warnings. A missing assembly
surfaces as a normal load failure; benign headless host-warning classification
(spec 021) is unaffected. This is stated in the GENERATED banner of every emitted
script.
