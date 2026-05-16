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
    val init: options: ViewerOptions -> ViewerModel * ViewerEffect list
    val update: msg: ViewerMsg -> model: ViewerModel -> ViewerModel * ViewerEffect list
