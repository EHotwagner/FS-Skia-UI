# Tasks: General Navigation-Key Delivery (R5)

**Feature branch**: `100-general-navigation-keys`
**Spec**: `specs/100-general-navigation-keys/spec.md`
**Plan**: `specs/100-general-navigation-keys/plan.md`

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
remains `[S]` when completed. **None planned for this feature.** R5's router
(`Focus.route`) and host resolver are **pure, total** functions of the focused
control's declared role + `NavigationKeys`/`NavRange` metadata + the live
selection/value model. Every honest failure mode is a **designed no-op with no
spurious dispatch** (no `NavigationKeys` for the key → `Fallthrough`; empty
selection group or unresolvable current index → no dispatch; boundary clamp at a
first/last item, min/max value, or grid edge → no dispatch past the bound) —
normal control flow, asserted explicitly so "nothing happened" is a verified
outcome, not a swallowed error or an error path to fixture. The headline US1
selection-move and the US2 declared-step proofs run through the **real**
`runInteractiveApp` host seam; the closed-model proof is FsCheck-driven
(`Check.One`, no `testProperty` in this repo). Any `[S]` that appears triggers
the full Principle V disclosure regime.

## Tier & MVU posture

This is a **Tier 1 (contracted) change** — uniform across all tasks, so per-task
`[T1]` marks are omitted. Public `.fsi` surface moves in three Controls modules:

- `src/Controls/Focus.fsi` — new closed `Direction` and `NavIntent`; the
  `KeyRouting.Navigate` case becomes `Navigate of NavIntent`; `route` widens to
  take the control's **role** + declared `NavRange` alongside the keyboard op.
- `src/Controls/Types.fsi` — new `NavRange { Step; Min; Max }`; new closed
  `NavPayload` (`SteppedValue` / `MovedSelection` / `MovedCell`); a new optional
  `Navigation : NavRange option` field on `AccessibilityMetadata`; a new optional
  `Nav : NavPayload option` field on `ControlEvent` (the existing
  `Payload : string option` is **retained** for backward compatibility).
- `src/Controls/Accessibility.fsi` — `metadata` accepts a `NavRange option`;
  `keyboardFor` keeps its declared per-role `NavigationKeys` (already present).

The host per-intent resolver in `src/Controls.Elmish/ControlsElmish.fs`
(`routeFocusedKey` `Navigate` arm, `:455–478`, replacing the slider-only
`steppedValue` float at `:366–381`) stays **module-internal** by default — no
`ControlsElmish.fsi` change (the public `runInteractiveApp` / `InteractiveAppHost`
surface is **unchanged**). Because `src/Controls/**/*.fsi` lines change, `Route`
**escalates** to the serialized six-target maintainer-verify path; api-surface +
per-package `.fsi.txt` baselines for `FS.Skia.UI.Controls` are **recaptured**
(`PerPackageSurface.captureCurrent` — `RefreshSurfaceBaselines` does **not** cover
the per-package snapshots).

**No new public MVU surface.** R5 adds **no** consumer `Model`/`Msg`/`Effect`/
`Cmd`/`init`/`update`; the consumer's `view : 'model -> Control<'msg>` contract and
`'model` are untouched. `Focus.route` and the resolver are **pure** (role +
metadata + key → intent → `'msg list`); navigation produces a `'msg list`, never
I/O — `Effect`/`Cmd` is unchanged. The key event is interpreted only at the host
edge (`runInteractiveApp`), reusing the **landed E4 (feature 094) `routeFocusedKey`
seam** and the E2 (091/092) `RetainedId` focused identity. The dispatched `'msg`
for a selection role now carries the moved item via the consumer's existing
`"selected"`/`"changed"` binding — no consumer API rename.

