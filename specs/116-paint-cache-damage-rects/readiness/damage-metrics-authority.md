# Damage-metrics authority (feature 116)

The authoritative definition of the three damage carriers, pinned here so the contract's "small vs
frame-spanning" headline maps to concrete deterministic integers.

## Definitions (research §a)

- **`RepaintedNodeCount`** = the count of nodes whose own paint was REPAINTED this frame — every node
  that flows through `paintFresh` (the `carry` / `buildFresh` / `Update`-own-repaint branches). This is
  the existing `recomputed` counter (so it includes genuinely-shifted nodes, honest per FR-002), now
  surfaced. An all-`Keep` (idle) frame repaints nothing → `0`.
- **`DirtyRectCount`** = the count of DISTINCT repainted boxes (one per repainted node's evaluated
  `Fragment.Box`, identical boxes deduplicated; `None` boxes contribute none). `<= RepaintedNodeCount`.
- **`DirtyArea`** = the summed integer `width * height` over the DISTINCT repainted boxes. Integer
  (control geometry is integer, FR-016).

## The summed-area vs union question (resolved)

research §a fixes `DirtyArea` as the **summed distinct-box area** (the simplest deterministic default;
a spatial union/merge coalescer is a deterministic plan option deferred this rung). The
damage-metrics-contract's prose characterization "theme switch → DirtyArea = FrameArea (the union ≈
frame)" is realized here as **frame-spanning**: a theme switch repaints EVERY node
(`RepaintedNodeCount = TotalNodeCount`), so its summed area is far larger than any localized box. A
localized change repaints one box (`DirtyArea < FrameArea`). The tests therefore assert the
RELATIONSHIP that distinguishes the two (localized `DirtyArea` ≪ theme-switch `DirtyArea`,
`RepaintedNodeCount` localized `< TotalNodeCount` = theme-switch), not a literal union equal to the
frame area. The exact frozen integers per corpus scene are pinned by the regenerated 109 goldens.

## Evidence

`tests/Controls.Tests/Feature116DamageTests.fs`, `tests/Elmish.Tests/Feature116MetricsTests.fs`, and the
regenerated 109 perf-corpus goldens.
