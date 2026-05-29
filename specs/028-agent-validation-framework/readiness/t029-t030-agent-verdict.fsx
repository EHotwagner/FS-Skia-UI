#r "../../../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"

open System
open System.Text.Json
open FS.Skia.UI.AgentValidation

let source =
    { Kind = GitMergeBaseDiff
      Feature = Some "028-agent-validation-framework"
      MergeBase = Some "5d804e6"
      Paths = [ "src/Lib/AgentValidation.fsi" ]
      Diagnostics = [] }

let verdict =
    AgentVerdict.aggregate
        "./fake.sh build -t Verify"
        source
        [ "package-surface" ]
        [ "PackageSurfaceCheck"; "EvidenceGraph" ]
        [ { Gate = "PackageSurfaceCheck"
            Outcome = GatePassed
            Artifacts = [ "readiness/package-surface-expectations.md" ] }
          { Gate = "EvidenceGraph"
            Outcome = GatePassed
            Artifacts = [ "readiness/evidence-graph.md" ] } ]
        [ "readiness/agent-verdict.md" ]
        (DateTimeOffset.Parse("2026-05-28T12:00:00Z"))

let json = AgentVerdict.toJson verdict
let markdown = AgentVerdict.toMarkdown verdict

printfn "%s" json
printfn "%s" markdown

use document = JsonDocument.Parse json
let root = document.RootElement

if root.GetProperty("status").GetString() <> "passed" then
    failwith "expected passed verdict"

if root.GetProperty("failure_class").ValueKind <> JsonValueKind.Null then
    failwith "passed verdict should serialize null failure_class"

if not (markdown.Contains("focused-authoritative", StringComparison.Ordinal)) then
    failwith "markdown should preserve focused authority wording"

let failed =
    AgentVerdict.aggregate
        "./fake.sh build -t Verify"
        source
        [ "package-surface" ]
        [ "PackageSurfaceCheck" ]
        [ { Gate = "PackageSurfaceCheck"
            Outcome = GateFailed(Product, ProductFailure, "surface mismatch")
            Artifacts = [] } ]
        []
        (DateTimeOffset.Parse("2026-05-28T12:00:00Z"))

printfn "failed-status: %A owner=%A class=%A diagnostics=%A" failed.Status failed.FailureOwner failed.FailureClass failed.Diagnostics

if failed.Status <> Failed || failed.FailureOwner <> Product || failed.FailureClass <> ProductFailure then
    failwith "expected product failure classification"
