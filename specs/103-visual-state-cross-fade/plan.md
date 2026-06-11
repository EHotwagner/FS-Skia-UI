# Implementation Plan: True Visual-State Cross-Fade

**Branch**: `103-visual-state-cross-fade` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/103-visual-state-cross-fade/spec.md`

## Summary

R6 (the final rung of the controls-architecture-evolution roadmap, §11.3) makes a live
visual-state transition a **genuine cross-fade** instead of fading the new appearance in
from transparent. The decisive grounding fact found during planning: feature-073's
`Animation.applyAt` **never applies the `Color` tween** — it samples opacity and transform
only (the `Color` tween is counted by `isSettled` but never recolors the scene), and a
*single* Scene `Color` tween cannot represent the multi-channel paint
(`Foreground`/`Fill`/`Stroke`) that `Style.resolve` produces anyway. So the roadmap's loose
"feed the style delta into `applyAt`" is not a real path.

**Technical approach.** Realize the cross-fade by **compositing the two cached static
own-scene snapshots** of the retained identity — the *prior* state's painted own-scene
(already cached on the previous frame's `RetainedNode.Fragment.OwnScene`) under the *next*
state's painted own-scene — each driven by the existing opacity tween via the **public**
`Animation.applyAt`: the prior layer fades out (`1 → 0`), the next layer fades in (`0 → 1`).
For any region painted in both states the source-over composite yields a displayed color
**strictly between** the two endpoints (SC-001); both shrinking and growing paint are
correct; and the existing settle path (a settled clock is dropped / paints `ownStatic`
verbatim) guarantees the final frame and at-rest output stay **byte-identical** (FR-004,
FR-005) with no change to that path. The only new state is a **prior-snapshot field** added
to the internal `AnimationClock`, captured at transition start from the matched prior
retained fragment. Because the Scene `Color` tween is *not* the realization mechanism, the
`AnimationClock` doc is reconciled to describe the snapshot-composite cross-fade and drop
the unfulfilled standalone color-tween claim (FR-009, the spec's explicit "trim the doc"
path).

This holds every permanent non-goal: the animated "channel set" is the node's own painted
appearance (closed, token-derived by construction via `Style.resolve` upstream) — **no**
open per-property animation surface, no consumer transition API, no easing/duration knobs.

## Technical Context

**Language/Version**: F# / .NET
**Primary Dependencies**: `FS.Skia.UI.Controls` (the `RetainedRender` internal module); reuses
`FS.Skia.UI.Scene` `Animation.applyAt` / opacity tween (no Scene source change). N/A new packages.
**Testing**: Expecto + FsCheck (repo has no `testProperty`; use `Check.One` with a fixed
injected-delta sequence, per feature 099). FAKE targets per `Route`. Deterministic injected-delta
sampling for evidence (no wall-clock).
**Target Platform**: Windows and Linux (no platform narrowing; rendering is deterministic scene
assembly, GPU not required for the byte-identity/interpolation evidence).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Tier**: **Tier 1 (contracted change)** — alters observable behavior (mid-transition paint) and
touches an internal-but-baselined `.fsi` (`AnimationClock` gains a field; doc reconciled). Per the
project routing, any `src/Controls/**` edit escalates `Route` to the `controls-public-surface` gate
set even though the **public** consumer surface is unchanged. Full artifact chain applies (spec,
plan, `.fsi`, per-package surface baseline, tests, evidence).

### Repository Governance Decisions

- **Template ownership**: N/A — no change to `template/**`, `.template.config/template.json`, Spec
  Kit assets, package policy, or command surface. R6 is framework-internal to `src/Controls`.
- **Dependency impact**: N/A — no `Directory.Packages.props` / `docs/dependencies.md` change; no new
  dependency; reuses the existing Scene `Animation` API. `DependencyReport` coverage unchanged.
- **Command-surface impact**: N/A — no `build.fsx`/target definition change. Validation runs the
  gates `Route` prints for a `controls-public-surface`-escalated change, then `EvidenceGraph` +
  `EvidenceAudit`. FAKE-backed commands run **sequentially** in the deterministic order (Dev →
  GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit);
  the actual minimal list is whatever `Route` prints for this diff.
- **Generated project impact**: N/A — no generated-project default/minimal content, selected
  Controls guidance, or generated `Dev` behavior change.
- **Evidence paths**: under `specs/103-visual-state-cross-fade/readiness/` —
  `at-rest-byte-identity.md` (rendered fragment == static, no animation attribute),
  `final-frame-identity.md` (settled frame == snapped static, all channels),
  `mid-flight-interpolation.md` (intermediate sampled color strictly between endpoints, SC-001),
  `determinism.md` (fixed injected-delta sequence reproducibility), plus `evidence-graph.md`,
  `evidence-audit.md` (verdict token), `generated-validation.md`
  (`package-resolution=resolved`, `package-mismatch=false`), and the standard not-applicable set if
  the window-visibility audit fires on literal filenames.
- **`.fsi` / contract impact**: `src/Controls/RetainedRender.fsi` — the **internal** `AnimationClock`
  type gains a prior-snapshot field and its doc-comment is reconciled to the snapshot-composite
  cross-fade (drops the standalone Scene-`Color`-tween claim, FR-009). The **public**
  `runInteractiveApp`/consumer surface is **unchanged**. Per-package surface baseline moves (internal
  `.fsi` is captured) → recapture via `PerPackageSurface.captureCurrent` (RefreshSurfaceBaselines does
  NOT regenerate per-package snapshots — `[[per-package-baseline-not-in-refresh-target]]`).
- **MVU/effect boundary**: N/A as new MVU surface — R6 adds no `Model`/`Msg`/`Effect`/interpreter. It
  reuses the feature-099 host-tick → `advance` → assemble seam; `updateClockForState` and the
  assemble walk stay pure (clock advanced from injected deltas, no wall-clock, no hidden mutable
  registry — Principle IV/III honored).
- **Synthetic evidence**: **None planned.** Every proof drives the real `RetainedRender` assemble path
  with real `Style.resolve`-painted snapshots and injected deltas. No mocks/fakes/`[S]`. (If any
  arises it gets `[S]` + the five-surface disclosure; not anticipated.)
- **Test evidence**: failing-first semantic tests through the `RetainedRender.step` surface (the same
  internal entry the 099/101 tests use via `<InternalsVisibleTo>`): (a) mid-flight color strictly
  between endpoints (red→green initially absent because the new appearance fades in from transparent;
  green after the prior-snapshot composite); (b) final-frame == snapped static; (c) at-rest
  byte-identity + no animation attribute; (d) determinism under a fixed delta sequence; (e) held
  state stays a scoped `Keep` / single repaint; (f) Controls + Elmish suites stay green.
- **Observability**: no new diagnostics needed; the existing `RemeasuredNodeCount`/diagnostics are
  untouched. Unsupported-environment messaging unchanged (assembly is GPU-free).
- **Deferred scope**: Out — consumer-facing transition authoring API, configurable easing/duration,
  transform-channel animation on state change, animating channels `Style.resolve` does not produce,
  default arrow-key routing for `Chart`/`Graph`/`Progress` (separate R8-noted decision). R6 is the
  last roadmap rung; no successor.

**Gate result**: PASS. One justified complexity note (below); no unjustified violations.

### Complexity / simplicity justification (Principle III)

The cross-fade is realized by reusing the **public** `Animation.applyAt` opacity tween over two
existing cached snapshots — the plainest path that reuses existing machinery. The single added piece
of state (a prior-snapshot `Scene list` on the internal `AnimationClock`) is necessary because, once
the visual state flips on the first transition frame, the prior appearance is no longer recoverable
from the next frame's tree; it must be captured at transition start. No new abstractions, operators,
SRTP, reflection, or computation expressions are introduced.

## Project Structure

```
specs/103-visual-state-cross-fade/
  spec.md                      # feature spec (done)
  plan.md                      # this file
  research.md                  # Phase 0 — design fork resolved (applyAt-ignores-Color → snapshot composite)
  data-model.md                # Phase 1 — AnimationClock prior-snapshot field + entities/invariants
  quickstart.md                # Phase 1 — how to drive + observe the cross-fade deterministically
  contracts/
    retained-render-crossfade.md  # internal RetainedRender cross-fade contract & invariants
  checklists/requirements.md   # spec quality checklist (done)
  readiness/                   # evidence artifacts (Phase implement)

src/Controls/
  RetainedRender.fs            # updateClockForState (capture prior snapshot), sampleOnPaint (composite),
                               #   assemble walk (thread prior snapshot by identity)
  RetainedRender.fsi           # internal AnimationClock: + prior-snapshot field; reconcile doc (FR-009)
  Style.fs / Style.fsi         # unchanged (endpoints already produced by Style.resolve upstream)

test/ (Controls + Elmish suites)  # add failing-first cross-fade semantic tests via RetainedRender.step
```

## Phase 0 — Outline & Research

See [research.md](./research.md). Resolves the single real design fork (the `Color`-tween dead-end vs
snapshot composite vs per-channel `ResolvedStyle` re-paint), the retarget-on-second-change semantics,
and the doc-reconciliation decision (FR-009). No NEEDS CLARIFICATION remain.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md): the `AnimationClock` prior-snapshot field, the
  `VisualStateValue` `Keep` interaction, and the cross-fade invariants (at-rest / final-frame
  byte-identity, determinism, mid-flight strictly-between).
- [contracts/retained-render-crossfade.md](./contracts/retained-render-crossfade.md): the internal
  `RetainedRender` cross-fade contract — inputs (prior + next own scenes, clock), the composite recipe,
  and the seven asserted invariants mapped to FRs/SCs.
- [quickstart.md](./quickstart.md): deterministic recipe to drive a `Normal → Hover` transition through
  `RetainedRender.step` with injected deltas and observe the interpolated color.
- Agent context: update the `AGENTS.md` SPECKIT plan reference to this plan.

## Post-Design Constitution Re-Check

Re-evaluated after Phase 1: still **PASS**. The design adds one internal field and reuses public Scene
opacity sampling; public surface unchanged; byte-identity at the two stable points is guaranteed by the
**unchanged** settle/fast path; doc↔behavior gap is closed rather than reopened (FR-009). No new
synthetic evidence, no new dependencies, no MVU surface, no command-surface change.
```
