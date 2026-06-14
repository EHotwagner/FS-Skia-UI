module ControlsPreview.Render

// Feature 079 (US1, FR-003/FR-008 / R2) — the committed, compiled, deterministic render
// harness that feature 078 lacked. It loops the single sample source (PreviewSamples.fs)
// and produces each demonstrative preview through the SAME real render-only evidence path:
//   typed front door → Widget.toControl → Control.render Theme.light → SceneNode.Group
//   → SkiaViewer.captureScreenshotEvidence (CaptureMode = ViewerRenderTargetPng).
// Unsupported entries write NO image (and any stale one is removed). Rendering requires a
// render-capable host (Skia native libs); the docs build + currency gate stay GPU-free.
//
// Invoke the generator on a render-capable host:
//   dotnet run --project tests/ControlsPreview.Harness -- --render
// Re-running is idempotent: byte-identical PNGs (asserted in PreviewHarnessTests.fs).

open System
open System.IO
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open ControlsPreview.Samples

/// Walk up from the binary location to the repository root (the dir holding FS-Skia-UI.sln),
/// so the generator/tests resolve docs paths regardless of the working directory.
let repositoryRoot () =
    let rec find (dir: string | null) =
        match dir with
        | null -> failwith "repository root (FS-Skia-UI.sln) not found above the binary"
        | d ->
            if File.Exists(Path.Combine(d, "FS-Skia-UI.sln")) then d
            else find (Path.GetDirectoryName d)
    find (AppContext.BaseDirectory.TrimEnd('/', '\\'))

/// Committed preview location for a control id: docs/img/controls/<id>.png.
let previewPath (root: string) (id: string) =
    Path.Combine(root, "docs", "img", "controls", id + ".png")

let private toScene (w: Widget<unit>) : SceneNode =
    let result = Control.render Theme.light (Widget.toControl w)
    // captureScreenshotEvidence takes a SceneNode; wrap the control's Scene in a Group.
    Group [ result.Scene ]

/// Render one widget to a PNG at `outputPath` through the real render-only evidence path.
let renderWidgetToPng (width: int) (height: int) (outputPath: string) (w: Widget<unit>) : ScreenshotEvidenceResult =
    match Path.GetDirectoryName outputPath with
    | null -> ()
    | dir -> Directory.CreateDirectory dir |> ignore

    let request: ScreenshotEvidenceRequest =
        { Command = "screenshot"
          AppOrSample = "controls-preview-079"
          OutputPath = outputPath
          Width = width
          Height = height
          RendererMode = "viewer-render-target"
          CaptureMode = ViewerRenderTargetPng
          HostFacts = []
          Timeout = TimeSpan.FromSeconds 30.0 }

    let options: ViewerOptions =
        { Title = "controls-preview"; InitialSize = { Width = width; Height = height }; PresentMode = ViewerPresentMode.OffscreenReadback; FrameRateCap = None }

    Viewer.captureScreenshotEvidence request options (toScene w)

/// Render one demonstrative sample to a path; `None` for an unsupported entry.
let renderSampleTo (outputPath: string) (s: ControlSampleDefinition) : ScreenshotEvidenceResult option =
    match s.Build with
    | Some build -> Some(renderWidgetToPng (fst s.Canvas) (snd s.Canvas) outputPath (build ()))
    | None -> None

/// The generator: regenerate every committed preview from the single sample source.
/// Demonstrative ⇒ render docs/img/controls/<id>.png; Unsupported ⇒ ensure no image.
/// Returns (rendered, unsupported); fails loudly on any real RenderingFailure.
let regenerateAll () : int * int =
    let root = repositoryRoot ()
    let mutable rendered = 0
    let failures = System.Collections.Generic.List<string * string>()

    for s in samples do
        let target = previewPath root s.Id

        match s.Kind with
        | Demonstrative ->
            let r = renderWidgetToPng (fst s.Canvas) (snd s.Canvas) target ((Option.get s.Build) ())
            if r.Status = ScreenshotOk then rendered <- rendered + 1
            else failures.Add(s.Id, sprintf "%A" r.Status)
        | Unsupported ->
            // Honest no-image declaration: remove any stale committed image.
            if File.Exists target then File.Delete target

    let unsupportedCount = List.length unsupportedSamples
    printfn
        "controls-preview render: rendered=%d unsupported=%d total=%d (root=%s)"
        rendered
        unsupportedCount
        (List.length samples)
        root

    if failures.Count > 0 then
        failwithf "render failures (real RenderingFailure, not silently downgraded): %A" (List.ofSeq failures)

    rendered, unsupportedCount
