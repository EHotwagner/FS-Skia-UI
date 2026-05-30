# Contract: Generated FSI Load Script

Covers FR-009. Defines the generated `.fsx` load script emitted with a generated
application and the rule that keeps it in sync with the assembly set.

## What is emitted

Alongside the generated `Product` app, the template emits a single `.fsx` load
script in the generated product root. Running it in FSI (`dotnet fsi <script>`
or `#load`-ing it from another script) loads the app plus its transitive
`FS.Skia.UI.*` references without the author enumerating any reference by hand.

Shape (illustrative — exact references are derived, not hand-written):

```fsharp
// GENERATED — do not edit. Regenerated from Directory.Packages.props.
// Loads the Product app and its transitive FS.Skia.UI references for FSI.
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "src/Product/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
// ... remaining transitive FS.Skia.UI.* assemblies ...
#r "src/Product/bin/Debug/net10.0/Product.dll"
open Product
```

## In-sync derivation (FR-009)

- The reference set is **derived** from the generated `Directory.Packages.props`
  pinned versions plus the generated `Product` output assembly, confirmed
  against `tests/Product.Tests/obj/project.assets.json` after restore.
- The script is regenerated whenever the app is generated; it is NOT a
  hand-maintained reference list. When the app's assembly set changes, the
  script changes with it (FR-009, US4 scenario 2).
- A `// GENERATED — do not edit` banner makes the regeneration contract visible.

## Benign host-warning preservation

Loading in a headless/unsupported host MUST keep benign host-warning
classification intact (spec 021 host-warning contract):

- Known benign environment warnings (e.g. GTK `colorreload-gtk-module` /
  `window-decorations-gtk-module` load failures) remain classified benign **only**
  when load and first frame/render succeed.
- Real `LaunchFailure` / `RenderingFailure` / `LayoutFailure` / `PackageFailure`
  / artifact-write failures remain fatal and are never suppressed by the load
  script.

## Validation

- `GeneratedProductCheck` — the `.fsx` is present in the generated file list and
  its `#r` set matches the resolved package/assembly set; framework-only files
  remain absent.
- `GeneratedGuidanceCheck` — generated `README.md` / `docs/product.md` document
  the single load step (FR-009, SC-005) and do not recommend
  assembly-reflection or repo-source inspection.
- Evidence: real FSI load transcript for a freshly generated app under
  `readiness/fsi-load-script.md`.

## Documentation entry point

Generated `README.md` / `docs/product.md` describe one copy-pasteable step:
"run the generated load script in FSI", with no manual transitive-reference
edits (SC-005).
