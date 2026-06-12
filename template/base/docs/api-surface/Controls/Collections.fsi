namespace FS.Skia.UI.Controls

/// The slice of a virtualized list currently realized: `FirstIndex`/`Count` within `Total`.
type VisibleRange =
    { FirstIndex: int
      Count: int
      Total: int }

/// State of a virtualizing collection: scroll offset, viewport/row geometry, `SelectedKeys`,
/// and the derived `VisibleRange` keyed by `ControlId`.
type CollectionModel =
    { ControlId: ControlId
      ItemCount: int
      RowHeight: float
      ViewportHeight: float
      ScrollOffset: float
      SelectedKeys: Set<string>
      VisibleRange: VisibleRange
      RecalculationThresholdMs: int }

/// Messages that drive a `CollectionModel`: `ScrollTo`, `SelectKey`/`ToggleKey`, `ReplaceItemCount`.
type CollectionMsg =
    | ScrollTo of float
    | SelectKey of string
    | ToggleKey of string
    | ReplaceItemCount of int

/// Side effect emitted when a collection update shifts the realized window (`VisibleRangeChanged`).
type CollectionEffect =
    | VisibleRangeChanged of VisibleRange

/// Virtualization model for large scrolling lists: `visibleRange`/`init`/`update` over `CollectionModel`.
module Collections =
    /// Compute the realized `VisibleRange` from row height, viewport height, scroll offset, and item total.
    val visibleRange: rowHeight: float -> viewportHeight: float -> scrollOffset: float -> totalItems: int -> VisibleRange
    /// Build the initial `CollectionModel` for a `controlId` and emit its first `CollectionEffect` list.
    val init: controlId: ControlId -> itemCount: int -> rowHeight: float -> viewportHeight: float -> CollectionModel * CollectionEffect list
    /// Apply a `CollectionMsg` to the `CollectionModel`, returning the next model and any effects.
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list
