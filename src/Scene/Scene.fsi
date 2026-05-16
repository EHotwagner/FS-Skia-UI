namespace FS.Skia.UI.Scene

type Size =
    { Width: int
      Height: int }

type Color =
    { Red: byte
      Green: byte
      Blue: byte
      Alpha: byte }

type Point =
    { X: float
      Y: float }

type Rect =
    { X: float
      Y: float
      Width: float
      Height: float }

type Paint =
    { Fill: Color option
      StrokeWidth: float option
      Opacity: float }

type SceneNode =
    | Empty
    | Rectangle of id: string * bounds: Rect * paint: Paint
    | Text of id: string * position: Point * text: string * paint: Paint
    | Group of id: string * children: SceneNode list

module Colors =
    val transparent: Color
    val rgb: red: byte -> green: byte -> blue: byte -> Color

module Scene =
    val empty: SceneNode
    val rectangle: id: string -> bounds: Rect -> fill: Color -> SceneNode
    val text: id: string -> position: Point -> value: string -> fill: Color -> SceneNode
    val group: id: string -> children: SceneNode list -> SceneNode
    val describe: node: SceneNode -> string
