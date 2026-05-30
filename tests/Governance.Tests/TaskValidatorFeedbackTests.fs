module TaskValidatorFeedbackTests

(* SYNTHETIC FIXTURE: malformed task titles and registry mismatches in this
   file are validator error-path inputs, not product behavior substitutes. *)

open System
open System.IO
open Expecto
open GovernanceTestSupport

let private skillFile root relativePath name =
    writeFixtureFile
        root
        relativePath
        $"""---
name: {name}
---

# {name}
"""
    |> ignore

let private writeValidatorFeature root tasks deps =
    writeFixtureFile
        root
        "tasks.md"
        $"""# Tasks: Validator Fixture

## Status Legend

- `[ ]` - pending
- `[X]` - done with real evidence
- `[S]` - done with synthetic evidence only
- `[F]` - failed
- `[-]` - skipped

## Phase 1: Setup

{tasks}

## Synthetic-Evidence Inventory

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
"""
    |> ignore

    writeFixtureFile
        root
        "tasks.deps.yml"
        $"""schema_version: "1.0"
tasks:
{deps}
"""
    |> ignore

let private graphDeps taskId skillist deps =
    let quotedSkills =
        match skillist with
        | [] -> "[]"
        | skills -> skills |> List.map (fun skill -> $"\"{skill}\"") |> String.concat ", " |> sprintf "[%s]"

    let quotedDeps =
        match deps with
        | [] -> "[]"
        | deps -> deps |> String.concat ", " |> sprintf "[%s]"

    $"  {taskId}:\n    deps: {quotedDeps}\n    skillist: {quotedSkills}\n"

let private runGraph root =
    runProcess
        "python3"
        $".specify/extensions/evidence/scripts/python/compute-task-graph.py \"{root}\""

let private runGraphOnly root =
    runProcess
        "bash"
        $".specify/extensions/evidence/scripts/bash/run-audit.sh \"{root}\" --graph-only"

let private writeSpecKitSkills root =
    skillFile root ".agents/skills/speckit-evidence-graph/SKILL.md" "speckit-evidence-graph"
    skillFile root ".agents/skills/speckit-evidence-audit/SKILL.md" "speckit-evidence-audit"
    skillFile root ".agents/skills/speckit-tasks/SKILL.md" "speckit-tasks"
    skillFile root ".agents/skills/speckit-implement/SKILL.md" "speckit-implement"
    skillFile root ".agents/skills/speckit-constitution/SKILL.md" "speckit-constitution"

let private allTriggerTerms =
    [ "graph validation"
      "task graph"
      "evidence graph"
      "readiness validation"
      "tasks.deps.yml"
      "structured task metadata"
      "mirror mismatch"
      "skillist field"
      "skillist, list typing"
      "obvious capability"
      "multi-skill dependency order"
      "migration blocker"
      "validator diagnostics"
      "EvidenceGraph"
      "evidence audit"
      "diff-scan"
      "synthetic propagation"
      "readiness-blocking"
      "EvidenceAudit"
      "task generation"
      "/speckit.tasks"
      "speckit.tasks"
      "task-generation"
      "task templates"
      "tasks-template"
      "tasks template"
      "generated task guidance"
      "post-generation skill evaluation"
      "implementation loading"
      "/speckit.implement"
      "speckit.implement"
      "implementation-loading"
      "implementation skill"
      "implementation command"
      "load each"
      "skill-load"
      "before implementation"
      "constitution"
      "constitutional" ]

