// US3 (spec 037, FR-007 / SC-004): mixed Scene/Controls authoring in the
// previously-failing open order (Scene first, Controls LAST). It constructs an
// unqualified scene `Text` node plus a shared bounds literal.
//
// Pre-fix: the unqualified `Text` resolves to the leaked `ControlEventOrigin.Text`
// case (nullary), so applying it to arguments yields the opaque
//   "This value is not a function and cannot be applied" / "has type ControlEventOrigin"
// error that pointed nowhere near the cause.
//
// Post-fix ([<RequireQualifiedAccess>] on ControlEventOrigin): the case no longer
// leaks into the opened Controls namespace, so `Text (...)` resolves to the Scene
// text constructor and the file compiles.

#I __SOURCE_DIRECTORY__
#r "../../../../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "../../../../src/Controls/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"

open FS.Skia.UI.Scene
open FS.Skia.UI.Controls

// FR-008: reuse the shared Scene bounds type for the bounds literal rather than a
// look-alike record, so record-field inference does not hijack resolution.
let bounds: FS.Skia.UI.Scene.Rect =
    { X = 0.0; Y = 0.0; Width = 240.0; Height = 80.0 }

// Unqualified scene text node — the collision point.
let node =
    Text((bounds.X, bounds.Y), "Hello, scene", { Red = 24uy; Green = 88uy; Blue = 128uy; Alpha = 255uy })

printfn "scene text node resolved: %A" node
