// CONTRACT SKETCH — feature 108 US5 (FR-016). src/KeyboardInput/KeyboardInput.fsi edit.
namespace FS.Skia.UI.KeyboardInput

/// Modifier state recovered at the key boundary. The raw key string already carries
/// `Ctrl+`/`Alt+`/`Shift+`/`Meta+` prefixes; today `normalize` collapses `"Ctrl+L"`
/// to `Unknown "Ctrl+L"` and loses them. Parsing them here makes chords as dependable
/// as plain keys (FR-016) with NO backend change.
type KeyModifiers =
    { Ctrl: bool
      Alt: bool
      Shift: bool
      Meta: bool }

module ViewerKeyboard =
    // existing: normalize / normalizeEvent / toKeyId  (UNCHANGED for back-compat)

    val noModifiers: KeyModifiers

    /// Strip leading `Ctrl+`/`Alt+`/`Shift+`/`Meta+` (case-insensitive, any order) off
    /// the raw key, then normalise the base key. Returns the base `ViewerKey`, the
    /// down/up flag, and the held modifiers. An unmodified key -> `noModifiers` and the
    /// SAME `ViewerKey` as `normalizeEvent` (byte-identical routing); zero silent loss
    /// for a chord (SC-009).
    val normalizeEventWithModifiers:
        event: ViewerKeyEvent -> ViewerKey * bool * KeyModifiers
