# Contract: `FS.Skia.UI.SkillSupport.Wrap` (FR-010)

## Surface (new public `.fsi` — Tier-1 escalation)

```fsharp
// Wrap.fsi — recurring arcade-helper family (FR-010, feature 063).
//
// Shortest wrap-aware delta on a toroidal axis. Pure scalar arithmetic — NO state,
// NO I/O, NO Scene/Layout dependency, so SkillSupport stays dependency-light. The
// consumer threads this through their pure Elmish `update` (e.g. camera-relative
// targeting on a wrap-around world).

namespace FS.Skia.UI.SkillSupport

module Wrap =
    /// Shortest wrap-aware delta from `fromX` to `toX` on a toroidal axis of width
    /// `worldWidth` (> 0). Result is the signed distance of least magnitude in
    /// (-worldWidth/2, worldWidth/2].
    val wrapDeltaX: worldWidth: float -> fromX: float -> toX: float -> float
```

## Behavioral contract

- **Pure & deterministic**: same inputs ⇒ same output; no globals, no wall-clock.
- **Range**: for `worldWidth > 0`, `wrapDeltaX worldWidth a b ∈ (-worldWidth/2,
  worldWidth/2]`.
- **Shortest-path**: `wrapDeltaX w a b` equals `(b - a)` adjusted by the multiple of
  `w` that minimizes `|result|`; e.g. on `w = 100`, `wrapDeltaX 100 90 10 = 20`
  (not `-80`), `wrapDeltaX 100 10 90 = -20`.
- **Identity**: `wrapDeltaX w a a = 0`.

## Packaging

| File | Change |
|---|---|
| `src/SkillSupport/Wrap.fsi`, `Wrap.fs` | new module |
| `src/SkillSupport/SkillSupport.fsproj` | add `Wrap.fsi`/`Wrap.fs` Compile entries (after `Hud`) |
| `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` | add the `Wrap` module (baseline updated with the `.fsi` in the same change-set) |
| `.agents/skills/fs-skia-layout-readability/SKILL.md` | skill reference, alongside the `reserveHudBand` note |

## Verification

- Expecto: range, shortest-path examples, identity, and symmetry
  (`wrapDeltaX w a b = -(wrapDeltaX w b a)` except at the `+w/2` boundary).
- FSI transcript exercising the packed `Wrap.wrapDeltaX` surface.
- `PackageSurfaceCheck` / `PerPackageSurfaceDiff` green after the baseline update.

## Deferred (documented, not shipped — D9/D10)

- Camera-centered projection → documented in `fs-skia-layout-readability` (closure
  over per-game state, soft `Scene.Point` dependency, varies per game).
- `--evidence-run` deterministic-summary shape → documented in `fs-skia-evidence-mode`
  (field set varies per game; only a thin common core).
