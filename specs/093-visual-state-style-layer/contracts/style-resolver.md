# Contract: State→Style Resolver & Style-Class Public Surface

**Package**: `FS.Skia.UI.Controls` · **Tier**: 1 (public surface moves) · **Kind**: UI authoring
contract + internal resolver. Surface baselines (controls-public-surface, per-package,
cross-package) MUST be recaptured.

## C1 — Style-class authoring surface (`Types.fsi`, `Attributes.fsi`)

```fsharp
namespace FS.Skia.UI.Controls

[<RequireQualifiedAccess>]
type StyleVariant =
    | Primary | Danger | Ghost | Neutral | Success | Warning

type StyleClass =
    | Variant of StyleVariant
    | Custom of string

// new arm on the existing public union
and AttrValue<'msg> =
    | ...
    | StyleClassesValue of StyleClass list

module Attributes =
    /// Attach an ordered list of style classes (attach order = list order). Lowers to a
    /// single Style-category attribute carrying `StyleClassesValue`. Absent ≡ `[]` ≡ base.
    val styleClasses : classes: StyleClass list -> Attr<'msg>
```

**Guarantees**
- `styleClasses []` is observationally equal to attaching no class (the behavior-preserving
  base case, FR-005).
- List order is significant and preserved end-to-end — it is the attach order the resolver
  folds (FR-003). Attaching `styleClasses` twice follows the codebase's last-writer attribute
  convention (the last `styleClasses` attribute's list is used).
- `StyleVariant` is closed; consumers get a compile error on a non-existent variant (FR-001).

## C2 — Resolver (`Style.fsi`)

```fsharp
namespace FS.Skia.UI.Controls

type ResolvedStyle =
    { Foreground: Color; Fill: Color; Stroke: Color; StrokeWidth: float
      FontFamily: string option; FontSize: float; FontWeight: int option }

module Style =
    /// Pure, total, deterministic resolution. Precedence (last-writer-wins per field):
    ///   base(theme,kind-default) < classes (earlier < later) < visual state.
    /// No selector matching, no specificity, no cross-control cascade.
    val resolve : theme: Theme -> classes: StyleClass list -> state: VisualState -> ResolvedStyle
```

**Guarantees (testable)**
- **G1 Purity/determinism** — `resolve t cs s = resolve t cs s` for all inputs; no
  clock/randomness/Map-iteration dependence (SC-004, ≥1000 FsCheck inputs).
- **G2 Totality** — defined for every `(Theme, StyleClass list, VisualState)`, including all
  eight `VisualState` cases, every `StyleVariant`, and any `Custom` string (unknown ⇒ identity
  delta, never an exception or silent drop) (FR-002, FR-004).
- **G3 Precedence** — for any field set by both a class and the visual state, the **state**
  value wins; for any field set by two classes, the **later** wins (FR-003). Verified per
  generated combination (SC-002, SC-004).
- **G4 Base fidelity** — `resolve theme [] state` for a migrated kind reproduces the prior
  procedural styling exactly; the resolver-driven render is structurally-`Scene`-equal to the
  captured procedural baseline for every `(kind, theme, state)` (FR-005, SC-003).
- **G5 Distinctness** — two different `StyleVariant`s on one kind under one theme yield
  distinguishably different `ResolvedStyle`s (SC-001); each `VisualState` yields a distinct
  `ResolvedStyle` for a representative kind (SC-002).
- **G6 Token provenance** — every color/size in any `ResolvedStyle` originates from `Theme` /
  generated `DesignTokens`; no inline literal bypasses `DesignTokenDrift` (FR-008). Contrast is
  governed solely by the existing `ContrastCheck` gate; an insufficient-contrast `Custom` class
  still resolves to a concrete value (FR-007, SC-006).

## C3 — Migrated-control render path (`Control.fs` `ControlInternals`)

**Guarantees**
- For the migrated kinds, paint/typography flows through `Style.resolve`; **no per-kind inline
  visual-state color branch remains** for them (SC-003 inspection clause).
- Unmigrated kinds are byte-for-byte unchanged — no render-output delta (SC-007).
- The retained render path (`RetainedRender`, 091/092) inherits the resolver via the shared
  `ControlInternals` factoring; E3 does not alter the 067/091/092 identity scheme (FR-006).

## C4 — Non-goals (permanent, asserted negative)

The surface introduces **no** selector matching, specificity algebra, cross-control cascade,
attached/dependency property, lookless `ControlTemplate`, data binding, or observable. The
`view : 'model -> Control<'msg>` contract is unchanged; a consumer who attaches no class sees no
behavior change (FR-009).
