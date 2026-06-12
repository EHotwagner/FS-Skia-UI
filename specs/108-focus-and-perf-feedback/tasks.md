# Tasks: Focus Visibility, Performance Instrumentation, and ControlsShowcase3 Feedback Follow-ups

**Feature branch**: `108-focus-and-perf-feedback`
**Spec**: `specs/108-focus-and-perf-feedback/spec.md`
**Plan**: `specs/108-focus-and-perf-feedback/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. **None planned for this feature** —
the plan's Synthetic-evidence section declares every obligation has a real path.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the change is reachable from a
user-facing entry point and that path was actually exercised (FSI against the
packed library, a responds-proof capture, or a readiness artifact under
`readiness/`). Core/model changes whose unit tests pass green do **not** satisfy
`[X]` for a `[US*]` task. For the stateful host-loop stories (US2/US3/US4/US5),
`[X]` also requires the public `InteractiveAppHost` contract exercised, pure
`update`/stepper transitions tested, the per-frame metrics asserted, and the
host-edge interpreter run through `Perf.runScript` / the live responds-proof.

## Success-criterion → assertion mapping

Each headline SC is paired with a concrete enforcing assertion, noted on the task
line as `(SC-00x)`:
- SC-001/002 — `Feature108` focus tests assert exactly-one `Focused` per kind incl. unkeyed (T008/T010).
- SC-003/005 — byte-stable `FrameMetrics` golden + idle/pure-hover no-rebuild asserts (T013/T015/T017).
- SC-004/006 — K-moves→≤1-processed + click-during-move-within-one-frame asserts (T021).
- SC-007/008/009 — `Control.map` `%A` structural-equality, tri-state cycle, modifier round-trip (T026).
- SC-010 — `Contrast.ratio` vs WCAG reference pairs (T032).
- SC-011 — host-seam authority + readiness-checklist discoverable from in-repo docs (T036/T037).
- SC-012 — at-rest byte-identity asserted by `markFocused None ≡ tree` + inert host-field defaults (T008/T013).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US7]** — user-story scope
- **[T1]** — this whole feature is Tier 1 (contracted); per-task tier omitted (matches spec).

Every task has a matching entry in `tasks.deps.yml`; every line mirrors the
structured `skillist` via `[skillist: ...]` (`[skillist: []]` when empty).

## Canonical Verification Targets

`Route` is authoritative — run `./fake.sh build -t Route` against the real diff and
run only the gates it prints (`--enforce` for missing evidence). This feature edits
public `.fsi` across Controls / Controls.Elmish / KeyboardInput and adds a `Theming`
surface, so `Route` **escalates** to the controls-public-surface (maintainer-verify)
route. FAKE-backed targets share `.fake` state and run **sequentially** in the
deterministic order:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

`RefreshSurfaceBaselines` regenerates aggregate + per-package surface baselines (and
the skillist/skill tree if a new skill section is added). The per-package `.fsi.txt`
snapshots are recaptured explicitly via `PerPackageSurface.captureCurrent`.

**Governance risk levels**: small = framework-internal `.fs` only (inner-loop `Dev`);
medium = test/readiness/doc additions; broad = public `.fsi` surface move + new
`Theming` module + new `InteractiveAppHost` fields (this feature → full
controls-public-surface route). Broad validation is required here; non-authoritative
aggregate results (e.g. an aggregate-hang focused rerun) are recorded in
`readiness/aggregate-hang-diagnostics.md`.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm `specs/108-focus-and-perf-feedback/` artifacts (spec, plan, research, data-model, contracts, quickstart, checklists) are present and link spec + plan from this task list
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold `readiness/` placeholders discoverable before implementation — `focus-ring/`, `perf-metrics/` (`frame-metrics.golden`, `coalescing.md`), `responds-proof/`, the window-visibility-class set (`interactive-visible-window.md`, `close-reason-separation.md`, `window-state-diagnostics.md`, `window-options.md`, `real-image-evidence.md`, `generated-validation.md`), plus `skill-loading.md`, `readiness-contract.md`, `aggregate-hang-diagnostics.md`, `governance-risk-levels.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, and `evidence-audit.md`; each names the authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier (Tier 1 contracted), affected packages (Controls, Controls.Elmish, KeyboardInput, SkillSupport), public-API impact, MVU applicability, and evidence obligations in `readiness/governance-risk-levels.md`

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-ui-widgets, fs-skia-controls-host, fs-skia-keyboard-input] Draft the public `.fsi` signatures per `data-model.md`/`contracts/`: `Focus.markFocused`; `Control.map` / `Widget.map`; DataGrid tri-state sort; `Theming.resolve`/`toTheme` + `RolePalette` (new `Theming.fsi`); `KeyModifiers` + `normalizeEventWithModifiers`; `FrameMetrics` / `FrameInput` / `Perf.runScript` + additive `MapKeyChord` / `OnFrameMetrics` host fields; `EvidenceTour.run` (new). No access modifiers on `.fs` top-level bindings; `val internal` for cross-assembly-internal helpers
- [X] T005 [skillist: fs-skia-evidence-mode] Exercise the draft `.fsi` from FSI (`scripts/*-prelude.fsx` or ad-hoc), including representative focus-stamp and host-field construction paths, and capture the transcript to `readiness/fsi-session.txt`
- [X] T006 [skillist: []] Record surface-area baselines (aggregate + per-package `.fsi.txt`) for the new/changed public modules before implementation moves them
- [X] T007 [skillist: fs-skia-evidence-mode] Record unsupported-scope handling and failure diagnostics in `readiness/runtime-limitations.md` — deferred damage-rect/hover-local/backend motion compression; offscreen + responds-proof is the documented evidence path, live Vulkan window not required

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 — Focus visibility (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-ui-widgets] Failing-first tests `tests/Controls.Tests/Feature108Focus*`: `Focus.markFocused` stamps exactly one `Focused` on the identity (`Key ?? path`) for keyed **and** unkeyed focusable controls; `markFocused None tree` is structural-Scene-identical to `tree` (SC-012); structural/non-focusable elements are skipped (FR-004); a consumer-set non-Normal state (e.g. Disabled) wins (SC-001/002, FR-001..005)

