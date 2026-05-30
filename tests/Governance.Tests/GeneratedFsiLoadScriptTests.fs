module GeneratedFsiLoadScriptTests

(* US4 (spec 037, FR-009 / SC-005): a generated `.fsx` load script is emitted with
   every generated product. It loads the app plus its transitive FS.Skia.UI.*
   references in one FSI step and stays in sync with the assembly set (derived,
   not hand-maintained). These tests pin the template + build contract; the
   end-to-end FSI load of a generated app is captured in readiness evidence. *)

open Expecto
open GovernanceTestSupport

[<Tests>]
let generatedFsiLoadScriptTests =
    testList "US4 generated FSI load script" [

        test "template carries a generated load script referencing the app and transitive FS.Skia.UI set" {
            let script = read "template/base/load-product.fsx"
            [ "GENERATED — do not edit"
              "Product.dll"
              "FS.Skia.UI.Scene.dll"
              "FS.Skia.UI.Controls.dll"
              "open Product"
              "dotnet fsi load-product.fsx" ]
            |> List.iter (fun needle -> expectContains script needle $"load-product.fsx contains {needle}")
        }

        test "build emits the load script derived from the built Product output and requires it in the file list" {
            let build = read "build.fsx"
            // Emitted via GenerateV3Products, derived from the built output assembly set.
            expectContains build "emitFsiLoadScript" "build.fsx defines/calls emitFsiLoadScript"
            expectContains build "Directory.EnumerateFiles(outDir, \"FS.Skia.UI*.dll\")" "load script is derived from the built FS.Skia.UI assembly set"
            // Required in the generated-product file list.
            expectContains build "\"load-product.fsx\"" "scanV3GeneratedRow requires load-product.fsx"
        }

        test "generated guidance documents the single FSI-load step (SC-005)" {
            [ "template/base/README.md"; "template/base/docs/product.md" ]
            |> List.iter (fun path ->
                let content = read path
                expectContains content "load-product.fsx" $"{path} names the load script"
                expectContains content "dotnet fsi load-product.fsx" $"{path} documents the single FSI-load step")
        }

        test "load script preserves benign host-warning classification (spec 021)" {
            let script = read "template/base/load-product.fsx"
            // The script references and opens only; it must not suppress real failures.
            expectContains script "neither emits nor suppresses host warnings" "load script documents non-suppression"
            expectContains script "missing assembly" "load script notes real load failures still surface"
        }
    ]
