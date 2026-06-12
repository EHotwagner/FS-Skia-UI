module Feature108EvidenceTourTests

// Feature 108 (US3, FR-009 generic half) — `EvidenceTour.run` folds an ordered Msg script over a
// pure update, projecting each post-step model into a running accumulator, byte-stable across runs.

open Expecto
open FS.Skia.UI.SkillSupport

[<Tests>]
let tests =
    testList "Feature 108 EvidenceTour.run (US3, FR-009)" [
        test "run projects each post-step model in order (the seed is not projected)" {
            let result = EvidenceTour.run [ 1; 2; 3 ] 0 (fun msg model -> model + msg) (fun model acc -> acc @ [ model ]) []
            Expect.equal result [ 1; 3; 6 ] "running totals after each message"
        }

        test "run is deterministic across repeated runs (byte-stable)" {
            let script = [ "a"; "b"; "c" ]
            let update (msg: string) (model: string) = model + msg
            let project model acc = model :: acc
            let r1 = EvidenceTour.run script "" update project []
            let r2 = EvidenceTour.run script "" update project []
            Expect.equal r1 r2 "identical inputs -> identical accumulator"
            Expect.equal r1 [ "abc"; "ab"; "a" ] "left fold over the script"
        }

        test "an empty script returns the initial accumulator unchanged" {
            let result = EvidenceTour.run [] 0 (fun msg model -> model + msg) (fun model acc -> model :: acc) [ 42 ]
            Expect.equal result [ 42 ] "no messages -> initial accumulator"
        }
    ]
