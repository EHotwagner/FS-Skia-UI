# Implementation Plan: Add Animations — Declarative Motion for FS.Skia.UI

**Branch**: `073-add-animations` | **Date**: 2026-06-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/073-add-animations/spec.md`

## Summary

This is a **Tier 1 (contracted), additive** feature that adds a declarative animation
capability as a bounded **representative slice** (following the `065`/`072` pattern), not
the full motion system. An author declares — as data against an existing widget — that a
bounded set of visual properties (opacity; transform translate/scale/rotate; color) should
travel from a start value to a target value over a duration shaped by a named easing curve;
the framework produces the in-between frames and settles to rest.

The technical approach keeps the **deterministic core pure and in the `FS.Skia.UI.Scene`
package** (a new `Animation` module + `Animation.fsi`), because (a) the animatable
properties are Scene concepts — opacity already exists on `Paint`, transforms already map to
the existing `PerspectiveTransform`/`PerspectiveNode`, and `Color` is a Scene record — and
(b) the repository's deterministic, render-only evidence path (`SceneEvidence.render`,
`RendererMode = "deterministic-scene"`) already lives in Scene. Animation is modeled as a
**pure sampling function of an explicit `TimeSpan` time value** (`applyAt : elapsed ->
Animation -> Scene -> SceneNode`), so the same inputs and the same time sample always
produce byte-identical output (FR-004, FR-009, SC-003). A deliberate **identity-at-rest**
lowering rule (opacity 1.0 and identity transform emit *no* wrapper node) makes a settled
animation byte-identical to the static rendering of the same widget (FR-006, SC-004) and
guarantees un-animated views are unchanged (FR-007, SC-005).

State-driven transitions (FR-005, Story 2) are handled by a small **pure MVU helper**
(`AnimationState<'a>` with `retarget`/`advance`/`value`/`isActive`) that the author holds in
their own model and drives from their own `update` — so the author-facing `init`/`update`/
`view` contract is unchanged. Time advancement and **redraw gating** integrate as a thin,
additive **animation-tick subscription** at the Elmish-adapter edge that emits frame-delta
messages **only while at least one animation is active** and stops when all settle — resolving
the FR-006/FR-001 tension at the framework-request level without rewriting the host's internal
present loop. **No new dependency** is added: easing is pure float→float math; the time model
is BCL `System.TimeSpan`.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: **None added.** Existing only — `FS.Skia.UI.Scene` (the `Color`,
`Paint`, `Rect`, `Point`, `PerspectiveTransform`, `PerspectiveNode`, and `SceneEvidence`
surfaces the animation module builds on), the BCL (`System.TimeSpan` as the time model — no
package), SkiaSharp (render path, unchanged), `Fable.Elmish` (existing — the subscription/`Cmd`
plumbing the tick helper reuses), Expecto + FsCheck (tests), and the FAKE front-end
`FS.Skia.UI.Build` (routing, surface/evidence gates). `Directory.Packages.props` /
`docs/dependencies.md` are untouched.
**Testing**: Expecto over `tests/Scene.Tests/` (new `AnimationTests.fs` — easing curves,
tween sampling, clamping, identity-at-rest, the `AnimationState` retargeting state machine,
non-positive-duration edge), `tests/Parity.Tests/` (new animation golden fixtures + the
settled≡static and un-animated-unchanged parity assertions, captured with
`FS_SKIA_CAPTURE_GOLDEN=1`), and `tests/Elmish.Tests/` (the tick subscription emits while
active and goes silent once settled). FsCheck property tests for easing monotonicity/endpoint
pinning and lerp bounds. Deterministic render-only evidence via `SceneEvidence.render` sampled
at explicit `TimeSpan` points.
**Target Platform**: Windows and Linux. The animation core is pure and renders headlessly
through the deterministic-scene evidence path (no GPU window needed); real runs feed real
elapsed `TimeSpan` into the identical pure model.

