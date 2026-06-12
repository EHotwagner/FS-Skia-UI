# Implementation Plan: Focus Visibility, Performance Instrumentation, and ControlsShowcase3 Feedback Follow-ups (feature 108)

**Branch**: `108-focus-and-perf-feedback` | **Date**: 2026-06-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/108-focus-and-perf-feedback/spec.md`

## Summary

Feature 108 closes the gap between what the offscreen/unit-test layer proves and
what running the real persistent window (`ControlsElmish.runInteractiveApp`)
surfaced in the sibling ControlsShowcase3 feedback: **focus that updates in the
model but paints nothing**, **pointer input that stalls under continuous
movement**, and **no per-frame signal to log, assert, or attach as evidence**.
Seven user stories, prioritised P1→P3:

1. **US1 — Focus visibility (P1).** Add a framework-supported entry
   `Focus.markFocused : ControlId option -> Control<'msg> -> Control<'msg>` that
   stamps `VisualState.Focused` on the control whose identity (`Key ?? structural
   path`, reusing feature 098's path identity) matches the focused id — exactly
   one, byte-identical when `None`. Driven by the existing `Focus.order`/`traverse`
   enumeration so traversal reaches **unkeyed** focusable controls (FR-002) and
   skips structural/non-focusable elements (FR-004). This generalises the
   consumer's hand-rolled `View.markFocused` workaround into the public surface
   (FR-001/003/005).
2. **US2 — Per-frame metrics (P1).** Surface a public `FrameMetrics` record from
   the host loop reporting `RemeasuredNodeCount` (from the existing
   `WorkReductionRecord`), `PointerSamplesReceived`, `PointerMovesProcessed`, and
   `ViewRebuilt: bool`, plus a separately-reported timing field excluded from the
   determinism guarantee (FR-006/007/008).
3. **US3 — Deterministic perf driver (P2).** A pure, headless frame-stepping entry
   (`Perf.runScript`) that folds an ordered `FrameInput` script over the pure host
   update + `RetainedRender.step`, advancing one frame per step and accumulating a
   byte-stable `FrameMetrics list` (FR-009/010). A generic, consumer-facing
   `SkillSupport.EvidenceTour.run` fold combinator covers the message-script case.
4. **US4 — Pointer-move coalescing (P2).** In `runInteractiveApp` and the pure
   stepper, coalesce continuous moves (`HoverEnter`/`HoverLeave`/`DragMove`) to **at
   most one processed move per frame**, keeping the latest position and retaining
   the path for drags; discrete interactions (press/release/click/drag
   begin/end/cancel/scroll/secondary) are never coalesced or dropped (FR-011/012).
   Event-driven tick stays the documented default; clocks still advance from the
   injected delta (FR-013).
5. **US5 — Composition/input ergonomics (P3).** `Control.map`/`Widget.map`
   (`('a -> 'b) -> Control<'a> -> Control<'b>`, structure/key/identity preserving,
   FR-014); DataGrid tri-state sort asc → desc → none (FR-015); a modifier-aware key
   boundary — parse `Ctrl/Alt/Shift/Meta` prefixes into a `KeyModifiers` value and
   deliver it through an additive `MapKeyChord` seam so chords are as dependable as
   plain keys (FR-016).
6. **US6 — Live theming (P3).** Reuse the shipped `Color.Contrast.ratio` for WCAG
   contrast; add `Theming.resolve` (theme mode + accent → role palette) and
   `Theming.toTheme` (project a role palette onto the framework `Theme`); document
   the render-path-vs-reuse-key split (FR-017/018).
7. **US7 — Discoverability (P3).** A host-seam authority note in
   `template/base/docs/scaffold-map.md` (FR-019) and a discoverable
   interactive-feature readiness checklist enumerating the window-visibility
   readiness files + `key=value` tokens (FR-020).

The change is architecture-preserving (constitution: declarative-retained MVU core,
no XAML/data-binding/CSS): no new open key-handler surface, no reflection, no new
runtime dependency. **At-rest output (no focus, `VisualState.Normal`, no pending
input) stays byte-identical** (SC-012). The deeper repaint optimisations from the
feedback survey (damage-rect repaint, hover-as-local-invalidation, backend
motion-event compression, `speckit.snapshot-source-tree`, ListView slicing) are
explicitly **out of scope** and deferred to bounded follow-ups.

This is a **Tier 1 (contracted)** change — public `.fsi` surface moves in `Focus`,
`Control`/`Widget`, `DataGrid`, `Theme`/`Theming`, `KeyboardInput`, and
`ControlsElmish` — and it is stateful host-loop work, but the MVU boundary is the
established `runInteractiveApp`/`RetainedRender` seam (features 091/092/094/096–103);
108 extends it, keeping `update` pure and all I/O at the host edge.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`,
`FS.Skia.UI.KeyboardInput`, `FS.Skia.UI.Color`, `FS.Skia.UI.SkillSupport` — **no
new package dependency**; SkiaSharp/Vulkan only transitively via the live evidence
host.
**Testing**: Expecto + FsCheck (`Check.One`; no `testProperty` in this repo), FAKE
targets, deterministic offscreen evidence, and a compiled self-closing host for the
responds-vs-renders artifact. Render proofs use **structural-Scene** equality
(`SceneEvidence.renderPng`/readback are deterministic capability-hash functions, not
pixel encoders — see `feature-091-reconciler-render-path-wiring`).
**Target Platform**: Windows and Linux. Live-window evidence via the X11 path
(`live-vulkan-window-x11-path`); a live Vulkan window is **not required** — offscreen
deterministic + responds-proof is sufficient (spec Assumptions).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Pre-Phase-0 evaluation**: PASS. Tier 1 contracted change with the full artifact
chain planned (spec ✓, plan, `.fsi` updates, surface + per-package baselines, tests,
real evidence). The stateful host-loop work reuses the landed `runInteractiveApp` /
`RetainedRender` MVU seam; `update` stays pure and pointer/key/tick are interpreted
only at the host edge (Principle IV). No new dependencies (Principle III / Engineering
Constraints). Closed, plain DU/record types are used throughout — no SRTP,
reflection, custom operators, or non-trivial computation expressions (Principle III).

