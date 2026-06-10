# US2 — interactive states render distinctly and survive re-renders (SC-002)

## Evidence (real, in-repo)

`tests/Controls.Tests/Feature093StyleResolverTests.fs`:

- each differentiated `VisualState` resolves to a **distinct** token-derived
  style (the 7 non-`Loading` states are pairwise distinct);
- `Loading` inherits `Normal`'s paint, preserving FR-005 parity (the baseline
  paints them identically, so the resolver keeps that identity);
- **state wins over class** for an overlapping field: `Ghost` + `Focused` →
  `Stroke = theme.Accent` (state), while the Ghost class's non-overlapping
  `Fill = transparent` / `Foreground` are retained;
- a **later class wins** over an earlier one (`[Primary; Danger] → Fill = Danger`);
- `Disabled` + `Danger` compose per the fixed order (state's `Fill = Muted` wins).

Survival across re-renders is in `sc005-retained-identity.md`.

## Result

PASS — each `VisualState` resolves distinctly and the fixed
`base < classes-in-order < state` precedence holds, exercised through the public
`Style.resolve` surface.
