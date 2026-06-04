namespace FS.Skia.UI.SkillSupport

module Hud =

    type BandEdge =
        | Top
        | Bottom

    type Band = { Offset: float; Size: float }

    type HudLayout = { HudBand: Band; Gameplay: Band }

    let reserveHudBand (surface: float) (bandSize: float) (edge: BandEdge) : HudLayout =
        // Clamp the reserved band to the surface (a band larger than the surface
        // leaves zero gameplay, never a negative remainder).
        let hud = max 0.0 (min bandSize surface)
        let gameplay = surface - hud
        match edge with
        | Top ->
            { HudBand = { Offset = 0.0; Size = hud }
              Gameplay = { Offset = hud; Size = gameplay } }
        | Bottom ->
            { HudBand = { Offset = gameplay; Size = hud }
              Gameplay = { Offset = 0.0; Size = gameplay } }
