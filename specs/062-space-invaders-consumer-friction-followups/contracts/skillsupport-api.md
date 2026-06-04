# Contract: FS.Skia.UI.SkillSupport Helpers (FR-010/012) — Tier 1

**Surface type:** new public `.fsi` in `src/SkillSupport/` + new per-package
surface baseline. This is the **only** Tier-1 change-set; `.fsi` and baseline are
updated together (Principle II, FR-012). Sketched in FSI before `.fs` (Principle I).

## C1 — `Random` module (`.fsi` shape, draft)

```fsharp
namespace FS.Skia.UI.SkillSupport

/// Deterministic, replayable seeded RNG. No ambient System.Random.
/// splitmix64 expands the seed; xorshift64 generates the stream.
module Random =

    /// Opaque pure RNG state (the xorshift64 stream word).
    [<Struct>]
    type RngState

    /// Expand a seed into an initial state (avoids the all-zero fixed point).
    val seedRng : seed: uint64 -> RngState

    /// One step: the next 64-bit value and the advanced state.
    val nextRng : state: RngState -> uint64 * RngState

    /// Uniform-ish value in [0, n); requires n > 0.
    val nextBelow : n: int -> state: RngState -> int * RngState
```

**Invariants (tested, failing-first):**
- *Determinism/replay:* `seedRng s` then the same call sequence yields an
  identical value stream on any platform/run.
- *Bounds:* `nextBelow n` returns a value in `[0, n)` for `n > 0`.
- *Purity:* no `System.Random`, no wall-clock, no shared mutable; `mutable` only
  inside the step function, disclosed at the use site.

## C2 — `Hud` module (`.fsi` shape, draft)

```fsharp
namespace FS.Skia.UI.SkillSupport

/// Reserve a fixed HUD band along one axis; clamp gameplay to the remainder.
/// Plain floats — no Scene.Rect dependency (consumer converts at the call site).
module Hud =

    type BandEdge = Top | Bottom

    /// (offset, size) along the reserved axis.
    type Band = { Offset: float; Size: float }

    /// Reserved HUD band + clamped gameplay remainder that partition `surface`.
    type HudLayout = { HudBand: Band; Gameplay: Band }

    val reserveHudBand : surface: float -> bandSize: float -> edge: BandEdge -> HudLayout
```

**Invariants (tested):** `HudBand.Size = min bandSize surface`;
`Gameplay.Size = surface - HudBand.Size ≥ 0`; bands are non-overlapping and
partition `surface`. Convention (skill text): **overdraw the HUD last.**

## C3 — Packaging & baseline (FR-012)

- `src/SkillSupport/SkillSupport.fsproj`: add `Random.fsi/.fs`, `Hud.fsi/.fs`
  `Compile` entries (order: `.fsi` before `.fs`).
- New `readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt` created and kept
  in sync with the `.fsi` (validated by `PackageSurfaceCheck`/`PerPackageSurfaceDiff`).
- Already pinned for consumers in `template/base/Directory.Packages.props`; bump +
  pack on merge.

## C4 — Skill reference (FR-010/011-#2)

`fs-skia-layout-readability` (HUD/gameplay-region owner) references `Hud`;
`fs-skia-elmish` (pure-`update` threading owner) references `Random`, each
pointing at the new surface so an arcade-demo author finds them before
re-implementing.

## Acceptance (SC-006)

`seedRng`/`nextRng`/`nextBelow` and `reserveHudBand` are shipped with a skill
reference and surface baseline; the three deferred helpers have a recorded
per-helper rationale (research.md D10).
