module ControlsPreview.HarnessTests

// Feature 079 — failing-first governance tests for the render harness (Principle I/VI).
// T008 (P1): the single sample source is TOTAL over catalogFacts (no gap/orphan) and every
//            Demonstrative entry renders recognizable, non-empty content (no empty widget).
// T009 (P4/FR-008/SC-004): the render harness is idempotent — re-rendering yields
//            byte-identical PNGs — and the committed assets match a fresh render of the source.
//
// These exercise the REAL render-only path and so require a render-capable host; they live in
// this harness project (which references the typed Controls front door + SkiaViewer + SkiaSharp)
// rather than the SkiaSharp-free FS.Skia.UI.Build test project, and are deliberately NOT in
// defaultTestProjects (GPU-free CI Dev/Test would fail). Run on a render-capable host:
//   dotnet run --project tests/ControlsPreview.Harness -- --sequenced

open System.IO
open Expecto
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Build
open ControlsPreview.Samples
open ControlsPreview.Render

let private catalogOrder = CatalogGen.catalogFacts |> List.map (fun f -> f.Id)
let private catalogIds = catalogOrder |> Set.ofList
let private sampleIdSet = sampleIds |> Set.ofList

[<Tests>]
let totalityTests =
    testList "Feature 079 sample-source totality + explicitness (T008, P1)" [
        test "sample ids set-equal catalogFacts ids (P1.1 totality)" {
            Expect.equal sampleIdSet catalogIds "every catalog id has exactly one sample def; no gap, no orphan"
        }

        test "sample ids are unique and follow catalogFacts order" {
            Expect.equal (List.length sampleIds) (List.length (List.distinct sampleIds)) "no duplicate sample ids"
            Expect.equal sampleIds catalogOrder "the single source is declared in catalogFacts order"
        }

        test "every Demonstrative entry renders non-empty content via the render-only path (P1.2)" {
            // Renders each demonstrative sample and asserts a real ScreenshotOk plus committed
            // bytes above the pinned trivial floor — proving none renders an empty-content widget.
            for s in demonstrative do
                let tmp = Path.Combine(Path.GetTempPath(), sprintf "fs079-explicit-%s.png" s.Id)
                let r = renderWidgetToPng (fst s.Canvas) (snd s.Canvas) tmp ((Option.get s.Build) ())
                Expect.equal r.Status ScreenshotOk (sprintf "%s renders via ViewerRenderTargetPng" s.Id)
                let bytes = (FileInfo tmp).Length
                Expect.isGreaterThan
                    bytes
                    CatalogDocsGen.trivialPreviewFloorBytes
                    (sprintf "%s preview has non-trivial content (> %d-byte floor)" s.Id CatalogDocsGen.trivialPreviewFloorBytes)
        }

        test "exactly one honest Unsupported entry (custom-control)" {
            Expect.equal
                (unsupportedSamples |> List.map (fun s -> s.Id))
                [ "custom-control" ]
                "custom-control is the only unsupported entry; the rest are demonstrative"
        }
    ]

[<Tests>]
let idempotenceTests =
    testList "Feature 079 render idempotence (T009, P4/FR-008/SC-004)" [
        test "re-rendering each sample yields byte-identical PNGs" {
            for s in demonstrative do
                let a = Path.Combine(Path.GetTempPath(), sprintf "fs079-idem-a-%s.png" s.Id)
                let b = Path.Combine(Path.GetTempPath(), sprintf "fs079-idem-b-%s.png" s.Id)
                renderWidgetToPng (fst s.Canvas) (snd s.Canvas) a ((Option.get s.Build) ()) |> ignore
                renderWidgetToPng (fst s.Canvas) (snd s.Canvas) b ((Option.get s.Build) ()) |> ignore
                Expect.equal (File.ReadAllBytes a) (File.ReadAllBytes b) (sprintf "%s renders byte-identically across runs" s.Id)
        }

        test "each committed preview matches a fresh render of the same source" {
            // Stronger guarantee than self-idempotence: the committed docs/img/controls/<id>.png
            // equals a fresh render, so the committed assets are provably from this single source.
            let root = repositoryRoot ()
            for s in demonstrative do
                let committed = previewPath root s.Id
                Expect.isTrue (File.Exists committed) (sprintf "committed preview present for %s (run with --render first)" s.Id)
                let fresh = Path.Combine(Path.GetTempPath(), sprintf "fs079-fresh-%s.png" s.Id)
                renderWidgetToPng (fst s.Canvas) (snd s.Canvas) fresh ((Option.get s.Build) ()) |> ignore
                Expect.equal (File.ReadAllBytes committed) (File.ReadAllBytes fresh) (sprintf "%s committed bytes match a fresh render" s.Id)
        }

        test "no committed image exists for an Unsupported entry" {
            let root = repositoryRoot ()
            for s in unsupportedSamples do
                Expect.isFalse (File.Exists(previewPath root s.Id)) (sprintf "%s is unsupported and commits no image" s.Id)
        }
    ]
