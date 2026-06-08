module ControlsPreview.Fidelity

// Feature 080 (T016/T017, US2, FR-007/FR-008/FR-013) — the decoded-content fidelity gate.
//
// For every Demonstrative control it decodes the committed `docs/img/controls/<id>.png`, computes
// the lit-pixel coverage + distinct-colour signature OUTSIDE the title band, re-renders the control
// for its `Scene.describe` primitive-kind signature, and checks the per-control `ContentSignature`.
// Every `Unsupported` control must have NO committed PNG. The sample set is asserted TOTAL over
// `CatalogGen.catalogFacts` (fail-closed: a catalog id with neither a `Signature` nor
// `UnsupportedNoPreview` fails, naming the control). The retained fixture matrix
// (`fixtures/fidelity/{lowfi,faithful}`) is the durable red→green guard: every `lowfi` (079-style
// label-on-box) PNG MUST fail its control's pixel signature, every `faithful` PNG MUST pass.
// Native-Skia-absent decoding is a BLOCKING host warning, never a silent pass (fs-skia-evidence-mode).

open System
open System.IO
open System.Collections.Generic
open SkiaSharp
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed
open FS.Skia.UI.Build
open ControlsPreview.FidelityTypes
open ControlsPreview.Samples
open ControlsPreview.Render

/// Quantize a colour to 16-level channels so anti-aliasing noise does not inflate the
/// distinct-colour count nor split the background.
let private quantize (c: SKColor) = (int c.Red / 16, int c.Green / 16, int c.Blue / 16)

/// Decode a PNG and measure coverage + distinct non-background colours OUTSIDE the title band.
/// `Error` carries a decode/native failure message (surfaced as a blocking host warning).
let decodeOutsideBand (path: string) (titleBand: int) : Result<float * int, string> =
    try
        use bmp = SKBitmap.Decode(path)
        if isNull (box bmp) then
            Result.Error(sprintf "SkiaSharp could not decode %s (null bitmap)" path)
        else
            let w, h = bmp.Width, bmp.Height
            // Background = the most common quantized colour across the whole image.
            let counts = Dictionary<int * int * int, int>()
            for yy in 0 .. h - 1 do
                for xx in 0 .. w - 1 do
                    let q = quantize (bmp.GetPixel(xx, yy))
                    counts[q] <- (match counts.TryGetValue q with | true, v -> v | _ -> 0) + 1
            let background = (counts |> Seq.maxBy (fun kv -> kv.Value)).Key
            let band = min titleBand h
            let mutable lit = 0
            let mutable total = 0
            let distinct = HashSet<int * int * int>()
            for yy in band .. h - 1 do
                for xx in 0 .. w - 1 do
                    total <- total + 1
                    let q = quantize (bmp.GetPixel(xx, yy))
                    if q <> background then
                        lit <- lit + 1
                        distinct.Add q |> ignore
            Result.Ok(float lit / float (max 1 total), distinct.Count)
    with ex ->
        Result.Error(sprintf "decode threw for %s: %s" path ex.Message)

let private liveKinds (s: ControlSampleDefinition) : SceneElementKind list =
    match s.Build with
    | Some build -> (Control.render Theme.light (Widget.toControl (build ()))).Scene |> Scene.describe
    | None -> []

let private countKind k kinds = kinds |> List.filter ((=) k) |> List.length

let private evalPixel (px: PixelSignature) coverage distinctColors : string option =
    if coverage < px.MinCoverageOutsideTitleBand then
        Some(sprintf "coverage %.4f below %.4f outside the title band" coverage px.MinCoverageOutsideTitleBand)
    elif distinctColors < px.MinDistinctColors then
        Some(sprintf "%d distinct colours below %d outside the title band" distinctColors px.MinDistinctColors)
    else
        None

let private evalPrimitive (kinds: SceneElementKind list) (ps: PrimitiveSignature) : string option =
    let missing = ps.RequiredKinds |> List.filter (fun k -> not (List.contains k kinds))
    let lowCounts = ps.MinKindCounts |> List.filter (fun (k, n) -> countKind k kinds < n)
    if not (List.isEmpty missing) then
        Some(sprintf "missing required primitive kind(s) %A" missing)
    elif not (List.isEmpty lowCounts) then
        Some(
            lowCounts
            |> List.map (fun (k, n) -> sprintf "%A present %d need >=%d" k (countKind k kinds) n)
            |> String.concat ", "
        )
    else
        None

