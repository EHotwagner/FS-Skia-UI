// CONTRACT SKETCH — curated public surface of the generated token module (Phase 1).
// Final home: src/Controls/DesignTokens.fsi (curated by hand; Principle II — the .fs is generated).
// This is the additive public delta to FS.Skia.UI.Controls (FR-002, FR-008, US3). The Theme type
// and Theme module signatures are UNCHANGED; this is the only new public surface.
//
// FSI validation (Principle I) before the generated .fs exists:
//   dotnet fsi> open FS.Skia.UI.Controls
//   dotnet fsi> DesignTokens.Light.foreground        // val it : Color = { Red=31uy; ... }
//   dotnet fsi> DesignTokens.Light.foreground = Theme.light.Foreground   // val it : bool = true

namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene

/// Typed, compiler-checked design-token values generated from
/// `src/Controls/design-tokens.tokens.json` (the DTCG single source of truth).
/// Token VALUES are generated; this curated signature is the sole public-surface declaration.
/// Token references are greppable and stay in lock-step with the DTCG source via DesignTokenDrift.
module DesignTokens =

    /// Light-theme primitives (feed Theme.light; value-identical to the pre-feature literals).
    module Light =
        val foreground : Color
        val background : Color
        val accent : Color
        val danger : Color
        val muted : Color
        val fontFamily : string option
        val fontSize : float
        val density : float
        val cornerRadius : float
        val contrastRequiredRatio : float

    /// Dark-theme primitives (feed Theme.dark; value-identical to the pre-feature literals).
    module Dark =
        val foreground : Color
        val background : Color
        val accent : Color
        val danger : Color
        val muted : Color
        val fontFamily : string option
        val fontSize : float
        val density : float
        val cornerRadius : float
        val contrastRequiredRatio : float
