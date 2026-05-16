namespace FS.Skia.UI.Controls

open FS.Skia.UI

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Theme =
    let light : Theme =
        { Name = "light"
          Foreground = Colors.rgba 31uy 41uy 55uy 255uy
          Background = Colors.rgba 248uy 250uy 252uy 255uy
          Accent = Colors.rgba 37uy 99uy 235uy 255uy
          Danger = Colors.rgba 185uy 28uy 28uy 255uy
          Muted = Colors.rgba 100uy 116uy 139uy 255uy
          FontFamily = None
          FontSize = 14.0
          Density = 1.0
          CornerRadius = 4.0
          ContrastRequiredRatio = 4.5 }

    let dark =
        { light with
            Name = "dark"
            Foreground = Colors.rgba 241uy 245uy 249uy 255uy
            Background = Colors.rgba 17uy 24uy 39uy 255uy
            Accent = Colors.rgba 96uy 165uy 250uy 255uy
            Muted = Colors.rgba 148uy 163uy 184uy 255uy }

    let withDensity (density: float) (theme: Theme) =
        { theme with Density = max 0.5 density }

    let withAccent accent (theme: Theme) =
        { theme with Accent = accent }

    let resolve (overrides: Theme option) =
        overrides |> Option.defaultValue light
