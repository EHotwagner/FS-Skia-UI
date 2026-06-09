namespace FS.Skia.UI.Build.Evidence

open System.Collections.Generic

type EffectiveStatus =
    | DeclaredEff of DeclaredStatus
    | AutoSynthetic

type ResolvedTask =
    { Task: TaskRecord
      Effective: EffectiveStatus
      RootCause: string list }

type GraphVerdict =
    | Ok
    | Error

type Graph =
    { Order: string list
      Nodes: Map<string, TaskRecord> }

type GraphResult =
    { FeatureName: string
      Verdict: GraphVerdict
      Errors: string list
      Warnings: string list
      Cycles: string list list
      Tasks: ResolvedTask list
      RootCause: Map<string, string list> }

module Graph =

    let allDeps (t: TaskRecord) : string list =
        let seen = HashSet<string>()
        [ for d in t.ExplicitDeps @ t.PhaseDeps do
              if seen.Add d then yield d ]

    let ofTasks (tasks: TaskRecord list) : Graph =
        { Order = tasks |> List.map (fun t -> t.Id)
          Nodes = tasks |> List.map (fun t -> t.Id, t) |> Map.ofList }

    let effectiveString (e: EffectiveStatus) : string =
        match e with
        | DeclaredEff d -> TaskParser.declaredString d
        | AutoSynthetic -> "auto-synthetic"

    let detectCycles (g: Graph) : string list list =
        let WHITE, GRAY, BLACK = 0, 1, 2
        let color = Dictionary<string, int>()
        for tid in g.Order do
            color.[tid] <- WHITE
        let stack = ResizeArray<string>()
        let cycles = ResizeArray<string list>()

        let rec visit (tid: string) =
            color.[tid] <- GRAY
            stack.Add tid
            for d in allDeps g.Nodes.[tid] do
                match color.TryGetValue d with
                | false, _ -> () // dangling ref — skip (not a graph node)
                | true, c ->
                    if c = GRAY then
                        let idx = stack.IndexOf d
                        if idx >= 0 then
                            cycles.Add(List.ofSeq (stack.GetRange(idx, stack.Count - idx)) @ [ d ])
                        else
                            cycles.Add [ d; tid ]
                    elif c = WHITE then
                        visit d
            color.[tid] <- BLACK
            stack.RemoveAt(stack.Count - 1)

        for tid in g.Order do
            if color.[tid] = WHITE then
                visit tid

        List.ofSeq cycles

    let topoSort (g: Graph) : string list =
        let indeg = Dictionary<string, int>()
        for tid in g.Order do
            indeg.[tid] <- 0
        let rev = Dictionary<string, ResizeArray<string>>()
        for tid in g.Order do
            for d in allDeps g.Nodes.[tid] do
                if g.Nodes.ContainsKey d then
                    indeg.[tid] <- indeg.[tid] + 1
                    match rev.TryGetValue d with
                    | true, lst -> lst.Add tid
                    | _ ->
                        let lst = ResizeArray<string>()
                        lst.Add tid
                        rev.[d] <- lst

        let queue = Queue<string>()
        for tid in g.Order |> List.filter (fun t -> indeg.[t] = 0) |> List.sort do
            queue.Enqueue tid
        let order = ResizeArray<string>()
        while queue.Count > 0 do
            let tid = queue.Dequeue()
            order.Add tid
            let next =
                match rev.TryGetValue tid with
                | true, lst -> lst |> List.ofSeq |> List.sort
                | _ -> []
            for nxt in next do
                indeg.[nxt] <- indeg.[nxt] - 1
                if indeg.[nxt] = 0 then
                    queue.Enqueue nxt
        List.ofSeq order

    let propagate (g: Graph) (topo: string list) : ResolvedTask list * Map<string, string list> =
        let effective = Dictionary<string, EffectiveStatus>()
        let rootCause = Dictionary<string, string list>()

        let isTainted (d: string) =
            match g.Nodes.TryGetValue d, effective.TryGetValue d with
            | (true, dep), (true, eff) ->
                (match eff with
                 | DeclaredEff Synthetic
                 | AutoSynthetic -> true
                 | _ -> false)
                && not (TaskParser.isAcceptedSeh dep)
            | _ -> false

        for tid in topo do
            let t = g.Nodes.[tid]
            // Feature 087 (FR-009): taint propagates over REAL data dependencies
            // (`ExplicitDeps`, incl. owns/consumes) ONLY — never over the
            // auto-injected phase-checkpoint edges (`PhaseDeps`). Toposort, cycle
            // detection, and ordering keep using `allDeps` (above), so a
            // phase-checkpoint edge still orders the graph but no longer carries
            // synthetic contamination to an unrelated downstream task.
            let tainted = t.ExplicitDeps |> List.distinct |> List.filter isTainted
            let eff =
                if t.Declared = Synthetic then
                    DeclaredEff Synthetic
                elif t.Declared = Done && not (List.isEmpty tainted) then
                    rootCause.[tid] <- List.sort tainted
                    AutoSynthetic
                else
                    DeclaredEff t.Declared
            effective.[tid] <- eff

        let resolved =
            g.Order
            |> List.sort
            |> List.map (fun tid ->
                { Task = g.Nodes.[tid]
                  Effective = (match effective.TryGetValue tid with
                               | true, e -> e
                               | _ -> DeclaredEff g.Nodes.[tid].Declared)
                  RootCause =
                    match rootCause.TryGetValue tid with
                    | true, rc -> rc
                    | _ -> [] })

        resolved, (rootCause |> Seq.map (fun kv -> kv.Key, kv.Value) |> Map.ofSeq)
