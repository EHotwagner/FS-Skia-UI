# SC-007 — computed order passes `Accessibility.validate`; metadata-only

The computed traversal order for the representative view passes `Accessibility.validate`, and the
order + key semantics derive **solely** from `AccessibilityMetadata` with no parallel hand-rolled
table. The `view` contract is unchanged for keyboard-free consumers.

- evidence-kind=validate-order
- renderer-mode=DeterministicRenderOnly
- status=pass

## Results

- Every focusable control in the representative view (`button` act-0 / `slider` nav-1 / `button`
  act-none) passes `Accessibility.validate` with **zero error diagnostics** — including the
  activation-only `Button` (R1: an activation-only control is valid; traversal is engine-level).
- Each `FocusStop`'s `Role` / `Keyboard` / `FocusOrder` is a projection of the control's own
  `AccessibilityMetadata` — there is no parallel table; the stop IS the metadata.
- The `view : 'model -> Control<'msg>` contract is unchanged: a consumer that adds no keyboard
  interaction renders and behaves identically (FR-009) — the FSI transcript exercises the additive
  `Focus` surface without touching `view`.

## Authoritative tests

- `Feature094FocusTests` / `094 US3 validate-order (SC-007)`
- `Feature094FocusTests` / `094 R1 Accessibility correction (defaultFor / validate)`
