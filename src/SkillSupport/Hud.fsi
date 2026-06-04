// Hud.fsi — recurring arcade-helper family (FR-010, feature 062).
//
// Reserve a fixed HUD band along one axis; clamp the gameplay region to the
// remainder. Plain `float`s only — NO Scene.Rect dependency, so SkillSupport
// stays dependency-light; the consumer converts to their own geometry type at the
// call site (consistent with the fs-skia-scene record-label-collision guidance).
// Convention (skill text): overdraw the HUD last. Visibility lives here
// (Principle II).
namespace FS.Skia.UI.SkillSupport

module Hud =

    /// Which edge of the axis the reserved HUD band occupies.
    type BandEdge =
        | Top
        | Bottom

    /// One region along the reserved axis: `Offset` from the axis origin, `Size`
    /// the extent.
    type Band = { Offset: float; Size: float }

    /// The reserved HUD band plus the clamped gameplay remainder; the two `Band`s
    /// are non-overlapping and partition `surface`.
    type HudLayout = { HudBand: Band; Gameplay: Band }

    /// Reserve a HUD band of `bandSize` at `edge` of an axis of extent `surface`,
    /// returning the band and the clamped gameplay remainder. Clamp invariants:
    /// `HudBand.Size = min bandSize surface`, `Gameplay.Size = surface - HudBand.Size ≥ 0`,
    /// and the two bands partition `surface` without overlap.
    val reserveHudBand: surface: float -> bandSize: float -> edge: BandEdge -> HudLayout
