# Implementation Plan: Mouse Input & Pointer Events

**Branch**: `075-mouse-input-events` | **Date**: 2026-06-07 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/075-mouse-input-events/spec.md`

## Summary

Deliver the public, consumer-facing pointer-interaction contract that turns the
host's raw pointer stream into meaningful, control-addressed interactions (hover
enter/leave, click, press/release, drag lifecycle, secondary-button click, and
wheel/scroll), at parity with the existing keyboard input story. The framework
already has every lower-level piece: the host publishes `PointerMoved/Pressed/
Released`, `ControlRuntime` already models `HoveredControl`/`PressedControls`/
`ActiveDrag`/`FocusedControl`, and `Layout.hitTestComputed` already maps a point
to the front-most visible node. What is missing — and what this feature adds — is
(1) a **host contract extension** carrying mouse-button identity and wheel deltas,
(2) a **pure coordination front door** in `FS.Skia.UI.Controls` that hit-tests a
pointer sample, computes ordered hover-enter/leave + press/release/click + drag +
scroll outcomes (with the click-vs-drag threshold and per-button tracking), and
(3) an **MVU bridge** in `FS.Skia.UI.Controls.Elmish` that lowers the new pointer
effects into `Cmd<'msg>`, exactly as `interpretKeyboardEffect`/
`interpretControlEffect` do today.

Technical approach: model the coordination layer as a pure Elmish/MVU reducer
(`PointerState` + `PointerMsg` + `PointerInteraction` effects + `init`/`update`/
`replay`) layered over `ControlRuntime`, consuming `Layout.hitTestComputed`. The
front door is host-independent — it speaks a neutral `PointerSample` value, not
`ViewerEvent` — so `FS.Skia.UI.Controls` keeps its existing dependency footprint
(Scene, Layout, KeyboardInput) and the host glue lives in the consumer/sample,
mirroring how `samples/InteractiveViewer` already translates `ViewerEvent` into
runtime messages. Determinism and recorded-event replay (FR-009, SC-005) come for
free from a pure `update` + a `replay` fold, exactly as keyboard does.

