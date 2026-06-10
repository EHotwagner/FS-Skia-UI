# Contract: Typed Slot Surface (public consumer API)

The **public, typed, closed-per-kind** authoring surface for filling slots. This is the only
way a consumer fills a slot; it makes an undeclared slot a compile-time error (FR-001,
SC-006). Additive to the existing typed `Props` front doors — a consumer who fills no slot
sees no change (mirrors E3's `Classes = []`).

## `Button` (leaf-with-regions) — `FS.Skia.UI.Controls.Typed`, `Widgets/Primitives.fsi`

```fsharp
type ButtonProps<'msg> =
    { Id: ControlId option
      Text: string
      Enabled: bool
      Intent: ButtonIntent
      Classes: StyleClass list
      Leading:  Widget<'msg> option   // E5: fill the leading slot (e.g. an icon ahead of the label)
      Trailing: Widget<'msg> option   // E5: fill the trailing slot
      OnClick: 'msg option }

module Button =
    val defaults: ButtonProps<'msg>          // Leading = None; Trailing = None (+ existing defaults)
    val view: props: ButtonProps<'msg> -> Widget<'msg>
```

## `Panel` (composite-container) — `FS.Skia.UI.Controls.Typed`, `Widgets/Containers.fsi`

```fsharp
type PanelProps<'msg> =
    { Id: ControlId option
      Header: Widget<'msg> option      // E5: fill/replace the header chrome region
      Footer: Widget<'msg> option      // E5: fill the footer chrome region
      Children: Widget<'msg> list }    // existing content (intrinsic region)

module Panel =
    val defaults: PanelProps<'msg>           // Header = None; Footer = None; Children = []
    val view: props: PanelProps<'msg> -> Widget<'msg>
```

## Lowering behavior (the `view` contract)

| Field value | Lowers to |
|---|---|
| `Leading = None` (and all other slot fields `None`) | **no** slot attribute — byte-identical to the pre-E5 front door |
| `Leading = Some w` | a `Slot`-category attr carrying `("leading", Widget.toControl w)` |
| `Leading = Some emptyWidget` | a fill with empty content — renders an **empty** leading region by consumer choice (≠ unfilled) |
| two fields set | one slot attr carrying both pairs; lowering places each at its region |

The `view` builds the slot fill list by conditionally including each set field (the E3
`Primitives.fs:85-101` conditional-yield pattern), then yields `ControlInternals.slotFill
fills` only when the list is non-empty.

## Closure / non-goal guarantees

| ID | Guarantee | How |
|---|---|---|
| TS-1 | **Typed & closed**: a consumer cannot name a slot a kind does not declare | the slot is a record field; an undeclared name has no field ⇒ compile error |
| TS-2 | **No free-form escape hatch**: no public string-keyed slot builder | the `Attr`-level builder is `internal`; only typed `Props` fields are public (FR-001) |
| TS-3 | **Additive**: filling no slot is byte-identical to pre-E5 | `None` defaults ⇒ no slot attr ⇒ pre-slot lowering (FR-003) |
| TS-4 | **Composes**: a filled `Widget<'msg>` is a real control — binds (E1), styles (E3), focuses (E4), keeps identity (E2) | the fill is injected into `Children` (slot-mechanism.md, step 3) |
| TS-5 | **Single model→view path**: the fill is a static value the consumer's `view` computed | no `DataContext` / binding; `view`/`update` unchanged (FR-002, FR-008) |

## FSI usage (Principle I — exercise the surface before `.fs`)

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

// an icon-leading button (leading slot filled, trailing left default/empty)
let saveButton model =
    Button.view
        { Button.defaults with
            Text = "Save"
            Leading = Some (Icon.view { Icon.defaults with Glyph = "💾" })   // any Widget<'msg>
            OnClick = Some Save }

// a panel with a custom header region, content unchanged
let card model =
    Panel.view
        { Panel.defaults with
            Header   = Some (TextBlock.view { TextBlock.defaults with Text = "Account" })
            Children = [ /* existing content widgets */ ] }
```

(`Icon` is illustrative of "any `Widget<'msg>`" — the slot accepts any sub-tree; it need not be
a catalog primitive.)
