module EvidenceAlgorithmTests

// T022/T023/T024 (US2): typed unit + FsCheck property coverage for the graph
// algorithms and the audit-status region scan on inputs the golden fixtures do
// not exercise (SC-002, FR-014). Assertions are over TYPED Graph/StatusRegion
// results, never string scraping.

open Expecto
open FsCheck
open FS.Skia.UI.Build.Evidence

let private emptySeh : SehMetadata =
    { Annotation = false
      ApprovalLabel = false
      DesignSource = ""
      SyntheticInputClass = ""
      ExpectedErrorBehavior = ""
      Rationale = ""
      AcceptanceStatus = ""
      Diagnostics = [] }

let private mkTask (id: string) (declared: DeclaredStatus) (deps: string list) : TaskRecord =
    { Id = id
      Declared = declared
      Phase = None
      Story = None
      Tier = None
      Parallel = false
      LineNo = 0
      Title = id
      SkillistMirror = Some []
      Skillist = []
      ExplicitDeps = deps
      PhaseDeps = []
      SkillMatchAssessments = []
      Seh = emptySeh }

let private propagateGraph (tasks: TaskRecord list) =
    let g = Graph.ofTasks tasks
    let order = Graph.topoSort g
    Graph.propagate g order

[<Tests>]
let evidenceAlgorithmTests =
    testList
        "US2 evidence algorithms"
        [ test "cycle detection flags a cyclic graph" {
              let g = Graph.ofTasks [ mkTask "T001" Done [ "T002" ]; mkTask "T002" Done [ "T001" ] ]
              let cycles = Graph.detectCycles g
              Expect.isNonEmpty cycles "a T001<->T002 cycle is detected"
          }

          test "cycle detection accepts an acyclic graph" {
              let g = Graph.ofTasks [ mkTask "T001" Done []; mkTask "T002" Done [ "T001" ]; mkTask "T003" Done [ "T001"; "T002" ] ]
              Expect.isEmpty (Graph.detectCycles g) "no cycle in a DAG"
          }

          test "Kahn topological order respects deps with id-sorted tie-break" {
              let g = Graph.ofTasks [ mkTask "T003" Done [ "T001" ]; mkTask "T002" Done [ "T001" ]; mkTask "T001" Done [] ]
              let order = Graph.topoSort g
              Expect.equal order [ "T001"; "T002"; "T003" ] "roots first, then ascending-id tie-break"
          }

          test "multi-synthetic-root: a Done task tainted by two synthetic roots is auto-synthetic with both in root-cause" {
              let tasks =
                  [ mkTask "T001" Synthetic []
                    mkTask "T002" Synthetic []
                    mkTask "T003" Done [ "T002"; "T001" ] ]
              let resolved, rootCause = propagateGraph tasks
              let t3 = resolved |> List.find (fun r -> r.Task.Id = "T003")
              Expect.equal t3.Effective AutoSynthetic "T003 propagates to auto-synthetic"
              Expect.equal (rootCause.["T003"]) [ "T001"; "T002" ] "root cause is the sorted synthetic roots"
          }

          test "empty graph propagates to an empty result" {
              let resolved, rootCause = propagateGraph []
              Expect.isEmpty resolved "no tasks"
              Expect.isTrue (Map.isEmpty rootCause) "no root causes"
          }

          testCase "property: no synthetic anywhere => no auto-synthetic nodes"
          <| fun () ->
              let prop (k: int) =
                  let n = (abs k % 25) + 1
                  let tasks =
                      [ for i in 0 .. n - 1 ->
                            let id = sprintf "T%03d" i
                            let deps = if i = 0 then [] else [ sprintf "T%03d" (i - 1) ]
                            mkTask id Done deps ]
                  let resolved, _ = propagateGraph tasks
                  resolved |> List.forall (fun r -> r.Effective <> AutoSynthetic)
              Check.QuickThrowOnFailure prop

          testCase "property (monotonicity): a synthetic root taints the whole downstream Done chain"
          <| fun () ->
              let prop (k: int) =
                  let n = (abs k % 24) + 2
                  let tasks =
                      [ for i in 0 .. n - 1 ->
                            let id = sprintf "T%03d" i
                            let declared = if i = 0 then Synthetic else Done
                            let deps = if i = 0 then [] else [ sprintf "T%03d" (i - 1) ]
                            mkTask id declared deps ]
                  let resolved, _ = propagateGraph tasks
                  // every Done task downstream of the synthetic root is auto-synthetic
                  resolved
                  |> List.filter (fun r -> r.Task.Id <> "T000")
                  |> List.forall (fun r -> r.Effective = AutoSynthetic)
              Check.QuickThrowOnFailure prop

          // --- T024: audit-status region scan -------------------------------

          test "first audit-status region wins; later regions are ignored" {
              let text =
                  "```audit-status\nexact-package-match=true\n```\n\n```audit-status\nexact-package-match=false\n```\n"
              let r = StatusRegion.scanText "f.md" text
              Expect.isFalse r.Blocking "the first (clean) region is authoritative"
          }

          test "duplicate key within a region is a parse error" {
              let text = "```audit-status\nexact-package-match=true\nexact-package-match=true\n```\n"
              let r = StatusRegion.scanText "f.md" text
              Expect.isTrue r.Blocking "duplicate key blocks"
              Expect.isTrue (r.Reasons |> List.exists (fun x -> x.Contains "duplicate key in audit-status region")) "names the duplicate-key parse error"
          }

          test "prose is never interpreted as status" {
              let text = "This mentions taskbar-only=true and nu1603 in prose.\n\n```audit-status\nexact-package-match=true\n```\n"
              let r = StatusRegion.scanText "f.md" text
              Expect.isFalse r.Blocking "blocker terms in prose outside the region do not block"
          }

          test "blocking condition: taskbar-only=true" {
              let r = StatusRegion.scanText "f.md" "```audit-status\ntaskbar-only=true\n```\n"
              Expect.isTrue r.Blocking "taskbar-only=true blocks"
          }

          test "blocking condition: taskbar-entry=true with window-visible=false" {
              let r = StatusRegion.scanText "f.md" "```audit-status\ntaskbar-entry=true\nwindow-visible=false\n```\n"
              Expect.isTrue r.Blocking "taskbar-entry + window-visible=false blocks"
          }

          test "blocking condition: exact-package-match not in {true,yes}" {
              let r = StatusRegion.scanText "f.md" "```audit-status\nexact-package-match=partial\n```\n"
              Expect.isTrue r.Blocking "non-true exact-package-match blocks"
          }

          test "blocking condition: package-resolution=nu1603" {
              let r = StatusRegion.scanText "f.md" "```audit-status\npackage-resolution=nu1603\n```\n"
              Expect.isTrue r.Blocking "package-resolution=nu1603 blocks"
          } ]
