module ParitySceneOutputTests

// Feature 048 (US1, FR-004, Principle VI, SC-003): the scene-output golden re-derivation
// test. For each seed it re-runs the deterministic encoder over the current host's scene
// and asserts the output is BYTE-IDENTICAL to the committed golden (0-byte diff). It is
// red until the encoder and committed fixtures exist (T008). Scene-output is the
// AUTHORITATIVE parity oracle; screenshots corroborate (headless flake).
//
// Set FS_SKIA_CAPTURE_GOLDEN=1 to (re-)capture the goldens from the current host instead
// of asserting (used once at the pin to write the fixtures, then never in CI).

open System
open System.IO
open Expecto

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

[<Tests>]
let sceneOutputGoldenTests =
    testList
        "v3-host-golden scene-output"
        [ for (seedId, size, sceneThunk) in ParitySceneOutput.seeds do
              yield test (sprintf "%s re-derives byte-identically from the current host" seedId) {
                  let actual = ParitySceneOutput.encode seedId size (sceneThunk ())
                  let goldenPath = Path.Combine(goldenDir, seedId + ".txt")

                  if capture then
                      Directory.CreateDirectory goldenDir |> ignore
                      File.WriteAllText(goldenPath, actual)
                      Expect.isTrue (File.Exists goldenPath) "golden captured"
                  else
                      Expect.isTrue
                          (File.Exists goldenPath)
                          (sprintf "golden fixture must exist at %s (capture with FS_SKIA_CAPTURE_GOLDEN=1)" goldenPath)

                      let expected = File.ReadAllText goldenPath
                      Expect.equal actual expected (sprintf "%s scene-output is byte-identical to its golden (SC-003)" seedId)
              }

          yield test "the encoder is deterministic across repeated runs" {
              for (seedId, size, sceneThunk) in ParitySceneOutput.seeds do
                  let first = ParitySceneOutput.encode seedId size (sceneThunk ())
                  let second = ParitySceneOutput.encode seedId size (sceneThunk ())
                  Expect.equal first second (sprintf "%s encoding is stable across runs" seedId)
          } ]
