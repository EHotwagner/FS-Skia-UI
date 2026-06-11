# Tasks: Runtime Visual-State Bridge (R1)

**Feature branch**: `096-runtime-visual-state-bridge`
**Spec**: `specs/096-runtime-visual-state-bridge/spec.md`
**Plan**: `specs/096-runtime-visual-state-bridge/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` is an annotation for design-approved synthetic error-handling work; it
remains `[S]` when completed. **None planned for this feature** — `deriveVisualState`
is a pure, total projection (an id named by no interaction state resolves to
`Normal`, never an exception) and `applyRuntimeVisualState` is a pure, total tree
walk (a `Normal`-and-unset node is returned unchanged, never a throw); there is no
runtime error path to fixture. All precedence/byte-identity/focus-survival/responds
evidence is real (FsCheck real inputs, structural `Scene` / resolved-style equality,
the **live** retained render-step path, the reused responds-proof). Any `[S]` that
appears triggers the full Principle V disclosure regime.

## Tier & MVU posture

This is a **Tier 1** change — it **moves public surface**: a single new public
projection `val deriveVisualState : model:ControlRuntimeModel -> controlId:ControlId
-> VisualState` on `src/Controls/ControlRuntime.fsi`. Every phase is Tier 1, so
per-task `[T1]` marks are omitted. The `applyRuntimeVisualState` host bridge is
**internal** (omitted from the `.fsi`, reached by `Controls.Tests` / `Elmish.Tests`
via the existing `InternalsVisibleTo`); the widened-kind geometry reuses the existing
`VisualState`-threaded private render path (no new control public type).

**MVU/Elmish is read-only here.** `Model` = the existing `ControlRuntimeModel`
(owned and mutated by the pointer/focus reducers); R1 adds **no** `Msg`/`Effect`/
`Cmd`/`init`/`update` and mutates no runtime state. The new code is **pure** —
`deriveVisualState` (model → state) and `applyRuntimeVisualState` (model + tree →
tree) — and the host applies the bridge at the **interpreter edge** (`renderRetained`
assembles a read-only `ControlRuntimeModel` from `pointerState`+`focused` and stamps
the tree before `RetainedRender.step`, in the `ControlId` domain / pre-reconcile).

This is **not** a persistent graphical viewer feature. Parity is structural `Scene`
/ resolved-style equality (the `SceneEvidence` render functions are deterministic
capability-hash functions, not pixel encoders) and the responds-proof is the **live
retained render-step path** (input → `Update` patch → restyle) that an inert/
un-bridged build fails — not a render-only screenshot and not a persistent window.
Recorded as a visible decision in T003: the viewer-launch task-generation rule does
not apply (no persistent-launch obligation).

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the user-reachable surface — the public
`ControlRuntime.deriveVisualState` through the packed library, or the **live**
retained path through `renderRetained` / the real `applyRuntimeVisualState` bridge —
was actually exercised. Passing unit tests on the pure helpers alone do **not**
satisfy `[X]`. Because the runtime model is read-only, MVU evidence for these stories
is the read of the existing `ControlRuntimeModel` driving the resolved style / the
reconciler `Update` patch on the live path; no new transition is introduced to assert.

## Success-criterion → assertion mapping

- **SC-001** (hover/press/selected restyle with zero consumer code) → T011 bridged-
  restyle test + T016 host wiring (`live-restyle.md`).
- **SC-002** (focus indicator automatic; survives a sibling-shifting re-render via E2
  identity) → T017 focus-indicator test + T018 live-retained focus-stability test +
  T019 identity wiring (`focus-survives-reshuffle.md`), not a hand-seeded map.
- **SC-003** (`Normal`-and-unset emits no attribute; `Scene`-byte-identical;
  `RecomputedNodeCount` unchanged at rest) → T012 byte-identity test + T016 evidence.
- **SC-004** (precedence pure/total/deterministic over ≥1000 combos; fixed order
  holds; consumer non-`Normal` preserved 100%) → T021 arbitration test + T022 FsCheck
  property (`derive-precedence.md`).
- **SC-005** (localized interaction → single `Update` patch; O(hovered-subtree) via
  `WorkReduction`) → T024 partial-repaint evidence.
- **SC-006** (migrated set widened to `Button`/`CheckBox`/`Slider`/`TextBox`/
  `RadioGroup`/`Switch`; unmigrated kinds no render delta) → T015 geometry widening +
  T023 widened-kinds evidence.
- **SC-007** (contrast gate stays the single contrast authority; no second policy /
  new token literal) → T025 `ContrastCheck` evidence.
- **SC-008** (`view : 'model -> Control<'msg>` contract unchanged; additive; no
  binding/observable/dependency-property/selector/template surface) → T009 non-goal
  record + T012 byte-identity at rest.

## Non-SC requirement traceability