**Persistent-launch / viewer-launch rule does not newly apply.** R5 wires
navigation into the **existing** `runInteractiveApp` host loop; it adds and changes
**no** default-executable / persistent-launch entry point. The
**responds-vs-renders** proof (arrow → selection-move on a focused radio-group/tab)
is captured through the **real** `runInteractiveApp` seam via the compiled
self-closing host (`live-vulkan-window-x11-path`) — a pre-R5 build dispatches
nothing and fails it — not a new persistent-launch/screenshot obligation. At-rest
rendered output is **unchanged** (navigation produces a `'msg`, no layout/render
algorithm change). Recorded as a **visible decision** in T003: no persistent-launch
/ window-visibility obligation is introduced; `window-visibility.md` and
`real-image-evidence.md` record this as not-applicable with honest values, and
`real-image-evidence.md` cross-references the responds-vs-renders capture as the
rendered-output evidence captured through the deterministic seam.

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the user-reachable surface — a focused control
navigated through the **real** `runInteractiveApp` host seam (the same loop the
live window uses), producing the dispatched `'msg` — was actually exercised and
produced the observable behavior (a moved selection / a stepped value / a moved
cell, or a verified no-op at a boundary). Passing unit tests on the pure
`Focus.route` → `NavIntent` mapping alone do **not** satisfy `[X]` for a `[US*]`
task. Because the consumer runtime model is untouched, the MVU evidence for these
stories is the resolver reading the live selection/value model through the real
seam and dispatching the role's binding — no hand-seeded identity map (mirrors the
landed `Feature094*` routing tests).

## Success-criterion → assertion mapping

- **SC-001** (a focused **selection role** (radio-group/tab) moves selection on
  arrow keys and dispatches its `"selected"`/`"changed"` binding with the moved
  selection on the **live host**; a pre-R5 build dispatches nothing and fails) →
  T009 failing-first selection-move test through `runInteractiveApp` + T010
  resolver `SelectionMove` arm + T011 (`responds-vs-renders.md`).
- **SC-002** (a focused slider steps by its **declared step** within declared
  bounds, while a **default-step** slider stays **byte-identical** to the pre-R5
  numeric path — non-regressive) → T012 failing-first declared-step + golden test +
  T013 resolver `ValueStep` arm (declared `NavRange`, replaces hardcoded `0.1`/`0..1`)
  + T014 (`declared-step.md`).
- **SC-003** (a focused **grid role** moves selection by a 2-D delta on arrow keys
  and dispatches the resulting coordinate/item, edge behavior follows the clamp
  policy) → T015 failing-first grid 2-D move test + T016 resolver `GridMove` arm +
  T017 (`role-coverage.md`).
- **SC-004** (navigation is **metadata-driven** — every covered role's outcome is
  reproduced purely from its declared role + `NavigationKeys` metadata and the
  closed intent/payload model, no per-kind host special-casing beyond role
  classification; `Accessibility.validate` passes for the navigated controls) →
  T018 metadata-driven + `Accessibility.validate` suite + T019 (`role-coverage.md`).
- **SC-005** (`NavIntent` and navigation `ControlEvent` payloads are a **closed,
  exhaustively-matched set**; no free-form key-handler surface) → T018 FsCheck
  closed-set/exhaustiveness proof + T019 (`closed-model.md`).
- **SC-006** (representative tests cover a **value role, a linear selection role,
  and a grid role** — not slider-only — including the boundary (clamp) policy for
  each) → T009 (selection clamp) + T012 (value min/max clamp) + T015 (grid edge
  clamp) + T018 (role-coverage assertion).
- **SC-007** (the escalated serialized order is green with `EvidenceAudit` passing,
  no synthetic/stub work; surface baselines recaptured) → T020 surface recapture +
  T022 first-four sequential + T023 graph + T024 audit (`surface-baseline.md` /
  `validation-log.md` / `evidence-graph.md` / `evidence-audit.md`).

## Non-SC requirement traceability

- **FR-001** (closed navigation intent derived from role + key, not a slider-shaped
  assumption; intent set `ValueStep | SelectionMove of Direction | GridMove`) →
  T006 (`Focus.route` role → `NavIntent`).
- **FR-002** (range/value roles step by **declared** `NavRange.Step` within
  min/max, not a hardcoded constant; dispatch the value binding) → T013.
- **FR-003** (linear selection roles move the index prev/next/first/last from the
  existing selection model; dispatch the `"selected"` binding with a closed
  selection payload) → T010.
- **FR-004** (grid roles apply a 2-D delta and dispatch `"selected"` with the
  resulting cell, clamped/wrapped at edges) → T016.
- **FR-005** (the nav payload shapes are a **closed set** on `ControlEvent` — no
  per-control hand-rolled payload, no free-form key handler) → T005 (the closed
  `Nav : NavPayload option` field) + T018 (the closed-set proof).
