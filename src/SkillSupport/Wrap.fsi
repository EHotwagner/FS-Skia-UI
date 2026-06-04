// Wrap.fsi — recurring arcade-helper family (FR-010, feature 063).
//
// Shortest wrap-aware delta on a toroidal axis. Pure scalar arithmetic — NO state,
// NO I/O, NO Scene/Layout dependency, so SkillSupport stays dependency-light. The
// consumer threads this through their pure Elmish `update` (e.g. camera-relative
// targeting on a wrap-around world). Visibility lives here (Principle II).
namespace FS.Skia.UI.SkillSupport

module Wrap =
    /// Shortest wrap-aware delta from `fromX` to `toX` on a toroidal axis of width
    /// `worldWidth` (> 0). Result is the signed distance of least magnitude in
    /// (-worldWidth/2, worldWidth/2]. Pure and deterministic; identity
    /// `wrapDeltaX w a a = 0`; e.g. `wrapDeltaX 100 90 10 = 20`,
    /// `wrapDeltaX 100 10 90 = -20`.
    val wrapDeltaX: worldWidth: float -> fromX: float -> toX: float -> float
