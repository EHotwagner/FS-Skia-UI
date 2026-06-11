// CONTRACT SKETCH — feature 100 (R5). Intended additions to src/Controls/Types.fsi.
// Existing fields are shown for context; only the marked additions are new.

namespace FS.Skia.UI.Controls

/// Declared range metadata for value/range roles — the SOLE source of step/bounds,
/// replacing the hardcoded 0.1 / 0..1 slider constant (FR-002). A DEFAULT-step slider
/// declares { Step = 0.1; Min = 0.0; Max = 1.0 } to stay byte-identical (FR-007).
type NavRange =
    { Step: float
      Min: float
      Max: float }

/// AccessibilityMetadata gains an optional Navigation range (new field only).
type AccessibilityMetadata =
    { Role: AccessibilityRole
      NameSource: string
      State: string list
      FocusOrder: int option
      Keyboard: KeyboardOperation
      Contrast: ContrastEvidence option
      Navigation: NavRange option }   // NEW

/// The closed set of navigation-outcome payload shapes (FR-005, SC-005). Mirrors NavIntent.
type NavPayload =
    | SteppedValue of value: float
    | MovedSelection of index: int * item: string option
    | MovedCell of row: int * col: int

/// ControlEvent gains a closed typed nav payload; Payload: string option is RETAINED so
/// existing click/changed/text consumers and the slider numeric path are unchanged. A
/// selection move dual-sets Payload (moved item id) AND Nav (closed MovedSelection).
type ControlEvent =
    { Kind: string
      ControlId: ControlId option
      Origin: ControlEventOrigin
      Payload: string option
      Nav: NavPayload option }        // NEW
