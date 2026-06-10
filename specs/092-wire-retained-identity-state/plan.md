# Implementation Plan: Wire Retained Identity Into Live Interactive State

**Branch**: `092-wire-retained-identity-state` | **Date**: 2026-06-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/092-wire-retained-identity-state/spec.md`

## Summary

Feature 091 wired the 067 keyed reconciler onto the render path through `module internal
RetainedRender`, but the *cross-frame identity* it computes (`StateByIdentity`, keyed by the
stable `RetainedId`) is never consumed by the live `ControlsElmish` host — focus and text input
still flow through the unstable path-derived `ControlId` maps inherited from feature 090. As a
result the headline E2 benefit (focus/text survive a positional shift) is proven only by tests
that hand-seed the identity map, not in the running app.

This feature connects the two halves: it re-keys the live interactive state (focus target,
text-input model, animation clock) onto `RetainedId` by hit-testing against the retained tree's
per-node boxes; folds the 090 focus/text-targeting defects that sit on the same path; and brings
the wired path's measured/documented behavior into agreement with reality (a distinct
shifted-work counter, theme in the fragment reuse key, single first-frame paint, first-frame
diagnostics). Output stays byte-identical to a full rebuild.

**Change tier: Tier 1 (contracted change).** Public surfaces move: `SkiaViewer.fsi`
(`InteractiveViewerHost.MapKey` widens to carry multiple messages — FR-006), `ControlsElmish.fsi`
(new/changed focus-routing seam functions over the retained structure), and the internal
`RetainedRender.fsi` (work-reduction + theme + first-frame contract). Surface baselines update
accordingly.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: SkiaSharp 4 preview (pinned), Elmish (host runtime); no new dependency.
**Testing**: Expecto + FsCheck in `tests/Controls.Tests` (reaches `RetainedRender` via
`InternalsVisibleTo("Controls.Tests")`) and `tests/Elmish.Tests` (adapter seams); FAKE targets via
the compiled front end; deterministic render-only evidence (structural `Scene`/identity equality —
no live Vulkan window required, per `fs-skia-evidence-mode`).
**Target Platform**: Windows and Linux (framework libraries; not platform-narrowed).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**` content, sample, or `.template.config/template.json`
  change. This is framework-library behavior; generated projects consume it transitively after the
  standard post-merge version bump + template pin refresh (no template authoring in this feature).
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`, `docs/dependencies.md`,
  and `DependencyReport` coverage are unchanged (no dependency added or version moved).
- **Command-surface impact**: No `build.fsx`/target *definitions* change and no new gate is added.
  Validation runs `Route` first, then only the printed gates. Because public `.fsi` files change,
  `Route` is expected to **escalate** to the consumer-contract tier; run the serialized FAKE-backed
  order sequentially (shared `.fake` state — never concurrent): 1. `./fake.sh build -t Dev`,
  2. `GeneratedGuidanceCheck`, 3. `TemplateCheck`, 4. `GeneratedProductCheck`, 5. `EvidenceGraph`,
  6. `EvidenceAudit`. Re-run sequentially if any failure looks race-like.
- **Generated project impact**: N/A — generated default/minimal contents, selected Controls guidance,
  local skills, and generated `Dev` behavior are unchanged. The behavior improvement reaches generated
  projects only via the refreshed package pins, not via scaffold edits.
- **Evidence paths**: `specs/092-wire-retained-identity-state/readiness/` — subfolders:
  `live-survival/` (focus + draft-text + clock survive a positional shift driven through the real
  adapter seam, plus the rebuild-every-frame baseline failing the same proof); `focus-resolution/`
  (keyed, unkeyed, and keyed-container-wrapped fields focus; pre-filled multi-line first-keystroke
  append); `work-reduction/` (changed vs shifted counters under a sibling-shifting change);
  `theme-reuse/` (byte-identity under a theme change); `multi-frame/` (3+-frame chained round-trip
  parity); plus the test log and surface-baseline diffs. Parity/survival proofs are authoritative as
  structural `Scene`/identity equality (SceneEvidence render functions are deterministic
  capability-hash functions, not pixel encoders — `fs-skia-evidence-mode`).
- **`.fsi` / contract impact**: **Tier 1 — yes.** `SkiaViewer.fsi`: `InteractiveViewerHost.MapKey`
  widens from `ViewerKey -> bool -> 'msg option` to a multi-message result (FR-006). `ControlsElmish.fsi`:
  focus-routing seam re-keyed to the retained structure (new/changed seam functions; package-surface
  rule applies — Controls.Elmish `.fsi` routes to package-surface, not controls-public-surface).
  `RetainedRender.fsi` (internal module, zero public-surface delta): `WorkReductionRecord` gains a
  shifted-work field; `RetainedRender<'msg>` gains the per-loop `Theme`; `init` returns first-frame
  diagnostics; a new retained-tree hit-test is added. Per-package and cross-package surface baselines
  are recaptured; a compatibility/migration note is added for the MapKey widening.
- **MVU/effect boundary**: The consumer host already owns `Model`/`Msg`/`update`/`init`; this feature
  does not add a public `Effect`/`Cmd`. The focus/text/animation state is **interpreter-edge state**
  held in the `runInteractiveApp` closure (mutable refs — allowed by Principle III for edge state),
  re-keyed from `ControlId` to `RetainedId`. The consumer `view`/`update` stay pure; the only mutation
  is at the edge interpreter. Pure transition tests assert the seam functions (given a retained state +
  input → next retained state + product messages); interpreter behavior is exercised through the real
  adapter seam (not a hand-seeded map) per SC-001.
