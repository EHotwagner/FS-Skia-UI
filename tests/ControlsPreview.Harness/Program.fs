module ControlsPreview.Program

// Dual-mode entry point:
//   --render   : run the generator (regenerate docs/img/controls/<id>.png from the single
//                sample source) on a render-capable host. Feature 079 T011.
//   otherwise  : run the Expecto harness suites (totality / explicitness / idempotence).

open Expecto

[<EntryPoint>]
let main argv =
    if argv |> Array.exists (fun a -> a = "--render") then
        ControlsPreview.Render.regenerateAll () |> ignore
        0
    elif argv |> Array.exists (fun a -> a = "--fidelity") then
        // Feature 080 (T017): decode each committed PNG, check its ContentSignature + the fixture
        // matrix, write readiness/control-fidelity.md, and exit non-zero on any failure.
        ControlsPreview.Fidelity.run ()
    else
        Tests.runTestsInAssemblyWithCLIArgs [] argv
