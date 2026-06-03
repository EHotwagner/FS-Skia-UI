module OwnsValidationTests

// Feature 059 — failing-first coverage for the owns-driven evidence-ownership
// model (FR-006/FR-007/FR-009/FR-010) and the hint-resolution contract (FR-008).
// US3: tasks.deps.yml parses `owns:` and the missing-wrapper case is directive.
// US5: ownership derives from `owns:`, free-form titles never block, unknown
//      owns values and missing implied skills are reported.
// US4: every bundled `fs-skia-*` hint id resolves to a consumer-registerable skill.

open System.IO
open System.Text.RegularExpressions
open Expecto
open FS.Skia.UI.Build.Evidence
open GovernanceTestSupport

let private emptySeh : SehMetadata =
    { Annotation = false
      ApprovalLabel = false
      DesignSource = ""
      SyntheticInputClass = ""
      ExpectedErrorBehavior = ""
      Rationale = ""
      AcceptanceStatus = ""
      Diagnostics = [] }

let private mkTask (id: string) (title: string) (mirror: string list) (deps: string list) : TaskRecord =
    { Id = id
      Declared = Done
      Phase = None
      Story = None
      Tier = None
      Parallel = false
      LineNo = 0
      Title = title
      SkillistMirror = Some mirror
      Skillist = []
      ExplicitDeps = deps
      PhaseDeps = []
      SkillMatchAssessments = []
      Seh = emptySeh }

let private registry () = SkillRegistry.build repositoryRoot

let private wrapped (body: string) =
    "schema_version: \"1.0\"\ntasks:\n" + body

[<Tests>]
let ownsValidationTests =
    testList
        "Feature 059 owns-driven ownership + hint resolution"
        [ // --- US3 / FR-007: parser directives -------------------------------
          test "bare top-level task keys emit one standalone wrapper directive (FR-007)" {
              let yml =
                  "schema_version: \"1.0\"\nT001:\n  deps: []\n  skillist: []\nT002:\n  deps: [T001]\n  skillist: []\n"
              let model = DepsParser.parse yml
              Expect.isTrue
                  (model.Errors |> List.exists (fun e -> e.Contains "found bare task keys"))
                  "the directive names the missing tasks: wrapper"
              Expect.isEmpty model.Order "no tasks parse when the wrapper is absent"
          }

          test "bare-key structural failure is not buried under per-id no-key errors (FR-007/SC-003)" {
              let tasks = [ for i in 1..5 -> mkTask (sprintf "T%03d" i) "t" [] [] ]
              let yml =
                  "schema_version: \"1.0\"\nT001:\n  deps: []\n  skillist: []\nT002:\n  deps: []\n  skillist: []\n"
              let deps = DepsParser.parse yml
              let res = Audit.validateAndMerge (registry ()) tasks deps
              Expect.isFalse
                  (res.Errors |> List.exists (fun e -> e.Contains "has no key for it"))
                  "the per-id no-key flood is suppressed when the deps map is structurally empty"
          }

          // --- US3 / FR-006: owns parses --------------------------------------
          test "owns field parses into the entry (FR-006)" {
              let yml =
                  wrapped "  T001:\n    deps: []\n    skillist: [\"speckit-evidence-graph\"]\n    owns: [\"graph-validation\"]\n"
              let model = DepsParser.parse yml
              Expect.equal model.Map.["T001"].Owns (Some [ "graph-validation" ]) "owns is read as a list"
          }

          test "absent owns is None and is not an error (owns is optional)" {
              let yml = wrapped "  T001:\n    deps: []\n    skillist: []\n"
              let model = DepsParser.parse yml
              Expect.equal model.Map.["T001"].Owns None "absent owns parses to None"
          }

          // --- US5 / FR-010: free-form titles never block --------------------
          test "free-form titles containing former trigger phrases no longer block (SC-006)" {
              let tasks =
                  [ mkTask "T001" "Validate the task graph and evidence graph readiness validation flow" [] [] ]
              let yml = wrapped "  T001:\n    deps: []\n    skillist: []\n    owns: []\n"
              let deps = DepsParser.parse yml
              let res = Audit.validateAndMerge (registry ()) tasks deps
              Expect.isFalse
                  (res.Errors |> List.exists (fun e -> e.Contains "high-confidence" || e.Contains "matches"))
                  "no title-derived omission error is raised"
          }

          // --- US5 / FR-010: ownership derives from owns ---------------------
          test "owns graph-validation without the implied skill is a directive error" {
              let tasks = [ mkTask "T001" "free form title" [] [] ]
              let yml = wrapped "  T001:\n    deps: []\n    skillist: []\n    owns: [\"graph-validation\"]\n"
              let deps = DepsParser.parse yml
              let res = Audit.validateAndMerge (registry ()) tasks deps
              Expect.isTrue
                  (res.Errors
                   |> List.exists (fun e -> e.Contains "owns graph-validation requires skill speckit-evidence-graph"))
                  "the implied skill is required in the skillist"
          }

          test "owns graph-validation with the implied skill present passes clean" {
              let tasks = [ mkTask "T001" "free form title" [ "speckit-evidence-graph" ] [] ]
              let yml =
                  wrapped "  T001:\n    deps: []\n    skillist: [\"speckit-evidence-graph\"]\n    owns: [\"graph-validation\"]\n"
              let deps = DepsParser.parse yml
              let res = Audit.validateAndMerge (registry ()) tasks deps
              Expect.isFalse
                  (res.Errors |> List.exists (fun e -> e.Contains "owns graph-validation requires"))
                  "no ownership error when the implied skill is declared"
          }

          test "unknown owns value is rejected naming the allowed set" {
              let tasks = [ mkTask "T001" "free form title" [] [] ]
              let yml = wrapped "  T001:\n    deps: []\n    skillist: []\n    owns: [\"bogus\"]\n"
              let deps = DepsParser.parse yml
              let res = Audit.validateAndMerge (registry ()) tasks deps
              Expect.isTrue
                  (res.Errors
                   |> List.exists (fun e ->
                       e.Contains "unknown owns value 'bogus'"
                       && e.Contains "graph-validation, evidence-audit, task-generation, implementation-loading, constitution"))
                  "the allowed vocabulary is named in the error"
          }

          // --- US4 / FR-008: hint ids resolve --------------------------------
          test "every bundled fs-skia hint id resolves to a consumer-registerable skill (FR-008/SC-004)" {
              let agentSkills =
                  Directory.GetDirectories(fullPath ".agents/skills")
                  |> Array.map Path.GetFileName
                  |> Set.ofArray
              let productSkills =
                  Directory.GetDirectories(fullPath "template/product-skills")
                  |> Array.map Path.GetFileName
                  |> Set.ofArray
              let consumerRegisterable =
                  agentSkills
                  |> Set.union productSkills
                  |> Set.add "fs-skia-samples"
                  |> Set.add "fs-skia-feedback-capture"

              let hintSources =
                  [ ".agents/skills/speckit-tasks/SKILL.md"
                    ".specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml" ]

              for src in hintSources do
                  let text = read src
                  for m in Regex.Matches(text, "fs-skia-[a-z0-9-]+") do
                      let id = m.Value
                      Expect.isTrue
                          (consumerRegisterable.Contains id)
                          (sprintf "%s references hint id '%s', which must resolve to a consumer-registerable skill" src id)
          } ]