- **FR-003** (consumer-set semantic state preserved and out-ranks derived; single
  carrier channel) → T021 arbitration test + T014 bridge preservation branch.
- **FR-004** (bridge stamps pre-reconcile in the `ControlId` domain → scoped `Update`
  patch) → T014 bridge + T016 host call site + T024 partial-repaint.
- **FR-007** (resolved style attaches to E2 stable retained identity; R1 consumes,
  never re-derives, the 067/091/092 scheme) → T018 + T019.
- **FR-008** (no new token literal / second contrast policy) → T025.
- **FR-009** (additive; permanent non-goals introduced: none) → T009 + T012.

## Governance risk levels

- **Small** — the pure `deriveVisualState` precedence + the `applyRuntimeVisualState`
  tree walk + the widened geometry: focused validation is `Dev` + the targeted
  `Controls.Tests` precedence/byte-identity/widened-kind suites.
- **Medium** — the `renderRetained` host wiring + the live focus-survival / responds
  path: `Dev` + the `Elmish.Tests` live-retained suites + the responds-proof.
- **Broad** — the public `ControlRuntime.fsi` surface move escalates to
  controls-public-surface, so the serialized `Dev → GeneratedGuidanceCheck →
  TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit` path applies
  **plus `ContrastCheck`**. FAKE-backed targets run **sequentially** (shared `.fake`
  state); aggregate results are recorded as **non-authoritative** unless re-confirmed
  sequentially. Run `./fake.sh build -t Route` first and run exactly the gates it
  prints.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Every task has a matching `tasks.deps.yml` entry; every line mirrors the
  structured `skillist` via `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory artifacts are present and linked (spec, plan, research, data-model, quickstart, `contracts/control-runtime-bridge.md`, `checklists/requirements.md`) and that `.specify/feature.json` resolves `specs/096-runtime-visual-state-bridge`
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold audit-discoverable readiness placeholders under `readiness/`: `derive-precedence.md`, `live-restyle.md`, `focus-survives-reshuffle.md`, `byte-identity-at-rest.md`, `partial-repaint.md`, `widened-kinds.md`, `responds-proof.md`, `contrast.md`, `fsi-transcript.md`, `surface-baselines.md`, plus `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `real-image-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier 1 (public surface moves: `ControlRuntime.fsi` gains `deriveVisualState`), affected layers (`FS.Skia.UI.Controls` projection + internal bridge + widened geometry; `FS.Skia.UI.Controls.Elmish` host call site), public-API impact (single additive `val`; `applyRuntimeVisualState` internal), MVU applicability (reads the existing `ControlRuntimeModel`; no new `Msg`/`Effect`/`update`; pure bridge at the interpreter edge), and the evidence obligations from the plan; record as a **visible decision** that this is **not** a persistent graphical viewer feature (deterministic render-step + structural `Scene`/resolved-style equality; the responds-proof is the live retained render-step path), so no persistent-launch obligation applies
- [X] T004 [P] [skillist: []] Run `./fake.sh build -t Route`; confirm the controls-public-surface escalation (the serialized six-target path **plus `ContrastCheck`**) and record the authoritative gate list plus the small/medium/broad governance risk levels for this Tier-1 surface move into `readiness/governance-risk-levels.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-ui-widgets] Draft the public surface — `val deriveVisualState : model:ControlRuntimeModel -> controlId:ControlId -> VisualState` on `src/Controls/ControlRuntime.fsi` with the doc comment per `contracts/control-runtime-bridge.md` (purely additive; no existing signature changes; `RetainedId` stays out of the surface — the bridge binds in the `ControlId` domain)
- [X] T006 [P] [skillist: fs-skia-reconciliation] Add the internal bridge seam in `src/Controls/ControlRuntime.fs` (NOT declared in the `.fsi` → automatically internal): the `applyRuntimeVisualState` signature + a `setVisualState` helper (replace-or-append the last-writer `visualState` attribute that `ControlInternals.visualStateOf` reads), reachable from `Controls.Tests` / `Elmish.Tests` via the existing `InternalsVisibleTo` (`Controls.fsproj`); reuse `ControlInternals.visualStateOf` directly (`ControlRuntime.fs` compiles after `Control.fs`)
- [X] T007 [skillist: fs-skia-ui-widgets] Exercise the draft `deriveVisualState` from FSI against the packed library — `Hover` for a hovered id, `Pressed` out-ranking `Hover`, `Normal` for an unknown id — per the contract FSI block; capture the session transcript to `readiness/fsi-transcript.md`
- [X] T008 [P] [skillist: []] Record the initial surface-area baseline expectations for the changed public module (`ControlRuntime.fsi`: controls-public-surface / per-package / cross-package) as the pre-change reference for the Phase 6 recapture
- [X] T009 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope handling, the permanent non-goals, and failure diagnostics into `readiness/runtime-limitations.md`: a single carrier channel (the pre-existing `Attr.visualState` — no second/parallel consumer-state channel, FR-003), no new `VisualState` case, no new token literal or second contrast policy (FR-008), the bridge is total and silent (a `Normal`-and-unset node is a no-op, FR-005), it operates only in the `ControlId` domain (never `RetainedId`), a non-migrated kind derives state but produces no visible change, and no data-binding/observable/dependency-property/selector/template surface is introduced (FR-009)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — a running control restyles on interaction with zero consumer code

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fs-skia-ui-widgets, fs-skia-testing] Add a failing-first `deriveVisualState` runtime-precedence test: the runtime-derivable order `Pressed > Selected > Focused > Hover > Normal` holds (the runtime tail of FR-002), an id named by no interaction state resolves to `Normal`, and identical `(model, id)` inputs always yield an identical result (totality + determinism underpinning SC-001/SC-004)
- [X] T011 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-testing] Add a failing-first bridged-restyle test: a migrated control whose id is the `HoveredControl` / in `PressedControls` / the `Selection.ControlId` of a `ControlRuntimeModel` resolves to the matching `Hover`/`Pressed`/`Selected` style with a **no-attribute** consumer `view` (via `applyRuntimeVisualState` + `Style.resolve`, never a hand-authored attribute), and a non-interacted sibling resolves `Normal` (SC-001)
- [X] T012 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-testing, fs-skia-evidence-mode] Add a failing-first byte-identity-at-rest test: a `Normal`-and-unset control is returned from `applyRuntimeVisualState` **unchanged** (no attribute added) and is structurally-`Scene`-equal to the un-bridged build, with `RecomputedNodeCount` unchanged at rest (FR-005, SC-003, SC-008)

