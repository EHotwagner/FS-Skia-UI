# US3 — focus indicator via the E3 `Focused` visual-state (SC-005)

The focused control is visibly indicated through E3's `Focused` visual-state resolved by the
`Style` resolver — **no parallel procedural per-kind focus-paint branch**. The indicator moves with
focus and is removed from the previously-focused control.

- evidence-kind=focus-indicator
- renderer-mode=DeterministicRenderOnly
- status=pass
- e3-dependency=feature 093 (E3) has LANDED — the `Style` resolver is present and handles
  `VisualState.Focused`; the E3-resolver path is asserted directly (no `[-]` fallback needed).

## Results

- `Style.resolve theme baseStyle [] Focused` ≠ `Style.resolve theme baseStyle [] Normal` — the
  `Focused` visual-state resolves to a distinct style (the indicator is resolver-driven).
- `(Style.resolve theme baseStyle [] Focused).Stroke = theme.Accent` — the focus indicator IS the
  resolver's `Focused` stroke (a token-derived accent), not an inline literal or a procedural branch.
- moves-with-focus: the indicator (accent stroke) appears on exactly the control whose `VisualState`
  is `Focused`; an unfocused (`Normal`) control does not carry it — so moving the `Focused` state
  from one control to another moves the indicator.
- no-new-token: the indicator resolves through E3's existing `Focused` style; no new token-derived
  colour is introduced, so `ContrastCheck` remains the sole contrast authority and is unaffected.

## Authoritative tests

- `Feature094FocusRoutingTests` / `094 US3 focus indicator via E3 resolver (SC-005)`
