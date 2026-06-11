# Feature Specification: General Navigation-Key Delivery

**Feature Branch**: `100-general-navigation-keys`  
**Created**: 2026-06-11  
**Status**: Draft  
**Input**: User description: "create the next part of @docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md"

## Context & Source

This feature is **R5 — General navigation-key delivery** from the controls
architecture evolution roadmap
(`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`, §10.7).
It is the **final** remediation in the roadmap's recommended order
**R1 → {R3, R2} → R4 → R5**, after R1 (feature 096, runtime visual-state bridge),
R2 (feature 097, incremental partial re-layout), R3 (feature 098, binding-aware
recovery), and R4 (feature 099, animation clock on retained identity) have all
shipped and merged.

R5 **completes E4** (the focus / keyboard-traversal / input-routing step). E4's exit
criterion — "a focused non-text control responds to its activation/**navigation**
keys" — is today met only for **numeric (slider) controls**. The host's
focused-key navigation arm filters bindings by `EventKind = "changed"` only and
emits a **hardcoded 0..1 slider float** with a fixed step. Composite selection roles
(radio-group, tab, menu, list, segmented, grid) carry the right arrow
`NavigationKeys` in their accessibility metadata, but they bind via `"selected"` —
which the navigation arm never matches — and even a matching `"changed"` binding
would receive a slider-domain float, not a selection index. So the spec's own
clarification (composite arrows fire the selection binding) is unrealized: a focused
radio-group or tab strip does nothing on arrow keys in the live window.

R5 generalizes the single slider-shaped path into a **metadata-driven, closed
navigation-intent model** that routes the focused control's arrow/home/end keys to
the correct binding and payload for its role — value-step for range roles, selection
moves for selection roles, 2-D moves for grid roles — so navigation works for **all**
interactive roles, not just numeric ones.

R5 is **architecture-preserving and non-goal-preserving**: it finishes wiring a
capability E4 already built the seam for. It introduces no data binding, no
dependency properties, no CSS selectors, no template engine, and no open
key-handler surface (which would drift toward the rejected routed-event system).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A focused selection control moves selection on arrow keys (Priority: P1)

A consumer runs an interactive app with a focusable selection control (a radio-group,
tab strip, segmented control, menu, or list) authored the documented way — a
`"selected"` binding, **no custom key handler**. The user focuses the control (by
Tab traversal or pointer click) and presses an arrow key (or Home/End). The
selection **moves** to the previous/next/first/last item within the group, and the
control's `"selected"` binding is dispatched with a **selection payload** identifying
the newly selected item. The change is visible in the live window (and, via R1, the
newly selected item shows its `Selected`/`Focused` visual).

**Why this priority**: This is the headline gap R5 exists to close — the most common
non-numeric navigation case (radio-group / tabs) is completely dead on arrow keys
today. It is the primary proof that navigation is no longer slider-only.

**Independent test**: On the live host, focus a radio-group with several options and
press Down/Up (or Right/Left per role); the dispatched message reflects the moved
selection index/item, and re-pressing past the last/first item behaves per the
clarified wrap/clamp policy (see Assumptions). A pre-R5 build dispatches nothing and
fails the proof.

### User Story 2 - A focused slider steps by its declared step, not a hardcoded constant (Priority: P1)

A consumer focuses a slider (or other range/value control: numeric stepper,
interactive progress) and presses arrow keys. The value steps by the control's
**declared step metadata** (e.g. a slider with step `5` over `0..100` moves by `5`),
not by a hardcoded `0.1` over an assumed `0..1` domain. Existing slider keyboard
behavior remains correct — it is now one arm (`ValueStep`) of the general model
rather than the only path.

**Why this priority**: Range navigation is the one role that *works* today, but it
works by a hardcoded constant that is wrong for any slider whose domain/step differ
from `0..1`/`0.1`. R5 must generalize without regressing the numeric path, and the
declared-step fix is a real correctness gain, not just a refactor.

**Independent test**: Focus a slider declared with a non-default step and range;
arrow keys move the value by exactly the declared step within the declared bounds;
the dispatched value matches. A golden test pins the numeric path against the prior
behavior for a default-step slider (non-regressive) while proving the declared-step
slider now steps correctly.