/// True when a verdict failed purely because the host could not decode (native Skia absent).
let mutable private decodeBlocked = false

let verdictFor (root: string) (s: ControlSampleDefinition) : FidelityVerdict =
    let path = previewPath root s.Id
    match s.Fidelity with
    | UnsupportedNoPreview ->
        let hasImg = File.Exists path
        { ControlId = s.Id
          Declared = s.Fidelity
          CoverageOutsideTitleBand = None
          DistinctColors = None
          PresentKinds = []
          Passed = not hasImg
          FailureReason = if hasImg then Some(sprintf "%s is Unsupported but a committed PNG exists at %s" s.Id path) else None }
    | Signature signature ->
        if not (File.Exists path) then
            { ControlId = s.Id
              Declared = s.Fidelity
              CoverageOutsideTitleBand = None
              DistinctColors = None
              PresentKinds = []
              Passed = false
              FailureReason = Some(sprintf "%s: Demonstrative control has no committed PNG at %s" s.Id path) }
        else
            match decodeOutsideBand path signature.Pixel.TitleBandHeight with
            | Result.Error e ->
                decodeBlocked <- true
                { ControlId = s.Id
                  Declared = s.Fidelity
                  CoverageOutsideTitleBand = None
                  DistinctColors = None
                  PresentKinds = []
                  Passed = false
                  FailureReason = Some(sprintf "%s: %s" s.Id e) }
            | Result.Ok(coverage, distinct) ->
                let kinds = liveKinds s
                let reasons =
                    [ evalPixel signature.Pixel coverage distinct
                      match signature.Primitive with
                      | Some ps -> evalPrimitive kinds ps
                      | None -> None ]
                    |> List.choose id
                { ControlId = s.Id
                  Declared = s.Fidelity
                  CoverageOutsideTitleBand = Some coverage
                  DistinctColors = Some distinct
                  PresentKinds = kinds
                  Passed = List.isEmpty reasons
                  FailureReason = if List.isEmpty reasons then None else Some(sprintf "%s: %s" s.Id (String.concat "; " reasons)) }

/// One fixture row: a committed fixture PNG checked pixel-only against its control's signature.
type FixtureResult =
    { Id: string
      Bucket: string        // "lowfi" | "faithful"
      ExpectedPass: bool
      ActualPass: bool
      Detail: string
      Ok: bool }            // gate agrees with the expectation

let private signatureById =
    samples
    |> List.choose (fun s -> match s.Fidelity with | Signature sg -> Some(s.Id, sg) | _ -> None)
    |> Map.ofList

// (* SYNTHETIC FIXTURE: the files under fixtures/fidelity/{lowfi,faithful} are synthetic gate
//    TEST VECTORS, not product evidence — lowfi/* are retained 079-style label-on-box renders
//    (sourced from the pre-fix tree) asserted to FAIL, faithful/* are the regenerated faithful
//    counterparts asserted to PASS. They prove the gate's discrimination (SC-003); the catalog
//    previews themselves are real renders, so no product task is [S]. *)
let fixtureResults (root: string) : FixtureResult list =
    let dir bucket = Path.Combine(root, "tests", "ControlsPreview.Harness", "fixtures", "fidelity", bucket)
    let check bucket expectedPass =
        let d = dir bucket
        if not (Directory.Exists d) then []
        else
            Directory.GetFiles(d, "*.png")
            |> Array.sort
            |> Array.toList
            |> List.map (fun file ->
                let id = Path.GetFileNameWithoutExtension file |> Option.ofObj |> Option.defaultValue ""
                match Map.tryFind id signatureById with
                | None ->
                    { Id = id; Bucket = bucket; ExpectedPass = expectedPass; ActualPass = false
                      Detail = sprintf "no signature declared for fixture id %s" id; Ok = false }
                | Some sg ->
                    match decodeOutsideBand file sg.Pixel.TitleBandHeight with
                    | Result.Error e ->
                        decodeBlocked <- true
                        { Id = id; Bucket = bucket; ExpectedPass = expectedPass; ActualPass = false; Detail = e; Ok = false }
                    | Result.Ok(coverage, distinct) ->
                        let actualPass = evalPixel sg.Pixel coverage distinct |> Option.isNone
                        { Id = id; Bucket = bucket; ExpectedPass = expectedPass; ActualPass = actualPass
                          Detail = sprintf "coverage=%.4f distinct=%d" coverage distinct
                          Ok = actualPass = expectedPass })
    check "lowfi" false @ check "faithful" true

