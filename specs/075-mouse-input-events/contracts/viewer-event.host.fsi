// CONTRACT SKETCH — FS.Skia.UI.SkiaViewer host event extension (feature 075).
// Target file: src/SkiaViewer/Host/Diagnostics.fsi (extend the existing ViewerEvent).
// This is a Phase-1 design artifact; the authoritative surface is the real .fsi.
//
// Compatibility note: extending the PointerPressed/PointerReleased case ARITY is a
// source-level change for any match expression over ViewerEvent. The only existing
// matcher (src/SkiaViewer/SkiaViewer.fs) returns None for Pointer* and is updated
// in lockstep. New cases (PointerScrolled, PointerExited) are purely additive.

namespace FS.Skia.UI.SkiaViewer.Host

/// Mouse button identity carried by host pointer press/release events (FR-013).
type ViewerPointerButton =
    | PrimaryButton
    | SecondaryButton
    | MiddleButton

type ViewerEvent =
    | Loaded
    | UpdateTick of elapsedSeconds: float
    | RenderTick of elapsedSeconds: float
    | KeyDown of key: string
    | KeyUp of key: string
    | PointerMoved of x: float * y: float
    // CHANGED: now carries the originating button.
    | PointerPressed of x: float * y: float * button: ViewerPointerButton
    | PointerReleased of x: float * y: float * button: ViewerPointerButton
    // NEW: wheel/scroll with signed per-axis delta (FR-014).
    | PointerScrolled of x: float * y: float * deltaX: float * deltaY: float
    // NEW: pointer left the window / host lost pointer focus — drives FR-007 cancel.
    | PointerExited
    | Resized of Size
    | CloseRequested
    | DiagnosticReported of RenderDiagnostic
