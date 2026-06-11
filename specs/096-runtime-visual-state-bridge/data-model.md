# Phase 1 Data Model: Runtime Visual-State Bridge (R1)

R1 introduces **no new persisted type**. It adds two functions over existing types
and a precedence model. This document enumerates the entities the bridge reads and
produces, and the closed precedence as a decision table.

## Entities (all existing — read-only or pass-through)

### `ControlRuntimeModel` (read-only input)

`src/Controls/ControlRuntime.fs:35`–`44`. The bridge reads four fields:

| Field             | Type                       | Bridge use                               |
|-------------------|----------------------------|------------------------------------------|
| `FocusedControl`  | `ControlId option`         | derives `Focused` for the named id       |
| `HoveredControl`  | `ControlId option`         | derives `Hover` for the named id         |
| `PressedControls` | `Set<ControlId>`           | derives `Pressed` for each contained id  |
| `Selection`       | `ControlSelection option`  | derives `Selected` for `Selection.ControlId` |

All other fields (`Caret`, `Composition`, `ActiveDrag`, `Diagnostics`,
`RecentEffects`) are ignored. The bridge **never mutates** this model.

### `Control<'msg>` (input + output, pass-through-with-stamp)

`src/Controls/Types.fsi:285`–`293`. Relevant fields:

| Field        | Type                   | Bridge use                                        |
|--------------|------------------------|---------------------------------------------------|
| `Key`        | `ControlId option`     | id = `Key |> Option.defaultValue Kind`            |
| `Kind`       | `ControlKind` (string) | id fallback                                        |
| `Attributes` | `Attr<'msg> list`      | read pre-existing `visualState`; set/keep it      |
| `Children`   | `Control<'msg> list`   | recursion target                                  |

`ControlId = string`, `ControlKind = string` (`Types.fsi:7,9`).

### `VisualState` (output codomain)

`src/Controls/Types.fsi:187`: `Normal | Disabled | Hover | Pressed | Focused |
Selected | Loading | Validation of ValidationState`. R1 adds **no case**.

- **Runtime-derivable** (produced by `deriveVisualState`): `Pressed`, `Selected`,
  `Focused`, `Hover`, `Normal`.
- **Consumer-only** (never produced by derivation; only preserved): `Disabled`,
  `Validation`, `Loading`. (`Selected` can arrive on either channel and resolves the
  same way.)

### `Attr.visualState` (the single carrier)

`src/Controls/Attributes.fs:72`: `create "visualState" State (VisualStateValue state)`.
This one attribute is **both** the consumer-intent input channel (read via
`ControlInternals.visualStateOf`) **and** the bridge's output channel. There is no
second/parallel channel (FR-003).

## Function 1 — `deriveVisualState` (PUBLIC, pure, total)

```fsharp
ControlRuntime.deriveVisualState : ControlRuntimeModel -> ControlId -> VisualState
```

Returns the highest-ranked runtime-derivable state for the id, else `Normal`.

### Precedence decision table (highest match wins)

| Rank | Condition on `(model, id)`                                | Result     |
|------|-----------------------------------------------------------|------------|
| 1    | `model.PressedControls.Contains id`                       | `Pressed`  |
| 2    | `model.Selection` is `Some s` and `s.ControlId = id`      | `Selected` |
| 3    | `model.FocusedControl = Some id`                          | `Focused`  |
| 4    | `model.HoveredControl = Some id`                          | `Hover`    |
| 5    | otherwise                                                 | `Normal`   |

The full FR-002 order is `Disabled > Validation > Loading > Pressed > Selected >
Focused > Hover > Normal`; ranks 1–4 above are its runtime-derivable tail. Ranks for
`Disabled`/`Validation`/`Loading` are never reachable from runtime state and are
honoured only by the consumer-preservation rule below.

### Invariants (property-tested, SC-004)

