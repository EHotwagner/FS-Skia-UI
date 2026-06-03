module SkillValidationTests

open System
open Expecto
open GovernanceTestSupport

let requiredSkillSections =
    [ "Scope"
      "Public Contract"
      "Build Commands"
      "Test Commands"
      "Evidence"
      "Package Boundary"
      "Generated Product" ]

let private taskBlock taskId (depsYaml: string) =
    let marker = $"  {taskId}:"
    let start = depsYaml.IndexOf(marker, StringComparison.Ordinal)

    if start < 0 then
        ""
    else
        let rest = depsYaml.Substring(start + marker.Length)
        let next = rest.IndexOf("\n  T", StringComparison.Ordinal)

        if next < 0 then rest else rest.Substring(0, next)

let private taskLine taskId (tasksMarkdown: string) =
    tasksMarkdown.Replace("\r\n", "\n").Split('\n')
    |> Array.tryFind (fun line -> line.Contains($" {taskId} ", StringComparison.Ordinal))
    |> Option.defaultValue ""

let private requiresLayoutEvidenceSkill (text: string) =
    [ "HUD"
      "gameplay"
      "layout evidence"
      "readability"
      "generated validation"
      "public contract"
      "host-warning"
      "warning-classification"
      "Product.Program.view"
      "Product.Program.generatedHost"
      "Product.Program.update" ]
    |> List.exists (fun needle -> text.Contains(needle, StringComparison.OrdinalIgnoreCase))

[<Tests>]
let skillValidationTests =
    testList "V3 local skill validation" [
        test "capability skills declare required sections and command references" {
            readCatalogCapabilities ()
            |> List.iter (fun capability ->
                match capability.Skill with
                | Some skillPath ->
                    if fileExists skillPath then
                        let content = read skillPath

                        requiredSkillSections
                        |> List.iter (fun section -> Expect.stringContains content $"## {section}" $"{skillPath} contains {section}")

                        Expect.stringContains content "./fake.sh build -t" $"{skillPath} names FAKE verification commands"
                    else
                        Expect.isFalse (directoryExists "specs/009-v3-modular-framework") $"{capability.Id} skill exists at {skillPath}"
                | None -> failtestf "%s has no skill path" capability.Id)
        }

        test "generated product receives selected skills only" {
            expectFileContains
                "build.fsx"
                [ "copySelectedSkills"
                  "fs-skia-project"
                  "fs-skia-scene"
                  "fs-skia-skiaviewer"
                  "fs-skia-elmish"
                  "fs-skia-keyboard-input"
                  "fs-skia-ui-widgets"
                  "unrelated capability skills" ]
        }

        test "split layout/evidence implementation skills are discoverable in the capability inventory" {
            // Feature 059 (FR-012): the former fs-skia-layout-evidence catch-all is split
            // into fs-skia-layout-readability and fs-skia-evidence-mode.
            expectFileContains
                "template/capabilities.yml"
                [ "implementationSkills inventory"
                  "skillId: fs-skia-layout-readability"
                  ".agents/skills/fs-skia-layout-readability/SKILL.md"
                  "skillId: fs-skia-evidence-mode"
                  ".agents/skills/fs-skia-evidence-mode/SKILL.md"
                  "generated-game-hud-readability"
                  "public-scene-host-update-guidance"
                  "host-warning-classification" ]
        }

        test "active layout evidence tasks mirror fs-skia-layout-evidence in structured and visible metadata" {
            let deps = read "specs/020-asteroids-integration-feedback/tasks.deps.yml"
            let tasks = read "specs/020-asteroids-integration-feedback/tasks.md"

            let taskIds =
                [ "T002"; "T003"; "T004"; "T005"; "T006"; "T007"; "T008"; "T009"; "T010"
                  "T011"; "T012"; "T013"; "T014"; "T015"; "T016"; "T017"; "T018"; "T019"; "T020"; "T021"
                  "T022"; "T023"; "T024"; "T025"; "T026"; "T027"; "T028"; "T029"; "T030"; "T031"
                  "T032"; "T033"; "T034"; "T037"; "T038" ]

            taskIds
            |> List.iter (fun taskId ->
                Expect.stringContains (taskBlock taskId deps) "\"fs-skia-layout-evidence\"" $"{taskId} structured skillist includes layout evidence skill"
                Expect.stringContains (taskLine taskId tasks) "fs-skia-layout-evidence" $"{taskId} visible task line mirrors layout evidence skill")
        }

        test "layout evidence task metadata fixture rejects omitted skill" {
            let malformedLine = "- [ ] T900 [US1] [skillist: fs-skia-template-update] Update generated game HUD readability validation"

            Expect.isTrue (requiresLayoutEvidenceSkill malformedLine) "fixture describes layout evidence work"
            Expect.isFalse (malformedLine.Contains("fs-skia-layout-evidence", StringComparison.Ordinal)) "fixture omits required skill and would be rejected"
        }

        test "skill-loading evidence workflow derives one required row per real task skill pairing" {
            // Feature 043 (T029): re-pointed off the deleted compute-task-graph.py
            // source onto the compiled typed validator in build/Governance/Evidence/Audit.
            let deps = read "specs/027-generated-evidence-workflow/tasks.deps.yml"
            let auditFsi = read "build/Governance/Evidence/Audit.fsi"

            [ "T001:"
              "skillist: [\"speckit-tasks\", \"speckit-implement\"]"
              "T021:"
              "skillist: [\"speckit-tasks\", \"speckit-implement\", \"speckit-evidence-graph\"]" ]
            |> List.iter (fun needle -> Expect.stringContains deps needle $"real task metadata includes {needle}")

            Expect.stringContains auditFsi "validateSkillLoadingEvidence" "typed engine exposes the skill-loading-evidence validator"
        }

        test "skill-loading evidence workflow Synthetic rejects duplicate masking and invalid timestamp ordering" {
            // Feature 043 (T029): re-pointed onto the compiled typed validator.
            let auditSource = read "build/Governance/Evidence/Audit.fs"

            [ "duplicate skill-loading evidence row"
              "collapsed task range"
              "multi-skill prose row"
              "loaded_at must be earlier than work_started_at"
              "equal timestamps" ]
            |> List.iter (fun needle ->
                Expect.stringContains auditSource needle $"skill-loading validator diagnoses {needle}")
        }
    ]
