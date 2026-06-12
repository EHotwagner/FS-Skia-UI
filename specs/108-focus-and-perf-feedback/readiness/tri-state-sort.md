# DataGrid tri-state sort proof (T030, SC-008, FR-015)

enforcing-test=tests/Controls.Tests/Feature108CompositionTests.fs

`DataGrid.update (SortBy column)` cycles the sort on the SAME column through three states and clears on
the third toggle; a DIFFERENT column restarts at `Ascending`:

| Press | model.Sort before | model.Sort after | DataGridSortChanged effect |
|-------|-------------------|------------------|----------------------------|
| 1 (col "name") | None | Some { name; Ascending } | Some { name; Ascending } |
| 2 (col "name") | Some Ascending | Some { name; Descending } | Some { name; Descending } |
| 3 (col "name") | Some Descending | None | None  (clearing transition) |
| (col "b" after "a"=Asc) | Some { a; Ascending } | Some { b; Ascending } | Some { b; Ascending } |

The clearing transition emits `DataGridSortChanged None`, so the consumer no longer intercepts the third
press to clear the sort. No `.fsi` type change (behaviour-only on `DataGrid.fs`).