### Implementation

- [X] T013 [US1] [skillist: fs-skia-ui-widgets] Implement `deriveVisualState` in `src/Controls/ControlRuntime.fs` — the closed runtime-derivable precedence (`PressedControls.Contains id` → `Pressed`; `Selection` is `Some s` and `s.ControlId = id` → `Selected`; `FocusedControl = Some id` → `Focused`; `HoveredControl = Some id` → `Hover`; else `Normal`); pure, total, deterministic, with no per-kind branching
- [X] T014 [US1] [skillist: fs-skia-reconciliation] Implement `applyRuntimeVisualState` in `src/Controls/ControlRuntime.fs` — per node `id = Key |> Option.defaultValue Kind`; if `ControlInternals.visualStateOf` is `<> Normal` return the node unchanged (consumer wins, FR-003); else match `deriveVisualState model id`: `Normal` → node unchanged (emit nothing, FR-005), `derived` → `setVisualState derived`; recurse the structural `Children`; pure (no `model` mutation), stamping in the `ControlId` domain so a change becomes a scoped reconciler `Update` patch (FR-004)
- [X] T015 [US1] [skillist: fs-skia-ui-widgets] Widen the migrated geometry — add `(classes, state)` params to `sliderGeom` / `textFieldGeom` / `radioGeom` / `switchGeom` in `src/Controls/Control.fs` and route their paint through `Style.resolve theme baseStyle classes state` (matching `buttonGeom` / `checkboxGeom`); at `classes = []`, `state = Normal` the output is **byte-identical** to today (FR-006; the widened half of SC-006)
- [X] T016 [US1] [skillist: fs-skia-elmish] Wire the bridge into the host: in `renderRetained` (`src/Controls.Elmish/ControlsElmish.fs:555`) assemble a read-only `ControlRuntimeModel` from the live `pointerState` (`Hover`/`Presses`, already `ControlId`-keyed) + `focused` (`RetainedId` resolved back to `ControlId` via the prior retained tree) and apply `applyRuntimeVisualState` to `host.View size model` **before** `RetainedRender.init`/`step` (pre-reconcile, `ControlId` domain); capture US1 to `readiness/live-restyle.md` (SC-001) and the at-rest result to `readiness/byte-identity-at-rest.md` (FR-005, SC-003)

**Checkpoint**: User Story 1 is functional and testable independently.

---

## Phase 4: User Story 2 (US2) — a focused control shows a focus indicator that survives unrelated re-renders

### Tests First (Principle I)

- [X] T017 [P] [US2] [skillist: fs-skia-reconciliation, fs-skia-testing] Add a failing-first focus-indicator test: a `ControlRuntimeModel` whose `FocusedControl` is a migrated focusable control resolves that control with its `Focused` indicator via the bridge + E3 resolver and **no consumer focus attribute**; when focus moves to a different control, the previously-focused one returns to its non-focused resolution and the newly-focused one gains the indicator (SC-002, US2.1/US2.3)
- [X] T018 [P] [US2] [skillist: fs-skia-reconciliation, fs-skia-testing] Add a failing-first focus-survives-reshuffle test over the **live** retained path: across a sibling-shifting unrelated re-render the `Focused` indicator stays on the same control via E2 retained identity — demonstrated through the live-path identity, **not** a hand-seeded `StateByIdentity` map (SC-002, FR-007)

