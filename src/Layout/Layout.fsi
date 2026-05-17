namespace FS.Skia.UI.Layout

open FS.Skia.UI.Scene

module Layout =
    val evaluate : available: AvailableSpace -> root: LayoutNode -> LayoutResult
    val evaluateIncremental :
        previous: LayoutResult ->
        changedNodeIds: LayoutNodeId list ->
        available: AvailableSpace ->
        root: LayoutNode ->
            LayoutResult

    val renderComputed : result: LayoutResult -> root: LayoutNode -> Scene
    val snapBounds : policy: PixelSnapPolicy -> bounds: LayoutBounds -> LayoutBounds
    val hitTestComputed : policy: PixelSnapPolicy -> result: LayoutResult -> x: float -> y: float -> LayoutNodeId option
    val initWorkflow : available: AvailableSpace -> root: LayoutNode -> LayoutWorkflowModel * LayoutWorkflowEffect list
    val updateWorkflow : msg: LayoutWorkflowMsg -> model: LayoutWorkflowModel -> LayoutWorkflowModel * LayoutWorkflowEffect list
    val interpretWorkflowEffect : effect: LayoutWorkflowEffect -> model: LayoutWorkflowModel -> LayoutWorkflowMsg
    val horizontalStack : config: StackConfig -> children: LayoutChild list -> Scene
    val verticalStack : config: StackConfig -> children: LayoutChild list -> Scene
    val dock : config: DockConfig -> children: LayoutChild list -> Scene
    val measureHorizontal : config: StackConfig -> children: LayoutChild list -> LayoutBounds list
    val measureVertical : config: StackConfig -> children: LayoutChild list -> LayoutBounds list
