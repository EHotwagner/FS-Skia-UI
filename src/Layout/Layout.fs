namespace FS.Skia.UI.Layout

open FS.Skia.UI

module Layout =
    let content children =
        children |> List.map _.Content |> Scene.group

    let innerBounds (bounds: LayoutBounds) (padding: LayoutPadding) =
        { X = bounds.X + padding.Left
          Y = bounds.Y + padding.Top
          Width = max 0.0 (bounds.Width - padding.Left - padding.Right)
          Height = max 0.0 (bounds.Height - padding.Top - padding.Bottom) }

    let measureHorizontal (config: StackConfig) (children: LayoutChild list) =
        let inner = innerBounds config.Bounds config.Padding
        let count = max 1 children.Length
        let totalSpacing = config.Spacing * float (max 0 (children.Length - 1))
        let width = max 0.0 ((inner.Width - totalSpacing) / float count)

        children
        |> List.mapi (fun index _ ->
            { LayoutBounds.X = inner.X + float index * (width + config.Spacing)
              Y = inner.Y
              Width = width
              Height = inner.Height })

    let measureVertical (config: StackConfig) (children: LayoutChild list) =
        let inner = innerBounds config.Bounds config.Padding
        let count = max 1 children.Length
        let totalSpacing = config.Spacing * float (max 0 (children.Length - 1))
        let height = max 0.0 ((inner.Height - totalSpacing) / float count)

        children
        |> List.mapi (fun index _ ->
            { LayoutBounds.X = inner.X
              Y = inner.Y + float index * (height + config.Spacing)
              Width = inner.Width
              Height = height })

    let horizontalStack (_: StackConfig) (children: LayoutChild list) = content children
    let verticalStack (_: StackConfig) (children: LayoutChild list) = content children
    let dock (_: DockConfig) (children: LayoutChild list) = content children
