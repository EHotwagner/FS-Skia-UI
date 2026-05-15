namespace FS.Skia.UI.Layout

open System
open Facebook.Yoga
open FS.Skia.UI

module Layout =
    let finite value = Double.IsFinite value

    let nonNegative value = finite value && value >= 0.0

    let clampNonNegative value =
        if nonNegative value then value else 0.0

    let diagnostic nodeId code severity message constraintName fallbackApplied =
        { NodeId = nodeId
          Code = code
          Severity = severity
          Message = message
          Constraint = constraintName
          FallbackApplied = fallbackApplied }

    let normalizeDimension nodeId name value =
        match value with
        | Some value when nonNegative value -> value, []
        | Some value ->
            0.0,
            [ diagnostic
                  nodeId
                  InvalidLayoutValue
                  FS.Skia.UI.Layout.DiagnosticSeverity.Warning
                  $"Invalid {name} value '{value}' was normalized to 0."
                  (Some name)
                  true ]
        | None -> 0.0, []

    let normalizeOptionalDimension nodeId name value =
        match value with
        | Some value when nonNegative value -> Some value, []
        | Some value ->
            Some 0.0,
            [ diagnostic
                  nodeId
                  InvalidLayoutValue
                  FS.Skia.UI.Layout.DiagnosticSeverity.Warning
                  $"Invalid {name} value '{value}' was normalized to 0."
                  (Some name)
                  true ]
        | None -> None, []

    let normalizePadding nodeId (padding: LayoutPadding) =
        let left, leftDiagnostics = normalizeDimension nodeId "padding-left" (Some padding.Left)
        let top, topDiagnostics = normalizeDimension nodeId "padding-top" (Some padding.Top)
        let right, rightDiagnostics = normalizeDimension nodeId "padding-right" (Some padding.Right)
        let bottom, bottomDiagnostics = normalizeDimension nodeId "padding-bottom" (Some padding.Bottom)

        { Left = left
          Top = top
          Right = right
          Bottom = bottom },
        leftDiagnostics @ topDiagnostics @ rightDiagnostics @ bottomDiagnostics

    let normalizeGap nodeId (gap: LayoutGap) =
        let row, rowDiagnostics = normalizeDimension nodeId "row-gap" (Some gap.Row)
        let column, columnDiagnostics = normalizeDimension nodeId "column-gap" (Some gap.Column)
        { Row = row; Column = column }, rowDiagnostics @ columnDiagnostics

    let normalizeAvailable (available: AvailableSpace) =
        let width =
            if nonNegative available.Width then
                available.Width
            else
                0.0

        let height =
            if nonNegative available.Height then
                available.Height
            else
                0.0

        let diagnostics =
            [ if not (nonNegative available.Width) then
                  diagnostic None InvalidAvailableSpace FS.Skia.UI.Layout.DiagnosticSeverity.Error "Invalid available width was normalized to 0." (Some "available-width") true
              if not (nonNegative available.Height) then
                  diagnostic None InvalidAvailableSpace FS.Skia.UI.Layout.DiagnosticSeverity.Error "Invalid available height was normalized to 0." (Some "available-height") true ]

        { available with Width = width; Height = height }, diagnostics

    let validateTree (root: LayoutNode) =
        let rec collect path (node: LayoutNode) =
            let own =
                if String.IsNullOrWhiteSpace node.Id then
                    [ diagnostic None InvalidLayoutValue FS.Skia.UI.Layout.DiagnosticSeverity.Error $"Layout node at {path} has an empty id." (Some "node-id") true ]
                else
                    []

            own @ (node.Children |> List.mapi (fun index child -> collect $"{path}/{index}" child) |> List.concat)

        let ids =
            let rec loop (node: LayoutNode) =
                node.Id :: (node.Children |> List.collect loop)

            loop root

        let duplicateDiagnostics =
            ids
            |> List.filter (String.IsNullOrWhiteSpace >> not)
            |> List.countBy id
            |> List.choose (fun (nodeId, count) ->
                if count > 1 then
                    Some(diagnostic (Some nodeId) DuplicateLayoutNodeId FS.Skia.UI.Layout.DiagnosticSeverity.Error $"Duplicate layout node id '{nodeId}' appears {count} times." (Some "node-id") true)
                else
                    None)

        collect "root" root @ duplicateDiagnostics

    let constrain nodeId requested minSize maxSize axis =
        let minValue, minDiagnostics = normalizeOptionalDimension nodeId $"min-{axis}" minSize
        let maxValue, maxDiagnostics = normalizeOptionalDimension nodeId $"max-{axis}" maxSize

        let conflictDiagnostics =
            match minValue, maxValue with
            | Some minValue, Some maxValue when minValue > maxValue ->
                [ diagnostic nodeId UnsatisfiedConstraint FS.Skia.UI.Layout.DiagnosticSeverity.Warning $"Minimum {axis} exceeds maximum {axis}; maximum was used." (Some axis) true ]
            | _ -> []

        let bounded =
            let afterMin =
                match minValue with
                | Some value -> max value requested
                | None -> requested

            match maxValue with
            | Some value -> min value afterMin
            | None -> afterMin

        max 0.0 bounded, minDiagnostics @ maxDiagnostics @ conflictDiagnostics

    let measureLeaf nodeId availableWidth availableHeight (measure: ContentMeasure option) =
        match measure with
        | None -> 0.0, 0.0, []
        | Some measure ->
            let response =
                measure
                    { AvailableWidth = max 0.0 availableWidth
                      WidthMode = FS.Skia.UI.Layout.MeasureMode.AtMost
                      AvailableHeight = max 0.0 availableHeight
                      HeightMode = FS.Skia.UI.Layout.MeasureMode.AtMost }

            let diagnostics = response.Diagnostics

            if nonNegative response.Width && nonNegative response.Height then
                response.Width, response.Height, diagnostics
            else
                0.0,
                0.0,
                diagnostics
                @ [ diagnostic nodeId UnmeasurableContent FS.Skia.UI.Layout.DiagnosticSeverity.Warning "Invalid measurement output was normalized to 0x0." (Some "measure") true ]

    let preferredMainSize isRow availableMain (node: LayoutNode) =
        let explicit =
            if isRow then
                node.Intent.Size.Width
            else
                node.Intent.Size.Height

        match node.Visibility, explicit, node.Intent.FlexBasis with
        | Collapsed, _, _ -> 0.0
        | _, Some value, _ when nonNegative value -> value
        | _, _, Some value when nonNegative value -> value
        | _ ->
            let measuredWidth, measuredHeight, _ = measureLeaf (Some node.Id) availableMain availableMain node.Measure
            if isRow then measuredWidth else measuredHeight

    let alignOffset align available childSize =
        match align with
        | LayoutAlign.Center -> max 0.0 ((available - childSize) / 2.0)
        | LayoutAlign.End -> max 0.0 (available - childSize)
        | _ -> 0.0

    let rec layoutNode (bounds: LayoutBounds) (node: LayoutNode) =
        let padding, paddingDiagnostics = normalizePadding (Some node.Id) node.Intent.Padding
        let gap, gapDiagnostics = normalizeGap (Some node.Id) node.Intent.Gap
        let margin, marginDiagnostics = normalizePadding (Some node.Id) node.Intent.Margin

        let widthFromIntent, widthDiagnostics =
            match node.Intent.Size.Width with
            | Some value -> normalizeDimension (Some node.Id) "width" (Some value)
            | None -> bounds.Width, []

        let heightFromIntent, heightDiagnostics =
            match node.Intent.Size.Height with
            | Some value -> normalizeDimension (Some node.Id) "height" (Some value)
            | None -> bounds.Height, []

        let width, minMaxWidthDiagnostics = constrain (Some node.Id) widthFromIntent node.Intent.MinSize.Width node.Intent.MaxSize.Width "width"
        let height, minMaxHeightDiagnostics = constrain (Some node.Id) heightFromIntent node.Intent.MinSize.Height node.Intent.MaxSize.Height "height"

        let ownBounds: LayoutBounds =
            match node.Visibility with
            | Collapsed ->
                { X = bounds.X + margin.Left
                  Y = bounds.Y + margin.Top
                  Width = 0.0
                  Height = 0.0 }
            | _ ->
                { X = bounds.X + margin.Left
                  Y = bounds.Y + margin.Top
                  Width = max 0.0 (width - margin.Left - margin.Right)
                  Height = max 0.0 (height - margin.Top - margin.Bottom) }

        let own: ComputedBounds = { NodeId = node.Id; Bounds = ownBounds; Visibility = node.Visibility }
        let diagnostics = paddingDiagnostics @ gapDiagnostics @ marginDiagnostics @ widthDiagnostics @ heightDiagnostics @ minMaxWidthDiagnostics @ minMaxHeightDiagnostics

        if node.Visibility = Collapsed || List.isEmpty node.Children then
            [ own ], diagnostics
        else
            let inner: LayoutBounds =
                { X = ownBounds.X + padding.Left
                  Y = ownBounds.Y + padding.Top
                  Width = max 0.0 (ownBounds.Width - padding.Left - padding.Right)
                  Height = max 0.0 (ownBounds.Height - padding.Top - padding.Bottom) }

            let children = node.Children
            let isRow = node.Intent.Direction = LayoutDirection.Row
            let mainAvailable = if isRow then inner.Width else inner.Height
            let crossAvailable = if isRow then inner.Height else inner.Width
            let mainGap = if isRow then gap.Column else gap.Row
            let crossGap = if isRow then gap.Row else gap.Column

            let childDescriptors =
                children
                |> List.map (fun child ->
                    let basis = preferredMainSize isRow mainAvailable child
                    let grow = if nonNegative child.Intent.FlexGrow then child.Intent.FlexGrow else 0.0
                    let shrink = if nonNegative child.Intent.FlexShrink then child.Intent.FlexShrink else 1.0
                    child, basis, grow, shrink)

            let totalBasis = childDescriptors |> List.sumBy (fun (_, basis, _, _) -> basis)
            let totalGap = mainGap * float (max 0 (children.Length - 1))
            let remaining = mainAvailable - totalBasis - totalGap
            let totalGrow = childDescriptors |> List.sumBy (fun (_, _, grow, _) -> grow)
            let totalShrink = childDescriptors |> List.sumBy (fun (_, _, _, shrink) -> shrink)

            let mainSizes =
                childDescriptors
                |> List.map (fun (child, basis, grow, shrink) ->
                    let adjusted =
                        if node.Intent.Wrap = LayoutWrap.Wrap then
                            basis
                        elif remaining > 0.0 && totalGrow > 0.0 then
                            basis + remaining * grow / totalGrow
                        elif remaining < 0.0 && totalShrink > 0.0 then
                            basis + remaining * shrink / totalShrink
                        elif remaining > 0.0 && basis = 0.0 && totalGrow = 0.0 then
                            basis
                        elif basis = 0.0 && children.Length > 0 then
                            max 0.0 ((mainAvailable - totalGap) / float children.Length)
                        else
                            basis

                    let axis = if isRow then "width" else "height"
                    let minValue = if isRow then child.Intent.MinSize.Width else child.Intent.MinSize.Height
                    let maxValue = if isRow then child.Intent.MaxSize.Width else child.Intent.MaxSize.Height
                    let constrained, constrainedDiagnostics = constrain (Some child.Id) adjusted minValue maxValue axis
                    child, constrained, constrainedDiagnostics)

            let wrapLines =
                if node.Intent.Wrap = LayoutWrap.Wrap then
                    (([], [], 0.0), mainSizes)
                    ||> List.fold (fun (lines, current, used) (child, size, childDiagnostics) ->
                        let nextUsed = if List.isEmpty current then size else used + mainGap + size
                        if not (List.isEmpty current) && nextUsed > mainAvailable then
                            ((List.rev current) :: lines, [ child, size, childDiagnostics ], size)
                        else
                            (lines, (child, size, childDiagnostics) :: current, nextUsed))
                    |> fun (lines, current, _) -> List.rev ((List.rev current) :: lines |> List.filter (List.isEmpty >> not))
                else
                    [ mainSizes ]

            let childResults, childDiagnostics, _ =
                (([], diagnostics, 0.0), wrapLines)
                ||> List.fold (fun (allBounds, allDiagnostics, crossOffset) line ->
                    let lineMain =
                        line |> List.sumBy (fun (_, size, _) -> size)

                    let lineCross =
                        if List.isEmpty line then 0.0 else max 0.0 ((crossAvailable - crossGap * float (wrapLines.Length - 1)) / float wrapLines.Length)

                    let startMain =
                        match node.Intent.JustifyContent with
                        | LayoutAlign.Center -> max 0.0 ((mainAvailable - lineMain - mainGap * float (max 0 (line.Length - 1))) / 2.0)
                        | LayoutAlign.End -> max 0.0 (mainAvailable - lineMain - mainGap * float (max 0 (line.Length - 1)))
                        | _ -> 0.0

                    let _, lineBounds, lineDiagnostics =
                        ((startMain, [], allDiagnostics), line)
                        ||> List.fold (fun (mainOffset, boundsAcc, diagnosticsAcc) (child, mainSize, childSizeDiagnostics) ->
                            let measuredWidth, measuredHeight, measureDiagnostics = measureLeaf (Some child.Id) mainSize lineCross child.Measure
                            let explicitCross = if isRow then child.Intent.Size.Height else child.Intent.Size.Width
                            let measuredCross = if isRow then measuredHeight else measuredWidth
                            let crossSize =
                                match child.Visibility, (child.Intent.AlignSelf |> Option.defaultValue node.Intent.AlignItems), explicitCross with
                                | Collapsed, _, _ -> 0.0
                                | _, LayoutAlign.Stretch, None -> lineCross
                                | _, _, Some value when nonNegative value -> min lineCross value
                                | _ -> if measuredCross > 0.0 then min lineCross measuredCross else lineCross

                            let crossAlign = child.Intent.AlignSelf |> Option.defaultValue node.Intent.AlignItems
                            let crossPosition = crossOffset + alignOffset crossAlign lineCross crossSize
                            let childBounds: LayoutBounds =
                                if isRow then
                                    { X = inner.X + mainOffset
                                      Y = inner.Y + crossPosition
                                      Width = mainSize
                                      Height = crossSize }
                                else
                                    { X = inner.X + crossPosition
                                      Y = inner.Y + mainOffset
                                      Width = crossSize
                                      Height = mainSize }

                            let childComputed, childLayoutDiagnostics = layoutNode childBounds child
                            mainOffset + mainSize + mainGap, boundsAcc @ childComputed, diagnosticsAcc @ childSizeDiagnostics @ measureDiagnostics @ childLayoutDiagnostics)

                    allBounds @ lineBounds, lineDiagnostics, crossOffset + lineCross + crossGap)

            own :: childResults, childDiagnostics

    let yogaAlign align =
        match align with
        | LayoutAlign.Auto -> YGAlign.Auto
        | LayoutAlign.Start -> YGAlign.FlexStart
        | LayoutAlign.Center -> YGAlign.Center
        | LayoutAlign.End -> YGAlign.FlexEnd
        | LayoutAlign.Stretch -> YGAlign.Stretch
        | LayoutAlign.SpaceBetween -> YGAlign.SpaceBetween
        | LayoutAlign.SpaceAround -> YGAlign.SpaceAround
        | LayoutAlign.SpaceEvenly -> YGAlign.SpaceEvenly

    let yogaJustify align =
        match align with
        | LayoutAlign.Center -> YGJustify.Center
        | LayoutAlign.End -> YGJustify.FlexEnd
        | LayoutAlign.SpaceBetween -> YGJustify.SpaceBetween
        | LayoutAlign.SpaceAround -> YGJustify.SpaceAround
        | LayoutAlign.SpaceEvenly -> YGJustify.SpaceEvenly
        | _ -> YGJustify.FlexStart

    let yogaMeasureMode mode =
        match mode with
        | Facebook.Yoga.MeasureMode.Exactly -> FS.Skia.UI.Layout.MeasureMode.Exactly
        | Facebook.Yoga.MeasureMode.AtMost -> FS.Skia.UI.Layout.MeasureMode.AtMost
        | _ -> FS.Skia.UI.Layout.MeasureMode.Undefined

    let setOptional value apply =
        match value with
        | Some value when nonNegative value -> apply (single value)
        | _ -> ()

    let applyYogaStyle (yogaNode: Node) (node: LayoutNode) =
        YGNodeStyleAPI.YGNodeStyleSetFlexDirection(
            yogaNode,
            if node.Intent.Direction = LayoutDirection.Row then
                YGFlexDirection.Row
            else
                YGFlexDirection.Column
        )

        YGNodeStyleAPI.YGNodeStyleSetFlexWrap(
            yogaNode,
            if node.Intent.Wrap = LayoutWrap.Wrap then
                YGWrap.Wrap
            else
                YGWrap.NoWrap
        )

        YGNodeStyleAPI.YGNodeStyleSetAlignItems(yogaNode, yogaAlign node.Intent.AlignItems)
        node.Intent.AlignSelf |> Option.iter (fun align -> YGNodeStyleAPI.YGNodeStyleSetAlignSelf(yogaNode, yogaAlign align))
        YGNodeStyleAPI.YGNodeStyleSetJustifyContent(yogaNode, yogaJustify node.Intent.JustifyContent)
        YGNodeStyleAPI.YGNodeStyleSetDisplay(yogaNode, if node.Visibility = Collapsed then YGDisplay.None else YGDisplay.Flex)
        YGNodeStyleAPI.YGNodeStyleSetPadding(yogaNode, YGEdge.Left, single (clampNonNegative node.Intent.Padding.Left))
        YGNodeStyleAPI.YGNodeStyleSetPadding(yogaNode, YGEdge.Top, single (clampNonNegative node.Intent.Padding.Top))
        YGNodeStyleAPI.YGNodeStyleSetPadding(yogaNode, YGEdge.Right, single (clampNonNegative node.Intent.Padding.Right))
        YGNodeStyleAPI.YGNodeStyleSetPadding(yogaNode, YGEdge.Bottom, single (clampNonNegative node.Intent.Padding.Bottom))
        YGNodeStyleAPI.YGNodeStyleSetMargin(yogaNode, YGEdge.Left, single (clampNonNegative node.Intent.Margin.Left))
        YGNodeStyleAPI.YGNodeStyleSetMargin(yogaNode, YGEdge.Top, single (clampNonNegative node.Intent.Margin.Top))
        YGNodeStyleAPI.YGNodeStyleSetMargin(yogaNode, YGEdge.Right, single (clampNonNegative node.Intent.Margin.Right))
        YGNodeStyleAPI.YGNodeStyleSetMargin(yogaNode, YGEdge.Bottom, single (clampNonNegative node.Intent.Margin.Bottom))
        YGNodeStyleAPI.YGNodeStyleSetGap(yogaNode, YGGutter.Row, single (clampNonNegative node.Intent.Gap.Row))
        YGNodeStyleAPI.YGNodeStyleSetGap(yogaNode, YGGutter.Column, single (clampNonNegative node.Intent.Gap.Column))
        setOptional node.Intent.Size.Width (fun value -> YGNodeStyleAPI.YGNodeStyleSetWidth(yogaNode, value))
        setOptional node.Intent.Size.Height (fun value -> YGNodeStyleAPI.YGNodeStyleSetHeight(yogaNode, value))
        setOptional node.Intent.MinSize.Width (fun value -> YGNodeStyleAPI.YGNodeStyleSetMinWidth(yogaNode, value))
        setOptional node.Intent.MinSize.Height (fun value -> YGNodeStyleAPI.YGNodeStyleSetMinHeight(yogaNode, value))
        setOptional node.Intent.MaxSize.Width (fun value -> YGNodeStyleAPI.YGNodeStyleSetMaxWidth(yogaNode, value))
        setOptional node.Intent.MaxSize.Height (fun value -> YGNodeStyleAPI.YGNodeStyleSetMaxHeight(yogaNode, value))
        YGNodeStyleAPI.YGNodeStyleSetFlexGrow(yogaNode, single (clampNonNegative node.Intent.FlexGrow))
        YGNodeStyleAPI.YGNodeStyleSetFlexShrink(yogaNode, single (clampNonNegative node.Intent.FlexShrink))
        node.Intent.FlexBasis |> Option.iter (fun basis -> if nonNegative basis then YGNodeStyleAPI.YGNodeStyleSetFlexBasis(yogaNode, single basis))

    let yogaFailureInjectionEnabled () =
        let mutable enabled = false
        AppContext.TryGetSwitch("FS.Skia.UI.Layout.ForceYogaFailure", &enabled) && enabled

    let tryYogaLayout (available: AvailableSpace) (root: LayoutNode) =
        let measurementDiagnostics = ResizeArray<LayoutDiagnostic>()
        let nodePairs = ResizeArray<LayoutNode * Node>()

        let rec createNode (node: LayoutNode) =
            let yogaNode = YGNodeAPI.YGNodeNew()
            nodePairs.Add(node, yogaNode)
            applyYogaStyle yogaNode node

            match node.Measure, node.Children with
            | Some measure, [] ->
                let callback =
                    YGMeasureFunc(fun _ width widthMode height heightMode ->
                        let response =
                            measure
                                { AvailableWidth = float width
                                  WidthMode = yogaMeasureMode widthMode
                                  AvailableHeight = float height
                                  HeightMode = yogaMeasureMode heightMode }

                        measurementDiagnostics.AddRange(response.Diagnostics)

                        if nonNegative response.Width && nonNegative response.Height then
                            YGSize(Width = single response.Width, Height = single response.Height)
                        else
                            measurementDiagnostics.Add(
                                diagnostic
                                    (Some node.Id)
                                    UnmeasurableContent
                                    FS.Skia.UI.Layout.DiagnosticSeverity.Warning
                                    "Invalid measurement output was normalized to 0x0."
                                    (Some "measure")
                                    true
                            )

                            YGSize(Width = 0.0f, Height = 0.0f))

                YGNodeAPI.YGNodeSetMeasureFunc(yogaNode, callback)
            | _ -> ()

            node.Children
            |> List.iteri (fun index child ->
                let childNode = createNode child
                YGNodeAPI.YGNodeInsertChild(yogaNode, childNode, unativeint index))

            yogaNode

        let mutable rootYoga = Unchecked.defaultof<Node>
        let mutable rootCreated = false

        try
            // Test-only diagnostic switch used to exercise the recoverable Yoga fallback path without changing the public API.
            if yogaFailureInjectionEnabled () then
                invalidOp "Forced Yoga execution failure."

            rootYoga <- createNode root
            rootCreated <- true
            YGNodeStyleAPI.YGNodeStyleSetWidth(rootYoga, single available.Width)
            YGNodeStyleAPI.YGNodeStyleSetHeight(rootYoga, single available.Height)
            YGNodeAPI.YGNodeCalculateLayout(rootYoga, single available.Width, single available.Height, YGDirection.LTR)

            let rec read absoluteX absoluteY (node: LayoutNode) (yogaNode: Node) =
                let x = absoluteX + float (YGNodeLayoutAPI.YGNodeLayoutGetLeft yogaNode)
                let y = absoluteY + float (YGNodeLayoutAPI.YGNodeLayoutGetTop yogaNode)
                let own =
                    { NodeId = node.Id
                      Bounds =
                        { X = x
                          Y = y
                          Width = max 0.0 (float (YGNodeLayoutAPI.YGNodeLayoutGetWidth yogaNode))
                          Height = max 0.0 (float (YGNodeLayoutAPI.YGNodeLayoutGetHeight yogaNode)) }
                      Visibility = node.Visibility }

                let children =
                    node.Children
                    |> List.mapi (fun index child ->
                        let childYoga: Node = YGNodeAPI.YGNodeGetChild(yogaNode, unativeint index) |> Unchecked.nonNull
                        read x y child childYoga)
                    |> List.concat

                own :: children

            let bounds = read 0.0 0.0 root rootYoga
            YGNodeAPI.YGNodeFreeRecursive(rootYoga)
            rootCreated <- false
            Ok(bounds, List.ofSeq measurementDiagnostics)
        with ex ->
            if rootCreated then
                try
                    YGNodeAPI.YGNodeFreeRecursive(rootYoga)
                with _ ->
                    ()

            Result.Error ex

    let evaluate available root =
        let available, availableDiagnostics = normalizeAvailable available
        let rootBounds: LayoutBounds =
            { X = 0.0
              Y = 0.0
              Width = available.Width
              Height = available.Height }

        let _, pureValidationDiagnostics = layoutNode rootBounds root

        let bounds, diagnostics =
            match tryYogaLayout available root with
            | Ok(bounds, yogaDiagnostics) -> bounds, yogaDiagnostics @ pureValidationDiagnostics
            | Result.Error ex ->
                let bounds, pureDiagnostics = layoutNode rootBounds root
                let fallbackDiagnostic =
                    diagnostic
                        (Some root.Id)
                        FallbackBoundsApplied
                        FS.Skia.UI.Layout.DiagnosticSeverity.Warning
                        $"Yoga execution failed recoverably; pure fallback layout was applied. {ex.GetType().Name}: {ex.Message}"
                        (Some "yoga")
                        true

                bounds, fallbackDiagnostic :: pureDiagnostics
        let allDiagnostics = availableDiagnostics @ validateTree root @ diagnostics

        let fallbackDiagnostics =
            if allDiagnostics |> List.exists (fun item -> item.FallbackApplied) then
                [ diagnostic None FallbackBoundsApplied FS.Skia.UI.Layout.DiagnosticSeverity.Info "One or more layout inputs required bounded fallback geometry." None true ]
            else
                []

        { Bounds = bounds
          Diagnostics = allDiagnostics @ fallbackDiagnostics
          Invalidated = [ root.Id ]
          Revision = 1L }

    let evaluateIncremental previous changedNodeIds available root =
        let result = evaluate available root
        { result with
            Revision = previous.Revision + 1L
            Invalidated = changedNodeIds |> List.distinct }

    let rec contentById (node: LayoutNode) =
        seq {
            yield node.Id, node.Content
            for child in node.Children do
                yield! contentById child
        }

    let renderComputed (result: LayoutResult) (root: LayoutNode) =
        let content = contentById root |> Map.ofSeq

        result.Bounds
        |> List.choose (fun item ->
            match item.Visibility, Map.tryFind item.NodeId content with
            | Visible, Some(Some scene) -> Some scene
            | _ -> None)
        |> Scene.group

    let snapValue mode (scale: float) (value: float) =
        let scaled = value * scale

        let snapped =
            match mode with
            | SnapMode.Floor -> Math.Floor scaled
            | SnapMode.Round -> Math.Round(scaled, MidpointRounding.AwayFromZero)
            | SnapMode.Expand -> Math.Floor scaled

        snapped / scale

    let snapEnd mode (scale: float) (value: float) =
        let scaled = value * scale

        let snapped =
            match mode with
            | SnapMode.Expand -> Math.Ceiling scaled
            | SnapMode.Floor -> Math.Floor scaled
            | SnapMode.Round -> Math.Round(scaled, MidpointRounding.AwayFromZero)

        snapped / scale

    let snapBounds (policy: PixelSnapPolicy) (bounds: LayoutBounds) =
        let scale =
            if finite policy.ScaleFactor && policy.ScaleFactor > 0.0 then
                policy.ScaleFactor
            else
                1.0

        let x = snapValue policy.Mode scale bounds.X
        let y = snapValue policy.Mode scale bounds.Y
        let right = snapEnd policy.Mode scale (bounds.X + bounds.Width)
        let bottom = snapEnd policy.Mode scale (bounds.Y + bounds.Height)

        ({ X = x
           Y = y
           Width = max 0.0 (right - x)
           Height = max 0.0 (bottom - y) }
        : LayoutBounds)

    let hitTestComputed (policy: PixelSnapPolicy) (result: LayoutResult) (x: float) (y: float) =
        result.Bounds
        |> List.rev
        |> List.tryPick (fun item ->
            if item.Visibility = Visible then
                let bounds = snapBounds policy item.Bounds

                if x >= bounds.X && x <= bounds.X + bounds.Width && y >= bounds.Y && y <= bounds.Y + bounds.Height then
                    Some item.NodeId
                else
                    None
            else
                None)

    let initWorkflow available root =
        { Root = root
          Available = available
          Result = None
          LastChangedNodeIds = [ root.Id ]
          PixelSnapPolicy = Defaults.pixelSnapPolicy 1.0 },
        [ EvaluateLayout ]

    let rec updateNode nodeId apply (node: LayoutNode) =
        let updated =
            if node.Id = nodeId then
                apply node
            else
                node

        { updated with Children = updated.Children |> List.map (updateNode nodeId apply) }

    let updateWorkflow msg model =
        match msg with
        | LayoutHostResized available ->
            { model with
                Available = available
                LastChangedNodeIds = [ model.Root.Id ] },
            [ EvaluateIncrementalLayout [ model.Root.Id ] ]
        | LayoutVisibilityChanged(nodeId, visibility) ->
            { model with
                Root = updateNode nodeId (fun node -> { node with Visibility = visibility }) model.Root
                LastChangedNodeIds = [ nodeId ] },
            [ EvaluateIncrementalLayout [ nodeId ] ]
        | LayoutIntentChanged(nodeId, intent) ->
            { model with
                Root = updateNode nodeId (fun node -> { node with Intent = intent }) model.Root
                LastChangedNodeIds = [ nodeId ] },
            [ EvaluateIncrementalLayout [ nodeId ] ]
        | LayoutMeasurementChanged nodeId ->
            { model with LastChangedNodeIds = [ nodeId ] },
            [ EvaluateIncrementalLayout [ nodeId ] ]
        | LayoutEvaluationCompleted result ->
            { model with
                Result = Some result
                LastChangedNodeIds = result.Invalidated },
            []

    let interpretWorkflowEffect effect model =
        let result =
            match effect, model.Result with
            | EvaluateLayout, _
            | EvaluateIncrementalLayout _, None -> evaluate model.Available model.Root
            | EvaluateIncrementalLayout changedNodeIds, Some previous -> evaluateIncremental previous changedNodeIds model.Available model.Root

        LayoutEvaluationCompleted result

    let content (children: LayoutChild list) =
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