let private writeReport (root: string) (verdicts: FidelityVerdict list) (fixtures: FixtureResult list) =
    let featureReadiness = Path.Combine(root, "specs", "080-control-render-fidelity", "readiness", "control-fidelity.md")
    let sb = Text.StringBuilder()
    let line (s: string) = sb.AppendLine s |> ignore
    line "# Control fidelity report (generated by ControlFidelityCheck)"
    line ""
    line "Decoded-content gate (feature 080, FR-007). One row per catalog control; coverage and"
    line "distinct colours are measured OUTSIDE the title band; present kinds are from the live"
    line "`Control.render Theme.light` → `Scene.describe`. Renderer mode: viewer-render-target."
    line ""
    line "| control id | declared | coverage | distinct colours | present kinds | verdict | failure reason |"
    line "|---|---|---|---|---|---|---|"
    for v in verdicts do
        let declared = match v.Declared with | Signature _ -> "Signature" | UnsupportedNoPreview -> "UnsupportedNoPreview"
        let cov = match v.CoverageOutsideTitleBand with | Some c -> sprintf "%.4f" c | None -> "n/a"
        let dc = match v.DistinctColors with | Some d -> string d | None -> "n/a"
        let kinds = v.PresentKinds |> List.distinct |> List.map (sprintf "%A") |> String.concat " "
        let verdict = if v.Passed then "pass" else "FAIL"
        let reason = v.FailureReason |> Option.defaultValue ""
        line (sprintf "| %s | %s | %s | %s | %s | %s | %s |" v.ControlId declared cov dc kinds verdict reason)
    line ""
    line "## Fixture matrix (SC-003 — durable red→green guard)"
    line ""
    line "| fixture | bucket | expected | actual | detail | ok |"
    line "|---|---|---|---|---|---|"
    for f in fixtures do
        line (sprintf "| %s | %s | %s | %s | %s | %s |"
                  f.Id f.Bucket
                  (if f.ExpectedPass then "pass" else "fail")
                  (if f.ActualPass then "pass" else "fail")
                  f.Detail
                  (if f.Ok then "ok" else "MISMATCH"))
    Directory.CreateDirectory(Path.GetDirectoryName featureReadiness |> Option.ofObj |> Option.defaultValue ".") |> ignore
    File.WriteAllText(featureReadiness, sb.ToString())
    featureReadiness

/// Run the gate. Returns process exit code (0 = pass). Writes the decoded-content report.
let run () : int =
    decodeBlocked <- false
    let root = repositoryRoot ()

    // Fail-closed totality over catalogFacts (FR-013): every catalog id needs a declaration.
    let catalogIds = CatalogGen.catalogFacts |> List.map (fun f -> f.Id) |> Set.ofList
    let declaredIds = sampleIds |> Set.ofList
    let undeclared = Set.difference catalogIds declaredIds

    let verdicts = samples |> List.map (verdictFor root)
    let fixtures = fixtureResults root
    let reportPath = writeReport root verdicts fixtures

    let failedVerdicts = verdicts |> List.filter (fun v -> not v.Passed)
    let badFixtures = fixtures |> List.filter (fun f -> not f.Ok)

    printfn "controls-preview fidelity: %d controls, %d failed; %d fixtures, %d mismatched"
        (List.length verdicts) (List.length failedVerdicts) (List.length fixtures) (List.length badFixtures)
    printfn "report: %s" reportPath

    if decodeBlocked then
        eprintfn "BLOCKING host warning: native Skia could not decode one or more PNGs (display/session availability). This is a blocking warning, not a silent pass."

    for id in undeclared do
        eprintfn "FAIL (fail-closed): catalog control '%s' has neither a Signature nor an UnsupportedNoPreview declaration" id
    for v in failedVerdicts do
        v.FailureReason |> Option.iter (eprintfn "FAIL %s")
    for f in badFixtures do
        eprintfn "FAIL fixture %s/%s: expected %s, got %s (%s)"
            f.Bucket f.Id (if f.ExpectedPass then "pass" else "fail") (if f.ActualPass then "pass" else "fail") f.Detail

    if Set.isEmpty undeclared && List.isEmpty failedVerdicts && List.isEmpty badFixtures then 0 else 1
