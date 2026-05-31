module FS.Skia.UI.Build.Findings

open System

type ValidationFinding =
    { ArtifactClass: string
      Path: string
      Rule: string
      Message: string }

let finding artifactClass path rule message =
    { ArtifactClass = artifactClass
      Path = path
      Rule = rule
      Message = message }

let renderDetail (findings: ValidationFinding list) =
    findings
    |> List.map (fun finding -> $"- `{finding.Path}` [{finding.Rule}]: {finding.Message}")
    |> String.concat Environment.NewLine
