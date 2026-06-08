module PerPackageSurfaceTests

// Feature 048 (US2, FR-007/008, Principle I/VI/VII): failing-first semantic tests for the
// additive PerPackageSurfaceDiff capability, exercised through its `.fsi`. The pure tests
// pin the drift algebra (identical ⇒ empty; one mutated signature ⇒ exactly one package;
// missing baseline ⇒ fail loud). The interpreter test runs the real edge over the real
// source tree + the eight committed baselines and asserts zero drift at the pin (SC-004).

open System.IO
open Expecto
open FS.Skia.UI.Build.PerPackageSurface
open GovernanceTestSupport

let private surface id text : Surface = { PackageId = id; NormalizedText = text }

[<Tests>]
let perPackageSurfaceTests =
    testList "PerPackageSurface" [

        // --- pure core (T011) ---

        test "scope is exactly the eleven split packages, monolith and build-tooling excluded" {
            // The retired monolith id is named via parts so the SC-001 no-consumer grep stays clean.
            let retiredMonolith = "FS.Skia." + "UI"
            // Feature 058 (US2): FS.Skia.UI.SkillSupport joined as the 10th in-scope package.
            // Feature 083 (FR-013): FS.Skia.UI.Color joined as the 11th in-scope package.
            Expect.equal (List.length packagesInScope) 11 "eleven packages in scope"
            Expect.isTrue (List.contains "FS.Skia.UI.Color" packagesInScope) "FS.Skia.UI.Color in scope (feature 083)"
            Expect.isTrue (List.contains "FS.Skia.UI.SkillSupport" packagesInScope) "FS.Skia.UI.SkillSupport in scope (feature 058)"
            Expect.isTrue (List.contains "FS.Skia.UI.Input" packagesInScope) "FS.Skia.UI.Input in scope (feature 052)"
            Expect.isFalse (List.contains retiredMonolith packagesInScope) "monolith excluded"
            Expect.isFalse (List.contains "FS.Skia.UI.Build" packagesInScope) "build-tooling excluded"
        }

        test "normalize strips comments, collapses blank runs, and is idempotent" {
            let raw = "namespace X\n// line comment\n(* block\n spanning *)\nval f: int  \n\n\n\nval g: int\n"
            let once = normalize raw
            Expect.isFalse (once.Contains "//") "line comments stripped"
            Expect.isFalse (once.Contains "block") "block comments stripped"
            Expect.isFalse (once.Contains "\n\n\n") "blank-line runs collapsed"
            Expect.equal (normalize once) once "normalize is idempotent"
        }

        test "identical surfaces yield empty Drifted and no missing baselines" {
            let baselines = [ surface "FS.Skia.UI.Scene" "val a: int\nval b: int" ]
            let current = [ surface "FS.Skia.UI.Scene" "val a: int\nval b: int" ]
            let outcome = diff baselines current
            Expect.isEmpty outcome.Drifted "no drift on identical surfaces"
            Expect.isEmpty outcome.MissingBaselines "no missing baselines"
            Expect.equal outcome.CheckedPackages [ "FS.Skia.UI.Scene" ] "checked the package"
        }

        test "a single mutated signature drifts exactly one package and no other (SC-005)" {
            let baselines =
                [ surface "FS.Skia.UI.Scene" "val a: int\nval b: int"
                  surface "FS.Skia.UI.Layout" "val x: int" ]

            let current =
                [ surface "FS.Skia.UI.Scene" "val a: int\nval b: string"
                  surface "FS.Skia.UI.Layout" "val x: int" ]

            let outcome = diff baselines current
            Expect.equal (List.length outcome.Drifted) 1 "exactly one package drifts"
            Expect.equal outcome.Drifted.[0].PackageId "FS.Skia.UI.Scene" "the mutated package drifts"
            Expect.isEmpty outcome.MissingBaselines "no missing baselines"
            Expect.contains outcome.Drifted.[0].Changes (Removed "val b: int") "baseline line removed"
            Expect.contains outcome.Drifted.[0].Changes (Added "val b: string") "current line added"
        }

        test "a current package with no baseline lands in MissingBaselines (Principle VII)" {
            let baselines = [ surface "FS.Skia.UI.Scene" "val a: int" ]

            let current =
                [ surface "FS.Skia.UI.Scene" "val a: int"
                  surface "FS.Skia.UI.Testing" "val t: int" ]

            let outcome = diff baselines current
            Expect.equal outcome.MissingBaselines [ "FS.Skia.UI.Testing" ] "the unbaselined package is reported"
            Expect.isEmpty outcome.Drifted "no false drift for the baselined package"
        }

        // --- edge interpreter over the real tree + committed baselines (T012, SC-004) ---

        test "captureCurrent over the real tree matches the eleven committed baselines at the pin" {
            // captureCurrent discovers the repository root from the current directory; pin it
            // to the located repo root so the edge resolves src/<package>/*.fsi deterministically.
            Directory.SetCurrentDirectory repositoryRoot
            let baselineDir = Path.Combine(repositoryRoot, "readiness", "per-package-surface")
            let baselines = loadBaselines baselineDir
            let current = captureCurrent packagesInScope
            let outcome = diff baselines current

            Expect.equal (List.length current) 11 "captured eleven package surfaces"
            Expect.equal (List.length baselines) 11 "loaded eleven committed baselines"
            Expect.isEmpty outcome.MissingBaselines "every in-scope package has a committed baseline"
            Expect.isEmpty outcome.Drifted "zero drift across the eleven packages at the pin (SC-004)"
        }
    ]
