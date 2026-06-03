module ProseSizeAccountingTests

// Feature 055 (US3, T014): byte-determinism of the pure prose-size accounting render.
// The IO enumeration lives in the front-end; this exercises the pure function over a
// known record so the report format (corrected baseline, summed current count, signed
// delta, restated target) is asserted without touching disk (FR-007, SC-005).
open Expecto
open FS.Skia.UI.Build.Guidance

[<Tests>]
let proseSizeAccountingTests =
    testList "Prose-size accounting (render)" [
        test "renders the corrected baseline, summed current count, signed delta and restated target" {
            let accounting =
                { Baseline = 6882
                  AgentsSkillsLines = 4000
                  SpecifyLines = 2900
                  Current = 6900
                  Delta = 18
                  RestatedTarget =
                    "no fixed line count this feature; tracked against the corrected ≈6,882 baseline" }

            let report = renderProseSizeAccounting accounting

            Expect.stringContains report "Corrected baseline (feature 046): 6882 lines" "states the corrected baseline"
            Expect.stringContains report ".agents/skills/**/*.md`: 4000 lines" "states the .agents/skills count"
            Expect.stringContains report ".specify/**/*.md`: 2900 lines" "states the .specify count"
            Expect.stringContains report "Current measured guidance-prose count: 6900 lines" "states the summed current count"
            Expect.stringContains report "Delta vs baseline: +18 lines" "states the signed delta"
            Expect.stringContains report "corrected ≈6,882 baseline" "states the restated target"
            Expect.isFalse (report.Contains "23,000") "no longer cites the discredited ~23,000 figure as the target"
        }

        test "negative delta renders with a leading minus and no double sign" {
            let accounting =
                { Baseline = 6882
                  AgentsSkillsLines = 3900
                  SpecifyLines = 2900
                  Current = 6800
                  Delta = -82
                  RestatedTarget = "reduction follow-up bounded" }

            let report = renderProseSizeAccounting accounting
            Expect.stringContains report "Delta vs baseline: -82 lines" "negative delta keeps its single minus sign"
        }
    ]
