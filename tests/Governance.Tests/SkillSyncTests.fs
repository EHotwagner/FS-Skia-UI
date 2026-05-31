module SkillSyncTests

(* Feature 040 / US2 — failing-first (Principle I/VI) semantic tests for the
   SHA-256 byte-identity comparator and skill-pair discovery used by the
   SkillSyncCheck gate. These exercise the real `FS.Skia.UI.Build.SkillSync`
   functions (no mocks); before SkillSync.fs existed they did not compile. *)

open System
open System.IO
open System.Text
open Expecto
open GovernanceTestSupport
open FS.Skia.UI.Build

let private pair slug claudeHash agentsHash : SkillSync.SkillPairResult =
    { Slug = slug
      ClaudePath = SkillSync.claudeRelPath slug
      AgentsPath = SkillSync.agentsRelPath slug
      ClaudeHash = claudeHash
      AgentsHash = agentsHash }

[<Tests>]
let skillSyncTests =
    testList "Feature 040 SkillSync comparator and discovery" [

        test "sha256Hex matches the known digest of \"abc\"" {
            let digest = SkillSync.sha256Hex (Encoding.ASCII.GetBytes "abc")
            Expect.equal digest "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad" "lowercase hex SHA-256 of abc"
        }

        test "equal bytes are in sync; differing bytes are out of sync and named" {
            let h1 = SkillSync.sha256Hex (Encoding.UTF8.GetBytes "identical")
            let h2 = SkillSync.sha256Hex (Encoding.UTF8.GetBytes "different")

            let synced = pair "fsharp-parsing" (Some h1) (Some h1)
            let drift = pair "fsharp-io-globbing" (Some h1) (Some h2)

            Expect.isTrue (SkillSync.inSync synced) "equal digests are in sync"
            Expect.isFalse (SkillSync.inSync drift) "differing digests are out of sync"

            let bad = SkillSync.drifted [ synced; drift ]
            Expect.equal (bad |> List.map (fun r -> r.Slug)) [ "fsharp-io-globbing" ] "drifted names only the offender"

            let message = SkillSync.renderFailureMessage [ synced; drift ]
            Expect.stringContains message "fsharp-io-globbing" "failure message names the drifted slug"
            Expect.stringContains message h1 "failure message prints the .claude digest"
            Expect.stringContains message h2 "failure message prints the .agents digest"
        }

        test "a missing file on either side is a failure, never a skip (Principle VII)" {
            let missingClaude = pair "fsharp-shell-process" None (Some "deadbeef")
            Expect.isFalse (SkillSync.inSync missingClaude) "missing .claude copy is drift, not pass"
            let message = SkillSync.renderFailureMessage [ missingClaude ]
            Expect.stringContains message "<missing>" "failure message marks the missing side"
        }

        test "checkAll discovers exactly the six committed pairs, all in sync" {
            let results = SkillSync.checkAll repositoryRoot
            Expect.equal (results |> List.map (fun r -> r.Slug)) SkillSync.expectedSlugs "the six expected slugs, in order"
            let bad = SkillSync.drifted results
            Expect.isEmpty bad $"committed skills are byte-identical across both trees: {SkillSync.renderFailureMessage results}"
        }

        test "checkPair surfaces a one-sided file as missing over a synthetic root" {
            let root = Path.Combine(Path.GetTempPath(), "skillsync-" + Guid.NewGuid().ToString("N"))
            let slug = "fsharp-parsing"
            let claudeDir = Path.Combine(root, ".claude", "skills", slug)
            Directory.CreateDirectory claudeDir |> ignore
            File.WriteAllText(Path.Combine(claudeDir, "SKILL.md"), "only the claude side exists\n")

            try
                let result = SkillSync.checkPair root slug
                Expect.isSome result.ClaudeHash "claude side present"
                Expect.isNone result.AgentsHash "agents side absent → None, not an exception"
                Expect.isFalse (SkillSync.inSync result) "one-sided pair is never in sync"
            finally
                Directory.Delete(root, true)
        }
    ]
