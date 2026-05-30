#r "nuget: FS.Skia.UI.Scene, 0.1.35-preview.1"
#r "nuget: FS.Skia.UI.KeyboardInput, 0.1.34-preview.1"
#r "nuget: FS.Skia.UI.SkiaViewer, 0.1.36-preview.1"

open FS.Skia.UI.Scene
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.SkiaViewer

let options: ViewerOptions =
    { Title = "Package authoring"
      InitialSize = { Width = 640; Height = 480 } }
let position = ViewerWindowPosition.Coordinates(x = 40, y = 60)
let (keyboard: KeyboardModel), keyboardEffects = Keyboard.init []
let down = KeyboardMsg.KeyDown "Enter"
let up = KeyboardMsg.KeyUp "Enter"
