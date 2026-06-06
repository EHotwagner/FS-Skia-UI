// CONTRACT SKETCH (Phase 1) — pure-display group. The byte-final signature is
// fixed during implementation against each control's legacy *.create and pinned
// by its lowering-parity test. Namespace isolates clean names from the legacy
// modules (065 Q2). Every field strongly typed — no `obj` (FR-003/SC-005).
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

type LabelProps<'msg> = { Id: ControlId option; Text: string }
type BadgeProps<'msg> = { Id: ControlId option; Text: string }
type IconProps<'msg> = { Id: ControlId option; Text: string }
type SeparatorProps<'msg> = { Id: ControlId option; Orientation: Orientation }
type ProgressBarProps<'msg> = { Id: ControlId option; Value: float }
type SpinnerProps<'msg> = { Id: ControlId option }
type ValidationMessageProps<'msg> = { Id: ControlId option; Text: string; Severity: ValidationState }
type RichTextProps<'msg> = { Id: ControlId option; Runs: RichTextRun list }       // reuse existing RichText run type
type ImageProps<'msg> = { Id: ControlId option; Value: ImageSource }              // reuse existing image source type

module Label =
    val defaults: LabelProps<'msg>
    /// Lowers structurally equal to `Label.create [ Label.text props.Text ]`.
    val view: props: LabelProps<'msg> -> Widget<'msg>

module Badge =
    val defaults: BadgeProps<'msg>
    val view: props: BadgeProps<'msg> -> Widget<'msg>

module Icon =
    val defaults: IconProps<'msg>
    val view: props: IconProps<'msg> -> Widget<'msg>

module Separator =
    val defaults: SeparatorProps<'msg>
    val view: props: SeparatorProps<'msg> -> Widget<'msg>

module ProgressBar =
    val defaults: ProgressBarProps<'msg>
    val view: props: ProgressBarProps<'msg> -> Widget<'msg>

module Spinner =
    val defaults: SpinnerProps<'msg>
    val view: props: SpinnerProps<'msg> -> Widget<'msg>

module ValidationMessage =
    val defaults: ValidationMessageProps<'msg>
    val view: props: ValidationMessageProps<'msg> -> Widget<'msg>

module RichText =
    val defaults: RichTextProps<'msg>
    /// Lowers structurally equal to `RichText.create [ RichText.runs props.Runs ]`.
    val view: props: RichTextProps<'msg> -> Widget<'msg>

module Image =
    val defaults: ImageProps<'msg>
    val view: props: ImageProps<'msg> -> Widget<'msg>
