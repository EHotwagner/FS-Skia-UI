# Tasks: Lookless Slot Composition

**Feature branch**: `095-lookless-slot-composition`
**Spec**: `specs/095-lookless-slot-composition/spec.md`
**Plan**: `specs/095-lookless-slot-composition/plan.md`

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
remains `[S]` when completed. **None planned for this feature** — slot lowering
is a **pure, total** function (every declared region has a default, an unfilled
slot falls back to its default chrome, lowering never throws), filling an
undeclared slot is a **compile-time error** (no runtime error path to fixture),
and all parity / composition / retained-identity evidence is real (structural
`Scene` / lowered-`Control<'msg>` equality from the actual lowering, a real
captured pre-slot baseline, FsCheck real inputs, and the **live** retained path
for SC-004 — never a hand-seeded `StateByIdentity` map). Any `[S]` that appears
triggers the full Principle V disclosure regime.

## Tier & MVU posture

This is a **Tier 1** change — it **moves public surface**: a new
`AttrCategory.Slot` case and `AttrValue.SlotFillsValue of (string *
Control<'msg>) list` case on `src/Controls/Types.fsi`, plus new typed `Props`
fields on `src/Controls/Widgets/Primitives.fsi` (`ButtonProps.Leading` /
`.Trailing`) and `src/Controls/Widgets/Containers.fsi` (`PanelProps.Header` /
`.Footer`), each `Widget<'msg> option`. Every phase is Tier 1, so per-task
`[T1]` marks are omitted. **MVU/Elmish is N/A**: slot lowering is a pure,
total structural function `(kind + slot fills) → Control<'msg>` — it introduces
no `Model`/`Msg`/`Effect`/`Cmd`, no interpreter, and no I/O (FR-006). Slotted
content's events/focus/text/animation use the **existing** E1 (binding
dispatch), E2 (retained identity), E3 (style resolve), and E4 (focus/key
routing) mechanisms unchanged (FR-005) — E5 owns, mutates, or re-derives none of
that state.

This is **not** a graphical viewer feature: parity is structural `Scene` /
lowered-`Control<'msg>` equality (`SceneEvidence` render functions are
deterministic capability-hash functions, not pixel encoders), deterministic
render-only, no live window required. There is therefore **no** persistent
graphical launch obligation for this feature (the viewer-launch task-generation
rule does not apply — recorded as a visible decision in T003).

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the user-reachable surface — the typed
slot-fill front door (`ButtonProps.Leading`/`.Trailing`,
`PanelProps.Header`/`.Footer`) through the packed `FS.Skia.UI.Controls.Typed`
library, exercised from FSI or a semantic test that loads the packed surface —
was actually exercised. Passing unit tests on internal `ControlInternals`
helpers alone do **not** satisfy `[X]`. For the US4 skill story, `[X]` requires
the edited skills to pass `SkillSyncCheck` / `SkillQualityCheck` /
`GeneratedGuidanceCheck` and a generated project to receive the guidance.

## Success-criterion → assertion mapping

- **SC-001** (fill a declared slot → content in that region; two slots → two
  distinct regions) → T009 slot-placement test + T010 lowering + T011 typed Props.
- **SC-002 / SC-007** (unfilled byte-identical; non-slotted kind unchanged) →
  T015 parity test vs captured pre-slot baseline + T016 baseline capture.
- **SC-003** (slotted content composes E1/E3/E4) → T018 compose test.
- **SC-004** (slotted content keeps E2 retained identity across a
  sibling-shifting re-render, live path) → T019 retained-identity test.
- **SC-005** (lowering pure / deterministic / total over ≥1000 inputs, never
  throws) → T012 FsCheck property.
- **SC-006** (typed-closed per kind: undeclared slot is a compile error; no
  `DataContext`/binding/template-instantiation surface) → T013 closure proof +
  non-goal inspection.
- **SC-008** (both consumer skills name + show a runnable example for all five
  rungs E1–E5; honest) → T021 + T022 + T023.
