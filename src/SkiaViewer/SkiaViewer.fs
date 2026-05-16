namespace FS.Skia.UI.SkiaViewer

open FS.Skia.UI.Scene

type ViewerOptions =
    { Title: string
      InitialSize: Size }

type ViewerModel =
    { Options: ViewerOptions
      IsRunning: bool
      LastScene: SceneNode option }

type ViewerMsg =
    | Start
    | Stop
    | Render of SceneNode

type ViewerEffect =
    | OpenWindow of title: string * size: Size
    | RenderScene of SceneNode
    | CloseWindow

module Viewer =
    let init options =
        { Options = options
          IsRunning = false
          LastScene = None },
        [ OpenWindow(options.Title, options.InitialSize) ]

    let update msg model =
        match msg with
        | Start -> { model with IsRunning = true }, []
        | Stop -> { model with IsRunning = false }, [ CloseWindow ]
        | Render scene -> { model with LastScene = Some scene }, [ RenderScene scene ]
