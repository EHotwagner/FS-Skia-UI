namespace FS.Skia.UI.Build.AgentValidation

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Text.Json

type ValidationGate = string
type ValidationRuleId = string
type FeatureId = string
type ChangedPath = string

type ChangedPathSourceKind =
    | ActiveFeatureMetadata
    | GitMergeBaseDiff
    | Unavailable

type ChangedPathSource =
    { Kind: ChangedPathSourceKind
      Feature: FeatureId option
      MergeBase: string option
      Paths: ChangedPath list
      Diagnostics: string list }

type ValidationAuthority =
    | InnerLoop
    | FocusedAuthority
    | AgentReadyAuthority
    | MaintainerVerify
    | AutomationFinal

type ValidationSelectionModel =
    { Feature: FeatureId option
      ChangedPathSource: ChangedPathSource option
      SelectedRuleIds: ValidationRuleId list
      RequiredGates: ValidationGate list
      Authority: ValidationAuthority
      Degraded: bool
      Diagnostics: string list }

type ValidationSelectionMsg =
    | LoadActiveFeatureMetadata
    | ActiveFeatureMetadataLoaded of FeatureId * ChangedPath list
    | ActiveFeatureMetadataUnavailable of string
    | LoadGitMergeBaseDiff
    | GitMergeBaseDiffLoaded of mergeBase: string * paths: ChangedPath list
    | GitMergeBaseDiffUnavailable of string
    | ContractLoaded of selectedRuleIds: ValidationRuleId list * requiredGates: ValidationGate list * authority: ValidationAuthority
    | SelectionFailed of string

type ValidationSelectionEffect =
    | ReadActiveFeatureMetadata
    | RunGitMergeBaseDiff
    | LoadValidationContract
    | WriteValidationSelectionReport

type ValidationSelectionInterpreterInputs =
    { RepositoryRoot: string
      FeatureMetadataPath: string
      ValidationContractPath: string
      SelectionReportPath: string
      BaseRef: string option }

type ValidationContractDefaults =
    { BroadFallbackCommand: string
      FinalGates: ValidationGate list }

type ValidationContractTier =
    { Id: string
      Authority: string
      DefaultGates: ValidationGate list }

type ValidationContractRule =
    { Id: ValidationRuleId
      Paths: string list
      FeatureConcerns: string list
      RequiredGates: ValidationGate list
      ExpectedArtifacts: string list
      TimeoutClass: string
      FailureOwner: string }

type ValidationContract =
    { SchemaVersion: int
      Defaults: ValidationContractDefaults
      Tiers: ValidationContractTier list
      RoutingRules: ValidationContractRule list }

type ValidationContractDiagnostic =
    { Code: string
      Path: string
      Message: string
      Owner: string }

type ValidationContractParseResult =
    | ValidationContractAccepted of ValidationContract
    | ValidationContractRejected of ValidationContractDiagnostic list

type ValidationFailureOwner =
    | Product
    | Template
    | Governance
    | Environment
    | UnsupportedHost
    | StalePrerequisite
    | MissingEvidence
    | Unknown

type ValidationFailureClass =
    | ProductFailure
    | TemplateFailure
    | GovernanceFailure
    | EnvironmentFailure
    | UnsupportedHostFailure
    | StalePrerequisiteFailure
    | MissingEvidenceFailure
    | UnknownFailure

type TimeoutClass =
    | Fast
    | Focused
    | Broad

type ValidationCost =
    | Low
    | Medium
    | High

type TargetMetadata =
    { Name: ValidationGate
      Description: string
      Dependencies: ValidationGate list
      DirectPrerequisites: ValidationGate list
      ExpectedOutputs: string list
      StaleAssumptions: string list
      TimeoutClass: TimeoutClass
      Cost: ValidationCost
      Authority: ValidationAuthority
      DefaultFailureOwner: ValidationFailureOwner
      Command: string }

