module BuildEngineUpdateTests

// Feature 045 (US3, T020/FR-013): direct typed unit tests for the relocated PURE engine
// transition `update : BuildMsg -> BuildModel -> BuildModel * BuildEffect list`. These
// assert real typed BuildEffect lists for representative targets and exercise update with
// NO repo-tree I/O — the headline capability the 4,767-line script could not be tested for.
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Engine.Update
open GovernanceTestSupport

// init derives the path model from the real repository root (reads .specify/feature.json
// once); update itself performs no filesystem/git/process I/O.
let private model = fst (init repositoryRoot)

[<Tests>]
let buildEngineUpdateTests =
    testList "Build engine update (pure transition)" [
        test "StartTarget Route emits exactly the RouteSelect effect" {
            let next, effects = update (StartTarget Targets.Route) model
            Expect.equal effects [ RouteSelect ] "Route delegates to the in-process selector"
            Expect.equal next model "update does not mutate the model for Route"
        }

        test "StartTarget BuildWorkflowCheck emits exactly the WorkflowSelfCheck effect" {
            let _, effects = update (StartTarget Targets.BuildWorkflowCheck) model
            Expect.equal effects [ WorkflowSelfCheck ] "self-check delegates to the workflow self-check effect"
        }

        test "TargetCompleted records the target and emits no effects" {
            let next, effects = update (TargetCompleted "Restore") model
            Expect.equal effects [] "completion is a pure state update"
            Expect.equal next.CompletedTargets [ "Restore" ] "completed target is recorded"
        }

        test "StartTarget Clean emits seven directory-clean effects" {
            let _, effects = update (StartTarget Targets.Clean) model
            Expect.equal (List.length effects) 7 "Clean wipes the seven managed directories"
            Expect.isTrue
                (effects |> List.forall (function CleanDirectoryContents _ -> true | _ -> false))
                "every Clean effect is a CleanDirectoryContents"
        }

        test "StartTarget CapabilityCheck emits the catalog check and its required output" {
            let _, effects = update (StartTarget Targets.CapabilityCheck) model
            Expect.equal
                effects
                [ CapabilityCatalogCheck
                  RequireFiles("capability catalog report output", [ model.CapabilityCatalogReportPath ]) ]
                "CapabilityCheck requests the catalog check then requires its report"
        }

        test "StartTarget Dev emits the dev verdict and aggregate-hang diagnostics writes" {
            let _, effects = update (StartTarget Targets.Dev) model
            Expect.isTrue
                (effects |> List.exists (function WriteFile(p, _) -> p.EndsWith "dev-verdict.txt" | _ -> false))
                "Dev writes the dev verdict file"
            Expect.isTrue
                (effects |> List.exists (function WriteStructuredReport(label, _, _) -> label = "aggregate hang diagnostics" | _ -> false))
                "Dev writes the aggregate-hang diagnostics report"
        }

        test "update is deterministic for the same message and model" {
            let a = update (StartTarget Targets.GeneratedProductCheck) model
            let b = update (StartTarget Targets.GeneratedProductCheck) model
            Expect.equal a b "update is a pure function of (msg, model)"
        }
    ]
