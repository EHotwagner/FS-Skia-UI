namespace FS.Skia.UI.Layout

open FS.Skia.UI

module Layout =
    val horizontalStack : config: StackConfig -> children: LayoutChild list -> Scene
    val verticalStack : config: StackConfig -> children: LayoutChild list -> Scene
    val dock : config: DockConfig -> children: LayoutChild list -> Scene
    val measureHorizontal : config: StackConfig -> children: LayoutChild list -> LayoutBounds list
    val measureVertical : config: StackConfig -> children: LayoutChild list -> LayoutBounds list
