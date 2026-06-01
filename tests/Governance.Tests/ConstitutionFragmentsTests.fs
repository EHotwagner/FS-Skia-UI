module ConstitutionFragmentsTests

(* Feature 044 / US3 — failing-first (Principle I/VI) typed tests for the constitution
   principle-fragment extractor + marker splice + currency checker. Exercises the real
   `FS.Skia.UI.Build.ConstitutionFragments` functions on in-memory fixtures (no mocks);
   before ConstitutionFragments.fs existed they did not compile. The splice test is a
   byte-equality assertion over the out-of-marker spans (FR-010). *)

open Expecto
open FS.Skia.UI.Build

let private fixtureConstitution =
    String.concat "\n"
        [ "# Constitution"
          ""
          "### I. Spec First"
          ""
          "Every change MUST start with a spec. More words here."
          ""
          "### II. Visibility Lives in `.fsi`"
          ""
          "Every public F# module MUST have a corresponding `.fsi` signature file. Extra prose."
          ""
          "### III. Simplicity"
          ""
          "Idiomatic is the default."
          ""
          "### IV. Elmish/MVU Is the Boundary"
          ""
          "Stateful or I/O workflows MUST be modeled through an MVU boundary first. And so on."
          ""
          "### V. Synthetic Evidence Requires Disclosure"
          ""
          "Synthetic evidence MUST be disclosed at every surface it appears. Repeatedly."
          ""
          "### VI. Test Evidence Is Mandatory"
          ""
          "Behavior-changing code MUST include tests that fail before and pass after. Always."
          "" ]

[<Tests>]
let constitutionFragmentsTests =
    testList "Feature 044 ConstitutionFragments extract + splice + currency" [

        test "extract derives the fixed fragment set deterministically from the headings" {
            let fragments = ConstitutionFragments.extract fixtureConstitution
            let ids = fragments |> List.map (fun f -> f.FragmentId)
            Expect.equal ids ConstitutionFragments.fragmentIds "the locked fragment inventory, in order"

            let fsi = fragments |> List.find (fun f -> f.FragmentId = "fsi-visibility")
            Expect.stringContains fsi.RenderedText "Visibility Lives" "fsi-visibility derives from Principle II heading"
            Expect.stringContains fsi.RenderedText "corresponding `.fsi` signature file." "first sentence of the body is captured"
        }

        test "extract raises when a required principle heading is missing (Principle VII)" {
            let broken = fixtureConstitution.Replace("### IV. Elmish/MVU Is the Boundary", "### IV-renamed")
            Expect.throws (fun () -> ConstitutionFragments.extract broken |> ignore) "missing required heading is a hard error"
        }

        test "regions locates every BEGIN/END GENERATED pair by id" {
            let template =
                String.concat "\n"
                    [ "Header prose."
                      "<!-- BEGIN GENERATED: constitution/tests-first -->"
                      "old text"
                      "<!-- END GENERATED: constitution/tests-first -->"
                      "Footer prose." ]
            let regions = ConstitutionFragments.regions template
            Expect.equal (regions |> List.map (fun r -> r.FragmentId)) [ "tests-first" ] "one region found by id"
        }

        test "splice replaces only the inner region and preserves every out-of-marker byte (FR-010)" {
            let fragments = ConstitutionFragments.extract fixtureConstitution
            let before = "PREAMBLE LINE ONE\nPREAMBLE LINE TWO\n"
            let after = "\nTRAILING PROSE that must survive byte-for-byte.\n"
            let template =
                before
                + "<!-- BEGIN GENERATED: constitution/mvu-boundary -->\nstale inner\n<!-- END GENERATED: constitution/mvu-boundary -->"
                + after

            let spliced = ConstitutionFragments.splice fragments template

            Expect.isTrue (spliced.StartsWith before) "bytes before the BEGIN marker are preserved"
            Expect.isTrue (spliced.EndsWith after) "bytes after the END marker are preserved"
            Expect.stringContains spliced "Elmish/MVU Is the Boundary" "the mvu-boundary fragment text was spliced in"
            Expect.isFalse (spliced.Contains "stale inner") "the stale inner text is gone"

            // Idempotence: splicing an already-current template is a fixed point.
            Expect.equal (ConstitutionFragments.splice fragments spliced) spliced "splice is idempotent on a current template"
        }

        test "currency flags a stale region after a simulated principle edit and passes a current one" {
            let fragments = ConstitutionFragments.extract fixtureConstitution
            let template =
                "<!-- BEGIN GENERATED: constitution/tests-first -->\nstale\n<!-- END GENERATED: constitution/tests-first -->\n"
            let onlyTestsFirst = fragments |> List.filter (fun f -> f.FragmentId = "tests-first")

            let staleCurrency = ConstitutionFragments.currency "tasks-template.md" onlyTestsFirst template
            Expect.equal staleCurrency.StaleFragments [ "tests-first" ] "the stale region is flagged"
            Expect.isSome (ConstitutionFragments.currencyDrift staleCurrency) "a Some diagnostic on drift"

            let current = ConstitutionFragments.splice fragments template
            let freshCurrency = ConstitutionFragments.currency "tasks-template.md" onlyTestsFirst current
            Expect.isEmpty freshCurrency.StaleFragments "a freshly spliced template is current"
            Expect.isNone (ConstitutionFragments.currencyDrift freshCurrency) "no diagnostic when current"
        }

        test "currency reports a missing marker and an unknown marker" {
            let fragments = ConstitutionFragments.extract fixtureConstitution
            // template carries an UNKNOWN marker id and omits the expected one
            let template =
                "<!-- BEGIN GENERATED: constitution/not-a-real-fragment -->\nx\n<!-- END GENERATED: constitution/not-a-real-fragment -->\n"
            let expectMvu = fragments |> List.filter (fun f -> f.FragmentId = "mvu-boundary")
            let cur = ConstitutionFragments.currency "plan-template.md" expectMvu template
            Expect.contains cur.MissingMarkers "mvu-boundary" "the expected fragment has no region → missing"
            Expect.contains cur.UnknownMarkers "not-a-real-fragment" "a region with no source fragment → unknown"
        }
    ]
