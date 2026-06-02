module AgentValidationFrameworkTests

open System
open System.Diagnostics
open System.IO
open System.Text.Json
open Expecto
open FS.Skia.UI.Build.AgentValidation
open GovernanceTestSupport

let private contractPath = "validation.contract.yml"

let private contractText () =
    Expect.isTrue (fileExists contractPath) "validation.contract.yml exists at repository root"
    read contractPath

let private buildText () =
    readBuildGovernanceSources ()

let private expectContractContainsAll (needles: string list) =
    let contract = contractText ()

    needles
    |> List.iter (fun needle ->
        Expect.stringContains contract needle $"validation contract contains {needle}")

let private withTempDirectory testBody =
    let dir = Path.Combine(Path.GetTempPath(), "fs-skia-agent-validation-" + Guid.NewGuid().ToString("N"))
    Directory.CreateDirectory dir |> ignore

    try
        testBody dir
    finally
        if Directory.Exists dir then
            Directory.Delete(dir, true)

let private runInDirectory workingDirectory fileName arguments =
    let startInfo = ProcessStartInfo(fileName)
    startInfo.WorkingDirectory <- workingDirectory
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    arguments |> List.iter startInfo.ArgumentList.Add

    match Process.Start startInfo |> Option.ofObj with
    | None ->
        failtestf "Could not start %s %A" fileName arguments
    | Some proc ->
        use proc = proc
        let stdout = proc.StandardOutput.ReadToEnd()
        let stderr = proc.StandardError.ReadToEnd()

        if proc.WaitForExit(30000) && proc.ExitCode = 0 then
            stdout
        else
            failtestf "%s %A failed with exit %i\nstdout:\n%s\nstderr:\n%s" fileName arguments proc.ExitCode stdout stderr