- **FR-006** (navigation driven **purely** from declared role + `NavigationKeys` +
  the existing model; no new open key-handler API, no per-kind host special-casing
  beyond role → intent classification) → T006 (the only role-specific branch) +
  T010/T013/T016 (the resolver branches on the **intent**, not the kind).
- **FR-007** (existing slider behavior remains correct as the `ValueStep` arm — a
  default-step slider byte-identical, a non-default-step slider now steps by its
  declared step) → T013 (byte-identical default-step golden; declared-step gain).
- **FR-008** (arrow/Home/End on a focused control with no matching `NavigationKeys`
  is a navigation **no-op**; E4 Space/Enter activation unaffected) → T006
  (`Fallthrough`) + T018 (non-navigable button no-op test).
- **FR-009** (boundary behavior follows a **single stated policy** — default clamp —
  applied uniformly across value, selection, grid) → T010 + T013 + T016 (clamp
  no-op at the bound in each resolver arm).
- **FR-010** (verification covers a **representative role from each intent class**,
  each validated against `Accessibility.validate`) → T018 + T019.

## Governance risk levels

- **Small** — the pure `Focus.route` → `NavIntent` mapping and the closed
  `NavIntent`/`NavPayload` exhaustiveness (totality, role → one intent class,
  no-`NavigationKeys` `Fallthrough`): focused validation is `Dev` + the targeted
  `Controls.Tests/Feature100*` route + closed-set suites.
- **Medium** — the host per-intent resolver in `routeFocusedKey` (selected-then-
  changed binding match, declared-step value clamp, grid 2-D clamp, dual-set
  `Payload`+`Nav`, empty/unset/boundary no-ops) driven through the live adapter:
  `Dev` + the `Elmish.Tests/Feature100*` selection-move / declared-step /
  grid-move / boundary-clamp / non-navigable-button suites.
- **Broad** — escalation **applies**: the `src/Controls/**/*.fsi` surface change
  (Focus/Types/Accessibility) forces the serialized `Dev → GeneratedGuidanceCheck →
  TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit`
  maintainer-verify path. **`Route` is authoritative** — run `./fake.sh build -t
  Route` against the actual diff and run exactly the gates it prints (`--enforce`
  for missing required evidence). FAKE-backed targets run **sequentially** (shared
  `.fake` state); aggregate results are recorded as **non-authoritative** unless
  re-confirmed sequentially.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]**, **[US4]** — user-story scope
