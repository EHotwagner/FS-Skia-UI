# US1 independent validation — a localized visual change reports a small dirty region

**Story**: A localized visual change reports a small dirty region; a whole-frame invalidation reports
frame-spanning damage; an idle frame reports zero.

## Path

Drive (a) a single-control content/visual change, (b) a theme switch, and (c) an idle (unchanged) frame
through the retained `RetainedRender.step` and assert the damage carriers
(`RepaintedNodeCount` / `DirtyRectCount` / `DirtyArea`) are small / frame-spanning / `0/0/0` proportional
to the change.

## Evidence

- `tests/Controls.Tests/Feature116DamageTests.fs` — a single fixed-size leaf content change repaints
  exactly one node (`RepaintedNodeCount = 1 <= 4 < TotalNodeCount`, `DirtyRectCount = 1`,
  `DirtyArea = 120*24`, `< FrameArea`); a theme switch repaints every node
  (`RepaintedNodeCount = TotalNodeCount`, frame-spanning area); an idle frame reports `0/0/0`; localized
  damage is strictly less than theme-switch damage (proportionality); the integer counts re-run
  byte-identically.
- `tests/Elmish.Tests/Feature116MetricsTests.fs` — over `ControlsElmish.Perf.runScript`, a localized
  change reports small damage with `RepaintedNodeCount >= 1` and `< total`, an idle frame reports
  `0/0/0`.
- 109 perf-corpus goldens (regenerated) carry the deterministic `RepaintedNodeCount` / `DirtyRectCount` /
  `DirtyArea` per frame.

DirtyArea definition (research §a, pinned in [damage-metrics-authority.md](./damage-metrics-authority.md)):
the summed integer `w*h` over the DISTINCT repainted boxes; a theme switch repaints every node so its
area is frame-spanning (≫ a localized box).

Result: PASS — damage is proportional to the change (SC-001).