type AgentVerdictStatus =
    | Passed
    | Failed
    | Unsupported
    | Degraded

type AgentVerdict =
    { Status: AgentVerdictStatus
      Authority: ValidationAuthority
      ChangedPathSource: ChangedPathSource
      SelectedRuleIds: ValidationRuleId list
      RequiredGates: ValidationGate list
      CompletedGates: ValidationGate list
      MissingGates: ValidationGate list
      SkippedGates: ValidationGate list
      MissingArtifacts: string list
      FailureOwner: ValidationFailureOwner
      FailureClass: ValidationFailureClass
      NextCommand: string option
      Artifacts: string list
      Diagnostics: string list
      TimestampUtc: System.DateTimeOffset }

type ValidationGateOutcome =
    | GatePassed
    | GateFailed of ValidationFailureOwner * ValidationFailureClass * diagnostic: string
    | GateUnsupportedHost of diagnostic: string
    | GateStalePrerequisite of diagnostic: string
    | GateMissingEvidence of diagnostic: string

type ValidationGateResult =
    { Gate: ValidationGate
      Outcome: ValidationGateOutcome
      Artifacts: string list }

module AgentVerdict =
    let private csv values =
        values |> String.concat ","

    let private distinct values =
        values |> List.distinct

    let private escapeJson (value: string) =
        use document = JsonDocument.Parse($"{{\"value\":{JsonSerializer.Serialize value}}}")
        document.RootElement.GetProperty("value").GetRawText()

    let private jsonArray values =
        values |> List.map escapeJson |> String.concat "," |> fun body -> "[" + body + "]"

    let private statusName status =
        match status with
        | Passed -> "passed"
        | Failed -> "failed"
        | Unsupported -> "unsupported"
        | Degraded -> "degraded"

    let private authorityName authority =
        match authority with
        | InnerLoop -> "inner-loop"
        | FocusedAuthority -> "focused-authority"
        | AgentReadyAuthority -> "focused-authoritative"
        | MaintainerVerify -> "maintainer-verify"
        | AutomationFinal -> "automation-final"

    let private sourceKindName kind =
        match kind with
        | ActiveFeatureMetadata -> "active-feature-metadata"
        | GitMergeBaseDiff -> "git-merge-base-diff"
        | Unavailable -> "unavailable"

    let private ownerName owner =
        match owner with
        | Product -> "product"
        | Template -> "template"
        | Governance -> "governance"
        | Environment -> "environment"
        | UnsupportedHost -> "unsupported-host"
        | StalePrerequisite -> "stale-prerequisite"
        | MissingEvidence -> "missing-evidence"
        | Unknown -> "unknown"

    let private failureClassName failureClass =
        match failureClass with
        | ProductFailure -> "product"
        | TemplateFailure -> "template"
        | GovernanceFailure -> "governance"
        | EnvironmentFailure -> "environment"
        | UnsupportedHostFailure -> "unsupported-host"
        | StalePrerequisiteFailure -> "stale-prerequisite"
        | MissingEvidenceFailure -> "missing-evidence"
        | UnknownFailure -> "unknown"

    let private appendJsonField (builder: StringBuilder) (name: string) (value: string) (isLast: bool) =
        builder.Append("  \"").Append(name).Append("\": ").Append(value) |> ignore

        if not isLast then
            builder.Append(",") |> ignore

        builder.AppendLine() |> ignore

    let private outcomeFailure outcome =
        match outcome with
        | GatePassed -> None
        | GateFailed(owner, failureClass, diagnostic) -> Some(Failed, owner, failureClass, diagnostic, None)
        | GateUnsupportedHost diagnostic -> Some(Unsupported, UnsupportedHost, UnsupportedHostFailure, diagnostic, None)
        | GateStalePrerequisite diagnostic -> Some(Degraded, StalePrerequisite, StalePrerequisiteFailure, diagnostic, None)
        | GateMissingEvidence diagnostic -> Some(Failed, MissingEvidence, MissingEvidenceFailure, diagnostic, None)

    let aggregate broadFallbackCommand changedPathSource selectedRuleIds requiredGates gateResults artifacts timestampUtc =
        let completedGates =
            gateResults |> List.map _.Gate |> distinct

        let missingGates =
            requiredGates
            |> List.filter (fun gate -> completedGates |> List.contains gate |> not)
            |> distinct

        let failures =
            gateResults
            |> List.choose (fun result ->
                outcomeFailure result.Outcome
                |> Option.map (fun failure -> result.Gate, failure))

        let status, owner, failureClass, nextCommand =
            match failures with
            | (gate, (Unsupported, owner, failureClass, diagnostic, _)) :: _ ->
                Unsupported, owner, failureClass, Some broadFallbackCommand
            | (gate, (Degraded, owner, failureClass, diagnostic, _)) :: _ ->
                Degraded, owner, failureClass, Some broadFallbackCommand
            | (gate, (Failed, owner, failureClass, diagnostic, _)) :: _ ->
                Failed, owner, failureClass, None
            | _ when not (List.isEmpty missingGates) ->
                Degraded, MissingEvidence, MissingEvidenceFailure, Some broadFallbackCommand
            | _ ->
                Passed, Unknown, UnknownFailure, None

        let diagnostics =
            gateResults
            |> List.choose (fun result ->
                match result.Outcome with
                | GatePassed -> None
                | GateFailed(_, _, diagnostic)
                | GateUnsupportedHost diagnostic
                | GateStalePrerequisite diagnostic
                | GateMissingEvidence diagnostic -> Some($"{result.Gate}: {diagnostic}"))

        { Status = status
          Authority = AgentReadyAuthority
          ChangedPathSource = changedPathSource
          SelectedRuleIds = distinct selectedRuleIds
          RequiredGates = distinct requiredGates
          CompletedGates = completedGates
          MissingGates = missingGates
          SkippedGates = []
          MissingArtifacts = if status = Failed && owner = MissingEvidence then diagnostics else []
          FailureOwner = owner
          FailureClass = failureClass
          NextCommand = nextCommand
          Artifacts = distinct (artifacts @ (gateResults |> List.collect _.Artifacts))
          Diagnostics = diagnostics
          TimestampUtc = timestampUtc }

    let toJson verdict =
        let builder = StringBuilder()
        builder.AppendLine("{") |> ignore

        let sourceFields =
            [ "\"kind\": " + escapeJson (sourceKindName verdict.ChangedPathSource.Kind)
              "\"feature\": "
              + (verdict.ChangedPathSource.Feature |> Option.map escapeJson |> Option.defaultValue "null")
              "\"merge_base\": "
              + (verdict.ChangedPathSource.MergeBase |> Option.map escapeJson |> Option.defaultValue "null")
              "\"paths\": " + jsonArray verdict.ChangedPathSource.Paths
              "\"diagnostics\": " + jsonArray verdict.ChangedPathSource.Diagnostics ]
            |> String.concat ", "

        appendJsonField builder "status" (escapeJson (statusName verdict.Status)) false
        appendJsonField builder "authority" (escapeJson (authorityName verdict.Authority)) false
        appendJsonField builder "changed_path_source" ("{ " + sourceFields + " }") false
        appendJsonField builder "selected_rule_ids" (jsonArray verdict.SelectedRuleIds) false
        appendJsonField builder "required_gates" (jsonArray verdict.RequiredGates) false
        appendJsonField builder "completed_gates" (jsonArray verdict.CompletedGates) false
        appendJsonField builder "missing_gates" (jsonArray verdict.MissingGates) false
        appendJsonField builder "skipped_gates" (jsonArray verdict.SkippedGates) false
        appendJsonField builder "missing_artifacts" (jsonArray verdict.MissingArtifacts) false
        appendJsonField builder "failure_owner" (escapeJson (ownerName verdict.FailureOwner)) false

        let failureClass =
            if verdict.Status = Passed then
                "null"
            else
                escapeJson (failureClassName verdict.FailureClass)

        appendJsonField builder "failure_class" failureClass false
        appendJsonField builder "next_command" (verdict.NextCommand |> Option.map escapeJson |> Option.defaultValue "null") false
        appendJsonField builder "artifacts" (jsonArray verdict.Artifacts) false
        appendJsonField builder "diagnostics" (jsonArray verdict.Diagnostics) false
        appendJsonField builder "timestamp_utc" (escapeJson (verdict.TimestampUtc.ToUniversalTime().ToString("O"))) true
        builder.Append("}") |> ignore
        builder.ToString()

    let toMarkdown verdict =
        let nextCommand = verdict.NextCommand |> Option.defaultValue "none"
        let failureClass = if verdict.Status = Passed then "none" else failureClassName verdict.FailureClass

        [ "# AgentReady Verdict"
          ""
          $"- status: `{statusName verdict.Status}`"
          $"- authority: `{authorityName verdict.Authority}`"
          $"- changed-path-source: `{sourceKindName verdict.ChangedPathSource.Kind}`"
          $"- required-gates: `{csv verdict.RequiredGates}`"
          $"- completed-gates: `{csv verdict.CompletedGates}`"
          $"- missing-gates: `{csv verdict.MissingGates}`"
          $"- failure-owner: `{ownerName verdict.FailureOwner}`"
          $"- failure-class: `{failureClass}`"
          $"- next-command: `{nextCommand}`" ]
        |> String.concat System.Environment.NewLine

