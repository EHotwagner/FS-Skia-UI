module SkillTreeGenTests

(* Feature 044 / US1 — failing-first (Principle I/VI) typed tests for the
   canonical-to-derived skill-tree generator + currency checker. These exercise the
   real `FS.Skia.UI.Build.SkillTreeGen` functions over in-memory SkillFile lists (no
   repo-tree dependence, no mocks); before SkillTreeGen.fs existed they did not compile.

   Coverage proof (SC-002): the enumeration covers a synthetic 26th skill that is in NO
   allowlist. Byte-identity (SC-001): the derived plan reproduces canonical bytes. A
   tampered derived byte yields a Some currency diagnostic. Empty/unreadable canonical
   input raises a generator error (Principle VII). *)

open System.Text
open Expecto
open FS.Skia.UI.Build

let private bytes (s: string) = Encoding.UTF8.GetBytes s

// Annotated constructors disambiguate the shared `Bytes` field between SkillFile and GenPlanEntry.
let private mkFile slug rel b : SkillTreeGen.SkillFile = { Slug = slug; RelPath = rel; Bytes = b }

let private canonicalFile slug body =
    mkFile slug (sprintf ".agents/skills/%s/SKILL.md" slug) (bytes body)

let private derivedFile (entry: SkillTreeGen.GenPlanEntry) = mkFile "" entry.DerivedRelPath entry.Bytes

let private manifestFile (plan: SkillTreeGen.SkillTreePlan) = mkFile "" plan.ManifestRelPath plan.ManifestBytes

// A representative canonical set INCLUDING a 26th slug outside any historical allowlist.
let private canonical =
    [ canonicalFile "fsharp-parsing" "# fsharp-parsing\nbody A\n"
      canonicalFile "fsharp-io-globbing" "# fsharp-io-globbing\nbody B\n"
      canonicalFile "speckit-merge" "# speckit-merge\nbody C\n"
      canonicalFile "a-brand-new-26th-skill" "# new skill\nbody Z\n" ]

[<Tests>]
let skillTreeGenTests =
    testList "Feature 044 SkillTreeGen generation + currency" [

        test "derivedRelPath translates only the tree root" {
            Expect.equal
                (SkillTreeGen.derivedRelPath ".agents/skills/fsharp-parsing/SKILL.md")
                ".claude/skills/fsharp-parsing/SKILL.md"
                "agents root swapped for claude root, rest preserved"
        }

        test "plan enumerates every canonical file incl. a 26th non-allowlisted skill (SC-002)" {
            let plan = SkillTreeGen.plan canonical
            let derivedPaths = plan.Entries |> List.map (fun e -> e.DerivedRelPath)

            Expect.contains derivedPaths ".claude/skills/a-brand-new-26th-skill/SKILL.md" "the 26th skill is covered by enumeration, no allowlist edit"
            Expect.equal (List.length plan.Entries) (List.length canonical) "one derived entry per canonical file"
            Expect.stringContains (Encoding.UTF8.GetString plan.ManifestBytes) "RefreshSurfaceBaselines" "manifest names the regeneration command"
        }

        test "derived plan reproduces canonical bytes exactly (SC-001 byte-identity)" {
            let plan = SkillTreeGen.plan canonical
            let derived = manifestFile plan :: (plan.Entries |> List.map derivedFile)
            let cur = SkillTreeGen.currency plan derived
            Expect.isTrue (SkillTreeGen.isCurrent cur) "a faithful regeneration is current"
            Expect.isNone (SkillTreeGen.currencyDrift cur) "no drift diagnostic when current"
        }

        test "a tampered derived byte yields a Some currency diagnostic naming the regen target" {
            let plan = SkillTreeGen.plan canonical

            let tampered =
                plan.Entries
                |> List.map (fun e ->
                    if e.DerivedRelPath.Contains "fsharp-parsing" then
                        mkFile "" e.DerivedRelPath (bytes "tampered!\n")
                    else
                        derivedFile e)

            let cur = SkillTreeGen.currency plan (manifestFile plan :: tampered)
            Expect.isFalse (SkillTreeGen.isCurrent cur) "tampered derived tree is not current"
            Expect.contains cur.StaleSlugs "fsharp-parsing" "the tampered slug is named stale"

            match SkillTreeGen.currencyDrift cur with
            | Some msg -> Expect.stringContains msg "RefreshSurfaceBaselines" "diagnostic names the regeneration command (FR-012)"
            | None -> failtest "expected a Some drift diagnostic"
        }

        test "a missing derived file and an orphan are both reported" {
            let plan = SkillTreeGen.plan canonical
            // drop one expected file, add one orphan with no canonical source
            let derived =
                (plan.Entries |> List.tail |> List.map derivedFile)
                @ [ mkFile "" ".claude/skills/ghost/SKILL.md" (bytes "ghost\n") ]

            let cur = SkillTreeGen.currency plan derived
            Expect.isFalse (List.isEmpty cur.MissingDerived) "the dropped file is reported missing"
            Expect.contains cur.OrphanDerived ".claude/skills/ghost/SKILL.md" "the orphan is reported"
        }

        test "empty canonical input raises rather than emitting a partial tree (Principle VII)" {
            Expect.throws (fun () -> SkillTreeGen.plan [] |> ignore) "an empty canonical set is a hard error"
        }

        test "unreadable (null-bytes) canonical input raises (Principle VII)" {
            let broken = [ mkFile "x" ".agents/skills/x/SKILL.md" (Unchecked.defaultof<byte[]>) ]
            Expect.throws (fun () -> SkillTreeGen.plan broken |> ignore) "an unreadable canonical file is a hard error"
        }
    ]
