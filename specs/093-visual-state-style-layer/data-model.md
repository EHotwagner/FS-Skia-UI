# Phase 1 Data Model: Declarative Visual-State & Style-Class Layer

All types live in `FS.Skia.UI.Controls`. New public surface is on `Types.fsi` (style-class
carriers + the `AttrValue` case) and the new `Style.fsi` (`ResolvedStyle`). Existing types
(`VisualState`, `ValidationState`, `Theme`, `DesignTokens`, `Control<'msg>`, `Attr<'msg>`,
`AttrValue<'msg>`, `AttrCategory`) are referenced, not redefined.

## Entity: StyleVariant *(new, public — `Types.fsi`)*

The typed, **closed** set of built-in semantic variants — the compiler-checked common path.

```fsharp
[<RequireQualifiedAccess>]
type StyleVariant =
    | Primary     // accent-derived primary action
    | Danger      // danger-derived destructive action
    | Ghost       // low-emphasis / transparent-fill
    | Neutral     // default-surface neutral (explicit "no intent")
    | Success     // success/confirmation intent
    | Warning     // caution intent
```

- **Closed**: no `Custom` arm here — free-form lives one level up in `StyleClass`. Closure is
  what guarantees the resolver's variant layer is a total `match` (FR-002, FR-004).
- **Exact set** is fixed here; SC-001 verifies 100% of this set. (Success/Warning are included
  because R7 maps them to tokens; if a value has no existing token it is added to the DTCG
  source per FR-008, never inlined.)
- **Validation rule**: none — it is a closed union; every arm is valid by construction.

## Entity: StyleClass *(new, public — `Types.fsi`)*

One attached-class entry: either a typed variant or a free-form, consumer-defined class. The
list element type carried on a control.

```fsharp
type StyleClass =
    | Variant of StyleVariant   // typed, compiler-checked common path (FR-001)
    | Custom of string          // free-form escape hatch for consumer token-derived styles
```

- **Ordering**: a control carries `StyleClass list`; **list position is attach order** —
  earlier entries are overridden by later ones in resolution (FR-003).
- **`Custom name` semantics**: resolves through the *same* fold as `Variant` (FR-001). A name
  the resolver maps to a token applies that token-derived delta; a name with no mapping resolves
  to the base (identity delta) — deterministic, never an error or silent drop (edge case;
  contrast still governed by `ContrastCheck`, FR-007).
- **Validation rule**: none structurally; `Custom ""` is permitted and resolves to identity.

## Entity: AttrValue case — StyleClassesValue *(new arm on existing public union — `Types.fsi`)*

The attached class list rides the existing `Attr<'msg>` mechanism under the existing
`AttrCategory.Style`, so `Control<'msg>` record shape is unchanged.

```fsharp
and AttrValue<'msg> =
    | TextValue of string
    | BoolValue of bool
    | FloatValue of float
    | StringListValue of string list
    | ValidationValue of ValidationState
    | StyleClassesValue of StyleClass list   // NEW — ordered attached classes (attach order = list order)
```

- **Relationship**: produced by `Attributes.styleClasses`; consumed by the resolver, which reads
  the *last* `styleClasses` attribute on a control (consistent with the codebase's
  `tryLast`/last-writer attribute convention) and folds its list in order.
- **Validation rule**: an absent `styleClasses` attribute ≡ `[]` ≡ the no-class base case (the
  behavior-preserving migration baseline, FR-005 / edge case "no class + Normal").

## Entity: ResolvedStyle *(new, public — `Style.fsi`)*

The per-control output of resolution: the concrete paint/typography the migrated kinds apply.
Flat record so last-writer-wins is per-field and structural equality (the parity proof) is a
plain record comparison.

```fsharp
type ResolvedStyle =
    { Foreground: Color           // text / icon color
      Fill: Color                 // primary fill / background color
      Stroke: Color               // stroke / border color
      StrokeWidth: float          // border thickness
      FontFamily: string option   // None ⇒ host default (mirrors Theme.FontFamily)
      FontSize: float
      FontWeight: int option }    // None ⇒ default weight
```