module ValidationContract =
    let knownGates =
        [ "AgentReady"
          "ControlsCatalogCheck"
          "ControlsInteractionCheck"
          "ControlsRenderingCheck"
          "Dev"
          "EvidenceAudit"
          "EvidenceGraph"
          "FsiTranscripts"
          "GeneratedGuidanceCheck"
          "GeneratedProductCheck"
          "PackageSurfaceCheck"
          "PerPackageSurfaceDiff"
          "TargetMetadataDrift"
          "TemplateCheck"
          "TemplateDrift"
          "Verify"
          "Ci" ]

    let private diagnostic code path message =
        { Code = code
          Path = path
          Message = message
          Owner = "governance" }

    let private contains (needle: string) (text: string) =
        text.Contains(needle, StringComparison.Ordinal)

    let private valuesAfter (label: string) (lines: string list) =
        let rec collect (acc: string list) (remaining: string list) =
            match remaining with
            | line :: rest when line.TrimStart().StartsWith("-", StringComparison.Ordinal) ->
                collect (line.Trim().TrimStart('-').Trim().Trim('"') :: acc) rest
            | _ -> List.rev acc

        lines
        |> List.tryFindIndex (fun line -> line.Trim() = $"{label}:")
        |> Option.map (fun index -> lines |> List.skip (index + 1) |> collect [])
        |> Option.defaultValue []

    let private firstScalar (label: string) (lines: string list) =
        lines
        |> List.tryPick (fun line ->
            let trimmed = line.Trim()
            let prefix = $"{label}:"

            if trimmed.StartsWith(prefix, StringComparison.Ordinal) then
                Some(trimmed.Substring(prefix.Length).Trim().Trim('"'))
            else
                None)

    let private ruleIds (lines: string list) =
        lines
        |> List.choose (fun line ->
            let trimmed = line.Trim()

            if trimmed.StartsWith("- id:", StringComparison.Ordinal) then
                Some(trimmed.Substring(5).Trim().Trim('"'))
            else
                None)

    let private ruleBlocks (lines: string list) =
        let rec collect (blocks: string list list) (current: string list) (remaining: string list) =
            match remaining with
            | [] ->
                match current with
                | [] -> List.rev blocks
                | _ -> List.rev (List.rev current :: blocks)
            | line :: rest when line.Trim().StartsWith("- id:", StringComparison.Ordinal) ->
                let blocks =
                    match current with
                    | [] -> blocks
                    | _ -> List.rev current :: blocks

                collect blocks [ line ] rest
            | line :: rest ->
                match current with
                | [] -> collect blocks current rest
                | _ -> collect blocks (line :: current) rest

        lines
        |> List.skipWhile (fun line -> line.Trim() <> "routing_rules:")
        |> collect [] []

    let private parseRuleBlock (block: string list) =
        let id =
            block
            |> List.tryPick (fun line ->
                let trimmed = line.Trim()

                if trimmed.StartsWith("- id:", StringComparison.Ordinal) then
                    Some(trimmed.Substring(5).Trim().Trim('"'))
                else
                    None)
            |> Option.defaultValue ""

        { Id = id
          Paths = valuesAfter "paths" block
          FeatureConcerns = valuesAfter "feature_concerns" block
          RequiredGates = valuesAfter "required_gates" block
          ExpectedArtifacts = valuesAfter "expected_artifacts" block
          TimeoutClass = firstScalar "timeout_class" block |> Option.defaultValue "focused"
          FailureOwner = firstScalar "failure_owner" block |> Option.defaultValue "governance" }

    let private schemaDiagnostics (text: string) (lines: string list) =
        let rules = ruleBlocks lines |> List.map parseRuleBlock

        let duplicateIds =
            rules
            |> List.map _.Id
            |> List.countBy id
            |> List.choose (fun (ruleId, count) ->
                if count > 1 then
                    Some(diagnostic "duplicate-rule-id" "routing_rules" $"duplicate rule id {ruleId}")
                else
                    None)

        let unknownGates =
            rules
            |> List.collect _.RequiredGates
            |> List.choose (fun gate ->
                if knownGates |> List.contains gate then
                    None
                else
                    Some(diagnostic "unknown-gate" "routing_rules.required_gates" $"unknown gate {gate}; no success verdict can be emitted"))

        let invalidPatterns =
            rules
            |> List.collect _.Paths
            |> List.choose (fun pattern ->
                if String.IsNullOrWhiteSpace pattern || pattern.StartsWith("/", StringComparison.Ordinal) || pattern.Contains("..") then
                    Some(diagnostic "invalid-path-pattern" "routing_rules.paths" $"invalid path pattern {pattern}")
                else
                    None)

        let requiredShape =
            [ "schema_version:", "schema_version"
              "defaults:", "defaults"
              "broad_fallback_command:", "defaults.broad_fallback_command"
              "tiers:", "tiers"
              "routing_rules:", "routing_rules" ]
            |> List.choose (fun (needle, path) ->
                if contains needle text then
                    None
                else
                    Some(diagnostic "malformed-field" path $"missing required field {path}"))

        requiredShape @ duplicateIds @ unknownGates @ invalidPatterns

    let parse (text: string) =
        let lines =
            text.Split([| "\r\n"; "\n" |], StringSplitOptions.None)
            |> Array.toList

        match schemaDiagnostics text lines with
        | diagnostics when diagnostics.Length > 0 -> ValidationContractRejected diagnostics
        | [] ->
            let version =
                firstScalar "schema_version" lines
                |> Option.bind (fun (value: string) ->
                    match Int32.TryParse value with
                    | true, parsed -> Some parsed
                    | false, _ -> None)
                |> Option.defaultValue 1

            let fallback =
                firstScalar "broad_fallback_command" lines
                |> Option.defaultValue "./fake.sh build -t Verify"

            let rules =
                ruleBlocks lines
                |> List.map parseRuleBlock

            ValidationContractAccepted
                { SchemaVersion = version
                  Defaults =
                    { BroadFallbackCommand = fallback
                      FinalGates = valuesAfter "final_gates" lines }
                  Tiers = []
                  RoutingRules = rules }
        | diagnostics -> ValidationContractRejected diagnostics

