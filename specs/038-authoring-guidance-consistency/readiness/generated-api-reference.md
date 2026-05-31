# Generated Local API Reference (US2, FR-004, SC-002)

A freshly generated project bundles the real public `.fsi` signatures for every
package its profile consumes, under `docs/api-surface/`, derived verbatim at
generation time from `src/.../*.fsi` (mechanical copy, never hand-maintained).
Bundling is implemented by `copyApiSurface` in `build.fsx` (called from
`generateV3Product`); the per-package contract set comes from
`template/capabilities.yml` `contracts:`.

## Bundled tree (app profile, generated)

`artifacts/generated-products/038-authoring-guidance-consistency/app-source/docs/api-surface/`
mirrors the source package directory to avoid same-named collisions
(`Controls/Types.fsi` vs `Layout/Types.fsi`):

```
docs/api-surface/Scene/Scene.fsi
docs/api-surface/SkiaViewer/SkiaViewer.fsi
docs/api-surface/Elmish/Elmish.fsi
docs/api-surface/KeyboardInput/KeyboardInput.fsi
docs/api-surface/Layout/Layout.fsi, Types.fsi, Graph.fsi, GraphValidation.fsi
docs/api-surface/Controls/Accessibility.fsi … Types.fsi (14 files)
```

## Reflection-free union-case field order (SC-002)

Read directly from the bundled `docs/api-surface/Scene/Scene.fsi`, with zero DLL
reflection:

```fsharp
type SceneNode =
    ...
    | Rectangle of (float * float * float * float) * Color
    | PaintedRectangle of Rect * Paint
    ...
    | Text of (float * float) * string * Color
```

An author determines `SceneNode.Rectangle`'s exact field order
(`(float * float * float * float) * Color`) locally.

## Drift / completeness enforcement (FR-004)

`scanV3GeneratedRow` (run by `GeneratedProductCheck`) fails loudly if a consumed
package's signatures are absent from the generated tree or differ byte-for-byte
from the source `.fsi`. Non-runtime capabilities whose contract is the
`no-public-surface` sentinel are skipped. Validated green by
`./fake.sh build -t GeneratedProductCheck` across the app, headless-scene,
governed, and sample-pack profiles — see `logs/generated-product-check.txt`.