[<Tests>]
let taskValidatorFeedbackTests =
    testList "Task validator feedback follow-ups" [
        test "malformed title Synthetic accepts mandated readiness filename without workflow trigger" {
            use fixture = new TempFixtureDirectory("title-filename-pass")
            writeSpecKitSkills fixture.Root
            writeValidatorFeature
                fixture.Root
                "- [ ] T001 [skillist: []] Complete readiness notes for readiness/skill-loading-evidence-workflow.md placeholder"
                (graphDeps "T001" [] [])

            let code, stdout, stderr = runGraph fixture.Root
            Expect.equal code 0 $"filename-only trigger validates: {stdout} {stderr}"
        }

        test "malformed title Synthetic still rejects omitted whole-word workflow skill" {
            use fixture = new TempFixtureDirectory("title-trigger-fail")
            writeSpecKitSkills fixture.Root
            writeValidatorFeature
                fixture.Root
                "- [ ] T001 [skillist: []] Run EvidenceGraph task graph validation"
                (graphDeps "T001" [] [])

            let code, stdout, stderr = runGraph fixture.Root
            Expect.equal code 2 $"whole-word trigger is rejected: {stdout} {stderr}"
            Expect.stringContains (stdout + stderr) "trigger_group=graph validation" "diagnostic names trigger group"
            Expect.stringContains (stdout + stderr) "matched_trigger=task graph" "diagnostic names matched trigger"
        }

        test "registry mismatch Synthetic reports accepted declared id and source path" {
            use fixture = new TempFixtureDirectory("registry-mismatch")
            skillFile fixture.Root ".agents/skills/directory-name/SKILL.md" "accepted-skill"
            writeValidatorFeature
                fixture.Root
                "- [ ] T001 [skillist: directory-name] Use directory-like skill declaration"
                (graphDeps "T001" [ "directory-name" ] [])

            let code, stdout, stderr = runGraph fixture.Root
            Expect.equal code 2 $"directory-like declaration is rejected: {stdout} {stderr}"
            Expect.stringContains (stdout + stderr) "accepted declared id is accepted-skill" "diagnostic names accepted id"
            Expect.stringContains (stdout + stderr) ".agents/skills/directory-name/SKILL.md" "diagnostic names source path"
        }

        test "task graph Synthetic preserves cycle missing dependency mirror unreadable and skill-order checks" {
            let source = read ".specify/extensions/evidence/scripts/python/compute-task-graph.py"

            [ "detect_cycles"
              "depends on {d}, which does not exist"
              "tasks.md mirror"
              "declared skill {skill_id} is not readable or not registered"
              "skillist order places {skill_id} before prerequisite {prerequisite}" ]
            |> List.iter (fun required ->
                Expect.stringContains source required $"validator preserves {required}")
        }

        test "generated task guidance documents readiness prefix trigger groups and safe examples" {
            [ ".specify/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
              ".agents/skills/speckit-tasks/SKILL.md"
              ".specify/presets/fsharp-opinionated/commands/speckit.tasks.md" ]
            |> List.iter (fun path ->
                let content = read path
                Expect.stringContains content "Complete readiness notes" $"{path} documents readiness prefix"
                Expect.stringContains content "Safe setup wording examples" $"{path} documents safe setup examples"
                allTriggerTerms
                |> List.iter (fun term -> Expect.stringContains content term $"{path} documents trigger term {term}"))
        }

        test "skill registry guidance documents authoritative roots and declared-name rule" {
            [ ".specify/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
              ".agents/skills/speckit-tasks/SKILL.md"
              ".specify/presets/fsharp-opinionated/commands/speckit.tasks.md" ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ ".agents/skills/*/SKILL.md"
                      "src/*/skill/SKILL.md"
                      "template/fragments/*/skill/SKILL.md"
                      "name:"
                      "directory name" ])
        }

        test "guidance proves FS.Skia.UI capability hints are advisory and cover common categories" {
            let required =
                [ "non-blocking"
                  "rendering"
                  "viewer"
                  "input"
                  "layout"
                  "controls"
                  "DataGrid"
                  "evidence"
                  "fs-skia-scene"
                  "fs-skia-skiaviewer"
                  "fs-skia-keyboard-input"
                  "fs-skia-layout"
                  "fs-skia-layout-evidence" ]

            [ ".specify/templates/tasks-template.md"
              ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
              ".agents/skills/speckit-tasks/SKILL.md"
              ".specify/presets/fsharp-opinionated/commands/speckit.tasks.md" ]
            |> List.iter (fun path -> expectFileContains path required)
        }

        test "graph-only command output labels graph validation success and failure modes" {
            use passing = new TempFixtureDirectory("graph-only-pass")
            writeValidatorFeature
                passing.Root
                "- [ ] T001 [skillist: []] Record setup evidence"
                (graphDeps "T001" [] [])

            let passCode, passStdout, passStderr = runGraphOnly passing.Root
            Expect.equal passCode 0 $"graph-only pass succeeds: {passStdout} {passStderr}"
            Expect.stringContains passStdout "speckit.evidence.graph (graph validation only)" "success output identifies graph validation"
            Expect.stringContains passStdout "Run EvidenceAudit for full merge-gate validation" "success output directs audit"

            use failing = new TempFixtureDirectory("graph-only-fail")
            writeValidatorFeature
                failing.Root
                "- [ ] T001 [skillist: []] Record setup evidence"
                "  T001:\n    deps: [T999]\n    skillist: []\n"

            let failCode, failStdout, failStderr = runGraphOnly failing.Root
            Expect.equal failCode 3 $"graph-only failure exits as graph failure: {failStdout} {failStderr}"
            Expect.stringContains failStdout "speckit.evidence.graph (graph validation only)" "failure output identifies graph validation"
            Expect.stringContains (failStdout + failStderr) "Graph compute failed" "failure remains graph-specific"
        }

        test "generated evidence report labels EvidenceGraph as graph validation only" {
            expectFileContains
                "template/base/build.fsx"
                [ "graph-validation-only"
                  "Run EvidenceAudit for full merge-gate validation"
                  "diff-scan and synthetic-evidence blocking checks" ]

            expectFileContains
                "build.fsx"
                [ "ran graph validation only"
                  "Run `EvidenceAudit` for full merge-gate validation" ]
        }
    ]