### Implementation

- [X] T009 [US1] [skillist: fs-skia-ui-widgets] Implement `Focus.markFocused` in `src/Controls/Focus.fs`/`.fsi` — `Focus.order`/`traverse`-driven, `Key ?? structural path` identity (feature 098), stamps `VisualState.Focused` on exactly the matching control, byte-identical when `None`
- [X] T010 [US1] [skillist: fs-skia-evidence-mode] Render-diff / structural-Scene evidence under `readiness/focus-ring/` proving exactly the focused control carries the ring for each focusable kind (button, slider, text box, radio group, switch) **including an unkeyed focusable control** (SC-001/002)
- [X] T011 [US1] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Capture an interactive responds-proof (`ControlsElmish.respondsProofOf` / `captureRespondsProof`) for focus-on-key under `readiness/responds-proof/`
- [X] T012 [US1] [skillist: []] Document the US1 independent validation path (the multi-control focus-traversal walkthrough) in `readiness/focus-ring/README.md`

**Checkpoint**: US1 is independently functional and testable.

---

## Phase 4: User Story 2 — Per-frame metrics (US2, P1)

### Tests First

- [X] T013 [P] [US2] [skillist: fs-skia-elmish, fs-skia-evidence-mode] Failing-first tests `tests/Elmish.Tests/Feature108Metrics*`: `FrameMetrics` count fields are byte-stable across repeated runs of one script; an idle frame reports `RemeasuredNodeCount = 0` and `ViewRebuilt = false`; a pure-hover frame reports no full rebuild; a `Tick` frame that drives an **active animation cross-fade** reports `ViewRebuilt = false` and a bounded (overlay-assembly, non-whole-tree) `RemeasuredNodeCount` — the cross-fade overlay path is not counted as a false full rebuild (spec Edge Case); `FrameDuration` excluded from the golden (SC-003/005/012, FR-006/007/008)

### Implementation

- [X] T014 [US2] [skillist: fs-skia-controls-host, fs-skia-elmish] Implement the `FrameMetrics` record + per-frame metric accumulation in the host loop and the additive `OnFrameMetrics` sink (inert default → at-rest byte-identical) in `src/Controls.Elmish/ControlsElmish.fs`/`.fsi`; update every `InteractiveAppHost` construction site (samples, FSI preludes, generated host) for the new field
- [X] T015 [US2] [skillist: fs-skia-evidence-mode] Produce `readiness/perf-metrics/frame-metrics.golden` — byte-stable count golden over a scripted input sequence (timing reported separately, excluded) (SC-003)
- [X] T016 [US2] [skillist: []] Document the US2 independent validation path

**Checkpoint**: US2 is independently functional and testable.

---

## Phase 5: User Story 3 — Deterministic perf driver (US3, P2)

