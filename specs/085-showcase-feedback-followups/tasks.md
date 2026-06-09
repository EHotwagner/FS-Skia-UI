# Tasks: ControlsShowcase Consumer Feedback Follow-ups

**Feature branch**: `085-showcase-feedback-followups`
**Spec**: `specs/085-showcase-feedback-followups/spec.md`
**Plan**: `specs/085-showcase-feedback-followups/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/evidence-graph.md` for the propagated view.

`[SEH]` is a design-time annotation paired with `synthetic-error-handling-approved`;
none are anticipated for this feature (see plan "Synthetic evidence").

## Vertical-slice rule (US phases)

A `[US*]` task may only be `[X]` when the change is reachable from a user-facing
entry point and that path was actually exercised — an FSI session against the
packed library, a host launch, a manual walk-through with transcript, or a
screenshot under `readiness/`. For the I/O-bearing pointer story (US2), `[X]`
also requires MVU evidence: the `InteractiveAppHost` `Init`/`Update`/`MapPointer`
contract exercised, pure `Update` transitions tested, emitted `ViewerEffect`s
asserted, and the `runInteractiveApp` interpreter run against the real
host/adapter path. Domain/unit-test-only passes do **not** satisfy `[US*]` `[X]`.

## Success-criterion → assertion mapping

- **SC-001** ⇒ T010 (two structurally different trees ⇒ `Scene` differs; nested
  children painted) + T013 (non-empty per-page screenshot diff).
- **SC-002** ⇒ T016 (synthetic pointer press through `runInteractiveApp`
  dispatches the bound `msg` and changes the model) + T018 (durable visible-window
  launch) + T019 (host-observable pointer-dispatch evidence — satisfies the
  "observable from the host, not only from headless tests" clause).
- **SC-003** ⇒ T020 (five spellings map; unknown stays `Unknown raw`; totality).
- **SC-004** ⇒ T023 (two extents, no fixed-size upscale) + T025 (one documented
  workaround flag).
- **SC-005** ⇒ T027 (typed-controls consumer note + `catalog.yml` `module:` probe
  recipe so availability is confirmed without DLL reflection or `docs/api-surface/`)
  + `SkillQualityCheck` (T034 regeneration asserts the guidance is present).
- **SC-006** ⇒ T011 (Feature-080 preview goldens green; `runApp` literal intact)
  + T033/T034/T036/T037.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase). FAKE-backed targets
  are never `[P]`: they share `.fake` state and must run sequentially.
- **[US1]**..**[US5]** — user-story scope. **[T1]** — Tier 1 (escalated
  `maintainer-verify`), the classification for the whole feature; omitted on lines
  where it matches the feature default.

Every task has a matching `tasks.deps.yml` entry and mirrors its structured
`skillist` via `[skillist: ...]` (`[skillist: []]` when none applies).

## Governance risk level

- **Small / focused** — the `normalize` behavior fix (US3) and the doc/skill edits
  (US5): focused validation = the relevant unit tests + `GeneratedGuidanceCheck` /
  `SkillSyncCheck`.
- **Medium** — the size-aware view (US4): host render tests + `Dev`.
- **Broad** — new public `.fsi` surface (US1 `renderTree`, US2 pointer host): broad
  validation = the escalated six-target order. `GeneratedProductCheck` is known to
  fail locally for environment reasons and is recorded as **non-authoritative**
  (see memory `generated-product-check-env-failure`); aggregate hang/results
  diagnostics are recorded in `readiness/aggregate-hang-diagnostics.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory and link `spec.md` + `plan.md`; record the branch and the escalated Tier-1 `maintainer-verify` classification
- [X] T002 [P] [skillist: []] Scaffold audit-enforced readiness files discoverable before implementation: `readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, `readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md`, `readiness/real-image-evidence.md`, `readiness/generated-guidance-validation.md`, `readiness/framework-guidance.md`, `readiness/evidence-vocabulary.md`, `readiness/evidence-graph.md`, `readiness/evidence-audit.md`, and the window-visibility class (`interactive-visible-window.md`, `close-reason-separation.md`, `window-state-diagnostics.md`, `window-options.md`, `generated-validation.md`) — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Record the affected layer, additive public-API impact (FR-001, FR-004, FR-006, FR-009), MVU/effect applicability (US2 I/O-bearing), and required evidence obligations
- [X] T004 [P] [skillist: []] Run `./fake.sh build -t Route` on the current spec-only diff and record the baseline tier (expect `focused-authority` pre-edit; re-checked post-edit in T033)

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-ui-widgets, fs-skia-scene] Draft the `Control.renderTree : Theme -> Size -> Control<'msg> -> ControlRenderResult<'msg>` addition in `src/Controls/Control.fsi` (additive; `render`/`Widget.render` untouched per FR-003)
- [X] T006 [P] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Draft the `InteractiveAppHost<'model,'msg>` record (`Init`/`Update`/`View: Size -> 'model -> SceneNode`/`MapKey`/`MapPointer`/`Tick`/`Diagnostics`) and `Viewer.runInteractiveApp` in `src/SkiaViewer/SkiaViewer.fsi`, leaving `GeneratedAppHost` + `Viewer.runApp` literal intact (FR-006)
- [X] T007 [skillist: fs-skia-skiaviewer, fs-skia-elmish] Exercise the draft `.fsi` from FSI (renderTree distinctness, host record construction + `runInteractiveApp` bounded run, representative `Init`/`Update`/`MapPointer`); capture the transcript to `readiness/fsi-session.txt`
- [X] T008 [skillist: []] Record surface-area baselines for the new/changed public modules (`FS.Skia.UI.Controls`, `FS.Skia.UI.SkiaViewer`) so post-implementation drift is reviewable
- [X] T009 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope handling and failure diagnostics in `readiness/runtime-limitations.md` and `readiness/aggregate-hang-diagnostics.md` (no live key/pointer injection ⇒ synthetic-event-through-real-adapter is the honest bar, not `[S]`)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 — Faithful nested-tree rendering (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fs-skia-scene, fs-skia-ui-widgets] Add a failing `renderTree` distinctness golden: two structurally different nested trees produce different `Scene`s and nested children (not just the outer container) are laid out and painted (SC-001)
- [X] T011 [P] [US1] [skillist: fs-skia-ui-widgets] Add a preservation guard asserting `Control.render`/`Widget.render` behavior + the Feature-080 `ControlFidelityCheck` goldens stay green (FR-003)

