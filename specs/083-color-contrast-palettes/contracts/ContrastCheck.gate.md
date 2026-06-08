# Contract: `ContrastCheck` build gate

The gate the framework runs to enforce that shipped themes keep the contrast
promise their own `contrastRequiredRatio` token makes (FR-007, FR-008).

## Selection (routing)

- Selected for `src/Controls/**` changes (appended to the existing
  `controls-public-surface` rule's gate list) — so any design-token / theme edit
  routes it.
- Selected for `src/Color/**` changes via a new `color-contrast` routing rule.
- Registered in `AgentValidation.knownGates` and the regenerated
  `validation.contract.yml` (FR-011). Generated from `Routing.fs`; never
  hand-edited; `TargetMetadataDrift` enforces currency.

## Input

- The generated Light and Dark theme token values from `src/Controls/DesignTokens.fs`
  (alias-resolved). Each theme's `contrastRequiredRatio` token (text target).
- The explicit, documented `ValidatedPairing list` (foreground token, background
  token, role) — NOT the cartesian product (FR-009).

## Procedure

1. For each theme (light, dark) and each validated pairing:
   1. Resolve foreground + background token names to `Color` values.
   2. If a color carries alpha < 255, composite it over the theme `background`
      token (FR-004).
   3. Measure the WCAG ratio.
   4. Pick the required threshold: `Text` → the theme's `contrastRequiredRatio`;
      `GraphicOrUi` → fixed 3.0; `Decorative` → recorded, not enforced (verdict
      `Exempt`).
   5. Emit a `PairingOutcome` row.
2. Write `readiness/color-contrast-evidence.md` with every row (both themes,
   per-pairing measured vs. required, pass/fail).

## Output / exit

- **PASS** when every enforced pairing meets its threshold in both themes
  (SC-001).
- **FAIL** when any enforced pairing falls below, naming per failing row: both
  token names, both resolved colors, measured ratio, required ratio, theme, and
  role (FR-008). Fail-loud, actionable (Principle VII).

## Regression guarantee (SC-005)

Injecting a sub-threshold `$value` into any validated pairing's token (via the
DTCG source) and regenerating MUST make the gate fail with the pairing, measured
ratio, and required ratio in the message. Restoring an accessible value MUST make
it pass.

## Reference behavior (SC-002)

The underlying contrast computation reproduces WCAG reference values:
black-on-white = 21:1, white-on-white = 1:1, within tolerance 0.01.
