module Product.Program

open FS.Skia.UI.Controls

type Model =
    { Name: string
      CanSave: bool }

type Msg =
    | NameChanged of string
    | SaveRequested

let controlsExampleView model =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Product controls" ]
            TextBox.create [
                TextBox.value model.Name
                TextBox.onChanged NameChanged
            ]
            Button.create [
                Button.text "Save"
                Button.enabled model.CanSave
                Button.onClick SaveRequested
            ]
        ]
    ]

[<EntryPoint>]
let main _ =
    let view = controlsExampleView { Name = "Product"; CanSave = true }
    printfn "Generated product controls: %d" (Control.count view)
    0
