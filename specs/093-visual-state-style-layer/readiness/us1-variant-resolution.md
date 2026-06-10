# US1 — style a control by intent (SC-001)

A consumer attaches a semantic style class to a control and it renders with the
token-derived paint for that intent, with zero color/theme math.

## Evidence (real, in-repo)

- **Resolver level** — `tests/Controls.Tests/Feature093StyleResolverTests.fs`:
  - each built-in `StyleVariant` resolves to a token-derived `ResolvedStyle`;
  - the six variants are **pairwise distinguishable** under one theme;
  - `Primary.Fill = theme.Accent`, `Danger.Fill = theme.Danger`, and the two
    differ token-appropriately;
  - a free-form `Custom "primary"` resolves **identically** to `Variant Primary`
    (same fold); an unknown `Custom` is an identity delta (no drop/throw).
- **Vertical slice (user-reachable)** — the typed front door:
  `Typed.Button.view { defaults with Classes = [ Variant StyleVariant.Danger ] }`
  lowers to `Attributes.styleClasses`, and the migrated render responds:
  `Feature093ParityTests` asserts a `Danger`-classed Button's resolved paint
  **differs** from the no-class Button (the resolver consumed the class), while a
  no-class Button stays byte-identical to the procedural baseline.

## Result

PASS — `dotnet test --filter "Feature 093"` → 23/23 green. The consumer selects
intent declaratively; the resolver derives the paint from `DesignTokens`/`Theme`.
