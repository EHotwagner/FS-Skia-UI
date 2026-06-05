// CONTRACT SKETCH (Phase 1) — proposed src/Controls/Widgets/Primitives.fsi
// The four pure (non-stateful) typed controls. Typed modules live under the
// `FS.Skia.UI.Controls.Typed` namespace so they keep clean names without
// shadowing the legacy `module Button`/`module TextBox` in Control.fsi (FR-010).
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

/// Semantic style intent for a button (Variant taxonomy class).
type ButtonIntent =
    | Primary
    | Secondary
    | Danger
    | Ghost

/// Layout orientation for a Stack.
type StackOrientation =
    | Vertical
    | Horizontal

/// Immutable, compiler-checked authoring surface for a text block.
type TextBlockProps<'msg> =
    { Id: ControlId option
      Text: string }

/// Immutable, compiler-checked authoring surface for a button. `OnClick = None`
/// lowers to NO event binding (FR-008 edge case), never a default message.
type ButtonProps<'msg> =
    { Id: ControlId option
      Text: string
      Enabled: bool
      Intent: ButtonIntent
      OnClick: 'msg option }

/// Immutable, compiler-checked authoring surface for a checkbox.
type CheckBoxProps<'msg> =
    { Id: ControlId option
      Text: string
      Checked: bool
      OnChanged: (bool -> 'msg) option }

/// Immutable, compiler-checked authoring surface for a stack container.
type StackProps<'msg> =
    { Id: ControlId option
      Orientation: StackOrientation
      Spacing: float
      Children: Widget<'msg> list }

/// Public contract module exposed by this FS.Skia.UI package.
module TextBlock =
    val defaults: TextBlockProps<'msg>
    val view: props: TextBlockProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Button =
    val defaults: ButtonProps<'msg>
    val view: props: ButtonProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module CheckBox =
    val defaults: CheckBoxProps<'msg>
    val view: props: CheckBoxProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Stack =
    val defaults: StackProps<'msg>
    val view: props: StackProps<'msg> -> Widget<'msg>
