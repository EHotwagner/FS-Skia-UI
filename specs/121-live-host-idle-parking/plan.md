# Implementation Plan: Live Host Pacing, Surface Honesty & Viewer Ergonomics

**Branch**: `121-live-host-idle-parking` | **Date**: 2026-06-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/121-live-host-idle-parking/spec.md`

## Summary

Reshaped from ControlsShowcase4 downstream feedback after reconciling the report to
shipped truth (live paint-skip and quit-via-`CloseWindow` already exist in 0.1.127).
The genuine work: (1) add an additive, defaulted **frame-cap** field to `ViewerOptions`
and make the native event loop gate **both** update and render cadence by it
(`src/SkiaViewer`); (2) make the live per-tick animation-clock advance **allocation-free
when no clock is active** (`src/Controls.Elmish`); (3) **publish** the
`PointerInteraction` / `ViewerPointerPhaseKind` / `PointerButton` signatures under
`docs/api-surface/` and guard them; (4) **document** present-mode selection, the new
frame-cap lever, and the environment-bound free-run in the viewer-host skill, recording
the already-shipped paint-skip and quit paths. All behavior stays byte-identical at rest;
the persistent-window cadence change is asserted on the extracted pure pacing decision
plus reasoning (it is not drivable headless) and recorded honestly.

## Technical Context

**Language/Version**: F# / .NET (SDK 10.0.300 floats, no global.json)
**Primary Dependencies**: SkiaSharp, Silk.NET.Windowing (native present loop); no new deps
**Testing**: Expecto (`tests/`), FAKE targets via `./fake.sh`, `RefreshSurfaceBaselines`
for surface/doc regeneration, deterministic evidence (no live-window assertion)
**Target Platform**: Windows + Linux; persistent interactive window is not drivable in
headless CI (recorded in `readiness/runtime-limitations.md`)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: No `.template.config/template.json` change. The generated
  template's package **pins** are refreshed by the chained `/fs-skia-template-update`
  after the post-merge version bump. Decision: **defer the template pin update to that
  step; no template source/asset change in this feature.**
- **Dependency impact**: **N/A — no dependency change.** No `Directory.Packages.props`,
  `docs/dependencies.md`, or `DependencyReport` change; no new package is added.
- **Command-surface impact**: No `build.fsx` / wrapper / target change and no new gate.
  Run `Route` first and run only the gates it prints. FAKE-backed commands share `.fake`
  state — run sequentially in deterministic order (`Dev` → `GeneratedGuidanceCheck` →
  `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`) when
  more than one is needed; `RefreshSurfaceBaselines` regenerates surface/doc baselines.
- **Generated project impact**: No change to default/minimal generated contents or
  generated `Dev` behavior. The viewer-host **skill** guidance (FR-006/FR-007) is the
  only generated-into-projects artifact touched; `SkillSyncCheck` keeps `.claude` synced
  to the canonical `.agents` source.
- **Evidence paths**: `specs/121-live-host-idle-parking/` — `research.md`,
  `data-model.md`, `contracts/`, `quickstart.md`, `tasks.md`, `evidence-audit.md` (with
  verdict token), `generated-validation.md`; test evidence in `tests/` (pacing-decision +
  clock-advance unit tests); `readiness/runtime-limitations.md` free-run note.
- **`.fsi` / contract impact**: `src/SkiaViewer/SkiaViewer.fsi` `ViewerOptions` gains one
  additive, defaulted field; new `docs/api-surface/` entries for the three pointer/host
  types. `Controls.Elmish.fsi` shape is **unchanged** (the `wrappedTick` fix is internal).
  Surface baselines regenerate via `RefreshSurfaceBaselines`; compatibility note: existing
  `ViewerOptions` construction keeps compiling via a defaulting path.

<!-- Generated from .specify/memory/constitution.md by `./fake.sh build -t RefreshSurfaceBaselines`; do not hand-edit between the markers. -->
<!-- BEGIN GENERATED: constitution/fsi-visibility -->
**II. Visibility Lives in `.fsi`, Not in `.fs`** — Every public F# module MUST have a corresponding `.fsi` signature file.
<!-- END GENERATED: constitution/fsi-visibility -->
- **MVU/effect boundary**: No new `Msg`/`Effect`/`Cmd` and no change to the pure `update`
  contract. The already-wired `CloseWindow` `ViewerEffect` is the documented quit path
  (FR-007); interpreter behavior is unchanged. The native loop's render-cadence gating is
  a host-loop change, not an MVU-contract change.
- **Synthetic evidence**: None. No mocks/fakes/placeholders/canned responses. The
  persistent-window cadence is asserted on the **real** extracted pacing function plus
  reasoning; the un-drivable live window is disclosed as an environment limitation in
  `readiness/runtime-limitations.md` (honest non-coverage, not `[S]` synthetic data).
- **Test evidence**: Failing-first unit tests for (a) the pure frame-cap pacing decision
  (cap gates render cadence; invalid cap rejected) and (b) the clock-advance no-alloc
  invariant (reference-equality on all-inactive; correct advance otherwise). A doc/api-
  surface drift check guards the published pointer types. Governance gates per `Route`.
- **Observability**: An invalid frame-cap surfaces a clear startup diagnostic (same
  channel as positive-size validation). No new log path. The viewer-host skill documents
  the free-run as a benign environment limitation with the frame-cap mitigation.
- **Deferred scope**: Spec Kit tooling asks (spec-lint, source-snapshot, catalog-
  coverage) deferred — out of framework scope. No new quit contract field (already
  shipped). No headless-window responsiveness. Template pin refresh handled by the
  chained `/fs-skia-template-update`.

**Constitution gates**: PASS — additive `.fsi` with a defaulting path (II); plainest-F#
loop gate + `Map.exists` reference-equality short-circuit, both mutation-on-hot-path
disclosed (III); spec → FSI → tests → implement order honored (I).

## Project Structure

```
specs/121-live-host-idle-parking/
  spec.md            # reshaped (this feature)
  plan.md            # this file
  research.md        # Phase 0 — pacing + no-alloc + surface-publish decisions
  data-model.md      # Phase 1 — ViewerOptions field, pacing decision, clock-advance
  contracts/
    viewer-options.md  # public ViewerOptions frame-cap contract
    api-surface.md     # pointer/host types to publish under docs/api-surface/
  quickstart.md      # consumer: set a frame-cap, quit, pick a present mode
  checklists/requirements.md
  tasks.md           # Phase 2 (/speckit-tasks)

