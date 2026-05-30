#r "nuget: FS.Skia.UI.Scene, 0.1.35-preview.1"
#r "nuget: FS.Skia.UI.Controls, 0.1.34-preview.1"
#r "nuget: FS.Skia.UI.Controls.Elmish, 0.1.34-preview.1"

open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish

type Msg =
    | Changed of string

let text: Control<Msg> =
    FS.Skia.UI.Controls.TextBlock.create [
        FS.Skia.UI.Controls.TextBlock.text "Package-shaped"
    ]

let input: Control<Msg> =
    FS.Skia.UI.Controls.TextBox.create [
        FS.Skia.UI.Controls.TextBox.onChanged Changed
    ]

let grid: Control<Msg> =
    FS.Skia.UI.Controls.DataGrid.create [] []

let adapterDiagnostic =
    ControlsElmish.diagnostic "package-authoring" "sample" "Controls.Elmish adapter reached"
