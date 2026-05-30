namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type ControlCaret =
    { ControlId: ControlId
      Index: int }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlSelection =
    { ControlId: ControlId
      Start: int
      End: int }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlComposition =
    { ControlId: ControlId
      Text: string }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlDrag =
    { ControlId: ControlId
      StartX: float
      StartY: float
      CurrentX: float
      CurrentY: float }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlRuntimeEffect =
    | FocusChanged of ControlId option
    | HoverChanged of ControlId option
    | PressedControlsChanged of ControlId list
    | CaretChanged of ControlCaret option
    | SelectionChanged of ControlSelection option
    | CompositionChanged of ControlComposition option
    | DragChanged of ControlDrag option
    | StaleTarget of ControlId
    | CancelledInteraction of ControlId option
    | ReportControlRuntimeDiagnostic of ControlDiagnostic

/// Public contract type exposed by this FS.Skia.UI package.
type ControlRuntimeModel =
    { FocusedControl: ControlId option
      HoveredControl: ControlId option
      PressedControls: Set<ControlId>
      Caret: ControlCaret option
      Selection: ControlSelection option
      Composition: ControlComposition option
      ActiveDrag: ControlDrag option
      Diagnostics: ControlDiagnostic list
      RecentEffects: ControlRuntimeEffect list }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlRuntimeMsg =
    | FocusControl of ControlId option
    | HoverControl of ControlId option
    | PressControl of ControlId
    | ReleaseControl of ControlId
    | SetCaret of ControlCaret option
    | SetSelection of ControlSelection option
    | StartComposition of ControlId * string
    | CommitComposition of ControlId
    | StartDrag of ControlId * float * float
    | MoveDrag of float * float
    | EndDrag
    | FocusLost
    | RemoveControl of ControlId
    | RecoverStaleTarget of ControlId
    | CancelInteraction of ControlId option
    | Reset

/// Public contract module exposed by this FS.Skia.UI package.
module ControlRuntime =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: unit -> ControlRuntimeModel * ControlRuntimeEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: ControlRuntimeMsg -> model: ControlRuntimeModel -> ControlRuntimeModel * ControlRuntimeEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val diagnostics: model: ControlRuntimeModel -> ControlDiagnostic list