src/SkiaViewer/
  SkiaViewer.fsi     # ViewerOptions += FrameCap (additive, defaulted)
  SkiaViewer.fs      # thread frame-cap into ViewerConfiguration.TargetFrameRate; validate
  Host/OpenGl.fs     # runEventLoop: gate DoRender by frameInterval (FR-002)

src/Controls.Elmish/
  ControlsElmish.fs  # wrappedTick: no-alloc when no clock active (FR-004) — internal

docs/api-surface/    # new published signatures for PointerInteraction/PhaseKind/Button
.agents/skills/fs-skia-viewer-host/  # present-mode + frame-cap + env-limit guidance (FR-006/007)
  (regenerate .claude peer via RefreshSurfaceBaselines / SkillSyncCheck)
readiness/runtime-limitations.md     # free-run-is-environment-bound note
```

## Complexity Tracking

No constitution deviations requiring justification. The one mutation-bearing change (the
`runEventLoop` cadence gate) lives in an already-`mutable` native loop on a measured hot
path; the no-alloc short-circuit is a plain `Map.exists` guard before a `Map.map`.

## Phase 0 / Phase 1 Outputs

- Phase 0: [research.md](./research.md) — all unknowns resolved (no NEEDS CLARIFICATION).
- Phase 1: [data-model.md](./data-model.md), [contracts/](./contracts/),
  [quickstart.md](./quickstart.md); agent context (`AGENTS.md`) repointed to this plan.
</content>
