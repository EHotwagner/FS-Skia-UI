// CONTRACT SKETCH — feature 108 US3 (FR-009 generic half). New src/SkillSupport/EvidenceTour.fsi.
// Consumer-generic, dependency-free (no Controls/Color ref); sits beside SkillSupport.Random.
namespace FS.Skia.UI.SkillSupport

module EvidenceTour =
    /// Fold an ordered `Msg` script over a pure `update`, one step per Msg, projecting
    /// each resulting model into a running accumulator. Generalises the consumer's
    /// hand-rolled frame-counted "tour" into a reusable, byte-stable combinator
    /// (pair with `SkillSupport.Random` for byte-stable seeded input).
    val run:
        script: 'msg list ->
        seed: 'model ->
        update: ('msg -> 'model -> 'model) ->
        project: ('model -> 'acc -> 'acc) ->
        initial: 'acc ->
            'acc
