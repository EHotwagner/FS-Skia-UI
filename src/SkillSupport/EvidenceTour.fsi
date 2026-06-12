namespace FS.Skia.UI.SkillSupport

/// Feature 108 (US3, FR-009 generic half): a consumer-generic, dependency-free combinator that
/// folds an ordered `Msg` script over a pure `update`, one step per `Msg`, projecting each
/// resulting model into a running accumulator. Generalises a consumer's hand-rolled frame-counted
/// "tour" into a reusable, byte-stable combinator — pair with `SkillSupport.Random` for byte-stable
/// seeded input. No `Controls`/`Color` dependency; sits beside `SkillSupport.Random`.
module EvidenceTour =
    /// Fold `script` over `update` starting from `seed`, projecting every produced model (in order)
    /// into `initial` via `project`. The seed model is NOT projected — only the post-step models —
    /// so an N-message script produces exactly N projections. Pure, total, deterministic: identical
    /// inputs yield an identical accumulator across runs and runtimes.
    val run:
        script: 'msg list ->
        seed: 'model ->
        update: ('msg -> 'model -> 'model) ->
        project: ('model -> 'acc -> 'acc) ->
        initial: 'acc ->
            'acc
