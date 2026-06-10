# US2 — focused control responds to its activation/navigation keys (SC-002)

Through the **real** `routeFocusedKey` adapter path (no hand-seeded identity map, reached via
`InternalsVisibleTo`): a focused `Button` activates on each `ActivationKey` producing exactly the
pointer-equivalent message once (no double-dispatch); a focused `Slider` changes value on its
`NavigationKeys` (ArrowLeft/Right).

- evidence-kind=focused-key-delivery
- renderer-mode=DeterministicRenderOnly
- status=pass
- driven-through=`ControlsElmish.routeFocusedKey` over the live `RetainedRender` structure

## Results

- Focused `Button` + `Enter` → `[Clicked]` (exactly one message — the same a pointer click
  dispatches; no double-dispatch); + `Space` → `[Clicked]`.
- Focused `Slider` + `ArrowRight` → `SliderChanged 0.6` (steps the value up from 0.5);
  `ArrowLeft` → `SliderChanged 0.4` (steps down). One value-change message, mirroring the
  pointer-driven change.
- An unmatched key (`Q`) → no product message and no traversal message (`Fallthrough` total path,
  SC-006); the host then consults `host.MapKey`.
- The classification is `Focus.route` (consumption-wins): `ActivationKeys` → `Activate`,
  `NavigationKeys` → `Navigate`, tested before the Tab test (FR-007).

## Authoritative tests

- `Feature094FocusRoutingTests` / `094 US2 routeFocusedKey activation + navigation (SC-002)`
- `Feature094FocusTests` / `094 US2 key routing (Focus.route, SC-002/FR-007)`