### Implementation

- [X] T012 [US1] [skillist: fs-skia-ui-widgets, fs-skia-scene] Implement `Control.renderTree` in `src/Controls/Control.fs`: recursive Yoga layout at the output `Size` plus paint of nested containers and their children (FR-001, FR-002)
- [X] T013 [US1] [skillist: fs-skia-skiaviewer, fs-skia-evidence-mode] Capture per-page render-distinctness screenshots + diff to `evidence/render-distinctness/*.png`; confirm the diff between two distinct pages is non-empty and record `readiness/real-image-evidence.md` (SC-001)
- [X] T014 [US1] [skillist: []] Document the US1 independent validation path (FSI distinctness check + screenshot diff) in `readiness/visual-evidence-honesty.md`

**Checkpoint**: User Story 1 is independently functional and testable.

---

## Phase 4: User Story 2 — Pointer-driven interaction in the durable host (US2, P1)

### Tests First (Principle I, Principle IV, Principle VI)

- [X] T015 [P] [US2] [skillist: fs-skia-elmish, fs-skia-skiaviewer] Add pure pointer-routing transition tests: hit-test (`Layout` × `EventBindings` by `ControlId`) → `PointerInteraction` with the 4px click/drag fold → `MapPointer` → `msg`; assert pure `Update` transitions and emitted `ViewerEffect`s (FR-004)
- [X] T016 [P] [US2] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Add a failing headless host-dispatch test: deliver a synthetic `PointerPressed`/`PointerReleased` at a control's bounds through `runInteractiveApp` and observe the bound `msg` dispatched and the model changed (FR-004, FR-005; SC-002)

### Implementation

- [X] T017 [US2] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Implement `InteractiveAppHost` + `Viewer.runInteractiveApp` in `src/SkiaViewer/SkiaViewer.fs`, routing `ViewerEvent.Pointer*` via `ControlsElmish.interpretPointerOutcome`; keep the `Viewer.runApp viewerOptions generatedHost` GovernanceTests literal reachable (FR-004, FR-006)
- [X] T018 [US2] [skillist: fs-skia-skiaviewer] Persistent graphical launch: launch the interactive host as a durable visible window from the default executable path (not bounded smoke/metadata) and capture `readiness/interactive-visible-window.md`, `readiness/window-state-diagnostics.md`, `readiness/close-reason-separation.md`, `readiness/window-options.md` as `key=value` blocks
- [X] T019 [US2] [skillist: fs-skia-evidence-mode, fs-skia-skiaviewer] Capture live/synthetic-through-adapter pointer-dispatch evidence to `evidence/pointer-dispatch.md` (`key=value`, msg + model change, FR-005; SC-002)

**Checkpoint**: User Story 2 is independently functional and testable.

---

## Phase 5: User Story 3 — Toolkit key-name normalization (US3, P2)

### Tests First (Principle I, Principle VI)

- [X] T020 [P] [US3] [skillist: fs-skia-keyboard-input] Add a failing `normalize` mapping test: `Number5`/`Digit5`/`Keypad5`/`Key5` → `Digit 5`, `KeyL` → `Letter 'L'` (case-insensitive), and an unrecognized name still → `Unknown raw` (totality, no regression) (SC-003)

### Implementation

- [X] T021 [US3] [skillist: fs-skia-keyboard-input] Implement the `Number*`/`Digit*`/`Keypad*`/`Key{n}` digit families and `Key{X}` letter family in `src/KeyboardInput/KeyboardInput.fs` `normalize`; preserve the terminal `Unknown raw` arm and the unchanged `.fsi`/`ViewerKey` union (FR-007, FR-008)
- [X] T022 [US3] [skillist: fs-skia-keyboard-input] Capture the `normalize` mapping evidence + test log to `evidence/normalize-mapping.md` (SC-003)

