# US2 — an unfilled control is byte-identical to today (SC-002 / SC-007)

**Authoritative test:** `Feature095SlotCompositionTests` → `095 US2 unfilled byte-identity (SC-002 / SC-007)`.
**Parity baselines:** [`parity/button.light.normal.scene.txt`](./parity/button.light.normal.scene.txt),
[`parity/button.dark.normal.scene.txt`](./parity/button.dark.normal.scene.txt) (frozen pre-slot oracle,
captured by the `095 evidence capture` test).
**Renderer mode:** DeterministicRenderOnly ([[fs-skia-evidence-mode]]).
**Failure class:** product-defect (a shifted default render is a parity defect).

## Result: PASS

- An **unfilled** Button carries **no** slot attribute and **no** peripheral children — the
  peripheral default regions contribute **zero geometry**, so the label position is invariant.
- The unfilled Button's render is structurally-`Scene`-equal to the **frozen pre-slot oracle**
  (`frozenButtonGeom`) for both `light` and `dark` themes — byte-identical to its pre-slot render
  (SC-002).
- An **unfilled** Panel lowers identically (`sprintf "%A"`) to the legacy no-slot Panel — additive.
- A **non-slotted** kind (`CheckBox`) gains no slot attribute and no slot children — exposing slots
  is scoped to the representative `Button` + `Panel`; other kinds are unchanged (SC-007).

Byte-identity is by construction: with all slot fields `None`, the typed view adds no slot attribute
and `ControlInternals.lowerSlots` is a no-op (its `[] -> control` fast path).
