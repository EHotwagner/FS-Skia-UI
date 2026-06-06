module ParityAnimationOutputTests

// Feature 073: animation parity + deterministic-evidence tests.
//
//  • Identity-at-rest (T014, SC-004): `Animation.applyAt` returns the target's node
//    UNWRAPPED at settle (byte-identical to static); a non-identity transform wraps
//    in a `PerspectiveNode`.
//  • Deterministic sampling (T025/T026, SC-003): sampled frames are distinct with
//    monotone underlying property values, byte-identical on re-derivation, and the
//    captured goldens re-derive byte-identically (set FS_SKIA_CAPTURE_GOLDEN=1 to
//    (re-)capture). Each frame also renders through the existing deterministic-scene
//    evidence path.
//  • Opt-in parity (T029, SC-005): `Animation.empty` (and no-animation) is
//    byte-identical to the static render — no new required parameter.

open System
open System.IO
open Expecto
open FS.Skia.UI.Scene

let rec private findRoot (dir: string) =
    if Directory.GetFiles(dir, "*.sln").Length > 0 then
        dir
    else
        match Directory.GetParent dir |> Option.ofObj with
        | Some parent -> findRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" dir

let private repositoryRoot = findRoot AppContext.BaseDirectory

let private goldenDir =
    Path.Combine(repositoryRoot, "tests", "Parity.Tests", "fixtures", "v3-host-golden", "scene-output")

let private capture =
    match Environment.GetEnvironmentVariable "FS_SKIA_CAPTURE_GOLDEN" with
    | null -> false
    | value -> value = "1" || value.ToLowerInvariant() = "true"

let private size = ParityAnimationOutput.outputSize

let private settledNode (scene: Scene) =
    match scene.Nodes with
    | [ single ] -> single
    | _ -> failwith "reference panel must be a single-node scene"

[<Tests>]
let identityAtRestTests =
    testList "Animation identity-at-rest (SC-004)" [
        test "settled entrance returns the target node UNWRAPPED (byte-identical to static)" {
            let panel = ParityAnimationOutput.panelScene ()
            let node = Animation.applyAt (TimeSpan.FromMilliseconds 300.0) ParityAnimationOutput.entrance panel
            Expect.equal node (settledNode panel) "settled (opacity=1, identity transform) ⇒ the static node, unwrapped"
        }

        test "applyAt over Animation.empty is the static node at every sample" {
            let panel = ParityAnimationOutput.panelScene ()
            let expected = settledNode panel
            for elapsedMs in [ 0.0; 50.0; 1000.0 ] do
                let node = Animation.applyAt (TimeSpan.FromMilliseconds elapsedMs) Animation.empty panel
                Expect.equal node expected (sprintf "empty animation is the static node at t=%fms" elapsedMs)
        }

        test "a non-identity transform sample wraps in a PerspectiveNode carrying the matrix" {
            let panel = ParityAnimationOutput.panelScene ()
            let node = Animation.applyAt TimeSpan.Zero ParityAnimationOutput.entrance panel
            match node with
            | PerspectiveNode(m, _) -> Expect.equal m.M23 24.0 "start sample carries translateY = 24 in M23"
            | other -> failtestf "expected a PerspectiveNode at the start sample, got %A" (Scene.describe { Nodes = [ other ] })
        }

        test "settled frame's deterministic-scene hash equals the static render's hash (SC-004)" {
            let panel = ParityAnimationOutput.panelScene ()
            let settledFrame = { Nodes = [ Animation.applyAt (TimeSpan.FromMilliseconds 300.0) ParityAnimationOutput.entrance panel ] }
            let settled = SceneEvidence.renderHash size settledFrame
            let staticEv = SceneEvidence.renderHash size panel
            match settled, staticEv with
            | Ok a, Ok b -> Expect.equal a.Value b.Value "settled animation hash = static hash"
            | _ -> failtest "deterministic-scene render must succeed for both"
        }
    ]

[<Tests>]
let optInParityTests =
    testList "Animation opt-in parity (FR-007, SC-005)" [
        test "Animation.empty renders byte-identically to the static scene (no new parameter)" {
            let panel = ParityAnimationOutput.panelScene ()
            let emptyFrame = { Nodes = [ Animation.applyAt TimeSpan.Zero Animation.empty panel ] }
            let withEmpty = SceneEvidence.renderHash size emptyFrame
            let staticEv = SceneEvidence.renderHash size panel
            match withEmpty, staticEv with
            | Ok a, Ok b -> Expect.equal a.Value b.Value "empty-animation hash = static hash"
            | _ -> failtest "deterministic-scene render must succeed for both"
        }
    ]