**Routing note**: `./fake.sh build -t Route` on the *spec/plan-only* working tree prints the
docs/specify tier. Once the implementation diff lands (new `src/Scene/Animation.fsi`/`.fs`,
`Scene.fsproj` compile entry, the additive Elmish tick-subscription `.fsi`/`.fs`,
`tests/**`, regenerated surface baselines, readiness), **re-run `Route`** and run exactly the
gates it then prints — a new public `src/**/*.fsi` selects the **`package-surface`** rule
(`FocusedAuthority`: `PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff`); the
`after_implement` hook runs `EvidenceAudit`. All Technical Context items are **fully
resolved** — no open clarifications remain (see `research.md`).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` change. This feature adds
  a package-internal module to `FS.Skia.UI.Scene` plus an additive Elmish subscription helper
  and tests; none is a generated-project template fragment, package-policy, or command-surface
  change the template must mirror. Animation becomes available to generated projects
  *additively* once the package version bumps post-merge — that bump/pack and the template-pin
  refresh are **out of scope** here, owned by `speckit-merge` / `fs-skia-template-update` (spec
  Package impact).
- **Dependency impact**: N/A — **no dependency added** (FR governance "no new third-party
  package"). Easing is pure float→float math (Principle III simplicity); the time model is BCL
  `System.TimeSpan`; color/opacity/transform interpolation reuses existing `FS.Skia.UI.Scene`
  types. `Directory.Packages.props`, `docs/dependencies.md`, generated template inclusion, and
  `DependencyReport` are unchanged.
- **Command-surface impact**: No `build.fsx` / `Routing.fs` / wrapper change — the feature
  reuses the existing `package-surface` rule and the `RefreshSurfaceBaselines` /
  `PerPackageSurface.captureCurrent` baseline path; `validation.contract.yml` stays generated
  from `Routing.fs`. **Run `./fake.sh build -t Route` first on the implementation diff and run
  only the gates it prints.** FAKE-backed targets share `.fake` state and are **not** safe to
  run concurrently — run them sequentially in the escalated order when more than one is needed:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  Safe non-FAKE reads/checks may still run in parallel.
- **Generated project impact**: N/A — no change to default/minimal generated contents,
  selected-Controls guidance, local skills, validation logs, placeholder/excluded-history
  scans, or generated `Dev` behavior. The new module is additive package surface, not
  generated-project output; the scaffold game model (`src/Product/Model.fs`/`View.fs`) and the
  durable evidence spine (`--scene-evidence`, `SceneEvidence.render`,
  `RendererMode = "deterministic-scene"`) are unaffected — the animation evidence helper feeds
  the *same* deterministic path the scaffold already uses (`docs/scaffold-map.md`).
- **Evidence paths**: All under `specs/073-add-animations/readiness/`:
  - `animation-front-door.md` — the bounded property/easing slice, the `Animation` /
    `AnimationState` design, and the explicit statement that sampling is **real** pure
    computation (no `[S]`).
  - `deterministic-sampling.md` — start/midpoint/end deterministic-scene evidence for the
    Story 1 and Story 2 reference animations (distinct hashes proving monotonic progression,
    byte-identical on re-capture and across a fresh process — SC-003).
  - `settled-static-parity.md` — the identity-at-rest proof that a settled animation equals
    the static render of the same widget (FR-006/SC-004) **and** that an un-animated view is
    byte-unchanged (FR-007/SC-005).
  - `redraw-gating.md` — evidence that the tick subscription emits while an animation is
    active and goes silent once all settle (FR-006), and that removing an animating widget
    stops its animation cleanly (edge case).
  - Gate evidence (`evidence-graph.md`, `evidence-audit.md`, per-package surface diff, FSI
    transcripts, skill-loading) lands under `readiness/` per the printed gates.
- **`.fsi` / contract impact**: **Tier 1, additive-only.** New public `.fsi` declarations:
  the `FS.Skia.UI.Scene.Animation` module (`Easing`, `Transform`, `Tween<'a>`, `Animation`,
  `AnimationState<'a>`, sampling functions) and an additive Elmish subscription helper. **No
  existing signature changes shape.** The Scene public-surface baseline
  (`readiness/surface-baselines/FS.Skia.UI.Scene.txt`, plus Elmish if touched) and the
  per-package surface baseline are regenerated to reflect **additions only**, verified by
  `PackageSurfaceCheck` / `PerPackageSurfaceDiff`. Compatibility: purely additive; every
  existing Scene/Elmish symbol is untouched and not deprecated.
- **MVU/effect boundary**: This feature *is* stateful/time-driven, so Principle IV applies.
  The boundary is explicit and pure:
  - **Model** — the author holds `AnimationState<'a>` values (current displayed value, target,
    start, elapsed, duration, easing) in their own model; the framework owns no hidden mutable
    animation registry.
  - **Msg** — an author-routed frame-delta message (`AnimationTick of TimeSpan`) carrying
    elapsed time since the last frame.
  - **Effect / Cmd** — time advancement is delivered as an Elmish **subscription** that yields
    `AnimationTick` deltas; it is wired at the interpreter edge, not executed inside `update`.
  - **`update`** — stays pure: `AnimationState.advance delta` / `AnimationState.retarget
    target` are pure transitions returning the next state.
  - **Interpreter edge** — the subscription samples real elapsed time (host) or supplied
    samples (evidence) and dispatches `AnimationTick`; it **only runs while at least one
    animation is active** (author-supplied `isAnimating : 'model -> bool`), so settled state
    requests no redraws (FR-006). The author's own `init`/`update`/`view` shape is unchanged
    for anyone who does not use animation (FR-007).
- **Synthetic evidence**: **None planned — no `[S]`/`[S*]`/`[SEH]`.** Easing and tween
  sampling are real pure computation; deterministic-scene evidence is real render-only output
  through the existing `SceneEvidence.render` path; parity fixtures are golden bytes captured
  from the real sampler (`FS_SKIA_CAPTURE_GOLDEN=1`), not fabricated literals; the tick
  subscription is exercised through the real Elmish subscription plumbing. `EvidenceAudit`
  must be PASS with no disclosures.