### Tests First

- [X] T017 [P] [US3] [skillist: fs-skia-evidence-mode, fs-skia-elmish] Failing-first tests: `Perf.runScript` produces a byte-stable `FrameMetrics list` for a scripted `FrameInput` sequence; `tests/SkillSupport.Tests/Feature108*` asserts `EvidenceTour.run` byte-stable outcome; assertions expressible for pure-hover-no-rebuild and idle-zero-remeasure (SC-003/005, FR-009/010)

### Implementation

- [X] T018 [US3] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Implement `Perf.runScript` — pure, headless fold of an ordered `FrameInput` script over the host's pure update + `RetainedRender.step`, one frame per step, sharing the coalescing/step code path with `runInteractiveApp`
- [X] T019 [US3] [skillist: fs-skia-evidence-mode] Implement `SkillSupport.EvidenceTour.run` generic ordered-`Msg` fold combinator in new `src/SkillSupport/EvidenceTour.fs`/`.fsi`, beside the shipped `SkillSupport.Random`
- [X] T020 [US3] [skillist: []] Document the US3 independent validation path (the deterministic driver walkthrough)

**Checkpoint**: US3 is independently functional and testable.

---

## Phase 6: User Story 4 — Pointer-move coalescing (US4, P2)

### Tests First

- [X] T021 [P] [US4] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Failing-first tests: K pointer-move samples in one frame → `PointerMovesProcessed ≤ 1` and `PointerSamplesReceived = K` (SC-004); a drag spanning samples preserves the coalesced path (FR-012); a click interleaved with moves is processed within one frame (SC-006); an idle event-driven tick advances animation clocks from the injected delta with no rebuild (FR-013)

### Implementation

- [X] T022 [US4] [skillist: fs-skia-controls-host, fs-skia-elmish] Implement pointer-move coalescing in `runInteractiveApp` + the shared stepper — moves only (`HoverEnter`/`HoverLeave`/`DragMove`), keep latest position, retain drag path; discrete interactions (press/release/click/drag begin/end/cancel/scroll/secondary) never coalesced or dropped; per-frame coalescing accumulator with a `// mutable: hot path / per frame` disclosure (FR-011/012)
- [X] T023 [US4] [skillist: fs-skia-controls-host] Make the event-driven interactive tick the documented default — no frame work scheduled when no input arrives, while active animation clocks still advance from the injected delta (FR-013)
- [X] T024 [US4] [skillist: fs-skia-evidence-mode] Produce `readiness/perf-metrics/coalescing.md` — N moves → 1 processed move, drag-path fidelity preserved, click-during-move processed within one frame (SC-004/006)
- [X] T025 [US4] [skillist: []] Document the US4 independent validation path

**Checkpoint**: US4 is independently functional and testable.

---

## Phase 7: User Story 5 — Composition & input ergonomics (US5, P3)

### Tests First

- [X] T026 [P] [US5] [skillist: fs-skia-ui-widgets, fs-skia-keyboard-input] Failing-first tests `tests/Controls.Tests/Feature108Map*` + `tests/KeyboardInput.Tests/Feature108*`: `Control.map`/`Widget.map` lower structurally equal to authoring directly in `'b` and preserve keys/focus identity (`%A` projection, `Check.One`) (SC-007); DataGrid sort cycles asc → desc → none on the third toggle (SC-008); `normalizeEventWithModifiers` parses `Ctrl/Alt/Shift/Meta` prefixes (any order, case-insensitive) to base key + `KeyModifiers`, unmodified keys byte-identical (SC-009, FR-014/015/016)

### Implementation

- [X] T027 [US5] [skillist: fs-skia-ui-widgets] Implement `Control.map` (`src/Controls/Control.fs`/`.fsi`) and `Widget.map` (`= ofControl ∘ Control.map f ∘ toControl`) — change only the message type, preserve `Kind`/`Key`/`Content`/`Accessibility`/`Children` shape and focus identity (FR-014)
- [X] T028 [US5] [skillist: fs-skia-ui-widgets] Implement the DataGrid tri-state sort cycle in `src/Controls/DataGrid.fs` (`None → Asc → Desc → None`; a different column restarts at `Asc`; `DataGridSortChanged None` fires on the clearing transition) with no `.fsi` type change (FR-015)
- [X] T029 [US5] [skillist: fs-skia-keyboard-input, fs-skia-controls-host] Implement `KeyModifiers` + `noModifiers` + `normalizeEventWithModifiers` in `src/KeyboardInput/KeyboardInput.fs`/`.fsi` and the additive `MapKeyChord` field on `InteractiveAppHost` (consulted before `MapKey`, inert default) in `ControlsElmish`; update every construction site (FR-016)
- [X] T030 [US5] [skillist: fs-skia-evidence-mode] Produce `readiness/control-map.md`, `readiness/tri-state-sort.md`, and `readiness/modifier-chord.md` proofs (SC-007/008/009)
- [X] T031 [US5] [skillist: []] Document the US5 independent validation path

