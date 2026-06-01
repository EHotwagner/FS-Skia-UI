module SkillSyncTests

(* Feature 040 → reframed by feature 044 / US1. SkillSyncCheck is no longer a six-slug
   byte-identity PEER check; it is a generation-currency check over the whole tree
   (`.claude/skills` must be a current regeneration of canonical `.agents/skills`,
   covered by enumeration). These re-pointed tests exercise the reframed
   `FS.Skia.UI.Build.SkillSync` adapter on in-memory inputs (no mocks). *)

open System.Text
open Expecto
open FS.Skia.UI.Build

let private bytes (s: string) = Encoding.UTF8.GetBytes s

let private mkFile rel b : SkillTreeGen.SkillFile = { Slug = ""; RelPath = rel; Bytes = b }

let private canonicalFile slug body : SkillTreeGen.SkillFile =
    { Slug = slug; RelPath = sprintf ".agents/skills/%s/SKILL.md" slug; Bytes = bytes body }

let private derivedOf (plan: SkillTreeGen.SkillTreePlan) : SkillTreeGen.SkillFile list =
    mkFile plan.ManifestRelPath plan.ManifestBytes
    :: (plan.Entries |> List.map (fun e -> mkFile e.DerivedRelPath e.Bytes))

let private canonical =
    [ canonicalFile "fsharp-parsing" "alpha\n"
      canonicalFile "speckit-merge" "beta\n" ]

[<Tests>]
let skillSyncTests =
    testList "Feature 044 SkillSync currency adapter" [

        test "a faithful regeneration is current; the report is a PASS" {
            let plan = SkillSync.planFromCanonical canonical
            let cur = SkillSync.currency plan (derivedOf plan)
            Expect.isTrue (SkillSync.isCurrent cur) "faithful regeneration is current"
            Expect.equal (SkillSync.renderFailureMessage cur) "" "no failure message when current"
            Expect.stringContains (SkillSync.renderReport plan cur) "PASS" "PASS report body"
        }

        test "a drifted derived tree is not current and the failure message names the regen target (FR-012)" {
            let plan = SkillSync.planFromCanonical canonical

            let drifted =
                derivedOf plan
                |> List.map (fun f ->
                    if f.RelPath.Contains "fsharp-parsing" then mkFile f.RelPath (bytes "TAMPERED\n") else f)

            let cur = SkillSync.currency plan drifted
            Expect.isFalse (SkillSync.isCurrent cur) "tampered tree is not current"
            let message = SkillSync.renderFailureMessage cur
            Expect.stringContains message "RefreshSurfaceBaselines" "failure message names the regeneration command"
            Expect.stringContains (SkillSync.renderReport plan cur) "FAIL" "FAIL report body"
        }
    ]
