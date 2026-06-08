# Phase 1 Data Model: Accessible Color Contrast & Palettes

Entities below are the value types of `FS.Skia.UI.Color` plus the gate's
governance data. All are immutable records / discriminated unions (Principle III);
no stateful workflow (no Model/Msg/Effect — see plan's MVU note).

## Color (existing — reused, not redefined)

The unit both contrast and palettes operate on is the existing
`FS.Skia.UI.Scene.Color` (`{ Red: byte; Green: byte; Blue: byte; Alpha: byte }`).
`FS.Skia.UI.Color` references Scene and does **not** declare a competing color
type.

## Role

The element kind that selects the applicable threshold.

| Case          | Threshold applied                                  |
|---------------|----------------------------------------------------|
| `Text`        | AAA ≥7:1, AA ≥4.5:1, AA-Large ≥3:1 (large = ≥18pt / ≥14pt bold) |
| `GraphicOrUi` | ≥3:1 (WCAG 1.4.11)                                  |
| `Decorative`  | exempt — no requirement (recorded, never enforced) |

```fsharp
type Role =
    | Text
    | GraphicOrUi
    | Decorative
```

## Verdict

Classification derived from a contrast ratio + role.

```fsharp
type Verdict =
    | Aaa            // Text only, ratio >= 7:1
    | Aa             // Text >= 4.5:1, or GraphicOrUi >= 3:1
    | AaLarge        // Text >= 3:1 (large text)
    | Fail           // below the role's minimum
    | Exempt         // Decorative role — recorded, never enforced (pass-equivalent)
    | Indeterminate  // non-solid paint — neither pass nor fail
```

- Validation: `Aaa`/`AaLarge` are only producible for `Text`; `GraphicOrUi`
  yields `Aa`/`Fail`; `Decorative` **always** yields `Exempt` regardless of the
  measured ratio (recorded, never enforced); `Indeterminate` only from non-solid
  paint input.

## ContrastResult

The return of the headline "ratio + verdict in one call" function (SC-004).

```fsharp
type ContrastResult =
    { Ratio: float          // 1.0 .. 21.0
      Role: Role
      Verdict: Verdict }
```

- `Ratio` is the computed WCAG ratio. For an `Indeterminate` (non-solid paint)
  input the ratio is reported as `nan` (`System.Double.NaN`, the documented
  not-applicable sentinel) and `Verdict = Indeterminate`. For an `Exempt`
  (`Decorative`) input the ratio still carries the measured WCAG value (recorded
  for the report) but no threshold is applied and `Verdict = Exempt`.

## PaletteRamp

An ordered, role-labelled sequence of colors for one hue family, in matched
light/dark variants (FR-005).

```fsharp
type StepRole =
    | AppBackground
    | SubtleBackground
    | ComponentBackground
    | Border
    | FocusRing
    | Solid
    | Text

type PaletteStep =
    { Index: int            // 1-based step within the ramp
      Role: StepRole
      Color: Color }

type RampVariant =
    | Light
    | Dark

type PaletteRamp =
    { Family: string        // e.g. "slate", "blue", "red"
      Variant: RampVariant
      Steps: PaletteStep list }
```

- Invariant (SC-003): for each family, a `Light` and a `Dark` ramp both exist,
  and at least one documented `Text`-step / background-step pair within a ramp
  measures ≥ 4.5:1 under the contrast function.

## ValidatedPairing (gate data — `FS.Skia.UI.Build`)

A named foreground/background/role tuple the `ContrastCheck` gate checks
(FR-009). Lives in the governance engine, not the shipped package.

```fsharp
type ValidatedPairing =
    { Foreground: string    // token name, e.g. "danger"
      Background: string    // token name, e.g. "background"
      Role: Role }
```

- The pairing *set* is explicit and documented (NOT the cartesian product).
- Text pairings are checked against the theme's `contrastRequiredRatio` token
  value; `GraphicOrUi` against fixed 3:1; `Decorative` recorded, not enforced.

## PairingOutcome (gate output row)

Per-pairing result the gate report renders (FR-008).

```fsharp
type PairingOutcome =
    { Theme: string                 // "light" | "dark"
      Pairing: ValidatedPairing
      ForegroundColor: Color        // resolved (alias + alpha-composited)
      BackgroundColor: Color
      Measured: float
      Required: float
      Passed: bool }
```

- A failing row carries both token names, both resolved colors, measured ratio,
  required ratio, theme, and role — the actionable failure (FR-008).

## RequiredRatio (existing token — read, not redefined)

The per-theme text target is the existing `contrastRequiredRatio` token
(`number`, currently 4.5 in both themes). The gate *reads* it from the generated
token values; it is not hardcoded and its meaning/value is not changed by this
feature (spec Unsupported scope).

## Relationships

- `ContrastResult` ← `ratio(Color, Color)` + `verdict(ratio, Role)`.
- `PaletteRamp` steps are `Color` values; a ramp pair feeds the contrast
  function to demonstrate the AA invariant.
- `ValidatedPairing` references token *names*; the gate resolves them to `Color`
  values from the generated `DesignTokens.fs` (alias-resolved, alpha-composited)
  and emits `PairingOutcome` rows.
