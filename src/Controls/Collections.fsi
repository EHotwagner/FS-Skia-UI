namespace FS.Skia.UI.Controls

type VisibleRange =
    { FirstIndex: int
      Count: int
      Total: int }

type CollectionModel =
    { ControlId: ControlId
      ItemCount: int
      RowHeight: float
      ViewportHeight: float
      ScrollOffset: float
      SelectedKeys: Set<string>
      VisibleRange: VisibleRange
      RecalculationThresholdMs: int }

type CollectionMsg =
    | ScrollTo of float
    | SelectKey of string
    | ToggleKey of string
    | ReplaceItemCount of int

type CollectionEffect =
    | VisibleRangeChanged of VisibleRange

module Collections =
    val visibleRange: rowHeight: float -> viewportHeight: float -> scrollOffset: float -> totalItems: int -> VisibleRange
    val init: controlId: ControlId -> itemCount: int -> rowHeight: float -> viewportHeight: float -> CollectionModel * CollectionEffect list
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list
