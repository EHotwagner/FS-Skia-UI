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
    let visibleRange rowHeight viewportHeight scrollOffset totalItems =
        if totalItems <= 0 || rowHeight <= 0.0 || viewportHeight <= 0.0 then
            { FirstIndex = 0; Count = 0; Total = max 0 totalItems }
        else
            let first = int (max 0.0 scrollOffset / rowHeight) |> min (totalItems - 1)
            let visible = int (ceil (viewportHeight / rowHeight)) + 1
            { FirstIndex = first; Count = min visible (totalItems - first); Total = totalItems }

    let init controlId itemCount rowHeight viewportHeight =
        let range = visibleRange rowHeight viewportHeight 0.0 itemCount

        { ControlId = controlId
          ItemCount = itemCount
          RowHeight = rowHeight
          ViewportHeight = viewportHeight
          ScrollOffset = 0.0
          SelectedKeys = Set.empty
          VisibleRange = range
          RecalculationThresholdMs = 16 },
        [ VisibleRangeChanged range ]

    let withRange model scrollOffset itemCount =
        let range = visibleRange model.RowHeight model.ViewportHeight scrollOffset itemCount
        { model with ScrollOffset = max 0.0 scrollOffset; ItemCount = itemCount; VisibleRange = range }, [ VisibleRangeChanged range ]

    let update msg model =
        match msg with
        | ScrollTo offset -> withRange model offset model.ItemCount
        | SelectKey key -> { model with SelectedKeys = Set.singleton key }, []
        | ToggleKey key ->
            let selected =
                if model.SelectedKeys.Contains key then
                    model.SelectedKeys.Remove key
                else
                    model.SelectedKeys.Add key

            { model with SelectedKeys = selected }, []
        | ReplaceItemCount count -> withRange model model.ScrollOffset (max 0 count)
