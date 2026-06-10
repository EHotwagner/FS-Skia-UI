# Phase 1 Data Model: Lookless Slot Composition (E5)

E5 introduces no new runtime state and no new top-level `Control` field. The "data model"
here is the **IR carrier** for slot fills, the **typed authoring surface** for the
representative kinds, and the **lowering function** that maps `(kind + slot fills) →
Control<'msg>`. All entities ride existing types; the only new public types are the two
`Types.fsi` additions.

## Entity 1 — Slot carrier (IR level, `Types.fsi` / `Types.fs`)

New closed-DU case and `AttrValue` case, mirroring E3's `Style` category and
`StyleClassesValue`.

```fsharp
type AttrCategory =
    | Content
    | Children
    | Layout
    | Style
    | Theme
    | State
    | Validation
    | Accessibility
    | Event
    | Data
    | Slot          // NEW (E5): the category under which named slot fills ride the Attr mechanism

and AttrValue<'msg> =
    | ...                                          // existing cases unchanged
    | StyleClassesValue of StyleClass list         // E3 precedent
    | VisualStateValue of VisualState              // E3 precedent
    | SlotFillsValue of (string * Control<'msg>) list   // NEW (E5): ordered name→fill association list
    | ...
```

**Fields / semantics**:

| Element | Type | Meaning | Validation / rules |
|---|---|---|---|
| `AttrCategory.Slot` | DU case | Marks an `Attr` as carrying slot fills | Closed; only the internal `slotFill` builder produces it |
| `SlotFillsValue` | `(string * Control<'msg>) list` | Each pair = (declared slot name, consumer's fill sub-tree) | Slot **name** is internal plumbing, never a public consumer string; a name **absent** from this list ⇒ that slot is *unfilled* (renders default); a name **present** ⇒ *filled* (renders the sub-tree, even if the sub-tree is empty content) |

**State transitions**: none — this is an immutable value on a `Control`. A control carries at
most one `Slot`-category attribute; the **last** one wins (the codebase's last-writer
convention, same as `styleClasses`).

## Entity 2 — Internal slot builder + extractor (`Control.fs`, `module internal ControlInternals`)

Not public surface; the typed views call the builder, the lowering reads via the extractor.

```fsharp
// In module internal ControlInternals
val slotFill   : fills: (string * Control<'msg>) list -> Attr<'msg>          // create "slot" Slot (SlotFillsValue fills)
val slotFillsOf: attrs: Attr<'msg> list -> (string * Control<'msg>) list      // tryLast "slot" → SlotFillsValue, else []
val slotFor    : name: string -> attrs: Attr<'msg> list -> Control<'msg> option // lookup one named slot
```

- `slotFill` mirrors `Attributes.styleClasses` (`Attributes.fs:71`) but lives **internal**
  (no public free-form slot builder, FR-001).
- `slotFillsOf` mirrors `ControlInternals.styleClassesOf` (`Control.fs:50-61`): `tryLast
  "slot"` → match `SlotFillsValue` → default `[]`.
- `slotFor name` is the per-region lookup the geometry functions call: `Some fill` ⇒ place it;
  `None` ⇒ render that region's default chrome.

## Entity 3 — Default slot content (per kind, in `ControlInternals` geometry)

The behavior-preserving base. Not a value type — it is the existing chrome each kind already
renders for a region.

| Kind | Slot name (internal) | Default content (unfilled) | Filled behavior |
|---|---|---|---|
| `Button` | `leading` | **empty** (zero geometry — label unshifted) | place fill ahead of the label region |
| `Button` | `trailing` | **empty** (zero geometry) | place fill after the label region |
| `Button` | (`label`) | the existing centered text label | — (label is intrinsic, not a public slot here) |
| `Panel` | `header` | **empty** (zero geometry) | place fill as a header region above content |
| `Panel` | (content) | the existing children pass-through | — (content is intrinsic) |
| `Panel` | `footer` | **empty** (zero geometry) | place fill as a footer region below content |

**Invariant (byte-identity, FR-003 / SC-002)**: with **no** slot attribute present, the
geometry function produces output structurally-`Scene`-equal to the captured pre-slot oracle
(`frozenButtonGeom`-style) across the kind's states. Peripheral defaults contribute **zero
geometry**, so the label/content position is invariant.

**Invariant (absent ≠ empty, Edge Cases)**: `leading` absent from `SlotFillsValue` ⇒ default
(no region). `leading` present with empty content ⇒ an empty region by consumer choice.

## Entity 4 — Typed slot-fill surface (`Widgets/Primitives.fsi`, `Widgets/Containers.fsi`)

The **public, closed** authoring surface. Additive fields on existing `Props` records; `None`
default lowers to **no** slot attribute (byte-identical, mirroring E3's `Classes = []`).

```fsharp
// Widgets/Primitives.fsi — ButtonProps gains two slot fields
type ButtonProps<'msg> =
    { Id: ControlId option
      Text: string
      Enabled: bool
      Intent: ButtonIntent
      Classes: StyleClass list
      Leading:  Widget<'msg> option   // NEW (E5): leading slot fill; None ⇒ no slot attr
      Trailing: Widget<'msg> option   // NEW (E5): trailing slot fill; None ⇒ no slot attr
      OnClick: 'msg option }

// Widgets/Containers.fsi — PanelProps gains two slot fields
type PanelProps<'msg> =
    { Id: ControlId option
      Header: Widget<'msg> option      // NEW (E5): header chrome-region fill; None ⇒ no slot attr
      Footer: Widget<'msg> option      // NEW (E5): footer chrome-region fill; None ⇒ no slot attr
      Children: Widget<'msg> list }
```

**Closure property (FR-001 / SC-006)**: the only way to fill a slot is to set one of these
named fields. A consumer cannot reference `Button.Header` or any undeclared slot — there is no
field, so it is a **compile-time error**. No public string-keyed builder exists.

**Lowering (`view`)** follows E3's conditional-yield pattern (`Primitives.fs:85-101`):
```fsharp
let view (props: ButtonProps<'msg>) : Widget<'msg> =
    let slots =
        [ match props.Leading  with Some w -> "leading",  Widget.toControl w | None -> ()
          match props.Trailing with Some w -> "trailing", Widget.toControl w | None -> () ]
    let attrs =
        [ // ... existing attrs ...
          match slots with
          | [] -> ()                                   // byte-identical: no slot attr
          | fills -> yield ControlInternals.slotFill fills ]
    // ... Button.create attrs |> withKeyOpt |> Widget.ofControl
```

`defaults` add `Leading = None; Trailing = None` (Button) and `Header = None; Footer = None`
(Panel).

## Entity 5 — Slot lowering (`(kind + slot fills) → Control<'msg>`)

The pure, total, deterministic mapping (FR-006, SC-005). In `ControlInternals`, the kind's
geometry function:
1. extracts `slotFillsOf attrs`;
2. for each declared region, `slotFor name` → place the fill sub-tree, else render the default;
3. injects every fill sub-tree into the lowered control's `Children` (so the keyed reconciler
   / focus / dispatch see them — Decision 4 in research.md), ordered by region position.

**Properties**:
- *Pure / deterministic*: no IO, no `Date.now`/random; identical `(kind, fills)` ⇒ identical
  IR (SC-005).
- *Total*: every region has a default; lowering never throws (SC-005).
- *Additive*: empty fill set ⇒ pre-slot IR (FR-003).
- *Composable*: fills land in `Children`, inheriting E1–E4 + E2 identity unchanged (FR-004,
  FR-005). Deeply-nested slot-bearing fills lower recursively with no special case.

## What E5 deliberately does NOT add (FR-008 non-goal line)

No `DataContext`, no binding expression / observable, no per-item template instantiation, no
`ControlTemplate` type, no dependency/attached properties, no CSS-selector styling, no new
top-level `Control` field, no second message channel. A slot fill is a **static
`Control<'msg>` value** the consumer's own `view` already computed; the consumer's single
`view`/`update` stays the only model→view→message path.