module ValidationSelection =
    let private matchesPattern (pattern: string) (path: string) =
        if pattern.EndsWith("/**", StringComparison.Ordinal) then
            let prefix = pattern.Substring(0, pattern.Length - 3)
            path.StartsWith(prefix, StringComparison.Ordinal)
        elif pattern.Contains("/**/*.", StringComparison.Ordinal) then
            let marker = "/**/*."
            let markerIndex = pattern.IndexOf(marker, StringComparison.Ordinal)
            let prefix = pattern.Substring(0, markerIndex)
            let extension = "." + pattern.Substring(markerIndex + marker.Length)
            path.StartsWith(prefix, StringComparison.Ordinal) && path.EndsWith(extension, StringComparison.Ordinal)
        elif pattern.Contains("**", StringComparison.Ordinal) then
            let prefix = pattern.Substring(0, pattern.IndexOf("**", StringComparison.Ordinal))
            path.StartsWith(prefix, StringComparison.Ordinal)
        else
            String.Equals(pattern, path, StringComparison.Ordinal)

    let selectRules changedPaths contract =
        let selected =
            contract.RoutingRules
            |> List.filter (fun rule ->
                changedPaths
                |> List.exists (fun changedPath ->
                    rule.Paths |> List.exists (fun pattern -> matchesPattern pattern changedPath)))

        let ruleIds = selected |> List.map _.Id |> List.distinct
        let gates = selected |> List.collect _.RequiredGates |> List.distinct

        ruleIds, gates, AgentReadyAuthority

    let init feature =
        { Feature = feature
          ChangedPathSource = None
          SelectedRuleIds = []
          RequiredGates = []
          Authority = AgentReadyAuthority
          Degraded = false
          Diagnostics = [] },
        [ ReadActiveFeatureMetadata ]

    let update msg model =
        match msg with
        | LoadActiveFeatureMetadata ->
            model, [ ReadActiveFeatureMetadata ]
        | ActiveFeatureMetadataLoaded(feature, paths) ->
            { model with
                Feature = Some feature
                ChangedPathSource =
                    Some
                        { Kind = ActiveFeatureMetadata
                          Feature = Some feature
                          MergeBase = None
                          Paths = paths
                          Diagnostics = [] } },
            [ LoadValidationContract ]
        | ActiveFeatureMetadataUnavailable diagnostic ->
            { model with Diagnostics = model.Diagnostics @ [ diagnostic ] }, [ RunGitMergeBaseDiff ]
        | LoadGitMergeBaseDiff ->
            model, [ RunGitMergeBaseDiff ]
        | GitMergeBaseDiffLoaded(mergeBase, paths) ->
            { model with
                ChangedPathSource =
                    Some
                        { Kind = GitMergeBaseDiff
                          Feature = model.Feature
                          MergeBase = Some mergeBase
                          Paths = paths
                          Diagnostics = [] } },
            [ LoadValidationContract ]
        | GitMergeBaseDiffUnavailable diagnostic ->
            { model with
                ChangedPathSource =
                    Some
                        { Kind = Unavailable
                          Feature = model.Feature
                          MergeBase = None
                          Paths = []
                          Diagnostics = [ diagnostic ] }
                Degraded = true
                Diagnostics = model.Diagnostics @ [ diagnostic ] },
            [ WriteValidationSelectionReport ]
        | ContractLoaded(ruleIds, gates, authority) ->
            { model with
                SelectedRuleIds = ruleIds |> List.distinct
                RequiredGates = gates |> List.distinct
                Authority = authority },
            [ WriteValidationSelectionReport ]
        | SelectionFailed diagnostic ->
            { model with
                Degraded = true
                Diagnostics = model.Diagnostics @ [ diagnostic ] },
            [ WriteValidationSelectionReport ]

