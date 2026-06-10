# SC-006 — purity / totality / determinism property (FsCheck, ≥1000)

`Focus.order` / `Focus.traverse` / `Focus.route` are pure, total, and deterministic over ≥1000
generated combinations, and an unmatched key is a defined no-op that never throws.

- evidence-kind=determinism-property
- renderer-mode=DeterministicRenderOnly
- status=pass
- generated-combinations=≥1000 per property (FsCheck `Config.QuickThrowOnFailure.WithMaxTest 1000`)

## Properties asserted

- **route determinism + totality** (≥1000): `route kb key isTab shift = route kb key isTab shift`
  for every generated `(KeyboardOperation, key, isTab, shift)`; the call never throws and always
  returns one of the four closed `KeyRouting` cases (an unmatched key is `Fallthrough`, never an
  exception).
- **route consumption-wins oracle** (≥1000): `route` equals the FR-007 oracle
  (`ActivationKeys` → `Activate`; else `NavigationKeys` → `Navigate`; else `isTab` → `Traverse`;
  else `Fallthrough`).
- **traverse cyclic + total** (≥1000): for a generated non-empty `TabOrder` of `n` stops, `n`
  successive `Next` from any start returns to the start.
- **traverse determinism** (≥1000): identical `(order, current, move)` → identical `next`.
- **order determinism**: `Focus.order c = Focus.order c` (no clock/randomness) — covered in
  `094 US1 tab order`.

## Authoritative tests

- `Feature094FocusTests` / `094 properties (FsCheck, SC-006)` — all four properties pass at 1000
  generated tests each ("Ok, passed 1000 tests.").