[<Tests>]
let agentValidationFrameworkTests =
    testList "Agent validation framework contract" [
        test "validation contract declares schema tiers defaults routing and diagnostics" {
            let contract = contractText ()

            [ "schema_version:"
              "tiers:"
              "tier1:"
              "tier2:"
              "defaults:"
              "routing_rules:"
              "expected_artifacts:"
              "timeout_class:"
              "failure_owner:"
              "unknown_gate_rejection:" ]
            |> List.iter (fun needle ->
                Expect.stringContains contract needle $"validation.contract.yml declares {needle}")
        }

        test "validation contract names required authoritative gates" {
            let contract = contractText ()

            [ "AgentReady"
              "GeneratedGuidanceCheck"
              "TemplateCheck"
              "PackageSurfaceCheck"
              "FsiTranscripts"
              "GeneratedProductCheck"
              "TargetMetadataDrift"
              "EvidenceGraph"
              "EvidenceAudit"
              "Verify"
              "Ci" ]
            |> List.iter (fun gate ->
                Expect.stringContains contract gate $"validation contract references {gate}")
        }

        test "validation contract routes representative changed paths to focused gates" {
            let contract = contractText ()

            [ "src/Controls/**"
              "template/base/**"
              "template/fragments/**"
              "docs/**"
              "build.fsx"
              "specs/**/tasks.md"
              "specs/**/tasks.deps.yml" ]
            |> List.iter (fun pathPattern ->
                Expect.stringContains contract pathPattern $"validation contract routes {pathPattern}")
        }

        test "validation contract rejects unknown gates with governance-owned diagnostics" {
            let contract = contractText ()

            [ "unknown gate"
              "governance"
              "diagnostic"
              "no success verdict" ]
            |> List.iter (fun required ->
                Expect.isTrue
                    (contract.Contains(required, StringComparison.OrdinalIgnoreCase))
                    $"validation contract describes unknown-gate rejection using {required}")
        }

        test "validation selection supports active feature paths git fallback degradation and gate union" {
            let source = buildText ()

            [ "type ValidationSelectionModel"
              "type ValidationSelectionMsg"
              "type ValidationSelectionEffect"
              "ReadActiveFeatureMetadata"
              "RunGitMergeBaseDiff"
              "ChangedPathSourceActiveFeature"
              "ChangedPathSourceGitMergeBase"
              "ChangedPathSourceUnavailable"
              "ValidationSelectionDegraded"
              "unionSelectedGates"
              "selectedRuleIds"
              "multi-rule gate union" ]
            |> List.iter (fun required ->
                Expect.stringContains source required $"validation selection preserves {required}")
        }

        test "agent verdict contract exposes required JSON fields and reviewer Markdown authority wording" {
            let source = buildText ()

            [ "type AgentVerdict"
              "AgentVerdictJson"
              "AgentVerdictMarkdown"
              "status"
              "authority"
              "changedPathSource"
              "selectedRuleIds"
              "requiredGates"
              "completedGates"
              "missingGates"
              "missingArtifacts"
              "failureOwner"
              "failureClass"
              "nextCommand"
              "non-authoritative aggregate"
              "focused authority" ]
            |> List.iter (fun required ->
                Expect.stringContains source required $"agent verdict contract preserves {required}")
        }

        test "target metadata drift contract names runnable target metadata outputs owners and dependencies" {
            let source = buildText ()

            // Feature 041: the target-metadata drift contract now lives in the compiled
            // FS.Skia.UI.Build.TargetMetadata module (scanned via readBuildGovernanceSources);
            // the intermediate TargetMetadataReport type was retired and the drift validator is
            // the pure `validateMetadataDrift`.
            [ "type TargetMetadata"
              "RunnableTargetName"
              "DirectPrerequisites"
              "ExpectedOutputs"
              "StaleAssumptions"
              "TimeoutClass"
              "Cost"
              "Authority"
              "FailureOwner"
              "Command"
              "validateMetadataDrift"
              "missing runnable target"
              "missing metadata"
              "missing expected output"
              "missing failure owner"
              "dependency divergence" ]
            |> List.iter (fun required ->
                Expect.stringContains source required $"target metadata drift contract preserves {required}")
        }

        test "Synthetic malformed validation contract fixtures reject invalid shapes" {
            // SYNTHETIC: intentionally malformed contract fixtures prove SEH rejection paths; real success routing evidence is recorded in readiness/validation-contract.md.
            let source = buildText ()

            [ "malformed-validation-contract.yml"
              "duplicate-rule-id"
              "unknown-gate"
              "invalid-path-pattern"
              "ValidationContractRejected"
              "no success verdict" ]
            |> List.iter (fun required ->
                Expect.stringContains source required $"malformed validation contract fixture preserves {required}")
        }

        test "Synthetic malformed agent verdict fixtures reject invalid handoff payloads" {
            // SYNTHETIC: intentionally malformed verdict fixtures prove SEH rejection paths; real AgentReady verdict evidence is recorded in readiness/agent-ready-verdict.md.
            let source = buildText ()

            [ "malformed-agent-verdict.json"
              "missing status"
              "missing authority"
              "missing requiredGates"
              "unknown failureClass"
              "AgentVerdictRejected"
              "governance-owned diagnostic" ]
            |> List.iter (fun required ->
                Expect.stringContains source required $"malformed verdict fixture preserves {required}")
        }
    ]

