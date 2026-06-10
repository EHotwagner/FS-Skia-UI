# Tasks: Declarative Visual-State & Style-Class Layer

**Feature branch**: `093-visual-state-style-layer`
**Spec**: `specs/093-visual-state-style-layer/spec.md`
**Plan**: `specs/093-visual-state-style-layer/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` is an annotation for design-approved synthetic error-handling work; it
remains `[S]` when completed. **None planned for this feature** — style
resolution is a pure, total function with no error path (Custom-unknown ⇒
identity delta, never an exception or silent drop), and all parity/determinism
evidence is real structural-`Scene` / `ResolvedStyle` equality. Any `[S]` that
appears triggers the full Principle V disclosure regime.

## Tier & MVU posture

This is a **Tier 1** change (public surface moves: new types on `Types.fsi`, a
new public `Style.fsi`, `Attributes.fsi` + typed-`Props` deltas) — every phase
is Tier 1, so per-task `[T1]` marks are omitted. **MVU/Elmish is N/A**: the
resolver is a pure, total, deterministic function of `(tokens + theme + classes
+ state)` — no `Model`/`Msg`/`Effect`/`Cmd`, no `init`/`update`, no interpreter
boundary. It *reads* the `VisualState` and retained identity that
`ControlRuntime` / `RetainedRender.StateByIdentity` already own (features
067/091/092) and re-derives none of it.

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the user-reachable surface (the typed front
door / public `Style.resolve` through the packed library, captured in an FSI
transcript or readiness artifact) was actually exercised — passing unit tests
on internal code alone do not satisfy `[X]`.

## Success-criterion → assertion mapping

- **SC-001** (variant distinctness) → T010 variant-distinctness test.
- **SC-002** (each state distinct + class/state precedence) → T014 state +
  precedence test.
- **SC-003** (byte-identical migration parity + no per-kind branch) → T021
  structural-`Scene` parity test vs captured baseline (T020) + T024 inspection.
- **SC-004** (purity/determinism + fixed precedence over ≥1000 inputs) → T015
  FsCheck property.
- **SC-005** (state look survives sibling-shifting re-render via live retained
  path) → T017 wiring + T018 live-path evidence (not a hand-seeded map).
- **SC-006** (contrast gate is sole authority) → T025 `ContrastCheck` /
  `DesignTokenDrift`.
- **SC-007** (unmigrated kinds unchanged) → T022 regression test.

## Governance risk levels

- **Small** — internal `Style.fs` fold logic / token deltas: focused validation
  is `Dev` + the targeted `Controls.Tests` suites.
- **Medium** — typed-`Props` + `ControlInternals` migration: `Dev` +
  parity/regression suites + `DesignTokenDrift` + `ContrastCheck`.
