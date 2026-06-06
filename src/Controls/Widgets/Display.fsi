namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

/// Immutable, compiler-checked authoring surface for rich text. `runs` required.
type RichTextProps<'msg> =
    { Id: ControlId option
      Runs: RichTextRun list }

/// Immutable, compiler-checked authoring surface for a short-form label.
type LabelProps<'msg> =
    { Id: ControlId option
      Text: string }

/// Immutable, compiler-checked authoring surface for an image. `value` required.
type ImageProps<'msg> =
    { Id: ControlId option
      Value: string }

/// Immutable, compiler-checked authoring surface for an icon glyph. `text` required.
type IconProps<'msg> =
    { Id: ControlId option
      Text: string }

/// Immutable, compiler-checked authoring surface for a visual separator.
type SeparatorProps<'msg> =
    { Id: ControlId option }

/// Immutable, compiler-checked authoring surface for a compact status badge.
type BadgeProps<'msg> =
    { Id: ControlId option
      Text: string }

/// Immutable, compiler-checked authoring surface for a determinate progress bar.
type ProgressBarProps<'msg> =
    { Id: ControlId option
      Value: float }

/// Immutable, compiler-checked authoring surface for an indeterminate spinner.
type SpinnerProps<'msg> =
    { Id: ControlId option }

/// Immutable, compiler-checked authoring surface for a validation message.
type ValidationMessageProps<'msg> =
    { Id: ControlId option
      Text: string
      Severity: ValidationState }

/// Public contract module exposed by this FS.Skia.UI package.
module RichText =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: RichTextProps<'msg>
    /// Lowers structurally equal to `RichText.create (RichText.block props.Runs) []`.
    val view: props: RichTextProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Label =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: LabelProps<'msg>
    /// Lowers structurally equal to `Label.create [ Label.text props.Text ]`.
    val view: props: LabelProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Image =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: ImageProps<'msg>
    /// Lowers structurally equal to `Image.create [ Image.source props.Value ]`.
    val view: props: ImageProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Icon =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: IconProps<'msg>
    /// Lowers structurally equal to `Icon.create [ Icon.name props.Text ]`.
    val view: props: IconProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Separator =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: SeparatorProps<'msg>
    /// Lowers structurally equal to `Separator.create []`.
    val view: props: SeparatorProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Badge =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: BadgeProps<'msg>
    /// Lowers structurally equal to `Badge.create [ Badge.text props.Text ]`.
    val view: props: BadgeProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module ProgressBar =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: ProgressBarProps<'msg>
    /// Lowers structurally equal to `ProgressBar.create [ ProgressBar.value props.Value ]`.
    val view: props: ProgressBarProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Spinner =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: SpinnerProps<'msg>
    /// Lowers structurally equal to `Spinner.create []`.
    val view: props: SpinnerProps<'msg> -> Widget<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module ValidationMessage =
    /// Authoring defaults; optional fields take their value from here.
    val defaults: ValidationMessageProps<'msg>
    /// Lowers structurally equal to `ValidationMessage.create` with the validation severity.
    val view: props: ValidationMessageProps<'msg> -> Widget<'msg>
