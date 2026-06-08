// Contract sketch for the new public package FS.Skia.UI.Color.
// This is the Phase-1 signature DESIGN (Principle I: sketch the .fsi, exercise in
// FSI, then implement). The real curated .fsi files live under src/Color/ and the
// per-package baseline at readiness/per-package-surface/FS.Skia.UI.Color.fsi.txt.

namespace FS.Skia.UI.Color

open FS.Skia.UI.Scene

/// The element kind a color is used for; selects the WCAG threshold (FR-003).
type Role =
    | Text          // AAA >=7:1, AA >=4.5:1, AA-Large >=3:1
    | GraphicOrUi   // >=3:1 (WCAG 1.4.11)
    | Decorative    // exempt

/// WCAG conformance classification for a ratio + role (FR-003, FR-004a).
type Verdict =
    | Aaa
    | Aa
    | AaLarge
    | Fail
    | Exempt        // Decorative role: recorded, never enforced (pass-equivalent)
    | Indeterminate // non-solid paint: neither pass nor fail

/// Ratio + role + verdict in one value (SC-004). For an `Indeterminate` input
/// `Ratio` is `nan` (System.Double.NaN — the documented not-applicable sentinel);
/// for an `Exempt` (Decorative) input `Ratio` carries the measured value but no
/// threshold is applied.
type ContrastResult =
    { Ratio: float
      Role: Role
      Verdict: Verdict }

/// WCAG 2.x relative-luminance + contrast measurement over Scene colors.
module Contrast =

    /// WCAG 2.x relative luminance of an opaque color (FR-001):
    /// 0.2126 R + 0.7152 G + 0.0722 B over sRGB-linearized channels.
    val relativeLuminance: color: Color -> float

    /// WCAG 2.x contrast ratio between two opaque colors (FR-002):
    /// (Llighter + 0.05) / (Ldarker + 0.05), in 1.0 .. 21.0.
    val ratio: a: Color -> b: Color -> float

    /// Composite a possibly-translucent color over an opaque background using
    /// deterministic source-over before measuring (FR-004).
    val compositeOver: background: Color -> foreground: Color -> Color

    /// Map a ratio + role to a verdict (FR-003). `Decorative` always returns
    /// `Exempt` regardless of ratio.
    val verdict: role: Role -> ratio: float -> Verdict

    /// Headline single call: ratio + role -> ContrastResult (SC-004). Composites
    /// `foreground` over `background` first if it carries alpha.
    val check: role: Role -> background: Color -> foreground: Color -> ContrastResult

    /// Solid-fill check from a Scene paint. Non-solid paints (gradient/shader/
    /// image fills) return Indeterminate, neither pass nor fail (FR-004a).
    val checkPaint: role: Role -> background: Color -> paint: Paint -> ContrastResult

/// Radix-derived, role-labelled accessible ramps (FR-005, FR-006). Reusable
/// catalog data only — NOT a second source of truth for shipped themes.
module Palettes =

    type StepRole =
        | AppBackground
        | SubtleBackground
        | ComponentBackground
        | Border
        | FocusRing
        | Solid
        | Text

    type RampVariant =
        | Light
        | Dark

    type PaletteStep =
        { Index: int
          Role: StepRole
          Color: Color }

    type PaletteRamp =
        { Family: string
          Variant: RampVariant
          Steps: PaletteStep list }

    /// Every available ramp (matched light + dark per family).
    val all: PaletteRamp list

    /// Look up a ramp by family + variant.
    val ramp: family: string -> variant: RampVariant -> PaletteRamp option

    /// The family names offered.
    val families: string list