- **Total**: defined for every `ControlId` (an unknown id → `Normal`).
- **Deterministic**: identical `(model, id)` → identical result.
- **Closed/unambiguous**: exactly one result per input; no per-kind branching.

## Function 2 — `applyRuntimeVisualState` (INTERNAL, pure)

```fsharp
applyRuntimeVisualState : ControlRuntimeModel -> Control<'msg> -> Control<'msg>
```

Per node (then recurse `Children`):

```
id        = node.Key |> Option.defaultValue node.Kind
consumer  = ControlInternals.visualStateOf node.Attributes   // absent ≡ Normal
result    =
    if consumer <> Normal then node                          // preserve, no re-stamp
    else
        match deriveVisualState model id with
        | Normal  -> node                                     // FR-005: emit nothing
        | derived -> { node with Attributes = setVisualState derived node.Attributes }
```

where `setVisualState` replaces an existing `visualState` attribute (or appends
one), keeping the last-writer convention `visualStateOf` reads.

### Consumer-vs-derived arbitration table (FR-003)

| Consumer attr | Derived state | Result node attr | Note                              |
|---------------|---------------|------------------|-----------------------------------|
| `Normal`/absent | `Normal`    | none             | byte-identical at rest (FR-005)   |
| `Normal`/absent | `Hover`     | `visualState Hover`   | derived fills the slot       |
| `Normal`/absent | `Pressed`   | `visualState Pressed` | derived fills the slot       |
| `Normal`/absent | `Focused`   | `visualState Focused` | derived fills the slot       |
| `Normal`/absent | `Selected`  | `visualState Selected`| derived fills the slot       |
| `Disabled`    | (any)         | `visualState Disabled` (unchanged) | consumer wins (FR-003) |
| `Selected`    | `Pressed`     | `visualState Selected` (unchanged) | consumer wins over derived |
| `Validation v`| (any)         | `visualState (Validation v)` (unchanged) | consumer wins   |
| `Loading`     | (any)         | `visualState Loading` (unchanged) | consumer wins        |

### Invariants

- **Identity at rest**: a `Normal`-and-unset node is returned **unchanged** (no
  attribute added/removed) → `Scene`-byte-identical to the un-bridged build (FR-005,
  SC-003); E2 `Keep → reuse` and E3 `[] → control` fast paths untouched.
- **Pre-reconcile / `ControlId` domain**: applied before `RetainedRender.step`, so a
  change is a scoped `Update` patch (FR-004, SC-005).
- **Pure**: no mutation of `model`; returns a new tree only where a stamp changes.
- **Kind-agnostic**: the bridge stamps purely from `(consumer-attr, deriveVisualState model id)`; it does **not** gate on migrated-kind membership. Only migrated geometry (FR-006) renders the stamped state, so a non-migrated *interacted* node may carry an inert `visualState` attribute the geometry ignores — visible output is unchanged (SC-006 "no render-output delta"). At-rest byte-identity (FR-005) is defined for non-interacted `Normal`-and-unset controls and is unaffected. (A migrated-kind gate was considered and rejected: it would add a second source of truth for the migrated set alongside the geometry dispatch and complicate the totality story.)

## Migrated-kind state table (FR-006, SC-006)

| Kind          | Geom function     | Reads `state` after R1? | Status        |
|---------------|-------------------|-------------------------|---------------|
| `button`      | `buttonGeom`      | yes (today)             | already migrated |
| `icon-button` | `buttonGeom`      | yes (today)             | already migrated |
| `check-box`   | `checkboxGeom`    | yes (today)             | already migrated |
| `slider`      | `sliderGeom`      | **yes (R1 widens)**     | **new migrant** |
| `text-box`    | `textFieldGeom`   | **yes (R1 widens)**     | **new migrant** |
| `radio-group` | `radioGeom`       | **yes (R1 widens)**     | **new migrant** |
| `switch`      | `switchGeom`      | **yes (R1 widens)**     | **new migrant** |
| all others    | (various)         | no                      | out of scope (R5/E3) |
