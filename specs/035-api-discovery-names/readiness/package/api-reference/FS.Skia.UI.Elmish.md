# FS.Skia.UI.Elmish Source-Shaped API Reference

package-id: FS.Skia.UI.Elmish
package-version: local
generated-from: curated-fsi
assembly-reflection: false
repository-source-authoring-fallback: false
symbol-count: 19
xml-summary-count: 6
source-fsi-paths:
- src/Elmish/Elmish.fsi
sampled-symbols:
omitted-symbol-reasons:
- none
unsupported-symbols:
- none
diagnostics:
- none

## Common Samples

## Curated Signatures
```fsharp
namespace FS.Skia.UI.Elmish

open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

/// Public contract type exposed by this FS.Skia.UI package.
type ElmishAdapterModel<'model> =
    { UserModel: 'model
      Scene: SceneNode
      Viewer: ViewerModel }

/// Public contract type exposed by this FS.Skia.UI package.
type ElmishAdapterMsg<'msg> =
    | UserMsg of 'msg
    | ViewerMsg of ViewerMsg

/// Public contract type exposed by this FS.Skia.UI package.
type ElmishAdapterEffect<'msg> =
    | DispatchUser of 'msg
    | DispatchViewer of ViewerEffect

/// Public contract module exposed by this FS.Skia.UI package.
module ElmishAdapter =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init:
        viewerOptions: ViewerOptions ->
        userModel: 'model ->
        scene: SceneNode ->
            ElmishAdapterModel<'model> * ElmishAdapterEffect<'msg> list

    /// Public contract function exposed by this FS.Skia.UI package.
    val update:
        render: ('model -> SceneNode) ->
        msg: ElmishAdapterMsg<'msg> ->
        model: ElmishAdapterModel<'model> ->
            ElmishAdapterModel<'model> * ElmishAdapterEffect<'msg> list

```
