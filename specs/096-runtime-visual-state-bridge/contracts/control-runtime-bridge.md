# Contract: Runtime Visual-State Bridge

This feature exposes **one** new public function and **one** internal host bridge,
plus a behavioral widening of four geometry functions. The public contract is the
only Tier-1 surface change.

## Public surface (Tier 1) — `FS.Skia.UI.Controls`

### `ControlRuntime.deriveVisualState`

```fsharp
// src/Controls/ControlRuntime.fsi  (added to module ControlRuntime)

/// Public contract function exposed by this FS.Skia.UI package.
/// Feature 096 (R1): the pure, total, deterministic projection from live
/// interaction state to a single VisualState. Selects the highest-ranked
/// runtime-derivable state for `controlId` under the fixed closed order
/// Pressed > Selected > Focused > Hover > Normal (the runtime-derivable tail of
/// FR-002's Disabled > Validation > Loading > Pressed > Selected > Focused > Hover
/// > Normal). A control named by no interaction state yields `Normal`. No per-kind
/// branching; identical inputs always yield an identical result.
val deriveVisualState: model: ControlRuntimeModel -> controlId: ControlId -> VisualState
```

**Guarantees** (property-tested, SC-004):
- Total — defined for every `ControlId`.
- Deterministic — identical `(model, controlId)` ⇒ identical `VisualState`.
- Closed — result is always one of `Pressed | Selected | Focused | Hover | Normal`;
  never `Disabled`/`Validation`/`Loading` (those are consumer-set, not derived).

**Stability / compatibility**: purely additive. No existing signature changes.
`controls-public-surface`, per-package, and cross-package baselines are recaptured.

**FSI exercise** (Principle I):

```fsharp
#r "FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Controls
let m = { fst (ControlRuntime.init ()) with HoveredControl = Some "btn" }
ControlRuntime.deriveVisualState m "btn"   // val it : VisualState = Hover
ControlRuntime.deriveVisualState m "other" // val it : VisualState = Normal
let pressed = { m with PressedControls = Set.ofList [ "btn" ] }
ControlRuntime.deriveVisualState pressed "btn" // val it : VisualState = Pressed (out-ranks Hover)
```

## Internal surface — `FS.Skia.UI.Controls` (NOT in any `.fsi`)

### `applyRuntimeVisualState`

```fsharp
// src/Controls/ControlRuntime.fs  (NOT declared in ControlRuntime.fsi → internal)

// Feature 096 (R1): internal host bridge. Stamps each control's derived VisualState
// onto the lowered Control<'msg> tree in the ControlId domain (pre-reconcile),
// preserving a consumer-set non-Normal attribute and emitting NOTHING at Normal.
// Reached by Controls.Tests / Elmish.Tests via InternalsVisibleTo.
let applyRuntimeVisualState (model: ControlRuntimeModel) (control: Control<'msg>) : Control<'msg>
```

**Contract**:
- For each node (id = `Key |> Option.defaultValue Kind`):
  - consumer non-`Normal` (read via `ControlInternals.visualStateOf`) ⇒ node
    returned unchanged (consumer wins, FR-003).
  - consumer `Normal`/absent and `deriveVisualState model id = Normal` ⇒ node
    returned **unchanged** (no attribute added; byte-identity at rest, FR-005).
  - consumer `Normal`/absent and derived `<> Normal` ⇒ node's `visualState`
    attribute set/replaced to the derived state.
- Recurses the structural `Children` field; pure (no `model` mutation).

**Why internal**: keeps the new public surface to the single `deriveVisualState`
projection; consumers on the built-in retained host get the behavior automatically
without calling the bridge (clarify 2026-06-11).

## Host integration — `FS.Skia.UI.Controls.Elmish`

`renderRetained` (`src/Controls.Elmish/ControlsElmish.fs:555`) assembles a read-only
`ControlRuntimeModel` from the host's live `pointerState` (hover/press) + `focused`
(`RetainedId` resolved to `ControlId` via the prior retained tree) and applies
`applyRuntimeVisualState` to `host.View size model` **before** `RetainedRender.init`/
`step`. On the first frame there is no prior retained tree, so `focused` resolves to
`None` (research §D5) and no focus indicator is derived until focus is established by
post-render interaction. No public surface change to `ControlsElmish`.

## Behavioral widening — `Control.fs` geometry (internal)

`sliderGeom`/`textFieldGeom`/`radioGeom`/`switchGeom` gain `(classes, state)` params
and route their paint through `Style.resolve theme baseStyle classes state`, matching
`buttonGeom`/`checkboxGeom`. At `state = Normal`, `classes = []` the output is
**byte-identical** to today. No public type or `.fsi` change; `Style.fs` unchanged.

## Negative contract (permanent non-goals — FR-009)

The bridge introduces **none** of: data binding, observable property graph,
dependency/attached properties, lookless templates, CSS selectors, a second
consumer-state channel, a new `VisualState` case, a new token literal, or a second
contrast policy. The `view : 'model -> Control<'msg>` consumer contract is unchanged.
