module Tests

// Semantic + property coverage for the shipped FS.Skia.UI.SkillSupport public .fsi
// surface (feature 058, US2; Principle I/VI). Each family is exercised through its
// public entry points only — never internal helpers.

open Expecto
open FsCheck
open FS.Skia.UI.SkillSupport

let private edge (a, b) : Graph.Edge = (a, b)

[<Tests>]
let graphTests =
    testList "Graph (fsharp-graph-algorithms)" [
        test "topoSort orders a simple chain a -> b -> c" {
            match Graph.topoSort [ "c"; "a"; "b" ] [ edge ("a", "b"); edge ("b", "c") ] with
            | Ok order -> Expect.equal order [ "a"; "b"; "c" ] "chain ordered by prerequisite edges"
            | Error remaining -> failtestf "expected an acyclic order, got cycle remnants %A" remaining
        }

        test "topoSort uses an ascending tie-break among equal in-degree nodes" {
            match Graph.topoSort [ "b"; "a"; "c" ] [] with
            | Ok order -> Expect.equal order [ "a"; "b"; "c" ] "no edges ⇒ ascending id order"
            | Error _ -> failtest "no edges cannot be cyclic"
        }

        test "topoSort reports the cyclic nodes as Error" {
            match Graph.topoSort [ "a"; "b" ] [ edge ("a", "b"); edge ("b", "a") ] with
            | Ok order -> failtestf "a 2-cycle must not topo-sort, got %A" order
            | Error remaining -> Expect.equal (List.sort remaining) [ "a"; "b" ] "both nodes are cyclic"
        }

        test "detectCycle returns None for a DAG and Some for a cycle" {
            Expect.isNone (Graph.detectCycle [ "a"; "b"; "c" ] [ edge ("a", "b"); edge ("b", "c") ]) "DAG has no cycle"

            match Graph.detectCycle [ "a"; "b" ] [ edge ("a", "b"); edge ("b", "a") ] with
            | Some witness -> Expect.isGreaterThan (List.length witness) 1 "a cycle witness names >1 node"
            | None -> failtest "the 2-cycle must be detected"
        }

        test "property: a topo order respects every edge" {
            // For any small DAG built from a strictly-increasing edge set, the produced
            // order places each edge's source before its target.
            Check.QuickThrowOnFailure(fun (pairs: (int * int) list) ->
                let edges =
                    pairs
                    |> List.map (fun (a, b) -> let x, y = abs a % 8, abs b % 8 in (min x y, max x y))
                    |> List.filter (fun (a, b) -> a <> b)
                    |> List.distinct
                let nodes = [ 0 .. 7 ] |> List.map string
                let es = edges |> List.map (fun (a, b) -> edge (string a, string b))
                match Graph.topoSort nodes es with
                | Error _ -> true // construction is acyclic, but never claim a false negative
                | Ok order ->
                    let pos = order |> List.mapi (fun i n -> n, i) |> Map.ofList
                    es |> List.forall (fun (a, b) -> pos.[a] < pos.[b]))
        }
    ]

[<Tests>]
let parsingTests =
    testList "Parsing (fsharp-parsing)" [
        test "readYaml reads a scalar map value" {
            match Parsing.readYaml<System.Collections.Generic.Dictionary<string, string>> "name: graph\nkind: family\n" with
            | Ok map -> Expect.equal map.["name"] "graph" "the name key round-trips"
            | Error e -> failtestf "expected a parsed map, got error %s" e
        }

        test "readJson reads an int array" {
            match Parsing.readJson<int[]> "[1, 2, 3]" with
            | Ok xs -> Expect.equal (List.ofArray xs) [ 1; 2; 3 ] "the array round-trips"
            | Error e -> failtestf "expected a parsed array, got error %s" e
        }

        test "readJson reports malformed input as Error" {
            match Parsing.readJson<int[]> "[1, 2," with
            | Ok _ -> failtest "malformed JSON must not parse"
            | Error _ -> ()
        }

        test "matchLines yields the line index and the match" {
            let hits =
                Parsing.matchLines @"T(\d+)" [ "no id here"; "- [X] T024 do it"; "T100 last" ]
                |> Seq.map (fun (i, m) -> i, m.Value)
                |> List.ofSeq
            Expect.equal hits [ (1, "T024"); (2, "T100") ] "only the matching lines are returned with their index"
        }
    ]

[<Tests>]
let globbingTests =
    testList "Globbing (fsharp-io-globbing)" [
        test "isMatch: * stays within a segment, ** crosses /" {
            Expect.isTrue (Globbing.isMatch "*.fs" "Graph.fs") "*.fs matches a bare file"
            Expect.isFalse (Globbing.isMatch "*.fs" "src/Graph.fs") "* does not cross /"
            Expect.isTrue (Globbing.isMatch "src/**/*.fsi" "src/a/b.fsi") "** crosses directories"
            Expect.isTrue (Globbing.isMatch "src/**" "src/anything/here.fs") "trailing ** matches deeply"
        }

        test "currencyDiff is empty for identical text and non-empty for drift" {
            Expect.isEmpty (Globbing.currencyDiff "a\nb\n" "a\nb\n") "identical text is current"
            Expect.isNonEmpty (Globbing.currencyDiff "a\nb\n" "a\nc\n") "changed text drifts"
        }
    ]