**Checkpoint**: User Story 3 is independently functional and testable.

---

## Phase 6: User Story 4 — Resolution-independent rendering without blur (US4, P2)

### Tests First (Principle I, Principle VI)

- [X] T023 [P] [US4] [skillist: fs-skia-skiaviewer, fs-skia-scene] Add a failing size-aware `View` test: render at two different surface extents and assert content is laid out to the actual extent (no fixed-size upscaling) (SC-004)

### Implementation

- [X] T024 [US4] [skillist: fs-skia-skiaviewer] Wire the size-aware `View: Size -> 'model -> SceneNode` into the `runInteractiveApp` render loop, sourcing the current extent from the real swapchain/window size (FR-009)
- [X] T025 [US4] [skillist: fs-skia-skiaviewer, fs-skia-evidence-mode] Capture size-aware render evidence to `evidence/size-aware-render/*.png` and record the windowed-fullscreen blur workaround (exactly one flag/setting, e.g. `--window-startup normal`) in `readiness/runtime-limitations.md` (SC-004; doc home lands in T028/T030)

**Checkpoint**: User Story 4 is independently functional and testable.

---

## Phase 7: User Story 5 — Accurate authoring guidance (US5, P3)

- [X] T026 [P] [US5] [skillist: fs-skia-skiaviewer] Author the new skill `.agents/skills/fs-skia-viewer-host/SKILL.md` (distinct-named to avoid the existing package `fs-skia-skiaviewer` collision): host input surface (keyboard `MapKey`; pointer `MapPointer` seam), preview-vs-tree distinction (`Control.render` preview vs `renderTree`), and the windowed-fullscreen blur caveat + workaround (FR-011)
- [X] T027 [P] [US5] [skillist: fs-skia-typed-controls] Add the consumer-side note + typed-surface probe recipe to `.agents/skills/fs-skia-typed-controls/SKILL.md`: author via `FS.Skia.UI.Controls.Typed.*`; verify availability from package / `catalog.yml` `module:` fields, **not** `docs/api-surface/` (FR-012)
- [X] T028 [P] [US5] [skillist: fs-skia-typed-controls] Update `template/base/docs/scaffold-map.md`: the typed front door is absent from `docs/api-surface/` (legacy `X.create` only) + how to enumerate the typed surface; include the windowed-fullscreen blur workaround (FR-013/FR-010)
- [X] T029 [P] [US5] [skillist: speckit-specify] Update `.specify/templates/spec-template.md`: the Framework Governance Prompts section is exempt from the "no implementation details" rule (FR-014)
- [X] T030 [P] [US5] [skillist: fs-skia-evidence-mode] Update `template/base/docs/evidence-formats.md` (and/or the `fs-skia-evidence-mode` skill): evidence token parsing reads `key=value` lines; a markdown table with the same tokens does **not** satisfy the validators (FR-015); record `readiness/evidence-vocabulary.md`
- [X] T031 [P] [US5] [skillist: speckit-specify] Update `.agents/skills/speckit-specify/SKILL.md`: add the multi-file external-URL snapshot recipe (enumerate a GitHub tree, fetch per file, assemble `source-spec.md` with per-file headers) (FR-016)
- [X] T032 [US5] [skillist: []] Document the US5 independent validation path in `readiness/framework-guidance.md`: each artifact states its fact; the `.claude` mirror is generated from `.agents` (cite `.agents`, regenerated in T034) and passes `SkillSyncCheck`/`SkillQualityCheck`

**Checkpoint**: User Story 5 is independently functional and testable.

---

## Phase 8: Integration & Polish

- [X] T033 [skillist: []] Re-run `./fake.sh build -t Route` after the contract-bearing edits and confirm escalation to `maintainer-verify` (FR-018; SC-006)
- [X] T034 [skillist: []] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate surface-area baselines, per-package `.fsi.txt` snapshots, the `.claude` skill mirror, and `skillist-reference.md` for the new `fs-skia-viewer-host` skill (FR-017)
- [X] T035 [skillist: fs-skia-template-update] Run the escalated FAKE order sequentially through `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`, recording `GeneratedProductCheck` as non-authoritative if it fails for the known environment reason
- [X] T036 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; refresh `readiness/evidence-graph.md`
- [X] T037 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm PASS or document every `--accept-synthetic` override; refresh `readiness/evidence-audit.md`
- [X] T038 [skillist: fs-skia-evidence-mode] Finalize the feature-local `readiness/evidence-audit.md`, the window-visibility evidence class, `generated-validation.md`/`generated-guidance-validation.md`, and `readiness/governance-risk-levels.md` with the non-authoritative aggregate result recording

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. No `[S]`/`[SEH]`
tasks are anticipated: where the headless environment lacks live key/pointer
injection, synthetic-event-through-the-real-host/adapter is the honest `[X]` bar
(it exercises the real path, not a literal fixture). Any wholly-literal fixture
discovered during implementation MUST be added here and carry `[S]`.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
