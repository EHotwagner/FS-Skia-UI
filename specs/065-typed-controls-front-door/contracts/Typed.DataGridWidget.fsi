// CONTRACT SKETCH (Phase 1) — proposed src/Controls/Widgets/DataGridWidget.fsi
// Stateful typed façade. REUSES the existing DataGridModel/Msg/Effect — no
// parallel state type (FR-006). init/update delegate to DataGrid.*.
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

/// Immutable, compiler-checked authoring surface for a data grid.
type DataGridProps<'msg> =
    { Id: ControlId
      Columns: DataGridColumn list
      Rows: DataGridRow list
      RowHeight: float
      ViewportHeight: float
      SelectedRows: Set<string>
      OnSelectionChanged: (string list -> 'msg) option }

/// Public contract module exposed by this FS.Skia.UI package.
module DataGrid =
    val defaults: controlId: ControlId -> DataGridProps<'msg>
    /// Delegates to DataGrid.init — initial model + effects equal the existing control.
    val init: props: DataGridProps<'msg> -> DataGridModel * DataGridEffect list
    /// Delegates to DataGrid.update — pure transition, no I/O.
    val update: msg: DataGridMsg -> model: DataGridModel -> DataGridModel * DataGridEffect list
    val view: props: DataGridProps<'msg> -> model: DataGridModel -> Widget<'msg>
