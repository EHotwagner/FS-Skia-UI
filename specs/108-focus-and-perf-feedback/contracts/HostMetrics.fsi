// CONTRACT SKETCH — feature 108 US2/US3/US4. src/Controls.Elmish/ControlsElmish.fsi edits.
namespace FS.Skia.UI.Controls.Elmish

open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Controls

/// Per-frame structured work/timing signal (US2, FR-006). The four count/bool fields
/// are the byte-stable determinism surface (FR-007); `FrameDuration` is reported for
/// real perf observation but EXCLUDED from golden assertions.
type FrameMetrics =
    { RemeasuredNodeCount: int        // from WorkReductionRecord.RemeasuredNodeCount
      PointerSamplesReceived: int     // raw pointer samples that arrived this frame
      PointerMovesProcessed: int      // <= 1 after coalescing
      ViewRebuilt: bool               // did the frame call host.View?
      FrameDuration: System.TimeSpan }

/// One step of the deterministic driver (US3, FR-009).
type FrameInput<'msg> =
    | Key of ViewerKey * KeyModifiers
    | Pointer of PointerInteraction
    | Tick of System.TimeSpan
    | Idle

// Additive InteractiveAppHost fields (inert defaults -> at-rest byte-identical):
//
//   MapKeyChord: ViewerKey -> KeyModifiers -> 'msg option
//       Consulted BEFORE the existing shift-only `MapKey`; default ignores modifiers
//       and defers to `MapKey`, so unmodified keys route exactly as today (FR-016).
//
//   OnFrameMetrics: FrameMetrics -> unit
//       Opt-in observability sink called once per frame; default = ignore (FR-006).

module Perf =
    /// Pure, headless, frame-counted (US3/FR-009). Folds an ordered input script over
    /// the host's pure update + RetainedRender.step, one frame per step, returning the
    /// per-frame metrics. Shares the EXACT coalescing + step code path with
    /// `runInteractiveApp` (no parallel logic — US3 locks the real path).
    ///   - K `Pointer` moves in one frame -> PointerMovesProcessed <= 1,
    ///     PointerSamplesReceived = K (FR-011/SC-004).
    ///   - `Idle` frame -> RemeasuredNodeCount = 0, ViewRebuilt = false (FR-008/SC-005).
    ///   - pure hover frame -> ViewRebuilt = false (SC-005).
    val runScript:
        host: InteractiveAppHost<'model, 'msg> ->
        size: FS.Skia.UI.Scene.Size ->
        script: FrameInput<'msg> list ->
            FrameMetrics list