### Implementation / Evidence

- [X] T019 [US2] [skillist: fs-skia-elmish, fs-skia-reconciliation] Confirm the host's `focused` (`RetainedId`) → `ControlId` resolution feeds `deriveVisualState`'s `Focused` rank so the indicator attaches to the E2 stable retained identity and survives the reshuffle; R1 **consumes** — never re-derives — the 067/091/092 identity scheme (FR-007); capture to `readiness/focus-survives-reshuffle.md` (SC-002)
- [X] T020 [US2] [skillist: fs-skia-elmish, fs-skia-evidence-mode] Capture an input→visible-restyle responds-proof on the live retained path (a hover/press/focus change → a reconciler `Update` patch → a restyle) that an inert/un-bridged build fails (identical frames / `Inert`); record to `readiness/responds-proof.md`

**Checkpoint**: User Story 2 is functional and testable independently.

---

## Phase 5: User Story 3 (US3) — consumer-set semantic state composes with derived interaction state by a closed order

### Tests First (Principle I)

- [X] T021 [P] [US3] [skillist: fs-skia-reconciliation, fs-skia-testing] Add a failing-first consumer-vs-derived arbitration test: a consumer-`Disabled` control the runtime also reports hovered/pressed/focused resolves `Disabled` (consumer state out-ranks derived, FR-003); a consumer-`Selected` control the runtime reports `Pressed` resolves `Selected`; a control the consumer left at `Normal` that the runtime reports focused resolves `Focused` (derived fills the `Normal` slot) — the single-carrier rule, no second channel (US3.1/US3.2/US3.3, SC-004 preservation half)

### Implementation / Evidence

- [X] T022 [US3] [skillist: fs-skia-ui-widgets, fs-skia-testing] Add the FsCheck property over `deriveVisualState` + `applyRuntimeVisualState`: purity / totality / determinism over **≥1000** generated `(ControlRuntimeModel, ControlId, consumer-state)` combinations, the fixed order (`Disabled > Validation > Loading > Pressed > Selected > Focused > Hover > Normal`) holds for every combination, and a consumer-set non-`Normal` state is preserved over any derived interaction state in **100%** of cases; record to `readiness/derive-precedence.md` (SC-004)

**Checkpoint**: User Story 3 is functional and testable independently.

---

## Phase 6: Integration & Polish

- [X] T023 [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Write `readiness/widened-kinds.md`: each of `button` / `check-box` / `slider` / `text-box` / `radio-group` / `switch` restyles on interaction and shows a focus indicator on the live path; the unmigrated kinds (incl. `toggle-button` / `list-box` / `multi-select-list` / `combo-box`) show **no render-output delta** (SC-006)
- [X] T024 [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Write `readiness/partial-repaint.md`: a single hover entering one control surfaces a single reconciler `Update` patch and the repaint is O(hovered-subtree) measured via the existing `WorkReduction` metric — not a whole-tree repaint (SC-005, FR-004)
- [X] T025 [skillist: fs-skia-design-tokens] Run `./fake.sh build -t ContrastCheck` and write `readiness/contrast.md`: no migrated control's bridged styling regresses its contrast result, and the bridge adds no second contrast policy and no new token literal (any styling flows through E3's `Style.resolve` over DTCG-sourced tokens) (SC-007, FR-008)
- [X] T026 [skillist: fs-skia-ui-widgets, fs-skia-elmish] Recapture Tier-1 surface baselines after the `.fsi` change via `./fake.sh build -t RefreshSurfaceBaselines` (controls-public-surface + cross-package) and `PerPackageSurface.captureCurrent` (per-package snapshots are **not** covered by `RefreshSurfaceBaselines`); record the diffs to `readiness/surface-baselines.md`
- [X] T027 [skillist: fs-skia-template-update] Run the serialized escalated non-concurrent gate prefix **sequentially** (shared `.fake` state) — `./fake.sh build -t Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` — recording the aggregate results as **non-authoritative** into `readiness/generated-guidance-validation.md`
- [X] T028 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory` + `tasks=<n>` match, no cycles, no dangling refs, no `[S*]` surprises; record to `readiness/evidence-graph.md`
- [X] T029 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic-propagation + diff-scan) or document every `--accept-synthetic` override; record to `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. **None planned**
— see the Status Legend rationale (pure/total projection and tree walk with no
runtime error path; real precedence/byte-identity/live-retained/responds
evidence). For any `[SEH]` rows, include the approval label, design-phase source,
synthetic input class, expected error behavior, and reviewer-visible acceptance
status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
