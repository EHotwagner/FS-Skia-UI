module FS.Skia.UI.Build.Capabilities

open System
open System.IO
open System.Collections
open System.Collections.Generic
open YamlDotNet.Serialization
open FS.Skia.UI.Build.Findings

type CapabilityRow =
    { Id: string
      DisplayName: string
      PackageId: string option
      Project: string option
      Contracts: string list
      Tests: string list
      Skill: string option
      TemplateFragment: string option
      Dependencies: string list
      Profiles: string list
      DefaultApp: bool
      Evidence: string list
      SurfaceBaseline: string option
      Docs: string option
      NonRuntime: bool }

// YamlDotNet deserializes an untyped document into nested IDictionary<obj,obj> for
// mappings, List<obj> for sequences, and string scalars. The projection below mirrors
// the field semantics of the retired hand-rolled parser exactly.
let private toMap (node: obj) : Map<string, obj> =
    match node with
    | :? IDictionary<obj, obj> as d -> d |> Seq.map (fun kv -> string kv.Key, kv.Value) |> Map.ofSeq
    | _ -> Map.empty

let private strOpt (m: Map<string, obj>) key =
    Map.tryFind key m |> Option.map string

let private strList (m: Map<string, obj>) key =
    match Map.tryFind key m with
    | Some value ->
        match value with
        | :? string -> []
        | :? IEnumerable as items -> [ for item in items -> string item ]
        | _ -> []
    | None -> []

let private boolField (m: Map<string, obj>) key =
    match strOpt m key with
    | Some value -> value.Equals("true", StringComparison.OrdinalIgnoreCase)
    | None -> false

let private toRow (m: Map<string, obj>) =
    { Id = strOpt m "id" |> Option.defaultValue ""
      DisplayName = strOpt m "displayName" |> Option.defaultValue ""
      PackageId = strOpt m "packageId"
      Project = strOpt m "project"
      Contracts = strList m "contracts"
      Tests = strList m "tests"
      Skill = strOpt m "skill"
      TemplateFragment = strOpt m "templateFragment"
      Dependencies = strList m "dependencies"
      Profiles = strList m "profiles"
      DefaultApp = boolField m "defaultApp"
      Evidence = strList m "evidence"
      SurfaceBaseline = strOpt m "surfaceBaseline"
      Docs = strOpt m "docs"
      NonRuntime = boolField m "nonRuntime" }

let readCatalog (yamlPath: string) =
    if not (File.Exists yamlPath) then
        failwithf "Missing capability catalog: %s" yamlPath

    let text = File.ReadAllText yamlPath
    let deserializer = DeserializerBuilder().Build()
    let root = deserializer.Deserialize<obj>(text) |> toMap

    match Map.tryFind "capabilities" root with
    | Some value ->
        match value with
        | :? IEnumerable as capabilities when not (value :? string) -> [ for capability in capabilities -> toRow (toMap capability) ]
        | _ -> []
    | None -> []

let validateRows catalogPath (surfaceBaselineExists: string -> bool) (capabilities: CapabilityRow list) =
    let ids = capabilities |> List.map (fun capability -> capability.Id) |> Set.ofList

    let requiredDefault =
        Set.ofList [ "Scene"; "SkiaViewer"; "Elmish"; "KeyboardInput"; "Layout"; "Controls" ]

    let defaultApp =
        capabilities
        |> List.filter (fun capability -> capability.DefaultApp)
        |> List.map (fun capability -> capability.DisplayName)
        |> Set.ofList

    [ if defaultApp <> requiredDefault then
          yield finding "capability-catalog" catalogPath "default-app" ("Default app set was " + String.Join(", ", defaultApp))

      for capability in capabilities do
          if String.IsNullOrWhiteSpace capability.DisplayName then
              yield finding "capability-catalog" capability.Id "displayName" "Missing displayName"

          if capability.Project.IsNone && not capability.NonRuntime then
              yield finding "capability-catalog" capability.Id "project" "Runtime capability is missing project"

          if capability.Contracts.IsEmpty then
              yield finding "capability-catalog" capability.Id "contracts" "Missing public contracts or no-public-surface record"

          if capability.Tests.IsEmpty then
              yield finding "capability-catalog" capability.Id "tests" "Missing test coverage entry"

          if capability.Skill.IsNone then
              yield finding "capability-catalog" capability.Id "skill" "Missing local skill"

          if capability.TemplateFragment.IsNone then
              yield finding "capability-catalog" capability.Id "templateFragment" "Missing template fragment"

          if capability.Profiles.IsEmpty then
              yield finding "capability-catalog" capability.Id "profiles" "Missing profile ownership"

          if capability.Evidence.IsEmpty then
              yield finding "capability-catalog" capability.Id "evidence" "Missing evidence classes"

          match capability.SurfaceBaseline with
          | Some "no-public-surface" -> ()
          | Some baseline when surfaceBaselineExists baseline -> ()
          | Some baseline -> yield finding "capability-catalog" capability.Id "surfaceBaseline" $"Missing surface baseline {baseline}"
          | None -> yield finding "capability-catalog" capability.Id "surfaceBaseline" "Missing surface baseline"

          for dependency in capability.Dependencies do
              if not (ids.Contains dependency) then
                  yield finding "capability-catalog" capability.Id "dependency" $"Unknown dependency {dependency}" ]

let renderReport (capabilities: CapabilityRow list) =
    [ "# Capability Catalog"
      ""
      "PASS: capability catalog metadata, dependency closure, default app set, contracts, tests, skills, fragments, evidence, and surface baselines are valid."
      ""
      "| Capability | Package | Project | Dependencies | Default app |"
      "|------------|---------|---------|--------------|-------------|"
      yield!
          capabilities
          |> List.map (fun capability ->
              let packageId = capability.PackageId |> Option.defaultValue "non-runtime"
              let project = capability.Project |> Option.defaultValue "non-runtime"
              let dependencies = if capability.Dependencies.IsEmpty then "(none)" else String.Join(", ", capability.Dependencies)
              $"| {capability.DisplayName} | `{packageId}` | `{project}` | {dependencies} | {capability.DefaultApp} |") ]
    |> String.concat Environment.NewLine