[<Tests>]
let agentValidationUs1Tests =
    testList "US1 validation routing" [
        test "routing scenarios cover controls templates evidence guidance docs package and build targets" {
            expectContractContainsAll
                [ "controls-public-surface"
                  "src/Controls/**"
                  "ControlsCatalogCheck"
                  "ControlsInteractionCheck"
                  "ControlsRenderingCheck"
                  "generated-template"
                  "template/base/**"
                  "TemplateCheck"
                  "GeneratedProductCheck"
                  "evidence-governance"
                  "specs/**/tasks.md"
                  "specs/**/tasks.deps.yml"
                  "EvidenceGraph"
                  "EvidenceAudit"
                  "generated-guidance"
                  "GeneratedGuidanceCheck"
                  "docs-only"
                  "docs/**"
                  "package-surface"
                  "PackageSurfaceCheck"
                  "build-target-contract"
                  "build.fsx" ]
        }

        test "routing scenarios preserve expected artifacts owners and timeout classes" {
            expectContractContainsAll
                [ "readiness/validation-contract.md"
                  "readiness/typed-controls-front-door.md"
                  "readiness/evidence-policy-separation.md"
                  "readiness/target-metadata.md"
                  "timeout_class: focused"
                  "timeout_class: broad"
                  "failure_owner: product"
                  "failure_owner: template"
                  "failure_owner: governance" ]
        }

        test "ValidationSelection init and update emit public MVU effects for active feature and contract load" {
            let model, effects = ValidationSelection.init (Some "028-agent-validation-framework")

            Expect.equal model.Feature (Some "028-agent-validation-framework") "init records feature"
            Expect.equal effects [ ReadActiveFeatureMetadata ] "init asks interpreter to read active feature metadata"

            let next, nextEffects =
                ValidationSelection.update
                    (ActiveFeatureMetadataLoaded("028-agent-validation-framework", [ "src/Controls/Types.fsi"; "build.fsx" ]))
                    model

            Expect.equal next.Feature (Some "028-agent-validation-framework") "active feature response records feature"
            Expect.equal nextEffects [ LoadValidationContract ] "active feature paths ask interpreter to load validation contract"
            Expect.equal (next.ChangedPathSource |> Option.map _.Kind) (Some ActiveFeatureMetadata) "active feature metadata is the changed path source"
        }

        test "ValidationSelection update emits git fallback and degraded unavailable effects" {
            let model, _ = ValidationSelection.init None

            let fallback, fallbackEffects =
                ValidationSelection.update (ActiveFeatureMetadataUnavailable "feature metadata missing") model

            Expect.equal fallbackEffects [ RunGitMergeBaseDiff ] "missing feature metadata asks for git merge-base diff"
            Expect.contains fallback.Diagnostics "feature metadata missing" "diagnostic records missing feature metadata"

            let degraded, degradedEffects =
                ValidationSelection.update (GitMergeBaseDiffUnavailable "git unavailable") fallback

            Expect.isTrue degraded.Degraded "unavailable git diff degrades selection"
            Expect.equal degradedEffects [ WriteValidationSelectionReport ] "degraded state emits selection report"
            Expect.equal (degraded.ChangedPathSource |> Option.map _.Kind) (Some Unavailable) "unavailable source is explicit"
        }

        test "ValidationSelection contract load unions duplicate gates and preserves selected rule ids" {
            let model, _ = ValidationSelection.init None

            let selected, effects =
                ValidationSelection.update
                    (ContractLoaded(
                        [ "controls-public-surface"; "build-target-contract"; "controls-public-surface" ],
                        [ "PackageSurfaceCheck"; "EvidenceGraph"; "PackageSurfaceCheck"; "EvidenceAudit" ],
                        AgentReadyAuthority))
                    model

            Expect.equal selected.SelectedRuleIds [ "controls-public-surface"; "build-target-contract" ] "duplicate rule ids are removed"
            Expect.equal selected.RequiredGates [ "PackageSurfaceCheck"; "EvidenceGraph"; "EvidenceAudit" ] "duplicate gates are removed"
            Expect.equal selected.Authority AgentReadyAuthority "authority is preserved"
            Expect.equal effects [ WriteValidationSelectionReport ] "selection emits report effect"
        }

        test "ValidationContract parser accepts real contract and reports governance diagnostics" {
            match ValidationContract.parse (contractText ()) with
            | ValidationContractAccepted contract ->
                Expect.equal contract.SchemaVersion 1 "schema version is parsed"
                Expect.isGreaterThan contract.RoutingRules.Length 5 "representative routing rules are parsed"
                Expect.equal contract.Defaults.BroadFallbackCommand "./fake.sh build -t Verify" "broad fallback command is parsed"
            | ValidationContractRejected diagnostics ->
                failtestf "real validation contract should parse without diagnostics: %A" diagnostics

            let invalid =
                """
schema_version: 1
defaults:
  broad_fallback_command: "./fake.sh build -t Verify"
tiers:
  agent-ready:
    authority: focused-authoritative
routing_rules:
  - id: duplicate-rule
    paths:
      - "../bad"
    required_gates:
      - MissingGate
    timeout_class: focused
    failure_owner: governance
  - id: duplicate-rule
    paths:
      - "src/Controls/**"
    required_gates:
      - EvidenceGraph
    timeout_class: focused
    failure_owner: governance
"""

            match ValidationContract.parse invalid with
            | ValidationContractAccepted _ -> failtest "invalid contract should be rejected"
            | ValidationContractRejected diagnostics ->
                let codes = diagnostics |> List.map _.Code
                Expect.contains codes "duplicate-rule-id" "duplicate rule ids are rejected"
                Expect.contains codes "unknown-gate" "unknown gates are rejected"
                Expect.contains codes "invalid-path-pattern" "invalid paths are rejected"
                Expect.isTrue (diagnostics |> List.forall (fun diagnostic -> diagnostic.Owner = "governance")) "diagnostics are governance-owned"
        }

        test "ValidationSelection pure rule selection unions gates and rule ids" {
            match ValidationContract.parse (contractText ()) with
            | ValidationContractRejected diagnostics ->
                failtestf "real validation contract should parse before rule selection: %A" diagnostics
            | ValidationContractAccepted contract ->
                let ruleIds, gates, authority =
                    ValidationSelection.selectRules
                        [ "src/Controls/Types.fsi"
                          "template/fragments/product/skill/SKILL.md"
                          "build.fsx" ]
                        contract

                Expect.contains ruleIds "controls-public-surface" "controls changes select controls rule"
                Expect.contains ruleIds "generated-guidance" "skill guidance changes select generated guidance rule"
                Expect.contains ruleIds "build-target-contract" "build target changes select build contract rule"
                Expect.contains gates "PackageSurfaceCheck" "controls gates are included"
                Expect.contains gates "GeneratedGuidanceCheck" "generated guidance gate is included"
                Expect.contains gates "EvidenceAudit" "build contract gate is included"
                Expect.equal gates (gates |> List.distinct) "selected gates are de-duplicated"
                Expect.equal authority AgentReadyAuthority "rule selection resolves agent-ready authority"
        }

        test "ValidationSelectionInterpreter reads real feature metadata loads contract and writes report" {
            withTempDirectory (fun temp ->
                let featureMetadataPath = Path.Combine(temp, ".specify", "feature.json")
                let contractPath = Path.Combine(temp, "validation.contract.yml")
                let reportPath = Path.Combine(temp, "readiness", "validation-selection.md")

                Path.GetDirectoryName featureMetadataPath
                |> Option.ofObj
                |> Option.iter (Directory.CreateDirectory >> ignore)
                File.WriteAllText(
                    featureMetadataPath,
                    """{"feature_directory":"specs/028-agent-validation-framework","changed_paths":["src/Controls/Types.fsi","build.fsx"]}""")
                File.WriteAllText(contractPath, contractText ())

                let inputs =
                    { RepositoryRoot = temp
                      FeatureMetadataPath = featureMetadataPath
                      ValidationContractPath = contractPath
                      SelectionReportPath = reportPath
                      BaseRef = None }

                match ValidationSelectionInterpreter.readActiveFeatureMetadata inputs with
                | Error diagnostic -> failtest diagnostic
                | Ok(feature, paths) ->
                    Expect.equal feature "028-agent-validation-framework" "feature id is read from active feature metadata"
                    Expect.contains paths "src/Controls/Types.fsi" "changed paths are read from active feature metadata"

                    let source =
                        { Kind = ActiveFeatureMetadata
                          Feature = Some feature
                          MergeBase = None
                          Paths = paths
                          Diagnostics = [] }

                    match ValidationSelectionInterpreter.loadValidationContract inputs source with
                    | Error diagnostic -> failtest diagnostic
                    | Ok(ruleIds, gates, authority) ->
                        Expect.contains ruleIds "controls-public-surface" "real contract selection includes controls rule"
                        Expect.contains ruleIds "build-target-contract" "real contract selection includes build rule"
                        Expect.contains gates "PackageSurfaceCheck" "real contract selection includes focused gate"
                        Expect.equal authority AgentReadyAuthority "real contract selection reports agent-ready authority"

                        let model =
                            { Feature = Some feature
                              ChangedPathSource = Some source
                              SelectedRuleIds = ruleIds
                              RequiredGates = gates
                              Authority = authority
                              Degraded = false
                              Diagnostics = [] }

                        match ValidationSelectionInterpreter.writeSelectionReport inputs model with
                        | Error diagnostic -> failtest diagnostic
                        | Ok report ->
                            Expect.isTrue (File.Exists reportPath) "selection report is written to the real filesystem"
                            Expect.stringContains report "controls-public-surface" "report includes selected rules"
                            Expect.stringContains report "PackageSurfaceCheck" "report includes selected gates")
        }

        test "ValidationSelectionInterpreter executes real git merge-base diff fallback" {
            withTempDirectory (fun temp ->
                runInDirectory temp "git" [ "init"; "-b"; "main" ] |> ignore
                runInDirectory temp "git" [ "config"; "user.email"; "agent-validation@example.invalid" ] |> ignore
                runInDirectory temp "git" [ "config"; "user.name"; "Agent Validation" ] |> ignore

                Directory.CreateDirectory(Path.Combine(temp, "src", "Controls")) |> ignore
                File.WriteAllText(Path.Combine(temp, "README.md"), "baseline")
                runInDirectory temp "git" [ "add"; "." ] |> ignore
                runInDirectory temp "git" [ "commit"; "-m"; "baseline" ] |> ignore
                let baseCommit = runInDirectory temp "git" [ "rev-parse"; "HEAD" ]

                runInDirectory temp "git" [ "checkout"; "-b"; "028-agent-validation-framework" ] |> ignore
                File.WriteAllText(Path.Combine(temp, "src", "Controls", "Types.fsi"), "namespace Temp")
                runInDirectory temp "git" [ "add"; "." ] |> ignore
                runInDirectory temp "git" [ "commit"; "-m"; "controls change" ] |> ignore

                let inputs =
                    { RepositoryRoot = temp
                      FeatureMetadataPath = Path.Combine(temp, ".specify", "feature.json")
                      ValidationContractPath = Path.Combine(temp, "validation.contract.yml")
                      SelectionReportPath = Path.Combine(temp, "readiness", "validation-selection.md")
                      BaseRef = Some "main" }

                match ValidationSelectionInterpreter.runGitMergeBaseDiff inputs with
                | Error diagnostic -> failtest diagnostic
                | Ok(mergeBase, paths) ->
                    Expect.equal mergeBase (baseCommit.Trim()) "merge-base comes from the real git repository"
                    Expect.equal paths [ "src/Controls/Types.fsi" ] "git fallback returns changed paths from merge-base diff")
        }

        test "Synthetic malformed routing fixtures reject unsafe contract inputs" {
            // SYNTHETIC: malformed YAML, duplicate ids, unknown targets, and invalid path patterns are approved SEH fixtures; real success routing evidence is readiness/validation-contract.md.
            let source = buildText ()

            [ "malformed-validation-contract.yml"
              "unknown-target.fixture.yml"
              "duplicate-rule.fixture.yml"
              "invalid-path-pattern.fixture.yml"
              "ValidationContractRejected"
              "governance-owned diagnostics"
              "no success verdict" ]
            |> List.iter (fun required ->
                Expect.stringContains source required $"synthetic malformed routing fixture preserves {required}")
        }
    ]