**Post-Phase-1 re-evaluation**: PASS. The design (research.md, data-model.md,
contracts/) keeps visibility in `.fsi` (Principle II — no access modifiers on `.fs`
top-level bindings; `val internal` where cross-assembly-internal is required),
exercises the public surface through FSI-shaped tests (Principle I), prefers the
plainest F# (Principle III — pure tree-walks, an explicit `mutable` only on the host
loop's per-frame pointer-coalescing accumulator with a `// mutable: hot path / per
frame` disclosure), and routes all I/O-adjacent behaviour through the pure
stepper + host-edge interpreter (Principle IV). No synthetic evidence is planned
(Principle V); every evidence obligation has a real path.

### Repository Governance Decisions

- **Template ownership**: Touches `template/base/docs/scaffold-map.md` (FR-019 host-seam
  authority note) and adds a discoverable interactive-feature readiness checklist
  (FR-020) under `template/base/docs/` and/or a skill — both are template-shipped doc
  assets, so `.template.config/template.json` file lists are checked and updated if a
  **new** doc file is added (an edit to an existing tracked file needs no manifest
  change). No new selectable capability and no runtime scaffold-content change.
  Template package **pins** bump only at squash-merge via `speckit-merge` (the separate
  template version track), not in this feature's diff.
- **Dependency impact**: N/A — no new package or version. `Directory.Packages.props`,
  `docs/dependencies.md`, generated-template inclusion, and `DependencyReport` coverage
  are unchanged; 108 adds only F# types/behaviour within existing packages and reuses
  the already-pinned `Color.Contrast` and `SkillSupport.Random` surfaces.
- **Command-surface impact**: No new FAKE target. `Dev` runs the new unit/property/
  integration tests. Because the diff edits public `.fsi` across Controls /
  Controls.Elmish / KeyboardInput (and adds a `Theming` surface), `Route` **escalates**
  to the controls-public-surface (maintainer-verify) route, so the serialized order
  applies. `Route` is authoritative — run `./fake.sh build -t Route` against the real
  diff and run only the gates it prints (`--enforce` for missing evidence). FAKE-backed
  targets run **sequentially** in the deterministic order (shared `.fake` state):
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  `RefreshSurfaceBaselines` regenerates aggregate + per-package surface baselines (and
  the skillist/skill tree if a new skill is added for FR-020).
- **Generated project impact**: No default/minimal generated-content change and no
  scaffold-behaviour change. Generated apps gain focus-ring, per-frame metrics,
  pointer coalescing, `Control.map`, tri-state sort, modifier chords, and theming
  helpers for free once packages bump at merge. The two doc additions (FR-019/020) are
  guidance the generated project ships; no placeholder/excluded-history/`Dev`-behaviour
  change in the generated project.
- **Evidence paths**: under `specs/108-focus-and-perf-feedback/`:
  - `readiness/focus-ring/` — render-diff/structural-Scene proof that exactly the
    focused control carries the ring, per focusable kind, including an **unkeyed**
    focusable control (US1: SC-001/002).
  - `readiness/perf-metrics/frame-metrics.golden` — byte-stable count golden over a
    scripted input sequence; timing reported but excluded (US2/US3: SC-003/005).
  - `readiness/perf-metrics/coalescing.md` — N moves → 1 processed move; drag path
    preserved; click-during-move processed within one frame (US4: SC-004/006).
  - `readiness/responds-proof/` — interactive responds-proof
    (`ControlsElmish.respondsProofOf` / `captureRespondsProof`) for focus-on-key and
    pointer behaviour.
  - `readiness/control-map.md`, `readiness/tri-state-sort.md`,
    `readiness/modifier-chord.md` — US5 proofs (SC-007/008/009).
  - `readiness/theming-contrast.md` — WCAG reference pairs + render-path/reuse-key
    split demo (US6: SC-010).
  - The full **window-visibility-class** readiness set (interactive feature):
    `interactive-visible-window.md`, `close-reason-separation.md`,
    `window-state-diagnostics.md`, `window-options.md`, `real-image-evidence.md`,
    `generated-validation.md` — plus `skill-loading.md`, `readiness-contract.md`,
    `aggregate-hang-diagnostics.md`, `evidence-audit.md` (verdict token), and
    `generated-validation.md` (package-resolution=resolved, package-mismatch=false).
  - Recaptured published api-surface + per-package `.fsi.txt` baselines for every
    edited module, plus `EvidenceGraph`/`EvidenceAudit` output.
- **`.fsi` / contract impact**: **Tier 1.** Signatures change in:
  `src/Controls/Focus.fsi` (`markFocused`); `src/Controls/Control.fsi` +
  `src/Controls/Widget.fsi` (`map`); `src/Controls/DataGrid.fsi` (tri-state sort —
  `SortBy` cycle and/or a `ClearSort` case + `DataGridSort option` clearing);
  `src/Controls/Theme.fsi` or a new `src/Controls/Theming.fsi`
  (`resolve`/`toTheme`); `src/KeyboardInput/KeyboardInput.fsi` (`KeyModifiers` +
  modifier-aware normalize); `src/Controls.Elmish/ControlsElmish.fsi`
  (`FrameMetrics`, `FrameInput`, `Perf.runScript`, the additive `MapKeyChord` field
  on `InteractiveAppHost`, an opt-in `OnFrameMetrics` sink); and
  `src/SkillSupport/EvidenceTour.fsi` (generic fold). `Color.Contrast.ratio` is reused
  (no Color `.fsi` change anticipated). Every `.fsi` edit requires recaptured
  published api-surface + per-package baselines
  (`PerPackageSurface.captureCurrent`; `RefreshSurfaceBaselines` does **not** cover the
  per-package `.fsi.txt` snapshots — regenerate those explicitly). Compatibility:
  adding the `MapKeyChord`/`OnFrameMetrics` fields to `InteractiveAppHost` is a
  record-construction change — every framework construction site (samples, FSI
  preludes, generated host) is updated in the same change; the additive
  `MapKeyChord`/`OnFrameMetrics` carry inert defaults so unmodified-key and
  no-metrics behaviour is byte-identical. No consumer API rename.
- **MVU/effect boundary**: Stateful host input/render routing reusing the landed
  `runInteractiveApp`/`RetainedRender` seam. `Model` — the consumer's `'model`
  (unchanged) plus the host's retained state (`RetainedRender.StateByIdentity`,
  pointer `Pointer.state`, focus identity) and a new **per-frame pointer-coalescing
  accumulator** (latest move + retained drag path, reset each frame). `Msg` — the
  consumer's `'msg` plus the internal host messages; the modifier-aware boundary
  delivers a `ViewerKey * KeyModifiers` (or a `MapKeyChord` call) without changing
  `update` purity. `Effect`/`Cmd` — none new; coalescing and metrics produce data, not
  I/O. `init` — unchanged. `update` — `Focus.markFocused`, `Control.map`, the DataGrid
  tri-state transition, modifier parsing, and the `Perf.runScript` fold are **pure**;
  pointer coalescing and metric accumulation happen in the host-edge interpreter, not
  in any consumer `update`. Evidence: pure transition/property tests on every pure
  function + the deterministic `Perf.runScript` golden through the real
  `RetainedRender` seam (no hand-seeded identity map), plus the live responds-vs-renders
  capture.
- **Synthetic evidence**: **None planned.** Every evidence obligation has a real path:
  structural-Scene focus-ring diffs over real `renderTree`, a real deterministic
  `FrameMetrics` golden through `RetainedRender.step`, real `Color.Contrast.ratio`
  against the published WCAG reference pairs, `Control.map`/tri-state/modifier property
  proofs over the public surface, and a live-host responds-proof. No `[S]`/`[SEH]` task
  is anticipated. If a live-window capture proves infeasible in the run environment,
  the render-only deterministic + responds-proof path is the documented fallback and any
  residual gap is disclosed per Principle V, not silently greened.
- **Test evidence**: Failing-first semantic tests, story-grouped:
  - `tests/Controls.Tests/Feature108*` — `Focus.markFocused` stamps exactly one
    `Focused` (keyed and unkeyed), `None` byte-identical, structural elements skipped;
    `Control.map`/`Widget.map` structural-equivalence + identity preservation
    (`Check.One`); DataGrid tri-state asc→desc→none; `Theming.resolve`/`toTheme` +
    `Contrast.ratio` WCAG reference pairs.
  - `tests/Elmish.Tests/Feature108*` — `Perf.runScript` byte-stable `FrameMetrics`
    golden; idle frame zero re-measure + no rebuild; pure-hover frame no full rebuild;
    K moves → ≤1 processed move + ≤1 hit-test; click-during-move processed within one
    frame; event-driven tick advances clocks with no rebuild; modifier-chord delivery.
  - `tests/KeyboardInput.Tests/Feature108*` — modifier parsing (Ctrl/Alt/Shift/Meta
    prefixes → base key + `KeyModifiers`); unmodified keys unchanged.
  - `tests/SkillSupport.Tests/Feature108*` — `EvidenceTour.run` byte-stable outcome.
  - Governance: recaptured surface baselines validated by the per-module surface-area
    test; updated stale baseline-bearing tests if the `.fsi` shape moves.
  Each test fails on a pre-108 build (no `markFocused`/`map`; bi-state sort; per-sample
  pointer processing; shift-only key boundary).
- **Observability**: The `FrameMetrics` record **is** the new structured diagnostic —
  it turns invisible per-frame work into an observable, loggable, assertable signal
  (Principle VII), surfaced via an opt-in `OnFrameMetrics` sink and the `Perf.runScript`
  outcome. Honest failure modes are surfaced as explicit no-ops asserted in tests
  (idle frame = zero work, coalesced burst = one move) so "nothing happened" is a
  verified outcome, not a swallowed error. A missing required evidence artifact fails
  `Route --enforce` (named artifact + requiring tier). Timing is reported separately and
  excluded from golden assertions, never used to weaken a count assertion.
- **Deferred scope**: Current obligations = the seven user stories above with
  representative (not full-52) control coverage for the focus ring, and pointer
  coalescing as the in-scope perf fix. Explicitly deferred bounded follow-ups (spec Out
  of Scope): damage-rect/dirty-region repaint, hover-as-local-invalidation re-stamp,
  X11/Wayland backend motion-event compression, `speckit.snapshot-source-tree` tooling,
  and consumer-side ListView visible-window slicing. No new platform/distribution
  target; no live-Vulkan-window requirement.

## Project Structure

```
specs/108-focus-and-perf-feedback/
├── spec.md                         # feature specification (input)
├── plan.md                         # this file
├── research.md                     # Phase 0 — resolved decisions
├── data-model.md                   # Phase 1 — closed type surface + placement
├── contracts/
│   ├── Focus.markFocused.fsi       # focus-stamp entry sketch
│   ├── ControlWidget.map.fsi       # Control.map / Widget.map sketch
│   ├── DataGrid.tristate.fsi       # tri-state sort sketch
│   ├── Theming.fsi                 # resolve / toTheme + contrast reuse sketch
│   ├── KeyModifiers.fsi            # modifier-aware key boundary sketch
│   ├── HostMetrics.fsi             # FrameMetrics / FrameInput / Perf.runScript / host fields
│   ├── EvidenceTour.fsi            # generic SkillSupport fold sketch
│   └── behavior.md                 # coalescing + metrics + focus-order behaviour contract
├── quickstart.md                   # Phase 1 — consumer + maintainer walkthrough
├── checklists/
│   └── requirements.md             # spec quality checklist
└── readiness/                      # (created at implementation) evidence artifacts

