# Contract — Typed Control Public Surface (additive)

The public UI contract this feature exposes to consumers of `FS.Skia.UI.Controls`. All
additions live in the `FS.Skia.UI.Controls.Typed` namespace and are declared in `.fsi`
(Principle II). **No existing declaration changes** (Tier 1, additive-only).

## New value types

```fsharp
namespace FS.Skia.UI.Controls.Typed
open FS.Skia.UI.Controls
open FS.Skia.UI.Scene            // reuse the existing Color type

type SplitButtonItem = { Key: string; Label: string }
type ColorSwatch     = { Name: string; Color: Color }
```

## New control modules (signature shape)

```fsharp
type ToggleButtonProps<'msg> =
    { Id: ControlId option; Text: string; IsOn: bool; Enabled: bool
      OnToggle: (bool -> 'msg) option }
module ToggleButton =
    val defaults : ToggleButtonProps<'msg>
    val view     : props: ToggleButtonProps<'msg> -> Widget<'msg>

type SplitButtonProps<'msg> =
    { Id: ControlId option; Text: string; Enabled: bool; IsOpen: bool
      Items: SplitButtonItem list; OnClick: 'msg option
      OnSelected: (string -> 'msg) option }
module SplitButton =
    val defaults : SplitButtonProps<'msg>
    val view     : props: SplitButtonProps<'msg> -> Widget<'msg>

type DatePickerProps<'msg> =
    { Id: ControlId option; Value: System.DateOnly option; Enabled: bool
      IsOpen: bool; OnChange: (System.DateOnly -> 'msg) option }
module DatePicker =
    val defaults : DatePickerProps<'msg>
    val view     : props: DatePickerProps<'msg> -> Widget<'msg>

type TimePickerProps<'msg> =
    { Id: ControlId option; Value: System.TimeOnly option; Enabled: bool
      OnChange: (System.TimeOnly -> 'msg) option }
module TimePicker =
    val defaults : TimePickerProps<'msg>
    val view     : props: TimePickerProps<'msg> -> Widget<'msg>

type ColorPickerProps<'msg> =
    { Id: ControlId option; Swatches: ColorSwatch list; Selected: ColorSwatch option
      OnSelected: (ColorSwatch -> 'msg) option }
module ColorPicker =
    val defaults : ColorPickerProps<'msg>
    val view     : props: ColorPickerProps<'msg> -> Widget<'msg>
```

## Contract guarantees (asserted by tests)

1. **Existence/contract**: each module exposes `defaults` and `view`; each `Props` type
   exists with the fields above. *(contract test, red-first)*
2. **No `obj`, no string-encoded structured value**: no new `Props` field is `obj`; date/
   time/color use `DateOnly`/`TimeOnly`/`Color`. *(grep + type test)*
3. **Lowering parity**: `view props |> Widget.toControl` is structurally equal to the
   explicit composition of existing legacy builders (order-normalized, events canonicalized).
   *(keystone test, red-first)*
4. **Optional callbacks lower to no binding** when `None`; the control still lowers.
   *(interaction test)*
5. **Composed only from existing `ControlKind`s** — no new IR node; renderer unchanged.
   *(rendering test + manual diff review)*
6. **Additive surface**: the controls public-surface and per-package surface baselines show
   additions only. *(`PackageSurfaceCheck` / `PerPackageSurfaceDiff`)*
