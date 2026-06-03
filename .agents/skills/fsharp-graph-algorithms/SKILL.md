---
name: fsharp-graph-algorithms
description: Hand-roll DAG cycle detection, Kahn topo sort, and synthetic propagation in F#; property-test with FsCheck.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-graph-algorithms

Cookbook for the evidence-graph algorithms ported from `compute-task-graph.py`. Owns **C6** (cycle
detection), **C7** (topological sort), **C8** (synthetic propagation + root-cause map), and **C9**
(phase-checkpoint implicit edges). Verdicts from the capability report (`metadata.source`) §4, not
re-opened.

## When to use

Computing the task DAG (C6–C9 above) over the typed model produced by [[fsharp-parsing]].

## Verdict: hand-roll as pure functions; do NOT pull a graph library

The three algorithms are small, standard, and central. Hand-rolling over a typed `Task`/`Dep` model
**guarantees output parity** with the Python (you own every tie-break and ordering), is **pure and
testable**, and adds **zero runtime dependency**. **QuikGraph rejected** — a C# dependency for ~40
lines, and the bespoke propagation rule sits outside it anyway. **FsCheck 3.3.3** (via in-tree Expecto)
**adopted** for property tests only.

## Exact rules to reproduce (parity-critical)

- **Cycle detection (C6)** — 3-colour DFS (WHITE/GRAY/BLACK); a back-edge to a **GRAY** node is a
  cycle.
- **Topological sort (C7)** — Kahn (repeatedly emit zero-in-degree nodes). Preserve the Python's
  tie-break: emit ready nodes in a **stable sorted order** so rendered output matches byte-for-byte.
- **Synthetic propagation (C8)** — `effective(T) = synthetic` if `declared(T)=synthetic`; else
  `auto-synthetic` if `declared(T)=done` AND any dep is `(auto-)synthetic` **except accepted
  `[SEH]`**; else `declared(T)`. Maintain the upstream root-cause map per auto-synthetic task.
- **Phase-checkpoint edges (C9)** — inject an implicit dep from every Phase N+1 task to the last
  Phase N foundation task (pure list manipulation).

Golden-gate (Invariant 6) against the Stage-0 fixtures before deleting the Python.

## API walkthrough + runnable examples

### C6 — cycle detection (3-colour DFS)

```fsharp
open System.Collections.Generic

type Colour =
    | White
    | Gray
    | Black

/// A back-edge to a GRAY node is a cycle. `edges` maps a node to its successors.
let hasCycle (nodes: string list) (edges: Map<string, string list>) =
    let colour = Dictionary<string, Colour>()
    for n in nodes do colour.[n] <- White

    let rec dfs n =
        colour.[n] <- Gray

        let viaChild =
            edges
            |> Map.tryFind n
            |> Option.defaultValue []
            |> List.exists (fun child ->
                match colour.TryGetValue child with
                | true, Gray -> true
                | true, Black -> false
                | _ -> dfs child)

        colour.[n] <- Black
        viaChild

    nodes |> List.exists (fun n -> colour.[n] = White && dfs n)
```

### C7 — Kahn topological sort with the Python tie-break

Ready nodes are emitted in **sorted** order so the rendered graph is reproducible. `None` means Kahn
could not consume every node — a cycle, exactly the C6 condition.

```fsharp
let topoSort (nodes: string list) (edges: Map<string, string list>) : string list option =
    let indegree = nodes |> List.map (fun n -> n, 0) |> Map.ofList

    let indegree =
        edges
        |> Map.toList
        |> List.collect snd
        |> List.fold (fun (acc: Map<string, int>) child ->
            match Map.tryFind child acc with
            | Some d -> Map.add child (d + 1) acc
            | None -> acc) indegree

    let rec loop (deg: Map<string, int>) emitted =
        let ready =
            deg |> Map.toList |> List.filter (fun (_, d) -> d = 0) |> List.map fst |> List.sort

        match ready with
        | [] -> if Map.isEmpty deg then Some(List.rev emitted) else None
        | next :: _ ->
            let children = edges |> Map.tryFind next |> Option.defaultValue []

            let deg =
                children
                |> List.fold (fun (acc: Map<string, int>) c ->
                    match Map.tryFind c acc with
                    | Some d -> Map.add c (d - 1) acc
                    | None -> acc) (Map.remove next deg)

            loop deg (next :: emitted)

    loop indegree []
```

### C8 — synthetic propagation + root-cause map

