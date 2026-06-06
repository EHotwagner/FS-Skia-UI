// DesignTokens.fsi — curated public surface of the generated token module (feature 069).
// Principle II: this hand-curated signature is the SOLE public-surface declaration; the
// paired DesignTokens.fs is GENERATED from src/Controls/design-tokens.tokens.json (the DTCG
// single source of truth) and carries no access modifiers. Regenerate via
// `./fake.sh build -t RefreshSurfaceBaselines`; currency is enforced by DesignTokenDrift.
//
// This is the additive public delta to FS.Skia.UI.Controls (FR-002, FR-008, US3). The Theme
// type and the Theme module signatures are UNCHANGED; this is the only new public surface.

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