[<Tests>]
let deterministicSamplingTests =
    testList "Animation deterministic sampling (SC-003, SC-007)" [
        yield test "underlying property values move monotonically across start/mid/end" {
            let times = [ TimeSpan.Zero; TimeSpan.FromMilliseconds 150.0; TimeSpan.FromMilliseconds 300.0 ]
            let opacityTween = Option.get ParityAnimationOutput.entrance.Opacity
            let transformTween = Option.get ParityAnimationOutput.entrance.Transform

            let opacities = times |> List.map (fun t -> Tween.sample Animation.lerpFloat t opacityTween)
            let translateYs = times |> List.map (fun t -> (Tween.sample Transform.lerp t transformTween).TranslateY)

            Expect.isTrue (opacities = List.sort opacities && List.distinct opacities = opacities) "opacity strictly increases 0→1"
            Expect.isTrue (translateYs = List.sortDescending translateYs && List.distinct translateYs = translateYs) "translateY strictly decreases 24→0"
            Expect.equal (List.head opacities) 0.0 "start opacity is 0"
            Expect.equal (List.last opacities) 1.0 "settled opacity is 1"
        }

        yield test "sampleFrames produces one Scene per requested time" {
            let times = [ TimeSpan.Zero; TimeSpan.FromMilliseconds 150.0; TimeSpan.FromMilliseconds 300.0 ]
            let frames = Animation.sampleFrames times ParityAnimationOutput.entrance (ParityAnimationOutput.panelScene ())
            Expect.equal (List.length frames) 3 "one frame per time sample"
        }

        yield test "each sampled frame renders through the deterministic-scene evidence path" {
            for (_, animation, elapsed) in ParityAnimationOutput.samples do
                let frame = { Nodes = [ Animation.applyAt elapsed animation (ParityAnimationOutput.panelScene ()) ] }
                match SceneEvidence.renderHash size frame with
                | Result.Ok _ -> ()
                | Result.Error e -> failtestf "deterministic-scene render failed: %s" e.Message
        }

        yield test "re-derivation of the value-aware encoding is byte-identical (same process)" {
            for sample in ParityAnimationOutput.samples do
                let first = ParityAnimationOutput.encodeSample sample
                let second = ParityAnimationOutput.encodeSample sample
                Expect.equal first second "value-aware encoding is stable across runs"
        }

        yield test "start / mid / settled frames are distinct (value-aware progression)" {
            let entranceEncodings =
                ParityAnimationOutput.samples
                |> List.filter (fun (l, _, _) -> l.StartsWith "entrance")
                |> List.map ParityAnimationOutput.encodeSample
            Expect.equal (List.distinct entranceEncodings |> List.length) 3 "entrance start/mid/settled are three distinct encodings"
        }

        yield test "concurrent independent animations sample without interference" {
            let elapsed = TimeSpan.FromMilliseconds 100.0
            let a = ParityAnimationOutput.encodeFrame "x" ParityAnimationOutput.entrance elapsed
            let b = ParityAnimationOutput.encodeFrame "x" ParityAnimationOutput.glide elapsed
            Expect.notEqual a b "two different animations at the same time produce independent frames"
        }

        for (label, animation, elapsed) in ParityAnimationOutput.samples do
            yield
                test (sprintf "%s golden re-derives byte-identically" label) {
                    let actual = ParityAnimationOutput.encodeFrame label animation elapsed
                    let goldenPath = Path.Combine(goldenDir, sprintf "animation-%s.txt" label)

                    if capture then
                        Directory.CreateDirectory goldenDir |> ignore
                        File.WriteAllText(goldenPath, actual)
                        Expect.isTrue (File.Exists goldenPath) "golden captured"
                    else
                        Expect.isTrue
                            (File.Exists goldenPath)
                            (sprintf "golden fixture must exist at %s (capture with FS_SKIA_CAPTURE_GOLDEN=1)" goldenPath)

                        let expected = File.ReadAllText goldenPath
                        Expect.equal actual expected (sprintf "%s frame is byte-identical to its golden (SC-003)" label)
                }
    ]
