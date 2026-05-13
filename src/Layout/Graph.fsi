namespace FS.Skia.UI.Layout

open FS.Skia.UI

type GraphTarget =
    | Node of nodeId: string
    | Edge of edgeIndex: int

module Graph =
    val layout : graph: GraphDefinition -> Result<GraphLayoutResult, GraphValidationIssue list>
    val directed : graph: GraphDefinition -> Result<Scene, GraphValidationIssue list>
    val undirected : graph: GraphDefinition -> Result<Scene, GraphValidationIssue list>
    val hitTest : layout: GraphLayoutResult -> x: float -> y: float -> GraphTarget option
