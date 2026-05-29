namespace FS.Skia.UI.Controls

module Control =
    val create: kind: ControlKind -> attrs: Attr<'msg> list -> Control<'msg>
    val standard: kind: StandardControlKind -> attrs: Attr<'msg> list -> Control<'msg>
    val customControl: kind: string -> attrs: Attr<'msg> list -> Control<'msg>
    val lowerStandard: control: Control<'msg> -> Control<'msg>
    val lowerCustom: control: Control<'msg> -> Control<'msg>
    val withKey: key: ControlId -> control: Control<'msg> -> Control<'msg>
    val render: theme: Theme -> control: Control<'msg> -> ControlRenderResult<'msg>
    val diagnostics: control: Control<'msg> -> ControlDiagnostic list
    val dispatch: event: ControlEvent -> control: Control<'msg> -> 'msg list
    val count: control: Control<'msg> -> int

module TextBlock =
    val create: Attr<'msg> list -> Control<'msg>
    val text: string -> Attr<'msg>

module Label =
    val create: Attr<'msg> list -> Control<'msg>
    val text: string -> Attr<'msg>

module Image =
    val create: Attr<'msg> list -> Control<'msg>
    val source: string -> Attr<'msg>

module Icon =
    val create: Attr<'msg> list -> Control<'msg>
    val name: string -> Attr<'msg>

module Separator =
    val create: Attr<'msg> list -> Control<'msg>

module Badge =
    val create: Attr<'msg> list -> Control<'msg>
    val text: string -> Attr<'msg>

module Button =
    val create: Attr<'msg> list -> Control<'msg>
    val text: string -> Attr<'msg>
    val enabled: bool -> Attr<'msg>
    val onClick: 'msg -> Attr<'msg>
    val onClickWith: (ControlEvent -> 'msg) -> Attr<'msg>

module IconButton =
    val create: Attr<'msg> list -> Control<'msg>
    val icon: string -> Attr<'msg>
    val onClick: 'msg -> Attr<'msg>

module CheckBox =
    val create: Attr<'msg> list -> Control<'msg>
    val text: string -> Attr<'msg>
    val checked': bool -> Attr<'msg>
    val onChanged: (bool -> 'msg) -> Attr<'msg>

module Switch =
    val create: Attr<'msg> list -> Control<'msg>
    val checked': bool -> Attr<'msg>
    val onChanged: (bool -> 'msg) -> Attr<'msg>

module Slider =
    val create: Attr<'msg> list -> Control<'msg>
    val value: float -> Attr<'msg>
    val onChanged: (float -> 'msg) -> Attr<'msg>

module NumericInput =
    val create: Attr<'msg> list -> Control<'msg>
    val value: float -> Attr<'msg>
    val onChanged: (float -> 'msg) -> Attr<'msg>

module TextBox =
    val create: Attr<'msg> list -> Control<'msg>
    val value: string -> Attr<'msg>
    val readOnly: bool -> Attr<'msg>
    val validation: ValidationState -> Attr<'msg>
    val onChanged: (string -> 'msg) -> Attr<'msg>

module TextArea =
    val create: Attr<'msg> list -> Control<'msg>
    val value: string -> Attr<'msg>
    val onChanged: (string -> 'msg) -> Attr<'msg>

module RadioGroup =
    val create: Attr<'msg> list -> Control<'msg>
    val items: string list -> Attr<'msg>
    val selected: string -> Attr<'msg>
    val onChanged: (string -> 'msg) -> Attr<'msg>

module Stack =
    val create: Attr<'msg> list -> Control<'msg>
    val children: Control<'msg> list -> Attr<'msg>

module Grid =
    val create: Attr<'msg> list -> Control<'msg>
    val children: Control<'msg> list -> Attr<'msg>

module Dock =
    val create: Attr<'msg> list -> Control<'msg>
    val children: Control<'msg> list -> Attr<'msg>

module Wrap =
    val create: Attr<'msg> list -> Control<'msg>
    val children: Control<'msg> list -> Attr<'msg>

module Border =
    val create: Attr<'msg> list -> Control<'msg>
    val child: Control<'msg> -> Attr<'msg>

module Panel =
    val create: Attr<'msg> list -> Control<'msg>
    val children: Control<'msg> list -> Attr<'msg>

module ProgressBar =
    val create: Attr<'msg> list -> Control<'msg>
    val value: float -> Attr<'msg>

module Spinner =
    val create: Attr<'msg> list -> Control<'msg>

module ValidationMessage =
    val create: Attr<'msg> list -> Control<'msg>
    val text: string -> Attr<'msg>

module Tabs =
    val create: Attr<'msg> list -> Control<'msg>
    val items: string list -> Attr<'msg>
    val selected: string -> Attr<'msg>
    val onChanged: (string -> 'msg) -> Attr<'msg>

module Menu =
    val create: Attr<'msg> list -> Control<'msg>
    val items: string list -> Attr<'msg>
    val onSelected: (string -> 'msg) -> Attr<'msg>

module Toolbar =
    val create: Attr<'msg> list -> Control<'msg>
    val children: Control<'msg> list -> Attr<'msg>

module Tooltip =
    val create: Attr<'msg> list -> Control<'msg>
    val text: string -> Attr<'msg>

module Dialog =
    val create: Attr<'msg> list -> Control<'msg>
    val children: Control<'msg> list -> Attr<'msg>

module Toast =
    val create: Attr<'msg> list -> Control<'msg>
    val text: string -> Attr<'msg>

module Overlay =
    val create: Attr<'msg> list -> Control<'msg>
    val child: Control<'msg> -> Attr<'msg>
