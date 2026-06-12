namespace FS.Skia.UI.SkillSupport

// Feature 108 (US3, FR-009): the generic ordered-Msg fold (see EvidenceTour.fsi). A left fold that
// threads (model, acc): each message advances the model through `update`, then `project` folds the
// NEW model into the accumulator. Pure and deterministic — no I/O, no wall-clock, no randomness.
module EvidenceTour =
    let run
        (script: 'msg list)
        (seed: 'model)
        (update: 'msg -> 'model -> 'model)
        (project: 'model -> 'acc -> 'acc)
        (initial: 'acc)
        : 'acc =
        script
        |> List.fold
            (fun (model, acc) msg ->
                let model' = update msg model
                model', project model' acc)
            (seed, initial)
        |> snd