- **SC-009** (a generated project that selects Controls receives the guidance) →
  T024.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- FAKE-backed targets (`./fake.sh`) share `.fake` state and MUST run
  **sequentially** in the deterministic escalated order; non-FAKE reads/checks
  may be parallel.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory and link spec + plan; verify `.specify/feature.json` resolves `specs/095-lookless-slot-composition`
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold `specs/095-lookless-slot-composition/readiness/` placeholders discoverable before implementation — the US/SC evidence files (`us1-slot-fill-regions.md`, `us2-unfilled-byte-identical.md`, `us3-compose-e1-e4.md`, `sc004-retained-identity.md`, `sc005-lowering-property.md`, `sc006-typed-closed-and-nongoals.md`, `us4-skill-e1-e5.md`, `fsi-transcript.md`, `surface-baselines.md`, `parity/`) and the audit-enforced contract files (`governance-risk-levels.md`, `runtime-limitations.md`, `aggregate-hang-diagnostics.md`, `generated-guidance-validation.md`, `real-image-evidence.md`), each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier (Tier 1, public surface moves), affected layer (`FS.Skia.UI.Controls`), public-API impact (`Types.fsi` + `Widgets/Primitives.fsi` + `Widgets/Containers.fsi`), Elmish/MVU applicability (N/A — pure structural lowering), and evidence obligations; record as a **visible decision** that this is not a graphical viewer feature (deterministic render-only evidence, no live window), so no persistent-launch obligation applies

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-typed-controls] Draft the public `.fsi` surface: `AttrCategory.Slot` case + `AttrValue.SlotFillsValue of (string * Control<'msg>) list` case on `src/Controls/Types.fsi` (mirroring E3's `Style` / `StyleClassesValue`); additive `Widget<'msg> option` slot fields on `src/Controls/Widgets/Primitives.fsi` (`ButtonProps.Leading` / `.Trailing`) and `src/Controls/Widgets/Containers.fsi` (`PanelProps.Header` / `.Footer`), each defaulting `None`; document honestly (a slot lowers to `Control<'msg>`; it is not a data-bound template)
- [X] T005 [P] [skillist: fs-skia-reconciliation] Add the internal carrier seam behind `module internal ControlInternals` (`src/Controls/Control.fs[i]`): `slotFill : (string * Control<'msg>) list -> Attr<'msg>` (builds `Slot`/`SlotFillsValue`), `slotFillsOf` (`tryLast "slot"` extractor, default `[]`), and `slotFor : string -> Attr<'msg> list -> Control<'msg> option` — **no** public free-form `Attr.slot` builder and **no** public `SlotName` type (FR-001)
- [X] T006 [skillist: fs-skia-ui-widgets] Exercise the draft typed slot-fill front door from FSI through the packed library surface (representative `Button.Leading` and `Panel.Header` fills), capturing the session transcript to `readiness/fsi-transcript.md`
- [X] T007 [skillist: []] Record surface-area baselines for the new / changed public modules (controls-public-surface / per-package / cross-package) as the pre-change reference for the Phase 7 recapture
- [X] T008 [skillist: fs-skia-evidence-mode] Record unsupported-scope handling and failure diagnostics: the FR-008 non-goal line (no `DataContext`, binding expression, per-item template instantiation, dependency/attached properties, CSS-selector styling) and the totality guarantee (every region has a default; lowering never throws; absent-name ⇒ default, present-name ⇒ fill even if empty), into `readiness/runtime-limitations.md`

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — fill a named slot to re-skin shape

### Tests First (Principle I)

- [X] T009 [P] [US1] [skillist: fs-skia-typed-controls, fs-skia-testing] Add a failing slot-placement test in `Controls.Tests`: filling a declared named slot lowers the supplied `Control<'msg>` sub-tree into that region of the lowered IR, and filling two distinct slots places into two distinct regions without collision/swap (SC-001) — fails before lowering places fills

### Implementation

- [X] T010 [US1] [skillist: fs-skia-reconciliation] Implement the pure, total, deterministic slot lowering in `ControlInternals`: per declared region, `slotFor name` ⇒ place the fill sub-tree, else render the region's default; inject fills into the lowered control's `Children` (ordered by region position) so they inherit E1–E4 + E2 identity by construction (FR-002, FR-004, FR-006)
- [X] T011 [US1] [skillist: fs-skia-typed-controls] Add the typed `Props` slot fields + view lowering for both representative kinds — `Button` (`Leading`/`Trailing` flanking the label, `Primitives.fs`) and `Panel` (`Header`/`Footer` around content, `Containers.fs`) — following E3's conditional-yield pattern (`None` everywhere ⇒ no slot `Attr` ⇒ byte-identical); update `defaults` with the new `None` fields (FR-001, FR-007)
- [X] T012 [US1] [skillist: fs-skia-testing] Add an FsCheck property asserting slot-lowering purity / determinism / totality over ≥1000 generated `(kind, slot fills)` combinations (both kinds, arbitrary filled subsets incl. the empty-content case): identical inputs ⇒ identical IR, lowering never throws (SC-005)
- [X] T013 [US1] [skillist: fs-skia-typed-controls] Add the typed-closure proof + non-goal inspection: a does-not-compile fixture showing that filling a slot a kind does not declare (e.g. `Button.Header`) is a **compile-time error**, and a structural inspection confirming **no** `DataContext` / binding / template-instantiation surface was introduced (SC-006, FR-008); write `readiness/sc006-typed-closed-and-nongoals.md`
- [X] T014 [US1] [skillist: fs-skia-evidence-mode] Write `readiness/us1-slot-fill-regions.md` (the named-slot fill + two-distinct-regions proof, SC-001) and `readiness/sc005-lowering-property.md` (the determinism/totality property result, SC-005)

**Checkpoint**: US1 functional — a consumer can fill a declared slot, closure is compiler-enforced.

---

## Phase 4: User Story 2 (US2) — an unfilled control is byte-identical to today

### Tests First (Principle I)

- [X] T015 [P] [US2] [skillist: fs-skia-testing, fs-skia-evidence-mode] Add a failing parity test: a slot-bearing kind with **no** slots filled is structurally-`Scene`-equal to a captured pre-slot baseline (`frozenButtonGeom`-style oracle) for each representative `(kind, theme, state)`, and a control kind not given slots is unchanged (SC-002, SC-007) — fails if exposing slots shifts the default render

### Implementation

- [X] T016 [US2] [skillist: fs-skia-evidence-mode] Capture the pre-slot parity baselines under `readiness/parity/<kind>.<theme>.<state>.scene.txt`, confirm the peripheral defaults (`Leading`/`Trailing`/`Header`/`Footer`) contribute **zero geometry** so the label/content position is invariant, and make the T015 parity test pass (FR-003)
- [X] T017 [US2] [skillist: fs-skia-evidence-mode] Write `readiness/us2-unfilled-byte-identical.md` recording the unfilled byte-identity result and the unmigrated-kind-unchanged regression (SC-002 / SC-007)

**Checkpoint**: US2 functional — exposing slots is additive and behavior-preserving.

---

## Phase 5: User Story 3 (US3) — slotted content composes with E1–E4 and survives re-renders

### Tests First (Principle I)

- [X] T018 [P] [US3] [skillist: fs-skia-elmish, fs-skia-keyboard-input, fs-skia-testing] Add a compose test filling a slot with an interactive, style-classed, focusable control: its authored binding dispatches the expected message through the existing flat per-`ControlId` mechanism (E1), its style class/visual state resolves through E3's resolver, and it appears in the E4 tab order (SC-003)
- [X] T019 [P] [US3] [skillist: fs-skia-reconciliation, fs-skia-testing] Add a retained-identity test exercising the **live** retained render path: a focused/with-text slotted control keeps its focus/text across a sibling-shifting (092-case) model update, demonstrated through the live path rather than a hand-seeded `StateByIdentity` map (SC-004)

### Implementation / Evidence

- [X] T020 [US3] [skillist: fs-skia-evidence-mode] Write `readiness/us3-compose-e1-e4.md` (E1 dispatch + E3 resolve + E4 tab order, SC-003) and `readiness/sc004-retained-identity.md` (live-path retained identity across the sibling shift, SC-004)

**Checkpoint**: US3 functional — slotted content is a first-class sub-tree, not a routing/focus/style dead-zone.

---

## Phase 6: User Story 4 (US4) — consumer capability guidance for the whole E1–E5 surface

- [X] T021 [P] [US4] [skillist: fs-skia-ui-widgets] Expand the package-owned consumer skill `src/Controls/skill/SKILL.md` (`fs-skia-ui-widgets`) to name and show a **runnable consumer example** for every rung E1–E5 (live event dispatch, retained identity + how to key for it, style class/variant + visual state, focus/keyboard traversal, slot fill), honest (a slot lowers to `Control<'msg>`, not a data-bound template; retained identity is a property of the keyed tree, not a binding) (FR-010)
- [X] T022 [P] [US4] [skillist: fs-skia-generated-controls-guidance] Expand the template-fragment consumer skill `template/fragments/controls/skill/SKILL.md` (`fs-skia-generated-controls-guidance`) with the same honest, runnable E1–E5 guidance so a `dotnet new fs-skia-ui` project selecting Controls receives it (FR-011)
- [X] T023 [US4] [skillist: fs-skia-template-update] Regenerate the `.claude` skill peer from the canonical `.agents` source via `./fake.sh build -t RefreshSurfaceBaselines` (never hand-edited) and confirm `SkillSyncCheck`, `SkillQualityCheck`, and `GeneratedGuidanceCheck` are green (SC-008)
- [X] T024 [US4] [skillist: fs-skia-template-update] Generate a project selecting the Controls capability and confirm it receives the updated E1–E5 guidance (SC-009); write `readiness/us4-skill-e1-e5.md` recording the inspection + the three green governance checks + the generated-project confirmation

**Checkpoint**: US4 functional — a real `dotnet new fs-skia-ui` consumer discovers the E1–E5 capabilities.

---

## Phase 7: Integration & Polish

- [X] T025 [skillist: fs-skia-typed-controls] Recapture controls-public-surface / per-package / cross-package surface baselines after the `.fsi` change via `./fake.sh build -t RefreshSurfaceBaselines` and `PerPackageSurface.captureCurrent` (per-package snapshots are **not** covered by `RefreshSurfaceBaselines`); write `readiness/surface-baselines.md` with the baseline diffs
- [X] T026 [skillist: fs-skia-template-update] Run the escalated serialized FAKE order (sequential — `.fake` state is not concurrency-safe): `./fake.sh build -t Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`, plus `ContrastCheck` / `ControlFidelity` (apply to slotted-content rendering as to any control); record the governance risk level (small/medium/broad) and how non-authoritative aggregate results are recorded in `readiness/governance-risk-levels.md`
- [X] T027 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory` + `tasks=<n>` match, no cycles, no dangling refs, no `[S*]` surprises
- [X] T028 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic-propagation + diff-scan) or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. **None planned**
— see the Status Legend rationale (pure/total lowering, compile-time closure,
real parity/property/live-retained evidence). For any `[SEH]` rows, include the
approval label, design-phase source, synthetic input class, expected error
behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
