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
    /// Authoring defaults; optional fields take their value from here.
    val defaults: TextBlockProps<'msg>
    /// Lowers structurally equal to `TextBlock.create [ TextBlock.text props.Text ]`.
    val view: props: TextBlockProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Button =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: ButtonProps<'msg>
    /// Lowers structurally equal to the legacy `Button.create` attrs;
    /// `OnClick = None` lowers to no event binding.
    val view: props: ButtonProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module CheckBox =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: CheckBoxProps<'msg>
    /// Lowers structurally equal to the legacy `CheckBox.create` attrs;
    /// `OnChanged = None` lowers to no event binding.
    val view: props: CheckBoxProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Stack =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: StackProps<'msg>
    /// Lowers children via `Widget.toControl` into `Stack.children`, order preserved.
    val view: props: StackProps<'msg> -> Widget<'msg>