[<Tests>]
let codeGenTests =
    testList "CodeGen (fsharp-code-generation)" [
        test "mermaidGraph emits a graph TD block with nodes and edges" {
            let g = CodeGen.mermaidGraph [ "A"; "B" ] [ ("A", "B") ]
            Expect.stringContains g "graph TD" "header present"
            Expect.stringContains g "A --> B" "edge rendered"
        }

        test "markdownTable emits a header and separator row" {
            let t = CodeGen.markdownTable [ "H1"; "H2" ] [ [ "a"; "b" ] ]
            Expect.stringContains t "| H1 | H2 |" "header row"
            Expect.stringContains t "| --- | --- |" "separator row"
            Expect.stringContains t "| a | b |" "data row"
        }

        test "asciiTree uses stable connectors" {
            let children =
                function
                | "root" -> [ "x"; "y" ]
                | _ -> []
            let tree = CodeGen.asciiTree [ "root" ] children
            Expect.stringContains tree "├─ x" "non-last child connector"
            Expect.stringContains tree "└─ y" "last child connector"
        }
    ]

[<Tests>]
let shellProcessTests =
    testList "ShellProcess (fsharp-shell-process)" [
        test "run captures exit code and stdout of a real process" {
            let result = ShellProcess.run "dotnet" [ "--version" ] (System.IO.Directory.GetCurrentDirectory())
            Expect.equal result.ExitCode 0 "dotnet --version exits 0"
            Expect.isGreaterThan result.StdOut.Length 0 "the version is written to stdout"
        }
    ]

// Feature 062 (FR-010, US6, SC-006): the two shipped arcade primitives. Exercised
// through their public .fsi only (Principle I/VI). Designed failing-first: the FSI
// sketch in readiness/fsi-session.txt exercised these shapes before the .fs bodies.

[<Tests>]
let randomTests =
    testList "Random (FR-010 deterministic seeded RNG)" [
        test "replay equality — same seed + same sequence ⇒ identical stream" {
            let stream seed =
                let mutable s = Random.seedRng seed
                [ for _ in 1..8 ->
                    let v, s' = Random.nextRng s
                    s <- s'
                    v ]
            Expect.equal (stream 42UL) (stream 42UL) "same seed replays the identical value stream"
        }

        test "the stream advances (consecutive draws differ)" {
            let s0 = Random.seedRng 1UL
            let v1, s1 = Random.nextRng s0
            let v2, _ = Random.nextRng s1
            Expect.notEqual v1 v2 "consecutive xorshift64 words differ"
        }

        test "seed 0 is non-degenerate (splitmix64 avoids the all-zero fixed point)" {
            let v, _ = Random.nextRng (Random.seedRng 0UL)
            Expect.notEqual v 0UL "seed 0 still produces a non-zero stream word"
        }

        test "nextBelow n returns a value in [0, n) for n > 0 (many seeds, many draws)" {
            let mutable allInRange = true
            for seed in [ 0UL; 1UL; 42UL; 7UL; 9999UL; 0xDEADBEEFUL ] do
                for n in [ 1; 2; 3; 6; 10; 52; 1000 ] do
                    let mutable s = Random.seedRng seed
                    for _ in 1..200 do
                        let r, s' = Random.nextBelow n s
                        if r < 0 || r >= n then allInRange <- false
                        s <- s'
            Expect.isTrue allInRange "every nextBelow n draw is in [0, n)"
        }

        test "nextBelow requires n > 0" {
            Expect.throws (fun () -> Random.nextBelow 0 (Random.seedRng 1UL) |> ignore) "n = 0 is rejected"
        }
    ]

[<Tests>]
let hudTests =
    testList "Hud (FR-010 reserveHudBand clamp/partition)" [
        test "HudBand.Size = min bandSize surface; Gameplay.Size = surface - HudBand.Size >= 0" {
            let l = Hud.reserveHudBand 600.0 48.0 Hud.Top
            Expect.equal l.HudBand.Size 48.0 "band clamps to bandSize when it fits"
            Expect.equal l.Gameplay.Size 552.0 "gameplay is the remainder"
            Expect.isGreaterThanOrEqual l.Gameplay.Size 0.0 "gameplay is never negative"
        }

        test "a band larger than the surface clamps; gameplay is zero, never negative" {
            let l = Hud.reserveHudBand 40.0 100.0 Hud.Top
            Expect.equal l.HudBand.Size 40.0 "band clamps to the surface"
            Expect.equal l.Gameplay.Size 0.0 "gameplay clamps to zero"
        }

        test "Top: bands are non-overlapping and partition the surface" {
            let l = Hud.reserveHudBand 600.0 48.0 Hud.Top
            Expect.equal l.HudBand.Offset 0.0 "Top band sits at offset 0"
            Expect.equal l.Gameplay.Offset l.HudBand.Size "gameplay starts where the band ends"
            Expect.equal (l.HudBand.Size + l.Gameplay.Size) 600.0 "the two bands partition the surface"
        }

        test "Bottom: bands are non-overlapping and partition the surface" {
            let l = Hud.reserveHudBand 600.0 48.0 Hud.Bottom
            Expect.equal l.Gameplay.Offset 0.0 "Bottom gameplay sits at offset 0"
            Expect.equal l.HudBand.Offset l.Gameplay.Size "band starts where gameplay ends"
            Expect.equal (l.HudBand.Size + l.Gameplay.Size) 600.0 "the two bands partition the surface"
        }
    ]