- Every task has a matching `tasks.deps.yml` entry; every line mirrors the
  structured `skillist` via `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory artifacts are present and linked (`spec.md`, `plan.md`, `research.md`, `data-model.md`, `quickstart.md`, `contracts/Focus.nav.fsi`, `contracts/Types.nav.fsi`, `contracts/resolver.behavior.md`, `checklists/requirements.md`) and that `.specify/feature.json` resolves `specs/100-general-navigation-keys`
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold audit-discoverable readiness placeholders under `readiness/`: `responds-vs-renders.md`, `declared-step.md`, `role-coverage.md`, `closed-model.md`, `surface-baseline.md`, `fsi-transcript.md`, `validation-log.md`, plus `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `visual-evidence-honesty.md`, `window-visibility.md`, `real-image-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action (use `key=value` lines, not bare image-filename claims; `window-visibility.md` records the not-applicable decision with honest values per T003; `real-image-evidence.md` records the **responds-vs-renders** capture through the real `runInteractiveApp` seam as the rendered-output evidence, cross-referencing `responds-vs-renders.md` — there is no persistent-launch / window obligation)
- [X] T003 [P] [skillist: []] Record feature Tier 1 (contracted: public `.fsi` moves in `Focus`/`Types`/`Accessibility`), affected layers (`FS.Skia.UI.Controls` — `Focus.fsi`/`.fs` `Direction`/`NavIntent`/widened `route`; `Types.fsi`/`.fs` `NavRange`/`NavPayload`/`ControlEvent.Nav`/`AccessibilityMetadata.Navigation`; `Accessibility.fsi`/`.fs` `metadata` widening + per-role `NavigationKeys`/`NavRange`; `FS.Skia.UI.Controls.Elmish` — `ControlsElmish.fs` `Navigate`-arm resolver only, module-internal), public-API impact (the **public** `runInteractiveApp`/`InteractiveAppHost` surface is **unchanged**; only the three Controls `.fsi` files move; `Payload : string option` retained on `ControlEvent`), MVU applicability (no new consumer `Model`/`Msg`/`Effect`/`update`; `Focus.route` + resolver are pure; navigation produces `'msg list`, no I/O; the host loop is the interpreter edge reusing the landed E4 seam + E2 retained identity), and the four evidence obligations from the plan; record as a **visible decision** that the persistent-launch / viewer-launch task-generation rule does **not** newly apply (no default-exe / persistent-launch entry point added; navigation is observed through the existing `runInteractiveApp` seam; at-rest rendered output unchanged; no window-visibility / screenshot obligation)
- [X] T004 [skillist: []] Run `./fake.sh build -t Route` (note: `Route` escalates only **after** the `.fsi` edits exist — T004 records the **expected** escalation, T022/T023/T024 verify it on the real diff); confirm the `src/Controls/**/*.fsi` change **escalates** to the serialized six-target maintainer-verify path (`Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit`) and record the authoritative gate list plus the small/medium/broad governance risk levels into `readiness/governance-risk-levels.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-ui-widgets] In `src/Controls/Types.fsi` + `Types.fs` add the closed value types and the two new optional fields (data-model §`NavRange`/`NavPayload`/`AccessibilityMetadata`/`ControlEvent`): `type NavRange = { Step: float; Min: float; Max: float }`; `type NavPayload = SteppedValue of value: float | MovedSelection of index: int * item: string option | MovedCell of row: int * col: int`; add `Navigation : NavRange option` to `AccessibilityMetadata` and `Nav : NavPayload option` to `ControlEvent`, **retaining** `Payload : string option` (research R-3 — avoid churning every existing click/changed/text/pointer event). Define the types in **both** the `.fsi` and `.fs`; update **every** framework-internal `AccessibilityMetadata`/`ControlEvent` construction site in the same change to supply the new field (`Navigation = None` / `Nav = None` defaults). Capture the current `FS.Skia.UI.Controls` api-surface + per-package `.fsi.txt` baselines as the **pre-change reference** for the Phase-7 recapture (SC-007)
- [X] T006 [skillist: fs-skia-keyboard-input] In `src/Controls/Focus.fsi` + `Focus.fs` add `type Direction = Previous | Next | First | Last` and `type NavIntent = ValueStep of delta: float | SelectionMove of Direction | GridMove of rowDelta: int * colDelta: int`; change `KeyRouting.Navigate` from nullary to `Navigate of NavIntent`; **widen** `route` to take the control's `AccessibilityRole` + declared `NavRange option` alongside the keyboard op + key, keeping the unchanged E4 precedence (activation & navigation membership tested **before** the Tab test) (research R-1/R-5, data-model §`KeyRouting`). `route` is the **single role-specific branch** (FR-006): map role + key to the intent class per orientation — linear selection `ArrowUp`/`ArrowLeft`→`Previous`, `ArrowDown`/`ArrowRight`→`Next`, `Home`→`First`, `End`→`Last`; range `ArrowRight`/`ArrowUp`→`+Step`, `ArrowLeft`/`ArrowDown`→`−Step`, `Home`→min, `End`→max (only when a `NavRange` is present); grid `ArrowUp/Down`→`(±1,0)`, `ArrowLeft/Right`→`(0,±1)`. A key **absent** from the role's `NavigationKeys` → `Fallthrough` (FR-008 no-op). `route` stays **pure & total**; `ValueStep` carries a **delta** (declared `Step` × sign), not a resolved value — the host applies + clamps (research R-1)
- [X] T007 [skillist: fs-skia-ui-widgets] In `src/Controls/Accessibility.fsi` + `Accessibility.fs` widen `metadata` to accept a `NavRange option` and thread it into the produced `AccessibilityMetadata.Navigation`; keep `keyboardFor`'s already-declared per-role `NavigationKeys` (Tab Left/Right; RadioGroup all four; Grid all four — research R-5) unchanged. Declare a **default-step slider** `NavRange` of `{ Step = 0.1; Min = 0.0; Max = 1.0 }` so the pre-R5 constant is reproduced **byte-identically** (FR-007); leave non-range roles `Navigation = None`. Confirm `validate` still accepts a range role with `Navigation = None` (it simply cannot value-step — FR-008) and continues to flag a focusable control with no operable key set
- [X] T008 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope, permanent non-goals, and failure diagnostics into `readiness/runtime-limitations.md` (Out of Scope / Assumptions): no consumer-facing custom key-binding/remapping API and no free-form per-key handler surface (would drift toward the rejected routed-event system); no authored navigation DSL; no type-ahead / incremental-search selection; no multi-select range extension (Shift-arrow) — single-selection moves only; no drag-reorder; **no** focus-traversal (Tab/Shift-Tab) or activation (Space/Enter) change — those are E4, unchanged; full-52-control navigation coverage beyond the representative value/selection/grid roles is a later fitness pass; boundary policy defaults to **clamp** (wrap is opt-in metadata, not shipped here); the honest failure modes are **no-ops with no spurious dispatch** (no `NavigationKeys`, empty group, unset index, boundary clamp), asserted as verified outcomes; this is the **final** roadmap remediation (R1–R5) — no successor

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — a focused selection control moves selection on arrow keys

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fs-skia-elmish, fs-skia-testing] Add the failing-first **selection-move** suite (`tests/Elmish.Tests/Feature100*`, fails against the un-wired slider-only `Navigate` arm; SC-001/FR-003/FR-009): focus a **radio-group** with several items authored the documented way (`"selected"`/`"changed"` binding, **no custom key handler**) through the **real** `runInteractiveApp` host seam, press Down/Up (and Home/End per role), and assert the dispatched `'msg`/`ControlEvent` carries the moved index/item — `Payload = Some itemId` **and** `Nav = Some (MovedSelection (newIndex, Some itemId))` (research R-2 dual-set) — on the role's selected-then-changed binding; assert **boundary clamp** (last + Next, first + Previous → **no dispatch**) and **empty group / unresolvable current index → no dispatch** (research R-7). Add the paired pure `tests/Controls.Tests/Feature100*` assertion that `Focus.route` for a linear selection role + arrow key yields the exact `SelectionMove Direction`. A pre-R5 build dispatches nothing and fails
- [X] T010 [US1] [skillist: fs-skia-elmish, fs-skia-keyboard-input] Replace the slider-only `Navigate` arm of `routeFocusedKey` (`src/Controls.Elmish/ControlsElmish.fs:455–478` — **line refs are indicative; locate the `Navigate` arm by name**, the working tree may have drifted) with the **uniform per-intent resolver** (a pure `(node, NavIntent) -> 'msg list`, module-internal; contracts/resolver.behavior.md) and implement the `SelectionMove dir` arm fully (makes T009 **GREEN**; FR-003/FR-006): read `Items` (count) + current index (index of current `value`/`selected` in `Items`, `src/Controls/Widgets/Input.fs:18–22`, `Control.fs:1616–1620`); empty items or unresolved index → **no dispatch**; compute `Previous=i-1`/`Next=i+1`/`First=0`/`Last=n-1`, **clamp** to `[0,n-1]`, clamped==current → **no dispatch**; dispatch the role's binding matching `EventKind = "selected"` then falling back to `"changed"` (research R-2) with `Payload = Some itemId` **and** `Nav = Some (MovedSelection ...)`. The resolver branches on the **intent** (not the kind); the `ValueStep` arm initially ports the existing `steppedValue` behavior unchanged (US2 makes it declared-step) and the `GridMove` arm is a **no-dispatch** placeholder reproducing pre-R5 grid behavior (US3 completes it) so the match is total with no stub marker. The public `ControlsElmish.fsi` surface stays unchanged
- [X] T011 [US1] [skillist: fs-skia-evidence-mode] Capture US1 to `readiness/responds-vs-renders.md` (the real `runInteractiveApp` seam via the compiled self-closing host, `live-vulkan-window-x11-path`): a focused radio-group/tab arrow press moves selection and dispatches its binding with the moved item; name the items, the pressed keys, and the dispatched `MovedSelection`; an un-wired/pre-R5 build dispatches nothing and cannot produce this artifact (SC-001)

