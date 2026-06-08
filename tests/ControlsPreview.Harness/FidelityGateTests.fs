module ControlsPreview.FidelityGateTests

// Feature 080 (T015, US2, SC-003/FR-013) — gate tests over the decoded-content fidelity gate.
//
// (* SYNTHETIC FIXTURE: these tests drive the synthetic gate test vectors under
//    fixtures/fidelity/{lowfi,faithful} — retained 079 label-on-box renders (must fail) and
//    their faithful counterparts (must pass). The fixtures are gate test vectors, not product
//    evidence; the catalog previews are real renders. *)
//
// These run on a render-capable host (they decode committed PNGs) under `-- --sequenced`.

open Expecto
open FS.Skia.UI.Build
open ControlsPreview.FidelityTypes
open ControlsPreview.Samples
open ControlsPreview.Render
open ControlsPreview.Fidelity

let private root = repositoryRoot ()

[<Tests>]
let fixtureMatrixTests =
    testList "Feature 080 fidelity fixture matrix (T015, SC-003)" [
        test "every lowfi fixture FAILS and every faithful fixture PASSES its control signature" {
            let results = fixtureResults root
            Expect.isNonEmpty results "fixtures are committed and discovered"
            let lowfi = results |> List.filter (fun r -> r.Bucket = "lowfi")
            let faithful = results |> List.filter (fun r -> r.Bucket = "faithful")
            Expect.isNonEmpty lowfi "lowfi fixtures present"
            Expect.isNonEmpty faithful "faithful fixtures present"
            for r in lowfi do
                Expect.isFalse r.ActualPass (sprintf "lowfi %s (079 label-on-box) must FAIL its signature (%s)" r.Id r.Detail)
            for r in faithful do
                Expect.isTrue r.ActualPass (sprintf "faithful %s must PASS its signature (%s)" r.Id r.Detail)
            for r in results do
                Expect.isTrue r.Ok (sprintf "%s/%s matches its expected verdict" r.Bucket r.Id)
        }
    ]

[<Tests>]
let perControlVerdictTests =
    testList "Feature 080 per-control fidelity verdicts (T022)" [
        test "every committed Demonstrative preview passes its signature; Unsupported has no image" {
            for s in samples do
                let v = verdictFor root s
                Expect.isTrue v.Passed (sprintf "%s passes its fidelity signature: %s" s.Id (v.FailureReason |> Option.defaultValue "n/a"))
        }
    ]

[<Tests>]
let failClosedTests =
    testList "Feature 080 fail-closed totality (FR-013)" [
        test "every catalogFacts id has a fidelity declaration (no gap)" {
            let catalogIds = CatalogGen.catalogFacts |> List.map (fun f -> f.Id) |> Set.ofList
            let declaredIds = sampleIds |> Set.ofList
            let undeclared = Set.difference catalogIds declaredIds
            Expect.isEmpty undeclared (sprintf "catalog ids missing a fidelity declaration: %A" undeclared)
        }

        test "a Demonstrative sample carries a Signature, never UnsupportedNoPreview" {
            for s in demonstrative do
                match s.Fidelity with
                | Signature _ -> ()
                | UnsupportedNoPreview -> failtestf "%s is Demonstrative but declares UnsupportedNoPreview" s.Id
        }
    ]