- **Broad** — the public `*.fsi` surface move escalates to controls-public-
  surface: the full serialized `Dev → GeneratedGuidanceCheck → TemplateCheck →
  GeneratedProductCheck → EvidenceGraph → EvidenceAudit` path is required.
  FAKE-backed targets run **sequentially** (shared `.fake` state); aggregate
  results are recorded as **non-authoritative** unless re-confirmed sequentially.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Every task has a matching `tasks.deps.yml` entry; every line mirrors the
  structured `skillist` via `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory artifacts are present and linked (spec, plan, research, data-model, quickstart, `contracts/style-resolver.md`, `contracts/attach-class-surface.md`, `checklists/requirements.md`)
- [X] T002 [skillist: []] Record feature Tier 1, affected layer (`FS.Skia.UI.Controls`), public-API impact (`Types.fsi`, new `Style.fsi`, `Attributes.fsi`, migrated `Widgets/*.fsi`), Elmish/MVU applicability (N/A — pure total resolver, no `Model`/`Msg`/`Effect`), and the evidence obligations from the plan
- [X] T003 [P] [skillist: []] Scaffold audit-discoverable readiness placeholders under `readiness/`: `us1-variant-resolution.md`, `us2-visualstate-and-precedence.md`, `us3-parity-baseline.md`, `sc004-determinism-property.md`, `sc005-retained-identity.md`, `sc006-contrast-authority.md`, `sc007-unmigrated-unchanged.md`, `fsi-transcript.md`, `surface-baselines.md`, plus `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `real-image-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T004 [P] [skillist: []] Run `./fake.sh build -t Route`; confirm the controls-public-surface escalation and record the authoritative gate list plus the small/medium/broad governance risk levels for this Tier-1 surface move into `readiness/governance-risk-levels.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Draft the public `.fsi` surface: `StyleVariant` (closed `Primary|Danger|Ghost|Neutral|Success|Warning`), `StyleClass = Variant | Custom`, the `StyleClassesValue` arm on `AttrValue<'msg>` (`Types.fsi`); new `Style.fsi` (`ResolvedStyle` record + `val resolve : Theme -> ResolvedStyle (* kind base *) -> StyleClass list -> VisualState -> ResolvedStyle`, the `base` supplied by the caller per migrated kind so `resolve` stays kind-agnostic); `Attributes.styleClasses` builder (`Attributes.fsi`); insert `Style.fsi`/`Style.fs` into `Controls.fsproj` after `Theme.fs` and before `Attributes`, with a base-only `resolve` stub so Foundation compiles
- [X] T006 [P] [skillist: fs-skia-design-tokens] Add any variant-specific token the resolver needs to the DTCG single source (`design-tokens.tokens.json`) and regenerate `DesignTokens`; confirm no inline color/size literals bypass `DesignTokenDrift` (FR-008)
- [X] T007 [skillist: fs-skia-evidence-mode] Exercise the draft `.fsi` from FSI through the loaded/packed library (`Style.resolve`, `Attributes.styleClasses`, typed `Classes`) and capture the session to `readiness/fsi-transcript.md`
- [X] T008 [skillist: fs-skia-typed-controls] Capture initial surface-area baselines (controls-public-surface, per-package, cross-package) for the new/changed public modules so the Tier-1 deltas are reviewable
- [X] T009 [skillist: []] Record unsupported-scope handling and failure diagnostics — resolver totality (no exception path), `Custom`-unknown ⇒ identity delta (not error/silent-drop), contrast deferred to `ContrastCheck`, and the permanent non-goals (selectors/specificity/cascade/dependency-props/data-binding) — into `readiness/runtime-limitations.md`

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — style a control by intent

### Tests First

- [X] T010 [P] [US1] [skillist: fs-skia-testing] Add a failing-first variant-distinctness test: each built-in `StyleVariant` resolves to its token-derived `ResolvedStyle`, two variants on one kind under one theme differ in the variant-appropriate way, and a free-form `Custom` class resolves through the same fold (SC-001)

### Implementation

- [X] T011 [US1] [skillist: fs-skia-design-tokens] Implement the **class layer** of `Style.resolve`: an exhaustive `StyleVariant` → `ResolvedStyle` token-derived delta for every arm, and `Custom name` → known-name delta / unknown ⇒ identity, folded left-to-right in attach order (FR-001, last-writer-wins setup for FR-003)
- [X] T012 [US1] [skillist: fs-skia-typed-controls] Add the typed front-door attach-class affordance: `Classes: StyleClass list` on the migrated controls' `Props` (`Widgets/Buttons` box+label migrant + `Widgets/Primitives` `CheckBox`/`CheckBoxProps` rich-geometry migrant), `defaults` `Classes = []`, `view` lowering to `Attributes.styleClasses Classes`, and `Classes = []` lowering to **no** style attribute (A1 additive — byte-identical to today)
- [X] T013 [US1] [skillist: fs-skia-evidence-mode] Capture `readiness/us1-variant-resolution.md` — a semantic variant resolves to its token-derived style and two variants differ token-appropriately, exercised through the packed typed front door (vertical slice)

**Checkpoint**: User Story 1 is independently functional and testable.

---

## Phase 4: User Story 2 (US2) — interactive states render distinctly and survive re-renders

### Tests First

- [X] T014 [P] [US2] [skillist: fs-skia-testing] Add a failing-first state + precedence test: each `VisualState` the procedural baseline differentiates resolves to a distinct token-derived style (states the baseline paints identically stay identical, preserving parity), the visual state wins over a class for an overlapping field, the class's non-overlapping fields are retained, and a later class wins over an earlier one (SC-002, FR-003/FR-004)
- [X] T015 [P] [US2] [skillist: fs-skia-testing] Add the FsCheck purity/determinism + fixed-precedence property over ≥1000 generated `(theme, classes, state)` combinations — identical inputs ⇒ identical `ResolvedStyle`, and `base < classes-in-order < state` holds for every generated case (SC-004)

### Implementation

- [X] T016 [US2] [skillist: fs-skia-design-tokens] Implement the **state layer** of `Style.resolve`: an exhaustive `VisualState` → token-derived delta (incl. `Validation` mapping its `ValidationState` severity deterministically), applied **after** the class fold so a state's owned field overrides any class value (FR-003, FR-004)
- [X] T017 [US2] [skillist: fs-skia-reconciliation] Attach the state-driven resolved style to E2's stable retained identity — the resolver is re-invoked per frame through the existing `RetainedRender.StateByIdentity` / `ControlInternals` path (067/091/092), reading the live `VisualState`/animation clock and altering none of the identity scheme (FR-006)
- [X] T018 [US2] [skillist: fs-skia-reconciliation] Capture `readiness/sc005-retained-identity.md` — a hover/focus/selected look survives a sibling-shifting model update through the **live** retained path, not a hand-seeded `StateByIdentity` map (SC-005, the 092 gap this avoids repeating)
- [X] T019 [US2] [skillist: fs-skia-evidence-mode] Capture `readiness/us2-visualstate-and-precedence.md` — each `VisualState` resolves distinctly and the fixed class-vs-state precedence holds, exercised through the packed surface (vertical slice)

**Checkpoint**: User Story 2 is independently functional and testable.

---

## Phase 5: User Story 3 (US3) — one declarative resolver replaces procedural per-kind styling

### Fixture & Tests First

- [X] T020 [US3] [skillist: fs-skia-scene, fs-skia-evidence-mode] Capture the **pre-refactor** procedural styling as structural-`Scene` baselines `readiness/parity/<kind>.<theme>.<state>.scene.txt` for every migrated `(kind, theme, state)` no-class case — this must precede the refactor so it pins the behavior-preserving target (FR-005, SC-003)
- [X] T021 [P] [US3] [skillist: fs-skia-scene, fs-skia-testing] Add a failing-first parity test asserting the resolver-driven render is structurally-`Scene`-equal to the captured procedural baseline for each migrated `(kind, theme, state)` no-class case (SC-003)
- [X] T022 [P] [US3] [skillist: fs-skia-testing] Add an unmigrated-unchanged regression test asserting kinds left on the procedural path show no render-output delta (SC-007)

### Implementation

- [X] T023 [US3] [skillist: fs-skia-ui-widgets, fs-skia-scene] Migrate the representative controls' paint in `ControlInternals` (`Control.fs`) to compute each migrated kind's default `ResolvedStyle` base and call `Style.resolve theme base classes state`, reading back `ResolvedStyle` fields; remove the per-kind inline visual-state color branch for them; ensure base fidelity (`resolve theme base [] state` reproduces the procedural output exactly so parity holds byte-identically)
- [X] T024 [US3] [skillist: fs-skia-evidence-mode] Capture `readiness/us3-parity-baseline.md` — the migrated kinds' resolver output is structurally-`Scene`-equal to the procedural baseline and inspection confirms no per-kind color branch remains for them (SC-003 inspection clause)

**Checkpoint**: User Story 3 is independently functional and testable.

---

## Phase 6: Integration & Polish

- [X] T025 [skillist: fs-skia-design-tokens] Run `DesignTokenDrift` + `ContrastCheck`; confirm the contrast gate is the sole authority — a deliberately contrast-insufficient `Custom` class is flagged (not silently dropped) and no migrated default styling regresses its contrast result — and capture `readiness/sc006-contrast-authority.md` (SC-006, FR-007)
- [X] T026 [skillist: fs-skia-typed-controls] Surface-area baseline refresh (Tier 1): `./fake.sh build -t RefreshSurfaceBaselines` + `PerPackageSurface.captureCurrent`; capture the recaptured controls-public-surface / per-package / cross-package diffs to `readiness/surface-baselines.md`
- [X] T027 [skillist: fs-skia-evidence-mode] Capture `readiness/sc004-determinism-property.md` (≥1000-input property results) and `readiness/sc007-unmigrated-unchanged.md` (regression result), recording aggregate results as non-authoritative until re-confirmed sequentially
- [X] T028 [skillist: fs-skia-template-update] Run the serialized escalated FAKE-backed gates **sequentially** — `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck` — and record the non-authoritative aggregate verdict; rerun sequentially on any race-like failure before any product-regression claim
- [X] T029 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed feature directory + task count match, no cycles, no dangling refs, no `[S*]` surprises
- [X] T030 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm the merge-gate verdict PASS with no synthetic-propagation or diff-scan hits (no `--accept-synthetic` expected; document any override)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
