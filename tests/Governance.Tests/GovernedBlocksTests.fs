module GovernedBlocksTests

(* Feature 057 / US1 — failing-first (Principle I/VI) typed tests for the canonical
   governed-block store + marker-splice render + currency checker. Exercises the real
   `FS.Skia.UI.Build.GovernedBlocks` functions on in-memory fixtures (no mocks); before
   GovernedBlocks.fs existed they did not compile. The splice test is a byte-equality
   assertion over the out-of-marker spans (FR-006). *)

open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.GovernedBlocks
open GovernanceTestSupport

let private norm (s: string) = s.Replace("\r\n", "\n")

let private fixtureBlock =
    { Id = "demo-block"
      CanonicalText = "Canonical [WHO] prose that every home file must carry identically."
      Targets = [ "a.md", Verbatim ]
      Tokens = []
      Obligations = [] }

[<Tests>]
let governedBlocksTests =
    testList "Feature 057 GovernedBlocks render + splice + currency" [

        test "renderText is verbatim for Verbatim and fills placeholders for Substituted" {
            Expect.equal
                (renderText Verbatim fixtureBlock.CanonicalText)
                fixtureBlock.CanonicalText
                "Verbatim preserves the placeholder-bearing canonical text"

            let substituted = renderText (Substituted(Map.ofList [ "WHO", "framework" ])) fixtureBlock.CanonicalText
            Expect.stringContains substituted "Canonical framework prose" "the [WHO] placeholder is substituted"
            Expect.isFalse (substituted.Contains "[WHO]") "no placeholder remains after substitution"
        }

        test "splice replaces only the inner region and preserves every out-of-marker byte (FR-006)" {
            let before = "PREAMBLE ONE\nPREAMBLE TWO\n"
            let after = "\nTRAILING PROSE that must survive byte-for-byte.\n"
            let fileText =
                before
                + "<!-- BEGIN GENERATED: gov/demo-block -->\nstale inner\n<!-- END GENERATED: gov/demo-block -->"
                + after

            let spliced = splice fixtureBlock Verbatim fileText

            Expect.isTrue (spliced.StartsWith before) "bytes before the BEGIN marker are preserved"
            Expect.isTrue (spliced.EndsWith after) "bytes after the END marker are preserved"
            Expect.stringContains spliced "Canonical [WHO] prose" "the canonical text was spliced in"
            Expect.isFalse (spliced.Contains "stale inner") "the stale inner text is gone"
            Expect.equal (splice fixtureBlock Verbatim spliced) spliced "splice is idempotent on a current file"
        }

        test "splice leaves an unknown marker id byte-for-byte untouched (honest drift)" {
            let fileText = "<!-- BEGIN GENERATED: gov/not-this-block -->\nx\n<!-- END GENERATED: gov/not-this-block -->\n"
            Expect.equal (splice fixtureBlock Verbatim fileText) fileText "a region for another block id is untouched"
        }

        test "currency reports Current for a faithful regeneration and Stale for a tampered copy naming file + source" {
            let current = splice fixtureBlock Verbatim "<!-- BEGIN GENERATED: gov/demo-block -->\nx\n<!-- END GENERATED: gov/demo-block -->\n"
            let cur = currency "a.md" fixtureBlock Verbatim current
            Expect.equal cur.Status Current "a freshly spliced region is current"
            Expect.isNone (currencyDrift cur) "no diagnostic when current"

            let tampered = current.Replace("Canonical [WHO] prose", "tampered prose")
            let drifted = currency "a.md" fixtureBlock Verbatim tampered
            Expect.equal drifted.Status Stale "a hand-edited region is stale"
            let diag = currencyDrift drifted
            Expect.isSome diag "a Some diagnostic on drift"
            Expect.stringContains diag.Value "a.md" "the diagnostic names the drifted file"
            Expect.stringContains diag.Value "demo-block" "the diagnostic names the canonical source block"
        }

        test "currency reports Missing when the file carries no region for the block" {
            let cur = currency "a.md" fixtureBlock Verbatim "no markers here at all\n"
            Expect.equal cur.Status Missing "absent region is Missing"
            Expect.isSome (currencyDrift cur) "Missing yields a diagnostic"
        }

        test "the canonical store blocks each declare at least one target and a marker id" {
            Expect.isNonEmpty governedBlocks "the governed-block inventory is non-empty"
            governedBlocks
            |> List.iter (fun b ->
                Expect.isNonEmpty b.Targets $"block {b.Id} declares targets"
                Expect.isFalse (b.Id = "") "block has a non-empty id")
        }

        // Feature 057 (US1, class 4 / FR-007): the placeholder-bearing twin is canonical;
        // constitution.md is its concrete render and the preset twin is a verbatim copy.
        // These golden tests read the real repo files, so a wrong substitution edit fails
        // here (and `TargetMetadataDrift` in the build) rather than silently churning the
        // committed constitution.
        test "renderConstitution Concrete reproduces the committed constitution.md byte-for-byte" {
            let canonical = read constitutionCanonicalRel
            let expected = read constitutionConcreteRel
            let rendered = renderConstitution Concrete canonical
            Expect.equal (norm rendered) (norm expected) "concrete render equals committed constitution.md"
        }

        test "the two constitution twins are byte-identical (verbatim render)" {
            let canonical = read constitutionCanonicalRel
            let twin = read constitutionTwinRel
            Expect.equal (norm (renderConstitution Twin canonical)) (norm twin) "preset twin equals canonical twin"
        }

        test "constitutionDrift flags a tampered constitution render naming file + source" {
            let canonical = read constitutionCanonicalRel
            let rendered = renderConstitution Concrete canonical
            Expect.isNone (constitutionDrift constitutionConcreteRel constitutionCanonicalRel rendered (read constitutionConcreteRel)) "current render is not flagged"
            let drift = constitutionDrift constitutionConcreteRel constitutionCanonicalRel rendered "tampered content"
            Expect.isSome drift "a tampered copy is flagged"
            Expect.stringContains drift.Value constitutionConcreteRel "diagnostic names the drifted file"
            Expect.stringContains drift.Value constitutionCanonicalRel "diagnostic names the canonical source"
        }
    ]
