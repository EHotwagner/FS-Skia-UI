module Feature061GovernanceTests

// Feature 061 (breakout-consumer-friction-followups): executable coverage for
// the self-describing governance output (FR-004 readiness diagnostic, FR-007
// graph verdict line), the four-prompt feedback contract (FR-003 / D6 / FB-5),
// the single defect-class spelling (FR-005), and the consumer-facing content
// edits (FR-006/008/009/010). Governance-output assertions are over the pure
// Render/Scans functions, never log scraping.

open Expecto
open FS.Skia.UI.Build.Evidence
open GovernanceTestSupport

let private mkGraph (verdict: GraphVerdict) (errors: string list) : GraphResult =
    { FeatureName = "061-breakout-consumer-friction-followups"
      Verdict = verdict
      Errors = errors
      Warnings = []
      Cycles = []
      Tasks = []
      RootCause = Map.empty }

[<Tests>]
let feature061GovernanceTests =
    testList "feature 061 consumer-friction follow-ups" [

        // --- FR-007 (T020): explicit greppable graph verdict line -----------
        test "FR-007 EvidenceGraph prints an explicit clean verdict token" {
            let line = Render.graphVerdictLine (mkGraph GraphVerdict.Ok [])
            Expect.equal line "verdict=ok (no cycles, no dangling refs, no [S*])" "clean graph verdict token (GV-1/GV-3)"
        }

        test "FR-007 a failing graph prints verdict=error with the reason inline" {
            let line = Render.graphVerdictLine (mkGraph GraphVerdict.Error [ "T009 depends on T999, which does not exist" ])
            Expect.stringStarts line "verdict=error (" "failing graph verdict token (GV-2)"
            Expect.stringContains line "T999" "failure reason is inline"
        }

        // --- FR-004 (T016): self-describing readiness-contract diagnostic ----
        test "FR-004 a missing readiness file prints its full required shape" {
            // No readiness files supplied -> every readiness-contract file is missing.
            let input =
                { ReadinessDir = "specs/x/readiness"
                  FeatureText = ""
                  ReadinessFiles = [] }
            let rc = Scans.readinessContract input
            let diag = Render.readinessContractDiagnostics rc

            // RC-1: the file name, status, and the FULL enforced token list print
            // (not a bare count, not just the first missing token).
            Expect.stringContains diag "readiness-contract: governance-risk-levels.md" "names the failing file"
            Expect.stringContains diag "status: missing" "reports the status"
            // Feature 063 (FR-004) relabelled this line full-required-set:/absent-from-file:.
            Expect.stringContains diag "full-required-set:" "prints the full-required-set line"
            for token in [ "small"; "medium"; "broad"; "required evidence"; "broad validation" ] do
                Expect.stringContains diag token (sprintf "full-required-set includes %s" token)
            Expect.stringContains diag "readiness-contract: aggregate-hang-diagnostics.md" "covers every failing file"
            Expect.stringContains diag "readiness-contract: runtime-limitations.md" "covers every failing file"
        }

        test "FR-004 RC-2 a partial file still prints the FULL enforced token list" {
            // governance-risk-levels.md present but missing 'broad validation'.
            let input =
                { ReadinessDir = "specs/x/readiness"
                  FeatureText = ""
                  ReadinessFiles =
                    [ "governance-risk-levels.md",
                      "small medium broad required evidence\n" ] }
            let rc = Scans.readinessContract input
            let diag = Render.readinessContractDiagnostics rc
            Expect.stringContains diag "status: partial" "partial status mapped from incomplete"
            // The full enforced list — including tokens already present — is printed,
            // so the consumer recovers the whole shape, not just the gap (RC-1/RC-2).
            Expect.stringContains diag "full-required-set: small, medium, broad, required evidence, broad validation" "full enforced list"
            Expect.stringContains diag "absent-from-file: broad validation" "absent subset is the actual gap"
        }

        test "FR-004 a satisfied scan prints no readiness diagnostic" {
            let input = { ReadinessDir = "specs/x/readiness"; FeatureText = "no markers"; ReadinessFiles = [] }
            // readinessContract always checks its three files; a fully-populated set yields no hits.
            let populated =
                { input with
                    ReadinessFiles =
                        [ "governance-risk-levels.md", "small medium broad required evidence broad validation\n"
                          "aggregate-hang-diagnostics.md", "verdict stage elapsed duration last observed command focused rerun non-authoritative aggregate\n"
                          "runtime-limitations.md", ".NET 10 desktop OpenGL SkiaSharp preview unsupported macOS/mobile/browser no software-renderer fallback\n" ] }
            let rc = Scans.readinessContract populated
            Expect.equal (Render.readinessContractDiagnostics rc) "" "no hits -> empty diagnostic"
        }

        // --- FR-003 / D6 / FB-5: four-prompt feedback contract --------------
        test "FR-003 the feedback skill enumerates exactly four prompts and a Skill gaps section" {
            let body = read "template/feedback/skill/SKILL.md"
            for marker in [ "1."; "2."; "3."; "4." ] do
                Expect.stringContains body marker (sprintf "feedback skill enumerates prompt %s" marker)
            Expect.isFalse (body.Contains "5.") "no fifth prompt"
            Expect.stringContains body "## Skill gaps" "record schema gains the Skill gaps section (FB-2)"
            Expect.stringContains body "additional or new skills" "fourth prompt wording (FB-1)"
        }

        test "FB-4 no surviving 'three prompts' reference in the 058 sourcing contract" {
            let contract = read "specs/058-skills-quality-feedback/contracts/feedback-capture.md"
            Expect.isFalse (contract.ToLowerInvariant().Contains "three prompts") "058 contract states four prompts"
        }

        // --- FR-005 (DC-1/DC-2): single defect-class spelling ---------------
        // Feature 062 (FR-005, D5) single-sourced the window-visibility diagnostic
        // classes into EvidenceFormatSchema.windowDiagnosticClasses, which Scans.fs now
        // references; the canonical `product-defect` spelling lives there. Re-pointed to
        // the new single source — the assertion (one spelling, no project-prefixed
        // variant) is unchanged.
        test "FR-005 the readiness scan requires one defect-class spelling: product-defect" {
            let schema = read "build/Governance/Evidence/EvidenceFormatSchema.fs"
            Expect.stringContains schema "product-defect" "audit requires product-defect"
            Expect.isFalse (schema.Contains "-defect\"" && schema.Contains "breakoutdemo") "no project-prefixed <project>-defect literal"
            // Scans.fs single-sources the class set from the schema (no divergent literal).
            let scans = read "build/Governance/Evidence/Scans.fs"
            Expect.stringContains scans "EvidenceFormatSchema.windowDiagnosticClasses" "Scans single-sources the diagnostic-class set"
        }

        // --- FR-006 (BD-3): Dev vs Test/Verify quickstart guidance ----------
        test "FR-006 generated quickstart distinguishes Dev from Test/Verify" {
            for p in [ "template/base/README.md"; "template/base/docs/product.md" ] do
                let body = read p
                Expect.stringContains body "Verify" (sprintf "%s names the authoritative compile/test path" p)
                Expect.stringContains body "dotnet test" (sprintf "%s names dotnet test" p)
        }

        // --- FR-008 (BD-5): plan template self-describes GeneratedGuidanceCheck
        test "FR-008 the plan template inlines the GeneratedGuidanceCheck pass criteria" {
            for p in [ ".specify/presets/fsharp-opinionated/templates/plan-template.md"
                       ".specify/templates/plan-template.md" ] do
                let body = read p
                Expect.stringContains body "GeneratedGuidanceCheck" (sprintf "%s names the gate" p)
        }

        // --- FR-009 (BD-6): exact preset tasks-template path ----------------
        test "FR-009 speckit-tasks names the exact preset template path" {
            let skill = read ".agents/skills/speckit-tasks/SKILL.md"
            Expect.stringContains skill ".specify/presets/fsharp-opinionated/templates/tasks-template.md" "preset tasks-template path"
            Expect.stringContains skill ".specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml" "preset tasks-deps path"
        }

        // --- FR-010 (BD-4): consumer-internal duplicate-DU pitfall ----------
        test "FR-010 the keyboard pitfalls note covers consumer-internal DU collisions" {
            let skill = read "template/product-skills/fs-skia-keyboard-input/SKILL.md"
            Expect.stringContains skill "GameMode.Launch" "consumer-internal DU example"
            Expect.stringContains skill "Msg.Launch" "consumer-internal DU example"
        }
    ]