src/Controls/
├── Focus.fsi / Focus.fs            # markFocused (path-identity stamp, order-driven)
├── Control.fsi / Control.fs        # Control.map
├── Widget.fsi / Widget.fs          # Widget.map
├── DataGrid.fsi / DataGrid.fs      # tri-state sort (asc→desc→none)
├── Theme.fsi|Theming.fsi / *.fs    # Theming.resolve / toTheme (Theme is here)
└── ControlRuntime.fs               # applyRuntimeVisualState parity unaffected at Normal

src/KeyboardInput/
└── KeyboardInput.fsi / .fs         # KeyModifiers + modifier-aware normalizeEvent

src/Controls.Elmish/
└── ControlsElmish.fsi / .fs        # FrameMetrics, FrameInput, Perf.runScript,
                                    # MapKeyChord + OnFrameMetrics host fields,
                                    # pointer-move coalescing, event-driven tick default

src/SkillSupport/
└── EvidenceTour.fsi / .fs          # generic ordered-Msg fold combinator

template/base/docs/
├── scaffold-map.md                 # FR-019 host-seam authority note
└── interactive-readiness.md (new)  # FR-020 readiness checklist (or skill section)

tests/Controls.Tests/Feature108*.fs
tests/Elmish.Tests/Feature108*.fs
tests/KeyboardInput.Tests/Feature108*.fs
tests/SkillSupport.Tests/Feature108*.fs
```

## Phase 0 — Outline & Research

Complete. See [research.md](./research.md): resolved decisions covering focus-stamp
identity (reuse 098 `Key ?? path`), focus-driven-by-`Focus.order`, `FrameMetrics`
field set + timing exclusion, the pure `Perf.runScript` stepper vs. the generic
`EvidenceTour` fold, pointer-coalescing policy (moves only, latest position, drag
path retained), the modifier-aware boundary shape (parse prefixes → `KeyModifiers`;
deliver via additive `MapKeyChord`), **theming-helper placement** (Controls, not
SkillSupport, because `toTheme` projects onto the Controls `Theme` type — divergence
from the spec's tentative "skill-support surface" wording, justified by package
layering), and the doc/checklist homes. No `NEEDS CLARIFICATION` remains.

## Phase 1 — Design & Contracts

Complete. [data-model.md](./data-model.md) (closed type surface + placement +
identity/parity invariants), [contracts/](./contracts/) (`.fsi` sketches + the
coalescing/metrics/focus-order behaviour contract), [quickstart.md](./quickstart.md)
(consumer focus-ring + live-theming walkthrough and maintainer evidence walkthrough).
`AGENTS.md` SPECKIT plan reference updated to this plan.

## Phase 2 — Next command

`/speckit-tasks` will break this into story-grouped tasks (US1 focus P1, US2 metrics
P1, US3 driver P2, US4 coalescing P2, US5 ergonomics P3, US6 theming P3, US7 docs P3)
with `tasks.deps.yml` + `skillist` metadata. Applicable capability skills:
`fs-skia-ui-widgets` (Controls focus/map/sort/theming), `fs-skia-controls-host` +
`fs-skia-elmish` (host metrics/coalescing/tick/key boundary), `fs-skia-keyboard-input`
(modifier parsing), `fs-skia-design-tokens` (theming helpers + live-theme pattern),
and `fs-skia-evidence-mode` (deterministic driver + responds-proof + window-visibility
readiness).
