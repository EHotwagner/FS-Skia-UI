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

    /// Public contract function exposed by this FS.Skia.UI package.
    /// Feature 096 (R1): the pure, total, deterministic projection from live
    /// interaction state to a single VisualState. Selects the highest-ranked
    /// runtime-derivable state for `controlId` under the fixed closed order
    /// Pressed > Selected > Focused > Hover > Normal (the runtime-derivable tail of
    /// FR-002's Disabled > Validation > Loading > Pressed > Selected > Focused > Hover
    /// > Normal). A control named by no interaction state yields `Normal`. No per-kind
    /// branching; identical inputs always yield an identical result.
    val deriveVisualState: model: ControlRuntimeModel -> controlId: ControlId -> VisualState

    /// Feature 096 (R1): internal host bridge — NOT public surface. Stamps each control's derived
    /// VisualState onto the lowered Control<'msg> tree in the ControlId domain (pre-reconcile),
    /// preserving a consumer-set non-Normal attribute and emitting NOTHING at Normal (byte-identity
    /// at rest). Declared `internal` so the Controls.Elmish host and Controls.Tests / Elmish.Tests
    /// reach it via InternalsVisibleTo without enlarging the package's public contract.
    val internal applyRuntimeVisualState: model: ControlRuntimeModel -> control: Control<'msg> -> Control<'msg>