### User Story 3 - A focused grid control moves selection in two dimensions (Priority: P2)

A consumer focuses a grid/2-D-selection role and presses arrow keys; the selection
moves by a **2-D delta** (row/column) and dispatches the `"selected"` binding with
the resulting cell coordinate or item, clamped/wrapped at the grid edges per policy.

**Why this priority**: Grid navigation completes the role coverage the roadmap names
(value / selection / grid) and proves the intent model is genuinely general, but it
is a less common role than linear selection, so it is P2 behind the radio-group/tab
and slider cases.

**Independent test**: Focus a grid with known dimensions and current cell; Up/Down
move by a row, Left/Right by a column; the dispatched coordinate matches the expected
neighbor and edge behavior follows the clamp/wrap policy.

### User Story 4 - Navigation is metadata-driven and stays a closed model (Priority: P1)

The navigation behavior of every interactive control is determined **entirely by its
declared accessibility role + `NavigationKeys` metadata and the closed
navigation-intent / payload model** — not by per-control hand-rolled key handling and
not by any consumer-supplied free-form key handler. Adding a new role to the
navigation behavior means classifying it into the existing closed intent set
(value / selection / grid), never opening a new key-handler surface.

**Why this priority**: This is the governance/architecture invariant that keeps R5
inside the constitution and the permanent non-goals. A free-form per-key handler
would re-introduce exactly the routed-event/open-handler design the roadmap rejects.
It is a P1 correctness-of-design criterion validated against `Accessibility.validate`.

**Independent test**: For each covered role, the navigation outcome is reproduced
purely from its declared role + `NavigationKeys` metadata (no per-kind special-case
branch in the host beyond the closed intent resolver); `Accessibility.validate`
passes for the navigated controls; the `NavIntent` and `ControlEvent` navigation
payloads are a closed, exhaustively-matched set.

### Edge Cases

- **Focused control has no navigation metadata / is non-navigable**: An arrow key on
  a focused control that declares no `NavigationKeys` (e.g. a plain button) is a
  **no-op** for navigation (it does not dispatch a selection/value move); activation
  keys (Space/Enter) remain unaffected (E4 behavior).
- **Selection move at a boundary** (first item + previous, last item + next): resolved
  by the **clamp-vs-wrap policy** (see Assumptions) — the default is clamp (stay at the
  boundary, no dispatch beyond it), stated explicitly so implementers do not diverge.
- **Range value at a bound** (min + decrement, max + increment): clamps to the bound;
  no overshoot past min/max; a no-op dispatch policy at the bound matches the slider's
  existing behavior.
- **Empty selection group** (zero items) or **unset current index**: an arrow key is a
  no-op (nothing to move to / from); no spurious dispatch.
- **Both a value and a selection binding present on one control**: the control's
  declared **role** is the single arbiter of which intent arm applies; a role maps to
  exactly one intent class, so there is no ambiguity.
- **Home/End keys**: where the role's `NavigationKeys` declare them, Home → first,
  End → last (selection roles) or min/max (range roles); absent from metadata, they are
  no-ops. **Grid roles declare arrow keys only**, so Home/End on a focused grid fall through
  to a navigation no-op.
- **Grid edge with no neighbor**: clamps at the edge (no row/column past the bounds)
  per the same clamp policy as linear selection.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The focused-key navigation path MUST derive a **closed navigation intent**
  from the focused control's declared **role + key**, not from a single slider-shaped
  assumption. The intent set is closed and exhaustive:
  `ValueStep` (range/value roles) | `SelectionMove of Direction`
  (linear selection roles) | `GridMove of (rowDelta, colDelta)` (grid roles).
- **FR-002**: For **range/value roles** (slider, numeric stepper, interactive progress),
  a `ValueStep` intent MUST step the value by the control's **declared step metadata**
  within its declared min/max bounds — **not** by a hardcoded constant or an assumed
  `0..1` domain — and dispatch the value binding with the stepped value.
- **FR-003**: For **linear selection roles** (radio-group, tab, menu, list, segmented),
  a `SelectionMove` intent MUST move the selection index (previous / next / first / last)
  within the group, reading the item count and current index from the control's
  existing selection model, and dispatch the **selection binding matched by
  `EventKind` — trying `"selected"`, then falling back to `"changed"`** (roles such as
  radio-group and tab bind `"changed"`, so a `"selected"`-only match would leave the
  headline US1 case dead) — with a closed **selection payload** (selected index and/or
  item id).
