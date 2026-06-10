module Feature090GovernanceTests

// Feature 090 US3 (RESPONDS-EVIDENCE-1, FR-007/P4/SC-006): the responds-vs-renders obligation is
// recorded durably in the evidence-mode skill tree, and the generated `.claude` mirror is
// byte-identical to its canonical `.agents` source (`SkillSyncCheck`).

open Expecto
open GovernanceTestSupport

let private agentsSkill = ".agents/skills/fs-skia-evidence-mode/SKILL.md"
let private claudeSkill = ".claude/skills/fs-skia-evidence-mode/SKILL.md"

[<Tests>]
let feature090GovernanceTests =
    testList "Feature 090 responds-vs-renders obligation" [

        // P4 — the durable obligation text binds future interactive-UI stories; it lives in BOTH the
        // canonical `.agents` source and its generated `.claude` mirror.
        test "the responds-proof obligation is present in the evidence-mode skill tree (FR-007, P4)" {
            [ agentsSkill; claudeSkill ]
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "responds-proof"
                      "input→visible-change"
                      "renders but does not respond"
                      "Inert"
                      "verdict=Responsive"
                      "captureRespondsProof" ])
        }

        // SC-006 — the `.claude` mirror is byte-identical to its `.agents` source (SkillSyncCheck).
        test ".claude evidence-mode skill is byte-identical to its .agents source (SkillSyncCheck, SC-006)" {
            let agents = read agentsSkill
            let claude = read claudeSkill
            Expect.equal claude agents ".claude evidence-mode skill mirrors its .agents source byte-for-byte"
        }
    ]