- **Field provenance**: each field's value is the **last writer** under
  `base < classes (earlier<later) < state`. Geometry is NOT in `ResolvedStyle` — the resolver
  governs paint/typography only (R3); geometry stays computed as today.
- **Default (no-class, base) value**: exactly reproduces the current procedural styling for the
  migrated `(kind, theme)` so parity holds byte-identically (FR-005, SC-003).
- **Validation rule**: total — every field is always populated (the base layer sets all fields;
  later layers only overwrite). No `Option` "unset" states beyond the genuinely-optional
  `FontFamily`/`FontWeight` that mirror `Theme`/`Scene` font semantics.

## Resolution: the fold *(behavior, `Style.fsi`/`Style.fs`)*

```fsharp
module Style =
    /// Pure, total, deterministic. Precedence (FR-003):
    ///   base (the migrated kind's default, supplied by the caller)
    ///     < each class in attach order < visual state.
    /// Last-writer-wins per ResolvedStyle field. No selectors / specificity / cascade.
    /// `theme` carries the active palette (DTCG-generated) and selects the
    /// `DesignTokens.Light`/`Dark` set the class/state deltas read (FR-008); no inline literals.
    val resolve :
        theme: Theme ->
        baseStyle: ResolvedStyle ->
        classes: StyleClass list ->
        state: VisualState ->
        ResolvedStyle
```

- **base** (`baseStyle` parameter): the migrated kind's default styling as a `ResolvedStyle`,
  computed token/theme-derived by the caller (`ControlInternals`) per migrated kind — this is what
  carries the kind-specificity, so `resolve` itself stays kind-agnostic. For the default
  (no-class, Normal) case `resolve theme baseStyle [] Normal = baseStyle` exactly (parity,
  FR-005, SC-003).
- **class layer**: `List.fold` left-to-right; each `StyleClass` maps to a partial
  `ResolvedStyle -> ResolvedStyle` overwrite of the fields it owns. `Variant` arms are an
  exhaustive `match` over the closed `StyleVariant`; `Custom name` maps known names to a delta,
  unknown to identity.
- **state layer**: applied **after** classes; `VisualState -> (ResolvedStyle -> ResolvedStyle)`
  is an exhaustive `match` over all eight cases (Normal/Disabled/Hover/Pressed/Focused/Selected/
  Loading/Validation), where `Validation` maps its `ValidationState` severity to a deterministic
  delta. A state's owned field overrides any class's value for that field (FR-003).
- **token sourcing**: every color/size originates from `Theme` / generated `DesignTokens`
  (DTCG source); the active `DesignTokens.Light`/`Dark` set the variant/state deltas read is
  selected from `theme` (so a variant's accent/danger/success family is reachable without a
  separate token parameter); no inline literals (FR-008).

### Resolution worked example (precedence, FR-003)

| Layer | sets `Fill` | sets `Stroke` | sets `Foreground` |
|-------|-------------|---------------|-------------------|
| base (Button, light) | surface | foreground | foreground |
| `Variant Primary` | **accent** | accent | onAccent |
| `Variant Custom "subtle"` (later) | **accentMuted** | — | — |
| state `Disabled` | **muted** | muted | muted |

Result `Fill = muted` (state wins over both classes); `Stroke = muted` (state over Primary);
`Foreground = muted` (state). Had state been `Normal`, `Fill = accentMuted` (later class wins
over Primary). This is the SC-002 / edge-case "Disabled + danger" composition.

## State transitions

Style resolution itself is **stateless** — a pure function with no transitions. The relevant
state (`VisualState`, animation clock, retained identity) is owned and transitioned **elsewhere**
(`ControlRuntime`, `RetainedRender.StateByIdentity`, features 067/091/092); the resolver only
*reads* the current `VisualState` and is re-invoked per frame by the existing retained path
(FR-006). E3 introduces no new state machine, `Model`, `Msg`, or `Effect`.
