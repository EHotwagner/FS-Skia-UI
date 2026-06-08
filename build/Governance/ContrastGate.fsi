// ContrastGate.fsi — the ContrastCheck gate's validated-pairing set + pure evaluator
// (feature 083, FR-007/FR-008/FR-009). Build-tooling scope (build/Governance, net10.0);
// NOT a tracked runtime surface baseline (same status as DesignTokenGen.fsi / CatalogGen.fsi).
//
// The WCAG arithmetic is single-sourced from the shipped FS.Skia.UI.Color package
// (Contrast.ratio / compositeOver) consumed by ProjectReference — the gate does not
// re-implement the formula. The pure core evaluates an explicit, documented pairing set over
// resolved token colors; the only filesystem read (the generated token values) lives at the
// Engine/Interpret.fs edge (Principle IV). `outcomesFromFacts` bridges the DTCG fact table
// (DesignTokenGen.parse, alias-resolved) into outcomes so the gate honors each theme's own
// contrastRequiredRatio.
module FS.Skia.UI.Build.ContrastGate

open FS.Skia.UI.Scene
open FS.Skia.UI.Color

/// A named foreground/background/role tuple the gate checks (FR-009). Token names are
/// resolved against the generated theme tokens; the role selects the threshold.
type ValidatedPairing =
    { Foreground: string
      Background: string
      Role: Role }

/// Per-pairing result the gate report renders (FR-008). A failing row carries both token
/// names (via Pairing), both resolved colors, measured + required ratios, theme, and role.
type PairingOutcome =
    { Theme: string
      Pairing: ValidatedPairing
      ForegroundColor: Color
      BackgroundColor: Color
      Measured: float
      Required: float
      Passed: bool }

/// The fixed WCAG 1.4.11 graphic/UI threshold (3.0).
val graphicThreshold: float

/// The explicit, documented validated-pairing set — NOT the token cartesian product (FR-009).
val validatedPairings: ValidatedPairing list

/// Pure: evaluate every validated pairing for one theme. `tokens` maps token name -> resolved
/// opaque Color; `textRequiredRatio` is the theme's contrastRequiredRatio (Text target).
/// Non-opaque foregrounds are composited over the pairing's background token before measuring.
/// A pairing naming a token absent from `tokens` is skipped (no row).
val evaluateTheme: theme: string -> tokens: Map<string, Color> -> textRequiredRatio: float -> PairingOutcome list

/// Pure: bridge the alias-resolved DTCG fact table into outcomes for the light + dark themes.
val outcomesFromFacts: facts: DesignTokenGen.DesignTokenFact list -> PairingOutcome list

/// Pure: the enforced rows that fell below threshold (Decorative is recorded, never enforced).
val failures: outcomes: PairingOutcome list -> PairingOutcome list

/// Pure: fail-loud diagnostic lines for failing rows (FR-008) — token names, resolved colors,
/// measured + required ratios, theme, role.
val failureDiagnostics: outcomes: PairingOutcome list -> string list

/// Pure: the readiness report body (both themes, per-pairing measured vs required, pass/fail).
val renderReport: outcomes: PairingOutcome list -> string