This is a **Tier 1 (contracted change)** / consumer-contract change: it adds
public `.fsi` surface in `Controls`, `Controls.Elmish`, and `SkiaViewer`, moves
their surface baselines, and therefore routes to the escalated
maintainer-verify path. Run `./fake.sh build -t Route` first and run only the
gates it prints.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new packages. Consumes existing `FS.Skia.UI.Layout`
(`hitTestComputed`), `FS.Skia.UI.Controls` (`ControlRuntime`), and Silk.NET input
(already referenced by `FS.Skia.UI.SkiaViewer`) for the `MouseButton` parameter
(already delivered to the press/release handlers but currently discarded) and the
`IMouse.Scroll` event (not yet subscribed).
**Testing**: Expecto + FsCheck (pure `update`/threshold/ordering property tests,
parity/replay determinism tests), FAKE targets, FSI transcripts through the packed
libraries, and a runnable sample with captured readiness evidence.
**Target Platform**: Windows and Linux (host pointer + wheel events available on
both via Silk.NET; GPU-passthrough caveat from the constitution applies to the
sample's visual proof, not to the deterministic interaction tests).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial evaluation**: PASS. The feature is stateful/interactive, so Principle IV
(Elmish/MVU boundary) governs the design — satisfied by the `PointerState`/
`PointerMsg`/`PointerInteraction`/`init`/`update`/`replay` model with a pure
`update` and host I/O kept at the edge. Principle I/II (Spec → FSI → tests →
impl; visibility in `.fsi`) is satisfied by the `contracts/` `.fsi` sketches and
the per-package baseline updates. No constitutional violation requires
justification; no Principle III complexity exemptions are needed (plain records,
discriminated unions, and a fold — no SRTP, reflection, type providers, or
non-trivial computation expressions).

**Post-design re-evaluation**: PASS. The Phase 1 design introduces no new
dependency, keeps `update` pure, expresses every I/O boundary as data
(`PointerInteraction` effects + `AdapterEffect` cases interpreted at the edge),
and adds curated `.fsi` for every new public module. Per-button press tracking and
the click/drag threshold live as plain record/`Map` state, not clever
abstractions.

### Repository Governance Decisions

- **Template ownership**: The `dotnet new fs-skia-ui` template (`template/**`,
  `.template.config/template.json`) does **not** need source changes for the
  framework contract itself. **Decision**: add the new mouse sample to the
  generated **Samples** capability fragment (`template/fragments/samples/`) so
  generated projects can demonstrate pointer interaction, mirroring the existing
  keyboard sample fragment; this is a `TemplateCheck`-gated change. Selected
  Controls guidance text that mentions input gains a pointer paragraph. No
  package-policy or command-surface template change.
- **Dependency impact**: **N/A — no dependency change.** No entry is added to
  `Directory.Packages.props`; Silk.NET (the source of `MouseButton`/`Scroll`) is
  already a pinned dependency of `FS.Skia.UI.SkiaViewer`. `docs/dependencies.md`
  and `DependencyReport` coverage are unchanged because no new package, version,
  or maintenance owner is introduced.
- **Command-surface impact**: No `build.fsx`/`scripts/build/**` routing-rule
  change is anticipated (no new package identity, no new gate). The change
  **escalates** under existing rules because it touches public `src/**/*.fsi`,
  the host contract, and `template/**`. Required gates are whatever
  `./fake.sh build -t Route` prints — expected to be the serialized
  maintainer-verify order, run sequentially (FAKE-backed targets share `.fake`
  state and are not safe to run concurrently):
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  New `.fsi` surface will additionally require `./fake.sh build -t
  RefreshSurfaceBaselines` and per-package surface regeneration
  (`PerPackageSurface.captureCurrent`) — the latter is **not** covered by
  `RefreshSurfaceBaselines` (see project memory).
- **Generated project impact**: Default/minimal generated contents are unchanged.
  The optional Samples capability gains the pointer sample (see Template
  ownership). Generated `Dev` behavior, placeholder scans, excluded-history scans,
  and validation logs are unaffected. Selected Controls guidance gains a short
  pointer-interaction note so generated guidance describes the new front door.
- **Evidence paths**: All real evidence lands under
  `specs/075-mouse-input-events/readiness/`:
  - `readiness/fsi/pointer-frontdoor.md` — FSI transcript exercising the packed
    `FS.Skia.UI.Controls` pointer front door (hover/click/drag/scroll) and the
    `Controls.Elmish` bridge.
  - `readiness/sample-smoke/PointerInteractionGallery.txt` — sample smoke log.
  - `readiness/package-surfaces/` + `readiness/package/` — moved per-package
    `.fsi.txt` snapshots and packed-library test output for `Controls`,
    `Controls.Elmish`, `SkiaViewer`.
  - `readiness/logs/` — `Dev`/gate logs from the serialized order.
  - `readiness/evidence-graph.md`, `readiness/evidence-audit.md`,
    `readiness/task-graph.{md,json}` — graph + audit artifacts.
  - `readiness/generated-product-verify/` — generated-product verification
    (non-authoritative locally; `GeneratedProductCheck` is a known local
    environment failure per project memory — record the environment-failure
    classification rather than treating it as a product defect).
- **`.fsi` / contract impact**: **Yes — Tier 1.** New/extended public signatures:
  (a) `FS.Skia.UI.SkiaViewer` host `ViewerEvent` extended with mouse-button
  identity on `PointerPressed`/`PointerReleased` and a new wheel/scroll case
  (`contracts/viewer-event.host.fsi`); (b) new `FS.Skia.UI.Controls` pointer front
  door — `PointerButton`, `PointerPhase`, `PointerSample`, `PointerInteraction`,
  `PointerState`, `PointerMsg`, and a `Pointer` module (`init`/`update`/`replay`/
  hit-test helper) (`contracts/pointer.controls.fsi`); (c)
  `FS.Skia.UI.Controls.Elmish` `interpretPointerEffect` plus any new
  `AdapterEffect` case (`contracts/pointer.controls-elmish.fsi`). Surface
  baselines and per-package `.fsi.txt` snapshots for these three packages move;
  `Layout` and `KeyboardInput` baselines are unchanged. Compatibility note:
  extending the `ViewerEvent` `PointerPressed`/`PointerReleased` case arity is a
  source-level change, but the only existing matcher returns `None`
  (`SkiaViewer.fs`), so blast radius is contained; documented in the contract.
- **MVU/effect boundary**: The pointer coordination layer is the MVU boundary.
  - `Model`: `PointerState` — current hover target, per-button press candidates
    (`Map<PointerButton, PressCandidate>`), active drag, last sampled position,
    and the drag threshold — composed alongside the existing `ControlRuntimeModel`
    (which continues to own `HoveredControl`/`PressedControls`/`ActiveDrag`/
    `FocusedControl`).
  - `Msg`: `PointerMsg` — `Move`/`Down`/`Up`/`Wheel`/`WindowExited`/`FocusLost`,
    each carrying coordinates and (where applicable) `PointerButton`/delta.
  - `Effect`/`Cmd<Msg>`: `PointerInteraction` list (consumer-facing
    hover-enter/leave, press, release, click, secondary-click, drag begin/move/
    end, scroll, plus stale-target/cancel diagnostics) — lowered to
    `AdapterCommand<'msg>`/`Cmd<'msg>` by `Controls.Elmish.interpretPointerEffect`.
  - `init`: initial `PointerState` (no hover, no press, configured threshold) +
    no startup effects.
  - `update`: pure `PointerMsg -> LayoutResult -> PointerState -> PointerState *
    PointerInteraction list` (curried, matching the `.fsi`; hit-testing via
    `Layout.hitTestComputed`; the
    `LayoutResult` is an input, never fetched inside `update`, preserving purity
    and testability).
  - Interpreter at the edge: the sample/host translates `ViewerEvent.Pointer*`
    into `PointerSample`/`PointerMsg`, runs `update`, and dispatches the lowered
    `Cmd<'msg>` — real evidence via the FSI transcript and sample smoke log.
- **Synthetic evidence**: **None planned as `[S]`.** All interaction tests use
  real, scripted pointer sequences against the real pure `update` and the real
  packed libraries — these are genuine deterministic inputs, not synthetic
  substitutes. The window-exit / focus-lost **cancel** path (FR-007) and the
  stale-target diagnostic (FR-010) validate explicit error/edge paths whose real
  host trigger is awkward to script deterministically; if a task can only be
  proven via a fabricated host signal it will be marked `[SEH]`
  (`synthetic-error-handling-approved`) with a Synthetic-Evidence Inventory row
  (design source, rationale, synthetic input class, expected error behavior,
  acceptance) — decided at task generation, never relabeled at implementation
  time. The sample's screenshot visual proof follows evidence-mode render-only
  honesty rules (`fs-skia-evidence-mode`).
- **Test evidence**: Failing-first semantic tests for each user story —
  hover-enter/leave ordering (US1/SC-001), click iff press+release on same control
  (US2/SC-002), drag begin/move/end + sub-threshold-is-click (US3/SC-003),
  secondary-button discrimination + independent per-button tracking (US4/SC-008),
  wheel delta addressed to control-under-pointer (US5/SC-009),
  cancel-on-window-exit/focus-loss (SC-004), and replay determinism (SC-005,
  identical outcomes on a re-run sequence). Governance/property tests:
  FsCheck properties for "no duplicate/skipped hover transitions" and "press/
  release pair never dropped/reordered under move bursts" (FR-003/FR-008).
  Packed-library FSI tests exercise the public front door; sample smoke test
  produces the readiness log. Keyboard-only regression (SC-006) re-runs an
  existing keyboard sample unchanged.
- **Observability**: Actionable diagnostics on the unresolved/stale paths
  (FR-010): a hit-test miss inside the window and a pointer event referencing a
  removed/relayouted control both emit a `PointerInteraction` diagnostic
  (carrying the offending coordinate/target) rather than dispatching to a wrong
  control — mirroring `ControlRuntimeEffect.StaleTarget`/`CancelledInteraction`
  and `Controls.Elmish`'s `ReportAdapterDiagnostic`. The cancel path
  (FR-007/SC-004) emits an explicit cancelled-interaction effect. Missing-artifact
  failures and unsupported-environment messages follow existing gate behavior;
  the GPU/Vulkan smoke caveat distinguishes implementation defects from
  window-system/presentation setup.
- **Deferred scope**: Out of scope for v1 and recorded as bounded follow-ups —
  double-click / multi-click counting (consumers can derive from click timing),
  gesture recognition beyond click/drag (pinch/rotate/multi-touch), touch/stylus
  pressure, OS cursor-shape/custom-cursor art, cross-window drag-and-drop,
  platform pointer-acceleration tuning, context-menu UI rendering (framework
  surfaces the secondary-button event, not a menu widget), and horizontal/
  high-resolution (precision) wheel beyond a signed delta per axis. Broader visual
  regression beyond the single sample's render-only screenshot is deferred.

## Project Structure

Real paths for this feature (existing files extended unless marked **new**):

```
src/
  SkiaViewer/
    Host/Diagnostics.fsi        # extend ViewerEvent: button on press/release + wheel case
    Host/Diagnostics.fs         # mirror the type change
    Host/Vulkan.fs              # capture MouseButton (drop the `_` discard); add IMouse.Scroll handler; window-exit signal
    SkiaViewer.fs               # pass-through (currently returns None for Pointer*)
  Controls/
    Pointer.fsi                 # **new** public front door: PointerButton/Phase/Sample/Interaction/State/Msg + Pointer module
    Pointer.fs                  # **new** pure init/update/replay + hit-test over Layout.hitTestComputed + ControlRuntime
    ControlRuntime.fsi/.fs      # extend only if per-button/explicit enter-leave effects are surfaced here (see research.md)
    Controls.fsproj             # add Pointer.fs(i) to the compile order
  Controls.Elmish/
    ControlsElmish.fsi          # **add** interpretPointerEffect (+ AdapterEffect case if needed)
    ControlsElmish.fs           # bridge PointerInteraction -> AdapterCommand<'msg>

samples/
  PointerInteractionGallery/    # **new** runnable sample: hover/click/drag/secondary/scroll wiring
    Program.fs                  # ViewerEvent.Pointer* -> PointerSample -> Pointer.update -> Elmish Cmd

template/fragments/samples/     # **add** pointer sample fragment so generated Samples capability includes it

readiness/per-package-surface/  # moved snapshots: FS.Skia.UI.Controls(.Elmish).fsi.txt, FS.Skia.UI.SkiaViewer.fsi.txt
specs/075-mouse-input-events/
  research.md  data-model.md  quickstart.md  contracts/  readiness/
```

**Architecture note (dependency boundaries)**: `FS.Skia.UI.Controls` must remain
host-independent — it depends on Scene/Layout/KeyboardInput and **not** on
`SkiaViewer`. The pointer front door therefore speaks a neutral `PointerSample`
(x/y/button/phase/delta), and the `ViewerEvent.Pointer* -> PointerSample`
translation lives in the consumer (the sample), exactly as
`samples/InteractiveViewer` already translates `ViewerEvent` for keyboard. This
keeps the existing acyclic project graph intact and adds no new package identity
or dependency edge. See `research.md` for the rejected alternatives (host-coupled
helper in `Controls.Elmish` or a new package).
