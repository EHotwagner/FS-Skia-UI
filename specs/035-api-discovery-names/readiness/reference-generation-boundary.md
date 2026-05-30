# Reference Generation Boundary

## Inputs

Reference generation is repository-owned and uses curated `.fsi` files only.
The initial package input map is:

| Package id | Curated `.fsi` inputs |
|------------|-----------------------|
| `FS.Skia.UI.Scene` | `src/Scene/Scene.fsi` |
| `FS.Skia.UI.SkiaViewer` | `src/SkiaViewer/SkiaViewer.fsi` |
| `FS.Skia.UI.Elmish` | `src/Elmish/Elmish.fsi` |
| `FS.Skia.UI.KeyboardInput` | `src/KeyboardInput/KeyboardInput.fsi` |
| `FS.Skia.UI.Layout` | `src/Layout/Layout.fsi`, `src/Layout/Types.fsi`, `src/Layout/Graph.fsi`, `src/Layout/GraphValidation.fsi` |
| `FS.Skia.UI.Controls` | `src/Controls/*.fsi` |
| `FS.Skia.UI.Controls.Elmish` | `src/Controls.Elmish/ControlsElmish.fsi` |
| `FS.Skia.UI.Testing` | `src/Testing/Testing.fsi` |

Compiled assembly reflection is not an authoring input. Repository source
implementation files are not an authoring fallback.

## Outputs

Deterministic package-adjacent outputs are written under:

- `specs/035-api-discovery-names/readiness/package/api-reference/index.md`
- `specs/035-api-discovery-names/readiness/package/api-reference/<PackageId>.md`
- `specs/035-api-discovery-names/readiness/package/api-reference/report.json`

Packaging may later include the generated Markdown in `.nupkg` artifacts, but
the validation boundary is package-adjacent until T014 wires packaging behavior.

## Report Schema

Each package entry records:

- `package-id`
- `package-version`
- `source-fsi-paths`
- `reference-output-path`
- `symbol-count`
- `sampled-symbols`
- `xml-summary-count`
- `unsupported-symbols`
- `omitted-symbol-reasons`
- `diagnostics`
- `assembly-reflection: false`
- `repository-source-authoring-fallback: false`

Markdown output mirrors the same fields and includes source-shaped excerpts for
types, records, union cases, modules, values, parameter labels, return shapes,
and XML summaries where present.

## FAKE Boundaries

Reference generation belongs at FAKE/script/test boundaries:

- `PackageSurfaceCheck` validates generated reference files and surface
  baselines.
- `FsiTranscripts` validates public authoring transcripts that consume package
  references.
- `PackLocal` or a package-adjacent target may refresh reference artifacts for
  local package validation.
- `GeneratedGuidanceCheck` validates that generated guidance points to the
  package reference before reflection or repository source inspection.

No runtime dependency is added. If a parser or documentation dependency becomes
necessary, it must be pinned in `Directory.Packages.props`, documented in
`docs/dependencies.md`, covered by `DependencyReport`, and reviewed for template
package impact before implementation.
