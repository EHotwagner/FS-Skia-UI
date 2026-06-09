module Feature084GovernanceTests

// Feature 084 — failing-first (Principle I/VI) governance coverage for the
// window-options consumer follow-ups:
//   * FR-007 / SC-003 (T017): the rendered evidence-formats.md window-visibility
//     file list equals the engine's single-sourced required set, so the shipped
//     doc cannot drift behind the scan.
//   * FR-008 / FR-009 / SC-004 (T018): the audit's per-blocker stdout block names
//     area + file + one-line reason + hit-file path, and the diff-scan base-ref
//     line is resolved or explicitly absent (never misleading).
//   * FR-010 / FR-011 / SC-005 (T024): scaffold-map.md uses project-named paths, a
//     durable-but-must-re-point class, and a non-game layout-region remap example.
// All assertions call the real pure functions / read the shipped doc — no log scraping.

open Expecto
open FS.Skia.UI.Build.Evidence
open GovernanceTestSupport

[<Tests>]
let feature084GovernanceTests =
    testList "feature 084 window-options consumer follow-ups" [

        // --- FR-007 / SC-003 (T017): evidence-formats ↔ required-set parity ----
        test "FR-007 SC-003 the evidence-formats window-visibility list equals the engine required set" {
            let schemaFiles =
                EvidenceFormatSchema.schemas
                |> List.filter (fun s -> s.FormatClass = WindowVisibility)
                |> List.map (fun s -> s.FileName)

            Expect.equal schemaFiles EvidenceFormatSchema.windowVisibilityFiles "schema window-visibility entries equal the single-sourced required file set"
            Expect.hasLength schemaFiles 7 "all seven engine-required window-visibility files are enumerated"

            let rendered = EvidenceFormatSchema.renderReferenceDoc ()
            for fileName in EvidenceFormatSchema.windowVisibilityFiles do
                Expect.stringContains rendered (sprintf "### `%s`" fileName) (sprintf "the shipped doc documents %s" fileName)
        }

        // --- FR-008 / SC-004 (T018): per-blocker legibility on stdout ----------
        test "FR-008 SC-004 audit blocker diagnostics name area, file, reason and hit-file per blocker" {
            let hit area path reason missing =
                { ScanHit.basic path reason with
                    Missing = missing
                    ValidationArea = Some area }

            let rc =
                { Area = "readiness-contract"
                  BlockingCount = 1
                  Hits = [ hit "readiness-contract" "specs/084/readiness/interactive-visible-window.md" "missing visible-window readiness fields" (Some [ "accessible-window"; "self-closed-for-evidence" ]) ] }

            let wv =
                { Area = "window-visibility"
                  BlockingCount = 1
                  Hits = [ hit "window-visibility" "specs/084/readiness/window-options.md" "missing option rows" (Some [ "startup-state" ]) ] }

            let out =
                Render.auditBlockerDiagnostics
                    [ "readiness-contract", "readiness-contract-hits.json", rc
                      "window-visibility", "window-visibility-hits.json", wv ]

            Expect.stringContains out "[readiness-contract] interactive-visible-window.md" "names the readiness-contract blocker file"
            Expect.stringContains out "reason: missing visible-window readiness fields" "prints the one-line reason"
            Expect.stringContains out "accessible-window, self-closed-for-evidence" "prints the absent terms"
            Expect.stringContains out "hit-file: readiness/readiness-contract-hits.json" "names the originating hit-file path"
            Expect.stringContains out "[window-visibility] window-options.md" "names the window-visibility blocker file"
            Expect.stringContains out "hit-file: readiness/window-visibility-hits.json" "names the window-visibility hit-file path"
        }

        test "FR-008 audit blocker diagnostics are empty when no area has hits" {
            let clean = { Area = "readiness-contract"; BlockingCount = 0; Hits = [] }
            Expect.equal (Render.auditBlockerDiagnostics [ "readiness-contract", "readiness-contract-hits.json", clean ]) "" "no hits yields no blocker block"
        }

        // --- FR-009 / SC-004 (T018): base-ref line is resolved or explicit -----
        test "FR-009 SC-004 the diff-scan base-ref line reports the resolved base or an explicit absence" {
            Expect.stringContains (Render.diffScanBaseRefLine (Some "main") (Some "abc1234")) "base_ref: main (merge-base abc1234)" "resolved base ref with merge-base sha"
            Expect.equal (Render.diffScanBaseRefLine (Some "main") None) "diff-scan base_ref: main" "resolved base ref without a merge-base sha"

            let absent = Render.diffScanBaseRefLine None None
            Expect.stringContains absent "none" "no default-branch ancestor is reported"
            Expect.stringContains absent "by absence, not a clean diff" "the absence is non-misleading (not a clean diff)"
        }

        // --- FR-010 / FR-011 / SC-005 (T024): trustworthy scaffold-map ---------
        test "FR-010 FR-011 SC-005 scaffold-map uses project-named paths, a must-re-point class, and a non-game remap" {
            let map = read "template/base/docs/scaffold-map.md"
            Expect.stringContains map "<ProjectName>" "uses the project-name placeholder, not a literal src/Product path"
            Expect.stringContains map "<ProductDir>" "uses the product-dir placeholder"
            Expect.stringContains map "src/<ProjectName>/**" "notes the generated tree path shape"
            Expect.stringContains map "must re-point" "names the durable-but-must-re-point class"
            Expect.stringContains map "re-pointing the model-field references" "defines durable as keep-file/re-point-model-fields"
            Expect.stringContains map "LayoutEvidence.fs" "classifies LayoutEvidence.fs as must-re-point"
            Expect.stringContains map "EvidenceCommands.fs" "classifies EvidenceCommands.fs as must-re-point"
            Expect.stringContains map "HUD region → headers" "non-game HUD remap example"
            Expect.stringContains map "Gameplay region → main content grid" "non-game gameplay remap example"
        }
    ]
