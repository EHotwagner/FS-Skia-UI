namespace FS.Skia.UI.Controls

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Theme =
    val light: Theme
    val dark: Theme
    val withDensity: density: float -> theme: Theme -> Theme
    val withAccent: accent: FS.Skia.UI.Color -> theme: Theme -> Theme
    val resolve: overrides: Theme option -> Theme
