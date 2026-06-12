// CONTRACT SKETCH — feature 108 US5 (FR-015). src/Controls/DataGrid.fs behaviour change.
// The PUBLIC .fsi types are UNCHANGED — only the SortBy update transition changes.
namespace FS.Skia.UI.Controls

// Unchanged public types (for reference):
//   type DataGridSortDirection = Ascending | Descending
//   type DataGridSort = { ColumnKey: string; Direction: DataGridSortDirection }
//   DataGridMsg.SortBy of string
//   effect DataGridSortChanged of DataGridSort option
//
// Behaviour (DataGrid.update on `SortBy column`), three-state cycle on the SAME column:
//   current sort = None                      -> Some { column; Ascending }
//   current sort = Some(column, Ascending)   -> Some { column; Descending }
//   current sort = Some(column, Descending)  -> None          // third toggle clears
//   current sort = Some(other,  _)           -> Some { column; Ascending }
//
// Emits `DataGridSortChanged None` on the clearing transition, so the consumer no
// longer intercepts the third press to clear `Sort = None` (SC-008). An explicit
// `ClearSort` message is an equivalent alternative but is NOT added (the cycle suffices).
