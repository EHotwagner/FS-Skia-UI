# Phase 0 Research: General Navigation-Key Delivery (R5 / feature 100)

All Technical-Context unknowns resolved below. Each entry: **Decision / Rationale /
Alternatives considered**, grounded in the actual source surveyed during planning.

## R-1: Where the navigation intent is produced (`Focus.route` vs. host)

**Decision.** `Focus.route` is widened so its `Navigate` case **carries a closed
`NavIntent`** derived from the focused control's **role + pressed key + declared range
metadata**. The host's `routeFocusedKey` `Navigate` arm becomes a **uniform per-intent
resolver** that reads the live selection/value model and dispatches the role's binding.

`Focus.route` today (`src/Controls/Focus.fs:109-117`) returns
`KeyRouting = Activate | Navigate | Traverse of FocusMove | Fallthrough` from a bare
`KeyboardOperation`. R5 changes the signature to take the control's **role** (and the
declared range metadata) alongside the keyboard op, and returns
`Navigate of NavIntent`.

```fsharp
type Direction = Previous | Next | First | Last
type NavIntent =
    | ValueStep  of delta: float            // signed step delta from declared step × key sign
    | SelectionMove of Direction
    | GridMove   of rowDelta: int * colDelta: int
```

**Rationale.** Key Entities and FR-001 state the intent is *"Produced by `Focus.route`
from the control's declared role + the pressed key"*. Putting the **role → intent-class**
mapping (the single role-specific branch per the interacting-requirements note) inside the
pure `Focus.route` keeps it total, exhaustively testable, and free of host state. The host
resolver stays uniform: it only applies the intent to live model facts (current value /
index / cell) and clamps. This is the FR-006 split — role classification in `route`, a
single parameterized resolver in the host, no per-kind host special-casing.

**Why `ValueStep` carries a *delta*, not a resolved value.** `Focus.route` is pure over
metadata + key and does **not** know the control's current value. The declared step
(metadata) × key sign yields a signed delta; the host adds it to the live value and clamps
to declared min/max. This keeps the declared-step correctness (FR-002/FR-007) in `route`
while the live read + clamp stays in the host where the value lives.

**Alternatives considered.**
- *Keep `route` returning bare `Navigate`, classify in the host.* Rejected — violates the
  spec's "produced by `Focus.route`" entity and scatters the role branch into the host,
  the exact per-kind special-casing FR-006 forbids.
- *`ValueStep of resolvedValue`.* Rejected — would force `route` to take the live current
  value, coupling the pure router to mutable model state.

## R-2: Which binding a `SelectionMove` dispatches — `"selected"` vs. `"changed"`

**Decision.** The resolver dispatches the **selection role's declared binding by
`EventKind`, trying `"selected"` then falling back to `"changed"`** — covering both
conventions present in the codebase — with the selection payload set on **both** the
stringly `Payload` (the moved item id, so existing `onChanged`/`onSelected` string
consumers keep working) **and** the new closed `Nav` payload (R-3).

