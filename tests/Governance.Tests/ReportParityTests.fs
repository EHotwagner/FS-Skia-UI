module ReportParityTests

(* Feature 041 / US2 — golden-diff parity (FR-008a, R1/R2). The three reports are
   rendered through the real library and asserted byte-identical to the pre-extraction
   golden fixtures committed under fixtures/reports-golden/. capability-catalog.md and
   target-metadata-drift.md are fully deterministic; target-metadata.json is rendered
   from the typed model parsed back out of the fixture (the absolute repository root and
   the timestamp are normalized to placeholders so the oracle is portable — R2). The
   build-edge full-file diff against the live report is recorded under readiness/. *)

open System
open System.IO
open System.Text.Json
open Expecto
open GovernanceTestSupport
open FS.Skia.UI.Build

let private goldenDir = fullPath "tests/Governance.Tests/fixtures/reports-golden"
let private goldenText (name: string) = File.ReadAllText(Path.Combine(goldenDir, name))

let private orEmpty value = value |> Option.ofObj |> Option.defaultValue ""
let private str (el: JsonElement) (prop: string) = el.GetProperty(prop).GetString() |> orEmpty
let private strArray (el: JsonElement) (prop: string) =
    [ for item in el.GetProperty(prop).EnumerateArray() -> item.GetString() |> orEmpty ]

let private parseMetadata (json: string) : TargetMetadata.TargetMetadata list =
    use doc = JsonDocument.Parse json

    [ for t in doc.RootElement.GetProperty("targets").EnumerateArray() ->
        { RunnableTargetName = str t "runnable_target_name"
          DirectPrerequisites = strArray t "direct_prerequisites"
          ExpectedOutputs = strArray t "expected_outputs"
          StaleAssumptions = strArray t "stale_assumptions"
          TimeoutClass = str t "timeout_class"
          Cost = str t "cost"
          Authority = str t "authority"
          FailureOwner = str t "failure_owner"
          Command = str t "command" } ]

[<Tests>]
let reportParityTests =
    testList "Feature 041 report golden-diff parity" [

        test "capability-catalog.md is byte-identical to the golden fixture" {
            let actual =
                Capabilities.renderReport (Capabilities.readCatalog (fullPath "template/capabilities.yml"))
                + Environment.NewLine

            Expect.equal actual (goldenText "capability-catalog.md") "capability catalog report parity"
        }

        test "target-metadata-drift.md (PASS) is byte-identical to the golden fixture" {
            Expect.equal (TargetMetadata.driftMarkdown []) (goldenText "target-metadata-drift.md") "drift report parity"
        }

        test "target-metadata.json renderer reproduces the golden bytes from the typed model" {
            let golden = goldenText "target-metadata.json"
            let metadata = parseMetadata golden
            let rendered = TargetMetadata.metadataJson "__GENERATED_AT_UTC__" [] metadata
            Expect.equal rendered golden "metadata JSON renderer reproduces the golden bytes (every line incl. paths)"
        }

        test "the live timestamp (R2) is the only non-deterministic byte and is well-formed" {
            let sample = TargetMetadata.metadataJson (DateTimeOffset.UtcNow.ToString("O")) [] []
            use doc = JsonDocument.Parse sample
            let timestamp = doc.RootElement.GetProperty("generated_at_utc").GetString() |> orEmpty
            Expect.isTrue (DateTimeOffset.TryParse timestamp |> fst) "generated_at_utc is present and well-formed"
        }
    ]
