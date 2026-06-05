// CONTRACT SKETCH (Phase 1) — proposed src/Controls/Widgets/TextBoxWidget.fsi
// Stateful typed façade. REUSES the existing TextInputModel/Msg/Effect — no
// parallel state type (FR-006). init/update delegate to TextInput.* and their
// results are asserted equal to the existing control's (Principle IV).
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

/// Immutable, compiler-checked authoring surface for a text box. `Id` is required
/// identity for a stateful control. `OnChanged = None` lowers to no binding.
type TextBoxProps<'msg> =
    { Id: ControlId
      Mode: TextInputMode
      Value: string
      ReadOnly: bool
      Validation: ValidationState
      OnChanged: (string -> 'msg) option }

/// Public contract module exposed by this FS.Skia.UI package.
module TextBox =
    val defaults: controlId: ControlId -> TextBoxProps<'msg>
    /// Delegates to TextInput.init — initial model + effects equal the existing control.
    val init: props: TextBoxProps<'msg> -> TextInputModel * TextInputEffect list
    /// Delegates to TextInput.update — pure transition, no I/O.
    val update: msg: TextInputMsg -> model: TextInputModel -> TextInputModel * TextInputEffect list
    val view: props: TextBoxProps<'msg> -> model: TextInputModel -> Widget<'msg>
