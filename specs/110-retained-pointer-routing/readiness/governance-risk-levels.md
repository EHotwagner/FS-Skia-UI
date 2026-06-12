# Governance risk levels (feature 110)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls.Elmish (+ internal FS.Skia.UI.Controls retained surface)
public-api-impact=yes (breaking: add `FullRenderFallbackCount: int` to `FrameMetrics`; narrow
`FullRenderCount`/`ViewCalled` so routing increments neither)
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; dispatch OUTCOMES
byte-identical — only the routing MECHANISM that produces the dispatched messages changes, FR-006/FR-011)
route-tier=agent-ready (controls-public-surface, maintainer-verify)

## Risk classification

- **small** — a test-only or golden edit that does not touch the public `.fsi`: focused validation is
  `Dev` plus the affected test list.
- **medium** — the `FrameMetrics` `.fsi`/`.fs` field change + construction-site updates: focused
  validation is `RefreshSurfaceBaselines` + `Dev` + `GeneratedProductCheck`.
- **broad** — the full escalated controls-public-surface (maintainer-verify) set, required because the
  public `ControlsElmish.fsi` surface changes breakingly (the new `FrameMetrics` field).
  **broad validation** is mandatory before merge. Non-authoritative aggregate results are advisory only
  in [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is the
  focused per-target rerun.

THIS feature is **broad** (a breaking public `.fsi` change to `FrameMetrics`).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

Run `./fake.sh build -t Route` against the real diff and obey its printed minimal list. For this change
Route escalates to controls-public-surface and prints:

`Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck,
ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck,
ControlsDocCoverageCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck,
TemplateDrift, EvidenceGraph, EvidenceAudit`

`RefreshSurfaceBaselines` regenerates the surface + per-package baselines after the field + internal-seam
additions (the only public-surface delta is the `FrameMetrics` field; the new `routeRetainedInteraction`
/ `routeRetainedPointer` / `authoredControlIds` seams are `internal`).

## Required evidence per risk level

- **required evidence** (broad / public `.fsi`): recaptured surface + per-package baselines
  (`PackageSurfaceCheck` / `PerPackageSurfaceDiff` green over the regenerated baselines showing ONLY the
  `FrameMetrics` field delta + the internal retained-route seams) and the new field's `///` doc
  (doc-preservation gate).
- **required evidence** (US1 zero-routing-render): a routed move and a routed press/click each perform
  zero routing full renders and never fall back (`Feature110RetainedRoutingTests`, SC-001/002/005/009).
- **required evidence** (US2 parity): the retained route's dispatched message list + matched identity +
  focus outcome equal the preserved oracle's over keyed / unkeyed-same-kind-sibling / composite / nested
  scenes (`Feature110RetainedRoutingParityTests`, SC-003/004).
- **required evidence** (US3 fallback): every normal scenario reports `FullRenderFallbackCount = 0`
  (SC-005) and a forced unroutable case increments it by one while still dispatching identically to the
  oracle (`Feature110FallbackTests`, SC-006).
- **required evidence** (corpus regen): the feature-109 pointer goldens regenerated so routing
  full-render counts drop to zero, with the before/after delta recorded in
  [routing-fullrender-delta.md](./routing-fullrender-delta.md) (FR-010/SC-007).
- **required evidence** (byte-identity): at-rest rendered output + control geometry byte-identical to the
  pre-feature state — the standing Scene-parity golden suite under `Dev` is the authority
  ([byte-identity-authority.md](./byte-identity-authority.md), FR-011/SC-008).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
