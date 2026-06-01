module SkillistViewTests

(* Feature 044 / US2 — failing-first (Principle I/VI) typed tests for the skillist
   derived-view renderer + currency checker. Exercises the real
   `FS.Skia.UI.Build.SkillistView` functions on in-memory inputs (no mocks); before
   SkillistView.fs existed they did not compile. *)

open Expecto
open FS.Skia.UI.Build

[<Tests>]
let skillistViewTests =
    testList "Feature 044 SkillistView render + splice + currency" [

        test "renderAnnotation renders a non-empty list comma-separated, order preserved" {
            Expect.equal (SkillistView.renderAnnotation [ "a"; "b" ]) "[skillist: a, b]" "two-element render"
        }

        test "renderAnnotation renders the empty list as the literal empty token" {
            Expect.equal (SkillistView.renderAnnotation []) "[skillist: []]" "empty render"
        }

        test "spliceAnnotation changes only the bracketed token and preserves the rest of the line" {
            let line = "- [ ] T007 [P] [US1] [skillist: old-one, old-two] Add failing typed tests in path/File.fs"
            let spliced = SkillistView.spliceAnnotation [ "fsharp-io-globbing"; "fsharp-build-orchestration" ] line
            Expect.equal
                spliced
                "- [ ] T007 [P] [US1] [skillist: fsharp-io-globbing, fsharp-build-orchestration] Add failing typed tests in path/File.fs"
                "only the [skillist: …] token is rewritten; every other byte preserved"
        }

        test "spliceAnnotation can rewrite a populated token to the empty token" {
            let line = "- [ ] T020 [US3] [skillist: fsharp-build-orchestration] do the thing"
            Expect.equal
                (SkillistView.spliceAnnotation [] line)
                "- [ ] T020 [US3] [skillist: []] do the thing"
                "populated → empty token"
        }

        test "spliceAnnotation raises when the line carries no annotation token (omitted metadata is invalid)" {
            Expect.throws (fun () -> SkillistView.spliceAnnotation [ "a" ] "- [ ] T099 no annotation here" |> ignore) "no token → hard error"
        }

        test "currency flags a stale derived annotation and passes a current one" {
            let canonical = [ "T007", [ "fsharp-io-globbing"; "fsharp-build-orchestration" ]; "T010", [ "fsharp-build-orchestration" ] ]
            let derived =
                [ "T007", Some [ "fsharp-io-globbing"; "fsharp-build-orchestration" ] // current
                  "T010", Some [ "stale-skill" ] ] // stale
            let items = SkillistView.currency canonical derived
            let stale = items |> List.filter (fun i -> i.IsStale) |> List.map (fun i -> i.TaskId)
            Expect.equal stale [ "T010" ] "only the divergent task is stale"
            match SkillistView.currencyDrift items with
            | Some msg -> Expect.stringContains msg "RefreshSurfaceBaselines" "currency diagnostic names the regen command"
            | None -> failtest "expected a Some drift diagnostic"
        }

        test "an absent annotation is reported as stale, not silently inserted" {
            let canonical = [ "T015", [ "fsharp-code-generation" ] ]
            let derived = [ "T015", None ]
            let items = SkillistView.currency canonical derived
            Expect.isTrue (items.Head.IsStale) "absent annotation is invalid → stale"
        }

        test "currency is active-feature scoped — only canonical task ids are considered" {
            let canonical = [ "T001", [] ]
            let derived = [ "T001", Some []; "T999", Some [ "ghost" ] ]
            let items = SkillistView.currency canonical derived
            Expect.equal (items |> List.map (fun i -> i.TaskId)) [ "T001" ] "tasks outside the canonical (active) set are never re-derived"
        }
    ]