- **Synthetic evidence**: The duplicate-key first-frame diagnostic test (FR-009/SC-005) uses a
  deliberately-malformed duplicate-keyed literal tree — an `[SEH]` `synthetic-error-handling-approved`
  error-path case (the diagnostic is produced by the real wired path; only the malformed input is a
  literal), mirroring 091's existing KeyCollision `[SEH]` test. All other evidence is real (real
  `RetainedRender`/`TextInput`/`Control` calls, real structural equality). No mocks/fakes/in-memory
  substitutes elsewhere; no ordinary `[S]` is planned.
- **Test evidence**: Failing-first Expecto/FsCheck tests: (a) `Feature092` live-survival driving the
  real focus→keystroke→shift→keystroke seam (no manual `StateByIdentity` seeding) + baseline-fails;
  (b) focus-resolution across keyed/unkeyed/wrapped fields + pre-filled multi-line append; (c) multiple
  change-bindings all dispatched; (d) work-reduction changed/shifted counters under a shift; (e) theme
  change → byte-identity; (f) chained 3+-frame round-trip; (g) the four 067/091 invariants still pass.
  Plus governance/surface-baseline tests for the `.fsi` deltas.
- **Observability**: First-frame and standing duplicate-key `KeyCollision` diagnostics surface through
  the host diagnostics channel (de-duped once per standing collision) — FR-009 closes the frame-0 gap.
  No silent swallow on the render or input path; the path stays total (Principle VII).
- **Deferred scope**: Caret/selection/IME/undo/redo/clipboard text-editing UX (E4); a theme-toggle
  *UI* (only reuse-correctness under a theme change is in scope); live windowed pixel-PNG capture;
  XAML/data-binding/dependency-property/lookless-template/CSS-selector capability (permanent non-goals).

**Gate result**: PASS — no unjustified violation. Tier 1 obligations (`.fsi` updates, surface
baselines, compatibility note, failing-first tests, evidence) are planned. The single complexity
note (widening a public seam for an edge requirement) is justified in research.md R6.

## Project Structure

```
src/
  Controls/
    RetainedRender.fs / .fsi      # WorkReductionRecord +ShiftedNodeCount; RetainedRender +Theme;
                                   # init returns first-frame diagnostics + single paint inputs;
                                   # new retainedHitTest (point -> RetainedId option);
                                   # theme folded into the fragment reuse decision (Keep/Update)
  Controls.Elmish/
    ControlsElmish.fs / .fsi      # focus/text/clock state re-keyed ControlId -> RetainedId;
                                   # focus-on-click resolves via retainedHitTest; TextInput seeded
                                   # from the control's current value + line-mode; all matched
                                   # change-bindings dispatched; renderRetained surfaces frame-0
                                   # diagnostics and paints once
  SkiaViewer/
    SkiaViewer.fsi (+ .fs)        # InteractiveViewerHost.MapKey widened to a multi-message result
tests/
  Controls.Tests/
    Feature092*.fs                # RetainedRender work-reduction/theme/first-frame/hit-test + invariants
  Elmish.Tests/
    Feature092*.fs                # live-survival, focus-resolution, multi-binding dispatch seams
specs/092-wire-retained-identity-state/
  plan.md research.md data-model.md quickstart.md contracts/ readiness/
```

**Structure decision**: Behavior lands in the existing `FS.Skia.UI.Controls` and
`FS.Skia.UI.Controls.Elmish` libraries plus a single public-seam widening in `FS.Skia.UI.SkiaViewer`.
No new project or package. Tests extend the existing `Controls.Tests` and `Elmish.Tests` suites;
`Controls.Elmish` already has `InternalsVisibleTo` wiring as needed and the seam functions stay on the
package surface (already routed to the package-surface rule).

## Phase 0 — Research

See [research.md](./research.md). Resolves: the FR-004 retained-tree hit-test mechanism and why it
disambiguates unkeyed siblings; how the live focus/text/clock state threads onto `RetainedId` through
`StateByIdentity` (FR-001/2/3); FR-005 value/line-mode seeding and the carried-draft-vs-model-value
conflict resolution; FR-006 seam widening + escalation justification; FR-007 shifted-counter accounting;
FR-008 theme-in-reuse-key; FR-009 single first-frame paint + first-frame diagnostics; the testable-seam
extraction that lets SC-001 drive the real adapter without a window.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — the retained/identity/interactive-state entities and their
  field-level deltas.
- [contracts/](./contracts/) — the `.fsi` signature deltas for `RetainedRender`, `ControlsElmish`,
  and `SkiaViewer`, with the compatibility/migration note for the MapKey widening.
- [quickstart.md](./quickstart.md) — the FSI-first walkthrough proving focus + draft text survive a
  positional shift through the real adapter seam.
- Agent context: the `AGENTS.md` SPECKIT plan pointer is updated to this plan.

## Phase 2 — (planning only; tasks generated by `/speckit-tasks`)

Constitution re-check after Phase 1 design: **PASS** — the design (data-model + contracts) keeps the
consumer `view`/`update` pure, confines mutation to the interpreter edge, adds no dependency, holds
output byte-identical to a full rebuild, and keeps every `.fsi`/baseline obligation in scope. No new
violation introduced by the design.
