// CONTRACT SKETCH — FS.Skia.UI.Controls.Elmish pointer MVU bridge (feature 075).
// Target files: src/Controls.Elmish/ControlsElmish.fsi + .fs (ADD to existing module).
// Mirrors interpretKeyboardEffect / interpretControlEffect.

namespace FS.Skia.UI.Controls.Elmish

open FS.Skia.UI.Controls

// AdapterEffect<'msg> is extended only if existing cases cannot carry a pointer
// diagnostic. Preferred: reuse ReportAdapterDiagnostic (map PointerDiagnostic ->
// AdapterDiagnostic) and DispatchProductMessage / DispatchControlRuntimeMessage
// for meaningful outcomes, so NO new AdapterEffect case is required.

module ControlsElmish =
    /// Lower a single PointerInteraction into adapter commands.
    ///   mapInteraction : consumer router for meaningful, control-addressed
    ///                    interactions (click / drag / scroll / hover) -> 'msg.
    /// Framework-only / no-op interactions lower to []. Diagnostics lower to
    /// ReportAdapterDiagnostic. ControlRuntimeMsg list returned by Pointer.update
    /// is dispatched via the existing DispatchControlRuntimeMessage path by the
    /// caller (or a convenience overload below).
    val interpretPointerEffect:
        mapInteraction: (PointerInteraction -> 'msg option) ->
        interaction: PointerInteraction ->
            AdapterCommand<'msg>

    /// Convenience: lower the (PointerInteraction list, ControlRuntimeMsg list)
    /// produced by Pointer.update in one call — interactions through
    /// mapInteraction, runtime messages through DispatchControlRuntimeMessage.
    val interpretPointerOutcome:
        mapInteraction: (PointerInteraction -> 'msg option) ->
        interactions: PointerInteraction list ->
        runtimeMessages: ControlRuntimeMsg list ->
            AdapterCommand<'msg>
