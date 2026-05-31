module FS.Skia.UI.Build.TargetMetadata

open System

type TargetMetadata =
    { RunnableTargetName: string
      DirectPrerequisites: string list
      ExpectedOutputs: string list
      StaleAssumptions: string list
      TimeoutClass: string
      Cost: string
      Authority: string
      FailureOwner: string
      Command: string }

type TargetMetadataDrift =
    | MissingRunnableTarget of string
    | MissingMetadata of string
    | MissingExpectedOutput of string
    | MissingFailureOwner of string
    | DependencyDivergence of string

let validateMetadataDrift runnableTargets metadata =
    let metadataByName =
        metadata
        |> List.map (fun row -> row.RunnableTargetName, row)
        |> Map.ofList

    [ for target in runnableTargets do
          if metadataByName.ContainsKey target |> not then
              yield MissingMetadata target

      for row in metadata do
          if runnableTargets |> List.contains row.RunnableTargetName |> not then
              yield MissingRunnableTarget row.RunnableTargetName

          if row.ExpectedOutputs.IsEmpty then
              yield MissingExpectedOutput row.RunnableTargetName

          if String.IsNullOrWhiteSpace row.FailureOwner then
              yield MissingFailureOwner row.RunnableTargetName

          if row.DirectPrerequisites |> List.exists String.IsNullOrWhiteSpace then
              yield DependencyDivergence row.RunnableTargetName
          else
              () ]

let driftDiagnostic drift =
    match drift with
    | MissingRunnableTarget target -> $"missing runnable target: {target}"
    | MissingMetadata target -> $"missing metadata: {target}"
    | MissingExpectedOutput target -> $"missing expected output: {target}"
    | MissingFailureOwner target -> $"missing failure owner: {target}"
    | DependencyDivergence target -> $"dependency divergence: {target}"

let validateAgainstRepo contractReferences docReferences runnableTargets metadata =
    let metadataNames = metadata |> List.map (fun row -> row.RunnableTargetName)
    let metadataDrift = validateMetadataDrift runnableTargets metadata |> List.map driftDiagnostic

    let contractDrift =
        contractReferences
        |> List.filter (fun target -> metadataNames |> List.contains target |> not)
        |> List.map (fun target -> $"validation contract references target without metadata: {target}")

    let docsDrift =
        docReferences
        |> List.filter (fun target -> metadataNames |> List.contains target |> not)
        |> List.map (fun target -> $"docs reference target without metadata: {target}")

    metadataDrift @ contractDrift @ docsDrift

let private jsonEscape (value: string) =
    value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n")

let private jsonString value =
    "\"" + jsonEscape value + "\""

let private jsonArray values =
    values
    |> List.map jsonString
    |> String.concat ", "
    |> fun value -> "[ " + value + " ]"

let metadataJson generatedAtUtc diagnostics (metadata: TargetMetadata list) =
    [ "{"
      $"  \"generated_at_utc\": {jsonString generatedAtUtc},"
      $"  \"diagnostics\": {jsonArray diagnostics},"
      "  \"targets\": ["
      yield!
          metadata
          |> List.mapi (fun index row ->
              let suffix = if index = metadata.Length - 1 then "" else ","
              [ "    {"
                $"      \"runnable_target_name\": {jsonString row.RunnableTargetName},"
                $"      \"direct_prerequisites\": {jsonArray row.DirectPrerequisites},"
                $"      \"expected_outputs\": {jsonArray row.ExpectedOutputs},"
                $"      \"stale_assumptions\": {jsonArray row.StaleAssumptions},"
                $"      \"timeout_class\": {jsonString row.TimeoutClass},"
                $"      \"cost\": {jsonString row.Cost},"
                $"      \"authority\": {jsonString row.Authority},"
                $"      \"failure_owner\": {jsonString row.FailureOwner},"
                $"      \"command\": {jsonString row.Command}"
                $"    }}{suffix}" ]
              |> String.concat Environment.NewLine)
      "  ]"
      "}" ]
    |> String.concat Environment.NewLine

let driftMarkdown (diagnostics: string list) =
    [ "# Target Metadata Drift"
      ""
      if diagnostics |> List.isEmpty then
          "PASS: runnable target registry, target metadata, validation contract target references, and docs are aligned."
      else
          "FAIL: target metadata drift was detected."
          ""
          yield! diagnostics |> List.map (fun diagnostic -> $"- {diagnostic}") ]
    |> String.concat Environment.NewLine