**Checkpoint**: User Story 1 is functional and testable independently.

---

## Phase 4: User Story 2 (US2) — a focused slider steps by its declared step

### Tests First (Principle I, Principle VI)

- [X] T012 [P] [US2] [skillist: fs-skia-elmish, fs-skia-testing] Add the failing-first **declared-step** + **non-regressive golden** suite (`tests/Elmish.Tests/Feature100*` + `tests/Controls.Tests/Feature100*`; SC-002/FR-002/FR-007/FR-009): focus a slider declared with a **non-default** `NavRange` (e.g. `{ Step = 5.0; Min = 0.0; Max = 100.0 }`) through the real seam, press arrows, assert the value moves by **exactly** the declared step within bounds and the dispatched `Nav = Some (SteppedValue target)` matches (a pre-R5 build steps by the hardcoded `0.1` regardless and fails); assert **min/max clamp** (at the bound + step toward it → **no dispatch**); and pin a **byte-identical golden** for a **default-step** slider (`{ 0.1; 0.0; 1.0 }`) proving the dispatched value equals the pre-R5 `steppedValue` path exactly (non-regressive). Add the paired pure `Focus.route` assertion that a range role + arrow yields `ValueStep (±Step)`
- [X] T013 [US2] [skillist: fs-skia-elmish, fs-skia-keyboard-input] Implement the `ValueStep delta` arm of the resolver (makes T012 **GREEN**; FR-002/FR-007), replacing the hardcoded `navStep = 0.1` / `Math.Clamp(.., 0.0, 1.0)` in `steppedValue` (`src/Controls.Elmish/ControlsElmish.fs:366–381` — **line refs indicative; locate `steppedValue` by name**): read the current value (`controlFloatValue`) and the declared `NavRange { Step; Min; Max }` from the focused control's metadata; `target = clamp(current + delta, Min, Max)`; `target == current` (already at the bound) → **no dispatch** (clamp no-op); else dispatch the value binding (`EventKind = "changed"`) with `Payload = Some (string target)` **and** `Nav = Some (SteppedValue target)`. A default-step slider (`{0.1;0;1}`) produces a value byte-identical to the pre-R5 path (FR-007 / the T012 golden)
- [X] T014 [US2] [skillist: fs-skia-evidence-mode] Capture US2 to `readiness/declared-step.md`: a non-default-step slider steps by its declared step within declared bounds (named step/min/max and the observed stepped values), and a default-step slider's dispatched value is byte-identical to the pre-R5 numeric golden (read from the T012 suite, not assumed) (SC-002)