let private sampleSource =
    { Kind = GitMergeBaseDiff
      Feature = Some "028-agent-validation-framework"
      MergeBase = Some "abc123"
      Paths = [ "src/Lib/AgentValidation.fsi" ]
      Diagnostics = [] }

let private verdictFor gateResults requiredGates =
    AgentVerdict.aggregate
        "./fake.sh build -t Verify"
        sampleSource
        [ "package-surface" ]
        requiredGates
        gateResults
        [ "readiness/agent-verdict.md" ]
        (DateTimeOffset.Parse("2026-05-28T12:00:00Z"))

[<Tests>]
let agentValidationUs2Tests =
    testList "US2 agent verdict" [
        test "AgentVerdict aggregate classifies passed failed unsupported degraded stale and missing evidence outcomes" {
            let passed =
                verdictFor [ { Gate = "PackageSurfaceCheck"; Outcome = GatePassed; Artifacts = [ "readiness/package.md" ] } ] [ "PackageSurfaceCheck" ]

            Expect.equal passed.Status Passed "all required gates passed"
            Expect.equal passed.MissingGates [] "passed verdict has no missing gates"

            let failed =
                verdictFor
                    [ { Gate = "PackageSurfaceCheck"
                        Outcome = GateFailed(Product, ProductFailure, "surface mismatch")
                        Artifacts = [] } ]
                    [ "PackageSurfaceCheck" ]

            Expect.equal failed.Status Failed "product gate failure fails verdict"
            Expect.equal failed.FailureOwner Product "product gate failure owns verdict"

            let unsupported =
                verdictFor
                    [ { Gate = "GeneratedProductCheck"
                        Outcome = GateUnsupportedHost "desktop prerequisites unavailable"
                        Artifacts = [] } ]
                    [ "GeneratedProductCheck" ]

            Expect.equal unsupported.Status Unsupported "unsupported host is explicit"
            Expect.equal unsupported.FailureClass UnsupportedHostFailure "unsupported host failure class is explicit"

            let stale =
                verdictFor
                    [ { Gate = "TemplateCheck"
                        Outcome = GateStalePrerequisite "template package is stale"
                        Artifacts = [] } ]
                    [ "TemplateCheck" ]

            Expect.equal stale.Status Degraded "stale prerequisite degrades verdict"
            Expect.equal stale.NextCommand (Some "./fake.sh build -t Verify") "degraded verdict names broad fallback"

            let missing =
                verdictFor
                    [ { Gate = "EvidenceAudit"
                        Outcome = GateMissingEvidence "readiness/evidence-audit.md missing"
                        Artifacts = [] } ]
                    [ "EvidenceAudit" ]

            Expect.equal missing.Status Failed "missing evidence fails verdict"
            Expect.equal missing.FailureOwner MissingEvidence "missing evidence owns verdict"
        }

        test "AgentVerdict aggregate calculates completed missing gates next command and diagnostics" {
            let verdict =
                verdictFor
                    [ { Gate = "EvidenceGraph"; Outcome = GatePassed; Artifacts = [ "readiness/evidence-graph.md" ] } ]
                    [ "EvidenceGraph"; "EvidenceAudit" ]

            Expect.equal verdict.CompletedGates [ "EvidenceGraph" ] "completed gates come from gate results"
            Expect.equal verdict.MissingGates [ "EvidenceAudit" ] "missing gates are required minus completed"
            Expect.equal verdict.Status Degraded "missing required gate degrades the handoff"
            Expect.equal verdict.NextCommand (Some "./fake.sh build -t Verify") "missing gates select broad fallback"
            Expect.contains verdict.Artifacts "readiness/evidence-graph.md" "gate artifacts are preserved"
        }

        test "Synthetic forced error-result fixtures classify environment unsupported stale and missing-evidence paths" {
            // SYNTHETIC: forced gate outcomes are approved SEH fixtures for rare environment/prerequisite states; real command evidence is recorded in readiness/environment-failure-classification.md.
            let cases =
                [ GateFailed(Environment, EnvironmentFailure, "process launch failed"), EnvironmentFailure
                  GateUnsupportedHost "unsupported desktop host", UnsupportedHostFailure
                  GateStalePrerequisite "stale local package", StalePrerequisiteFailure
                  GateMissingEvidence "missing report", MissingEvidenceFailure ]

            cases
            |> List.iteri (fun index (outcome, expectedClass) ->
                let verdict = verdictFor [ { Gate = $"Gate{index}"; Outcome = outcome; Artifacts = [] } ] [ $"Gate{index}" ]
                Expect.equal verdict.FailureClass expectedClass $"synthetic forced outcome {index} maps to expected class")
        }

        test "AgentVerdict JSON and Markdown expose authoritative handoff fields" {
            let verdict =
                verdictFor [ { Gate = "PackageSurfaceCheck"; Outcome = GatePassed; Artifacts = [] } ] [ "PackageSurfaceCheck" ]

            let json = AgentVerdict.toJson verdict
            use document = JsonDocument.Parse json
            let root = document.RootElement

            Expect.equal (root.GetProperty("status").GetString()) "passed" "JSON status is contract casing"
            Expect.equal (root.GetProperty("authority").GetString()) "focused-authoritative" "JSON authority is contract wording"
            Expect.equal (root.GetProperty("changed_path_source").GetProperty("kind").GetString()) "git-merge-base-diff" "JSON changed path source is nested"
            Expect.equal (root.GetProperty("failure_class").ValueKind) JsonValueKind.Null "passed verdict has null failure class"
            Expect.equal (root.GetProperty("required_gates").GetArrayLength()) 1 "required gates are an array"

            let markdown = AgentVerdict.toMarkdown verdict
            Expect.stringContains markdown "focused-authoritative" "Markdown does not claim stronger authority"
            Expect.stringContains markdown "PackageSurfaceCheck" "Markdown names required gates"
        }
    ]
