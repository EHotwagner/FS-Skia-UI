module TargetMetadataTests

(* Feature 041 / US1 — failing-first (Principle I/VI) typed-finding tests for the
   target-metadata drift validator extracted into FS.Skia.UI.Build.TargetMetadata.
   These exercise the real library function over crafted typed inputs (no string
   matching, no mocks); before TargetMetadata.fs existed they did not compile (SC-004). *)

open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.TargetMetadata

let private meta name expectedOutputs failureOwner prerequisites : TargetMetadata =
    { RunnableTargetName = name
      DirectPrerequisites = prerequisites
      ExpectedOutputs = expectedOutputs
      StaleAssumptions = []
      TimeoutClass = "focused"
      Cost = "low"
      Authority = "authoritative"
      FailureOwner = failureOwner
      Command = "./fake.sh build -t " + name }

[<Tests>]
let targetMetadataDriftTests =
    testList "Feature 041 TargetMetadata drift (typed cases)" [

        test "MissingMetadata when a runnable target has no metadata row" {
            let metadata = [ meta "Build" [ "out" ] "governance" [] ]
            let drift = TargetMetadata.validateMetadataDrift [ "Build"; "Restore" ] metadata
            Expect.contains drift (MissingMetadata "Restore") "Restore is runnable but has no metadata row"
        }

        test "MissingRunnableTarget when a metadata row is not a runnable target" {
            let metadata = [ meta "Ghost" [ "out" ] "governance" [] ]
            let drift = TargetMetadata.validateMetadataDrift [] metadata
            Expect.contains drift (MissingRunnableTarget "Ghost") "Ghost has metadata but is not runnable"
        }

        test "MissingExpectedOutput when a metadata row has no expected outputs" {
            let metadata = [ meta "Build" [] "governance" [] ]
            let drift = TargetMetadata.validateMetadataDrift [ "Build" ] metadata
            Expect.contains drift (MissingExpectedOutput "Build") "Build declares no expected output"
        }

        test "MissingFailureOwner when a metadata row has a blank failure owner" {
            let metadata = [ meta "Build" [ "out" ] "   " [] ]
            let drift = TargetMetadata.validateMetadataDrift [ "Build" ] metadata
            Expect.contains drift (MissingFailureOwner "Build") "Build has a blank failure owner"
        }

        test "DependencyDivergence when a metadata row has a blank prerequisite" {
            let metadata = [ meta "Build" [ "out" ] "governance" [ "" ] ]
            let drift = TargetMetadata.validateMetadataDrift [ "Build" ] metadata
            Expect.contains drift (DependencyDivergence "Build") "Build has a blank prerequisite"
        }

        test "the derived typed registry has no structural drift against itself (SC-003)" {
            // The single-source DU + total spec make a runnable target without metadata (or
            // vice versa) unrepresentable; the derived metadata registry is drift-free.
            let runnable = Targets.requiredTargetNames

            let metadata =
                Targets.allTargets
                |> List.map (fun t ->
                    let spec = Targets.spec t
                    meta spec.Name [ "out" ] spec.FailureOwner (spec.DirectPrerequisites |> List.map Targets.name))

            Expect.isEmpty (TargetMetadata.validateMetadataDrift runnable metadata) "no structural drift in the derived registry"
        }
    ]