- **FR-004**: For **grid roles**, a `GridMove (rowDelta, colDelta)` intent MUST apply a
  2-D selection delta and dispatch the `"selected"` binding with the resulting cell
  coordinate / item, clamped or wrapped at the grid edges per the boundary policy.
- **FR-005**: The selection-move and value-step `ControlEvent` payload shapes MUST be a
  **closed set** on `ControlEvent`, so navigation stays metadata-driven with no
  per-control hand-rolled payload construction and no arbitrary/free-form key handler
  surface.
- **FR-006**: Navigation behavior MUST be driven **purely** from the focused control's
  declared accessibility **role + `NavigationKeys` metadata** and the control's existing
  selection/value model. No new open key-handler API, and no per-kind special-casing in
  the host beyond classifying a role into the closed intent set.
- **FR-007**: The existing slider keyboard behavior MUST remain correct as the
  `ValueStep` arm of the general model — a default-step slider behaves as it did before
  R5 (non-regressive), while a non-default-step/range slider now steps by its declared
  step (a correctness gain).
- **FR-008**: An arrow/Home/End key on a focused control whose role declares **no
  matching `NavigationKeys`** MUST be a navigation **no-op** (no selection/value
  dispatch); activation-key delivery (Space/Enter) from E4 MUST be unaffected.
- **FR-009**: Boundary behavior (first/last item, min/max value, grid edge) MUST follow a
  **single stated policy** (default: clamp) applied uniformly across value, selection,
  and grid roles, so different implementers resolve edges consistently.
- **FR-010**: Verification MUST cover a **representative role from each intent class** —
  a value role (slider), a linear selection role (radio-group or tab), and a grid/list
  role — not slider-only, each validated against `Accessibility.validate`.

> Interacting / conflicting requirements: FR-005 and FR-006 are deliberately distinct, not
> redundant — **FR-005 closes the *payload* set** (the `ControlEvent`/`NavPayload` shapes are
> a closed, exhaustively-matched DU), while **FR-006 governs the *routing source*** (the
> behavior is driven purely from declared role + `NavigationKeys` metadata, with no per-kind
> host branch beyond role → intent classification). FR-006 (metadata-driven, closed model, no
> per-kind special-casing) vs. FR-002/FR-003/FR-004 (role-specific value/selection/grid
> behaviors). Resolution: the **role → intent class** mapping is the only role-specific
> branch; once an intent is chosen, the per-intent resolver is uniform and parameterized
> by declared metadata (step, item count, grid dimensions). Adding a role means assigning
> it to one of the three existing intent classes — never adding a new handler or payload
> shape.
>
> FR-007 (non-regressive slider) vs. FR-002 (declared step replaces hardcoded constant):
> "non-regressive" is defined against a **default-step** slider (whose declared step
> equals the old constant), which must stay byte-identical; a slider whose declared step
> differs is *expected* to change (it was previously wrong), and that change is the
> intended correctness gain, not a regression.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This
> section is *expected* to name concrete packages, `.fsi` signatures, build targets,
> effects/interpreters, Vulkan/Skia, and evidence paths — that is its purpose.

- **Package impact**: Active packages `FS.Skia.UI.Controls` (the `Focus.route` →
  `NavIntent` model, `ControlEvent` navigation payloads, selection controls exposing
  index/count, role/`NavigationKeys`/step metadata) and `FS.Skia.UI.Controls.Elmish`
  (the `routeFocusedKey` `Navigate` arm → per-role resolver) change. No legacy Charts
  migration. Generated package consumers (template, generated app host) gain general
  navigation transparently; no consumer API rename is required.
- **Public contract impact**: Expect `.fsi` changes in `src/Controls/Focus.fsi`
  (`route` returning the new closed `NavIntent`), `src/Controls/Types.fsi` /
  `ControlRuntime.fsi` (the closed `ControlEvent` selection-move / value-step payload
  shapes; the `NavIntent` / `Direction` types), and possibly
  `src/Controls.Elmish/ControlsElmish.fsi` if the resolver seam is promoted beyond
  module-internal. The accessibility role/`NavigationKeys`/step metadata surface in
  `src/Controls/Accessibility.fsi` may widen. Any `.fsi` edit escalates to the
  controls-public-surface (agent-ready / maintainer-verify) route and requires
  recaptured published api-surface + per-package baselines.
