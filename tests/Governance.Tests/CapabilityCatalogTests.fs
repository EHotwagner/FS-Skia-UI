module CapabilityCatalogTests

(* Feature 041 / US4 — failing-first (Principle I/VI) typed-finding tests for the
   capability-catalog validator extracted into FS.Skia.UI.Build.Capabilities. These
   exercise the real pure validator over crafted typed rows (rule ids, not string
   matching; the surface-baseline probe is injected, no disk). Before Capabilities.fs
   existed they did not compile (SC-004). *)

open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Capabilities

let private row id displayName project dependencies surfaceBaseline : CapabilityRow =
    { Id = id
      DisplayName = displayName
      PackageId = Some "FS.Skia.UI.Example"
      Project = project
      Contracts = [ "src/Example/Example.fsi" ]
      Tests = [ "tests/Example.Tests/Example.Tests.fsproj" ]
      Skill = Some "template/product-skills/example/SKILL.md"
      TemplateFragment = Some "template/fragments/example"
      Dependencies = dependencies
      Profiles = [ "app" ]
      DefaultApp = false
      Evidence = [ "capability-catalog" ]
      SurfaceBaseline = surfaceBaseline
      Docs = Some "docs/reports/example.md"
      NonRuntime = false }

let private rules (findings: Findings.ValidationFinding list) = findings |> List.map (fun f -> f.Rule)
let private alwaysExists (_: string) = true

[<Tests>]
let capabilityCatalogTests =
    testList "Feature 041 capability catalog validation (typed rule ids)" [

        test "displayName rule fires when a row has a blank display name" {
            let rows = [ row "scene" "" (Some "src/Scene/Scene.fsproj") [] (Some "no-public-surface") ]
            let findings = Capabilities.validateRows "template/capabilities.yml" alwaysExists rows
            Expect.contains (rules findings) "displayName" "blank displayName is flagged"
        }

        test "dependency rule fires when a row references an unknown capability id" {
            let rows = [ row "scene" "Scene" (Some "src/Scene/Scene.fsproj") [ "ghost" ] (Some "no-public-surface") ]
            let findings = Capabilities.validateRows "template/capabilities.yml" alwaysExists rows
            Expect.contains (rules findings) "dependency" "unknown dependency is flagged"
        }

        test "project rule fires when a runtime row is missing its project" {
            let rows = [ row "scene" "Scene" None [] (Some "no-public-surface") ]
            let findings = Capabilities.validateRows "template/capabilities.yml" alwaysExists rows
            Expect.contains (rules findings) "project" "runtime row missing project is flagged"
        }

        test "surfaceBaseline rule fires when the baseline file is absent (injected probe)" {
            let rows = [ row "scene" "Scene" (Some "src/Scene/Scene.fsproj") [] (Some "readiness/surface-baselines/Missing.txt") ]
            let findings = Capabilities.validateRows "template/capabilities.yml" (fun _ -> false) rows
            Expect.contains (rules findings) "surfaceBaseline" "absent surface baseline is flagged"
        }

        test "default-app rule fires when the default app set is not the required six" {
            let rows = [ { row "scene" "Scene" (Some "src/Scene/Scene.fsproj") [] (Some "no-public-surface") with DefaultApp = true } ]
            let findings = Capabilities.validateRows "template/capabilities.yml" alwaysExists rows
            Expect.contains (rules findings) "default-app" "wrong default-app set is flagged"
        }
    ]
