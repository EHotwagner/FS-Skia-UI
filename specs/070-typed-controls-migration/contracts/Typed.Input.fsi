// CONTRACT SKETCH (Phase 1) — pure input/command group. Each carries one optional
// event; `event = None` lowers to NO binding (FR-005), never a default message.
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

// ButtonIntent is reused from the existing 065 Primitives surface.

type IconButtonProps<'msg> =
    { Id: ControlId option
      Text: string
      Enabled: bool
      Intent: ButtonIntent
      OnClick: 'msg option }

type NumericInputProps<'msg> =
    { Id: ControlId option
      Value: float
      Min: float option
      Max: float option
      Step: float
      ReadOnly: bool
      OnChanged: (float -> 'msg) option }

type RadioGroupProps<'msg> =
    { Id: ControlId option
      Items: RadioItem list
      SelectedKey: string option
      OnChanged: (string -> 'msg) option }

type SwitchProps<'msg> =
    { Id: ControlId option
      Checked: bool
      OnChanged: (bool -> 'msg) option }

type SliderProps<'msg> =
    { Id: ControlId option
      Value: float
      Min: float
      Max: float
      Step: float
      OnChanged: (float -> 'msg) option }

module IconButton =
    val defaults: IconButtonProps<'msg>
    /// Lowers ≡ legacy `IconButton.create`; `OnClick = None` → no binding.
    val view: props: IconButtonProps<'msg> -> Widget<'msg>

module NumericInput =
    val defaults: NumericInputProps<'msg>
    val view: props: NumericInputProps<'msg> -> Widget<'msg>

module RadioGroup =
    val defaults: RadioGroupProps<'msg>
    val view: props: RadioGroupProps<'msg> -> Widget<'msg>

module Switch =
    val defaults: SwitchProps<'msg>
    val view: props: SwitchProps<'msg> -> Widget<'msg>

module Slider =
    val defaults: SliderProps<'msg>
    val view: props: SliderProps<'msg> -> Widget<'msg>
