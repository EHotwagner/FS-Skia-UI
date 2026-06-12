# Governance risk levels (feature 111)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls.Elmish (+ internal FS.Skia.UI.Controls retained surface, consumed only)
public-api-impact=yes (new `FrameCause` DU + `FrameMetrics` `DiffRan`/`LayoutRan`/`PaintRan`; narrowed `ViewCalled`/`FullRenderCount` on model-unchanged frames)
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; dispatch OUTCOMES byte-identical, FR-008 — only per-frame phase SCHEDULING + cause/phase observability change)
route-tier=agent-ready (package-surface)

## Risk classification

- **small** — a test-only or golden edit that does not touch the public `.fsi`: focused
  validation is `Dev` plus the affected test list.
- **medium** — the `FrameMetrics`/`FrameCause` `.fsi`/`.fs` change + construction-site updates:
  focused validation is `RefreshSurfaceBaselines` + `Dev` + the surface diffs.
- **broad** — the full package-surface set Route prints, required because the public
  `ControlsElmish.fsi` surface changes (a new `FrameCause` type + `FrameMetrics` fields).
  **broad validation** is mandatory before merge. Non-authoritative aggregate results are advisory
  only in [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict
  is the focused per-target rerun.

THIS feature is **broad** (a breaking public `.fsi` change to `FrameMetrics` + the new `FrameCause`).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

Run `./fake.sh build -t Route` against the real diff and obey its printed minimal list. For this change
Route routes to the **package-surface** tier (the change touches `FS.Skia.UI.Controls.Elmish`, NOT the
Controls catalog, so it does NOT escalate to the heavier controls-public-surface set — same as feature
109). Route prints:

`Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`

`RefreshSurfaceBaselines` regenerates the top-level surface baseline (gains the `FrameCause` type +
cases) **and** the per-package surface (the `FrameMetrics` fields) after the additions.

## Required evidence per risk level

- **required evidence** (broad / public `.fsi`): recaptured top-level surface + per-package baselines
  (`PackageSurfaceCheck` / `PerPackageSurfaceDiff` green over the regenerated baselines showing the
  `FrameCause` type + `FrameMetrics` field delta) and the new fields' `///` docs (doc-preservation gate).
- **required evidence** (US1 cause): every frame's `FrameCause` matches its trigger, byte-stable
  (`Feature111FrameCauseTests`, SC-001/SC-005).
- **required evidence** (US2 phase record): the four phase bools per frame class
  (`Feature111PhaseRecordTests`, SC-002/SC-004).
- **required evidence** (US3 view-skip): an animation tick / model-unchanged frame is view-free and
  byte-identical; continuous drag + animation are frame-rate work (`Feature111ViewSkipTests`,
  SC-003/SC-007/SC-008) + the updated `Feature109MetricsHonestyTests` (FR-011).
- **required evidence** (corpus regen): the feature-109 goldens regenerated to carry cause/phase and
  show view-free tick frames, with the before/after delta in
  [view-free-delta.md](./view-free-delta.md) (FR-010/SC-006).
- **required evidence** (byte-identity): at-rest rendered output + geometry byte-identical — the
  standing Scene-parity golden suite under `Dev`
  ([byte-identity-authority.md](./byte-identity-authority.md), FR-008/SC-007).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
