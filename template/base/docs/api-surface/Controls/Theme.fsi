namespace FS.Skia.UI.Controls

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
/// Public contract module exposed by this FS.Skia.UI package.
module Theme =
    /// Public contract function exposed by this FS.Skia.UI package.
    val light: Theme
    /// Public contract function exposed by this FS.Skia.UI package.
    val dark: Theme
    /// Public contract function exposed by this FS.Skia.UI package.
    val withDensity: density: float -> theme: Theme -> Theme
    /// Public contract function exposed by this FS.Skia.UI package.
    val withAccent: accent: FS.Skia.UI.Scene.Color -> theme: Theme -> Theme
    /// Public contract function exposed by this FS.Skia.UI package.
    val resolve: overrides: Theme option -> Theme
