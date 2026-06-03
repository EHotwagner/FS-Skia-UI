namespace FS.Skia.UI.SkillSupport

open System.Collections.Generic

module Graph =

    type NodeId = string
    type Edge = NodeId * NodeId

    let private adjacency (nodes: NodeId list) (edges: Edge list) =
        let known = HashSet<NodeId>(nodes)
        let succ = Dictionary<NodeId, ResizeArray<NodeId>>()
        for n in nodes do
            succ.[n] <- ResizeArray<NodeId>()
        for (from, to_) in edges do
            if known.Contains from && known.Contains to_ then
                succ.[from].Add to_
        succ

    let topoSort (nodes: NodeId list) (edges: Edge list) : Result<NodeId list, NodeId list> =
        let known = HashSet<NodeId>(nodes)
        let indeg = Dictionary<NodeId, int>()
        for n in nodes do
            indeg.[n] <- 0
        let succ = adjacency nodes edges
        for (from, to_) in edges do
            if known.Contains from && known.Contains to_ then
                indeg.[to_] <- indeg.[to_] + 1

        let queue = Queue<NodeId>()
        for n in nodes |> List.filter (fun n -> indeg.[n] = 0) |> List.sort do
            queue.Enqueue n

        let order = ResizeArray<NodeId>()
        while queue.Count > 0 do
            let n = queue.Dequeue()
            order.Add n
            for nxt in succ.[n] |> List.ofSeq |> List.sort do
                indeg.[nxt] <- indeg.[nxt] - 1
                if indeg.[nxt] = 0 then
                    queue.Enqueue nxt

        if order.Count = List.length nodes then
            Ok(List.ofSeq order)
        else
            let ordered = HashSet<NodeId>(order)
            Error(nodes |> List.filter (fun n -> not (ordered.Contains n)) |> List.sort)

    let detectCycle (nodes: NodeId list) (edges: Edge list) : NodeId list option =
        let WHITE, GRAY, BLACK = 0, 1, 2
        let color = Dictionary<NodeId, int>()
        for n in nodes do
            color.[n] <- WHITE
        let succ = adjacency nodes edges
        let stack = ResizeArray<NodeId>()
        let mutable witness = None

        let rec visit (n: NodeId) =
            if witness.IsNone then
                color.[n] <- GRAY
                stack.Add n
                for d in succ.[n] do
                    if witness.IsNone then
                        match color.TryGetValue d with
                        | true, c when c = GRAY ->
                            let idx = stack.IndexOf d
                            if idx >= 0 then
                                witness <- Some(List.ofSeq (stack.GetRange(idx, stack.Count - idx)) @ [ d ])
                            else
                                witness <- Some [ d; n; d ]
                        | true, c when c = WHITE -> visit d
                        | _ -> ()
                color.[n] <- BLACK
                stack.RemoveAt(stack.Count - 1)

        for n in nodes do
            if color.[n] = WHITE && witness.IsNone then
                visit n

        witness