module ValidationSelectionInterpreter =
    let private tryProperty (names: string list) (element: JsonElement) =
        names
        |> List.tryPick (fun name ->
            match element.TryGetProperty name with
            | true, property -> Some property
            | false, _ -> None)

    let private stringArray (property: JsonElement) =
        if property.ValueKind = JsonValueKind.Array then
            property.EnumerateArray()
            |> Seq.choose (fun item ->
                if item.ValueKind = JsonValueKind.String then
                    match item.GetString() |> Option.ofObj with
                    | Some value when not (String.IsNullOrWhiteSpace value) ->
                        Some(value.Replace('\\', '/'))
                    | _ ->
                        None
                else
                    None)
            |> Seq.toList
        else
            []

    let private featureIdFromDirectory (directory: string) =
        if String.IsNullOrWhiteSpace directory then
            ""
        else
            Path.GetFileName(directory.TrimEnd('/', '\\'))
            |> Option.ofObj
            |> Option.defaultValue ""

    let readActiveFeatureMetadata inputs =
        try
            if not (File.Exists inputs.FeatureMetadataPath) then
                Error $"active feature metadata not found at {inputs.FeatureMetadataPath}"
            else
                use document = JsonDocument.Parse(File.ReadAllText inputs.FeatureMetadataPath)
                let root = document.RootElement

                let feature =
                    root
                    |> tryProperty [ "feature_id"; "featureId"; "feature_directory"; "featureDirectory" ]
                    |> Option.bind (fun property ->
                        if property.ValueKind = JsonValueKind.String then
                            property.GetString() |> Option.ofObj
                        else
                            None)
                    |> Option.map featureIdFromDirectory
                    |> Option.defaultValue ""

                let paths =
                    root
                    |> tryProperty [ "changed_paths"; "changedPaths"; "paths" ]
                    |> Option.map stringArray
                    |> Option.defaultValue []

                if String.IsNullOrWhiteSpace feature then
                    Error "active feature metadata did not include a feature id"
                elif List.isEmpty paths then
                    Error $"active feature metadata for {feature} did not include changed paths"
                else
                    Ok(feature, paths)
        with ex ->
            Error $"active feature metadata read failed: {ex.Message}"

    let private runGit inputs (arguments: string list) =
        let startInfo = ProcessStartInfo("git")
        startInfo.WorkingDirectory <- inputs.RepositoryRoot
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        arguments |> List.iter startInfo.ArgumentList.Add

        match Process.Start startInfo |> Option.ofObj with
        | None ->
            Error "git process did not start"
        | Some proc ->
            use proc = proc
            let stdout = proc.StandardOutput.ReadToEnd()
            let stderr = proc.StandardError.ReadToEnd()

            if proc.WaitForExit(30000) && proc.ExitCode = 0 then
                Ok stdout
            else
                let diagnostic =
                    if String.IsNullOrWhiteSpace stderr then stdout else stderr

                Error(diagnostic.Trim())

    let runGitMergeBaseDiff inputs =
        let baseRef = inputs.BaseRef |> Option.defaultValue "main"

        match runGit inputs [ "merge-base"; baseRef; "HEAD" ] with
        | Error diagnostic -> Error $"git merge-base {baseRef} HEAD failed: {diagnostic}"
        | Ok mergeBaseText ->
            let mergeBase = mergeBaseText.Trim()

            if String.IsNullOrWhiteSpace mergeBase then
                Error $"git merge-base {baseRef} HEAD returned no commit"
            else
                match runGit inputs [ "diff"; "--name-only"; $"{mergeBase}...HEAD" ] with
                | Error diagnostic -> Error $"git diff {mergeBase}...HEAD failed: {diagnostic}"
                | Ok diffText ->
                    let paths =
                        diffText.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        |> Array.map (fun path -> path.Trim().Replace('\\', '/'))
                        |> Array.filter (String.IsNullOrWhiteSpace >> not)
                        |> Array.distinct
                        |> Array.toList

                    if List.isEmpty paths then
                        Error $"git diff {mergeBase}...HEAD returned no changed paths"
                    else
                        Ok(mergeBase, paths)

    let loadValidationContract (inputs: ValidationSelectionInterpreterInputs) (changedPathSource: ChangedPathSource) =
        try
            if not (File.Exists inputs.ValidationContractPath) then
                Error $"validation contract not found at {inputs.ValidationContractPath}"
            else
                match ValidationContract.parse (File.ReadAllText inputs.ValidationContractPath) with
                | ValidationContractRejected diagnostics ->
                    let message =
                        diagnostics
                        |> List.map (fun diagnostic -> $"{diagnostic.Code} at {diagnostic.Path}: {diagnostic.Message}")
                        |> String.concat "; "

                    Error $"validation contract rejected: {message}"
                | ValidationContractAccepted contract ->
                    Ok(ValidationSelection.selectRules changedPathSource.Paths contract)
        with ex ->
            Error $"validation contract load failed: {ex.Message}"

    let private csv values =
        if List.isEmpty values then
            "(none)"
        else
            values |> String.concat ", "

    let writeSelectionReport (inputs: ValidationSelectionInterpreterInputs) (model: ValidationSelectionModel) =
        try
            let source =
                match model.ChangedPathSource with
                | Some source -> $"%A{source.Kind}"
                | None -> "Unavailable"

            let paths =
                match model.ChangedPathSource with
                | Some source -> source.Paths
                | None -> []

            let feature = model.Feature |> Option.defaultValue "(none)"

            let content =
                [ "# Validation Selection Report"
                  ""
                  $"- feature: `{feature}`"
                  $"- changed-path-source: `{source}`"
                  $"- selected-rule-ids: `{csv model.SelectedRuleIds}`"
                  $"- required-gates: `{csv model.RequiredGates}`"
                  $"- authority: `%A{model.Authority}`"
                  $"- degraded: `{model.Degraded}`"
                  $"- changed-paths: `{csv paths}`"
                  $"- diagnostics: `{csv model.Diagnostics}`" ]
                |> String.concat Environment.NewLine

            let directory = Path.GetDirectoryName inputs.SelectionReportPath |> Option.ofObj

            match directory with
            | Some directory when not (String.IsNullOrWhiteSpace directory) ->
                Directory.CreateDirectory directory |> ignore
            | _ ->
                ()

            File.WriteAllText(inputs.SelectionReportPath, content)
            Ok content
        with ex ->
            Error $"validation selection report write failed: {ex.Message}"

    let interpret (inputs: ValidationSelectionInterpreterInputs) effect (model: ValidationSelectionModel) =
        match effect with
        | ReadActiveFeatureMetadata ->
            match readActiveFeatureMetadata inputs with
            | Ok(feature, paths) -> Some(ActiveFeatureMetadataLoaded(feature, paths))
            | Error diagnostic -> Some(ActiveFeatureMetadataUnavailable diagnostic)
        | RunGitMergeBaseDiff ->
            match runGitMergeBaseDiff inputs with
            | Ok(mergeBase, paths) -> Some(GitMergeBaseDiffLoaded(mergeBase, paths))
            | Error diagnostic -> Some(GitMergeBaseDiffUnavailable diagnostic)
        | LoadValidationContract ->
            match model.ChangedPathSource with
            | Some changedPathSource ->
                match loadValidationContract inputs changedPathSource with
                | Ok(ruleIds, gates, authority) -> Some(ContractLoaded(ruleIds, gates, authority))
                | Error diagnostic -> Some(SelectionFailed diagnostic)
            | None -> Some(SelectionFailed "validation contract cannot load before changed-path source is available")
        | WriteValidationSelectionReport ->
            match writeSelectionReport inputs model with
            | Ok _ -> None
            | Error diagnostic -> Some(SelectionFailed diagnostic)