**Checkpoint**: User Story 2 is functional and testable independently.

---

## Phase 5: User Story 3 (US3) — a focused grid control moves selection in two dimensions

### Tests First (Principle I, Principle VI)

- [X] T015 [P] [US3] [skillist: fs-skia-elmish, fs-skia-testing] Add the failing-first **grid 2-D move** suite (`tests/Elmish.Tests/Feature100*` + `tests/Controls.Tests/Feature100*`; SC-003/FR-004/FR-009): focus a grid/data-grid with known dimensions and a current cell through the real seam, press Up/Down (row) and Left/Right (column), and assert the dispatched `Nav = Some (MovedCell (newRow, newCol))` (and `Payload` set to the resulting cell/item id) matches the expected neighbor; assert **edge clamp** (an edge cell + an outward arrow → **no dispatch**). Add the paired pure `Focus.route` assertion that a grid role + arrow yields the exact `GridMove (rowDelta, colDelta)`. A pre-R5 build (grid does nothing on arrows) fails
- [X] T016 [US3] [skillist: fs-skia-elmish, fs-skia-keyboard-input] Implement the `GridMove (rowDelta, colDelta)` arm of the resolver, replacing the T010 no-dispatch placeholder (makes T015 **GREEN**; FR-004/FR-009): read the grid dimensions (`data-grid` `Columns`/`Rows`) + current `(row, col)` (`FocusedCell`, `src/Controls/Widgets/DataGridWidget.fs:7-8,35`); `newRow = clamp(row + rowDelta, 0, rows-1)`, `newCol = clamp(col + colDelta, 0, cols-1)`; `(newRow,newCol) == (row,col)` → **no dispatch** (edge clamp); else dispatch the selection binding (selected-then-changed, research R-2) with `Nav = Some (MovedCell (newRow,newCol))` and `Payload` set to the resulting cell/item id. Still branches on the **intent** — no per-kind branch beyond the role classification already done in `Focus.route`
- [X] T017 [US3] [skillist: fs-skia-evidence-mode] Capture US3 into `readiness/role-coverage.md` (the grid section): a focused grid moves selection by a 2-D delta and dispatches the resulting coordinate, with edge clamp; name the grid dims, current cell, pressed keys, and dispatched `MovedCell`, validated against `Accessibility.validate` for the grid role (SC-003)