**Rationale.** The catalog shows selection controls split across two binding kinds:
`onSelected → "selected"` for `list-view`, `list-box`, `menu`, `tree-view`, `data-grid`,
charts (`src/Controls/Catalog.fs:122-214`), but `RadioGroup.onChanged → "changed"`
(`src/Controls/Control.fs:1616-1620`) and Tab similarly. The spec/roadmap frames selection
binding as `"selected"`, but a resolver that matched only `"selected"` would leave
radio-group — the **headline P1 role (US1)** — dead. Matching the declared selection
binding (selected-then-changed) realizes US1 for radio-group *and* US3-style list/menu
roles without forcing a consumer rename (Framework Governance Prompts: "no consumer API
rename is required"). Dual-setting `Payload` (string) keeps the existing
`onChanged map = event.Payload |> Option.defaultValue "" |> map` consumers green while the
typed `Nav` field satisfies the closed-set obligation (FR-005, SC-005).

**Alternatives considered.**
- *Standardize every selection role on `"selected"` and migrate radio-group/tab.* Rejected
  for R5 — a consumer-visible binding rename, out of scope ("no consumer API rename"),
  and a larger blast radius than the resolver fallback.
- *Match only `"changed"`.* Rejected — strands `onSelected`-bound list/menu/grid roles.

## R-3: Closed `ControlEvent` navigation payload shape

**Decision.** Add a **new closed field** `Nav: NavPayload option` to `ControlEvent`,
leaving the existing `Payload: string option` intact:

```fsharp
type NavPayload =
    | SteppedValue   of value: float
    | MovedSelection of index: int * item: string option
    | MovedCell      of row: int * col: int

type ControlEvent =
    { Kind: string
      ControlId: ControlId option
      Origin: ControlEventOrigin
      Payload: string option
      Nav: NavPayload option }
```

**Rationale.** FR-005/SC-005 require the nav payloads to be a **closed set on
`ControlEvent`**. A new typed field gives the closed, exhaustively-matched set the
property test pins, while preserving `Payload: string option` avoids churning every
existing click/changed/text event construction and consumer (the slider numeric path, the
text seam, pointer dispatch all keep their string `Payload`). The closed `NavPayload`
mirrors the closed `NavIntent` one-to-one (value / selection / grid), so the
exhaustiveness proof is a direct match arm.

**Alternatives considered.**
- *Replace `Payload: string option` with a closed DU.* Rejected — breaks every existing
  `ControlEvent` literal (pointer/click/text/slider) and every consumer that reads the
  string payload; far larger than R5's scope and risks regressing E1/E4 paths.
- *Encode the selection index back into the string `Payload`.* Rejected — stringly-typed,
  fails the "closed set" requirement, and loses the item/coordinate distinction.

## R-4: Where declared step / min / max for range roles live

**Decision.** Extend `AccessibilityMetadata` with a **closed optional range field**
carrying declared `Step`/`Min`/`Max` for range roles, threaded through
`Accessibility.metadata` / `keyboardFor`:

```fsharp
type NavRange = { Step: float; Min: float; Max: float }
// AccessibilityMetadata gains:  Navigation: NavRange option
```

`Focus.route` reads `Navigation` to size a `ValueStep`; the host resolver reads the same
`Navigation` to clamp the stepped value to `[Min, Max]`.

**Rationale.** Key Entities names *"(for range roles) declared step/min/max in
`AccessibilityMetadata`"* as the **sole** source replacing the hardcoded `0.1`/`0..1`
(`steppedValue`, `src/Controls.Elmish/ControlsElmish.fs:366-381`). Putting it in
`AccessibilityMetadata` (alongside `Role` and `Keyboard`) keeps navigation fully
metadata-driven (FR-002, SC-004) and lets `Accessibility.validate` reason about it.
`option` keeps non-range roles (button, radio, list) unaffected and lets a default-step
slider stay byte-identical (FR-007): a slider whose declared `Step = 0.1`, `Min = 0`,
`Max = 1` reproduces the old constant exactly.

**Alternatives considered.**
- *Read step/min/max from control attributes instead of metadata.* Rejected — the spec
  pins them to `AccessibilityMetadata`; attributes are presentation/geometry, and
  `Accessibility.validate` (FR-010) inspects metadata, not attrs.
- *New parallel metadata record outside `AccessibilityMetadata`.* Rejected — two sources
  of navigation truth; the resolver and `validate` would both need to consult two places.

## R-5: Direction mapping and role orientation (arrows / Home / End)

**Decision.** `Focus.route` maps keys to `Direction`/deltas **per role orientation**,
using the role to disambiguate axis:
- Linear selection — `ArrowUp`/`ArrowLeft` → `Previous`, `ArrowDown`/`ArrowRight` → `Next`,
  `Home` → `First`, `End` → `Last` (a role only honours the keys present in its
  `NavigationKeys`; Tab is horizontal-only, radio-group both).
- Range — `ArrowRight`/`ArrowUp` → +step, `ArrowLeft`/`ArrowDown` → −step; `Home` → min,
  `End` → max (where declared).
- Grid — `ArrowUp/Down` → `(±1, 0)`, `ArrowLeft/Right` → `(0, ±1)`.

**Rationale.** `keyboardFor` (`src/Controls/Accessibility.fs:70-96`) already declares
exactly these `NavigationKeys` per role (Tab: Left/Right; RadioGroup: all four; Grid: all
four), so the mapping consumes existing metadata rather than inventing a key map
(Assumptions: "`NavigationKeys` metadata is already declared"). Membership-gating on the
role's `NavigationKeys` makes a key absent from metadata a no-op (FR-008, Edge Cases).

**Alternatives considered.** A global key→direction table independent of role — rejected
because horizontal (Tab) vs. vertical (menu/list) vs. 2-D (grid) need role context, and a
flat table would fire spurious moves for keys a role does not declare.

## R-6: Boundary policy (clamp vs. wrap)

**Decision.** **Clamp by default**, applied uniformly across value, selection, and grid in
the host resolver (stay at the boundary, dispatch nothing past it). Wrap is permitted only
where a role's metadata explicitly opts in; R5 ships clamp for all representative roles.

**Rationale.** FR-009 and Assumptions pin clamp as the single stated default so
implementers do not diverge; it matches the slider's existing `Math.Clamp` at `[0,1]`
(`ControlsElmish.fs:381`). A no-op dispatch at the boundary mirrors the slider's current
behavior (Edge Cases: "no overshoot past min/max").

**Alternatives considered.** Wrap-by-default — rejected; the spec states clamp is the
default and wrap is opt-in metadata.

## R-7: Empty / unset selection model

**Decision.** When the selection model reports **zero items** or **no current index**, an
arrow key is a **no-op** (no dispatch). The resolver reads item count + current index from
the control's existing selection model (`Items` + current `value`/`selected`), mapping the
current item id to an index; an unresolvable current index is treated as "unset" → no-op.

**Rationale.** Edge Cases require empty-group and unset-index to be no-ops with no spurious
dispatch. The selection facts come from the control's existing `Items` list and current
selection attribute (`src/Controls/Widgets/Input.fs:18-22`,
`src/Controls/Control.fs:1616-1620`) — R5 surfaces index/count from these rather than
introducing new selection state (Assumptions).

## R-8: Test & evidence strategy

**Decision.** Three test tiers mirroring E4 (feature 094), plus a live artifact:
1. **Pure `Focus.route` NavIntent tests** (`tests/Controls.Tests/`) — extend
   `Feature094FocusTests` style: given role + metadata + key, assert the exact `NavIntent`;
   an exhaustiveness/property proof that `NavIntent` and `NavPayload` are closed,
   totally-matched sets (FsCheck `Check.One`, per repo convention — no `testProperty`).
2. **Host resolver routing tests** (`tests/Elmish.Tests/`) — extend
   `Feature094FocusRoutingTests` through the real `RetainedRender` seam: a focused
   radio-group moves selection and dispatches its binding with the moved index; a focused
   non-default-step slider steps by its declared step; a focused grid moves 2-D; boundary
   clamp for each; a non-navigable focused button is a no-op.
3. **Non-regressive numeric golden** — a default-step slider's dispatched value stays
   byte-identical to the pre-R5 path (FR-007/SC-002).
4. **Live responds-vs-renders artifact** under
   `specs/100-general-navigation-keys/evidence/` — arrow-key → selection-move on a focused
   radio-group/tab on the live host (a pre-R5 build dispatches nothing and fails it), via
   the compiled self-closing host path (`live-vulkan-window-x11-path`).

`Accessibility.validate` is asserted for each representative role (FR-010/SC-004).

**Rationale.** Mirrors the landed E4 evidence shape (the two `Feature094*` test files) so
the seam is exercised the same way `runInteractiveApp` wires it, with no hand-seeded
identity map. Satisfies the four evidence obligations named in Framework Governance
Prompts: responds-vs-renders, declared-step, role-coverage, closed-model.

**Alternatives considered.** `testProperty` for exhaustiveness — rejected; the repo has no
`testProperty` helper (memory: feature 099 gotcha), use `Check.One`.
