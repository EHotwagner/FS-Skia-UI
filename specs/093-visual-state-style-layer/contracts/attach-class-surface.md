# Contract: Typed Front-Door Attach-Class Affordance

**Package**: `FS.Skia.UI.Controls` (namespace `FS.Skia.UI.Controls.Typed`) · **Tier**: 1
(typed `Props` deltas) · scoped to the **migrated** controls only.

## C1 — `Props` gains an ordered class list

Each migrated control's typed `Props` record (e.g. `ButtonProps<'msg>` in `Widgets/Buttons.fsi`,
and the chosen rich-family widget's `Props`) gains:

```fsharp
type ButtonProps<'msg> =
    { ...
      Classes: StyleClass list }   // NEW — ordered attached style classes; default []
```

- `defaults` sets `Classes = []` so existing call sites compile and lower unchanged (additive).
- `view` lowers `Classes` to `Attributes.styleClasses Classes` on the produced
  `Control<'msg>`; `Classes = []` lowers to **no** style attribute (identical to today's
  output — the parity baseline case).

**Guarantees**
- **A1 Additive** — a consumer who never sets `Classes` gets byte-identical lowering to the
  pre-feature typed front door (SC-007, FR-009). Verified by the existing typed-parity test for
  the migrated control with `Classes = []`.
- **A2 Order-preserving** — `Classes` list order reaches the resolver as attach order (FR-003).
- **A3 Typed common path** — the closed `StyleVariant` makes `Variant StyleVariant.Primary`
  compiler-checked; `Custom "…"` is the free-form hatch (FR-001).
- **A4 Scope** — only migrated controls gain the field this feature; extending the affordance to
  the remaining catalog is the out-of-scope follow-up (FR-005).

## C2 — Lowering equivalence

For any migrated control `c` and classes `cs`:

```
view { defaults with Classes = cs }   ≡   <legacy builder for c> |> withAttr (Attributes.styleClasses cs)
```

(structural `Control<'msg>` equality — the typed front door is a thin lowering, not a second
styling path; mirrors the existing typed-controls parity contract).
