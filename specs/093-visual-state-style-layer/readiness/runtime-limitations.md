# Runtime limitations & resolver totality — feature 093

## Resolver totality (no error path)

`Style.resolve` is a **pure, total, deterministic** function of
`(theme, baseStyle, classes, state)`. It has no failure mode of its own:

- The closed `StyleVariant` set is an exhaustive `match` (no partial match).
- A `Custom name` the resolver maps to no known token resolves to the **base**
  (identity delta) — never an exception, never a silent drop (documented edge
  case; covered by the unknown-`Custom` test).
- All eight `VisualState` cases are handled; `Validation` maps its
  `ValidationState` severity deterministically. `Normal`/`Loading` are identity.

Contrast is **deferred to `ContrastCheck`** — the resolver introduces no second
contrast policy. An insufficient-contrast `Custom` class still resolves to a
concrete value; the existing gate is the sole authority that flags it (FR-007).

## Permanent non-goals (asserted negative)

No selector matching, specificity algebra, cross-control cascade,
attached/dependency property, lookless `ControlTemplate`, data binding, or
observable is introduced (FR-009 permanent roadmap non-goals).

## Host/runtime support envelope (unchanged by this feature)

The shipped host targets **.NET 10 desktop** with **Vulkan** via the
**SkiaSharp preview** native stack. **unsupported macOS/mobile/browser** targets
remain out of scope, and there is **no software-renderer fallback**. This
feature is render-only and additive: it opens no new Skia/Vulkan surface and
requires no live window — parity is proven by structural `Scene` /
`ResolvedStyle` equality (deterministic render-only evidence).
