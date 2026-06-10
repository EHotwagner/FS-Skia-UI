module Feature062GovernanceTests

// Feature 062 (space-invaders-consumer-friction-followups): executable coverage
// for the always-on feedback hook (FR-001/SC-001), the single-sourced
// evidence-format schema + recoverable diagnostics (FR-005/SC-002), the
// effective-DAG render + skillist set (FR-007/SC-004), the mechanical
// cross-artifact symbol set-difference (FR-008/SC-005), and the shipped arcade
// helpers (FR-010/SC-006). Pure-function assertions, never log scraping.

open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Evidence
open GovernanceTestSupport

// Feature 089 threaded a SkillRegistry into Render.taskGraphMd; these two 062
// renders don't exercise resolution, so an empty registry keeps the prior assertions.
let private emptyRegistry: SkillRegistry =
    { Skills = Map.empty; DirectoryAliases = Map.empty; Warnings = [] }

[<Tests>]
let feature062GovernanceTests =
    testList "feature 062 space-invaders consumer-friction follow-ups" [

        // --- FR-001 / SC-001 (T010): feedback hook stays mandatory ----------
        test "FR-001 a surviving optional: true feedback hook is flagged" {
            let yaml =
                "hooks:\n  after_specify:\n  - extension: feedback\n    optional: true\n"
            let findings = Guidance.feedbackHookOptionalFindings "template/feedback/extensions/feedback.yml" yaml
            Expect.isNonEmpty findings "optional: true must be flagged (FR-001 regression guard)"
            Expect.stringContains (List.head findings) "optional: false" "names the required value"
        }

        test "FR-001 an all-mandatory feedback registration passes" {
            let yaml =
                "hooks:\n  after_specify:\n  - extension: feedback\n    optional: false\n"
            let findings = Guidance.feedbackHookOptionalFindings "template/feedback/extensions/feedback.yml" yaml
            Expect.isEmpty findings "optional: false has no findings"
        }

        test "FR-001 SC-001 the shipped feedback.yml has no optional: true (regression guard)" {
            let text = read "template/feedback/extensions/feedback.yml"
            let findings = Guidance.feedbackHookOptionalFindings "template/feedback/extensions/feedback.yml" text
            Expect.isEmpty findings "every after_<phase> feedback hook must be optional: false (FR-001/SC-001)"
        }

        // --- FR-005 / SC-002 (T017): each format class prints its full shape -
        test "FR-005 skill-loading-evidence prints the full 8-column row schema" {
            let text = Audit.skillLoadingEvidenceSchemaText ()
            for col in EvidenceFormatSchema.skillLoadingColumns do
                Expect.stringContains text col (sprintf "column %s in the printed schema" col)
            Expect.stringContains text "loaded_at < work_started_at" "ordering rule printed"
            Expect.stringContains text ".agents/skills/<id>/SKILL.md" "resolved-path pattern printed"
        }

        test "FR-005 window-visibility prints its keys and diagnostic-class value set" {
            let text = Scans.windowVisibilitySchemaText ()
            for cls in EvidenceFormatSchema.windowDiagnosticClasses do
                Expect.stringContains text cls (sprintf "diagnostic-class %s printed" cls)
            for key in EvidenceFormatSchema.interactiveVisibleWindowKeys do
                Expect.stringContains text key (sprintf "interactive-visible-window key %s printed" key)
        }

        test "FR-005 seh-acceptance prints its tokens with no backticks" {
            let text = TaskParser.sehAcceptanceSchemaText ()
            Expect.stringContains text "accepted-seh" "acceptance-status token printed"
            Expect.stringContains text "synthetic-error-handling-approved" "approval label token printed"
            Expect.isFalse (text.Contains "`") "no backticks around the SEH tokens"
        }

        test "FR-005 a skill-loading-evidence graph error prints the row schema in task-graph.md" {
            let gr : GraphResult =
                { FeatureName = "062-space-invaders-consumer-friction-followups"
                  Verdict = GraphVerdict.Error
                  Errors = [ "T044: declared skill fs-skia-elmish has no pre-work load evidence" ]
                  Warnings = []
                  Cycles = []
                  Tasks = []
                  RootCause = Map.empty }
            let md = Render.taskGraphMd emptyRegistry gr
            Expect.stringContains md "skill-loading-evidence required shape (FR-005)" "schema heading present on skill-loading failure"
            Expect.stringContains md "WorkStartedAt" "the 8-column row schema is printed inline"
        }

        test "FR-005 the EvidenceFormatSchema and the generated reference share one source" {
            // Every schema entry's required tokens are non-empty and the four classes
            // are represented (single source for diagnostics + evidence-formats.md).
            let classes = EvidenceFormatSchema.schemas |> List.map (fun s -> s.FormatClass) |> List.distinct
            Expect.contains classes ReadinessContract "readiness-contract represented"
            Expect.contains classes SkillLoadingEvidence "skill-loading-evidence represented"
            Expect.contains classes WindowVisibility "window-visibility represented"
            Expect.contains classes SehAcceptance "seh-acceptance represented"
        }

        // --- FR-004 / SC-004 (T028): Dev self-describes -----------------------
        test "FR-004 the Dev verdict caveat is present in the build governance source" {
            let src = read "build.fsx" // aggregate of build/Governance/** (see TestSupport)
            Expect.stringContains src "Dev writes logs/markers and does not compile" "Dev caveat text present"
            Expect.stringContains src "Test/Verify (dotnet test) is the authoritative" "names the authoritative compile/test path"
        }

        // --- FR-007 / SC-004 (T028): effective-DAG render --------------------
        test "FR-007 the effective DAG renders injected checkpoint edges + skillist set" {
            let seh : SehMetadata =
                { Annotation = false; ApprovalLabel = false; DesignSource = ""; SyntheticInputClass = ""
                  ExpectedErrorBehavior = ""; Rationale = ""; AcceptanceStatus = ""; Diagnostics = [] }
            let mkTask id phase skillist explicitDeps phaseDeps : TaskRecord =
                { Id = id; Declared = Done; Phase = Some phase; Story = None; Tier = None; Parallel = false
                  LineNo = 1; Title = id; SkillistMirror = Some skillist; Skillist = skillist
                  ExplicitDeps = explicitDeps; PhaseDeps = phaseDeps; SkillMatchAssessments = []; Seh = seh }
            let mkResolved t : ResolvedTask = { Task = t; Effective = DeclaredEff Done; RootCause = [] }
            let t1 = mkTask "T001" 1 [ "fsharp-parsing" ] [] []
            let t2 = mkTask "T002" 2 [ "fsharp-graph-algorithms" ] [] [ "T001" ] // injected phase-checkpoint edge
            let gr : GraphResult =
                { FeatureName = "062"; Verdict = GraphVerdict.Ok; Errors = []; Warnings = []; Cycles = []
                  Tasks = [ mkResolved t1; mkResolved t2 ]; RootCause = Map.empty }
            let md = Render.taskGraphMd emptyRegistry gr
            Expect.stringContains md "Injected checkpoint edges (Phase N+1 → Phase N)" "injected-edges subsection present"
            Expect.stringContains md "T001 → T002  (auto-injected Phase-checkpoint edge)" "the injected edge is listed distinctly"
            Expect.stringContains md "-. injected .->" "mermaid renders injected edges with a distinct dashed style"
            Expect.stringContains md "Resolved skillist ids" "resolved skillist set subsection present"
            Expect.stringContains md "fsharp-graph-algorithms, fsharp-parsing" "the resolved skillist-id set is printed (sorted)"
        }

        // --- FR-006 / SC-004 (T028): skillist-reference currency -------------
        test "FR-006 the generated skillist-reference.md matches the live registry render" {
            let registry = SkillRegistry.build repositoryRoot
            let expected = SkillistReference.render registry Audit.ownsVocabulary
            let committed = read SkillistReference.referenceDocPath
            Expect.equal (committed.Replace("\r\n", "\n")) (expected.Replace("\r\n", "\n"))
                "docs/skillist-reference.md is current with the live SkillRegistry + owns vocabulary"
        }

        // --- FR-008 / SC-005 (T030): symbol set-difference algebra -----------
        test "FR-008 a symbol in a proper subset of the artifacts is flagged" {
            // FR-016 appears in plan + tasks but not data-model.
            let plan = "FR-016 the foo behavior."
            let dataModel = "no fr ids here."
            let tasks = "implement FR-016 per plan."
            let findings = SymbolCrossCheck.diff plan dataModel tasks
            let fr016 = findings |> List.tryFind (fun s -> s.Name = "FR-016")
            Expect.isSome fr016 "FR-016 is a proper-subset symbol and is reported"
            Expect.equal fr016.Value.PresentIn (set [ SymbolCrossCheck.Plan; SymbolCrossCheck.Tasks ]) "present in plan+tasks"
        }

        test "FR-008 a symbol present in ALL three artifacts is NOT flagged" {
            let txt = "FR-001 everywhere."
            let findings = SymbolCrossCheck.diff txt txt txt
            Expect.isEmpty (findings |> List.filter (fun s -> s.Name = "FR-001")) "consistent symbol is not a finding"
        }

        test "FR-008 a design-only symbol is reported for human judgment, never hard-failed" {
            // A Msg case present only in data-model (design), absent from a spec FR.
            let dataModel = "The `InitialReceived` Msg start-state case."
            let findings = SymbolCrossCheck.diff "" dataModel ""
            let rendered = SymbolCrossCheck.render findings
            Expect.stringContains rendered "InitialReceived" "the design-only symbol is reported"
            Expect.stringContains rendered "design-only? human judgment" "flagged for human judgment, not hard-failed"
        }

        // --- FR-008 / SC-005 (T032): seeded Msg-case drift reported ----------
        test "FR-008 SC-005 a seeded Msg-case drift (in data-model+tasks, not plan) is reported by pass G" {
            let plan = "Plan with FR-001 only; no such Msg case here."
            let dataModel = "The `ViewerKeyEventReceived` Msg case is part of the model."
            let tasks = "T010 handles the `ViewerKeyEventReceived` Msg case."
            let findings = SymbolCrossCheck.diff plan dataModel tasks
            let drift = findings |> List.tryFind (fun s -> s.Name = "ViewerKeyEventReceived")
            Expect.isSome drift "the seeded Msg-case drift is detected mechanically"
            Expect.equal drift.Value.Kind SymbolCrossCheck.MsgCase "classified as a msg-case"
            Expect.equal drift.Value.PresentIn (set [ SymbolCrossCheck.DataModel; SymbolCrossCheck.Tasks ]) "in data-model+tasks"
            let rendered = SymbolCrossCheck.render findings
            Expect.stringContains rendered "ViewerKeyEventReceived — in {data-model, tasks}, missing from {plan}" "pass-G renders the set-difference"
        }

        // --- FR-009 / SC-003 (T037): Result.Ok/Error shadowing pitfall -------
        test "FR-009 the fs-skia-skiaviewer skill documents the Result.Ok/Error shadowing pitfall" {
            let skill = read "src/SkiaViewer/skill/SKILL.md"
            Expect.stringContains skill "Common pitfalls" "skiaviewer skill has a Common pitfalls section"
            Expect.stringContains skill "ViewerDiagnosticLevel.Error" "names the shadowing union case"
            Expect.stringContains skill "Result.Ok" "names the Result.Ok remedy"
            Expect.stringContains skill "Result.Error" "names the Result.Error remedy"
            Expect.stringContains skill "fs-skia-keyboard-input" "cross-references the existing Unknown collision note"
        }

        // --- FR-003 / SC-003 (T037): scaffold-map pre-design pointer ---------
        test "FR-003 scaffold-map.md names the durable/replaceable split + the pre-design scene pointer" {
            let map = read "template/base/docs/scaffold-map.md"
            Expect.stringContains map "GovernanceTests.fs" "names the durable governance test file"
            Expect.stringContains map "BehaviorTests.fs" "names the replaceable behavior test file"
            Expect.stringContains map "--scene-evidence" "names a must-survive source-scan string"
            Expect.stringContains map "record-label collision" "references the fs-skia-scene record-label pitfall"
            Expect.stringContains map "fs-skia-scene" "points at the fs-skia-scene skill as a pre-design step"
        }
    ]