**Checkpoint**: User Story 3 is functional and testable independently.

---

## Phase 6: User Story 4 (US4) — navigation is metadata-driven and stays a closed model

### Tests First (Principle I, Principle VI)

- [X] T018 [P] [US4] [skillist: fs-skia-testing, fs-skia-ui-widgets] Add the **closed-model + metadata-driven** suite (`tests/Controls.Tests/Feature100*`; SC-004/SC-005/SC-006/FR-005/FR-006/FR-008/FR-010): an FsCheck `Check.One` **exhaustiveness/closed-set** proof that `NavIntent` and `NavPayload` are closed, **totally-matched** sets (a total match arm over every case, one-to-one `NavIntent`↔`NavPayload`; no free-form key surface — research R-8, no `testProperty` in this repo); a **metadata-driven** assertion that each covered role's navigation outcome is reproduced **purely** from its declared role + `NavigationKeys` (+ `NavRange`) metadata and the closed intent/payload model, with the resolver branching only on the intent (no per-kind host special-case); `Accessibility.validate` **passes** for the representative value (slider), linear-selection (radio-group/tab), and grid roles (FR-010); and a **non-navigable button** (no matching `NavigationKeys`) is a navigation **no-op** on arrows while Space/Enter activation (E4) is unaffected (FR-008)
- [X] T019 [US4] [skillist: fs-skia-evidence-mode] Capture `readiness/closed-model.md` (the `NavIntent`/`NavPayload` closed, totally-matched proof — read from the T018 suite, not assumed) and complete `readiness/role-coverage.md` (the value + linear-selection + grid sections, each validated by `Accessibility.validate`, plus the non-navigable-button no-op) (SC-004/SC-005/SC-006/FR-010)

**Checkpoint**: User Story 4 is functional and testable independently.

---

## Phase 7: Integration & Polish

- [X] T020 [P] [skillist: fs-skia-ui-widgets] Recapture the `FS.Skia.UI.Controls` api-surface + per-package `.fsi.txt` baselines (`PerPackageSurface.captureCurrent`; `RefreshSurfaceBaselines` does **not** cover the per-package snapshots) vs the T005 pre-change reference and confirm the diff shows **exactly** the `Focus`/`Types`/`Accessibility` surface moves (`Direction`/`NavIntent`/widened `route`; `NavRange`/`NavPayload`/`ControlEvent.Nav`/`AccessibilityMetadata.Navigation`; widened `metadata`) with no other drift; confirm the public `ControlsElmish.fsi` `runInteractiveApp`/`InteractiveAppHost` surface is **unchanged**; record to `readiness/surface-baseline.md` (SC-007)
- [X] T021 [P] [skillist: fs-skia-ui-widgets] Exercise navigation from FSI against the packed library per `quickstart.md` — host a focused radio-group, press an arrow, observe the selection move + dispatched binding with **zero** consumer key-handling code; host a non-default-step slider and observe declared-step movement; confirm a focused button is a no-op on arrows but activates on Space/Enter — capture the session transcript to `readiness/fsi-transcript.md`
- [X] T022 [skillist: fs-skia-testing] Run exactly the gates `Route` printed (T004) — the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck` prefix **sequentially** (shared `.fake` state, never concurrently) — and record the aggregate results as **non-authoritative** into `readiness/generated-guidance-validation.md` and the run transcript into `readiness/validation-log.md`; rerun any race-like FAKE failure sequentially before any product-regression claim; if an aggregate hangs, record the diagnosis in `readiness/aggregate-hang-diagnostics.md` (SC-007)
- [X] T023 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory` + `tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises; record to `readiness/evidence-graph.md`
- [X] T024 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic-propagation + diff-scan; no synthetic/stub work) or document every `--accept-synthetic` override; record to `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. **None planned** — see
the Status Legend rationale (`Focus.route` and the host resolver are pure, total
functions of the declared role + `NavigationKeys`/`NavRange` metadata and the live
selection/value model; every boundary, empty-group, unset-index, and
no-`NavigationKeys` case is a designed **no-op with no spurious dispatch**, normal
control flow rather than an error path to fixture; the US1 selection-move and US2
declared-step proofs run through the **real** `runInteractiveApp` seam; the
closed-model proof is FsCheck `Check.One`-driven). For any `[SEH]` rows, include the
approval label, design-phase source, synthetic input class, expected error
behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
