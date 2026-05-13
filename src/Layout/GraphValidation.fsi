namespace FS.Skia.UI.Layout

type GraphValidationIssue =
    | DuplicateNodeId of string
    | MissingSource of edgeIndex: int * nodeId: string
    | MissingTarget of edgeIndex: int * nodeId: string
    | SelfLoop of edgeIndex: int * nodeId: string
    | CycleDetected of nodeIds: string list

module GraphValidation =
    val validate : graph: GraphDefinition -> GraphValidationIssue list
    val hasCycle : graph: GraphDefinition -> bool
    val disconnectedComponents : graph: GraphDefinition -> string list list