- **Test evidence**: **Failing-first then green.** Unit tests assert `Easing.apply` pins
  endpoints (`f 0.0 = 0.0`, `f 1.0 = 1.0`) and is monotonic per curve (FsCheck), `Tween.sample`
  clamps out-of-domain time to the endpoints, non-positive duration resolves immediately to the
  end value, and `AnimationState.retarget` mid-flight continues from `Current` (no snap-back) —
  all red before `Animation.fs` exists. Parity tests assert (a) sampled frames at start/mid/end
  produce *distinct* monotone hashes, (b) the settled frame is byte-identical to the static
  render (identity-at-rest), and (c) an un-animated scene is byte-unchanged — red before the
  lowering rule lands. The subscription test asserts ticks flow while active and stop once
  settled. SC mapping: SC-002 (monotone+endpoint), SC-003 (re-capture/fresh-process identity),
  SC-004 (settled≡static), SC-005 (un-animated unchanged), SC-006 (retarget no jump), SC-007
  (edge cases).
- **Observability**: Animation introduces no new uncaught failure mode (FR-010). Where
  rendering is unsupported/headless, sampling falls through the existing
  benign/blocking/deferred host-warning classification carried by
  `GovernanceTests.visualEvidenceGuidance` (`docs/scaffold-map.md` must-survive vocabulary) —
  the deterministic-scene evidence path is structural and never requires a GPU. Non-positive
  duration and out-of-range time samples resolve to documented deterministic outcomes rather
  than throwing; `PackageSurfaceCheck` names any non-additive surface delta; the parity test
  reports the captured viewports/time samples and the byte-identical re-capture claim.
- **Deferred scope**: General physics/spring simulation as a system, gesture-/input-driven
  interactive scrubbing, sequenced/chained timelines and keyframe tracks, particle systems,
  video playback, GPU/shader visual effects, and any layout-reflowing animation beyond the
  bounded transform/opacity/color set all remain **deferred** and enumerated under the spec's
  Unsupported scope. Rewriting the host's internal Vulkan present loop for fine-grained
  per-widget redraw regions is also deferred — this feature gates redraws at the
  framework-request (subscription) level only.

## Project Structure

```
src/Scene/Animation.fsi / Animation.fs             # NEW — Easing, Transform, Tween<'a>,
                                                   #   Animation, AnimationState<'a>, applyAt,
                                                   #   sampleFrames, identity-at-rest lowering
src/Scene/Scene.fsproj                             # MODIFIED — add the Animation compile entry
                                                   #   (after Scene.fs; Animation depends on it)

src/Elmish/AnimationTick.fsi / AnimationTick.fs    # NEW — additive subscription helper:
                                                   #   AnimationTick msg + tickSubscription that
                                                   #   emits frame deltas only while active
src/Elmish/Elmish.fsproj                           # MODIFIED — add the new compile entry

tests/Scene.Tests/AnimationTests.fs                # NEW — easing/tween/clamp/state-machine/edge
tests/Scene.Tests/Scene.Tests.fsproj               # MODIFIED — add compile entry
tests/Parity.Tests/AnimationOutputTests.fs         # NEW — sampled-frame goldens, settled≡static,
                                                   #   un-animated-unchanged
tests/Parity.Tests/fixtures/v3-host-golden/scene-output/
    animation-*.txt                                # NEW — captured golden frame hashes
tests/Elmish.Tests/AnimationTickTests.fs           # NEW — active-emits / settled-silent gating

readiness/surface-baselines/FS.Skia.UI.Scene.txt   # REGENERATED — additive Animation surface
readiness/surface-baselines/FS.Skia.UI.Elmish.txt  # REGENERATED — additive tick helper surface
readiness/per-package-surface/FS.Skia.UI.Scene.fsi.txt   # REGENERATED — additive
readiness/per-package-surface/FS.Skia.UI.Elmish.fsi.txt  # REGENERATED — additive

specs/073-add-animations/
    spec.md  plan.md  research.md  data-model.md  quickstart.md
    contracts/
        animation-surface.contract.md
        deterministic-evidence.contract.md
    readiness/
        animation-front-door.md
        deterministic-sampling.md
        settled-static-parity.md
        redraw-gating.md
```

> Naming note: the new module lives under the existing `FS.Skia.UI.Scene` namespace as a
> dedicated `Animation` module — clean names (`Easing`, `Transform`, `Tween`, `Animation`,
> `AnimationState`) with no legacy collision. Per `docs/scaffold-map.md`'s record-label
> warning, `Transform` uses motion-specific labels (`TranslateX`/`TranslateY`/`ScaleX`/
> `ScaleY`/`RotationDegrees`) — **not** Scene's `X`/`Y`/`Width`/`Height` — to avoid bare-literal
> inference collisions. Exact file split and the Elmish-vs-Controls.Elmish home of the tick
> helper are tasks-phase details; the surface stays additive either way.