- **State workflow impact**: The interactive host's focused-key routing changes — the
  `Navigate` arm now classifies the focused control's role into the closed `NavIntent`
  and resolves to the correct binding + payload, replacing the slider-only float path.
  This is internal host routing; the MVU `view : 'model -> Control<'msg>` contract and
  the consumer's `'model` are unchanged. The dispatched `'msg` for selection roles now
  carries a selection payload the consumer reads via its `"selected"` binding.
- **Layout/rendering impact**: No layout or rendering algorithm change in this feature —
  navigation produces a `'msg`, and any visible change is the consumer's resulting
  re-render plus (via R1) the moved selection's `Selected`/`Focused` visual. Vulkan/Skia
  output, incremental measure (R2), and scoped repaint are unaffected by the routing
  change itself.
- **Evidence obligations**: A **responds-vs-renders** runtime artifact proving
  arrow-key → selection-move on a focused selection role (radio-group/tab) on the live
  host (a pre-R5 build dispatches nothing and fails it); a **declared-step** proof that a
  non-default-step slider steps by its declared step while a default-step slider stays
  byte-identical (non-regressive numeric golden); a **role-coverage** proof spanning a
  value role, a linear selection role, and a grid role, each validated by
  `Accessibility.validate`; a **closed-model** property/exhaustiveness proof that the
  `NavIntent` and navigation `ControlEvent` payloads are a closed, totally-matched set.
  Real evidence paths under `specs/100-general-navigation-keys/` (e.g. `evidence/`),
  plus the standard `EvidenceGraph` / `EvidenceAudit` artifacts.
- **Unsupported scope**: No consumer-facing custom key-binding/remapping API and no
  free-form per-key handler surface (would drift toward the rejected routed-event
  system). No new authored navigation DSL. No type-ahead / incremental-search selection,
  drag-reorder, or multi-select range extension (Shift-arrow selection growth) — R5
  delivers single-selection moves only. No focus-traversal change (Tab/Shift-Tab order
  is E4, unchanged). Full-52-control navigation coverage beyond the representative roles
  remains a later fitness pass. Release, platform, and distribution boundaries unchanged.
- **Build-target impact**: `Dev` runs the new unit/property/integration tests. The
  change is consumer-contract-touching (`.fsi` moves in Focus/Types/Accessibility), so
  the escalated path applies: `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit` per the serialized
  six-target order. `Route` is the authority — run `./fake.sh build -t Route` against the
  actual diff and run only the gates it prints; `--enforce` for missing evidence. No new
  FAKE target is expected.

## Success Criteria *(mandatory)*

- **SC-001**: On the live host, a focused **selection role** (radio-group or tab) moves
  its selection on arrow keys and dispatches its `"selected"`/`"changed"` binding (matched
  selected-then-changed, per FR-003) with the moved selection; a pre-R5 build dispatches
  nothing and fails this criterion.
- **SC-002**: A focused slider steps by its **declared step** within its declared bounds
  (verified for a non-default step/range), while a **default-step** slider remains
  **byte-identical** to the pre-R5 numeric path (non-regressive).
- **SC-003**: A focused **grid role** moves selection by a 2-D delta on arrow keys and
  dispatches the resulting coordinate/item, with edge behavior following the stated
  boundary policy.
- **SC-004**: Navigation is **metadata-driven** — every covered role's outcome is
  reproduced purely from its declared role + `NavigationKeys` metadata and the closed
  intent/payload model, with no per-kind host special-casing beyond role classification;
  `Accessibility.validate` passes for the navigated controls.
- **SC-005**: The `NavIntent` and navigation `ControlEvent` payloads are a **closed,
  exhaustively-matched set** (property/exhaustiveness-tested); there is no free-form
  key-handler surface.
- **SC-006**: Representative tests cover a **value role, a linear selection role, and a
  grid role** — not slider-only — including the boundary (clamp) policy for each.