// US1 / FR-002/FR-003/FR-004/FR-014: the template build.fsx resolves the feature
// from .specify/feature.json (with the SPECKIT_FEATURE_DIR override), fails loud,
// and never synthesises a bundled sample.
[<Tests>]
let templateFeatureResolverTests =
    let buildFsx = read "template/base/build.fsx"

    testList
        "Feature 059 template build.fsx feature resolver"
        [ test "resolver reads .specify/feature.json and honors the SPECKIT_FEATURE_DIR override (FR-002)" {
              Expect.stringContains buildFsx ".specify/feature.json" "build.fsx reads feature.json"
              Expect.stringContains buildFsx "feature_directory" "build.fsx reads the feature_directory key"
              Expect.stringContains buildFsx "SPECKIT_FEATURE_DIR" "build.fsx honors the override variable"
          }

          test "resolver fails loud naming the source and override, with no sample fallback (FR-003)" {
              Expect.stringContains buildFsx "Cannot resolve the feature to validate" "build.fsx fails with an actionable message"
              Expect.stringContains buildFsx "never falls back to a bundled sample" "build.fsx documents no sample fallback"
          }

          test "resolver echoes the resolved feature directory and task count (FR-004)" {
              Expect.stringContains buildFsx "feature-directory=" "build.fsx echoes the resolved feature directory"
              Expect.stringContains buildFsx "tasks=" "build.fsx echoes the task count"
          }

          test "the runtime sample synthesiser and its selector are removed (FR-014)" {
              Expect.isFalse (buildFsx.Contains "ensureGeneratedEvidencePackage") "the sample synthesiser is gone"
              Expect.isFalse (buildFsx.Contains "generated-evidence-workflow") "the runtime sample feature is gone"
              Expect.isFalse (buildFsx.Contains "GENERATED_EVIDENCE_FEATURE_DIR") "the sample-era selector is gone"
          } ]
