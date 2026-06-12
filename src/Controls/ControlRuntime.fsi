namespace FS.Skia.UI.Controls

/// The text caret position (`Index`) within a focused control identified by `ControlId`.
type ControlCaret =
    { ControlId: ControlId
      Index: int }

/// A text selection range (`Start`..`End`) within the control identified by `ControlId`.
type ControlSelection =
    { ControlId: ControlId
      Start: int
      End: int }

/// In-flight IME composition `Text` being entered into the control identified by `ControlId`.
type ControlComposition =
    { ControlId: ControlId
      Text: string }

/// An active pointer drag on `ControlId`, tracking start (`StartX`/`StartY`) and current (`CurrentX`/`CurrentY`) coordinates.
type ControlDrag =
    { ControlId: ControlId
      StartX: float
      StartY: float
      CurrentX: float
      CurrentY: float }

/// An observable side effect emitted by `ControlRuntime.update` when interaction state changes (focus, hover, caret, selection, drag, diagnostics).
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

/// The aggregate runtime interaction state: focused/hovered/pressed controls, `Caret`, `Selection`, `Composition`, `ActiveDrag`, and accumulated `Diagnostics`.
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

/// An input message driving the runtime transition, e.g. `FocusControl`, `HoverControl`, `PressControl`, `SetCaret`, `StartDrag`, or `Reset`.
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

/// MVU runtime tracking control focus, hover, press, caret/selection, composition, drag, and derived visual state.
module ControlRuntime =
    /// Seeds an empty `ControlRuntimeModel` with no focus or interaction and its initial effects.
    val init: unit -> ControlRuntimeModel * ControlRuntimeEffect list
    /// Pure transition applying `msg` to `model`, returning the next model and the `ControlRuntimeEffect` list it raises.
    val update: msg: ControlRuntimeMsg -> model: ControlRuntimeModel -> ControlRuntimeModel * ControlRuntimeEffect list
    /// Returns the `ControlDiagnostic` list currently accumulated in `model`.
    val diagnostics: model: ControlRuntimeModel -> ControlDiagnostic list

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
