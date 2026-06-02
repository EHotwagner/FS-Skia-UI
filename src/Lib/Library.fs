namespace FS.Skia.UI

open System
open System.IO

type ParityStatus =
    | Supported
    | Adapted
    | Excluded
    | NotYetSupported

type EvidenceType =
    | SemanticTest
    | Screenshot
    | Smoke
    | Package
    | Documentation
    | ManualReview

type ParityEvidenceItem =
    { CapabilityId: string
      Capability: string
      Status: ParityStatus
      EvidenceType: EvidenceType
      Command: string
      Path: string
      AdaptationNotes: string
      ConflictsWithConstraints: bool }

type ParityReport =
    { Feature: string
      BaselineCommit: string
      Items: ParityEvidenceItem list }

module Parity =
    let baselineCommit = "7aac43dd12903f93004d0c2bf7c6254318a366dc"

    let capabilityIds =
        [ "core-viewer"
          "scene-dsl"
          "rendering-skia-translation"
          "shaders-effects"
          "screenshots"
          "performance-evidence"
          "charts"
          "datagrid"
          "layout"
          "graphs"
          "examples-demos"
          "documentation" ]

    let createItem capabilityId capability status evidenceType command path adaptationNotes conflictsWithConstraints =
        { CapabilityId = capabilityId
          Capability = capability
          Status = status
          EvidenceType = evidenceType
          Command = command
          Path = path
          AdaptationNotes = adaptationNotes
          ConflictsWithConstraints = conflictsWithConstraints }

    let createReport feature items =
        { Feature = feature
          BaselineCommit = baselineCommit
          Items = items }

    let validateMergeReady report =
        [ if report.BaselineCommit <> baselineCommit then
              $"baseline commit mismatch: {report.BaselineCommit}"

          let reportedIds =
              report.Items |> List.map _.CapabilityId |> Set.ofList

          for capabilityId in capabilityIds do
              if not (reportedIds.Contains capabilityId) then
                  $"missing capability: {capabilityId}"

          for item in report.Items do
              if item.Status = NotYetSupported && not item.ConflictsWithConstraints then
                  $"not merge-ready: {item.CapabilityId}"

              if String.IsNullOrWhiteSpace item.Command then
                  $"missing command: {item.CapabilityId}"

              if String.IsNullOrWhiteSpace item.Path then
                  $"missing evidence path: {item.CapabilityId}" ]

    let escapeJson (value: string) =
        value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")

    let statusText = function
        | Supported -> "Supported"
        | Adapted -> "Adapted"
        | Excluded -> "Excluded"
        | NotYetSupported -> "NotYetSupported"

    let evidenceText = function
        | SemanticTest -> "SemanticTest"
        | Screenshot -> "Screenshot"
        | Smoke -> "Smoke"
        | Package -> "Package"
        | Documentation -> "Documentation"
        | ManualReview -> "ManualReview"

    let itemJson item =
        [ "\"capabilityId\": \"" + escapeJson item.CapabilityId + "\""
          "\"capability\": \"" + escapeJson item.Capability + "\""
          "\"status\": \"" + statusText item.Status + "\""
          "\"evidenceType\": \"" + evidenceText item.EvidenceType + "\""
          "\"command\": \"" + escapeJson item.Command + "\""
          "\"path\": \"" + escapeJson item.Path + "\""
          "\"adaptationNotes\": \"" + escapeJson item.AdaptationNotes + "\""
          "\"conflictsWithConstraints\": " + item.ConflictsWithConstraints.ToString().ToLowerInvariant() ]
        |> String.concat ",\n      "
        |> fun body -> "    {\n      " + body + "\n    }"

    let toJson report =
        let itemsJson =
            report.Items
            |> List.map itemJson
            |> String.concat ",\n"

        "{\n"
        + $"  \"feature\": \"{escapeJson report.Feature}\",\n"
        + $"  \"baselineCommit\": \"{escapeJson report.BaselineCommit}\",\n"
        + "  \"items\": [\n"
        + itemsJson
        + "\n  ]\n"
        + "}\n"

    let writeJson (path: string) report =
        match System.IO.Path.GetDirectoryName path with
        | null
        | "" -> ()
        | directory -> Directory.CreateDirectory(directory) |> ignore

        File.WriteAllText(path, toJson report)
