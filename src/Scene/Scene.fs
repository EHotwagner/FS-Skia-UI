namespace FS.Skia.UI.Scene

open System

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
    let transparent =
        { Red = 0uy
          Green = 0uy
          Blue = 0uy
          Alpha = 0uy }

    let rgb red green blue =
        { Red = red
          Green = green
          Blue = blue
          Alpha = 255uy }

module Scene =
    let empty = Empty

    let solid fill =
        { Fill = Some fill
          StrokeWidth = None
          Opacity = 1.0 }

    let rectangle id bounds fill =
        Rectangle(id, bounds, solid fill)

    let text id position value fill =
        Text(id, position, value, solid fill)

    let group id children =
        Group(id, children)

    let describe node =
        let rec loop depth current =
            let indent = String.replicate depth "  "

            match current with
            | Empty -> $"{indent}Empty"
            | Rectangle(id, bounds, _) -> $"{indent}Rectangle {id} {bounds.Width}x{bounds.Height}"
            | Text(id, _, value, _) -> $"{indent}Text {id} {value}"
            | Group(id, children) ->
                let childDescriptions = children |> List.map (loop (depth + 1))
                String.concat Environment.NewLine ($"{indent}Group {id}" :: childDescriptions)

        loop 0 node