- **SC-007**: The serialized validation order (or the minimal gate set `Route` prints for
  the actual diff) is **green**, with `EvidenceAudit` passing and no synthetic or stubbed
  work.

## Key Entities *(data involved)*

- **Navigation intent (`NavIntent`)**: The closed, role-derived classification of a
  focused control's navigation key — `ValueStep` (range), `SelectionMove of Direction`
  (linear selection), or `GridMove of (rowDelta, colDelta)` (grid). Produced by
  `Focus.route` from the control's declared role + the pressed key.
- **Navigation `ControlEvent` payload**: The closed set of payload shapes carrying a
  navigation outcome — a stepped value (range) or a selection move (index / item id,
  and for grids a coordinate) — dispatched on the control's value or `"selected"`
  binding.
- **Declared navigation metadata**: The role, `NavigationKeys`, and (for range roles)
  declared step/min/max in `AccessibilityMetadata`; the **sole** source of which keys
  navigate and how far, replacing the hardcoded slider constant.
- **Selection model facts**: The item count and current index/coordinate a selection or
  grid control already tracks, read by the resolver to compute the moved selection
  (controls expose index/count for the nav payload).

## Assumptions

- **One role → one intent class.** A control's declared accessibility role maps to
  exactly one of the three intent classes (value / linear-selection / grid). No role is
  navigated by more than one class, so a control bearing both a value and a `"selected"`
  binding is disambiguated by its role alone.
- **Boundary policy defaults to clamp.** At a first/last selection item, a min/max value,
  or a grid edge, navigation **clamps** (stays at the boundary, no dispatch past it) by
  default. A wrap policy is allowed where a role's metadata explicitly opts in, but clamp
  is the stated default so implementers do not diverge. **R5 ships no wrap opt-in metadata
  field** — `NavRange` carries only `Step`/`Min`/`Max`, and every representative role ships
  clamp; the wrap opt-in is a bounded follow-up, not an implementable surface in this
  feature.
- **Single-selection moves only.** R5 delivers prev/next/first/last (and 2-D) **single**
  selection moves. Range-extension (Shift-arrow growing a multi-selection) and type-ahead
  search are out of scope.
- **The control's selection/value model already tracks index/count/current.** Selection
  controls expose (or already hold) the item count and current index R5 reads; R5 surfaces
  these for the nav payload rather than introducing new selection state.
- **`NavigationKeys` metadata is already declared** for the composite roles (confirmed
  present at `src/Controls/Accessibility.fs`) and for sliders; R5 consumes and (where
  needed) widens this metadata rather than inventing a new key map.
- **E4 traversal and activation are landed and unchanged** (feature 094): R5 generalizes
  only the *navigation* (arrow/Home/End) arm; Tab/Shift-Tab focus order and Space/Enter
  activation are not modified.
- **R1 is landed** (feature 096), so a moved selection's `Selected`/`Focused` visual is
  shown automatically on the live path — R5 benefits from but does not depend on it.

## Dependencies

- **E4 (feature 094, focus / keyboard-traversal / input-routing)** — provides the
  `routeFocusedKey` / focused-control key-delivery seam this feature generalizes.
  **R5 sequences after E4.**
- **E2 (features 091 + 092, retained identity)** — provides the stable focused identity
  the navigation routes to.
- **R1 (feature 096, runtime visual-state bridge)** — benefits R5 (the moved selection
  shows its `Selected`/`Focused` visual automatically); not a hard prerequisite.
- Independent of R2 (097), R3 (098), and R4 (099); neither blocks nor is blocked by them.

## Out of Scope

- A consumer-facing custom key-binding / remapping API or any free-form per-key handler
  surface (would drift toward the rejected routed-event system).
- Multi-select range extension (Shift-arrow growing a selection) and type-ahead /
  incremental-search selection.
- Drag-reorder or any pointer-driven selection reordering.
- Focus-traversal order (Tab/Shift-Tab) and activation keys (Space/Enter) — those are E4,
  unchanged here.
- Full-52-control navigation coverage beyond the representative value/selection/grid roles
  (a later fitness pass).
- This is the **final** roadmap remediation (R1–R5); there is no successor remediation in
  the controls architecture evolution roadmap.