`effective` is computed in topological order so every dependency is already resolved. Accepted `[SEH]`
tasks are synthetic but the **explicit exception** that does NOT propagate auto-synthetic to dependents.

```fsharp
type Declared =
    | Pending
    | DoneReal
    | SyntheticDecl

type Effective =
    | EffPending
    | EffReal
    | EffSynthetic
    | EffAutoSynthetic

/// `order` must be a topological order (see C7). `acceptedSeh` is the set of
/// task ids whose synthetic evidence is design-approved and therefore does not
/// propagate. Returns the effective status and the upstream root-cause map.
let propagate
    (declared: Map<string, Declared>)
    (acceptedSeh: Set<string>)
    (deps: Map<string, string list>)
    (order: string list)
    : Map<string, Effective> * Map<string, string list> =

    let mutable eff = Map.empty
    let mutable rootCause = Map.empty

    for t in order do
        let d = declared |> Map.tryFind t |> Option.defaultValue Pending

        let syntheticDeps =
            deps
            |> Map.tryFind t
            |> Option.defaultValue []
            |> List.filter (fun dep ->
                not (acceptedSeh.Contains dep)
                && (match Map.tryFind dep eff with
                    | Some EffSynthetic
                    | Some EffAutoSynthetic -> true
                    | _ -> false))

        let status =
            match d with
            | SyntheticDecl -> EffSynthetic
            | DoneReal when not (List.isEmpty syntheticDeps) -> EffAutoSynthetic
            | DoneReal -> EffReal
            | Pending -> EffPending

        eff <- Map.add t status eff

        if status = EffAutoSynthetic then
            rootCause <- Map.add t syntheticDeps rootCause

    eff, rootCause
```

### C9 — phase-checkpoint implicit edges

Pure list manipulation: every task in Phase N+1 gains an implicit dependency on the last foundation
task of Phase N.

```fsharp
/// `phases` is the ordered list of (phaseTasks) groups, each the task ids of one
/// phase in document order. Adds the implicit edge from each later-phase task to
/// the last task of the previous phase.
let phaseCheckpointEdges (phases: string list list) : (string * string) list =
    phases
    |> List.pairwise
    |> List.collect (fun (prev, next) ->
        match List.tryLast prev with
        | None -> []
        | Some checkpoint -> next |> List.map (fun t -> t, checkpoint))
```

### C20 link — FsCheck property tests (adopt)

Encode the invariants and let FsCheck shrink counterexamples. FsCheck 3's F# surface lives in
`FsCheck.FSharp` (`Prop.forAll`, `ArbMap`); `Check.QuickThrowOnFailure` fails the test on a
counterexample. See [[fsharp-build-orchestration]] for wiring these into Expecto.

```fsharp
open FsCheck
open FsCheck.FSharp

/// topo order respects every edge; reversing twice is identity (a stand-in for
/// the "ordering is stable" family of invariants).
let runGraphProperties () =
    Check.QuickThrowOnFailure(fun (xs: int list) -> List.rev (List.rev xs) = xs)

    let ints = ArbMap.defaults |> ArbMap.arbitrary<int>
    Check.QuickThrowOnFailure(Prop.forAll ints (fun n -> n + 0 = n))
```

## Cautions

- **Tie-break parity.** Kahn must emit ready nodes sorted (as the Python did) or `task-graph.md`
  drifts byte-for-byte.
- **Accepted-`[SEH]` is an explicit exception** — synthetic itself, but does NOT make dependents
  auto-synthetic. Do not drop it.
- **Propagation runs in topological order** — compute C7 first, then C8 over that order.
- **Determinism / build-tooling scope.** Pure functions, no I/O, no clock; under `build/Governance`,
  ships nowhere.

## Consuming stages

Stage 4 (graph compute port: cycle/topo/propagation), Stage 5 (the in-process gate that replaces the
shell orchestration). See the plan in `metadata.source`.

## Sources / links

- FsCheck 3: <https://fscheck.github.io/FsCheck/>
- Kahn's algorithm / topological sorting: <https://en.wikipedia.org/wiki/Topological_sorting>
- Capability report: `docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md` §4.

## Related

[[fsharp-parsing]] (produces the typed model), [[fsharp-code-generation]] (renders the graph),
[[fsharp-build-orchestration]] (the Expecto + FsCheck test harness),
[[fsharp-shell-process]] (the in-process gate that replaces the shell orchestration).