**Checkpoint**: US5 is independently functional and testable.

---

## Phase 8: User Story 6 — Live theming (US6, P3)

### Tests First

- [X] T032 [P] [US6] [skillist: fs-skia-design-tokens] Failing-first tests `tests/Controls.Tests/Feature108Theming*`: `Theming.resolve` (mode + accent → `RolePalette`) and `Theming.toTheme` (role palette → `Theme`); `Color.Contrast.ratio` matches the WCAG relative-luminance reference for known pairs and the AA thresholds (≥4.5:1 normal, ≥3:1 large) are checkable (SC-010, FR-017/018)

### Implementation

- [X] T033 [US6] [skillist: fs-skia-design-tokens, fs-skia-ui-widgets] Implement `Theming.resolve`/`toTheme` + `RolePalette` in new `src/Controls/Theming.fs`/`.fsi`, reusing `FS.Skia.UI.Color.Contrast.ratio` (no Color `.fsi` change) (FR-017)
- [X] T034 [US6] [skillist: fs-skia-design-tokens, fs-skia-evidence-mode] Document the supported live-theming render-path-vs-reuse-key split (model-derived paint theme on the render path, static `host.Theme` for the reuse key) and capture `readiness/theming-contrast.md` with the WCAG reference pairs + demo (FR-018, SC-010)
- [X] T035 [US6] [skillist: []] Document the US6 independent validation path

**Checkpoint**: US6 is independently functional and testable.

---

## Phase 9: User Story 7 — Discoverability & governance (US7, P3)

- [X] T036 [US7] [skillist: fs-skia-controls-host] Add the host-seam authority note to `template/base/docs/scaffold-map.md` (FR-019) — the `Controls.Elmish` `runInteractiveApp` / `InteractiveAppHost` / `PointerInteraction` seam is "present in package, not in `docs/api-surface/` — authority is the `fs-skia-controls-host` skill + `ControlsElmish.fsi`," alongside the typed-front-door absence note
- [X] T037 [US7] [skillist: fs-skia-evidence-mode] Add the discoverable interactive-feature readiness checklist (`template/base/docs/interactive-readiness.md` and/or a skill section) enumerating the window-visibility-class readiness files + required `key=value` tokens an interactive `EvidenceAudit` demands; update `.template.config/template.json` file lists if a new doc file is added (FR-020)
- [X] T038 [US7] [skillist: []] Document the US7 independent validation path — a reader can identify the host-seam authority and enumerate the readiness files/tokens from in-repo docs alone before running `EvidenceAudit` (SC-011)

**Checkpoint**: US7 is independently functional and testable.

---

## Phase 10: Integration & Polish

- [X] T039 [skillist: []] Run `RefreshSurfaceBaselines` (aggregate + per-package surface baselines, skill tree if a section was added) and recapture per-package `.fsi.txt` (`PerPackageSurface.captureCurrent`) for every edited module; confirm `./fake.sh build -t Route --enforce` passes with required evidence present
- [X] T040 [skillist: fs-skia-evidence-mode] Complete the window-visibility-class readiness set with honest values — `interactive-visible-window.md`, `close-reason-separation.md`, `window-state-diagnostics.md`, `window-options.md`, `real-image-evidence.md`, and `generated-validation.md` (`package-resolution=resolved`, `package-mismatch=false`)
- [X] T041 [skillist: speckit-implement] Record the skill-loading evidence workflow in `readiness/skill-loading.md` — one skill-loading note per `[X]` task, the red-green evidence log, graph before/after paths around each status change, governance risk levels, and non-authoritative aggregate reporting
- [X] T042 [skillist: fs-skia-template-update] Run the serialized FAKE order sequentially: `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` (shared `.fake` state; rerun sequentially on any race-like failure)
- [X] T043 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; record before/after graph paths
- [X] T044 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (0 synthetic) and `evidence-audit.md` carries its verdict token

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — plan declares every obligation has a real path)_ | | | | | | | | |
