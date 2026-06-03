// Graph.fsi — fsharp-graph-algorithms family.
//
// Generic DAG core extracted for the SkillSupport shipped library: Kahn
// topological sort with a deterministic ascending-id tie-break, and 3-colour DFS
// cycle detection. Operates on a plain node-list + edge-list so it is reusable by
// any consumer (the governance synthetic-propagation rule is one such consumer,
// kept in build/Governance). Pure. Visibility lives here (Principle II).
namespace FS.Skia.UI.SkillSupport

module Graph =

    /// A graph node identifier.
    type NodeId = string

    /// A directed edge `from -> to` (the `to` node depends on `from` having run,
    /// i.e. `from` is a prerequisite of `to`).
    type Edge = NodeId * NodeId

    /// Kahn topological sort with a deterministic ascending-`NodeId` tie-break.
    /// `Ok order` lists every node in a valid run order; `Error remaining` lists the
    /// nodes that could not be ordered because they participate in (or depend on) a
    /// cycle. Edges that reference unknown nodes are ignored.
    val topoSort: nodes: NodeId list -> edges: Edge list -> Result<NodeId list, NodeId list>

    /// 3-colour DFS cycle detection. Returns one cycle witness (a list of node ids
    /// with the closing id repeated, e.g. `[a; b; c; a]`) if any cycle is present,
    /// otherwise `None`.
    val detectCycle: nodes: NodeId list -> edges: Edge list -> NodeId list option
