namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type VisibleRange =
    { FirstIndex: int
      Count: int
      Total: int }

/// Public contract type exposed by this FS.Skia.UI package.
type CollectionModel =
    { ControlId: ControlId
      ItemCount: int
      RowHeight: float
      ViewportHeight: float
      ScrollOffset: float
      SelectedKeys: Set<string>
      VisibleRange: VisibleRange
      RecalculationThresholdMs: int }

/// Public contract type exposed by this FS.Skia.UI package.
type CollectionMsg =
    | ScrollTo of float
    | SelectKey of string
    | ToggleKey of string
    | ReplaceItemCount of int

/// Public contract type exposed by this FS.Skia.UI package.
type CollectionEffect =
    | VisibleRangeChanged of VisibleRange

/// Public contract module exposed by this FS.Skia.UI package.
module Collections =
    /// Public contract function exposed by this FS.Skia.UI package.
    val visibleRange: rowHeight: float -> viewportHeight: float -> scrollOffset: float -> totalItems: int -> VisibleRange
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: controlId: ControlId -> itemCount: int -> rowHeight: float -> viewportHeight: float -> CollectionModel * CollectionEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list
